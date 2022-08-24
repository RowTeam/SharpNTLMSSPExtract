using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNTLMSSPExtract.Domain
{
    public class ArgumentParserContent
    {
        public string target { get; }
        public string threads { get; }
        public string port { get; }

        public ArgumentParserContent(Dictionary<string, string> arguments)
        {

            target = ArgumentParser(arguments, "/target");
            threads = ArgumentParser(arguments, "/threads");
            port = ArgumentParser(arguments, "/port");

        }

        private string ArgumentParser(Dictionary<string, string> arguments, string flag)
        {
            if (arguments.ContainsKey(flag) && !string.IsNullOrEmpty(arguments[flag]))
            {
                return arguments[flag];
            }
            return null;
        }
    }
}
