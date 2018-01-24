using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bit2C.API {
    public class Client {
        private const bool DIAGNOS = true;



        private const int WAIT = 10;
        private string Key;
        private string secret;
        //private UInt32 _nonce;
        private string nonce {
            get {
                return DateTime.UtcNow.Ticks.ToString();
            }
        }
        private string URL { get; set; }
        private WebClient client { get; set; }

        private static double ConvertToUnixTimestamp(DateTime date) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private string GetQueryString(object obj) {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + p.GetValue(obj, null).ToString();

            return String.Join("&", properties.ToArray());
        }



        private static string ComputeHash(string secret, string message) {
            var key = Encoding.ASCII.GetBytes(secret.ToUpper());
            string hashString;

            using (var hmac = new HMACSHA512(key)) {
                var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(message));
                hashString = Convert.ToBase64String(hash);
            }

            return hashString;
        }

        public Client(string url, string key, string secret) {
            this.URL = url;
            this.Key = key;
            this.secret = secret;
            client = new WebClient();
            client.Headers.Add("key", key);
        }

        public List<ExchangesTrade> GetTrades(PairType Pair = PairType.BtcNis, long since = 0, double date = 0) {
            using (WebClient client = new WebClient()) {
                System.Threading.Thread.Sleep(WAIT);
                string result = client.DownloadString(URL + "Exchanges/" + Pair.ToString() + "/trades.json");
                List<ExchangesTrade> response = Serializer.Deserialize<List<ExchangesTrade>>(result);
                return response;
            }
        }

        public Ticker GetTicker(PairType Pair = PairType.BtcNis) {
            using (WebClient client = new WebClient()) {




                System.Threading.Thread.Sleep(WAIT);
                var urlTicker = URL + "Exchanges/" + Pair.ToString() + "/Ticker.json";
                string result = client.DownloadString(urlTicker);
                Ticker response = Serializer.Deserialize<Ticker>(result);
                return response;
            }
        }

        public OrderBook GetOrderBook(PairType Pair = PairType.BtcNis) {
            using (WebClient client = new WebClient()) {
                System.Threading.Thread.Sleep(WAIT);
                string result = client.DownloadString(URL + "Exchanges/" + Pair.ToString() + "/orderbook.json");
                if (DIAGNOS) {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("mm:ss"));
                }
                OrderBook response = Serializer.Deserialize<OrderBook>(result);
                return response;
            }
        }

        //FundsHistory
        public object FundsHistory() {
            System.Threading.Thread.Sleep(WAIT);
            string qString = "nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "AccountAPI/FundsHistory.json";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "GET");
            return Serializer.Deserialize<List<AccountRaw>>(result);
        }

        public string OrdersHistory(DateTime? fromTime, DateTime? toTime, string pair) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = string.Format("fromTime={0}&toTime={1}&pair={2}", fromTime, toTime, pair) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/OrdersHistory";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            return result;
        }

        public List<AccountRaw> AccountHistory(DateTime? fromTime, DateTime? toTime) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = string.Format("fromTime={0}&toTime={1}", fromTime, toTime) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AccountHistory";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            List<AccountRaw> response = Serializer.Deserialize<List<AccountRaw>>(result);
            return response;
        }

        public string AddCoinFundsRequest(string Coin, bool refreash) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = string.Format("Coin={0}&refreash={1}", Coin, refreash) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddCoinFundsRequest";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            return result;
        }

        public string AddFund(decimal Total, string Reference, bool IsDeposit) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = string.Format("Total={0}&Reference={1}&IsDeposit={2}", Total, Reference, IsDeposit) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddFund";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            return result;
        }

        public string GetMyId() {
            System.Threading.Thread.Sleep(WAIT);
            string qString = "nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Payment/GetMyId";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            return result;
        }

        public string Send(Guid payTo, decimal Total, string coin) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = string.Format("payTo={0}&Total={1}&coin={2}&nonce={3}", payTo, Total, coin, nonce);
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Payment/Send ";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");

            return result;
        }


        public AddOrderResponse AddOrder(OrderData data) {
            System.Threading.Thread.Sleep(WAIT);
            data.Amount = decimal.Round(data.Amount, 4);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrder";
            string result = string.Empty;
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            try {
                result = Query(qString, url, Key, sign, "POST");
            }
            catch (Exception ex) {
                var post = string.Format("ex:{4} result:{0} data:{1} qString:{2} url:{3}", result, data, qString, url, ex.ToString());
                throw new Exception(post);
            }
            AddOrderResponse response = Serializer.Deserialize<AddOrderResponse>(result);
            if (!string.IsNullOrEmpty(response.error)) {
                var post = string.Format("result:{0} data:{1} qString:{2} url:{3}", result, data, qString, url);
                throw new Exception(post);
            }
            return response;
        }

        //AddOrderMarketPriceBuy
        public AddOrderResponse AddOrderMarketPriceBuy(OrderData data) {
            System.Threading.Thread.Sleep(WAIT);
            data.Amount = decimal.Round(data.Amount, 4);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrderMarketPriceBuy";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            AddOrderResponse response = Serializer.Deserialize<AddOrderResponse>(result);
            if (!string.IsNullOrEmpty(response.error)) {
                var post = string.Format("result:{0} data:{1} qString:{2} url:{3}", result, data, qString, url);
                throw new Exception(post);
            }
            return response;
        }

        public Orders MyOrders(PairType pair) {
            try {
                System.Threading.Thread.Sleep(WAIT);
                string qString = "pair=" + pair.ToString() + "&nonce=" + nonce;
                var sign = ComputeHash(this.secret, qString);
                var url = URL + "Order/MyOrders";
                if (DIAGNOS) {
                    System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
                }
                string result = Query(qString, url, Key, sign, "GET");
                if (!string.IsNullOrEmpty(result)) {
                    JObject orders = JObject.Parse(result);
                    if (orders["error"] != null) {
                        throw new Exception(orders["error"].ToString());
                    }
                    if (orders != null) {
                        var pairOrders = orders[pair.ToString()].ToString();
                        Orders response = Serializer.Deserialize<Orders>(pairOrders);
                        return response;
                    }
                }

                return null;
            }
            catch (Exception ex) {
                throw;
            }
        }

        public void ClearMyOrders(PairType pair) {
            try {
                System.Threading.Thread.Sleep(WAIT);
                string qString = "nonce=" + nonce + "&pair=" + pair.ToString();
                var sign = ComputeHash(this.secret, qString);
                var url = URL + "Order/MyOrders";
                if (DIAGNOS) {
                    System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
                }
                string result = Query(qString, url, Key, sign, "POST");
                Orders myOrders = Serializer.Deserialize<Orders>(result);
                foreach (var item in myOrders.ask) {
                    CancelOrder(item.id);
                }
                foreach (var item in myOrders.bid) {
                    CancelOrder(item.id);
                }
            }
            catch (Exception ex) {

                throw;
            }
        }

        public UserBalance Balance() {
            System.Threading.Thread.Sleep(WAIT);
            string qString = "nonce=" + nonce;

            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Account/Balance";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "GET");
            UserBalance response = Serializer.Deserialize<UserBalance>(result);
            if (!string.IsNullOrEmpty(response.error)) {
                var post = string.Format("result:{0} data:{1} qString:{2} url:{3}", result, "", qString, url);
                System.Diagnostics.Debug.WriteLine(response.error);
                throw new Exception(post);
            }
            return response;
        }

        public void CancelOrder(long id) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = "id=" + id.ToString() + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/CancelOrder";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            OrderResponse response = Serializer.Deserialize<OrderResponse>(result);
            if (!string.IsNullOrEmpty(response.error)) {
                var post = string.Format("result:{0} data:{1} qString:{2} url:{3}", result, id, qString, url);
                throw new Exception(post);
            }
        }

        public string AddOrderMarketPriceBuy(OrderBuy data) {
            System.Threading.Thread.Sleep(WAIT);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrderMarketPriceBuy";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            return result;
        }

        public string GetOrderById(long id) {
            System.Threading.Thread.Sleep(WAIT);
            string qString = "id=" + id.ToString() + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/GetById";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "GET");

            return result;
        }

        public string AddOrderMarketPriceSell(OrderSell data) {
            System.Threading.Thread.Sleep(WAIT);
            data.Amount = decimal.Round(data.Amount, 4);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            string qString = GetQueryString(data) + "&nonce=" + nonce;
            var sign = ComputeHash(this.secret, qString);
            var url = URL + "Order/AddOrderMarketPriceSell";
            if (DIAGNOS) {
                System.Diagnostics.Debug.WriteLine("nonce={0} qString={1} url={2} Key={3}", nonce, qString, url, Key);
            }
            string result = Query(qString, url, Key, sign, "POST");
            return result;
        }

        private string Query(string qString, string url, string key, string sign, string method) {
            var data = Encoding.ASCII.GetBytes(qString);

            if (method.ToUpper().Equals("GET")) {
                if (url.Contains("?")) {
                    url += "&" + qString;
                }
                else {
                    url += "?" + qString;
                }

            }
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            if (request == null)
                throw new Exception("Non HTTP WebRequest");

            request.Method = method;
            request.Timeout = 15000;
            if (method.ToUpper().Equals("POST")) {
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
            }


            request.Headers.Add("Key", key);
            request.Headers.Add("Sign", sign);
            if (method.ToUpper().Equals("POST")) {
                var reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            var response = request.GetResponse();
            var resStream = response.GetResponseStream();
            var resStreamReader = new StreamReader(resStream);
            var resString = resStreamReader.ReadToEnd();

            //string requestParam = string.Format(" qString:{0},  url:{1},  key:{2},  sign:{3},  method:{4}", qString, url, key, sign, method);
            //GlobalNLogger.Log.TraceException("TradeHelper",new Exception( resString));


            return resString;
        }
    }

}
