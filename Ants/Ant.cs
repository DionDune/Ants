using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Ants
{
    public class Hive
    {
        public List<Ant> Ants { get; set; }
        public List<Path> Paths { get; set; }
        public List<Path> DestitutePaths { get; set; }
        public Point Position { get; set; }

        public bool returnCall { get; set; }
        public List<Ant> antsReturned { get; set; }

        public int foodCount { get; set; }


        public Hive(Point position, int antCount)
        {
            Position = position;
            Paths = new List<Path>();
            DestitutePaths = new List<Path>();
            Ants = new List<Ant>();

            returnCall = false;
            antsReturned = new List<Ant>();

            foodCount = 0;


            for (int i = 0; i < antCount; i++)
            {
                Ants.Add(new Ant(Position));
                Ants.Last().Hive = this;
            }
        }



        public (Path, int) getCrossingPath(Point chosenPosition)
        {
            foreach (Path Path in Paths)
            {
                int index = Path.Positions.IndexOf(chosenPosition);
                if (index > -1)
                {
                    return (Path, index);
                }
            }

            return (null, 0);
        }


        public void triggerRecall()
        {
            returnCall = true;

            foreach (Ant Ant in Ants)
            {
                Ant.followingPath = false;
            }
        }

        public void enact(Game1 Game)
        {
            enactAI(Game);
            foodHandler(Game);
        }
        private void enactAI(Game1 Game)
        {
            foreach (Ant Ant in Ants)
            {
                Ant.enactAI(Game.Grid);
            }

            // RETURN CALL LOGIC
            if (antsReturned.Count() == Ants.Count())
            {
                Paths.Clear();

                foreach (Ant Ant in Ants)
                {
                    Ant.destinationFound = false;
                    Ant.followingPath = false;
                    Ant.PathIndex = 0;
                    Ant.path = new Path(this, new List<Point>(), false);
                    Ant.returned = false;
                }

                returnCall = false;
                antsReturned = new List<Ant>();
            }
        }
        private void foodHandler(Game1 Game)
        {
            if (foodCount >= Game.Settings.antFoodCost)
            {
                Ants.Add(new Ant(Position));
                Ants.Last().Hive = this;
                foodCount -= Game.Settings.antFoodCost;
            }
        }
    }


    public class Ant
    {
        public Hive Hive { get; set; }
        public Point Position { get; set; }
        public bool hasFood { get; set; }

        public Path path { get; set; }
        public bool followingPath { get; set; }
        public bool destinationFound { get; set; }
        public bool returned { get; set; }

        public int PathIndex;

        public Ant(Point Pos)
        {
            Position = Pos;
            path = new Path(Hive, new List<Point>() { Position }, false);
            hasFood = false;

            followingPath = false;
            destinationFound = false;
            PathIndex = 0;

            Hive = null;
            returned = false;
        }


        public void RandomMove(Grid Grid)
        {
            Random random = new Random();

            Point Movement = new Point( random.Next(-1, 2), random.Next(-1, 2) );

            if (!returned)
            {
                while (Position.X + Movement.X < 0 || Position.X + Movement.X >= Grid.Dimentions.X ||
                   Position.Y + Movement.Y < 0 || Position.Y + Movement.Y >= Grid.Dimentions.Y)
                {
                    Movement = new Point(random.Next(-1, 2), random.Next(-1, 2));
                }


                if (Grid.Slots[Position.Y + Movement.Y][Position.X + Movement.X].isSolid == false)
                {
                    path.Positions.Add(Position);
                    Position += Movement;
                }


                // Hive crossed on return / Ant returns to hive
                if (Hive.returnCall && Position == Hive.Position)
                {
                    if (!Hive.antsReturned.Contains(this))
                    {
                        Hive.antsReturned.Add(this);
                        returned = true;
                        return;
                    }
                }


                // Destination found
                if (Grid.Slots[Position.Y][Position.X].isFood)
                {
                    path.isComplete = true;
                    path.FoodSlot = Grid.Slots[Position.Y][Position.X];
                    path.Hive = Hive;
                    path.takeFood(this);

                    Hive.Paths.Add(path);
                    destinationFound = true;
                    PathIndex = path.Positions.Count() - 1;
                }
                else
                {
                    (Path, int) crossingPath = Hive.getCrossingPath(Position);

                    if (crossingPath.Item1 != null)
                    {
                        crossingPath.Item1.joinAnt(this, crossingPath.Item2);
                    }
                    else
                        path.simplifyFromLast(this);
                }
            }
        }
        public void PathMove()
        {
            path.firstSlotHandler(this);

            if (Hive.returnCall && returned)
                return;

            // Start Slot
            if (PathIndex == 0)
            {
                path.firstSlotHandler(this);

                followingPath = true;
                hasFood = false;

                // Ant returns to hive
                if (Hive.returnCall)
                {
                    if (!Hive.antsReturned.Contains(this))
                    {
                        Hive.antsReturned.Add(this);
                        returned = true;
                        return;
                    }
                }

                // Ants abandon destitue paths
                if (path.isDestitute)
                {
                    path.leaveAnt(this);
                    return;
                }
            }
            // End Slot
            else if (PathIndex == path.Positions.Count() - 1)
            {
                path.lastSlotHandler(this);
            }



            if (followingPath && path.Positions.Count() > 1)
            {
                Position = path.Positions[PathIndex + 1];
                PathIndex++;
            }
            else if (path.Positions.Count() > 1 && PathIndex > 0)
            {
                Position = path.Positions[PathIndex - 1];
                PathIndex--;
            }
        }

        public static void MoveAnts(Grid Grid, List<Ant> Ants)
        {
            foreach (Ant Ant in Ants)
            {
                if (Ant.destinationFound)
                {
                    Ant.PathMove();
                }
                else
                {
                    Ant.RandomMove(Grid);
                }
            }
        }
        public void enactAI(Grid Grid)
        {
            if (destinationFound)
            {
                PathMove();
            }
            else
            {
                RandomMove(Grid);
            }
        }


    }

    public class Path
    {
        public Hive Hive { get; set; }
        public List<Point> Positions { get; set; }
        public GridSlot FoodSlot { get; set; }
        public int AntCount { get; set; }
        public bool isComplete { get; set; }
        public bool isDestitute { get; set; }

        public Path(Hive Hive, List<Point> prevPositions, bool Complete)
        {
            Positions = prevPositions;
            FoodSlot = null;
            AntCount = 1;
            isComplete = Complete;
            isDestitute = false;
            Hive = null;
        }

        public bool joinAnt(Ant Ant, int index)
        {
            if (!isDestitute)
            {
                Ant.path = this;
                Ant.destinationFound = true;
                Ant.PathIndex = index;
                AntCount++;

                return true;
            }

            return false;
        }
        public void leaveAnt(Ant Ant)
        {
            Ant.path = new Path(Hive, new List<Point>(), false);
            Ant.destinationFound = false;
            Ant.followingPath = false;
            Ant.PathIndex = 0;
            AntCount--;

            handleDestitution();
        }

        private void handleDestitution()
        {
            if (isDestitute && AntCount == 0)
            {
                Hive.DestitutePaths.Add(this);
                Hive.Paths.Remove(this);
            }
        }
        public void takeFood(Ant Ant)
        {
            if (FoodSlot.foodCount > 0)
                Ant.hasFood = true;

            if (FoodSlot.foodCount <= 0)
            {
                isDestitute = true;
                FoodSlot.isFood = false;
            }

            FoodSlot.foodCount--;
        }

        public void lastSlotHandler(Ant Ant)
        {
            Ant.followingPath = false;
            takeFood(Ant);
        }
        public void firstSlotHandler(Ant Ant)
        {
            if (Ant.hasFood)
            {
                Ant.hasFood = false;
                Hive.foodCount++;
            }
            
        }

        private void simplify(int Index, Ant Ant)
        {
            for (int i = Index - 1; i > 0; i--)
            {
                if (Positions[i] == Positions[Index])
                {
                    Positions.RemoveRange(i, Index - i);
                    Ant.PathIndex -= Index - i;
                    break;
                }
            }
        }
        public void simplifyFromLast(Ant Ant)
        {
            simplify(Positions.Count - 1, Ant);
        }
        public void simplifyFrom(int Index, Ant Ant)
        {
            simplify(Index, Ant);
        }
    }
}
