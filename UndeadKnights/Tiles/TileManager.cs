using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content;
using System.Security.Cryptography;
using System.IO;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-29-23
// Purpose       | Manages all the tiles and upgrades them when needed
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    enum TileType
    {
        Grass,
        Path,
        Tree,
        Rock,
        TownHall,
        House,
        Farm,
        Wall,
        Gate,
        Turret,
        Armory,
        ShootingRange,
        Stable,
        Ballista
    }

    internal class TileManager
    {
        // --- Fields --- //

        // Tile Grid
        private Tile[,] tileGrid;
        private Texture2D environmentSpriteSheet;
        private Texture2D buildingSpriteSheet;
        private Random rng;

        //Singleton

        public static TileManager instance = null;

        public static TileManager Get
        {
            get
            {
                if (instance == null)
                {
                    instance = new TileManager();
                }

                return instance;
            }
        }

        // --- Properties --- //

        public Tile[,] TileGrid { get { return tileGrid; } }



        // --- Constructor --- //

        public void Initialize(ContentManager content,
           Point windowsize, GraphicsDevice gd)
        {
            // Create new grid of tiles
            tileGrid = new Tile[51, 51];
            environmentSpriteSheet = content.Load<Texture2D>("EnvironmentSpriteSheet");
            buildingSpriteSheet = content.Load<Texture2D>("Buildings");
            // Create randomifier
            rng = new Random();

            NewMap();
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j].Update(gt, new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j].Draw(sb);
                }
            }
        }


        public void NewMap()
        {
            // Set all to grass
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                }
            }

            // (right now its path because i dont have a town hall
            GeneratePath();

            // Set middle to town hall
            tileGrid[25, 25] = new Building(TileType.TownHall, buildingSpriteSheet, 200, 1);
        }

        /// <summary>
        /// Generates the path that the enemies will come from
        /// </summary>
        public void GeneratePath()
        {
            FillPath(new Point(25,25),new Point(0,rng.Next(0,51)));
            FillPath(new Point(25, 25), new Point(rng.Next(0, 51), 50));
            FillPath(new Point(25, 25), new Point(50, rng.Next(0, 51)));

            FillGrass(20);
        }

        /// <summary>
        /// Generates a path between two points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void FillPath(Point start, Point end)
        {
            if (tileGrid[end.X,end.Y].TileType != TileType.Path)
            {

                // Start by whichever direction is farther
                if (Math.Abs(start.X - end.X) >Math.Abs(start.Y - end.Y))
                {
                    // Move toward X a random amount of times
                    if (start.X < end.X)
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.X - end.X), 2) / 4)+1); i++)
                        {
                            if (start.X > 0 && start.X < 50)
                            {
                                start.X++;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.X - end.X), 2) / 4)+1); i++)
                        {
                            if (start.X > 0 && start.X < 50)
                            {
                                start.X--;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                } else
                {
                    // Move toward end in Y direction a random amount of times
                    if (start.Y < end.Y)
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.Y - end.Y), 2) / 4)+1); i++)
                        {
                            if (start.Y > 0 && start.Y < 50)
                            {
                                start.Y++;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    } else
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.Y - end.Y), 2) / 4)+1); i++)
                        {
                            if (start.Y > 0 && start.Y < 50)
                            {
                                start.Y--;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                }

                // Reccurs until at the end
                FillPath(start, end);
            }
        }

        /// <summary>
        /// Replaces grass with other resources
        /// </summary>
        /// <param name="GrassPercentLeft"> Percent of tiles that will remain grass</param>
        public void FillGrass(int GrassPercentLeft)
        {
            // Loop for every tile
            for (int x = 0; x < 51; x++)
            {
                for (int y = 0; y < 51; y++)
                {
                    // Check if its grass
                    if (tileGrid[x,y].TileType == TileType.Grass)
                    {
                        // If it is do rng to change to tree or rock
                        if (rng.Next(0,100) > GrassPercentLeft)
                        {
                            // Determine if its will be a rock or tree
                            if (rng.Next(0, 100) > 20)
                            {
                                // If the tree is near a path dont spawn (with randomization)
                                if (rng.Next(1,3) < NearestPath(x, y))
                                {
                                    tileGrid[x, y] = new Tile(TileType.Tree, environmentSpriteSheet);
                                }
                            }
                            else
                            {
                                // If the rock is near a path dont spawn (with randomization)
                                if (rng.Next(3,8) < NearestPath(x, y))
                                {
                                    tileGrid[x, y] = new Tile(TileType.Rock, environmentSpriteSheet);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Return how far away the nearest path is to the given tile
        /// Returns 11 if it is 11 or more
        /// </summary>
        /// <param name="startingX"></param>
        /// <param name="startingY"></param>
        /// <returns></returns>
        public int NearestPath(int startingX, int startingY)
        {
            int distance = 0;

            
            // Keep going until found path or distnace is greater than 11
            while (distance < 11)
            {
                // Starts right and goes clockwise outwards
                int x = distance;
                int y = 0;

                if (IsPath(startingX + x, startingY + y))
                {
                    return distance;
                }
                // Right to down
                while (y < distance)
                {
                    x-=1;
                    y+=1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // down to left
                while (x > -distance)
                {
                    x-=1;
                    y-=1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // Left to up
                while (y > -distance)
                {
                    x+=1;
                    y-=1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // up to right
                while (x < distance)
                {
                    x+=1;
                    y+=1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // increase distance
                distance++;
            }
            
            return distance;
        }

        /// <summary>
        /// Returns true if entered tile is a path
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsPath(int x, int y)
        {
            if (x >= 0 && y >= 0
                && x <51 && y < 51)
            {
                if (tileGrid[x, y].TileType != TileType.Grass
                    && tileGrid[x,y].TileType != TileType.Tree
                    && tileGrid[x,y].TileType != TileType.Rock)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
