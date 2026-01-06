using System.Text.Json.Serialization;

namespace SimulatorBL.JsonMappers
{
    public class AllRecordsNames
    {
        [JsonPropertyName("male_first_name")]
        public List<string> MaleFirstNames { get; set; }

        [JsonPropertyName("female_first_name")]
        public List<string> FemaleFirstNames { get; set; }

        [JsonPropertyName("male_last_name")]
        public List<string> MaleLastNames { get; set; }

        [JsonPropertyName("female_last_name")]
        public List<string> FemaleLastNames { get; set; }
    }
}