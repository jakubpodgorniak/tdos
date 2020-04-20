using System.IO;
using Newtonsoft.Json;
using TDOS.Game.GameplayLoops;
using TDOS.Game.GameplayLoops.Concrete;

namespace TDOS.Game
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = LoadConfiguration();
            var gameplayLoop = args.Length > 0
                ? CreateGameplayLoop(args[0], configuration)
                : CreateGameplayLoop("standalone", configuration);

            using (var game = new TDOSGame(gameplayLoop))
            {
                game.Run();
            }
        }

        private static Configuration.Configuration LoadConfiguration()
            => JsonConvert.DeserializeObject<Configuration.Configuration>(
                File.ReadAllText(@"Resources\Configuration.json"));

        private static IGameplayLoop CreateGameplayLoop(string loopName, Configuration.Configuration configuration)
            => loopName switch
            {
                "server" => new Server(configuration),
                "client" => new Client(),
                _ => new Standalone(configuration)
            };
    }
}
