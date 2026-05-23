using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class Lighthouse
    {
        GameManager gm;
        public Vector2 PlayerStart = new Vector2(675, 710);
        public Texture2D FloorTexture;       // ชั้น 1
        public Texture2D Floor2Texture;      // ชั้น 2

        // ขนาด map เดิม
        public Rectangle Bounds = new Rectangle(165, 0, 1000, 850);

        // ประตูออก (กลับ Basement)
        public Rectangle BasementDoor = new Rectangle(560, 780, 220, 32);

        // ชั้นปัจจุบัน
        public int Floor = 1;

        // ตำแหน่งบันได (ตรงกันทั้งสองชั้น เพื่อให้รู้สึกว่าเดินขึ้น-ลงจุดเดียวกัน)
        public Rectangle StairsUp = new Rectangle(220, 260, 90, 40);     // ใน floor 1 → ขึ้นชั้น 2
        public Rectangle StairsDown = new Rectangle(220, 260, 90, 40);   // ใน floor 2 → ลงชั้น 1

        // จุด spawn เมื่อสลับชั้น
        public Vector2 SpawnOnFloor2 = new Vector2(310, 320);
        public Vector2 SpawnOnFloor1 = new Vector2(310, 320);

        // delay กันสลับชั้นซ้ำทันที
        float floorSwitchCooldown = 0f;

        List<Rectangle> wallsFloor1 = new List<Rectangle>();
        List<Rectangle> wallsFloor2 = new List<Rectangle>();
        List<Rectangle> walls => Floor == 2 ? wallsFloor2 : wallsFloor1;



        public Lighthouse(GameManager gm, Texture2D floorTexture, Texture2D floor2Texture)
        {
            this.gm = gm;
            this.FloorTexture = floorTexture;
            this.Floor2Texture = floor2Texture;

            BuildFloor1Walls();
            BuildFloor2Walls();
        }

        void BuildFloor1Walls()
        {
            //4 tit
            wallsFloor1.Add(new Rectangle(0, 790, 1000, 32));
            wallsFloor1.Add(new Rectangle(0, 0, 220, 1000));
            wallsFloor1.Add(new Rectangle(0, 0, 2000, 220));
            wallsFloor1.Add(new Rectangle(1150, 0, 220, 1000));
            //extra มุม (บันไดซ้าย — เปิดช่องให้เดินขึ้นได้ ลด wall ให้แคบลง)
            wallsFloor1.Add(new Rectangle(200, 200, 160, 40));
            // (เอา wall ที่บล็อกบันไดเดิม Rectangle(200, 230, 100, 60) ออก เพื่อให้เดินบน StairsUp ได้)

            wallsFloor1.Add(new Rectangle(980, 200, 160, 60));
            wallsFloor1.Add(new Rectangle(1070, 240, 100, 60));

            //โตีะ
            wallsFloor1.Add(new Rectangle(620, 150, 320, 100));
            //กล่อง
            wallsFloor1.Add(new Rectangle(240, 440, 190, 160));
            wallsFloor1.Add(new Rectangle(960, 430, 180, 160));
            wallsFloor1.Add(new Rectangle(880, 575, 60, 80));
            wallsFloor1.Add(new Rectangle(840, 250, 100, 40));

            //ข้างประตู
            wallsFloor1.Add(new Rectangle(460, 630, 130, 300));
            wallsFloor1.Add(new Rectangle(320, 690, 180, 300));
            wallsFloor1.Add(new Rectangle(220, 620, 140, 300));
            wallsFloor1.Add(new Rectangle(280, 660, 140, 300));

            wallsFloor1.Add(new Rectangle(770, 670, 130, 300));

            //sink น้ำ
            wallsFloor1.Add(new Rectangle(180, 380, 70, 40));
        }

        void BuildFloor2Walls()
        {
            // ขอบห้อง 4 ด้าน (ใช้ขอบเดียวกับชั้น 1 เพื่อ Bounds เท่ากัน)
            wallsFloor2.Add(new Rectangle(0, 790, 1000, 32));
            wallsFloor2.Add(new Rectangle(0, 0, 220, 1000));
            wallsFloor2.Add(new Rectangle(0, 0, 2000, 220));
            wallsFloor2.Add(new Rectangle(1150, 0, 220, 1000));

            // กรอบมุมเฉียงทั้ง 4 มุม (ตามรูป floor2-2 ที่เป็นห้องแปดเหลี่ยม)
            wallsFloor2.Add(new Rectangle(200, 200, 160, 40));
            wallsFloor2.Add(new Rectangle(980, 200, 160, 60));
            wallsFloor2.Add(new Rectangle(1070, 240, 100, 60));
            wallsFloor2.Add(new Rectangle(220, 720, 130, 80));
            wallsFloor2.Add(new Rectangle(950, 720, 200, 80));

            // โต๊ะ/เก้าอี้ ล่างซ้าย
            wallsFloor2.Add(new Rectangle(260, 590, 110, 90));

            // เตียง ขวาล่าง + พรม (เดินผ่านพรมได้ — แค่กั้นเตียง)
            wallsFloor2.Add(new Rectangle(870, 560, 220, 160));

            // บันไดใหญ่ขวา (decorative, เดินทะลุไม่ได้)
            wallsFloor2.Add(new Rectangle(820, 200, 130, 220));

            // โคมไฟ/โต๊ะกลางบน
            wallsFloor2.Add(new Rectangle(540, 230, 80, 70));
            wallsFloor2.Add(new Rectangle(740, 230, 60, 60));
        }

        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            if (floorSwitchCooldown > 0f) floorSwitchCooldown -= dt;

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

            // สลับชั้นด้วยบันได
            if (floorSwitchCooldown <= 0f)
            {
                if (Floor == 1 && StairsUp.Intersects(playerRect))
                {
                    Floor = 2;
                    gm.Player.Position = SpawnOnFloor2;
                    floorSwitchCooldown = 0.8f;
                    gm.Log("Climbed up to floor 2");
                }
                else if (Floor == 2 && StairsDown.Intersects(playerRect))
                {
                    Floor = 1;
                    gm.Player.Position = SpawnOnFloor1;
                    floorSwitchCooldown = 0.8f;
                    gm.Log("Climbed down to floor 1");
                }
            }

            // ประตูกลับ Basement — เฉพาะตอนอยู่ชั้น 1 เท่านั้น
            if (Floor == 1 && BasementDoor.Intersects(playerRect))
            {
                gm.fader.FadingOut = true;
                gm.pendingSceneChangeToBasement = true;
                gm.pendingFromLighthouse = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Texture2D tex = (Floor == 2) ? Floor2Texture : FloorTexture;
            sb.Draw(tex, Bounds, Color.White);

            // hint บันได
            Vector2 playerPos = gm.Player.Position;
            Rectangle playerRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, 32, 64);

            if (Floor == 1 && StairsUp.Intersects(playerRect))
            {
                Vector2 hint = new Vector2(StairsUp.Center.X - 60, StairsUp.Top - 30);
                sb.DrawString(gm.DefaultFont, "Walk to go up", hint, Color.Yellow);
            }
            else if (Floor == 2 && StairsDown.Intersects(playerRect))
            {
                Vector2 hint = new Vector2(StairsDown.Center.X - 60, StairsDown.Top - 30);
                sb.DrawString(gm.DefaultFont, "Walk to go down", hint, Color.Yellow);
            }

            //sb.Draw(gm.WhitePixel, BasementDoor, Color.Gray * 0.5f);
            //sb.Draw(gm.WhitePixel, StairsUp, Color.Cyan * 0.3f);
            //foreach (var wall in walls)
            //{
            //    sb.Draw(gm.WhitePixel, wall, Color.Brown * 0.5f); // กำแพงสีน้ำตาล
            //}
        }

        public void Reset()
        {
            Floor = 1;
            floorSwitchCooldown = 0f;
        }
    }
}
