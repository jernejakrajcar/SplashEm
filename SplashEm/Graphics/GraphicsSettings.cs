using Microsoft.Xna.Framework;

namespace SplashEm.Graphics
{
    public class GraphicsSettingsComponent : DrawableGameComponent
    {
        private GraphicsDeviceManager _graphics;

        public GraphicsSettingsComponent(Game game) : base(game)
        {
            _graphics = new GraphicsDeviceManager(game);
            InitializeGraphicsSettings();
        }

        private void InitializeGraphicsSettings()
        {
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.PreferredBackBufferHeight = 2280;
            // Dodajte druge grafične nastavitve, če jih imate
        }

        public override void Initialize()
        {
            // Dodajte kodo inicializacije, če je potrebno
            base.Initialize();
        }

        // Dodajte druge metode ali lastnosti, če so potrebne
    }
}
