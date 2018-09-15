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

            ExchangeAPI[] exchangeAPIs = new[] {
                apiProvider.GetApi(Exchange.Poloniex)
            };

            var crawler = new NetworkCrawler(exchangeAPIs);
            var accountBalance = 1m;

            crawler.InitializeNetwork().GetAwaiter().GetResult();

            PrintInColor("How deep you want to search?", ConsoleColor.White);
            while (true)
            {
                try {
                    crawler.SearchDepth = int.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception e) { PrintInColor(e.ToString(), ConsoleColor.Red); }
            }

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

            accountBalance = crawler.IterativeFindTraceWithProfit(12, accountBalance);
            crawler.ShowBestFoundedTrace();
            
            Console.ReadLine();
        }
    }
}
