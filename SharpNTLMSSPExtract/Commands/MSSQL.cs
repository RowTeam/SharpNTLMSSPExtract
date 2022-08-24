using System;
using System.Threading;
using SharpNTLMSSPExtract.Domain;
using SharpNTLMSSPExtract.Commands;
using SharpNTLMSSPExtract.Lib;
using System.Collections.Generic;

namespace SharpNTLMSSPExtract.Commands
{
    public class MSSQL : ICommand
    {
        public static string CommandName => "mssql";

        static void StartDoStuff(string target, int port)
        {
            var _SSPKey = new SSPKey();
            _SSPKey.Target = target;

            var response = Networking.Socket_SendPayload(target, port, CommandName);
            if (response.Length == 0) return;

            NTLMSSPExtract.ParsingSocketStremResponse(ref response, ref _SSPKey);
            Helpers.WriteLine.ParsingTriageNTLMSSPKey(_SSPKey);
        }
        
        public void Execute(ArgumentParserContent arguments)
        {
            int port = 1433;
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
