using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    // Data access class for LocalDrivingLicenseApplication database operations.
    public static class ClsLocalDrivingLicenseApplicationData
    {
        // Read a local driving license application from SqlDataReader and return as DTO.
        private static async Task<LocalDrivingLicenseApplicationDTO> ReadDataAsync(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = await Comd.ExecuteReaderAsync())
                {
                    if (await Reader.ReadAsync())
                    {
                        return new LocalDrivingLicenseApplicationDTO
                        {
                            ApplicationID = Reader["ApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicationID"]) : -1,
                            ApplicantPersonID = Reader["ApplicantPersonID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicantPersonID"]) : -1,
                            ApplicationDate = Reader["ApplicationDate"] != DBNull.Value ? Convert.ToDateTime(Reader["ApplicationDate"]) : DateTime.Now,
                            ApplicationTypeID = Reader["ApplicationTypeID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicationTypeID"]) : -1,
                            Status = Reader["ApplicationStatus"] != DBNull.Value ? Convert.ToByte(Reader["ApplicationStatus"]) : default(byte),
                            LastStatusDate = Reader["LastStatusDate"] != DBNull.Value ? Convert.ToDateTime(Reader["LastStatusDate"]) : DateTime.Now,
                            PaidFees = Reader["PaidFees"] != DBNull.Value ? Convert.ToSingle(Reader["PaidFees"]) : 0,
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1,
                            LocalDrivingLicenseApplicationID = Reader["LocalDrivingLicenseApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["LocalDrivingLicenseApplicationID"]) : -1,
                            LicenseClassID = Reader["LicenseClassID"] != DBNull.Value ? Convert.ToInt32(Reader["LicenseClassID"]) : -1,
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }

            return null;
        }

        // Add parameters to command based on DTO values.
        private static void AddParameters(SqlCommand Comd, LocalDrivingLicenseApplicationDTO LocalDrivingLicenseAppLicationDTO)
        {
            if (Comd.CommandText.Contains("@ApplicationID"))
                Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseAppLicationDTO.ApplicationID;
            if (Comd.CommandText.Contains("@LicenseClassID"))
                Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LocalDrivingLicenseAppLicationDTO.LicenseClassID;
        }

        // Get all local driving license applications from the view.
        public static async Task<DataTable> GetAllLocalDrivingLicenseApplicationAsync()
        {
            string Query = "SELECT * FROM LocalDrivingLicenseApplications_View";

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

        // Check if a person has an active (in-progress) application for the same license class.
        public static async Task<bool> HasActiveApplicationAsync(int PersonID, int LicenseClassID)
        {
            string Query = @"SELECT COUNT(*) FROM LocalDrivingLicenseApplications
                            JOIN Applications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID 
                            WHERE ApplicantPersonID = @PersonID AND ApplicationStatus = 1 AND LicenseClassID = @LicenseClassID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();

                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        return await UtilitiesClass.ExecuteScalarAsync(Comd) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Find an application by its ID and return as DTO.
        public static async Task<LocalDrivingLicenseApplicationDTO> FindAsync(int LocalDrivingLicenseApplicationID)
        {
            string Query = @"SELECT * FROM LocalDrivingLicenseFullApplications_View
                             WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Insert a new local driving license application and return the new ID (or -1 on error).
        public static async Task<int> AddNewAsync(LocalDrivingLicenseApplicationDTO LocalDrivingLicenseAppLicationDTO)
        {
            string Query = @"INSERT INTO LocalDrivingLicenseApplications (ApplicationID, LicenseClassID)
                             VALUES (@ApplicationID, @LicenseClassID);
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        AddParameters(Comd, LocalDrivingLicenseAppLicationDTO);
                        return await UtilitiesClass.ExecuteScalarAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        // Update an existing local driving license application.
        public static async Task<bool> UpdateAsync(LocalDrivingLicenseApplicationDTO LocalDrivingLicenseAppLicationDTO)
        {
            string Query = @"UPDATE LocalDrivingLicenseApplications
                             SET LicenseClassID = @LicenseClassID
                             WHERE ApplicationID = @ApplicationID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        AddParameters(Comd, LocalDrivingLicenseAppLicationDTO);

                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);

                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        public static async Task<bool> DeleteAsync(int LocalDrivingLicenseApplicationID)
        {
            string Qeury = "DELETE FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Qeury, Conn))
                    {
                        Comd.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Find an active application for a person and license class.
        public static async Task<LocalDrivingLicenseApplicationDTO> FindActiveApplicationAsync(int PersonID, int LicenseClassID)
        {
            string Query = @"SELECT * FROM LocalDrivingLicenseApplications
                            JOIN Applications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID 
                            WHERE ApplicantPersonID = @PersonID AND ApplicationStatus = 1 AND LicenseClassID = @LicenseClassID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Count how many times a person has attempted a specific test type.
        public static async Task<byte> TestTrialCountAsync(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            string Query = @"SELECT COUNT(*) FROM Tests AS T
                            INNER JOIN TestAppointments AS TA ON TA.TestAppointmentID = T.TestAppointmentID
                            WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID AND TA.TestTypeID = @TestTypeID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        Comd.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;
                        int Result = await UtilitiesClass.ExecuteScalarAsync(Comd);

                        return Result == -1 ? (byte)0 : (byte)Result;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return 0; }
        }

        // Check if there is an active (unlocked) appointment for a specific test type.
        public static async Task<bool> HasActiveAppointmentAsync(int TestTypeID, int LocalDrivingLicenseApplicationID)
        {
            string Query = @"SELECT COUNT(*) FROM TestAppointments
                            WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID AND TestTypeID = @TestTypeID AND IsLocked = 0 ";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        Comd.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        return await UtilitiesClass.ExecuteScalarAsync(Comd) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Check if the person has ever taken a specific test type (whether passed or failed).
        public static async Task<bool> DoesAttendTestTypeAsync(int TestTypeID, int LocalDrivingLicenseApplicationID)
        {
            string Query = @"SELECT COUNT(*) FROM Tests AS T
                            INNER JOIN TestAppointments AS TA ON TA.TestAppointmentID = T.TestAppointmentID
                            WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID AND TestTypeID = @TestTypeID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        Comd.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = LocalDrivingLicenseApplicationID;

                        return await UtilitiesClass.ExecuteScalarAsync(Comd) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}