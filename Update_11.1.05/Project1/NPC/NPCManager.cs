using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1.NPC
{
    public class NPCManager
    {
        GameManager gm;
        List<NPC> npcs = new List<NPC>();

        public NPCManager(GameManager gm) { this.gm = gm; }

        public void SpawnDailyNPCs()
        {
            npcs.Clear();

            List<string[]> possibleDialogues = new List<string[]>
            {
                new string[] { "The forest is dark.", "Use your flashlight to light the way." },
                new string[] { "You looking for the note.", "You can check it again by open your book." },
                new string[] { "Beware of the traps.", "You didn't want to get caught by one." },
                new string[] { "DON'T forget to keep lighting the campfire.", "Before everything turn dark and it is over." },
                new string[] { "If you see something be careful.", "Running away might be a better option..." }

            };

            Vector2 npcPosition = new Vector2(735, 150);
            npcs.Add(new NPC(gm, npcPosition, gm.content.Load<Texture2D>("NPC01"), possibleDialogues));
        }

        public void ResetDailyNPCDialogue()
        {
            List<string[]> possibleDialogues = new List<string[]>
            {
                new string[] { "The forest is dark.", "Use your flashlight to light the way." },
                new string[] { "You looking for the note.", "You can check it again by open your book." },
                new string[] { "Beware of the traps.", "You didn't want to get caught by one." },
                new string[] { "DON'T forget to keep lighting the campfire.", "Before everything turn dark and it is over." },
                new string[] { "If you see something be careful.", "Running away might be a better option..." }

            };

            foreach (var npc in npcs)
            {
                npc.SetDailyDialogue(possibleDialogues);
            }
        }

        public void Update(GameTime gt)
        {
            foreach (var n in npcs) n.Update(gt);
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            Matrix cam = Camera.GetTransform(gm.root, gt);

            foreach (var n in npcs)
            {
                n.Draw(sb);
            }

            foreach (var n in npcs)
            {
                n.DrawInteractionHint(sb, gm.DefaultFont, cam);
            }
        }
    }

    public class NPC
    {
        GameManager gm;
        public Vector2 Position;
        Texture2D tex;

        int frameWidth = 28;
        int frameHeight = 64;
        int frameCount = 5;
        int currentFrame = 0;
        double frameTime = 0.2;
        double timer = 0;

        List<string[]> DialogueSequences;
        int nextSequenceIndex = 0;
        Random rng = new Random();

        public NPC(GameManager gm, Vector2 pos, Texture2D tex, List<string[]> dialogueSequences)
        {
            this.gm = gm;
            Position = pos;
            this.tex = tex;
            SetDailyDialogue(dialogueSequences);
        }

        public void SetDailyDialogue(List<string[]> dialogueSequences)
        {
            DialogueSequences = dialogueSequences.OrderBy(x => rng.Next()).ToList();
            nextSequenceIndex = 0;
        }

        public void Update(GameTime gt)
        {
            timer += gt.ElapsedGameTime.TotalSeconds;
            if (timer >= frameTime)
            {
                currentFrame = (currentFrame + 1) % frameCount;
                timer = 0;
            }

            float distance = Vector2.Distance(gm.Player.Position, Position);

            if (distance < 48 && InputManager.KeyPressed(Keys.E))
            {
                string[] sequence;
                if (nextSequenceIndex < DialogueSequences.Count)
                {
                    sequence = DialogueSequences[nextSequenceIndex];
                    nextSequenceIndex++;
                }
                else
                {
                    sequence = new string[] { "I have nothing more to say today." };
                }

                gm.DialogueSystem.StartDialog(sequence);
                gm.Player.IsTalking = true;

                // 🔑 Unlock Note สำหรับ Codex
                foreach (string line in sequence)
                {
                    // Collected = false เพื่อให้แสดงใน NPC Codex
                    gm.Codex.UnlockNote(new Note("NPC01", line) { Collected = false });
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle srcRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            sb.Draw(tex, Position, srcRect, Color.White, 0f, new Vector2(frameWidth / 2f, frameHeight / 2f), 1.5f, SpriteEffects.None, 0f);
        }


        public void DrawInteractionHint(SpriteBatch sb, SpriteFont font, Matrix camera)
        {
            float distance = Vector2.Distance(gm.Player.Position, Position);
            if (distance < 48)
            {
                Vector2 textSize = font.MeasureString("Press E to talk");

                // offset ให้ข้อความอยู่เหนือหัว NPC
                Vector2 worldOffset = new Vector2(0, -frameHeight / 2 - 20);
                Vector2 screenPos = Position + worldOffset; // ไม่ต้อง Transform ด้วยกล้อง

                // Center text
                Vector2 textPos = screenPos - new Vector2(textSize.X / 2, textSize.Y);

                sb.DrawString(font, "Press E to talk", textPos, Color.Yellow);
            }
        }
    }

}
