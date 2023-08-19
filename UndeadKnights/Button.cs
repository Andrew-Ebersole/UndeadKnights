using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;



// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-19-23
// Purpose       | Used for basic UI elements to create clickable buttons
// ---------------------------------------------------------------- //

namespace UndeadKnights
{
    internal class Button
    {
        // --- Fields --- //

        // Location and Text
        private Rectangle rectangle;
        private string text;
        private SpriteFont font;
        private Color textColor;

        // Mouse Data
        private MouseState currentMS;
        private MouseState previousMS;
        private bool isPressed;
        private bool isHovered;

        // Graphics
        private Texture2D singleColor;



        // --- Properties --- //

        /// <summary>
        /// Returns true on the first frame the button is pressed
        /// </summary>
        public bool IsPressed { get { return isPressed; } }

        /// <summary>
        /// Returns true if the mouse overlaps the button
        /// </summary>
        public bool IsHovered { get { return isHovered; } }

        /// <summary>
        /// Allows the text to be changed
        /// </summary>
        public string Text { get { return text; } set { text = value; } }

        /// <summary>
        /// Returns the rectangle of the button
        /// </summary>
        public Rectangle PositionRectangle { get { return rectangle; } set { rectangle = value; } }

        // --- Constructor --- //

        /// <summary>
        /// Initalize a new button
        /// </summary>
        /// <param name="rectangle"> the location and size of the button </param>
        /// <param name="text"> text to display </param>
        public Button(Rectangle rectangle, string text, GraphicsDevice gd, SpriteFont font)
        {
            // Location and text
            this.rectangle = rectangle;
            this.text = text;
            this.font = font;

            // Mouse data
            currentMS = new MouseState();
            previousMS = new MouseState();
            isPressed = false;

            // Graphics
            singleColor = new Texture2D(gd, 1, 1);
            singleColor.SetData(new Color[] { Color.White });

        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            // Get the current mouse state
            currentMS = Mouse.GetState();

            // Check if the mouse is overlapping the button
            if (new Rectangle(currentMS.X, currentMS.Y, 0, 0).Intersects(rectangle))
            {

                // Set is hover to true
                isHovered = true;

                // Check if this is the first frame left click has been held down
                if (currentMS.LeftButton == ButtonState.Pressed
                && previousMS.LeftButton == ButtonState.Released)
                {
                    // isPressed = true when the button is left clicked
                    isPressed = true;
                } else
                {
                    isPressed = false;
                }
            } else
            {
                isHovered = false;
                isPressed = false;
            }

            // Set the previous mouse state to the last mouse state
            // This should remain at the end of the update method
            previousMS = currentMS;
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            // Draw the background
            sb.Draw(singleColor, rectangle, Color.Black * 0.4f);

            if (isHovered)
            {
                textColor = Color.DarkRed;
            } else
            {
                textColor = Color.White;
            }

            // Draw the text with a 10 pixel margin
            sb.DrawString(font, text, new Vector2(rectangle.X+10, rectangle.Y), textColor);
        }
    }
}
