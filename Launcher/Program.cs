using ExchangeProvider;
using TraceMapper;
using ExchangeSharp;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiProvider = new ExchangeApiProvider();

            ExchangeAPI[] exchangeAPIs = new[] {
                apiProvider.GetApi(Exchange.Bitfinex)
            };

            var crawler = new NetworkCrawler(exchangeAPIs);

            crawler.InitializeNetwork().GetAwaiter().GetResult();


            System.Console.ReadLine();
        }
    }
}
