using System.Runtime;
using SimulatorBL.DTO;
using SimulatorBL.Exceptions;

namespace UnitTestSimulationSolution
{
    public class UnitTestSimulationInformation
    {
        private SimulationInformation CreateValidSimulationInfo()
        {
            return new SimulationInformation(
               1, "Test Klant", 18, 80, 123, 100, 50, 10, DateTime.Now.AddYears(-1), // test with info of one year ago
               30, new Dictionary<string, int> (), new Dictionary<string, int>(), new Dictionary<string, int>(), 
               new Dictionary<string, Dictionary<string, int>>(), "Belgium", 2023
            );
        }

        private SimulationInformation dummyInformation =
             new SimulationInformation(0, "Dummy", 1, 100, 1, 1, 1, 1, DateTime.Now, 20, null, null, null, 
                 new Dictionary<string, Dictionary<string, int>>(), "netherlands", 2023);


        //constructor

        [Fact]
        public void Test_constructor_valid()
        {
            var simInfo = CreateValidSimulationInfo();
            //test with null and random data
            Assert.NotNull(simInfo);
            Assert.Equal("Test Klant", simInfo.ClientName);
            Assert.Equal(18, simInfo.MinAge);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_constructor_id_invalid(int id)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(id, "Dummy", 1, 100, 1, 1, 1, 1, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(),"NL", 2023));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void Test_constructor_clientname_invalid(string name)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, name , 1, 100, 1, 1, 1, 1, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_constructor_minAge_invalid(int minAge)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", minAge, 100, 1, 1, 1, 1, DateTime.Now, 20, null, null, null, 
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_constructor_maxAge_invalid(int maxAge)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, maxAge, 1, 1, 1, 1, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_constructor_randomSeed_invalid(int randomSeed)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, 80, randomSeed, 1, 1, 1, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_constructor_amountOfCust_invalid(int amount)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, 80, 1, amount, 1, 1, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        public void Test_constructor_letterPerc_invalid(int perc)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, 80, 1, 1, 1, perc, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(),"NL", 2023));
        }

        [Fact]
        public void Test_constructor_creationDate_invalid()
        {
            DateTime creationDate = DateTime.Now.AddDays(1);
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, 80, 1, 1, 1, 1, creationDate, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]       
        public void Test_constructor_averageAgeOG_invalid(int age)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, 80, 1, 1, 1, 1, DateTime.Now, age, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), "NL", 2023));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void Test_constructor_country_invalid(string country)
        {
            Assert.Throws<SimulationException>(() => new SimulationInformation(1, "Dummy", 1, 100, 1, 1, 1, 1, DateTime.Now, 20, null, null, null,
                new Dictionary<string, Dictionary<string, int>>(), country, 2023));
        }

        

         //properties

         [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999)]
        public void Test_id_valid(int id)
        {
            dummyInformation.Id = id;
            Assert.Equal(id, dummyInformation.Id);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Test_id_invalid(int id)
        {
           Assert.Throws<SimulationException>(() => dummyInformation.Id = id);
        }

        [Theory]
        [InlineData("K")]
        [InlineData("Bram")]
        public void Test_clientname_valid(string name)
        {
            dummyInformation.ClientName = name;
            Assert.Equal(name, dummyInformation.ClientName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void Test_clientname_invalid(string name)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.ClientName = name);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]      
        [InlineData(999)]
        public void Test_minAge_valid(int minAge)
        {
            dummyInformation.MinAge = minAge;
            Assert.Equal(minAge, dummyInformation.MinAge);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Test_minAge_invalid(int minAge)
        {
           Assert.Throws<SimulationException>(() => dummyInformation.MinAge = minAge);           
        }


        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(999)]
        public void Test_maxAge_valid(int age)
        {
            dummyInformation.MinAge = age;
            Assert.Equal(age, dummyInformation.MinAge);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Test_maxAge_invalid(int randomSeed)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.MinAge = randomSeed);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(999)]
        public void Test_randomSeed_valid(int randomSeed)
        {
            dummyInformation.MinAge = randomSeed;
            Assert.Equal(randomSeed, dummyInformation.MinAge);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Test_randomSeed_invalid(int randomSeed)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.MinAge = randomSeed);
        }

       
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(999)]
        public void Test_amountOfCust_valid(int amount)
        {
            dummyInformation.AmountOfCust = amount;
            Assert.Equal(amount, dummyInformation.AmountOfCust);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Test_amountOfCust_invalid(int amount)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.AmountOfCust = amount);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(99)]
        public void Test_letterPercentage_valid(int letterPercentage)
        {
            dummyInformation.HouseNumberLetterPercentage = letterPercentage;
            Assert.Equal(letterPercentage, dummyInformation.HouseNumberLetterPercentage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        [InlineData(999)]
        public void Test_letterPercentage_invalid(int letterPercentage)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.HouseNumberLetterPercentage = letterPercentage);
        }

        [Fact]    
        public void Test_creationDate_valid()
        {
            DateTime creationDate = DateTime.Now.AddDays(-5);
            dummyInformation.CreationDate = creationDate;
            Assert.Equal(creationDate, dummyInformation.CreationDate);
        }

        [Fact]
        public void Test_creationDate_invalid()
        {
            DateTime creationDate = DateTime.Now.AddDays(1);
            Assert.Throws<SimulationException>(() => dummyInformation.CreationDate = creationDate);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(99)]
        public void Test_averageAgeOriginal_valid(int age)
        {
            dummyInformation.AverageAgeOriginal = age;
            Assert.Equal(age, dummyInformation.AverageAgeOriginal);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_averageAgeOriginal_invalid(int age)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.AverageAgeOriginal = age);
        }

        // average age now

        //TEST 1: no time
        [Fact]
        public void AverageAgeNow_NoTimePassed_EqualsOriginal()
        {
            // Arrange
            dummyInformation.AverageAgeOriginal = 20;
            dummyInformation.CreationDate = DateTime.Now; // Net gemaakt

            // Act
            int result = dummyInformation.AverageAgeNow;

            // Assert
            Assert.Equal(20, result);
        }

        // TEST 2: 10 yearss
        [Fact]
        public void AverageAgeNow_TenYearsPassed_AddsTenYears()
        {
            // Arrange
            dummyInformation.AverageAgeOriginal = 20;
            // We simuleren dat dit 10 jaar geleden gemaakt is
            dummyInformation.CreationDate = DateTime.Now.AddYears(-10);

            // Act
            int result = dummyInformation.AverageAgeNow;

            // Assert
            // 20 + 10 = 30
            Assert.Equal(30, result);
        }

        // TEST 3: testing round function
        [Fact]
        public void AverageAgeNow_SmallTimePassed_RoundsDown()
        {
            dummyInformation.AverageAgeOriginal = 20;
            // 140 days is 0.38 year. Math.Round will go to lowest number
            dummyInformation.CreationDate = DateTime.Now.AddDays(-140);
            int result = dummyInformation.AverageAgeNow;
            // 20 + 0.38 = 20.38 -> Round -> 20
            Assert.Equal(20, result);
        }

        // TEST 4: testing rounding function up
        [Fact]
        public void AverageAgeNow_MoreThanHalfYearPassed_RoundsUp()
        {
            // Arrange
            dummyInformation.AverageAgeOriginal = 20;
            // 250 --> 0.68
            // Math.Round will go up
            dummyInformation.CreationDate = DateTime.Now.AddDays(-250);
            int result = dummyInformation.AverageAgeNow;
            Assert.Equal(21, result);
        }

        [Theory]
        [InlineData("K")]
        [InlineData("Bram")]
        public void Test_country_valid(string country)
        {
            dummyInformation.Country = country;
            Assert.Equal(country, dummyInformation.Country);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void Test_country_invalid(string country)
        {
            Assert.Throws<SimulationException>(() => dummyInformation.Country = country);
        }

        // not in theory because it doesn't accept datetime.now as parameter

        [Fact]
        public void Test_year_valid_past()
        {
            int pastYear = 2000;
            dummyInformation.Year = pastYear;
            Assert.Equal(pastYear, dummyInformation.Year);
        }

        [Fact]
        public void Test_year_valid_currentYear()
        {
            int currentYear = DateTime.Now.Year;
            dummyInformation.Year = currentYear;
            Assert.Equal(currentYear, dummyInformation.Year);
        }

        [Fact]
        public void Test_year_invalid_future()
        {
            int futureYear = DateTime.Now.Year + 1;
            Assert.Throws<SimulationException>(() => dummyInformation.Year = futureYear);
        }


    }       
}
