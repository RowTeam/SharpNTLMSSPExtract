using System;
using System.Text;

namespace SharpDetectionNTLMSSP.FunModule
{
    class SMB : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var response = new byte[1024];
            socketMessage.SendMessage(NTLMSSPBuffer.smb_buffer_v1);
            response = socketMessage.ReceiveMessage();
            socketMessage.SendMessage(NTLMSSPBuffer.smb_buffer_v2);
            response = socketMessage.ReceiveMessage();
            
            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(response, _TriageNTLMSSPKey, ref response);

            var veraw = Encoding.Default.GetString(response).Split(new String[] { "\0\0\0" }, StringSplitOptions.RemoveEmptyEntries);
            _TriageNTLMSSPKey.NativeOs = veraw[0].Replace("\0", "");
            _TriageNTLMSSPKey.NativeLanManager = veraw[1].Replace("\0", "");

            return _TriageNTLMSSPKey;
        }
    }
}
