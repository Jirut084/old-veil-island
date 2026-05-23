using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project1.Stage_and_Day
{
    public class TreasureBox
    {
        GameManager gm;
        Texture2D tex;
        public Vector2 Position;
        public bool IsOpened = false;
        float holdProgress = 0f;
        float holdTime = 2f;

        int frameWidth;
        int frameHeight;

        string itemType;
        Texture2D itemTexture;

        public TreasureBox(GameManager gm, Texture2D tex, Vector2 position, string itemType, Texture2D itemTexture)
        {
            this.gm = gm;
            this.tex = tex;
            this.Position = position;
            this.itemType = itemType;
            this.itemTexture = itemTexture;

            frameWidth = tex.Width / 2;
            frameHeight = tex.Height;
        }

        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, (int)(frameWidth * 0.35f), (int)(frameHeight * 0.35f));

        public void Update(Player player, GameTime gt)
        {
            if (IsOpened) return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            // ใช้ player Rect แทน distance
            Rectangle playerRect = new Rectangle((int)player.Position.X - 10, (int)player.Position.Y - 10, 64, 64);

            if (playerRect.Intersects(Hitbox))
            {
                if (InputManager.KeyDown(Keys.E))
                {
                    holdProgress += dt;
                    if (holdProgress >= holdTime) Open();
                }
                else holdProgress = Math.Max(0f, holdProgress - dt);
            }
            else
            {
                holdProgress = Math.Max(0f, holdProgress - dt);
            }
        }

        void Open()
        {
            if (IsOpened) return;
            IsOpened = true;

            Vector2 dropPos = Position + new Vector2(frameWidth / 2 + 8, frameHeight / 4);
            var item = new PickupItem(gm, dropPos, itemTexture, itemType, isNote: false, isAxe: itemType == "axe");

            // เพิ่ม scale ของขวานให้ใหญ่ขึ้น
            if (item.IsAxe)
                item.Scale = new Vector2(2f, 2f); // ขยาย 2 เท่า

            gm.Level.SpawnPickup(item);

            gm.Log($"Treasure opened! {itemType} dropped.");
        }


        public void Draw(SpriteBatch sb, Player player)
        {
            // วาดกล่อง
            int frameIdx = IsOpened ? 1 : 0;
            sb.Draw(tex, Position, new Rectangle(frameIdx * frameWidth, 0, frameWidth, frameHeight),
                Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);

            if (IsOpened) return;

            // ตรวจว่าผู้เล่นอยู่ใกล้
            Rectangle playerRect = new Rectangle((int)player.Position.X - 10, (int)player.Position.Y - 10, 64, 64);
            if (playerRect.Intersects(Hitbox))
            {
                int barWidth = (int)(frameWidth * 0.3f);
                int barHeight = 5;
                Vector2 barPos = Position + new Vector2(0, -10); // วาดเหนือกล่อง

                // Background bar
                sb.Draw(gm.WhitePixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.Gray);

                // Progress bar
                sb.Draw(gm.WhitePixel, new Rectangle((int)barPos.X, (int)barPos.Y, (int)(barWidth * (holdProgress / holdTime)), barHeight), Color.Green);

                // วาดข้อความบอกปุ่ม
                sb.DrawString(gm.DefaultFont, "Hold E to open", new Vector2(Position.X, Position.Y - 25), Color.White);
            }
        }

    }
}
