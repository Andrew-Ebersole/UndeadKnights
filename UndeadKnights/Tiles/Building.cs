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
        private double timer;
        


        // --- Properties --- //

        public int Health { get { return health; } }

        public int Level { get { return level; } }

        public int People { get { return people; } }

        public int MaxPeople { get { return maxPeople; } }

        public double Timer { get { return timer; } }

        // --- Constructor --- //

        public Building(TileType tiletype, Texture2D texture, int level) 
            : base(tiletype, texture)
        {
            this.level = level;
            timer = 0;

            switch (tileType)
            {
                case TileType.Farm:
                    this.spriteLocation = new Point(1, 0);
                    health = 10;
                    break;
                case TileType.FarmFull:
                    this.spriteLocation = new Point(2, 0);
                    health = 10;
                    break;
                case TileType.TownHall:
                    this.spriteLocation = new Point(3, 0);
                    health = 200;
                    break;
                case TileType.House:
                    this.spriteLocation = new Point(0, 1);
                    maxPeople = 2;
                    health = 50;
                    break;
                case TileType.Armory:
                    this.spriteLocation = new Point(1, 1);
                    health = 100;
                    maxPeople = 2;
                    break;
                case TileType.ShootingRange:
                    this.spriteLocation = new Point(2, 1);
                    health = 50;
                    maxPeople = 2;
                    break;
                case TileType.Stable:
                    this.spriteLocation = new Point(3, 1);
                    health = 50;
                    maxPeople = 1;
                    break;
            }
        }



        // --- Methods --- //

        public override void Update(GameTime gt, Point location)
        {
            base.Update(gt,location);
            timer += gt.ElapsedGameTime.Milliseconds;
        }




    }
}
