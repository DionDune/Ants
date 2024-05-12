using System;
using System.Collections.Generic;
using System.Linq;
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

        public Settings Settings;
        inputHandler inputHandler;
        Grid Grid;
        public Hive Hive;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _random = new Random();

            Settings = new Settings();
            inputHandler = new inputHandler();
            Grid = new Grid(Settings);
            Hive = new Hive(Settings.hivePosition, Settings.antCount);


            for (int i = 0; i < Grid.Dimentions.X; i++)
            {
                Grid.Slots[0][i].isFood = true;
                Grid.Slots.Last()[i].isFood = true;
            }
            for (int i = 0; i < Grid.Dimentions.Y; i++)
            {
                Grid.Slots[i][0].isFood = true;
                Grid.Slots[i].Last().isFood = true;
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




        void DrawLine(Vector2 point, float Length, float Angle, Color Color, float Thickness)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(Length, Thickness);

            _spriteBatch.Draw(Texture_White, point, null, Color, Angle, origin, scale, SpriteEffects.None, 0);
        }
        void DrawLineBetween(Vector2 Point1, Vector2 Point2, Color Color, float Thickness)
        {
            Vector2 DistanceVector = new Vector2(
                Point2.X - Point1.X,
                Point2.Y - Point1.Y
                );
            float Angle = (float)Math.Atan2(DistanceVector.Y, DistanceVector.X);
            float DistanceValue = Math.Abs(Vector2.Distance(Point1, Point2));

            DrawLine(Point1, DistanceValue, Angle, Color, Thickness);
        }




        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            inputHandler.enact(this);

            Hive.enactAI(Grid);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Hive.returnCall)
                GraphicsDevice.Clear(Color.Purple);
            else
                GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();



            // Grid Drawing
            foreach (List<GridSlot> Row in Grid.Slots)
            {
                foreach(GridSlot Slot in Row)
                {
                    if (Settings.renderGrid)
                        _spriteBatch.Draw(Texture_White, new Rectangle(Slot.Position.X * 10 + 2, Slot.Position.Y * 10 + 2, 6, 6), Color.Gray);

                    if (Slot.isFood)
                    {
                        _spriteBatch.Draw(Texture_White, new Rectangle(Slot.Position.X * 10, Slot.Position.Y * 10, 10, 10), Color.Gold);
                    }
                }
            }


            // Rogue Ant Path Drawing
            if (Settings.renderRougePaths)
                foreach (Ant Ant in Hive.Ants)
                {
                    if (!Ant.destinationFound)
                    foreach (Point Pos in Ant.path.Positions)
                    {
                        _spriteBatch.Draw(Texture_White, new Rectangle(Pos.X * 10, Pos.Y * 10, 10, 10), Color.DarkRed * 0.25F);
                    }
                }


            if (Settings.renderCompletePaths)
            {
                if (Settings.renderPathSquares)
                    // Complete Path Drawing - Undertone
                    foreach (Path Path in Hive.Paths)
                    {
                        foreach (Point Pos in Path.Positions)
                        {
                            float Opacity = 0;
                            for (int i = 0; i < Path.AntCount; i++)
                                Opacity = 1 - ((1 - Opacity) * 0.75F);
                            _spriteBatch.Draw(Texture_White, new Rectangle(Pos.X * 10, Pos.Y * 10, 10, 10), Color.DarkRed * Opacity);
                        }
                    }

                // Complete Path Drawing - Highlight
                foreach (Path Path in Hive.Paths)
                {
                    foreach (Point Pos in Path.Positions)
                    {
                        if (Settings.renderPathSquares)
                            _spriteBatch.Draw(Texture_White, new Rectangle(Pos.X * 10, Pos.Y * 10, 10, 10), Color.Blue * 0.25F);


                        if (Settings.renderPathLines)
                            if (Path.Positions.IndexOf(Pos) < Path.Positions.Count() - 1)
                            {
                                DrawLineBetween(new Vector2(Pos.X * 10 + 5, Pos.Y * 10 + 5),
                                                new Vector2(Path.Positions[Path.Positions.IndexOf(Pos) + 1].X * 10 + 5, 
                                                            Path.Positions[Path.Positions.IndexOf(Pos) + 1].Y * 10 + 5),
                                                Color.Blue, 5
                                                );
                            }
                    }
                }
            }


            // Ant Drawing
            if (Settings.renderAnts)
                foreach (Ant Ant in Hive.Ants)
                {
                    // Ant Drawing
                    if (Ant.destinationFound)
                    {
                        _spriteBatch.Draw(Texture_White, new Rectangle(Ant.Position.X * 10, Ant.Position.Y * 10, 10, 10), Color.Turquoise);

                        if (Ant.followingPath)
                            _spriteBatch.Draw(Texture_White, new Rectangle(Ant.Position.X * 10 + 1, Ant.Position.Y * 10 + 1, 8, 8), Color.Purple);
                    }
                    else
                        _spriteBatch.Draw(Texture_White, new Rectangle(Ant.Position.X * 10, Ant.Position.Y * 10, 10, 10), Color.Red);
                }

            // Hive Position Drawing
            _spriteBatch.Draw(Texture_White, new Rectangle(Hive.Position.X * 10, Hive.Position.Y * 10, 10, 10), Color.HotPink);



            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}