using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDetectionNTLMSSP
{
    public class TriageNTLMSSPKey
    {
        public String Target { get; set; }
        public Int32 Port { get; set; }
        public String Type { get; set; }

        public Int32 NDR64Syntax { get; set; }

        public Int16 OsBuildNumber { get; set; }
        public Byte OsMajor { get; set; }
        public Byte OsMinor { get; set; }

        public String NbtComputerName { get; set; }
        public String NbtDomainName { get; set; }
        public String DnsComputerName { get; set; }
        public String DnsDomainName { get; set; }
        public DateTime TimeStamp { get; set; }


        public String NativeOs { get; set; }
        public String NativeLanManager { get; set; }
    }
}
