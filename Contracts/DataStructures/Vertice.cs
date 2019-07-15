using System;
using System.Collections.Generic;
using Contracts;
using Contracts.Enums;

namespace Contracts.DataStructures
{
    public class Vertice
    {
        public readonly Currency Currency;
        public List<Edge> Edges;
        public bool IsArbitrary =>
            Currency == Constants.ArbitraryCurrency;

        public bool IsEnter =>
            Currency == Constants.EnterCurrency;

        public decimal ArbitraryValue
        {
            get
            {
                var edgeToArbitrary = Edges.Find(e => e.Head.Currency == Constants.ArbitraryCurrency);
                if (edgeToArbitrary == null)
                    return 0;

                return IsArbitrary ? 1m : edgeToArbitrary.ExchangeRate;
            }
        }
        
        public Vertice(Currency currency)
        {
            Currency = currency;
            Edges = new List<Edge>();
        }
    }
}
