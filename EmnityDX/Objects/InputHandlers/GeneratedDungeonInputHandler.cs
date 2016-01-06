using EmnityDX.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using EmnityDX.Objects.Components;
using EmnityDX.Engine.ProceduralGeneration;

namespace EmnityDX.Objects.InputHandlers
{
    public class GeneratedDungeonInputHandler : IUserInputHandler
    {
        public void HandleInput(LevelComponents levelComponents, GraphicsDeviceManager graphics, GameTime gameTime, 
            KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera, ref DungeonTiles[,] dungeonGrid)
        {
            PlayerMovement(graphics, gameTime, camera, prevKeyboardState, levelComponents, ref dungeonGrid);


            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
            {
                camera.ResetScreenScale(graphics, new Vector2(3968 * 2, 2232 * 2));
            }
        }

        private void PlayerMovement(GraphicsDeviceManager graphics, GameTime gameTime, Camera camera, KeyboardState prevKey, LevelComponents levelComponents, ref DungeonTiles[,] dungeonGrid)
        {
            Random random = new Random();
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Entity entity in levelComponents.Entities)
            {
                if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                {
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.NumPad8) && !prevKey.IsKeyDown(Keys.NumPad8))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.Y -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad2) && !prevKey.IsKeyDown(Keys.NumPad2))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.Y += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad6) && !prevKey.IsKeyDown(Keys.NumPad6))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.X += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad4) && !prevKey.IsKeyDown(Keys.NumPad4))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.X -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad7) && !prevKey.IsKeyDown(Keys.NumPad7))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.X -= 1;
                        pos.Pos.Y -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad9) && !prevKey.IsKeyDown(Keys.NumPad9))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.X += 1;
                        pos.Pos.Y -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad1) && !prevKey.IsKeyDown(Keys.NumPad1))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.X -= 1;
                        pos.Pos.Y += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad3) && !prevKey.IsKeyDown(Keys.NumPad3))
                    {
                        Position pos = levelComponents.PositionComponents[entity.Id];
                        pos.Pos.X += 1;
                        pos.Pos.Y += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            levelComponents.PositionComponents[entity.Id] = pos;
                        }
                    }
                }
            }
        }


    }
}
