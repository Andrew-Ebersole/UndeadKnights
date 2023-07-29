using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 7-29-23
// Purpose       | Makes up a multitude of different tiles
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    internal class Tile
    {
        // --- Fields --- //

        private Texture2D texture;
        private Point spriteLocation;
        private TileType tileType;


        // --- Properties --- //

        public TileType TileType { get { return tileType; } }



        // --- Constructor --- //

        public Tile(TileType tileType, Texture2D texture)
        {
            this.texture = texture;
            this.tileType = tileType;

            switch (tileType)
            {
                case TileType.Grass:
                    this.spriteLocation = new Point(1, 0);
                    break;
                case TileType.Path:
                    this.spriteLocation = new Point(0, 0);
                    break;
                case TileType.Tree:
                    this.spriteLocation = new Point(0, 1);
                    break;
                case TileType.Rock:
                    this.spriteLocation = new Point(1,1);
                    break;
            }
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {

        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb, Point location)
        {
            int tileSize = GameManager.Get.TileSize;

            sb.Draw(texture,
                new Rectangle(tileSize * location.X - GameManager.Get.Camera.X*tileSize/25
                , tileSize * location.Y - GameManager.Get.Camera.Y*tileSize/25
                , tileSize, tileSize),
                new Rectangle(spriteLocation.X*512,spriteLocation.Y*512,512,512),
                Color.White);
        }
    }
}
