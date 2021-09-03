using System.Net.Http;
using System.Net.Http.Headers;
using BankSystem.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BankSystem.Service
{
    public class RateService
    {
        public async Task<RateResponse> GetRate()
        {
            HttpResponseMessage responseMessage;
            RateResponse rateResponse;
            using (var client = new HttpClient())
            {
                string url = "https://www.cbr-xml-daily.ru/latest.js";
               
                responseMessage = await client.GetAsync(url);
                responseMessage.EnsureSuccessStatusCode();
                string serializeMessage = await responseMessage.Content.ReadAsStringAsync();
                rateResponse = JsonConvert.DeserializeObject<RateResponse>(serializeMessage);
            }

            return rateResponse;
        }
    }
}