using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndeadKnights.Tiles;
using System;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 1-4-24
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
        Credits,
        Tutorial
    }

    internal class MenuUI
    {
        // --- Fields --- //

        // GameStates
        GameState gameFSM;
        GameState lastState;
        private Texture2D singlecolor;
        private SpriteFont vinque48;
        private SpriteFont vinque72;
        private bool displayTutorial;

        // Buttons
        List<List<Button>> buttons;

        // Window size
        private Rectangle window;

        // Singleton
        private static MenuUI instance = null;

        // Tutorial
        Tutorial tutorial;

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

        public GameState GameFSM { get { return gameFSM; } set { gameFSM = value; } }

        public GameState LastState { get { return lastState; } set { lastState = value; } }



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
            lastState = GameState.Menu;


            // Window size
            window = new Rectangle(new Point(0,0),windowsize);

            // Enter locations of all buttons
            CreateButtons(gd);

            tutorial = new Tutorial();
            displayTutorial = true;
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt, GraphicsDeviceManager graphicsDevice)
        {
            
            // Tutorial checks
            if (displayTutorial)
            {
                // Controls (0 seconds)
                if (GameManager.Get.PlayTime > 0 && !tutorial.ScreensShown.Contains(TutorialScreen.Controls))
                {
                    tutorial.UpdateScreen(TutorialScreen.Controls);
                    gameFSM = GameState.Tutorial;
                }

                // Enemies (60 seconds "Night Time")
                if (GameManager.Get.PlayTime > 60000 && !tutorial.ScreensShown.Contains(TutorialScreen.Enemies))
                {
                    tutorial.UpdateScreen(TutorialScreen.Enemies);
                    gameFSM = GameState.Tutorial;
                }
            }

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
                        lastState = GameFSM;
                        gameFSM = GameState.Game;
                        GameManager.Get.NewGame();
                    }
                    if (buttons[0][1].IsPressed) { lastState = GameFSM; gameFSM = GameState.Settings; }
                    if (buttons[0][2].IsPressed) { lastState = GameFSM; gameFSM = GameState.Credits; }

                    
                    break;

                case GameState.Settings: // --- Settings ----------------------------------------//
                    foreach (Button button in buttons[1])
                    {
                        button.Update(gt);
                    }

                    // Return to previous screen
                    if (buttons[1][0].IsPressed) { gameFSM = lastState; lastState = GameState.Settings; }

                    // Toggle fullscreen
                    if (buttons[1][1].IsPressed)
                    {
                        if (graphicsDevice.IsFullScreen == false)
                        {
                            graphicsDevice.IsFullScreen = true;
                            buttons[1][1].Text = "True";
                        } else
                        {
                            graphicsDevice.IsFullScreen = false;
                            buttons[1][1].Text = "False";
                        }
                        graphicsDevice.ApplyChanges();
                    }

                    // Toggle for how to play menus to show up
                    if (buttons[1][2].IsPressed)
                    {
                        if (displayTutorial)
                        {
                            displayTutorial = false;
                            buttons[1][2].Text = "Off";
                        }
                        else
                        {
                            displayTutorial = true;
                            buttons[1][2].Text = "On";
                        }
                        graphicsDevice.ApplyChanges();
                    }

                    // Quit game if last state was game
                    if (buttons[1][3].IsPressed
                        && lastState == GameState.Game)
                    {
                        gameFSM = GameState.Menu;
                    }
                    break;

                case GameState.Credits: // --- Credits -------------------------------------------//
                    foreach (Button button in buttons[2])
                    {
                        button.Update(gt);
                    }
                    if (buttons[2][0].IsPressed) { gameFSM = lastState; lastState = GameState.Credits; }
                    break;

                case GameState.Game: // --- Game ------------------------------------------------//
                    foreach (Button button in buttons[3])
                    {
                        button.Update(gt);
                    }
                    break;

                case GameState.GameOver: // --- GameOver ----------------------------------------//
                    foreach (Button button in buttons[4])
                    {
                        button.Update(gt);
                    }
                    if (buttons[4][0].IsPressed) { lastState = gameFSM; gameFSM = GameState.Menu; }
                    break;

                case GameState.Tutorial: // --- Tutorial ----------------------------------------//
                    tutorial.Update(gt);

                    if (tutorial.IsPressed)
                    {
                        gameFSM = GameState.Game;

                        TriggerTutorial(TutorialScreen.CollectingResources);
                    }
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

                    // Draw the title shadow
                    sb.DrawString(vinque72,
                        "Undead Knights",
                        new Vector2(window.Width/12+2,window.Height/12+2),
                        Color.DarkRed);

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Undead Knights",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);

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
                        Color.White);

                    sb.DrawString(vinque48,
                        "Fullscreen",
                        new Vector2(window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    sb.DrawString(vinque48,
                        "Tutorial",
                        new Vector2(window.Width / 12, 6 * window.Height / 12),
                        Color.White);

                    // Draw all the buttons
                    for (int i = 0; i < 3; i++)
                    {
                        buttons[1][i].Draw(sb);
                    }
                    // Only draw quit game button if the last state was game
                    if (lastState == GameState.Game)
                    {
                        buttons[1][3].Draw(sb);
                    }
                    break;

                case GameState.Credits: // --- Credits -------------------------------------------//

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Credits",
                        new Vector2(window.Width / 12, window.Height / 12),
                        Color.White);

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

                    // Draw the title shadow
                    sb.DrawString(vinque72,
                        "Game Over",
                        new Vector2(3*window.Width / 12+3, 4*window.Height / 12+3),
                        Color.DarkRed);

                    // Draw the title
                    sb.DrawString(vinque72,
                        "Game Over",
                        new Vector2(3 * window.Width / 12, 4 * window.Height / 12),
                        Color.White);

                    // Draw all the buttons
                    foreach (Button button in buttons[4])
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Tutorial: // --- Tutorial ----------------------------------------//
                    tutorial.Draw(sb);
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
                "Return",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            buttons[1].Add(new Button(new Rectangle(6 * window.Width / 12, 4 * window.Height / 12,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "False",    // Text
                gd,         // Graphics Device
                vinque48)); // Font

            buttons[1].Add(new Button(new Rectangle(6 * window.Width / 12, 6 * window.Height / 12,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "True",    // Text
                gd,         // Graphics Device
                vinque48)); // Font

            buttons[1].Add(new Button(new Rectangle(5 * window.Width / 12, 21 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Quit To Menu",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            // --- Credits  ----------------------------------------//
            buttons[2].Add(new Button(new Rectangle(1 * window.Width / 12, 21 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Return",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

            // ---  GameOver ----------------------------------------//
            buttons[4].Add(new Button(new Rectangle(9 * window.Width / 24, 15 * window.Height / 24,   // X,Y
                3 * window.Width / 12, 1 * window.Height / 12),                                      // Width,Height
                "Return",     // Text
                gd,         // Graphics Device
                vinque48)); // Font

        }


        public void TriggerTutorial(TutorialScreen screen)
        {
            switch(screen)
            {
                case TutorialScreen.CollectingResources:
                    // Collecting Resources (After First Screen Goes Away)
                    if (!tutorial.ScreensShown.Contains(TutorialScreen.CollectingResources))
                    {
                        tutorial.UpdateScreen(TutorialScreen.CollectingResources);
                        gameFSM = GameState.Tutorial;
                    }
                    break;

                case TutorialScreen.Building:
                    // Building (After First Ground Clicked)
                    if (!tutorial.ScreensShown.Contains(TutorialScreen.Enemies))
                    {
                        tutorial.UpdateScreen(TutorialScreen.Enemies);
                        gameFSM = GameState.Tutorial;
                    }
                    break;

                case TutorialScreen.CreatingTroops:
                    // Creating Troops (After First Armory Built)
                    if (!tutorial.ScreensShown.Contains(TutorialScreen.Enemies))
                    {
                        tutorial.UpdateScreen(TutorialScreen.Enemies);
                        gameFSM = GameState.Tutorial;
                    }
                    break;
            }
        }
    }
}
