using System;
using System.IO;
using OpenQA.Selenium.Chrome;

namespace SilverPexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new Configuration();

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