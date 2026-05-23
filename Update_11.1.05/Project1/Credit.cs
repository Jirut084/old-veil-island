using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project1
{
    public class Credit
    {
        GameManager gm;
        SpriteFont font;
        Texture2D bg;
        Texture2D whitePixel;
        KeyboardState prevKeyState;

        public float Alpha = 0f;
        public bool Active = false;

        string[] creditTexts = new string[]
        {
            "Programmer: Bannasorn , Ratchanon",
            "Designer: Thaneth",
            "Artist: Jirat , Pasit",
            "Special Thanks: Bannasorn",
        };

        public Credit(GameManager gm, SpriteFont font, Texture2D bg, Texture2D whitePixel)
        {
            this.gm = gm;
            this.font = font;
            this.bg = bg;
            this.whitePixel = whitePixel;
        }

        public void Start()
        {
            Alpha = 0f;
            Active = true;
            gm.fader.FadingIn = true;
        }

        public void Update(GameTime gt)
        {
            if (!Active) return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();

            if (gm.fader.FadingIn)
            {
                Alpha += dt * 1f;
                if (Alpha >= 1f)
                {
                    Alpha = 1f;
                    gm.fader.FadingIn = false;
                }
            }
            else if (gm.fader.FadingIn)
            {
                Alpha += dt * 1f;
                if (Alpha >= 1f)
                {
                    Alpha = 1f;
                    gm.fader.FadingOut = false;
                }
            }

            else 
            {
                if (Alpha >= 1f && !gm.fader.FadingOut)
                {
                    if (ks.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyUp(Keys.Enter))
                    {
                        Active = false;
                        gm.StartMainMenu();

                    }
                }


                
            }
            
            
                

            prevKeyState = ks;
        }

        public void Draw(SpriteBatch sb, int screenW, int screenH)
        {
            if (!Active) return;

            // BG
            sb.Draw(bg, new Rectangle(0, 0, screenW, screenH), Color.White * Alpha);

            // สี่เหลี่ยมดำโปร่ง
            sb.Draw(whitePixel, new Rectangle(0, 0, screenW, screenH), new Color(0, 0, 0, 0.5f) * Alpha);

            // Title Credit
            string title = "CREDIT";
            float scale = 3f;
            Vector2 titleSize = font.MeasureString(title) * scale;
            Vector2 pos = new Vector2(screenW / 2, 100) - titleSize / 2f;
            sb.DrawString(font, title, pos, Color.Yellow * Alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Text list
            float startY = 200;
            for (int i = 0; i < creditTexts.Length; i++)
            {
                Vector2 size = font.MeasureString(creditTexts[i]);
                sb.DrawString(font, creditTexts[i], new Vector2(screenW / 2 - size.X / 2, startY + i * 80), Color.White * Alpha);
            }

            // ปุ่ม Back
            string backText = "[BACK]";
            Vector2 backSize = font.MeasureString(backText);
            sb.DrawString(font, backText, new Vector2(screenW - backSize.X - 30, screenH - backSize.Y - 30), Color.Orange * Alpha);
        }
    }
}
