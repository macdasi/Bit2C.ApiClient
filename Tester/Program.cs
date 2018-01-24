using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester {
    class Program {
        static void Main(string[] args) {

            string key1 = "[]";
            string secret1 = "[]";
            string url = "https://www.bit2c.co.il/home/";

            Bit2C.API.Client client = new Bit2C.API.Client(url, key1, secret1);

            var b = client.Balance();
            var myOrders = client.MyOrders(Bit2C.API.PairType.BtcNis);
            var ob = client.GetOrderBook(Bit2C.API.PairType.BtcNis);
            var ticker = client.GetTicker(Bit2C.API.PairType.BtcNis);
            var trades = client.GetTrades(Bit2C.API.PairType.BtcNis);
            var orderResponseFailed = client.AddOrder(new Bit2C.API.OrderData { Amount = 0.001m, IsBid = true, Pair = Bit2C.API.PairType.BtcNis, Price = 1000 });
            var orderResponse = client.AddOrder(new Bit2C.API.OrderData { Amount = 0.001m, IsBid = true, Pair = Bit2C.API.PairType.BtcNis, Price = 10010 });
            client.CancelOrder(orderResponse.NewOrder.id);
            client.AddOrderMarketPriceBuy(new Bit2C.API.OrderBuy { Pair = "BtcNis", Total = 100 });
            client.AddOrderMarketPriceSell(new Bit2C.API.OrderSell { Pair = "BtcNis", Amount = 0.0001m });


        }
    }
}
