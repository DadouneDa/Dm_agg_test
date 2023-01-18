using DmAggregator.ApplicationInsights;
using DmAggregator.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace DmAggregator.Services.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisCacheService : IRedisCacheService
    {
        class DummyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private const string c_DependencyType = "Redis";
        private static IDisposable s_DummyDisposable => new DummyDisposable();
        private readonly bool _trackRedis;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly string _serversStr;
        private readonly IDatabase _db;
        private ConnectionMultiplexer _connection;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="telemetryClient"></param>
        /// <exception cref="ArgumentException"></exception>
        public RedisCacheService(ILogger<RedisCacheService> logger, IConfiguration configuration, TelemetryClient telemetryClient)
        {
            _logger = logger;
            this._telemetryClient = telemetryClient;
            string? connectionString = configuration.GetConnectionString("Redis");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            string? extraRedisConfig = configuration.GetSection("Redis")["extraConfig"];

            if (!string.IsNullOrEmpty(extraRedisConfig))
            {
                connectionString = $"{connectionString},{extraRedisConfig}";
            }

            // This configuration parameter can be enables to track Redis Application Insights Dependencies
            this._trackRedis = configuration.GetSection("Redis").GetValue<bool>("trackDependencies");

            _configurationOptions = ConfigurationOptions.Parse(connectionString);

            _logger.LogWarning($"Trying to connect to Redis using connection string '{connectionString}'");

            // For local development run:
            // docker run --name mycontainer -d -p 6379:6379 redis
            // (if name in use run 'docker start mycontainer')
            // and use connection string "localhost"
            // To run redis-cli: docker exec -it  mycontainer redis-cli
            _connection = ConnectionMultiplexer.Connect(_configurationOptions);

            _logger.LogWarning($"Successfully connected to Redis!");

            var servers = _connection.GetServers();
            foreach (var server in servers)
            {
                _logger.LogWarning($"Connected to server '{server.EndPoint}' version '{server.Version}'");
            }

            this._serversStr = string.Join('|', servers.Select(s => s.EndPoint.ToString()));

            this._db = _connection.GetDatabase();

            this.DbTestAsync().GetAwaiter().GetResult();
        }


        /// <inheritdoc/>
        public async Task<T?> GetObject<T>(string key)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisGetObject, AppInsightsConsts.MacProperty, key)))
            {
                return await this._db.GetObject<T>(key);
            }
        }

        /// <inheritdoc/>
        public async Task<T?> GetObjectSetExpiry<T>(string key, TimeSpan ttl)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisGetObjectSetExpiry,
                AppInsightsConsts.MacProperty, key, AppInsightsConsts.TtlProperty, ttl.TotalSeconds.ToString())))
            {
                return await this._db.GetObjectSetExpiry<T>(key, ttl);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SetObject<T>(string key, T obj, TimeSpan expiry)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisSetObject,
                AppInsightsConsts.MacProperty, key, AppInsightsConsts.TtlProperty, expiry.TotalSeconds.ToString())))
            {
                return await this._db.SetObject<T>(key, obj, expiry);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SetObject<T>(string key, T obj)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisSetObject, AppInsightsConsts.MacProperty, key)))
            {
                return await this._db.SetObject<T>(key, obj);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> KeyDeleteAsync(string key)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisKeyDelete, AppInsightsConsts.MacProperty, key)))
            {
                return await this._db.KeyDeleteAsync(key);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> KeyExpireAsync(string key, TimeSpan ttl)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisKeyExpire,
                AppInsightsConsts.MacProperty, key, AppInsightsConsts.TtlProperty, ttl.TotalSeconds.ToString())))
            {
                return await this._db.KeyExpireAsync(key, ttl);
            }
        }

        /// <inheritdoc/>
        public async Task<TimeSpan?> KeyTimeToLiveAsync(string key)
        {
            using (IDisposable operation = !this._trackRedis ? s_DummyDisposable : this._telemetryClient.StartOperation(
                this.SetDependency(AppInsightsConsts.DepRedisKeyTtl, AppInsightsConsts.MacProperty, key)))
            {
                return await this._db.KeyTimeToLiveAsync(key);
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GetVersion()
        {
            var servers = this._db.Multiplexer.GetServers();
            return servers.ToDictionary(x => x.EndPoint.ToString()!, x => x.Version.ToString());
        }

        private async Task DbTestAsync()
        {

            _logger.LogInformation($"Successfully connected to Redis server");

            var result = await this._db.PingAsync();

            int cnt = 5;
            TimeSpan pingTime = new TimeSpan();
            for (int i = 0; i < cnt; i++)
            {
                pingTime += await this._db.PingAsync();
            }

            _logger.LogInformation($"Average ping time: {pingTime.TotalMilliseconds / cnt}ms");

            string key = Guid.NewGuid().ToString();
            string val = "zz";
            await this._db.StringSetAsync(key, val);
            if (await this._db.StringGetAsync(key) != val)
            {
                throw new InvalidOperationException($"Unexpected test value '{val}' read");
            }
            if (!await this._db.KeyDeleteAsync(key))
            {
                throw new InvalidOperationException($"KeyDeleteAsync fail");
            }
        }

        private DependencyTelemetry SetDependency(string data, string customPropName, string customPropValue)
        {
            DependencyTelemetry dependency = this.InitDependency(data);

            if (!string.IsNullOrEmpty(customPropName))
            {
                dependency.Properties[customPropName] = customPropValue;
            }
            return dependency;
        }

        private DependencyTelemetry SetDependency(string data, 
            string customPropName1, string customPropValue1, string customPropName2, string customPropValue2)
        {
            DependencyTelemetry dependency = this.InitDependency(data);

            if (!string.IsNullOrEmpty(customPropName1))
            {
                dependency.Properties[customPropName1] = customPropValue1;
            }
            if (!string.IsNullOrEmpty(customPropName2))
            {
                dependency.Properties[customPropName2] = customPropValue2;
            }
            return dependency;
        }


        private DependencyTelemetry SetDependency(string data, IDictionary<string, string>? props = null)
        {
            DependencyTelemetry dependency = this.InitDependency(data);

            if (props != null)
            {
                foreach (var item in props)
                {
                    dependency.Properties.Add(item);
                }
            }
            return dependency;
        }

        private DependencyTelemetry InitDependency(string data)
        {
            return new DependencyTelemetry(
                dependencyTypeName: c_DependencyType,
                target: string.Empty,
                dependencyName: this._serversStr,
                data: data);
        }


    }
}
