using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using SimulatorBL.Configuration;
using SimulatorBL.Domain;
using SimulatorBL.Enum;
using SimulatorBL.Interfaces;

namespace SimulatorDL
{
    public class RawDataWriterRepo : IRawDataWriter
    {
        private Dictionary<string, int> _municipalitys;   //key is municipality, municipality Id
        private Dictionary<(string, int), int> _countrys;       //key is country name + year, value countryVersionId
        private readonly string _connection;

        public RawDataWriterRepo() 
        {
            _connection = AppConfig.Current.ConnectionStrings.ClientSimulatorDb;
            _municipalitys = new Dictionary<string, int>();
            _countrys = GetDatasetCountryAndId();
            if (_countrys == null) _countrys = new();
        }
      

        private Dictionary<(string, int), int> GetDatasetCountryAndId()     
        {
            Dictionary<(string, int), int> data = new();
            string sql = "select cv.Id countryVersionId, cv.uploadYear, c.name country from CountryVersion cv join Country c on c.Id = cv.fk_countryId";

            using (SqlConnection sqlConn = new SqlConnection(_connection))
            using (SqlCommand cmd = sqlConn.CreateCommand())
            {
                sqlConn.Open();
                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int year = (int)rd["uploadYear"];
                        string country = ((string)rd["country"]).Trim().ToLower();                      
                        int id = (int)rd["countryVersionId"];

                        data.Add((country, year), id);
                    }
                }

            }

            return data;
        }

        public bool WriteRecordsToDB(List<NameRecord> firstNames, List<NameRecord> lastNames, List<AddressRecord> addresses)
        {                                                              
            //standard true: so if method is not used because it's not necessary, the user will still know if everything else succeeded.
            bool fnSucces = true;
            bool lnSucces = true;
            bool adSucces = true;
            
                if (firstNames.Count > 0) fnSucces = WriteNameRecordsToDB(firstNames, "firstNameRecord"); 
                if (lastNames.Count > 0) lnSucces = WriteNameRecordsToDB(lastNames, "lastNameRecord");              
                if (addresses.Count > 0) adSucces = WriteAddressesToDB(addresses);           
            
            return fnSucces && lnSucces && adSucces;
            
        }

        public bool WriteImportMetaDataToDB(bool isNames, bool isAddresses, string path, string country)
        {         
            string sql = "insert into ImportMetaData (importDate, sourceFile, IsNamesData, IsAddressData, fk_countryVersionId) " +
                "values (@importDate, @sourceFile, @IsNamesData, @IsAddressData, @fk_countryVersionId)";

            using (SqlConnection sqlConn = new SqlConnection(_connection))
            using (SqlCommand cmd = sqlConn.CreateCommand())
            {
                sqlConn.Open();
                cmd.CommandText = sql;

                SqlTransaction trans = sqlConn.BeginTransaction();
                cmd.Transaction = trans;

                try
                {
                    cmd.Parameters.AddWithValue("@importDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@sourceFile", path);
                    // false = 0        true = 1
                    cmd.Parameters.AddWithValue("@IsNamesData", isNames == true ? 1 : 0);
                    cmd.Parameters.AddWithValue("@IsAddressData", isAddresses == true ? 1 : 0);
                    cmd.Parameters.AddWithValue("@fk_countryVersionId", _countrys[(country, DateTime.Now.Year)]);

                    cmd.ExecuteNonQuery();

                    trans.Commit();
                    return true;
                }
                catch { trans.Rollback(); return false; }

                              
            }       
       }

        private bool WriteNameRecordsToDB( List<NameRecord> names, string table)
        {
            int countryVersion = 0;
           
            string sqlName = $"insert into {table} (Name, Frequency, Gender, fk_countryVersionId) " +
               "values (@Name, @Frequency, @Gender, @fk_country)";
            string sqlCountry = "insert into Country (name) output INSERTED.ID values (@name)"; 
            string sqlCoVe = "insert into CountryVersion (uploadYear, fk_countryId) output INSERTED.ID values (@year, @fk_countryId)";

            using (SqlConnection sqlConn = new SqlConnection(_connection))
            using (SqlCommand cmdName = sqlConn.CreateCommand())
            using (SqlCommand cmdCountry = sqlConn.CreateCommand())
            using (SqlCommand cmdCoVe = sqlConn.CreateCommand())
            {
                sqlConn.Open(); 

                cmdName.CommandText = sqlName;
                cmdCountry.CommandText = sqlCountry;
                cmdCoVe.CommandText = sqlCoVe;
             
                cmdName.Parameters.Add(new SqlParameter("@Name", System.Data.SqlDbType.NVarChar));
                cmdName.Parameters.Add(new SqlParameter("@Frequency", System.Data.SqlDbType.Int));
                cmdName.Parameters.Add(new SqlParameter("@Gender", System.Data.SqlDbType.NVarChar));
                cmdName.Parameters.Add(new SqlParameter("@fk_country", System.Data.SqlDbType.Int));

                cmdCountry.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));

                cmdCoVe.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
                cmdCoVe.Parameters.Add(new SqlParameter("@fk_countryId", SqlDbType.Int));



                SqlTransaction trans = sqlConn.BeginTransaction();
               
                try
                {
                    cmdName.Transaction = trans;
                    cmdCountry.Transaction = trans;
                    cmdCoVe.Transaction = trans;
                    
                    
                    foreach (var nameRecord in names)
                    {
                        string country = names[0].Country.ToString();
                        int year = DateTime.Now.Year;
                        //if country already exists: get id from dict
                        if ( _countrys != null && _countrys.ContainsKey((country.Trim().ToLower(), year)))
                        {
                            countryVersion = _countrys[(names[0].Country.ToString(), year)];
                        }
                        else // if not, add country to db and make link with countryversion + get this Id and add to table(namerecord)
                        {
                            cmdCountry.Parameters["@name"].Value = country.Trim().ToLower();
                            int countryId = (int)cmdCountry.ExecuteScalar();

                            cmdCoVe.Parameters["@year"].Value = DateTime.Now.Year;
                            cmdCoVe.Parameters["@fk_countryId"].Value = countryId;
                            countryVersion = (int)cmdCoVe.ExecuteScalar();

                            //add to dictionary
                            _countrys.Add((country.Trim().ToLower(), year), countryVersion);
                        }

                        cmdName.Parameters["@Name"].Value = nameRecord.Name.ToLower().Trim();
                        cmdName.Parameters["@Frequency"].Value = nameRecord.Frequency;
                        cmdName.Parameters["@Gender"].Value = nameRecord.Gender.ToString().ToLower().Trim();
                        cmdName.Parameters["@fk_country"].Value = countryVersion;

                        cmdName.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception ex) { trans.Rollback(); return false; }
            }
        }
     
        private bool WriteAddressesToDB(List<AddressRecord> addresses)
        {         
            int countryVersion = 0;        

            string sqlMunic = "insert into MunicipalityRecord (Municipality_name, fk_countryVersionId) " +
                "output INSERTED.ID values (@MunicName, @fk_country)";

            string sqlAd = "insert into AddressRecord (fk_municipalityId, Street, fk_countryVersionId) " +
                "output INSERTED.ID values (@Municipality, @Street, @fk_country)";

            string sqlCoVe = "insert into CountryVersion (uploadYear, fk_countryId) output INSERTED.ID values (@year, @fk_countryId)";

            string sqlCountry = "insert into Country (name) output INSERTED.ID values (@name)";        

            using (SqlConnection sqlConn = new SqlConnection(_connection))
            using (SqlCommand cmdMun = sqlConn.CreateCommand())
            using(SqlCommand cmdAd = sqlConn.CreateCommand())
            using (SqlCommand cmdCoVe = sqlConn.CreateCommand())
            using (SqlCommand cmdCountry = sqlConn.CreateCommand())
            {
                sqlConn.Open();

                cmdMun.CommandText = sqlMunic;    
                cmdAd.CommandText = sqlAd;
                cmdCountry.CommandText = sqlCountry;
                cmdCoVe.CommandText = sqlCoVe;
                
                cmdMun.Parameters.Add(new SqlParameter("@MunicName", System.Data.SqlDbType.NVarChar));
                cmdMun.Parameters.Add(new SqlParameter("@fk_country", System.Data.SqlDbType.Int));

                cmdAd.Parameters.Add(new SqlParameter("@Municipality", System.Data.SqlDbType.Int));
                cmdAd.Parameters.Add(new SqlParameter("@Street", System.Data.SqlDbType.NVarChar));
                cmdAd.Parameters.Add(new SqlParameter("@fk_country", System.Data.SqlDbType.Int));

                cmdCoVe.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
                cmdCoVe.Parameters.Add(new SqlParameter("@fk_countryId", SqlDbType.Int));

                cmdCountry.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));

                SqlTransaction trans = sqlConn.BeginTransaction();

                try
                {
                    cmdMun.Transaction = trans;
                    cmdAd.Transaction = trans;
                    cmdCoVe.Transaction = trans;
                    cmdCountry.Transaction = trans;

                    foreach (var address in addresses)
                    {
                        //COUNTRY
                        string country = address.Country.ToString().Trim().ToLower();
                        int year = DateTime.Now.Year;

                        //if country already exists: get id from dict
                        if (_countrys.ContainsKey((country.Trim().ToLower(), year)))
                        {
                            countryVersion = _countrys[(country.Trim().ToLower(), year)];
                        }
                        else // if not, add country to db and make link with countryversion + get this Id and add to table(namerecord)
                        {
                            cmdCountry.Parameters["@name"].Value = country.Trim().ToLower();
                            int countryId = (int)cmdCountry.ExecuteScalar();

                            cmdCoVe.Parameters["@year"].Value = DateTime.Now.Year;
                            cmdCoVe.Parameters["@fk_countryId"].Value = countryId;
                            countryVersion = (int)cmdCoVe.ExecuteScalar();

                            //add to dictionary
                            _countrys.Add((country.Trim(), year), countryVersion);
                        }

                        if (address.Municipality != null)
                        {
                            //MUNICIPALITY
                            if (!_municipalitys.ContainsKey(address.Municipality))
                            {
                                cmdMun.Parameters["@MunicName"].Value = address.Municipality.ToLower().Trim();
                                cmdMun.Parameters["@fk_country"].Value = countryVersion;

                                int index = (int)cmdMun.ExecuteScalar();
                                _municipalitys.Add(address.Municipality, index);

                            }
                        }

                        if (address.Street != null)
                        {

                            //ADDRESRECORD
                            cmdAd.Parameters["@Municipality"].Value = address.Municipality != null ? _municipalitys[address.Municipality] : DBNull.Value; // fk
                            
                            cmdAd.Parameters["@Street"].Value = address.Street.ToLower().Trim();
                            cmdAd.Parameters["@fk_country"].Value = countryVersion; //fk

                            cmdAd.ExecuteNonQuery();
                        }
                        
                    }

                trans.Commit();
                    return true;
                }
                catch (Exception ex) { trans.Rollback(); return false; }
            }
        }
    }
}
