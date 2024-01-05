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
// Last Update   | 8-15-23
// Purpose       | Tiles that have health and can be destroyed
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    internal class Building : Tile
    {
        // --- Fields --- //

        private int maxHealth;
        private int health;
        private int level;
        private int people;
        private int maxPeople;
        private double timer;
        


        // --- Properties --- //

        public int Health { get { return health; } }

        public int Level { get { return level; } }

        public int People { get { return people; } set { people = value; } }

        public int MaxPeople { get { return maxPeople; } }

        public double Timer { get { return timer; } }

        // --- Constructor --- //

        public Building(TileType tiletype, Texture2D texture, int level) 
            : base(tiletype, texture)
        {
            this.level = level;
            timer = 0;
            people = 0;

            switch (tileType)
            {
                case TileType.Farm:
                    this.spriteLocation = new Point(1, 0);
                    maxHealth = 10;
                    break;
                case TileType.FarmFull:
                    this.spriteLocation = new Point(2, 0);
                    maxHealth = 10;
                    break;
                case TileType.TownHall:
                    this.spriteLocation = new Point(3, 0);
                    maxHealth = 200;
                    break;
                case TileType.House:
                    this.spriteLocation = new Point(0, 1);
                    maxPeople = 2;
                    maxHealth = 50;
                    break;
                case TileType.Armory:
                    this.spriteLocation = new Point(1, 1);
                    maxHealth = 100;
                    maxPeople = 2;
                    break;
                case TileType.ShootingRange:
                    this.spriteLocation = new Point(2, 1);
                    maxHealth = 50;
                    maxPeople = 2;
                    break;
                case TileType.Stable:
                    this.spriteLocation = new Point(3, 1);
                    maxHealth = 50;
                    maxPeople = 1;
                    break;
            }
            health = maxHealth;
        }



        // --- Methods --- //

        /// <summary>
        /// Updates building every frame
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="location"></param>
        public override void Update(GameTime gt, Point location)
        {
            base.Update(gt,location);
            timer += gt.ElapsedGameTime.Milliseconds;

            if (!GameManager.Get.IsNight)
            {
                health = maxHealth;
            }
        }

        /// <summary>
        /// Draws the building
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="location"></param>
        public override void Draw(SpriteBatch sb, Point location)
        {
            // draws the tile
            base.Draw(sb, location);

            // draw text displaying health (for debug)
            int tileSize = GameManager.Get.TileSize;
            Point camera = GameManager.Get.Camera;
            //sb.DrawString(GameManager.Get.Vinque24,
            //    $"{health}",
            //    new Vector2(location.X * tileSize - camera.X * tileSize / 25,
            //    location.Y * tileSize - camera.Y * tileSize / 25),
            //    Color.White);
        }

        /// <summary>
        /// Removes inputted amount of damage from the buildings health
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            health -= damage;
        }



    }
}
