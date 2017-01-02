using System;

namespace SilverPexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = ReadPassword();

            var pexer = new Pexer();
            pexer.Login(username, password);

            while (true)
            {
                pexer.KillAllMonsters();
                pexer.WaitForMonsters();
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
                password += key.KeyChar;
                Console.Write("*");
            }
            Console.WriteLine();
            return password;
        }
    }
}
