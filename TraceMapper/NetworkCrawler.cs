using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using ExchangeProvider;
using ExchangeSharp;

namespace TraceMapper
{
    public class NetworkCrawler
    {
        private CurrencyNetwork _currencyNetwork;
        private ExchangeAPI[] _exchangeApis;

        public NetworkCrawler(ExchangeAPI[] exchangeApis)
        {
            _currencyNetwork = new CurrencyNetwork();
            _exchangeApis = exchangeApis;
        }

        public async Task InitializeNetwork()
        {
            foreach (var exchangeApi in _exchangeApis)
            { 
                var stopWatch = new Stopwatch();

                Console.Write("Downloading actual tickers...");
                stopWatch.Start();
                var tickersFromExchange = await exchangeApi.GetTickersAsync();
                stopWatch.Stop();
                Console.WriteLine($"   <= Done! ({Stopwatch.GetTimestamp()})");
                

                Console.Write("Feeding actual tickers...");
                stopWatch.Restart();
                foreach (var tickerKV in tickersFromExchange)
                {
                    _currencyNetwork.AddEdge(tickerKV.Key, tickerKV.Value, exchangeApi);
                }
                Console.WriteLine($"   <= Done! ({Stopwatch.GetTimestamp()})");
            }
        } 

        public void FindTraceWithProfit()
        {
            throw new NotImplementedException();
        }
    }
}
