using System;
using System.Reflection;
using System.Diagnostics;
using SharpNTLMSSPExtract.Domain;
using SharpNTLMSSPExtract.Commands;
using System.Collections.Generic;
using SharpNTLMSSPExtract.Helpers;

namespace SharpNTLMSSPExtract
{
    class Program
    {
        static string FileName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// 添加新方法
        /// </summary>
        private static Dictionary<string, Func<ICommand>> AddDictionary()
        {
            Dictionary<string, Func<ICommand>> _availableCommands = new Dictionary<string, Func<ICommand>>();
            _availableCommands.Add(SMB.CommandName, () => new SMB());
            _availableCommands.Add(WMI.CommandName, () => new WMI());
            _availableCommands.Add(WinRM.CommandName, () => new WinRM());
            _availableCommands.Add(MSSQL.CommandName, () => new MSSQL());
            _availableCommands.Add(Exchange.CommandName, () => new Exchange());
            _availableCommands.Add(RDP.CommandName, () => new RDP());

            return _availableCommands;
        }

        /// <summary>
        /// 執行方法
        /// </summary>
        private static void MainExecute(string commandName, ArgumentParserContent parsedArgs)
        {
            Helpers.Info.ShowLogo();

            try
            {
                var commandFound = new CommandCollection().ExecuteCommand(commandName, parsedArgs, AddDictionary());

                Console.WriteLine();
                // 如果未找到方法，則輸出使用方法
                if (commandFound == false)
                    Helpers.Info.ShowUsage();
            }
            catch (Exception e)
            {
                Console.WriteLine($"\r\n[!] Unhandled {FileName} exception:\r\n");
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            // 尝试解析命令行参数
            var parsed = ArgumentParser.Parse(args);
            if (parsed.ParsedOk == false)
            {
                Helpers.Info.ShowLogo();
                Helpers.Info.ShowUsage();
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var commandName = args.Length != 0 ? args[0] : "";
            MainExecute(commandName, parsed.Arguments);

            stopwatch.Stop();
            TimeSpan timespan = stopwatch.Elapsed;
            Writer.Info($"Count: {Helpers.WriteLine.count} Time taken: {timespan.TotalSeconds}s");
        }
    }
}
