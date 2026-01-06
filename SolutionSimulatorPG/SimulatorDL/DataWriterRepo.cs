using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SimulatorBL.Configuration;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Enum;
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;
using static System.Net.Mime.MediaTypeNames;

namespace SimulatorDL
{
    public class DataWriterRepo : IWriterRepo
    {
        private readonly string _connection;
        private int simConfigId = 0;
        private int simDatasetId = 0;
        private int fkMunic = 0;
        private int fkCountrVersion = 0;
        private SqlConnection conn;
        private SqlTransaction transaction;

        public DataWriterRepo()
        {
            _connection = AppConfig.Current.ConnectionStrings.ClientSimulatorDb;
            conn = new SqlConnection(_connection);        
        }

        public bool WriteDatasetToDB(SimulationInformation sim, List<Customer> customers)
        {
           
            if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {         
                transaction = conn.BeginTransaction();
                fkCountrVersion = GetCountryVersionId(sim.Year, sim.Country);
                WriteSimulationConfiguration(sim, conn);
                WriteSimulationDataset(sim);
                WriteCustomer(customers);
                if (sim.MunicipalityPerc.Count > 0) WriteSelectedMunicipalities(sim.MunicipalityPerc);

                transaction.Commit();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                conn.Close();
                return false;
            }
           
        }

        private void WriteSelectedMunicipalities(Dictionary<string, int> municipalityPerc)
        {
            string sql = "insert into selectedMunicipality (fk_Municipality, percentage, fk_simConfig) values (@fk_Municipality, @percentage, @fk_simConfig)";
            Dictionary<string, int> municIDs = GetMunicipalitieIDs(municipalityPerc.Keys.ToList());

            if (conn.State == ConnectionState.Closed) conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                try
                {
                    cmd.Parameters.Add("@fk_Municipality", SqlDbType.Int);
                    cmd.Parameters.Add("@percentage", SqlDbType.Int);
                    cmd.Parameters.Add("@fk_simConfig", SqlDbType.Int);

                    cmd.Parameters["@fk_simConfig"].Value = simConfigId;

                    foreach (string munic in municipalityPerc.Keys)
                    {
                        cmd.Parameters["@fk_Municipality"].Value = municIDs[munic];
                        cmd.Parameters["@percentage"].Value = municipalityPerc[munic]; 
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { throw new Exception(ex.Message); }
            }
                
        }

        private Dictionary<string, int> GetMunicipalitieIDs(List<string> selecMunic)
        {
            Dictionary<string, int> data = new Dictionary<string, int>();
    
            string sql = "SELECT Id, Municipality_name from MunicipalityRecord where fk_countryversionId = @id";
            if (conn.State == ConnectionState.Closed) conn.Open();

            // adding parameters to sql statement
            var paramNames = new List<string>();
            for (int i = 0; i < selecMunic.Count; i++)
            {
                paramNames.Add("@p" + i);
            }
           string inClause = string.Join(", ", paramNames);
           sql += $" and Municipality_name IN ({inClause})";

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                //adding countryversion so we get the right municipalityIds of the right version

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = fkCountrVersion;
                for (int i = 0; i < selecMunic.Count; i++)
                {
                    cmd.Parameters.Add("@p" + i, SqlDbType.NVarChar).Value = selecMunic[i];
                }

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {                      
                        data.Add((string)rd["Municipality_name"], (int)rd["Id"]);
                    }
                }

            }

            return data;

        }

        private void WriteCustomer(List<Customer> customers)
        {           
            string sql = "insert into customer (firstname, lastname, gender, Birthdate, fk_simDataset, Street, Municipality, houseNumber) " +
                "values (@firstname, @lastname, @gender, @Birthdate, @fk_simDataset, @Street, @Municipality, @houseNumber)";

            if (conn.State == ConnectionState.Closed) conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = sql;
                try
                {
                    cmd.Parameters.Add("@firstname", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@lastname", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@gender", SqlDbType.NVarChar);                   
                    cmd.Parameters.Add("@Birthdate", SqlDbType.DateTime);
                    cmd.Parameters.Add("@fk_simDataset", SqlDbType.Int);
                    cmd.Parameters.Add("@street", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Municipality", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@houseNumber", SqlDbType.NVarChar);

                    cmd.Parameters["@fk_simDataset"].Value = simDatasetId;

                    foreach (Customer customer in customers)
                    {
                        cmd.Parameters["@firstname"].Value = customer.Name;
                        cmd.Parameters["@lastname"].Value = customer.Lastname;
                        cmd.Parameters["@gender"].Value = customer.Gender.ToString();                  
                        cmd.Parameters["@Birthdate"].Value = customer.BirthDate;
                        cmd.Parameters["@Street"].Value = customer.Street;
                        cmd.Parameters["@Municipality"].Value = customer.Municipality;
                        cmd.Parameters["@houseNumber"].Value = customer.HouseNumber;
                        cmd.ExecuteNonQuery();

                    }

                }
                catch(Exception ex) { throw new Exception(ex.Message); }
            }
        }

        private void WriteSimulationDataset(SimulationInformation sim)
        {
            string sql = "insert into SimulationDataset (creationDate, fk_simConfig, averageAge) output INSERTED.ID values (@creationDate, @fk_simConfig, @averageAge)";
            if (conn.State == ConnectionState.Closed) conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = sql;

                try
                {
                    cmd.Parameters.AddWithValue("creationDate", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("fk_simConfig", simConfigId);
                    cmd.Parameters.AddWithValue("averageAge", sim.AverageAgeOriginal);
                    simDatasetId = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex) { throw new Exception(ex.Message); }

            }
        }

        private int GetCountryVersionId(int uploadYear, string country)
        {
            //TODO can I get this method in a seperate service like a lookup repo? (it's the same method)
            string sql = "select cv.Id from CountryVersion cv JOIN Country c on c.Id = cv.fk_countryId " +
               "WHERE cv.uploadYear = @year and c.name = @country;";

            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@year", uploadYear);
            cmd.Parameters.AddWithValue("country", country);

            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return (int)result;
            }
            else
            {
                throw new Exception($"CountryVersion not found for {country} and {uploadYear}");
            }
        }

        private void WriteSimulationConfiguration(SimulationInformation sim, SqlConnection conn)
        {
            string sql = "insert into SimulationConfiguration (clientName, minAge, maxAge, randomSeed, fk_countryVersion, " +
                "numberOfCustomers, houseNumberMax, houseNumberLetterPercent) " +
                "output INSERTED.ID values (@clientName, @minAge, @maxAge, @randomSeed, @fk_countryVersion, @numberOfCustomers, @houseNumberMax, @houseNumberLetterPercent)";
            

            if (conn.State == ConnectionState.Closed) conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = sql;

                try
                {
                    cmd.Parameters.AddWithValue("clientName", sim.ClientName);
                    cmd.Parameters.AddWithValue("minAge", sim.MinAge);
                    cmd.Parameters.AddWithValue("maxAge", sim.MaxAge);
                    cmd.Parameters.AddWithValue("randomSeed", sim.RandomSeed);
                    cmd.Parameters.AddWithValue("fk_countryVersion", fkCountrVersion);
                    cmd.Parameters.AddWithValue("numberOfCustomers", sim.AmountOfCust);
                    cmd.Parameters.AddWithValue("houseNumberMax", sim.MaxHouseNr); 
                    cmd.Parameters.AddWithValue("houseNumberLetterPercent", sim.HouseNumberLetterPercentage);

                    simConfigId = (int)cmd.ExecuteScalar();
                    sim.Id = simConfigId;

                }
                catch (Exception ex) { throw new Exception(ex.Message); }
            }



        }

    }
}
