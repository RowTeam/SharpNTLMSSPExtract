using System;
using System.Net;

namespace SharpDetectionNTLMSSP.FunModule
{
    class WINRM : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            string Url = $@"http://{_TriageNTLMSSPKey.Target}:{_TriageNTLMSSPKey.Port}/wsman";
            HttpWebResponse response = WebRequestAndResponse.AuthorizationWebRequesting(Url);
            var challenge = response.GetResponseHeader("WWW-Authenticate");

            var responseByte = Convert.FromBase64String(challenge.Split()[1]);
            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(responseByte, _TriageNTLMSSPKey, ref responseByte);

            return _TriageNTLMSSPKey;
        }
    }
}
