using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Eridian_Websites.Models.Catalogs;
using Eridian_Websites.Models.Products;
using Eridian_Websites.Models.Files;


namespace Eridian_Websites.Data
{
    public class NucleusDbContext : DbContext
    {
        public NucleusDbContext(DbContextOptions<NucleusDbContext> options) : base(options)
        {
        }

        // Add DbSet for each table you want to work with
        public DbSet<Product> Products { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<ChannelListing> ChannelListings { get; set; }
        public DbSet<CatalogProduct> CatalogProducts { get; set; }
        public DbSet<Models.Files.File> Files { get; set; }
        public DbSet<FileLookupType> FileLookupTypes { get; set; }
        public DbSet<CatalogLookupType> CatalogLookupTypes { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Products", "Products");
            modelBuilder.Entity<Catalog>().ToTable("Catalogs", "Catalogs");
            modelBuilder.Entity<ChannelListing>().ToTable("ChannelListings", "Catalogs");
            modelBuilder.Entity<Models.Files.File>().ToTable("Files", "Files");
            modelBuilder.Entity<FileLookupType>().ToTable("LookupTypes", "Files");
            modelBuilder.Entity<CatalogLookupType>().ToTable("LookupTypes", "Catalogs");

            modelBuilder.Entity<ProductImages>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("View_CatalogEcommerceImages", "Files");
            });

            modelBuilder.Entity<CatalogProduct>(entity =>
            {
                entity.ToTable("Products", "Catalogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CatalogId).HasColumnName("CatalogID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.IsActive).HasColumnName("isActive");
            });

        }
    }
}
