using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using OpenQA.Selenium.Chrome;

namespace SilverPexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddNLog();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            try
            {
                var configuration = new Configuration(logger);

                if (string.IsNullOrWhiteSpace(configuration.Username))
                {
                    Console.Write("Username: ");
                    configuration.Username = Console.ReadLine();
                }

                if (string.IsNullOrWhiteSpace(configuration.Password))
                {
                    Console.Write("Password: ");
                    configuration.Password = ConsoleHelper.ReadPassword();
                }

                string driverOSFolder = GetDriverOSFolder();
                using (var driver = new ChromeDriver(Path.Combine(System.AppContext.BaseDirectory, "drivers", driverOSFolder)))
                {
                    var pexer = new Pexer(configuration, driver, logger);

                    pexer.Login();
                    while (pexer.Continue)
                    {
                        pexer.KillAllMonsters();
                        pexer.WaitForMonsters();
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogCritical($"An error has occured: {e.Message}\n{e.StackTrace}");
            }
        }

        private static string GetDriverOSFolder()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "windows";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }
            throw new NotSupportedException("Operating system not supported");
        }
    }
}