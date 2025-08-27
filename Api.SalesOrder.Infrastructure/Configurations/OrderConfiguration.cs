using Api.SalesOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.SalesOrder.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.CustomerEmail)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.Total)
                .HasPrecision(18, 2);

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.CreatedBy)
                .HasMaxLength(100);

            builder.Property(o => o.UpdatedBy)
                .HasMaxLength(100);

            builder.HasMany(o => o.OrderDetails)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => new { o.CustomerName, o.OrderDate })
                .HasDatabaseName("IX_Orders_Customer_Date");
        }
    }
}
