using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SharpDetectionNTLMSSP.FunModule;

namespace SharpDetectionNTLMSSP
{
    class Program
    {
        private static int count = 0;
		protected static string Format(string args_0, string args_1, string args_2) => String.Format("{0} {1,-28}: {2}\r\n", args_0, args_1, args_2);
        protected static string Format( string args_1, string args_2) => String.Format("  [>] {0,-18}: {1}\r\n", args_1, args_2);
		
        static void ParsingTriageNTLMSSPKey(TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var result = String.Empty;
            result += Format("[*]", "Detecting Remote Computer of ", _TriageNTLMSSPKey.Target);

            if (String.IsNullOrEmpty(_TriageNTLMSSPKey.NativeOs))
            {
                if (_TriageNTLMSSPKey.NDR64Syntax != 0)                    
                    result += Format("Native OS", $"Windows Version {_TriageNTLMSSPKey.OsMajor}.{_TriageNTLMSSPKey.OsMinor} Build {_TriageNTLMSSPKey.OsBuildNumber} x{_TriageNTLMSSPKey.NDR64Syntax.ToString()}");
                else
                    result += Format("Native OS", $"Windows Version {_TriageNTLMSSPKey.OsMajor}.{_TriageNTLMSSPKey.OsMinor} Build {_TriageNTLMSSPKey.OsBuildNumber}");                    
            }
            else
            {
                result += Format("Native OS", _TriageNTLMSSPKey.NativeOs);
            }
            result += Format("DNS domain name", _TriageNTLMSSPKey.DnsDomainName);
            result += Format("DNS computer name", _TriageNTLMSSPKey.DnsComputerName);
            result += Format("Time stamp", _TriageNTLMSSPKey.TimeStamp.ToString("yyyy-MM-dd HH-mm-ss ddd"));

            count += 1;
            Console.WriteLine(result);
        }

        static void StartDoStuff(string target, int port, string typeKey)
        {
            var socketMessage = new SocketStream(target, port);
            if (!socketMessage.OK) return;

            var _TriageNTLMSSPKey = new TriageNTLMSSPKey();
            _TriageNTLMSSPKey.Target = target;
            _TriageNTLMSSPKey.Port = port;
            _TriageNTLMSSPKey.Type = typeKey;

            ModuleScan _ModuleScan = null;
            var type = Type.GetType("SharpDetectionNTLMSSP.FunModule." + typeKey);
            if (type != null)
            {
                _ModuleScan = (ModuleScan)Activator.CreateInstance(type);
            }
            _TriageNTLMSSPKey = _ModuleScan.StartScan(socketMessage, _TriageNTLMSSPKey);
            if (_TriageNTLMSSPKey == null) return;
            ParsingTriageNTLMSSPKey(_TriageNTLMSSPKey);
        }

        static void StartThreadPool(Dictionary<string, string> arguments, int maxThreads)
        {
            HashSet<string> targetHosts = Wantprefixlen.wantprefixlen(arguments["-target"]);
            ThreadPool.SetMaxThreads(maxThreads, 1);
            var count = new CountdownEvent(targetHosts.Count);

            foreach (string singleTarget in targetHosts)
            {
                ThreadPool.QueueUserWorkItem(status => 
                { 
                    StartDoStuff(singleTarget, Convert.ToInt32(arguments["-port"]), arguments["-module"].ToUpper()); 
                    count.Signal(); 
                });
            }
            count.Wait();
            Console.WriteLine("---------------Script execution completed---------------");
        }

        static void ParseArguments(Dictionary<string, string> arguments)
        {
            int maxThreads = 15;
            int workers, async;

            if (arguments.ContainsKey("-threads"))
            {
                ThreadPool.GetAvailableThreads(out workers, out async);
                if (Convert.ToInt32(arguments["-threads"]) <= workers)
                {
                    maxThreads = Convert.ToInt32(arguments["-threads"]);
                }
                else
                {
                    Console.WriteLine("[!] Error - not enough available worker threads in the .net thread pool (max available = " + workers + ")");
                    Environment.Exit(0);
                }
            }

            if (arguments.ContainsKey("-port") && arguments.ContainsKey("-module") && arguments.ContainsKey("-target"))
            {
                StartThreadPool(arguments, maxThreads);
            }
            else
            {
                Console.WriteLine("[!] Error - run with '-help' flag to see additional flags");
                Environment.Exit(0);
            }
        }

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (args.Length <= 0 || args[0].ToLower() == "help" || args[0] == "-h" || args[0].ToLower() == "-help")
            {
                Info.ShowUsage();
                return;
            }

            var parsed = ArgumentParser.Parse(args);
            if (parsed.ParsedOk == false)
            {
                Info.ShowUsage();
                return;
            }
            ParseArguments(parsed.Arguments);

            stopwatch.Stop();
            TimeSpan timespan = stopwatch.Elapsed;
            
            Console.WriteLine("[*] Count: {0}, Time taken: {1}s", count, timespan.TotalSeconds);
        }
    }
}
