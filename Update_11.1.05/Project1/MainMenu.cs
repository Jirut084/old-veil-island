using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class MainMenu
    {
        GameManager gm;

        Texture2D bg;
        Texture2D texLogo;
        Texture2D texStart, texStartPressed;
        Texture2D texContinue, texContinuePressed;
        Texture2D texTutorial, texTutorialPressed;
        Texture2D texCredit, texCreditPressed;
        Texture2D texExit, texExitPressed;

        public float Alpha = 0f;

        string[] menuItems = new string[] { "Start", "Continue", "Tutorial", "Credit", "Exit" };
        int selectedIndex = 0;
        KeyboardState prevKeyState;

        string actionOnFadeOut = null;

        Vector2[] buttonPositions;

        public MainMenu(GameManager gm, Texture2D bg, Texture2D texLogo,
            Texture2D texStart, Texture2D texStartPressed,
            Texture2D texContinue, Texture2D texContinuePressed,
            Texture2D texTutorial, Texture2D texTutorialPressed,
            Texture2D texCredit, Texture2D texCreditPressed,
            Texture2D texExit, Texture2D texExitPressed)
        {
            this.gm = gm;
            this.bg = bg;
            this.texLogo = texLogo;
            this.texStart = texStart; this.texStartPressed = texStartPressed;
            this.texContinue = texContinue; this.texContinuePressed = texContinuePressed;
            this.texTutorial = texTutorial; this.texTutorialPressed = texTutorialPressed;
            this.texCredit = texCredit; this.texCreditPressed = texCreditPressed;
            this.texExit = texExit; this.texExitPressed = texExitPressed;

            int startY = 320;
            int gap = 70;
            buttonPositions = new Vector2[menuItems.Length];
            for (int i = 0; i < menuItems.Length; i++)
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
                        ResetGameState();
                        gm.SaveSystem?.Delete();
                        gm.StartOpeningScene();
                        gm.InMainMenu = false;
                        gm.InLighthouse = false;
                    }
                    else if (actionOnFadeOut == "Continue")
                    {
                        ResetGameState();
                        if (gm.SaveSystem != null && gm.SaveSystem.Load())
                        {
                            gm.InMainMenu = false;
                            gm.StartBasement();
                        }
                        else
                        {
                            gm.Log("No save file.");
                            gm.fader.FadingIn = true; // กลับไป MainMenu
                        }
                    }
                    else if (actionOnFadeOut == "Credit")
                    {
                        gm.CreditScene.Start();
                        gm.CreditScene.Active = true;
                    }
                    else if (actionOnFadeOut == "Tutorial")
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
                    do { selectedIndex = (selectedIndex + 1) % menuItems.Length; }
                    while (IsItemDisabled(selectedIndex));
                }
                if (ks.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up))
                {
                    do { selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length; }
                    while (IsItemDisabled(selectedIndex));
                }
                if (Alpha >= 1f && !gm.fader.FadingOut)
                {
                    if (ks.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyUp(Keys.Enter))
                    {
                        string item = menuItems[selectedIndex];
                        if (IsItemDisabled(selectedIndex)) return;

                        if (item == "Exit")
                        {
                            gm.root.Exit();
                        }
                        else
                        {
                            actionOnFadeOut = item;
                            gm.fader.FadingOut = true;
                        }
                    }
                }
            }

            prevKeyState = ks;
        }

        bool IsItemDisabled(int i)
        {
            if (menuItems[i] == "Continue")
                return gm.SaveSystem == null || !gm.SaveSystem.HasSave;
            return false;
        }

        void ResetGameState()
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
        }

        public void Draw(SpriteBatch sb, int screenW, int screenH)
        {
            if (bg != null)
                sb.Draw(bg, new Rectangle(0, 0, screenW, screenH), Color.White * Alpha);

            if (texLogo != null)
            {
                float logoScale = 2.2f;
                Vector2 logoPos = new Vector2(50, 70);
                sb.Draw(texLogo, logoPos, null, Color.White * Alpha, 0f, Vector2.Zero, logoScale, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < menuItems.Length; i++)
            {
                Texture2D texToDraw = menuItems[i] switch
                {
                    "Start"    => (i == selectedIndex ? texStartPressed : texStart),
                    "Continue" => (i == selectedIndex ? texContinuePressed : texContinue),
                    "Tutorial" => (i == selectedIndex ? texTutorialPressed : texTutorial),
                    "Credit"   => (i == selectedIndex ? texCreditPressed : texCredit),
                    "Exit"     => (i == selectedIndex ? texExitPressed : texExit),
                    _          => texStart
                };

                float a = Alpha * (IsItemDisabled(i) ? 0.4f : 1f);
                sb.Draw(texToDraw, buttonPositions[i], null, Color.White * a, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }
        }

    }
}
