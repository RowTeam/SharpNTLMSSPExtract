using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharpDetectionNTLMSSP
{
    public class WebRequestAndResponse
    {
        private static bool cert(object o, X509Certificate x, X509Chain c, SslPolicyErrors s) { return true; }
        private static String DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";

        public static HttpWebResponse AuthorizationWebRequesting(string Url)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(cert);
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.MaxServicePoints = int.MaxValue;

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
                return response;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return response;
                }
                return response;
            }
        }
    }
}
