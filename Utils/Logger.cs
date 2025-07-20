using System;
using System.IO;

namespace CAFEHOLIC.Utils
{
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string logDirectory;
        private static readonly string logFilePath;

        static Logger()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                logDirectory = Path.Combine(baseDir, "logs");

                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }

<<<<<<< HEAD
                logFilePath = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");
=======
                logFilePath = Path.Combine(logDirectory, $"LOG_{DateTime.Now:dd/MM/yyyy}.txt");
>>>>>>> origin/develop
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể khởi tạo Logger: " + ex.Message);
            }
        }

        public static void Info(string className, string message) => WriteLog("INFO", className, message);
        public static void Warn(string className, string message) => WriteLog("WARN", className, message);
        public static void Error(string className, string message, Exception ex = null)
        {
            var fullMessage = ex == null ? message : $"{message}\nException: {ex}";
            WriteLog("ERROR", className, fullMessage);
        }

        private static void WriteLog(string level, string className, string message)
        {
<<<<<<< HEAD
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{className}] {message}";
=======
            string logEntry = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] [{level}] [{className}] {message}";
>>>>>>> origin/develop

            lock (_lock)
            {
                try
                {
                    File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Không thể ghi file log: " + ex.Message);
                    Console.WriteLine("Log lỗi: " + logEntry);
                }

                Console.WriteLine(logEntry);
            }
        }

    }
}
