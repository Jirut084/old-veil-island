using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project1
{
    public class ScreenFader
    {
        public float Alpha = 0f;
        public bool FadingOut = false;
        public bool FadingIn = false;

        // 3 วินาทีเต็ม (1/3)
        public float Speed = 1f / 1f;

        public Action OnFadeOutComplete;

        public void StartFadeOut(Action onComplete)
        {
            FadingOut = true;
            FadingIn = false;
            OnFadeOutComplete = onComplete;
        }

        public void StartFadeIn()
        {
            FadingIn = true;
            FadingOut = false;
        }

        public void Update(GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            if (FadingOut)
            {
                Alpha += Speed * dt;
                if (Alpha >= 1f)
                {
                    Alpha = 1f;
                    FadingOut = false;
                    OnFadeOutComplete?.Invoke();
                    StartFadeIn();
                }
            }
            else if (FadingIn)
            {
                Alpha -= Speed * dt;
                if (Alpha <= 0f)
                {
                    Alpha = 0f;
                    FadingIn = false;
                }
            }
        }

        public void Draw(SpriteBatch sb, Texture2D whitePixel, int width, int height)
        {
            if (Alpha > 0f)
            {
                sb.Draw(whitePixel, new Rectangle(0, 0, width, height), Color.Black * Alpha);
            }
        }
    }
}
