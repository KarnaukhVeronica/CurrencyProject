using LiveCharts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Cryptocurrency
{
    // class for charts
    public class Chart
    {
        [JsonIgnore]
        public SeriesCollection series { get; set; }

        [JsonIgnore]
        public List<string> labels { get; set; }
    }

    // class for markets
    public class Market
    {
        [JsonProperty("identifier")]
        public string identifier { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("description")]
        public string? description { get; set; }

        [JsonProperty("has_trading_incentive")]
        public bool has_trading_incentive { get; set; }

        [JsonProperty("trust_scrore_rank")]
        public int? trust_scrore_rank { get; set; }
    }

    // class for tickers
    public class Ticker
    {
        [JsonProperty("base")]
        public string base_ { get; set; }

        [JsonProperty("target")]
        public string target { get; set; }

        [JsonProperty("market")]
        public Market market { get; set; }

        [JsonProperty("trust_score")]
        public string trust_score { get; set; }
    }

    // class for links
    public class Links
    {
        [JsonProperty("homepage")]
        public List<string>? homepage { get; set;  }

        [JsonProperty("blockchain_site")]
        public List<string>? blockchain_site { get; set; }

        [JsonProperty("github")]
        public List<string>? github { get; set; }
    }

    // class for coins
    public class Currency
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("symbol")]
        public string symbol { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("current_price")]
        public decimal? current_price { get; set; }

        [JsonProperty("market_cap_rank")]
        public int? market_cap_rank { get; set; }

        [JsonProperty("description")]
        public Dictionary<string, string>? description { get; set; }

        // retrieve only english
        [JsonIgnore]
        public string? descriptionEN =>
            (description != null && description.ContainsKey("en")) ? description["en"] : null;

        [JsonProperty("links")]
        public Links? links { get; set; }

        // all links in one list
        public List<string>? linksAll =>
            (links?.homepage ?? Enumerable.Empty<string>())
            .Concat(links?.blockchain_site ?? Enumerable.Empty<string>())
            .Concat(links?.github ?? Enumerable.Empty<string>())
            .Where(link => !string.IsNullOrWhiteSpace(link))
            .ToList();

        [JsonProperty("categories")]
        public List<string>? categories { get; set; }

        [JsonProperty("total_volume")]
        public decimal? total_volume { get; set; }

        [JsonProperty("price_change_percentage_24h")]
        public double? price_change_percentage_24h { get; set; }

        [JsonProperty("tickers")]
        public List<Ticker>? tickers { get; set; }

        [JsonIgnore]
        public List<string>? marketNames { get; set; }

        [JsonIgnore]
        public Chart? chart { get; set; }
    }
}
