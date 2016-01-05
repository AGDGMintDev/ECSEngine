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
        OCT_1 = 1 << 9,  //TEST
        OCT_2 = 1 << 10,  //TEST
        OCT_3 = 1 << 11,  //TEST
        OCT_4 = 1 << 12,  //TEST
        OCT_5 = 1 << 13,  //TEST
        OCT_6 = 1 << 14,  //TEST
        OCT_7 = 1 << 15,  //TEST
        OCT_8 = 1 << 16,  //TEST
        BLOCKED_RANGE = 1 << 17
    }
}
