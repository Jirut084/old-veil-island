using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project1
{
    public class Enemy
    {
        protected GameManager gm;
        public Vector2 Position;
        public Texture2D tex;

        public int frameWidth { get; set; } = 73;
        public int frameHeight { get; set; } = 113;

        public virtual Rectangle Hitbox => new Rectangle(
            (int)(Position.X - frameWidth / 2),
            (int)(Position.Y - frameHeight / 2),
            frameWidth, frameHeight
        );

        // เปลี่ยนชื่อ field ให้เหมือน Enemy1
        public int health = 100;
        public bool Dead => health <= 0;
        public int damage = 20;
        public float speed = 200f;

        protected int frameCount = 4;
        protected int currentFrame = 0;
        protected int row = 0;
        protected double frameTime = 0.2;
        protected double timer = 0;

        protected float detectRange = 400f;
        protected float attackCooldown = 1f;
        protected float attackTimer = 0f;

        public bool WasHit = false;
        protected float hitTimer = 0f;

        Vector2 patrolTarget;
        float patrolRadius = 300f;

        public Enemy(GameManager gm, Vector2 pos, Texture2D tex, int width = 73, int height = 113)
        {
            this.gm = gm;
            Position = pos;
            this.tex = tex;
            frameWidth = width;
            frameHeight = height;

            patrolTarget = Position;
        }

        public virtual void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            attackTimer -= dt;

            // Animation
            timer += dt;
            if (timer >= frameTime)
            {
                currentFrame = (currentFrame + 1) % frameCount;
                timer = 0;
            }
            row = 0;

            if (WasHit)
            {
                hitTimer -= dt;
                if (hitTimer <= 0f) WasHit = false;
            }

            Vector2 dir;
            float playerDist = Vector2.Distance(Position, gm.Player.Position);

            if (playerDist < detectRange)
            {
                float chaseSpeed = speed * 1.5f;
                dir = gm.Player.Position - Position;
                if (dir != Vector2.Zero) dir.Normalize();

                dir += EvadeProjectiles();
                dir = AvoidObstacles(dir);

                Position += dir * chaseSpeed * dt;

                if (playerDist < 40f && attackTimer <= 0f)
                {
                    gm.Player.TriggerHitSound();
                    gm.Player.TakeDamage(damage);
                    attackTimer = attackCooldown;
                }
            }
            else
            {
                if (Vector2.Distance(Position, patrolTarget) < 10f)
                {
                    patrolTarget = Position + new Vector2(
                        gm.Level.Rand.Next(-(int)patrolRadius, (int)patrolRadius),
                        gm.Level.Rand.Next(-(int)patrolRadius, (int)patrolRadius)
                    );
                }

                dir = patrolTarget - Position;
                if (dir != Vector2.Zero) dir.Normalize();
                dir = AvoidObstacles(dir);

                Position += dir * speed * 0.5f * dt;
            }


        }

        public virtual void Draw(SpriteBatch sb)
        {
            var color = WasHit ? Color.Red : Color.White;
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            sb.Draw(tex, Position, sourceRect, color, 0f, new Vector2(frameWidth / 2f, frameHeight / 2f), 2f, SpriteEffects.None, 0f);
        }

        public void TakeDamage(int dmg)
        {
            health -= dmg;
            WasHit = true;
            hitTimer = 0.2f;
            if (Dead) OnDeath();
        }

        protected virtual void OnDeath() { }

        protected Vector2 AvoidObstacles(Vector2 desiredDir)
        {
            Vector2 force = Vector2.Zero;
            float obstacleWeight = 2f;

            foreach (var wall in gm.Level.walls)
            {
                if (Hitbox.Intersects(wall))
                {
                    Rectangle overlap = Rectangle.Intersect(Hitbox, wall);
                    Vector2 away = Position - new Vector2(wall.Center.X, wall.Center.Y);
                    if (away != Vector2.Zero) away.Normalize();
                    force += away * Math.Max(overlap.Width, overlap.Height);
                }
            }

            foreach (var obj in gm.Level.FixedObjects)
            {
                if (Hitbox.Intersects(obj.Hitbox))
                {
                    Rectangle overlap = Rectangle.Intersect(Hitbox, obj.Hitbox);
                    Vector2 away = Position - obj.Hitbox.Center.ToVector2();
                    if (away != Vector2.Zero) away.Normalize();
                    force += away * Math.Max(overlap.Width, overlap.Height);
                }
            }

            Vector2 finalDir = desiredDir + force * obstacleWeight;
            if (finalDir != Vector2.Zero) finalDir.Normalize();
            return finalDir;
        }

        protected Vector2 EvadeProjectiles()
        {
            Vector2 evade = Vector2.Zero;
            foreach (var proj in gm.Level.Projectiles)
            {
                float dist = Vector2.Distance(Position, proj.Position);
                if (dist < 100f)
                {
                    Vector2 away = Position - proj.Position;
                    if (away != Vector2.Zero) away.Normalize();
                    evade += away / dist;
                }
            }
            return evade;
        }
    }
}
