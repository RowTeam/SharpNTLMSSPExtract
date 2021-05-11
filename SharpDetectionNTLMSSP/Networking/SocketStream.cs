using System;
using System.Net.Sockets;
using System.Threading;

namespace SharpDetectionNTLMSSP
{
    class SocketStream
    {
        public Boolean OK = false;
        public Socket socket = null;

        public SocketStream(String ip, int port)
        {
            try
            {
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip, port);
                OK = true;
            }
            catch
            {
                OK = false;
                return;
            }
        }

        public void SendMessage(Byte[] buffer)
        {
            try
            {
                socket.Send(buffer);
            }
            catch
            {
                return;
            }
        }

        public byte[] ReceiveMessage()
        {
            byte[] response = new byte[1024];
            try
            {
                socket.Receive(response);
            }
            catch
            {
                return new byte[] { };
            }

            return response;
        }
    }
}
