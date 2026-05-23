using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class Projectile
    {
        private GameManager gm;          // เก็บ reference ของ GameManager
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public int Width, Height;
        public int Damage;
        public bool Remove = false;

        public Projectile(GameManager gm, Vector2 pos, Vector2 vel, Texture2D tex, int w, int h, int dmg)
        {
            this.gm = gm ?? throw new ArgumentNullException(nameof(gm)); // ป้องกัน null
            Position = pos;
            Velocity = vel;
            Texture = tex ?? throw new ArgumentNullException(nameof(tex)); // ป้องกัน null
            Width = w;
            Height = h;
            Damage = dmg;
        }

        public void Update(GameTime gt)
        {
            if (gm?.Level == null) return; // ป้องกัน Null Reference

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            Position += Velocity * dt;

            // Remove ถ้าออกนอกขอบเขต
            if (Position.X < 0 || Position.Y < 0 ||
                Position.X > gm.Level.LevelSize.X || Position.Y > gm.Level.LevelSize.Y)
            {
                Remove = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (Texture == null) return; // ป้องกัน Null Reference
            sb.Draw(Texture, Position, null, Color.White, 0f,
                    new Vector2(Width / 2f, Height / 2f), 1f, SpriteEffects.None, 0f);
        }
    }
}
