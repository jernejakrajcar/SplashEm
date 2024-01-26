using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashEm
{
    public class AngryTargetManager
    {
        public static List<AngryTarget> AngryTargets { get; } = new List<AngryTarget>();
        private static Texture2D ForegroundTexture => TextureManager.Instance.ForegroundTexture;

        public static void AddAngryTarget(AngryTarget angryTarget)
        {
            AngryTargets.Add(angryTarget);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var angryTarget in AngryTargets)
            {
                angryTarget.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var angryTarget in AngryTargets)
            {
                angryTarget.Draw(spriteBatch);
            }
        }

        // Dodajte druge metode za obvladovanje trkov, upravljanje dosežkov itd.
        public static void HandleHit()
        {
            GenerateNewTargets();
        }

        private static Random random = new Random();

        private static Vector2 GenerateRandomTargetPosition()
        {
            int screenWidth = 1080;  // Širina zaslona
            int screenHeight = 2000; // Višina zaslona

            // Generiraj naključne koordinate znotraj območja
            int x = random.Next(-100, screenWidth);
            int y = random.Next(300, screenHeight - 200);

            return new Vector2(x, y);
        }

        public static void GenerateNewTargets()
        {
            Vector2 targetPosition = GenerateRandomTargetPosition();
            Vector2 targetVelocity = new Vector2(200, 0);

            // Če je tarča generirana na desni strani, uporabi zrcaljenje
            if (targetPosition.X > GraphicsDeviceManager.DefaultBackBufferWidth / 2)
            {
                AngryTarget newTarget = new AngryTarget(ForegroundTexture, targetPosition, -targetVelocity, false); // Uporabi zrcaljenje
                AddAngryTarget(newTarget);
            }
            else
            {
                AngryTarget newTarget = new AngryTarget(ForegroundTexture, targetPosition, targetVelocity, true); // Običajna orientacija
                AddAngryTarget(newTarget);
            }
        }
    }

}
