/************************************************************************************ 
 * Copyright (c) 2014, TU Ilmenau
 * 
 * Build with GoblinsXna framework und the Help from tutorial 8/10/12
 * GoblinsXna use:
 *                  XNA to create the 3D Scenes
 *                  ALVAR Framework to Augmented Reallity
 *                  Newton Physic Engine
 *                  OpenCV to Capture Images
 *                  DirectShow to Capture Images
 * Viel Dank guys
 * ===================================================================================
 * Authors:  Luis Rojas (luis-alejandro.rojas-vargas@tu-ilmenau.de) 
 *           Julian Castro (julian.castro-bosiso@tu-ilmenau.de)
 *           Ricardo Rieckhof (ricardo.rieckhof@tu-ilmenau.de)
 *************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/***** Namespaces to Audio *****/
using Microsoft.Xna.Framework.Audio;
using GoblinXNA;
using GoblinXNA.Sounds;

namespace CarGameAR
{
    class SoundManager
    {
        #region Member Fields

        private SoundEffectInstance GSound;
        private SoundEffectInstance MetSound;
        private SoundEffectInstance MenuSnd;
        private SoundEffectInstance CarCrash1Sound;
        private SoundEffectInstance CarCrash2Sound;
        private SoundEffectInstance CarCrash3Sound;
        private SoundEffectInstance AceleraSound;
        private SoundEffectInstance Muerte1Sound;
        private SoundEffectInstance Laser1Sound;
        private SoundEffectInstance Laser2Sound;
        private SoundEffectInstance SelectMenuSound;

        private SoundEffectInstance MusicLoop1;

        #endregion

        #region Constructor

        public SoundManager()
        {
        }

        #endregion

        #region Properties

        public SoundEffectInstance GroundSound
        {
            get { return GSound; }
        }

        public SoundEffectInstance MetalSound
        {
            get { return MetSound; }
        }

        public SoundEffectInstance MenuSound
        {
            get { return MenuSnd; }
        }

        public SoundEffectInstance SelectMenu
        {
            get { return SelectMenuSound; }
        }

        public SoundEffectInstance CarCrash1
        {
            get { return CarCrash1Sound; }
        }

        public SoundEffectInstance CarCrash2
        {
            get { return CarCrash2Sound; }
        }

        public SoundEffectInstance CarCrash3
        {
            get { return CarCrash3Sound; }
        }

        public SoundEffectInstance Acelera
        {
            get { return AceleraSound; }
        }

        public SoundEffectInstance Muerte1
        {
            get { return Muerte1Sound; }
        }

        public SoundEffectInstance Laser1
        {
            get { return Laser1Sound; }
        }

        public SoundEffectInstance Laser2
        {
            get { return Laser2Sound; }
        }

        public SoundEffectInstance MusicScene1
        {
            get { return MusicLoop1; }
        }

        #endregion

        #region Public Methods

        public void SoundLoad()
        {
            Console.WriteLine(DateTime.Now.ToString() + " Loading Sound\n");
            
            GSound = State.Content.Load<SoundEffect>("Sounds/Dirt_Sand_02").CreateInstance();
            MetSound = State.Content.Load<SoundEffect>("Sounds/metal_01").CreateInstance();
            MenuSnd = State.Content.Load<SoundEffect>("Sounds/Flyby_02").CreateInstance();
            SelectMenuSound = State.Content.Load<SoundEffect>("Sounds/selectmenu").CreateInstance();
            CarCrash1Sound = State.Content.Load<SoundEffect>("Sounds/carcrash1").CreateInstance();
            CarCrash2Sound = State.Content.Load<SoundEffect>("Sounds/carcrash2").CreateInstance();
            CarCrash3Sound = State.Content.Load<SoundEffect>("Sounds/carcrash3").CreateInstance();
            AceleraSound = State.Content.Load<SoundEffect>("Sounds/acelera").CreateInstance();
            Muerte1Sound = State.Content.Load<SoundEffect>("Sounds/muerte2").CreateInstance();
            Laser1Sound = State.Content.Load<SoundEffect>("Sounds/laser1").CreateInstance();
            Laser2Sound = State.Content.Load<SoundEffect>("Sounds/laser2").CreateInstance();

            MusicLoop1 = State.Content.Load<SoundEffect>("Sounds/esc1_1loop").CreateInstance();
        }

        #endregion
    }
}
