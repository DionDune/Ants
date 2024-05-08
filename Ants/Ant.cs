using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Ants
{
    internal class Ant
    {
        public Point Position { get; set; }

        public List<Point> PreviousePositions { get; set; }
        public bool followingPath { get; set; }
        public bool destinationFound { get; set; }

        public Ant(Point Pos)
        {
            Position = Pos;
            PreviousePositions = new List<Point>();
            followingPath = false;
            destinationFound = false;
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

            Position += Movement;


            if (Grid.Slots[Position.Y][Position.X].isFood)
                destinationFound = true;
        }
        public void PathMove()
        {
            if (followingPath)
            {
                if (PreviousePositions.IndexOf(Position) == PreviousePositions.Count() - 1)
                    followingPath = false;
                else
                    Position = PreviousePositions[PreviousePositions.IndexOf(Position) + 1];
            }
            else
            {
                if (PreviousePositions.IndexOf(Position) < 2)
                    followingPath = true;
                else
                    Position = PreviousePositions[PreviousePositions.IndexOf(Position) - 1];
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
    }
}
