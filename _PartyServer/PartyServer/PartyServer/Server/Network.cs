using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace SNetwork
{
	public class Network
	{

        public static bool IsConnected(Socket socket)
        {
            try
            {
                bool connected = !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);

                return connected;
            }
            catch (SocketException) { return false; }
        }
    }
}
