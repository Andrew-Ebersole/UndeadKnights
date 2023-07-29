using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UndeadKnights.Tiles;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-28-23
// Purpose       | Manages all the content in the game, updates all
//               | The players, monsters, and tiles
// ---------------------------------------------------------------- //

namespace UndeadKnights
{
    internal class GameManager
    {
        // --- Fields --- //

        private Point camera;
        private int tileSize;
        private double playTime;
        private int nights;
        private SpriteFont vinque24;
        private Rectangle mapSize;
        private KeyboardState currentKS;
        private KeyboardState previousKS;
        
        // Singleton
        private static GameManager instance = null;

        public static GameManager Get
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }

                return instance;
            }
        } 


        // --- Properties --- //

        public double PlayTime { get { return playTime; } set { playTime = value; } }

        public int TileSize { get { return tileSize; } }

        public Point Camera { get { return camera; } }
        // --- Constructor --- //

        public void Initialize(ContentManager content,
           Point windowsize, GraphicsDevice gd)
        {
            vinque24 = content.Load<SpriteFont>("vinque-24");
            tileSize = 50;
            camera = new Point(0, 0);

            currentKS = Keyboard.GetState();
            previousKS = Keyboard.GetState();

            TileManager.Get.Initialize(content, windowsize, gd);
        }


        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            if (MenuUI.Get.GameFSM == GameState.Game)
            {
                currentKS = Keyboard.GetState();

                // Zoom controls
                if (tileSize < 200 && currentKS.IsKeyDown(Keys.Up)) { tileSize += 2;}
                if (tileSize > 50 && currentKS.IsKeyDown(Keys.Down)) { tileSize -= 2;}

                // Move camera controls
                if (currentKS.IsKeyDown(Keys.A)) { camera.X-=5; }
                if (currentKS.IsKeyDown(Keys.W)) { camera.Y-=5; }
                if (currentKS.IsKeyDown(Keys.D)) { camera.X+=5; }
                if (currentKS.IsKeyDown(Keys.S)) { camera.Y+=5; }

                // Make sure camera doesn't go out of bounds
                if (camera.X < 0) { camera.X = 0; }
                if (camera.Y < 0) { camera.Y = 0; }
                // Formula assumes that window size is 1920x1080
                if (camera.X > 1250-1920/(tileSize/25)) { camera.X = 1250 - 1920 / (tileSize / 25); }
                if (camera.Y > 1250-1080/(tileSize/25)) { camera.Y = 1250 - 1080 / (tileSize / 25); }

                previousKS = currentKS;
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            if (MenuUI.Get.GameFSM == GameState.Game)
            {
                TileManager.Get.Draw(sb);
            }

            // Draw Camera Position
            sb.DrawString(vinque24,$"({Camera.X},{Camera.Y}) : {tileSize} " +
                $": {1250 - 1080 / (tileSize / 25)}",new Vector2(10,10),Color.White);
        }

        /// <summary>
        /// Return if it is the first frame the key was pressed
        /// </summary>
        /// <param name="key"> key to check </param>
        /// <returns> if the key was pressed </returns>
        private bool SingleKeyPress(Keys key)
        {
            if (currentKS.IsKeyDown(key)
                && previousKS.IsKeyUp(key))
            {
                return true;
            }
            return false;
        }
    }
}
