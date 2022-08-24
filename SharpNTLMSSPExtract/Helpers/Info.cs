using System;

namespace SharpNTLMSSPExtract.Helpers
{
    public static class Info
    {
        public static void ShowLogo()
        {
            Console.WriteLine($@"
   _____ _                      _   _ _______ _      __  __  _____ _____ _____  ______      _                  _   
  / ____| |                    | \ | |__   __| |    |  \/  |/ ____/ ____|  __ \|  ____|    | |                | |  
 | (___ | |__   __ _ _ __ _ __ |  \| |  | |  | |    | \  / | (___| (___ | |__) | |__  __  _| |_ _ __ __ _  ___| |_ 
  \___ \| '_ \ / _` | '__| '_ \| . ` |  | |  | |    | |\/| |\___ \\___ \|  ___/|  __| \ \/ / __| '__/ _` |/ __| __|
  ____) | | | | (_| | |  | |_) | |\  |  | |  | |____| |  | |____) |___) | |    | |____ >  <| |_| | | (_| | (__| |_ 
 |_____/|_| |_|\__,_|_|  | .__/|_| \_|  |_|  |______|_|  |_|_____/_____/|_|    |______/_/\_\\__|_|  \__,_|\___|\__|
                         | |                                                                                       
                         |_|   by:RowTeam          Current Time: {DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}
");
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowUsage()
        {
            string FileName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            string usage = $@"
### Command Line Usage ###

    {FileName} smb/wmi/winrm/mssql/exchange/rdp

SharpNTLMSSPExtract.exe smb /target:10.0.100.233 /port:445 /threads:15
";
            Console.WriteLine(usage);
            Environment.Exit(0);
        }
    }
}
