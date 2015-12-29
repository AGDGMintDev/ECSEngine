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

namespace EmnityDX.Objects.LevelData.PlayingStateLevels
{
    public class RandomGeneration: Level
    {

        private List<Rectangle> rooms;
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
            GenerateDungeon();
            CreatePlayer(content, graphics);
        }

        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            Level nextLevel = this;
            Movement(graphics,gameTime, camera, prevKeyboardState);
            base.UpdateLevel(gameTime, content, graphics, prevKeyboardState, prevMouseState, prevGamepadState, camera);
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
            PositionComponents[playerId] = new Position() { Pos = new Vector2((int)Math.Ceiling((double)rooms[0].Center.X / 40) * 40, (int)Math.Ceiling((double)rooms[0].Center.Y / 40) * 40) };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }

        private void GenerateDungeon()
        {
            Random random = new Random();
            var levelSize = 5000;
            var minSize = 10;
            var maxSize = 30;
            var cellSize = 40;
            var attempts = 1500;
            var tries = 0;

            while (tries < attempts)
            {
                Rectangle room = new Rectangle();
                room.X = (int)Math.Ceiling((double)random.Next(0, levelSize) / 40) * 40;
                room.Y = (int)Math.Ceiling((double)random.Next(0, levelSize) / 40) * 40;
                room.Width = (int)Math.Ceiling((double)random.Next(minSize * cellSize, maxSize * cellSize) / 40) * 40;
                room.Height = (int)Math.Ceiling((double)random.Next(minSize * cellSize, maxSize * cellSize) / 40) * 40;

                if (rooms.Any(x =>  ((new Rectangle(x.X,x.Y,x.Width+cellSize+cellSize+cellSize,x.Height+cellSize+cellSize+cellSize)).Intersects(room))))
                {
                    tries++;
                }
                else
                {
                    rooms.Add(room);
                }
            }

            //Connect rooms with halls
            //Select a random part of a room, try making a wall up and down.  Try making a wall left and right.
            //Keep trying until a collision is found.
            rooms.ForEach(x =>
            {
                Rectangle closest = new Rectangle();
                Rectangle closestRoom = new Rectangle();
                float distance = 0;
                rooms.ForEach(y =>
                {
                    if (!x.Equals(y))
                    {
                        Rectangle diff = Rectangle.Union(x, y);
                        if (closest.IsEmpty)
                        {
                            closest = diff;
                        }
                        float newDistance = Vector2.Distance(x.Location.ToVector2(), y.Location.ToVector2());
                        if ((int)distance == 0 || (int)newDistance < (int)distance)
                        {
                            if (!highHalls.Any(z => z.Equals(diff)) && !slideHalls.Any(u => u.Equals(diff)) && !halls.Any(i => i.Equals(diff)))
                            {
                                closest = diff;
                                closestRoom = y;
                                distance = newDistance;
                            }
                            else
                            {
                                System.Diagnostics.Debug.Write(closest);
                            }
                        }
                    }
                });
                //x and closestRoom are the two rooms that should be connected with a hallway.  Draw one between them somehow <---------------------------------------------------------
                if(x.Bottom <= closestRoom.Top || closestRoom.Bottom <= x.Top)
                {
                    highHalls.Add(closest);
                }
                else if (x.Left >= closestRoom.Right || closestRoom.Left >= x.Right)
                {
                    slideHalls.Add(closest);
                }
                else
                {
                    halls.Add(closest);
                }
                //halls.Add(closest);
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
            rooms.ForEach(x =>
            {
                spriteBatch.Draw(floor, x, Color.LemonChiffon);
            });
            halls.ForEach(x =>
            {
                spriteBatch.Draw(floor, x, Color.DodgerBlue * 0.3f);
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
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.S))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.Y += 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.D))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.X += 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
                    //            PositionComponents[entity.Id] = pos;
                    //    }
                    //    if (keyState.IsKeyDown(Keys.A))
                    //    {
                    //        Position pos = PositionComponents[entity.Id];
                    //        pos.Pos.X -= 40;
                    //        if (rooms.Any(x => x.Contains(pos.Pos)) || halls.Any(x => x.Contains(pos.Pos)))
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
