using Contracts;
using Contracts.DataStructures;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Helpers.Helpers;

namespace Triangulator
{
    public class Trader
    {
        private ExchangeAPI _exchangeApi;

        public async Task PlaceOrdersChain(decimal initialAmount, IEnumerable<Edge> edges)
        {
            var firstTradeInChain = true;
            Vertice previousVertice = null;
            foreach (var edge in edges)
            {
                decimal amount;
                var amountsDictionary = _exchangeApi.GetAmountsAsync();
                if (firstTradeInChain)
                {
                    amount = (await amountsDictionary)[Constants.ArbitraryCurrency.ToString()];
                }
                else
                {
                    var currencyToGetAmount = edge.Inverted ?
                            previousVertice.Currency.ToString() :
                            edge.Head.Currency.ToString();

                    amount = (await amountsDictionary)[Constants.ArbitraryCurrency.ToString()];
                }
                firstTradeInChain = false;
                previousVertice = edge.Head;

                var request = new ExchangeOrderRequest()
                {
                    Amount = amount,
                    IsBuy = !edge.Inverted,
                    MarketSymbol = edge.TickerName,
                    Price = edge.Inverted ? edge.Ticker.Bid : edge.Ticker.Ask
                };

                await TryToPlaceOrder(request);
            }
        }

        public async Task TryToPlaceOrder(ExchangeOrderRequest orderRequest)
        {
            try
            {
                var result = await PlaceOrder(orderRequest);
                var messageColor = result.Result == ExchangeAPIOrderResult.Filled ? ConsoleColor.Green :ConsoleColor.Red;
                PrintInColor(result.Message, messageColor);
            }
            catch (Exception e)
            {
                PrintInColor(e.Message, ConsoleColor.Red);
                throw;
            }
        }

        public async Task<ExchangeOrderResult> PlaceOrder(ExchangeOrderRequest orderRequest)
        {
            return await _exchangeApi.PlaceOrderAsync(orderRequest);
        }
    }
}