using System;
using System.Collections.Generic;
using Contracts;

namespace TraceMapper
{
    public class Vertice
    {
        public readonly Currency Currency;
        public List<Edge> Edges;
        public bool IsArbitrary =>
            Currency == Constats.ArbitraryCurrency;

        decimal ArbitraryValue =>
            IsArbitrary ? 1m : Edges.Find(e => e.Head.Currency == Constats.ArbitraryCurrency).ExchangeRate;
        
        public Vertice(Currency currency)
        {
            Currency = currency;
            Edges = new List<Edge>();
        }
    }
}
