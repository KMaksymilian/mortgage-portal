using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Models {

    [Owned]
    public class MoneyModel {

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "varchar(3)")]
        public string CurrencyCode { get; set; } = "PLN";


        public MoneyModel(decimal amount = 0, string currencyCode = "PLN") {
            Amount = amount;
            CurrencyCode = currencyCode;
        }

        public override string ToString() => $"{Amount:N2} {CurrencyCode}";
    }
}
