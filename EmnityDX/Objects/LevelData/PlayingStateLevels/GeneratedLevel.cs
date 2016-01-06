using EmnityDX.Engine;
using EmnityDX.Engine.ProceduralGeneration;
using EmnityDX.Objects.Components;
using EmnityDX.Objects.InputHandlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Objects.LevelData.PlayingStateLevels
{
    public class GeneratedLevel : ILevel
    {
        #region Components
        private LevelComponents levelComponents;
        #endregion

        #region Dungeon Environment Variables
        private Texture2D floor;
        Vector2 dungeonDimensions;
        private DungeonTiles[,] dungeonGrid = null;
        private IUserInputHandler inputHandler;
        private const int cellSize = 40;
        private Random random = new Random();
        private float opacity = 0.1f;  //Follow the opacity and NEWLY_FOUND tiles for fade.
        #endregion

        #region Constructor/Destructor
        public GeneratedLevel(IGenerationAlgorithm dungeonGeneration, int worldMin, int worldMax, IUserInputHandler handler)
        {
            levelComponents = new LevelComponents();
            inputHandler = handler;
            dungeonDimensions = dungeonGeneration.GenerateDungeon(ref dungeonGrid, worldMin, worldMax, random);
        }
        #endregion

        #region Load Logic
        public void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            floor = content.Load<Texture2D>("Sprites/ball");
            CreatePlayer(content, graphics, camera);
        }

        private void CreatePlayer(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            Guid playerId = levelComponents.CreateEntity();
            levelComponents.Entities.Where(x => x.Id == playerId).Single().ComponentFlags =
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_HEALTH | Component.COMPONENT_ISPLAYER | Component.COMPONENT_ISCOLLIDABLE;
            levelComponents.SpriteComponents[playerId] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball"), Color = Color.Red };
            levelComponents.VelocityComponents[playerId] = new Velocity() { x = 1000, y = 1000 };
            int fillX = 0;
            int fillY = 0;
            do
            {
                fillX = random.Next(0, (int)dungeonDimensions.X);
                fillY = random.Next(0, (int)dungeonDimensions.Y);
            } while ((dungeonGrid[fillX, fillY] & DungeonTiles.FLOOR) != DungeonTiles.FLOOR);
            levelComponents.PositionComponents[playerId] = new Position() { Pos = new Vector2(fillX, fillY) };
            levelComponents.HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
            camera.Position = new Vector2((int)levelComponents.PositionComponents[playerId].Pos.X * cellSize + levelComponents.SpriteComponents[playerId].SpritePath.Bounds.Center.X, 
                (int)levelComponents.PositionComponents[playerId].Pos.Y * cellSize + levelComponents.SpriteComponents[playerId].SpritePath.Bounds.Center.Y);
        }
        #endregion

        #region Update Logic
        public ILevel UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            ILevel nextLevel = this;
            levelComponents.EntitiesToDelete.ForEach(x => levelComponents.DestroyEntity(x));
            levelComponents.EntitiesToDelete.Clear();
            ShowTiles();
            inputHandler.HandleInput(levelComponents, graphics, gameTime, prevKeyboardState, prevMouseState, prevGamepadState, camera, ref dungeonGrid);
            UpdateCamera(camera, gameTime);
            #region Debug changing levels
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter))
            {
                nextLevel = new GeneratedLevel(new CaveGeneration(),75,125, new GeneratedDungeonInputHandler());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.RightShift) && !prevKeyboardState.IsKeyDown(Keys.RightShift))
            {
                nextLevel = new GeneratedLevel(new CaveArenaGeneration(), 75, 100, new GeneratedDungeonInputHandler());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.RightControl) && !prevKeyboardState.IsKeyDown(Keys.RightControl))
            {
                nextLevel = new GeneratedLevel(new RuinsGeneration(), 75, 225, new GeneratedDungeonInputHandler());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.RightAlt) && !prevKeyboardState.IsKeyDown(Keys.RightAlt))
            {
                nextLevel = new GeneratedLevel(new RuinsArenaGeneration(), 75, 300, new GeneratedDungeonInputHandler());
            }
            #endregion
            return nextLevel;
        }

        private void ShowTiles()
        {
            int sightRange = 12;
            Guid entity = levelComponents.Entities.Where(x => (x.ComponentFlags & Component.COMPONENT_ISPLAYER) == Component.COMPONENT_ISPLAYER).FirstOrDefault().Id;
            Vector2 position = levelComponents.PositionComponents[entity].Pos;
            //Reset Range
            for(int i = 0; i < dungeonDimensions.X; i++)
            {
                for(int j = 0; j < dungeonDimensions.Y; j++)
                {
                    dungeonGrid[i, j] &= ~DungeonTiles.IN_RANGE;
                    dungeonGrid[i, j] &= ~DungeonTiles.BLOCKED_RANGE;
                }
            }

            /*
                         Shared
                         edge by
              Shared     1 & 2      Shared
              edge by\      |      /edge by
              1 & 8   \     |     / 2 & 3
                       \1111|2222/
                       8\111|222/3
                       88\11|22/33
                       888\1|2/333
              Shared   8888\|/3333  Shared
              edge by-------@-------edge by
              7 & 8    7777/|\4444  3 & 4
                       777/6|5\444
                       77/66|55\44
                       7/666|555\4
                       /6666|5555\
              Shared  /     |     \ Shared
              edge by/      |      \edge by
              6 & 7      Shared     4 & 5
                         edge by 
                         5 & 6
             */
            List<Vector2> blockingSquares = new List<Vector2>();
            //Octant 1
            for (int z = 0; z < sightRange; z++)
            {
                for(int j = 0; j <=  z; j++)
                {
                    for(int i = 0; i <= j; i++)
                    {
                        if ( position.X - i >= 0 && position.Y - j >= 0)
                        {
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y - j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X - i, (int)position.Y - j);
                                for (int l = 0; l <= sightRange - z; l++)
                                {
                                    for (int m = 0; m <= l; m++)
                                    {
                                        if (blockedPosition.X - m >= 0 && blockedPosition.Y - l >= 0 && (new Vector2((int)blockedPosition.X - m, (int)blockedPosition.Y - l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X - m, (int)blockedPosition.Y - l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }

                            if ((dungeonGrid[(int)position.X - i, (int)position.Y - j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X - i, (int)position.Y - j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X - i, (int)position.Y - j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X - i, (int)position.Y - j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 2
            for (int z = 0; z < sightRange; z++)
            {
                for (int j = 0; j <= z; j++)
                {
                    for (int i = 0; i <= j; i++)
                    {
                        if (position.X + i < dungeonDimensions.X && position.Y - j >= 0)
                        {
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y - j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X + i, (int)position.Y - j);
                                for (int l = 0; l <= sightRange - z; l++)
                                {
                                    for (int m = 0; m <= l; m++)
                                    {
                                        if (blockedPosition.X + m < dungeonDimensions.X && blockedPosition.Y - l >= 0 && (new Vector2((int)blockedPosition.X + m, (int)blockedPosition.Y - l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X + m, (int)blockedPosition.Y - l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y - j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X + i, (int)position.Y - j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X + i, (int)position.Y - j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X + i, (int)position.Y - j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 3
            for (int z = 0; z < sightRange; z++)
            {
                for (int i = 0; i <= z; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (position.X + i < dungeonDimensions.X && position.Y - j >= 0)
                        {
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y - j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X + i, (int)position.Y - j);
                                for (int m = 0; m <= sightRange - z; m++)
                                {
                                    for (int l = 0; l <= m; l++)
                                    {
                                        if (blockedPosition.X + m < dungeonDimensions.X && blockedPosition.Y - l >= 0 && (new Vector2((int)blockedPosition.X + m, (int)blockedPosition.Y - l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X + m, (int)blockedPosition.Y - l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y - j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X + i, (int)position.Y - j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X + i, (int)position.Y - j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X + i, (int)position.Y - j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 4
            for (int z = 0; z < sightRange; z++)
            {
                for (int i = 0; i <= z; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (position.X + i < dungeonDimensions.X && position.Y + j < dungeonDimensions.Y)
                        {
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y + j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X + i, (int)position.Y + j);
                                for (int m = 0; m <= sightRange - z; m++)
                                {
                                    for (int l = 0; l <= m; l++)
                                    {
                                        if (blockedPosition.X + m < dungeonDimensions.X && blockedPosition.Y + l < dungeonDimensions.Y && (new Vector2((int)blockedPosition.X + m, (int)blockedPosition.Y + l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X + m, (int)blockedPosition.Y + l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y + j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X + i, (int)position.Y + j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X + i, (int)position.Y + j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X + i, (int)position.Y + j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 5
            for (int z = 0; z < sightRange; z++)
            {
                for (int j = 0; j <= z; j++)
                {
                    for (int i = 0; i <= j; i++)
                    {
                        if (position.X + i < dungeonDimensions.X && position.Y + j < dungeonDimensions.Y)
                        {
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y + j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X + i, (int)position.Y + j);
                                for (int l = 0; l <= sightRange - z; l++)
                                {
                                    for (int m = 0; m <= l; m++)
                                    {
                                        if (blockedPosition.X + m < dungeonDimensions.X && blockedPosition.Y + l < dungeonDimensions.Y && (new Vector2((int)blockedPosition.X + m, (int)blockedPosition.Y + l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X + m, (int)blockedPosition.Y + l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X + i, (int)position.Y + j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X + i, (int)position.Y + j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X + i, (int)position.Y + j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X + i, (int)position.Y + j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 6
            for (int z = 0; z < sightRange; z++)
            {
                for (int j = 0; j <= z; j++)
                {
                    for (int i = 0; i <= j; i++)
                    {
                        if (position.X - i >= 0 && position.Y + j < dungeonDimensions.Y)
                        {
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y + j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X - i, (int)position.Y + j);
                                for (int l = 0; l <= sightRange - z; l++)
                                {
                                    for (int m = 0; m <= l; m++)
                                    {
                                        if (blockedPosition.X - m >= 0 && blockedPosition.Y + l < dungeonDimensions.Y && (new Vector2((int)blockedPosition.X + m, (int)blockedPosition.Y + l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X - m, (int)blockedPosition.Y + l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y + j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X - i, (int)position.Y + j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X - i, (int)position.Y + j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X - i, (int)position.Y + j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 7
            for (int z = 0; z < sightRange; z++)
            {
                for (int i = 0; i <= z; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (position.X - i >= 0 && position.Y + j < dungeonDimensions.Y)
                        {
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y + j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X - i, (int)position.Y + j);
                                for (int m = 0; m <= sightRange - z; m++)
                                {
                                    for (int l = 0; l <= m; l++)
                                    {
                                        if (blockedPosition.X - m >= 0 && blockedPosition.Y + l < dungeonDimensions.Y && (new Vector2((int)blockedPosition.X - m, (int)blockedPosition.Y + l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X - m, (int)blockedPosition.Y + l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y + j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X - i, (int)position.Y + j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X - i, (int)position.Y + j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X - i, (int)position.Y + j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }

            //Octant 8
            for (int z = 0; z < sightRange; z++)
            {
                for (int i = 0; i <= z; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (position.X - i >= 0 && position.Y - j >= 0)
                        {
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y - j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                            {
                                Vector2 blockedPosition = new Vector2((int)position.X - i, (int)position.Y - j);
                                for (int m = 0; m <= sightRange - z; m++)
                                {
                                    for (int l = 0; l <= m; l++)
                                    {
                                        if (blockedPosition.X - m >= 0 && blockedPosition.Y - l >= 0 && (new Vector2((int)blockedPosition.X - m, (int)blockedPosition.Y - l) != blockedPosition))
                                        {
                                            dungeonGrid[(int)blockedPosition.X - m, (int)blockedPosition.Y - l] |= DungeonTiles.BLOCKED_RANGE;
                                        }
                                    }
                                }

                            }
                            if ((dungeonGrid[(int)position.X - i, (int)position.Y - j] & DungeonTiles.BLOCKED_RANGE) != (DungeonTiles.BLOCKED_RANGE))
                            {
                                if ((dungeonGrid[(int)position.X - i, (int)position.Y - j] & DungeonTiles.FOUND) != (DungeonTiles.FOUND))
                                {
                                    dungeonGrid[(int)position.X - i, (int)position.Y - j] |= DungeonTiles.NEWLY_FOUND;
                                }
                                dungeonGrid[(int)position.X - i, (int)position.Y - j] |= DungeonTiles.FOUND | DungeonTiles.IN_RANGE;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateCamera(Camera camera, GameTime gameTime)
        {
            Guid entity = levelComponents.Entities.Where(x => (x.ComponentFlags & Component.COMPONENT_ISPLAYER) == Component.COMPONENT_ISPLAYER).FirstOrDefault().Id;
            camera.Target = new Vector2((int)levelComponents.PositionComponents[entity].Pos.X * cellSize + levelComponents.SpriteComponents[entity].SpritePath.Bounds.Center.X, 
                (int)levelComponents.PositionComponents[entity].Pos.Y * cellSize + levelComponents.SpriteComponents[entity].SpritePath.Bounds.Center.Y);
            if (Vector2.Distance(camera.Position, camera.Target) > 0)
            {
                float distance = Vector2.Distance(camera.Position, camera.Target);
                Vector2 direction = Vector2.Normalize(camera.Target - camera.Position);
                float velocity = distance * 2.5f;
                camera.Position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        #endregion

        #region Draw Logic
        public void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            DrawDungeon(spriteBatch, graphics, camera);
            DrawPlayer(spriteBatch, graphics, camera);
        }

        private void DrawDungeon(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            
            bool incrementNew = false;
            for (int i = 0; i < (int)dungeonDimensions.X; i++)
            {
                for (int j = 0; j < (int)dungeonDimensions.Y; j++)
                {
                    if ((dungeonGrid[i, j] & DungeonTiles.FOUND) == DungeonTiles.FOUND && (dungeonGrid[i,j] & DungeonTiles.IN_RANGE) != DungeonTiles.IN_RANGE
                         && (dungeonGrid[i, j] & DungeonTiles.NEWLY_FOUND) != DungeonTiles.NEWLY_FOUND)
                    {
                        if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                            spriteBatch.Draw(floor, tile, Color.DarkGreen);
                        }
                        if ((dungeonGrid[i, j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                        {
                            Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                            spriteBatch.Draw(floor, tile, Color.DarkViolet);
                        }
                    }
                    else if ((dungeonGrid[i, j] & DungeonTiles.FOUND) == DungeonTiles.FOUND && (dungeonGrid[i, j] & DungeonTiles.IN_RANGE) == DungeonTiles.IN_RANGE
                        && (dungeonGrid[i, j] & DungeonTiles.NEWLY_FOUND) != DungeonTiles.NEWLY_FOUND)
                    {
                        if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                            spriteBatch.Draw(floor, tile, Color.Green);
                        }
                        if ((dungeonGrid[i, j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                        {
                            Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                            spriteBatch.Draw(floor, tile, Color.Violet);
                        }
                    }
                    else if ((dungeonGrid[i, j] & DungeonTiles.NEWLY_FOUND) == DungeonTiles.NEWLY_FOUND)
                    {
                        incrementNew = true;
                        if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                            spriteBatch.Draw(floor, tile, new Color(Color.Green, opacity));
                        }
                        if ((dungeonGrid[i, j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                        {
                            Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                            spriteBatch.Draw(floor, tile, new Color(Color.Violet, opacity));
                        }
                    }


                }
            }
            if (incrementNew)
            {
                opacity += .21f;
            }
            if (opacity > 1)
            {
                opacity = .1f;
                for (int i = 0; i < (int)dungeonDimensions.X; i++)
                {
                    for (int j = 0; j < (int)dungeonDimensions.Y; j++)
                    {
                        dungeonGrid[i, j] &= ~DungeonTiles.NEWLY_FOUND;
                    }
                }
            }
        }

        private void DrawPlayer(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            foreach (Entity entity in levelComponents.Entities)
            {
                if ((entity.ComponentFlags & (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER)) == (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER))
                {
                    spriteBatch.Draw(levelComponents.SpriteComponents[entity.Id].SpritePath, new Vector2((int)levelComponents.PositionComponents[entity.Id].Pos.X * cellSize, 
                        (int)levelComponents.PositionComponents[entity.Id].Pos.Y * cellSize), levelComponents.SpriteComponents[entity.Id].Color);
                }
            }
        }
        #endregion
    }
}
