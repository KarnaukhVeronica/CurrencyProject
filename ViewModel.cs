using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Input;

namespace Cryptocurrency
{
    // class for connection between View and Model
    public class ViewModel : INotifyPropertyChanged
    {
        // constructor
        public ViewModel()
        {
            NavigateHyperlinkCommand = new RelayCommand<string>(NavigateHyperlink);
            ClickButtonCommand = new RelayCommand<string>(ClickButton);

            collectionTopCurrencies = new ObservableCollection<Currency>();
            collectionSearchCurrencies = new ObservableCollection<Currency>();
        }

        // trigger property changing
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //
        // commands
        //

        // activate hyperlink
        public ICommand NavigateHyperlinkCommand { get; }
        private void NavigateHyperlink(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: Failed to open URL: " + ex.Message);
            }
        }

        // activate button
        public ICommand ClickButtonCommand { get;  }
        private void ClickButton(string button)
        {
            switch (button)
            {
                case "Search":
                    {
                        Debug.WriteLine("Search Button was clicked!");
                        SearchCurrency();
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Error: Button not identified!");
                        break;
                    }
            }
        }


        //
        // general variables
        //

        // change search prompt - connect to View
        private string searchPrompt;
        public string SearchPrompt
        {
            get
            {
                return searchPrompt;
            }

            set
            {
                searchPrompt = value;
                OnPropertyChanged(nameof(SearchPrompt));
            }
        }

        // save error message
        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }


        //
        // tab control
        //

        // change selected tab - connect to View
        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get
            {
                return selectedTabIndex;
            }

            set
            {
                if (selectedTabIndex != value)
                {
                    Debug.WriteLine($"Selected tab index: {value}");
                    selectedTabIndex = value;
                    OnPropertyChanged(nameof(SelectedTabIndex));
                    TabChanged(selectedTabIndex);
                }
            }
        }

        // change selected tab - logic
        private void TabChanged(int index)
        {
            ErrorMessage = string.Empty;
            Debug.WriteLine("Tab was changed!");
            switch (index)
            {
                case 0:
                    {
                        Debug.WriteLine("Tab 0 opened!");
                        break;
                    }
                case 1:
                    {
                        Debug.WriteLine("Tab 1 opened!");
                        LoadTabTopCurrency();
                        break;
                    }
                case 2:
                    {
                        Debug.WriteLine("Tab 2 opened!");
                        break;
                    }
                case 3:
                    {
                        Debug.WriteLine("Tab 3 opened!");
                        break;
                    }
                case 4:
                    {
                        Debug.WriteLine("Tab 4 opened!");
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Error: Tab not identified!");
                        break;
                    }
            }
        }

        // tab 1 - load top currency - connect to View
        private ObservableCollection<Currency> collectionTopCurrencies;
        public ObservableCollection<Currency> CollectionTopCurrencies
        {
            get
            {
                return collectionTopCurrencies;
            }

            set
            {
                if (collectionTopCurrencies != value)
                {
                    Debug.WriteLine("Collection of Top Currency was changed!");
                    collectionTopCurrencies = value;
                    OnPropertyChanged(nameof(CollectionTopCurrencies));
                }
            }
        }

        // tab 1 - load top currency - logic
        private void LoadTabTopCurrency()
        {
            try
            {
                // get top 10 currencies from API

                string endpoints = "/coins/markets";
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "vs_currency", "uah" },
                    { "order", "market_cap_rank" },
                    { "per_page", "10" },
                    { "page", "1" }
                };

                string response = Model.makeAPICall(endpoints, parameters);
                var topCurrencies = JsonConvert.DeserializeObject<Currency[]>(response);
                CollectionTopCurrencies = new ObservableCollection<Currency>(topCurrencies);
            }
            catch (WebException exc)
            {
                Debug.WriteLine("Error: Response for tab 1 can't be disparsed!");
                Debug.WriteLine("Error WebException: " + exc.Message);

                CollectionTopCurrencies = new ObservableCollection<Currency>();
                ErrorMessage = "Server isn't available, try later!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: Response for tab 1 can't be disparsed!");
                Debug.WriteLine("Error Exception: " + ex.Message);

                CollectionTopCurrencies = new ObservableCollection<Currency>();
                ErrorMessage = "Unexpected error, try later!";
            }
        }

        // tab 2 - search - connect to View
        private ObservableCollection<Currency> collectionSearchCurrencies;
        public ObservableCollection<Currency> CollectionSearchCurrencies
        {
            get
            {
                return collectionSearchCurrencies;
            }

            set
            {
                if (collectionSearchCurrencies != value)
                {
                    Debug.WriteLine("Collection of Search Currency was changed!");
                    collectionSearchCurrencies = value;
                    OnPropertyChanged(nameof(CollectionSearchCurrencies));
                }
            }
        }

        // tab 2 - search - logic
        private void SearchCurrency()
        {
            ErrorMessage = string.Empty;
            Debug.WriteLine($"Search for {searchPrompt} was initiated!");
            try
            {
                // get results for search promt from API

                string endpoints = "/search";
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "query", searchPrompt }
                };

                string response = Model.makeAPICall(endpoints, parameters);

                var coins = JObject.Parse(response)["coins"].ToObject<List<dynamic>>();

                var coinsByName = coins
                                    .Where(c =>
                                    ((string)c.symbol).IndexOf(searchPrompt, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    ((string)c.name).IndexOf(searchPrompt, StringComparison.OrdinalIgnoreCase) >= 0)
                                    .ToList();

                string ids = string.Join(",", coinsByName.Select(c => (string)c.id));

                if (ids != String.Empty)
                {
                    // get info for searched currencies from API

                    endpoints = "/coins/markets";

                    parameters.Clear();
                    parameters.Add("vs_currency", "uah");
                    parameters.Add("ids", ids);
                    parameters.Add("order", "name");
                    parameters.Add("per_page", "250");
                    parameters.Add("page", "1");

                    response = Model.makeAPICall(endpoints, parameters);
                    var searchCurrencies = JsonConvert.DeserializeObject<Currency[]>(response);
                    CollectionSearchCurrencies = new ObservableCollection<Currency>(searchCurrencies);
                }
                else
                {
                    ErrorMessage = "Not Found";
                    Debug.WriteLine("Search unsuccessfull!");
                    CollectionSearchCurrencies = new ObservableCollection<Currency>();
                }
            }
            catch (WebException exc)
            {
                Debug.WriteLine("Error: Response for tab 2 can't be disparsed!");
                Debug.WriteLine("Error WebException: " + exc.Message);

                CollectionSearchCurrencies = new ObservableCollection<Currency>();
                ErrorMessage = "Server isn't available, try later!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: Response for tab 2 can't be disparsed!");
                Debug.WriteLine("Error Exception: " + ex.Message);

                CollectionSearchCurrencies = new ObservableCollection<Currency>();
                ErrorMessage = "Unexpected error, plase try later!";
            }
        }

        // change selected item for tab 3 - connect to View
        private Currency selectedItem;
        public Currency SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                if (value != null)
                {
                    Debug.WriteLine("Selection item was changed!");
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    ItemChanged(selectedItem.id);
                }
            }
        }

        // change selected item for tab 3 - connect to View
        private Currency detailsItem;
        public Currency DetailsItem
        {
            get
            {
                return detailsItem;
            }

            set
            {
                if (value != null)
                {
                    Debug.WriteLine("Item details!");
                    detailsItem = value;
                    OnPropertyChanged(nameof(DetailsItem));
                }
            }
        }

        // change selected item for tab 3 - logic
        private void ItemChanged(string id)
        {
            ErrorMessage = string.Empty;

            Debug.WriteLine("Item selection was changed!");

            try
            {
                // get general info about specific currency from API

                string endpoints = $"/coins/{id}";
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                string response = Model.makeAPICall(endpoints, parameters);

                // remove unnecessary data
                var jsonObj = JObject.Parse(response);
                jsonObj["image"] = jsonObj["image"]?["large"]?.ToString();
                jsonObj["current_price"] = jsonObj["market_data"]?["current_price"]?["uah"]?.Value<decimal>() ?? 0;
                jsonObj["price_change_percentage_24h"] = jsonObj["market_data"]?["price_change_percentage_24h"]?.Value<double>() ?? 0;
                jsonObj["total_volume"] = jsonObj["market_data"]?["total_volume"]?["uah"]?.Value<decimal>() ?? 0;
                var item = JsonConvert.DeserializeObject<Currency>(jsonObj.ToString());

                // get info about specific currency's tickers from API

                var _endpoints = endpoints + "/tickers";
                response = Model.makeAPICall(_endpoints, parameters);

                // transform data
                var tickers = JObject.Parse(response)["tickers"].ToObject<List<Ticker>>();
                var market_names = tickers.Select(t => t.market.name).Distinct().ToList();
                item.marketNames = market_names;

                // get info about specific currency's chnage in price from API

                _endpoints = endpoints + "/market_chart";

                parameters.Clear();
                parameters.Add("vs_currency", "uah");
                parameters.Add("days", "7");

                response = Model.makeAPICall(_endpoints, parameters);
                var prices = JObject.Parse(response)["prices"].ToObject<List<List<string>>>();

                // save as chart
                var chart = new Chart();
                chart.series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title="Chart for change of price",
                        Values = new ChartValues<decimal>(prices.Select(p => decimal.Parse(p[1], CultureInfo.InvariantCulture)))
                    }
                };
                chart.labels = prices.Select(p => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(p[0])).DateTime.ToString("G")).ToList();
                item.chart = chart;

                // update item
                DetailsItem = item;
                // change tab
                SelectedTabIndex = 3;
            }
            catch (WebException exc)
            {
                Debug.WriteLine("Error: Response for tab 3 can't be disparsed!");
                Debug.WriteLine("Error WebException: " + exc.Message);

                ErrorMessage = "Server isn't available, try later!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: Response for tab 3 can't be disparsed!");
                Debug.WriteLine("Error Exception: " + ex.Message);

                ErrorMessage = "Unexpected error, try later!";
            }
        }
    }
}
