namespace MortgageComparer.Entities {
    public class OfferToBankEntity : BasicEntity{
        public int OfferId { get; set; }
        public int UserId { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string StatusDescription { get; set; } = string.Empty;


    }
}
