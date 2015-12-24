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

namespace EmnityDX.Objects.LevelData.PlayingStateLevels
{
    public class TestLevel: Level
    {
        private List<Guid> _entitiesToDestroy;

        public override void LoadLevel(ContentManager content, GraphicsDeviceManager graphics)
        {
            CreateTestPlayer(content, graphics);
            _entitiesToDestroy = new List<Guid>();
        }

        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState)
        {
            Level nextLevel = this;
            Movement(graphics, gameTime);
            if(Keyboard.GetState().IsKeyDown(Keys.C))
            {
                ShootBullet(content, graphics);
            }
            _entitiesToDestroy.ForEach(x => DestroyEntity(x));
            _entitiesToDestroy.Clear();
            return nextLevel;
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            DrawEntities(spriteBatch);
            DrawLabels(spriteBatch);
        }

        private void ShootBullet(ContentManager content, GraphicsDeviceManager graphics)
        {
            Random random = new Random();
            Guid id = CreateEntity();
            Entities.Where(x => x.Id == id).Single().ComponentFlags =
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION;
            SpriteComponents[id] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball") };
            VelocityComponents[id] = new Velocity() { x = random.Next(-150,150), y = random.Next(-150, 150) };
            PositionComponents[id] = new Position() { Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2)};
        }

        private void CreateTestPlayer(ContentManager content, GraphicsDeviceManager graphics)
        {
            Guid playerId = CreateEntity();
            Entities.Where(x => x.Id == playerId).Single().ComponentFlags =
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_LABEL | Component.COMPONENT_HEALTH | Component.COMPONENT_ISPLAYER;
            SpriteComponents[playerId] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball") };
            VelocityComponents[playerId] = new Velocity() { x = 350, y = 350 };
            PositionComponents[playerId] = new Position() { Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) };
            LabelComponents[playerId] = new Label() { Text = "Player Label", SpriteFont = content.Load<SpriteFont>("SpriteFonts/CaviarDreamsBold12"), Color = Color.Wheat };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }

        private void Movement(GraphicsDeviceManager graphics, GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & ComponentMasks.PLAYER_MOVEMENT) == ComponentMasks.PLAYER_MOVEMENT)
                {
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.W) && PositionComponents[entity.Id].Pos.Y > 0)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y -= VelocityComponents[entity.Id].y * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.S) && PositionComponents[entity.Id].Pos.Y < graphics.GraphicsDevice.Viewport.Height - SpriteComponents[entity.Id].SpritePath.Bounds.Height)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y += VelocityComponents[entity.Id].y * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.D) && PositionComponents[entity.Id].Pos.X < graphics.GraphicsDevice.Viewport.Width - SpriteComponents[entity.Id].SpritePath.Bounds.Width)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X += VelocityComponents[entity.Id].x * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.A) && PositionComponents[entity.Id].Pos.X > 0)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= VelocityComponents[entity.Id].x * delta;
                        PositionComponents[entity.Id] = pos;
                    }
                }
                else if ((entity.ComponentFlags & ComponentMasks.MOVEMENT) == ComponentMasks.MOVEMENT)
                {
                    Position pos = PositionComponents[entity.Id];
                    pos.Pos.Y += VelocityComponents[entity.Id].y * delta;
                    pos.Pos.X += VelocityComponents[entity.Id].x * delta;
                    if( pos.Pos.X > graphics.GraphicsDevice.Viewport.Width || pos.Pos.Y > graphics.GraphicsDevice.Viewport.Height || pos.Pos.X < 0 || pos.Pos.Y < 0)
                    {

                        pos.Pos.Y = graphics.GraphicsDevice.Viewport.Height - pos.Pos.Y;
                        pos.Pos.X = graphics.GraphicsDevice.Viewport.Width - pos.Pos.X;
                    }
                    PositionComponents[entity.Id] = pos;
                }
            }
        }

        private void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER)) == (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER))
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, new Vector2((int)PositionComponents[entity.Id].Pos.X, (int)PositionComponents[entity.Id].Pos.Y), Color.Crimson);
                }
                else if ((entity.ComponentFlags & ComponentMasks.DRAWABLE) == ComponentMasks.DRAWABLE)
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, new Vector2((int)PositionComponents[entity.Id].Pos.X, (int)PositionComponents[entity.Id].Pos.Y));
                }
            }
        }

        private void DrawLabels(SpriteBatch spriteBatch)
        {
            Entities.Where(x => (x.ComponentFlags & ComponentMasks.DRAWABLE_LABEL) == ComponentMasks.DRAWABLE_LABEL).ToList().ForEach(y =>
                spriteBatch.DrawString(LabelComponents[y.Id].SpriteFont, LabelComponents[y.Id].Text, 
                new Vector2((int)PositionComponents[y.Id].Pos.X, (int)PositionComponents[y.Id].Pos.Y - 30), 
                LabelComponents[y.Id].Color));

            Entities.Where(x => (x.ComponentFlags & ComponentMasks.DRAWABLE_HEALTH) == ComponentMasks.DRAWABLE_HEALTH).ToList().ForEach(y =>
            {
                spriteBatch.DrawString(LabelComponents[y.Id].SpriteFont,
                    "Max Health: " + HealthComponents[y.Id].MaxHealth + "\nCurrentHealth: " + HealthComponents[y.Id].CurrentHealth,
                    new Vector2((int)PositionComponents[y.Id].Pos.X,
                    (int)PositionComponents[y.Id].Pos.Y + SpriteComponents[y.Id].SpritePath.Height + 10), Color.Red
                );
            }
            );
        }






    }
}
