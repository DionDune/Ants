using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ants
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random _random;
        Texture2D Texture_White;

        Grid Grid;
        List<Ant> Ants;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1800;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _random = new Random();

            Grid = new Grid(new Point(100, 100));
            Ants = new List<Ant>();

            for (int i = 0; i < 500; i++)
            {
                Ants.Add( new Ant( new Point(50, 50) ) );
            }

            for (int i = 0; i < 5; i++)
            {
                Grid.Slots[_random.Next(0, Grid.Dimentions.Y)][_random.Next(0, Grid.Dimentions.X)].isFood = true;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Procedurally Creating and Assigning a 1x1 white texture to Color_White
            Texture_White = new Texture2D(GraphicsDevice, 1, 1);
            Texture_White.SetData(new Color[1] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            Ant.MoveAnts(Grid, Ants);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();


            foreach (List<GridSlot> Row in Grid.Slots)
            {
                foreach(GridSlot Slot in Row)
                {
                    _spriteBatch.Draw(Texture_White, new Rectangle(Slot.Position.X * 10 + 2, Slot.Position.Y * 10 + 2, 6, 6), Color.Gray);

                    if (Slot.isFood)
                    {
                        _spriteBatch.Draw(Texture_White, new Rectangle(Slot.Position.X * 10, Slot.Position.Y * 10, 10, 10), Color.Gold);
                    }
                }
            }

            foreach (Ant Ant in Ants)
            {
                if (Ant.destinationFound)
                    _spriteBatch.Draw(Texture_White, new Rectangle(Ant.Position.X * 10, Ant.Position.Y * 10, 10, 10), Color.Blue);
                else
                    _spriteBatch.Draw(Texture_White, new Rectangle(Ant.Position.X * 10, Ant.Position.Y * 10, 10, 10), Color.Red);
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}