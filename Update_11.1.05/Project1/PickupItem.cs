using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.NPC;


namespace Project1
{
    public class PickupItem
    {
        public Vector2 Position;
        public Texture2D Texture;
        public string Id;
        public bool PickedUp = false;
        public bool IsKeyItem;

        public bool IsNote = false;
        public bool IsAxe = false;

        public Note NoteRef;

        public PickupItem(GameManager gm, Vector2 pos, Texture2D tex, string id, bool isKeyItem = false, bool isNote = false, bool isAxe = false, Note noteRef = null)
        {
            Position = pos;
            Texture = tex;
            Id = id;
            IsKeyItem = isKeyItem;
            IsNote = isNote;
            IsAxe = isAxe;
            NoteRef = noteRef;
        }

        public Vector2 Scale = Vector2.One;

        public void Draw(SpriteBatch sb)
        {
            if (PickedUp) return;
            sb.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
