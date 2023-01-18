namespace DmAggregator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OvocServiceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SectionName = "OvocService";

        /// <summary>
        /// Ovoc base URL
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// OVOC auth username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// OVOC auth password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Danger! Disable OVOC cert check
        /// </summary>
        public bool DangerDisableOvocCertCheck { get; set; }

        /// <summary>
        /// HTTP client timeout in seconds
        /// </summary>
        public int TimeoutSec { get; set; } = 5;

        /// <summary>
        /// If true then a fake OVOC service will be used for testing only.
        /// </summary>
        public bool FakeOvocService { get; set; }
    }
}
