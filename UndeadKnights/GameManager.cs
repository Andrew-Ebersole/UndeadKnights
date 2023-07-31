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
// Last Update   | 7-31-23
// Purpose       | Manages all the content in the game, updates all
//               | The players, monsters, and tiles
// ---------------------------------------------------------------- //

namespace UndeadKnights
{
    internal class GameManager
    {
        // --- Fields --- //

        //
        private Point camera;
        private int tileSize;

        // stats
        private double playTime;
        private int nights;

        // fonts
        private SpriteFont vinque24;

        // inputs
        private KeyboardState currentKS;
        private KeyboardState previousKS;

        // resources 
        private int wood;
        private int stone;
        private int food;
        private int people;

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

        public int Wood { get { return wood; } set {  wood = value; } }
        public int Stone { get {  return stone; } set {  stone = value; } }
        public int Food { get { return food; } set {  food = value; } }
        public int People { get { return people; } set { people = value; } }


        // --- Constructor --- //

        public void Initialize(ContentManager content,
           Point windowsize, GraphicsDevice gd)
        {
            vinque24 = content.Load<SpriteFont>("vinque-24");

            currentKS = Keyboard.GetState();
            previousKS = Keyboard.GetState();

            // Starting materials
            wood = 10;
            stone = 10;
            food = 10;
            people = 10;

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
                if (tileSize < 100 && SingleKeyPress(Keys.Up)) {
                    camera.X += (int)(1920/(3*Math.Pow(2,(tileSize/25))));
                    camera.Y += (int)(1080 / (3 * Math.Pow(2, (tileSize / 25))));
                    tileSize += 25; }
                if (tileSize > 50 && SingleKeyPress(Keys.Down)) { tileSize -= 25;
                    camera.X -= (int)(1920 / (3 * Math.Pow(2, (tileSize / 25))));
                    camera.Y -= (int)(1080 / (3 * Math.Pow(2, (tileSize / 25))));
                }

                // Move camera controls
                if (currentKS.IsKeyDown(Keys.A)) { camera.X-=5; }
                if (currentKS.IsKeyDown(Keys.W)) { camera.Y-=5; }
                if (currentKS.IsKeyDown(Keys.D)) { camera.X+=5; }
                if (currentKS.IsKeyDown(Keys.S)) { camera.Y+=5; }

                // Make sure camera doesn't go out of bounds
                if (camera.X < 0) { camera.X = 0; }
                if (camera.Y < 0) { camera.Y = 0; }
                // Formula assumes that window size is 1920x1080
                if (camera.X > 1275-1920/(tileSize/25)) { camera.X = 1275 - 1920 / (tileSize / 25); }
                if (camera.Y > 1275-1080/(tileSize/25)) { camera.Y = 1275 - 1080 / (tileSize / 25); }

                // update timer
                playTime += gt.ElapsedGameTime.Milliseconds;

                // update subclasses
                TileManager.Get.Update(gt);

                previousKS = currentKS;
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            if (MenuUI.Get.GameFSM == GameState.Game
                || MenuUI.Get.GameFSM == GameState.GameOver)
            {
                // Draw subclasses
                TileManager.Get.Draw(sb);

                // Display People
                sb.DrawString(vinque24,
                    "People: 0",
                    new Vector2(10, 10),
                    Color.White);

                // Display Resources
                sb.DrawString(vinque24,
                    $"Wood: {wood}" +
                    $"\nStone: {stone}" +
                    $"\nFood: {food}",
                    new Vector2(10,55),
                    Color.White);
                
                // Timer
                sb.DrawString(vinque24,
                    $"{Math.Round(Math.Floor(playTime/60000))}:{Math.Round((playTime/1000)%60):00}",
                    new Vector2(1800,10),
                    Color.White);

                // Draw Camera Position
                //sb.DrawString(vinque24,$"({Camera.X},{Camera.Y}) : {tileSize} ", new Vector2(10,10),Color.White);
            }
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

        public void NewGame()
        {
            TileManager.Get.NewMap();
            tileSize = 50;
            camera = new Point(155, 370);
        }
    }
}
