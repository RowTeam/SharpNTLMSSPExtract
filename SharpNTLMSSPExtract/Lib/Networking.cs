using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharpNTLMSSPExtract.Lib
{
    public class Networking
    {
        public static byte[] Socket_SendPayload(string target, int port, string type)
        {
            byte[] tmpBuffer = new byte[1024];
            byte[] response = new byte[] { };
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(target, port);

                    switch (type)
                    {
                        case "smb1":
                            socket.Send(NTLMSSPBuffer.smb_buffer_v1_1);
                            socket.Receive(tmpBuffer);
                            socket.Send(NTLMSSPBuffer.smb_buffer_v1_2);
                            break;
                        case "smb2":
                            socket.Send(NTLMSSPBuffer.smb_buffer_v2_1);
                            socket.Receive(tmpBuffer);
                            socket.Send(NTLMSSPBuffer.smb_buffer_v2_2);
                            socket.Receive(tmpBuffer);
                            socket.Send(NTLMSSPBuffer.smb_buffer_v2_3);
                            break;
                        case "wmi0":
                            socket.Send(NTLMSSPBuffer.dcerpc_buffer_v2);
                            break;
                        case "wmi1":
                            socket.Send(NTLMSSPBuffer.dcerpc_buffer_v1);
                            break;
                        case "mssql":
                            socket.Send(NTLMSSPBuffer.mssql_buffer_v1);
                            socket.Receive(tmpBuffer);
                            socket.Send(NTLMSSPBuffer.mssql_buffer_v2);
                            break;
                    }
                    int length = socket.Receive(tmpBuffer);
                    response = tmpBuffer.Take(length).ToArray();
                }
            }
            catch {}

            return response;
        }

        private static bool cert(object o, X509Certificate x, X509Chain c, SslPolicyErrors s) { return true; }

        public static byte[] Web_SendPayload(string target, string type)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(cert);
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.MaxServicePoints = int.MaxValue;

            byte[] response = new byte[] { };

            try
            {
                HttpWebRequest hwr = WebRequest.Create(target) as HttpWebRequest;
                hwr.AllowAutoRedirect = false;
                hwr.Timeout = 15000;
                hwr.Method = "POST";
                hwr.ContentType = "text/xml; charset=utf-8";
                hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";
                hwr.ContentLength = 0;
                hwr.Headers.Add("Authorization", "Negotiate TlRMTVNTUAABAAAAB4IIogAAAAAAAAAAAAAAAAAAAAAGAbEdAAAADw==");
                var tmp = hwr.GetResponse() as HttpWebResponse;
                var challenge = (hwr.GetResponse() as HttpWebResponse).GetResponseHeader("WWW-Authenticate");

                switch (type)
                {
                    case "winrm":
                        response = Convert.FromBase64String(challenge.Split()[1]);
                        break;
                    case "exchange":
                        response = Convert.FromBase64String(challenge.Split(',')[0].Split()[1]);
                        break;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse responseEx = ex.Response as HttpWebResponse;
                try
                {
                    if (responseEx.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var challenge = responseEx.GetResponseHeader("WWW-Authenticate");
                        switch (type)
                        {
                            case "winrm":
                                response = Convert.FromBase64String(challenge.Split()[1]);
                                break;
                            case "exchange":
                                response = Convert.FromBase64String(challenge.Split(',')[0].Split()[1]);
                                break;
                        }
                    }
                }
                catch
                {
                }
            }

            return response;
        }
    }
}