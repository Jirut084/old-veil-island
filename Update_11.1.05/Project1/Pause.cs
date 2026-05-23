using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Project1
{
    public class Pause
    {
        private GameManager gm;
        private SpriteFont font;
        private int selectedIndex = 0;

        Texture2D texMainmenu, texMainmenu_pressed;
        Texture2D texContinue, texContinue_pressed;

        private string[] menuItems = new string[]
        {
            "Continue",
            "Main Menu"
        };

        public bool IsPaused { get; private set; } = false;

        public Pause(GameManager gm, SpriteFont font, Texture2D texContinue, Texture2D texContinue_pressed, Texture2D texMainmenu, Texture2D texMainmenu_pressed)
        {
            this.gm = gm;
            this.font = font;

            this.texContinue = texContinue; this.texContinue_pressed = texContinue_pressed;
            this.texMainmenu = texMainmenu; this.texMainmenu_pressed = texMainmenu_pressed;
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            selectedIndex = 0;

            if (IsPaused)
                MediaPlayer.Pause(); // หยุดเพลง
            else
                MediaPlayer.Resume(); // เล่นเพลงต่อ
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPaused) return;

            if (InputManager.KeyPressed(Keys.Down))
            {
                selectedIndex++;
                if (selectedIndex >= menuItems.Length) selectedIndex = 0;
            }
            if (InputManager.KeyPressed(Keys.Up))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = menuItems.Length - 1;
            }

            if (InputManager.KeyPressed(Keys.Enter))
            {
                if (selectedIndex == 0) // Continue
                {
                    TogglePause();
                }

                else if (selectedIndex == 1) // Main Menu
                {
                    gm.StartMainMenu();
                    TogglePause();
                }
            }
        }

        public void Draw(SpriteBatch sb, int screenWidth, int screenHeight)
        {
            if (!IsPaused) return;

            float alpha = 1f;

            string title = "PAUSED";
            float pauseScale = 1.5f;
            Vector2 titleSize = font.MeasureString(title) * pauseScale;
            sb.DrawString(font, title, new Vector2((screenWidth - titleSize.X) / 2, 100), Color.Yellow, 0f, Vector2.Zero, pauseScale, SpriteEffects.None, 0f);

            Texture2D texToDraw;
            Vector2 pos;

            // วาดปุ่ม
            for (int i = 0; i < menuItems.Length; i++)
            {
                texToDraw = i == 0 ? (i == selectedIndex ? texContinue_pressed : texContinue)
                                    : (i == selectedIndex ? texMainmenu_pressed : texMainmenu);

                pos = new Vector2((screenWidth - texToDraw.Width * 1.5f) / 2, 330 + i * 80);

                sb.Draw(texToDraw, pos, null, Color.White * alpha, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }
        }
    }
}
