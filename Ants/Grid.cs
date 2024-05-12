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


        public Grid(Settings Settings)
        {
            Dimentions = Settings.gridSize;

            GenerateGrid(Settings);
        }

        private void GenerateGrid(Settings Settings)
        {
            Random _random = new Random();
            Slots = new List<List<GridSlot>>();

            for (int y = 0; y < Dimentions.Y; y++)
            {
                Slots.Add(new List<GridSlot>());

                for (int x = 0; x < Dimentions.X; x++)
                {
                    Slots.Last().Add(new GridSlot( new Point(x, y), Settings.foodSlotCapacity ) );
                }
            }

            // Wall Slots
            for (int i = 0; i < Dimentions.X; i++)
            {
                Slots[0][i].isFood = true;
                Slots.Last()[i].isFood = true;
            }
            for (int i = 0; i < Dimentions.Y; i++)
            {
                Slots[i][0].isFood = true;
                Slots[i].Last().isFood = true;
            }

            // Random Slots
            if (Settings.gridIsRandom)
                for (int i = 0; i < Settings.gridFoodSlotsCount; i++)
                {
                    Point SlotPos = new Point(
                        _random.Next(0, Dimentions.X),
                        _random.Next(0, Dimentions.Y)
                        );

                    if (SlotPos != Settings.hivePosition)
                        Slots[SlotPos.Y][SlotPos.X].isFood = true;
                }
        }
    }

    public class GridSlot
    {
        public Point Position { get; set; }
        public bool isFood { get; set; }
        public int foodCount { get; set; }

        public GridSlot(Point position, int FoodCapacity)
        {
            Position = position;
            foodCount = FoodCapacity;
        }
    }
}
