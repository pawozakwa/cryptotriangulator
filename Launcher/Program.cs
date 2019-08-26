using ExchangeProvider;
using System;
using System.Threading;
using System.Diagnostics;

using static Helpers.Helpers;
using static Helpers.SoundsProvider;
using Triangulator.CoreComponents;
using Triangulator.Credits;
using Contracts.Enums;
using ExchangeSharp;

namespace Triangulator
{
    class Program
    {
        static void Main(string[] args)
        {
            SaveStartTimeToReportFile();

            CustomizeConsole();
            Demo.ShowLogo();
                        
            var network = new CurrencyNetwork();
            var api = new ExchangeBinanceAPI();   //apiProvider.GetApi(Exchange.Bleutrade);
            var trader = new Trader(api);            
            var crawler = new NetworkCrawler(api, network, trader);

            var accountBalance = 1.0m;// trader.GetArbitraryAmount().GetAwaiter().GetResult();

            var simulationLenght = 2000000;

            var cycleTimer = new Stopwatch();

            for (int i = 0; i < simulationLenght; i++)
            {
                cycleTimer.Restart();
                Console.WriteLine("+--------------------------------------------------------------------------------------------------+");

                crawler.InitializeNetwork().GetAwaiter().GetResult();
                //accountBalance = crawler.IterativeFindTraceWithProfit(depth, accountBalance);

                Stopwatch stopWatch = new Stopwatch();

                stopWatch.Restart();
                crawler.FindBestChainOfTransactions(accountBalance);
                PrintInColor($"Searching took {stopWatch.Elapsed.TotalSeconds} seconds", ConsoleColor.Cyan);
                crawler.ShowBestFoundedTrace();

                Console.WriteLine();

                var delay = Math.Max((1500 - (int)cycleTimer.ElapsedMilliseconds), 0);
                Console.WriteLine($"Wait for {delay} ms.");
                Thread.Sleep(delay); // Avoid ticker get rejection
            }

            Console.ReadLine();
        }

        private static void SaveStartTimeToReportFile()
        {
            AddToFileOnDesktop(new string[]
            {
                 "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
                 $"Uruchomienie: {DateTime.Now}"
            });
        }

        private static void CustomizeConsole()
        {
            Console.Title = "~Crypto Triangulator~";
        }

        //private Exchange AskUserForExchange()
        //{
        //    PrintInColor("Hello mr. Money Greedy bestard                (;)} ", ConsoleColor.White);
        //    PrintInColor("Please, Enter number of exchange you want to explore:", ConsoleColor.White);

        //var values = Enum.GetValues(typeof(Foos));
        //for (int i = 0; i < values.Length - 1; i++)
        //{
        //    values[i].
        //}
        //foreach (var item in values)
        //{
        //    
        //}

        //    while (true)
        //    {
        //        try
        //        {
        //            crawler.SearchDepth = int.Parse(Console.ReadLine());
        //            break;
        //        }
        //        catch (Exception e) { PrintInColor(e.ToString(), ConsoleColor.Red); }
        //    }
        //}

        private static int AskUserForSearchDepth()
        {
            PrintInColor("How deep you want to search?", ConsoleColor.White);
            while (true)
            {
                try
                { return int.Parse(Console.ReadLine()); }
                catch (Exception e)
                { PrintInColor(e.ToString(), ConsoleColor.Red); }
            }
        }

        private static decimal AskUserForCommisions()
        {
            PrintInColor("Commisions?", ConsoleColor.White);
            while (true)
            {
                try
                { return decimal.Parse(Console.ReadLine()); }
                catch (Exception e)
                { PrintInColor(e.ToString(), ConsoleColor.Red); }
            }
        }
    }
}
