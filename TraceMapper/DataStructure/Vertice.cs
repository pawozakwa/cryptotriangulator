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
            Currency == Constants.ArbitraryCurrency;

        public bool IsEnter =>
            Currency == Constants.EnterCurrency;

        public decimal ArbitraryValue =>
            IsArbitrary ? 1m : Edges.Find(e => e.Head.Currency == Constants.ArbitraryCurrency).ExchangeRate;
        
        public Vertice(Currency currency)
        {
            Currency = currency;
            Edges = new List<Edge>();
        }
    }
}
