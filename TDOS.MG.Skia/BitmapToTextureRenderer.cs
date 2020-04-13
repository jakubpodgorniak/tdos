using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkiaSharp;

namespace TDOS.MG.Skia
{
    public class BitmapToTextureRenderer : IDisposable
    {
        public BitmapToTextureRenderer(GraphicsDevice graphicsDevice, int width, int height)
            : this(graphicsDevice, SKColors.Transparent, width, height) { }

        public BitmapToTextureRenderer(
            GraphicsDevice graphicsDevice,
            SKColor backgroundColor,
            int width,
            int height)
        {
            this.graphicsDevice = graphicsDevice;
            this.width = width;
            this.height = height;
            this.bitmap = new SKBitmap(width, height);
            this.canvas = new SKCanvas(bitmap);
            Texture = new Texture2D(graphicsDevice, width, height);
            BackgroundColor = backgroundColor;
            Pixels = new Color[width * height];
            drawers = new Dictionary<string, ICanvasDrawer>();
            drawersStatusesById = new Dictionary<string, bool>();
        }

        public Texture2D Texture { get; }

        public SKColor BackgroundColor { get; set; }

        public Color[] Pixels { get; }

        public void AddDrawer(string id, ICanvasDrawer drawer)
        {
            drawers.Add(id, drawer);
            drawersStatusesById.Add(id, true);
        }

        public void EnableDrawer(string id)
        {
            drawersStatusesById[id] = true;
        }

        public void DisableDrawer(string id)
        {
            drawersStatusesById[id] = false;
        }

        public void Render()
        {
            canvas.Clear(BackgroundColor);

            foreach (var drawer in drawers.Where(p => drawersStatusesById[p.Key]).Select(p => p.Value))
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
        private readonly IDictionary<string, ICanvasDrawer> drawers;
        private readonly IDictionary<string, bool> drawersStatusesById;
    }
}
