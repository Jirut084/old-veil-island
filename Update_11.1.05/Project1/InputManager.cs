using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public static class InputManager
    {
        public static KeyboardState KeyState, PrevKeyState;
        public static MouseState MouseState, PrevMouseState;
        public static int MouseScrollDelta { get; private set; }
        public static int MouseX { get; private set; }
        private static MouseState previousMouse;

        public static void Update()
        {
            PrevKeyState = KeyState;
            PrevMouseState = MouseState;
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            MouseScrollDelta = MouseState.ScrollWheelValue - previousMouse.ScrollWheelValue;
            MouseX = MouseState.X;
            previousMouse = MouseState;
        }

        public static bool KeyPressed(Keys k) => KeyState.IsKeyDown(k) && !PrevKeyState.IsKeyDown(k);
        public static bool KeyDown(Keys k) => KeyState.IsKeyDown(k);
        public static bool MouseLeftPressed() => MouseState.LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton == ButtonState.Released;
        public static bool MouseRightDown() => MouseState.RightButton == ButtonState.Pressed;
        public static bool MouseRightPressed() => MouseState.RightButton == ButtonState.Pressed && PrevMouseState.RightButton == ButtonState.Released;
        public static bool MouseLeftReleased() => MouseState.LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed;

        // ✅ เพิ่ม function แปลง mouse screen → world
        public static Vector2 MouseWorldPos(Matrix cameraTransform)
        {
            Vector2 screenPos = new Vector2(MouseState.X, MouseState.Y);
            Matrix inverse;
            Matrix.Invert(ref cameraTransform, out inverse);
            return Vector2.Transform(screenPos, inverse);
        }

        public static void Reset()
        {
            KeyState = Keyboard.GetState();
            PrevKeyState = KeyState;
            MouseState = Mouse.GetState();
            PrevMouseState = MouseState;
            previousMouse = MouseState;
            MouseScrollDelta = 0;
        }

    }
}
