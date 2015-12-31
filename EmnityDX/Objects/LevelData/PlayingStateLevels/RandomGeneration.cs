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
    public class RandomGeneration: Level
    {

        private List<Rectangle> rooms;
        private List<DungeonRoom> dungeonRooms;
        private List<Rectangle> halls;
        private List<Rectangle> highHalls;
        private List<Rectangle> slideHalls;
        private Texture2D floor;

        public override void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            rooms = new List<Rectangle>();
            halls = new List<Rectangle>();
            highHalls = new List<Rectangle>();
            slideHalls = new List<Rectangle>();
            floor = content.Load<Texture2D>("Sprites/Ball");
            dungeonRooms = new List<DungeonRoom>();
            GenerateDungeon();
            CreatePlayer(content, graphics);
        }

        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            Level nextLevel = this;
            Movement(graphics,gameTime, camera, prevKeyboardState);
            base.UpdateLevel(gameTime, content, graphics, prevKeyboardState, prevMouseState, prevGamepadState, camera);
            if(Keyboard.GetState().IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter))
            {
                nextLevel = new RandomGeneration();
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
            VelocityComponents[playerId] = new Velocity() { x = 10000, y = 10000 };
            PositionComponents[playerId] = new Position() { Pos = new Vector2((int)Math.Ceiling((double)dungeonRooms[0].Room.Center.X / 40) * 40, (int)Math.Ceiling((double)dungeonRooms[0].Room.Center.Y / 40) * 40) };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }

        private void GenerateDungeon()
        {
            Random random = new Random();
            var levelHeight = random.Next(5000, 10000);
            var levelWidth = random.Next(5000, 10000);
            var minSize = 10;
            var maxSize = 30;
            var cellSize = 40;
            var attempts = 100;
            var tries = 0;

            //while (tries < attempts)
            //{
            //    Rectangle room = new Rectangle();
            //    room.X = (int)Math.Ceiling((double)random.Next(0, levelSize) / 40) * 40;
            //    room.Y = (int)Math.Ceiling((double)random.Next(0, levelSize) / 40) * 40;
            //    room.Width = (int)Math.Ceiling((double)random.Next(minSize * cellSize, maxSize * cellSize) / 40) * 40;
            //    room.Height = (int)Math.Ceiling((double)random.Next(minSize * cellSize, maxSize * cellSize) / 40) * 40;

            //    if (rooms.Any(x =>  ((new Rectangle(x.X,x.Y,x.Width+cellSize+cellSize+cellSize,x.Height+cellSize+cellSize+cellSize)).Intersects(room))))
            //    {
            //        tries++;
            //    }
            //    else
            //    {
            //        rooms.Add(room);
            //    }
            //}

            while (tries < attempts)
            {
                DungeonRoom newRoom = new DungeonRoom();
                Rectangle room = new Rectangle();
                room.X = (int)Math.Ceiling((double)random.Next(0, levelHeight) / 40) * 40;
                room.Y = (int)Math.Ceiling((double)random.Next(0, levelWidth) / 40) * 40;
                room.Width = (int)Math.Ceiling((double)random.Next(minSize * cellSize, maxSize * cellSize) / 40) * 40;
                room.Height = (int)Math.Ceiling((double)random.Next(minSize * cellSize, maxSize * cellSize) / 40) * 40;
                if (dungeonRooms.Any( x => x.Room.Intersects(room)))
                {
                    tries++;
                }
                else
                {
                    newRoom.Room = room;
                    dungeonRooms.Add(newRoom);
                }
            }

            //Connect rooms with halls
            //Select a random part of a room, try making a wall up and down.  Try making a wall left and right.
            //Keep trying until a collision is found.
            dungeonRooms.ForEach(x =>
            {
            Rectangle closest = new Rectangle();
            DungeonRoom closestRoom = null;
            float distance = 0;
            dungeonRooms.ForEach(y =>
            {
                if (!x.Equals(y) && !y.Connected)
                {
                    Rectangle diff = Rectangle.Union(x.Room, y.Room);
                    if (closest.IsEmpty)
                    {
                        closest = diff;
                    }
                    float newDistance = Vector2.Distance(x.Room.Location.ToVector2(), y.Room.Location.ToVector2());
                    if ((int)distance == 0 || (int)newDistance < (int)distance)
                    {
                        closest = diff;
                        closestRoom = y;
                        distance = newDistance;
                    }
                }
            });
            //closestRoom.Connected = true;
            //Basic Hallway Algorithm  (NOT WORKING):
            //If the current room is above the closest room, then make a hallway going down from the current room.
            //----------If that hallway did not reach the closestroom, build a hallway going left or right to the closest room.
            //If the current room is below the closest room, then make a hallway going down from the closest room.
            //----------If that hallway did not reach the current room, build a hallway going left or right to the current room.
            //If the current room is left of the closest room, then make a hallway going right from the current room.
            //----------If that hallway did not reach the closest room, build a hallway going up or down to the closest room.
            //If the current room is to the right of the closest room, build a hallway going right from the closest room.
            //----------If that hallway did not reach the current room, build a hallway going up or down to the current room.
            //Check that a traversal of all "ConnectedRooms" lists hits every room in DungeonRooms.  Any room it did not hit, remove.
            if (closestRoom != null)
            {
                    List<Rectangle> corridor = new List<Rectangle>();
                    Rectangle vertHall = new Rectangle();
                    Rectangle longHall = new Rectangle();
                    if (x.Room.Bottom < closestRoom.Room.Top)
                    {
                        vertHall = new Rectangle(x.Room.Left, x.Room.Bottom, 80, Math.Abs(x.Room.Bottom - closestRoom.Room.Top) + 40);
                        if(!vertHall.Intersects(closestRoom.Room))
                        {
                            if(x.Room.X < closestRoom.Room.X)
                            {
                                longHall = new Rectangle(vertHall.Left, vertHall.Bottom, closestRoom.Room.Left - vertHall.Right + 80, 80);
                            }
                            else
                            {
                                longHall = new Rectangle(vertHall.Right, vertHall.Bottom, vertHall.Right - closestRoom.Room.Left + 80, 80);
                            }
                        }
                    }
                    else if (closestRoom.Room.Bottom < x.Room.Top)
                    {
                        vertHall = new Rectangle(closestRoom.Room.Left, closestRoom.Room.Bottom, 80, Math.Abs(closestRoom.Room.Bottom - x.Room.Top) + 40);
                        if (!vertHall.Intersects(x.Room))
                        {
                            if (closestRoom.Room.X < x.Room.X)
                            {
                                longHall = new Rectangle(closestRoom.Room.Left, vertHall.Bottom, x.Room.Left - vertHall.Right + 80, 80);
                            }
                            else
                            {
                                longHall = new Rectangle(closestRoom.Room.Right, vertHall.Bottom, vertHall.Right - x.Room.Left + 80, 80);
                            }
                        }
                    }
                    if (closestRoom.Room.Right < x.Room.Left)
                    {
                        longHall = new Rectangle(closestRoom.Room.Right, closestRoom.Room.Top, Math.Abs(closestRoom.Room.Right - x.Room.Left) + 40, 80);
                        if (!longHall.Intersects(closestRoom.Room))
                        {
                            if(closestRoom.Room.Y < x.Room.Y)
                            {
                                vertHall = new Rectangle(longHall.Right, longHall.Top, 80, x.Room.Top - longHall.Top);
                            }
                            else
                            {
                                vertHall = new Rectangle(closestRoom.Room.Right, longHall.Top, 80, longHall.Top - closestRoom.Room.Bottom);
                            }
                        }
                    }
                    else if (x.Room.Right < closestRoom.Room.Left)
                    {
                        longHall = new Rectangle(x.Room.Right, x.Room.Top, Math.Abs(x.Room.Right - closestRoom.Room.Left) + 40, 80);
                    }

                    if (!vertHall.IsEmpty)
                    {
                        halls.Add(vertHall);
                    }
                    if (!longHall.IsEmpty)
                    {
                        halls.Add(longHall);
                    }
                    closestRoom.Connected = true;
                }
                #region deprecatedcomments
                //var lengthAttempt = 0;
                //Rectangle longHall;
                //Rectangle highHall;
                //bool add = true;
                //do
                //{
                //    lengthAttempt++;
                //    longHall = new Rectangle((int)Math.Ceiling((double)x.Center.X / 40) * 40, (int)Math.Ceiling((double)x.Center.Y / 40) * 40, 80, (40 * lengthAttempt));
                //    highHall = new Rectangle((int)Math.Ceiling((double)x.Center.X / 40) * 40, (int)Math.Ceiling((double)x.Center.Y / 40) * 40, (40 * lengthAttempt), 80);
                //    if (lengthAttempt > levelSize * 40)
                //    {
                //        add = false;
                //        break;
                //    }
                //} while (rooms.Where(y => longHall.Intersects(y)).Count() < 2 && rooms.Where(y => highHall.Intersects(y)).Count() < 2);
                //if (add)
                //{
                //    halls.Add(longHall);
                //    halls.Add(highHall);
                //}
                //    var lengthAttempt = 0;
                //    Rectangle longHall;
                //    Rectangle highHall;
                //    bool add = true;
                //    do
                //    {
                //        lengthAttempt++;
                //        longHall = new Rectangle((int)Math.Ceiling((double)x.Center.X / 40) * 40, (int)Math.Ceiling((double)x.Center.Y / 40) * 40, 80, (40 * lengthAttempt));
                //        if (lengthAttempt > levelSize * 400)
                //        {
                //            add = false;
                //            break;
                //        }
                //    } while ((rooms.Where(y => longHall.Intersects(y)).Count() < 2 && halls.Where(y => longHall.Intersects(y)).Count() < 2));
                //    if (add)
                //    {
                //        halls.Add(longHall);
                //    }

                //    do
                //    {
                //        lengthAttempt++;
                //        highHall = new Rectangle((int)Math.Ceiling((double)x.Center.X / 40) * 40, (int)Math.Ceiling((double)x.Center.Y / 40) * 40, (40 * lengthAttempt), 80);
                //        if (lengthAttempt > levelSize * 400)
                //        {
                //            add = false;
                //            break;
                //        }
                //    } while (rooms.Where(y => highHall.Intersects(y)).Count() < 2 && halls.Where(y => highHall.Intersects(y)).Count() < 2);
                //    if (add)
                //    {
                //        halls.Add(highHall);
                //    }
                #endregion
            });

        }
        
        private void DrawDungeon(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            dungeonRooms.ForEach(x =>
            {
                spriteBatch.Draw(floor, x.Room, Color.LemonChiffon);
            });
            halls.ForEach(x =>
            {
                spriteBatch.Draw(floor, x, Color.LemonChiffon);
            });
            highHalls.ForEach(x =>
            {
                spriteBatch.Draw(floor, x, Color.ForestGreen * 0.3f);
            });
            slideHalls.ForEach(x =>
            {
                spriteBatch.Draw(floor, x, Color.MediumPurple * 0.3f);
            });
        }

        private void Movement(GraphicsDeviceManager graphics, GameTime gameTime, Camera camera, KeyboardState prevKey)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Entity entity in Entities)
            {
                //if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                //{
                //    KeyboardState keyState = Keyboard.GetState();
                //    if (keyState.IsKeyDown(Keys.W))
                //    {
                //        Position pos = PositionComponents[entity.Id];
                //        pos.Pos.Y -= VelocityComponents[entity.Id].y * delta;
                //            PositionComponents[entity.Id] = pos;
                //    }
                //    if (keyState.IsKeyDown(Keys.S))
                //    {
                //        Position pos = PositionComponents[entity.Id];
                //        pos.Pos.Y += VelocityComponents[entity.Id].y * delta;
                //            PositionComponents[entity.Id] = pos;
                //    }
                //    if (keyState.IsKeyDown(Keys.D))
                //    {
                //        Position pos = PositionComponents[entity.Id];
                //        pos.Pos.X += VelocityComponents[entity.Id].x * delta;
                //            PositionComponents[entity.Id] = pos;
                //    }
                //    if (keyState.IsKeyDown(Keys.A))
                //    {
                //        Position pos = PositionComponents[entity.Id];
                //        pos.Pos.X -= VelocityComponents[entity.Id].x * delta;
                //            PositionComponents[entity.Id] = pos;
                //    }
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
                    if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                    {
                        KeyboardState keyState = Keyboard.GetState();
                        if (keyState.IsKeyDown(Keys.W))
                        {
                            Position pos = PositionComponents[entity.Id];
                            pos.Pos.Y -= 40;
                            if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                                PositionComponents[entity.Id] = pos;
                        }
                        if (keyState.IsKeyDown(Keys.S))
                        {
                            Position pos = PositionComponents[entity.Id];
                            pos.Pos.Y += 40;
                            if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                                PositionComponents[entity.Id] = pos;
                        }
                        if (keyState.IsKeyDown(Keys.D))
                        {
                            Position pos = PositionComponents[entity.Id];
                            pos.Pos.X += 40;
                            if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                                PositionComponents[entity.Id] = pos;
                        }
                        if (keyState.IsKeyDown(Keys.A))
                        {
                            Position pos = PositionComponents[entity.Id];
                            pos.Pos.X -= 40;
                            if (dungeonRooms.Any(x => x.Room.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                                PositionComponents[entity.Id] = pos;
                        }
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
