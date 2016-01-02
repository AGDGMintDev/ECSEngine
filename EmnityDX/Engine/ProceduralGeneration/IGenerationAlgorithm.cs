using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine.ProceduralGeneration
{
    public interface IGenerationAlgorithm
    {
        Vector2 GenerateDungeon(ref DungeonTiles[,] dungeonGrid, int worldMin, int worldMax, Random random);
    }
}
