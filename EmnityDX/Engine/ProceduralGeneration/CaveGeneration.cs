using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine.ProceduralGeneration
{
    public class CaveGeneration : IGenerationAlgorithm
    {
        public Vector2 GenerateDungeon(ref DungeonTiles[,] dungeonGrid, int worldMin, int worldMax, Random random)
        {
            int worldI = random.Next(worldMin, worldMax);
            int worldJ = random.Next(worldMin, worldMax);

            dungeonGrid = new DungeonTiles[worldI, worldJ];

            bool acceptable = false;

            while (!acceptable)
            {
                //Cellular Automata
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        int choice = random.Next(0, 101);
                        dungeonGrid[i, j] = (choice <= 41) ? DungeonTiles.ROCK : DungeonTiles.FLOOR;
                    }
                }

                int iterations = 12;
                for (int z = 0; z <= iterations; z++)
                {
                    DungeonTiles[,] newMap = new DungeonTiles[worldI, worldJ];
                    for (int i = 0; i < worldI; i++)
                    {
                        for (int j = 0; j < worldJ; j++)
                        {
                            int numRocks = 0;
                            int farRocks = 0;
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

                            //Check 8 directions for far rocks

                            //Topleft
                            if (i - 2 < 0 || j - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i - 2, j - 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Top
                            if (j - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i, j - 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Topright
                            if (i + 2 > worldI - 2 || j - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i + 2, j - 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Left
                            if (i - 2 < 0)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i - 2, j] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Right
                            if (i + 2 > worldI - 2)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i + 2, j] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Bottomleft
                            if (i - 2 < 0 || j + 2 > worldJ - 2)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i - 2, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //Bottom
                            if (j + 2 > worldJ - 2)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            //BottomRight
                            if (i + 2 > worldI - 2 || j + 2 > worldJ - 2 || dungeonGrid[i + 2, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }
                            else if (dungeonGrid[i + 2, j + 2] == DungeonTiles.ROCK)
                            {
                                farRocks += 1;
                            }


                            if (numRocks >= 5 || i == 0 || j == 0 || i == worldI - 1 || j == worldJ - 1 || (z <= 3 && numRocks + farRocks <= 2))
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
                for(int i = 0; i < worldI; i++)
                {
                    for(int j = 0; j < worldJ; j++)
                    {
                        if((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR)
                        {
                            totalTiles += 1.0;
                            if ((dungeonGrid[i, j] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
                            {
                                connectedTiles += 1.0;
                            }
                        }
                    }
                }

                if(connectedTiles / totalTiles >= .90)
                {
                    for (int i = 0; i < worldI; i++)
                    {
                        for (int j = 0; j < worldJ; j++)
                        {
                            if ((dungeonGrid[i, j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && (dungeonGrid[i, j] & DungeonTiles.VISITED) != DungeonTiles.VISITED)
                            {
                                dungeonGrid[i, j] = DungeonTiles.WALL;
                            }
                        }
                    }
                    acceptable = true;
                }

            }

            return new Vector2(worldI,worldJ);
        }

        private void FloodFill(int x, int y, int worldI, int worldJ, ref DungeonTiles[,] dungeonGrid)
        {
            if (x < 0 || y < 0 || x >= worldI || y >= worldJ)
            {
                return;
            }
            if((dungeonGrid[x,y] & DungeonTiles.VISITED) == DungeonTiles.VISITED)
            {
                return;
            }
            if ((dungeonGrid[x, y] & DungeonTiles.WALL) == DungeonTiles.WALL)
            {
                return;
            }

            Queue<Vector2> floodQueue = new Queue<Vector2>();
            floodQueue.Enqueue(new Vector2(x, y));
            
            while(floodQueue.Count > 0)
            {
                Vector2 pos = floodQueue.Dequeue();
                if((dungeonGrid[(int)pos.X, (int)pos.Y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && (dungeonGrid[(int)pos.X, (int)pos.Y] & DungeonTiles.VISITED) != DungeonTiles.VISITED)
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
                    foreach(var vector in toAdd)
                    {
                        if(!floodQueue.Contains(vector))
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
