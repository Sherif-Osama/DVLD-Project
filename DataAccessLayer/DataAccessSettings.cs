using System.Configuration;
namespace DataAccessLayer
{
    // Central class for database connection Settings
    internal static class DataAccessSettings
    {
        // SQL Server connection string
        // Static so it can be used anywhere in the DAL without creating an instance
        //static public string ConnectionString = "Server=.;Database=DVLD;User Id=sa;Password=123456";

        static public string ConnectionString { get => ConfigurationManager.ConnectionStrings["DVLDConnectionStrings"].ConnectionString; }
    }
}
