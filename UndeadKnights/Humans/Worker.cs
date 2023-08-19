using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndeadKnights.Tiles;
using System.IO;
using System.Threading;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-11-23
// Purpose       | Variant of the Human that gathers resources
// ---------------------------------------------------------------- //

namespace UndeadKnights.Humans
{
    enum WorkerState
    {
        MoveToResource,
        Mine,
        ReturnHome,
        Home
    }
    internal class Worker : Human
    {
        // --- Fields --- //


        private int mineTimer;
        private Point tileToDestroy;
        WorkerState currentState;


        // --- Properties --- //

        public WorkerState State { get { return currentState; } }


        // --- Constructor --- //

        public Worker(Texture2D texture, Point home) : base(texture, home)
        {
            currentState = WorkerState.Home;
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public override void Update(GameTime gt)
        {
            base.Update(gt);

            switch (currentState)
            {
                case WorkerState.MoveToResource:
                    if (!Hitbox.Intersects(new Rectangle(
                            (int)(tileToDestroy.X * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.25f),
                            (int)(tileToDestroy.Y * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.25f),
                            (int)(GameManager.Get.TileSize * 0.5f),
                            (int)(GameManager.Get.TileSize * 0.5f))))
                    {
                        if (path.Count > 0)
                        {
                            // Check if the worker is touching the next path
                            if (Hitbox.Intersects(new Rectangle(
                                (int)(path[0].Pos.X * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.25f),
                                (int)(path[0].Pos.Y * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.25f),
                                (int)(GameManager.Get.TileSize * 0.5f),
                                (int)(GameManager.Get.TileSize * 0.5f))))
                            {
                                path.RemoveAt(0);
                            }
                            else
                            {
                                // Move faster when on path
                                if (TileManager.Get.TileGrid[(int)(Math.Floor((double)(Hitbox.X / GameManager.Get.TileSize))),
                                    (int)(Math.Floor((double)(Hitbox.Y / GameManager.Get.TileSize)))].TileType == TileType.Path)
                                {
                                    position -= GetDirectionVector(position, new Vector2(path[0].Pos.X, path[0].Pos.Y)) * 1.25f;
                                }
                                else
                                {
                                    position -= GetDirectionVector(position, new Vector2(path[0].Pos.X, path[0].Pos.Y));
                                }
                            }
                        } else
                        {
                            // Move faster when on path
                            if (TileManager.Get.TileGrid[(int)(Math.Floor((double)(Hitbox.X / GameManager.Get.TileSize))),
                                (int)(Math.Floor((double)(Hitbox.Y / GameManager.Get.TileSize)))].TileType == TileType.Path)
                            {
                                position -= GetDirectionVector(position, new Vector2(tileToDestroy.X, tileToDestroy.Y)) * 1.25f;
                            }
                            else
                            {
                                position -= GetDirectionVector(position, new Vector2(tileToDestroy.X, tileToDestroy.Y));
                            }
                        }
                    } else
                    {
                        currentState = WorkerState.Mine;
                        mineTimer = 0;
                    }
                    break;



                case WorkerState.Mine:
                    // Update mine timer
                    mineTimer += gt.ElapsedGameTime.Milliseconds;

                    // After 5 seconds break the resource
                    if (mineTimer > 5000)
                    {

                        // Destroy resource
                        TileManager.Get.TileToGrass(tileToDestroy);

                        Pathfind(home);
                        currentState = WorkerState.ReturnHome;
                    }
                    break;

                    case WorkerState.ReturnHome:
                    // keep pathfinding home until touching the house
                    if (!Hitbox.Intersects(new Rectangle(
                            (int)(home.X * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.45f),
                            (int)(home.Y * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.45f),
                            (int)(GameManager.Get.TileSize * 0.1f),
                            (int)(GameManager.Get.TileSize * 0.1f))))
                    {
                        if (path.Count > 0)
                        {
                            // Check if the worker is touching the next path
                            if (Hitbox.Intersects(new Rectangle(
                                (int)(path[0].Pos.X * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.25f),
                                (int)(path[0].Pos.Y * GameManager.Get.TileSize + GameManager.Get.TileSize * 0.25f),
                                (int)(GameManager.Get.TileSize * 0.5f),
                                (int)(GameManager.Get.TileSize * 0.5f))))
                            {
                                path.RemoveAt(0);
                            }
                            else
                            {
                                // Move faster when on path
                                if (TileManager.Get.TileGrid[(int)(Math.Floor((double)(Hitbox.X / GameManager.Get.TileSize))),
                                    (int)(Math.Floor((double)(Hitbox.Y/GameManager.Get.TileSize)))].TileType == TileType.Path)
                                {
                                    position -= GetDirectionVector(position, new Vector2(path[0].Pos.X, path[0].Pos.Y)) * 1.25f;
                                }
                                else
                                {
                                    position -= GetDirectionVector(position, new Vector2(path[0].Pos.X, path[0].Pos.Y));
                                }
                            }
                        } else
                        {
                            // Move faster when on path
                            if (TileManager.Get.TileGrid[(int)(Math.Floor((double)(Hitbox.X / GameManager.Get.TileSize))),
                                (int)(Math.Floor((double)(Hitbox.Y / GameManager.Get.TileSize)))].TileType == TileType.Path)
                            {
                                position -= GetDirectionVector(position, new Vector2(home.X,home.Y)) * 1.25f;
                            }
                            else
                            {
                                position -= GetDirectionVector(position, new Vector2(home.X,home.Y));
                            }
                        }
                    }
                    else
                    {
                        currentState = WorkerState.Home;
                    }
                    break;
            }
        }

        /// <summary>
        /// Updates the workers objective to destory the resources at current location
        /// </summary>
        /// <param name="location"></param>
        public bool DestroyResource(Point location)
        {
            path.Clear();
            Pathfind(location);

            if (path.Count > 0)
            {
                currentState = WorkerState.MoveToResource;
                this.tileToDestroy = location;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a normalized vector pointing in the direction you are going
        /// </summary>
        /// <param name="position"></param>
        /// <param name="going"></param>
        /// <returns></returns>
        private Vector2 GetDirectionVector(Vector2 position, Vector2 going)
        {
            going += new Vector2(0.25f, 0.25f);
            going *= 25;

            Vector2 returnVector = Vector2.Normalize(position - going);

            // Check if the vector has real numbers in it
            if (Math.Abs(returnVector.X) >= 0)
            {
                return returnVector;
            }

            // If not return defualt vector
            return new Vector2(0, 0);
        }
    }
}
