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
    public class State
    {
        protected ContentManager _content;
        public List<Entity> Entities { get; set; }
        public Dictionary<Guid, Health> HealthComponents { get; set; }
        public Dictionary<Guid, Label> LabelComponents { get; set; }
        public Dictionary<Guid, Position> PositionComponents { get; set; }
        public Dictionary<Guid, Velocity> VelocityComponents { get; set; }
        public Dictionary<Guid, Sprite> SpriteComponents { get; set; }
        protected KeyboardState PrevKeybaordState { get; set; }

        ~State()
        {
            Entities.Clear();
            Entities.Clear();
            HealthComponents.Clear();
            LabelComponents.Clear();
            PositionComponents.Clear();
            VelocityComponents.Clear();
            SpriteComponents.Clear();
            _content.Unload();
        }

        public virtual void LoadContent(ContentManager Content)
        {

            Entities = new List<Entity>();
            HealthComponents = new Dictionary<Guid, Health>();
            LabelComponents = new Dictionary<Guid, Label>();
            PositionComponents = new Dictionary<Guid, Position>();
            VelocityComponents = new Dictionary<Guid, Velocity>();
            SpriteComponents = new Dictionary<Guid, Sprite>();
            _content = new ContentManager(Content.ServiceProvider, "Content");
        }

        public virtual void UnloadContent()
        {
            if (_content != null) { _content.Unload(); }
        }

        public virtual State UpdateContent(GameTime gametime, ref GraphicsDeviceManager graphics)
        {
            return this;
        }

        public virtual void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {

        }

        public Guid CreateEntity()
        {
            Entity newEntity = new Entity();
            Entities.Add(newEntity);
            return newEntity.Id;
        }

        public void DestroyEntity(Guid id)
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
