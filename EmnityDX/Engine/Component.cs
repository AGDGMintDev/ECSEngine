﻿using System;
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
        COMPONENT_ISCOLLIDABLE = 1 << 6
    }

    public struct ComponentMasks
    {
        public const Component MOVEMENT = Component.COMPONENT_POSITION | Component.COMPONENT_VELOCITY;
        public const Component PLAYER_MOVEMENT = MOVEMENT | Component.COMPONENT_ISPLAYER;
        public const Component DRAWABLE = Component.COMPONENT_POSITION | Component.COMPONENT_SPRITE;
        public const Component DRAWABLE_LABEL = Component.COMPONENT_POSITION | Component.COMPONENT_LABEL;
        public const Component DRAWABLE_HEALTH = Component.COMPONENT_POSITION | Component.COMPONENT_HEALTH | Component.COMPONENT_LABEL;
    }
}
