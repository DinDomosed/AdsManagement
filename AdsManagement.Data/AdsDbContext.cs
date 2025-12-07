using Microsoft.EntityFrameworkCore;
using AdsManagement.Domain.Models;
using AdsManagement.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AdsManagement.Data
{
    public class AdsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Advertisement> Advertisements { get; set; } = null!;
        public DbSet<AdvertisementImage> AdvertisementImages { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        public AdsDbContext(DbContextOptions<AdsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdsDbContext).Assembly);

            foreach(var entityTypeMetaData in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityTypeMetaData.ClrType;
                if (clrType == null)
                    continue;

                if(typeof(BaseEntity).IsAssignableFrom(clrType))
                {
                    var entityTypeBuilder = modelBuilder.Entity(clrType);

                    entityTypeBuilder.HasKey("Id");

                    entityTypeBuilder.Property("Id")
                        .HasColumnName("Id")
                        .HasColumnType("uuid")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("gen_random_uuid()");
                }
            }
        }
    }
}
