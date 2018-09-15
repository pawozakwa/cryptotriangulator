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

        #region Network initialization

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
                Console.Write("Feeding Network with actual tickers...");
                stopWatch.Restart();
                foreach (var tickerKV in tickersFromExchange)
                {
                    _currencyNetwork.AddEdge(tickerKV.Key, tickerKV.Value, exchangeApi);
                }
                Console.WriteLine($"   <= Done!");
            }
        }

        #endregion

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

        private int _searchDepth = 5;
        public int SearchDepth
        {
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Value has to bed over 0");
                else _searchDepth = value;
            }
            get => _searchDepth;
        }

        private decimal _commision = 0.1m / 100m;
        public decimal Commision
        {
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("0 < commision < 1");
                else _commision = value;
            }
            get => _commision;
        }

        private List<Edge> _bestTrace;

        public void ShowBestFoundedTrace()
        {
            if (_bestTrace == null) throw new Exception("There is no best trace");

            if (BestTraceProfit < 1)
            {
                Helpers.Helpers.PrintInColor("Founded chain is not profitable yet...", ConsoleColor.DarkGray);
            }

            var previousConsoleColor = Console.ForegroundColor;

            var result = $"==========BEST FOUNDED TRACE==========" +
                          Environment.NewLine + $"Result after: {BestTraceProfit}";
            var resultFoot = @"======================================" + Environment.NewLine;
            Helpers.Helpers.PrintInColor(result, ConsoleColor.Red);

            Console.Write($">{Constats.ArbitraryCurrency} {Environment.NewLine}");
            foreach (var edge in _bestTrace)
                Console.Write($">{edge.Head.Currency} {Environment.NewLine}");

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
            currentValue -= currentValue * _commision;
        }

        private static decimal SimulateExchange(ref decimal currentValue, Edge currentEdge) => currentValue /= currentEdge.ExchangeRate;

        #endregion

        #region Bellman-Ford aproach

        private void BellmanFordFindtraceWithProfit()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
