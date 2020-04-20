using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDOS.MG.Skia;
using TDOS.MG.Utils;

namespace TDOS.Game.GameplayLoops.Concrete
{
    public class Client : IGameplayLoop
    {
        public Client()
        {
            outDatagrams = new ConcurrentQueue<byte[]>();
        }

        public void Initialize(
            BitmapToTextureRenderer bitmapToTextureRenderer,
            FrameRateCounter frameRateCounter)
        {
            var udpClient = new UdpClient();
            udpClient.Connect(IPAddress.Parse("127.0.0.1"), 30001);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    while (outDatagrams.TryDequeue(out var datagram))
                    {
                        try
                        {
                            udpClient.Send(datagram, datagram.Length);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());

                            throw;
                        }
                    }
                }
            });
        }

        public void LoadContent(ContentManager Content)
        {
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
            var datagram = pressedKeys.SelectMany(key => BitConverter.GetBytes((int)key)).ToArray();

            if (datagram.Length > 0)
            {
                outDatagrams.Enqueue(datagram);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        private readonly ConcurrentQueue<byte[]> outDatagrams;
    }
}
