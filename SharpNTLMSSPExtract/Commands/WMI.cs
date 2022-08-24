using System;
using System.Threading;
using SharpNTLMSSPExtract.Domain;
using SharpNTLMSSPExtract.Lib;
using System.Collections.Generic;

namespace SharpNTLMSSPExtract.Commands
{
    public class WMI : ICommand
    {
        public static string CommandName => "wmi";

        private static int ParsingNDR64Syntax(byte[] responseBuffer)
        {
            if (responseBuffer.Length == 0) return 0;
            var NDR64SyntaxStr = BitConverter.ToString(responseBuffer).Replace("-", "");
            return NDR64SyntaxStr.Contains("33057171BABE37498319B5DBEF9CCC36") ? 64 : 86;
        }

        static void StartDoStuff(string target, int port)
        {
            try
            {
                var _SSPKey = new SSPKey();
                _SSPKey.Target = target;

                var response = Networking.Socket_SendPayload(target, port, "wmi0");
                _SSPKey.NDR64Syntax = ParsingNDR64Syntax(response);

                response = Networking.Socket_SendPayload(target, port, "wmi1");
                if (response.Length == 0) return;

                NTLMSSPExtract.ParsingSocketStremResponse(ref response, ref _SSPKey);
                Helpers.WriteLine.ParsingTriageNTLMSSPKey(_SSPKey);
            }
            catch { }
        }
        
        public void Execute(ArgumentParserContent arguments)
        {
            int port = 135;
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
