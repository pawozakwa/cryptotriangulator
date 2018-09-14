using ExchangeSharp;

namespace TraceMapper
{
    public struct Edge
    {
        public readonly ExchangeTicker Ticker;
        public readonly Vertice Head;        
        public decimal ExchangeRate;
        public ExchangeAPI ExchangeApi;

        public Edge(Vertice head, ExchangeTicker ticker, ExchangeAPI exchangeApi, bool Inverted = false)
        {
            Head = head;
            Ticker = ticker;
            ExchangeApi = exchangeApi;
            ExchangeRate = Inverted ? 1 / ticker.Bid : ticker.Ask;
        }
    }
}
