using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class PrepRoom
    {
        GameManager gm;
        public Vector2 LevelSize = new Vector2(1500, 850);
        //public Vector2 PlayerStart = new Vector2(1200 / 2, 768 / 2);
        public Texture2D FloorTexture;
        public Rectangle Bounds = new Rectangle(0, 0, 1500, 850);

        // Door trigger to the main level
        public Rectangle ExitDoor = new Rectangle(-15, 240, 50, 100); // right side door
        public Rectangle BaseDoor = new Rectangle(1495, 240, 50, 100); // right side door

        public Vector2 SpawnFromBasement = new Vector2(1450, 240);    // spawn ถ้ามาจาก Basement

        public PrepRoom(GameManager gm, Texture2D floorTexture)
        {
            this.gm = gm;
            this.FloorTexture = floorTexture;
        }
        public void Update(GameTime gt)
        {
            Rectangle floor = Bounds;
            Vector2 pos = gm.Player.Position;
            int playerW = 32;
            int playerH = 64;

            int marginX = playerW;  // same as width
            int marginY = playerH;  // same as height

            pos.X = MathHelper.Clamp(pos.X, floor.Left + marginX, floor.Right - marginX);
            pos.Y = MathHelper.Clamp(pos.Y, floor.Top + marginY, floor.Bottom - marginY);

            gm.Player.Position = pos;

            Rectangle playerRect = new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, playerW, playerH);



            // Check if player reached door
            if (ExitDoor.Intersects(new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, 32, 64)))
            {
                //gm.StartMainLevel(); // function we’ll add in GameManager
                gm.fader.FadingOut = true;
                gm.pendingSceneChangeToMain = true;          
            }
            if (BaseDoor.Intersects(new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, 32, 64)))
            {
                //gm.StartMainLevel(); // function we’ll add in GameManager
                gm.fader.FadingOut = true;
                //gm.InBasement = false;
                gm.pendingSceneChangeToBase = true;
            }
        }
        //public Vector2 GetStartPoint() => new Vector2(200, 300);
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(FloorTexture, Bounds, Color.White);
            //sb.Draw(gm.WhitePixel, BaseDoor, Color.Gray);
        }
    }
}