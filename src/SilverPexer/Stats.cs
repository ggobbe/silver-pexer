namespace SilverPexer
{
    public class Stats
    {
        public int Constitution { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        public int Total => this.Constitution + this.Strength + this.Agility + this.Intelligence;
    }
}