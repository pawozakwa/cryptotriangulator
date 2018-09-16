using ExchangeProvider;
using TraceMapper;
using ExchangeSharp;
using System;
using static Helpers.Helpers;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Demo.ShowLogo();

            var apiProvider = new ExchangeApiProvider();            
            var crawler = new NetworkCrawler(apiProvider.GetApi(Exchange.Poloniex));
            var accountBalance = 1m;

            crawler.InitializeNetwork().GetAwaiter().GetResult();

            AskUserForSearchDepth(crawler);
            AskUserForCommisions(crawler);

            accountBalance = crawler.IterativeFindTraceWithProfit(12, accountBalance);
            crawler.ShowBestFoundedTrace();

            Console.ReadLine();
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

        private static void AskUserForSearchDepth(NetworkCrawler crawler)
        {
            PrintInColor("How deep you want to search?", ConsoleColor.White);
            while (true)
            {
                try
                {
                    crawler.SearchDepth = int.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception e) { PrintInColor(e.ToString(), ConsoleColor.Red); }
            }
        }

        private static void AskUserForCommisions(NetworkCrawler crawler)
        {
            PrintInColor("Commisions?", ConsoleColor.White);
            while (true)
            {
                try
                {
                    crawler.Commision = decimal.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception e) { PrintInColor(e.ToString(), ConsoleColor.Red); }
            }
        }
    }
}
