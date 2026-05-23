using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class Enemy2 : Enemy
    {
        private float scale = 0.3f;
        private Texture2D whitePixel;

        public Enemy2(GameManager gm, Vector2 pos, Texture2D tex)
            : base(gm, pos, tex, 667, 750)
        {
            frameCount = 6;
            frameWidth = 667;
            frameHeight = 750;
            frameTime = 0.8;
            whitePixel = gm.WhitePixel;

            // ปรับค่าเฉพาะ Enemy2 (ชื่อเหมือน Enemy1)
            speed = 170f;
            health = 50;
            damage = 10;
        }

        public override Rectangle Hitbox
        {
            get
            {
                float collisionScale = 0.2f; // ทำให้ hitbox เล็กลงมาก
                int hbWidth = (int)(frameWidth * collisionScale);
                int hbHeight = (int)(frameHeight * collisionScale);

                return new Rectangle(
                    (int)(Position.X - hbWidth / 2f),
                    (int)(Position.Y - hbHeight / 2f),
                    hbWidth,
                    hbHeight
                );
            }
        }

        public override void Update(GameTime gt)
        {
            float prevAttackTimer = attackTimer;
            base.Update(gt);

            // base.Update โจมตีไปแล้ว (attackTimer ถูก reset เป็น attackCooldown) → เพิ่ม debuff
            if (prevAttackTimer <= 0f && attackTimer > 0f)
            {
                gm.Player.speedDebuffTimer = 3f;
            }
        }


        public override void Draw(SpriteBatch sb)
        {
            var color = WasHit ? Color.Red : Color.White;
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            sb.Draw(tex, Position, sourceRect, color, 0f,
                    new Vector2(frameWidth / 2f, frameHeight / 2f),
                    scale, SpriteEffects.None, 0f);

            // วาด hitbox debug
            //Rectangle hb = Hitbox;
            //sb.Draw(whitePixel, new Rectangle(hb.X, hb.Y, hb.Width, 2), Color.Red);
            //sb.Draw(whitePixel, new Rectangle(hb.X, hb.Y + hb.Height - 2, hb.Width, 2), Color.Red);
            //sb.Draw(whitePixel, new Rectangle(hb.X, hb.Y, 2, hb.Height), Color.Red);
            //sb.Draw(whitePixel, new Rectangle(hb.X + hb.Width - 2, hb.Y, 2, hb.Height), Color.Red);
        }
    }
}
