using ExchangeSharp;
using System;
using System.Diagnostics;
using System.Threading;
using Triangulator.CoreComponents;
using Triangulator.Credits;
using static Helpers.Helpers;

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
            FixBinanceApi(ref api);


            var trader = new Trader(api);
            var crawler = new NetworkCrawler(api, network, trader);

            var accountBalance = 1.0m;// trader.GetArbitraryAmount().GetAwaiter().GetResult();

            var simulationLenght = 2000000;

            MainTriangulatorLoop(crawler, accountBalance, simulationLenght);

            Console.ReadLine();
        }

        private static void MainTriangulatorLoop(NetworkCrawler crawler, decimal accountBalance, int simulationLenght)
        {
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

        private static void FixBinanceApi(ref ExchangeBinanceAPI api)
        {
            api.RequestWindow = TimeSpan.FromSeconds(59);
        }
    }
}
