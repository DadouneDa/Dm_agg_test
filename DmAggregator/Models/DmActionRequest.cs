namespace DmAggregator.Models
{
    /// <summary>
    /// IPP action request from device manager
    /// </summary>
    public class DmActionRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>00908f9a9623</example>
        public required string Mac { get; set; }

        /// <summary>
        /// Contains the action as JSON string that should be returned to IPP
        /// </summary>
        /// <example>{\"cmd\": \"reset\"}</example>
        public required string Body { get; set; }

        /// <summary>
        /// Expiration in seconds
        /// </summary>
        /// <example>120</example>
        public int Expiredin { get; set; }

        /// <summary>
        /// Note - this property must NOT be set by device manager. It is set by this application.
        /// </summary>
        public long ExpiredUtcTicks { get; set; }

        /// <summary>
        /// For journal
        /// </summary>
        /// <example>Operator name</example>
        public required string Operator { get; set; }

        /// <summary>
        /// For journal
        /// </summary>
        /// <example>Reset</example>
        public required string Action { get; set; }
    }
}
