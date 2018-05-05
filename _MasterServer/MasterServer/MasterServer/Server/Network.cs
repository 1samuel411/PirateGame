using System.Net.Sockets;

namespace SNetwork
{
    public class Network
    {
        public static bool IsConnected(Socket socket)
        {
            try
            {
                var connected = !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);

                return connected;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}