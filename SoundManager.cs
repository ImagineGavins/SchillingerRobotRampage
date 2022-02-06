using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Schillinger_RobotRampage
{
    class SoundManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region ~Sounds~
        SoundEffect shotSE;
        SoundEffect explosionSE;

        SoundEffectInstance shotSEI;
        SoundEffectInstance explosionSEI;

        public Dictionary<string, SoundEffectInstance> sounds;
        #endregion

        public SoundManager(Game game)
            : base(game) { }

        protected override void LoadContent()
        {
            sounds = new Dictionary<string, SoundEffectInstance>();

            shotSE = ((Game1)Game).Content.Load<SoundEffect>(@"Sounds/XWing fire");
            // explosionSE = ((Game1)Game).Content.Load<SoundEffect>(@"");

            shotSEI = shotSE.CreateInstance();
            // explosionSEI = explosionSE.CreateInstance();

            sounds.Add("shot", shotSEI);
            sounds.Add("explosion", explosionSEI);
        }
    }
}
