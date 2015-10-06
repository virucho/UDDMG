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

//#define GEN_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Referencias Sockets
using System.Threading;
using System.Net;
using System.Net.Sockets;
//Uso de archivo de configuracion
using System.Configuration;
//Generell Gamespace
using CarGameAR;

namespace AR_CarServer
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
    }

    public class PlayerControl
    {
        // Buttons
        public bool[] Buttons = new bool[3];

        //Axes
        public int[] AcelValue = new int[3];
    }
    
    class ARServer
    {
        /*********************** Variables Globales *********************/

        #region Member Fields

        //Datos de manejo conexiones
        private int MaxClients = 4;
        private string[] IpClients;
        //Socket
        private Socket listener;
        //Manejo de Eventos
        private ManualResetEvent allDone = new ManualResetEvent(false);

        //Manejo del Programa
        private bool Ciclo = true;

        //Datos de configuracion
        static int listenPort = Convert.ToInt16(ConfigurationManager.AppSettings["ListenPort"]);

        private PlayerControl[] PlyControl;

        // The current state for the Game
        Game_Logic GameLogic;

        //Delegate Callback AddPlayer
        public CallAddPlayer CallBackAddPlayer;

        #endregion

        #region Constructor

        public ARServer(Game_Logic GameLogic)
        {
            //Asination Logic Obj
            this.GameLogic = GameLogic;
        }

        #endregion

        #region Properties

        public bool ThreadActive
        {
            get { return Ciclo; }
            set { Ciclo = value; }
        }

        public int ServerPort
        {
            get { return listenPort; }
        }

        public PlayerControl[] ControlPly
        {
            get { return PlyControl; }
            set { PlyControl = value; }
        }

        #endregion

        #region Server Functions

        public int StartServer()
        {
            int x;
            //Init Values
            IpClients = new string[MaxClients];
            PlyControl = new PlayerControl[MaxClients];
            for (x = 0; x < MaxClients; x++)
            {
                PlyControl[x] = new PlayerControl();
            }

            //Logs de Identificacion de version
            Console.WriteLine(" *** Ar CarGame Server: " + Version() + " ***");
            Console.WriteLine("Listen Port: " + listenPort.ToString() + "\n");
            Console.WriteLine(DateTime.Now.ToString() + " Starting Server...\n");

            //Configuro y arranco el nuevo hilo de manejo de la conexion
            Thread hiloListening = new Thread(new ThreadStart(StartListening));
            hiloListening.Start();

            Console.WriteLine("\n\nWaiting for Connections....\n");

            return 0;
        }

        public void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(MaxClients);

                while (Ciclo)
                {
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    //Wait until a connection is made before continuing
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                Console.WriteLine("\nPress ENTER to Close App...");
                return;
            }
        }

        public void CloseServer()
        {
            Ciclo = false;
            allDone.Set();
            //close the Server
            listener.Close();
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            int IdPlayer = 0;
            
            try
            {
                if (Ciclo == true)
                {
                    allDone.Set();
                    Socket listener = (Socket)ar.AsyncState;
                    Socket handler = listener.EndAccept(ar);

                    //Agrego cliente para identificarlo
                    IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
                    //Busco si ya esta en la lista
                    IdPlayer = AcelNetwork.SchClientList(remoteIpEndPoint.Address.ToString(), ref IpClients);
                    //Si no se encontro agrego
                    if (IdPlayer == -1)
                    {
                        if (GameLogic.CurrentGameState == Game_Logic.Game_States.SEL_PLY ||
                            GameLogic.CurrentGameState == Game_Logic.Game_States.MENU_MODE ||
                            GameLogic.CurrentGameState == Game_Logic.Game_States.INTRO)
                        {
                            IdPlayer = AcelNetwork.AddClientList(remoteIpEndPoint.Address.ToString(), ref IpClients);

                            if (IdPlayer == -1)
                                Console.WriteLine("ERROR - Max Client\n");
                            else
                            {
                                Console.WriteLine("New Client: " + remoteIpEndPoint.Address);
                                //Call callBack for new player
                                if (CallBackAddPlayer != null)
                                    CallBackAddPlayer(IdPlayer);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Old Client: " + IdPlayer.ToString() + " - " + remoteIpEndPoint.Address);
                    }

                    // Create the state object 
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            int bytesRead;
            string[] AcelVariables;
            int Idplayer = 0;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            //Ip Client
            IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;

            try
            {
                // Read data from the client socket. 
                bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //Transformo los datos a typo string
                    content = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);

                    /************ Decodificacion de datos ********/
                    //Tomo los Datos antes del LF y CF
                    content = content.Split('\n')[0];
                    content = content.Replace("*", "");
                    content = content.Replace(" ", "");

                    //Separo los Daros de X, Y, Z
                    AcelVariables = content.Split('/');

                    if (AcelVariables.Length > 2)
                    {
                        
#if GEN_DEBUG
                        //Decodifico                        
                        Console.Clear();
                        Console.WriteLine("X " + AcelVariables[0]);
                        Console.WriteLine("Y " + AcelVariables[1]);
                        Console.WriteLine("Z " + AcelVariables[2]);
                        Console.WriteLine("Btn Fire " + AcelVariables[3]);
                        Console.WriteLine("Btn Gas " + AcelVariables[4]);
                        Console.WriteLine("Btn brake " + AcelVariables[5]);
#endif
                        
                        //Adquiero Id del Jugador 
                        Idplayer = AcelNetwork.SchClientList(remoteIpEndPoint.Address.ToString(), ref IpClients);

                        //Axes
                        PlyControl[Idplayer].AcelValue[0] = Convert.ToInt32(AcelVariables[0]);
                        PlyControl[Idplayer].AcelValue[1] = Convert.ToInt32(AcelVariables[1]);
                        PlyControl[Idplayer].AcelValue[2] = Convert.ToInt32(AcelVariables[2]);

                        //Botones
                        if (Convert.ToInt32(AcelVariables[3]) == 10)
                            PlyControl[Idplayer].Buttons[0] = true;
                        else
                            PlyControl[Idplayer].Buttons[0] = false;

                        if (Convert.ToInt32(AcelVariables[4]) == 10)
                            PlyControl[Idplayer].Buttons[1] = true;
                        else
                            PlyControl[Idplayer].Buttons[1] = false;

                        if (Convert.ToInt32(AcelVariables[5]) == 10)
                            PlyControl[Idplayer].Buttons[2] = true;
                        else
                            PlyControl[Idplayer].Buttons[2] = false;
                    }

                    if(Ciclo)
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                    return;
                }
            }
            catch (Exception Exa)
            {
                Console.WriteLine("ReadCallback" + Exa.ToString());

                //Elimino cliente de la lista
                //AcelNetwork.RmClientList(remoteIpEndPoint.Address.ToString(), ref IpClients);

                if (Ciclo)
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                return;
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);

                //handler.Shutdown(SocketShutdown.Both);
               // handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion

        #region Add Functions

        public string SetLocalIp()
        {
            String strHostName = string.Empty;
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            // Then using host name, get the IP address list..
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                if (addr[i].AddressFamily == AddressFamily.InterNetwork)
                    return addr[i].ToString();
            }

            return "Error";
        }

        public static string Version()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " - 28/01/2014";
        }

        public void AddPlayerOne()
        {
            IpClients[0] = "127.0.0.1";
        }

        #endregion

        public delegate void CallAddPlayer(int Idplayer);
    }
}
