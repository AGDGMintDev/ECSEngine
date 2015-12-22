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
    public class PauseState : State
    {
        public PauseState(ContentManager Content, ref GraphicsDeviceManager graphics, KeyboardState lastState = new KeyboardState())
        {
            base.LoadContent(Content);
            CreatePauseText(ref graphics);
            PrevKeybaordState = lastState;
        }

        public override State UpdateContent(GameTime gametime, ref GraphicsDeviceManager graphics)
        {
            State nextState = this;

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !PrevKeybaordState.IsKeyDown(Keys.Enter))
                return null;


            PrevKeybaordState = Keyboard.GetState();
            return nextState;
        }

        public override void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {
            DrawLabels(spriteBatch);
        }

        private void CreatePauseText(ref GraphicsDeviceManager graphics)
        {
            Guid id = CreateEntity();
            Entities.Where(x => x.Id == id).Single().ComponentFlags = Component.COMPONENT_POSITION | Component.COMPONENT_LABEL;
            PositionComponents[id] = new Position() { Pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) };
            LabelComponents[id] = new Label() { Text = "[GAME PAUSED]", SpriteFont = _content.Load<SpriteFont>("SpriteFonts/CaviarDreamsBold12"), Color = Color.White };
        }

        private void DrawLabels(SpriteBatch spriteBatch)
        {
            Entities.Where(x => (x.ComponentFlags & ComponentMasks.DRAWABLE_LABEL) == ComponentMasks.DRAWABLE_LABEL).ToList().ForEach(y =>
                spriteBatch.DrawString(LabelComponents[y.Id].SpriteFont, LabelComponents[y.Id].Text, new Vector2(PositionComponents[y.Id].Pos.X, PositionComponents[y.Id].Pos.Y - 30), LabelComponents[y.Id].Color));
        }
    }
}
