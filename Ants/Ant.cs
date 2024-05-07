﻿using System;
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

        public int PathIndex;

        public Ant(Point Pos)
        {
            Position = Pos;
            PreviousePositions = new List<Point>();
            followingPath = false;
            destinationFound = false;
            PathIndex = 0;
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



            if (Grid.Slots[Position.Y][Position.X].isFood)
                destinationFound = true;
            PathIndex = PreviousePositions.Count() - 1;
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
    }
}
