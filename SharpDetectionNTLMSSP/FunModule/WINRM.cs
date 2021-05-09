using System;
using System.Net;

namespace SharpDetectionNTLMSSP.FunModule
{
    class WINRM : ModuleScan
    {
        private static String DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";

        public override TriageNTLMSSPKey StartScan(SocketStream socketMessage, TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var response = new byte[1024];
            response = WebRequesting(_TriageNTLMSSPKey.IP, _TriageNTLMSSPKey.Port);
            _TriageNTLMSSPKey = ParsingResponse.ParsingSocketStremResponse(response, _TriageNTLMSSPKey, ref response);

            return _TriageNTLMSSPKey;
        }

        public byte[] WebRequesting(String ip, int port)
        {
            string Url = $@"http://{ip}:{port}/wsman";
            try
            {
                HttpWebRequest hwr = WebRequest.Create(Url) as HttpWebRequest;
                hwr.AllowAutoRedirect = false;
                hwr.Timeout = 15000;
                hwr.Method = "POST";
                hwr.ContentType = "text/xml; charset=utf-8";
                hwr.UserAgent = DefaultUserAgent;
                hwr.Headers.Add("Authorization", "Negotiate TlRMTVNTUAABAAAAB4IIogAAAAAAAAAAAAAAAAAAAAAGAbEdAAAADw==");
                HttpWebResponse response = hwr.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var challenge = response.GetResponseHeader("WWW-Authenticate");
                    return Convert.FromBase64String(challenge.Split()[1]);
                }
            }
            return new byte[]{ 0x00};
        }
    }
}
