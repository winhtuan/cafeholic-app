using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CAFEHOLIC.Utils
{
    public static class LoggerBookingManager
    {
        private static readonly object _lock = new object();
        private static readonly string logDirectory;
        private static readonly string logFilePath;

        static LoggerBookingManager()
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                logDirectory = Path.Combine(baseDir, "logs");

                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                logFilePath = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");

                // Không nên xóa file mỗi lần chạy → giữ log theo ngày
                // File.Delete(logFilePath); ❌ bỏ dòng này đi
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
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{className}] {message}";

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

                Console.WriteLine(logEntry); // Console cho dev thấy luôn
            }
        }
    }
}
