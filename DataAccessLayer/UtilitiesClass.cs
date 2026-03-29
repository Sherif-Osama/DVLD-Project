using DataAccessLayer.LogErorr;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    // Utility class for SQL command execution.
    internal static class UtilitiesClass
    {
        // Execute query and return results as DataTable.
        public static DataTable ExecuteDataTable(SqlCommand Command)
        {
            DataTable dT = new DataTable();
            try
            {
                using (SqlDataReader Reader = Command.ExecuteReader())
                {
                    if (Reader.HasRows) { dT.Load(Reader); return dT; }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }

        // Execute scalar query and return integer result (or -1 if invalid).
        public static int ExecuteScalar(SqlCommand Command)
        {
            object Result = Command.ExecuteScalar();

            if ((Result != null && int.TryParse(Result.ToString(), out int values) && values > 0))
            { return values; }
            else
            { return -1; }
        }

        // Execute INSERT/UPDATE/DELETE and return affected row count (or -1 on error).
        public static int ExecuteNonQuery(SqlCommand Command)
        {
            try
            {
                int Value = Command.ExecuteNonQuery();
                return Value;
            }
            catch (Exception Ex)
            { ClsLogger.Log(Ex); return 0; }
        }
    }
}