namespace SimulatorBL.Configuration
{
    public class SettingsConfig
    {
        //class necessary for (de)serialization of file
        public List<string> SkippedHighwayTypes { get; set; }
        public List<string> CountriesWithAllRecords { get; set; }
        public List<string> CountriesWithOnlyNameRecords { get; set; }
    }
}
