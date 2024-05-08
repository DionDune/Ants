using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Ants
{
    internal class Hive
    {
        public List<Ant> Ants { get; set; }
        public List<List<Point>> Paths { get; set; }

        public Hive(Point Position, int antCount)
        {
            Paths = new List<List<Point>>();
            Ants = new List<Ant>();

            for (int i = 0; i < antCount; i++)
            {
                Ants.Add(new Ant(Position));
                Ants.Last().Hive = this;
            }
        }

        public (List<Point>, int) getCrossingPath(Point chosenPosition)
        {
            foreach (List<Point> Path in Paths)
            {
                for (int i = 0; i < Path.Count(); i++)
                {
                    if (chosenPosition == Path[i])
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
        }
    }


    internal class Ant
    {
        public Hive Hive { get; set; }
        public Point Position { get; set; }

        public List<Point> PreviousePositions { get; set; }
        public bool followingPath { get; set; }
        public bool destinationFound { get; set; }

        public int PathIndex;

        public Ant(Point Pos)
        {
            Position = Pos;
            PreviousePositions = new List<Point>();
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

            PreviousePositions.Add(Position);
            Position += Movement;


            // Destination found
            if (Grid.Slots[Position.Y][Position.X].isFood)
            {
                destinationFound = true;
                Hive.Paths.Add(PreviousePositions);
                PathIndex = PreviousePositions.Count() - 1;
            }
            else
            {
                (List<Point>, int) crossingPath = Hive.getCrossingPath(Position);

                if (crossingPath.Item1 != null)
                {
                    destinationFound = true;
                    PreviousePositions = crossingPath.Item1;
                    PathIndex = crossingPath.Item2;
                }
            }
        }
        public void PathMove()
        {
            if (PathIndex == 0)
                followingPath = true;
            else if (PathIndex == PreviousePositions.Count() - 1)
                followingPath = false;


            if (followingPath)
            {
                Position = PreviousePositions[PathIndex + 1];
                PathIndex++;
            }
            else
            {
                Position = PreviousePositions[PathIndex - 1];
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
}
