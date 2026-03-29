using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public static class ClsCountryData
    {
        private static CountryDTO ReadData(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = Comd.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        return new CountryDTO
                        {
                            CountryID = Reader["CountryID"] != DBNull.Value ? Convert.ToInt32(Reader["CountryID"]) : -1,
                            CountryName = Reader["CountryName"] != DBNull.Value ? Convert.ToString(Reader["CountryName"]) : string.Empty,
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Retrieves all countries
        static public DataTable GetAllCountries()
        {
            string Query = "SELECT * FROM Countries order by CountryName;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        return UtilitiesClass.ExecuteDataTable(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static CountryDTO Find(int CountryID)
        {
            string Query = "SELECT * FROM Countries WHERE CountryID = @CountryID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;

                        return ReadData(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static CountryDTO Find(string CountryName)
        {
            string Query = "SELECT * FROM Countries WHERE CountryName = @CountryName";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryName;

                        return ReadData(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }
    }
}