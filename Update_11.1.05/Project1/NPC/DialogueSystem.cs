using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1;

public class DialogueSystem
{
    GameManager gm;
    public bool Active = false;
    Queue<string> lines = new Queue<string>();

    public DialogueSystem(GameManager gm) { this.gm = gm; }

    public void StartDialog(string[] dialog)
    {
        lines.Clear();
        foreach (var s in dialog) lines.Enqueue(s);
        Active = true;

        gm.Player.IsTalking = true;
    }

    public void Update(GameTime gt)
    {
        if (!Active) return;

        if (InputManager.KeyPressed(Keys.E))
        {
            if (lines.Count > 0)
            {
                lines.Dequeue();
            }
            else
            {
                EndDialog();
            }
        }
    }

    public void EndDialog()
    {
        Active = false;

        gm.Player.IsTalking = false;
    }

    public void Draw(SpriteBatch sb)
    {
        if (!Active) return;
        string cur = lines.Count > 0 ? lines.Peek() : "";
        var rect = new Rectangle(100, GameRoot.ScreenHeight - 180, GameRoot.ScreenWidth - 200, 160);

        sb.Draw(gm.WhitePixel, rect, Color.Black * 0.7f);
        sb.DrawString(gm.DefaultFont, cur, new Vector2(rect.X + 16, rect.Y + 16), Color.White);
    }
}
