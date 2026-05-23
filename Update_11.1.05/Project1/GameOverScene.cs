using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class GameOverScene
    {
        GameManager gm;
        SpriteFont DefaultFont;
        Texture2D whitePixel;

        public float Alpha = 0f;
        public bool Active = false; // track if GameOver is active

        public GameOverScene(GameManager gm, SpriteFont font, Texture2D whitePixel)
        {
            this.gm = gm;
            this.DefaultFont = font;
            this.whitePixel = whitePixel;
        }

        public void Start()
        {
            Active = true;
            Alpha = 0f;
            gm.fader.Alpha = 0f;
            gm.fader.FadingIn = true;
            gm.fader.FadingOut = false;
        }

        float fadeInSpeed = 1f;
        float fadeOutSpeed = 1f;

        public void Update(GameTime gt)
        {
            if (!Active) return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            if (gm.fader.FadingIn)
            {
                Alpha += dt * fadeInSpeed;
                if (Alpha >= 1f)
                {
                    Alpha = 1f;
                    gm.fader.FadingIn = false;
                }
            }
            else if (gm.fader.FadingOut)
            {
                Alpha -= dt * fadeOutSpeed;
                if (Alpha <= 0f)
                {
                    Alpha = 0f;
                    gm.fader.FadingOut = false;
                    Active = false;

                    // รีเซ็ตระบบทั้งหมดก่อนกลับ MainMenu
                    gm.DayManager.ResetAll();
                    gm.NpcManager.ResetDailyNPCDialogue();
                    gm.Inventory.Clear();
                    gm.Inventory.KeyItems.Clear();
                    gm.Codex.Reset();
                    gm.Player.ResetAll();
                    gm.campfire.Reset();
                    gm.Level.Reset();

                    gm.GameOverStarted = false;
                    gm.StartMainMenu();
                }
            }
        }

        public void Draw(SpriteBatch sb, int screenW, int screenH)
        {
            if (!Active) return;

            sb.Draw(whitePixel, new Rectangle(0, 0, screenW, screenH), Color.Black * Alpha);

            float scale = 4f;
            string msg = "GAME OVER";
            Vector2 size = DefaultFont.MeasureString(msg);
            Vector2 pos = new Vector2(screenW / 2, screenH / 2) - (size * scale / 2f);
            sb.DrawString(DefaultFont, msg, pos, Color.Red * Alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            string btn = "[Press Enter for Main Menu]";
            Vector2 size2 = DefaultFont.MeasureString(btn);
            sb.DrawString(DefaultFont, btn, new Vector2(screenW / 2, screenH / 2 + 80) - size2 / 2, Color.White * Alpha);
        }

        public void HandleInput()
        {
            if (!Active) return;

            if (Alpha >= 1f && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gm.fader.FadingOut = true;
            }
        }
    }
}
