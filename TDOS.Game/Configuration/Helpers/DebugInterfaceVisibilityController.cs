using TDOS.Game.Resources;
using TDOS.MG.Skia;

namespace TDOS.Game.Configuration.Helpers
{
    public class DebugInterfaceVisibilityController
    {
        public DebugInterfaceVisibilityController(BitmapToTextureRenderer bitmapToTextureRenderer)
        {
            this.bitmapToTextureRenderer = bitmapToTextureRenderer;
        }

        public void RefreshVisibility(DebugInterfaceConfiguration config)
        {
            RefreshVisibility(Drawers.FpsCounter, config.FpsCounterVisible);
            RefreshVisibility(Drawers.Colliders, config.CollidersDrawing);
            RefreshVisibility(Drawers.BodiesStatus, config.BodiesPositionDrawing);
        }

        private void RefreshVisibility(string drawerId, bool isVisible)
        {
            if (isVisible)
            {
                bitmapToTextureRenderer.EnableDrawer(drawerId);
            }
            else
            {
                bitmapToTextureRenderer.DisableDrawer(drawerId);
            }
        }

        private readonly BitmapToTextureRenderer bitmapToTextureRenderer;
    }
}
