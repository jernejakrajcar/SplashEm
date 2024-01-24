using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplashEm
{
    public class PoliceTargetManager
    {
        public static List<PoliceTarget> PoliceTargets { get; } = new List<PoliceTarget>();
        private static Texture2D ForegroundTexture => TextureManager.Instance.ForegroundTexture;

        private static Random random = new Random();

        public static void AddPoliceTarget(PoliceTarget policeTarget)
        {
            PoliceTargets.Add(policeTarget);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var policeTarget in PoliceTargets)
            {
                policeTarget.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var policeTarget in PoliceTargets)
            {
                policeTarget.Draw(spriteBatch);
            }
        }

        public static void GenerateNewPoliceTarget()
        {
            Vector2 policeTargetPosition = GenerateRandomTargetPosition();
            Vector2 policeTargetVelocity = new Vector2(200, 0);

            PoliceTarget newPoliceTarget = new PoliceTarget(ForegroundTexture, policeTargetPosition, policeTargetVelocity, true);
            AddPoliceTarget(newPoliceTarget);
        }

        private static Vector2 GenerateRandomTargetPosition()
        {
            int screenWidth = 1080;  // Širina zaslona
            int screenHeight = 2000; // Višina zaslona

            int x = random.Next(-100, screenWidth);
            int y = random.Next(300, screenHeight);

            return new Vector2(x, y);
        }
    }
}
