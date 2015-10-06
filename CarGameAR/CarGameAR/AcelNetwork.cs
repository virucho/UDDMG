using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Referencias Sockets
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace AR_CarServer
{
    class AcelNetwork
    {
        public enum CmdNetwork
        {
            ADD_PLY = 0x0A,
            RM_PLY = 0x0B,
            DATA_ACEL = 0x0C
        }

        public static byte[] BuildMsg(string IdNetObj, byte[] Datos)
        {
            byte[] Outmsg = new byte[Datos.Length + IdNetObj.Length + 3];
            byte[] id = HelpConv.ConvertToByte(IdNetObj);

            Outmsg[0] = Convert.ToByte(Outmsg.Length - 2);
            Outmsg[1] = 0x00;
            Outmsg[2] = Convert.ToByte(id.Length);
            id.CopyTo(Outmsg, 3);
            Datos.CopyTo(Outmsg, 3 + id.Length);

            return Outmsg;
        }

        public static void SendCmdCar(Socket Scksender, int AcelX, int AcelY, int AcelZ, byte[] Buttons, byte IdPlayer)
        {
            // Encode the data string into a byte array.
            byte[] msg = new byte[1 + 1 + 12 + 3];
            byte[] OutMsg = null;

            int bytesSent = 0;

            msg[0] = (byte)CmdNetwork.DATA_ACEL;
            msg[1] = IdPlayer;
            HelpConv.FillByteArray(ref msg, 2, BitConverter.GetBytes(AcelX));
            HelpConv.FillByteArray(ref msg, 6, BitConverter.GetBytes(AcelY));
            HelpConv.FillByteArray(ref msg, 10, BitConverter.GetBytes(AcelZ));
            msg[14] = Buttons[0];
            msg[15] = Buttons[1];
            msg[16] = Buttons[2];

            OutMsg = BuildMsg("ControlCarNetworkObject", msg);

            //Envio longitud
            bytesSent = Scksender.Send(BitConverter.GetBytes(OutMsg.Length));

            //Hago pausa
            System.Threading.Thread.Sleep(100);

            // Send the data through the socket.
            bytesSent = Scksender.Send(OutMsg);
        }

        public static void SendNewPlayer(Socket Scksender, int IdPlayer)
        {
            // Encode the data string into a byte array.
            byte[] msg = new byte[1 + 1];
            byte[] OutMsg = null;

            int bytesSent = 0;

            msg[0] = (byte)CmdNetwork.ADD_PLY;
            msg[1] = (byte)IdPlayer;

            OutMsg = BuildMsg("ControlCarNetworkObject", msg);

            //Envio longitud
            bytesSent = Scksender.Send(BitConverter.GetBytes(OutMsg.Length));

            //Hago pausa
            System.Threading.Thread.Sleep(100);

            // Send the data 
            bytesSent = Scksender.Send(OutMsg);
        }

        public static int AddClientList(string ipClient, ref string[] Lista)
        {
            int x = 0;
            for(x = 0; x < Lista.Length; x++)
            {
                if (Lista[x] == null)
                {
                    Lista[x] = ipClient;
                    return x + 1;
                }
            }
            return -1;
        }

        public static bool RmClientList(string ipClient, ref string[] Lista)
        {
            int x = 0;
            for (x = 0; x < Lista.Length; x++)
            {
                if (Lista[x] == ipClient)
                {
                    Lista[x] = null;
                    return true;
                }
            }
            return false;
        }

        public static int SchClientList(string ipClient, ref string[] Lista)
        {
            int x = 0;
            for (x = 0; x < Lista.Length; x++)
            {
                if (Lista[x] == ipClient)
                    return x;
            }
            return -1;
        }
    }

    public class HelpConv
    {
        public static void FillByteArray(ref byte[] dest, int destStartIndex, byte[] src)
        {
            int length = (src.Length > (dest.Length - destStartIndex)) ?
                (dest.Length - destStartIndex) : src.Length;
            Buffer.BlockCopy(src, 0, dest, destStartIndex, length);
        }

        public static byte[] ConvertToByte(String s)
        {
            return System.Text.Encoding.ASCII.GetBytes(s);
        }
    }
}
