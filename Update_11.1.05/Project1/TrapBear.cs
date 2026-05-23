using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class TrapBear
    {
        GameManager gm;
        public Vector2 Position;
        Texture2D tex;
        public bool Active = true;
        public bool PlayerCaught = false;

        float escapeTimer = 0;
        int mashCount = 0;
        int mashTarget = 30;

        public int MashCount { get { return mashCount; } }
        public int MashTarget { get { return mashTarget; } }

        public TrapBear(GameManager gm, Vector2 pos, Texture2D tex)
        {
            this.gm = gm; Position = pos; this.tex = tex;
        }

        public void Update(GameTime gt)
        {
            if (PlayerCaught)
            {
                escapeTimer -= (float)gt.ElapsedGameTime.TotalSeconds;

                if (InputManager.KeyPressed(Keys.Q))
                    mashCount++;
                if (InputManager.KeyPressed(Keys.E))
                    mashCount++;

                if (mashCount >= mashTarget)
                    ReleasePlayer();

                if (escapeTimer <= 0)
                {
                    gm.Player.Health -= 20;
                    ReleasePlayer();
                    Active = false;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (Active)
            {
                sb.Draw(tex, Position, null, Color.White, 0f,
                        new Vector2(tex.Width / 2f, tex.Height / 2f),
                        1f, SpriteEffects.None, 0f);
            }

            // วาด HUD เมื่อผู้เล่นติดอยู่
            if (PlayerCaught)
            {
                Vector2 playerPos = gm.Player.Position;
                Vector2 hudPos = playerPos + new Vector2(0, -80); // เหนือหัว player

                string msg = "Press Q & E rapidly to escape";
                var size = gm.DefaultFont.MeasureString(msg);
                sb.DrawString(gm.DefaultFont, msg, hudPos - size / 2, Color.Yellow);



                float progress = (float)mashCount / mashTarget;
                Rectangle bg = new Rectangle((int)hudPos.X - 40, (int)hudPos.Y - 20, 80, 8);
                sb.Draw(gm.WhitePixel, bg, Color.DarkRed);
                sb.Draw(gm.WhitePixel, new Rectangle(bg.X, bg.Y, (int)(bg.Width * progress), bg.Height), Color.LimeGreen);
            }
        }



        public void TriggerOnPlayer(Player p)
        {
            if (!Active || PlayerCaught) return;
            PlayerCaught = true;
            escapeTimer = 6f; // seconds to mash
            mashCount = 0;
            p.Stamina = Math.Max(0, p.Stamina - 30);
            p.IsStuck = true;

            p.TriggerTrapHitSound();
        }

        void ReleasePlayer()
        {
            PlayerCaught = false;
            Active = false;
            gm.Player.IsStuck = false;
        }
    }

}