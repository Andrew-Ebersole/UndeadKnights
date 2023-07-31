using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndeadKnights.Tiles;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-30-23
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
        private SpriteFont vinque48;
        private SpriteFont vinque72;

        // Buttons
        List<List<Button>> buttons;

        // Window size
        private Rectangle window;

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

        public List<List<Button>> Buttons { get { return buttons; } }

        public GameState GameFSM { get { return gameFSM; } }

        // --- Constructor --- //

        /// <summary>
        /// Initializes the MenuUI class
        /// </summary>
        /// <param name="content"></param>
        /// <param name="windowsize"></param>
        /// <param name="gd"></param>
        public void Initialize(ContentManager content,
            Point windowsize, GraphicsDevice gd)
        {
            // Fonts
            vinque48 = content.Load<SpriteFont>("vinque-48");
            vinque72 = content.Load<SpriteFont>("vinque-72");

            // Defualt Game State
            gameFSM = GameState.Menu;

            // Window size
            window = new Rectangle(new Point(0,0),windowsize);

            // Enter locations of all buttons
            CreateButtons(gd);
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            // Menu finite state machine
            switch (gameFSM)
            {
                case GameState.Menu: // --- Menu ------------------------------------------------//
                    foreach (Button button in buttons[0])
                    {
                        button.Update(gt);
                    }
                    if (buttons[0][0].IsPressed) 
                    { 
                        gameFSM = GameState.Game;
                        GameManager.Get.PlayTime = 0;
                        GameManager.Get.NewGame();
                    }
                    if (buttons[0][1].IsPressed) { gameFSM = GameState.Settings; }
                    if (buttons[0][2].IsPressed) { gameFSM = GameState.Credits; }

                    break;

                case GameState.Settings: // --- Settings ----------------------------------------//
                    foreach (Button button in buttons[1])
                    {
                        button.Update(gt);
                    }
                    if (buttons[1][0].IsPressed) { gameFSM = GameState.Menu; }
                    break;

                case GameState.Credits: // --- Credits -------------------------------------------//
                    foreach (Button button in buttons[2])
                    {
                        button.Update(gt);
                    }
                    if (buttons[2][0].IsPressed) { gameFSM = GameState.Menu; }
                    break;

                case GameState.Game: // --- Game ------------------------------------------------//
                    foreach (Button button in buttons[3])
                    {
                        button.Update(gt);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Back)) { gameFSM = GameState.GameOver; }
                    break;

                case GameState.GameOver: // --- GameOver ----------------------------------------//
                    foreach (Button button in buttons[4])
                    {
                        button.Update(gt);
                    }
                    if (buttons[4][0].IsPressed) { gameFSM = GameState.Menu; }
                    break;
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            
            // Menu finite state machine
            switch (gameFSM)
            {
                case GameState.Menu: // --- Menu ------------------------------------------------//

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Undead Knights",
                        new Vector2(window.Width/12,window.Height/12),
                        Color.Black);

                    // Draw all the buttons
                    foreach (Button button in buttons[0])
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Settings: // --- Settings ----------------------------------------//

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Settings",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.Black);

                    // Draw all the buttons
                    foreach (Button button in buttons[1])
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Credits: // --- Credits -------------------------------------------//

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Credits",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.Black);

                    // Draw all the buttons
                    foreach (Button button in buttons[2])
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Game: // --- Game ------------------------------------------------//
                    
                    // Draw all the buttons
                    foreach (Button button in buttons[3])
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.GameOver: // --- GameOver ----------------------------------------//

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Game Over",
                        new Vector2(3*window.Width / 12, 4*window.Height / 12),
                        Color.Black);

                    // Draw all the buttons
                    foreach (Button button in buttons[4])
                    {
                        button.Draw(sb);
                    }
                    break;
            }
        }

        private void CreateButtons(GraphicsDevice gd)
        {
            // Instantiate the list
            buttons = new List<List<Button>> 
            { 
                new List<Button>(),
                new List<Button>(),
                new List<Button>(),
                new List<Button>(),
                new List<Button>(),
            };

            // --- Menu ----------------------------------------//
            buttons[0].Add(new Button(new Rectangle(1*window.Width/12,8*window.Height/24,   // X,Y
                3*window.Width/12,1*window.Height/12),                                      // Width,Height
                "Play",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            buttons[0].Add(new Button(new Rectangle(1 * window.Width / 12, 11 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Settings",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            buttons[0].Add(new Button(new Rectangle(1 * window.Width / 12, 14 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Credits",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            buttons[0].Add(new Button(new Rectangle(1 * window.Width / 12, 17 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Quit",     // Text
                gd,         // Graphics Device
                vinque48)); // Font


            // --- Settings  ----------------------------------------//
            buttons[1].Add(new Button(new Rectangle(1 * window.Width / 12, 21 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Return to Menu",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            // --- Credits  ----------------------------------------//
            buttons[2].Add(new Button(new Rectangle(1 * window.Width / 12, 21 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Return to Menu",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            // ---  GameOver ----------------------------------------//
            buttons[4].Add(new Button(new Rectangle(9 * window.Width / 24, 15 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Return to Menu",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

        }

    }
}
