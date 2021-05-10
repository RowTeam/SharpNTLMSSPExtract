using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDetectionNTLMSSP
{
    public static class ArgumentParser
    {
        public static ArgumentParserResult Parse(IEnumerable<string> args)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var arguments = new Dictionary<string, string>(comparer);
            try
            {
                foreach (var argument in args)
                {
                    var idx = argument.IndexOf('=');
                    if (idx > 0)
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                    else
                        arguments[argument] = string.Empty;
                }

                return ArgumentParserResult.Success(arguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ArgumentParserResult.Failure();
            }
        }
    }
}
