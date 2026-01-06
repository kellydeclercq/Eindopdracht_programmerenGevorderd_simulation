using System.Collections.ObjectModel;
using System.Net;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Enum;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Manager
{
    public class SimulatorService : ICustomerGenerator
    {
        private IWriterRepo _writer;
        private DataRequestService _requestService;
        private int _mySeed;
        private Random _random;

        public SimulatorService(IWriterRepo writerRepo, DataRequestService requestService, int mySeed)
        {
            _writer = writerRepo;
            _requestService = requestService;
            _mySeed = mySeed;
            _random = new Random(_mySeed);
        }

        public bool StartSimulation(string clientName, int year, string country, int minAge, int maxAge, int numberOfCust,
         Dictionary<string, int> municipalityPerc, int maxHousenr, int percentageLetters, ObservableCollection<SimulationInformation> _simulationInformationDTOs)
        {
            List<Record> records = _requestService.GetSpecificRecords(year, country, municipalityPerc.Keys.ToList()).ToList();

            var fnRecord = records.OfType<NameRecord>().Where(x => x.nameType == NameType.Firstname).ToList();
            var lnRecord = records.OfType<NameRecord>().Where(x => x.nameType == NameType.Lastname).ToList();
            var addressRecord = records.OfType<AddressRecord>().ToList();
            var maleNames = fnRecord.Where(x => x.Gender == Gender.Male).ToList();
            var femaleNames = fnRecord.Where(x => x.Gender == Gender.Female).ToList();

            //NAMES WEIGHT CALCULATIONS: based on binary search
            long[] maleWeights = GetCumulativeWeight(maleNames);
            long[] femaleWeights = GetCumulativeWeight(femaleNames);

            //MUNICIPALITY WEIGHT CALCULATIONS: based on pool of names
            // fill pool of municpality based on exact amount
            List<string> municipalityPool = GenerateMunicipalityPool(numberOfCust, municipalityPerc);

      
            // Street mapping as lookup dictionary: no null variables
            Dictionary<string, List<string>> streetsPerMunicipalityMap = addressRecord
            .Where(x => !string.IsNullOrWhiteSpace(x.Municipality) && !string.IsNullOrWhiteSpace(x.Street))
            .GroupBy(x => x.Municipality)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Street).ToList());

            // in case there are null variables
            List<string> allStreets = addressRecord.Where(x => !string.IsNullOrWhiteSpace(x.Street)).Select(x => x.Street).ToList();
            
            List<Customer> customers = new List<Customer>();
            bool simSucces = true; // stays true unless exception gets thrown

            //SIMULATION
            if (fnRecord.Count > 0 && lnRecord.Count > 0)
            {
                for (int i = 0; i < numberOfCust; i++)
                {
                    try
                    {
                        Gender gender = _random.Next(0, 2) == 0 ? Gender.Female : Gender.Male;
                        string name = (gender == Gender.Female) ? GetRandomName(femaleNames, femaleWeights) : GetRandomName(maleNames, maleWeights);
                        string lastName = lnRecord[_random.Next(lnRecord.Count)].Name;

                        // choose municpality from pool
                        string municipality = municipalityPool[i];
                        string street = "";
                        if (streetsPerMunicipalityMap.ContainsKey(municipality) && streetsPerMunicipalityMap[municipality].Count > 0)
                        {
                            var validStreets = streetsPerMunicipalityMap[municipality];
                            street = validStreets[_random.Next(validStreets.Count)];
                        }
                        else
                        {
                            if (allStreets.Count > 0) street = allStreets[_random.Next(allStreets.Count)];
                        }

                        string number = GetRandomNumber(percentageLetters, maxHousenr);
                        DateTime birthDate = GetBirthdate(minAge, maxAge);

                        customers.Add(new Customer(name, lastName, gender, street, municipality, country, number, birthDate));
                    }
                    catch (Exception ex) { return false; } // if simulation failed, return immediatly
                }
            }
            else { return false; }

            // UPLOAD
            Dictionary<string, int> streetsPerMunic = customers.GroupBy(x => x.Municipality)
                .ToDictionary(x => x.Key, x => x.Select(c => c.Street).Count());

            int averageAge = CalculateAverageAge(customers);
            Dictionary<string, int> clientsPerMunic = customers.GroupBy(x => x.Municipality).ToDictionary(m => m.Key, s => s.Count());
            Dictionary<string, Dictionary<string, int>> nameOccurence = GetCategorizedNameOccurences(customers);

            SimulationInformation sim = new SimulationInformation(0, clientName, minAge, maxAge, _mySeed, numberOfCust, maxHousenr,
                percentageLetters, DateTime.Now, averageAge, municipalityPerc, streetsPerMunic, clientsPerMunic, nameOccurence, country, year);

            bool uploadCustomers = _writer.WriteDatasetToDB(sim, customers);
            if (simSucces && uploadCustomers) _simulationInformationDTOs.Add(sim);

            return simSucces && uploadCustomers; // if simulation and uploading succeeded it will return true
        }

        private Dictionary<string, Dictionary<string, int>> GetCategorizedNameOccurences(List<Customer> customers)
        {
            return new Dictionary<string, Dictionary<string, int>>
            {
 
                {
                    "femaleName",
                    customers
                        .Where(c => c.Gender == Gender.Female)
                        .Select(c => c.Name)
                        .GroupBy(n => n)
                        .ToDictionary(g => g.Key, g => g.Count())
                },

                {
                    "maleName",
                    customers
                        .Where(c => c.Gender == Gender.Male)
                        .Select(c => c.Name)
                        .GroupBy(n => n)
                        .ToDictionary(g => g.Key, g => g.Count())
                },

                {
                    "lastNames",
                    customers
                        .Select(c => c.Lastname)
                        .GroupBy(n => n)
                        .ToDictionary(g => g.Key, g => g.Count())
                }
            };
        }

        private List<string> GenerateMunicipalityPool(int numberOfCust, Dictionary<string, int> municipalityPerc)
        {
            List<string> municipalityPool = new List<string>();
            // calculate how many customers per munic
            foreach (var kvp in municipalityPerc)
            {
                // Formule: (total amount * Percentage) / 100
                int countForThisMunicipality = (numberOfCust * kvp.Value) / 100;

                for (int i = 0; i < countForThisMunicipality; i++)
                {
                    municipalityPool.Add(kvp.Key);
                }
            }

            // If  list is not full due to rounding (33% of 100 = 33, 3 x 33 = 99),
            // we fill in the remain with a random municipality from list 
            while (municipalityPool.Count < numberOfCust)
            {
                var randomKey = municipalityPerc.Keys.ElementAt(_random.Next(municipalityPerc.Count));
                municipalityPool.Add(randomKey);
            }

            // Shuffle the bag 
            // why? customers are not grouped this way
            municipalityPool = municipalityPool.OrderBy(x => _random.Next()).ToList();

            return municipalityPool;
        }

        private int CalculateAverageAge(List<Customer> customers)
        {
            if (customers == null) return 0;

            var today = DateTime.Today;
            double averageAge = customers
              .Where(c => c.BirthDate != default)
              .Average(c =>
              {
                  var age = today.Year - c.BirthDate.Year;
                  if (c.BirthDate.Date > today.AddYears(-age)) age--;
                  return age;
              });

            return (int)Math.Round(averageAge);
        }

        private DateTime GetBirthdate(int minAge, int maxAge)
        {
            int trueMinAge = Math.Min(minAge, maxAge);
            int trueMaxAge = Math.Max(minAge, maxAge);

            DateTime oldest = DateTime.Today.AddYears(-trueMaxAge);
            DateTime youngest = DateTime.Today.AddYears(-trueMinAge);
            TimeSpan timeSpan = youngest - oldest;

            //calc difference
            int randomDaysToAdd = _random.Next(0, timeSpan.Days);
            //add days to oldest date
            return oldest.AddDays(randomDaysToAdd);

        }

        private string GetRandomNumber(int percentageLetters, int maxHousenr)
        {
            string number = _random.Next(1, maxHousenr + 1).ToString();

            // Check if we should add a letter           
            if (_random.Next(0, 100) < percentageLetters)
            {
                //Generate a random letter (a through z)
                //start at a (+ 25 extra options)
                char randomLetter = (char)('a' + _random.Next(0, 26));
                number += randomLetter;
            }
            return number;
        }

        private string GetRandomName(List<NameRecord> names, long[] weight)
        {
            // Check         
            if (names.Count == 1) return names[0].Name;

            long totalWeight = weight[weight.Length - 1];

            // Generates a number between 1 and TotalWeight (inclusive)
            // Not 0, otherwise the first person would have a higher chance than usual
            long target = _random.NextInt64(1, totalWeight + 1);

            // Binary Search (The quick search)
            // We search where target would fit in the _cumulativeWeights array
            int index = Array.BinarySearch(weight, target);

            // Array.BinarySearch returns: (Microsoft page)
            // - A positive number if the exact match is found.
            // - A negative number if the match is not found.
            // - This negative number is the bitwise complement of the index of the next larger element.

            if (index < 0)
            {
                // We use the bitwise complement operator (~) to turn this to the correct index
                index = ~index;
            }

            return names[index].Name;
        }

        private long[] GetCumulativeWeight(List<NameRecord> nameList)
        {
            long[] cumWeight = new long[nameList.Count];
            long currentSum = 0;

            for (int i = 0; i < nameList.Count; i++)
            {

                long freq = nameList[i].Frequency > 0 ? nameList[i].Frequency : 1;

                currentSum += freq;
                cumWeight[i] = currentSum;
            }

            return cumWeight;
        }
    }
}
