using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine.ProceduralGeneration
{
    [Flags]
    public enum DungeonTiles : ulong
    {
        NONE = 0,
        UNVISITED = 1 << 0,
        VISITED = 1 << 1,
        WALL = 1 << 2,
        ROCK = 1 << 3,
        FLOOR = 1 << 4,
        DOOR = 1 << 5,
        SHOP_FLOOR = 1 << 6,
        FOUND = 1 << 7,
        IN_RANGE = 1 << 8,
        BLOCKED_RANGE = 1 << 9,
        NEWLY_FOUND = 1 << 10,
    }
}
