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
        #endregion

        #region Constructor/Destructor
        public GeneratedLevel(IGenerationAlgorithm dungeonGeneration, int worldMin, int worldMax, IUserInputHandler handler)
        {
            levelComponents = new LevelComponents();
            inputHandler = handler;
            dungeonDimensions = dungeonGeneration.GenerateDungeon(ref dungeonGrid, worldMin, worldMax, random);
        }

        ~GeneratedLevel()
        {
            Array.Clear(dungeonGrid, 0, (int)dungeonDimensions.X * (int)dungeonDimensions.Y);
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
            inputHandler.HandleInput(levelComponents, graphics, gameTime, prevKeyboardState, prevMouseState, prevGamepadState, camera, ref dungeonGrid);
            UpdateCamera(camera, gameTime);
            ShowTiles();
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
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
            {
                camera.ResetScreenScale(graphics, new Vector2(3968 * 2, 2232 * 2));
            }
            return nextLevel;
        }

        private void ShowTiles()
        {
            int sightRange = 6;
            Guid entity = levelComponents.Entities.Where(x => (x.ComponentFlags & Component.COMPONENT_ISPLAYER) == Component.COMPONENT_ISPLAYER).FirstOrDefault().Id;
            Vector2 position = levelComponents.PositionComponents[entity].Pos;
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
            for (int i = 0; i < (int)dungeonDimensions.X; i++)
            {
                for (int j = 0; j < (int)dungeonDimensions.Y; j++)
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
