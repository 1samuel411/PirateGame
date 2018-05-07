using System.Net.Sockets;

namespace SNetwork
{
    public class Network
    {
        public static bool IsConnected(Socket socket)
        {
            try
            {
                if (socket == null)
                {
                    return false;
                }
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