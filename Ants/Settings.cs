﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Ants
{
    public class Settings
    {
        public Point gridSize { get; set; }
        public Point hivePosition { get; set; }
        public int antCount { get; set; }

        public bool renderGrid { get; set; }
        public bool renderAnts { get; set; }
        public bool renderRougePaths { get; set; }
        public bool renderCompletePaths { get; set; }


        public Settings()
        {
            gridSize = new Point(190, 100);
            hivePosition = new Point(95, 50);
            antCount = 750;

            renderGrid = true;
            renderAnts = true;
            renderRougePaths = true;
            renderCompletePaths = true;
        }
    }
}
