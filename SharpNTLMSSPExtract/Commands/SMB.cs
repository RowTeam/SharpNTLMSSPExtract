using System;
using System.Text;
using System.Threading;
using SharpNTLMSSPExtract.Lib;
using SharpNTLMSSPExtract.Domain;
using System.Collections.Generic;

namespace SharpNTLMSSPExtract.Commands
{
    public class SMB : ICommand
    {
        public static string CommandName => "smb";

        static void StartDoStuff(string target, int port)
        {
            try
            {
                var _SSPKey = new SSPKey();
                _SSPKey.Target = target;
                _SSPKey.Port = port;
                _SSPKey.Type = CommandName;
                string flag = "smb1";
                var response = Networking.Socket_SendPayload(target, port, "smb1");
                if (response.Length == 0)
                {
                    flag = "smb2";
                    response = Networking.Socket_SendPayload(target, port, "smb2");
                }
                if (response.Length == 0) return;

                NTLMSSPExtract.ParsingSocketStremResponse(ref response, ref _SSPKey);

                if (flag.Equals("smb1"))
                {
                    var veraw = Encoding.Default.GetString(response).Split(new String[] { "\0\0\0" }, StringSplitOptions.RemoveEmptyEntries);
                    var tmp = veraw[0].Replace("\0", "");
                    _SSPKey.NativeOs = tmp.Substring(tmp.IndexOf("W"));
                    _SSPKey.NativeLanManager = veraw[1].Replace("\0", "");
                }

                Helpers.WriteLine.ParsingTriageNTLMSSPKey(_SSPKey);
            }
            catch { }
        }

        public void Execute(ArgumentParserContent arguments)
        {
            int port = 445;

            HashSet<string> targetHosts = Wantprefixlen.wantprefixlen(arguments.target);
            ThreadPool.SetMaxThreads(Helpers.Options.SetMaxThreads(arguments, ref port), 1);
            var count = new CountdownEvent(targetHosts.Count);

            foreach (string singleTarget in targetHosts)
            {
                ThreadPool.QueueUserWorkItem(status =>
                {
                    StartDoStuff(singleTarget, port);
                    count.Signal();
                });
            }
            count.Wait();
        }
    }
}
