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
using GoblinXNA.UI;
using GoblinXNA.Physics;
using GoblinXNA.Physics.Newton1;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/***** Namespaces to use GoblinsXNA *****/
using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Shaders;
using Model = GoblinXNA.Graphics.Model;

/***** Namespaces to Audio *****/
using Microsoft.Xna.Framework.Audio;
using GoblinXNA.Sounds;

namespace CarGameAR
{
    class Game_Logic
    {
        #region GameLogic Enums

        public enum Game_States
        {
            INTRO,
            HISTORY,
            MENU_MODE,
            SEL_PLY,
            MODE_PLAY,
            LEVEL_SEL,
            BUILDING,
            PLAY,
            PAUSE,
            POINTS_MENU,
            POINTS_MENU_T,
            END_GAME,
            CREDITS
        }
        
        public enum Game_SCR
        {
            GAME_LOADING,
            RENDERING,
            RUNNING
        }

        //Menus
        public enum Game_Mode
        {
            //SIGLE_PLY = 0,
            MULTI_PLY = 0,
            STORY = 1,
            CREDITOS = 2
        }

        public enum Play_MultiMode
        {
            DEATHMATCH = 0,
            TOURNAMENT = 1,
            TIME = 2
        }

        public enum Pausa_Status
        {
            RETURN = 0,
            EXIT = 1
        }

        public enum EndAction
        {
            REMATCH = 0,
            NEW_GAME = 1,
            MAIN_MENU = 2
        }

        public enum EndTour
        {
            NEXT_LEVEL = 0,
            EXIT_GAME = 1
        }

        #endregion

        #region Contants

        // Number of max Shooting Bullet
        public const int MAX_BULLET = 10;

        #endregion

        #region Member Fields

        // The current state for the Game
        private Game_States GameState;
        private Game_States LstState;
        //State for the Screen
        private Game_SCR ScrennState;

        // Game Mode
        private Game_Mode Mode;
        // Multiplayer Mode
        private Play_MultiMode MultiMode;
        //Curren Status for the game
        private Pausa_Status Pause_Status;
        //End action for the Match
        private EndAction EndMenu;
        //PersonHistory
        private int PersonHist;
        //Credits Counts
        private int CreditScr;
        private int CreditCount;
        //Nex Level
        private EndTour NextLevel;
        //Is Paused?
        private bool PauseGame;

        //Number of Current Players
        private int CurrentPlayer;
        //Info for each Player
        public Game_Player[] Players;
        //Numero Maximo de Jugadores
        private int MaxPlayers;

        //History
        private string[] HistoryIni;
        private string[] HistoryEnd;

        //Weapons Objets
        private Game_Weapon[] Weapons;
        //Numero Maximo de Armas
        private int MaxWeapons;

        //Game Level
        private int Level;
        //Level Height
        private int LevelH;

        private static string[] PersonName = new string[] { "Briaana",
                                                            "Ralf-7",
                                                            "C788",
                                                            "Brianna 2",
                                                            "Gullkkaaa",
                                                            "Drall",
                                                            "Kahn",
                                                            "Robert Swanson"};

        private SoundManager SndGame;                   //Class to Manager the Soundseffects

        #endregion

        #region Constructor

        public Game_Logic(int NumMaxPLayer)
        {
            int x;

            SndGame = new SoundManager();

            /**********Creation Logic Game ********/
            
            GameState = Game_States.INTRO;
            LstState = Game_States.INTRO;
            ScrennState = Game_SCR.RUNNING;
            CurrentPlayer = 0;
            MaxPlayers = NumMaxPLayer;

            Players = new Game_Player[MaxPlayers];
            for (x = 0; x < MaxPlayers; x++)
            {
                Players[x] = new Game_Player();
            }

            //Only the Player 1 Is connected (use the KeyBoard)
            AddPlayer("Player 1", 1);

            //Defino Personaje Por Player
            Players[0].PlySelect = 1;
            //Players[1].PlySelect = 4;
            //Players[2].PlySelect = 5;
            //Players[3].PlySelect = 8;
            Players[1].PlySelect = 2;
            Players[2].PlySelect = 3;
            Players[3].PlySelect = 4;

            /*** History ****/
            HistoryIni = new string[8];
            HistoryEnd = new string[8];
            CreateHistory();

            //Weapons
            Weapons = new Game_Weapon[MaxPlayers];
            MaxWeapons = NumMaxPLayer;

            //pause
            PauseGame = false;

            //Initial Level
            Level = 0;
            LevelH = 10;

            //Mode Multiplayer
            MultiMode = Play_MultiMode.DEATHMATCH;
            //Mode of Game
            Mode = Game_Mode.MULTI_PLY;
            //Status
            Pause_Status = Pausa_Status.RETURN;
        }

        #endregion

        #region Properties

        public Game_States CurrentGameState
        {
            get { return GameState; }
            set { GameState = value; }
        }

        public Game_States LastGameState
        {
            get { return LstState; }
            set { LstState = value; }
        }

        public int NumberPlayers
        {
            get { return CurrentPlayer; }
            //set { CurrentPlayer = value; }
        }

        public int MAX_NUM_BULLET
        {
            get { return MAX_BULLET; }
        }

        public Game_SCR StateScreen
        {
            get { return ScrennState; }
            set { ScrennState = value; }
        }

        public Game_Mode GameMode
        {
            get { return Mode; }
            set { Mode = value; }
        }

        public Play_MultiMode MultiplayerMode
        {
            get { return MultiMode; }
            set { MultiMode = value; }
        }

        public Pausa_Status Pause_State
        {
            get { return Pause_Status; }
            set { Pause_Status = value; }
        }

        public EndAction EndActionMenu
        {
            get { return EndMenu; }
            set { EndMenu = value; }
        }

        public EndTour EndActionTour
        {
            get { return NextLevel; }
            set { NextLevel = value; }
        }

        public int CharHistory
        {
            get { return PersonHist; }
            set { PersonHist = value; }
        }

        public bool IsPaused
        {
            get { return PauseGame; }
            set { PauseGame = value; }
        }

        public int ActualLevel
        {
            get { return Level; }
            set { Level = value; }
        }

        public int LevelHeight
        {
            get { return LevelH; }
            set { LevelH = value; }
        }

        public SoundManager SoundGame
        {
            get { return SndGame; }
            set { SndGame = value; }
        }

        public Game_Weapon[] GameWeapons
        {
            get { return Weapons; }
            set { Weapons = value; }
        }

        public string[] CharacterNames
        {
            get { return PersonName; }
        }

        public string[] HistoryEndgame
        {
            get { return HistoryEnd; }
        }

        public string[] HistoryPers
        {
            get { return HistoryIni; }
        }

        public int CreditCounter
        {
            get { return CreditCount; }
            set { CreditCount = value; }
        }

        public int CreditScreen
        {
            get { return CreditScr; }
            set { CreditScr = value; }
        }

        #endregion

        #region Public Methods

        public int AddPlayerSch(string PlyName)
        {
            int x;

            for (x = 0; x < MaxPlayers; x++)
            {
                if (Players[x].IsConnected == false)
                {
                    Players[x].ConnectPLayer(x);
                    Players[x].PlyName = PlyName;
                    CurrentPlayer++;
                    return x;
                }
            }

            return -1;
        }

        public void AddPlayer(string PlyName, int IdPly)
        {
            if (Players[IdPly -1].IsConnected == false)
            {
                Players[IdPly -1].ConnectPLayer(IdPly -1);
                Players[IdPly -1].PlyName = PlyName;

                switch(IdPly)
                {
                    case 1:
                        Players[IdPly - 1].InitialPos = new Vector3(50, 10, 10);
                        break;
                    case 2:
                        Players[IdPly - 1].InitialPos = new Vector3(-50, 10, 10);
                        break;
                    case 3:
                        Players[IdPly - 1].InitialPos = new Vector3(80, 30, 10);
                        break;
                    case 4:
                        Players[IdPly - 1].InitialPos = new Vector3(-80, 30, 10);
                        break;
                }

                CurrentPlayer++;
            }

        }

        public void RmPlayer(int Id)
        {
            Players[Id -1].DisconnectPlayer();
            CurrentPlayer--;
        }

        public void ResetCurrentPlayers()
        {
            int x;
            for (x = 0; x < CurrentPlayer; x++)
                Players[x].ResetPlayer();
        }

        public void ReviveCurrentPlayers()
        {
            int x;
            for (x = 0; x < CurrentPlayer; x++)
                Players[x].RevivePlayer();
        }

        public void CreateHistory()
        {
            HistoryEnd[0] = "After the winning the final tournament Briaana was \ngreeted by the producers, they offered her one Billion \ncredits to start working as a spokesman to \"Robert \nSwanson Productions\" She Accepted. However, she disappeared \na short time after that. She was last seen going to the \n house of the family of the girl she had killed. One month \nlater the family received one billion dollars.";
            HistoryEnd[1] = "Following his victory Ralf-7 sent c788 to the recycling \ncamps ending the robot. He then would demand Robert \nSwanson and against all odds win the trial. This made him \nreally popular in all earth and mars as well. He was \nelected president of the united earth and is now working \nto create a unified human Martian state. Even as president, \nhe visits his fallen team tomb once every week.";
            HistoryEnd[2] = "Winning the championship did not meant anything to C788 he went on and continue to participate in the show until \n500 years later when the show and humanity were \ndestroyed by a two comets. Then he decided to search for \nmore living beings to destroy. He spent millions of years \ndoing this, until life ceased to exist in the universe.";
            HistoryEnd[3] = "Due to her victory the tournament the producers realized \nthat Briaana2 was free from her control. However, it was \ntoo late, brianna2 killed them. She spent the following 5 \nyears searching for Robert Swanson, finally finding him \nin a secret base on the Andes Mountains. Once inside, she \nrealized that she could not killed him, because his \ncounsiusness was now inside every computer in \nthe world. She is still trying to find a way to kill him.";
            HistoryEnd[4] = "His plan worked like clockwork. His victory inspired all \nMartians to revolt and, after 2 years, defeat the humans \noppresors and make them their slaves. However, there \nwas one thing Gullkkaaa wanted to do. He wanted to finish \nRobert Swanson, with the help of all Martians he develop \na computer virus that deleted all traces of Robert \nSwanson consciousness in all the universe.";
            HistoryEnd[5] = "Drall took over the moon base using his powers. He then \nrepaired c788 and used it to destroy the mobile corps \nattack. He then spent 2 years building an army to conquer \nthe world. However, the united earth government decided \nto nuke the moon with 10000 missiles. His body was never \nfound.";
            HistoryEnd[6] = "After his victory, Gengis Khan became very famous. He \ntoured the world and soon realize that there was more \nto living than conquering others. He married a supermodel \nand went to live at a paradise island. There he decided that \nhis time was done and gave Richard control of his body. \nRichard finally got what he wanted, and 60 years later \nhe died a happy man.";
            HistoryEnd[7] = "Robert Swanson proved that he was the best at \neverything, no force in the entire world could oppose him. \nwhen it was discovered that his consciousness was \neverywhere and basically ran everything, it didn't \nmattered to anyone, everybody loved him. 500 years later, \nhis counssunes had taken over everything, the humans, \nMartians and robots were united as Robert Swanson \ncontrolled them all like part of his body. He then sent \nmissionaries to all corners of the universe, they \"convert \" \nother races to serve him. 1 million years later Robert \nSwanson had the power to control everything even the \nbirth of stars. One day he decided to test his powers so he \ncreated a new planet, that was eventually called earth.";

            HistoryIni[0] = "Since a young age Briaana had a love affair\nwith cars. She entered the mobile Corps\nwhen was 16 and participated in the first\ninvasion of mars. She decided to start a\ncarrear in racing after the war. During\nthis time she married and had two kids.\nRobert swanson offered a job in his new\nprogram, she declined, she didn't wan't to\nkill anymore.\n\nHowever, her life would suffer a drastic\nchange. A martian terrorist attack killed\nher two sons and left her husband disbaled\nCompletly heartbroken, she decided to kill\nall the terrorists. During her rampage she\nkilled an innocent girl. For this she was\nsenteced to death.\n\nRobert Swanson learned about this and\noffered to free her and clone her kids if\nshe won the UDDMG for 20 years. Since then\nshe has participated in over 12\ntournaments winning all of them.\n";
            HistoryIni[1] = "He was created to the perfect soldier. and \nhe was. He was not only genetically \ngenitically modified to be superior in every-\nway, but was educated in the best military \nschools since he was 3. At age 6 he killed \nhis first man. At age 12 he completed his \nfirst solo mission, in Mars. At age 17 he \nofficially entered the Space Marines were \nhe served with distinction for 15 years. He \nwould then work for a private contractor \nas riot police. He is also known for being the \none who put down C788 after it's killing \nspree, which also took the entire squad of \nRalf-7. When the verdict came and C788 \nwas set free he went berserk and searched \nfor Robert Swanson. \n\nSwanson offered the chance to kill  c788 \nif he would just entered the show. He \naccepted.";
            HistoryIni[2] = "5 years ago C788 was activated. He  was \nsupposed to bring peace to all humanity \nbut malfunction on his main processor \nmade him a cold blooded killer. The \nscientists  which were building him knew \nabout this but decided to activated him \nanyway because they wanted to get \nfamous. However, once activated he \nkilled all the people working in the project.\nHe was apprehended by ralf-7 riot squad and \nwas the first robot to stand trial. \nHe was sentenced to the recycling chambers. \n\nRobert Swanson saw potential on this \nmachine and offered him a place on his show \nafter buying the whole judicial system, \nC788 was spared and went to become a \nstar on the show. \n\nHe holds no grudge against ralf-7 but \nlooks forward to killing him.";
            HistoryIni[3] = "Since Briaana was having such a huge success \npeople were starting to get bored. The \nproducers of the show decided to clone her \nin hopes of making a real nemesis for her. \n\nThis worked perfectly. For everyone except \nfor Briaana2 ,as she was named. She was \nmade with a subconscious switch on her \nwhich prevented her from killing Briaana \nbecause that would hurt the ratings. \nHowever the last Briaana2 learned about \nthis and switched it off so that the next \none would not have it. After killing herself \na new briaana2 was automatically cloned. \n\nShe has the memories of all the others \nBriaanas and can't wait to avenge them.";
            HistoryIni[4] = "Gullkkaaa was a warrior monk of the \nbleeding eye for 100 years until the human \ninvasion 25 years ago. He founded the \nresistance movement in Mars and \nconducted terrorist activities on earth. \nHe was responsible for the nuclear bomb \nattack in Buenos Aires and the flesh eating \nbacteria that decimated Lima. However, he \nwas finally caught by Ralf-7. \n\nRobert Swanson had the idea of making his \n execution a public event, however it didn't \n went as planned. He defeated all his \nopponents including Robert Swanson. This \nhappened seven more times. The show \nproducers notice that since then the \nratings in mars had exploded and decided \nto keep him alive some more time. \n\n Gullkkaaa now accepts his fate and sees \nthe show as a mean to inspire his people.";
            HistoryIni[5] = "A computer programmer  from Colombia \nLuis, also known as Drall in the web, was \nknown throughout the world for making \nawesome computer games. However, at \none point during a secret project in \nGermany he went mad and killed all of his \nteam mates. \n\nHe was spared of death row because Robert \nSwanson really like his games, and bought \nhis freedom. However, he would have to \nparticipate in the show for many years. \n\nSince that day in Germany, Drall has been \nable to interface directly with computers \nusing only his mind, this made him a very \ngood rider and is the reason he is still alive.";
            HistoryIni[6] = "Richard Stevenson was always weak, the \nbullies always picked him at school and \nwhen he finally got a job at MIT his \ncolleges stole all of his work. 20 years \nago he decided enough was enough, so he \nbuilt a time machine to capture the mind of \nGenhis Kahn, with this, he thought, he would \ndestroy his enemies. \n\nHe spent 10 years searching for Genhis Kahn \nand when he finally found him, he was able \nto get his mind but when he put it in his \nbody the mind of Kahn took over. \n\nGenhis kahn returned to the present were \nhe started his plan to conquer the \nworld. The first fase was to be famous. \nThe Ultimate Demolition Derby was \nthe perfect place to do this.";
            HistoryIni[7] = "He is the most interesting man in the world \nand he knows it. He owns almost all of \nearth and is the man responsible for the \nUltimate Demolition Derby on the Moon \nwith guns. He participated on his free time \nand was never defeated until Gullkkaaa \ncame in. \n\nTo many Robert Swanson died that day, but \nhe still makes appearances on the show, \nhowever, nobody has actually seen him \nonly his car. \n\nit is speculated that he has put his mind \nin a computer and has made many copies. \nif this is true, he could be anywhere and \nwould be immortal.";
        }

        #endregion

        #region CallBacks

        public void CallbackBulletGround(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10f)
            {
                //Notifier.AddMessage("Nock-Nock - Contact with speed of " + contactSpeed);
                SoundGame.GroundSound.Play();
                SoundGame.GroundSound.Volume = 0.2f;
            }

        }

        public void CallbackBulletObst(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10f)
            {
                //Notifier.AddMessage("Nock-Nock - Contact with speed of " + contactSpeed);
                SoundGame.MetalSound.Play();
                SoundGame.MetalSound.Volume = 0.2f;
            }

        }

        public void CallbackCarObst(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10f)
            {
                Notifier.AddMessage("Nock-Nock - Contact with speed of " + contactSpeed);
            }
            SoundGame.CarCrash1.Play();
        }

        /*********************** Bullet ********************/

        public void CallbackBulletHitCar0(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10.1f)
            {
                Notifier.AddMessage("Pum-Pum - VEL " + contactSpeed);
                Players[0].PlyHealth -= 2;
            }
        }

        public void CallbackBulletHitCar1(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10.1f)
            {
                Notifier.AddMessage("Pum-Pum - VEL " + contactSpeed);
                Players[1].PlyHealth -= 2;
            }
        }

        public void CallbackBulletHitCar2(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10.1f)
            {
                Notifier.AddMessage("Pum-Pum - VEL " + contactSpeed);
                Players[2].PlyHealth -= 2;
            }
        }

        public void CallbackBulletHitCar3(Vector3 contactPosition, Vector3 contactNormal,
                float contactSpeed, float colObj1ContactTangentSpeed, float colObj2ContactTangentSpeed,
                Vector3 colObj1ContactTangentDirection, Vector3 colObj2ContactTangentDirection)
        {
            // Only play sound if the collision/contact speed is above 4
            if (contactSpeed > 10.1f)
            {
                Notifier.AddMessage("Pum-Pum - VEL " + contactSpeed);
                Players[3].PlyHealth -= 2;
            }
        }

        public void prucall(IPhysicsObject physObj1, IPhysicsObject physObj2)
        {
            Vector3 vel = physObj1.InitialLinearVelocity;
            Vector3 vel2 = physObj2.InitialLinearVelocity;
            
            Notifier.AddMessage("Aquiiiiiii");
        }

        public void oooocall()
        {
            Notifier.AddMessage("jodeerrr");
        }

        #endregion
    }

    /******************* Class PLAYER ***********************/

    class Game_Player
    {
        #region Member Fields

        //Informative
        private string Nombre;                          //Name of Player
        private int CharSelected;                       //Id Character Selected
        private Color PlyColor;                         //Color Player
        //Config
        private bool IsOK;                              //is OK
        private bool IsConn;                            //Is the Control Connected
        //car
        private Vector3 InitPosCar;                     //Init Pos Car
        private Vector3 AcPosCar;                       //Actual Pos Car
        //Play
        private int Health;                             //health
        private int GPopularity;                        //popularity
        private bool Dead;                              //Is Dead
        private List<Bullet> PlyBullets;                //Bullets For Player
        //Points
        private int Kills;                              //Number of Kills
        private int Deads;                              //Number of Deads in Tournament

        #endregion

        #region Constructor

        public Game_Player()
        {
            CharSelected = 1;
            IsConn = false;
            PlyColor = Color.White;
            Health = 100;
            GPopularity = 0;
            Kills = 0;
            Deads = 0;
            Nombre = "player X";

            InitPosCar = new Vector3(60, 50, 10);
            AcPosCar = new Vector3(0, 10, 40);

            IsOK = false;
            IsDead = false;

            PlyBullets = new List<Bullet>();
        }

        #endregion

        #region Properties

        public bool IsConnected
        {
            get { return IsConn; }
        }

        public Color PlayerColor
        {
            get { return PlyColor; }
        }

        public int PlySelect
        {
            get { return CharSelected; }
            set { CharSelected = value; }
        }

        public bool IsPlyOK
        {
            get { return IsOK; }
            set { IsOK = value; }
        }

        public bool IsDead
        {
            get { return Dead; }
            set { Dead = value; }
        }

        public Vector3 InitialPos
        {
            get { return InitPosCar; }
            set { InitPosCar = value; }
        }

        public Vector3 ActualPos
        {
            get { return AcPosCar; }
            set { AcPosCar = value; }
        }

        public int Popularity
        {
            get { return GPopularity; }
            set { GPopularity = value; }
        }

        public int PlyHealth
        {
            get { return Health; }
            set { Health = value; }
        }

        public string PlyName
        {
            get { return Nombre; }
            set { Nombre = value; }
        }

        public int NumKills
        {
            get { return Kills; }
            set { Kills = value; }
        }

        public int NumDeads
        {
            get { return Deads; }
            set { Deads = value; }
        }

        public List<Bullet> PlayerBullets
        {
            get { return PlyBullets; }
            set { PlyBullets = value; }
        }

        #endregion

        #region Public Methods

        public void ConnectPLayer(int Id)
        {
            IsConn = true;
            //Defino Color Por Player
            switch(Id)
            {
                case 0:
                    PlyColor = Color.Blue;
                    break;
                case 1:
                    PlyColor = Color.Red;
                    break;
                case 2:
                    PlyColor = Color.Green;
                    break;
                case 3:
                    PlyColor = Color.Yellow;
                    break;
            }
        }

        public void DisconnectPlayer()
        {
            IsConn = false;
        }

        public void ResetPlayer()
        {
            Health = 100;
            GPopularity = 0;
            Kills = 0;
            Deads = 0;

            //InitPosCar = new Vector3(30, 10, 10);
            AcPosCar = new Vector3(0, 10, 40);

            IsOK = false;
            IsDead = false;
        }

        public void RevivePlayer()
        {
            Health = 100;
            GPopularity = 0;

            IsDead = false;
        }

        #endregion
    }

    /***************** Class Bullet ***********************/

    class Bullet
    {
        #region Member Fields

        // Time of live for Bullets
        public const int BULLET_TIMELIVE = 1;

        private GeometryNode ShootBullet;           //Geometry for Bullet
        private int livetime;                       //live time for Bullet in the Physic World
        private bool isvisible;                     //Is in the World?
        private TransformNode BulletTrans;          //Transfor Node for Bullet

        #endregion

        #region Constructors

        public Bullet(Vector3 InitPos, PrimitiveModel BulletModel, Material Material, Vector3 DirBullet, MarkerNode grdMarkerNode)
        {
            //Create Bullet
            ShootBullet = new GeometryNode();
            ShootBullet.Name = "ShootBullet" + ShootBullet.ID;
            ShootBullet.Model = BulletModel;

            ShootBullet.Material = Material;
            ShootBullet.Physics.Interactable = true;
            ShootBullet.Physics.Collidable = true;
            ShootBullet.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            ShootBullet.Physics.Mass = 60f;
            ShootBullet.Physics.MaterialName = "Bullet";
            ShootBullet.AddToPhysicsEngine = true;

            // Assign the initial velocity to this shooting box
            ShootBullet.Physics.InitialLinearVelocity = new Vector3(DirBullet.X * 80, DirBullet.Y * 80, DirBullet.Z * 50);

            BulletTrans = new TransformNode();
            BulletTrans.Translation = InitPos;

            grdMarkerNode.AddChild(BulletTrans);
            BulletTrans.AddChild(ShootBullet);

            //Normal asignament
            isvisible = true;

            //Agrego Segundo desde que se creo
            livetime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds) + BULLET_TIMELIVE;
        }

        #endregion

        #region Public Methods

        public void RemoveBullet()
        {
            isvisible = false;
            ShootBullet.Physics.Interactable = false;
            ShootBullet.Physics.Collidable = false;
            BulletTrans.Translation = new Vector3(0, 0, -10);
        }

        public static int SearchBall(List<Bullet> Balas)
        {
            int x;

            for(x=0; x < Balas.Count; x++)
            {
                if (Balas[x].IsVisible == false)
                    return x;
            }

            return -1;
        }

        public void ReAddBullet(Vector3 InitPos, Vector3 DirBullet)
        {
            isvisible = true;
            BulletTrans.Translation = InitPos;
            ShootBullet.Physics.Interactable = true;
            ShootBullet.Physics.Collidable = true;

            //Agrego 2 Segundos desde que se creo
            livetime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds) + BULLET_TIMELIVE;

            // Assign the initial velocity to this shooting box
            ShootBullet.Physics.InitialLinearVelocity = new Vector3(DirBullet.X * 80, DirBullet.Y * 80, DirBullet.Z * 50);
        }

        #endregion

        #region Properties

        public GeometryNode GeometryBullet
        {
            get { return ShootBullet; }
            set { ShootBullet = value; }
        }

        public int LiveTime
        {
            get { return livetime; }
            set { livetime = value; }
        }

        public bool IsVisible
        {
            get { return isvisible; }
            set { isvisible = value; }
        }

        #endregion
    }

    /***************** Class Weapon ***********************/

    class Game_Weapon
    {
        #region Enums
        
        //Weapons
        public enum RotDirection
        {
            LINKS,
            RECHS
        };

        public enum RotMode
        {
            CONTINUE,
            BACK
        };

        #endregion

        #region Constants
        // Number of max Shooting Bullet
        public const int NORMAL_FOV = 30;
        public const int DELTA_THETA = 1;
        public const int WAIT_RELOAD = 55; //Entre es bien 50-70
        public const int WAIT_SHOOT = 55; //Entre es bien 50-70
        #endregion

        #region Member Fields

        private bool Active;                            //Is Active?

        private int FOV;                                //Fiel of View
        private Vector3 ActPos;                         //Actual Pos X,Y

        private int DeltaGiro;                          //Delta turn
        private RotDirection GirDir;                    //Direction 
        private RotMode RotaMode;                       //Rotation Mode
        private int ActAngle;                           //Actual Angle
        private int InitAngle;                          //Initial Angle
        private int MaxAng;                             //Max Angle
        private double RadActAngle;                     //Actual angle in Radians
        private double RadOffset;

        private Vector3 BulletOffset;                   //Offset x,y,z from Bullet Pos
        private List<Bullet> WBullets;                  //Bullets For Weapon
        private PrimitiveModel BulletModel;             //Bullet Model
        private Material BulletMat;                     //Material For Bullets
        
        private int ReloadTime;                         //Time between shoots
        private int CountToReload;                      //Count To Reload
        private int WaitBullet;                         //Time to Shoot
        private int CountToShoot;                       //Count to Shoot
        private int NumBullets;                         //Num of Bullets to Shoot in Sequency
        private int CountOfBullets;                     //Count of Bullets

        private MarkerNode GMarkerNode;                 //Ground Marke Node
        private TransformNode PapaTransNode;            //Parent T node
        private GeometryNode ObstModel;                 //Geometry Node und Model

        #endregion

        #region Constructor

        public Game_Weapon(PrimitiveModel BulletModel, Material BulletMaterial, MarkerNode GrdMarkerNode)
        {
            FOV = NORMAL_FOV;
            DeltaGiro = DELTA_THETA;
            Active = true;
            ActAngle = 0;
            InitAngle = 0;
            MaxAng = 360;
            RadActAngle = 0;
            GirDir = RotDirection.RECHS;
            RotaMode = RotMode.CONTINUE;

            this.BulletModel = BulletModel;
            BulletMat = BulletMaterial;
            GMarkerNode = GrdMarkerNode;

            WBullets = new List<Bullet>();
            
            //Config type of Shoot
            WaitBullet = WAIT_SHOOT;
            CountToShoot = WaitBullet;
            ReloadTime = WAIT_RELOAD;
            CountToReload = 0;
            CountOfBullets = 0;
            NumBullets = Game_Logic.MAX_BULLET;

            RadOffset = Math.PI * (95) / 180.0;
            BulletOffset = new Vector3(0, 10, 30);
        }

        #endregion

        #region Properties

        public int FieldOfVision
        {
            get { return FOV; }
            set { FOV = value; }
        }

        public GeometryNode WeaponModel
        {
            get { return ObstModel; }
            set { ObstModel = value; }
        }

        public TransformNode PapaWeapon
        {
            get { return PapaTransNode; }
            set { PapaTransNode = value; }
        }

        public int DeltaTheta
        {
            get { return DeltaGiro; }
            set { DeltaGiro = value; }
        }

        public double OffsetTheta
        {
            get { return RadOffset; }
            set { RadOffset = value; }
        }

        public bool IsActive
        {
            get { return Active; }
            set { Active = value; }
        }

        public int ActualAngle
        {
            get { return ActAngle; }
            //set { ActAngle = value; }
        }

        public int InitialAngle
        {
            get { return InitAngle; }
            set { InitAngle = value; }
        }

        public int EndAngle
        {
            get { return MaxAng; }
            set { MaxAng = value; }
        }

        public Vector3 ActualPosXY
        {
            //get { return ActPos; }
            set { ActPos = value; }
        }

        public RotDirection GirDirection
        {
            get { return GirDir; }
            set { GirDir = value; }
        }

        public RotMode RotationMode
        {
            get { return RotaMode; }
            set { RotaMode = value; }
        }

        public int WaitToShoot
        {
            get { return WaitBullet; }
            set { WaitBullet = value; }
        }

        public int TimeToReload
        {
            get { return ReloadTime; }
            set { ReloadTime = value; }
        }

        public int Cartridge
        {
            get { return NumBullets; }
            set { NumBullets = value; }
        }

        public List<Bullet> WeaponBullet
        {
            get { return WBullets; }
            set { WBullets = value; }
        }

        #endregion

        #region Public Methods

        public bool Dispara()
        {
            if (CountOfBullets < NumBullets)
            {
                if (CountToShoot < WaitBullet) //Wait for Reload Time in Frames/seg
                {
                    CountToShoot++;
                    return false;
                }

                float X, Y;

                X = Convert.ToSingle(Math.Cos(RadActAngle + RadOffset));
                Y = Convert.ToSingle(Math.Sin(RadActAngle + RadOffset));

                Vector3 DirBullet = new Vector3(X, Y, 0.0f);

                //Restart Counts
                CountToShoot = 0;

                //Increment Bullet Count
                CountOfBullets++;

                //Search if there are Free Bullets Slots
                if (WBullets.Count < Game_Logic.MAX_BULLET)
                {
                    Bullet Bala = new Bullet(ActPos + BulletOffset, BulletModel, BulletMat, DirBullet, GMarkerNode);
                    WBullets.Add(Bala);
                }
                else
                {
                    int Index = Bullet.SearchBall(WBullets);
                    if (Index > -1)
                    {
                        //Adicciono Bala
                        WBullets[Index].ReAddBullet(ActPos + BulletOffset, DirBullet);
                    }
                }

                return true;
            }
            else
            {
                if (CountToReload < ReloadTime) //Wait for Reload Time in Frames/seg
                    CountToReload++;
                else
                {
                    CountToReload = 0;
                    CountOfBullets = 0;
                }
                
                return false;
            }
        }

        public void AddPos(Vector3 ActualPos)
        {
            this.ActPos = ActualPos;
            //this.ActPos.Z = 30;
        }

        public void Rotation()
        {
            //Chage the angle
            if(GirDir == RotDirection.RECHS)
                ActAngle += DeltaGiro;
            else
                ActAngle -= DeltaGiro;

            //Validate of the tipe of Rotation
            if (RotaMode == RotMode.CONTINUE)
            {
                if (ActAngle > MaxAng)
                    ActAngle -= MaxAng;
            }
            else
            {
                if (ActAngle > MaxAng)
                    GirDir = RotDirection.LINKS;
                if (ActAngle < InitAngle)
                    GirDir = RotDirection.RECHS;
            }

            //Saco Radianes
            RadActAngle = Math.PI * ActAngle / 180.0;

            //parentTransNode.Translation = ActPos;
            //parentTransNode.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.ToRadians(ActAngle));
        }

        public bool IsInRange(Vector3 PosTarget)
        {
            double fact1, fact2;

            fact1 = Math.Pow((ActPos.X - PosTarget.X),2);
            fact2 = Math.Pow((ActPos.X - PosTarget.X), 2);

            fact1 = Math.Sqrt(fact1 + fact2);

            if (FOV > fact1)
                return true;
            else
                return false;
        }

        #endregion
    }

}
