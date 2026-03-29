using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    // Data access class for License database operations.
    public static class ClsLicensesData
    {
        // Read a license from SqlDataReader and return as LicenseDTO.
        private static LicenseDTO ReadLicenseInfo(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = Comd.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        return new LicenseDTO
                        {
                            LicenseID = Reader["LicenseID"] != DBNull.Value ? Convert.ToInt32(Reader["LicenseID"]) : -1,
                            ApplicationID = Reader["ApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicationID"]) : -1,
                            DriverID = Reader["DriverID"] != DBNull.Value ? Convert.ToInt32(Reader["DriverID"]) : -1,
                            LicenseClassID = Reader["LicenseClass"] != DBNull.Value ? Convert.ToInt32(Reader["LicenseClass"]) : -1,
                            IssueDate = Reader["IssueDate"] != DBNull.Value ? Convert.ToDateTime(Reader["IssueDate"]) : DateTime.MinValue,
                            ExpirationDate = Reader["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(Reader["ExpirationDate"]) : DateTime.MinValue,
                            Notes = Reader["Notes"] != DBNull.Value ? Reader["Notes"].ToString() : string.Empty,
                            PaidFees = Reader["PaidFees"] != DBNull.Value ? Convert.ToSingle(Reader["PaidFees"]) : 0,
                            IsActive = Reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(Reader["IsActive"]) : false,
                            IssueReason = Reader["IssueReason"] != DBNull.Value ? Convert.ToByte(Reader["IssueReason"]) : default(byte),
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Add parameters to command based on LicenseDTO values.
        private static void SetParameters(SqlCommand Comd, LicenseDTO License)
        {
            if (Comd.CommandText.Contains("@LicenseID"))
                Comd.Parameters.Add("@LicenseID", SqlDbType.Int).Value = License.LicenseID;
            if (Comd.CommandText.Contains("@ApplicationID"))
                Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = License.ApplicationID;
            if (Comd.CommandText.Contains("@DriverID"))
                Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = License.DriverID;
            if (Comd.CommandText.Contains("@LicenseClass"))
                Comd.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = License.LicenseClassID;
            if (Comd.CommandText.Contains("@IssueDate"))
                Comd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = License.IssueDate;
            if (Comd.CommandText.Contains("@ExpirationDate"))
                Comd.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = License.ExpirationDate;
            if (Comd.CommandText.Contains("@PaidFees"))
                Comd.Parameters.Add("@PaidFees", SqlDbType.Float).Value = License.PaidFees;
            if (Comd.CommandText.Contains("@IsActive"))
                Comd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = License.IsActive;
            if (Comd.CommandText.Contains("@IssueReason"))
                Comd.Parameters.Add("@IssueReason", SqlDbType.TinyInt).Value = License.IssueReason;
            if (Comd.CommandText.Contains("@CreatedByUserID"))
                Comd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = License.CreatedByUserID;
            if (Comd.CommandText.Contains("@Notes"))
            {
                if (string.IsNullOrWhiteSpace(License.Notes))
                    Comd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    Comd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = License.Notes;
            }
        }

        public static DataTable GetDriverLicenses(int DriverID)
        {
            string Query = @"
                            SELECT LicenseID,
                            ApplicationID,
                            LicenseClasses.ClassName,
                            IssueDate,
                            ExpirationDate,
                            IsActive FROM Licenses
                            INNER JOIN LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
                            WHERE DriverID = @DriverID ORDER BY IssueDate DESC";

            using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                Conn.Open();
                using (SqlCommand Comd = new SqlCommand(Query, Conn))
                {
                    Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;

                    return UtilitiesClass.ExecuteDataTable(Comd);
                }
            }
        }

        // Find a license by ID and return as LicenseDTO.
        public static LicenseDTO Find(int LicenseID)
        {
            string Query = @"SELECT TOP 1 *  FROM Licenses WHERE LicenseID = @LicenseID;";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;

                        return ReadLicenseInfo(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Find a license by application ID and return as LicenseDTO.
        public static LicenseDTO FindByApplicationID(int ApplicationID)
        {
            string Query = @"SELECT TOP 1 * FROM Licenses WHERE ApplicationID = @ApplicationID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();

                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = ApplicationID;
                        return ReadLicenseInfo(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Find a license by driver ID and license class ID and return as LicenseDTO.
        public static LicenseDTO Find(int DriverID, int LicenseClassID)
        {
            string Query = @"SELECT TOP 1 * FROM Licenses
                            WHERE DriverID = @DriverID AND LicenseClass = @LicenseClassID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        return ReadLicenseInfo(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static LicenseDTO FindActiveLicenseByDriverID(int DriverID, int LicenseClassID)
        {
            string Query = @"SELECT TOP 1 * FROM Licenses
                            WHERE DriverID = @DriverID AND LicenseClass = @LicenseClassID AND IsActive = 1;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        return ReadLicenseInfo(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Check if a person has an existing license for a specific license class.
        public static bool IsLicenseExist(int Person, int LicenseClassID)
        {
            string Query = @"SELECT COUNT(*) FROM Licenses
                            INNER JOIN Drivers on Drivers.DriverID = Licenses.DriverID
                            WHERE LicenseClass = @LicenseClassID AND Drivers.PersonID = @PersonID;";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = Person;

                        return UtilitiesClass.ExecuteScalar(Comd) != -1;
                    }
                }

            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        public static bool HasLicense(int DriverID, int LicenseClassID)
        {
            string Query = @"SELECT COUNT(*) FROM Licenses
                            WHERE DriverID = @DriverID AND LicenseClass = @LicenseClassID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        return UtilitiesClass.ExecuteScalar(Comd) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        public static bool HasActiveLicense(int DriverID, int LicenseClassID)
        {
            string Query = @"SELECT COUNT(*) FROM Licenses
                            WHERE DriverID = @DriverID AND LicenseClass = @LicenseClassID AND IsActive = 1;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;

                        return UtilitiesClass.ExecuteScalar(Comd) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Check if a license has been issued for a person in a specific license class.
        public static bool IssueLicense(int PersonID, int LicenseClassID)
        {
            string Query = @"SELECT COUNT(*) FROM Licenses
                            INNER JOIN Drivers on Drivers.DriverID = Licenses.DriverID
                            WHERE LicenseClass = @LicenseClassID AND Drivers.PersonID = @PersonID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseClassID", SqlDbType.Int).Value = LicenseClassID;
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;
                        return UtilitiesClass.ExecuteScalar(Comd) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        public static bool DeactivateCurrentLicense(int LicenseID)
        {
            string Query = @"UPDATE Licenses SET IsActive = 0 WHERE LicenseID = @LicenseID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;
                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Insert a new license and return the new ID (or -1 on error).
        public static int AddNew(LicenseDTO License)
        {
            string Query = @"INSERT INTO 
                            Licenses(ApplicationID,DriverID,LicenseClass,
                            IssueDate,ExpirationDate,
                            Notes,PaidFees,IsActive,
                            IssueReason,CreatedByUserID)
                           VALUES (@ApplicationID,@DriverID,@LicenseClass,@IssueDate,
                            @ExpirationDate,@Notes,@PaidFees,@IsActive,@IssueReason,@CreatedByUserID)
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, License);

                        return UtilitiesClass.ExecuteScalar(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        // Update an existing license record.
        public static bool Update(LicenseDTO License)
        {
            string Query = @"UPDATE Licenses SET 
                            ApplicationID = @ApplicationID,
                            DriverID = @DriverID,
                            LicenseClass = @LicenseClass,
                            IssueDate = @IssueDate,
                            ExpirationDate = @ExpirationDate,
                            Notes = @Notes,
                            PaidFees = @PaidFees,
                            IsActive = @IsActive,
                            IssueReason = @IssueReason,
                            CreatedByUserID = @CreatedByUserID
                            WHERE LicenseID = @LicenseID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, License);

                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}