using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 9-15-23
// Last Update   | 1-3-24
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
        private int screensShown;

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

        public int ScreensShown { get { return screensShown; } }

        // --- Constructor --- //

        public Tutorial()
        {
            screen = TutorialScreen.None;
            timer = 0;
            screensShown = 0;
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
                case TutorialScreen.Controls:
                    break;

                case TutorialScreen.CollectingResources:
                    break;

                case TutorialScreen.Building:
                    break;

                case TutorialScreen.CreatingTroops:
                    break;

                case TutorialScreen.Enemies:
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
            screensShown++;
            timer = 0;
        }

        private void DrawScreen(SpriteBatch sb)
        {
            Texture2D singleColor = GameManager.Get.SingleColor;
            SpriteFont font = GameManager.Get.Vinque24;
            Rectangle window = new Rectangle(0, 0, 1920, 1080);
            sb.Draw(singleColor, window, Color.Black * 0.4f);
        }
    }
}
