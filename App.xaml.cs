using CAFEHOLIC;
using CAFEHOLIC.dao;
using CAFEHOLIC.DAO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Windows;
using CAFEHOLIC;

namespace CAFEHOLIC
{
    public partial class App : Application
    {
        public static ILoggerFactory LoggerFactory { get; private set; }

        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                Configuration = new ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFile("Logs/app-{Date}.txt");
                builder.SetMinimumLevel(LogLevel.Information);
            });
                LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                });
                System.Diagnostics.Debug.WriteLine("[App] Configuration and LoggerFactory initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Error initializing Configuration: {ex.Message}, InnerException: {ex.InnerException?.Message}, StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Lỗi khi khởi tạo cấu hình: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}