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
        UNVISITED = 1 << 0,
        VISITED = 1 << 1,
        WALL = 1 << 2,
        ROCK = 1 << 3,
        FLOOR = 1 << 4,
        DOOR = 1 << 5,


    }
}
