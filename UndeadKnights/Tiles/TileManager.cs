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
using System.IO;
using Microsoft.Xna.Framework.Input;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-31-23
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
        FarmFull,
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
        private Texture2D buildingSpriteSheet;
        private Random rng;

        // Buttons
        private List<Button> buttons;
        private List<Button> usedButtons;

        // Upgraded tile
        private Tile upgradedTile;

        // Fonts
        private SpriteFont vinque24;

        // Mouse State
        private MouseState currentMS;
        private MouseState previousMS;


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
            buildingSpriteSheet = content.Load<Texture2D>("Buildings");
            // Create randomifier
            rng = new Random();

            vinque24 = content.Load<SpriteFont>("vinque-24");

            // Generate map
            NewMap();

            // Generate buttons
            GenerateButtons(gd);

            // Mouse state 
            currentMS = Mouse.GetState();
            previousMS = Mouse.GetState();

            usedButtons = new List<Button>();
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            currentMS = Mouse.GetState();


            // Update Buttons
            // If they clicked off delete ui buttons
            // This should remain above the updating of tiles
            bool clickedOff = false;
            if (currentMS.LeftButton == ButtonState.Pressed
                && previousMS.LeftButton == ButtonState.Released
                && usedButtons.Count > 0)
            {
                clickedOff = true;
            }
            foreach (Button b in usedButtons)
            {
                b.Update(gt);
                if (b.IsPressed)
                {
                    clickedOff = false;
                    System.Diagnostics.Debug.WriteLine(b.IsPressed);
                }
            }
            if (clickedOff)
            {
                usedButtons.Clear();
            }


            // Update all tiles
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j].Update(gt, new Point(i, j));
                    
                    // Check when tile is clicked
                    if (tileGrid[i, j].IsPressed
                        && usedButtons.Count == 0
                        && !clickedOff)
                    {
                        switch (tileGrid[i, j].TileType)
                        {
                            case TileType.Grass:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.Farm, TileType.House, TileType.Armory, TileType.Wall});
                                break;

                            case TileType.Path:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.Gate});
                                break;

                            case TileType.Tree:
                                tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                                GameManager.Get.Wood += 1;
                                break;

                            case TileType.Rock:
                                tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                                GameManager.Get.Stone += 1;
                                break;

                            case TileType.House:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {});
                                break;

                            case TileType.FarmFull:
                                tileGrid[i, j] = new Building(TileType.Farm, buildingSpriteSheet,10,1);
                                GameManager.Get.Food += 1;
                                break;

                            case TileType.Armory:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.ShootingRange, TileType.Stable});
                                break;

                            case TileType.Wall:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.Gate,TileType.Turret });
                                break;

                            default:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>(){ });
                                break;

                        }
                    }

                    // Grow farm
                    if (tileGrid[i,j].TileType == TileType.Farm)
                    {
                        // If the farm has been around for 30 seconds it will grow into farm full
                        Building newbuilding = (Building)tileGrid[i,j];
                        if (newbuilding.Timer > 30000)
                        {
                            tileGrid[i, j] = new Building
                                (TileType.FarmFull,buildingSpriteSheet,10,newbuilding.Level);
                        }
                    }
                }
            }

            

            // This should remain at end of update
            previousMS = currentMS;

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
                    tileGrid[i, j].Draw(sb, new Point(i,j));
                }
            }

            // draw UI buttons
            foreach (Button b in usedButtons)
            {
                b.Draw(sb);
            }
        }

        /// <summary>
        /// Creates a new map with paths and trees and stuff
        /// </summary>
        public void NewMap()
        {
            // Set all to grass
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                }
            }

            // (right now its path because i dont have a town hall
            GeneratePath();

            // Set middle to town hall
            tileGrid[25, 25] = new Building(TileType.TownHall, buildingSpriteSheet, 200, 1);
        }

        /// <summary>
        /// Generates the path that the enemies will come from
        /// </summary>
        private void GeneratePath()
        {
            FillPath(new Point(25,25),new Point(0,rng.Next(0,51)));
            FillPath(new Point(25, 25), new Point(rng.Next(0, 51), 50));
            FillPath(new Point(25, 25), new Point(50, rng.Next(0, 51)));

            FillGrass(20);
        }

        /// <summary>
        /// Generates a path between two points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void FillPath(Point start, Point end)
        {
            if (tileGrid[end.X,end.Y].TileType != TileType.Path)
            {

                // Start by whichever direction is farther
                if (Math.Abs(start.X - end.X) >Math.Abs(start.Y - end.Y))
                {
                    // Move toward X a random amount of times
                    if (start.X < end.X)
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.X - end.X), 2) / 4)+1); i++)
                        {
                            if (start.X > 0 && start.X < 50)
                            {
                                start.X++;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.X - end.X), 2) / 4)+1); i++)
                        {
                            if (start.X > 0 && start.X < 50)
                            {
                                start.X--;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                } else
                {
                    // Move toward end in Y direction a random amount of times
                    if (start.Y < end.Y)
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.Y - end.Y), 2) / 4)+1); i++)
                        {
                            if (start.Y > 0 && start.Y < 50)
                            {
                                start.Y++;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    } else
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.Y - end.Y), 2) / 4)+1); i++)
                        {
                            if (start.Y > 0 && start.Y < 50)
                            {
                                start.Y--;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                }

                // Reccurs until at the end
                FillPath(start, end);
            }
        }

        /// <summary>
        /// Replaces grass with other resources
        /// </summary>
        /// <param name="GrassPercentLeft"> Percent of tiles that will remain grass</param>
        private void FillGrass(int GrassPercentLeft)
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
                                if (rng.Next(1,3) < NearestPath(x, y))
                                {
                                    tileGrid[x, y] = new Tile(TileType.Tree, environmentSpriteSheet);
                                }
                            }
                            else
                            {
                                // If the rock is near a path dont spawn (with randomization)
                                if (rng.Next(3,8) < NearestPath(x, y))
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
        private int NearestPath(int startingX, int startingY)
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
        private bool IsPath(int x, int y)
        {
            if (x >= 0 && y >= 0
                && x <51 && y < 51)
            {
                if (tileGrid[x, y].TileType != TileType.Grass
                    && tileGrid[x,y].TileType != TileType.Tree
                    && tileGrid[x,y].TileType != TileType.Rock)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates the UI menu that displays different upgrade options
        /// </summary>
        /// <param name="clickedTile"> The tile that is being upgraded</param>
        /// <param name="tileTypes"> The different types of tiles that it can be turned into </param>
        private void UpgradeOptions(Tile clickedTile, List<TileType> tileTypes)
        {
            upgradedTile = clickedTile;
            usedButtons.Clear();
            usedButtons.Add(buttons[0]);

            // Add the buttons that are requested
            foreach (TileType t in tileTypes)
            {
                switch (t)
                {
                    case TileType.House:
                        usedButtons.Add(buttons[1]);
                        break;
                    case TileType.Farm:
                        usedButtons.Add(buttons[2]);
                        break;
                    case TileType.Armory:
                        usedButtons.Add(buttons[3]);
                        break;
                    case TileType.ShootingRange:
                        usedButtons.Add(buttons[4]);
                        break;
                    case TileType.Stable:
                        usedButtons.Add(buttons[5]);
                        break;
                    case TileType.Wall:
                        usedButtons.Add(buttons[6]);
                        break;
                    case TileType.Gate:
                        usedButtons.Add(buttons[7]);
                        break;
                    case TileType.Turret:
                        usedButtons.Add(buttons[8]);
                        break;
                }

                // If the clicked tile contains people add the add people buttons
                if (clickedTile.TileType == TileType.House
                    || clickedTile.TileType == TileType.Armory
                    || clickedTile.TileType == TileType.ShootingRange
                    || clickedTile.TileType == TileType.Stable)
                {
                    usedButtons.Add(buttons[9]);
                }
            }

            // Moves the UI to the middle of screen
            usedButtons[0].PositionRectangle = new Rectangle(960-tileTypes.Count*75-25, 440, 150 * tileTypes.Count + 50, 200);

            for (int i = 1; i < usedButtons.Count; i++) 
            {
                usedButtons[i].PositionRectangle =
                    new Rectangle(usedButtons[0].PositionRectangle.X + 25 + 150 * (i - 1),
                    usedButtons[0].PositionRectangle.Y + 25, 150, 150);
            }
        }

        private void GenerateButtons(GraphicsDevice gd)
        {
            buttons = new List<Button>();

            // Background
            buttons.Add(new Button(new Rectangle(-1000, -1000, 400, 200)
                , "", gd, vinque24));

            // Place house
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "House", gd, vinque24));

            // Place Farm
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Farm", gd, vinque24));

            // Place Armory
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Armory", gd, vinque24));

            // Place Shooting Range
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Archery", gd, vinque24));

            // Place Stable
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Stable", gd, vinque24));

            // Place Wall
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Wall", gd, vinque24));

            // Place Gate
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Gate", gd, vinque24));

            // Place Turret
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Turret", gd, vinque24));

            // Add People
            buttons.Add(new Button(new Rectangle(-1000, -1000, 100, 50),
                "People", gd, vinque24));
        }
    }
}
