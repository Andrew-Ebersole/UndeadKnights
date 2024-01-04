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
using UndeadKnights.Humans;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 1-4-24
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
        private Texture2D wallSpriteSheet;
        private Texture2D gateSpriteSheet;
        private Texture2D turretSpriteSheet;
        private Random rng;

        // Buttons
        private List<Button> buttons;
        private List<Button> usedButtons;
        private double clickDelayTimer;

        // Upgraded tile
        private Point upgradedTile;

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
            wallSpriteSheet = content.Load<Texture2D>("wall");
            gateSpriteSheet = content.Load<Texture2D>("gate");
            turretSpriteSheet = content.Load<Texture2D>("turret");

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
                && usedButtons.Count > 0
                && clickDelayTimer > 100)
            {
                clickedOff = true;
            }
            // Update buttons position
            if (usedButtons.Count > 0)
            {
                UpdateUIButtonPosition();

                // Trigger Building Tutorial
                MenuUI.Get.TriggerTutorial(TutorialScreen.Building);
            }

            foreach (Button b in usedButtons)
            {
                b.Update(gt);
                if (b.IsPressed)
                {
                    clickedOff = false;
                }
            }
            if (clickedOff)
            {
                usedButtons.Clear();
            }
            else
            {

                // Build building if have enough resources
                foreach (Button b in usedButtons)
                {
                    if (b.IsPressed
                        && clickDelayTimer > 100)
                    {

                        // House
                        if (b == buttons[1] && GameManager.Get.Wood >= 2)
                        {
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Building(TileType.House, buildingSpriteSheet, 1);
                            GameManager.Get.Wood -= 2;
                            clickedOff = true;
                        }

                        // Farm
                        if (b == buttons[2] && HumanManager.Get.WorkingWorkers() + 1 < HumanManager.Get.TotalWorkers())
                        {
                            HumanManager.Get.RemoveUnusedHuman();
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Building(TileType.Farm, buildingSpriteSheet, 1);
                            clickedOff = true;
                        }

                        // Armory
                        if (b == buttons[3] && GameManager.Get.Stone >= 3)
                        {
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Building(TileType.Armory, buildingSpriteSheet, 1);
                            GameManager.Get.Stone -= 3;
                            clickedOff = true;

                            // Trigger Tutorial for troops
                            MenuUI.Get.TriggerTutorial(TutorialScreen.CreatingTroops);
                        }

                        // Archery
                        if (b == buttons[4] && GameManager.Get.Wood >= 3)
                        {
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Building(TileType.ShootingRange, buildingSpriteSheet, 1);
                            GameManager.Get.Wood -= 3;
                            clickedOff = true;
                        }

                        // Stable
                        if (b == buttons[5] && GameManager.Get.Wood >= 5)
                        {
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Building(TileType.Stable, buildingSpriteSheet, 1);
                            GameManager.Get.Wood -= 5;
                            clickedOff = true;
                        }

                        // Wall 
                        if (b == buttons[6] && GameManager.Get.Wood >= 1)
                        {
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Wall(TileType.Wall, wallSpriteSheet, 1);
                            GameManager.Get.Wood -= 1;
                            clickedOff = true;
                        }

                        // Gate
                        if (b == buttons[7] && GameManager.Get.Wood >= 2)
                        {
                            // If the gate is placed on the path save the data so if it gets deleted it will
                            // be a path instead of grass
                            if (tileGrid[upgradedTile.X,upgradedTile.Y].TileType == TileType.Path)
                            {
                                tileGrid[upgradedTile.X, upgradedTile.Y] = new Wall(TileType.Gate, gateSpriteSheet, 1);
                                tileGrid[upgradedTile.X, upgradedTile.Y].OnPath = true;
                            } else
                            {
                                tileGrid[upgradedTile.X, upgradedTile.Y] = new Wall(TileType.Gate, gateSpriteSheet, 1);
                            }
                            GameManager.Get.Wood -= 2;
                            clickedOff = true;
                        }

                        // Turret
                        if (b == buttons[8] && HumanManager.Get.WorkingWorkers() + 1 < HumanManager.Get.TotalWorkers())
                        {
                            HumanManager.Get.RemoveUnusedHuman();
                            tileGrid[upgradedTile.X, upgradedTile.Y] = new Wall(TileType.Turret, turretSpriteSheet, 1);
                            clickedOff = true;
                        }

                        // People
                        if (b == buttons[9] && GameManager.Get.Food >= 1)
                        {
                            Building newBuilding = (Building)tileGrid[upgradedTile.X, upgradedTile.Y];
                            if (HumanManager.Get.PeopleAtLocation
                                (new Point(upgradedTile.X,upgradedTile.Y)) < newBuilding.MaxPeople)
                            {
                                GameManager.Get.Food -= 1;
                                HumanManager.Get.AddPeople(new Point(upgradedTile.X,upgradedTile.Y));
                                clickedOff = true;
                            }
                        }

                    }
                }

                if (clickedOff == true)
                {
                    // hide UI
                    usedButtons.Clear();
                }
            }


            // Update all tiles
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    tileGrid[i, j].Update(gt, new Point(i, j));

                    // Check when tile is left clicked
                    if (tileGrid[i, j].IsPressed
                        && usedButtons.Count == 0
                        && !clickedOff)
                    {
                        // Reset click delay
                        clickDelayTimer = 0;

                        // Check what type of tile was clicked
                        switch (tileGrid[i, j].TileType)
                        {
                            case TileType.Grass:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.Farm, TileType.House, TileType.Armory, TileType.Wall},
                                    new Point(i, j));
                                break;

                            case TileType.Path:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.Gate},
                                    new Point(i, j));
                                break;

                            case TileType.Tree:
                                if (HumanManager.Get.TotalWorkers() > HumanManager.Get.WorkingWorkers())
                                {
                                    HumanManager.Get.DestoryResource(new Point(i, j));
                                }
                                break;

                            case TileType.Rock:
                                if (HumanManager.Get.TotalWorkers() > HumanManager.Get.WorkingWorkers())
                                {
                                    HumanManager.Get.DestoryResource(new Point(i, j));
                                }
                                break;

                            case TileType.FarmFull:
                                if (HumanManager.Get.TotalWorkers() > HumanManager.Get.WorkingWorkers())
                                {
                                    HumanManager.Get.DestoryResource(new Point(i, j));
                                }
                                break;

                            case TileType.Armory:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.ShootingRange, TileType.Stable},
                                    new Point(i, j));

                                // Update people text
                                UpdatePeopleText(tileGrid[i, j]);
                                break;

                            case TileType.ShootingRange:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {},
                                    new Point(i, j));

                                // Update people text
                                UpdatePeopleText(tileGrid[i, j]);
                                break;

                            case TileType.Stable:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {},
                                    new Point(i, j));

                                // Update people text
                                UpdatePeopleText(tileGrid[i, j]);
                                break;

                            case TileType.House:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {},
                                    new Point(i, j));

                                // Update people text
                                UpdatePeopleText(tileGrid[i, j]);
                                break;

                            case TileType.Wall:
                                UpgradeOptions(tileGrid[i, j], new List<TileType>() {
                                    TileType.Gate,TileType.Turret },
                                    new Point(i, j));
                                break;

                        }

                        
                    }

                    // If right clicked delete tile as long as its not
                    // one of the tiles that cant be deleted
                    if (tileGrid[i, j].IsRightClicked
                        && tileGrid[i, j].TileType != TileType.Tree
                        && tileGrid[i, j].TileType != TileType.Rock
                        && tileGrid[i, j].TileType != TileType.Path
                        && tileGrid[i, j].TileType != TileType.TownHall)
                    {
                        // Refund materials
                        switch (tileGrid[i, j].TileType)
                        {
                            case TileType.Wall:
                                GameManager.Get.Wood += 1;
                                break;

                            case TileType.Gate:
                                GameManager.Get.Wood += 2;
                                break;

                            case TileType.Turret:
                                GameManager.Get.Wood += 1;
                                GameManager.Get.Food += 1;
                                break;

                            case TileType.House:
                                GameManager.Get.Wood += 2;
                                HumanManager.Get.RemoveHouse(new Point(i, j));
                                break;

                            case TileType.Armory:
                                GameManager.Get.Stone += 3;
                                break;

                            case TileType.ShootingRange:
                                GameManager.Get.Stone += 3;
                                GameManager.Get.Wood += 3;
                                break;

                            case TileType.Stable:
                                GameManager.Get.Stone += 3;
                                GameManager.Get.Wood += 5;
                                break;
                        }

                        // Return tile to default tile
                        if (tileGrid[i, j].OnPath)
                        {
                            tileGrid[i, j] = new Tile(TileType.Path, environmentSpriteSheet);
                        } else
                        {
                            tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                        }
                    }

                    // If tile health = 0 return to default

                    if (tileGrid[i,j] is Building)
                    {
                        Building building = (Building)tileGrid[i, j];

                        if (building.Health <= 0)
                        {
                            // Return tile to default tile
                            if (building.OnPath)
                            {
                                tileGrid[i, j] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                            else
                            {
                                tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                            }
                        }
                    }
                }
            }

            // Update the timer
            clickDelayTimer += gt.ElapsedGameTime.Milliseconds;

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
                    tileGrid[i, j].Draw(sb, new Point(i, j));
                    if (i == upgradedTile.X && j == upgradedTile.Y
                        && usedButtons.Count > 0)
                    {
                        sb.Draw(GameManager.Get.SingleColor,
                            new Rectangle(i * GameManager.Get.TileSize - GameManager.Get.Camera.X * GameManager.Get.TileSize / 25,
                            j * GameManager.Get.TileSize - GameManager.Get.Camera.Y * GameManager.Get.TileSize / 25,
                            GameManager.Get.TileSize, GameManager.Get.TileSize),
                            Color.White * 0.5f);
                    }
                }
            }


            // draw UI buttons
            foreach (Button b in usedButtons)
            {
                b.Draw(sb);
            }
        }

        #region Map Generation

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


            // Create path
            GeneratePath();

            // Set area around th to grass
            for (int i = 23; i < 27; i++)
            {
                for (int j = 13; j < 17; j++)
                {
                    if (tileGrid[i, j].TileType != TileType.Path)
                    {
                        tileGrid[i, j] = new Tile(TileType.Grass, environmentSpriteSheet);
                    }
                }
            }

            // Set middle to town hall
            tileGrid[25, 15] = new Building(TileType.TownHall, buildingSpriteSheet, 1);

            // Create a house and farm
            Point currentPos = new Point(25,15);

            // Place the house in a random location
            while (currentPos.X >= 0 && currentPos.Y >= 0
                && currentPos.Y < 51 && currentPos.Y < 51)
            {
                currentPos = new Point(25, 15);
                currentPos += new Point(rng.Next(-2, 3), rng.Next(-2, 3));
                if (tileGrid[currentPos.X,currentPos.Y].TileType == TileType.Grass)
                {
                    tileGrid[currentPos.X, currentPos.Y] = new Building(TileType.House, buildingSpriteSheet, 1);
                    HumanManager.Get.AddPeople(currentPos);
                    break;
                }
            }

            // Place a farm
            currentPos = new Point(25,15);
            while (currentPos.X >= 0 && currentPos.Y >= 0
                && currentPos.Y < 51 && currentPos.Y < 51)
            {
                currentPos = new Point(25, 15);
                currentPos += new Point(rng.Next(-2, 3), rng.Next(-2, 3));
                if (tileGrid[currentPos.X, currentPos.Y].TileType == TileType.Grass)
                {
                    tileGrid[currentPos.X, currentPos.Y] = new Building(TileType.FarmFull, buildingSpriteSheet, 1);
                    break;
                }
            }

        }

        /// <summary>
        /// Generates the path that the enemies will come from
        /// </summary>
        private void GeneratePath()
        {
            FillPath(new Point(25, 15), new Point(0, rng.Next(0, 51)));
            FillPath(new Point(25, 15), new Point(rng.Next(0, 51), 50));
            FillPath(new Point(25, 15), new Point(50, rng.Next(0, 51)));

            FillGrass(20);
        }

        /// <summary>
        /// Generates a path between two points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void FillPath(Point start, Point end)
        {
            if (tileGrid[end.X, end.Y].TileType != TileType.Path)
            {

                // Whichever direction is further is more likely to be picked
                if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
                {
                    // Move toward X a random amount of times
                    if (start.X < end.X)
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.X - end.X), 2) / 4) + 1); i++)
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
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.X - end.X), 2) / 4) + 1); i++)
                        {
                            if (start.X > 0 && start.X < 50)
                            {
                                start.X--;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                }
                else
                {
                    // Move toward end in Y direction a random amount of times
                    if (start.Y < end.Y)
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.Y - end.Y), 2) / 4) + 1); i++)
                        {
                            if (start.Y > 0 && start.Y < 50)
                            {
                                start.Y++;
                                tileGrid[start.X, start.Y] = new Tile(TileType.Path, environmentSpriteSheet);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < rng.Next(1, Math.Abs((int)Math.Pow((start.Y - end.Y), 2) / 4) + 1); i++)
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
        public void FillGrass(int GrassPercentLeft)
        {
            // Loop for every tile
            for (int x = 0; x < 51; x++)
            {
                for (int y = 0; y < 51; y++)
                {
                    // Check if its grass
                    if (tileGrid[x, y].TileType == TileType.Grass)
                    {
                        // If it is do rng to change to tree or rock
                        if (rng.Next(0, 100) > GrassPercentLeft)
                        {
                            // Determine if its will be a rock or tree
                            if (rng.Next(0, 100) > 25)
                            {
                                // If the tree is near a path dont spawn (with randomization)
                                if (rng.Next(1, 3) < NearestPath(x, y))
                                {
                                    tileGrid[x, y] = new Tile(TileType.Tree, environmentSpriteSheet);
                                }
                            }
                            else
                            {
                                // If the rock is near a path dont spawn (with randomization)
                                if (rng.Next(3, 6) < NearestPath(x, y))
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
                    x -= 1;
                    y += 1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // down to left
                while (x > -distance)
                {
                    x -= 1;
                    y -= 1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // Left to up
                while (y > -distance)
                {
                    x += 1;
                    y -= 1;
                    if (IsPath(startingX + x, startingY + y))
                    {
                        return distance;
                    }
                }
                // up to right
                while (x < distance)
                {
                    x += 1;
                    y += 1;
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
                && x < 51 && y < 51)
            {
                if (tileGrid[x, y].TileType != TileType.Grass
                    && tileGrid[x, y].TileType != TileType.Tree
                    && tileGrid[x, y].TileType != TileType.Rock)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region UI buttons
        /// <summary>
        /// Creates the UI menu that displays different upgrade options
        /// </summary>
        /// <param name="clickedTile"> The tile that is being upgraded</param>
        /// <param name="tileTypes"> The different types of tiles that it can be turned into </param>
        private void UpgradeOptions(Tile clickedTile, List<TileType> tileTypes, Point tilePosition)
        {
            upgradedTile = tilePosition;
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

            }

            // If the clicked tile contains people add the add people buttons
            if (clickedTile.TileType == TileType.House
                || clickedTile.TileType == TileType.Armory
                || clickedTile.TileType == TileType.ShootingRange
                || clickedTile.TileType == TileType.Stable)
            {
                usedButtons.Add(buttons[9]);
            }


            // Moves the UI to under 
            UpdateUIButtonPosition();

        }

        /// <summary>
        /// Moves the UI buttons to under the upgrade tile
        /// If it is offscreen or overlapping the upgrade tile
        /// it will move somewhere else on screen
        /// </summary>
        private void UpdateUIButtonPosition()
        {
            // Important numbers
            int tileSize = GameManager.Get.TileSize;
            Point camera = GameManager.Get.Camera;

            // Create the background area
            Rectangle backgroundArea = new Rectangle(tileSize + ((upgradedTile.X * tileSize) - camera.X * tileSize / 25) - (usedButtons.Count - 1) * 75 - 25,
                tileSize + (upgradedTile.Y * tileSize) - camera.Y * tileSize / 25,
                150 * (usedButtons.Count - 1) + 50, 200);

            // Check for offscreen
            if (backgroundArea.X < 0)
            {
                backgroundArea.X = 0;
            }
            if (backgroundArea.Y < 0)
            {
                backgroundArea.Y = 0;
            }
            if (backgroundArea.X + backgroundArea.Width > 1920)
            {
                backgroundArea.X = 1920 - backgroundArea.Width;
            }
            if (backgroundArea.Y + backgroundArea.Height > 1080)
            {
                backgroundArea.Y = 1080 - backgroundArea.Height;
            }

            // Create the buttons
            usedButtons[0].PositionRectangle = backgroundArea;
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
                "House \n\n2 wood", gd, vinque24));

            // Place Farm
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Farm \n\n1 person", gd, vinque24));

            // Place Armory
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Armory \n\n3 stone", gd, vinque24));

            // Place Shooting Range
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Archery \n\n3 wood", gd, vinque24));

            // Place Stable
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Stable \n\n5 wood", gd, vinque24));

            // Place Wall
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Wall \n\n1 wood", gd, vinque24));

            // Place Gate
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Gate \n\n2 wood", gd, vinque24));

            // Place Turret
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "Turret \n\n1 person", gd, vinque24));

            // Add People
            buttons.Add(new Button(new Rectangle(-1000, -1000, 150, 150),
                "People \n\n1 food", gd, vinque24));
        }

        #endregion

        /// <summary>
        /// Updates the text displaying amount of people to
        /// the amount of people in given tile
        /// </summary>
        /// <param name="tile"></param>
        private void UpdatePeopleText(Tile tile)
        {
            if (tile is Building)
            {
                Building tileToUpgrade = (Building)tile;
                buttons[9].Text = $"People\n\nFood 1\n" +
                    $"({HumanManager.Get.PeopleAtLocation(new Point(upgradedTile.X, upgradedTile.Y))}" +
                    $"/{tileToUpgrade.MaxPeople})";
            }
        }

        /// <summary>
        /// Resets mined resources to the original state
        /// </summary>
        /// <param name="tile"></param>
        public void TileToGrass(Point tile)
        {
            switch (tileGrid[tile.X, tile.Y].TileType)
            {
                case TileType.Tree:
                    tileGrid[tile.X, tile.Y] = new Tile(TileType.Grass, environmentSpriteSheet);
                    GameManager.Get.Wood += 1;
                    break;

                case TileType.Rock:
                    tileGrid[tile.X, tile.Y] = new Tile(TileType.Grass, environmentSpriteSheet);
                    GameManager.Get.Stone += 1;
                    break;

                case TileType.FarmFull:
                    tileGrid[tile.X, tile.Y] = new Building(TileType.Farm, buildingSpriteSheet, 1);
                    GameManager.Get.Food += 1;
                    break;
            }
        }

        public void GrowFarms()
        {
            for (int i = 0; i < 51; i++)
            {
                for (int j = 0; j < 51; j++)
                {
                    // Grow farm
                    if (tileGrid[i, j].TileType == TileType.Farm)
                    {
                        tileGrid[i, j] = new Building
                                (TileType.FarmFull, buildingSpriteSheet, 1);
                    }
                }
            }
        }


    }
}
