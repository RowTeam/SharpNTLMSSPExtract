using System;
using System.Threading;
using SharpNTLMSSPExtract.Domain;
using SharpNTLMSSPExtract.Commands;
using SharpNTLMSSPExtract.Lib;
using System.Collections.Generic;

namespace SharpNTLMSSPExtract.Commands
{
    public class Exchange : ICommand
    {
        public static string CommandName => "exchange";

        static void StartDoStuff(string target, int port)
        {
            var _SSPKey = new SSPKey();
            _SSPKey.Target = target;

            target = $"https://{target}:{port}/ews/exchange.asmx";

            var response = Networking.Web_SendPayload(target, CommandName);
            if (response.Length == 0) return;

            NTLMSSPExtract.ParsingSocketStremResponse(ref response, ref _SSPKey);
            Helpers.WriteLine.ParsingTriageNTLMSSPKey(_SSPKey);
        }
        
        public void Execute(ArgumentParserContent arguments)
        {
            int port = 443;
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
