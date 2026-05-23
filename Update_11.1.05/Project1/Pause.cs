using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project1
{
    public class Pause
    {
        private GameManager gm;
        private SpriteFont font;
        private int selectedIndex = 0;

        Texture2D texMainmenu, texMainmenu_pressed;
        Texture2D texContinue, texContinue_pressed;

        // ฟีดแบ็ก save
        float saveMessageTimer = 0f;
        string saveMessage = "";
        Color saveMessageColor = Color.LimeGreen;

        private string[] menuItems = new string[]
        {
            "Continue",
            "Save Game",
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
                MediaPlayer.Pause();
            else
                MediaPlayer.Resume();
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (saveMessageTimer > 0f) saveMessageTimer -= dt;

            if (!IsPaused) return;

            // Quick-save ด้วย F5 ระหว่าง pause
            if (InputManager.KeyPressed(Keys.F5))
                TrySave();

            if (InputManager.KeyPressed(Keys.Down))
            {
                selectedIndex = (selectedIndex + 1) % menuItems.Length;
            }
            if (InputManager.KeyPressed(Keys.Up))
            {
                selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
            }

            if (InputManager.KeyPressed(Keys.Enter))
            {
                string item = menuItems[selectedIndex];
                if (item == "Continue")
                {
                    TogglePause();
                }
                else if (item == "Save Game")
                {
                    TrySave();
                }
                else if (item == "Main Menu")
                {
                    gm.StartMainMenu();
                    TogglePause();
                }
            }
        }

        void TrySave()
        {
            if (gm.SaveSystem == null)
            {
                ShowMessage("Save system unavailable.", Color.OrangeRed);
                return;
            }
            bool ok = gm.SaveSystem.Save();
            if (ok)
                ShowMessage("Game saved!", Color.LimeGreen);
            else
                ShowMessage("Save failed.", Color.OrangeRed);
        }

        void ShowMessage(string text, Color color)
        {
            saveMessage = text;
            saveMessageColor = color;
            saveMessageTimer = 2f;
        }

        public void Draw(SpriteBatch sb, int screenWidth, int screenHeight)
        {
            if (!IsPaused)
            {
                // toast หลังออกจาก pause ก็ยังโชว์อยู่ ถ้ายังไม่หมดเวลา
                DrawSaveToast(sb, screenWidth, screenHeight);
                return;
            }

            // ทำพื้นหลังให้มืดลงเล็กน้อย
            sb.Draw(gm.WhitePixel, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black * 0.45f);

            string title = "PAUSED";
            float pauseScale = 1.5f;
            Vector2 titleSize = font.MeasureString(title) * pauseScale;
            sb.DrawString(font, title, new Vector2((screenWidth - titleSize.X) / 2, 100), Color.Yellow, 0f, Vector2.Zero, pauseScale, SpriteEffects.None, 0f);

            int baseY = 300;
            int gap = 80;

            for (int i = 0; i < menuItems.Length; i++)
            {
                string item = menuItems[i];
                bool selected = i == selectedIndex;

                if (item == "Continue")
                {
                    var tex = selected ? texContinue_pressed : texContinue;
                    var pos = new Vector2((screenWidth - tex.Width * 1.5f) / 2, baseY + i * gap);
                    sb.Draw(tex, pos, null, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                }
                else if (item == "Main Menu")
                {
                    var tex = selected ? texMainmenu_pressed : texMainmenu;
                    var pos = new Vector2((screenWidth - tex.Width * 1.5f) / 2, baseY + i * gap);
                    sb.Draw(tex, pos, null, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                }
                else
                {
                    // วาดเป็น text button (ไม่มี texture สำหรับ Save)
                    DrawTextButton(sb, item, baseY + i * gap, screenWidth, selected);
                }
            }

            // ปุ่มลัด
            string hint = "Press F5 to quick save";
            Vector2 hintSize = font.MeasureString(hint);
            sb.DrawString(font, hint, new Vector2((screenWidth - hintSize.X) / 2, baseY + menuItems.Length * gap + 30), Color.Gray);

            DrawSaveToast(sb, screenWidth, screenHeight);
        }

        void DrawTextButton(SpriteBatch sb, string label, int y, int screenWidth, bool selected)
        {
            float scale = 1.5f;
            Vector2 size = font.MeasureString(label) * scale;
            float btnW = size.X + 60;
            float btnH = size.Y + 16;
            var rect = new Rectangle(
                (int)((screenWidth - btnW) / 2),
                y,
                (int)btnW,
                (int)btnH
            );

            Color bg = selected ? new Color(255, 220, 100) : new Color(80, 60, 40);
            Color border = selected ? Color.Yellow : Color.Gray;
            Color textColor = selected ? Color.Black : Color.White;

            sb.Draw(gm.WhitePixel, rect, bg);
            // border (top/bottom/left/right)
            sb.Draw(gm.WhitePixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), border);
            sb.Draw(gm.WhitePixel, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), border);
            sb.Draw(gm.WhitePixel, new Rectangle(rect.X, rect.Y, 2, rect.Height), border);
            sb.Draw(gm.WhitePixel, new Rectangle(rect.Right - 2, rect.Y, 2, rect.Height), border);

            Vector2 textPos = new Vector2(rect.X + (rect.Width - size.X) / 2, rect.Y + (rect.Height - size.Y) / 2);
            sb.DrawString(font, label, textPos, textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        void DrawSaveToast(SpriteBatch sb, int screenWidth, int screenHeight)
        {
            if (saveMessageTimer <= 0f || string.IsNullOrEmpty(saveMessage)) return;

            float alpha = MathHelper.Clamp(saveMessageTimer / 2f, 0f, 1f);
            Vector2 size = font.MeasureString(saveMessage);
            float scale = 1.2f;
            size *= scale;
            int padding = 14;

            var rect = new Rectangle(
                (int)((screenWidth - size.X) / 2 - padding),
                screenHeight - 120,
                (int)(size.X + padding * 2),
                (int)(size.Y + padding)
            );
            sb.Draw(gm.WhitePixel, rect, Color.Black * (0.7f * alpha));
            Vector2 pos = new Vector2(rect.X + padding, rect.Y + padding / 2);
            sb.DrawString(font, saveMessage, pos, saveMessageColor * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
