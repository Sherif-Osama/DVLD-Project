using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace DataAccessLayer
{
    // Data access class for Applications database operations.
    public static class ClsApplicationsData
    {
        // Read an application from SqlDataReader and return as ApplicationDTO.
        private static async Task<ApplicationDTO> ReadDataAsync(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Read = await Comd.ExecuteReaderAsync())
                {
                    if (await Read.ReadAsync())
                    {
                        return new ApplicationDTO
                        {
                            ApplicationID = Read["ApplicationID"] != DBNull.Value ? Convert.ToInt32(Read["ApplicationID"]) : -1,
                            ApplicantPersonID = Read["ApplicantPersonID"] != DBNull.Value ? Convert.ToInt32(Read["ApplicantPersonID"]) : -1,
                            ApplicationDate = Read["ApplicationDate"] != DBNull.Value ? Convert.ToDateTime(Read["ApplicationDate"]) : DateTime.MinValue,
                            ApplicationTypeID = Read["ApplicationTypeID"] != DBNull.Value ? Convert.ToInt32(Read["ApplicationTypeID"]) : -1,
                            Status = Read["ApplicationStatus"] != DBNull.Value ? Convert.ToByte(Read["ApplicationStatus"]) : (byte)0,
                            LastStatusDate = Read["LastStatusDate"] != DBNull.Value ? Convert.ToDateTime(Read["LastStatusDate"]) : DateTime.MinValue,
                            PaidFees = Read["PaidFees"] != DBNull.Value ? Convert.ToSingle(Read["PaidFees"]) : 0,
                            CreatedByUserID = Read["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Read["CreatedByUserID"]) : -1
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Add parameters to command based on ApplicationDTO values.
        private static void AddParameters(SqlCommand Comd, ApplicationDTO Application)
        {
            if (Comd.CommandText.Contains("@ApplicationID"))
                Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = Application.ApplicationID;
            if (Comd.CommandText.Contains("@ApplicantPersonID"))
                Comd.Parameters.Add("@ApplicantPersonID", SqlDbType.Int).Value = Application.ApplicantPersonID;
            if (Comd.CommandText.Contains("@ApplicationDate"))
                Comd.Parameters.Add("@ApplicationDate", SqlDbType.DateTime).Value = Application.ApplicationDate;
            if (Comd.CommandText.Contains("@ApplicationTypeID"))
                Comd.Parameters.Add("@ApplicationTypeID", SqlDbType.Int).Value = Application.ApplicationTypeID;
            if (Comd.CommandText.Contains("@ApplicationStatus"))
                Comd.Parameters.Add("@ApplicationStatus", SqlDbType.TinyInt).Value = Application.Status;
            if (Comd.CommandText.Contains("@LastStatusDate"))
                Comd.Parameters.Add("@LastStatusDate", SqlDbType.DateTime).Value = Application.LastStatusDate;
            if (Comd.CommandText.Contains("@PaidFees"))
                Comd.Parameters.Add("@PaidFees", SqlDbType.Float).Value = Application.PaidFees;
            if (Comd.CommandText.Contains("@CreatedByUserID"))
                Comd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = Application.CreatedByUserID;
        }

        // Find an application by ID and return as ApplicationDTO.
        public static async Task<ApplicationDTO> FindAsync(int ApplicationID)
        {
            string Query = "SELECT * FROM Applications WHERE ApplicationID = @ApplicationID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Insert a new application and return the new ID (or -1 on error).
        public static async Task<int> AddNewAsync(ApplicationDTO Application)
        {
            string Query = @"INSERT INTO Applications (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
                             VALUES (@ApplicantPersonID, @ApplicationDate, @ApplicationTypeID, @ApplicationStatus, @LastStatusDate, @PaidFees, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        AddParameters(Comd, Application);

                        return await UtilitiesClass.ExecuteScalarAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        // Update an existing application record.
        public static async Task<bool> UpdateAsync(ApplicationDTO Application)
        {
            string Query = @"UPDATE Applications
                             SET ApplicantPersonID = @ApplicantPersonID,
                                 ApplicationDate = @ApplicationDate,
                                 ApplicationTypeID = @ApplicationTypeID,
                                 ApplicationStatus = @ApplicationStatus,
                                 LastStatusDate = @LastStatusDate,
                                 PaidFees = @PaidFees,
                                 CreatedByUserID = @CreatedByUserID
                             WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        AddParameters(Comd, Application);
                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Find an active (in-progress) application for a person with a specific application type.
        public static async Task<ApplicationDTO> FindActiveApplicationTypeAsync(int ApplicantPersonID, int ApplicationTypeID)
        {
            string Query = "SELECT * FROM Applications WHERE ApplicationTypeID = @ApplicationTypeID AND ApplicantPersonID = @ApplicantPersonID And ApplicationStatus = 1";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicantPersonID", SqlDbType.Int).Value = ApplicantPersonID;
                        Comd.Parameters.Add("@ApplicationTypeID", SqlDbType.Int).Value = ApplicationTypeID;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<bool> UpdateStatusAsync(int ApplicationID, short NewStatus)
        {
            string Query = @"UPDATE Applications
                             SET ApplicationStatus = @ApplicationStatus,
                                 LastStatusDate = @LastStatusDate
                             WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        Comd.Parameters.Add("@ApplicationStatus", SqlDbType.SmallInt).Value = NewStatus;
                        Comd.Parameters.Add("@LastStatusDate", SqlDbType.DateTime).Value = DateTime.Now;

                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        public static async Task<bool> DeleteAsync(int ApplicationID)
        {
            string Qeury = "DELETE From Applications WHERE ApplicationID = @ApplicationID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Qeury, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}