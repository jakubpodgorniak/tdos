using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkiaSharp;

namespace TDOS.MG.Skia
{
    public class BitmapToTextureRenderer : IDisposable
    {
        public BitmapToTextureRenderer(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, SKColors.Transparent) { }

        public BitmapToTextureRenderer(GraphicsDevice graphicsDevice, SKColor backgroundColor)
        {
            this.graphicsDevice = graphicsDevice;
            width = this.graphicsDevice.PresentationParameters.BackBufferWidth;
            height = this.graphicsDevice.PresentationParameters.BackBufferHeight;
            this.bitmap = new SKBitmap(width, height);
            this.canvas = new SKCanvas(bitmap);
            Texture = new Texture2D(graphicsDevice, width, height);
            BackgroundColor = backgroundColor;
            Pixels = new Color[width * height];
            drawers = new List<ICanvasDrawer>();
        }

        public Texture2D Texture { get; }

        public SKColor BackgroundColor { get; set; }

        public Color[] Pixels { get; }

        public void AddDrawer(ICanvasDrawer drawer)
        {
            drawers.Add(drawer);
        }

        public void Render()
        {
            canvas.Clear(BackgroundColor);

            foreach (var drawer in drawers)
            {
                drawer.Draw(canvas);
            }

            SetTexturePixels();
        }

        public void Dispose()
        {
            canvas.Dispose();
        }

        private void SetTexturePixels()
        {
            var bitmapPixels = bitmap.Pixels;
            for (int i = 0; i < Pixels.Length; i++)
            {
                var pixel = bitmapPixels[i];

                Pixels[i] = new Color(pixel.Red, pixel.Green, pixel.Blue, pixel.Alpha);
            }

            Texture.SetData(Pixels);
        }

        private readonly GraphicsDevice graphicsDevice;
        private readonly int width;
        private readonly int height;
        private readonly SKBitmap bitmap;
        private readonly SKCanvas canvas;
        private readonly IList<ICanvasDrawer> drawers;
    }
}
