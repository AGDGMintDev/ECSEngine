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
using EmnityDX.Engine.ProceduralGeneration;

namespace EmnityDX.Objects.LevelData.PlayingStateLevels
{
    public class Caves : Level
    {
        private Texture2D floor;
        private int worldI = 0;
        private int worldJ = 0;
        private DungeonTiles[,] dungeonGrid = null;
        private const int cellSize = 40;
        private Random random = new Random();

        public Caves(int worldMin, int worldMax)
        {
            worldI = random.Next(worldMin, worldMax);
            worldJ = random.Next(worldMin, worldMax);
        }

        ~Caves()
        {
            Array.Clear(dungeonGrid, 0, worldI * worldJ);
        }

        #region Load Logic
        public override void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            floor = content.Load<Texture2D>("Sprites/Ball");
            //wall = content.Load<Texture2D>("Sprites/asciiwall");
            CaveGeneration.GenerateDungeon(ref dungeonGrid, worldI, worldJ, random);
            CreatePlayer(content, graphics, camera);
        }

        private void CreatePlayer(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            Guid playerId = CreateEntity();
            Entities.Where(x => x.Id == playerId).Single().ComponentFlags =
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_HEALTH | Component.COMPONENT_ISPLAYER | Component.COMPONENT_ISCOLLIDABLE;
            SpriteComponents[playerId] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball"), Color = Color.Red };
            VelocityComponents[playerId] = new Velocity() { x = 1000, y = 1000 };
            int fillX = 0;
            int fillY = 0;
            do
            {
                fillX = random.Next(0, worldI);
                fillY = random.Next(0, worldJ);
            } while ((dungeonGrid[fillX, fillY] & DungeonTiles.FLOOR) != DungeonTiles.FLOOR);
            PositionComponents[playerId] = new Position() { Pos = new Vector2(fillX, fillY) };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
            camera.Position = new Vector2((int)PositionComponents[playerId].Pos.X * cellSize + SpriteComponents[playerId].SpritePath.Bounds.Center.X, (int)PositionComponents[playerId].Pos.Y * cellSize + SpriteComponents[playerId].SpritePath.Bounds.Center.Y);
        }
        #endregion

        #region Update Logic
        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            Level nextLevel = this;
            Movement(graphics, gameTime, camera, prevKeyboardState);
            UpdateCamera(camera, gameTime);
            ShowTiles();
            base.UpdateLevel(gameTime, content, graphics, prevKeyboardState, prevMouseState, prevGamepadState, camera);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter))
            {
                nextLevel = new Caves(75, 125);
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
            Guid entity = Entities.Where(x => (x.ComponentFlags & Component.COMPONENT_ISPLAYER) == Component.COMPONENT_ISPLAYER).FirstOrDefault().Id;
            Vector2 position = PositionComponents[entity].Pos;
        }

        private void Movement(GraphicsDeviceManager graphics, GameTime gameTime, Camera camera, KeyboardState prevKey)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                {
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.NumPad8) && !prevKey.IsKeyDown(Keys.NumPad8))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad2) && !prevKey.IsKeyDown(Keys.NumPad2))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad6) && !prevKey.IsKeyDown(Keys.NumPad6))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad4) && !prevKey.IsKeyDown(Keys.NumPad4))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad7) && !prevKey.IsKeyDown(Keys.NumPad7))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= 1;
                        pos.Pos.Y -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad9) && !prevKey.IsKeyDown(Keys.NumPad9))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X += 1;
                        pos.Pos.Y -= 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad1) && !prevKey.IsKeyDown(Keys.NumPad1))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= 1;
                        pos.Pos.Y += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    if (keyState.IsKeyDown(Keys.NumPad3) && !prevKey.IsKeyDown(Keys.NumPad3))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X += 1;
                        pos.Pos.Y += 1;
                        if (((dungeonGrid[(int)pos.Pos.X, (int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
                        {
                            PositionComponents[entity.Id] = pos;
                        }
                    }
                    camera.Target = new Vector2((int)PositionComponents[entity.Id].Pos.X * cellSize + SpriteComponents[entity.Id].SpritePath.Bounds.Center.X, (int)PositionComponents[entity.Id].Pos.Y * cellSize + SpriteComponents[entity.Id].SpritePath.Bounds.Center.Y);
                }
            }
        }

        private void UpdateCamera(Camera camera, GameTime gameTime)
        {
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
        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            DrawDungeon(spriteBatch, graphics, camera);
            DrawPlayer(spriteBatch, graphics, camera);
        }

        private void DrawDungeon(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            for (int i = 0; i < worldI; i++)
            {
                for (int j = 0; j < worldJ; j++)
                {
                    if ((dungeonGrid[i, j] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
                    {
                        Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                        spriteBatch.Draw(floor, tile, Color.DarkGreen);
                    }
                }
            }
        }

        private void DrawPlayer(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER)) == (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER))
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, new Vector2((int)PositionComponents[entity.Id].Pos.X * cellSize, (int)PositionComponents[entity.Id].Pos.Y * cellSize), SpriteComponents[entity.Id].Color);
                }
            }
        }
        #endregion

    }
}
