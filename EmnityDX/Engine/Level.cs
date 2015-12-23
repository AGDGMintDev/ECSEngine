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

namespace EmnityDX.Engine
{
    public class Level
    {
        public List<Entity> Entities { get; private set; }
        public Dictionary<Guid, Health> HealthComponents { get; private set; }
        public Dictionary<Guid, Label> LabelComponents { get; private set; }
        public Dictionary<Guid, Position> PositionComponents { get; private set; }
        public Dictionary<Guid, Velocity> VelocityComponents { get; private set; }
        public Dictionary<Guid, Sprite> SpriteComponents { get; private set; }


        public Level()
        {
            Entities = new List<Entity>();
            HealthComponents = new Dictionary<Guid, Health>();
            LabelComponents = new Dictionary<Guid, Label>();
            PositionComponents = new Dictionary<Guid, Position>();
            VelocityComponents = new Dictionary<Guid, Velocity>();
            SpriteComponents = new Dictionary<Guid, Sprite>();
    }

        ~Level()
        {
            Entities.Clear();
            Entities.Clear();
            HealthComponents.Clear();
            LabelComponents.Clear();
            PositionComponents.Clear();
            VelocityComponents.Clear();
            SpriteComponents.Clear();
        }

        public virtual void LoadLevel(ContentManager content, GraphicsDeviceManager graphics)
        {

        }

        public virtual Level UpdateLevel(GameTime gameTime, ContentManager content, GraphicsDeviceManager graphics, KeyboardState prevKeyboardState, MouseState prevMouseState, GamePadState prevGamepadState)
        {
            return this;
        }

        public virtual void DrawLevel(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {

        }

        protected Guid CreateEntity()
        {
            Entity newEntity = new Entity();
            Entities.Add(newEntity);
            return newEntity.Id;
        }

        protected void DestroyEntity(Guid id)
        {
            Entity removal = Entities.Where(x => x.Id == id).FirstOrDefault();
            if (removal != null)
            {
                Entities.Remove(removal);
                HealthComponents.Remove(id);
                LabelComponents.Remove(id);
                PositionComponents.Remove(id);
                VelocityComponents.Remove(id);
                SpriteComponents.Remove(id);
            }
        }
    }
}
