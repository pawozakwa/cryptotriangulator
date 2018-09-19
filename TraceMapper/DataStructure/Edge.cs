using System;
using ExchangeSharp;

namespace TraceMapper
{
    public struct Edge
    {
        public readonly string TickerName;
        public readonly ExchangeTicker Ticker;
        public readonly Vertice Head;        
        public decimal ExchangeRate;
        public ExchangeAPI ExchangeApi;

        public bool Inverted;

        public Edge(string tickerName, Vertice head, ExchangeTicker ticker, ExchangeAPI exchangeApi, bool inverted = false)
        {
            TickerName = tickerName;
            Head = head;
            Ticker = ticker;
            ExchangeApi = exchangeApi;
            ExchangeRate = inverted ? 1 / ticker.Bid : ticker.Ask;
            Inverted = inverted;
            VerifyCorrectness();
        }

        private void VerifyCorrectness()
        {
            if (ExchangeRate == 0)
                throw new ArgumentException();
        }
    }
}
