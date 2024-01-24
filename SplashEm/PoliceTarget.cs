using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace SplashEm { 
public class PoliceTarget : Sprite
{
    public Vector2 position;
    private Vector2 velocity;
    public Vector2 size;
    private bool isMovingLeft;
    private bool isStopped;
    private float stopTimer;
    private float stopInterval = 5.0f;
    private Random random;
        private Rectangle[] frames; // Dodali smo frames za animacijo
        private int currentFrame;
        private float frameTimer;
        private float frameInterval = 0.2f;

        public PoliceTarget(Texture2D texture, Vector2 position, Vector2 velocity, bool isMovingLeft) : base(texture)
    { 
        this.position = position;
        this.velocity = velocity;
        this.size = new Vector2(180, 350);
        this.isMovingLeft = isMovingLeft;
        isStopped = false;
        stopTimer = 0.0f;
        this.random = new Random();
        GenerateNewStopInterval();

            //dodatni frami za animcijo
            frames = new Rectangle[3];
            frames[0] = new Rectangle(0, 400, 100, 200);
            frames[1] = new Rectangle(100, 400, 100, 200);
            frames[2] = new Rectangle(200, 400, 100, 200);
        }

    public void Update(GameTime gameTime)
    {
        if (!isStopped)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            stopTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (stopTimer >= stopInterval)
            {
                isStopped = true;
                stopTimer = 0.0f;
            }
        }
        else
        {
            stopTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (stopTimer >= stopInterval / 2)
            {
                isStopped = false;
                stopTimer = 0.0f;
                GenerateNewStopInterval();
            }
        }

            if (position.X - 200 > GraphicsDeviceManager.DefaultBackBufferWidth)
            {
                position = new Vector2(-200, position.Y);
            }

            // Posodabljanje animacije
            if (!isStopped)
            {
                frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (frameTimer >= frameInterval)
                {
                    frameTimer = 0f;
                    currentFrame = (currentFrame + 1) % frames.Length;
                }
            }
        }

    public void Draw(SpriteBatch spriteBatch)
    {
            // Draw the police target based on its state
            Rectangle sourceRectangle = frames[currentFrame];
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

        if (isMovingLeft)
        {
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        }
        else
        {
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
        }
    }

    private void GenerateNewStopInterval()
    {
        stopInterval = (float)random.NextDouble() * (10.0f - 3.0f) + 3.0f;
    }

    public bool IsCollision(Splash splash)
    {
        Rectangle splashRect = new Rectangle((int)splash.Position.X, (int)splash.Position.Y, 100, 100);
        Rectangle policeTargetRect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

        return splashRect.Intersects(policeTargetRect);
    }


    }
}