using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class HUD
    {
        GameManager gm;

        // --- สำหรับ log messages ---
        private List<LogMessage> messages = new List<LogMessage>();
        private class LogMessage
        {
            public string Text;
            public float Timer;
            public LogMessage(string text, float duration)
            {
                Text = text;
                Timer = duration;
            }
        }

        public HUD(GameManager gm)
        {
            this.gm = gm;
        }

        // --- ฟังก์ชัน Log ---
        public void Log(string text, float duration = 3f)

        {
            messages.Add(new LogMessage(text, duration));
        }



        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                messages[i].Timer -= dt;
                if (messages[i].Timer <= 0)
                    messages.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            // --- ข้อมูลพื้นฐาน ---
            sb.DrawString(gm.DefaultFont, gm.DayManager.GetTimeString(), new Vector2(10, 6), Color.White);

            // --- Health Bar ---
            int hpBarWidth = 100;
            int hpBarHeight = 12;
            Vector2 hpBarPos = new Vector2(10, 28);
            float hpFraction = (float)gm.Player.Health / gm.Player.MaxHealth;
            sb.Draw(gm.WhitePixel, new Rectangle((int)hpBarPos.X, (int)hpBarPos.Y, hpBarWidth, hpBarHeight), Color.Gray * 0.5f);
            sb.Draw(gm.WhitePixel, new Rectangle((int)hpBarPos.X, (int)hpBarPos.Y, (int)(hpBarWidth * hpFraction), hpBarHeight), Color.Red);
            sb.DrawString(gm.DefaultFont, $"{gm.Player.Health}/{gm.Player.MaxHealth}", hpBarPos + new Vector2(hpBarWidth + 5, 0), Color.White);

            // --- Stamina Bar ---
            int barWidth = 100;
            int barHeight = 12;
            Vector2 barPos = new Vector2(10, 50);
            float staFraction = gm.Player.Stamina / gm.Player.MaxStamina;
            sb.Draw(gm.WhitePixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.Gray * 0.5f);
            Color fillColor = gm.Player.staminaLocked ? Color.OrangeRed : Color.Green;
            sb.Draw(gm.WhitePixel, new Rectangle((int)barPos.X, (int)barPos.Y, (int)(barWidth * staFraction), barHeight), fillColor);
            sb.DrawString(gm.DefaultFont, $"{(int)gm.Player.Stamina}/{gm.Player.MaxStamina}", barPos + new Vector2(barWidth + 5, 0), fillColor);

            // --- Battery Bar ---
            int battBarWidth = 100;
            int battBarHeight = 12;
            Vector2 battPos = new Vector2(10, 86);
            float battFraction = gm.Player.Battery / 100f;
            sb.Draw(gm.WhitePixel, new Rectangle((int)battPos.X, (int)battPos.Y, battBarWidth, battBarHeight), Color.Gray * 0.5f);
            sb.Draw(gm.WhitePixel, new Rectangle((int)battPos.X, (int)battPos.Y, (int)(battBarWidth * battFraction), battBarHeight), Color.Blue);
            sb.DrawString(gm.DefaultFont, $"Battery {(int)gm.Player.Battery}%", battPos + new Vector2(battBarWidth + 5, 0), Color.White);

            // inside HUD.Draw(SpriteBatch sb) replace KeyItem display with:
            sb.DrawString(gm.DefaultFont, $"Notes: {gm.Codex.PlayerNoteCount}", new Vector2(10, 100), Color.Cyan);


            // --- Inventory ---
            gm.Inventory.Draw(sb);

            // --- Draw log messages ---
            int startY = 150;

            int spacing = 25;
            float rightEdge = 1350;


            for (int i = 0; i < messages.Count; i++)
            {
                string text = messages[i].Text;
                Vector2 textSize = gm.DefaultFont.MeasureString(text);
                float posX = rightEdge - textSize.X;
                sb.DrawString(gm.DefaultFont, text, new Vector2(posX, startY + i * spacing), Color.Yellow);
            }
            // Controls array
            string[] controls = new string[]
            {
                "W/A/S/D to walk",
                "Shift to run",
                "E to interact",
                "Spacebar to open flashlight",
                "1/2/3/4 to select",
                "C to open/close book",
                "Esc to pause",
                "R to reload"
            };

            // เริ่ม Y หลัง Notes
            Vector2 notesPos = new Vector2(10, 100); // ตำแหน่ง Notes

            int controlsStartY = (int)notesPos.Y + spacing + 10; // 10 px ช่องว่างใต้ Notes
            float leftEdge = 10; // ขอบซ้าย

            for (int i = 0; i < controls.Length; i++)
            {
                sb.DrawString(gm.DefaultFont, controls[i], new Vector2(leftEdge, controlsStartY + i * spacing), Color.White);
            }


        }
        public void Reset()
        {
            messages.Clear();
        }
    }
}
