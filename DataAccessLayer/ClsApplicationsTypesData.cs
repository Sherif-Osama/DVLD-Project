using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class ClsApplicationsTypesData
    {
        // Sets SQL parameters based on the DTO object values
        private static void SetParameters(SqlCommand Command, ApplicationTypesDTO ApplicationTypes)
        {
            try
            {
                if (Command.CommandText.Contains("@ApplicationTypeID"))
                    Command.Parameters.Add("@ApplicationTypeID", SqlDbType.Int).Value = ApplicationTypes.ID;
                if (Command.CommandText.Contains("@ApplicationFees"))
                    Command.Parameters.Add("@ApplicationFees", SqlDbType.SmallMoney).Value = ApplicationTypes.Fees;
                if (Command.CommandText.Contains("@ApplicationTypeTitle"))
                    Command.Parameters.Add("@ApplicationTypeTitle", SqlDbType.NVarChar).Value = ApplicationTypes.Title;
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); }
        }

        // Reads data from SQL reader and maps it to a DTO object
        private static async Task<ApplicationTypesDTO> ReadTypeDataAsync(SqlCommand command)
        {
            try
            {
                using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                {
                    if (await Reader.ReadAsync())
                    {
                        return new ApplicationTypesDTO
                        {
                            ID = Reader["ApplicationTypeID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicationTypeID"]) : -1,
                            Title = Reader["ApplicationTypeTitle"] != DBNull.Value ? Reader["ApplicationTypeTitle"].ToString() : string.Empty,
                            Fees = Reader["ApplicationFees"] != DBNull.Value ? Convert.ToSingle(Reader["ApplicationFees"]) : -1
                        };
                    }
                }

            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Returns all records from ApplicationTypes table
        public static async Task<DataTable> GetAllApplicationsTypesAsync()
        {
            string Query = "SELECT * FROM ApplicationTypes";
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

        // Finds an ApplicationType record by its ID
        public static async Task<ApplicationTypesDTO> FindAsync(int ApplicationID)
        {
            string Query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        return await ReadTypeDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Finds an ApplicationType record by its title
        public static async Task<ApplicationTypesDTO> FindAsync(string ApplicationTitle)
        {
            string Query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeTitle = @ApplicationTitle";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationTitle", SqlDbType.NVarChar).Value = ApplicationTitle;
                        return await ReadTypeDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Updates an existing ApplicationType record in the database
        public static async Task<bool> UpdateApplicationsTypesAsync(ApplicationTypesDTO ApplicationTypes)
        {
            string Query = @"UPDATE ApplicationTypes
                             SET
                             ApplicationTypeTitle = @ApplicationTypeTitle,
                             ApplicationFees = @ApplicationFees
                             WHERE ApplicationTypeID = @ApplicationTypeID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, ApplicationTypes);

                        return await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}