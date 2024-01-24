using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class TextureManager
{
    private static TextureManager instance;

    public Texture2D ForegroundTexture { get; private set; }

    private TextureManager() { }

    public static TextureManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TextureManager();
            }
            return instance;
        }
    }

    public void LoadTextures(ContentManager content)
    {
        ForegroundTexture = content.Load<Texture2D>("img/slikovni-atlas-nov");
        // Dodajte druge teksture, ki jih potrebujete, glede na vaše zahteve
    }
}
