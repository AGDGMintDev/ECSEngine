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

namespace EmnityDX.Objects.States
{
    public class TestState : State
    {

        public TestState(ContentManager Content, ref GraphicsDeviceManager graphics)
        {
            base.LoadContent(Content);
            CreateTestPlayer(ref graphics);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override State UpdateContent(GameTime gametime, ref GraphicsDeviceManager graphics)
        {
            State nextState = this;
            PlayerMovement(ref graphics);
            if(Keyboard.GetState().IsKeyDown(Keys.Enter) && !PrevKeybaordState.IsKeyDown(Keys.Enter))
            {
                nextState = new PauseState(_content, ref graphics, Keyboard.GetState());
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                return null;

            PrevKeybaordState = Keyboard.GetState();
            return nextState;
        }

        public override void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {
            DrawEntities(spriteBatch);
            DrawLabels(spriteBatch);
        }

        private void CreateTestPlayer(ref GraphicsDeviceManager graphics)
        {
            Guid playerId = CreateEntity();
            Entities.Where(x => x.Id == playerId).Single().ComponentFlags = 
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_LABEL | Component.COMPONENT_HEALTH;
            SpriteComponents[playerId] = new Sprite() {  SpritePath = _content.Load<Texture2D>("Sprites/Ball") };
            VelocityComponents[playerId] = new Velocity() { x = 5, y = 5 };
            PositionComponents[playerId] = new Position() { Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) };
            LabelComponents[playerId] = new Label() { Text = "Player Label", SpriteFont = _content.Load<SpriteFont>("SpriteFonts/CaviarDreamsBold12"), Color = Color.Wheat };
            HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }

        

        private void PlayerMovement(ref GraphicsDeviceManager graphics)
        {
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & ComponentMasks.MOVEMENT) == ComponentMasks.MOVEMENT)
                {
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.W) && PositionComponents[entity.Id].Pos.Y > 0)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y -= VelocityComponents[entity.Id].y;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.S) && PositionComponents[entity.Id].Pos.Y < graphics.GraphicsDevice.Viewport.Height - SpriteComponents[entity.Id].SpritePath.Bounds.Height)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.Y += VelocityComponents[entity.Id].y;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.D) && PositionComponents[entity.Id].Pos.X < graphics.GraphicsDevice.Viewport.Width - SpriteComponents[entity.Id].SpritePath.Bounds.Width)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X += VelocityComponents[entity.Id].x;
                        PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.A) && PositionComponents[entity.Id].Pos.X > 0)
                    {
                        Position pos = PositionComponents[entity.Id];
                        pos.Pos.X -= VelocityComponents[entity.Id].x;
                        PositionComponents[entity.Id] = pos;
                    }
                }
            }
        }

        private void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in Entities)
            {
                if ((entity.ComponentFlags & ComponentMasks.DRAWABLE) == ComponentMasks.DRAWABLE)
                {
                    spriteBatch.Draw(SpriteComponents[entity.Id].SpritePath, PositionComponents[entity.Id].Pos);
                }
            }
        }

        private void DrawLabels(SpriteBatch spriteBatch)
        {
            Entities.Where(x => (x.ComponentFlags & ComponentMasks.DRAWABLE_LABEL) == ComponentMasks.DRAWABLE_LABEL).ToList().ForEach(y =>
                spriteBatch.DrawString(LabelComponents[y.Id].SpriteFont, LabelComponents[y.Id].Text, new Vector2(PositionComponents[y.Id].Pos.X, PositionComponents[y.Id].Pos.Y - 30), LabelComponents[y.Id].Color));

            Entities.Where(x => (x.ComponentFlags & ComponentMasks.DRAWABLE_HEALTH) == ComponentMasks.DRAWABLE_HEALTH).ToList().ForEach(y =>
                spriteBatch.DrawString(LabelComponents[y.Id].SpriteFont, 
                    "Max Health: " + HealthComponents[y.Id].MaxHealth + "\nCurrentHealth: " + HealthComponents[y.Id].CurrentHealth, 
                    new Vector2(PositionComponents[y.Id].Pos.X, 
                    PositionComponents[y.Id].Pos.Y + SpriteComponents[y.Id].SpritePath.Height + 10), Color.Red
                ));
        }
    }
}
