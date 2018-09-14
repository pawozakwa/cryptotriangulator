using ExchangeProvider;
using TraceMapper;
using ExchangeSharp;
using System;

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

            for (int i = 0; i < 10; i++)
            {
                crawler.InitializeNetwork().GetAwaiter().GetResult();

                accountBalance = crawler.IterativeFindTraceWithProfit(3, accountBalance);
                crawler.ShowBestFoundedTrace();
            }

            Console.ReadLine();
        }
    }
}
