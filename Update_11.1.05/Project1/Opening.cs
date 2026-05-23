using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class Opening
    {
        GameManager gm;
        SpriteFont DefaultFont;
        Texture2D whitePixel;

        public float Alpha = 0f;
        public bool Active = false;
        public bool FadingOut = false;

        float fadeInSpeed = 2f;
        float fadeOutSpeed = 2f;

        public Opening(GameManager gm, SpriteFont DefaultFont, Texture2D whitePixel)
        {
            this.gm = gm;
            this.DefaultFont = DefaultFont;
            this.whitePixel = whitePixel;
        }

        public void Start()
        {
            Active = true;
            Alpha = 0f;
            FadingOut = false;
        }

        public void Update(GameTime gt)
        {
            if (!Active) return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            if (!FadingOut)
            {
                // Fade in
                Alpha += dt * fadeInSpeed;
                if (Alpha >= 1f) Alpha = 1f;
            }
            else
            {
                // Fade out
                Alpha -= dt * fadeOutSpeed;
                if (Alpha <= 0f)
                {
                    Alpha = 0f;
                    Active = false;

                    // ไป Basement หลัง fade out
                    gm.OpeningActive = false;
                    gm.StartBasement();
                }
            }
        }

        public void Draw(SpriteBatch sb, int screenW, int screenH)
        {
            if (!Active) return;

            // Background ดำ พร้อม fade
            sb.Draw(whitePixel, new Rectangle(0, 0, screenW, screenH), Color.Black * Alpha);

            // ข้อความใหญ่
            string title = "I thought that this job was about taking care of the light house";
            float bigScale = 2f;
            Vector2 titleSize = DefaultFont.MeasureString(title) * bigScale;
            Vector2 titlePos = new Vector2(screenW / 2, 100) - titleSize / 2f;
            sb.DrawString(DefaultFont, title, titlePos, Color.White * Alpha, 0f, Vector2.Zero, bigScale, SpriteEffects.None, 0f);

            // ข้อความเล็ก
            string smallText = "but...I guess I was wrong, not only I have to survive but now I must get out of here";
            float smallScale = 1.5f;
            Vector2 smallSize = DefaultFont.MeasureString(smallText) * smallScale;
            Vector2 smallPos = new Vector2(screenW / 2, titlePos.Y + titleSize.Y +180) - smallSize / 2f;
            sb.DrawString(DefaultFont, smallText, smallPos, Color.White * Alpha, 0f, Vector2.Zero, smallScale, SpriteEffects.None, 0f);
            string smallText1 = "that thing in the forest...will be my life time worse nightmare...";
            Vector2 smallPos1 = new Vector2(screenW / 2 + 80, titlePos.Y + titleSize.Y +220) - smallSize / 2f;
            sb.DrawString(DefaultFont, smallText1, smallPos1, Color.White * Alpha, 0f, Vector2.Zero, smallScale, SpriteEffects.None, 0f);
            string smallText2 = "nobody know where it came from or why is it here and I won't dare to find out";
            Vector2 smallPos2 = new Vector2(screenW / 2, titlePos.Y + titleSize.Y +260) - smallSize / 2f;
            sb.DrawString(DefaultFont, smallText2, smallPos2, Color.White * Alpha, 0f, Vector2.Zero, smallScale, SpriteEffects.None, 0f);

            // ข้อความ Enter to continue
            string continueText = "Press Enter to continue";
            float smallerScale = 1f;
            Vector2 continueSize = DefaultFont.MeasureString(continueText) * smallerScale;
            Vector2 continuePos = new Vector2(screenW - 130, screenH - 50) - continueSize / 2f;
            sb.DrawString(DefaultFont, continueText, continuePos, Color.Yellow * Alpha, 0f, Vector2.Zero, smallerScale, SpriteEffects.None, 0f);
        }
        public void HandleInput()
        {
            if (!Active) return;

            // รอให้ fade-in เสร็จ ก่อนจะ fade-out
            if (!FadingOut && Alpha >= 1f && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                FadingOut = true;
            }
        }
    }
}
