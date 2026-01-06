using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SimulatorBL.Configuration;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Enum;
using SimulatorBL.Interfaces;

namespace SimulatorDL
{
    public class DataRequestRepo : IDataRequestRepo
    {

        private readonly string _connection;
        private SqlConnection sqlConn; 

        public DataRequestRepo()
        {
            _connection = AppConfig.Current.ConnectionStrings.ClientSimulatorDb;
        }

        public List<Record> GetSpecificRecords(int year, string country, List<string> selectedMunicipalities)
        {
            List<Record> records = new List<Record>();
           
            sqlConn = new SqlConnection(_connection);
            sqlConn.Open();

            int countryVersion = GetCountryVersion(year, country);
  
                var fnList = GetNames(countryVersion, country, year, "firstNameRecord");
                var lnList = GetNames(countryVersion, country, year, "lastNameRecord");
                var addressList = GetAddresses(countryVersion, country, year, selectedMunicipalities);

            records.AddRange(fnList);
            records.AddRange(lnList);
            records.AddRange(addressList);
            sqlConn.Close();
            return records;
        }
       
        public int GetCountryVersion(int year, string country) 
        {
            string sql = "select cv.Id from CountryVersion cv JOIN Country c on c.Id = cv.fk_countryId " +
                "WHERE cv.uploadYear = @year and c.name = @country;";

            if (sqlConn.State == ConnectionState.Closed) sqlConn.Open();

            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("country", country);

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return (int)result;
            }
            else
            {
                throw new Exception($"CountryVersion not found for {country} and {year}");
            }
        }

        private List<Record> GetAddresses(int countryVersion, string country, int year, List<string> selMunic)
        {
            List<Record> records = new();          
            string inClause = "";

            string sql = " SELECT a.Id, m.Municipality_name as name, a.Street as street FROM AddressRecord a " +
                "JOIN MunicipalityRecord m ON m.Id = a.fk_municipalityId " +
                "WHERE a.fk_countryVersionId = @countryVersion ";        

            if (selMunic != null && selMunic.Count > 0)
            {
                var paramNames = new List<string>();
                for (int i = 0; i < selMunic.Count; i++)
                {
                    paramNames.Add("@p" + i);
                }
                inClause = string.Join(", ", paramNames);
                sql += $" AND m.Municipality_name IN ({inClause})";
            }

            SqlCommand cmd = sqlConn.CreateCommand();

            cmd.CommandText = sql;
            cmd.Parameters.Add("@countryVersion", SqlDbType.Int).Value = countryVersion;
            for (int i = 0; i < selMunic.Count; i++)
            {              
                cmd.Parameters.Add("@p" + i, SqlDbType.NVarChar).Value = selMunic[i];
            }

            if (sqlConn.State == ConnectionState.Closed) sqlConn.Open();

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {                  
                    Record record = new AddressRecord((int)rd["Id"], (string)rd["name"], (string)rd["street"], country, year);
                    records.Add(record);
                }
            }

            //check if no link with addresses (Czech): records will be 0

            if (records.Count <= 0)
            {
                string sqlMunic = "SELECT distinct m.Id, m.Municipality_name as name FROM MunicipalityRecord m " +
                $"WHERE m.fk_countryVersionId = @countryVersion";
                if (selMunic != null && selMunic.Count > 0) sqlMunic += $" AND m.Municipality_name IN ({inClause})";
                
                string sqlStreet = "select distinct a.Id, a.street from AddressRecord a where a.fk_countryVersionId = @countryVersion";

                SqlCommand cmdMunic = sqlConn.CreateCommand();
                SqlCommand cmdStreet = sqlConn.CreateCommand();

                cmdMunic.CommandText = sqlMunic;
                cmdStreet.CommandText = sqlStreet;

                for (int i = 0; i < selMunic.Count; i++)
                {
                    cmdMunic.Parameters.Add("@p" + i, SqlDbType.NVarChar).Value = selMunic[i];
                }

                cmdMunic.Parameters.AddWithValue("@countryVersion", countryVersion);
                cmdStreet.Parameters.AddWithValue("@countryVersion", countryVersion);

                using (SqlDataReader rdMunic = cmdMunic.ExecuteReader())
                {
                    while (rdMunic.Read())
                    {
                        Record record = new AddressRecord((int)rdMunic["Id"], (string)rdMunic["name"], null, country, year);
                        records.Add(record);
                    }
                }

                using (SqlDataReader rdStreet = cmdStreet.ExecuteReader())
                { 

                    while (rdStreet.Read())
                    {                     
                        Record record = new AddressRecord((int)rdStreet["Id"], null, (string)rdStreet["street"], country, year);
                        records.Add(record);
                    }
                }

               


            }

            return records;
        }

        private List<Record> GetNames(int countryVersion, string country, int year, string table)
        {
            List<Record> records = new();

            NameType type = (table.Contains("firstName"))? NameType.Firstname : NameType.Lastname;
            string sql = $"select * from {table} where fk_countryVersionId = @countryVersion";

            SqlCommand cmd = sqlConn.CreateCommand();
            
                cmd.CommandText = sql;
                cmd.Parameters.Add("@countryVersion", SqlDbType.Int).Value = countryVersion;

           

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    string name = (string)rd["Name"];
                    int frequency = (int)rd["Frequency"];
                    if (!Enum.TryParse<Gender>((string)rd["Gender"], true, out Gender gender)) throw new Exception($"gender not found as enum");
                    
                    Record record = new NameRecord((int)rd["Id"], name, type, frequency, gender, country, year);
                    records.Add(record);
                }
            }
           
            return records;
        }

        public List<string> GetAllCountries()
        {
            List<string> countries = new();
            string sql = "select name from Country;";

            using (SqlConnection conn = new SqlConnection(_connection))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sql;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countries.Add(((string)reader["name"]).Trim().ToLower());
                    }

                }              
            } 
            return countries;
        }

        public List<string> GetAllMunicipalities(string country)
        {
            List<string> municipalities = new();
            string sql = "select distinct m.Municipality_name from MunicipalityRecord m " +
                "join CountryVersion v on v.id = m.fk_countryversionId " +
                "join Country ct on ct.id = v.fk_countryId " +
                "where ct.name = @country;";

            using (SqlConnection conn = new SqlConnection(_connection))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@country", country);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        municipalities.Add(((string)reader["Municipality_name"]).Trim());
                    }
                }
            }
            return municipalities;
        }

        public List<MetaDataImportDTO> GetAllDatasets()
        {
            List<MetaDataImportDTO> data = new();
            string sql = "select im.Id, im.importDate, im.sourceFile, im.IsNamesData, im.IsAddressData, c.name country, cv.uploadYear from ImportMetadata im " +
                " join CountryVersion cv on im.fk_countryVersionId = cv.Id join Country c on c.Id = cv.fk_countryId;";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connection))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sql;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            MetaDataImportDTO dataDTO = new MetaDataImportDTO((int)rd["Id"], ((DateTime)rd["importDate"]).Date, (string)rd["sourceFile"],
                                (int)rd["IsNamesData"] == 0 ? false : true,
                                (int)rd["IsAddressData"] == 0 ? false : true,
                                ((string)rd["country"]).Trim(), (int)rd["uploadYear"]);
                            data.Add(dataDTO);
                        }

                    }
                }
            }
            catch (Exception ex) { return null; } //if something is wrong nothing will be returned
            return data;
        }

        public List<SimulationInformation> GetAllSimulations()
        {
            try
            {
                List<SimulationInformation> simulationInformation = new();               

                using (SqlConnection conn = new SqlConnection(_connection))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();              

                    // opvragen client per municpality 
                    Dictionary<int, Dictionary<string, int>> clientsPerMunic = GetClientsPerMunic(conn);      //key is simconfig id, value is combination municipality and amount of clients
                    // streets per municipality 
                    Dictionary<int, Dictionary<string, int>> streetsPerMunic = GetStreetsPerMunic(conn);         // key is simconfig, values is combination of municipality and amount of streetsPerMunic               
                    // dictionary munic
                    Dictionary<int, Dictionary<string, int>> selectedMunic = GetSelectedMunicipalities(conn);

                    Dictionary<int, Dictionary<string, Dictionary<string, int>>> nameOccurences = GetNameOccurences(conn);
                    // simconfig 
                    simulationInformation = GetSimulationInformation(clientsPerMunic, streetsPerMunic, selectedMunic, nameOccurences, conn);
                }


                return simulationInformation;
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); return null; }
        }

        private Dictionary<int, Dictionary<string, Dictionary<string, int>>> GetNameOccurences(SqlConnection conn)
        {
            Dictionary<int, Dictionary<string, Dictionary<string, int>>> data = new();
            string sql = @"SELECT 
                sd.fk_simConfig AS SimId, 
                'lastNames' AS Category, 
                c.lastname AS Name, 
                COUNT(*) AS Amount
            FROM customer c
            INNER JOIN SimulationDataset sd ON c.fk_simDataset = sd.Id
            GROUP BY sd.fk_simConfig, c.lastname

            UNION ALL

            SELECT 
                sd.fk_simConfig AS SimId, 
                'femaleName' AS Category, 
                c.firstname AS Name, 
                COUNT(*) AS Amount
            FROM customer c
            INNER JOIN SimulationDataset sd ON c.fk_simDataset = sd.Id
            WHERE c.gender = 'Female' 
            GROUP BY sd.fk_simConfig, c.firstname

            UNION ALL

            SELECT 
                sd.fk_simConfig AS SimId, 
                'maleName' AS Category, 
                c.firstname AS Name, 
                COUNT(*) AS Amount
            FROM customer c
            INNER JOIN SimulationDataset sd ON c.fk_simDataset = sd.Id
            WHERE c.gender = 'Male'
            GROUP BY sd.fk_simConfig, c.firstname;";

            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    int simId = (int)rd["SimId"];
                    string category = (string)rd["Category"];
                    string name = (string)rd["Name"];
                    int amount = (int)rd["Amount"];

                    if (!data.ContainsKey(simId)) data.Add(simId, new Dictionary<string, Dictionary<string, int>>());
                    if (!data[simId].ContainsKey(category)) data[simId].Add(category, new Dictionary<string, int>());
                    data[simId][category].Add(name, amount);
                }
            }

           return data;
        }

        private Dictionary<int, Dictionary<string,int>> GetSelectedMunicipalities(SqlConnection conn)
        {
            try
            {
                Dictionary<int, Dictionary<string,int>> data = new();
                string sql = "select mr.Municipality_name, sm.fk_simConfig, sm.percentage from selectedMunicipality sm " +
                    " join MunicipalityRecord mr on mr.id = sm.fk_Municipality " +
                    " order by fk_simConfig";
               
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int id = (int)rd["fk_simConfig"];
                        string name = (string)rd["Municipality_name"];
                        int percentage = rd["percentage"] == DBNull.Value ? 100 : (int)rd["percentage"]; // if it's zero set to 100 default

                        if (!data.ContainsKey(id)) data.Add(id, new Dictionary<string, int>());
                        data[id].Add(name, percentage);
                    }
                }
                return data;
            }
            catch (Exception ex) { return null; }           
        }

        private List<SimulationInformation> GetSimulationInformation(Dictionary<int, Dictionary<string, int>> clientsPerMunic, 
            Dictionary<int, Dictionary<string, int>> streetsPerMunic, Dictionary<int, Dictionary<string, int>> selectedMunic, Dictionary<int, 
                Dictionary<string, Dictionary<string, int>>> nameOccurences, SqlConnection conn)
        {
            try
            {
                List<SimulationInformation> data = new List<SimulationInformation>();
                string sql = "select sc.Id, sc.clientName, sc.minAge, sc.maxAge, sd.averageAge, sc.randomSeed, sc.numberOfCustomers, sc.houseNumberMax, " +
                    "sc.houseNumberLetterPercent, sd.creationDate, c.name, cv.uploadYear from SimulationConfiguration sc " +
                    " join SimulationDataset sd on sd.fk_simConfig = sc.Id " +
                    " join CountryVersion cv on cv.Id = sc.fk_countryVersion " +
                    " join Country c on c.Id =  cv.fk_countryId;";

                
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int id = (int)rd["Id"];
                        string clientName = (string)rd["clientName"];
                        int minAge = (int)rd["minAge"];
                        int maxage = (int)rd["maxAge"];
                        int averageAge = (rd["averageAge"] == DBNull.Value) ? 0 : (int)rd["averageAge"];
                        int randomSeed = (int)rd["randomSeed"];
                        int numberOfCust = (int)rd["numberOfCustomers"];
                        int houseNumerMax = (int)rd["houseNumberMax"];
                        int houseNumberLetPerc = (int)rd["houseNumberLetterPercent"];
                        DateTime creationDate = (DateTime)rd["creationDate"];
                        string country = (string)rd["name"];
                        var selecMunic = selectedMunic.ContainsKey(id)? selectedMunic[id] : null;
                        int year = (int)rd["uploadYear"];

                        data.Add(new SimulationInformation(id, clientName, minAge, maxage, randomSeed, numberOfCust, houseNumerMax, houseNumberLetPerc
                         , creationDate, averageAge, selecMunic , streetsPerMunic[id], clientsPerMunic[id], nameOccurences[id], country, year));
                    }
                }
                return data;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return null;  }
        }

        private Dictionary<int, Dictionary<string, int>> GetStreetsPerMunic(SqlConnection conn)
        {
            try
            {
                Dictionary<int, Dictionary<string, int>> data = new();
                string sql = "select sd.fk_simConfig simId, c.Municipality , count(c.id) streets from customer c" +
                    " join SimulationDataset sd on sd.id = c.fk_simDataset " +
                    " group by c.Municipality,  sd.fk_simConfig";

                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int simId = (int)rd["simId"];
                        string municipality = (string)rd["Municipality"];
                        int amountOfStreets = (int)rd["streets"];

                        if (!data.ContainsKey(simId)) data.Add(simId, new Dictionary<string, int>());
                        data[simId].Add(municipality, amountOfStreets);
                    }
                }
                return data;
            } catch(Exception ex) { Console.WriteLine(ex.Message); return null; }

        }

        private Dictionary<int, Dictionary<string, int>> GetClientsPerMunic(SqlConnection conn)
        {
            Dictionary<int, Dictionary<string, int>> data = new();
            string sql = "SELECT sd.fk_simConfig AS SimulationId, c.Municipality, COUNT(c.Id) AS amount_of_clients FROM customer c" +
                " JOIN SimulationDataset sd ON c.fk_simDataset = sd.Id" +
                " GROUP BY sd.fk_simConfig,c.Municipality";
            

            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    int simId = (int)rd["SimulationId"];
                    string municipality = (string)rd["Municipality"];
                    int amountOfClients = (int)rd["amount_of_clients"];

                    if (!data.ContainsKey(simId)) data.Add(simId, new Dictionary<string, int>());
                    data[simId].Add(municipality, amountOfClients);
                }
            }

            return data;
        }

        public List<Customer> GetSpecificCustomers(int id)
        {
            string sql = "select c.*, ct.name country from customer c" +
                " join SimulationDataset sd on sd.Id = c.fk_simDataset" +
                " join SimulationConfiguration sc on sc.Id = sd.fk_simConfig" +
                " join CountryVersion cv on cv.Id = sc.fk_countryVersion " +
                " join Country ct on ct.Id = cv.fk_countryId where sc.Id = @id ";
            List<Customer> data = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(_connection))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        data.Add(new Customer((string)rd["firstname"], (string)rd["lastname"], Enum.Parse<Gender>((string)rd["gender"]),
                            (string)rd["Street"], (string)rd["Municipality"], (string)rd["country"], (string)rd["houseNumber"], (DateTime)rd["Birthdate"]));

                    }
                }
            }

            return data;
        }
    }
    
}
