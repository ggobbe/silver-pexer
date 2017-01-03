using System;

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

            using (var pexer = new Pexer(configuration))
            {
                pexer.Login();

                while (true)
                {
                    pexer.KillAllMonsters();
                    pexer.WaitForMonsters();
                }
            }
        }
    }
}