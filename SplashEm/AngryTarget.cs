using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.Transliterator;

namespace SplashEm
{
    public class AngryTarget : Sprite
    {
        private Vector2 velocity; // Hitrost in smer gibanja
        public Vector2 size;

        private Rectangle[] frames; // Polje pravokotnikov, ki predstavljajo animacijske okvirje
        private int currentFrame;   // Kazalec trenutnega okvirja
        private float frameTimer;   // Časovnik za nadzor preklopa okvira
        private float frameInterval = 0.2f; // Časovni interval med menjavo okvira
        private bool IsMovingLeft;

        public enum AngryTargetState
        {
            Moving,
            LookingUp
        }

        public AngryTargetState currentState;

        private int currentLife;
        private bool isHit;

        public AngryTarget(Texture2D texture, Vector2 position, Vector2 velocity, bool IsMovingLeft) : base(texture)
        {
            this.velocity = velocity;
            this.Position = position;
            this.IsMovingLeft = IsMovingLeft;

            this.size = new Vector2(180, 350);

            // Inicializacija polja okvirov s specifičnimi pravokotniki
            frames = new Rectangle[3];
            frames[0] = new Rectangle(0, 200, 90, 200);
            frames[1] = new Rectangle(90, 200, 90, 200);
            frames[2] = new Rectangle(200, 200, 90, 200);

            // Privzeto stanje je "moving"
            currentState = AngryTargetState.Moving;

            currentLife = 1; // Nastavite število življenj
            isHit = false;
        }

        public void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case AngryTargetState.Moving:
                    UpdateMoving(gameTime);
                    break;

                case AngryTargetState.LookingUp:
                    UpdateLookingUp(gameTime);
                    break;
            }
        }

        public void UpdateMoving(GameTime gameTime)
        {
            // Posodobitev položaja glede na hitrost in čas
            Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Posodobitev animacijskega okvirja
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
            }

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
                    Position = new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth + 200, Position.Y);
                }
            }

            if (isHit)
            {
                SwitchToLookingUpState();
            }
        }

        public void SwitchToLookingUpState()
        {
            currentState = AngryTargetState.LookingUp;
            velocity = Vector2.Zero;
            lookingUpTimer = 0.0f;
        }
        private float lookingUpTimer; // Spremenljivka za sledenje pretečenega časa v stanju "looking up"
        private float lookingUpDuration = 2.0f; // Čas v sekundah, koliko časa naj traja stanje "looking up"

        private void UpdateLookingUp(GameTime gameTime)
        {
            currentFrame = 0;

            // Dodajte logiko za prehod nazaj v stanje "moving", če je to potrebno
            // Povečajte časovnik za pretečeni čas
            lookingUpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Preverite, ali je pretečeni čas večji od želenega trajanja
            if (lookingUpTimer >= lookingUpDuration)
            {
                // Če je pretečeni čas dosegel ali presegel želeno trajanje, preklopite nazaj v stanje "moving"
                currentState = AngryTargetState.Moving;
                this.velocity = new Vector2(200, 0);
            }
        }

        public bool IsCollision(Splash splash)
        {
            Rectangle splashRect = new Rectangle((int)splash.Position.X, (int)splash.Position.Y, 100, 100);
            Rectangle angryTargetRect = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);

            return splashRect.Intersects(angryTargetRect);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = frames[currentFrame];

            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);

            if (IsMovingLeft)
            {
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.Green);
            }
            else
            {
                // Zrcaljenje po vertikalni osi
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.Green, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            }
        }
    }

}
