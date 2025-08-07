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

    }
}
