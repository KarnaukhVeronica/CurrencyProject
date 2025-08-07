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
        // tab control
        //
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
                default:
                    {
                        Debug.WriteLine("Error: Tab not identified!");
                        break;
                    }
            }
        }
    }
}
