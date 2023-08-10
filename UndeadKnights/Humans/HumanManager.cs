using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using UndeadKnights.Tiles;
using Microsoft.Xna.Framework.Content;
using System.Security.Cryptography.X509Certificates;
using System.IO;

// ---------------------------------------------------------------- //
// Collaborators | Andrew Ebersole
// Created Date  | 7-26-23
// Last Update   | 8-9-23
// Purpose       | Maages all the humans in the game
// ---------------------------------------------------------------- //

namespace UndeadKnights.Humans
{
    internal class HumanManager
    {
        // --- Fields --- //

        public List<Human> Humans;
        private Texture2D workerTexture;



        // --- Properties --- //

        public static HumanManager instance = null;

        public static HumanManager Get
        {
            get
            {
                if (instance == null)
                {
                    instance = new HumanManager();
                } 
                
                return instance;
                
            }
        }


        // --- Constructor --- //

        public void Initialize(ContentManager content,
           Point windowsize, GraphicsDevice gd)
        {
            // Set worker texture
            workerTexture = content.Load<Texture2D>("worker");

            NewGame();
        }



        // --- Methods --- // 

        /// <summary>
        /// Called every frame to update functions
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            foreach (Human human in Humans)
            {
                human.Update(gt);
            }
        }

        /// <summary>
        /// Called every frame to update graphical elements
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            foreach (Human human in Humans)
            {
                human.Draw(sb);
            }
        }

        /// <summary>
        /// Returns the total number of workers
        /// </summary>
        public int TotalWorkers()
        {
            int totalWorkers = 0;

            // Check if the human is a worker
            foreach (Human human in Humans)
            {
                if (human is Worker)
                {
                    totalWorkers++;
                }
            }
            return totalWorkers;
        }

        /// <summary>
        /// Returns the number of workers that don't have a current tile to destory
        /// </summary>
        /// <returns></returns>
        public int WorkingWorkers()
        {
            int workingWorkers = 0;

            foreach (Human human in Humans)
            {
                if (human is Worker)
                {
                    Worker worker = (Worker)human;
                    if (worker.State == WorkerState.Mine
                        || worker.State == WorkerState.MoveToResource)
                    {
                        workingWorkers++;
                    }
                }
            }
            return workingWorkers;
        }

        /// <summary>
        /// Returns amount of Humans that have given location as their home
        /// </summary>
        /// <param name="Location"></param>
        public int PeopleAtLocation(Point location)
        {
            // count amount of people at given location
            int count = 0;
            foreach (Human human in Humans)
            {
                if (human.Home == location)
                {
                    count++;
                }
            }

            return count;
        }

        public void AddPeople(Point location)
        {
            // Check if tile is a building
            if (TileManager.Get.TileGrid[location.X,location.Y] is Building)
            {
                // Check if people is not at the builidng limit
                Building home = (Building)TileManager.Get.TileGrid[location.X, location.Y];
                if (home.People < home.MaxPeople)
                {
                    // check what type of building it is
                    switch (home.TileType)
                    {
                        case TileType.House:
                            Humans.Add(new Worker(workerTexture,location));
                            break;

                        case TileType.Armory:

                            break;

                        case TileType.ShootingRange:

                            break;

                        case TileType.Stable:

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sends an available worker to destory the given resource
        /// </summary>
        /// <param name="tile"></param>
        public void DestoryResource(Point tilePoint)
        {
            foreach (Human human in Humans)
            {
                if (human is Worker
                    && !GameManager.Get.IsNight)
                {
                    Worker worker = (Worker)human;
                    if (worker.State == WorkerState.Home
                        || worker.State == WorkerState.ReturnHome)
                    {
                        if (worker.DestroyResource(tilePoint))
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initalizes everything at the start of a round
        /// </summary>
        public void NewGame()
        {
            // Create list of humans
            Humans = new List<Human>();
        }

        public void RemoveUnusedHuman()
        {
            for (int i = 0; i < Humans.Count; i++)
            {
                if (Humans[i] is Worker)
                {
                    Humans.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveHouse(Point home)
        {
            List<Human> humansToRemove = new List<Human>();

            // Find which humans home is at given point
            foreach (Human human in Humans)
            {
                if (human.Home == home)
                {
                    humansToRemove.Add(human);
                }
            }

            // Remove the human at the given point
            while (humansToRemove.Count > 0)
            {
                Humans.Remove(humansToRemove[0]);
                humansToRemove.RemoveAt(0);
                GameManager.Get.Food++;
            }

        }
    }
}
