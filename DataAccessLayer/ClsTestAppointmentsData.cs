using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    // Data access class for TestAppointments database operations.
    public static class ClsTestAppointmentsData
    {
        // Read a test appointment from SqlDataReader and return as TestAppointmentsDTO.
        private static TestAppointmentsDTO ReadAppointment(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = Comd.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        return new TestAppointmentsDTO
                        {
                            TestAppointmentID = Reader["TestAppointmentID"] != DBNull.Value ? Convert.ToInt32(Reader["TestAppointmentID"]) : -1,
                            TestTypeID = Reader["TestTypeID"] != DBNull.Value ? Convert.ToInt32(Reader["TestTypeID"]) : -1,
                            LocalDrivingLicenseApplicationID = Reader["LocalDrivingLicenseApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["LocalDrivingLicenseApplicationID"]) : -1,
                            AppointmentDate = Reader["AppointmentDate"] != DBNull.Value ? Convert.ToDateTime(Reader["AppointmentDate"]) : DateTime.MinValue,
                            PaidFees = Reader["PaidFees"] != DBNull.Value ? Convert.ToSingle(Reader["PaidFees"]) : 0,
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1,
                            IsLocked = Reader["IsLocked"] != DBNull.Value ? Convert.ToBoolean(Reader["IsLocked"]) : false,
                            RetakeTestApplicationID = Reader["RetakeTestApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["RetakeTestApplicationID"]) : -1
                        };
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return null; }

            return null;
        }

        // Add parameters to command based on TestAppointmentsDTO values.
        private static void SetParameters(SqlCommand Cmd, TestAppointmentsDTO TAD)
        {
            if (Cmd.CommandText.Contains("@TestAppointmentID"))
                Cmd.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = TAD.TestAppointmentID;
            if (Cmd.CommandText.Contains("@TestTypeID"))
                Cmd.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TAD.TestTypeID;
            if (Cmd.CommandText.Contains("@LocalDrivingLicenseApplicationID"))
                Cmd.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = TAD.LocalDrivingLicenseApplicationID;
            if (Cmd.CommandText.Contains("@AppointmentDate"))
                Cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = TAD.AppointmentDate;
            if (Cmd.CommandText.Contains("@PaidFees"))
                Cmd.Parameters.Add("@PaidFees", SqlDbType.SmallMoney).Value = TAD.PaidFees;
            if (Cmd.CommandText.Contains("@CreatedByUserID"))
                Cmd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = TAD.CreatedByUserID;
            if (Cmd.CommandText.Contains("@IsLocked"))
                Cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = TAD.IsLocked;
            if (Cmd.CommandText.Contains("@RetakeTestApplicationID"))
            {
                if (TAD.RetakeTestApplicationID == -1)
                    Cmd.Parameters.AddWithValue("@RetakeTestApplicationID", DBNull.Value);
                else
                    Cmd.Parameters.Add("@RetakeTestApplicationID", SqlDbType.Int).Value = TAD.RetakeTestApplicationID;
            }
        }

        // Get all appointments for an application and test type, ordered by date descending.
        public static DataTable GetAllApplicationTestAppointments(int ApplicationID, int TestTypeID)
        {
            string Query = @"SELECT TestAppointmentID, AppointmentDate,PaidFees, IsLocked 
                                FROM TestAppointments 
                                WHERE TestTypeID = @TestTypeID AND LocalDrivingLicenseApplicationID = @ApplicationID
                                order by AppointmentDate desc";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        return UtilitiesClass.ExecuteDataTable(Cmd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return null; }
        }

        // Find an appointment by ID and return as TestAppointmentsDTO.
        public static TestAppointmentsDTO Find(int AppointmentID)
        {
            string Query = @"SELECT * FROM TestAppointments 
                                WHERE TestAppointmentID = @AppointmentID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.Add("@AppointmentID", SqlDbType.Int).Value = AppointmentID;

                        return ReadAppointment(Cmd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return null; }
        }

        // Check if an appointment is locked.
        public static bool IsLocked(int AppointmentID)
        {
            string Query = @"SELECT IsLocked FROM TestAppointments 
                                WHERE TestAppointmentID = @AppointmentID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.Add("@AppointmentID", SqlDbType.Int).Value = AppointmentID;

                        return (UtilitiesClass.ExecuteScalar(Cmd) != -1);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return false; }
        }

        // Insert a new appointment and return the new ID (or -1 on error).
        public static int AddNew(TestAppointmentsDTO TAD)
        {
            string Query = @"INSERT INTO TestAppointments(TestTypeID,LocalDrivingLicenseApplicationID,AppointmentDate,PaidFees,CreatedByUserID,IsLocked,RetakeTestApplicationID)
                                 VALUES (@TestTypeID,@LocalDrivingLicenseApplicationID, @AppointmentDate, @PaidFees, @CreatedByUserID, @IsLocked, @RetakeTestApplicationID) 
                                  SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Cmd, TAD);

                        return UtilitiesClass.ExecuteScalar(Cmd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return -1; }
        }

        // Update an existing appointment record.
        public static bool Update(TestAppointmentsDTO TAD)
        {
            string Query = @"UPDATE TestAppointments SET 
                                TestTypeID = @TestTypeID,
                                LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID,
                                AppointmentDate = @AppointmentDate,
                                PaidFees = @PaidFees,
                                CreatedByUserID = @CreatedByUserID,
                                IsLocked = @IsLocked,
                                RetakeTestApplicationID = @RetakeTestApplicationID
                             WHERE TestAppointmentID = @TestAppointmentID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Cmd, TAD);

                        return (UtilitiesClass.ExecuteNonQuery(Cmd) > 0);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return false; }
        }

        public static bool LockAppointment(int AppointmentID)
        {
            string Query = @"UPDATE TestAppointments SET 
                                IsLocked = 1 WHERE TestAppointmentID = @TestAppointmentID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = AppointmentID;

                        return (UtilitiesClass.ExecuteNonQuery(Cmd) > 0);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return false; }
        }
    }
}