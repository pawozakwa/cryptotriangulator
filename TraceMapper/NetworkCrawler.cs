using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExchangeSharp;
using Contracts;
using System.Collections.Generic;

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
                Console.WriteLine($"   <= Done!");
                Console.Write("Feeding actual tickers...");
                stopWatch.Restart();
                foreach (var tickerKV in tickersFromExchange)
                {
                    _currencyNetwork.AddEdge(tickerKV.Key, tickerKV.Value, exchangeApi);
                }
                Console.WriteLine($"   <= Done!");
            }
        }

        #region Iterative aproach

        public decimal IterativeFindTraceWithProfit(int searchDepth = 3, decimal arbitraryCurrencyAmount = 1m)
        {
            Vertice enterVertice =
                _currencyNetwork.VerticesDictionary[Constats.ArbitraryCurrency];

            _bestTrace = null;
            BestTraceProfit = 0;

            foreach (var e in enterVertice.Edges)
            {
                var singleList = new List<Edge> { e };
                FollowTransaction(singleList, arbitraryCurrencyAmount);
            }

            return BestTraceProfit;
        }

        public decimal BestTraceProfit { private set; get; }

        private List<Edge> _bestTrace;
        private int _searchDepth = 5;

        private readonly decimal commision = 0.1m / 100m;

        public void ShowBestFoundedTrace()
        {
            if (_bestTrace == null) throw new Exception("There is no best trace");

            var previousConsoleColor = Console.ForegroundColor;

            var result = @"==========BEST FOUNDED TRACE==========
                            Profit: {BestTraceProfit}
                            {Constats.ArbitraryCurrency}";
            var resultFoot = @"======================================
                                ";
            Helpers.Helpers.PrintInColor(result, ConsoleColor.Red);

            foreach (var edge in _bestTrace)
                Console.Write($" => {edge.Head.Currency}");

            Helpers.Helpers.PrintInColor(resultFoot, ConsoleColor.Red);
        }

        private void FollowTransaction(List<Edge> edges, decimal currentValue, int currentDepth = 0)
        {
            currentDepth++;
            if (currentDepth >= _searchDepth)
                return;

            var currentEdge = edges[edges.Count - 1];
            var nextVertice = currentEdge.Head;

            SimulateExchange(ref currentValue, currentEdge);
            SimulateCommision(ref currentValue);

            var nextVertArbitraryValue = nextVertice.ArbitraryValue;
            if (nextVertArbitraryValue == 0) return;

            var arbitraryValue = currentValue / nextVertArbitraryValue;

            if (arbitraryValue > BestTraceProfit)
            {
                _bestTrace = edges;
                BestTraceProfit = arbitraryValue;
                Helpers.Helpers.PrintInColor($"$$$ New best trace founded! [{BestTraceProfit}] $$$", ConsoleColor.Yellow);
            }

            foreach (var e in nextVertice.Edges)
            {
                var extendedEdges = new List<Edge>(edges);
                extendedEdges.Add(e);
                FollowTransaction(extendedEdges, currentValue, currentDepth);
            }
        }

        private void SimulateCommision(ref decimal currentValue)
        {
            currentValue -= currentValue * commision;
        }

        private static decimal SimulateExchange(ref decimal currentValue, Edge currentEdge) => currentValue /= currentEdge.ExchangeRate;

        #endregion

        private void BellmanFordFindtraceWithProfit()
        {
            throw new NotImplementedException();
        }
    }
}
