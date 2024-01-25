using Android.Telephony.Mbms;
using Java.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SplashEm.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Android.Content;
using System.Xml.Serialization;
using static Android.Icu.Text.Transliterator;
using Microsoft.Xna.Framework.Media;
using Javax.Security.Auth;

namespace SplashEm
{
    public class Game1 : Game
    {
        private GraphicsSettingsComponent _graphicsSettingsComponent;

        private GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Target target;

        private ScoreManager scoreManager;
        private LivesManager livesManager;

        private List<Rectangle> heartsRectangles = new List<Rectangle>();

        private bool isGameOver = false;
        private bool isPaused = false;

        private bool isSoundOn = true; // Privzeto stanje zvoka je vklopljeno
        private bool isMusicOn = true;

        private SoundEffect splashSoundEffect;
        private SoundEffectInstance splashSoundInstance; 

        private SoundEffect loseLifeSoundEffect;
        private SoundEffectInstance loseLifeSoundInstance;

        private SoundEffect gainPointsSoundEffect;
        private SoundEffectInstance gainPointsSoundInstance;

        private Song music;

        private Rectangle homeButtonRect; // Pravokotnik za gumb home
        private Rectangle settingsIconRect;  // Pravokotnik za ikono zvoka
        private Rectangle playButtonRect;
        private Rectangle soundIconRect = new Rectangle(230, 825, 200, 200);
        private Rectangle musicIconRect = new Rectangle(636, 825, 200, 200);
        private Rectangle backButtonRect = new Rectangle(428, 1340, 200, 180);
        private Rectangle gameDiffRect = new Rectangle(330, 1400, 420, 120);


        enum GameState
        {
            Playing,
            Paused,
            Menu,
            Settings
        }

        enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        private GameState gameState = GameState.Menu;

        private Difficulty currentDifficulty = Difficulty.Easy;

        public Game1()
        {
            _graphicsSettingsComponent = new GraphicsSettingsComponent(this);
            Components.Add(_graphicsSettingsComponent);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            LoadSoundSettings();
            livesManager = new LivesManager(3);
        }

        Texture2D background;
        Rectangle backgroundR;

        Texture2D menu;

        Texture2D home;

        Texture2D settings;

        Texture2D foreground;

        Texture2D splash_animation;

        Rectangle scoreR;

        Rectangle trophyR;

        Rectangle pauseR;

        Rectangle balonR;

        Rectangle numberR;

        Rectangle splashR;

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            backgroundR.X = 0;
            backgroundR.Y = 0;
            backgroundR.Width = 1080;
            backgroundR.Height = 2280;

            homeButtonRect = new Rectangle(300, 1100, 420, 190); // Pravokotnik za gumb home
            settingsIconRect = new Rectangle(20, 39, 260, 260); // Pravokotnik za ikono zvoka
            playButtonRect = new Rectangle(300, 1000, 420, 190);

            int initialLives = 3; // Število življenj na začetku
            for (int i = 0; i < initialLives; i++)
            {
                Rectangle heartRect = new Rectangle(5 + i * 140, 5, 150, 150);
                heartsRectangles.Add(heartRect);
            }

            scoreR.X = 750;           
            scoreR.Y = 5;
            scoreR.Width = 150;
            scoreR.Height = 150;

            trophyR.X = 900;
            trophyR.Y = 5;
            trophyR.Width = 150;
            trophyR.Height = 150;

            scoreManager = new ScoreManager();
            scoreManager.OnScoreUpdated += HandleScoreTexture;


            pauseR.X = 5;
            pauseR.Y = 160;
            pauseR.Width = 150;
            pauseR.Height = 150;

            balonR.X = 850;
            balonR.Y = 1980;
            balonR.Width = 180;
            balonR.Height = 180;

            numberR.X = 750;
            numberR.Y = 2000;
            numberR.Width = 150;
            numberR.Height = 150;

            splashR = new Rectangle(-200, -200, 150, 150);

            SplashManager.Init();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.Instance.LoadTextures(Content);
            foreground = TextureManager.Instance.ForegroundTexture;

            //nalaganje grafik
            background = Content.Load<Texture2D>("img/Splash-em-background");
            splash_animation = Content.Load<Texture2D>("img/splash-ani");

            home = Content.Load<Texture2D>("img/home-screen");
            menu = Content.Load<Texture2D>("img/menu-pause");
            settings = Content.Load<Texture2D>("img/settings-menu");


            //nalaganje zvočnih efektov
            splashSoundEffect = Content.Load<SoundEffect>("audio/splash"); 
            splashSoundInstance = splashSoundEffect.CreateInstance();

            loseLifeSoundEffect = Content.Load<SoundEffect>("audio/fail");
            loseLifeSoundInstance = loseLifeSoundEffect.CreateInstance();

            gainPointsSoundEffect = Content.Load<SoundEffect>("audio/coin-pickup"); 
            gainPointsSoundInstance = gainPointsSoundEffect.CreateInstance();

            //nalaganje pesmi
            music = Content.Load<Song>("audio/game-music-loop-2");
            if (isMusicOn)
            {
                MediaPlayer.Play(music);
            }
            MediaPlayer.IsRepeating = true;


            Vector2 targetPosition = new Vector2(250, 1570); // Set your initial position
            Vector2 targetVelocity = new Vector2(200, 0);   // Set your initial velocity

            // Create a new target
            target = new Target(foreground, targetPosition, targetVelocity, true);

            // Add the target to the manager
            TargetManager.AddTarget(target);


        }

        // Ob zagonu aplikacije ali obnovitvi nastavitev
        private void LoadSoundSettings()
        {
            try
            {
                // Uporaba SharedPreferences za preverjanje nastavitev zvoka
                var sharedPreferences = Android.App.Application.Context.GetSharedPreferences("SoundSettings", FileCreationMode.Private);
                var sharedPreferences2 = Android.App.Application.Context.GetSharedPreferences("MusicSettings", FileCreationMode.Private);
                isSoundOn = sharedPreferences.GetBoolean("SoundOn", true);
                isMusicOn = sharedPreferences2.GetBoolean("MusicOn", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sound settings: {ex.Message}");
            }
        }

        // Ob spremembi stanja zvoka
        private void SaveSoundSettings()
        {
            try
            {
                // Uporaba SharedPreferences za shranjevanje nastavitev zvoka
                var sharedPreferences = Android.App.Application.Context.GetSharedPreferences("SoundSettings", FileCreationMode.Private);
                var sharedPreferences2 = Android.App.Application.Context.GetSharedPreferences("MusicSettings", FileCreationMode.Private);
                var editor = sharedPreferences.Edit();
                editor.PutBoolean("SoundOn", isSoundOn);
                editor.Apply();
                var editor2 = sharedPreferences2.Edit();
                editor2.PutBoolean("MusicOn", isMusicOn);
                editor2.Apply();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving sound settings: {ex.Message}");
            }
        }

        private void GeneratePoliceTarget()
        {
            Vector2 policePosition = new Vector2(-300, 1000); // Set your initial position
            Vector2 policeVelocity = new Vector2(100, 0);   // Set your initial velocity
            Texture2D policeTargetTexture = Content.Load<Texture2D>("img/slikovni-atlas-new");


            // Create a new target
            PoliceTarget policeTarget = new PoliceTarget(policeTargetTexture, policePosition, policeVelocity, true);

            PoliceTargetManager.AddPoliceTarget(policeTarget);
            Console.WriteLine(PoliceTargetManager.PoliceTargets.Count);

        }

        private void SetupGameForDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    // Nastavitev za easy težavnost
                    break;

                case Difficulty.Medium:
                    // Nastavitev za medium težavnost
                    livesManager = new LivesManager(3);
                    // Dodajte policiste
                    GeneratePoliceTarget();
                    break;

                case Difficulty.Hard:
                    // Nastavitev za hard težavnost
                    livesManager = new LivesManager(3);
                    //dodaj tarče pametnega agenta!

                    // Dodajte policiste
                    GeneratePoliceTarget();
                    break;
            }
        }


        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    UpdatePlaying(gameTime);
                    break;
                case GameState.Paused:
                    UpdatePaused();
                    break;
                case GameState.Menu:
                    UpdateMenu();
                    break;
                case GameState.Settings:
                    UpdateSettings();
                    break;
            }

            base.Update(gameTime);
        }


        void UpdatePlaying(GameTime gameTime)
        {
            if (!isGameOver && !isPaused)
            {
                TouchCollection tc = TouchPanel.GetState();
                foreach (TouchLocation tl in tc)
                {
                    if (TouchLocationState.Pressed == tl.State)
                    {
                        if (!IsTouchOnPauseButton(tl.Position))
                        {
                            //Update the position of the touched element
                            splashR.X = (int)tl.Position.X - 50;
                            splashR.Y = (int)tl.Position.Y - 50;
                            // Create a new Splash object

                            Splash sp = new Splash(splash_animation, new Vector2(splashR.X, splashR.Y))
                            {
                                InitialAlpha = 1.0f, // Set the initial alpha value
                                FadeSpeed = 0.8f, // Set the fade speed
                            };
                            if (isSoundOn)
                            {
                                splashSoundInstance.Play();
                            }
                            //add the new splash object to the list
                            SplashManager.AddSplash(sp);
                        }
                        else
                        {
                            Console.WriteLine("Pause");
                            gameState = GameState.Paused;
                        }
                    }
                }

                // Update the target
                TargetManager.Update(gameTime);

                // Check for collisions between splashes and targets
                foreach (var splash in SplashManager.Splashes.ToList()) // Use ToList() to avoid modification during iteration
                {
                    splash.Update(gameTime);
                    foreach (var target in TargetManager.Targets.ToList()) // Use ToList() to avoid modification during iteration
                    {
                        if (IsCollision(splash, target))
                        {
                            TargetManager.HandleHit();
                            // Remove the splash and target
                            SplashManager.Splashes.Remove(splash);
                            TargetManager.Targets.Remove(target);
                            scoreManager.UpdateScore(1); // Dodajte toliko točk, kolikor želite ob vsakem zadetku tarče
                            if (isSoundOn)
                            {
                                gainPointsSoundInstance.Play();
                            }
                            break;
                        }
                    }

                    switch (currentDifficulty)
                    {
                        case Difficulty.Medium:
                            //handle police targets
                            foreach (var policeTarget in PoliceTargetManager.PoliceTargets.ToList())
                            {
                                if (policeTarget.IsCollision(splash))
                                {
                                    HandlePoliceHit(); // Dodajte kodo za obdelavo trčenja s policistom
                                    SplashManager.Splashes.Remove(splash); // Odstranite splash
                                }
                                break;
                            }
                            Console.WriteLine(PoliceTargetManager.PoliceTargets.Count);
                            Console.WriteLine("GLEDAM COLLISION S POLICISTOM!!!");
                            
                            break;
                        case Difficulty.Hard:
                            //handle police targets
                            foreach (var policeTarget in PoliceTargetManager.PoliceTargets.ToList())
                            {
                                if (policeTarget.IsCollision(splash))
                                {
                                    HandlePoliceHit(); // Dodajte kodo za obdelavo trčenja s policistom
                                    SplashManager.Splashes.Remove(splash); // Odstranite splash
                                }
                                break;
                            }
                            
                            //handle special targets
                            break;
                        default:
                            break;
                    }

                    
                }

                SplashManager.Update(gameTime);
                PoliceTargetManager.Update(gameTime);
            }
            if (isPaused)
            {
                HandleInput();
            }
            base.Update(gameTime);
        }

        private void UpdatePaused()
        {
            // Logika za pavzirano stanje
            // ...

            TouchCollection tc = TouchPanel.GetState();
            foreach (TouchLocation tl in tc)
            {
                if (TouchLocationState.Pressed == tl.State)
                {
                    // Preveri, ali je dotik znotraj pravokotnika gumba home
                    if (homeButtonRect.Contains(tl.Position))
                    {
                        // Uporabnik je kliknil gumb home, preklopi v meni
                        gameState = GameState.Menu;
                        ResetGame();
                    }
                    else
                    {
                        gameState = GameState.Playing;
                    }
                }
            }
        }

        private void ResetGame()
        {
            // Ponastavite vse potrebne spremenljivke in stanja na začetne vrednosti
            isGameOver = false;
            isPaused = false;
            scoreManager.ResetScore();
            livesManager.ResetLives();

            // Če želite, lahko tudi počistite seznam splash-ov, ciljev itd.
            SplashManager.Splashes.Clear();
            PoliceTargetManager.PoliceTargets.Clear();
        }

        private void UpdateMenu()
        { 

            TouchCollection tc = TouchPanel.GetState();
            foreach (TouchLocation tl in tc)
            {
                if (TouchLocationState.Pressed == tl.State)
                {
                    if (settingsIconRect.Contains(tl.Position))
                    {
                        //Uporabnik je kliknil ikono za nastavitve
                        gameState = GameState.Settings;
                    }

                    if (playButtonRect.Contains(tl.Position))
                    {
                        //Uporabnik je kliknil play
                        gameState = GameState.Playing;
                    }

                    if (gameDiffRect.Contains(tl.Position))
                    {
                        //Uporabnik hoče spremenit težavnost
                        switch (currentDifficulty)
                        {
                            case Difficulty.Easy:
                                currentDifficulty = Difficulty.Medium;
                                SetupGameForDifficulty(currentDifficulty);
                                break;
                            case Difficulty.Medium:
                                currentDifficulty = Difficulty.Hard;
                                SetupGameForDifficulty(currentDifficulty);
                                break;
                            case Difficulty.Hard:
                                currentDifficulty = Difficulty.Easy;
                                SetupGameForDifficulty(currentDifficulty);
                                break;
                        }

                    }
                    
                }
            }
        }

        private void UpdateSettings()
        {
            // Logika za nastavitve

            TouchCollection tc = TouchPanel.GetState();
            foreach (TouchLocation tl in tc)
            {
                if (TouchLocationState.Pressed == tl.State)
                {
                    if (soundIconRect.Contains(tl.Position))
                    {
                        // Uporabnik je kliknil ikono zvoka, preklopi stanje zvoka
                        isSoundOn = !isSoundOn;
                        SaveSoundSettings(); // Shranite nastavitve zvoka
                    }

                    if (musicIconRect.Contains(tl.Position))
                    {
                        if (isMusicOn)
                        {
                            MediaPlayer.Stop();
                            isMusicOn = false;
                        }
                        else
                        {
                            MediaPlayer.Play(music);
                            isMusicOn = true;
                        }
                        SaveSoundSettings();
                    }

                    if (backButtonRect.Contains(tl.Position))
                    {
                        //Uporabnik je kliknil play
                        gameState = GameState.Menu;
                    }
                }
            }
        }


        // Helper method to check for collision between splash and target
        private bool IsCollision(Splash splash, Target target)
        {
            Rectangle splashRect = new Rectangle((int)splash.Position.X, (int)splash.Position.Y, 100, 100);
            Rectangle targetRect = new Rectangle((int)target.Position.X, (int)target.Position.Y, (int)target.size.X, (int)target.size.Y);
            
            return splashRect.Intersects(targetRect);
        }

        private void HandlePoliceHit()
        {
            
            livesManager.DecreaseLives();
            
            if (livesManager.Lives <= 0)
            {
                //konec igre, zgubil si!!!
                Console.WriteLine("Zgubil si!!!");
                heartsRectangles.RemoveAt(heartsRectangles.Count - 1);
                if (isSoundOn)
                {
                    loseLifeSoundInstance.Play();
                }
                isGameOver = true;
            }
            else
            {
                // Če še niste izgubili vseh življenj, odstranite en srček
                heartsRectangles.RemoveAt(heartsRectangles.Count - 1);
                if (isSoundOn)
                {
                    loseLifeSoundInstance.Play();
                }
            }
        }



        private bool IsTouchOnPauseButton(Vector2 touchPosition)
        {
            // Preverite, ali dotik pade na območje gumba za pavzo
            Console.WriteLine("Touched");
            return pauseR.Contains(touchPosition);
        }

        private void HandleInput()
        {
            TouchCollection tc = TouchPanel.GetState();

            foreach (TouchLocation tl in tc)
            {
                if (TouchLocationState.Pressed == tl.State)
                {
                    if (pauseR.Contains(tl.Position))
                    {
                        // Uporabnik je kliknil gumb za pavzo
                        isPaused = !isPaused; // Zamenjajte stanje pavze
                    }
                    else
                    {
                        // Uporabnik je kliknil drugam na zaslonu (npr. izvedel dotik za splash)
                        // Dodajte kodo za dotik na drugih delih zaslona
                    }
                }
            }
        }



        private void HandleScoreTexture(int newScore)
        {
            // Ustvarite novo teksturo na osnovi trenutnega števila zadetkov
            scoreR = new Rectangle(900, 5, 150, 150); // Nastavite ustrezne koordinate
            int digitWidth = 150; // Širina ene številke
            int digitSpacing = 1; // Razmik med številkami
            int scoreWidth = (newScore.ToString().Length * digitWidth) + ((newScore.ToString().Length - 1) * digitSpacing);

            // Uporabite ustrezno koordinato Y glede na vaš dizajn
            scoreR.X = 1080 - scoreWidth - 150; // Privzeti odmik od desnega roba zaslona
        }


        private Rectangle GetSourceRectangleForDigit(int digit)
        {
            int digitWidth = 100; // Širina ene številke
            int digitHeight = 100; // Višina ene številke

            int sourceX = digitWidth * digit;
            int sourceY = 0;

            return new Rectangle(sourceX, sourceY, digitWidth, digitHeight);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Playing:
                    // Risanje igranja
                    _spriteBatch.Draw(background, backgroundR, Color.White);
                    Rectangle sourceR = new Rectangle(0, 100, 100, 100);

                    foreach (var heartRect in heartsRectangles)
                    {
                        _spriteBatch.Draw(foreground, heartRect, sourceR, Color.White);
                    }
                    _spriteBatch.Draw(foreground, scoreR, GetSourceRectangleForDigit(scoreManager.score), Color.White);


                    Rectangle sourceScore = new Rectangle(200, 100, 100, 100);
                    _spriteBatch.Draw(foreground, trophyR, sourceScore, Color.White);

                    Rectangle sourcePause = new Rectangle(100, 100, 100, 100);
                    _spriteBatch.Draw(foreground, pauseR, sourcePause, Color.White);
                    Rectangle sourceBalon = new Rectangle(300, 100, 100, 100);
                    _spriteBatch.Draw(foreground, balonR, sourceBalon, Color.White);

                    Rectangle source0 = new Rectangle(900, 0, 100, 100);
                    _spriteBatch.Draw(foreground, numberR, source0, Color.Aqua);
                    Rectangle source2 = new Rectangle(100, 0, 100, 100);
                    _spriteBatch.Draw(foreground, new Rectangle(650, 2000, 150, 150), source2, Color.Aqua);

                    // Draw the target
                    TargetManager.Draw(_spriteBatch);


                    SplashManager.Draw(_spriteBatch);

                    if(currentDifficulty == Difficulty.Medium || currentDifficulty == Difficulty.Hard)
                    {
                        PoliceTargetManager.Draw(_spriteBatch);
                    }
                    

                    /* tukaj je koda za preverjanje detekcije trkov med objekti:
                    // Risanje AABB okoli Splash objektov
                    foreach (var splash in SplashManager.Splashes)
                    {
                        Rectangle splashRect = new Rectangle((int)splash.Position.X, (int)splash.Position.Y, 170, 125);
                        _spriteBatch.Draw(menu, splashRect, new Rectangle(0, 0, 1080, 1920), Color.Blue*0.5f); // Dodajte metodo za risanje pravokotnika
                    }

                    // Risanje AABB okoli Target objektov
                    foreach (var target in TargetManager.Targets)
                    {
                        Rectangle targetRect = new Rectangle((int)target.Position.X, (int)target.Position.Y, (int)target.size.X, (int)target.size.Y);
                        _spriteBatch.Draw(menu, targetRect, new Rectangle(0, 0, 1080, 1920), Color.Orange * 0.5f); // Dodajte metodo za risanje pravokotnika
                    }

                    // Risanje AABB okoli PoliceTarget objektov
                    foreach (var policeTarget in PoliceTargetManager.PoliceTargets)
                    {
                        Rectangle policeTargetRect = new Rectangle((int)policeTarget.position.X, (int)policeTarget.position.Y, (int)policeTarget.size.X, (int)policeTarget.size.Y);
                        _spriteBatch.Draw(menu, policeTargetRect, new Rectangle(0, 0, 1080, 1920), Color.Cyan * 0.5f); // Dodajte metodo za risanje pravokotnika
                    }*/
                    break;
                case GameState.Paused:
                    // Risanje pavza stanja
                    Rectangle sourceMenu = new Rectangle(0, 0, 1080, 1920);
                    _spriteBatch.Draw(menu, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), sourceMenu, Color.White);
                    break;
                case GameState.Menu:
                    // Risanje menija
                    Rectangle sourceHome = new Rectangle(0, 0, 1080, 1920);
                    _spriteBatch.Draw(home, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), sourceHome, Color.White);
                    switch (currentDifficulty)
                    {
                        case Difficulty.Easy:
                            Rectangle sourceEasy = new Rectangle(0, 800, 400, 100);
                            _spriteBatch.Draw(foreground, gameDiffRect, sourceEasy, Color.White);
                            break;
                        case Difficulty.Medium:
                            Rectangle sourceMedium = new Rectangle(0, 700, 400, 100);
                            _spriteBatch.Draw(foreground, gameDiffRect, sourceMedium, Color.White);
                            break;
                        case Difficulty.Hard:
                            Rectangle sourceHard = new Rectangle(0, 900, 400, 100);
                            _spriteBatch.Draw(foreground, gameDiffRect, sourceHard, Color.White);
                            break;
                    }
                    break;
                case GameState.Settings:
                    Rectangle sourceSettings = new Rectangle(0, 0, 1080, 1920);
                    _spriteBatch.Draw(settings, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), sourceSettings, Color.White);

                    //_spriteBatch.Draw(menu, backButtonRect, new Rectangle(0, 0, 1080, 1920), Color.Blue * 0.5f);

                    if (!isSoundOn)
                    {
                        Rectangle sourceMute = new Rectangle(100, 600, 100, 100);
                        _spriteBatch.Draw(foreground, soundIconRect, sourceMute, Color.White);
                    }
                    else if (isSoundOn)
                    {
                        Rectangle sourceUnmute = new Rectangle(0, 600, 100, 100);
                        _spriteBatch.Draw(foreground, soundIconRect, sourceUnmute, Color.White);
                    }
                    if (!isMusicOn)
                    {
                        Rectangle sourceMute = new Rectangle(300, 600, 100, 100);
                        _spriteBatch.Draw(foreground, musicIconRect, sourceMute, Color.White);
                    }
                    else if (isMusicOn)
                    {
                        Rectangle sourceUnmute = new Rectangle(200, 600, 100, 100);
                        _spriteBatch.Draw(foreground, musicIconRect, sourceUnmute, Color.White);
                    }
                    break;
            }

            


            _spriteBatch.End();

            

            base.Draw(gameTime);
        }
    }
}