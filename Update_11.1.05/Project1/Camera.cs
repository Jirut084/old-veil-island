using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Project1
{
    public static class Camera
    {
        private static float currentZoom = 1f;
        private static float targetZoom = 1f;
        private static float zoomSpeed = 2f;
        public static Matrix GetTransform(GameRoot root, GameTime gameTime)
        {

            if (root.GM?.Player == null) return Matrix.Identity;

            targetZoom = root.GM.Player.IsStuck ? 1.5f : 1f;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentZoom = MathHelper.Lerp(currentZoom, targetZoom, zoomSpeed * dt);

            float screenW = GameRoot.ScreenWidth;
            float screenH = GameRoot.ScreenHeight;
            float worldLeft, worldTop, worldW, worldH;

            if (root.GM.InBasement)
            {
                worldLeft = root.GM.basement.Bounds.Left;
                worldTop = root.GM.basement.Bounds.Top;
                worldW = root.GM.basement.Bounds.Width;
                worldH = root.GM.basement.Bounds.Height;
            }
            else if (root.GM.InPrepZone)
            {
                worldLeft = root.GM.prepareroom.Bounds.Left;
                worldTop = root.GM.prepareroom.Bounds.Top;
                worldW = root.GM.prepareroom.Bounds.Width;
                worldH = root.GM.prepareroom.Bounds.Height;
            }
            else if (root.GM.InLighthouse)
            {
                worldLeft = root.GM.lighthouse.Bounds.Left;
                worldTop = root.GM.lighthouse.Bounds.Top;
                worldW = root.GM.lighthouse.Bounds.Width;
                worldH = root.GM.lighthouse.Bounds.Height;
            }
            else
            {
                worldLeft = 0;
                worldTop = 0;
                worldW = (int)root.GM.Level.LevelSize.X;
                worldH = (int)root.GM.Level.LevelSize.Y;
            }

            float halfW = screenW / 2f / currentZoom;
            float halfH = screenH / 2f / currentZoom;

            Vector2 p = root.GM.Player.Position;
            float camX = MathHelper.Clamp(p.X, halfW, worldW - halfW);
            float camY = MathHelper.Clamp(p.Y, halfH, worldH - halfH);

            return
                Matrix.CreateTranslation(-camX, -camY, 0f) *
                Matrix.CreateScale(currentZoom) *
                Matrix.CreateTranslation(screenW / 2f, screenH / 2f, 0f);
        }
    }
}
