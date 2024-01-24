using Android.Locations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SplashEm
{
    public class SplashManager
    {
        public static List<Splash> Splashes = new List<Splash>();
        

        public static void Init()
        {
        }

        public static void AddSplash(Splash data)
        {
            Splashes.Add(data);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var s in Splashes)
            {
                s.Update(gameTime);
            }

            //Splashes.RemoveAll((s) => s.CurrentAlpha <= 0.0f);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var s in Splashes)
            {
                s.Draw(spriteBatch);
            }
        }


    }
}
