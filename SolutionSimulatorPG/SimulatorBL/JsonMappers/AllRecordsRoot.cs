using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimulatorBL.JsonMappers
{
    public class AllRecordsRoot
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("address")]
        public AllRecordsAddresses Addresses { get; set; }

        [JsonPropertyName("name")]
        public AllRecordsNames Names { get; set; }
    }
}
