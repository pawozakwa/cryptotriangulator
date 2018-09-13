using ExchangeSharp;

namespace TraceMapper
{
    public struct Edge
    {
        public readonly Vertice Head;

        public readonly ExchangeTicker Ticker;
        
        public decimal ExchangeRate;

        public ExchangeAPI ExchangeApi;

        public Edge(Vertice head, ExchangeTicker ticker, ExchangeAPI exchangeApi)
        {
            Head = head;
            ExchangeRate = ticker.Ask;
            Ticker = ticker;
            ExchangeApi = exchangeApi;
        }
    }
}
