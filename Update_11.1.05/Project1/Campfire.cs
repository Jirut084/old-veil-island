using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1;
using System;
using System.Linq;

public class Campfire
{
    GameManager gm;
    public Texture2D tex;
    public Vector2 Position;

    bool showHint = false;
    bool showHint1 = false;

    public int Level = 1;           // level ของ campfire
    public float MaxDuration = 120f; // Level 1 = 120s
    public float remainingTime = 0f;
    public bool FireOn => remainingTime > 0f;
    public bool FireOff => remainingTime <= 0f;
    public float LightAlpha = 0f;
    public float TargetAlpha = 1f;
    public float AlphaInterpSpeed = 4f;
    public float FillAmount = 10f;

    public int frameWidth { get; set; } = 32;
    public int frameHeight { get; set; } = 32;
    int frameCount = 8;
    int currentFrame = 0;
    int row = 0;
    double frameTime = 0.2;
    double timer = 0;

    public Campfire(GameManager gm, Texture2D texture)
    {
        this.gm = gm;
        this.tex = texture;
        Position = new Vector2(1200, 400);
        remainingTime = MaxDuration;
    }

    public void Update(GameTime gt)
    {
        float dt = (float)gt.ElapsedGameTime.TotalSeconds;
        showHint = false;
        showHint1 = false;

        // Animation
        timer += dt;
        if (timer >= frameTime)
        {
            currentFrame = (currentFrame + 1) % frameCount;
            timer = 0;
        }
        row = 0;

        // ตรวจว่าผู้เล่นอยู่ใกล้
        if (Vector2.Distance(gm.Player.Position, Position) < 60f)
        {
            // ตรวจว่าผู้เล่นเลือกขวานใน Hotbar
            var selectedSlot = gm.Inventory.Hotbar[gm.Inventory.SelectedIndex];
            bool holdingWood = selectedSlot.Item?.Id == "wood";

            if (holdingWood && Level == 1)
            {
                // เฉพาะตอนถือขวานและยังไม่ upgrade
                showHint = true;
                showHint1 = false;

                if (InputManager.KeyPressed(Keys.E))
                {
                    Upgrade();
                }
            }
            else
            {
                // เติมไฟปกติ
                showHint = false;
                showHint1 = true;
                if (InputManager.KeyPressed(Keys.E))
                {
                    remainingTime += FillAmount;
                    if (remainingTime > MaxDuration) remainingTime = MaxDuration;
                    TargetAlpha = 1f;
                }
            }
        }


        // ลดเวลา
        if (remainingTime > 0f)
        {
            remainingTime -= dt;
            if (remainingTime < 0f) remainingTime = 0f;
        }

        if (FireOff) return;

        TargetAlpha = FireOn ? 1f : 0f;
        LightAlpha = MathHelper.Lerp(LightAlpha, TargetAlpha, MathHelper.Clamp(AlphaInterpSpeed * dt, 0f, 1f));
    }

    void Upgrade()
    {
        Level = 2;
        MaxDuration = 180f;
        remainingTime = MaxDuration;

        // ลบขวานจาก inventory/hotbar
        for (int i = 0; i < gm.Inventory.Hotbar.Length; i++)
        {
            if (gm.Inventory.Hotbar[i].Item?.Id == "wood")
            {
                gm.Inventory.Hotbar[i].Item = null;
                break;
            }
        }

        for (int x = 0; x < gm.Inventory.Grid.GetLength(0); x++)
        {
            for (int y = 0; y < gm.Inventory.Grid.GetLength(1); y++)
            {
                if (gm.Inventory.Grid[x, y].Item?.Id == "wood")
                {
                    gm.Inventory.Grid[x, y].Item = null;
                    return; // exit afterลบแล้ว
                }
            }
        }
    }

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
        sb.Draw(tex, Position, sourceRect, Color.White, 0f, new Vector2(frameWidth / 2f, frameHeight / 2f), 4f, SpriteEffects.None, 0f);

        if (showHint)
        {
            Vector2 textSize = font.MeasureString("Press E to Upgrade");
            Vector2 textPos = Position + new Vector2(-textSize.X / 2, -60);
            sb.DrawString(font, "Press E to Upgrade", textPos, Color.Yellow);
        }
        if (showHint1)
        {
            Vector2 textSize = font.MeasureString("Press E to Refill");
            Vector2 textPos = Position + new Vector2(-textSize.X / 2, -60);
            sb.DrawString(font, "Press E to Refill", textPos, Color.Yellow);
        }
        
        if (FireOn)
        {
            string timeText = $"{Math.Ceiling(remainingTime)}s";
            Vector2 textSize = font.MeasureString(timeText);
            Vector2 textPos = Position + new Vector2(-textSize.X / 2, 50);
            sb.DrawString(font, timeText, textPos, Color.OrangeRed);
        }
    }

    public void Reset()
    {
        Level = 1;
        MaxDuration = 120f;
        remainingTime = MaxDuration;
        TargetAlpha = 1f;
        LightAlpha = 0f;
        currentFrame = 0;
        timer = 0;
    }
}
