using Bogus;
using DmAggregator.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DmAggregator.Controllers
{
    /// <summary>
    /// IPP Generator Controller
    /// </summary>
    [Route("api/generator")]
    [ApiController]
    public class IPPGeneratorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public IPPGeneratorController(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Generate IPP keep alive messages looped back to this server
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("keep-alive")]
        public async Task<ActionResult> Generate(IPPGeneratorRequest req)
        {
            UriBuilder ub = new UriBuilder(this.Request.GetEncodedUrl());

            using (var httpClient = this._httpClientFactory.CreateClient())
            {
                for (int loops = 0; loops < req.NumLoops; loops++)
                {
                    // Note - seed is reset to same value for each loop
                    if (req.Seed.HasValue)
                    {
                        Randomizer.Seed = new Random(req.Seed.Value);
                    }
                    else
                    {
                        Randomizer.Seed = new Random();
                    }

                    for (int i = 0; i < req.NumIPP; i++)
                    {
                        var faker = new Faker();

                        var extensionData = new Dictionary<string, object>
                        {
                            ["sessionId"] = faker.Random.Hexadecimal(length:8, prefix:""),
                            ["emsUserName"] = (i + req.UserIdStart).ToString(),
                            ["emsUserPassword"] = faker.Internet.Password(32),
                            ["ip"] = faker.Internet.IpAddress().ToString(),
                            ["subnet"] = "255.255.255.0",
                            ["vlanId"] = faker.Random.Int(0, 400).ToString(),
                            ["model"] = "420HD",
                            ["fwVersion"] = faker.System.Semver(),
                            ["userAgent"] = $"{faker.Company.CompanyName()} /{faker.System.Semver()}",
                            ["userName"] = (i + req.UserIdStart).ToString(),
                            ["userId"] = faker.Internet.Email(),
                            ["phoneNumber"] = (i + req.UserIdStart).ToString(),
                            ["status"] = "registered",
                            ["sipProxy"] = faker.Internet.IpAddress().ToString(),
                        };

                        var keepAliveFaker = new Faker<IPPKeepAliveRequest>()
                            .RuleFor(k => k.MAC, f => f.Internet.Mac(separator: ""))
                            .RuleFor(k => k.ExtensionData, f => extensionData);

                        var path = HttpRouteConstants.IPPPhoneMgrStatusBase + HttpRouteConstants.KeepAlivePath;

                        path = path
                            .Replace("{customerId}", req.CustomerId ?? faker.Random.Guid().ToString())
                            .Replace("{profileId}", faker.Random.Guid().ToString());

                        var keepAlive = keepAliveFaker.Generate();

                        ub.Path = path;

                        var resp = await httpClient.PostAsJsonAsync(ub.Uri, keepAlive);

                        resp.EnsureSuccessStatusCode();
                    }
                }
            }

            return Ok();
        }
    }
}
