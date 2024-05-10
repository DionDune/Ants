using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Ants
{
    internal class inputHandler
    {
        private List<Keys> PreviouseKeys { get; set; }
        private List<Keys> NewKeys { get; set; }
        
        public inputHandler()
        {
            PreviouseKeys = new List<Keys>();
        }


        public void enact(Game1 Game)
        {
            NewKeys = Keyboard.GetState().GetPressedKeys().ToList();


            // Toggle Recall
            if (isNewPress(Keys.R) == true)
                Game.Hive.triggerRecall();

            // Grid rendering 
            if (isNewPress(Keys.G) == true)
                Game.Settings.renderGrid = !Game.Settings.renderGrid;

            // Ant rendering
            if (isNewPress(Keys.A) == true)
                Game.Settings.renderAnts = !Game.Settings.renderAnts;


            PreviouseKeys = NewKeys;
        }

        private bool? isNewPress(Keys Key)
        {
            if (!PreviouseKeys.Contains(Key) && NewKeys.Contains(Key))
                return true;
            else if (PreviouseKeys.Contains(Key) && NewKeys.Contains(Key))
                return false;

            return null;
        }
    }
}
