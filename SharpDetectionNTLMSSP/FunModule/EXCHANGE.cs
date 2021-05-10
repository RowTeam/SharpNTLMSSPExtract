using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharpDetectionNTLMSSP.FunModule
{
    class EXCHANGE : ModuleScan
    {
        static bool cert(object o, X509Certificate x, X509Chain c, SslPolicyErrors s) { return true; }
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
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(cert);
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.MaxServicePoints = int.MaxValue;

            string Url = $@"https://{ip}:{port}/ews/exchange.asmx";
            try
            {
                HttpWebRequest hwr = WebRequest.Create(Url) as HttpWebRequest;
                hwr.AllowAutoRedirect = false;
                hwr.Timeout = 15000;
                hwr.Method = "POST";
                hwr.ContentType = "text/xml; charset=utf-8";
                hwr.UserAgent = DefaultUserAgent;
                hwr.ContentLength = 0;
                hwr.Headers.Add("Authorization", "Negotiate TlRMTVNTUAABAAAAB4IIogAAAAAAAAAAAAAAAAAAAAAGAbEdAAAADw==");
                HttpWebResponse response = hwr.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var challenge = response.GetResponseHeader("WWW-Authenticate");

                    return Convert.FromBase64String(challenge.Split(',')[0].Split()[1]);
                }
            }
            return new byte[] { 0x00 };
        }
    }
}
