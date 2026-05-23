using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class SaveSystem
    {
        public Vector2 Position;
        public Texture2D Texture;

        private bool playerNearby = false;
        private bool showSavedMessage = false;
        private double savedMessageTimer = 0;
        private const double savedMessageDuration = 3.0; // 3 วิ
        private bool isSaved = false;

        GameManager gm;
        public SaveSystem(GameManager gm) { this.gm = gm; }

        //public SaveSystem(Vector2 position)
        //{
        //    Position = position;
        //}

        //public void Update(GameTime gameTime, Player player)
        //{
        //    playerNearby = Vector2.Distance(player.Center, Position) < 50f;

        //    KeyboardState kb = Keyboard.GetState();
        //    if (playerNearby && kb.IsKeyDown(Keys.E) && !isSaved)
        //    {
        //        SaveGame(player);
        //    }

        //    if (showSavedMessage)
        //    {
        //        savedMessageTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        //        if (savedMessageTimer <= 0)
        //        {
        //            showSavedMessage = false;
        //            isSaved = false; // กลับเป็นสีเทา
        //        }
        //    }
        //}

        //public void SaveGame(Player player)
        //{
        //    isSaved = true; // เปลี่ยนกล่องเป็นสีน้ำเงิน
        //    showSavedMessage = true;
        //    savedMessageTimer = savedMessageDuration;
        //}

        //public void Draw(SpriteBatch spriteBatch)
        //{
        //    Color boxColor = isSaved ? Color.Blue : Color.Gray;
        //    Utils.DrawRectangle(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, 64, 64), boxColor, 2);
        //}

        //public void DrawInteractionHint(SpriteBatch spriteBatch, SpriteFont font)
        //{
        //    if (playerNearby && !isSaved)
        //    {
        //        spriteBatch.DrawString(font, "Press E to Save", Position - new Vector2(0, 20), Color.Yellow);
        //    }
        //}

        //public void DrawSavedMessage(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphics)
        //{
        //    if (showSavedMessage)
        //    {
        //        float alpha = (float)(savedMessageTimer / savedMessageDuration);
        //        Color color = Color.Red * alpha;
        //        string message = "Saved!";
        //        Vector2 size = font.MeasureString(message);
        //        Vector2 position = new Vector2(graphics.Viewport.Width / 2 - size.X / 2, graphics.Viewport.Height / 2 - size.Y / 2);


        //        spriteBatch.DrawString(font, message, position, color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

        //    }
        //}

        public void SaveToCalendar() { /* Use simple binary or json save; omitted for brevity */ }
        public void LoadFromCalendar() { }
    }
}


