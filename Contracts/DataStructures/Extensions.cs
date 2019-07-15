using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Contracts.Enums;

namespace Contracts.DataStructures
{
    public static class Extensions
    {
        public static Edge Last(this List<Edge> e) => e[e.Count - 1];

        public static List<Edge> GetCompleteChain(this List<Edge> e)
        {
            Edge missingEdge;
            try
            {
                missingEdge = e.Last().Head.Edges.Find(edge => edge.Head.Currency == Constants.ArbitraryCurrency);

            }
            catch (Exception)
            {
                return e;
                throw;
            }

            var completeChain = new List<Edge>(e);
            completeChain.Add(missingEdge);
            return completeChain;
        }

    }
}
