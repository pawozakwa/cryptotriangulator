using System;
using System.Net.Http;
using Binance.Net;
using static Helpers.Helpers;

namespace Triangulator.CoreComponents
{
    public class BinanceSocketManager
    {
        private const string BinanceApiUrl = "https://api.binance.com";
        private const string OpenUserStringEndpoint = "/api/v1/userDataStream";


        private BinanceSocketClient _socketClient;
        

        private static readonly HttpClient client = new HttpClient();

        public BinanceSocketManager()
        {
            _socketClient = new BinanceSocketClient();
            //_socketClient.SubscribeToUserStream(
        }

        private void TestApi()
        {
            var testResponse = client.GetAsync(BinanceApiUrl);
            PrintInColor($"Socket test result: {testResponse}", ConsoleColor.DarkYellow);
        }

        private void OpenSocket()
        {

        }

        private void CloseSocket()
        {

        }

    }
}
