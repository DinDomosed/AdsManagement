using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdsManagement.Data.EntityConfigurations
{
    internal class CommentTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasIndex(c => new { c.AdvertisementId, c.CreatedAt });

            builder.HasIndex(c => c.UserId);

            builder.Property(c => c.Text)
                .HasColumnName("Text")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(c => c.Estimation)
                .HasColumnName("Estimation")
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnName("Date_of_creation")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.HasOne(c => c.Advertisement)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
