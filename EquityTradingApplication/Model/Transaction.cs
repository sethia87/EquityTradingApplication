namespace EquityTradingApplication.Model;

internal class Transaction
{
    public int TransactionID { get; set; }
    public int TradeID { get; set; }
    public int Version { get; set; }
    public string SecurityCode { get; set; }
    public int Quantity { get; set; }
    public string Action { get; set; } // INSERT, UPDATE, CANCEL
    public string BuySell { get; set; } // Buy, Sell
}
