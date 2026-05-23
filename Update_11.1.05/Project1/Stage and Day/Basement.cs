using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class Basement
    {
        GameManager gm;
        public Vector2 PlayerStart = new Vector2(1900 / 2 , 1350 / 2);
        public Texture2D FloorTexture;
        //public Vector2 LevelSize = new Vector2(1500, 800);
        public Rectangle Bounds = new Rectangle(0, 0, 1900, 1350);

        // Door trigger to the main level
        public Rectangle ExitDoor = new Rectangle(-15, 260, 50, 100); 

        public Rectangle LightHouseDoor = new Rectangle(805, 130, 95, 30);

        List<Rectangle> walls = new List<Rectangle>();

        public Vector2 SpawnFromPrep = new Vector2(50, 260);

        public Vector2 SpawnFromLighthouseDoor = new Vector2(805, 130);

        //player.regen
        float healthRegenTimer = 0f;
        public Basement(GameManager gm, Texture2D floorTexture)
        {
            this.gm = gm;
            this.FloorTexture = floorTexture;

            walls.Add(new Rectangle(630, 90, 470, 50));
            walls.Add(new Rectangle(580, 70, 570, 50));
            walls.Add(new Rectangle(540, 40, 640, 50));

            walls.Add(new Rectangle(650, 0, 150, 170));
            walls.Add(new Rectangle(910, 0, 150, 170));

            //campfire
            walls.Add(new Rectangle(1130, 355, 160, 100));

            //sea
            walls.Add(new Rectangle(1420, 0, 10, 370));
            walls.Add(new Rectangle(1420, 360, 100, 10));
            walls.Add(new Rectangle(1510, 360, 10, 90));

            walls.Add(new Rectangle(1510, 450, 500, 10));
            walls.Add(new Rectangle(1510, 630, 500, 10));

            walls.Add(new Rectangle(1510, 630, 10, 550));
            walls.Add(new Rectangle(1510, 1170, 100, 10));
            walls.Add(new Rectangle(1600, 1170, 10, 200));
        }

        //NpcManager.SpawnDailyNPCs();
        
        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            //regen
            if (gm.Player.Health < gm.Player.MaxHealth)
            {
                healthRegenTimer += dt * 5f; // 5 หน่วย/วิ
                int heal = (int)healthRegenTimer;
                if (heal > 0)
                {
                    gm.Player.Health += heal;
                    if (gm.Player.Health > gm.Player.MaxHealth)
                        gm.Player.Health = gm.Player.MaxHealth;
                    healthRegenTimer -= heal;
                }
            }

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

            foreach (var wall in walls)
            {
                if (playerRect.Intersects(wall))
                {
                    Rectangle overlap = Rectangle.Intersect(playerRect, wall);

                    if (overlap.Width < overlap.Height)
                    {

                        if (playerRect.Center.X < wall.Center.X)
                            gm.Player.Position.X -= overlap.Width;
                        else
                            gm.Player.Position.X += overlap.Width;
                    }
                    else
                    {

                        if (playerRect.Center.Y < wall.Center.Y)
                            gm.Player.Position.Y -= overlap.Height;
                        else
                            gm.Player.Position.Y += overlap.Height;
                    }


                    playerRect.X = (int)gm.Player.Position.X;
                    playerRect.Y = (int)gm.Player.Position.Y;
                }
            }

            // Check if player reached door
            if (ExitDoor.Intersects(new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, 32, 64)))
            {
                gm.fader.FadingOut = true;
                //gm.StartPrepZone(); // function we’ll add in GameManager
                //gm.InBasement = false;          
                gm.pendingSceneChangeToPrep = true;
            }
            if (LightHouseDoor.Intersects(new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, 32, 64)))
            {
                gm.fader.FadingOut = true;
                gm.pendingSceneChangeToLighthouse = true;
                gm.pendingFromBasement = true; 
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(FloorTexture, Bounds, Color.White);

            // Draw the door as a solid rectangle
            //sb.Draw(gm.WhitePixel, ExitDoor, Color.Gray);
            //sb.Draw(gm.WhitePixel, LightHouseDoor, Color.Gray);
            //foreach (var wall in walls)
            //{
            //    sb.Draw(gm.WhitePixel, wall, Color.Brown * 0.5f); // กำแพงสีน้ำตาล
            //}
        }

    }

}
