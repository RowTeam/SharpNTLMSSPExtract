using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharpDetectionNTLMSSP.FunModule
{
    class EXCHANGE : ModuleScan
    {
        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            string Url = $@"https://{_TriageNTLMSSPKey.Target}:{_TriageNTLMSSPKey.Port}/ews/exchange.asmx";
            HttpWebResponse response = WebRequestAndResponse.AuthorizationWebRequesting(Url);
            var challenge = response.GetResponseHeader("WWW-Authenticate");

            var responseByte = Convert.FromBase64String(challenge.Split(',')[0].Split()[1]);
            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(responseByte, _TriageNTLMSSPKey, ref responseByte);

            return _TriageNTLMSSPKey;
        }
    }
}
