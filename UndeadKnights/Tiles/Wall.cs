using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-31-23
// Last Update   | 8-1-23
// Purpose       | Wall based buildings
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    internal class Wall : Tile
    {
        // --- Fields --- //

        private int health;
        private int level;
        private double timer;



        // --- Properties --- //

        public int Health { get { return health; } }

        public int Level { get { return level; } }

        public double Timer { get { return timer; } }

        // --- Constructor --- //

        public Wall(TileType tiletype, Texture2D texture, int level)
            : base(tiletype, texture)
        {
            this.level = level;
            timer = 0;

            switch (tileType)
            {
                case TileType.Wall:
                    health = 75;
                    break;

                case TileType.Gate:
                    health = 50;
                    break;

                case TileType.Turret:
                    health = 100;
                    break;
            }

            // spriteLocation will be updated each frame to check for nearby walls
            this.spriteLocation = new Point(0, 0);
        }



        // --- Methods --- //

        public override void Update(GameTime gt, Point location)
        {
            base.Update(gt, location);
            connectToOtherWalls(location);
        }

        /// <summary>
        /// Check all the nearby walls for locations to attach to
        /// </summary>
        private void connectToOtherWalls(Point location)
        {
            Tile[,] tilegrid = TileManager.Get.TileGrid;

            bool top = false;
            bool bottom = false;
            bool left = false;
            bool right = false;

            // Check if the surrounding walls are also walls
            if (tilegrid[location.X, location.Y - 1] != null
                && IsWallType(tilegrid[location.X,location.Y - 1]))
            { top = true; }
            if (tilegrid[location.X, location.Y + 1] != null
                && IsWallType(tilegrid[location.X, location.Y + 1]))
            { bottom = true; }
            if (tilegrid[location.X - 1, location.Y] != null
                && IsWallType(tilegrid[location.X - 1, location.Y]))
            { left = true; }
            if (tilegrid[location.X + 1, location.Y] != null
                && IsWallType(tilegrid[location.X + 1, location.Y]))
            { right = true; }

            // choose the appropriate texture based off of the surrounding walls
            if (top){ this.spriteLocation = new Point(3, 2); }
            if (bottom) { this.spriteLocation = new Point(1, 2); }
            if (left) { this.spriteLocation = new Point(0, 2); }
            if (right) { this.spriteLocation = new Point(2, 2); }
            if (top && bottom) { this.spriteLocation = new Point(1, 0); }
            if (left && right) { this.spriteLocation = new Point(2, 0); }
            if (top && left) { this.spriteLocation = new Point(0, 1); }
            if (top && right) { this.spriteLocation = new Point(3, 1); }
            if (bottom && left) { this.spriteLocation = new Point(1, 1); }
            if (bottom && right) { this.spriteLocation = new Point(2, 1); }
            if (top && bottom && left) { this.spriteLocation = new Point(1, 3); }
            if (top && bottom && right) { this.spriteLocation = new Point(3, 3); }
            if (top && left && right) { this.spriteLocation = new Point(0, 3); }
            if (bottom && left && right) { this.spriteLocation = new Point(2, 3); }
            if (top && bottom && left && right) { this.spriteLocation = new Point(3, 0); }
        }


        /// <summary>
        /// Determines if the given tile is a type of wall
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        private bool IsWallType(Tile tile)
        {
            if (tile.TileType == TileType.Wall
                || tile.TileType == TileType.Gate
                || tile.TileType == TileType.Turret)
            {
                return true;
            }

            return false;
        }




    }
}
