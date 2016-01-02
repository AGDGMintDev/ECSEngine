using EmnityDX.Objects.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine
{
    [Flags]
    public enum Component : ulong
    {
        NONE = 0,
        COMPONENT_HEALTH = 1 << 0,
        COMPONENT_POSITION = 1 << 1,
        COMPONENT_VELOCITY = 1 << 2,
        COMPONENT_LABEL = 1 << 3,
        COMPONENT_SPRITE = 1 << 4,
        COMPONENT_ISPLAYER = 1 << 5,
        COMPONENT_ISCOLLIDABLE = 1 << 6,
        COMPONENT_STATISTICS = 1 << 7
    }

    public struct ComponentMasks
    {
        public const Component MOVEMENT = Component.COMPONENT_POSITION | Component.COMPONENT_VELOCITY;
        public const Component PLAYER_MOVEMENT = MOVEMENT | Component.COMPONENT_ISPLAYER;
        public const Component DRAWABLE = Component.COMPONENT_POSITION | Component.COMPONENT_SPRITE;
        public const Component DRAWABLE_LABEL = Component.COMPONENT_POSITION | Component.COMPONENT_LABEL;
        public const Component DRAWABLE_HEALTH = Component.COMPONENT_POSITION | Component.COMPONENT_HEALTH | Component.COMPONENT_LABEL;
        public const Component DRAWABLE_ITEM = Component.COMPONENT_SPRITE | Component.COMPONENT_POSITION | Component.COMPONENT_ISCOLLIDABLE | Component.COMPONENT_STATISTICS | Component.COMPONENT_LABEL;
    }

    public class LevelComponents
    {
        public List<Entity> Entities { get; private set; }
        public Dictionary<Guid, Health> HealthComponents { get; private set; }
        public Dictionary<Guid, Label> LabelComponents { get; private set; }
        public Dictionary<Guid, Position> PositionComponents { get; private set; }
        public Dictionary<Guid, Velocity> VelocityComponents { get; private set; }
        public Dictionary<Guid, Statistics> StatisticsComponents { get; private set; }
        public Dictionary<Guid, Sprite> SpriteComponents { get; private set; }
        public List<Guid> EntitiesToDelete { get; private set; }

        public LevelComponents()
        {
            Entities = new List<Entity>();
            HealthComponents = new Dictionary<Guid, Health>();
            LabelComponents = new Dictionary<Guid, Label>();
            PositionComponents = new Dictionary<Guid, Position>();
            VelocityComponents = new Dictionary<Guid, Velocity>();
            SpriteComponents = new Dictionary<Guid, Sprite>();
            StatisticsComponents = new Dictionary<Guid, Statistics>();
            EntitiesToDelete = new List<Guid>();
        }

        ~LevelComponents()
        {
            Entities.Clear();
            Entities.Clear();
            HealthComponents.Clear();
            LabelComponents.Clear();
            PositionComponents.Clear();
            VelocityComponents.Clear();
            SpriteComponents.Clear();
            StatisticsComponents.Clear();
            EntitiesToDelete.Clear();
        }

        public Guid CreateEntity()
        {
            Entity newEntity = new Entity();
            this.Entities.Add(newEntity);
            return newEntity.Id;
        }

        public void DestroyEntity(Guid id)
        {
            Entity removal = this.Entities.Where(x => x.Id == id).FirstOrDefault();
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
