using Newtonsoft.Json;

namespace BankSystem.Models
{
    public class RateResponse
    {
        [JsonProperty("disclaimer")]
        public string Disclaimer { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("base")]
        public string Base { get; set; }
        [JsonProperty("rates")]
        public Valuta Rates { get; set; }
    }
}