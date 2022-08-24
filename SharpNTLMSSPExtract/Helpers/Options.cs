using System;
using System.Threading;
using SharpNTLMSSPExtract.Domain;

namespace SharpNTLMSSPExtract.Helpers
{
    public class Options
    {
        public static int SetMaxThreads(ArgumentParserContent arguments, ref int port)
        {
            int maxThreads = 15;
            int workers, async;

            if (!string.IsNullOrEmpty(arguments.port))
                port = Convert.ToInt32(arguments.port);

            if (!string.IsNullOrEmpty(arguments.threads))
            {
                ThreadPool.GetAvailableThreads(out workers, out async);
                if (Convert.ToInt32(arguments.threads) <= workers)
                    maxThreads = Convert.ToInt32(arguments.threads);
                else
                    Writer.Error($"not enough available worker threads in the .net thread pool (max available = {workers} )");
            }
            return maxThreads;
        }

        public static int ReadInt2(byte[] src, int srcIndex)
        {
            return unchecked(src[srcIndex] & 0xFF) + ((src[srcIndex + 1] & 0xFF) << 8);
        }
    }
}
