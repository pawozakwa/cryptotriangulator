using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ExchangeSharp;
using Contracts;
using System.Collections.Generic;

using static Helpers.Helpers;
using static Helpers.SoundsProvider;
using Contracts.Enums;
using Contracts.DataStructures;
using System.Text;

namespace Triangulator.CoreComponents
{
    public class NetworkCrawler
    {
        #region Network initialization

        private CurrencyNetwork _currencyNetwork;
        private ExchangeAPI _exchangeApi;
        private Trader _trader;
        public NetworkCrawler(ExchangeAPI exchangeApi, CurrencyNetwork network, Trader trader)
        {
            PrintInColor("Creating network crawler", color: ConsoleColor.Cyan);
            LoadContants();
            _exchangeApi = exchangeApi;
            _currencyNetwork = network;
            _trader = trader;
        }

        public async Task InitializeNetwork()
        {
            //_currencyNetwork = new CurrencyNetwork(); //This can be moved to constructor to update without recreation, but Edge struct must become class

            //Remember about cleanup
            //In produce environment Network should be recreated in some periods

            _bestChainToVertice = new Dictionary<Currency, decimal>();
            var stopWatch = new Stopwatch();

            Console.Write("Downloading actual tickers...");
            stopWatch.Start();
            var tickersFromExchange = _exchangeApi.GetTickersAsync();
            stopWatch.Stop();
            Console.WriteLine($"   <= Done!");
            Console.Write("Feeding Network with actual tickers...");
            stopWatch.Restart();
            foreach (var tickerKV in await tickersFromExchange)
            {
                if (tickerKV.Value.Ask == 0 || tickerKV.Value.Bid == 0 || tickerKV.Value.Last == 0)
                    continue;

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

        #region Scanning network
        
        public decimal FindBestChainOfTransactions(decimal arbitraryCurrencyAmount = 1m)
        {
            Vertice enterVertice =
                _currencyNetwork.VerticesDictionary[Constants.EnterCurrency];

            _bestChain = null;
            BestChainProfit = 0;

            PrintInColor("Crawling through the network ...", ConsoleColor.DarkGray);

            if(Constants.LiveProgressBar)
                enterVertice.Edges.ForEach(e => Console.Write("_"));

            Parallel.ForEach(enterVertice.Edges, (e) =>
                {   
                    var singleList = new List<Edge> { e };
                    FollowTransaction(singleList, arbitraryCurrencyAmount);
                    if(Constants.LiveProgressBar)
                        Console.Write("*");
                });

            return BestChainProfit;
        }

        private void FollowTransaction(List<Edge> edges, decimal currentValue, int currentDepth = 0)
        {
            currentDepth++;
            if (currentDepth >= _searchDepth)
                return;

            var currentEdge = edges.Last();
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
        
        public decimal BestChainProfit { private set; get; }

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

        private List<Edge> _bestChain;

        public void ShowBestFoundedTrace()
        {
            lock (this)
            {
                if (_bestChain == null) throw new Exception("There is no best trace");

                var bestCompleteChain = _bestChain.GetCompleteChain();

                if (BestChainProfit < 1m)
                {
                    HandleNoProfitableTrace(bestCompleteChain);
                }
                else
                {
                    HandleProfitableTrace(bestCompleteChain);
                }
            }
        }

        private void HandleProfitableTrace(List<Edge> bestCompleteChain)
        {
            PrintInColor("$$$ Profitable path founded! $$$", ConsoleColor.Green);
            double rewardPercentage = ((double)BestChainProfit - 1) * 100.0;
            PrintInColor($"$$$ Profit size: {string.Format("{0:0.00}", rewardPercentage)}%       $$$", ConsoleColor.DarkGreen);

            var result = $"==========BEST FOUNDED TRACE==========" +
                          Environment.NewLine + $"Possible result after: {BestChainProfit}";
            PrintInColor(result, ConsoleColor.Magenta);

            WriteChainToConsole(bestCompleteChain);

            //TODO: Calculate real reward for every potentially profitable trace

            WriteChainToFile(result, bestCompleteChain);

            var realReward = GetOptimizedReward(bestCompleteChain);

            if (realReward > 0)
            {
                PlayWinnerMusic();
                PrintInColor($"Real result: {realReward.ToString()}", ConsoleColor.Yellow);
                AddToProfitFileOnDesktop(realReward.ToString());
            }

            var resultFoot = @"======================================" + Environment.NewLine;
            PrintInColor(resultFoot, ConsoleColor.Magenta);
            AddToFileOnDesktop(resultFoot);
        }

        private void HandleNoProfitableTrace(List<Edge> bestCompleteChain)
        {
            var formattedReward = String.Format("{0:N6}", (double)BestChainProfit);
            PrintInColor($"Founded chain is not profitable yet, only {formattedReward}% left ...", ConsoleColor.DarkGray);
            WriteChainToConsole(bestCompleteChain);
        }

        private void WriteChainToConsole(List<Edge> chain)
        {
            Console.Write($"> {Constants.EnterCurrency}");
            foreach (var edge in chain)
                Console.Write($" > {edge.Head.Currency}");
            //Console.Write($" > {Constants.EnterCurrency} {Environment.NewLine}");
            Console.WriteLine();
        }

        private void WriteChainToFile(string result, List<Edge> chain)
        {
            var builder = new StringBuilder();
            builder.Append(result);
            builder.Append($"> {Constants.EnterCurrency}");

            foreach (var edge in chain)
                builder.Append($" > {edge.Head.Currency}");

            AddToFileOnDesktop(builder.ToString());
        }

        private void CheckForNewBestGlobalProfit(List<Edge> edges, decimal arbitraryValue)
        {
            if (arbitraryValue < BestChainProfit) return;
            Debug($"New best chain profit: {arbitraryValue}");
            _bestChain = edges;
            BestChainProfit = arbitraryValue;
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

        #region Calculate exact current reward

        private const decimal minimalInvestitionSize = 0.00001m;

        private List<Edge> _chainToOptimize;

        public decimal GetOptimizedReward(List<Edge> chainToAnalyze, decimal maxAmountToInvest = 0.1m)
        {
            _chainToOptimize = chainToAnalyze;

            var booksRecevingTasks = new List<Task<ExchangeOrderBook>>();
            Edge edge;
            for (int i = 0; i < chainToAnalyze.Count; i++)
            {
                edge = chainToAnalyze[i];
                booksRecevingTasks.Add(_exchangeApi.GetOrderBookAsync(edge.TickerName, 500));
            }

            var profitsDictionary = new Dictionary<decimal, decimal>();

            FillProfitDictionary(booksRecevingTasks, ref profitsDictionary, minimalInvestitionSize, maxAmountToInvest, minimalInvestitionSize);

            var bestProfit = FindBestAmountToInvest(profitsDictionary, out decimal bestAmountToInvest);

            if(bestProfit > 0)
                _trader.PlaceOrdersChain(bestAmountToInvest, chainToAnalyze).Start();
            
            SaveProfitToReportFileIfAny(chainToAnalyze, bestProfit, bestAmountToInvest);

            WriteAllOrderBooksToFile(booksRecevingTasks);

            return bestProfit;
        }

        private static void SaveProfitToReportFileIfAny(List<Edge> chainToAnalyze, decimal bestProfit, decimal bestAmountToInvest)
        {
            if (bestProfit > 0)
            {
                var report = new List<string>(){
                    "--------------Founded profitable trace!-------------",
                };
                var chain = "BTC";
                foreach (var edgeToPrint in chainToAnalyze)
                    chain += $" > {edgeToPrint.Head.Currency}";

                report.AddRange(new List<string>(){
                        chain,
                        "Real profit which can be obtained:",
                        bestProfit.ToString() + " BTC",
                        "By investing:",
                        bestAmountToInvest.ToString() + " BTC",
                        "----------------------------------------------------"
                });

                AddToFileOnDesktop(report.ToArray());
            }
            else
            {
                AddToFileOnDesktop("BAAAD! There is no amount to invest with real profit...");
            }
        }

        private decimal FindBestAmountToInvest(Dictionary<decimal, decimal> profitsDictionary, out decimal bestAmountToInvest)
        {
            decimal maxProfit = decimal.MinValue;
            bestAmountToInvest = decimal.Zero;

            foreach(var keyValuePair in profitsDictionary)
            {
                var profit = keyValuePair.Value - keyValuePair.Key;

                PrintInColor($"In this chain investing:{keyValuePair.Key} -> {keyValuePair.Value}, profit: {profit}", ConsoleColor.White);

                if (profit > maxProfit)
                {
                    maxProfit = profit;
                    bestAmountToInvest = keyValuePair.Key;
                }
            }

            return maxProfit;
        }

        private void FillProfitDictionary(List<Task<ExchangeOrderBook>> getBooksTasks, ref Dictionary<decimal, decimal> profitDictionary, decimal start, decimal limit, decimal minimalStep, int depth = 8){
    
            if (depth == 0) 
                return;
                
            depth--;
                
            var mid = (limit - start) *0.5m;
            var p1 = mid - minimalStep * 0.5m;
            var p2 = mid + minimalStep * 0.5m;
            
            profitDictionary[p1] = GetExactCurrentReward(getBooksTasks, p1);
            profitDictionary[p2] = GetExactCurrentReward(getBooksTasks, p2);
            
            if(profitDictionary[p1] > profitDictionary[p2])
                FillProfitDictionary(getBooksTasks, ref profitDictionary, start, p2, minimalStep, depth);
            else
                FillProfitDictionary(getBooksTasks, ref profitDictionary, p1, limit, minimalStep, depth);
        }

        private decimal GetExactCurrentReward(List<Task<ExchangeOrderBook>> booksRecevingTasks, decimal initialAmount = 0.1m )
        {
            decimal[] intermediateAmounts = new decimal[_chainToOptimize.Count + 1];
            intermediateAmounts[0] = initialAmount;

            Edge edge;
                        
            for (int i = 0; i < _chainToOptimize.Count; i++)
            {
                edge = _chainToOptimize[i];
                var orderBook = booksRecevingTasks[i].Result;

                //TODO: First iteration should update initial amount

                // HOW TO CALCULATE HOW MUCH BTC WE NEED TO SPENT TO GET MAX PROFIT?
                // !BINARY SEARCH!

                if (!edge.Inverted)
                {
                    foreach (var order in orderBook.Asks.Values)
                    {
                        var price = order.Price;

                        var amountToPay = Math.Min(intermediateAmounts[i], order.Amount * price);
                        if (amountToPay <= 0) break;

                        var amountToGet =  amountToPay / price;

                        SimulateCommision(ref amountToGet);

                        intermediateAmounts[i] -= amountToPay;
                        intermediateAmounts[i + 1] += amountToGet;

                        Debug($"Straight {edge.TickerName}: {amountToGet} for {amountToPay}   Price:{price}");
                    }
                }
                else
                {
                    foreach (var order in orderBook.Bids.Values)
                    {
                        var price = 1 / order.Price;

                        var amountToPay = Math.Min(intermediateAmounts[i], order.Amount * price);
                        if (amountToPay <= 0) break;

                        var amountToGet = amountToPay / price;

                        SimulateCommision(ref amountToGet);

                        intermediateAmounts[i] -= amountToPay;
                        intermediateAmounts[i + 1] += amountToGet;

                        Debug($"Inverted {edge.TickerName}: {amountToGet} for {amountToPay}   Price:{price}");
                    }
                }
            }

            //var percentLeft = (intermediateAmounts[intermediateAmounts.Length - 1] - initialAmount) * 100m;
            //return percentLeft;

            return intermediateAmounts[intermediateAmounts.Length - 1];
        }

        private void WriteAllOrderBooksToFile(List<Task<ExchangeOrderBook>> tasksForOrders)
        {
            Task.WaitAll(tasksForOrders.ToArray());

            foreach (var task in tasksForOrders)
            {
                AddToFileOnDesktop(task.Result.ToString());
            }
        }

        #endregion
    }
}
