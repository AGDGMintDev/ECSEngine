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

        public override void LoadLevel(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            CreateTestPlayer(content, graphics);
        }

        public override Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState, Camera camera)
        {
            Level nextLevel = this;
            Movement(graphics, gameTime, camera);
            CheckCollision();
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                ShootBullet(content, graphics, camera);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && prevMouseState.RightButton != ButtonState.Pressed)
            {
                CreateItem(content, graphics, camera);
            }
            base.UpdateLevel(gameTime, content, graphics, prevKeyboardState, prevMouseState, prevGamepadState, camera);
            return nextLevel;
        }

        public override void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Camera camera)
        {
            DrawEntities(spriteBatch, camera);
            DrawLabels(spriteBatch);
        }

        private void ShootBullet(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            bool stuff = camera.Bounds.Contains(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y));
            if (stuff)
            {
                Random random = new Random();
                Guid id = CreateEntity();
                Entities.Where(x => x.Id == id).Single().ComponentFlags =
                    Component.COMPONENT_SPRITE | Component.COMPONENT_POSITION | Component.COMPONENT_ISCOLLIDABLE;
                SpriteComponents[id] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball"), Color = Color.ForestGreen };
                PositionComponents[id] = new Position() { Pos = Vector2.Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y), camera.GetInverseMatrix()) };
            }
        }

        public void CreateItem(ContentManager content, GraphicsDeviceManager graphics, Camera camera)
        {
            bool stuff = camera.Bounds.Contains(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y));
            if (stuff)
            {
                Random random = new Random();
                double rarity = random.Next(10, 100);
                Guid id = CreateEntity();
                Entities.Where(x => x.Id == id).Single().ComponentFlags =
                    ComponentMasks.DRAWABLE_ITEM;
                LabelComponents[id] = new Label() { Color = Color.White, Text = "Dropped Item!", SpriteFont = content.Load<SpriteFont>("SpriteFonts/CaviarDreamsBold12") };
                StatisticsComponents[id] = new Statistics()
                {
                    FlavorText = "Wow!  I can't believe this just dropped!",
                    Rarity = rarity,
                    MentalDamage = random.Next(100) + rarity,
                    PhysicalDamage = random.Next(100) + rarity,
                    Value = rarity * 100,
                    SpecialEffects = "Every third kill with this weapon causes health to replenish."
                };
                SpriteComponents[id] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball"), Color = Color.Gold };
                PositionComponents[id] = new Position() { Pos = Vector2.Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y), camera.GetInverseMatrix()) };
            }
        }

        private void CreateTestPlayer(ContentManager content, GraphicsDeviceManager graphics)
        {
            Guid playerId = CreateEntity();
            Entities.Where(x => x.Id == playerId).Single().ComponentFlags =
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_LABEL | Component.COMPONENT_HEALTH | Component.COMPONENT_ISPLAYER | Component.COMPONENT_ISCOLLIDABLE;
            SpriteComponents[playerId] = new Sprite() { SpritePath = content.Load<Texture2D>("Sprites/Ball"), Color = Color.Red };
            VelocityComponents[playerId] = new Velocity() { x = 350, y = 350 };
            PositionComponents[playerId] = new Position() { Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) };
            LabelComponents[playerId] = new Label() { Text = "Player Label", SpriteFont = content.Load<SpriteFont>("SpriteFonts/CaviarDreamsBold12"), Color = Color.Wheat };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }

        private void CheckCollision()
        {
            Entities.Where(x => (x.ComponentFlags & (Component.COMPONENT_ISCOLLIDABLE | Component.COMPONENT_ISPLAYER)) == (Component.COMPONENT_ISCOLLIDABLE | Component.COMPONENT_ISPLAYER)).ToList().ForEach(y =>
            {
                Entities.Where(c => (c.ComponentFlags & Component.COMPONENT_ISCOLLIDABLE) == Component.COMPONENT_ISCOLLIDABLE && (c.ComponentFlags & Component.COMPONENT_ISPLAYER) != Component.COMPONENT_ISPLAYER).ToList().ForEach(z =>
                {
                    if (!Rectangle.Intersect(new Rectangle(PositionComponents[y.Id].Pos.ToPoint(), new Point(SpriteComponents[y.Id].SpritePath.Width, SpriteComponents[y.Id].SpritePath.Height)),
                        new Rectangle(PositionComponents[z.Id].Pos.ToPoint(), new Point(SpriteComponents[z.Id].SpritePath.Width, SpriteComponents[z.Id].SpritePath.Height))).IsEmpty)
                    {
                        Sprite sprite = SpriteComponents[z.Id];
                        sprite.Color = Color.PaleVioletRed;
                        SpriteComponents[z.Id] = sprite;
                    }
                });
            });
        }

        private void Movement(GraphicsDeviceManager graphics, GameTime gameTime, Camera camera)
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
                    if (keyState.IsKeyDown(Keys.A) && PositionComponents[entity.Id].Pos.X > 0)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= VelocityComponents[entity.Id].x * delta;
                        PositionComponents[entity.Id] = pos;
                    }

                    camera.Position = new Vector2((int)PositionComponents[entity.Id].Pos.X + SpriteComponents[entity.Id].SpritePath.Bounds.Center.X, (int)PositionComponents[entity.Id].Pos.Y + SpriteComponents[entity.Id].SpritePath.Bounds.Center.Y);
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

        private void DrawEntities(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER)) == (ComponentMasks.DRAWABLE | Component.COMPONENT_ISPLAYER))
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, new Vector2((int)PositionComponents[entity.Id].Pos.X, (int)PositionComponents[entity.Id].Pos.Y), SpriteComponents[entity.Id].Color);
                }
                else if ((entity.ComponentFlags & ComponentMasks.DRAWABLE) == ComponentMasks.DRAWABLE)
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, new Vector2((int)PositionComponents[entity.Id].Pos.X, (int)PositionComponents[entity.Id].Pos.Y), SpriteComponents[entity.Id].Color);
                }
                if (((entity.ComponentFlags & ComponentMasks.DRAWABLE_ITEM) == ComponentMasks.DRAWABLE_ITEM) &&
                    (new Rectangle(PositionComponents[entity.Id].Pos.ToPoint(), new Point(SpriteComponents[entity.Id].SpritePath.Width, SpriteComponents[entity.Id].SpritePath.Height)))
                    .Contains((Vector2.Transform(new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y), camera.GetInverseMatrix())).ToPoint()))
                {
                    var stats = StatisticsComponents[entity.Id];
                    spriteBatch.DrawString(LabelComponents[entity.Id].SpriteFont,
                    "Rarity: " + stats.Rarity + "\n"  +
                    "Mental Damage: " + stats.MentalDamage + "\n" +
                    "Physical Damage: " + stats.PhysicalDamage + "\n" + 
                    "Value: " + stats.Value + " $$\n" + 
                    "Special Effects: " + stats.SpecialEffects + "\n" + 
                    "\n\n" +
                    stats.FlavorText, Vector2.Transform(new Vector2(Mouse.GetState().Position.X + 35, Mouse.GetState().Position.Y), camera.GetInverseMatrix()), Color.Gold
                    );
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
