using System;
using SharpNTLMSSPExtract.Lib;

namespace SharpNTLMSSPExtract.Helpers
{
    public class WriteLine
    {
        public static int count = 0;
        protected static string Format(string args_0, string args_1, string args_2) => String.Format("{0} {1,-28}: {2}\r\n", args_0, args_1, args_2);
        protected static string Format(string args_1, string args_2) => String.Format("  [>] {0,-18}: {1}\r\n", args_1, args_2);

        public static void ParsingTriageNTLMSSPKey(SSPKey _SSPKey)
        {
            count += 1;
            var result = String.Empty;
            result += Format($"[{count}]", "Detecting Remote Computer of ", _SSPKey.Target);

            if (String.IsNullOrEmpty(_SSPKey.NativeOs))
            {
                if (_SSPKey.NDR64Syntax != 0)
                    result += Format("Native OS", $"Windows Version {_SSPKey.OsMajor}.{_SSPKey.OsMinor} Build {_SSPKey.OsBuildNumber} x{_SSPKey.NDR64Syntax}");
                else
                    result += Format("Native OS", $"Windows Version {_SSPKey.OsMajor}.{_SSPKey.OsMinor} Build {_SSPKey.OsBuildNumber}");
            }
            else
            {
                result += Format("Native OS", _SSPKey.NativeOs);
            }
            if (string.IsNullOrEmpty(_SSPKey.DnsDomainName))
            {
                result += Format("DNS domain name", $"{_SSPKey.DnsComputerName} -> {_SSPKey.NbtDomainName}");
            }
            else if (_SSPKey.DnsDomainName.ToLower() != _SSPKey.NbtDomainName.ToLower())
                result += Format("DNS domain name", $"{_SSPKey.DnsDomainName} -> {_SSPKey.NbtDomainName}");
            else
                result += Format("DNS domain name", _SSPKey.DnsDomainName);

            result += Format("Computer name", _SSPKey.NbtComputerName);
            result += Format("Time stamp", _SSPKey.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss ddd"));

            Writer.Line(result);
        }
    }
}
