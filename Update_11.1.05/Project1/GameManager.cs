using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Project1.NPC;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Project1
{
    public class GameManager
    {
        public GameRoot root;
        public ContentManager content;
        public GraphicsDevice graphics;
        public SpriteBatch spriteBatch;

        public float GameOverToMenuTimer = 0f; // วินาทีรอหลังกด Enter
        public float GameOverToMenuDelay = 0.5f; // รอ 0.5 วินาที


        // Core Systems
        public Player Player;
        public ProceduralLevel Level;
        public HUD Hud;
        public Inventory Inventory;
        public Minimap Minimap;
        public Codex Codex;
        public NPCManager NpcManager;
        public DayManager DayManager;
        public SaveSystem SaveSystem;
        public DialogueSystem DialogueSystem;
        public PickupItem PickupItem;
        public Ending Ending;
        public Opening OpeningScene;

        // Assets
        public Texture2D TexLogo, TexPlayer, TexEnemy, TexEnemy1, TexEnemy2, TexGrass, TexGrass2, TexPrep, TexTrap, TexPistol, TexBullet, TexCodexBG, TexHUDIcons, TexCampfire, TexKB, TexStart, TexContinue, TexCredit, TexTuto, TexExit, TexMainmenu, TexStart_pressed, TexContinue_pressed, TexCredit_pressed, TexTuto_pressed, TexExit_pressed, TexMainmenu_pressed;
        public SpriteFont DefaultFont;
        public Effect MultiplyLight;
        public Texture2D WhitePixel;

        // State
        public bool InSafeRoom = false;
        public bool InPrepZone = false;
        public bool InBasement = false;
        public bool InMainLevel = false;

        public bool EndingActive = false;
        public bool GameOverStarted = false;
        public bool OpeningActive = false;

        public AxeWeapon Axe;
        public ScreenFader fader;
        public Campfire campfire;
        public Basement basement;
        public MainMenu mainMenu;
        public PrepRoom prepareroom;
        public MainMenu MainMenu;
        public Pause PauseMenu;
        public GameOverScene GameOver;
        public Credit CreditScene;
        public Tutorial TutorialScene;


        public RenderTarget2D SceneTarget;
        public Texture2D LightMask;
        public float AmbientDarkness = 0.11f;

        public bool pendingSceneChangeToPrep = false;
        public bool pendingSceneChangeToMain = false;
        public bool pendingSceneChangeToBase = false;
        public bool pendingSceneFromMain = false;
        public bool pendingSceneToOpening = false;

        public Lighthouse lighthouse;
        public bool InLighthouse = false;

        public bool pendingSceneChangeToLighthouse = false;
        public bool pendingSceneChangeToBasement = false; // เวลากลับ



        public bool InMainMenu = true;
        public bool pendingSceneChangeToMainMenu = false;
        public bool pendingFromLighthouse = false; // เก็บสถานะ spawn มาจาก Lighthouse
        public bool pendingFromBasement = false;
        public bool pendingNextLevel = false;

        //sound
        public SoundEffect WalkDirtSfx;
        public SoundEffect WalkWoodSfx;
        public SoundEffect PickUp;
        public SoundEffect Hit;
        public SoundEffect Shoot;

        public Song BgBasement;
        //public Song BgLighthouse;
        //public Song BgPrepRoom;
        public Song BgMainLevel;
        public Song BgMainMenu;


        public GameManager(GameRoot root, ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            this.root = root;
            this.content = content;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
        }

        public void LoadAll()
        {
            // Load textures
            TexLogo = content.Load<Texture2D>("logo");
            TexPlayer = content.Load<Texture2D>("cha_walk_sheet_2");
            TexEnemy = content.Load<Texture2D>("monster1_1_walk");
            TexEnemy1 = content.Load<Texture2D>("monster4_1_walk");
            TexEnemy2 = content.Load<Texture2D>("Henmil");

            TexGrass = content.Load<Texture2D>("map1_1");
            TexGrass2 = content.Load<Texture2D>("map2");

            TexTrap = content.Load<Texture2D>("Trap");
            TexPistol = content.Load<Texture2D>("pistol");
            TexBullet = content.Load<Texture2D>("WeaponBullet");
            TexCodexBG = content.Load<Texture2D>("CodexBG");
            TexCampfire = content.Load<Texture2D>("ani_campfire");
            TexKB = content.Load<Texture2D>("KB");

            ////buttonTexture
            TexStart = content.Load<Texture2D>("B_start");
            TexContinue = content.Load<Texture2D>("B_continue");
            TexTuto = content.Load<Texture2D>("B_tutorial");
            TexCredit = content.Load<Texture2D>("B_credit");
            TexExit = content.Load<Texture2D>("B_exit");
            TexMainmenu = content.Load<Texture2D>("B_mainmenu");

            TexStart_pressed = content.Load<Texture2D>("B_start_pressed");
            TexContinue_pressed = content.Load<Texture2D>("B_continue_pressed");
            TexTuto_pressed = content.Load<Texture2D>("B_tutorial_pressed");
            TexCredit_pressed = content.Load<Texture2D>("B_credit_pressed");
            TexExit_pressed = content.Load<Texture2D>("B_exit_pressed");
            TexMainmenu_pressed = content.Load<Texture2D>("B_mainmenu_pressed");


            TexHUDIcons = content.Load<Texture2D>("HUD_Icons");
            DefaultFont = content.Load<SpriteFont>("DefaultFont");
            MultiplyLight = content.Load<Effect>("MultiplyLight");

            WalkDirtSfx = content.Load<SoundEffect>("walk_dirt");
            WalkWoodSfx = content.Load<SoundEffect>("walk_wood");
            PickUp = content.Load<SoundEffect>("pickup");
            Hit = content.Load<SoundEffect>("hit");
            Shoot = content.Load<SoundEffect>("gunshot");

            BgBasement = content.Load<Song>("basement");
            //BgLighthouse = content.Load<Song>("lighthouse");
            //BgPrepRoom = content.Load<Song>("prepzone");
            BgMainLevel = content.Load<Song>("mainlevel");
            BgMainMenu = content.Load<Song>("mainmenu");
            MediaPlayer.IsRepeating = true;




            lighthouse = new Lighthouse(this, content.Load<Texture2D>("floor1_4"), content.Load<Texture2D>("floor2-2"));

            WhitePixel = new Texture2D(graphics, 1, 1);
            WhitePixel.SetData(new[] { Color.White });
            // Render target
            PresentationParameters pp = graphics.PresentationParameters;
            SceneTarget = new RenderTarget2D(graphics, pp.BackBufferWidth, pp.BackBufferHeight);

            // Basement setup
            basement = new Basement(this, content.Load<Texture2D>("lighthouse_out"));
            prepareroom = new PrepRoom(this, content.Load<Texture2D>("preparezone_0"));

            // MAIN MENU
            MainMenu = new MainMenu(this, content.Load<Texture2D>("bg "), TexLogo,
                TexStart, TexStart_pressed,
                TexContinue, TexContinue_pressed,
                TexTuto, TexTuto_pressed,
                TexCredit, TexCredit_pressed,
                TexExit, TexExit_pressed);
            CreditScene = new Credit(this, DefaultFont, content.Load<Texture2D>("bg "), WhitePixel);
            TutorialScene = new Tutorial(this, DefaultFont, content.Load<Texture2D>("bg "), TexKB, WhitePixel);
            // Systems
            Inventory = new Inventory(this);
            Player = new Player(this, TexPlayer, DefaultFont);
            Level = new ProceduralLevel(this);
            Minimap = new Minimap(this);
            Hud = new HUD(this);
            Codex = new Codex(this, TexCodexBG);
            NpcManager = new NPCManager(this);
            NpcManager.SpawnDailyNPCs();
            DayManager = new DayManager(this);
            SaveSystem = new SaveSystem(this);
            DialogueSystem = new DialogueSystem(this);
            Axe = new AxeWeapon(this);
            //PickupItem = new PickupItem(this);

            PauseMenu = new Pause(this, DefaultFont, TexContinue, TexContinue_pressed, TexMainmenu, TexMainmenu_pressed);
            GameOver = new GameOverScene(this, DefaultFont, WhitePixel);
            Ending = new Ending(this, DefaultFont, WhitePixel);
            OpeningScene = new Opening(this, DefaultFont, WhitePixel);

            fader = new ScreenFader();
            MainMenu.Start();
            PlayBgMusic(BgMainMenu, 1f);

            InBasement = true;
            Player.Position = basement.PlayerStart;

            campfire = new Campfire(this, TexCampfire);

        }

        public void Log(string text)
        {
            Hud.Log(text);
        }

        public void StartBasement(bool fromPrep = false, bool fromLighthouse = false)
        {
            InBasement = true;
            InPrepZone = false;
            InMainLevel = false;
            InLighthouse = false;

            fader.FadingIn = true;

            // ตรวจสอบก่อนว่าเพลงกำลังเล่นหรือไม่
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != BgBasement)
                PlayBgMusic(BgBasement, 0.4f);

            if (fromPrep)
                Player.Position = basement.SpawnFromPrep;
            else if (fromLighthouse)
                Player.Position = new Vector2(basement.LightHouseDoor.X + basement.LightHouseDoor.Width / 2 + 5, basement.LightHouseDoor.Y + basement.LightHouseDoor.Height + 10);
            else
                Player.Position = basement.PlayerStart;

            // Auto-save เฉพาะตอนกลับจากด่าน (มี progress ใหม่)
            if (fromPrep || fromLighthouse)
                SaveSystem?.Save();
        }


        public void StartPrepZone(bool fromMainLevel = false, bool fromBasement = false)
        {
            InMainLevel = false;
            InBasement = false;
            InLighthouse = false;
            InPrepZone = true;

            fader.FadingIn = true;

            PlayBgMusic(BgMainLevel, 0.6f);

            Level.Reset();
            if (fromMainLevel)
                Player.Position = new Vector2(100, 250);
            else if (fromBasement)
                Player.Position = prepareroom.SpawnFromBasement; // spawn ข้างประตู Basement
            else
                Player.Position = new Vector2(1350, 300);
        }

        public void StartMainLevel()
        {

            GameOverStarted = false;
            EndingActive = false;
            InBasement = false;
            InLighthouse = false;
            InPrepZone = false;
            InMainLevel = true;

            fader.FadingIn = true;

            PlayBgMusic(BgMainLevel, 0.6f);

            //campfire.remainingTime = Math.Max(campfire.remainingTime, 1f);
            if (DayManager.Day == 1)
                Level = new ProceduralLevel(this);
            //else if (DayManager.Day == 2)
            //    Level = new ProceduralLevel2(this);

            Level.GenerateLevelSeed((int)DateTime.Now.Ticks);
            Level.SpawnInitialEntities();

            Player.Position = new Vector2(3900, 300);

            DayManager.StartDay();
            NpcManager.SpawnDailyNPCs();
        }


        //public void StartBase()
        //{
        //    InMainLevel = false;
        //    InBasement = true;
        //    InLighthouse = false;
        //    InPrepZone = false;
        //    fader.FadingIn = true;

        //    PlayBgMusic(BgBasement, 0.5f);

        //    Player.Position = new Vector2(100, 300);
        //    Level.Reset();
        //}

        public void StartLighthouse(bool fromBasement = false)
        {
            InBasement = false;
            InPrepZone = false;
            InMainLevel = false;
            InLighthouse = true;

            MediaPlayer.Stop();
            fader.FadingIn = true;
            Level.Reset();
            lighthouse.Reset();
            if (fromBasement)
                Player.Position = lighthouse.PlayerStart;

        }
        public void StartMainMenu()
        {
            // Reset ระบบทั้งหมด
            //Player.ResetAll();
            //Inventory.Clear();
            //DayManager.ResetAll();
            //NpcManager.ResetDailyNPCDialogue();
            //Codex.Reset();
            //Level.Reset();
            //Level.Projectiles.Clear();

            //GameOverStarted = false;

            InBasement = false;
            InPrepZone = false;
            InMainLevel = false;
            InLighthouse = false;
            fader.FadingIn = true;

            PlayBgMusic(BgMainMenu, 1f);

            InMainMenu = true;

            MainMenu.Start();
        }
        public void StartOpeningScene()
        {
            InBasement = false;
            InPrepZone = false;
            InMainLevel = false;
            InLighthouse = false;
            fader.FadingIn = true;

            OpeningActive = true;
            MediaPlayer.Pause();


            OpeningScene.Start();
        }
        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputManager.Update();

            // เปิด Opening ก่อน
            if (OpeningActive)
            {
                OpeningScene.Update(gameTime);
                OpeningScene.HandleInput();
                return;
            }

            if (!InMainMenu && InputManager.KeyPressed(Keys.Escape))
            {
                PauseMenu.TogglePause();
            }

            if (PauseMenu.IsPaused)
            {
                MediaPlayer.Pause();
                PauseMenu.Update(gameTime);
                return;
            }

            if (DialogueSystem.Active)
            {
                DialogueSystem.Update(gameTime);
                return;
            }

            if (Player.IsDead || campfire.FireOff)
            {
                MediaPlayer.Stop();
                if (!GameOverStarted)
                {
                    GameOver.Start();
                    GameOverStarted = true;
                }
                GameOver.Update(gameTime);
                GameOver.HandleInput();
                return;
            }

            if (EndingActive)
            {
                Ending.Update(gameTime);
                Ending.HandleInput();
                return;
            }

            if (CreditScene.Active)
            {
                CreditScene.Update(gameTime);
                return;
            }

            if (TutorialScene.Active)
            {
                TutorialScene.Update(gameTime);
                return;
            }

            if (InMainMenu)
            {
                MainMenu.Update(gameTime);
                if (CreditScene.Active) CreditScene.Update(gameTime);
                if (TutorialScene.Active) TutorialScene.Update(gameTime);
                return;
            }

            // Update ตาม Scene
            if (InBasement)
            {
                Player.Update(gameTime);
                Hud.Update(gameTime);
                basement.Update(gameTime);
                NpcManager.Update(gameTime);
                Codex.Update(gameTime);
                campfire.Update(gameTime);
                PauseMenu.Update(gameTime);
                Axe.Update(gameTime);
                Inventory.Update(gameTime);
            }
            else if (InPrepZone)
            {
                Player.Update(gameTime);
                Hud.Update(gameTime);
                prepareroom.Update(gameTime);
                Codex.Update(gameTime);
                campfire.Update(gameTime);
                PauseMenu.Update(gameTime);
                Axe.Update(gameTime);
                Inventory.Update(gameTime);
            }
            else if (InLighthouse)
            {
                Player.Update(gameTime);
                Hud.Update(gameTime);
                lighthouse.Update(gameTime);
                Codex.Update(gameTime);
                campfire.Update(gameTime);
                PauseMenu.Update(gameTime);
                Axe.Update(gameTime);
                Inventory.Update(gameTime);
            }
            else if (InMainLevel)
            {
                DayManager.Update(gameTime);
                Level.Update(gameTime);
                Player.Update(gameTime);
                Hud.Update(gameTime);
                Codex.Update(gameTime);
                Minimap.Update(gameTime);
                campfire.Update(gameTime);
                PauseMenu.Update(gameTime);
                Axe.Update(gameTime);
                Inventory.Update(gameTime);

                var fence1 = Level.fences.FirstOrDefault(f => f.Texture.Name == "fench1");
                if (Level.ExitDoor.Intersects(new Rectangle((int)Player.Position.X, (int)Player.Position.Y, 32, 64))
                    && fence1 != null && fence1.Destroyed)
                {
                    DayManager.NextDay();
                    pendingSceneChangeToLighthouse = true;
                    fader.FadingOut = true;
                }
            }

            // อัปเดต fader
            fader.Update(gameTime);

            if (fader.Alpha >= 1f)
            {
                if (pendingSceneChangeToPrep)
                {
                    StartPrepZone(fromMainLevel: pendingSceneFromMain, fromBasement: pendingFromBasement);
                    fader.FadingIn = true;
                    pendingSceneChangeToPrep = false;
                    pendingSceneFromMain = false;
                    pendingFromBasement = false;
                }
                else if (pendingSceneChangeToMain)
                {
                    StartMainLevel();
                    fader.FadingIn = true;
                    pendingSceneChangeToMain = false;
                }
                else if (pendingSceneChangeToBase)
                {
                    StartBasement(fromPrep: true);
                    fader.FadingIn = true;
                    pendingSceneChangeToBase = false;
                }
                else if (pendingSceneChangeToLighthouse)
                {
                    StartLighthouse(fromBasement: pendingFromBasement);
                    fader.FadingIn = true;
                    pendingSceneChangeToLighthouse = false;
                    pendingFromBasement = false;
                }
                else if (pendingSceneChangeToBasement)
                {
                    StartBasement(fromLighthouse: true);
                    fader.FadingIn = true;
                    pendingSceneChangeToBasement = false;
                    pendingFromLighthouse = false;
                    pendingNextLevel = true;
                    pendingSceneToOpening = false;

                    // เพิ่มตรงนี้เพื่อให้มั่นใจว่าเพลง Basement เล่นต่อ
                    PlayBgMusic(BgBasement, 0.4f);
                }
                else if (pendingNextLevel && !fader.FadingOut && !fader.FadingIn)
                {
                    pendingSceneChangeToMain = true;
                    pendingNextLevel = false;
                }
                else if (pendingSceneChangeToMainMenu)
                {
                    fader.FadingIn = true;
                    fader.FadingOut = false;
                    Player.ResetAll();
                    Inventory.Clear();
                    Inventory.KeyItems.Clear();
                    DayManager.ResetAll();
                    NpcManager.ResetDailyNPCDialogue();
                    Codex.Reset();
                    Level.Reset();
                    Level.Projectiles.Clear();
                    campfire.Reset();
                    GameOverStarted = false;
                    InBasement = false;
                    InPrepZone = false;
                    InMainLevel = false;
                    InLighthouse = false;
                    InMainMenu = true;
                    MainMenu.Start();
                    pendingSceneChangeToMainMenu = false;
                }
            }
        }


        public void Draw(GameTime gameTime)
        {


            // MainMenu, GameOver, Ending, Credit, Tutorial
            if (InMainMenu || ((Player.IsDead || campfire.FireOff) && GameOverStarted) || CreditScene.Active || TutorialScene.Active || EndingActive || OpeningActive)
            {
                graphics.SetRenderTarget(null);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

                if (EndingActive) Ending.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);
                else if (OpeningActive) OpeningScene.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);
                else if ((Player.IsDead || campfire.FireOff) && GameOverStarted) GameOver.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);
                else if (CreditScene.Active) CreditScene.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);
                else if (TutorialScene.Active) TutorialScene.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);
                else if (InMainMenu) MainMenu.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);

                if (!EndingActive) fader.Draw(spriteBatch, WhitePixel, graphics.Viewport.Width, graphics.Viewport.Height);

                spriteBatch.End();
                return;
            }

            // Render Target สำหรับ gameplay
            graphics.SetRenderTarget(SceneTarget);
            graphics.Clear(Color.Black);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetTransform(root, gameTime));

            if (InBasement)
            {
                basement.Draw(spriteBatch);
                NpcManager.Draw(spriteBatch, gameTime);
                campfire.Draw(spriteBatch, DefaultFont);
                Player.Draw(spriteBatch, gameTime);
                Axe.Draw(spriteBatch);
            }
            else if (InPrepZone)
            {
                prepareroom.Draw(spriteBatch);
                Player.Draw(spriteBatch, gameTime);
                Axe.Draw(spriteBatch);
            }
            else if (InLighthouse)
            {
                lighthouse.Draw(spriteBatch);
                Player.Draw(spriteBatch, gameTime);
                Axe.Draw(spriteBatch);
            }
            else
            {
                Level.Draw(spriteBatch);
                Level.DrawTraps(spriteBatch);
                Level.DrawItems(spriteBatch);
                Level.DrawEnemies(spriteBatch);
                foreach (var proj in Level.Projectiles) proj.Draw(spriteBatch);
                Player.Draw(spriteBatch, gameTime);
            }

            spriteBatch.End();

            graphics.SetRenderTarget(null);
            graphics.Clear(Color.Black);

            MultiplyLight.Parameters["SceneTex"]?.SetValue(SceneTarget);
            Matrix cam = Camera.GetTransform(root, gameTime);

            // Light params
            Vector2 playerScreenPos = Vector2.Transform(Player.Position, cam);
            Vector2 playerLightPos = new Vector2(playerScreenPos.X / SceneTarget.Width, playerScreenPos.Y / SceneTarget.Height);
            float playerLightRadius = 256f / Math.Max(SceneTarget.Width, SceneTarget.Height);

            Vector2 pointLightPos = Vector2.Zero;
            float pointLightAlpha = 0f;
            float pointLightRadius = 1000f / Math.Max(SceneTarget.Width, SceneTarget.Height);

            Vector2 campfireLightPos = Vector2.Zero;
            float campfireAlpha = 0f;
            float campfireLightRadius = 1024f / Math.Max(SceneTarget.Width, SceneTarget.Height);

            if (InBasement)
            {
                Vector2 campfireScreenPos = Vector2.Transform(campfire.Position, cam);
                campfireLightPos = new Vector2(campfireScreenPos.X / SceneTarget.Width, campfireScreenPos.Y / SceneTarget.Height);
                campfireAlpha = campfire.LightAlpha;
            }

            if (InLighthouse)
            {
                Vector2 centerScreen = Vector2.Transform(new Vector2(lighthouse.Bounds.Center.X, lighthouse.Bounds.Center.Y), cam);
                pointLightPos = new Vector2(centerScreen.X / SceneTarget.Width, centerScreen.Y / SceneTarget.Height);
                pointLightAlpha = 1f;
            }
            else pointLightAlpha = 0f;

            MultiplyLight.Parameters["PlayerLightPos"]?.SetValue(playerLightPos);
            MultiplyLight.Parameters["PlayerLightRadius"]?.SetValue(playerLightRadius);
            MultiplyLight.Parameters["PlayerLightAlpha"]?.SetValue(Player.FlashlightAlpha);

            MultiplyLight.Parameters["PointLightPos"]?.SetValue(pointLightPos);
            MultiplyLight.Parameters["PointLightRadius"]?.SetValue(pointLightRadius);
            MultiplyLight.Parameters["PointLightAlpha"]?.SetValue(pointLightAlpha);
            MultiplyLight.Parameters["PointLightColor"]?.SetValue(new Vector4(1f, 1f, 1f, 1f));

            MultiplyLight.Parameters["CampfireLightPos"]?.SetValue(campfireLightPos);
            MultiplyLight.Parameters["CampfireLightRadius"]?.SetValue(campfireLightRadius);
            MultiplyLight.Parameters["CampfireLightAlpha"]?.SetValue(campfireAlpha);
            MultiplyLight.Parameters["CampfireColor"]?.SetValue(new Vector4(1f, 0.7f, 0.3f, 1f));

            MultiplyLight.Parameters["LightTex"]?.SetValue(LightMask);
            MultiplyLight.Parameters["AmbientIntensity"]?.SetValue(AmbientDarkness);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Opaque, effect: MultiplyLight);
            spriteBatch.Draw(SceneTarget, new Rectangle(0, 0, SceneTarget.Width, SceneTarget.Height), Color.White);
            spriteBatch.End();

            // UI
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            Hud.Draw(spriteBatch);
            Inventory.Draw(spriteBatch);
            DialogueSystem.Draw(spriteBatch);
            fader.Draw(spriteBatch, WhitePixel, SceneTarget.Width, SceneTarget.Height);
            Codex.DrawUI(spriteBatch);
            PauseMenu.Draw(spriteBatch, graphics.Viewport.Width, graphics.Viewport.Height);

            if (InMainLevel) Minimap.Draw(spriteBatch);

            spriteBatch.End();
        }


        public void PlayBgMusic(Song song, float volume = 0.5f)
        {
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != song)
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
                MediaPlayer.Play(song);
            }
        }
    }
}