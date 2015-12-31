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
        ROCK = 1 << 6
    }

    public class Dungeon: Level
    {
        private Texture2D floor;
        private int worldI = 0;
        private int worldJ = 0;
        private DungeonTiles[,] dungeonGrid = null;
        private const int cellSize = 40;

        private Random random = new Random();

        public Dungeon(int worldMax, int worldMin)
        {
            worldI = random.Next(worldMin, worldMax);
            worldJ = random.Next(worldMin, worldMax);
        }

        ~Dungeon()
        {
            Array.Clear(dungeonGrid, 0, worldI * worldJ);
        }

        public override void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            floor = content.Load<Texture2D>("Sprites/Ball");
            CreatePlayer(content, graphics);
            GenerateDungeon();
        }

        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            Level nextLevel = this;
            Movement(graphics, gameTime, camera, prevKeyboardState);
            base.UpdateLevel(gameTime, content, graphics, prevKeyboardState, prevMouseState, prevGamepadState, camera);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter))
            {
                nextLevel = new Dungeon(300,100);
            }
            return nextLevel;
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            DrawDungeon(spriteBatch, graphics, camera);
            DrawPlayer(spriteBatch, graphics, camera);
        }

        private void CreatePlayer(ContentManager content, GraphicsDeviceManager graphics)
        {
            Guid playerId = CreateEntity();
            Entities.Where(x => x.Id == playerId).Single().ComponentFlags =
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_HEALTH | Component.COMPONENT_ISPLAYER | Component.COMPONENT_ISCOLLIDABLE;
            SpriteComponents[playerId] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball"), Color = Color.Red };
            VelocityComponents[playerId] = new Velocity() { x = 1000, y = 1000 };
            PositionComponents[playerId] = new Position() { Pos = new Vector2(worldI*cellSize / 2, worldJ*cellSize / 2) };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }

        private void GenerateDungeon()
        {
            dungeonGrid = new DungeonTiles[worldI, worldJ];
            var minSize = 12;
            var maxSize = 45;
            var attemptLimit = 100;
            var tries = 0;

            //Empty world
            //for (int i = 0; i < worldI * worldJ; i++)
            //{
            //    dungeonGrid[i % worldI, i / worldI] = DungeonTiles.ROCK;
            //}


                //Cellular Automata
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        int choice = random.Next(0,101);
                        dungeonGrid[i, j] = (choice <= 45) ? DungeonTiles.ROCK : DungeonTiles.FLOOR;
                    }
                }


            int iterations = 4;
            for (int z = 0; z <= iterations; z++)
            {
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        int numRocks = 0;
                        //Check 8 directions and self
                        //Self:
                        if(dungeonGrid[i,j] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Topleft
                        if(i-1 < 0 || j-1 < 0)
                        {
                            numRocks += 1;
                        }
                        else if (dungeonGrid[i - 1, j - 1] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Top
                        if(j-1 <0)
                        {
                            numRocks += 1;
                        }
                        else if(dungeonGrid[i, j - 1] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Topright
                        if(i+1 > worldI-1 || j-1 < 0)
                        {
                            numRocks += 1;
                        }
                        else if(dungeonGrid[i + 1, j - 1] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Left
                        if(i-1 < 0)
                        {
                            numRocks += 1;
                        }
                        else if(dungeonGrid[i - 1, j] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Right
                        if(i+1 > worldI-1)
                        {
                            numRocks += 1;
                        }
                        else if (dungeonGrid[i + 1, j] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Bottomleft
                        if(i-1 < 0 || j + 1 > worldJ-1)
                        {
                            numRocks += 1;
                        }
                        else if(dungeonGrid[i - 1, j + 1] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //Bottom
                        if(j+1 > worldJ-1)
                        {
                            numRocks += 1;
                        }
                        else if(dungeonGrid[i, j + 1] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }
                        //BottomRight
                        if(i+1 > worldI-1 || j+1 > worldJ-1)
                        {
                            numRocks += 1;
                        }
                        else if(dungeonGrid[i + 1, j + 1] == DungeonTiles.ROCK)
                        {
                            numRocks += 1;
                        }


                        if(numRocks >= 5 || i == 0 || j == 0 || i == worldI-1 || j == worldJ-1)
                        {
                            dungeonGrid[i, j] = DungeonTiles.ROCK;
                        }
                        else
                        {
                            dungeonGrid[i, j] = DungeonTiles.FLOOR;
                        }
                    }
                }
            }

            ////Create random rooms
            //while(tries < attemptLimit)
            //{

            //    var height = random.Next(minSize, maxSize);
            //    var width = random.Next(minSize, maxSize);
            //    var posX = random.Next(0, worldI-width);
            //    var posY = random.Next(0, worldJ-height);

            //    //Check if that room collides with anything
            //    bool collide = false;
            //    for(int i = posX; i < posX + width; i++)
            //    {
            //        for(int j = posY; j < posY + height; j++)
            //        {
            //            if((dungeonGrid[i,j] & DungeonTiles.ROCK) != DungeonTiles.ROCK)
            //            {
            //                collide = true;
            //            }
            //        }
            //    }

            //    if(!collide)
            //    {
            //        for (int i = posX; i < posX+width; i++)
            //        {
            //            for (int j = posY; j < posY+height; j++)
            //            {
            //                dungeonGrid[i, j] =  (j == posY || j == posY+height-1 || i == posX || i == posX+width-1)  ?  DungeonTiles.WALL : DungeonTiles.FLOOR;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        tries += 1;
            //    }
            //}
        }

        private void DrawDungeon(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            for(int i = 0; i < worldI; i++)
            {
                for(int j = 0; j < worldJ; j++)
                {
                    if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                    {
                        Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                        spriteBatch.Draw(floor, tile, Color.Green);
                    }
                    if ((dungeonGrid[i, j] & DungeonTiles.ROCK) == DungeonTiles.ROCK)
                    {
                        Rectangle tile = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                        spriteBatch.Draw(floor, tile, Color.Blue);
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
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y -= VelocityComponents[entity.Id].y * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y += VelocityComponents[entity.Id].y * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X += VelocityComponents[entity.Id].x * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= VelocityComponents[entity.Id].x * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    //if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                    //{
                    //    KeyboardState keyState = Keyboard.GetState();
                    //    if (keyState.IsKeyDown(Keys.W) && !prevKey.IsKeyDown(Keys.W))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.Y -= 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.S) && !prevKey.IsKeyDown(Keys.S))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.Y += 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.D) && !prevKey.IsKeyDown(Keys.D))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.X += 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.A) && !prevKey.IsKeyDown(Keys.A))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.X -= 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                    //{
                    //    KeyboardState keyState = Keyboard.GetState();
                    //    if (keyState.IsKeyDown(Keys.W))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.Y -= 40;
                    //        if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.S))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.Y += 40;
                    //        if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.D))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.X += 40;
                    //        if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.A))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.X -= 40;
                    //        if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    camera.Position = new Vector2((int)PositionComponents[entity.Id].Pos.X + SpriteComponents[entity.Id].SpritePath.Bounds.Center.X, (int)PositionComponents[entity.Id].Pos.Y + SpriteComponents[entity.Id].SpritePath.Bounds.Center.Y);
                }
            }
        }

        private void DrawPlayer(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER)) == (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER))
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, new Vector2((int)PositionComponents[entity.Id].Pos.X, (int)PositionComponents[entity.Id].Pos.Y), SpriteComponents[entity.Id].Color);
                }
            }
        }


    }
}
