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
    internal class Hive
    {
        public List<Ant> Ants { get; set; }
        public List<Path> Paths { get; set; }
        public bool returnCall { get; set; }
        public List<Ant> antsReturned { get; set; }

        public Hive(Point Position, int antCount)
        {
            Paths = new List<Path>();
            Ants = new List<Ant>();
            returnCall = false;
            antsReturned = new List<Ant>();

            for (int i = 0; i < antCount; i++)
            {
                Ants.Add(new Ant(Position));
                Ants.Last().Hive = this;
            }
        }
        public void triggerRecall()
        {
            returnCall = true;
        }

        public (Path, int) getCrossingPath(Point chosenPosition)
        {
            foreach (Path Path in Paths)
            {
                for (int i = 0; i < Path.Positions.Count(); i++)
                {
                    if (chosenPosition == Path.Positions[i])
                        return (Path, i);
                }
            }

            return (null, 0);
        }


        public void enactAI(Grid Grid)
        {
            foreach (Ant Ant in Ants)
            {
                Ant.enactAI(Grid);
            }

            // RETURN CALL LOGIC
            if (antsReturned.Count() == Ants.Count())
            {
                Paths.Clear();

                foreach (Ant Ant in Ants)
                {
                    Ant.destinationFound = false;
                    Ant.PathIndex = 0;
                    Ant.path = new Path(new List<Point>(), false);
                }

                returnCall = false;
                antsReturned = new List<Ant>();
            }

            Debug.WriteLine($"{Ants.Count()} : {antsReturned.Count()}");
        }
    }


    internal class Ant
    {
        public Hive Hive { get; set; }
        public Point Position { get; set; }

        public Path path { get; set; }
        public bool followingPath { get; set; }
        public bool destinationFound { get; set; }

        public int PathIndex;

        public Ant(Point Pos)
        {
            Position = Pos;
            path = new Path(new List<Point>(), false);
            followingPath = false;
            destinationFound = false;
            PathIndex = 0;
            Hive = null;
        }


        public void RandomMove(Grid Grid)
        {
            Random random = new Random();

            Point Movement = new Point( random.Next(-1, 2), random.Next(-1, 2) );

            while (Position.X + Movement.X < 0 || Position.X + Movement.X >= Grid.Dimentions.X ||
                   Position.Y + Movement.Y < 0 || Position.Y + Movement.Y >= Grid.Dimentions.Y)
            {
                Movement = new Point(random.Next(-1, 2), random.Next(-1, 2));
            }

            path.Positions.Add(Position);
            Position += Movement;


            // Destination found
            if (Grid.Slots[Position.Y][Position.X].isFood)
            {
                destinationFound = true;
                path.isComplete = true;
                Hive.Paths.Add(path);
                PathIndex = path.Positions.Count() - 1;
            }
            else
            {
                (Path, int) crossingPath = Hive.getCrossingPath(Position);

                if (crossingPath.Item1 != null)
                {
                    destinationFound = true;
                    crossingPath.Item1.joinAnt(this);
                    PathIndex = crossingPath.Item2;
                }
            }
        }
        public void PathMove()
        {
            if (PathIndex == 0)
            {
                followingPath = true;

                if (Hive.returnCall)
                {
                    if (!Hive.antsReturned.Contains(this))
                        Hive.antsReturned.Add(this);
                    return;
                }
            }
            else if (PathIndex == path.Positions.Count() - 1)
                followingPath = false;


            if (followingPath)
            {
                Position = path.Positions[PathIndex + 1];
                PathIndex++;
            }
            else
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

    internal class Path
    {
        public List<Point> Positions { get; set; }
        public int AntCount { get; set; }
        public bool isComplete { get; set; }

        public Path(List<Point> prevPositions, bool Complete)
        {
            Positions = prevPositions;
            AntCount = 1;
            isComplete = Complete;
        }

        public void joinAnt(Ant Ant)
        {
            Ant.path = this;
            AntCount++;
        }
    }
}
