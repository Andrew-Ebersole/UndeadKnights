using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-29-23
// Last Update   | 7-30-23
// Purpose       | Tiles that have health and can be destroyed
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    internal class Building : Tile
    {
        // --- Fields --- //

        private int health;
        private int level;
        private int people;
        private int maxPeople;
        


        // --- Properties --- //

        public int Health { get { return health; } }

        public int Level { get { return level; } }

        public int People { get { return people; } }

        public int MaxPeople { get { return maxPeople; } }



        // --- Constructor --- //

        public Building(TileType tiletype, Texture2D texture, int health, int level) 
            : base(tiletype, texture)
        {
            this.health = health;
            this.level = level;

            switch (tileType)
            {
                case TileType.Farm:
                    this.spriteLocation = new Point(1, 0);
                    break;
                case TileType.TownHall:
                    this.spriteLocation = new Point(3, 0);
                    break;
                case TileType.House:
                    this.spriteLocation = new Point(0, 1);
                    break;
                case TileType.Armory:
                    this.spriteLocation = new Point(1, 1);
                    break;
                case TileType.ShootingRange:
                    this.spriteLocation = new Point(2, 1);
                    break;
                case TileType.Stable:
                    this.spriteLocation = new Point(3, 1);
                    break;
            }
        }



        // --- Methods --- //






    }
}
