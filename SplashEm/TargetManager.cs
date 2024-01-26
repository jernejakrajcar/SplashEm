using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SplashEm
{
    public class TargetManager
    {
        public static List<Target> Targets { get; } = new List<Target>();
        private static Texture2D ForegroundTexture => TextureManager.Instance.ForegroundTexture;
        public static int Score { get; set; }

        private static int targetsHit = 0;

        public static void AddTarget(Target target)
        {
            Targets.Add(target);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var target in Targets)
            {
                target.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {

            foreach (var target in Targets)
            {
                target.Draw(spriteBatch);
            }
        }


        public static void HandleHit()
        {
            targetsHit++;
            if (targetsHit >= 10)
            {
                Console.WriteLine("Zmagali ste!");
                ResetGame();
            }
            else
            {

                // Dodajte nove tarče
                GenerateNewTargets();
            }
        }

        private static Random random = new Random();

        private static Vector2 GenerateRandomTargetPosition()
        {
            int screenWidth = 1080;  // Širina zaslona
            int screenHeight = 2000; // Višina zaslona

            // Generiraj naključne koordinate znotraj območja
            int x = random.Next(-100, screenWidth);
            int y = random.Next(300, screenHeight-200);

            return new Vector2(x, y);
        }

        public static void GenerateNewTargets()
        {
            Vector2 targetPosition = GenerateRandomTargetPosition();
            Vector2 targetVelocity = new Vector2(200, 0);

                // Če je tarča generirana na desni strani, uporabi zrcaljenje
                if (targetPosition.X > GraphicsDeviceManager.DefaultBackBufferWidth / 2)
                {
                    Target newTarget = new Target(ForegroundTexture, targetPosition, -targetVelocity, false); // Uporabi zrcaljenje
                    AddTarget(newTarget);
                }
                else
                {
                    Target newTarget = new Target(ForegroundTexture, targetPosition, targetVelocity, true); // Običajna orientacija
                    AddTarget(newTarget);
                }
        }


        public static bool IsGameWon()
        {
            return Targets.Count == 0;
        }

        public static void ResetGame()
        {
            Targets.Clear();
            Score = 0;
            // Dodajte nove tarče po potrebi ali izvedite druge ukrepe za ponovni zagon igre
        }
    }
}
