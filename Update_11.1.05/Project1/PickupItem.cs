using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Project1.NPC;


namespace Project1
{
    public class PickupItem
    {
        GameManager gm;
        public Vector2 Position;
        public Texture2D Texture;
        public string Id;
        public bool PickedUp = false;
        public bool IsKeyItem;

        public bool IsNote = false;
        public bool IsAxe = false;


        public Note NoteRef;

        SoundEffect pickup => gm.PickUp;
        SoundEffectInstance pickupInstance;
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

        //public void OnPickup()
        //{
        //    if (PickedUp) return;

        //    PickedUp = true;

        //    // เล่นเสียงตอนเก็บของ
        //    pickupInstance?.Stop();
        //    pickupInstance = pickup.CreateInstance();
        //    pickupInstance.Volume = 0.7f;
        //    pickupInstance.Play();
        //}

        public Vector2 Scale = Vector2.One; // default 1

        public void Draw(SpriteBatch sb)
        {
            if (PickedUp) return;
            sb.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
