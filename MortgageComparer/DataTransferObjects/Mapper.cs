using MortgageComparer.Entities;

namespace MortgageComparer.DataTransferObjects {
    public static class Mapper {
        public static OfferDto ToDto(this OfferEntity entity) {
            if (entity == null) { return null!; }

            return new OfferDto {
                // Mapowanie Id i podstawowych wartości
                OfferId = entity.Id,
                Percentage = entity.Percentage,
                StatusDescription = entity.StatusDescription,
                DocumentLink = entity.DocumentLink,
                DocumentLinkValidDate = entity.ContractLinkValidDate,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                ApprovedBy = entity.SingedBy, // Mapowanie na ApprovedBy zgodnie z Twoim DTO


                QuoteDto = new QuoteDto {

                },

                // BankName często bierzemy z relacji Quote lub stałej wartości
                BankName = "OurBank"
            };
        }

        public static OfferEntity ToEntity(this OfferDto dto, int userId) {
            if (dto == null) { return null!; }

            throw new NotImplementedException("Mapping from OfferDto to OfferEntity is not implemented yet.");

        }
    }
}

