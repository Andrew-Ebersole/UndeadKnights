using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-28-23
// Purpose       | Controls the menu of the game
// ---------------------------------------------------------------- //

namespace UndeadKnights
{
    // Enum for the different menu screens
    enum GameState
    {
        Menu,
        Game,
        GameOver,
        Settings,
        Credits
    }

    internal class MenuUI
    {
        // --- Fields --- //

        // GameStates
        GameState gameFSM;
        private Texture2D singlecolor;
        private SpriteFont arial12;

        // Buttons
        private Button testButton;



        // Singleton
        private static MenuUI instance = null;

        public static MenuUI Get
        {
            get
            {
                if (instance == null)
                {
                    instance = new MenuUI();
                }

                return instance;
            }
        }

        // --- Properties --- //





        // --- Constructor --- //

        /// <summary>
        /// Initializes the MenuUI class
        /// </summary>
        /// <param name="content"></param>
        /// <param name="windowsize"></param>
        /// <param name="gd"></param>
        public void Initialize(Microsoft.Xna.Framework.Content.ContentManager content,
            Point windowsize, GraphicsDevice gd)
        {
            arial12 = content.Load<SpriteFont>("arial-12");

            gameFSM = GameState.Menu;

            testButton = new Button(new Rectangle(10,10,100,100),"start game",gd,arial12);
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            testButton.Update(gt);

            if (testButton.IsPressed)
            {
                gameFSM = GameState.Game;
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            switch (gameFSM)
            {
                case GameState.Menu:
                    testButton.Draw(sb);
                    break;
            }
        }

    }
}
