using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace DataAccessLayer
{
    public static class ClsInternationalLicenseData
    {
        private static void SetParameters(SqlCommand Comd, InternationalLicenseDTO InternationalLicense)
        {
            if (Comd.CommandText.Contains("@InternationalLicenseID"))
                Comd.Parameters.Add("@InternationalLicenseID", SqlDbType.Int).Value = InternationalLicense.InternationalLicenseID;
            if (Comd.CommandText.Contains("@ApplicationID"))
                Comd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = InternationalLicense.ApplicationID;
            if (Comd.CommandText.Contains("@DriverID"))
                Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = InternationalLicense.DriverID;
            if (Comd.CommandText.Contains("@IssuedUsingLocalLicenseID"))
                Comd.Parameters.Add("@IssuedUsingLocalLicenseID", SqlDbType.Int).Value = InternationalLicense.IssuedUsingLocalLicenseID;
            if (Comd.CommandText.Contains("@IssueDate"))
                Comd.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = InternationalLicense.IssueDate;
            if (Comd.CommandText.Contains("@ExpirationDate"))
                Comd.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = InternationalLicense.ExpirationDate;
            if (Comd.CommandText.Contains("@IsActive"))
                Comd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = InternationalLicense.IsActive;
            if (Comd.CommandText.Contains("@CreatedByUserID"))
                Comd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = InternationalLicense.CreatedByUserID;
        }

        private static async Task<InternationalLicenseDTO> ReadDataAsync(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = await Comd.ExecuteReaderAsync())
                {
                    if (await Reader.ReadAsync())
                    {
                        return new InternationalLicenseDTO
                        {
                            ApplicationID = Reader["ApplicationID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicationID"]) : -1,
                            ApplicantPersonID = Reader["ApplicantPersonID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicantPersonID"]) : -1,
                            ApplicationDate = Reader["ApplicationDate"] != DBNull.Value ? Convert.ToDateTime(Reader["ApplicationDate"]) : DateTime.MinValue,
                            ApplicationTypeID = Reader["ApplicationTypeID"] != DBNull.Value ? Convert.ToInt32(Reader["ApplicationTypeID"]) : -1,
                            Status = Reader["ApplicationStatus"] != DBNull.Value ? Convert.ToByte(Reader["ApplicationStatus"]) : (byte)0,
                            LastStatusDate = Reader["LastStatusDate"] != DBNull.Value ? Convert.ToDateTime(Reader["LastStatusDate"]) : DateTime.MinValue,
                            PaidFees = Reader["PaidFees"] != DBNull.Value ? Convert.ToSingle(Reader["PaidFees"]) : 0,
                            CreatedByUserID = Reader["CreatedByUserID"] != DBNull.Value ? Convert.ToInt32(Reader["CreatedByUserID"]) : -1,
                            InternationalLicenseID = Reader["InternationalLicenseID"] != DBNull.Value ? Convert.ToInt32(Reader["InternationalLicenseID"]) : -1,
                            DriverID = Reader["DriverID"] != DBNull.Value ? Convert.ToInt32(Reader["DriverID"]) : -1,
                            IssuedUsingLocalLicenseID = Reader["IssuedUsingLocalLicenseID"] != DBNull.Value ? Convert.ToInt32(Reader["IssuedUsingLocalLicenseID"]) : -1,
                            IssueDate = Reader["IssueDate"] != DBNull.Value ? Convert.ToDateTime(Reader["IssueDate"]) : DateTime.MinValue,
                            ExpirationDate = Reader["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(Reader["ExpirationDate"]) : DateTime.MinValue,
                            IsActive = Reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(Reader["IsActive"]) : false,
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }
        public static async Task<DataTable> GetAllInternationalLicensesAsync()
        {
            string Query = @"SELECT InternationalLicenseID,
                                ApplicationID, 
                                DriverID,
                                IssuedUsingLocalLicenseID,
                                IssueDate,
                                ExpirationDate,
                                IsActive
                                FROM InternationalLicenses ORDER BY IssueDate DESC";
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

        public static async Task<DataTable> GetDriverLicensesAsync(int DriverID)
        {
            string Query = @"SELECT InternationalLicenseID, ApplicationID,
		                IssuedUsingLocalLicenseID , IssueDate, 
                        ExpirationDate, IsActive FROM InternationalLicenses WHERE DriverID = @DriverID ORDER BY IssueDate DESC";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;

                        return await UtilitiesClass.ExecuteDataTableAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<InternationalLicenseDTO> FindAsync(int InternationalLicenseID)
        {
            string Query = "SELECT * FROM InternationalLicenseFullApplication_View WHERE InternationalLicenseID = @InternationalLicenseID";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@InternationalLicenseID", SqlDbType.Int).Value = InternationalLicenseID;
                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<InternationalLicenseDTO> FindByDriverIDAsync(int DriverID)
        {
            string Query = "SELECT * FROM InternationalLicenseFullApplication_View WHERE DriverID = @DriverID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;

                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }
        public static async Task<InternationalLicenseDTO> GetActiveInternationalLicenseByDriverIDAsync(int DriverID)
        {
            string Query = "SELECT * FROM InternationalLicenseFullApplication_View WHERE DriverID = @DriverID AND IsActive = 1";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@DriverID", SqlDbType.Int).Value = DriverID;

                        return await ReadDataAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        public static async Task<int> AddNewAsync(InternationalLicenseDTO InternationalLicense)
        {
            string Query = @"INSERT INTO InternationalLicenses(ApplicationID,DriverID,IssuedUsingLocalLicenseID,IssueDate,ExpirationDate,IsActive,CreatedByUserID)
                                VALUES (@ApplicationID,@DriverID,@IssuedUsingLocalLicenseID,@IssueDate,@ExpirationDate,@IsActive,@CreatedByUserID)
                                SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, InternationalLicense);
                        return await UtilitiesClass.ExecuteScalarAsync(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        public static async Task<bool> UpdateAsync(InternationalLicenseDTO InternationalLicense)
        {
            string Query = @"UPDATE InternationalLicenses SET ApplicationID = @ApplicationID, DriverID = @DriverID, IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID, IssueDate = @IssueDate, ExpirationDate = @ExpirationDate, IsActive = @IsActive
                            WHERE InternationalLicenseID = @InternationalLicenseID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Conn.OpenAsync();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, InternationalLicense);

                        return (await UtilitiesClass.ExecuteNonQueryAsync(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}