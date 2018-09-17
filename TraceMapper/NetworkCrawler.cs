﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExchangeSharp;
using Contracts;
using System.Collections.Generic;

using static Helpers.Helpers;
using static Helpers.SoundsProvider;

namespace TraceMapper
{
    public class NetworkCrawler
    {
        #region Network initialization

        private CurrencyNetwork _currencyNetwork;
        private ExchangeAPI _exchangeApi;

        public NetworkCrawler(ExchangeAPI exchangeApi)
        {
            _exchangeApi = exchangeApi;
        }

        public async Task InitializeNetwork()
        {
            LoadContants();

            _currencyNetwork = new CurrencyNetwork();
            _bestChainToVertice = new Dictionary<Currency, decimal>();
            var stopWatch = new Stopwatch();

            Console.Write("Downloading actual tickers...");
            stopWatch.Start();
            var tickersFromExchange = await _exchangeApi.GetTickersAsync();
            stopWatch.Stop();
            Console.WriteLine($"   <= Done!");
            Console.Write("Feeding Network with actual tickers...");
            stopWatch.Restart();
            foreach (var tickerKV in tickersFromExchange)
            {
                try
                {
                    _currencyNetwork.AddEdge(tickerKV.Key, tickerKV.Value, _exchangeApi);
                }
                catch (Exception e)
                {
                    PrintInColor(e.ToString(), ConsoleColor.Red);
                    throw;
                }
            }
            Console.WriteLine($"   <= Done!");
        }

        private void LoadContants()
        {
            _commision = Constants.Commision;
            _searchDepth = Constants.SearchDepth;
        }

        #endregion

        #region Iterative aproach
        
        public decimal FindBestChainOfTransactions(decimal arbitraryCurrencyAmount = 1m)
        {
            Vertice enterVertice =
                _currencyNetwork.VerticesDictionary[Constants.EnterCurrency];

            _bestTrace = null;
            BestTraceProfit = 0;

            PrintInColor("Crawling through the network ...", ConsoleColor.DarkGray);

            if(Constants.LiveProgressBar)
                enterVertice.Edges.ForEach(e => Console.Write("_"));
            Console.WriteLine();

            Parallel.ForEach(enterVertice.Edges, (e) =>
                {   
                    var singleList = new List<Edge> { e };
                    FollowTransaction(singleList, arbitraryCurrencyAmount);
                    if(Constants.LiveProgressBar)
                        Console.Write("*");
                });
            Console.WriteLine();

            return BestTraceProfit;
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
            if (nextVertArbitraryValue == 0)
                return;

            var arbitraryValue = currentValue / nextVertArbitraryValue;

            lock (this)
            {
                if (!IsNewBestToVerticeProfit(nextVertice, arbitraryValue)) return;
                CheckForNewBestGlobalProfit(edges, arbitraryValue);
            }

            foreach (var e in nextVertice.Edges)
            {
                //if (edges.Contains(e))
                //    continue;

                var extendedEdges = new List<Edge>(edges) { e };
                FollowTransaction(extendedEdges, currentValue, currentDepth);
            }
        }
        
        public decimal BestTraceProfit { private set; get; }

        private int _searchDepth;
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

        private decimal _commision;
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
            lock (this)
            {
                if (_bestTrace == null) throw new Exception("There is no best trace");

                if (BestTraceProfit < 1)
                {
                    PrintInColor($"Founded chain is not profitable yet, only {BestTraceProfit} left ...", ConsoleColor.DarkGray);
                    WriteChainToConsole();
                }
                else
                {
                    PlayWinnerMusic();
                    PrintInColor("$$$ Profitable path founded! $$$", ConsoleColor.Green);
                    double rewardPercentage = ((double)BestTraceProfit - 1) * 100.0;
                    PrintInColor($"$$$ Profit size: {string.Format("{0:0.00}", rewardPercentage)}%       $$$", ConsoleColor.DarkGreen);

                    var previousConsoleColor = Console.ForegroundColor;
                    var result = $"==========BEST FOUNDED TRACE==========" +
                                  Environment.NewLine + $"Result after: {BestTraceProfit}";
                    PrintInColor(result, ConsoleColor.Magenta);
                    WriteChainToConsole();
                    var resultFoot = @"======================================" + Environment.NewLine;
                    PrintInColor(resultFoot, ConsoleColor.Magenta);
                }
            }
        }

        private void WriteChainToConsole()
        {
            Console.Write($"> {Constants.EnterCurrency}");
            foreach (var edge in _bestTrace)
                Console.Write($" > {edge.Head.Currency}");
            Console.Write($" > {Constants.EnterCurrency} {Environment.NewLine}");
        }

        private void CheckForNewBestGlobalProfit(List<Edge> edges, decimal arbitraryValue)
        {
            if (arbitraryValue < BestTraceProfit) return;            
            _bestTrace = edges;
            BestTraceProfit = arbitraryValue;
        }

        private Dictionary<Currency, decimal> _bestChainToVertice;

        private bool IsNewBestToVerticeProfit(Vertice verticeToCheck, decimal arbitraryValue)
        {
            if (!_bestChainToVertice.ContainsKey(verticeToCheck.Currency)
                 || arbitraryValue > _bestChainToVertice[verticeToCheck.Currency])
            {
                _bestChainToVertice[verticeToCheck.Currency] = arbitraryValue;
                return true;
            }
            return false;
        }

        private void SimulateCommision(ref decimal currentValue)
        {
            currentValue -= currentValue * _commision;
        }

        private static decimal SimulateExchange(ref decimal currentValue, Edge currentEdge) 
            => currentValue /= currentEdge.ExchangeRate;


        #endregion
    }
}
