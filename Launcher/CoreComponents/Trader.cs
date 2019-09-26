using Contracts;
using Contracts.DataStructures;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triangulator.CoreComponents;

using static Helpers.Helpers;

namespace Triangulator
{
    public class Trader
    {
        private ExchangeAPI _exchangeApi;

        const string keysFileName = @"F:\Projects\External\ExchangeSharp\ExchangeSharpConsole\bin\Release\net472\keys100.bin";

        private Task<decimal> _getAmount;

        private BinanceSocketManager _binanceSocketManager; 
        
        public Trader(ExchangeAPI api)
        {
            PrintInColor("Creating network Trader", color: ConsoleColor.Cyan);
            _exchangeApi = api;
            _exchangeApi.LoadAPIKeys(keysFileName);

            _getAmount = GetArbitraryAmount();

            _binanceSocketManager = new BinanceSocketManager();
        }

        public async Task PlaceOrdersChain(decimal initialAmount, IEnumerable<Edge> edges)
        {
            PrintInColor("Trying to place orders chain", ConsoleColor.Yellow);
            Console.WriteLine();

            //TODO This has to be received by sockets
            var amountToGet = await GetArbitraryAmount();

            Vertice previousVertice = new Vertice(Constants.ArbitraryCurrency);
            foreach (var edge in edges)
            {
                decimal amount = decimal.Zero;

                var price = edge.Inverted ? edge.Ticker.Bid : edge.Ticker.Ask;

                var currencyToPay = previousVertice.Currency.ToString();

                var partOfOrderToWaitFor = 0.8m;

                amount = await WaitForBalnceOnAccount(amountToGet, amount, currencyToPay, partOfOrderToWaitFor);

                if (edge.Inverted)
                {
                    amountToGet = amount / price;
                }
                else
                {
                    amountToGet = amount;
                    amount /= price;
                }

                previousVertice = edge.Head;

                var request = new ExchangeOrderRequest()
                {
                    Amount = amount,
                    ShouldRoundAmount = true,
                    IsBuy = !edge.Inverted,
                    MarketSymbol = edge.TickerName,
                    OrderType = OrderType.Market
                };

                await TryToPlaceOrder(request);
            }
        }

        private async Task<decimal> WaitForBalnceOnAccount(decimal amountToGet, decimal amount, string currencyToPay, decimal partOfOrderToWaitFor)
        {
            while (amount < (amountToGet * partOfOrderToWaitFor))
            {
                var amountsDictionary = await _exchangeApi.GetAmountsAsync();

                if (amountsDictionary.ContainsKey(currencyToPay))
                    amount = amountsDictionary[currencyToPay];
            }

            return amount;
        }

        public async Task TryToPlaceOrder(ExchangeOrderRequest orderRequest)
        {
            PrintInColor(orderRequest.GetOrderReport(), ConsoleColor.Yellow);

            ExchangeOrderResult result = null;
            var messageColor = ConsoleColor.Yellow;

            try
            {
                result = await PlaceOrder(orderRequest);
                if (result.Result == ExchangeAPIOrderResult.Filled)
                    messageColor = ConsoleColor.Green;
            }
            catch (Exception e)
            {
                PrintInColor(e.Message, ConsoleColor.Red);
            }
            finally
            {
                PrintInColor(result?.Message, messageColor);
            }
        }

        public async Task<ExchangeOrderResult> PlaceOrder(ExchangeOrderRequest orderRequest)
        {
            //Order could be placed by socket too
            return await _exchangeApi.PlaceOrderAsync(orderRequest);
        }

        private async Task<decimal> GetArbitraryAmount()
        {
            //And again amounts could be received by socket or at least by 
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

    static class LocalExtensions
    {
        public static string GetOrderReport(this ExchangeOrderRequest request)
        {
            var nl = Environment.NewLine;
            var reportBuilder = new StringBuilder();
            reportBuilder.Append($"On pair: {request.MarketSymbol} {nl}");
            reportBuilder.Append($"Is buy order: {request.IsBuy} {nl}");
            reportBuilder.Append($"Amount: {request.Amount} {nl}");
            return reportBuilder.ToString();
        }
    }
}