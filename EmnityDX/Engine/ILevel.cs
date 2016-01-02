using EmnityDX.Engine;
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
    public interface ILevel
    {
        void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera);
        ILevel UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera);
        void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera);
    }
}
