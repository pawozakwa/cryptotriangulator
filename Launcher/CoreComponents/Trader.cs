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

        const string keysFileName = @"F:\Projects\External\ExchangeSharp\ExchangeSharpConsole\bin\Release\net472\keys100.bin";

        private Task<decimal> _getAmount;

        public Trader(ExchangeAPI api)
        {
            PrintInColor("Creating network Trader", color: ConsoleColor.Cyan);
            _exchangeApi = api;
            _exchangeApi.LoadAPIKeys(keysFileName);

            _getAmount = GetArbitraryAmount();
        }

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

                    amount = (await amountsDictionary)[currencyToGetAmount];
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
                var messageColor = result.Result == ExchangeAPIOrderResult.Filled ? ConsoleColor.Green : ConsoleColor.Red;
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

        private async Task<decimal> GetArbitraryAmount()
        {
            var amountsDictionary = await _exchangeApi.GetAmountsAsync();
            return amountsDictionary[Constants.ArbitraryCurrency.ToString()];
        }

        public async Task<decimal> ArbitraryAmount()
        { 
            var result = await _getAmount;
            _getAmount = GetArbitraryAmount();
            return result;
        }
    }
}