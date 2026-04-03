using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace DataAccessLayer
{
    public static class ClsDetainedLicenseData
    {
        private static void SetParameters(SqlCommand Command, DetainedLicenseDTO DetainedLicense)
        {
            if (Command.CommandText.Contains("@DetainID"))
                Command.Parameters.Add("@DetainID", SqlDbType.Int).Value = DetainedLicense.DetainID;
            if (Command.CommandText.Contains("@LicenseID"))
                Command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = DetainedLicense.LicenseID;
            if (Command.CommandText.Contains("@DetainDate"))
                Command.Parameters.Add("@DetainDate", SqlDbType.DateTime).Value = DetainedLicense.DetainDate;
            if (Command.CommandText.Contains("@FineFees"))
                Command.Parameters.Add("@FineFees", SqlDbType.Float).Value = DetainedLicense.FineFees;
            if (Command.CommandText.Contains("@CreatedByUserID"))
                Command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = DetainedLicense.CreatedByUserID;
            if (Command.CommandText.Contains("@IsReleased"))
                Command.Parameters.Add("@IsReleased", SqlDbType.Bit).Value = DetainedLicense.IsReleased;
            if (Command.CommandText.Contains("@ReleaseDate"))
                Command.Parameters.Add("@ReleaseDate", SqlDbType.DateTime).Value = DetainedLicense.ReleaseDate;
            if (Command.CommandText.Contains("@ReleasedByUserID"))
                Command.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = DetainedLicense.ReleasedByUserID;
            if (Command.CommandText.Contains("@ReleaseApplicationID"))
                Command.Parameters.Add("@ReleaseApplicationID", SqlDbType.Int).Value = DetainedLicense.ReleaseApplicationID;
        }

        private static async Task<DetainedLicenseDTO> ReadDataAsync(SqlCommand Comd)
        {
            using (SqlDataReader Reader = await Comd.ExecuteReaderAsync())
            {
                try
                {
                    if (await Reader.ReadAsync())
                    {
                        return new DetainedLicenseDTO()
                        {
                            DetainID = Reader["DetainID"] != DBNull.Value ? Convert.ToInt32(Reader["DetainID"]) : -1,
                            LicenseID = Reader["LicenseID"] != DBNull.Value ? Convert.ToInt32(Reader["LicenseID"]) : -1,
                            DetainDate = Reader["DetainDate"] != DBNull.Value ? Convert.ToDateTime(Reader["DetainDate"]) : DateTime.MinValue,
                            FineFees = Reader["FineFees"] != DBNull.Value ? Convert.ToSingle(Reader["FineFees"]) : 0,
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1,
                            IsReleased = Reader["IsReleased"] != DBNull.Value ? Convert.ToBoolean(Reader["IsReleased"]) : false,
                            ReleaseDate = Reader["ReleaseDate"] != DBNull.Value ? Convert.ToDateTime(Reader["ReleaseDate"]) : DateTime.MinValue,
                            ReleasedByUserID = Reader["ReleasedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["ReleasedByUserID"]) : -1,
                            ReleaseApplicationID = Reader["ReleaseApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["ReleaseApplicationID"]) : -1,
                        };
                    }
                }
                catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
                return null;
            }
        }

        public static async Task<DataTable> GetAllDetainedLicensesAsync()
        {
            string Query = "SELECT * FROM DetainedLicenses_View";

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

        public static async Task<bool> IsLicenseDetainedAsync(int LicenseID)
        {
            string Query = @"SELECT COUNT(*) FROM DetainedLicenses WHERE LicenseID = @LicenseID AND IsReleased = 0";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;

                        return (await UtilitiesClass.ExecuteScalarAsync(Cmd) > 0);
                    }
                }
            }
            catch (Exception e) { ClsLogger.Log(e); return false; }
        }

        public static async Task<DetainedLicenseDTO> FindByLicenseIDAsync(int LicenseID)
        {
            string Query = @"SELECT TOP 1 * FROM DetainedLicenses WHERE LicenseID = @LicenseID
                              ORDER BY DetainDate DESC, DetainID DESC";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseID;

                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<DetainedLicenseDTO> FindAsync(int DetainID)
        {
            string Query = "SELECT * FROM DetainedLicenses WHERE DetainID = @DetainID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DetainID", SqlDbType.Int).Value = DetainID;

                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<int> AddNewAsync(DetainedLicenseDTO DetainedLicenseDTO)
        {
            string Query = @"INSERT INTO DetainedLicenses(LicenseID, DetainDate ,FineFees ,CreatedByUserID ,IsReleased)
                                VALUES(@LicenseID, @DetainDate, @FineFees, @CreatedByUserID, @IsReleased);                            
                                SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comm = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comm, DetainedLicenseDTO);
                        return await UtilitiesClass.ExecuteScalarAsync(Comm);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        public static async Task<bool> UpdateAsync(DetainedLicenseDTO DetainedLicenseDTO)
        {
            string Query = @"
                            UPDATE DetainedLicenses SET 
                            IsReleased = @IsReleased,
                            ReleaseDate = @ReleaseDate,
                            ReleasedByUserID = @ReleasedByUserID,
                            ReleaseApplicationID = @ReleaseApplicationID
                            WHERE DetainID = @DetainID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, DetainedLicenseDTO);

                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}