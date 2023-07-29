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
            tileGrid = new Tile[50, 50];
            environmentSpriteSheet = content.Load<Texture2D>("EnvironmentSpriteSheet");

            rng = new Random();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    tileGrid[i, j] = new Tile(environmentSpriteSheet, 
                        new Point(rng.Next(0, 2), rng.Next(0, 2)));
                }
            }
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
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    tileGrid[i, j].Draw(sb, new Point(i, j));
                }
            }
        }
    }
}
