using System;
using System.Diagnostics;

namespace DataAccessLayer.LogErorr
{
    internal static class ClsLogger
    {
        private const string source = "DVLD_APP";
        //private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
        static ClsLogger()
        {
            if (!EventLog.SourceExists(source))
            { EventLog.CreateEventSource(source, "Application"); }
        }

        public static void Log(Exception ex)
        {
            EventLog.WriteEntry(source, ex.ToString(), EventLogEntryType.Error);
            //string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
            //File.AppendAllText(LogPath, logEntry);
        }
    }
}