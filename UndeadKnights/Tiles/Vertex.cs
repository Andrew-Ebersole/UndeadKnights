using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 8-7-23
// Last Update   | 8-9-23
// Purpose       | Used for pathfinding using Dijkstra's
// ---------------------------------------------------------------- //

namespace UndeadKnights.Tiles
{
    internal class Vertex
    {
        // --- Fields --- //

        private bool permanent;
        private Vertex path;
        private float distance;
        private Point position;


        // --- Properties --- //

        /// <summary>
        /// If all sides have been checked
        /// </summary>
        public bool Permanent { get { return permanent; } set { permanent = value; } }

        /// <summary>
        /// Which vertex to go to do go to the start
        /// </summary>
        public Vertex Path { get { return path; } set {  path = value; } }


        /// <summary>
        /// Distnace to the start
        /// </summary>
        public float Distance { get { return distance; } set { distance = value; } }


        public Point Pos { get { return position; } set { position = value; } }

        // --- Constructor --- //

        public Vertex(int x, int y)
        {
            Permanent = false;
            Path = null;
            Distance = int.MaxValue;
            position = new Point(x, y);
        }
    }
}
