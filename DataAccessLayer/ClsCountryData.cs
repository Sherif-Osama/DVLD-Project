using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class ClsCountryData
    {
        private static async Task<CountryDTO> ReadDataAsync(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = await Comd.ExecuteReaderAsync())
                {
                    if (await Reader.ReadAsync())
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
        static public async Task<DataTable> GetAllCountriesAsync()
        {
            string Query = "SELECT * FROM Countries order by CountryName;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        return await UtilitiesClass.ExecuteDataTableAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<CountryDTO> FindAsync(int CountryID)
        {
            string Query = "SELECT * FROM Countries WHERE CountryID = @CountryID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;

                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<CountryDTO> FindAsync(string CountryName)
        {
            string Query = "SELECT * FROM Countries WHERE CountryName = @CountryName";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryName;

                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }
    }
}