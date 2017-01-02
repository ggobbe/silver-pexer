using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SilverPexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string username, password;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            Console.Write("Username: ");
            if (!string.IsNullOrWhiteSpace(configuration["username"]))
            {
                Console.WriteLine("[Read from appsettings.json]");
                username = configuration["username"];
            }
            else
            {
                username = Console.ReadLine();
            }

            Console.Write("Password: ");
            if (!string.IsNullOrWhiteSpace(configuration["password"]))
            {
                Console.WriteLine("[Read from appsettings.json]");
                password = configuration["password"];
            }
            else
            {
                password = ReadPassword();
            }

            using (var pexer = new Pexer(configuration["pathToInn"].Split(',')))
            {
                pexer.Login(username, password);

                while (true)
                {
                    pexer.KillAllMonsters();
                    pexer.WaitForMonsters();
                }
            }
        }

        private static string ReadPassword()
        {
            string password = null;
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    Console.WriteLine("\nBackspace is not supported, try again.");
                    Console.Write("Password: ");
                    return ReadPassword();
                }

                password += key.KeyChar;
                Console.Write("*");
            }
            Console.WriteLine();
            return password;
        }
    }
}
