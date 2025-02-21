using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using EquityTradingApplication.Command;
using EquityTradingApplication.Model;
using Transaction = EquityTradingApplication.Model.Transaction;

namespace EquityTradingApplication.ViewModel;

internal class EquityTradingViewModel : EquityTradingViewModelBase 
{
    #region Fields
    private ObservableCollection<Position> _positions;
    private ObservableCollection<Transaction> _transactions;
    private int _tradeID;
    private int _version;
    private string _securityCode;
    private int _quantity;
    private string _action;
    private string _buySell;
    #endregion

    #region Properties
    public ObservableCollection<Position> Positions
    {
        get { return _positions; }
        set { _positions = value; OnPropertyChanged(); }
    }
    public ObservableCollection<Transaction> Transactions
    {
        get { return _transactions; }
        set { _transactions = value; OnPropertyChanged(); }
    }
    public List<string> BuySellList { get; }

    public int TradeID
    {
        get => _tradeID;
        set { _tradeID = value; OnPropertyChanged(); }
    }

    public int Version
    {
        get => _version;
        set { _version = value; OnPropertyChanged(); }
    }

    public string SecurityCode
    {
        get => _securityCode;
        set { _securityCode = value; OnPropertyChanged(); }
    }

    public int Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); }
    }

    public string Action
    {
        get => _action;
        set { _action = value; OnPropertyChanged(); }
    }

    public string BuySell
    {
        get => _buySell;
        set { _buySell = value; OnPropertyChanged(); }
    }
    public ICommand SubmitTransactionCommand { get; set; }
    public List<string> ActionList { get; }
    #endregion

    #region Constructor
    public EquityTradingViewModel()
    {
        BuySellList = ["Buy", "Sell"];
        ActionList = ["INSERT", "UPDATE", "CANCEL"];

        Positions = new ObservableCollection<Position>();
        Transactions = new ObservableCollection<Transaction>();
        SubmitTransactionCommand = new RelayCommand(SubmitTransaction);
        
        LoadData();
    }
    #endregion

    #region Methods
    private async Task LoadData()
    {
        using var client = CreateClient();
        var response = await client.GetAsync(Constants.RequestPositionUri);

        if (response.IsSuccessStatusCode)
        {
            var positions = await response.Content.ReadFromJsonAsync<ObservableCollection<Position>>();
            if (positions != null)
            {
                Positions.Clear();
                foreach (var position in positions)
                {
                    Positions.Add(position);
                }
            }
        }
        else
        {
            MessageBox.Show("Error in retreiving position data");
        }

        response = await client.GetAsync(Constants.RequestTransactionUri);
        if (response.IsSuccessStatusCode)
        {
            var transactions = await response.Content.ReadFromJsonAsync<ObservableCollection<Transaction>>();
            if (transactions != null)
            {
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
        }
        else
        {
            MessageBox.Show("Error in retreiving transaction data");
        }
    }

    private void SubmitTransaction(Object obj)
    {
        var transaction = new Transaction
        {
            TradeID = this.TradeID,
            Version = this.Version,
            SecurityCode = this.SecurityCode,
            Quantity = this.Quantity,
            Action = this.Action,
            BuySell = this.BuySell
        };
        if (transaction != null)
        {
            UpdatePositions(transaction);
        }
    }

    private async Task UpdatePositions(Transaction transaction)
    {
        var position = Positions.FirstOrDefault(p => p.SecurityCode == transaction.SecurityCode);
        if (position == null)
        {
            Positions.Add(new Position { SecurityCode = transaction.SecurityCode, Quantity = transaction.Quantity });
        }
        else
        {
            position.Quantity += (transaction.BuySell == "Buy" ? transaction.Quantity : -transaction.Quantity);
        }

        using var client = CreateClient();
        var response = await client.PostAsJsonAsync(Constants.RequestTransactionUri, transaction);

        if (response.IsSuccessStatusCode)
        {
            LoadData();
            MessageBox.Show("Changes saved successfully");
        }
        else
        {
            MessageBox.Show("Failed to save changes");
        }
        Positions = new ObservableCollection<Position>(Positions);
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(Constants.UriString);
        return client;
    }

    #endregion
}
