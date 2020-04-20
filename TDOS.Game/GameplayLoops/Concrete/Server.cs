using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDOS.Box2D.Skia;
using TDOS.Game.Bootstrapping;
using TDOS.Game.Characters;
using TDOS.Game.Configuration.Helpers;
using TDOS.Game.Rendering;
using TDOS.Game.Rendering.Drawers;
using TDOS.Game.Resources;
using TDOS.MG.Skia;
using TDOS.MG.Utils;

namespace TDOS.Game.GameplayLoops.Concrete
{
    public class Server : IGameplayLoop
    {
        public Server(Configuration.Configuration configuration)
        {
            this.configuration = configuration;
            bodySprites = new List<BodySprite>();
            datagramQueue = new ConcurrentQueue<byte[]>();
        }

        public void Initialize(
            BitmapToTextureRenderer bitmapToTextureRenderer,
            FrameRateCounter frameRateCounter)
        {
            world = TemporaryBox2DWorldBootstrapper.CreateWorld();
            var heroBody = TemporaryBox2DWorldBootstrapper.CreateHeroBody(world);
            hero = new Character(heroBody)
            {
                Acceleration = 80f,
                MaxSpeed = 8f
            };

            messageQueueDrawer = new MessageQueueDrawer(5);
            bitmapToTextureRenderer.AddDrawer(Drawers.Colliders, new WorldDrawer(world, Constants.PixelsPerUnit));
            bitmapToTextureRenderer.AddDrawer(Drawers.BodiesStatus, new BodiesStatusDrawer(world, Constants.PixelsPerUnit) { DisplayVelocity = true });
            bitmapToTextureRenderer.AddDrawer(Drawers.FpsCounter, new FrameRateCounterDrawer(frameRateCounter));
            bitmapToTextureRenderer.AddDrawer(Drawers.MessageQueue, messageQueueDrawer);

            var visiblityController = new DebugInterfaceVisibilityController(bitmapToTextureRenderer);
            visiblityController.RefreshVisibility(configuration.DebugInterface);

            var configurationWatcher = new ConfigurationWatcher(
                @".\Resources\Configuration.json",
                config => visiblityController.RefreshVisibility(config.DebugInterface));

            var udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, 30001));
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var result = await udpServer.ReceiveAsync().ConfigureAwait(false);

                    datagramQueue.Enqueue(result.Buffer);
                }
            });
        }

        public void LoadContent(ContentManager Content)
        {
            bodySprites.Add(new BodySprite(
              Content.Load<Texture2D>("hero"),
              hero.Body,
              Constants.PixelsPerUnit,
              new Vector2(18, 22)));
        }

        public void UnloadContent() { }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (datagramQueue.TryDequeue(out var datagram))
            {
                messageQueueDrawer.AddMessage($"{DateTime.UtcNow:HH:mm:ss:ffff} - datagram received");

                var keys = ReadDatagramKeys(datagram);

                var activeDirections = new List<MoveDirection>();
                if (keys.Contains(Keys.W))
                {
                    activeDirections.Add(MoveDirection.Up);
                }

                if (keys.Contains(Keys.D))
                {
                    activeDirections.Add(MoveDirection.Right);
                }

                if (keys.Contains(Keys.S))
                {
                    activeDirections.Add(MoveDirection.Down);
                }

                if (keys.Contains(Keys.A))
                {
                    activeDirections.Add(MoveDirection.Left);
                }

                hero.UpdatePosition(activeDirections.ToArray());
            }

            world.Step(deltaTime, 6, 2);
        }

        private IEnumerable<Keys> ReadDatagramKeys(byte[] datagram)
        {
            var keysCount = datagram.Length / sizeof(int);

            for (int i = 0; i < keysCount; i++)
            {
                yield return (Keys)BitConverter.ToInt32(datagram, i * sizeof(int));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bodySprite in bodySprites.Reverse())
            {
                bodySprite.Render(spriteBatch);
            }
        }

        private readonly Configuration.Configuration configuration;

        private readonly IList<BodySprite> bodySprites;

        private readonly ConcurrentQueue<byte[]> datagramQueue;

        private World world;

        private Character hero;

        private MessageQueueDrawer messageQueueDrawer;
    }
}
