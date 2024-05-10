using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Ants
{
    public class Grid
    {
        public List<List<GridSlot>> Slots { get; set; }
        public Point Dimentions { get; set; }


        public Grid(Point dimentions)
        {
            Dimentions = dimentions;

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Slots = new List<List<GridSlot>>();

            for (int y = 0; y < Dimentions.Y; y++)
            {
                Slots.Add(new List<GridSlot>());

                for (int x = 0; x < Dimentions.X; x++)
                {
                    Slots.Last().Add(new GridSlot( new Point(x, y) ) );
                }
            }
        }
    }

    public class GridSlot
    {
        public Point Position { get; set; }
        public bool isFood { get; set; }
        public int foodCount { get; set; }

        public GridSlot(Point position)
        {
            Position = position;
        }
    }
}
