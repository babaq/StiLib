#region File Description
//-----------------------------------------------------------------------------
// SLInput.cs
//
// StiLib Input Service.
// Copyright (c) Zhang Li. 2008-09-22.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// Manage all input device states and all input events
    /// </summary>
    public class SLInput
    {
        # region Fields

        /// <summary>
        /// Mouse State
        /// </summary>
        static MouseState mouseState, mouseStateLast;

        /// <summary>
        /// Keyboard State
        /// </summary>
        static KeyboardState keyboardState, keyboardStateLast;

        /// <summary>
        /// GamePad State
        /// </summary>
        static GamePadState gamepadState, gamepadStateLast;

        #endregion

        #region Properties

        /// <summary>
        /// Get KeyBoard State
        /// </summary>
        public KeyboardState KEY
        {
            get { return keyboardState; }
        }

        /// <summary>
        /// Get Mouse State
        /// </summary>
        public MouseState MOUSE
        {
            get { return mouseState; }
        }

        /// <summary>
        /// Get GamePad State
        /// </summary>
        public GamePadState GAMEPAD
        {
            get { return gamepadState; }
        }

        /// <summary>
        /// Get Last KeyBoard State
        /// </summary>
        public KeyboardState KEYLAST
        {
            get { return keyboardStateLast; }
        }

        /// <summary>
        /// Get Last Mouse State
        /// </summary>
        public MouseState MOUSELAST
        {
            get { return mouseStateLast; }
        }

        /// <summary>
        /// Get Last GamePad State
        /// </summary>
        public GamePadState GAMEPADLAST
        {
            get { return gamepadStateLast; }
        }

        #endregion


        /// <summary>
        /// Init all the Input Devices States
        /// </summary>
        public SLInput()
        {
            keyboardStateLast = Keyboard.GetState();
            mouseStateLast = Mouse.GetState();
            gamepadStateLast = GamePad.GetState(PlayerIndex.One);

            keyboardState = keyboardStateLast;
            mouseState = mouseStateLast;
            gamepadState = gamepadStateLast;
        }

        /// <summary>
        /// Updata All the Input Devices States
        /// </summary>
        public virtual void Update()
        {
            // Back Up old state
            keyboardStateLast = keyboardState;
            mouseStateLast = mouseState;
            gamepadStateLast = gamepadState;

            // Get new state
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);
        }

        # region Input Events

        /// <summary>
        /// Is a Key at Up State
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyUp(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Is a Key at Down State
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Is a Key Just been pressed(Up State to Down State)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyPressed(Keys key)
        {
            return keyboardStateLast.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Is a Key Just been released(Down State to Up State)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyReleased(Keys key)
        {
            return keyboardStateLast.IsKeyDown(key) && keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Is a Key being hold(Down State to Down State)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyHold(Keys key)
        {
            return keyboardStateLast.IsKeyDown(key) && keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Is a Key being free(Up State to Up State)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyFree(Keys key)
        {
            return keyboardStateLast.IsKeyUp(key) && keyboardState.IsKeyUp(key);
        }


        /// <summary>
        /// Set Mouse Position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetMousePosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
        }

        /// <summary>
        /// Get Mouse Wheel Delta Value
        /// </summary>
        public int MouseWheelDelta
        {
            get { return mouseState.ScrollWheelValue - mouseStateLast.ScrollWheelValue; }
        }

        /// <summary>
        /// Is Mouse LeftButton at Up State
        /// </summary>
        /// <returns></returns>
        public bool IsMouseLeftButtonUp()
        {
            return mouseState.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse LeftButton at Down State
        /// </summary>
        /// <returns></returns>
        public bool IsMouseLeftButtonDown()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse LeftButton Just been pressed
        /// </summary>
        /// <returns></returns>
        public bool IsMouseLeftButtonPressed()
        {
            return mouseStateLast.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse LeftButton Just been released
        /// </summary>
        /// <returns></returns>
        public bool IsMouseLeftButtonReleased()
        {
            return mouseStateLast.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse LeftButton being hold
        /// </summary>
        /// <returns></returns>
        public bool IsMouseLeftButtonHold()
        {
            return mouseStateLast.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse LeftButton being free
        /// </summary>
        /// <returns></returns>
        public bool IsMouseLeftButtonFree()
        {
            return mouseStateLast.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse MiddleButton at Up State
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMiddleButtonUp()
        {
            return mouseState.MiddleButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse MiddleButton at Down State
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMiddleButtonDown()
        {
            return mouseState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse MiddleButton Just been pressed
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMiddleButtonPressed()
        {
            return mouseStateLast.MiddleButton == ButtonState.Released && mouseState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse MiddleButton Just been released
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMiddleButtonReleased()
        {
            return mouseStateLast.MiddleButton == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse MiddleButton being hold
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMiddleButtonHold()
        {
            return mouseStateLast.MiddleButton == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse MiddleButton being free
        /// </summary>
        /// <returns></returns>
        public bool IsMouseMiddleButtonFree()
        {
            return mouseStateLast.MiddleButton == ButtonState.Released && mouseState.MiddleButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse RightButton at Up State
        /// </summary>
        /// <returns></returns>
        public bool IsMouseRightButtonUp()
        {
            return mouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse RightButton at Down State
        /// </summary>
        /// <returns></returns>
        public bool IsMouseRightButtonDown()
        {
            return mouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse RightButton Just been pressed
        /// </summary>
        /// <returns></returns>
        public bool IsMouseRightButtonPressed()
        {
            return mouseStateLast.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse RightButton Just been released
        /// </summary>
        /// <returns></returns>
        public bool IsMouseRightButtonReleased()
        {
            return mouseStateLast.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Is Mouse RightButton being hold
        /// </summary>
        /// <returns></returns>
        public bool IsMouseRightButtonHold()
        {
            return mouseStateLast.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Is Mouse RightButton being free
        /// </summary>
        /// <returns></returns>
        public bool IsMouseRightButtonFree()
        {
            return mouseStateLast.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Released;
        }

        #endregion

    }
}
