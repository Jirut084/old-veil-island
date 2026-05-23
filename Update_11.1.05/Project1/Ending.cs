using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

namespace Project1
{
    public class Ending
    {
        GameManager gm;
        SpriteFont DefaultFont;
        Texture2D whitePixel;

        public float Alpha = 0f;
        public bool Active = false;
        public bool FadingOut = false;

        float fadeInSpeed = 2f;
        float fadeOutSpeed = 2f;

        public Ending(GameManager gm, SpriteFont font, Texture2D whitePixel)
        {
            this.gm = gm;
            this.DefaultFont = font;
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
                Alpha += dt * fadeInSpeed;
                if (Alpha >= 1f) Alpha = 1f;
            }
            else
            {
                Alpha -= dt * fadeOutSpeed;
                if (Alpha <= 0f)
                {
                    Alpha = 0f;
                    Active = false;

                    // Reset & go main menu
                    gm.DayManager.ResetAll();
                    gm.NpcManager.ResetDailyNPCDialogue();
                    gm.Inventory.Clear();
                    gm.Inventory.KeyItems.Clear();
                    gm.Codex.Reset();
                    gm.Player.ResetAll();
                    gm.campfire.Reset();
                    gm.Level.Reset();

                    gm.EndingActive = false;
                    gm.StartMainMenu();
                }
            }
        }

        public void Draw(SpriteBatch sb, int screenW, int screenH)
        {
            if (!Active) return;

            // Fade background
            sb.Draw(whitePixel, new Rectangle(0, 0, screenW, screenH), Color.Black * Alpha);

            // Big message
            string msg1 = "THANK YOU FOR PLAYING"; 
            float bigScale = 4f;
            Vector2 size1 = DefaultFont.MeasureString(msg1) * bigScale;
            Vector2 pos1 = new Vector2(screenW / 2, screenH / 2 - 200) - size1 / 2f;
            sb.DrawString(DefaultFont, msg1, pos1, Color.Yellow * Alpha, 0f, Vector2.Zero, bigScale, SpriteEffects.None, 0f);
            
            // Small message
            string msg2 = "END OF Prototype";
            float smallScale = 1f;
            Vector2 size2 = DefaultFont.MeasureString(msg2) * smallScale;
            Vector2 pos2 = new Vector2(screenW / 2, screenH / 2 + 30) - size2 / 2f;
            sb.DrawString(DefaultFont, msg2, pos2, Color.White * Alpha, 0f, Vector2.Zero, smallScale, SpriteEffects.None, 0f);

            string msg3 = "THANK YOU THANK YOU THANK YOU THANK YOU THANK YOU THANK YOU THANK YOU";
            Vector2 size3 = DefaultFont.MeasureString(msg3) * smallScale;
            Vector2 pos3 = new Vector2(screenW / 2, screenH / 2 -60) - size3 / 2f;
            sb.DrawString(DefaultFont, msg3, pos3, Color.Red * Alpha, 0f, Vector2.Zero, smallScale, SpriteEffects.None, 0f);

            
            // Button prompt
            string btn = "[Press E for Main Menu]";
            Vector2 size4 = DefaultFont.MeasureString(btn) * smallScale;
            Vector2 pos4 = new Vector2(screenW / 2, screenH / 2 + 80) - size4 / 2f;
            sb.DrawString(DefaultFont, btn, pos4, Color.White * Alpha);
        }

        public void HandleInput()
        {
            if (!Active) return;

            // รอให้ fade-in เสร็จสมบูรณ์ก่อน
            if (!FadingOut && Alpha >= 1f && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                FadingOut = true;
            }
        }
    }
}
