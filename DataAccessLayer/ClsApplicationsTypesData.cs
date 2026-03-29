using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

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
        private static ApplicationTypesDTO ReadTypeData(SqlCommand command)
        {
            try
            {
                using (SqlDataReader Reader = command.ExecuteReader())
                {
                    if (Reader.Read())
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
        public static DataTable GetAllApplicationsTypes()
        {
            string Query = "SELECT * FROM ApplicationTypes";
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

        // Finds an ApplicationType record by its ID
        public static ApplicationTypesDTO Find(int ApplicationID)
        {
            string Query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        return ReadTypeData(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Finds an ApplicationType record by its title
        public static ApplicationTypesDTO Find(string ApplicationTitle)
        {
            string Query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeTitle = @ApplicationTitle";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationTitle", SqlDbType.NVarChar).Value = ApplicationTitle;
                        return ReadTypeData(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Updates an existing ApplicationType record in the database
        public static bool UpdateApplicationsTypes(ApplicationTypesDTO ApplicationTypes)
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
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, ApplicationTypes);

                       return UtilitiesClass.ExecuteNonQuery(Comd) > 0;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}