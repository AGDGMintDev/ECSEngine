using EmnityDX.Engine;
using EmnityDX.Objects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Objects.States
{
    public class TestState : State
    {
        public override void LoadContent()
        {
            base.LoadContent();
            CreateTestPlayer();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void UpdateContent(GameTime gametime, ref GraphicsDeviceManager graphics)
        {
            EntityManager.Instance.UpdateContent(gametime, ref graphics);
        }

        public override void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {
            EntityManager.Instance.DrawContent(spriteBatch, ref graphics);
        }

        private void CreateTestPlayer()
        {
            Guid playerId = EntityManager.Instance.CreateEntity();
            EntityManager.Instance.Entities.Where(x => x.Id == playerId).Single().ComponentFlags = 
                Component.COMPONENT_SPRITE | Component.COMPONENT_VELOCITY | Component.COMPONENT_POSITION | Component.COMPONENT_LABEL | Component.COMPONENT_HEALTH;
            EntityManager.Instance.SpriteComponents[playerId] = new Sprite() {  SpritePath = _content.Load<Texture2D>("Sprites/Ball") };
            EntityManager.Instance.VelocityComponents[playerId] = new Velocity() { x = 5, y = 5 };
            EntityManager.Instance.PositionComponents[playerId] = new Position() { Pos = new Vector2(StateManager.Instance.Dimensions.X / 2, StateManager.Instance.Dimensions.Y / 2) };
            EntityManager.Instance.LabelComponents[playerId] = new Label() { NameLabel = "Player One" };
            EntityManager.Instance.HealthComponents[playerId] = new Health() { CurrentHealth = 50, MaxHealth = 100, MinHealth = 0 };
        }
    }
}
