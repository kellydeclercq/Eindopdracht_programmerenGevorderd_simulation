using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.Exceptions;

namespace SimulatorBL.DTO
{
    public class SimulationInformation
    {
        private string _clientName;
        private int _minAge;
        private int _maxAge;
        private int _randomSeed;
        private int _amountOfCust;
        private int _maxHouseNr;
        private int _houseNumberLetterPercentage;
        private DateTime _creationDate;
        private string _country;
        private int _averageAgeNow;
        private int _averageAgeOriginal;
        private int _id;
        private int _year;
        private readonly string higherThenZero = " has to be higher then 0";

        public int Id
        { 
            get => _id;
            set
            {
                if (value < 0) throw new SimulationException($"{nameof(Id)} can't be lower then 0");             
                _id = value;
            }
        }
        public string ClientName 
        { get => _clientName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) _clientName = value;
                else throw new SimulationException(nameof(ClientName) + " can't be null or whitespace");
            }
        }
        public int MinAge
        { 
            get => _minAge;
            set
            {
                if (value > 0 ) _minAge = value;
                else throw new SimulationException(nameof(MinAge)+ higherThenZero );
            }
        }
        public int MaxAge 
        { 
            get => _maxAge;
            set
            {
                if (value > 0) _maxAge = value;
                else throw new SimulationException(nameof(MaxAge) + higherThenZero);
            }
        }

        public int RandomSeed 
        { 
            get => _randomSeed;
            set
            {
                if (value > 0) _randomSeed = value;
                else throw new SimulationException(nameof(RandomSeed) + higherThenZero);
            }
        }
        public int AmountOfCust 
        { 
            get => _amountOfCust;
            set
            {
                if (value > 0) _amountOfCust = value;
                else throw new SimulationException(nameof(AmountOfCust) + higherThenZero);
            }
        }

        public int MaxHouseNr
        { get => _maxHouseNr;
            set
            {
                if (value > 0) _maxHouseNr = value;
                else throw new SimulationException(nameof(MaxHouseNr) + higherThenZero);
            }
        }
        public int HouseNumberLetterPercentage
        { 
            get => _houseNumberLetterPercentage;
            set
            {
                if(value > 0 && value <= 100) _houseNumberLetterPercentage = value;
                else throw new SimulationException(nameof(HouseNumberLetterPercentage) + higherThenZero);
            }
        }
        public DateTime CreationDate 
        {
            get => _creationDate;
            set
            {
                if (value <= DateTime.Now) _creationDate = value;
                else throw new SimulationException(nameof(CreationDate) + " can't be in the future.");
            }
        }

        public int AverageAgeOriginal
        {
            get { return _averageAgeOriginal; }
            set 
            {
                if (value <= 0) throw new SimulationException($"{nameof(AverageAgeOriginal)} {higherThenZero}");
                _averageAgeOriginal = value; 
            }

        }

        public int AverageAgeNow
        {
            get
            {
                TimeSpan timePassed = DateTime.Now - CreationDate;      
                double yearsAdded = timePassed.TotalDays / 365.25;      
                return (int)Math.Round(AverageAgeOriginal + yearsAdded);
            }
        }


        public Dictionary<string, int> MunicipalityPerc { get; set; }
        public Dictionary<string, int> StreetsPerMunicipality { get; set; } // key is municipality, value amount of streets
        public Dictionary<string, int> ClientsPerMunicipality { get; set; } // key is municipality, value amount of clients
        public string Country 
        { get => _country;
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) _country = value;
                else throw new SimulationException(nameof(Country) + " can't be null or whitespace");
            }
        }

        public int Year 
        { 
            get => _year;
            set
            {
                if (value > DateTime.Now.Year) throw new SimulationException($"{nameof(Year)}: can't be situated in the future");
                _year = value;
            }
        } 
        

        public SimulationInformation(int id, string clientName, int minAge, int maxAge, int randomSeed, int amountOfCust, int maxHouseNr, 
            int houseNumberLetterPercentage, DateTime creationDate, int averageAgeOriginal, Dictionary<string, int> municipalityPerc, 
            Dictionary<string, int> streetsPerMunicipality, Dictionary<string, int> clientsPerMunicipality, string country, int year)
        {
            Id = id;
            ClientName = clientName;
            MinAge = minAge;
            MaxAge = maxAge;
            RandomSeed = randomSeed;
            AmountOfCust = amountOfCust;
            MaxHouseNr = maxHouseNr;
            HouseNumberLetterPercentage = houseNumberLetterPercentage;
            CreationDate = creationDate;
            AverageAgeOriginal = averageAgeOriginal;
            MunicipalityPerc = municipalityPerc;
            StreetsPerMunicipality = streetsPerMunicipality;
            ClientsPerMunicipality = clientsPerMunicipality;
            Country = country;
            Year = year;
        }
      
    }
}
