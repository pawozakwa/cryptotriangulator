using System;
using System.Collections.Generic;
using ExchangeSharp;
using Contracts;
using System.IO;

using static Helpers.Helpers;

namespace TraceMapper
{
    public class CurrencyNetwork
    {
        public List<Edge> Edges;
        public Dictionary<Currency, Vertice> VerticesDictionary;

        public CurrencyNetwork()
        {
            Edges = new List<Edge>();
            VerticesDictionary = new Dictionary<Currency, Vertice>();
        }
        
        /// <returns>True if currency added or already exist in the graph, otherwise false</returns>
        public bool AddEdge(string tickerName, ExchangeTicker ticker, ExchangeAPI exchangeApi)
        {
            Currency head, tail;
            var separator = exchangeApi.SymbolSeparator;            
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

            try
            {
                head = (Currency)Enum.Parse(typeof(Currency), symbols[0]);
                tail = (Currency)Enum.Parse(typeof(Currency), symbols[1]);
            }
            catch (Exception e)
            {
                Debug("Exception during parsing ticker currencies:");
                Debug(e.Message);
                File.AppendAllLines("currencies.txt", new[] { symbols[1] });
                File.AppendAllLines("currencies.txt", new[] { symbols[0] });
                return false;
            }
            
            Debug($"Added edges: {head} <- {ticker.Ask} -> {tail}");

            var headVertice = VerticesDictionary.ContainsKey(head) ? VerticesDictionary[head] : new Vertice(head);
            var tailVertice = VerticesDictionary.ContainsKey(tail) ? VerticesDictionary[tail] : new Vertice(tail);

            var edge = new Edge(headVertice, ticker, exchangeApi);
            var backwardEdge = new Edge(tailVertice, ticker, exchangeApi, true);


            VerticesDictionary[head] = headVertice;
            VerticesDictionary[tail] = tailVertice;

            if(ticker.Ask == 0) return true;
            
            tailVertice.Edges.Add(edge);
            headVertice.Edges.Add(backwardEdge);

            Edges.Add(edge);
            Edges.Add(backwardEdge);

            return true;
        }
       
        // TODO
        //public void DrawNetwork()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
