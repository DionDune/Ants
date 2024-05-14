using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Ants
{
    internal class inputHandler
    {
        /// <summary>
        /// 
        /// Controls:
        ///     R - Trigger Recall
        ///     G - Toggle Grid rendering
        ///     A - Toggle Ant rendering
        /// 
        /// </summary>


        private List<Keys> PreviouseKeys { get; set; }
        private List<Keys> NewKeys { get; set; }
        private bool mouseLeftClick { get; set; }
        private bool mouseRightClick { get; set; }
        
        public inputHandler()
        {
            PreviouseKeys = new List<Keys>();
            mouseLeftClick = false;
            mouseRightClick = false;
        }


        public void enact(Game1 Game)
        {
            keyboardHandler(Game);
            mouseHandler(Game);
        }

        private void keyboardHandler(Game1 Game)
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

            // Destitute Path rendering
            if (isNewPress(Keys.D) == true)
                Game.Settings.renderDestituePaths = !Game.Settings.renderDestituePaths;


            // Force Reset Ants
            if (isNewPress(Keys.Y) == true)
            {
                foreach (Ant Ant in Game.Hive.Ants)
                {
                    Ant.Position = Ant.Hive.Position;

                    if (!Ant.Hive.antsReturned.Contains(Ant))
                    {
                        Ant.Hive.antsReturned.Add(Ant);
                        Ant.returned = true;
                    }
                }

            }



            PreviouseKeys = NewKeys;
        }
        private void mouseHandler(Game1 Game)
        {
            bool LeftClick = false;
            bool RightClick = false;
            bool AltState = false;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                LeftClick = true;
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                RightClick = true;
            if (PreviouseKeys.Contains(Keys.LeftShift))
                AltState = true;


            Point MouseGridPos = new Point(
                Mouse.GetState().X / 10,
                Mouse.GetState().Y / 10
                );

            if (MouseGridPos.X > -1 && MouseGridPos.X < Game.Grid.Dimentions.X &&
                MouseGridPos.Y > -1 && MouseGridPos.Y < Game.Grid.Dimentions.Y)
            {
                if (LeftClick)
                    Game.Grid.Slots[MouseGridPos.Y][MouseGridPos.X].ToggleSolid(!AltState);
                else if (RightClick)
                    Game.Grid.Slots[MouseGridPos.Y][MouseGridPos.X].ToggleFood(!AltState);
            }
        }

        private bool? isNewPress(bool IsLeft, bool ButtonState)
        {
            if (ButtonState == false)
                return null;

            if (IsLeft && !mouseLeftClick)
                return true;
            else if (IsLeft)
                return false;

            if (!IsLeft && !mouseRightClick)
                return true;
            else if (!IsLeft)
            {
                return false;
            }

            return null;
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
