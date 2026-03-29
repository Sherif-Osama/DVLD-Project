using System;
using System.IO;

namespace GlobalClasses
{
    public static class ClsLogger
    {
        private static readonly string LogPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");

        public static void Log(Exception ex)
        {
            string logEntry =
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
            File.AppendAllText(LogPath, logEntry);
        }
    }
}