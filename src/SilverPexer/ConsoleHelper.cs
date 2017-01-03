using System;

namespace SilverPexer
{
    public class ConsoleHelper
    {
        public static string ReadPassword()
        {
            string password = null;
            while (true)
            {
                var key = Console.ReadKey(true);
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