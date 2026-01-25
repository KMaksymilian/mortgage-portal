using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MortgageComparer.Entities;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Data.Configurations {
    public class OfferConfiguration : IEntityTypeConfiguration<OfferEntity> {
        public void Configure(EntityTypeBuilder<OfferEntity> builder) {
 
            builder.HasIndex(o => o.UserId)
                   .HasDatabaseName("IX_Offers_UserId");

            builder.HasIndex(o => o.UpdatedAt)
                   .HasDatabaseName("IX_Offers_UpdateDate");

            builder.HasIndex(o => o.CreatedAt)
                   .HasDatabaseName("IX_Offers_CreateDate");

            builder.HasIndex(o => o.Status)
                   .HasDatabaseName("IX_Offers_Status");

 
            builder.OwnsOne(o => o.Quote);
 


            builder.Property(o => o.Status)
                   .HasConversion<int>(); 

            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}