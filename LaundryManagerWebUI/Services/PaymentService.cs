using LaundryManagerWebUI.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration config;
        public PaymentService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            this.config = config;
        }

        public async Task<string> InitiazlizePayment()
        {
            string url = "https://sandbox.monnify.com/api/v1/merchant/transactions/init-transaction";
            var data = new
            {
                amount = 100.00,
                customerName = "Stephen Ikhane",
                customerEmail = "stephen@ikhane.com",
                paymentDescription = "Trial transaction",
                paymentReference = "ref" + GetPaymentReference(),
                currencyCode = "NGN",
                contractCode = config["LaundryManagerApi:monnifyContractCode"],
                redirectUrl = "https://my-merchants-page.com/transaction/confirm",
                paymentMethods = new string[] { "CARD", "ACCOUNT_TRANSFER" }

            };
            string json = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseString = await _httpClient.PostAsync(url, content);
            var resp = await responseString.Content.ReadAsStringAsync();
            return resp;
        }


        private string GetPaymentReference()
        {
            var randomNumber = new byte[10];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
