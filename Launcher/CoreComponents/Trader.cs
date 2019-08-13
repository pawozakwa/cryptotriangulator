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

        public async Task PlaceOrdersChain(decimal initialAmount, IEnumerable<ExchangeOrderRequest> exchanges)
        {
            foreach (var exchange in exchanges)
            {
                await TryToPlaceOrder(exchange);
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