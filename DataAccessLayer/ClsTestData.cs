using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    // Data access class for Test-related database operations.
    public static class ClsTestData
    {
        // Read a Test record from SqlDataReader and return as TestDTO.
        private static TestDTO ReadTest(SqlCommand Comm)
        {
            try
            {
                using (SqlDataReader Reader = Comm.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        return new TestDTO
                        {
                            TestID = Reader["TestID"] != DBNull.Value ? Convert.ToInt32(Reader["TestID"]) : -1,
                            TestAppointmentID = Reader["TestAppointmentID"] != DBNull.Value ? Convert.ToInt32(Reader["TestAppointmentID"]) : -1,
                            TestResult = Reader["TestResult"] != DBNull.Value ? Convert.ToBoolean(Reader["TestResult"]) : false,
                            Notes = Reader["Notes"] != DBNull.Value ? Reader["Notes"].ToString() : string.Empty,
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1
                        };
                    }
                }
            }
            catch (Exception e) { ClsLogger.Log(e); return null; }

            return null;
        }

        // Add parameters to the command based on TestDTO values.
        private static void SetParameters(SqlCommand Cmd, TestDTO Test)
        {
            if (Cmd.CommandText.Contains("@TestAppointmentID"))
                Cmd.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = Test.TestAppointmentID;
            if (Cmd.CommandText.Contains("@TestResult"))
                Cmd.Parameters.Add("@TestResult", SqlDbType.Bit).Value = Test.TestResult;
            if (Cmd.CommandText.Contains("@Notes"))
                Cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Test.Notes) ? (object)DBNull.Value : Test.Notes;
            if (Cmd.CommandText.Contains("@CreatedByUserID"))
                Cmd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = Test.CreatedByUserID;
        }

        // Find a test by ID and return TestDTO (or null if not found).
        public static TestDTO Find(int TestID)
        {
            string Query = @"SELECT *FROM Tests WHERE TestID = @TestID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.Add("@TestID", SqlDbType.Int).Value = TestID;

                        return ReadTest(Cmd);
                    }
                }
            }
            catch (Exception e) { ClsLogger.Log(e); return null; }
        }

        // Count passed tests for a given application ID.
        public static int GetPassedTestsCountForApplication(int LocalDrivingLicenseApplicationID)
        {
            string Query = @"SELECT COUNT(*) FROM Tests T
                           INNER JOIN TestAppointments TA ON T.TestAppointmentID = TA.TestAppointmentID
                           WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                           AND T.TestResult = 1";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                        int Result = UtilitiesClass.ExecuteScalar(Cmd);

                        return Result == -1 ? 0 : Result;
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return 0; }
        }

        // Check if a person passed a specific test type.
        public static bool HasPassedTestType(int TestTypeID, int LocalDrivingLicenseApplicationID)
        {
            string Query = @"SELECT COUNT(*) FROM Tests T
                           INNER JOIN TestAppointments TA ON T.TestAppointmentID = TA.TestAppointmentID
                           WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                           AND T.TestResult = 1
                           AND TA.TestTypeID = @TestTypeID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                        Cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        return (UtilitiesClass.ExecuteScalar(Cmd) > 0);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return false; }
        }

        // Add a new test and lock the appointment.
        public static int AddNew(TestDTO Test)
        {
            string Query = @"INSERT INTO Tests (TestAppointmentID, TestResult, Notes, CreatedByUserID)
                           VALUES (@TestAppointmentID, @TestResult, @Notes, @CreatedByUserID);
                           SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Cmd, Test);

                        return UtilitiesClass.ExecuteScalar(Cmd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return -1; }
        }

        // Update an existing test record.
        public static bool Update(TestDTO Test)
        {
            string Query = @"UPDATE Tests
                           SET TestAppointmentID = @TestAppointmentID,
                               TestResult = @TestResult,
                               Notes = @Notes,
                               CreatedByUserID = @CreatedByUserID
                           WHERE TestID = @TestID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Cmd, Test);

                        return (UtilitiesClass.ExecuteNonQuery(Cmd) > 0);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return false; }
        }
    }
}