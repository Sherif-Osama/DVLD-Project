using DataAccessLayer.LogErorr;
using DTO;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public static class ClsTestTypesData
    {
        // Reads a single TestType record from the database and converts it to a DTO object
        private static TestTypesDTO ReadTestType(SqlCommand Comd)
        {
            try
            {
                using (SqlDataReader Reader = Comd.ExecuteReader())
                {
                    if (Reader.Read())
                    {
                        return new TestTypesDTO
                        {
                            TestTypeID = Reader["TestTypeID"] != DBNull.Value ? Convert.ToInt32(Reader["TestTypeID"]) : -1,
                            Title = Reader["TestTypeTitle"] != DBNull.Value ? Reader["TestTypeTitle"].ToString() : string.Empty,
                            Description = Reader["TestTypeDescription"] != DBNull.Value ? Reader["TestTypeDescription"].ToString() : string.Empty,
                            Fees = Reader["TestTypeFees"] != DBNull.Value ? Convert.ToSingle(Reader["TestTypeFees"]) : -1,
                        };
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); }

            return null;
        }

        // Sets SQL parameters for a command based on DTO values
        private static void SetParameters(SqlCommand Command, TestTypesDTO TestTypesDTO)
        {
            if (Command.CommandText.Contains("@TestTypeID"))
                Command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypesDTO.TestTypeID;
            if (Command.CommandText.Contains("@TestTypeTitle"))
                Command.Parameters.Add("@TestTypeTitle", SqlDbType.NVarChar).Value = TestTypesDTO.Title;
            if (Command.CommandText.Contains("@TestTypeFees"))
                Command.Parameters.Add("@TestTypeFees", SqlDbType.SmallMoney).Value = TestTypesDTO.Fees;
            if (Command.CommandText.Contains("@TestTypeDescription"))
                Command.Parameters.Add("@TestTypeDescription", SqlDbType.NVarChar).Value = TestTypesDTO.Description;
        }

        // Retrieves all test types from the database
        public static DataTable GetAllTestType()
        {
            string Query = "SELECT * FROM TestTypes";
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

        // Finds a test type by its ID
        public static TestTypesDTO Find(int TestTypeID)
        {
            string Query = "SELECT * FROM TestTypes WHERE TestTypeID = @TestTypeID";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Connection))
                    {
                        Comd.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = TestTypeID;
                        return ReadTestType(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Finds a test type by its title
        public static TestTypesDTO Find(string TestTypeTitle)
        {
            string Query = "SELECT * FROM TestTypes WHERE TestTypeTitle = @TestTypeTitle";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Connection))
                    {
                        Comd.Parameters.Add("@TestTypeTitle", SqlDbType.NVarChar).Value = TestTypeTitle;
                        return ReadTestType(Comd);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
        }

        // Updates an existing test type record in the database
        public static bool Update(TestTypesDTO TestType)
        {
            string Query = @"UPDATE TestTypes
                            SET 
                            TestTypeTitle = @TestTypeTitle,
                            TestTypeDescription = @TestTypeDescription,
                            TestTypeFees = @TestTypeFees
                            WHERE TestTypes.TestTypeID = @TestTypeID";
            try
            {
                using (SqlConnection Connection = new SqlConnection(DataAccessSettings.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Comd = new SqlCommand(Query, Connection))
                    {
                        SetParameters(Comd, TestType);

                        return (UtilitiesClass.ExecuteNonQuery(Comd) > 0);
                    }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
        }
    }
}