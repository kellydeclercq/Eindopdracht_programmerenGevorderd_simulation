using System.Text.Json.Serialization;

namespace SimulatorBL.JsonMappers
{
    public class Names
    {
        [JsonPropertyName("first_name_male")]
        public List<string> MaleFirstNames { get; set; }

        [JsonPropertyName("first_name_female")]
        public List<string> FemaleFirstNames { get; set; }

        [JsonPropertyName("last_name")]
        public List<string> LastNames { get; set; }
    }
}