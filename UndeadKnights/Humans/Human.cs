using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UndeadKnights.Tiles;
using System.Collections;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-9-23
// Purpose       | Base class of Humans
// ---------------------------------------------------------------- //

namespace UndeadKnights.Humans
{
    internal class Human
    {
        // --- Fields --- //

        protected Texture2D texture;
        protected Vector2 position;
        protected Point home;
        protected List<Vertex> path;
        protected Vertex[,] vertices; 



        // --- Properties --- //

        public Point Home { get { return home; } }

        public Rectangle Hitbox { get { return new Rectangle(
            (int)position.X * GameManager.Get.TileSize / 25, (int)position.Y * GameManager.Get.TileSize / 25,
            GameManager.Get.TileSize / 4, GameManager.Get.TileSize / 2); } }


        // --- Constructor --- //

        public Human(Texture2D texture, Point home)
        {
            this.texture = texture;
            this.home = home;
            position = new Vector2(home.X * 25, home.Y * 25);
            vertices = new Vertex[51, 51];
            path = new List<Vertex>();
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public virtual void Update(GameTime gt)
        {

        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            // Draw the human if they arent at their home
            if (!Hitbox.Intersects(new Rectangle
                ((int)(home.X * GameManager.Get.TileSize + 0.25f * GameManager.Get.TileSize),
                (int)(home.Y * GameManager.Get.TileSize + 0.25f * GameManager.Get.TileSize),
                GameManager.Get.TileSize,GameManager.Get.TileSize)))
            {

                // Get Tile Size
                int tileSize = GameManager.Get.TileSize;

                // Find location based off camera
                Rectangle drawLocation = new Rectangle(
                    Hitbox.X - GameManager.Get.Camera.X * tileSize / 25,
                    Hitbox.Y - GameManager.Get.Camera.Y * tileSize / 25,
                    Hitbox.Width,
                    Hitbox.Height);

                // Draw
                sb.Draw(texture, drawLocation, Color.White);
            }

            // Visualize pathfinding
            //foreach (Vertex v in path)
            //{
            //    sb.Draw(texture,
            //        new Rectangle(v.Pos.X * GameManager.Get.TileSize - GameManager.Get.Camera.X * GameManager.Get.TileSize / 25,
            //        v.Pos.Y * GameManager.Get.TileSize - GameManager.Get.Camera.Y * GameManager.Get.TileSize / 25,
            //        GameManager.Get.TileSize,
            //        GameManager.Get.TileSize),
            //        Color.Green * 0.25f);
            //}
        }

        /// <summary>
        /// Creates a path leading from the character to the end tile
        /// </summary>
        /// <param name="endTile"></param>
        public void Pathfind(Point endTile)
        {
            // Create the new list
            for (int x = 0; x < 51; x++)
            {
                for (int y = 0; y < 51; y++)
                {
                    vertices[x, y] = new Vertex(x, y);
                }
            }

            // Make sure the character is inside of the map
            if (Hitbox.X / GameManager.Get.TileSize < 0
                || Hitbox.X / GameManager.Get.TileSize >= 51
                || Hitbox.Y / GameManager.Get.TileSize < 0 
                || Hitbox.Y / GameManager.Get.TileSize >= 51)
            {
                return;
            }

            // Set start and end values
            Vertex end = vertices[endTile.X, endTile.Y];
            Vertex start = vertices[(int)Hitbox.X / GameManager.Get.TileSize, (int)Hitbox.Y / GameManager.Get.TileSize];
            Vertex current = start;

            current.Distance = 0;
            // Set start distance to 0

            while (!end.Permanent)
            {
                // Check adjacents and update distance / path
                for (int x = 0; x < 4; x++)
                {

                    int xPos = current.Pos.X;
                    int yPos = current.Pos.Y;

                    if (x == 0)
                    {
                        xPos -= 1;
                    }
                    if (x == 1)
                    {
                        yPos -= 1;
                    }
                    if (x == 2)
                    {
                        xPos += 1;
                    }
                    if (x == 3)
                    {
                        yPos += 1;
                    }


                    // Make sure adjacent value is in bounds of the array
                    if (xPos >= 0 && xPos < 51
                        && yPos >= 0 && yPos < 51)
                    {
                        // Find the distance based off of given tiles type
                        float distance = 4000;
                        if (TileManager.Get.TileGrid[xPos, yPos].TileType == TileType.Path)
                        {
                            distance = 1.75f;
                        }
                        else if (TileManager.Get.TileGrid[xPos, yPos].TileType != TileType.Wall
                                && TileManager.Get.TileGrid[xPos, yPos].TileType != TileType.Turret
                                && TileManager.Get.TileGrid[xPos, yPos].TileType != TileType.Tree
                                && TileManager.Get.TileGrid[xPos, yPos].TileType != TileType.Rock)
                        {
                            distance = 2;
                        } else if (TileManager.Get.TileGrid[xPos, yPos].TileType == TileType.Tree
                                || TileManager.Get.TileGrid[xPos, yPos].TileType == TileType.Rock)
                        {
                            distance = 2000;
                        }

                        // If the adjacent vertex isn't permanent and the distance
                        // is greater than if it went through the current vertex
                        // Set the distance to be one greater than the current
                        // and set the path to be the current
                        if (vertices[xPos, yPos].Distance > current.Distance + distance
                            && !vertices[xPos, yPos].Permanent)
                        {
                            vertices[xPos, yPos].Distance = current.Distance + distance;
                            vertices[xPos, yPos].Path = current;

                        }
                    }


                }
                
                current.Permanent = true;
                current = ReturnNextClosest();
            }

            // If the distance is greater than 4000 then the character could only
            // get to the end by walking through a wall
            if (vertices[endTile.X, endTile.Y].Distance < 4000)
            {
                CalculatePath(start,end);
            }
        }

        /// <summary>
        /// Returns the next non permanent vertex that 
        /// </summary>
        /// <returns></returns>
        public Vertex ReturnNextClosest()
        {
            Vertex returnVertex = null;
            for (int x = 0; x < 51; x++)
            {
                for (int y = 0; y < 51; y++)
                {
                    // If the return vertex is null set it to the first non permanent vertex
                    if (returnVertex == null)
                    {
                        if (!vertices[x, y].Permanent)
                        {
                            returnVertex = vertices[x, y];
                        }
                    }
                    // find the vertix with the lowest distance from start 
                    else if (vertices[x, y].Distance < returnVertex.Distance
                        && !vertices[x, y].Permanent)
                    {
                        returnVertex = vertices[x, y];
                    }
                }
            }

            return returnVertex;
        }

        /// <summary>
        /// Draw the path between points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void CalculatePath(Vertex start, Vertex end)
        {
            Vertex current = end;

            path = new List<Vertex>();
            path.Add(current);

            // Start at end and continue until path reaches start
            while (start != current)
            {
                // Make sure there is a actual path
                if (current.Path != null)
                {
                    current = current.Path;
                    path.Add(current);
                }
            }

            // reverse the path so it goes from start to end rather than end to start
            List<Vertex> reversedPath = new List<Vertex>();

            for (int i = path.Count-1; i >= 0; i--)
            {
                reversedPath.Add(path[i]);
            }

            path = reversedPath;
        }

    }
}
