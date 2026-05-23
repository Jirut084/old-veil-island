using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Project1
{
    public class Minimap
    {
        GameManager gm;
        Texture2D blank;
        public Minimap(GameManager gm)
        {
            this.gm = gm;
            blank = new Texture2D(gm.root.GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });
        }


        public void Update(GameTime gt) { }


        public void Draw(SpriteBatch sb)
        {
            // top-right small map
            var size = new Rectangle(GameRoot.ScreenWidth - 210, 10, 200, 120);
            sb.Draw(blank, size, Color.Black * 0.6f);
            // draw player dot
            var rel = gm.Player.Position / gm.Level.LevelSize;
            var dot = new Vector2(size.X + rel.X * size.Width, size.Y + rel.Y * size.Height);
            sb.Draw(blank, new Rectangle((int)dot.X - 3, (int)dot.Y - 3, 6, 6), Color.Green);
            // draw key parts
            foreach (var p in gm.Level.GetPickupPositions())
            {
                var r = p / gm.Level.LevelSize;
                var pos = new Vector2(size.X + r.X * size.Width, size.Y + r.Y * size.Height);
                sb.Draw(blank, new Rectangle((int)pos.X - 3, (int)pos.Y - 3, 6, 6), Color.Orange);
            }
        }
    }
}
