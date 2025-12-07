using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Data.EntityConfigurations
{
    internal class AdImageTypeConfiguration : IEntityTypeConfiguration<AdvertisementImage>
    {
        public void Configure(EntityTypeBuilder<AdvertisementImage> builder)
        {
            builder.ToTable("Advertisement_Images");

            builder.Property(c => c.OriginalImagePath)
                .HasColumnName("Original_Image_Path")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.SmallImagePath)
                .HasColumnName("Small_Image_Path")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasOne(i => i.Advertisement)
                .WithMany(a => a.Images)
                .HasForeignKey(i => i.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
