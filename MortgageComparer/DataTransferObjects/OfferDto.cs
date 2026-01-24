using MortgageComparer.Entities;

namespace MortgageComparer.DataTransferObjects {
    public class OfferDto {
        public int Id { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public string? Currencycode { get; set; }
        public bool IsContractSigned { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }

        public OfferDto(BasicOfferEntity offerEntity) {
            Id = offerEntity.Id;
            LoanAmount = offerEntity.Quote.TotalAmountToPay.Amount;
            Currencycode = offerEntity.Quote.TotalAmountToPay.CurrencyCode;
            MonthlyInstallment = 0;
            IsContractSigned = offerEntity.SingedBy != null;
            CreateDate = offerEntity.CreatedAt;
            Status = offerEntity.Status.ToString();
        }

        public OfferDto() { }
    }

}
