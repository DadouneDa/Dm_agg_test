using DmAggregator.Aggregation;
using DmAggregator.ApplicationInsights;
using DmAggregator.Auth;
using DmAggregator.Controllers;
using DmAggregator.Hashing;
using DmAggregator.HostedServices;
using DmAggregator.Models;
using DmAggregator.Models.Validators;
using DmAggregator.Services.Ovoc;
using DmAggregator.Services.Redis;
using DmAggregator.Utils;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Log to Azure
builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
});

// Add services to the container.
builder.Services.AddControllers(o =>
{
    var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    o.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(lifetime: ServiceLifetime.Singleton);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "DmAggregator",
            Version = $"v{assemblyVersion}",
        }
     );

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.ExampleFilters();

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = ApiKeyAuthSchemeOptions.ApiKeyHeaderName,
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
    c.AddSecurityRequirement(requirement);
});

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

builder.Services.FluentValidateAndAddSingleton<AggregatorConfig>(builder.Configuration.GetSection(AggregatorConfig.SectionName));

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<Aggregator>();
var ovocServiceConfig = builder.Services.FluentValidateAndAddSingleton<OvocServiceConfig>(
    builder.Configuration.GetSection(OvocServiceConfig.SectionName));

var shortQConfig = builder.Configuration.GetSection(BackgroundSenderQueueConfig.ShortQConfigSectionName)
    .FluentValidate<BackgroundSenderQueueConfig>();

var fullQConfig = builder.Configuration.GetSection(BackgroundSenderQueueConfig.FullQConfigectionName)
    .FluentValidate<BackgroundSenderQueueConfig>();

var bgSenderConfig = builder.Services.FluentValidateAndAddSingleton<BackgroundMessageSenderConfig>(
    builder.Configuration.GetSection(BackgroundMessageSenderConfig.SectionName));

var getCustomersConfig = builder.Services.FluentValidateAndAddSingleton<BackgroudOvocPollingConfig>(
    builder.Configuration.GetSection(BackgroudOvocPollingConfig.SectionName));

builder.Services.AddSingleton<OvocCache>();

DmActionsConfig dmActionsConfig = builder.Services.FluentValidateAndAddSingleton<DmActionsConfig>(
    builder.Configuration.GetSection(DmActionsConfig.SectionName));

builder.Services.AddSingleton<IBackgroundSenderQueue<string>, BackgroundSenderQueue<string>>(
    sp => ActivatorUtilities.CreateInstance<BackgroundSenderQueue<string>>(sp, shortQConfig));
builder.Services.AddSingleton<IBackgroundSenderQueue<IPPKeepAliveRequest>, BackgroundSenderQueue<IPPKeepAliveRequest>>(
        sp => ActivatorUtilities.CreateInstance<BackgroundSenderQueue<IPPKeepAliveRequest>>(sp, fullQConfig));

string? ovocAuthHeaderVal = (!string.IsNullOrEmpty(ovocServiceConfig.Username) && !string.IsNullOrEmpty(ovocServiceConfig.Password)) ?
    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ovocServiceConfig.Username}:{ovocServiceConfig.Password}")) : null;

if (ovocServiceConfig.FakeOvocService)
{
    builder.Services.AddSingleton<IOvocService, FakeOvocService>();
}
else
{
    builder.Services.AddHttpClient<IOvocService, OvocService>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Change from default two minutes
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                ovocServiceConfig.DangerDisableOvocCertCheck ? (req, cert, chain, errors) => true : null,
        })
        .ConfigureHttpClient((sp, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(ovocServiceConfig.BaseUrl!);
            httpClient.Timeout = TimeSpan.FromSeconds(ovocServiceConfig.TimeoutSec);
            if (ovocAuthHeaderVal != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", ovocAuthHeaderVal);
            }
        });
}


builder.Services.AddHostedService<BackgroundMessageSender>();
builder.Services.AddHostedService<BackgroundOvocPolling>();

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.ConfigureTelemetryModule<EventCounterCollectionModule>(
    (module, o) =>
    {
        module.Counters.Clear();
        foreach (var counterName in AggregatorEventCountersSource.Log.GetCounterNames())
        {
            module.Counters.Add(new EventCounterCollectionRequest(AggregatorEventCountersSource.EventSourceName, counterName));
        }
    });

var authOptions = new ApiKeyAuthSchemeOptions();
builder.Configuration.GetSection(ApiKeyAuthSchemeOptions.SectionName).Bind(authOptions);

if (string.IsNullOrEmpty(authOptions.ApiKey) || (authOptions.ApiKey.StartsWith('<') && authOptions.ApiKey.EndsWith('>')))
{
    throw new ArgumentException($"Set '{nameof(authOptions.ApiKey)}' in a safe location");
}

builder.Services.AddAuthentication(ApiKeyAuthSchemeOptions.AuthSchemeName)
    .AddApiKeyAuthentication(o =>
    {
        o.HeaderName = authOptions.HeaderName;
        o.ApiKey = authOptions.ApiKey;
    });

var app = builder.Build();

#if false
// Note, in the past hashing was computed on the IPP KeepAlive body only, and performed by the middleware below.
// However, hashing must now also include non-body parts, such as IP address, customerId.
// Therefore, request body hash is not used anymore.
// Instead, it is computed manually
app.UseRequestBodyHash();
#endif

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// No https in k8s
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize RedisCache here to fail in startup
app.Services.GetRequiredService<IRedisCacheService>();

app.Run();
