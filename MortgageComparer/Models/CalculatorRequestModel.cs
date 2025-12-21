using MortgageComparer.Data;

namespace MortgageComparer.Models;

public class CalculatorRequestModel{
    public MoneyModel RequestedAmount { get; set; }
    public int InstalmentNumber { get; set; }

    public CalculatorRequestModel(MoneyModel requestedAmount, int instalmentNumber)
    {
        this.RequestedAmount = requestedAmount;
        this.InstalmentNumber = instalmentNumber;
    }
}

