using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.Transliterator;
using static Android.Webkit.WebStorage;

namespace SplashEm
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture;

        public Sprite(Texture2D texture)
        {
            this.Texture = texture;
        }

        public void Draw(SpriteBatch sp, Rectangle sourceRectangle)
        {
            sp.Draw(Texture, Position, sourceRectangle, Color.White);
        }
    }
}
