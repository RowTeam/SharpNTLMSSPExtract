using System;
using System.Net.Sockets;

namespace SharpDetectionNTLMSSP.FunModule
{
    class WMI : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var response = NDR64SyntaxtScan(_TriageNTLMSSPKey);
            _TriageNTLMSSPKey.NDR64Syntax = ParsingNDR64Syntax(response);

            socketMessage.SendMessage(NTLMSSPBuffer.dcerpc_buffer_v1);
            response = socketMessage.ReceiveMessage();
            if (response.Length == 0) return null;
            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(response, _TriageNTLMSSPKey, ref response);
            return _TriageNTLMSSPKey;
        }

        public byte[] NDR64SyntaxtScan(TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var response = new byte[1024];

            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(_TriageNTLMSSPKey.Target, _TriageNTLMSSPKey.Port);
                socket.Send(NTLMSSPBuffer.dcerpc_buffer_v2);
                socket.Receive(response);
            }
            catch
            {
                return new byte[] { };
            }
            return response;
        }

        private int ParsingNDR64Syntax(Byte[] responseBuffer)
        {
            if (responseBuffer.Length == 0) return 0;
            var NDR64SyntaxStr = BitConverter.ToString(responseBuffer).Replace("-", "");
            return NDR64SyntaxStr.Contains("33057171BABE37498319B5DBEF9CCC36") ? 64 : 86;
        }
    }
}
