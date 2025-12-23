using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Models {

    [Owned]
    public class MoneyModel {

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "varchar(3)")]
        public string? CurrencyCode { get; set; }

        protected MoneyModel() { }

        public MoneyModel(decimal amount, string? currencyCode) {
            Amount = amount;
            CurrencyCode = currencyCode ?? "PLN";
        }

        public override string ToString() => $"{Amount:N2} {CurrencyCode}";
    }
}
