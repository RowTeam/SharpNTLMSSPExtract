using System;

namespace SharpDetectionNTLMSSP
{
    class Info
    {
        public static void ShowUsage()
        {
            string usage = @"
SharpDetectionNTLMSSP.exe -module=smb -target=192.168.65.133 -port=445 -threads=15

Required Flags:
-target: The IP address of the target. the following formats are supported
	        192.168.65.133
            192.168.65.133,192.168.65.123
            192.168.65.0/24
            192.168.65.0/16
            192.168.65.0/8
            192.168.65.55-192.168.70.233
            target.txt
-module: The service module of the target. the following modules are supported
	        exchange
            mssql
            smb
            winrm
            wmi
-port: The corresponding port of the target's service module

Optional Flags:
-threads: Threads to use to concurently enumerate multiple remote hosts (Default: 15)
";
            Console.WriteLine(usage);
        }
    }
}
