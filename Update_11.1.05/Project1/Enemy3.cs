using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Project1
{
    public class Enemy3 : Enemy
    {
        private GameManager gm;
        private Texture2D tex;

        private float scale = 0.3f;
        private float frameTime = 0.12f;

        private int leftFrameIndex = 0;
        private int rightFrameIndex = 0;
        private float leftFrameTimer = 0f;
        private float rightFrameTimer = 0f;

        private bool facingRight = false;

        private List<Rectangle> leftFrames;
        private List<Rectangle> rightFrames;
        private List<Vector2> leftOrigins;
        private List<Vector2> rightOrigins;

        public Enemy3(GameManager gm, Vector2 pos, Texture2D tex)
            : base(gm, pos, tex, 708, 478)
        {
            this.gm = gm;
            this.Position = pos;
            this.tex = tex;
            BuildFrameData();
        }

        private void BuildFrameData()
        {
            leftFrames = new List<Rectangle>
            {
                new Rectangle(16, 60, 599, 335),
                new Rectangle(710, 40, 525, 360),
                new Rectangle(1370, 65, 635, 365),
                new Rectangle(2125, 70, 625, 350),
                new Rectangle(2825, 80, 720, 370),
                new Rectangle(3640, 75, 600, 335),
                new Rectangle(4370, 60, 630, 365),
                new Rectangle(5095, 35, 695, 380),
                new Rectangle(5880, 30, 625, 345),
                new Rectangle(6530, 25, 600, 335),
                new Rectangle(7150, 11, 630, 364)
            };

            rightFrames = new List<Rectangle>
            {
                new Rectangle(30, 510, 630, 365),
                new Rectangle(680, 525, 600, 335),
                new Rectangle(1305, 530, 625, 345),
                new Rectangle(2020, 534, 695, 381),
                new Rectangle(2810, 560, 630, 365),
                new Rectangle(3570, 565, 600, 335),
                new Rectangle(4265, 580, 630, 365),
                new Rectangle(5060, 570, 625, 350),
                new Rectangle(5805, 565, 635, 365),
                new Rectangle(6575, 540, 525, 360),
                new Rectangle(7195, 560, 597, 336)
            };

            leftOrigins = new List<Vector2>();
            rightOrigins = new List<Vector2>();

            foreach (var rect in leftFrames)
                leftOrigins.Add(new Vector2(rect.Width / 2f, rect.Height / 2f));
            foreach (var rect in rightFrames)
                rightOrigins.Add(new Vector2(rect.Width / 2f, rect.Height / 2f));
        }

        public override Rectangle Hitbox
        {
            get
            {
                float collisionScale = 0.2f;
                Rectangle currentFrameRect = facingRight ? rightFrames[rightFrameIndex] : leftFrames[leftFrameIndex];
                int hbWidth = (int)(currentFrameRect.Width * scale * collisionScale);
                int hbHeight = (int)(currentFrameRect.Height * scale * collisionScale);

                return new Rectangle(
                    (int)(Position.X - hbWidth / 2f),
                    (int)(Position.Y - hbHeight / 2f),
                    hbWidth,
                    hbHeight
                );
            }
        }

        public override void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            if (gm.Player != null)
            {
                bool newFacingRight = gm.Player.Position.X > Position.X;
                if (newFacingRight != facingRight)
                {
                    facingRight = newFacingRight;
                    if (facingRight)
                    {
                        rightFrameIndex = 0;
                        rightFrameTimer = 0f;
                    }
                    else
                    {
                        leftFrameIndex = 0;
                        leftFrameTimer = 0f;
                    }
                }
            }

            if (facingRight)
            {
                rightFrameTimer += dt;
                if (rightFrameTimer >= frameTime)
                {
                    rightFrameTimer -= frameTime;
                    rightFrameIndex = (rightFrameIndex + 1) % rightFrames.Count;
                }
            }
            else
            {
                leftFrameTimer += dt;
                if (leftFrameTimer >= frameTime)
                {
                    leftFrameTimer -= frameTime;
                    leftFrameIndex = (leftFrameIndex + 1) % leftFrames.Count;
                }
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            Rectangle src;
            Vector2 origin;

            if (facingRight)
            {
                src = rightFrames[rightFrameIndex];
                origin = rightOrigins[rightFrameIndex];
            }
            else
            {
                src = leftFrames[leftFrameIndex];
                origin = leftOrigins[leftFrameIndex];
            }

            Color tint = WasHit ? Color.Red : Color.White;

            sb.Draw(tex, Position, src, tint, 0f, origin, scale, SpriteEffects.None, 0f);

            // debug hitbox
            //Rectangle hb = Hitbox;
            //sb.Draw(gm.WhitePixel, new Rectangle(hb.X, hb.Y, hb.Width, 2), Color.Red);
            //sb.Draw(gm.WhitePixel, new Rectangle(hb.X, hb.Y + hb.Height - 2, hb.Width, 2), Color.Red);
            //sb.Draw(gm.WhitePixel, new Rectangle(hb.X, hb.Y, 2, hb.Height), Color.Red);
            //sb.Draw(gm.WhitePixel, new Rectangle(hb.X + hb.Width - 2, hb.Y, 2, hb.Height), Color.Red);
        }
    }
}