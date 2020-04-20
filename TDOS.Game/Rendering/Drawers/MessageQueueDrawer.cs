using System.Collections.Generic;
using SkiaSharp;
using TDOS.MG.Skia;

namespace TDOS.Game.Rendering.Drawers
{
    public class MessageQueueDrawer : ICanvasDrawer
    {
        public MessageQueueDrawer(int capacity)
        {
            messagesQueue = new Queue<string>();
            this.capacity = capacity;
        }

        public void AddMessage(string message)
        {
            if (messagesQueue.Count >= capacity)
            {
                messagesQueue.Dequeue();
            }

            messagesQueue.Enqueue(message);
        }

        public void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;

                var messages = messagesQueue.ToArray();

                for (int i = 0; i < messages.Length; i++)
                {
                    float y = 20f + (i * 20f);

                    canvas.DrawText(
                        messages[i],
                        new SKPoint(20f, y),
                        paint);
                }
            }
        }

        private readonly Queue<string> messagesQueue;

        private readonly int capacity;
    }
}
