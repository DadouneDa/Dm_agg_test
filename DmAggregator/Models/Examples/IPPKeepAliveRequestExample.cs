using Swashbuckle.AspNetCore.Filters;

namespace DmAggregator.Models.Examples
{
    /// <summary>
    /// Because of ExtensionData Dictionary, a specific example and package Swashbuckle.AspNetCore.Filters is needed
    /// </summary>
    public class IPPKeepAliveRequestExample : IExamplesProvider<IPPKeepAliveRequest>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IPPKeepAliveRequest GetExamples()
        {
            return new IPPKeepAliveRequest
            {
                MAC = "00908f9a9623",

                SessionId = "312bcd36",

                EmsUserPassword = "f09fd922600b78e597de8f91d54af7ef",

                ExtensionData = new Dictionary<string, object>
                {
                    ["emsUserName"] = "160000",
                    ["ip"] = "10.0.32.9",
                    ["subnet"] = "255.255.255.0",
                    ["vLanId"] = "350",
                    ["model"] = "420HD",
                    ["fwVersion"] = "2.2.16.142.46",
                    ["userAgent"] = "Genesys /2.2.16.142.46 (420HDG-Rev2-AC494; 00908F9A9623)",
                    ["userName"] = "160000",
                    ["phoneNumber"] = "160000",
                    ["status"] = "registered",
                    ["sipProxy"] = "65.243.174.141",
                }
            };
        }
    }
}
