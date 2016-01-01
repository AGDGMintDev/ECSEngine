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
using EmnityDX.Engine.Ancillary;

namespace EmnityDX.Objects.LevelData.PlayingStateLevels
{
    [Flags]
    public enum DungeonTiles
    {
        NONE = 0,
        FLOOR = 1 << 0,
        DOOR = 1 << 1,
        UNVISITED = 1 << 2,
        VISITED = 1 << 3,
        WALL = 1 << 4,
        FIRST_ROOM = 1 << 5,
        ROCK = 1 << 6,
        FOUND = 1 << 7
    }

    public class Caves: Level
    {
        private Texture2D floor;
        private Texture2D wall;
        private int worldI = 0;
        private int worldJ = 0;
        private DungeonTiles[,] dungeonGrid = null;
        private const int cellSize = 40;
        private Vector2 cameraTarget;
        private float cameraVelocity = 100.0f;

        private Random random = new Random();

        public Caves(int worldMax, int worldMin)
        {
            worldI = random.Next(worldMin, worldMax);
            worldJ = random.Next(worldMin, worldMax);
        }

        ~Caves()
        {
            Array.Clear(dungeonGrid, 0, worldI * worldJ);
        }

        public override void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            floor = content.Load<Texture2D>("Sprites/Ball");
            //wall = content.Load<Texture2D>("Sprites/asciiwall");
            GenerateDungeon();
            CreatePlayer(content, graphics, camera);
        }

        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            Level nextLevel = this;
            Movement(graphics, gameTime, camera, prevKeyboardState);
            ShowTiles();
            base.UpdateLevel(gameTime, content, graphics, prevKeyboardState, prevMouseState, prevGamepadState, camera);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter))
            {
                nextLevel = new Caves(125,75);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.PageDown))
            {
                camera.ResetScreenScale(graphics, new Vector2(3968 * 2, 2232 * 2));
            }
            if(Vector2.Distance(camera.Position, cameraTarget) > 0)
            {
                float distance = Vector2.Distance(camera.Position, cameraTarget);
                Vector2 direction = Vector2.Normalize(cameraTarget - camera.Position);
                float velocity = distance * 2.5f;
                camera.Position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            return nextLevel;
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            DrawDungeon(spriteBatch, graphics, camera);
            DrawPlayer(spriteBatch, graphics, camera);
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

        private void ShowTiles()
        {
            int sightRange = 6;
            Guid entity = Entities.Where(x => (x.ComponentFlags & Component.COMPONENT_ISPLAYER) == Component.COMPONENT_ISPLAYER).FirstOrDefault().Id;
            Vector2 position = PositionComponents[entity].Pos;
        }

        private void GenerateDungeon()
        {
            dungeonGrid = new DungeonTiles[worldI, worldJ];

                
            bool acceptable = false;

            while (!acceptable)
            {
                //Cellular Automata
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        int choice = random.Next(0, 101);
                        dungeonGrid[i, j] = (choice <= 41) ? DungeonTiles.ROCK : DungeonTiles.FLOOR;
                    }
                }

                int iterations = 12;
                for (int z = 0; z <= iterations; z++)
                {
                    DungeonTiles[,] newMap = new DungeonTiles[worldI, worldJ];
                    for (int i = 0; i < worldI; i++)
                    {
                        for (int j = 0; j < worldJ; j++)
                        {
                            int numRocks = 0;
                            int farRocks = 0;
                            //Check 8 directions and self
                            //Self:
                            if (dungeonGrid[i, j] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Topleft
                            if (i - 1 < 0 || j - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i - 1, j - 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Top
                            if (j - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i, j - 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Topright
                            if (i + 1 > worldI - 1 || j - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i + 1, j - 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Left
                            if (i - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i - 1, j] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Right
                            if (i + 1 > worldI - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i + 1, j] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Bottomleft
                            if (i - 1 < 0 || j + 1 > worldJ - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i - 1, j + 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Bottom
                            if (j + 1 > worldJ - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i, j + 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //BottomRight
                            if (i + 1 > worldI - 1 || j + 1 > worldJ - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i + 1, j + 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }

                            //Check 8 directions for far rocks

                            //Topleft
                            if (i - 2 < 0 || j - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i - 2, j - 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Top
                            if (j - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i, j - 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Topright
                            if (i + 2 > worldI - 2 || j - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i + 2, j - 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Left
                            if (i - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i - 2, j] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Right
                            if (i + 2 > worldI - 2)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i + 2, j] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Bottomleft
                            if (i - 2 < 0 || j + 2 > worldJ - 2)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i - 2, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Bottom
                            if (j + 2 > worldJ - 2)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //BottomRight
                            if (i + 2 > worldI - 2 || j + 2 > worldJ - 2 || dungeonGrid[i + 2, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i + 2, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }


                            if (numRocks >= 5 || i == 0 || j == 0 || i == worldI - 1 || j == worldJ - 1 || (z <= 3 && numRocks + farRocks <= 2))
                            {
                                newMap[i, j] = DungeonTiles.ROCK;
                            }
                            else
                            {
                                newMap[i, j] = DungeonTiles.FLOOR;
                            }
                        }
                    }
                    Array.Copy(newMap, dungeonGrid, worldJ * worldI);
                }

                int fillX = 0;
                int fillY = 0;
                do
                {
                    fillX = random.Next(0, worldI);
                    fillY = random.Next(0, worldJ);
                } while ((dungeonGrid[fillX, fillY] & DungeonTiles.FLOOR) != DungeonTiles.FLOOR);

                FloodFill(fillX, fillY);

                double connectedTiles = 0.0;
                double totalTiles = 0.0;
                for(int i = 0; i < worldI; i++)
                {
                    for(int j = 0; j < worldJ; j++)
                    {
                        if((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            totalTiles += 1.0;
                            if ((dungeonGrid[i, j] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
                            {
                                connectedTiles += 1.0;
                            }
                        }
                    }
                }

                if(connectedTiles / totalTiles >= .90)
                {
                    for (int i = 0; i < worldI; i++)
                    {
                        for (int j = 0; j < worldJ; j++)
                        {
                            if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && (dungeonGrid[i, j] & DungeonTiles.VISITED) != DungeonTiles.VISITED)
                            {
                                dungeonGrid[i, j] = DungeonTiles.WALL;
                            }
                        }
                    }
                    acceptable = true;
                }

            }

        }

        private void FloodFill(int x, int y)
        {
            if (x < 0 || y < 0 || x >= worldI || y >= worldJ)
            {
                return;
            }
            if((dungeonGrid[x,y] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
            {
                return;
            }
            if ((dungeonGrid[x, y] & DungeonTiles.WALL) == DungeonTiles.WALL)
            {
                return;
            }

            Queue<Vector2> floodQueue = new Queue<Vector2>();
            floodQueue.Enqueue(new Vector2(x, y));
            
            while(floodQueue.Count > 0)
            {
                Vector2 pos = floodQueue.Dequeue();
                if((dungeonGrid[(int)pos.X, (int)pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && (dungeonGrid[(int)pos.X, (int)pos.Y] & DungeonTiles.VISITED) != DungeonTiles.VISITED)
                {
                    x = (int)pos.X;
                    y = (int)pos.Y;
                    dungeonGrid[(int)pos.X, (int)pos.Y] = dungeonGrid[(int)pos.X, (int)pos.Y] | DungeonTiles.VISITED;
                    HashSet<Vector2> toAdd = new HashSet<Vector2>();
                    toAdd.Add(new Vector2(x + 1, y + 1));
                    toAdd.Add(new Vector2(x, y + 1));
                    toAdd.Add(new Vector2(x + 1, y));
                    toAdd.Add(new Vector2(x - 1, y - 1));
                    toAdd.Add(new Vector2(x - 1, y));
                    toAdd.Add(new Vector2(x, y - 1));
                    toAdd.Add(new Vector2(x + 1, y - 1));
                    toAdd.Add(new Vector2(x - 1, y + 1));
                    foreach(var vector in toAdd)
                    {
                        if(!floodQueue.Contains(vector))
                        {
                            floodQueue.Enqueue(vector);
                        }
                    }
                    toAdd.Clear();
                }
            }
            
        }

        private void DrawDungeon(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            for(int i = 0; i < worldI; i++)
            {
                for(int j = 0; j < worldJ; j++)
                {
                    if ((dungeonGrid[i, j] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
                    {
                        Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                        spriteBatch.Draw(floor, tile, Color.DarkGreen);
                    }
                }
            }
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
                        if(((dungeonGrid[(int)pos.Pos.X,(int)pos.Pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR))
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
                    cameraTarget = new Vector2((int)PositionComponents[entity.Id].Pos.X * cellSize + SpriteComponents[entity.Id].SpritePath.Bounds.Center.X, (int)PositionComponents[entity.Id].Pos.Y * cellSize + SpriteComponents[entity.Id].SpritePath.Bounds.Center.Y);
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


    }
}
