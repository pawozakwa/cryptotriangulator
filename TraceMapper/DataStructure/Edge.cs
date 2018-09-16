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

        public Edge(string tickerName, Vertice head, ExchangeTicker ticker, ExchangeAPI exchangeApi, bool Inverted = false)
        {
            TickerName = tickerName;
            Head = head;
            Ticker = ticker;
            ExchangeApi = exchangeApi;
            ExchangeRate = Inverted ? 1 / ticker.Bid : ticker.Ask;
        }
    }
}
