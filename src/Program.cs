namespace SilverPexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var pexer = new Pexer();
            pexer.Login("USERNAME", "PASSWORD");

            while (true)
            {
                pexer.KillAllMonsters();
                pexer.WaitForMonsters();
            }
        }
    }
}
