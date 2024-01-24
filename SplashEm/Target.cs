using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplashEm
{
    public class Target : Sprite
    {
        private Vector2 velocity; // Speed and direction of movement
        public Vector2 size;

        private Rectangle[] frames; // Array of rectangles representing animation frames
        private int currentFrame;   // Index of the current frame
        private float frameTimer;   // Timer to control frame switching
        private float frameInterval = 0.2f; // Time interval between frame changes
        private Boolean IsMovingLeft;


        public Target(Texture2D texture, Vector2 position, Vector2 velocity, Boolean IsMovingLeft) : base(texture)
        {
            this.velocity = velocity;
            this.Position = position;
            this.IsMovingLeft = IsMovingLeft;

            this.size = new Vector2(180, 350);

            // Initialize the frames array with specific rectangles
            frames = new Rectangle[3];
            frames[0] = new Rectangle(0, 200, 90, 200);
            frames[1] = new Rectangle(90, 200, 90, 200);
            frames[2] = new Rectangle(200, 200, 90, 200);
        }

        public void Update(GameTime gameTime)
        {
            // Update the position based on velocity and elapsed time
            Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the animation frame
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
            }

            // Example: If the target goes off the right edge, reset its position to the left edge.
            if (Position.X - 200 > GraphicsDeviceManager.DefaultBackBufferWidth)
            {
                if (IsMovingLeft)
                {
                    // Reset position to the left edge.
                    Position = new Vector2(-200, Position.Y);
                }
            }
            else if (Position.X < -200)
            {
                if (!IsMovingLeft)
                {
                    // Reset position to the right edge.
                    Position = new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth+200, Position.Y);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = frames[currentFrame];

            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);

            if (IsMovingLeft)
            {
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            }
            else
            {
                // Zrcaljenje po horizontalni osi
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            }

        }
    }
}
