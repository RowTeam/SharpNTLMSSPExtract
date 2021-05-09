using SharpDetectionNTLMSSP.FunModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

using static SharpDetectionNTLMSSP.FormatUtils;

namespace SharpDetectionNTLMSSP
{
    class Program
    {
        static void ParsingTriageNTLMSSPKey(TriageNTLMSSPKey _TriageNTLMSSPKey)
        {
            var result = String.Empty;

            result += String.Format("[*] Detecting Remote Computer of {0}\r\n", _TriageNTLMSSPKey.IP);

            if (String.IsNullOrEmpty(_TriageNTLMSSPKey.NativeOs))
            {
                if(_TriageNTLMSSPKey.NDR64Syntax != 0)
                    result += String.Format("  [>] Native OS            : Windows Version {0}.{1} Build {2} x{3}\r\n",  _TriageNTLMSSPKey.OsMajor, _TriageNTLMSSPKey.OsMinor,  _TriageNTLMSSPKey.OsBuildNumber, _TriageNTLMSSPKey.NDR64Syntax.ToString());
                else
                    result += String.Format("  [>] Native OS            : Windows Version {0}.{1} Build {2}\r\n", _TriageNTLMSSPKey.OsMajor, _TriageNTLMSSPKey.OsMinor, _TriageNTLMSSPKey.OsBuildNumber);
            }
            else
            {
                result += String.Format("  [>] Native OS            : {0}\r\n", _TriageNTLMSSPKey.NativeOs);
            }
            result += String.Format("  [>] NetBIOS computer name: {0}\r\n", _TriageNTLMSSPKey.NbtComputerName);
            result += String.Format("  [>] DNS computer name    : {0}\r\n", _TriageNTLMSSPKey.DnsComputerName);
            result += String.Format("  [>] Time stamp           : {0}\r\n", _TriageNTLMSSPKey.TimeStamp.ToString("yyyy-MM-dd HH-mm-ss ddd"));

            Console.WriteLine(result);
        }

        static void Start(string ip, int port, string typeKey)
        {
            var socketMessage = new SocketStream(ip, port);
            var _TriageNTLMSSPKey = new TriageNTLMSSPKey();
            _TriageNTLMSSPKey.IP = ip;
            _TriageNTLMSSPKey.Port = port;
            _TriageNTLMSSPKey.Type = typeKey;

            ModuleScan _ModuleScan = null;
            var type = Type.GetType("SharpDetectionNTLMSSP.FunModule." + typeKey);
            if (type != null)
            {
                _ModuleScan = (ModuleScan)Activator.CreateInstance(type);
            }
            _TriageNTLMSSPKey = _ModuleScan.StartScan(socketMessage, _TriageNTLMSSPKey);
            ParsingTriageNTLMSSPKey(_TriageNTLMSSPKey);
        }

        static void help()
        {
            Console.WriteLine("SharpDetectionNTLMSSP.exe -m Moudle -h ip -p port");
            Console.WriteLine("Moudel:");
            Console.WriteLine("\tExchagne");
            Console.WriteLine("\tMSSql");
            Console.WriteLine("\tSMB");
            Console.WriteLine("\tWinRM");
            Console.WriteLine("\tWMI");
        }

        static void Main(string[] args)
        {
            var ip = string.Empty;
            var port = 445;
            var typeKey = string.Empty;

            if (args.Length < 6)
            {
                help();
                Environment.Exit(0);
            }
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("-m") && args.Length > i + 1)
                {
                    typeKey = args[++i].ToUpper();
                }
                else if (args[i].Contains("-h") && args.Length > i + 1)
                {
                    ip = args[++i];
                }
                else if (args[i].Contains("-p") && args.Length > i + 1)
                {
                    port = int.Parse(args[++i]);
                }
            }
            Start(ip, port, typeKey);
        }
    }
}
