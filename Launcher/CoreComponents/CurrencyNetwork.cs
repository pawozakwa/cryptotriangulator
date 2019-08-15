using System;
using System.Collections.Generic;
using ExchangeSharp;
using System.IO;

using static Helpers.Helpers;
using Contracts.DataStructures;
using Contracts.Enums;
using Contracts;

namespace Triangulator.CoreComponents
{
    public class CurrencyNetwork
    {
        public List<Edge> Edges;
        public Dictionary<Currency, Vertice> VerticesDictionary;
        public Dictionary<string, Edge> ForwardEdgesDictionary;
        public Dictionary<string, Edge> BackwardEdgesDictionary;

        public CurrencyNetwork()
        {
            PrintInColor("Creating api provider", color: ConsoleColor.Cyan);
            Edges = new List<Edge>();
            VerticesDictionary = new Dictionary<Currency, Vertice>();
            ForwardEdgesDictionary = new Dictionary<string, Edge>();
            BackwardEdgesDictionary = new Dictionary<string, Edge>();
    }

        /// <returns>True if currency added or already exist in the graph, otherwise false</returns>
        public bool AddEdge(string tickerName, ExchangeTicker ticker, ExchangeAPI exchangeApi)
        {
            Currency? head, tail;
            var separator = exchangeApi.MarketSymbolSeparator;
            string[] symbols = new string[2];

            if (separator == "")
            {
                symbols[0] = tickerName.Substring(0, tickerName.Length / 2);
                symbols[1] = tickerName.Remove(0, tickerName.Length / 2);
            }
            else
            {
                symbols = tickerName.Split(separator);
            }

            head = ParseSymbolToEnum(symbols[0]);
            tail = ParseSymbolToEnum(symbols[1]);

            if (head == null || tail == null)
                return false;

            AddVerticesIfNew(head, tail, out Vertice headVertice, out Vertice tailVertice);
            TryToAddForwardEdge(tickerName, ticker, head, tail, headVertice, tailVertice);
            TryToAddBackwardEdge(tickerName, ticker, head, tail, headVertice, tailVertice);

            return true;
        }

        private void TryToAddBackwardEdge(string tickerName, ExchangeTicker ticker, Currency? head, Currency? tail, Vertice headVertice, Vertice tailVertice)
        {
            try
            {
                if (BackwardEdgesDictionary.ContainsKey(tickerName))
                {
                    BackwardEdgesDictionary[tickerName].Update(ticker, true);
                    return;
                }

                var backwardEdge = new Edge(tickerName, tailVertice, ticker, true);
                headVertice.Edges.Add(backwardEdge);
                Edges.Add(backwardEdge);
                BackwardEdgesDictionary.Add(tickerName, backwardEdge);
                PrintDebugForTicker(tickerName, ticker, head, tail, backwardEdge);
            }
            catch (Exception) {
                Debug("Ticker is broken, exchange rate equal zero!");
            }
        }

        private void TryToAddForwardEdge(string tickerName, ExchangeTicker ticker, Currency? head, Currency? tail, Vertice headVertice, Vertice tailVertice)
        {
            try
            {
                if (ForwardEdgesDictionary.ContainsKey(tickerName))
                {
                    ForwardEdgesDictionary[tickerName].Update(ticker, false);
                    return;
                }

                var edge = new Edge(tickerName, headVertice, ticker);
                tailVertice.Edges.Add(edge);
                Edges.Add(edge);
                ForwardEdgesDictionary.Add(tickerName, edge);
                PrintDebugForTicker(tickerName, ticker,  tail, head, edge);
            }
            catch (Exception) {
                Debug("Ticker is broken, exchange rate equal zero!");
            }
        }

        private static void PrintDebugForTicker(string tickerName, ExchangeTicker ticker, Currency? head, Currency? tail, Edge backwardEdge)
        {
            Debug($"Ticker:{tickerName} ask = {ticker.Ask} bid = {ticker.Bid} ");
            Debug($"Added edge: {tail} costs {backwardEdge.ExchangeRate} {head}");
        }

        private static Currency? ParseSymbolToEnum(string symbol)
        {
            try
            {
                return (Currency)Enum.Parse(typeof(Currency), symbol);
            }
            catch (Exception e)
            {
                Debug(e.Message);
                if (Constants.SaveWhatIsUnparsableToFile)
                    File.AppendAllLines("currencies.txt", new[] { symbol });
                return null;
            }
        }

        private void AddVerticesIfNew(Currency? head, Currency? tail, out Vertice headVertice, out Vertice tailVertice)
        {
            Currency h = (Currency)head;
            headVertice = VerticesDictionary.ContainsKey(h) ? VerticesDictionary[h] : new Vertice(h);
            VerticesDictionary[h] = headVertice;

            Currency t = (Currency)tail;
            tailVertice = VerticesDictionary.ContainsKey(t) ? VerticesDictionary[t] : new Vertice(t);
            VerticesDictionary[t] = tailVertice;
        }

        #region Drawing network

        public void OpenWindowWithNetworkRepresentation()
        {

        }

        #endregion
    }
}
