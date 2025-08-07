# CurrencyProject
This project is designed for displaying information about cryptocurencies.

## Used tools
Visual Studio, C#, .NET, WPF and its libraries, CoinGecko API, MVVM.

## CoinGecko API
The data is extracted from **CoinGecko API** (https://www.coingecko.com/).
To learn more about the requests to the API, look into documentation (https://docs.coingecko.com/reference/introduction).

## Key to API
**CoinGecko API** usually requires personal keys to obtain data, which allow users different abilities based on their plan (https://www.coingecko.com/en/api/pricing).
It is possible to work with **CoinGecko API** without using them, but it is advised to use free *Demo API* plan, because it allows more requests per minute.

To get your personal key, you need to sign up to *CoinGecko*, then in *Developer's Dashboard* (https://www.coingecko.com/en/developers/dashboard) add a new key.
To use personal key, you need to insert it into *API_KEY* variable in *Model.cs* file.
If you don't want to use the keys, no changes should be made; however, the program wouldn't be able to fetch data as easily.

## Functionality
The interface is displayed as a multi-page system:
1. *First page* includes information about the API.
2. *Second page* shows top 10 currencies returned by API. By clicking on the currency, user is redirected on the fourth page with detailed information about it.
3. *Third page* allows to search currencies by symbol or name. Clicking on currencies redirects user on the fourth page.
4. *Fourth page* is locked, being only opened programmatically. It shows detailed information about the chosen currency.
5. *Fifth page* is locked, being only opened programmatically. It is supposed to show detailed information about the chosen market. *(not implemented yet)*
6. *Sixth page* is supposed to show additional functionality, such as converting currencies into each other, changing settings of the program etc. *(not implemented yet)*

## Project Structure
1. *Classes.cs* contains all user classes needed for the project.
2. *Model.cs* contains connection to the data (API).
3. *View.xaml.cs* contains initialization of the interface.
4. *View.xaml* contains interface structure and connection to ViewModel variables and methods.
5. *ViewModel.cs* contains all needed connection between View and Model.
6. *res/* contains pictures for the project.
7. Other files were generated automatically.

## Additional
By default currency prices are shown in UAH. It is not yet possible to change it using the interface.
Considering amount of information on the fourth page, there is a possibility to scroll the page (other pages do not have this ability).
The code contains necessary comments for methods purpose etc.
All main operations have debug output to follow the process and look into possible bugs.
