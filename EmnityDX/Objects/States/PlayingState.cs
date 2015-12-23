using EmnityDX.Engine;
using EmnityDX.Objects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using EmnityDX.Objects.LevelData.PauseStateLevels;

namespace EmnityDX.Objects.States
{
    public class PlayingState : State
    {
        public PlayingState(Level level, ContentManager content, GraphicsDeviceManager graphics, MouseState mouseState = new MouseState(), GamePadState gamePadState = new GamePadState(), KeyboardState keyboardState = new KeyboardState()) 
            : base(level, content, graphics, mouseState, gamePadState, keyboardState)
        {

        }

        public override State UpdateContent(GameTime gameTime)
        {
            State nextState = this;
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !PrevKeyboardState.IsKeyDown(Keys.Enter))
            {
                nextState = new PauseState(new Pause(), Content, Graphics, Mouse.GetState(), GamePad.GetState(PlayerIndex.One), Keyboard.GetState());
            }

            if( base.UpdateContent(gameTime) == null)
            {
                return null;
            }
            return nextState;
        }

        public override void DrawContent(SpriteBatch spriteBatch)
        {
            base.DrawContent(spriteBatch);
        }
    }
}
