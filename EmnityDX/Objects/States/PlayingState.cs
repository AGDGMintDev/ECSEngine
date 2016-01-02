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

namespace EmnityDX.Objects.States
{
    public class PlayingState : State
    {
        public PlayingState(ILevel level, Camera camera, ContentManager content, GraphicsDeviceManager graphics, MouseState mouseState = new MouseState(), GamePadState gamePadState = new GamePadState(), KeyboardState keyboardState = new KeyboardState()) 
            : base(level, camera, content, graphics, mouseState, gamePadState, keyboardState)
        {

        }

        public override State UpdateContent(GameTime gameTime, Camera camera)
        {
            State nextState = this;

            if( base.UpdateContent(gameTime, camera) == null)
            {
                return null;
            }
            return nextState;
        }

        public override void DrawContent(SpriteBatch spriteBatch, Camera camera)
        {
            base.DrawContent(spriteBatch, camera);
        }
    }
}
