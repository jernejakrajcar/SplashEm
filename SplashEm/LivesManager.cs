using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashEm
{
    public class LivesManager
    {
        private int lives;

        public int Lives
        {
            get { return lives; }
        }

        public LivesManager(int initialLives)
        {
            lives = initialLives;
        }

        public void DecreaseLives()
        {
            lives--;
        }

        public void ResetLives()
        {
            lives = 3; // Nastavite življenja na začetno vrednost
        }
    }

}
