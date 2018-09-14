using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExchangeSharp;
using Contracts;
using System.Collections.Generic;
using static Helpers.Helpers;

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
                    try
                    {
                        _currencyNetwork.AddEdge(tickerKV.Key, tickerKV.Value, exchangeApi);
                    }
                    catch (Exception e )
                    {
                        Console.WriteLine(e);
                    }
                }
                Console.WriteLine($"   <= Done! ({Stopwatch.GetTimestamp()})");
            }
        }

        #region Iterative aproach

        public void IterativeFindTraceWithProfit(int searchDepth = 3, decimal arbitraryCurrencyAmount = 1m)
        {
            Vertice enterVertice =
                _currencyNetwork.VerticesDictionary[Constats.ArbitraryCurrency];

            _bestTrace = null;

            _bestTraceProfit = 0;

            foreach (var e in enterVertice.Edges)
            {
                var singleList = new List<Edge> { e };
                FollowTransaction(singleList, arbitraryCurrencyAmount);
            }
        }

        private int _searchDepth = 5;

        private List<Edge> _bestTrace;
        private decimal _bestTraceProfit;

        private void FollowTransaction(List<Edge> edges, decimal currentValue, int currentDepth = 0)
        {
            currentDepth++;
            if (currentDepth >= _searchDepth)
                return;

            var currentEdge = edges[edges.Count - 1];
            var nextVertice = currentEdge.Head;

            currentValue /= currentEdge.ExchangeRate;

            var nextVertArbitraryValue = nextVertice.ArbitraryValue;
            if (nextVertArbitraryValue == 0) return;

            var arbitraryValue = currentValue / nextVertArbitraryValue;

            if (arbitraryValue > _bestTraceProfit)
            {
                Debug("New best deal founded!!!");
                _bestTrace = edges;
                _bestTraceProfit = arbitraryValue;
            }

            foreach (var e in nextVertice.Edges)
            {
                var extendedEdges = new List<Edge>(edges);
                extendedEdges.Add(e);

                FollowTransaction(extendedEdges, currentValue, currentDepth);
            }
        }

        public void ShowBestFoundedTrace()
        {
            if(_bestTrace == null)
            {
                Debug("Something went wrong...");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("");
            Console.WriteLine("==========BEST FOUNDED TRACE==========");
            Console.WriteLine($"Profit: {_bestTraceProfit}");
            Console.Write($"{Constats.ArbitraryCurrency}");
            foreach (var edge in _bestTrace)            
                Console.Write($" => {edge.Head.Currency}");

            Console.WriteLine();
            Console.WriteLine("======================================");
        }

    #endregion

        private void BellmanFordFindtraceWithProfit()
        {
            throw new NotImplementedException();        
        }
    }
}
