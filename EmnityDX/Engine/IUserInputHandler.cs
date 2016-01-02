using EmnityDX.Engine.ProceduralGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine
{
    public interface IUserInputHandler
    {
        void HandleInput(LevelComponents levelComponents, GraphicsDeviceManager graphics, GameTime gameTime,
            KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera, ref DungeonTiles[,] dungeonGrid);
    }
}
