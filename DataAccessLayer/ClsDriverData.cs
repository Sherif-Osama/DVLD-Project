using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    // Data access class for Driver database operations.
    public class ClsDriverData
    {
        // Add parameters to command based on DriverDTO values.
        private static void SetParameters(SqlCommand Comd, DriverDTO Driver)
        {
            if (Comd.CommandText.Contains("@DriverID"))
                Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = Driver.DriverID;
            if (Comd.CommandText.Contains("@PersonID"))
                Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = Driver.PersonID;
            if (Comd.CommandText.Contains("@CreatedByUserID"))
                Comd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = Driver.CreatedByUserID;
            if (Comd.CommandText.Contains("@CreatedDate"))
                Comd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Driver.CreatedDate;
        }

        // Read a driver from SqlDataReader and return as DriverDTO.
        private static DriverDTO ReadDriverData(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = Comd.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        return new DriverDTO
                        {
                            DriverID = Reader["DriverID"] != DBNull.Value ? Convert.ToInt32(Reader["DriverID"]) : -1,
                            PersonID = Reader["PersonID"] != DBNull.Value ? Convert.ToInt32(Reader["PersonID"]) : -1,
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1,
                            CreatedDate = Reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(Reader["CreatedDate"]) : default(DateTime)
                        };
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return null; }
            return null;
        }

        // Get all drivers as a DataTable.
        public static DataTable GetAllDrivers()
        {
            string Query = "SELECT * FROM Drivers_View order by FullName";
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
            catch (Exception ex) { ClsLogger.Log(ex); return null; }
        }

        public static DriverDTO FindByPersonID(int PersonID)
        {
            string Query = "SELECT * FROM Drivers WHERE PersonID = @PersonID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        return ReadDriverData(Comd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return null; }
        }

        // Find a driver by ID and return as DriverDTO.
        public static DriverDTO Find(int DriverID)
        {
            string Query = "SELECT * FROM Drivers WHERE DriverID =  @DriverID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        return ReadDriverData(Comd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return null; }
        }

        // Insert a new driver and return the new ID (or -1 on error).
        public static int AddNew(DriverDTO Driver)
        {
            string Query = @"INSERT INTO Drivers (PersonID, CreatedByUserID, CreatedDate)
                           VALUES (@PersonID, @CreatedByUserID, @CreatedDate);
                            SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, Driver);

                        return UtilitiesClass.ExecuteScalar(Comd);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return -1; }
        }

        // Update an existing driver record.
        public static bool Update(DriverDTO Driver)
        {
            string Query = @"UPDATE Drivers SET 
                             PersonID = @PersonID,
                             CreatedByUserID = @CreatedByUserID,
                             CreatedDate = @CreatedDate
                             WHERE DriverID = @DriverID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, Driver);

                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception ex) { ClsLogger.Log(ex); return false; }
        }
    }
}