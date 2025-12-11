using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsManagement.Data.EntityConfigurations
{
    internal class AdvertisementTypeConfiguration : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder.ToTable("Advertisements");

            builder.Property(c => c.Title)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(120);

            builder.HasIndex(c => c.Title);

            builder.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(c => c.Number)
                .HasColumnName("Number")
                .IsRequired();

            builder.HasIndex(c => new { c.UserId, c.Number })
                .IsUnique();

            builder.Property(c => c.Rating)
                .HasColumnName("Rating")
                .HasColumnType("decimal(2,1)")
                .HasDefaultValue(0);

            builder.Property(c => c.CreatedAt)
                .HasColumnName("Date_of_creation")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(c => c.ExpiresAt)
                .HasColumnName("Expiration_date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.HasMany(a => a.Images)
                .WithOne(i => i.Advertisement)
                .HasForeignKey(i => i.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Comments)
                .WithOne(c => c.Advertisement)
                .HasForeignKey(c => c.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.User)
                .WithMany(u => u.Advertisements)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
