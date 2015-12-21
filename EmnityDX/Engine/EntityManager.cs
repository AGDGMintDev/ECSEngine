using EmnityDX.Objects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine
{
    public class EntityManager
    {
        private static EntityManager _instance;
        public List<Entity> Entities { get; set; }
        public Dictionary<Guid, Health> HealthComponents { get; set; }
        public Dictionary<Guid, Label> LabelComponents { get; set; }
        public Dictionary<Guid, Position> PositionComponents { get; set; }
        public Dictionary<Guid, Velocity> VelocityComponents { get; set; }
        public Dictionary<Guid, Sprite> SpriteComponents { get; set; }

        public static EntityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EntityManager();
                }
                return _instance;
            }
        }

        public EntityManager()
        {
            Entities = new List<Entity>();
            HealthComponents = new Dictionary<Guid, Health>();
            LabelComponents = new Dictionary<Guid, Label>();
            PositionComponents = new Dictionary<Guid, Position>();
            VelocityComponents = new Dictionary<Guid, Velocity>();
            SpriteComponents = new Dictionary<Guid, Sprite>();
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
            if(removal != null)
            {
                Entities.Remove(removal);
                HealthComponents.Remove(id);
                LabelComponents.Remove(id);
                PositionComponents.Remove(id);
                VelocityComponents.Remove(id);
                SpriteComponents.Remove(id);
            }
        }

        public void DestroyEntities()
        {
            Entities.Clear();
            Entities.Clear();
            HealthComponents.Clear();
            LabelComponents.Clear();
            PositionComponents.Clear();
            VelocityComponents.Clear();
            SpriteComponents.Clear();
        }

        public void UpdateContent(GameTime gametime, ref GraphicsDeviceManager graphics)
        {
            PlayerMovement();
        }

        public void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {
            DrawEntities(spriteBatch);
        }


        //HANDLING OF COMPONENT MASKS
        private void PlayerMovement()
        {
            foreach(Entity entity in Entities)
            {
                if((entity.ComponentFlags & ComponentMasks.MOVEMENT) == ComponentMasks.MOVEMENT)
                {
                    KeyboardState keyState = Keyboard.GetState();
                    if (keyState.IsKeyDown(Keys.W) && Instance.PositionComponents[entity.Id].Pos.Y > 0)
                    {
                        Position pos = Instance.PositionComponents[entity.Id];
                        pos.Pos.Y -= Instance.VelocityComponents[entity.Id].y;
                        Instance.PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.S) && Instance.PositionComponents[entity.Id].Pos.Y < StateManager.Instance.Dimensions.Y - Instance.SpriteComponents[entity.Id].SpritePath.Bounds.Height)
                    {
                        Position pos = Instance.PositionComponents[entity.Id];
                        pos.Pos.Y += Instance.VelocityComponents[entity.Id].y;
                        Instance.PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.D) && Instance.PositionComponents[entity.Id].Pos.X < StateManager.Instance.Dimensions.X - Instance.SpriteComponents[entity.Id].SpritePath.Bounds.Width)
                    {
                        Position pos = Instance.PositionComponents[entity.Id];
                        pos.Pos.X += Instance.VelocityComponents[entity.Id].x;
                        Instance.PositionComponents[entity.Id] = pos;
                    }
                    if (keyState.IsKeyDown(Keys.A) && Instance.PositionComponents[entity.Id].Pos.X > 0)
                    {
                        Position pos = Instance.PositionComponents[entity.Id];
                        pos.Pos.X -= Instance.VelocityComponents[entity.Id].x;
                        Instance.PositionComponents[entity.Id] = pos;
                    }
                }
            }
        }

        private void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach(Entity entity in Entities)
            {
                if((entity.ComponentFlags & ComponentMasks.DRAWABLE) == ComponentMasks.DRAWABLE)
                {
                    spriteBatch.Draw(Instance.SpriteComponents[entity.Id].SpritePath, Instance.PositionComponents[entity.Id].Pos);
                }
            }
        }


    }
}
