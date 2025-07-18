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
                // Đảm bảo log nằm trong thư mục bin/Debug/net8.0-windows/logs
                string baseDir = AppContext.BaseDirectory;
                logDirectory = Path.Combine(baseDir, "logs");

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    Console.WriteLine("Đã tạo thư mục log tại: " + logDirectory);
                }

                logFilePath = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");
                Console.WriteLine("Ghi log vào: " + logFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể khởi tạo Logger: " + ex.Message);
            }
        }

        public static void Info(string message) => WriteLog("INFO", message);

        public static void Warn(string message) => WriteLog("WARN", message);

        public static void Error(string message, Exception ex = null)
        {
            var fullMessage = ex == null ? message : $"{message}\nException: {ex}";
            WriteLog("ERROR", fullMessage);
        }

        private static void WriteLog(string level, string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

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

                // In ra Output Debug luôn
                Console.WriteLine(logEntry);
            }
        }
    }
}
