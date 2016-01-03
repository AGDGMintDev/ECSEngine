using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EmnityDX.Engine.ProceduralGeneration
{
    public class CaveArenaGeneration : IGenerationAlgorithm
    {
        public Vector2 GenerateDungeon(ref DungeonTiles[,] dungeonGrid, int worldMin, int worldMax, Random random)
        {
            int worldI = random.Next(worldMin, worldMax);
            int worldJ = random.Next(worldMin, worldMax);
            int maxFloor = (int)((worldI * worldJ) * .40);

            dungeonGrid = new DungeonTiles[worldI, worldJ];

            bool acceptable = false;

            while(!acceptable)
            {
                //Random Walk
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        dungeonGrid[i, j] = DungeonTiles.ROCK;
                    }
                }

                int randomCellX = random.Next(0, worldI);
                int randomCellY = random.Next(0, worldJ);
                dungeonGrid[randomCellX, randomCellY] = DungeonTiles.FLOOR | DungeonTiles.VISITED;
                int floorCount = 1;
                while (floorCount < maxFloor)
                {
                    int direction = random.Next(0, 8);
                    switch(direction)
                    {
                        case 0: //top left
                            randomCellX -= 1;
                            randomCellY -= 1;
                            break;
                        case 1: //top
                            randomCellY -= 1;
                            break;
                        case 2: //top right
                            randomCellX += 1;
                            randomCellY -= 1;
                            break;
                        case 3: //Left
                            randomCellX -= 1;
                            break;
                        case 4: //Right
                            randomCellX += 1;
                            break;
                        case 5: //bottom left
                            randomCellX -= 1;
                            randomCellY += 1;
                            break;
                        case 6: //bottom
                            randomCellY += 1;
                            break;
                        case 7: //bottom right
                            randomCellX += 1;
                            randomCellY += 1;
                            break;
                        default:
                            break;
                    }

                    if (randomCellY < 0)
                    {
                        randomCellY = 0;
                    }
                    if (randomCellX < 0)
                    {
                        randomCellX = 0;
                    }
                    if (randomCellY >= worldJ)
                    {
                        randomCellY = worldJ - 1;
                    }
                    if (randomCellX >= worldI)
                    {
                        randomCellX = worldI - 1;
                    }

                    if((dungeonGrid[randomCellX,randomCellY] & DungeonTiles.FLOOR) != DungeonTiles.FLOOR)
                    {
                        dungeonGrid[randomCellX, randomCellY] = DungeonTiles.FLOOR | DungeonTiles.VISITED;
                        floorCount += 1;
                    }
                }

                int iterations = 3;
                for (int z = 0; z <= iterations; z++)
                {
                    DungeonTiles[,] newMap = new DungeonTiles[worldI, worldJ];
                    for (int i = 0; i < worldI; i++)
                    {
                        for (int j = 0; j < worldJ; j++)
                        {
                            int numRocks = 0;
                            //Check 8 directions and self
                            //Self:
                            if (dungeonGrid[i, j] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Topleft
                            if (i - 1 < 0 || j - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i - 1, j - 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Top
                            if (j - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i, j - 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Topright
                            if (i + 1 > worldI - 1 || j - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i + 1, j - 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Left
                            if (i - 1 < 0)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i - 1, j] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Right
                            if (i + 1 > worldI - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i + 1, j] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Bottomleft
                            if (i - 1 < 0 || j + 1 > worldJ - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i - 1, j + 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //Bottom
                            if (j + 1 > worldJ - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i, j + 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }
                            //BottomRight
                            if (i + 1 > worldI - 1 || j + 1 > worldJ - 1)
                            {
                                numRocks += 1;
                            }
                            else if (dungeonGrid[i + 1, j + 1] == DungeonTiles.ROCK)
                            {
                                numRocks += 1;
                            }


                            if (numRocks >= 4 || i == 0 || j == 0 || i == worldI - 1 || j == worldJ - 1)
                            {
                                newMap[i, j] = DungeonTiles.ROCK;
                            }
                            else
                            {
                                newMap[i, j] = DungeonTiles.FLOOR;
                            }
                        }
                    }
                    Array.Copy(newMap, dungeonGrid, worldJ * worldI);
                }

                int fillX = 0;
                int fillY = 0;
                do
                {
                    fillX = random.Next(0, worldI);
                    fillY = random.Next(0, worldJ);
                } while ((dungeonGrid[fillX, fillY] & DungeonTiles.FLOOR) != DungeonTiles.FLOOR);

                this.FloodFill(fillX, fillY, worldI, worldJ, ref dungeonGrid);

                double connectedTiles = 0.0;
                double totalTiles = 0.0;
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            totalTiles += 1.0;
                            if ((dungeonGrid[i, j] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
                            {
                                connectedTiles += 1.0;
                            }
                        }
                    }
                }

                if (connectedTiles / totalTiles >= .70)
                {
                    for (int i = 0; i < worldI; i++)
                    {
                        for (int j = 0; j < worldJ; j++)
                        {
                            if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && (dungeonGrid[i, j] & DungeonTiles.VISITED) != DungeonTiles.VISITED)
                            {
                                dungeonGrid[i, j] = DungeonTiles.ROCK;
                            }
                        }
                    }
                    acceptable = true;
                }
            }

            //Mark Walls
            for (int i = 0; i < worldI; i++)
            {
                for (int j = 0; j < worldJ; j++)
                {
                    int numFloor = 0;
                    //Check 8 directions and self
                    //Self:
                    if (dungeonGrid[i, j] == DungeonTiles.ROCK)
                    {
                        //Topleft
                        if (!(i - 1 < 0 || j - 1 < 0) && (dungeonGrid[i - 1, j - 1] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //Top
                        if (!(j - 1 < 0) && (dungeonGrid[i, j - 1] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //Topright
                        if (!(i + 1 > worldI - 1 || j - 1 < 0) && (dungeonGrid[i + 1, j - 1] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //Left
                        if (!(i - 1 < 0) && (dungeonGrid[i - 1, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //Right
                        if (!(i + 1 > worldI - 1) && (dungeonGrid[i + 1, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //Bottomleft
                        if (!(i - 1 < 0 || j + 1 > worldJ - 1) && (dungeonGrid[i - 1, j + 1] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //Bottom
                        if (!(j + 1 > worldJ - 1) && (dungeonGrid[i, j + 1] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }
                        //BottomRight
                        if (!(i + 1 > worldI - 1 || j + 1 > worldJ - 1) && (dungeonGrid[i + 1, j + 1] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            numFloor += 1;
                        }

                        if (numFloor > 0)
                        {
                            dungeonGrid[i, j] = DungeonTiles.WALL;
                        }
                    }

                }
            }

            return new Vector2(worldI, worldJ);
        }

        private void FloodFill(int x, int y, int worldI, int worldJ, ref DungeonTiles[,] dungeonGrid)
        {
            if (x < 0 || y < 0 || x >= worldI || y >= worldJ)
            {
                return;
            }
            if ((dungeonGrid[x, y] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
            {
                return;
            }
            if ((dungeonGrid[x, y] & DungeonTiles.WALL) == DungeonTiles.WALL)
            {
                return;
            }

            Queue<Vector2> floodQueue = new Queue<Vector2>();
            floodQueue.Enqueue(new Vector2(x, y));

            while (floodQueue.Count > 0)
            {
                Vector2 pos = floodQueue.Dequeue();
                if ((dungeonGrid[(int)pos.X, (int)pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && (dungeonGrid[(int)pos.X, (int)pos.Y] & DungeonTiles.VISITED) != DungeonTiles.VISITED)
                {
                    x = (int)pos.X;
                    y = (int)pos.Y;
                    dungeonGrid[(int)pos.X, (int)pos.Y] = dungeonGrid[(int)pos.X, (int)pos.Y] | DungeonTiles.VISITED;
                    HashSet<Vector2> toAdd = new HashSet<Vector2>();
                    toAdd.Add(new Vector2(x + 1, y + 1));
                    toAdd.Add(new Vector2(x, y + 1));
                    toAdd.Add(new Vector2(x + 1, y));
                    toAdd.Add(new Vector2(x - 1, y - 1));
                    toAdd.Add(new Vector2(x - 1, y));
                    toAdd.Add(new Vector2(x, y - 1));
                    toAdd.Add(new Vector2(x + 1, y - 1));
                    toAdd.Add(new Vector2(x - 1, y + 1));
                    foreach (var vector in toAdd)
                    {
                        if (!floodQueue.Contains(vector))
                        {
                            floodQueue.Enqueue(vector);
                        }
                    }
                    toAdd.Clear();
                }
            }

        }
    }
}
