using EmnityDX.Objects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine
{
    public class State
    {
        protected ContentManager Content;
        protected MouseState PrevMouseState;
        protected GamePadState PrevGamepadState;
        protected KeyboardState PrevKeyboardState;
        protected Level CurrentLevel;
        protected GraphicsDeviceManager Graphics;

        public State(Level level, ContentManager content, GraphicsDeviceManager graphics, MouseState mouseState = new MouseState(), GamePadState gamePadState = new GamePadState(), KeyboardState keyboardState = new KeyboardState())
        {
            this.Content = new ContentManager(content.ServiceProvider, "Content");
            Graphics = graphics;
            PrevMouseState = mouseState;
            PrevGamepadState = gamePadState;
            PrevKeyboardState = keyboardState;
            SetLevel(level);
        }

        ~State()
        {
            if (Content != null) { Content.Unload(); }
        }

        public virtual State UpdateContent(GameTime gameTime)
        {

            Level nextLevel = CurrentLevel;
            nextLevel = CurrentLevel.UpdateLevel(gameTime, Content, Graphics, PrevKeyboardState, PrevMouseState, PrevGamepadState);
            if (nextLevel != CurrentLevel && nextLevel != null)
            {
                SetLevel(nextLevel);
            }
            if (nextLevel == null)
            {
                return null;
            }

            PrevKeyboardState = Keyboard.GetState();
            PrevMouseState = Mouse.GetState();
            PrevGamepadState = GamePad.GetState(PlayerIndex.One);
            return this;
        }

        public virtual void DrawContent(SpriteBatch spriteBatch)
        {
            CurrentLevel.DrawLevel(spriteBatch, Graphics);
        }

        protected void SetLevel(Level level)
        {
            if (Content != null && level != null)
            {
                Content.Unload();
                CurrentLevel = level;
                level.LoadLevel(Content, Graphics);
            }
        }
    }
}
