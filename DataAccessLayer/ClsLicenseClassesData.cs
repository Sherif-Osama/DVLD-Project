using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    // Data access class for LicenseClasses database operations.
    public static class ClsLicenseClassesData
    {
        // Read a license class from SqlDataReader and return as LicenseClassesDTO.
        private static async Task<LicenseClassesDTO> ReadDataAsync(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = await Comd.ExecuteReaderAsync())
                {
                    if (await Reader.ReadAsync())
                    {
                        return new LicenseClassesDTO
                        {
                            LicenseClassID = Reader["LicenseClassID"] != DBNull.Value ? Convert.ToInt32(Reader["LicenseClassID"]) : -1,
                            ClassName = Reader["ClassName"] != DBNull.Value ? Reader["ClassName"].ToString() : "",
                            ClassDescription = Reader["ClassDescription"] != DBNull.Value ? Reader["ClassDescription"].ToString() : "",
                            MinimumAllowedAge = Reader["MinimumAllowedAge"] != DBNull.Value ? Convert.ToByte(Reader["MinimumAllowedAge"]) : (byte)18,
                            DefaultValidityLength = Reader["DefaultValidityLength"] != DBNull.Value ? Convert.ToByte(Reader["DefaultValidityLength"]) : (byte)10,
                            ClassFees = Reader["ClassFees"] != DBNull.Value ? Convert.ToSingle(Reader["ClassFees"]) : -1
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Get all license class names as a DataTable.
        public static async Task<DataTable> GetAllLicenseClassesAsync()
        {
            string Query = "SELECT ClassName FROM LicenseClasses";
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

        // Find a license class by ID and return as LicenseClassesDTO.
        public static async Task<LicenseClassesDTO> FindAsync(int LicenseClassID)
        {
            string Query = "SELECT * FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Find a license class by name and return as LicenseClassesDTO.
        public static async Task<LicenseClassesDTO> FindAsync(string LicenseClassesName)
        {
            string Query = "SELECT * FROM LicenseClasses WHERE ClassName = @LicenseClassesName ";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseClassesName", SqlDbType.NVarChar).Value = LicenseClassesName;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }
    }
}