using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine.ProceduralGeneration
{
    [Flags]
    public enum DungeonTiles
    {
        NONE = 0,
        FLOOR = 1 << 0,
        DOOR = 1 << 1,
        UNVISITED = 1 << 2,
        VISITED = 1 << 3,
        WALL = 1 << 4,
        FIRST_ROOM = 1 << 5,
        ROCK = 1 << 6,
        FOUND = 1 << 7
    }
}
