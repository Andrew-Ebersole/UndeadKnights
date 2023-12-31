﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-15-23
// Purpose       | Makes up a multitude of different tiles
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    internal class Tile
    {
        // --- Fields --- //

        protected Texture2D texture;
        protected Point spriteLocation;
        protected TileType tileType;
        protected MouseState currentMS;
        protected MouseState previousMS;
        protected bool isPressed;
        protected bool isHovered;
        protected bool isRightClicked;
        protected int tileSize;
        protected Rectangle tilePos;
        protected double hoverTimer;
        protected bool onPath;
        // --- Properties --- //

        public TileType TileType { get { return tileType; } }

        /// <summary>
        /// Returns true on the first frame the tile is pressed
        /// </summary>
        public bool IsPressed { get { return isPressed; } }

        /// <summary>
        /// Returns true if the mouse overlaps the tile
        /// </summary>
        public bool IsHovered { get { return isHovered; } }

        /// <summary>
        /// returns true the first frame the tile is right clicked
        /// </summary>
        public bool IsRightClicked { get { return isRightClicked; } }

        /// <summary>
        /// stores if the tile was placed on a path or not
        /// </summary>
        public bool OnPath { get { return onPath; } set { onPath = value; } }


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

            // Mouse States
            currentMS = new MouseState();
            previousMS = Mouse.GetState();
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public virtual void Update(GameTime gt, Point location)
        {
            // Get the current mouse state
            currentMS = Mouse.GetState();

            // Find the current tile size
            int tileSize = GameManager.Get.TileSize;


            // Find the tiles position on the screen
            tilePos = new Rectangle(tileSize * location.X - GameManager.Get.Camera.X * tileSize / 25
                , tileSize * location.Y - GameManager.Get.Camera.Y * tileSize / 25
                , tileSize, tileSize);


            // Check if the mouse is overlapping the button
            if (new Rectangle(currentMS.X, currentMS.Y, 0, 0).Intersects(tilePos))
            {

                // Set is hover to true
                isHovered = true;
                hoverTimer += gt.ElapsedGameTime.TotalMilliseconds;

                // Check if this is the first frame left click has been held down
                if (currentMS.LeftButton == ButtonState.Pressed
                && previousMS.LeftButton == ButtonState.Released)
                {
                    // isPressed = true when the button is left clicked
                    isPressed = true;
                }
                else
                {
                    isPressed = false;
                }

                // Check for right click
                if (currentMS.RightButton == ButtonState.Pressed
                && previousMS.RightButton == ButtonState.Released)
                {
                    // isPressed = true when the button is right clicked
                    isRightClicked = true;
                }
                else
                {
                    isRightClicked = false;
                }
            }
            else
            {
                isHovered = false;
                isPressed = false;
                isRightClicked = false;
                hoverTimer = 0;
            }

            // Set the previous mouse state to the last mouse state
            // This should remain at the end of the update method
            previousMS = currentMS;
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb, Point location)
        {
            // Find the current tile size
            int tileSize = GameManager.Get.TileSize;

            // Find the locaiton in the grid
            tilePos = new Rectangle(tileSize * location.X - GameManager.Get.Camera.X * tileSize / 25
               , tileSize * location.Y - GameManager.Get.Camera.Y * tileSize / 25
               , tileSize, tileSize);

            // Draw tile
            sb.Draw(texture,
                tilePos,
                new Rectangle(spriteLocation.X * 512, spriteLocation.Y * 512, 512, 512),
                Color.White);
        }
    }
}
