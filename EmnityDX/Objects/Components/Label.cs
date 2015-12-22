using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmnityDX.Objects.Components
{
    public struct Label
    {
        public string Text { get; set; }
        public SpriteFont SpriteFont { get; set; }
        public Color Color { get; set; }
    }
}
