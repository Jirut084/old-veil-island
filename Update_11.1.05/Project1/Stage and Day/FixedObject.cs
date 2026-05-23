// FixedObject.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class FixedObject
    {
        public Vector2 Position;
        public Texture2D Texture;
        public Rectangle Hitbox;

        public float ScaleX = 1f;
        public float ScaleY = 1f;

        public FixedObject(Vector2 pos, Texture2D tex)
        {
            Position = pos;
            Texture = tex;
            Hitbox = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
        }

        // constructor แบบมี scale
        public FixedObject(Vector2 pos, Texture2D tex, Vector2 scale)
        {
            Position = pos;
            Texture = tex;
            ScaleX = scale.X;
            ScaleY = scale.Y;
            Hitbox = new Rectangle((int)pos.X, (int)pos.Y, (int)(tex.Width * scale.X), (int)(tex.Height * scale.Y));
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, new Vector2(ScaleX, ScaleY), SpriteEffects.None, 0f);
        }
    }

}
