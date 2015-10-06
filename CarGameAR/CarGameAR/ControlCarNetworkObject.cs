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
using System.Text;

using Microsoft.Xna.Framework;

using GoblinXNA.Network;
using GoblinXNA.Helpers;

namespace MyObj_Networking
{
    public enum CmdNetwork
    {
        ADD_PLY = 0x0A,
        RM_PLY  = 0x0B,
        DATA_ACEL = 0x0C
    };

    public enum CarButtons
    {
        BREAKS = 0x0A,
        SCAPE = 0x0B,
        _OK_ = 0x0C
    };
    
    /// <summary>
    /// An implementation of INetworkObject interface for transmitting signals from the Accelerometer
    /// to control de movements from the cars.
    /// </summary>
    public class ControlCarNetworkObject : INetworkObject
    {
        /// <summary>
        /// A delegate function to be called when Data of the Control's Car is sent over the network
        /// </summary>
        /// <param name="IdPlayer">a Id oft each Player</param>
        /// <param name="CDirection">The direction in X axe</param>
        public delegate void ShootFunction(CmdNetwork Cmmd, int Player, Vector3 CarDir, bool[] isButtons);

        #region Member Fields

        private bool readyToSend;           //Bit to able the data to send
        private bool hold;                  //
        private int sendFrequencyInHertz;   //able data to send periodicle with the refresh Frequency

        private bool reliable;
        private bool inOrder;

        // Data for Car´s Control
        private int idPlayer;
        private Vector3 CDirection;
        private ShootFunction callbackFunc;
        private bool[] Buttons;

        #endregion

        #region Constructors

        public ControlCarNetworkObject()
        {
            readyToSend = false;
            hold = false;
            sendFrequencyInHertz = 0;

            reliable = true;
            inOrder = true;

            Buttons = new bool[3];
        }

        #endregion

        #region Properties
        public String Identifier
        {
            get { return "ControlCarNetworkObject"; }
        }

        public bool ReadyToSend
        {
            get { return readyToSend; }
            set { readyToSend = value; }
        }

        public bool Hold
        {
            get { return hold; }
            set { hold = value; }
        }

        public int SendFrequencyInHertz
        {
            get { return sendFrequencyInHertz; }
            set { sendFrequencyInHertz = value; }
        }

        public bool Reliable
        {
            get { return reliable; }
            set { reliable = value; }
        }

        public bool Ordered
        {
            get { return inOrder; }
            set { inOrder = value; }
        }

        public ShootFunction CallbackFunc
        {
            get { return callbackFunc; }
            set { callbackFunc = value; }
        }

        public int IdPlayer
        {
            get { return idPlayer; }
            set { idPlayer = value; }
        }

        public Vector3 Direction
        {
            get { return CDirection; }
            set { CDirection = value; }
        }
        #endregion

        #region Public Methods

        public byte[] GetMessage()
        {
            // 1 byte: I Player
            // 4 bytes: Direction car
            byte[] data = new byte[1 + 4];

            data[0] = (byte)idPlayer;
            ByteHelper.FillByteArray(ref data, 1, BitConverter.GetBytes(CDirection.X));

            return data;
        }

        public void InterpretMessage(byte[] msg, int startIndex, int length)
        {
            // 1 byte: Type of Msg {    Add Player = 0x0A,
            //                          Remove Ply = 0x0B,
            //                          Data Acel  = 0x0C
            // 1 byte: Id player
            // 4 bytes: Direction car X
            // 4 bytes: Direction car Y
            // 4 bytes: Direction car Z
            // 1 byte: Button Fire is pressed = 0x0A
            // 1 byte: Button Gas is pressed = 0x0A
            // 1 byte: Button Brake is pressed = 0x0A

            int TypeOfMsg = (int)msg[startIndex];

            switch(TypeOfMsg)
            {
                case 0x0A:
                    idPlayer = (int)msg[startIndex + 1];
                    break;
                case 0x0C:
                    idPlayer = (int)msg[startIndex + 1];

                    CDirection.X = (float)ByteHelper.ConvertToInt(msg, startIndex + 2);
                    CDirection.Y = (float)ByteHelper.ConvertToInt(msg, startIndex + 6);
                    CDirection.Z = (float)ByteHelper.ConvertToInt(msg, startIndex + 10);

                    if (msg[startIndex + 14] == 0x0A)
                        Buttons[0] = true; //Fire
                    else
                        Buttons[0] = false;

                    if (msg[startIndex + 15] == 0x0A)
                        Buttons[1] = true; //Gas
                    else
                        Buttons[1] = false;

                    if (msg[startIndex + 16] == 0x0A)
                        Buttons[2] = true; //Brake
                    else
                        Buttons[2] = false;

                    break;
            }

            if (callbackFunc != null)
                callbackFunc((CmdNetwork)TypeOfMsg, idPlayer, CDirection, Buttons);
        }

        #endregion
    }
}
