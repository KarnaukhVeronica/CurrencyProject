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

            collectionTopCurrencies = new ObservableCollection<Currency>();
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



        //
        // general variables
        //
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
    }
}
