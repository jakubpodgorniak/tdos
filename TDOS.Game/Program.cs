namespace TDOS.Game
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new TDOSGame())
            {
                game.Run();
            }
        }
    }
}
