using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

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
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Socket Error, during sending: {ex.Message}");
            }
        }

        public byte[] ReceiveMessage()
        {
            byte[] response = new byte[1024];
            try
            {
                socket.Receive(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Socket Error, during receive: {ex.Message}");
            }
            return response;
        }
    }
}
