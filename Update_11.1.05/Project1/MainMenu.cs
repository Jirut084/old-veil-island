using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project1
{
    public class MainMenu
    {
        GameManager gm;

        Texture2D bg;

        Texture2D texLogo;
        Texture2D texStart, texStartPressed;
        Texture2D texTutorial, texTutorialPressed;
        Texture2D texCredit, texCreditPressed;
        Texture2D texExit, texExitPressed;

        public float Alpha = 0f;

        string[] menuItems = new string[] { "Start", "Tutorial", "Credit", "Exit" };
        int selectedIndex = 0;
        KeyboardState prevKeyState;

        string actionOnFadeOut = null;

        Vector2[] buttonPositions;

        // scale สำหรับ logo


        public MainMenu(GameManager gm, Texture2D bg, Texture2D texLogo,Texture2D texStart, Texture2D texStartPressed,Texture2D texTutorial, Texture2D texTutorialPressed,Texture2D texCredit, Texture2D texCreditPressed,Texture2D texExit, Texture2D texExitPressed)
        {
            this.gm = gm;
            this.bg = bg;
            this.texLogo = texLogo;

            this.texStart = texStart; this.texStartPressed = texStartPressed;
            this.texTutorial = texTutorial; this.texTutorialPressed = texTutorialPressed;
            this.texCredit = texCredit; this.texCreditPressed = texCreditPressed;
            this.texExit = texExit; this.texExitPressed = texExitPressed;

            int startY = 350;
            int gap = 80;
            buttonPositions = new Vector2[4];
            for (int i = 0; i < 4; i++)
                buttonPositions[i] = new Vector2(100, startY + i * gap);
        }
        

        public void Start()
        {
            Alpha = 0f;
            gm.fader.FadingIn = true;
            gm.fader.FadingOut = false;
            actionOnFadeOut = null;
        }

        float fadeInSpeed = 1f;
        float fadeOutSpeed = 1f;

        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();

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

                    if (actionOnFadeOut == "Start")
                    {
                        gm.Player.ResetAll();
                        gm.Inventory.Clear();
                        gm.Inventory.KeyItems.Clear();
                        gm.DayManager.ResetAll();
                        gm.NpcManager.ResetDailyNPCDialogue();
                        gm.Level.Reset();
                        gm.Codex.Reset();
                        gm.campfire.Reset();
                        gm.Hud.Reset();

                        gm.StartOpeningScene();
                        //gm.StartBasement();
                        //gm.Player.Position = gm.basement.PlayerStart;
                        gm.InMainMenu = false;
                        gm.InLighthouse = false;
                    }
                    if (actionOnFadeOut == "Credit")
                    {
                        gm.CreditScene.Start();
                        gm.CreditScene.Active = true;
                    }
                    if (actionOnFadeOut == "Tutorial")
                    {
                        gm.TutorialScene.Start();
                        gm.TutorialScene.Active = true;
                    }

                    actionOnFadeOut = null;
                }
            }
            else
            {
                if (ks.IsKeyDown(Keys.Down) && prevKeyState.IsKeyUp(Keys.Down))
                {
                    selectedIndex++;
                    if (selectedIndex >= menuItems.Length) selectedIndex = 0;
                }
                if (ks.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up))
                {
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = menuItems.Length - 1;
                }
                if (Alpha >= 1f && !gm.fader.FadingOut)
                {
                    if (ks.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyUp(Keys.Enter))
                    {
                        if (menuItems[selectedIndex] == "Start")
                        {
                            actionOnFadeOut = "Start";
                            gm.fader.FadingOut = true;
                        }
                        else if (menuItems[selectedIndex] == "Tutorial")
                        {
                            actionOnFadeOut = "Tutorial";
                            gm.fader.FadingOut = true;
                        }
                        else if (menuItems[selectedIndex] == "Credit")
                        {
                            actionOnFadeOut = "Credit";
                            gm.fader.FadingOut = true;
                        }
                        else if (menuItems[selectedIndex] == "Exit")
                        {
                            gm.root.Exit();
                        }
                    }
                }
            }

            prevKeyState = ks;
        }


        //float logoScale = 1f;
        //public void SetLogoScale(float scale)
        //{
        //    logoScale = scale;
        //}
        public void Draw(SpriteBatch sb, int screenW, int screenH)
        {
            // วาด background
            if (bg != null)
                sb.Draw(bg, new Rectangle(0, 0, screenW, screenH), Color.White * Alpha);

            // วาด logo ซ้ายบน
            if (texLogo != null)
            {
                float logoScale = 2.2f;
                Vector2 logoOrigin = Vector2.Zero;
                Vector2 logoPos = new Vector2(50, 70); // ซ้ายบน
                sb.Draw(texLogo, logoPos, null, Color.White * Alpha, 0f, logoOrigin, logoScale, SpriteEffects.None, 0f);
            }

            // วาดปุ่ม
            for (int i = 0; i < menuItems.Length; i++)
            {
                Texture2D texToDraw = i switch
                {
                    0 => (i == selectedIndex ? texStartPressed : texStart),
                    1 => (i == selectedIndex ? texTutorialPressed : texTutorial),
                    2 => (i == selectedIndex ? texCreditPressed : texCredit),
                    3 => (i == selectedIndex ? texExitPressed : texExit),
                    _ => texStart
                };

                sb.Draw(texToDraw, buttonPositions[i], null, Color.White * Alpha, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }
        }

    }
}
