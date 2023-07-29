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

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-26-23
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

            // Create randomifier
            rng = new Random();

            // Set all to grass
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j] = new Tile(TileType.Grass,environmentSpriteSheet);
                }
            }

            // Set middle to town hall
            // (right now its path because i dont have a town hall
            GeneratePath();
            tileGrid[25,25] = new Tile(TileType.Path,environmentSpriteSheet);
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {

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
                    tileGrid[i, j].Draw(sb, new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Generates the path that the enemies will come from
        /// </summary>
        public void GeneratePath()
        {
            // Right now path is hard coded
            // down the line will either use file reading or
            // preferrably procedurally generated paths
            for (int y = 25; y < 38; y++)
            {
                tileGrid[25, y] = new Tile(TileType.Path, environmentSpriteSheet);
            }
            for (int x = 25; x < 36; x++)
            {
                tileGrid[x, 38] = new Tile(TileType.Path, environmentSpriteSheet);
            }
            for (int y = 38; y < 51; y++)
            {
                tileGrid[36, y] = new Tile(TileType.Path, environmentSpriteSheet);
            }
            FillGrass(20);
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
                                if (rng.Next(2,5) < NearestPath(x, y))
                                {
                                    tileGrid[x, y] = new Tile(TileType.Tree, environmentSpriteSheet);
                                }
                            }
                            else
                            {
                                // If the rock is near a path dont spawn (with randomization)
                                if (rng.Next(5,10) < NearestPath(x, y))
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
                if (tileGrid[x, y].TileType == TileType.Path)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
