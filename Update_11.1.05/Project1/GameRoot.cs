using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Project1
{
    public class GameRoot : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int ScreenWidth = 1360;
        public static int ScreenHeight = 768;

        public GameManager GM { get; private set; }

        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;

            // ตั้งค่าหน้าจอ
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = true; // เปิด fullscreen อัตโนมัติ
            Window.IsBorderless = true;   

            // Event สำหรับ handle focus และ resize
            Window.ClientSizeChanged += OnClientSizeChanged;
            Activated += OnActivated;
            Deactivated += OnDeactivated;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GM = new GameManager(this, Content, GraphicsDevice, spriteBatch);
            GM.LoadAll();
        }

        KeyboardState prevKb;
        protected override void Update(GameTime gameTime)
        {
            // สลับ fullscreen ด้วย F11 (edge-trigger)
            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.F11) && !prevKb.IsKeyDown(Keys.F11))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }
            prevKb = kb;

            GM.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GM.Draw(gameTime);
            base.Draw(gameTime);
        }

        // คืน focus เมื่อกลับเข้าเกม
        private void OnActivated(object sender, EventArgs e)
        {
            InputManager.Reset();
            graphics.ApplyChanges();

            // เช็กก่อนว่าเกมกำลัง Pause หรือไม่
            if (GM != null && !GM.PauseMenu.IsPaused)
            {
                MediaPlayer.Resume(); // เล่นเพลงต่อก็ต่อเมื่อไม่อยู่ใน Pause
            }
        }

        // หยุด input / pause เพลงชั่วคราวเมื่อหลุด focus
        private void OnDeactivated(object sender, EventArgs e)
        {
            MediaPlayer.Pause();
        }

        // จัดการหน้าต่าง resize
        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            graphics.ApplyChanges();
        }
    }
}
