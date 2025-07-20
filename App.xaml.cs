using CAFEHOLIC.DAO;
using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using CAFEHOLIC;

namespace CAFEHOLIC
{
    public partial class App : Application
    {
        public static ILoggerFactory LoggerFactory { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFile("Logs/app-{Date}.txt");
                builder.SetMinimumLevel(LogLevel.Information);
            });

        }
    }
}