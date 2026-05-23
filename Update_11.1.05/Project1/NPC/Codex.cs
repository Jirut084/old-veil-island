using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1;
using Project1.NPC;
using System;
using System.Collections.Generic;
using System.Linq;

public class Codex
{
    GameManager gm;
    Texture2D bg;
    public bool Open = false;

    private List<Note> entries = new List<Note>();
    private List<Note> playerNotes = new List<Note>();

    public int Count => entries.Count;
    public int PlayerNoteCount => playerNotes.Count;

    int frameCount = 19;
    float currentFrame = 0f;
    float frameSpeed = 17;
    int frameWidth;
    int frameHeight;

    private int npcScroll = 0;
    private int playerScroll = 0;
    private int maxVisibleLines = 10;

    public Codex(GameManager gm, Texture2D bg)
    {
        this.gm = gm;
        this.bg = bg;
        frameWidth = bg.Width / frameCount;
        frameHeight = bg.Height;
    }

    public void UnlockPlayerNote(Note note)
    {
        if (!playerNotes.Contains(note))
        {
            note.Collected = true;
            playerNotes.Add(note);
        }
    }

    public void UnlockNote(Note note)
    {
        // เพิ่มเฉพาะถ้ายังไม่มีข้อความนี้
        if (!entries.Any(n => n.Text == note.Text))
            entries.Add(note);
    }

    public void Unlock(string text)
    {
        if (!entries.Any(n => n.Text == text))
        {
            entries.Add(new Note("", text) { Collected = true });
        }
    }

    public bool EntriesTextContains(string text)
    {
        return entries.Any(n => n.Text == text);
    }

    public void Update(GameTime gt)
    {
        if (InputManager.KeyPressed(Keys.C))
            Open = !Open;

        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        // Animation
        if (Open && currentFrame < frameCount - 1)
        {
            currentFrame += frameSpeed * delta;
            if (currentFrame > frameCount - 1) currentFrame = frameCount - 1;
        }
        else if (!Open && currentFrame > 0)
        {
            currentFrame -= frameSpeed * delta;
            if (currentFrame < 0) currentFrame = 0;
        }

        if (Open)
        {
            int scrollDelta = InputManager.MouseScrollDelta;
            if (scrollDelta != 0)
            {
                if (InputManager.MouseX < GameRoot.ScreenWidth / 2)
                {
                    npcScroll -= scrollDelta;
                    npcScroll = Math.Clamp(npcScroll, 0, Math.Max(0, entries.Count - maxVisibleLines));
                }
                else
                {
                    playerScroll -= scrollDelta;
                    playerScroll = Math.Clamp(playerScroll, 0, Math.Max(0, playerNotes.Count - maxVisibleLines));
                }
            }
        }
    }

    public void DrawUI(SpriteBatch sb)
    {
        if (currentFrame == 0 && !Open) return;

        Rectangle sourceRect = new Rectangle((int)currentFrame * frameWidth, 0, frameWidth, frameHeight);
        Rectangle destRect = new Rectangle(80, 10, GameRoot.ScreenWidth - 160, GameRoot.ScreenHeight - 80);
        sb.Draw(bg, destRect, sourceRect, Color.White);

        if (Open && currentFrame >= frameCount - 1)
        {
            float noteWidth = 350f; // ปรับความกว้าง Note ได้ที่นี่

            // NPC Codex
            sb.DrawString(gm.DefaultFont, "Dialogue - discovered:", new Vector2(280, 230), Color.Black);
            int y = 265;
            for (int i = npcScroll; i < Math.Min(entries.Count, npcScroll + maxVisibleLines); i++)
            {
                if (!entries[i].Collected)
                {
                    var lines = WrapText(gm.DefaultFont, entries[i].Text, noteWidth);
                    foreach (var line in lines)
                    {
                        sb.DrawString(gm.DefaultFont, line, new Vector2(280, y), Color.Brown);
                        y += 20;
                    }
                    y += 10;
                }
            }

            // Player Notes
            sb.DrawString(gm.DefaultFont, "Notes - discovered:", new Vector2(GameRoot.ScreenWidth / 2 + 120, 230), Color.Black);
            y = 265;
            for (int i = playerScroll; i < Math.Min(playerNotes.Count, playerScroll + maxVisibleLines); i++)
            {
                var lines = WrapText(gm.DefaultFont, playerNotes[i].Text, noteWidth);
                foreach (var line in lines)
                {
                    sb.DrawString(gm.DefaultFont, line, new Vector2(GameRoot.ScreenWidth / 2 + 118, y), Color.DarkGreen);
                    y += 20;
                }
                y += 10;
            }
        }
    }

    private List<string> WrapText(SpriteFont font, string text, float maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        string currentLine = "";

        foreach (var word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
            if (font.MeasureString(testLine).X > maxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }
        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return lines;
    }

    public void Reset()
    {
        entries.Clear();
        playerNotes.Clear();
        npcScroll = 0;
        playerScroll = 0;
        Open = false;
        currentFrame = 0f;
    }
}
