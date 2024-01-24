using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashEm
{
    public class Splash : Sprite
    {
        public new Vector2 Position
        {
            get { return base.Position; }
            set { base.Position = value; }
        }
        public float InitialAlpha { get; set; }
        public float FadeSpeed { get; set; }
        public float CurrentAlpha { get; private set; }

        private Rectangle[] frames; // Array of rectangles representing animation frames
        private int currentFrame;   // Index of the current frame
        private float frameTimer;   // Timer to control frame switching
        private float frameInterval = 0.1f; // Time interval between frame changes

        public Splash(Texture2D texture, Vector2 position) : base(texture)
        {
            CurrentAlpha = InitialAlpha;
            Position = position;

            frames = new Rectangle[8];
            frames[0] = new Rectangle(0, 0, 350, 250);
            frames[1] = new Rectangle(350, 0, 350, 250);
            frames[2] = new Rectangle(700, 0, 350, 250);
            frames[3] = new Rectangle(0, 250, 350, 250);
            frames[4] = new Rectangle(350, 250, 350, 250);
            frames[5] = new Rectangle(700, 250, 350, 250);
            frames[6] = new Rectangle(0, 500, 350, 250);
            frames[7] = new Rectangle(0, 500, 350, 250);
        }


        public void Update(GameTime gameTime)
        {
            // Update the fading effect
            InitialAlpha -= FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            InitialAlpha = Math.Max(0.0f, InitialAlpha); // Ensure alpha doesn't go below 0

            // Update the animation frame
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
            }
            if(InitialAlpha == 0.0f)
            {
                SplashManager.Splashes.Remove(this);
            }
            
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = frames[currentFrame];
            Rectangle destinationRect = new Rectangle((int)Position.X, (int)Position.Y, 170, 125);
            spriteBatch.Draw(Texture, destinationRect, sourceRectangle, Color.White*InitialAlpha);
        }
    }
}
