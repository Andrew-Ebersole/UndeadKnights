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
using UndeadKnights.Humans;


// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-19-23
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
        private float fps;

        // stats
        private double playTime;
        private int nights;
        private double fadeToNightTimer;

        // fonts
        private SpriteFont vinque24;

        // inputs
        private KeyboardState currentKS;
        private KeyboardState previousKS;

        // resources 
        private int wood;
        private int stone;
        private int food;

        private Texture2D singlecolor;

        // a boolean
        private bool tutorial;

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

        public Texture2D SingleColor { get { return singlecolor; } }

        public bool Tutorial { get { return tutorial; } set { tutorial = value; } }

        public SpriteFont Vinque24 { get { return vinque24; } }
        public bool IsNight 
        { 
            get 
            {
                if (Math.Floor(playTime / 60000) % 2 == 1) 
                { 
                    return true; 
                } 
                return  false; 
            } 
        }

        // --- Constructor --- //

        public void Initialize(ContentManager content,
           Point windowsize, GraphicsDevice gd)
        {
            vinque24 = content.Load<SpriteFont>("vinque-24");

            currentKS = Keyboard.GetState();
            previousKS = Keyboard.GetState();

            HumanManager.Get.Initialize(content, windowsize, gd);
            TileManager.Get.Initialize(content, windowsize, gd);

            // Graphics
            singlecolor = new Texture2D(gd, 1, 1);
            singlecolor.SetData(new Color[] { Color.White });

            // If tutorial will be displayed when game started
            tutorial = true;
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

                // Update Fade to night timer

                if (Math.Round(Math.Floor((playTime + 1000) / 60000)) % 2 == 1
                    && fadeToNightTimer <= 2000)
                {
                    // Dark screen when its night
                    fadeToNightTimer += gt.ElapsedGameTime.Milliseconds;
                    if (fadeToNightTimer > 2000)
                    {
                        fadeToNightTimer = 2000;
                    }
                } else if (fadeToNightTimer > 0)
                {
                    // Add +1 to the nights survived
                    // Refill grass with new trees and rocks
                    if (fadeToNightTimer == 2000)
                    {
                        nights++;
                        TileManager.Get.FillGrass(95);
                        TileManager.Get.GrowFarms();
                    }
                    // Undarken screen when its day
                    fadeToNightTimer -= gt.ElapsedGameTime.Milliseconds;
                    if (fadeToNightTimer < 0)
                    {
                        fadeToNightTimer = 0;
                    }
                }

                // update subclasses
                TileManager.Get.Update(gt);
                HumanManager.Get.Update(gt);

                // Pause when escape pressed
                if (SingleKeyPress(Keys.Escape))
                {
                    MenuUI.Get.GameFSM = GameState.Settings;
                    MenuUI.Get.LastState = GameState.Game;
                }

                previousKS = currentKS;
            }

            // update fps
            if (gt.ElapsedGameTime.Milliseconds > 0)
            {
                fps = 1000 / gt.ElapsedGameTime.Milliseconds;
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            if (MenuUI.Get.GameFSM == GameState.Game
                || MenuUI.Get.GameFSM == GameState.GameOver
                || (MenuUI.Get.GameFSM == GameState.Settings && MenuUI.Get.LastState == GameState.Game))
            {
                

                // Draw subclasses
                TileManager.Get.Draw(sb);
                HumanManager.Get.Draw(sb);

                // Draw nighttime overlay
                sb.Draw(singlecolor, new Rectangle(0, 0, 1920, 1080),
                        Color.Black * (0.45f * (float)(fadeToNightTimer / 2000f)));

                // Display People
                sb.DrawString(vinque24,
                    $"People: {HumanManager.Get.TotalWorkers()} " +
                    $"({HumanManager.Get.TotalWorkers() - HumanManager.Get.WorkingWorkers()})",
                    new Vector2(10, 10),
                    Color.White);

                // Display Resources
                sb.DrawString(vinque24,
                    $"Wood: {wood}" +
                    $"\nStone: {stone}" +
                    $"\nFood: {food}" +
                    $"\nFPS: {Math.Round(fps, 1)}",
                    new Vector2(10,55),
                    Color.White);
                
                // Timer
                sb.DrawString(vinque24,
                    $"{Math.Round(Math.Floor(playTime/60000))}:{Math.Floor(((playTime/1000)%60)):00}",
                    new Vector2(1700,50),
                    Color.White);

                // Nights Survived
                sb.DrawString(vinque24,
                    $"Nights: {nights}",
                    new Vector2(1700, 10),
                    Color.White);

                // If in settings menu draw background shadow
                if (MenuUI.Get.GameFSM == GameState.Settings
                    || MenuUI.Get.GameFSM == GameState.GameOver)
                {
                    sb.Draw(singlecolor, new Rectangle(0, 0, 1920, 1080), Color.Black * 0.45f);
                }

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
            HumanManager.Get.NewGame();
            TileManager.Get.NewMap();
            tileSize = 50;
            camera = new Point(155, 170);
            playTime = 0;
            fadeToNightTimer = 0;
            wood = 10;
            food = 5;
            stone = 5;
        }
    }
}
