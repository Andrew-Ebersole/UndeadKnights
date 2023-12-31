﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-7-23
// Purpose       | Variant of Human with a sword
// ---------------------------------------------------------------- //
namespace UndeadKnights.Humans
{
    internal class Knight : Human
    {
        // --- Fields --- //





        // --- Properties --- //





        // --- Constructor --- //

        public Knight(Texture2D texture, Point home) : base(texture, home)
        { }



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
        public void Draw(SpriteBatch sb)
        {

        }
    }
}
