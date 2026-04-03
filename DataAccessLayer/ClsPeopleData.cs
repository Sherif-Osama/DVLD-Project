using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class ClsPeopleData
    {
        // Reads a single person's data from a SqlCommand
        static private async Task<PersonDTO> ReadPeopleAsync(SqlCommand command)
        {
            try
            {
                using (SqlDataReader Reader = await command.ExecuteReaderAsync())
                {
                    if (await Reader.ReadAsync())
                    {
                        return new PersonDTO
                        {
                            PersonID = Reader["PersonID"] != DBNull.Value ? Convert.ToInt32(Reader["PersonID"]) : -1,
                            NationalNo = Reader["NationalNo"] != DBNull.Value ? Reader["NationalNo"].ToString() : string.Empty,
                            FirstName = Reader["FirstName"] != DBNull.Value ? Reader["FirstName"].ToString() : string.Empty,
                            SecondName = Reader["SecondName"] != DBNull.Value ? Reader["SecondName"].ToString() : string.Empty,
                            ThirdName = Reader["ThirdName"] != DBNull.Value ? Reader["ThirdName"].ToString() : string.Empty,
                            LastName = Reader["LastName"] != DBNull.Value ? Reader["LastName"].ToString() : string.Empty,
                            DateOfBirth = Reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(Reader["DateOfBirth"]) : DateTime.MinValue,
                            Gender = Reader["Gender"] != DBNull.Value ? Convert.ToInt32(Reader["Gender"]) : -1,
                            Address = Reader["Address"] != DBNull.Value ? Reader["Address"].ToString() : string.Empty,
                            Phone = Reader["Phone"] != DBNull.Value ? Reader["Phone"].ToString() : string.Empty,
                            Email = Reader["Email"] != DBNull.Value ? Reader["Email"].ToString() : string.Empty,
                            NationalityCountryID = Reader["NationalityCountryID"] != DBNull.Value ? Convert.ToInt32(Reader["NationalityCountryID"]) : -1,
                            ImagePath = Reader["ImagePath"] != DBNull.Value ? Reader["ImagePath"].ToString() : string.Empty
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Adds parameters to a SqlCommand for insert/update operations
        static private void SetParameters(SqlCommand Command, PersonDTO PersonInfo)
        {
            if (Command.CommandText.Contains("@ID"))
                Command.Parameters.Add("@ID", SqlDbType.Int).Value = PersonInfo.PersonID;
            if (Command.CommandText.Contains("@NationalNo"))
                Command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = PersonInfo.NationalNo;
            if (Command.CommandText.Contains("@FirstName"))
                Command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = PersonInfo.FirstName;
            if (Command.CommandText.Contains("@SecondName"))
                Command.Parameters.Add("@SecondName", SqlDbType.NVarChar).Value = PersonInfo.SecondName;
            if (Command.CommandText.Contains("@ThirdName"))
                Command.Parameters.Add("@ThirdName", SqlDbType.NVarChar).Value = PersonInfo.ThirdName;
            if (Command.CommandText.Contains("@LastName"))
                Command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = PersonInfo.LastName;
            if (Command.CommandText.Contains("@DateOfBirth"))
                Command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = PersonInfo.DateOfBirth;
            if (Command.CommandText.Contains("@Gender"))
                Command.Parameters.Add("@Gender", SqlDbType.TinyInt).Value = PersonInfo.Gender;
            if (Command.CommandText.Contains("@Address"))
                Command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = PersonInfo.Address;
            if (Command.CommandText.Contains("@Phone"))
                Command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = PersonInfo.Phone;
            if (Command.CommandText.Contains("@Email"))
                Command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = PersonInfo.Email;
            if (Command.CommandText.Contains("@NationalityCountryID"))
                Command.Parameters.Add("@NationalityCountryID", SqlDbType.Int).Value = PersonInfo.NationalityCountryID;
            if (Command.CommandText.Contains("@ImagePath"))
                Command.Parameters.Add("@ImagePath", SqlDbType.NVarChar).Value =
                string.IsNullOrEmpty(PersonInfo.ImagePath) ? (object)DBNull.Value : PersonInfo.ImagePath;
        }

        // Retrieves all people with their basic info and nationality
        static public async Task<DataTable> GetAllPeopleAsync()
        {
            string Query = "SELECT PersonID,NationalNo,FirstName,SecondName,ThirdName,LastName,DateOfBirth," +
                           "CASE WHEN Gender = 0 then 'Male' ELSE 'Female' end as Gender,Phone,Email," +
                           "Countries.CountryName as Nationality from People join Countries on Countries.CountryID = People.NationalityCountryID;";
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

        // Finds a single person by ID
        static public async Task<PersonDTO> FindAsync(int ID)
        {
            string Query = "SELECT * FROM People where PersonID = @ID";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        return await ReadPeopleAsync(Command);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }
        // Finds a single person by National No
        static public async Task<PersonDTO> FindAsync(string NationalNo)
        {
            string Query = "SELECT * FROM People where NationalNo = @NationalNo";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.Add("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;
                        return await ReadPeopleAsync(Command);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Inserts a new person and returns the new ID
        static public async Task<int> AddNewPersonAsync(PersonDTO NewPerson)
        {
            string Query = @"INSERT INTO People(NationalNo,FirstName,SecondName,ThirdName,LastName,DateOfBirth, Gender,Address,Phone,Email,NationalityCountryID,ImagePath)
                             VALUES (@NationalNo,@FirstName,@SecondName,@ThirdName,@LastName,@DateOfBirth, @Gender,@Address,@Phone,@Email,@NationalityCountryID,@ImagePath)
                             SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        SetParameters(Command, NewPerson);

                        return await UtilitiesClass.ExecuteScalarAsync(Command);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        // Updates an existing person
        static public async Task<bool> UpdatePersonInfoAsync(PersonDTO NewPerson)
        {
            string Query = @"UPDATE People
                            SET NationalNo=@NationalNo,FirstName=@FirstName,SecondName=@SecondName,
                                ThirdName=@ThirdName,LastName=@LastName,DateOfBirth=@DateOfBirth,Gender=@Gender,
                                Address=@Address,Phone=@Phone,Email=@Email,NationalityCountryID=@NationalityCountryID,
                                ImagePath=@ImagePath
                            WHERE PersonID= @ID";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        SetParameters(Command, NewPerson);

                        return (await UtilitiesClass.ExecuteNonQueryAsync(Command) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Deletes a person by ID
        public static async Task<bool> DeleteAsync(int PersonID)
        {
            string Query = @"Delete from People where PersonID = @PersonID";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.Add("@PersonID", SqlDbType.Int).Value = PersonID;

                        return (await UtilitiesClass.ExecuteNonQueryAsync(Command) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Checks if a person exists by NationalNo
        static public async Task<bool> PersonExistsAsync(string NationalNo)
        {
            string Query = @"SELECT COUNT(*) FROM People WHERE NationalNo = @NationalNo";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@NationalNo", SqlDbType.NVarChar).Value = NationalNo;

                        return await UtilitiesClass.ExecuteScalarAsync(Command) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }

        // Checks if a person exists by ID
        static public async Task<bool> PersonExistsAsync(int ID)
        {
            string Query = @"SELECT COUNT(*) FROM People WHERE  PersonID = @ID";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.Parameters.AddWithValue("@ID", SqlDbType.Int).Value = ID;

                        return await UtilitiesClass.ExecuteScalarAsync(Command) != -1;
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}