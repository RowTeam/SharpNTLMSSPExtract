using SharpRDPCheck;
using System.Threading;
using SharpNTLMSSPExtract.Lib;
using SharpNTLMSSPExtract.Domain;
using System.Collections.Generic;

namespace SharpNTLMSSPExtract.Commands
{
    public class RDP : ICommand
    {
        public static string CommandName => "rdp";

        static void StartDoStuff(string target, int port)
        {
            var _SSPKey = new SSPKey();
            _SSPKey.Target = target;
            _SSPKey.Port = port;
            _SSPKey.Type = CommandName;

            Options.Host = target;
            Options.Port = port;
            if (Network.Connect(Options.Host, Options.Port))
            {
                var response = MCS.RDPNTLMSSPNegotiate(null, false);
                if (response.Length == 0) return;

                NTLMSSPExtract.ParsingSocketStremResponse(ref response, ref _SSPKey);
                Helpers.WriteLine.ParsingTriageNTLMSSPKey(_SSPKey);
            }
            Network.DisConnect();
        }

        public void Execute(ArgumentParserContent arguments)
        {
            int port = 3389;
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
