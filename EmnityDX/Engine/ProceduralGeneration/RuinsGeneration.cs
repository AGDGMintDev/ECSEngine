using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EmnityDX.Engine.ProceduralGeneration
{
    public class RuinsGeneration : IGenerationAlgorithm
    {
        private enum BuildDirection
        {
            UP = 0,
            DOWN = 1,
            LEFT = 2,
            RIGHT = 4
        }

        public Vector2 GenerateDungeon(ref DungeonTiles[,] dungeonGrid, int worldMin, int worldMax, Random random)
        {
            int worldI = random.Next(worldMin, worldMax);
            int worldJ = random.Next(worldMin, worldMax);

            dungeonGrid = new DungeonTiles[worldI, worldJ];

            bool acceptable = false;

            while(!acceptable)
            {
                for (int i = 0; i < worldI; i++)
                {
                    for (int j = 0; j < worldJ; j++)
                    {
                        dungeonGrid[i, j] = DungeonTiles.ROCK;
                    }
                }

                int attempts = 0;
                int maxAttempts = 1000;
                int maxSize = 15;
                int minSize = 3;

                int minHall = 2;
                int maxHall = 30;

                //Create the first room
                int width = random.Next(minSize, maxSize);
                int height = random.Next(minSize, maxSize);

                for (int i = (worldI/2)-width; i < (worldI / 2) + width; i++)
                {
                    for (int j = (worldJ / 2) - height; j < (worldJ / 2) + height; j++)
                    {
                            dungeonGrid[i, j] = DungeonTiles.FLOOR;
                    }
                }

                while (attempts <= maxAttempts)
                {
                    //Choose random wall, decide feature to build, if feature can happen then make it happen otherwise an attempt was made
                    bool lastHallway = false;
                    bool foundWall = false;
                    BuildDirection direction = BuildDirection.UP;
                    int wallAttempts = 0;
                    int wallAttemptLimit = 10000;
                    int x = 0;
                    int y = 0;
                    //!(x < 0) && !(y < 0) && !(x >= worldI) && !(y >= worldJ)
                    while (!foundWall && wallAttempts < wallAttemptLimit)
                    {
                        //if (!lastHallway)
                        //{
                            x = random.Next(0, worldI);
                            y = random.Next(0, worldJ);
                        //}
                        if ( (dungeonGrid[x,y] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR )
                        {
                            //Check for an adjacent empty spot
                            //Up
                            if(!(y-1 < 0) && (dungeonGrid[x,y-1] & DungeonTiles.ROCK) == DungeonTiles.ROCK)
                            {
                                foundWall = true;
                                direction = BuildDirection.UP;
                            }
                            //Down
                            else if(!(y+1 >= worldJ) && (dungeonGrid[x,y+1] & DungeonTiles.ROCK) == DungeonTiles.ROCK)
                            {
                                foundWall = true;
                                direction = BuildDirection.DOWN;
                            }
                            //Left
                            else if(!(x-1 < 0) && (dungeonGrid[x-1,y] & DungeonTiles.ROCK) == DungeonTiles.ROCK)
                            {
                                foundWall = true;
                                direction = BuildDirection.LEFT;
                            }
                            //Right
                            else if(!(x+1 >= worldI) && (dungeonGrid[x+1,y] & DungeonTiles.ROCK) == DungeonTiles.ROCK)
                            {
                                foundWall = true;
                                direction = BuildDirection.RIGHT;
                            }
                            else
                            {
                                wallAttempts += 1;
                            }

                        }
                    }

                    //Found a wall and direction.  Try to build either a room or hallway
                    int hallwayOrRoom = random.Next(101);
                    if (foundWall)
                    {
                        if (hallwayOrRoom > 7) //Room
                        {
                            width = random.Next(minSize, maxSize);
                            height = random.Next(minSize, maxSize);
                            lastHallway = false;
                        }
                        else //Hallway
                        {
                            switch(direction)
                            {
                                case BuildDirection.UP:
                                case BuildDirection.DOWN:
                                    width = minHall;
                                    height = random.Next(minHall, maxHall);
                                    break;
                                case BuildDirection.LEFT:
                                case BuildDirection.RIGHT:
                                    width = random.Next(minHall, maxHall);
                                    height = minHall;
                                    break;
                            }
                            lastHallway = true;
                        }
                        if (direction == BuildDirection.UP)
                        {
                            x -= width;
                            y -= height;
                        }
                        else if(direction == BuildDirection.LEFT)
                        {
                            x -= width;
                            y -= height;
                        }
                        if (!buildRoom(x,y,height,width,worldI,worldJ,ref dungeonGrid, lastHallway))
                        {
                            attempts += 1;
                        }
                        else if (lastHallway)
                        {
                            attempts += 1;
                        }
                    }
                    else
                    {
                        attempts += 1;
                    }

                }

                acceptable = true;
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

        private bool buildRoom(int posX, int posY, int height, int width, int worldI, int worldJ, ref DungeonTiles[,] dungeonGrid, bool lastHallway)
        {
            for(int i = posX; i < posX + width; i++)
            {
                for(int j = posY; j < posY + height; j++)
                {
                    if (i < 1 || i >= worldI-1 || j < 1 || j >= worldJ-1)
                    {
                        return false;
                    }
                    if ((dungeonGrid[i,j] & DungeonTiles.FLOOR) == DungeonTiles.FLOOR && !lastHallway)
                    {
                        return false;
                    }
                    if ((dungeonGrid[i, j] & DungeonTiles.WALL) == DungeonTiles.WALL)
                    {
                        return false;
                    }
                }
            }

            for (int i = posX; i < posX + width; i++)
            {
                for (int j = posY; j < posY + height; j++)
                {
                        dungeonGrid[i, j] = DungeonTiles.FLOOR;
                }
            }

            return true;
        }

    }
}
