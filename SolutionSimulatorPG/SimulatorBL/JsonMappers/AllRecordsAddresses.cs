using System.Text.Json.Serialization;

namespace SimulatorBL.JsonMappers
{
    public class AllRecordsAddresses
    {
        [JsonPropertyName("city_name")]
        public List<string> CityNames { get; set; }

        [JsonPropertyName("street")]
        public List<string> Streets { get; set; }
    }
}