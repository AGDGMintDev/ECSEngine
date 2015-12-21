using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine
{
    public class StateManager
    {
        private static StateManager _instance;
        public Vector2 Dimensions { private set; get; }
        public ContentManager Content { private set; get; }
        private State _currentState;

        

        public static StateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StateManager();
                }
                return _instance;
            }

        }

        public void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, "Content");
            if (_currentState != null) { _currentState.LoadContent(); }
        }

        public void UnloadContent()
        {
            _currentState.UnloadContent();
        }

        public void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {
            _currentState.DrawContent(spriteBatch, ref graphics);
        }

        public void UpdateContent(GameTime gameTime, ref GraphicsDeviceManager graphics)
        {
            _currentState.UpdateContent(gameTime, ref graphics);
        }

        #region Instance Options
        public void SetWindowDimensions(int width, int height)
        {
            _instance.Dimensions = new Vector2(width, height);
        }

        public void SetScreen(State screen)
        {
            if (_currentState != null)
            {
                _currentState.UnloadContent();
            }
            _currentState = screen;
            _currentState.LoadContent();
        }
        #endregion
    }
}
