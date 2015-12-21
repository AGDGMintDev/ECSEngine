using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Engine
{
    public class State
    {
        protected ContentManager _content;

        public virtual void LoadContent()
        {
            _content = new ContentManager(StateManager.Instance.Content.ServiceProvider, "Content");
        }

        public virtual void UnloadContent()
        {
            if (_content != null) { _content.Unload(); }
            EntityManager.Instance.DestroyEntities();
        }

        public virtual void UpdateContent(GameTime gametime, ref GraphicsDeviceManager graphics)
        {

        }

        public virtual void DrawContent(SpriteBatch spriteBatch, ref GraphicsDeviceManager graphics)
        {

        }
    }
}
