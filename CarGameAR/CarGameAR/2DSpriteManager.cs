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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;
using GoblinXNA.SceneGraph;

namespace CarGameAR
{
    class My2DSpriteManager
    {
        #region GameLogic Enums

        public enum Game_Menus
        {
            MODE_MENU,
            MODEGAME_MENU,
            PAUSE_MENU,
        }

        #endregion

        #region Member Fields

        Game_Logic GameLogic;                               //Class to Control the Logic states for the Game 

        // Textures for the Game
        private Texture2D GameTitle;                        //Texture for the title
        private Texture2D GameLogo;                         //Texture for the Logo
        private Texture2D[] CarsTextures;                   //Car´sTextures for Menu
        private Texture2D[] BoxStatusTx;                    //Texture for the Box Status

        private Texture2D[] MenusSprites;                   //Texture for menus
        private Texture2D[] MenuMPlaySprites;               //Texture for menus
        private Texture2D[] MenuMPauseSprites;              //Texture for menus
        private Texture2D[] Levelsprites;                   //Texture for menus
        private Texture2D[] MenuLevelsprites;               //Texture for menus
        private Texture2D[] MenuEndGame;                    //Texture for menus
        private Texture2D[] MenuTourt;                      //Texture for menus
        private Texture2D[] MenuHistory;                    //Texture for menus
        private Texture2D[] Credits;                        //Texture for menus
        private Texture2D Luna;                             //Texture for menus
        private Texture2D Metal;                            //Texture for menus
        private Texture2D Marco;                            //Texture for menus
        private Texture2D Muerte;                           //Texture for menus
        private Texture2D Loading;                          //Texture for menus
        private Texture2D Trofeo;                           //Texture for menus

        //Sprite Fonts
        private SpriteFont DefaultFont;                     //DefaultFont
        private SpriteFont CalibriFont;                     //CalibriFont
        private SpriteFont PuntosFont;                      //rastmoFont
        private SpriteFont RoboCopFont;                     //RobocopFont
        private SpriteFont BladeFont;                       //Blade RunnerFont
        private SpriteFont CasioFont;                       //CasioFont

        //Data for Text fot the title
        private string TextTitle = "Hola";
        private string TextMsg = "Hallo";
        private string TextIP = "000.000.000.000";
        private string TextRobert = "ROBERT SWANSON SAYS:";
        private Color TxtTitleCol = Color.Black;            //Color text
        private int AlfaColor = 255;

        //From Menu
        private Vector2[] PosObjMenu;                       //Pos each Frame in Menu
        int FrameW = 250;
        int FrameH = 187;

        #endregion

        #region Properties

        public string ServerIp
        {
            get { return TextIP; }
            set { TextIP = value; }
        }

        public string TextMessage
        {
            get { return TextMsg; }
            set { TextMsg = value; }
        }

        #endregion

        #region Constructors

        public My2DSpriteManager(Game_Logic GameLogic)
        {
            //Asination Logic Obj
            this.GameLogic = GameLogic;

            //Instantiate Texture for Menu
            CarsTextures = new Texture2D[8];

            //Instatiate For Pos Menu
            PosObjMenu = new Vector2[8];

            //instancio Texturas para Box Status
            BoxStatusTx = new Texture2D[3];

            MenusSprites = new Texture2D[4];
            MenuMPlaySprites = new Texture2D[3];
            MenuMPauseSprites = new Texture2D[2];
            MenuLevelsprites = new Texture2D[3];
            MenuEndGame = new Texture2D[3];
            MenuTourt = new Texture2D[2];
            MenuHistory = new Texture2D[8];
            Credits = new Texture2D[3];

            Levelsprites = new Texture2D[3];
        }

        #endregion

        #region Override Methods

        public void LoadContent()
        {
            int x;

            Console.WriteLine(DateTime.Now.ToString() + " Loading Sprites\n");
            
            //Load Normal Font
            DefaultFont = State.Content.Load<SpriteFont>(@"Fonts/Sample");
            CalibriFont = State.Content.Load<SpriteFont>(@"Fonts/Calibri");
            PuntosFont = State.Content.Load<SpriteFont>(@"Fonts/Rastmo");
            RoboCopFont = State.Content.Load<SpriteFont>(@"Fonts/RoboCop");
            BladeFont = State.Content.Load<SpriteFont>(@"Fonts/Blade");
            CasioFont = State.Content.Load<SpriteFont>(@"Fonts/Casio");

            //load Sprite for Title
            GameLogo = State.Content.Load<Texture2D>(@"Sprites/logo");
            GameTitle = State.Content.Load<Texture2D>(@"Sprites/titulo");

            //Load BoxStatus 2D textures
            BoxStatusTx[0] = State.Content.Load<Texture2D>(@"Sprites/AzulIz");
            BoxStatusTx[1] = State.Content.Load<Texture2D>(@"Sprites/AzulCe");
            BoxStatusTx[2] = State.Content.Load<Texture2D>(@"Sprites/AzulDe");

            MenusSprites[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/multiplayer");
            MenusSprites[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/story");
            MenusSprites[2] = State.Content.Load<Texture2D>(@"Sprites/Menus/credits");
            MenusSprites[3] = State.Content.Load<Texture2D>(@"Sprites/Menus/credits");
            MenuMPlaySprites[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/Deathmatch");
            MenuMPlaySprites[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/Tournament");
            MenuMPlaySprites[2] = State.Content.Load<Texture2D>(@"Sprites/Menus/Timetrial");
            MenuMPauseSprites[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/Resumegame");
            MenuMPauseSprites[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/Endgame");
            MenuLevelsprites[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/Moon_Th");
            MenuLevelsprites[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/Arena");
            MenuLevelsprites[2] = State.Content.Load<Texture2D>(@"Sprites/Menus/Base");
            MenuEndGame[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/rematch");
            MenuEndGame[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/newgame");
            MenuEndGame[2] = State.Content.Load<Texture2D>(@"Sprites/Menus/mainmenu");
            MenuTourt[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/nextlevel");
            MenuTourt[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/exitgame");

            Luna = State.Content.Load<Texture2D>(@"Sprites/Menus/luna");
            Metal = State.Content.Load<Texture2D>(@"Sprites/Menus/Metal");
            Marco = State.Content.Load<Texture2D>(@"Sprites/Menus/marco");
            Muerte = State.Content.Load<Texture2D>(@"Sprites/Menus/DEAD");
            Loading = State.Content.Load<Texture2D>(@"Sprites/Loading");
            Trofeo = State.Content.Load<Texture2D>(@"Sprites/Menus/Trofeo");

            Levelsprites[0] = State.Content.Load<Texture2D>(@"Sprites/Menus/Level1");
            Levelsprites[1] = State.Content.Load<Texture2D>(@"Sprites/Menus/Level2");
            Levelsprites[2] = State.Content.Load<Texture2D>(@"Sprites/Menus/Level3");

            //Load Sprites for menu
            for (x = 0; x < CarsTextures.Length; x++)
            {
                CarsTextures[x] = State.Content.Load<Texture2D>(@"Sprites/CarMenu"+(x+1).ToString());
            }
            
            for (x = 0; x < MenuHistory.Length; x++)
            {
                MenuHistory[x] = State.Content.Load<Texture2D>(@"Sprites/History/MenuStory" + (x + 1).ToString());
            }

            for (x = 0; x < Credits.Length; x++)
            {
                Credits[x] = State.Content.Load<Texture2D>(@"Sprites/Credits/creditos" + (x + 1).ToString());
            }
        }

        public void Draw(GameTime gameTime)
        {
            switch (GameLogic.CurrentGameState)
            {
                case Game_Logic.Game_States.INTRO:
                    CreateIntro();
                    break;
                case Game_Logic.Game_States.END_GAME:
                    CreateEndGame();
                    break;
                case Game_Logic.Game_States.HISTORY:
                    CreateHistory();
                    break;
                case Game_Logic.Game_States.MENU_MODE:
                    CreateMenuMode();
                    break;
                case Game_Logic.Game_States.MODE_PLAY:
                    CreateMenuPlayMode();
                    break;
                case Game_Logic.Game_States.SEL_PLY:
                    if(GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                        CreateMenuPly();
                    break;
                case Game_Logic.Game_States.LEVEL_SEL:
                    CreateMenuSelLevel();
                    break;
                case Game_Logic.Game_States.BUILDING:
                    CreateBuilding();
                    break;
                case Game_Logic.Game_States.PLAY:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        //Put the Status Box for each Player
                        CreateBoxStatus(new Vector2(5, 5), GameLogic.Players[0]);
                        if(GameLogic.Players[1].IsConnected)
                            CreateBoxStatus(new Vector2(1050, 5), GameLogic.Players[1]);
                        if (GameLogic.Players[2].IsConnected)
                            CreateBoxStatus(new Vector2(5, 650), GameLogic.Players[2]);
                        if (GameLogic.Players[3].IsConnected)
                            CreateBoxStatus(new Vector2(1050, 650), GameLogic.Players[3]);
                    }
                    break;
                case Game_Logic.Game_States.POINTS_MENU:
                    CreateRanking();
                    break;
                case Game_Logic.Game_States.POINTS_MENU_T:
                    CreateRankingTour();
                    break;
                case Game_Logic.Game_States.PAUSE:
                    CreateMenuPause();
                    break;
                case Game_Logic.Game_States.CREDITS:
                    CreateCredits();
                    break;
            }

            if(GameLogic.IsPaused == true)
                CreateMenuPause();

            if (GameLogic.StateScreen == Game_Logic.Game_SCR.GAME_LOADING)
                CreateLoading();

            //PutGameData(1010, 540);
        }

        #endregion

        #region Public Methods

        public void LoadDateMenu()
        {
            int OffsetX = 60;
            int OffsetY = 150;
            int Space = 50;
            int x, y, PosY, PosX, Idx = 0;

            TextTitle = "Select Tour car";

            for (y = 0; y < 2; y++)
            {
                for (x = 0; x < 4; x++)
                {
                    //Calculo Pos de X
                    PosX = OffsetX + Space * x + FrameW * x;
                    PosY = OffsetY + Space * y + FrameH * y;

                    PosObjMenu[Idx] = new Vector2(PosX, PosY);
                    Idx++;
                }
            }
        }

        public void CreateLoading()
        {
            //TextTitle = "PAUSE GAME";

            UI2DRenderer.FillRectangle(new Rectangle(540, 200, Loading.Width, Loading.Height), Loading, Color.White);

            // Draw a 2D text string at the center of the screen
            //UI2DRenderer.WriteText(Vector2.Zero, TextTitle, TxtTitleCol,
            //    PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Top);
        }

        public void CreateBuilding()
        {
            TxtTitleCol = new Color(255, 10, 10, AlfaColor);
            AlfaColor -= 3;
            if (AlfaColor < 0)
                AlfaColor = 255;
            // texto en Construccion
            UI2DRenderer.WriteText(new Vector2(10,5), "Building", TxtTitleCol, BladeFont);
        }

        public void CreateMenuPly()
        {
            int Idx;
            string NumberPly;

            TextTitle = "Select Your Player";

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);

            for (Idx = 0; Idx < 8; Idx++ )
            {
                UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X, (int)PosObjMenu[Idx].Y, FrameW, FrameH), CarsTextures[Idx], Color.White);
                UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X - 15, (int)PosObjMenu[Idx].Y - 15, FrameW + 30, FrameH + 30), Marco, Color.White);
            }

            for (Idx = 0; Idx < GameLogic.NumberPlayers; Idx++)
            {
                UI2DRenderer.DrawRectangle(new Rectangle((int)PosObjMenu[GameLogic.Players[Idx].PlySelect - 1].X, 
                    (int)PosObjMenu[GameLogic.Players[Idx].PlySelect - 1].Y, 
                    FrameW, 
                    FrameH),
                    GameLogic.Players[Idx].PlayerColor, 5);

                if (!GameLogic.Players[Idx].IsPlyOK)
                    NumberPly = (Idx + 1).ToString();
                else
                    NumberPly = "X";
                //Player Number
                UI2DRenderer.WriteText(new Vector2(PosObjMenu[GameLogic.Players[Idx].PlySelect - 1].X + 8,
                    PosObjMenu[GameLogic.Players[Idx].PlySelect - 1].Y + 2),
                    NumberPly,
                    GameLogic.Players[Idx].PlayerColor,
                    BladeFont);
            }
 
            /******** text For the Screen ********/
            UI2DRenderer.WriteText(new Vector2(0, 50), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(1050, 680), TextIP, new Color(255, 10, 10, 255),
                CalibriFont, new Vector2(0.8f, 0.8f));

            UI2DRenderer.WriteText(new Vector2(0, 610), TextMsg, new Color(255, 10, 10, 255),
                BladeFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
        }

        public void CreateRanking()
        {
            int Idx;
            Idx = (int)GameLogic.EndActionMenu;

            TextTitle = "Battle Ranking";

            //Animation
            if (TxtTitleCol == Color.Red)
                TxtTitleCol = Color.Yellow;
            else
                TxtTitleCol = Color.Red;

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);
            UI2DRenderer.FillRectangle(new Rectangle(560, 560, MenuEndGame[Idx].Width, MenuEndGame[Idx].Height), MenuEndGame[Idx], Color.White);

            for (Idx = 0; Idx < GameLogic.NumberPlayers; Idx++)
            {
                UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X, 220, FrameW, FrameH), CarsTextures[GameLogic.Players[Idx].PlySelect - 1], Color.White);
                UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X - 15, 220 - 15, FrameW + 30, FrameH + 30), Marco, Color.White);
                
                if(GameLogic.Players[Idx].IsDead == true)
                    UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X - 15, 220 - 15, FrameW + 30, FrameH + 30), Muerte, Color.White);
                else
                    UI2DRenderer.WriteText(new Vector2((int)PosObjMenu[Idx].X - 5, 160), "Winner!", TxtTitleCol, PuntosFont);

                UI2DRenderer.WriteText(new Vector2((int)PosObjMenu[Idx].X + 10, 425), "Deaths: " + GameLogic.Players[Idx].NumDeads, Color.Red, BladeFont);
                UI2DRenderer.WriteText(new Vector2((int)PosObjMenu[Idx].X + 10, 460), "Kills: " + GameLogic.Players[Idx].NumKills, Color.Red, BladeFont);
            }

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0, 50), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
        }

        public void CreateRankingTour()
        {
            int Idx;
            Idx = (int)GameLogic.EndActionTour;

            TextTitle = "Tournament Ranking";

            //Animation
            if (TxtTitleCol == Color.Red)
                TxtTitleCol = Color.Yellow;
            else
                TxtTitleCol = Color.Red;

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);
            UI2DRenderer.FillRectangle(new Rectangle(560, 560, MenuTourt[Idx].Width, MenuTourt[Idx].Height), MenuTourt[Idx], Color.White);

            for (Idx = 0; Idx < GameLogic.NumberPlayers; Idx++)
            {
                UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X, 220, FrameW, FrameH), CarsTextures[GameLogic.Players[Idx].PlySelect - 1], Color.White);
                UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X - 15, 220 - 15, FrameW + 30, FrameH + 30), Marco, Color.White);

                if (GameLogic.Players[Idx].IsDead == true)
                    UI2DRenderer.FillRectangle(new Rectangle((int)PosObjMenu[Idx].X - 15, 220 - 15, FrameW + 30, FrameH + 30), Muerte, Color.White);
                else
                    UI2DRenderer.WriteText(new Vector2((int)PosObjMenu[Idx].X - 5, 160), "Winner!", TxtTitleCol, PuntosFont);

                UI2DRenderer.WriteText(new Vector2((int)PosObjMenu[Idx].X + 10, 425), "Deaths: " + GameLogic.Players[Idx].NumDeads, Color.Red, BladeFont);
                //UI2DRenderer.WriteText(new Vector2((int)PosObjMenu[Idx].X + 10, 460), "Kills: " + GameLogic.Players[Idx].NumKills, Color.Red, BladeFont);
            }

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0, 50), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
        }

        public void CreateEndGame()
        {
            int Idx;

            TextTitle = "Congratulations!!!";

            //Animation
            if (TxtTitleCol == Color.Red)
                TxtTitleCol = Color.Yellow;
            else
                TxtTitleCol = Color.Red;

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);
            UI2DRenderer.FillRectangle(new Rectangle(260, 440, Trofeo.Width, Trofeo.Height), Trofeo, Color.White);
            
            for (Idx = 0; Idx < GameLogic.NumberPlayers; Idx++)
            {
                
                if (GameLogic.Players[Idx].IsDead == false)
                {
                    UI2DRenderer.WriteText(new Vector2(210 - 5, 160), "Winner!", TxtTitleCol, PuntosFont);

                    UI2DRenderer.FillRectangle(new Rectangle(210, 220, FrameW, FrameH), CarsTextures[GameLogic.Players[Idx].PlySelect - 1], Color.White);
                    UI2DRenderer.FillRectangle(new Rectangle(210 - 15, 220 - 15, FrameW + 30, FrameH + 30), Marco, Color.White);

                    UI2DRenderer.FillRectangle(new Rectangle(600, 250, 600, 300), null, new Color(0, 0, 0, 255));

                    UI2DRenderer.WriteText(new Vector2(610, 255), GameLogic.HistoryEndgame[GameLogic.Players[Idx].PlySelect - 1], Color.LawnGreen,
                        CasioFont, new Vector2(0.75f, 0.75f));
                }
            }

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0, 50), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(480, 620), "Continue", TxtTitleCol,
                BladeFont, new Vector2(1.0f, 1.0f));
        }

        public void CreateHistory()
        {
            int Idx = GameLogic.CharHistory;

            TextTitle = GameLogic.CharacterNames[Idx];

            //Animation
            if (TxtTitleCol == Color.Red)
                TxtTitleCol = Color.Yellow;
            else
                TxtTitleCol = Color.Red;

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);
            UI2DRenderer.FillRectangle(new Rectangle(50, 190, MenuHistory[Idx].Width, MenuHistory[Idx].Height), MenuHistory[Idx], Color.White);

            UI2DRenderer.FillRectangle(new Rectangle(255, 220, FrameW, FrameH), CarsTextures[Idx], Color.White);
            UI2DRenderer.FillRectangle(new Rectangle(255 - 15, 220 - 15, FrameW + 30, FrameH + 30), Marco, Color.White);

            UI2DRenderer.FillRectangle(new Rectangle(600, 140, 440, 535), null, new Color(0, 0, 0, 255));

            UI2DRenderer.WriteText(new Vector2(610, 155), GameLogic.HistoryPers[Idx], Color.LawnGreen,
                CasioFont, new Vector2(0.75f, 0.75f));

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0, 50), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(450, 670), "Continue", TxtTitleCol,
                BladeFont, new Vector2(1.0f, 1.0f));
        }

        public void CreateMenuMode()
        {
            int Idx = (int)GameLogic.GameMode;

            TextTitle = "Select your game mode";

            //Velo
            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));

            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);
            UI2DRenderer.FillRectangle(new Rectangle(450, 200, MenusSprites[Idx].Width, MenusSprites[Idx].Height), MenusSprites[Idx], Color.White);

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0,80), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(0, 55), TextRobert, Color.Gray,
                RoboCopFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(1050, 680), TextIP, new Color(255, 10, 10, 255),
                CalibriFont, new Vector2(0.8f, 0.8f));
        }

        public void CreateMenuPlayMode()
        {
            int Idx = (int)GameLogic.MultiplayerMode;

            TextTitle = "Select the Multiplayer mode";

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);

            UI2DRenderer.FillRectangle(new Rectangle(450, 170, MenuMPlaySprites[Idx].Width, MenuMPlaySprites[Idx].Height), MenuMPlaySprites[Idx], Color.White);

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0, 90), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(0, 55), TextRobert, Color.Gray,
                RoboCopFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
        }

        public void CreateMenuSelLevel()
        {
            int Idx = (int)GameLogic.ActualLevel;

            TextTitle = "Select your level";

            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));

            UI2DRenderer.FillRectangle(new Rectangle(30, 180, MenuLevelsprites[Idx].Width, MenuLevelsprites[Idx].Height), MenuLevelsprites[Idx], Color.White);

            UI2DRenderer.FillRectangle(new Rectangle(450, 170, Levelsprites[Idx].Width, Levelsprites[Idx].Height), Levelsprites[Idx], Color.White);

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(new Vector2(0, 90), TextTitle, Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);

            UI2DRenderer.WriteText(new Vector2(0, 55), TextRobert, Color.Gray,
                RoboCopFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
        }

        public void CreateMenuPause()
        {
            int Idx = (int)GameLogic.Pause_State;

            TextTitle = "PAUSE GAME";

            UI2DRenderer.FillRectangle(new Rectangle(450, 200, MenuMPauseSprites[Idx].Width, MenuMPauseSprites[Idx].Height), MenuMPauseSprites[Idx], Color.White);

            // Draw a 2D text string at the center of the screen
            UI2DRenderer.WriteText(Vector2.Zero, TextTitle, TxtTitleCol,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Top);
        }

        public void CreateIntro()
        {
            TextTitle = "Press ENTER to continue";
            
            //Velo
            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 170));
            //Logo
            UI2DRenderer.FillRectangle(new Rectangle(440, 250, GameLogo.Width * 2, GameLogo.Height * 2), GameLogo, Color.White);
            //Titulo
            UI2DRenderer.FillRectangle(new Rectangle(190, 30, GameTitle.Width, GameTitle.Height), GameTitle, Color.White);

            //Animation
            if (TxtTitleCol == Color.Black)
                TxtTitleCol = Color.White;
            else
                TxtTitleCol = Color.Black;

            //Title 1
            //UI2DRenderer.WriteText(new Vector2(0, 30), "Robert Swanson Presents:", Color.Gray,
            //    RoboCopFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
            //Title A
            //UI2DRenderer.WriteText(new Vector2(0, 60), "ultimate demolition derby", Color.Red,
            //    PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
            //Title B
            //UI2DRenderer.WriteText(new Vector2(0, 110), "on the moon", Color.Red,
            //    PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
            //info Text
            UI2DRenderer.WriteText(new Vector2(450, 670), TextTitle, TxtTitleCol,
                RoboCopFont, new Vector2(1.0f, 1.0f));

            UI2DRenderer.WriteText(new Vector2(1050, 680), TextIP, new Color(255, 10, 10, 255),
                CalibriFont, new Vector2(0.8f, 0.8f));
        }

        public void CreateCredits()
        {
            int Idx = (int)GameLogic.CreditScreen;
            
            //Velo
            UI2DRenderer.FillRectangle(new Rectangle(0, 0, 1280, 720), null, new Color(0, 0, 0, 190));
            UI2DRenderer.FillRectangle(new Rectangle(130, 250, Luna.Width, Luna.Height), Luna, Color.White);

            UI2DRenderer.FillRectangle(new Rectangle(74, 200, Credits[Idx].Width, Credits[Idx].Height), Credits[Idx], Color.White);

            //Title 1
            UI2DRenderer.WriteText(new Vector2(0, 30), "Credits:", Color.Gray,
                RoboCopFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
            //Title A
            UI2DRenderer.WriteText(new Vector2(0, 60), "ultimate demolition derby", Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
            //Title B
            UI2DRenderer.WriteText(new Vector2(0, 110), "on the moon", Color.Red,
                PuntosFont, GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.None);
        }

        public void CreateBoxStatus(Vector2 BoxPos, Game_Player Player)
        {
            if (Player.PlyHealth < 0)
                Player.PlyHealth = 0;
            
            int PosX = (int)BoxPos.X;
            int PosXP = (int)BoxPos.X + BoxStatusTx[0].Width;
            int PoxY = (int)BoxPos.Y + 25;
            int Healt = Player.PlyHealth / 5;
            int Popul = Player.Popularity / 5;
            TextTitle = Player.PlyName;
            Color ColorHBar = new Color(new Vector4(255, 0, 0, 10));
            Color ColorHPolBar = new Color(new Vector4(255, 255, 255, 10));

            UI2DRenderer.FillRectangle(new Rectangle(PosX, PoxY, BoxStatusTx[0].Width, BoxStatusTx[0].Height), BoxStatusTx[0], ColorHBar);
            UI2DRenderer.FillRectangle(new Rectangle(PosX, PoxY + 20, BoxStatusTx[0].Width, BoxStatusTx[0].Height), BoxStatusTx[0], ColorHPolBar);
            
            PosX += BoxStatusTx[0].Width;
            UI2DRenderer.FillRectangle(new Rectangle(PosX, PoxY, BoxStatusTx[1].Width * Healt, BoxStatusTx[1].Height), BoxStatusTx[1], ColorHBar);
            //UI2DRenderer.FillRectangle(new Rectangle(PosX, PoxY + 20, BoxStatusTx[1].Width * Popul, BoxStatusTx[1].Height), BoxStatusTx[1], ColorHPolBar);

            PosX += BoxStatusTx[1].Width * Healt;
            PosXP += BoxStatusTx[1].Width * Popul;
            UI2DRenderer.FillRectangle(new Rectangle(PosX, PoxY, BoxStatusTx[2].Width, BoxStatusTx[2].Height), BoxStatusTx[2], ColorHBar);
            //UI2DRenderer.FillRectangle(new Rectangle(PosXP, PoxY + 20, BoxStatusTx[2].Width, BoxStatusTx[2].Height), BoxStatusTx[2], ColorHPolBar);

            UI2DRenderer.WriteText(BoxPos, TextTitle, Color.DarkGray, CalibriFont, new Vector2(0.5f, 0.5f));
        }

        #endregion

        #region DebugData

        public void PutCordenates(string Legend, Vector3 Cord, Vector2 Pos)
        {
            UI2DRenderer.WriteText(Pos,
                Legend +
                " X: " + Cord.X.ToString() +
                " Y: " + Cord.Y.ToString() +
                " Z: " + Cord.Z.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
        }

        public void PutGameData(int PosX, int PosY)
        {

            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 0), "Actual Level:" + GameLogic.ActualLevel.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 20), "Game State:" + GameLogic.CurrentGameState.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 40), "Game Mode:" + GameLogic.GameMode.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 60), "Multi Mode:" + GameLogic.MultiplayerMode.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 80), "Num Playes:" + GameLogic.NumberPlayers.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 100), "Pause:" + GameLogic.Pause_State.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            UI2DRenderer.WriteText(new Vector2(PosX, PosY + 120), "Std Screen:" + GameLogic.StateScreen.ToString(), Color.LightCyan, DefaultFont, new Vector2(0.5f, 0.5f));
            
        }

        #endregion
    }
}
