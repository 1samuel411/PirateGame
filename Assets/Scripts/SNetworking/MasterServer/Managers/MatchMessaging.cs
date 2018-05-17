using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SNetwork.Client
{
    public class MatchMessaging
    {
        private static MatchMessaging _instance;
        public static MatchMessaging instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MatchMessaging();
                }
                return _instance;
            }
        }

        // Header: 200
        public void SendPort(int newPort, int sendCode, int fromCode, int customCode,
            Socket sockets)
        {
            // Send Leave
            byte[] data = ByteParser.ConvertASCIIToBytes(newPort.ToString());
            SendFinal(data, 200, sendCode, fromCode, 0, sockets);
        }

        public void SendFinal(byte[] data, int header, int sendCode, int fromCode, int customCode, Socket socket)
        {
            if (!Network.IsConnected(socket)) return;

            byte headerByte = (byte)header;
            byte sendCodeByte = (byte)sendCode;

            byte[] newData = new byte[data.Length + 5];
            for (int i = 0; i < data.Length; i++)
            {
                newData[i + 5] = data[i];
            }
            newData[0] = headerByte;
            newData[1] = sendCodeByte;
            newData[2] = (byte)0;

            var customCodeByte = BitConverter.GetBytes(data.Length);
            newData[3] = customCodeByte[0];
            newData[4] = customCodeByte[1];

            socket.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }

        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
