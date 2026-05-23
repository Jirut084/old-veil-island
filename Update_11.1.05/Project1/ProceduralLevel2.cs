using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Stage_and_Day;

namespace Project1
{
    public class ProceduralLevel2 : ProceduralLevel
    {

        public ProceduralLevel2(GameManager gm) : base(gm)
        {

            walls.Clear();

            // กำหนดขอบใหม่สำหรับ map day2
            walls.Add(new Rectangle(0, 0, 4000, 229));
            walls.Add(new Rectangle(0, 2760, 4000, 3000));
            walls.Add(new Rectangle(0, 448, 325, 2805));
            walls.Add(new Rectangle(3760, 0, 239, 2556));

            //genroom
            walls.Add(new Rectangle(883, 2253, 317, 337));


            PlaceFixedObjects();
        }

        public override void PlaceFixedObjects()
        {
            FixedObjects.Clear();

            var textures = new Dictionary<string, Texture2D>()
        {
            { "tree2", gm.content.Load<Texture2D>("tree2") },
            { "rock0001", gm.content.Load<Texture2D>("rock0001") },
            { "rock0002", gm.content.Load<Texture2D>("rock0002") },
                { "rock0004", gm.content.Load<Texture2D>("rock0004") }
        };

            var sizes = new Dictionary<string, Vector2>()
        {
            { "tree2", new Vector2(225, 337) },
            { "rock0001", new Vector2(80, 75) },
            { "rock0002", new Vector2(80, 75) },
            { "rock0004", new Vector2(80, 75) },
        };

            var fixedPositions = new Dictionary<string, List<Vector2>>()
        {
            { "tree2", new List<Vector2>()
            {
                new Vector2(1125, 475),
                new Vector2(801, 702),
                new Vector2(1124, 926),
                new Vector2(1601, 1225),
                new Vector2(1767, 1373),
                new Vector2(1522, 1377),
                new Vector2(1610, 1530),
                new Vector2(2565, 2049),
                new Vector2(2405, 2275),
                new Vector2(2652, 2278),
                new Vector2(2501, 2431),
                new Vector2(3207, 548),
                new Vector2(3129, 701),
                new Vector2(3214, 781),
                new Vector2(3299, 706)
            }
        },
        { "rock0001", new List<Vector2>()
            {
                new Vector2(742, 927),
                new Vector2(1378, 849),
                new Vector2(979, 1223),
                new Vector2(1378, 1372),
                new Vector2(1944, 1676),
                new Vector2(1224, 2578),
                new Vector2(899, 2721),
                new Vector2(2335, 2571),
                new Vector2(2581, 2350),
                new Vector2(2813, 2570),
                new Vector2(2743, 2198),
                new Vector2(2822, 1300),
                new Vector2(3383, 925),
                new Vector2(3060, 620)
            }
        },
        { "rock0002", new List<Vector2>()
            {
                new Vector2(1117, 1735),
                new Vector2(800, 2561),
                new Vector2(2479, 1360),
                new Vector2(2637, 1214)
            }
        },
                { "rock0004", new List<Vector2>()
            {
                new Vector2(408, 843),
                new Vector2(1127, 2718),
                new Vector2(2479, 1360),
                new Vector2(3128, 993)
            }
        }
        };

            foreach (var kv in fixedPositions)
            {
                string texName = kv.Key;
                Vector2 newSize = sizes[texName];
                Texture2D tex = textures[texName];
                float scaleX = newSize.X / tex.Width;
                float scaleY = newSize.Y / tex.Height;

                foreach (var pos in kv.Value)
                    FixedObjects.Add(new FixedObject(pos, tex, new Vector2(scaleX, scaleY)));
            }
        }

        public override Vector2 GetStartPoint() => new Vector2(1546, 1142);

        public override void GenerateLevelSeed(int seed)
        {
            Rand = new Random(seed);
            enemies.Clear();
            pickups.Clear();
            traps.Clear();
            projectiles.Clear();
            Rooms.Clear();
            treasureBoxes.Clear();
            fences.Clear();

            // --- Spawn fences for this level ---
            fences.Add(new Fence(gm, gm.content.Load<Texture2D>("fench2"), new Vector2(1200, 600)));

            // --- Spawn treasure box (scissors) 
            treasureBoxes.Add(new TreasureBox(
                gm,
                gm.content.Load<Texture2D>("teasureBox"),
                new Vector2(1200, 800),
                "scissors",
                gm.content.Load<Texture2D>("Sprite_scissors"))
            );


            // Spawn enemies/traps ตามต้องการ (Day2)
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400));
                } while (!IsPositionValid(pos));
                enemies.Add(new Enemy(gm, pos, gm.TexEnemy));
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400));
                } while (!IsPositionValid(pos));
                traps.Add(new TrapBear(gm, pos, gm.TexTrap));
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);


        }



        // ใน ProceduralLevel
        public override void Draw(SpriteBatch sb)
        {
            // วาดพื้น
            sb.Draw(gm.TexGrass2, new Rectangle(0, 0, (int)LevelSize.X, (int)LevelSize.Y), Color.White);

            // วาดกำแพงเป็นสีแดงโปร่งใส
            foreach (var wall in walls)
            {
                sb.Draw(gm.WhitePixel, wall, Color.Red * 0.5f); // 50% transparency
            }

            // วาด exit ถ้ามี key items ครบ
            if (gm.DayManager.HasAllKeyItems())
            {
                sb.Draw(gm.WhitePixel, ExitDoor, Color.Gray);
                sb.DrawString(gm.DefaultFont, "EXIT", new Vector2(ExitDoor.X + 10, ExitDoor.Y + 10), Color.White);
            }

            // ตรวจสอบ prep door
            if (PrepDoor.Intersects(new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, 32, 64)))
            {
                gm.fader.FadingOut = true;
                gm.pendingSceneChangeToPrep = true;
                gm.pendingSceneFromMain = gm.InMainLevel;
            }

            // วาด fixed objects
            foreach (var obj in FixedObjects)
                obj.Draw(sb);
        }

    }
}
