
using EmnityDX.Engine;
using EmnityDX.Objects.LevelData;
using EmnityDX.Objects.LevelData.PlayingStateLevels;
using EmnityDX.Objects.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EmnityDX
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private State _currentState;
        private Queue<State> _states;
        private Camera _camera;
        private static readonly Vector2 _initialScale = new Vector2(800, 600);
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _states = new Queue<State>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Window.ClientSizeChanged += new System.EventHandler<System.EventArgs>(Resize_Window);
            this.IsMouseVisible = true;
            Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = (int)_initialScale.X;
            graphics.PreferredBackBufferHeight = (int)_initialScale.Y;

            _camera = new Camera(Vector2.Zero, Vector2.Zero, 0.0f, _initialScale, graphics);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _states.Enqueue(new PlayingState(new TestLevel(), _camera, Content, graphics));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            graphics.ApplyChanges();
            _currentState = _states.Peek();
            State nextState = _currentState.UpdateContent(gameTime, _camera);
            if(nextState != _currentState && nextState != null)
            {
                State tempState = _states.Dequeue();
                _states.Enqueue(nextState);
                if (tempState != _states.Peek())
                {
                    _states.Enqueue(tempState);
                }
            }
            else if (nextState == null)
            {
                _states.Dequeue();
            }
            if (_states.Count == 0)
            {
                Exit();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(transformMatrix: Matrix.CreateScale(GetScreenScale()));
            spriteBatch.Begin(transformMatrix: _camera.GetMatrix());
            _currentState.DrawContent(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Resize_Window(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Height != 0 && Window.ClientBounds.Width != 0) // Protect against minimizing
            {
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _camera.ResetScreenScale(graphics, _initialScale);
                _camera.Bounds = graphics.GraphicsDevice.Viewport.Bounds;
            }
        }
    }
}
