using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public static class ClsUserData
    {
        // Reads a single user record from the database and fills the User DTO
        private static UserDTO ReadUser(SqlCommand command)
        {
            using (SqlDataReader Reader = command.ExecuteReader())
            {
                try
                {
                    if (Reader.Read()) // Move to the first record
                    {
                        return new UserDTO()
                        {
                            UserID = Reader["UserID"] != DBNull.Value ? Convert.ToInt32(Reader["UserID"]) : -1,
                            PersonID = Reader["PersonID"] != DBNull.Value ? Convert.ToInt32(Reader["PersonID"]) : -1,
                            UserName = Reader["UserName"] != DBNull.Value ? Reader["UserName"].ToString() : string.Empty,
                            Password = Reader["Password"] != DBNull.Value ? Reader["Password"].ToString() : string.Empty,
                            IsActive = Reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(Reader["IsActive"]) : false,
                        };
                    }
                }
                catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
                return null;
            }
        }

        // Sets SQL parameters for the command based on the User DTO
        private static void SetParameters(SqlCommand Command, UserDTO User)
        {
            try
            {
                if (Command.CommandText.Contains("@UserID"))
                    Command.Parameters.Add("@UserID", SqlDbType.Int).Value = User.UserID;
                if (Command.CommandText.Contains("@PersonID"))
                    Command.Parameters.Add("@PersonID", SqlDbType.Int).Value = User.PersonID;
                if (Command.CommandText.Contains("@Password"))
                    Command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = User.Password;
                if (Command.CommandText.Contains("@UserName"))
                    Command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = User.UserName;
                if (Command.CommandText.Contains("@IsActive"))
                    Command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = User.IsActive;
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); }
        }

        // Retrieves all users joined with their full names from People table
        public static DataTable GetAllUsersData()
        {
            string Query = @"SELECT 
                            Users.UserID, 
                            Users.PersonID,
                            (People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) AS FullName, 
                            UserName, 
                            IsActive 
                            FROM Users 
                            INNER JOIN People ON People.PersonID = Users.PersonID";
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

        // Checks if a user exists by username only
        public static bool UserExists(string UserName)
        {
            string Query = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;

                        return UtilitiesClass.ExecuteScalar(Comd) != -1 ? true : false;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Checks if a given person is already a user
        public static bool IsUser(int PersonID)
        {
            string Query = "SELECT COUNT(*) FROM Users WHERE PersonID = @PersonID;";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        // Use parameters to avoid SQL injection
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        return UtilitiesClass.ExecuteScalar(Comd) != -1 ? true : false;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Finds a user by PersonID
        public static UserDTO FindUserByPersonID(int PersonID)
        {
            UserDTO User = new UserDTO();
            string Query = @"SELECT 
                            UserID, 
                            Password,
                            PersonID, 
                            UserName, 
                            IsActive 
                            FROM Users WHERE PersonID = @PersonID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        return ReadUser(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Finds a user by User ID
        public static UserDTO Find(int UserID)
        {
            UserDTO User = new UserDTO();
            string Query = @"SELECT 
                            UserID, 
                            Password,
                            PersonID, 
                            UserName, 
                            IsActive 
                            FROM Users WHERE UserID = @UserID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        return ReadUser(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Finds a user by UserName
        public static UserDTO Find(string UserName)
        {
            UserDTO User = new UserDTO();
            string Query = @"SELECT 
                            UserID, 
                            Password,
                            PersonID, 
                            UserName, 
                            IsActive 
                            FROM Users WHERE UserName = @UserName";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;

                        return ReadUser(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }

        }

        // Finds a user by both UserName and Password
        public static UserDTO Find(string UserName, string Password)
        {
            string Query = @"SELECT * from Users where UserName = @UserName and Password = @Password";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = UserName;
                        Comd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password;

                        return ReadUser(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Adds a new user and returns its generated UserID
        public static int AddNewUser(UserDTO User)
        {
            string Query = @"INSERT INTO Users (PersonID,UserName,Password,IsActive)
                            VALUES
                             (@PersonID,@UserName,@Password,@IsActive)
                               SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, User);

                        return UtilitiesClass.ExecuteScalar(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        // Updates an existing user record
        public static bool UpdateUser(UserDTO User)
        {
            string Query = @"UPDATE Users
                                SET 
                                PersonID = @PersonID,
                                UserName = @UserName,
                                IsActive = @IsActive
                                WHERE UserID = @UserID;";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        SetParameters(Comd, User);
                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Deletes a user by UserID
        public static bool DeleteUser(int UserID)
        {
            string Query = @"DELETE FROM Users WHERE UserID = @UserID";

            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        public static bool ChangePassword(int UserID, string NewPassWord)
        {
            string Query = @"update Users 
                            SET 
                            Password = @NewPassWord
                            WHERE UserID = @UserID;";
            try
            {
                using (SqlConnection Conn = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Conn.Open();

                    using (SqlCommand Comd = new SqlCommand(Query, Conn))
                    {
                        Comd.Parameters.Add("@NewPassWord", SqlDbType.NVarChar).Value = NewPassWord;
                        Comd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}