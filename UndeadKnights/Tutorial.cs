using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 9-15-23
// Last Update   | 1-4-24
// Purpose       | Used to show tips on how to play at different
//               | Stages in the game
// ---------------------------------------------------------------- //

namespace UndeadKnights
{
    enum TutorialScreen
    {
        None,
        Controls,
        CollectingResources,
        Building,
        CreatingTroops,
        Enemies
    }

    internal class Tutorial
    {
        // --- Fields --- //

        private TutorialScreen screen;
        private double timer;
        private List<TutorialScreen> screensShown;
        private Texture2D singleColor;
        private SpriteFont vinque48;
        private SpriteFont vinque72;
        private Rectangle window;

        // --- Properties --- //

        /// <summary>
        /// Determine if the tutorial screen has been displayed long enough
        /// for the player to click away. We don't want the player to accidentaly
        /// click before reading the tutorial.
        /// </summary>
        public bool IsPressed
        { get 
            { 
                if(timer > 800 && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    screen = TutorialScreen.None;
                    return true;
                }
                return false;
            } 
        }

        public List<TutorialScreen> ScreensShown { get { return screensShown; } }

        // --- Constructor --- //

        public Tutorial(ContentManager content, Rectangle window)
        {
            screen = TutorialScreen.None;
            timer = 0;
            screensShown = new List<TutorialScreen>();
            vinque48 = content.Load<SpriteFont>("vinque-48");
            vinque72 = content.Load<SpriteFont>("vinque-72");
            this.window = window;
        }




        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            timer += gt.ElapsedGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            DrawScreen(sb);

            // Choose what tutorial to show
            switch (screen)
            {
                case TutorialScreen.Controls: // --- Controls -----------------------------------//
                    // Draw the title
                    sb.DrawString(vinque72,
                        "Controls",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);

                    // Line 1
                    sb.DrawString(vinque48,
                        "WASD:",
                        new Vector2(window.Width / 12, 4 * window.Height / 12),
                        Color.White);
                    sb.DrawString(vinque48,
                        "move the camera",
                        new Vector2(6 * window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    // Line 2
                    sb.DrawString(vinque48,
                        "UP / DOWN Arrows:",
                        new Vector2(window.Width / 12, 6 * window.Height / 12),
                        Color.White);
                    sb.DrawString(vinque48,
                        "zoom in and out",
                        new Vector2(6 * window.Width / 12, 6 * window.Height / 12),
                        Color.White);

                    // Line 3
                    sb.DrawString(vinque48,
                        "Left Click:",
                        new Vector2(window.Width / 12, 8 * window.Height / 12),
                        Color.White);
                    sb.DrawString(vinque48,
                        "build and collect resources",
                        new Vector2(6 * window.Width / 12, 8 * window.Height / 12),
                        Color.White);

                    break;

                case TutorialScreen.CollectingResources: // --- Resources -----------------------------------//
                    // Draw the title
                    sb.DrawString(vinque72,
                        "Resources",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);

                    // Line 1
                    sb.DrawString(vinque48,
                        "Click trees and boulders to collect resources",
                        new Vector2(window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    // Line 2
                    sb.DrawString(vinque48,
                        "Click an empty tile to build",
                        new Vector2(window.Width / 12, 6 * window.Height / 12),
                        Color.White);

                    // Line 3
                    sb.DrawString(vinque48,
                        "Click on houses to create more people",
                        new Vector2(window.Width / 12, 8 * window.Height / 12),
                        Color.White);

                    // Line 4
                    sb.DrawString(vinque48,
                        "Collect food from farms",
                        new Vector2(window.Width / 12, 10 * window.Height / 12),
                        Color.White);
                    break;

                case TutorialScreen.Building: // --- Building -----------------------------------//
                    // Draw the title
                    sb.DrawString(vinque72,
                        "Building",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);

                    // Line 1
                    sb.DrawString(vinque48,
                        "Build more houses to gain more people",
                        new Vector2(window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    // Line 2
                    sb.DrawString(vinque48,
                        "Build farms to grow more food",
                        new Vector2(window.Width / 12, 6 * window.Height / 12),
                        Color.White);

                    // Line 3
                    sb.DrawString(vinque48,
                        "Build walls to stop enemies",
                        new Vector2(window.Width / 12, 8 * window.Height / 12),
                        Color.White);

                    // Line 4
                    sb.DrawString(vinque48,
                        "Build the Armory to creating troops",
                        new Vector2(window.Width / 12, 10 * window.Height / 12),
                        Color.White);
                    break;

                case TutorialScreen.CreatingTroops: // --- Troops -----------------------------------//
                    // Draw the title
                    sb.DrawString(vinque72,
                        "Troops",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);
                    // Line 1
                    sb.DrawString(vinque48,
                        "Train troops to defeat enemies",
                        new Vector2(window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    // Line 2
                    sb.DrawString(vinque48,
                        "Upgrade the armory to train specialized troops",
                        new Vector2(window.Width / 12, 6 * window.Height / 12),
                        Color.White);

                    // Line 3
                    sb.DrawString(vinque48,
                        "Archers can attack from behind walls",
                        new Vector2(window.Width / 12, 8 * window.Height / 12),
                        Color.White);

                    // Line 4
                    sb.DrawString(vinque48,
                        "Knights move quickly and have more health",
                        new Vector2(window.Width / 12, 10 * window.Height / 12),
                        Color.White);
                    break;

                case TutorialScreen.Enemies: // --- Enemies -----------------------------------//
                    // Draw the title
                    sb.DrawString(vinque72,
                        "Enemies",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);

                    // Line 1
                    sb.DrawString(vinque48,
                        "Enemies attack at night",
                        new Vector2(window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    // Line 2
                    sb.DrawString(vinque48,
                        "Workers stop working at night",
                        new Vector2(window.Width / 12, 6 * window.Height / 12),
                        Color.White);

                    // Line 3
                    sb.DrawString(vinque48,
                        "Use troops and build defenses to defeat the enemies",
                        new Vector2(window.Width / 12, 8 * window.Height / 12),
                        Color.White);

                    break;
            }
        }

        /// <summary>
        /// Call to Update  
        /// </summary>
        /// <param name="screen"></param>
        public void UpdateScreen(TutorialScreen screen)
        {
            this.screen = screen;
            screensShown.Add(screen);
            timer = 0;
        }

        private void DrawScreen(SpriteBatch sb)
        {
            singleColor = GameManager.Get.SingleColor;
            sb.Draw(singleColor, window, Color.Black * 0.7f);
        }
    }
}
