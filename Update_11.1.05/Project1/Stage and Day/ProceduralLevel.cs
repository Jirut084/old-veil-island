

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.NPC;
using Project1.Stage_and_Day;

namespace Project1
{
    public class ProceduralLevel
    {
        protected GameManager gm;

        public Vector2 LevelSize = new Vector2(4000, 3000);
        //public Vector2 MapSize = new Vector2(4000, 3000);
        public Random Rand = new Random();

        public Rectangle PrepDoor = new Rectangle(3980, 255, 100, 150); // prepRoomDoor
        public Rectangle ExitDoor = new Rectangle(50, 2800, 100, 150); // bottom-left example

        // Entities
        protected List<Enemy> enemies = new List<Enemy>();
        protected List<Projectile> projectiles = new List<Projectile>();

        public List<TrapBear> Traps => traps;

        protected List<TrapBear> traps = new List<TrapBear>();

        protected List<PickupItem> pickups = new List<PickupItem>();
        // ใน ProceduralLevel

        public List<Projectile> Projectiles => projectiles;

        public List<FixedObject> FixedObjects = new List<FixedObject>();

        protected List<TreasureBox> treasureBoxes = new List<TreasureBox>();

        public List<Fence> fences = new List<Fence>();


        // For minimap
        public List<Room> Rooms = new List<Room>();

        public List<Rectangle> walls = new List<Rectangle>();
        //bool timePaused = true;

        public class Fence
        {
            GameManager gm;
            public Vector2 Position;
            public Texture2D Texture;
            public bool Destroyed = false;
            public float Scale = 5f; // default

            public float HitProgress = 0f;
            public float HitPerSecond = 0.5f;

            public Rectangle Hitbox => new Rectangle(
    (int)Position.X,
    (int)Position.Y,
    (int)(Texture.Width * Scale),
    (int)(Texture.Height * Scale)
);

            public void HitWithAxe(float delta)
            {
                if (Destroyed) return;

                HitProgress += HitPerSecond * delta;

                if (HitProgress >= 1f)
                {
                    Destroyed = true;
                }
            }

            public Fence(GameManager gm, Texture2D texture, Vector2 pos, float scale = 5f)
            {
                this.gm = gm;
                this.Texture = texture;
                this.Position = pos;
                this.Scale = scale;
            }

            public void Draw(SpriteBatch sb)
            {
                if (Destroyed) return;
                sb.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }

            public void Break()
            {
                Destroyed = true;
                gm.Log("The fence has been destroyed.");
            }

        }

        public ProceduralLevel(GameManager gm)
        {
            this.gm = gm;

            //ขอบ
            walls.Add(new Rectangle(0, 0, 4000, 150));
            walls.Add(new Rectangle(250, 2930, 4000, 150));
            walls.Add(new Rectangle(0, 0, 195, 2700));
            walls.Add(new Rectangle(3840, 540, 150, 2700));

            //บ่อน้ำกลางจอ
            walls.Add(new Rectangle(1810, 1200, 485, 420));

            fences.Add(new Fence(gm, gm.content.Load<Texture2D>("fench1"), new Vector2(160, 2662)));

            PlaceFixedObjects();
        }

        public bool HitFenceAt(Vector2 pos, int range = 0)
        {
            int playerW = 32;
            int playerH = 64;

            // สร้าง hitbox ของ player ที่ตำแหน่ง pos
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, playerW, playerH);

            foreach (var f in fences)
            {
                if (f.Destroyed) continue;

                if (playerRect.Intersects(f.Hitbox))
                {
                    Rectangle overlap = Rectangle.Intersect(playerRect, f.Hitbox);

                    if (overlap.Width < overlap.Height)
                    {
                        if (playerRect.Center.X < f.Hitbox.Center.X)
                            gm.Player.Position.X -= overlap.Width;
                        else
                            gm.Player.Position.X += overlap.Width;
                    }
                    else
                    {
                        if (playerRect.Center.Y < f.Hitbox.Center.Y)
                            gm.Player.Position.Y -= overlap.Height;
                        else
                            gm.Player.Position.Y += overlap.Height;
                    }

                    // update rect หลังจากถูกผลัก
                    playerRect.X = (int)gm.Player.Position.X;
                    playerRect.Y = (int)gm.Player.Position.Y;

                    return true; // มีการชนรั้ว
                }
            }

            return false; // ไม่มีการชน
        }

        public void DrawFences(SpriteBatch sb)
        {
            foreach (var f in fences) f.Draw(sb);
        }

        public virtual void PlaceFixedObjects()
        {
            // โหลด textures
            var textures = new Dictionary<string, Texture2D>()
    {
        { "tree1", gm.content.Load<Texture2D>("tree1") },
        { "rock0001", gm.content.Load<Texture2D>("rock0001") },
        { "rock0002", gm.content.Load<Texture2D>("rock0002") },
        { "rock0004", gm.content.Load<Texture2D>("rock0004") }
    };

            // ขนาดวัตถุใหม่
            var sizes = new Dictionary<string, Vector2>()
    {
        { "tree1", new Vector2(225, 337) },
        { "rock0001", new Vector2(80, 75) },
        { "rock0002", new Vector2(80, 75) },
        { "rock0004", new Vector2(80, 75) }
    };

            // ตำแหน่ง fixed objects
            var fixedPositions = new Dictionary<string, List<Vector2>>()
{
    { "tree1", new List<Vector2>()
        {
            new Vector2(816,636), new Vector2(968,486), new Vector2(1140,636), new Vector2(972,862),
            new Vector2(1208,2131), new Vector2(1046,2283), new Vector2(1368,2286), new Vector2(1208,2512),
            new Vector2(3128,932), new Vector2(3128,1234), new Vector2(3360,860), new Vector2(3362,1160),
            new Vector2(3603,2137), new Vector2(3363,2433)
        }
    },
    { "rock0001", new List<Vector2>()
        {
            new Vector2(805,1073), new Vector2(1373,850), new Vector2(1293,2422),
            new Vector2(2174,1749), new Vector2(3531,2269), new Vector2(3376,2800)
        }
    },
    { "rock0002", new List<Vector2>()
        {
            new Vector2(706,910), new Vector2(1189,1057), new Vector2(1827,1136),
            new Vector2(1186,2711), new Vector2(3029,1286), new Vector2(3347,2335)
        }
    },
    { "rock0004", new List<Vector2>()
        {
            new Vector2(1436,2564), new Vector2(1919,1138), new Vector2(2075,1663), new Vector2(3276,2712)
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
                {
                    FixedObjects.Add(new FixedObject(pos, tex, new Vector2(scaleX, scaleY)));
                }
            }

        }

        public Rectangle Bounds = new Rectangle(0, 0, 4000, 3000);

        // เพิ่ม parameter range ที่ใหญ่ขึ้น (80) และให้ใช้ corridor รอบ fence
        public Fence GetNearbyFence(Vector2 playerPos, float range = 100f)
        {
            return fences.FirstOrDefault(f => !f.Destroyed &&
                                              Vector2.Distance(f.Position + new Vector2(f.Texture.Width / 2, f.Texture.Height / 2), playerPos) < range);
        }

        public bool CanPlayerMoveTo(Vector2 pos, GameTime gt, int playerWidth = 32, int playerHeight = 64)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, playerWidth, playerHeight);

            // ตรวจ wall
            foreach (var wall in walls)
                if (playerRect.Intersects(wall))
                    return false;

            foreach (var obj in FixedObjects)
            {
                Rectangle corridor = new Rectangle(
                    obj.Hitbox.X - 8,  // ปรับขนาด corridor
                    obj.Hitbox.Y - 8,
                    obj.Hitbox.Width + 16,
                    obj.Hitbox.Height + 16
                );
                if (playerRect.Intersects(corridor))
                    return false; // หรือใช้ผลักออกเหมือนเดิม
            }


            // ตรวจ fence ที่ยังไม่ถูกทำลาย + corridor
            // ตรวจ fence ที่ยังไม่ถูกทำลาย + solid collision
            foreach (var f in fences)
            {
                if (!f.Destroyed)
                {
                    Rectangle playerRectFence = new Rectangle((int)pos.X, (int)pos.Y, playerWidth, playerHeight);
                    if (playerRectFence.Intersects(f.Hitbox))
                    {
                        Rectangle overlap = Rectangle.Intersect(playerRectFence, f.Hitbox);

                        if (overlap.Width < overlap.Height)
                        {
                            if (playerRectFence.Center.X < f.Hitbox.Center.X)
                                pos.X -= overlap.Width;
                            else
                                pos.X += overlap.Width;
                        }
                        else
                        {
                            if (playerRectFence.Center.Y < f.Hitbox.Center.Y)
                                pos.Y -= overlap.Height;
                            else
                                pos.Y += overlap.Height;
                        }
                    }
                }
            }

            foreach (var tb in treasureBoxes)
            {
                if (tb.IsOpened) continue; // ถ้าเปิดแล้วเดินทะลุได้

                if (playerRect.Intersects(tb.Hitbox))
                {
                    // Soft collision: ให้เดินผ่านแกน X/Y ได้เล็กน้อยเพื่อไม่บล็อกการเปิด
                    Rectangle overlap = Rectangle.Intersect(playerRect, tb.Hitbox);

                    // ถ้า player อยู่ด้านบนหรือล่างของกล่อง ให้ผลักแค่แกน Y เล็กน้อย
                    if (playerRect.Bottom > tb.Hitbox.Top && playerRect.Top < tb.Hitbox.Bottom)
                    {
                        float pushY = overlap.Height * 0.3f; // ผลักน้อย ๆ
                        if (playerRect.Center.Y < tb.Hitbox.Center.Y)
                            gm.Player.Position.Y -= pushY;
                        else
                            gm.Player.Position.Y += pushY;
                    }

                    // ถ้า player อยู่ด้านซ้ายหรือขวาของกล่อง ให้ผลักแค่แกน X เล็กน้อย
                    if (playerRect.Right > tb.Hitbox.Left && playerRect.Left < tb.Hitbox.Right)
                    {
                        float pushX = overlap.Width * 0.3f; // ผลักน้อย ๆ
                        if (playerRect.Center.X < tb.Hitbox.Center.X)
                            gm.Player.Position.X -= pushX;
                        else
                            gm.Player.Position.X += pushX;
                    }

                    // update rect หลังจากปรับ position
                    playerRect.X = (int)gm.Player.Position.X;
                    playerRect.Y = (int)gm.Player.Position.Y;
                }
            }
            return true;
        }

        public virtual void GenerateLevelSeed(int seed)
        {
            Rand = new Random(seed);
            enemies.Clear();
            pickups.Clear();
            traps.Clear();
            projectiles.Clear();
            Rooms.Clear();
            treasureBoxes.Clear();

            // เก็บ fence คงที่ (fench1) ก่อนเคลียร์
            var keyFences = fences.Where(f => f.Texture.Name == "fench1").ToList();
            fences.Clear();

            // --- Spawn normal enemies 6 ตัว ---
            for (int i = 0; i < 1; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400));
                } while (!IsPositionValid(pos));

                Enemy e = new Enemy(gm, pos, gm.TexEnemy);
                enemies.Add(e);
            }

            // --- Spawn (Enemy2) 2 ตัว ---
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400));
                } while (!IsPositionValid(pos));

                //Enemy2 henmil = new Enemy2(gm, pos, gm.content.Load<Texture2D>("Henmil"));

                Enemy2 slime = new Enemy2(gm, pos, gm.content.Load<Texture2D>("monster4_1_walk"));
                enemies.Add(slime);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400));
                } while (!IsPositionValid(pos));

                //Enemy2 henmil = new Enemy2(gm, pos, gm.content.Load<Texture2D>("Henmil"));

                Enemy3 henmil = new Enemy3(gm, pos, gm.content.Load<Texture2D>("Henmil"));
                enemies.Add(henmil);
            }

            // --- Spawn notes ---
            var noteTexts = new string[]
            {
        "There is a tool chest in here but where...",
        "shit...I lost my gun, hope those things don't know how to use it.",
        "The exits is somewhere what if I go to the left side?",
        "A blockage on the exits? maybe if there is something to clear it...",
        "I got chase by those ugly things... if someone reading this you must get ou-"
            };

            for (int i = 0; i < 5; i++)
            {
                Vector2 pos;
                do { pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400)); }
                while (!IsPositionValid(pos));

                var noteTex = gm.content.Load<Texture2D>($"note{i + 1}");
                var note = new Note($"note{i + 1}", noteTexts[i]);
                pickups.Add(new PickupItem(gm, pos, noteTex, note.Id, isNote: true, isAxe: false, noteRef: note));
            }

            // --- Spawn ammo pickups ---
            for (int i = 0; i < 6; i++)
            {
                Vector2 pos;
                do { pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400)); }
                while (!IsPositionValid(pos));
                var ammoTex = gm.content.Load<Texture2D>("AmmoPickup");
                pickups.Add(new AmmoPickup(gm, pos, ammoTex, "ammo", 6));
            }

            // --- Spawn gun ---
            Vector2 gunPos;
            do
            {
                gunPos = new Vector2(Rand.Next(200, (int)LevelSize.X - 200), Rand.Next(200, (int)LevelSize.Y - 200));
            } while (!IsPositionValid(gunPos));

            Texture2D gunTex = gm.TexPistol;
            var gunPickup = new PickupItem(gm, gunPos, gunTex, "gun");
            gunPickup.Scale = new Vector2(2f, 2f);
            pickups.Add(gunPickup);

            // --- Spawn fixed wood pickup ---
            Texture2D woodTex = gm.content.Load<Texture2D>("Sprite_wood");
            var woodPickup = new PickupItem(gm, new Vector2(840, 1030), woodTex, "wood");
            woodPickup.Scale = new Vector2(2f, 2f);
            pickups.Add(woodPickup);

            // --- Spawn traps ---
            for (int i = 0; i < 12; i++)
            {
                Vector2 pos;
                do { pos = new Vector2(Rand.Next(400, (int)LevelSize.X - 400), Rand.Next(400, (int)LevelSize.Y - 400)); }
                while (!IsPositionValid(pos));
                traps.Add(new TrapBear(gm, pos, gm.TexTrap));
            }

            treasureBoxes.Add(new TreasureBox(gm, gm.content.Load<Texture2D>("teasureBox"), new Vector2(2678, 2435),
             "axe", gm.content.Load<Texture2D>("Sprite_axe")));

            // --- เพิ่ม fence คงที่กลับเข้า list ---
            fences.AddRange(keyFences);
        }


        public void SpawnPickup(PickupItem pi)
        {
            pickups.Add(pi);
        }

        public void SpawnInitialEntities()
        {
            // Additional initialization if needed
        }

        public virtual Vector2 GetStartPoint() => new Vector2(3900, 300);
        public Vector2 GetSafeRoomEntry() => new Vector2(LevelSize.X - 200, LevelSize.Y - 200);

        public virtual void Update(GameTime gt)
        {
            Rectangle floor = Bounds;

            Vector2 pos = gm.Player.Position;
            int playerW = 32;
            int playerH = 64;
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, playerW, playerH);

            // Clamp player within bounds
            int marginX = playerW;
            int marginY = playerH;
            pos.X = MathHelper.Clamp(pos.X, floor.Left + marginX, floor.Right - marginX);
            pos.Y = MathHelper.Clamp(pos.Y, floor.Top + marginY, floor.Bottom - marginY);
            gm.Player.Position = pos;

            // Update treasure boxes
            foreach (var tb in treasureBoxes)
                tb.Update(gm.Player, gt);

            // --- Player Collision ---
            // Walls
            foreach (var wall in walls)
                ResolveCollision(ref gm.Player.Position, playerRect, wall);

            // FixedObjects
            foreach (var obj in FixedObjects)
                ResolveCollision(ref gm.Player.Position, playerRect, obj.Hitbox);

            // Fences
            foreach (var f in fences)
            {
                if (!f.Destroyed)
                    ResolveCollision(ref gm.Player.Position, playerRect, f.Hitbox);
            }

            // Treasure Boxes (soft collision)
            foreach (var tb in treasureBoxes)
            {
                if (!tb.IsOpened)
                    //ResolveSoftCollision(ref gm.Player.Position, playerRect, tb.Hitbox);
                    ResolveCollision(ref gm.Player.Position, playerRect, tb.Hitbox);
            }
            foreach (var tb in treasureBoxes)
            {
                if (tb.IsOpened)
                    //ResolveSoftCollision(ref gm.Player.Position, playerRect, tb.Hitbox);
                    ResolveCollision(ref gm.Player.Position, playerRect, tb.Hitbox);
            }

            // --- Enemies Update & Collision ---
            foreach (var e in enemies) e.Update(gt);

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                Rectangle enemyRect = enemy.Hitbox;

                // Wall collision
                foreach (var wall in walls)
                    ResolveCollision(ref enemy.Position, enemyRect, wall, enemy);

                // FixedObjects collision
                foreach (var obj in FixedObjects)
                    ResolveCollision(ref enemy.Position, enemyRect, obj.Hitbox, enemy);

                // Fences collision
                foreach (var f in fences)
                    if (!f.Destroyed)
                        ResolveCollision(ref enemy.Position, enemyRect, f.Hitbox, enemy);

                // Treasure Boxes collision (hard)
                foreach (var tb in treasureBoxes)
                {
                    if (!tb.IsOpened)
                        ResolveCollision(ref enemy.Position, enemyRect, tb.Hitbox, enemy);
                }
                foreach (var tb in treasureBoxes)
                {
                    if (tb.IsOpened)
                        ResolveCollision(ref enemy.Position, enemyRect, tb.Hitbox, enemy);
                }

                // Enemy to Enemy collision
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (i == j) continue;
                    var other = enemies[j];
                    Rectangle otherRect = other.Hitbox;

                    if (enemyRect.Intersects(otherRect))
                    {
                        Rectangle overlap = Rectangle.Intersect(enemyRect, otherRect);
                        if (overlap.Width < overlap.Height)
                        {
                            float push = overlap.Width / 2f;
                            if (enemyRect.Center.X < otherRect.Center.X)
                            {
                                enemy.Position.X -= push;
                                other.Position.X += push;
                            }
                            else
                            {
                                enemy.Position.X += push;
                                other.Position.X -= push;
                            }
                        }
                        else
                        {
                            float push = overlap.Height / 2f;
                            if (enemyRect.Center.Y < otherRect.Center.Y)
                            {
                                enemy.Position.Y -= push;
                                other.Position.Y += push;
                            }
                            else
                            {
                                enemy.Position.Y += push;
                                other.Position.Y -= push;
                            }
                        }
                        // Update enemyRect after pushing
                        enemyRect.X = (int)enemy.Position.X - enemy.frameWidth / 2;
                        enemyRect.Y = (int)enemy.Position.Y - enemy.frameHeight / 2;
                    }
                }
            }

            // --- Projectiles ---
            foreach (var p in projectiles) p.Update(gt);

            // Projectile -> Enemy collision
            foreach (var proj in projectiles.ToList())
            {
                foreach (var e in enemies.ToList())
                {
                    if (Vector2.Distance(proj.Position, e.Position) < 22)
                    {
                        e.TakeDamage(proj.Damage);
                        proj.Remove = true;
                        break;
                    }
                }
            }
            projectiles.RemoveAll(p => p.Remove);
            enemies.RemoveAll(e => e.Dead);

            // Traps
            foreach (var t in traps) t.Update(gt);

            // --- Exit Door Check ---
            bool anyFenceBlockingExit = fences.Any(f => !f.Destroyed && f.Hitbox.Intersects(ExitDoor));
            if (!anyFenceBlockingExit && ExitDoor.Intersects(playerRect))
            {
                if (!gm.EndingActive)
                {
                    gm.fader.FadingIn = true;
                    gm.Ending.Start();
                    gm.EndingActive = true;
                }
            }
        }


        // --- Helper Functions ---
        private void ResolveCollision(ref Vector2 pos, Rectangle rect, Rectangle obstacle, Enemy enemy = null)
        {
            if (!rect.Intersects(obstacle)) return;

            Rectangle overlap = Rectangle.Intersect(rect, obstacle);

            if (overlap.Width < overlap.Height)
            {
                if (rect.Center.X < obstacle.Center.X)
                    pos.X -= overlap.Width;
                else
                    pos.X += overlap.Width;
            }
            else
            {
                if (rect.Center.Y < obstacle.Center.Y)
                    pos.Y -= overlap.Height;
                else
                    pos.Y += overlap.Height;
            }

            if (enemy != null)
            {
                rect.X = (int)enemy.Position.X - enemy.frameWidth / 2;
                rect.Y = (int)enemy.Position.Y - enemy.frameHeight / 2;
            }
        }

        private void ResolveSoftCollision(ref Vector2 pos, Rectangle rect, Rectangle obstacle)
        {
            if (!rect.Intersects(obstacle)) return;

            Rectangle overlap = Rectangle.Intersect(rect, obstacle);

            float pushX = overlap.Width * 0.3f;
            float pushY = overlap.Height * 0.3f;

            if (rect.Center.X < obstacle.Center.X)
                pos.X -= pushX;
            else
                pos.X += pushX;

            if (rect.Center.Y < obstacle.Center.Y)
                pos.Y -= pushY;
            else
                pos.Y += pushY;
        }


        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(gm.TexGrass, new Rectangle(0, 0, (int)LevelSize.X, (int)LevelSize.Y), Color.White);

            if (PrepDoor.Intersects(new Rectangle((int)gm.Player.Position.X, (int)gm.Player.Position.Y, 32, 64)))
            {
                //gm.StartMainLevel(); // function we’ll add in GameManager
                gm.fader.FadingOut = true;
                //gm.InBasement = false;
                gm.pendingSceneChangeToPrep = true;
                gm.pendingSceneFromMain = gm.InMainLevel;
            }

            // draw fences
            foreach (var f in fences) f.Draw(sb);

            // Draw treasure boxes
            foreach (var tb in treasureBoxes) tb.Draw(sb, gm.Player);

            // วาดข้อความสำหรับรั้วใกล้
            var nearbyFence = GetNearbyFence(gm.Player.Position);
            var currentSlot = gm.Inventory.Hotbar[gm.Inventory.SelectedIndex];
            bool holdingAxe = currentSlot != null && currentSlot.Item != null && currentSlot.Item.Id == "axe";

            if (nearbyFence != null && holdingAxe && !nearbyFence.Destroyed)
            {
                // คำนวณตำแหน่ง HUD
                Vector2 hudPos = nearbyFence.Position + new Vector2(nearbyFence.Texture.Width * nearbyFence.Scale + 10, 0);
                string msg = "Press E to break the wood";
                var size = gm.DefaultFont.MeasureString(msg);
                sb.DrawString(gm.DefaultFont, msg, hudPos - new Vector2(0, size.Y / 2), Color.Yellow);

                // แถบ progress
                float progress = nearbyFence.HitProgress / 1f; // HitProgress 0..1
                Rectangle bg = new Rectangle((int)hudPos.X, (int)hudPos.Y - 20, (int)size.X, 8);
                sb.Draw(gm.WhitePixel, bg, Color.DarkRed);
                sb.Draw(gm.WhitePixel, new Rectangle(bg.X, bg.Y, (int)(bg.Width * progress), bg.Height), Color.LimeGreen);
            }

            // Draw Exit Door only if there is no fence blocking it
            bool anyFenceBlockingExit = fences.Any(f => !f.Destroyed && f.Hitbox.Intersects(ExitDoor));
            if (!anyFenceBlockingExit)
            {
                sb.Draw(gm.WhitePixel, ExitDoor, Color.Gray);
                sb.DrawString(gm.DefaultFont, "EXIT", new Vector2(ExitDoor.X + 10, ExitDoor.Y + 10), Color.White);
            }

            foreach (var obj in FixedObjects)
                obj.Draw(sb);
        }

        protected virtual bool IsPositionValid(Vector2 pos, int margin = 50)
        {
            Rectangle area = new Rectangle((int)pos.X - margin, (int)pos.Y - margin, margin * 2, margin * 2);

            // ตรวจ wall ทั้งหมด
            foreach (var wall in walls)
            {
                if (area.Intersects(wall))
                    return false;
            }

            // ตรวจ FixedObjects (ต้นไม้/หิน)
            foreach (var obj in FixedObjects)
            {
                if (area.Intersects(obj.Hitbox))
                    return false;
            }

            foreach (var e in enemies)
            {
                if (area.Intersects(e.Hitbox))
                    return false;
            }

            return true;
        }
        public void DrawEnemies(SpriteBatch sb)
        {
            foreach (var e in enemies) e.Draw(sb);
        }
        public void DrawTraps(SpriteBatch sb)
        {
            foreach (var t in traps) t.Draw(sb);
        }
        
        public void DrawItems(SpriteBatch sb)
        {
            foreach (var it in pickups) it.Draw(sb);
        }

        public void AddProjectile(Projectile p) => projectiles.Add(p);

        public bool TryPickupAt(Vector2 playerPos)
        {
            for (int i = pickups.Count - 1; i >= 0; i--)
            {
                var p = pickups[i];
                float dist = Vector2.Distance(playerPos, p.Position);
                if (dist < 60f) // ในระยะเก็บของ
                {
                    if (p.IsNote && p.NoteRef != null)
                    {
                        gm.Codex.UnlockPlayerNote(p.NoteRef);
                        gm.Log($"You collected a note: \"{p.NoteRef.Text}\"");
                        pickups.RemoveAt(i);
                        return true;
                    }
                    else
                    {
                        bool added = gm.Inventory.AddItem(p);
                        if (added)
                        {
                            pickups.RemoveAt(i);

                            if (p.IsAxe)
                            {
                                gm.Log("You obtained an Axe.");
                            }
                            else
                            {
                                gm.Log($"{p.Id} collected!");
                            }

                            return true;
                        }
                        else
                        {
                            gm.Log("Inventory full!");
                            return false;
                        }
                    }
                }
            }

            return false;
        }


        public void Reset()
        {
            // เคลียร์ entities เก่า
            enemies.Clear();
            pickups.Clear();
            traps.Clear();
            projectiles.Clear();
            Rooms.Clear();

            Rand = new Random();

            //PrepDoor = new Rectangle(3980, 255, 100, 150);
            //ExitDoor = new Rectangle(50, 2800, 100, 150);

            //GenerateLevelSeed((int)DateTime.Now.Ticks);
        }
        public TrapBear CheckTrapCollision(Vector2 pos)
        {
            foreach (var t in traps)
            {
                if (!t.Active) continue;
                if (Vector2.Distance(pos, t.Position) < 24) return t;
            }
            return null;
        }
        public Enemy CheckEnemyCollision(Vector2 pos)
        {
            foreach (var e in enemies)
            {
                if (Vector2.Distance(e.Position, pos) < 16) return e;
            }
            return null;
        }
    }
}