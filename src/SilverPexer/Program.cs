using System;
using System.IO;
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
                .AddDebug()
                .AddNLog();
            ILogger logger = loggerFactory.CreateLogger<Program>();

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

            using (var driver = new ChromeDriver(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "drivers")))
            {
                var pexer = new Pexer(configuration, driver);

                pexer.Login();
                while (pexer.Continue)
                {
                    pexer.KillAllMonsters();
                    pexer.WaitForMonsters();
                }
            }
        }
    }
}