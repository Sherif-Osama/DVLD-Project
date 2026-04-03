using DataAccessLayer.LogErorr;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    // Utility class for SQL command execution.
    internal static class UtilitiesClass
    {
        // Execute query and return results as DataTable.
        public static async Task<DataTable> ExecuteDataTableAsync(SqlCommand Command)
        {
            DataTable dT = new DataTable();
            try
            {
                using (SqlDataReader Reader = await Command.ExecuteReaderAsync())
                {
                    if (Reader.HasRows) { dT.Load(Reader); return dT; }
                }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return null; }
            return null;
        }
        /// <summary>
        /// Executes a scalar SQL command asynchronously and returns a positive integer result.
        /// Returns -1 if the result is null, not a valid integer, or less than or equal to zero.
        /// Intended mainly for scenarios like retrieving newly generated IDs (e.g., SCOPE_IDENTITY).
        /// </summary>
        /// <param name="Command">The SQL command to execute.</param>
        /// <returns>
        /// A positive integer result if successful; otherwise, -1 to indicate failure or invalid result.
        /// </returns>
        public static async Task<int> ExecuteScalarAsync(SqlCommand Command)
        {
            try
            {
                object Result = await Command.ExecuteScalarAsync();

                if ((Result != null && int.TryParse(Result.ToString(), out int values) && values > 0))
                { return values; }
                else
                { return -1; }
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return -1; }
        }

        // Execute INSERT/UPDATE/DELETE and return affected row count (or -1 on error).
        public static async Task<int> ExecuteNonQueryAsync(SqlCommand Command)
        {
            try
            { return await Command.ExecuteNonQueryAsync(); }
            catch (Exception Ex)
            { ClsLogger.Log(Ex); return 0; }
        }
    }
}