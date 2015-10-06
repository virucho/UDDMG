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

/********** Components for the Game *********/
//#define USE_SOCKET_NETWORK
#define INIT_SERVER
#define USE_CAR
#define USE_OBSTACLES

/********** Only Debug **********/
#define USR_DEBUG
//#define PUT_CORDENATES

/***** Namespace Generic *****/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/***** Namespaces to use GoblinsXNA *****/
using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Physics.Newton1;
using GoblinXNA.Shaders;

/***** Namespaces to Audio *****/
using Microsoft.Xna.Framework.Audio;
using GoblinXNA.Sounds;

/***** Namespace to Car *****/
using GoblinXNA.Device.Generic;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;
using Space_VehicleCreator;

/***** Configuration File *****/
using System.Configuration;

/***** Thread ****/
using System.Threading;

/**** New Server ****/
using AR_CarServer;

/***** Namespace to use the Network *****/
#if USE_SOCKET_NETWORK
using GoblinXNA.Helpers;
using GoblinXNA.Device;
using GoblinXNA.Network;
//My Obj to Goblins
using MyObj_Networking;
#endif

namespace CarGameAR
{
    public class Game_Main : Microsoft.Xna.Framework.Game
    {
        #region GameLogic Enums

        public enum Menu_Return
        {
            NONE = 0,
            ENTER = 1,
            BACK = 2,
            OTHER = 3
        }

        public enum Ctrl_Player
        {
            KEYBOARD = 0,
            HANDY = 1,
            XBOX = 2
        }

        #endregion

        #region Member Fields
        /**** Global Variables For the Game ****/
        GraphicsDeviceManager graphics;         //Manager Scene from XNA
        Scene scene;                            //Logical Structure of Graphics from GoblinsXna
        
        static MarkerNode groundMarkerNode;     //Principal marker for AR ""Ground"
        MarkerNode[] HinderMarkeNodes;          //Array mit the marker for each Obstacle
        MarkerNode[] WeaponMarkeNodes;          //Array mit the marker for each weapon
        GeometryNode[] ObstModels;              //Array with the Obstacles Models
        GeometryNode[] CarModels;               //Array mit the Model fot cars
        static RaceCar[] CarObjPhy;             //Obj mit the Cars in the Physic Engine

        //Nodes for each specific part of the Scene
        TransformNode parentTNodeWeapon;        //Parent for all Weapons
        TransformNode parentTNodeCars;          //Parent for all Car

        static Game_Logic GameLogic;            //Class to Control the Logic states for the Game

        My2DSpriteManager My2Dmanager;          //Class to Manager the 2D Objects
        Graphics3D Manager3D;                   //Class to Manager the 3D Graphics

        Ctrl_Player[] CtrlPlayers;              //Control type for each Player

        //Datos de configuracion y Constantes
        static int Max_Models = 8;
        static int listenPort = Convert.ToInt16(ConfigurationManager.AppSettings["ListenPort"]);
        static string ServerFile = ConfigurationManager.AppSettings["ServerRoute"];
        static int Max_palyer = Convert.ToInt16(ConfigurationManager.AppSettings["Max_Player"]);
        static int Max_Markers =  Convert.ToInt16(ConfigurationManager.AppSettings["Max_Markers"]);
        static string CalibFile = ConfigurationManager.AppSettings["CamCalib"];
        static int GameResol = Convert.ToInt16(ConfigurationManager.AppSettings["GameResol"]);
        static int SelDevice = Convert.ToInt16(ConfigurationManager.AppSettings["DeviceSel"]);
        static string CapDeviceType = ConfigurationManager.AppSettings["CaptureDeviceTp"];

        //Control for players
        static string PlayerCtrl1 = ConfigurationManager.AppSettings["CtrlPlayer1"];

        static string[] ModelsName = new string[] { "Carro1",
                                                    "auto6",
                                                    "auto2",
                                                    "Carro1",
                                                    "auto3",
                                                    "auto4",
                                                    "auto5",
                                                    "auto6"};

        //Use for the KeyBoard Logic
        static int MaxkeyCount = 20;
        int IskeyUp = MaxkeyCount;
        bool keyCount = true;
        bool Bulletflag = false;

        ARServer ServidorAr;


#if USE_SOCKET_NETWORK
        //A Network Obj which transmits Car Direction
        ControlCarNetworkObject CtrlCarNetworkObj;
#endif
        #endregion

        public Game_Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; //Directorio de contenido Multimedia

            //Define the Resolution
            switch (GameResol)
            {
                case 0:
                    //Resolution FullScreen
                    this.graphics.IsFullScreen = true;
                    break;
                case 1:
                    graphics.PreferredBackBufferWidth = 640;
                    graphics.PreferredBackBufferHeight = 480;
                    break;
                case 2:
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;
                    break;
                case 3:
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 720;
                    break;
                default:
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;
                    break;
            }
        }

        /// <summary>
        /// To initializa external Components like GoblinsXna
        /// </summary>
        protected override void Initialize()
        {
            int idx;

            //Init Console
            Console.WriteLine("CarGame AR: " + Version());
            Console.WriteLine(DateTime.Now.ToString() + " Loading game\n");

            // To initialize Xna
            base.Initialize();

            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "");

            // Initialize the scene graph
            scene = new Scene();

            /********* Init Content from Game *********/

            //Instantiate the Markers
            HinderMarkeNodes = new MarkerNode[Max_Markers];
            WeaponMarkeNodes = new MarkerNode[Max_palyer]; //One Weapon for Player

            //Instantiate Models
            Console.WriteLine(DateTime.Now.ToString() + " Loading Car´s Models\n");
            
            CarModels = new GeometryNode[Max_Models];
            ModelLoader loader = new ModelLoader();
            for (idx = 0; idx < Max_Models; idx++)
            {
                CarModels[idx] = new GeometryNode("CarModel" + idx.ToString());
                CarModels[idx].Model = (Model)loader.Load("Models", ModelsName[idx]);
                ((Model)CarModels[idx].Model).UseInternalMaterials = true;
            }

            //Instantiate the Logic Game
            GameLogic = new Game_Logic(Max_palyer);

            //Create Obj for Cars
            CarObjPhy = new RaceCar[Max_palyer];

            //Instancio los Obj de los obstaculos
            ObstModels = new GeometryNode[Max_Markers];

            //Instantiate the 2DManager
            My2Dmanager = new My2DSpriteManager(GameLogic);
            My2Dmanager.LoadContent();

            //Load SoundEffects
            GameLogic.SoundGame.SoundLoad();

            /******************************************/

            // Use the newton physics engine
            scene.PhysicsEngine = new NewtonPhysics();

            //Config Physics
            SetupPhysics();

#if USE_SOCKET_NETWORK
            //Init Network
            State.EnableNetworking = true;
            State.IsServer = true;
            ConfigServer(14242);
#endif

            State.ThreadOption = (ushort)ThreadOptions.MarkerTracking;

            // Setup and Create optical marker tracking
            SetupMarkerTracking();
            CreateMarkerTracking();

            // Enable shadow mapping
            scene.ShadowMap = new MultiLightShadowMap();

            //Instantiate the 3DManager
            Manager3D = new Graphics3D(groundMarkerNode, scene.ShadowMap);

            //Config Callbacks for Collitions
            ConfigCallBacks();

            // Set up the lights used in the scene
            CreateLights();

            // Create 3D objects
            CreateObjects();

            // Show Frames-Per-Second on the screen for debugging
            State.ShowFPS = true;
            //State.ShowTriangleCount = true;
            //State.ShowNotifications = true;
            // Make the debugging message fade out after 3000 ms (3 seconds)
            Notifier.FadeOutTime = 1000;

            Console.WriteLine(DateTime.Now.ToString() + " End Loading\n");

            //Server
            ServidorAr = new ARServer(GameLogic);
            ServidorAr.CallBackAddPlayer = AgregaPlayer;

#if INIT_SERVER
            ServidorAr.StartServer();
            //Add Ip Address
            My2Dmanager.ServerIp = ServidorAr.SetLocalIp();
#endif
            //Config Controls
            ConfigControls();
            if (CtrlPlayers[0] == Ctrl_Player.KEYBOARD)
                ServidorAr.AddPlayerOne();

#if USR_DEBUG
            //GameLogic.CurrentGameState = Game_Logic.Game_States.SEL_PLY;
            //GameLogic.StateScreen = Game_Logic.Game_SCR.GAME_LOADING;

            //GameLogic.AddPlayer("Player 2", 2);
            //GameLogic.Players[1].IsPlyOK = true;
            //GameLogic.Players[1].PlySelect = 7;
            //GameLogic.Players[1].IsDead = true;
            //GameLogic.AddPlayer("Player 3", 3);
            //GameLogic.AddPlayer("Player 4", 4);
#endif

        }

        public static string Version()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " - 27/01/2014";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();

            base.UnloadContent();
        }

        protected override void Dispose(bool disposing)
        {
            //Kill Server
            ServidorAr.CloseServer();

            //Kill Music
            GameLogic.SoundGame.MusicScene1.Stop();
            
            scene.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Get State from KeyBoard to validate
            KeyboardState keyboardState = Keyboard.GetState();
            int Variable;
            int x, y;
            Menu_Return Retorno = Menu_Return.NONE;
            
            if (IskeyUp < MaxkeyCount)
            {
                IskeyUp++;
                keyCount = false;
            }
            else
                keyCount = true;

            //if (GameLogic.StateScreen == Game_Logic.Game_SCR.GAME_LOADING)
            //    My2Dmanager.CreateLoading();

            //Validation for the action in each State for the Game
            GameLogic.LastGameState = GameLogic.CurrentGameState;
            switch (GameLogic.CurrentGameState)
            {
                case Game_Logic.Game_States.INTRO:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        if (keyCount & keyboardState.IsKeyDown(Keys.Enter))
                        {
                            GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                            GameLogic.SoundGame.SelectMenu.Play();
                            GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                            IskeyUp = 0;
                        }
                    }
                    break;
                case Game_Logic.Game_States.CREDITS:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        GameLogic.CreditCounter++;
                        if (GameLogic.CreditCounter > 400)
                        {
                            GameLogic.CreditScreen++;
                            GameLogic.CreditCounter = 0;
                            if (GameLogic.CreditScreen > 2)
                                GameLogic.CreditScreen = 0;
                        }
                        
                        if (keyCount & keyboardState.IsKeyDown(Keys.Enter))
                        {
                            GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                            GameLogic.SoundGame.SelectMenu.Play();
                            GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                            IskeyUp = 0;
                        }
                    }
                    break;
                case Game_Logic.Game_States.END_GAME:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        if (keyCount & keyboardState.IsKeyDown(Keys.Enter))
                        {
                            //Reset Player
                            GameLogic.ResetCurrentPlayers();

                            GameLogic.CurrentGameState = Game_Logic.Game_States.CREDITS;
                            GameLogic.SoundGame.SelectMenu.Play();
                            GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                            IskeyUp = 0;
                        }
                    }
                    break;
                case Game_Logic.Game_States.HISTORY:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = GameLogic.CharHistory;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 8);
                        GameLogic.CharHistory = Variable;

                        if (Retorno == Menu_Return.ENTER)
                            GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                        if (Retorno == Menu_Return.BACK)
                            GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                    }
                    break;
                case Game_Logic.Game_States.MENU_MODE:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = (int)GameLogic.GameMode;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 3);
                        GameLogic.GameMode = (Game_Logic.Game_Mode)Variable;
                        
                        if (Retorno == Menu_Return.ENTER)
                        {
                            if (GameLogic.GameMode == Game_Logic.Game_Mode.MULTI_PLY)
                                GameLogic.CurrentGameState = Game_Logic.Game_States.SEL_PLY;
                            //else if (GameLogic.GameMode == Game_Logic.Game_Mode.SIGLE_PLY)
                            //    GameLogic.CurrentGameState = Game_Logic.Game_States.SEL_PLY;
                            else if (GameLogic.GameMode == Game_Logic.Game_Mode.STORY)
                                GameLogic.CurrentGameState = Game_Logic.Game_States.HISTORY;
                            else
                                GameLogic.CurrentGameState = Game_Logic.Game_States.CREDITS;
                        }
                        if (Retorno == Menu_Return.BACK)
                            GameLogic.CurrentGameState = Game_Logic.Game_States.INTRO;
                    }
                    break;
                case Game_Logic.Game_States.LEVEL_SEL:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = GameLogic.ActualLevel;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 3);
                        GameLogic.ActualLevel = Variable;

                        if (Retorno == Menu_Return.ENTER)
                            GameLogic.CurrentGameState = Game_Logic.Game_States.BUILDING;
                        if (Retorno == Menu_Return.BACK)
                        {
                            if (GameLogic.GameMode == Game_Logic.Game_Mode.MULTI_PLY)
                                GameLogic.CurrentGameState = Game_Logic.Game_States.MODE_PLAY;
                            else
                                GameLogic.CurrentGameState = Game_Logic.Game_States.LEVEL_SEL;
                        }
                    }
                    else if (GameLogic.StateScreen == Game_Logic.Game_SCR.GAME_LOADING)
                        AssignNames();
                    break;
                case Game_Logic.Game_States.MODE_PLAY:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = (int)GameLogic.MultiplayerMode;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 2);
                        GameLogic.MultiplayerMode = (Game_Logic.Play_MultiMode)Variable;

                        if (Retorno == Menu_Return.ENTER)
                        {
                            if (GameLogic.MultiplayerMode == Game_Logic.Play_MultiMode.DEATHMATCH)
                                GameLogic.CurrentGameState = Game_Logic.Game_States.LEVEL_SEL;
                            else if (GameLogic.MultiplayerMode == Game_Logic.Play_MultiMode.TOURNAMENT)
                            {
                                GameLogic.ActualLevel = 0;
                                GameLogic.CurrentGameState = Game_Logic.Game_States.BUILDING;
                            }
                            else
                                GameLogic.CurrentGameState = Game_Logic.Game_States.LEVEL_SEL;
                        }
                        if (Retorno == Menu_Return.BACK)
                            GameLogic.CurrentGameState = Game_Logic.Game_States.SEL_PLY;
                    }
                    else if (GameLogic.StateScreen == Game_Logic.Game_SCR.GAME_LOADING)
                        CreateWeapons();
                    break;
                case Game_Logic.Game_States.SEL_PLY:
                    switch (GameLogic.StateScreen)
                    {
                        case Game_Logic.Game_SCR.GAME_LOADING:
                            My2Dmanager.LoadDateMenu();
                            break;
                        case Game_Logic.Game_SCR.RUNNING:
                            bool AllesOK = true;
                            //Text Number of Players
                            if (GameLogic.NumberPlayers < 2)
                                My2Dmanager.TextMessage = "Minimum 2 Players";
                            else
                            { 
                                for (x = 0; x < GameLogic.NumberPlayers; x++)
                                {
                                    if (GameLogic.Players[x].IsPlyOK == false)
                                    {
                                        AllesOK = false;
                                        break;
                                    }
                                }
                                if (AllesOK == true)
                                    My2Dmanager.TextMessage = "Press Enter to continue";
                                else
                                    My2Dmanager.TextMessage = "Waiting all Players";
                            }

                            ValPlayerMenuKeys(keyboardState, AllesOK);
                            break;
                    }
                    break;// Switch CurrentGameState
                case Game_Logic.Game_States.BUILDING:
                    switch (GameLogic.StateScreen)
                    {
                        case Game_Logic.Game_SCR.GAME_LOADING:
                            //Create the Scene
                            if (GameLogic.ActualLevel == 0)
                                Manager3D.CreateLevel1();
                            else if (GameLogic.ActualLevel == 1)
                                Manager3D.CreateLevel2();
                            else
                                Manager3D.CreateLevel3();

                            break;
                        case Game_Logic.Game_SCR.RUNNING:
                            //Cambio las posiciones de los Objetos segun su marcador
                            TransformCubeReal();

                            if (keyCount & keyboardState.IsKeyDown(Keys.Enter))
                            {
                                VoyaZero(); // Fijo la Pos De los Objetos
                                for (x = 0; x < GameLogic.NumberPlayers; x++)
                                {
                                    GameLogic.GameWeapons[x].AddPos(GameLogic.GameWeapons[x].WeaponModel.Physics.PhysicsWorldTransform.Translation);
                                }

                                GameLogic.CurrentGameState = Game_Logic.Game_States.PLAY;
                                IskeyUp = 0;
                            }
                            break;
                    }

                    break;
                case Game_Logic.Game_States.PLAY:
                    switch (GameLogic.StateScreen)
                    {
                        case Game_Logic.Game_SCR.GAME_LOADING:
                            // Create 3D objects
                            CreatePlayerCar();

                            //Active Sim
                            ((NewtonPhysics)scene.PhysicsEngine).PauseSimulation = false;

                            break;
                        case Game_Logic.Game_SCR.RUNNING:
#if USE_CAR
                            if (GameLogic.IsPaused == false)
                            {
                                ValidateRunKeys(keyboardState);

                                //Animation Weapons
                                for (x = 0; x < GameLogic.NumberPlayers; x++)
                                {
                                    GameLogic.GameWeapons[x].Rotation();
                                    RotateWp(x);

                                    for (y = 0; y < GameLogic.NumberPlayers; y++)
                                    {
                                        if (GameLogic.GameWeapons[x].IsInRange(CarObjPhy[y].PhysicsWorldTransform.Translation))
                                        {
                                            if (GameLogic.GameWeapons[x].Dispara())
                                            {
                                                GameLogic.SoundGame.Laser1.Play();
                                                GameLogic.SoundGame.Laser1.Volume = 0.5f;
                                            }
                                        }
                                    }
                                }

                                //Musica Fondo
                                if (GameLogic.SoundGame.MusicScene1.State != SoundState.Playing)
                                {
                                    GameLogic.SoundGame.MusicScene1.Play();
                                    GameLogic.SoundGame.MusicScene1.Volume = 0.5f;
                                }
                            }
                            else
                            {
                                Variable = (int)GameLogic.Pause_State;
                                Retorno = ValMenuKeys(keyboardState, ref Variable, 2);
                                GameLogic.Pause_State = (Game_Logic.Pausa_Status)Variable;

                                if (keyCount & Retorno == Menu_Return.ENTER)
                                {
                                    GameLogic.IsPaused = false;
                                    IskeyUp = 0;
                                }
                            }
#endif
                            break;
                    }
                    break;
                case Game_Logic.Game_States.POINTS_MENU:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = (int)GameLogic.EndActionMenu;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 3);
                        GameLogic.EndActionMenu = (Game_Logic.EndAction)Variable;

                        if (Retorno == Menu_Return.ENTER)
                        {
                            if (GameLogic.EndActionMenu == Game_Logic.EndAction.MAIN_MENU)
                                GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                            else if (GameLogic.EndActionMenu == Game_Logic.EndAction.NEW_GAME)
                            {
                                Console.WriteLine(DateTime.Now.ToString() + " -- NEW_GAME --\n");
                                //Reset Scene
                                DestroyCars();
                                GameLogic.ResetCurrentPlayers();
                                Manager3D.DestroyLevel();

                                GameLogic.CurrentGameState = Game_Logic.Game_States.MODE_PLAY;
                            }
                            else //Rematch
                            {
                                GameLogic.ReviveCurrentPlayers();
                                scene.PhysicsEngine.RestartsSimulation();
                                
                                GameLogic.CurrentGameState = Game_Logic.Game_States.BUILDING;
                            }
                        }
                    }
                    break;
                case Game_Logic.Game_States.POINTS_MENU_T:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = (int)GameLogic.EndActionTour;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 2);
                        GameLogic.EndActionTour = (Game_Logic.EndTour)Variable;

                        if (Retorno == Menu_Return.ENTER)
                        {
                            if (GameLogic.ActualLevel != 2)
                            {
                                if (GameLogic.EndActionTour == Game_Logic.EndTour.EXIT_GAME)
                                {
                                    Console.WriteLine(DateTime.Now.ToString() + " -- NEW_GAME --\n");
                                    //Reset Scene
                                    DestroyCars();
                                    GameLogic.ResetCurrentPlayers();
                                    Manager3D.DestroyLevel();

                                    GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                                }
                                else //Next Level
                                {
                                    Console.WriteLine(DateTime.Now.ToString() + " -- NEX_LEVEL --\n");

                                    Manager3D.DestroyLevel();
                                    GameLogic.ReviveCurrentPlayers();
                                    scene.PhysicsEngine.RestartsSimulation();
                                    //Up Level
                                    GameLogic.ActualLevel++;

                                    GameLogic.CurrentGameState = Game_Logic.Game_States.BUILDING;
                                }
                            }
                            else
                            {
                                Console.WriteLine(DateTime.Now.ToString() + " -- END_GAME --\n");
                                //Reset Scene
                                DestroyCars();
                                Manager3D.DestroyLevel();

                                GameLogic.CurrentGameState = Game_Logic.Game_States.END_GAME;
                            }
                        }
                    }
                    break;
                case Game_Logic.Game_States.PAUSE:
                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        Variable = (int)GameLogic.Pause_State;
                        Retorno = ValMenuKeys(keyboardState, ref Variable, 2);
                        GameLogic.Pause_State = (Game_Logic.Pausa_Status)Variable;
                        if (Retorno == Menu_Return.ENTER)
                        {
                            GameLogic.CurrentGameState = Game_Logic.Game_States.PLAY;
                        }
                    }
                    break;
            }

            //Auto change for the current Screens
            if (GameLogic.LastGameState != GameLogic.CurrentGameState)
                GameLogic.StateScreen = Game_Logic.Game_SCR.GAME_LOADING; //Always when the Game State change, the Screen go to Loading
            else
            {
                if (GameLogic.StateScreen == Game_Logic.Game_SCR.GAME_LOADING)
                    GameLogic.StateScreen = Game_Logic.Game_SCR.RENDERING;
                else if (GameLogic.StateScreen == Game_Logic.Game_SCR.RENDERING)
                    GameLogic.StateScreen = Game_Logic.Game_SCR.RUNNING;
            }

            //Validaciones para Estado Play
            if (GameLogic.CurrentGameState == Game_Logic.Game_States.PLAY)
            {
                //Valido el tiempo de vida de las Balas
                int ActualTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
                for (y = 0; y < GameLogic.NumberPlayers; y++)
                {
                    for (x = 0; x < GameLogic.Players[y].PlayerBullets.Count; x++)
                    {
                        if (GameLogic.Players[y].PlayerBullets[x].IsVisible == true) //Si la Bala esta en el escenario
                        {
                            if (GameLogic.Players[y].PlayerBullets[x].LiveTime < ActualTime) // Si termino el tiempo de vida de la Bala
                            {
                                GameLogic.Players[y].PlayerBullets[x].RemoveBullet();
                            }
                        }
                    }
                }
                //Valido Balas de Weapons
                for (y = 0; y < GameLogic.NumberPlayers; y++)
                {
                    for (x = 0; x < GameLogic.GameWeapons[y].WeaponBullet.Count; x++)
                    {
                        if (GameLogic.GameWeapons[y].WeaponBullet[x].IsVisible == true) //Si la Bala esta en el escenario
                        {
                            if (GameLogic.GameWeapons[y].WeaponBullet[x].LiveTime < ActualTime) // Si termino el tiempo de vida de la Bala
                            {
                                GameLogic.GameWeapons[y].WeaponBullet[x].RemoveBullet();
                            }
                        }
                    }
                }

                //Valido la Vida de los personajes
                for (x = 0; x < GameLogic.NumberPlayers; x++)
                {
                    if (GameLogic.Players[x].PlyHealth <= 0 && GameLogic.Players[x].IsDead == false)
                    {
                        GameLogic.SoundGame.Muerte1.Play();
                        GameLogic.Players[x].IsDead = true;
                        GameLogic.Players[x].NumDeads++;

                        //CarObjPhy[x].Collidable = false;
                        //CarObjPhy[x].Interactable = false;

                        Matrix mat2 = Matrix.CreateTranslation(0.0f, 0.0f, -10.0f);

                        // Modify the transformation in the physics engine
                        //((NewtonPhysics)scene.PhysicsEngine).SetTransform(CarModels[x].Physics, mat2);

                        //Manager3D.AddLapida(CarObjPhy[x].PhysicsWorldTransform.Translation, GameLogic.LevelHeight, 5.5f);
                    }
                }

                //Valido si ya ganamos
                int NumMuertos = 0;
                for (x = 0; x < GameLogic.NumberPlayers; x++)
                {
                    if (GameLogic.Players[x].IsDead == true)
                        NumMuertos++;
                }
                if (NumMuertos == (GameLogic.NumberPlayers - 1))
                {
                    //Stop Music
                    GameLogic.SoundGame.MusicScene1.Stop();
                    //Stop Physic Sim
                    ((NewtonPhysics)scene.PhysicsEngine).PauseSimulation = true;

                    //Validation for Game Mode
                    if (GameLogic.MultiplayerMode == Game_Logic.Play_MultiMode.DEATHMATCH)
                    {
                        GameLogic.StateScreen = Game_Logic.Game_SCR.RUNNING;
                        GameLogic.CurrentGameState = Game_Logic.Game_States.POINTS_MENU;
                    }
                    else if (GameLogic.MultiplayerMode == Game_Logic.Play_MultiMode.TOURNAMENT)
                    {
                        GameLogic.StateScreen = Game_Logic.Game_SCR.RUNNING;
                        GameLogic.CurrentGameState = Game_Logic.Game_States.POINTS_MENU_T;
                    }
                }
            }
            
            scene.Update(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly, this.IsActive);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Renders 2D Objects
            My2Dmanager.Draw(gameTime);

#if PUT_CORDENATES
            My2Dmanager.PutCordenates("Marke ", HinderMarkeNodes[0].WorldTransformation.Translation, new Vector2(5,30));
            My2Dmanager.PutCordenates("Ground", groundMarkerNode.WorldTransformation.Translation, new Vector2(5, 48));
            if (ObstModels[0] != null)
            {
                My2Dmanager.PutCordenates("ObstW", ObstModels[0].WorldTransformation.Translation, new Vector2(5, 66));
                My2Dmanager.PutCordenates("ObstWW", ObstModels[0].Physics.PhysicsWorldTransform.Translation, new Vector2(5, 84));
            }
            if (CarModels[0] != null)
            {
                My2Dmanager.PutCordenates("CarW", CarModels[0].WorldTransformation.Translation, new Vector2(5, 102));
            }
#endif

            scene.Draw(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
        }

        #region Control valitations

        private void ValPlayerMenuKeys(KeyboardState keyboardState, bool AllesOK)
        {
            int x = 1;

            if (CtrlPlayers[0] == Ctrl_Player.KEYBOARD)
            {
                if (keyCount)
                {
                    if (keyboardState.IsKeyDown(Keys.Enter))
                    {
                        if (AllesOK == true)
                        {
                            if (GameLogic.GameMode == Game_Logic.Game_Mode.MULTI_PLY)
                            {
                                if (GameLogic.NumberPlayers > 1)
                                {
                                    GameLogic.CurrentGameState = Game_Logic.Game_States.MODE_PLAY;

                                    GameLogic.SoundGame.SelectMenu.Play();
                                    GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                                }
                            }
                            else
                            {
                                GameLogic.CurrentGameState = Game_Logic.Game_States.LEVEL_SEL;

                                GameLogic.SoundGame.SelectMenu.Play();
                                GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                            }
                        }
                        else
                        {
                            GameLogic.Players[0].IsPlyOK = true;

                            GameLogic.SoundGame.SelectMenu.Play();
                            GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                        }

                        IskeyUp = 0;
                    }
                    if (keyboardState.IsKeyDown(Keys.Escape))
                    {
                        GameLogic.Players[0].IsPlyOK = false;
                        IskeyUp = 0;
                    }
                    if (keyboardState.IsKeyDown(Keys.Back))
                    {
                        GameLogic.CurrentGameState = Game_Logic.Game_States.MENU_MODE;
                        IskeyUp = 0;
                    }

                    if (GameLogic.Players[0].IsPlyOK == false)
                    {
                        if (keyboardState.IsKeyDown(Keys.Right))
                        {
                            GameLogic.Players[0].PlySelect++;
                            if (GameLogic.Players[0].PlySelect > 8)
                                GameLogic.Players[0].PlySelect = 8;
                            IskeyUp = 0;
                            GameLogic.SoundGame.MenuSound.Play();
                            GameLogic.SoundGame.MenuSound.Volume = 0.2f;
                        }
                        if (keyboardState.IsKeyDown(Keys.Left))
                        {
                            GameLogic.Players[0].PlySelect--;
                            if (GameLogic.Players[0].PlySelect < 1)
                                GameLogic.Players[0].PlySelect = 1;
                            IskeyUp = 0;
                            GameLogic.SoundGame.MenuSound.Play();
                            GameLogic.SoundGame.MenuSound.Volume = 0.2f;
                        }
                        if (keyboardState.IsKeyDown(Keys.Up))
                        {
                            if (GameLogic.Players[0].PlySelect > 4)
                                GameLogic.Players[0].PlySelect -= 4;
                            IskeyUp = 0;
                            GameLogic.SoundGame.MenuSound.Play();
                            GameLogic.SoundGame.MenuSound.Volume = 0.2f;
                        }
                        if (keyboardState.IsKeyDown(Keys.Down))
                        {
                            if (GameLogic.Players[0].PlySelect < 5)
                                GameLogic.Players[0].PlySelect += 4;
                            IskeyUp = 0;
                            GameLogic.SoundGame.MenuSound.Play();
                            GameLogic.SoundGame.MenuSound.Volume = 0.2f;
                        }
                    }
                }
            }
            else
                x = 0;

            for (; x < GameLogic.NumberPlayers; x++)
            {
                ValidatemenuAcel(ServidorAr.ControlPly[x].AcelValue, x, ServidorAr.ControlPly[x].Buttons);
            }
        }

        private Menu_Return ValMenuKeys(KeyboardState keyboardState, ref int VarUpdate, int NumOptions)
        {
            if (keyCount)
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    GameLogic.StateScreen = Game_Logic.Game_SCR.GAME_LOADING;
                    IskeyUp = 0;
                    GameLogic.SoundGame.SelectMenu.Play();
                    GameLogic.SoundGame.SelectMenu.Volume = 0.2f;
                    return Menu_Return.ENTER;
                }
                if (keyboardState.IsKeyDown(Keys.Back))
                {
                    GameLogic.StateScreen = Game_Logic.Game_SCR.GAME_LOADING;
                    IskeyUp = 0;
                    return Menu_Return.BACK;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    VarUpdate--;
                    if (VarUpdate < 0)
                        VarUpdate = NumOptions - 1;
                    IskeyUp = 0;
                    GameLogic.SoundGame.MenuSound.Play();
                    GameLogic.SoundGame.MenuSound.Volume = 0.2f;
                    return Menu_Return.OTHER;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    VarUpdate++;
                    if (VarUpdate > (NumOptions - 1))
                        VarUpdate = 0;
                    IskeyUp = 0;
                    GameLogic.SoundGame.MenuSound.Play();
                    GameLogic.SoundGame.MenuSound.Volume = 0.2f;
                    return Menu_Return.OTHER;
                }
                return Menu_Return.NONE;
            }
            return Menu_Return.NONE;
        }

        /****************** Acel *******************/

        private void ValidatemenuAcel(int[] DirCar, int IdPlayer, bool[]CarButtons)
        {

            if (keyCount)
            {
                if (CarButtons[1])
                {
                    GameLogic.Players[IdPlayer].IsPlyOK = true;

                    GameLogic.SoundGame.SelectMenu.Play();
                    GameLogic.SoundGame.SelectMenu.Volume = 0.2f;

                    IskeyUp = 0;
                }
                if (CarButtons[2])
                {
                    GameLogic.Players[IdPlayer].IsPlyOK = false;
                    IskeyUp = 0;
                }


                if (!GameLogic.Players[IdPlayer].IsPlyOK)
                {
                    if (DirCar[1] > 30)
                    {
                        GameLogic.Players[IdPlayer].PlySelect++;
                        if (GameLogic.Players[IdPlayer].PlySelect > 8)
                            GameLogic.Players[IdPlayer].PlySelect = 8;
                        IskeyUp = 0;
                    }
                    if (DirCar[1] < -30)
                    {
                        GameLogic.Players[IdPlayer].PlySelect--;
                        if (GameLogic.Players[IdPlayer].PlySelect < 1)
                            GameLogic.Players[IdPlayer].PlySelect = 1;
                        IskeyUp = 0;
                    }
                    if (DirCar[0] < -30)
                    {
                        if (GameLogic.Players[IdPlayer].PlySelect > 4)
                            GameLogic.Players[IdPlayer].PlySelect -= 4;
                        IskeyUp = 0;
                    }
                    if (DirCar[0] > 30)
                    {
                        if (GameLogic.Players[IdPlayer].PlySelect < 5)
                            GameLogic.Players[IdPlayer].PlySelect += 4;
                        IskeyUp = 0;
                    }
                }
            }
        }

#if USE_CAR
        bool aceleraFlag = false;
        private void ValidateRunKeys(KeyboardState keyboardState)
        {
            int x = 1;

            if (CtrlPlayers[0] == Ctrl_Player.KEYBOARD)
            {
                // RIGHT & LEFT
                if (keyboardState.IsKeyDown(Keys.Right))
                    CarObjPhy[0].SetSteering(-1);
                else if (keyboardState.IsKeyDown(Keys.Left))
                    CarObjPhy[0].SetSteering(1);
                else
                    CarObjPhy[0].SetSteering(0);

                // UP & DOWN
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    CarObjPhy[0].SetTireTorque(16);

                    if (GameLogic.SoundGame.Acelera.State != SoundState.Playing && aceleraFlag == false)
                    {
                        GameLogic.SoundGame.Acelera.Play();
                        GameLogic.SoundGame.Acelera.Volume = 0.5f;
                        aceleraFlag = true;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                    CarObjPhy[0].SetTireTorque(-10);
                else
                {
                    CarObjPhy[0].SetTireTorque(0);
                    aceleraFlag = false;
                }

                // BRAKE
                if (keyboardState.IsKeyDown(Keys.Space))
                    CarObjPhy[0].ApplyHandBrakes(1);
                else
                    CarObjPhy[0].ApplyHandBrakes(0);

                //RESPAWN
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    //Quito sangre por caida
                    GameLogic.Players[0].PlyHealth -= 2;
                    CarObjPhy[0].Respawn(((NewtonPhysics)scene.PhysicsEngine).GetBody(CarObjPhy[0]));
                }

                //PAUSA
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    //GameLogic.CurrentGameState = Game_Logic.Game_States.PAUSE;
                    //GameLogic.IsPaused = true;
                    IskeyUp = 0;
                    return;
                }

                // SHOOT
                if (keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    Bulletflag = true;
                    return;
                }

                if (Bulletflag)
                {
                    if (keyboardState.IsKeyUp(Keys.LeftControl))
                    {
                        ShootCar(0);
                        Bulletflag = false;
                    }
                }
            }
            else
                x = 0;

            //Validation Control With Acel
            for (; x < GameLogic.NumberPlayers; x++)
            {
                if (GameLogic.Players[x].IsDead == false)
                {
                    if (keyCount)
                    {
                        if (ServidorAr.ControlPly[x].Buttons[0]) //Fire
                            ShootCar(x);
                        IskeyUp = 0;
                    }

                    if (ServidorAr.ControlPly[x].Buttons[1]) //Gas
                    {
                        CarObjPhy[x].SetTireTorque(16);
                        //Sound
                        if (GameLogic.SoundGame.Acelera.State != SoundState.Playing)
                        {
                            GameLogic.SoundGame.Acelera.Play();
                            GameLogic.SoundGame.Acelera.Volume = 0.5f;
                        }
                    }
                    else
                        CarObjPhy[x].SetTireTorque(0);

                    if (ServidorAr.ControlPly[x].Buttons[2]) //Brakes
                    {
                        if (ServidorAr.ControlPly[x].AcelValue[0] < -15)
                            CarObjPhy[x].SetTireTorque(-16);
                        else
                            CarObjPhy[x].ApplyHandBrakes(1);
                    }
                    else
                        CarObjPhy[x].ApplyHandBrakes(0);

                    // Control the car steering with right and left
                    if ((ServidorAr.ControlPly[x].AcelValue[1] > 20) && (ServidorAr.ControlPly[x].AcelValue[1] < 35))
                        CarObjPhy[x].SetSteering(-0.2f);
                    if ((ServidorAr.ControlPly[x].AcelValue[1] > 35) && (ServidorAr.ControlPly[x].AcelValue[1] < 55))
                        CarObjPhy[x].SetSteering(-0.3f);
                    if (ServidorAr.ControlPly[x].AcelValue[1] > 55)
                        CarObjPhy[x].SetSteering(-0.6f);

                    if ((ServidorAr.ControlPly[x].AcelValue[1] < -20) && (ServidorAr.ControlPly[x].AcelValue[1] > -35))
                        CarObjPhy[x].SetSteering(0.2f);
                    if ((ServidorAr.ControlPly[x].AcelValue[1] < -35) && (ServidorAr.ControlPly[x].AcelValue[1] > -55))
                        CarObjPhy[x].SetSteering(0.3f);
                    if (ServidorAr.ControlPly[x].AcelValue[1] < -55)
                        CarObjPhy[x].SetSteering(0.6f);

                    if ((ServidorAr.ControlPly[x].AcelValue[1] < 20) && (ServidorAr.ControlPly[x].AcelValue[1] > -20))
                        CarObjPhy[x].SetSteering(0);
                }//End if
            }//En for for players
        }
#endif

        public void ShootCar(int IdCar)
        {
            Vector3 PosCar, PosBall;
            PosCar = CarObjPhy[IdCar].PhysicsWorldTransform.Translation;
            PosBall = new Vector3(PosCar.X, PosCar.Y, PosCar.Z + 4);
            //Call Function for Bullets
            GameLogic.SoundGame.Laser1.Play();
            GameLogic.SoundGame.Laser1.Volume = 0.5f;

            if (GameLogic.Players[IdCar].PlayerBullets.Count < GameLogic.MAX_NUM_BULLET)
            {
                Bullet Bala = new Bullet(PosBall, Manager3D.BulletModel, Manager3D.BulletrMat, CarObjPhy[IdCar].PhysicsWorldTransform.Left, groundMarkerNode);
                GameLogic.Players[IdCar].PlayerBullets.Add(Bala);
            }
            else
            {
                int Index = Bullet.SearchBall(GameLogic.Players[IdCar].PlayerBullets);
                if (Index > -1)
                {
                    //Adicciono Bala
                    GameLogic.Players[IdCar].PlayerBullets[Index].ReAddBullet(PosBall, CarObjPhy[IdCar].PhysicsWorldTransform.Left);
                }
            }
        }

        #endregion

        #region Config

        private void ConfigControls()
        {
            CtrlPlayers = new Ctrl_Player[4];

            //Only Player 1 can use the Keyboard
            if (PlayerCtrl1 == "Tastatur")
                CtrlPlayers[0] = Ctrl_Player.KEYBOARD;
            else
                CtrlPlayers[0] = Ctrl_Player.HANDY;

            //Othe Players
            CtrlPlayers[1] = Ctrl_Player.HANDY;
            CtrlPlayers[2] = Ctrl_Player.HANDY;
            CtrlPlayers[3] = Ctrl_Player.HANDY;
        }

        private void SetupPhysics()
        {
            Console.WriteLine(DateTime.Now.ToString() + " Setup Physic\n");
            
            scene.PhysicsEngine = new NewtonPhysics(); //Use NewtonDynamics
            // Make the physics simulation space larger to 500x500 centered at the origin
            ((NewtonPhysics)scene.PhysicsEngine).WorldSize = new BoundingBox(Vector3.One * -250,
                Vector3.One * 250);
            // Increase the gravity
            scene.PhysicsEngine.Gravity = 60.0f;

            ((NewtonPhysics)scene.PhysicsEngine).MaxSimulationSubSteps = 5;
        }

        private void ConfigCallBacks()
        {
            Console.WriteLine(DateTime.Now.ToString() + " Setup Collisions CallBacks\n");
            
            // Create physics material to detect collision
            NewtonMaterial BullGroMat = new NewtonMaterial();
            // Bullet to Ground interaction
            BullGroMat.MaterialName1 = "Bullet";
            BullGroMat.MaterialName2 = "Ground";
            // Callback Function
            BullGroMat.ContactProcessCallback = GameLogic.CallbackBulletGround;

            // Add the physics Material interaction
            ((NewtonPhysics)scene.PhysicsEngine).AddPhysicsMaterial(BullGroMat);

            // Create physics material to detect collision
            NewtonMaterial BullObstMat = new NewtonMaterial();
            // Bullet to Ground interaction
            BullObstMat.MaterialName1 = "Bullet";
            BullObstMat.MaterialName2 = "Obstacle";
            // Callback Function
            BullObstMat.ContactProcessCallback = GameLogic.CallbackBulletObst;

            // Add the physics Material interaction
            ((NewtonPhysics)scene.PhysicsEngine).AddPhysicsMaterial(BullObstMat);

            //////////////////////////////////////

            // Create physics material to detect collision
            NewtonMaterial BullCarMat1 = new NewtonMaterial();
            BullCarMat1.MaterialName1 = "Bullet";
            BullCarMat1.MaterialName2 = "Car0";
            BullCarMat1.ContactProcessCallback = GameLogic.CallbackBulletHitCar0;
            //BullCarMat1.ContactBeginCallback = puercavida;
            //BullCarMat1.ContactEndCallback = GameLogic.oooocall;
            ((NewtonPhysics)scene.PhysicsEngine).AddPhysicsMaterial(BullCarMat1);

            NewtonMaterial BullCarMat2 = new NewtonMaterial();
            BullCarMat2.MaterialName1 = "Bullet";
            BullCarMat2.MaterialName2 = "Car1";
            BullCarMat2.ContactProcessCallback = GameLogic.CallbackBulletHitCar1;
            ((NewtonPhysics)scene.PhysicsEngine).AddPhysicsMaterial(BullCarMat2);

            NewtonMaterial BullCarMat3 = new NewtonMaterial();
            BullCarMat3.MaterialName1 = "Bullet";
            BullCarMat3.MaterialName2 = "Car2";
            BullCarMat3.ContactProcessCallback = GameLogic.CallbackBulletHitCar2;
            ((NewtonPhysics)scene.PhysicsEngine).AddPhysicsMaterial(BullCarMat3);

            NewtonMaterial BullCarMat4 = new NewtonMaterial();
            BullCarMat4.MaterialName1 = "Bullet";
            BullCarMat4.MaterialName2 = "Car3";
            BullCarMat4.ContactProcessCallback = GameLogic.CallbackBulletHitCar3;
            ((NewtonPhysics)scene.PhysicsEngine).AddPhysicsMaterial(BullCarMat4);
        }

        private void SetupMarkerTracking()
        {
            IVideoCapture captureDevice = null;

            Console.WriteLine(DateTime.Now.ToString() + " Setup Video & AR\n");

            // Create our video capture device, depend from Type, "look App.config"
            if (CapDeviceType == "OpenCV")
                captureDevice = new OpenCVCapture();
            else
                captureDevice = new DirectShowCapture();

            //captureDevice.VideoDeviceID = 0;
            captureDevice.InitVideoCapture(SelDevice, FrameRate._60Hz, Resolution._640x480,
                ImageFormat.R8G8B8_24, false);

            // Add this video capture device to the scene
            scene.AddVideoCaptureDevice(captureDevice);

            ALVARMarkerTracker tracker = new ALVARMarkerTracker();
            tracker.MaxMarkerError = 0.02f;
            tracker.InitTracker(captureDevice.Width, captureDevice.Height, CalibFile, 32.4f);

            // Set the marker tracker to use for our scene
            scene.MarkerTracker = tracker;

            // Display the camera image in the background
            scene.ShowCameraImage = true;

            //Config Gravity from Camera Cord
            scene.PhysicsEngine.GravityDirection = -Vector3.UnitZ; 
        }

        private void CreateMarkerTracking()
        {
            Console.WriteLine(DateTime.Now.ToString() + " Setup Markers\n");
            
            // Create a marker node to track a ground marker array.
            groundMarkerNode = new MarkerNode(scene.MarkerTracker, "ALVARGroundArray.xml");
            //Add marker to Scene
            scene.RootNode.AddChild(groundMarkerNode);

            /***** For Weapons *****/
            //Add Big Parent for All Weapons
            parentTNodeWeapon = new TransformNode();
            parentTNodeWeapon.Name = "Weapons";
            groundMarkerNode.AddChild(parentTNodeWeapon);
            /***** For Cars *****/
            parentTNodeCars = new TransformNode();
            parentTNodeCars.Name = "TCars";
            groundMarkerNode.AddChild(parentTNodeCars);

            /************** Array of markers **************/
            int HinIdx, MarkIdx = 29;

            //Add Markers with the Code Index between 29 - 32 for the Weapons (one for player)
            for (HinIdx = 0; HinIdx < Max_palyer; HinIdx++)
            {
                WeaponMarkeNodes[HinIdx] = new MarkerNode(scene.MarkerTracker, MarkIdx);
                scene.RootNode.AddChild(WeaponMarkeNodes[HinIdx]);
                MarkIdx++;
            }

            //Add Markers with the Code Index between 33 - 38 for the Obstacles
            for (HinIdx = 0; HinIdx < Max_Markers; HinIdx++)
            {
                HinderMarkeNodes[HinIdx] = new MarkerNode(scene.MarkerTracker, MarkIdx);
                scene.RootNode.AddChild(HinderMarkeNodes[HinIdx]);
                MarkIdx++;
            }
        }

        #endregion

        #region Scene Creation

        private void CreateLights()
        {
            Console.WriteLine(DateTime.Now.ToString() + " Setup Lights\n");
            
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.LightSource = lightSource;

            // Set this light node to cast shadows (by just setting this to true will not cast any shadows,
            // scene.ShadowMap needs to be set to a valid IShadowMap and Model.Shader needs to be set to
            // a proper IShadowShader implementation
            lightNode.CastShadows = true;

            // You should also set the light projection when casting shadow from this light
            lightNode.LightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                1, 1f, 500);

            scene.RootNode.AddChild(lightNode);
        }

        private void CreateObjects()
        {
            Console.WriteLine(DateTime.Now.ToString() + " CreateObjects\n");

#if USE_OBSTACLES
            //Asoc Marker to Obstacles

            if(ObstModels[0] == null)
                ObstModels[0] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(0), true, 8.5f);
            if (ObstModels[1] == null)
                ObstModels[1] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(3), true, 10.5f);
            if (ObstModels[2] == null)
                ObstModels[2] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(2), true, 10.5f);
            if (ObstModels[3] == null)
                ObstModels[3] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(4), true, 8.5f);
            if (ObstModels[4] == null)
                ObstModels[4] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(1), true, 8.5f);
            if (ObstModels[5] == null)
                ObstModels[5] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(5), true, 8.5f);

            if (ObstModels[6] == null)
                ObstModels[6] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(6), true, 8.5f);

            if (ObstModels[7] == null)
                ObstModels[7] = Graphics3D.AddModel(groundMarkerNode, "Obstacles", Manager3D.GetName(7), true, 8.5f);
            
#else
            //Graphics3D.CreateBall(groundMarkerNode);

            Manager3D.CreateCube(HinderMarkeNodes[0], new Vector4(0.5f, 0, 0, 1));

            Manager3D.CreateCube(HinderMarkeNodes[1], new Vector4(0.5f, 0.5f, 0, 1));

            Manager3D.CreateCube(HinderMarkeNodes[2], new Vector4(0.5f, 1.0f, 0, 1));

            Manager3D.CreateCube(HinderMarkeNodes[3], new Vector4(0.8f, 0.8f, 0, 1));

            Manager3D.CreateCube(HinderMarkeNodes[4], new Vector4(0.3f, 0, 0, 1));

            Manager3D.CreateCube(HinderMarkeNodes[5], new Vector4(0.3f, 0.3f, 0, 1));
#endif
        }

        private void CreatePlayerCar()
        {
            Console.WriteLine(DateTime.Now.ToString() + " CreatePlayerCar\n");

#if USE_CAR
            /****************** car ******************/
            int Idx;

            if (Manager3D.IsLoadCars == false)
            {
                for (Idx = 0; Idx < GameLogic.NumberPlayers; Idx++)
                {
                    // Add a race car for each Current Player
                    CarObjPhy[Idx] = VehicleCreator.AddRaceCar(scene, parentTNodeCars,
                                                            GameLogic.Players[Idx].InitialPos,
                                                            CarModels[GameLogic.Players[Idx].PlySelect - 1],
                                                            Idx);
                }
                Manager3D.IsLoadCars = true;
            }
#endif
        }

        private void CreateWeapons()
        {
            Console.WriteLine(DateTime.Now.ToString() + " CreateWeapons\n"); 
            
            int x;

            //Crete the Model un Logic for Weapons
            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                if (GameLogic.GameWeapons[x] == null)
                {
                    GameLogic.GameWeapons[x] = new Game_Weapon(Manager3D.BulletModel, Manager3D.BulletrMat, groundMarkerNode);
                    GameLogic.GameWeapons[x].WeaponModel = Graphics3D.AddModel(null, "Weapons", Manager3D.WeaponNames + (x + 1).ToString(), true, 5.5f);
                    //Add Papa
                    GameLogic.GameWeapons[x].PapaWeapon = (TransformNode)GameLogic.GameWeapons[x].WeaponModel.Parent;
                }

                if (x == 1)
                {
                    //Type Shoot
                    GameLogic.GameWeapons[x].WaitToShoot = 2;
                    GameLogic.GameWeapons[x].Cartridge = 4;
                    //Type Rotation
                    GameLogic.GameWeapons[x].EndAngle = 180;
                    GameLogic.GameWeapons[x].DeltaTheta = 2;
                    GameLogic.GameWeapons[x].OffsetTheta = Math.PI;
                    GameLogic.GameWeapons[x].RotationMode = Game_Weapon.RotMode.BACK;
                }

                if (x == 2)
                {
                    //Type Shoot
                    GameLogic.GameWeapons[x].WaitToShoot = 5;
                    GameLogic.GameWeapons[x].Cartridge = 2;
                    //Type Rotation
                    //GameLogic.GameWeapons[x].EndAngle = 180;
                    GameLogic.GameWeapons[x].DeltaTheta = 3;
                    GameLogic.GameWeapons[x].GirDirection = Game_Weapon.RotDirection.LINKS;
                    GameLogic.GameWeapons[x].RotationMode = Game_Weapon.RotMode.CONTINUE;
                }

                if (x == 3)
                {
                    //Type Shoot
                    GameLogic.GameWeapons[x].WaitToShoot = 2;
                    GameLogic.GameWeapons[x].Cartridge = 1;
                    GameLogic.GameWeapons[x].TimeToReload = 10;
                    //Type Rotation
                    GameLogic.GameWeapons[x].EndAngle = 180;
                    GameLogic.GameWeapons[x].DeltaTheta = 1;
                    GameLogic.GameWeapons[x].GirDirection = Game_Weapon.RotDirection.RECHS;
                    GameLogic.GameWeapons[x].RotationMode = Game_Weapon.RotMode.BACK;
                }
            }

            //Destroy all Childens
            parentTNodeWeapon.RemoveChildren();
            parentTNodeWeapon.Children.Clear();
            //Asociate Node To Scene
            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                parentTNodeWeapon.AddChild(GameLogic.GameWeapons[x].PapaWeapon);
            }
        }

        private void CreateMovFloor()
        {

        }

        private void VoyaZero()
        {
            int x;

            Console.WriteLine(DateTime.Now.ToString() + " VoyaZero\n");

            //Transform for the WEAPONS
            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                if (WeaponMarkeNodes[x].MarkerFound)
                {
                    Vector3 Newpos = GameLogic.GameWeapons[x].WeaponModel.Physics.PhysicsWorldTransform.Translation;
                    Newpos.Z = 0.2f;
                    Matrix mat =
                        Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                        Matrix.CreateScale(5.5f) *
                        Matrix.CreateTranslation(Newpos);

                    // Modify the transformation in the physics engine
                    ((NewtonPhysics)scene.PhysicsEngine).SetTransform(GameLogic.GameWeapons[x].WeaponModel.Physics, mat);
                }
            }

            for (x = 0; x < HinderMarkeNodes.Length; x++)
            {
                if (HinderMarkeNodes[x].MarkerFound)
                {
                    float Ang = Convert.ToSingle(Math.Atan(ObstModels[x].Physics.PhysicsWorldTransform.Left.Y / ObstModels[x].Physics.PhysicsWorldTransform.Left.X));
                    
                    Vector3 Newpos = ObstModels[x].Physics.PhysicsWorldTransform.Translation;
                    Newpos.Z = 0.2f;
                    Matrix mat =
                        Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                        Matrix.CreateRotationZ(Ang) *
                        Matrix.CreateScale(10.5f) *
                        Matrix.CreateTranslation(Newpos);

                    // Modify the transformation in the physics engine
                    ((NewtonPhysics)scene.PhysicsEngine).SetTransform(ObstModels[x].Physics, mat);
                }
            }
        }

        private void AssignNames()
        {
            int x;
            
            //Assign Name of Player
            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                GameLogic.Players[x].PlyName = GameLogic.CharacterNames[GameLogic.Players[x].PlySelect - 1];
            }
        }
        
        #endregion

        #region Running Scene

        private void RotateWp(int WeaponIdx)
        {
            Vector3 Newpos = GameLogic.GameWeapons[WeaponIdx].WeaponModel.Physics.PhysicsWorldTransform.Translation;
            Matrix mat =
                Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(GameLogic.GameWeapons[WeaponIdx].ActualAngle)) *
                Matrix.CreateScale(5.5f) *
                Matrix.CreateTranslation(Newpos);

            // Modify the transformation in the physics engine
            ((NewtonPhysics)scene.PhysicsEngine).SetTransform(GameLogic.GameWeapons[WeaponIdx].WeaponModel.Physics, mat);
        }

        private void TransformCubeReal()
        {
            int x;

            //Generate the transformation for each Weapon in the Level
            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                if (WeaponMarkeNodes[x].MarkerFound)
                {
                    Matrix mat = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * //Problem cordenates Blender
                            Matrix.CreateScale(5.5f) *                              //Scale
                            WeaponMarkeNodes[x].WorldTransformation *               //Actual POS
                            Matrix.Invert(groundMarkerNode.WorldTransformation);    //Find T for the transformation of the Ground

                    // Modify the transformation in the physics engine
                    ((NewtonPhysics)scene.PhysicsEngine).SetTransform(GameLogic.GameWeapons[x].WeaponModel.Physics, mat);
                }
            }

            for (x = 0; x < HinderMarkeNodes.Length; x++)
            {
                if (HinderMarkeNodes[x].MarkerFound)
                {
                    //solve Z problem
                    Vector3 Newpos = HinderMarkeNodes[x].WorldTransformation.Translation;
                    if (Newpos.Z < 0)
                        Newpos.Z = 0.2f;

                    Matrix mat = Matrix.CreateRotationX(MathHelper.ToRadians(90)) *
                            Matrix.CreateScale(10.5f) *
                            HinderMarkeNodes[x].WorldTransformation *
                            Matrix.Invert(groundMarkerNode.WorldTransformation);

                    // Modify the transformation in the physics engine
                    ((NewtonPhysics)scene.PhysicsEngine).SetTransform(ObstModels[x].Physics, mat);
                }
            }
        }

        #endregion

        #region Destoy & Clean Scene

        private void DestroyCars()
        {
            Console.WriteLine(DateTime.Now.ToString() + " DestroyCars\n");

            int x;

            //Delete car from pricipal Node
            parentTNodeCars.RemoveChildren();
            parentTNodeCars.Children.Clear();

            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                //Get paren from Geometry model from Car X
                TransformNode Taita = (TransformNode)CarModels[GameLogic.Players[x].PlySelect - 1].Parent;
                //Delete all Chlindren from paren und parent for all Children
                Taita.RemoveChildren();
                Taita.Children.Clear();
                //Delete all Tires from Model
                CarModels[GameLogic.Players[x].PlySelect - 1].RemoveChildren();
                CarModels[GameLogic.Players[x].PlySelect - 1].Children.Clear();

                CarModels[GameLogic.Players[x].PlySelect - 1].Physics = new PhysicsObject();
                CarModels[GameLogic.Players[x].PlySelect - 1].AddToPhysicsEngine = false;

                //GeometryNode NewCar = new GeometryNode("CarModel" + x.ToString());
                //NewCar.Model = CarModels[GameLogic.Players[x].PlySelect - 1].Model;
                //((Model)NewCar.Model).UseInternalMaterials = true;

                //CarModels[GameLogic.Players[x].PlySelect - 1].Dispose();
                //CarModels[GameLogic.Players[x].PlySelect - 1] = null;
                //CarModels[GameLogic.Players[x].PlySelect - 1] = NewCar;

                CarObjPhy[x].Model = null;
            }

            Manager3D.IsLoadCars = false;

            //GeometryNode a = new GeometryNode("hola");

            //GC.SuppressFinalize(CarObjPhy[0]);
            //CarObjPhy[0] = null;
            //CarModels[0].AddToPhysicsEngine = false;
            //CarModels[0].Physics = a.Physics;

            GC.Collect();

        }

        private void ReSetAllCars()
        {
            int x;
            //Reingreso los Autos
            for (x = 0; x < GameLogic.NumberPlayers; x++)
            {
                CarObjPhy[x].Collidable = true;
                CarObjPhy[x].Interactable = true;

                Matrix mat = Matrix.CreateTranslation(CarObjPhy[x].StartPos) *
                Matrix.CreateRotationX(MathHelper.ToRadians(90));

                ((NewtonPhysics)scene.PhysicsEngine).SetTransform(CarModels[x].Physics, mat);
            }
        }

        #endregion

        public void puercavida(IPhysicsObject physObj1, IPhysicsObject physObj2)
        {
            //Busco el Objeto Bala
            Vector3 VelBullet;
            Vector3 VelCar;
            Vector3 Difvel;

            if (physObj2.MaterialName == "Bullet")
            {
                VelBullet = ((NewtonPhysics)scene.PhysicsEngine).GetVelocity(physObj2);
                VelCar = ((NewtonPhysics)scene.PhysicsEngine).GetVelocity(physObj1);
            }
            else
            {
                VelBullet = ((NewtonPhysics)scene.PhysicsEngine).GetVelocity(physObj1);
                VelCar = ((NewtonPhysics)scene.PhysicsEngine).GetVelocity(physObj2);
            }


            Difvel = VelBullet - VelCar;
            float Dilivel = Difvel.LengthSquared();
            float Vel1 = VelBullet.LengthSquared();
            float vel2 = VelCar.LengthSquared();

            if (Dilivel > 50)
            {
                if(Vel1 > 90)
                    Notifier.AddMessage("Disparo");
                else
                    Notifier.AddMessage("Choque");
            }

            //if (VelBullet.LengthSquared() > 3.0f)
            //{
            //    Notifier.AddMessage("Disparo");
            //}
            //else
            //{
            //    if(VelCar.LengthSquared() > 3.0f)
            //        Notifier.AddMessage("Choque");
            //}
        }

        #region Network

        public void AgregaPlayer(int Player)
        {
            GameLogic.AddPlayer("Player X", Player);
        }

#if USE_SOCKET_NETWORK

        private void ConfigServer(int Port)
        {


            // Create a network object for Control the Car
            CtrlCarNetworkObj = new ControlCarNetworkObject();

            // Callback funtion for the Network obj "ControlCarCall"
            CtrlCarNetworkObj.CallbackFunc = ControlCarCall;

            // Create a network handler for handling the network transfers
            INetworkHandler networkHandler = new SocketNetworkHandler();

            //Creo El Servidor
            IServer server = null;

            //server
            server = new SocketServer(Port);

            //Numero de Clientes a esperar antes de la simulacion fisica
            State.NumberOfClientsToWait = 0;

            //Agrego CallBacks
            server.ClientConnected += new HandleClientConnection(ClientConnected);
            server.ClientDisconnected += new HandleClientDisconnection(ClientDisconnected);
            networkHandler.NetworkServer = server;

            // Assign the network handler used for this scene
            scene.NetworkHandler = networkHandler;

            // Add the ControlCar network object to the scene.
            scene.NetworkHandler.AddNetworkObject(CtrlCarNetworkObj);
        }

        private void ControlCarCall(CmdNetwork Cmmd, int IdPlayer, Vector3 DirCar, bool[] Buttons)
        {
            //Control del carro
            switch(Cmmd)
            {
                case CmdNetwork.ADD_PLY:
                    if(GameLogic.CurrentGameState != Game_Logic.Game_States.PLAY)
                        GameLogic.AddPlayer("Player" + IdPlayer.ToString(), IdPlayer);
                    break;
                case CmdNetwork.RM_PLY:
                    GameLogic.RmPlayer(IdPlayer);
                    break;
                case CmdNetwork.DATA_ACEL:

                    if (GameLogic.StateScreen == Game_Logic.Game_SCR.RUNNING)
                    {
                        switch (GameLogic.CurrentGameState)
                        {
                            case Game_Logic.Game_States.PLAY:
                                // Control the car steering with right and left arrow keys
                                if ((DirCar.Y > 20) && (DirCar.Y < 35))
                                    CarObjPhy[IdPlayer].SetSteering(-0.2f);
                                if ((DirCar.Y > 35) && (DirCar.Y < 55))
                                    CarObjPhy[IdPlayer].SetSteering(-0.3f);
                                if (DirCar.Y > 55)
                                    CarObjPhy[IdPlayer].SetSteering(-0.4f);
                                if ((DirCar.Y < -20) && (DirCar.Y > -35))
                                    CarObjPhy[IdPlayer].SetSteering(0.2f);
                                if ((DirCar.Y < -35) && (DirCar.Y > -55))
                                    CarObjPhy[IdPlayer].SetSteering(0.3f);
                                if (DirCar.Y < -55)
                                    CarObjPhy[IdPlayer].SetSteering(0.4f);
                                if ((DirCar.Y < 20) && (DirCar.Y > -20))
                                    CarObjPhy[IdPlayer].SetSteering(0);

                                // Control the car's forward torque with up and down arrow keys
                                //if (DirCar.X < -15)
                                //    CarObjPhy[IdPlayer].SetTireTorque(1);
                                //else if (DirCar.X > 15)
                                //    CarObjPhy[IdPlayer].SetTireTorque(-5);
                                //else
                                //    CarObjPhy[IdPlayer].SetTireTorque(0);
                                
                                CarObjPhy[IdPlayer].SetTireTorque(0);
                                CarObjPhy[IdPlayer].ApplyHandBrakes(0);
                                if (Buttons[0])//Disparo
                                {
                                    Vector3 PosCar, PosBall;
                                    PosCar = CarObjPhy[IdPlayer].PhysicsWorldTransform.Translation;
                                    PosBall = new Vector3(PosCar.X, PosCar.Y, PosCar.Z + 4);
                                    //Call Function for Bullets
                                    GameLogic.SoundGame.Laser1.Play();
                                    GameLogic.SoundGame.Laser1.Volume = 0.5f;
                                    //Manager3D.CreateShootBullet(PosBall, CarObjPhy[0].PhysicsWorldTransform.Left, Color.Blue);

                                    if (GameLogic.Players[IdPlayer].PlayerBullets.Count < GameLogic.MAX_NUM_BULLET)
                                    {
                                        Bullet Bala = new Bullet(PosBall, Manager3D.BulletModel, Manager3D.BulletrMat, CarObjPhy[IdPlayer].PhysicsWorldTransform.Left, groundMarkerNode);
                                        GameLogic.Players[IdPlayer].PlayerBullets.Add(Bala);
                                    }
                                    else
                                    {
                                        int Index = Bullet.SearchBall(GameLogic.Players[IdPlayer].PlayerBullets);
                                        if (Index > -1)
                                        {
                                            //Adicciono Bala
                                            GameLogic.Players[IdPlayer].PlayerBullets[Index].ReAddBullet(PosBall, CarObjPhy[IdPlayer].PhysicsWorldTransform.Left);
                                        }
                                    }
                                }
                                
                                if (Buttons[1])//Gas
                                    CarObjPhy[IdPlayer].SetTireTorque(1);
                                if(Buttons[2])//Freno
                                {
                                    if(DirCar.X < -15)
                                        CarObjPhy[IdPlayer].SetTireTorque(-5); //Reversa
                                    else
                                        CarObjPhy[IdPlayer].ApplyHandBrakes(25);
                                }
                                
                                break;
                            case Game_Logic.Game_States.SEL_PLY:
                                ValidatemenuAcel(DirCar, IdPlayer, Buttons);
                                break;
                        }
                    }
                    break;
            }
        }

        private void ClientDisconnected(string clientIP, int portNumber)
        {
            Notifier.AddMessage("Disconnected from " + clientIP + " at port " + portNumber);
        }

        private void ClientConnected(string clientIP, int portNumber)
        {
            Notifier.AddMessage("Accepted connection from " + clientIP + " at port " + portNumber);
        }

#endif
        #endregion
    }
}
