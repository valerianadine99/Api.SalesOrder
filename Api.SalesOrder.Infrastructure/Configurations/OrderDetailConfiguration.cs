using Api.SalesOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.SalesOrder.Infrastructure.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.ProductCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Quantity)
                .IsRequired();

            builder.Property(d => d.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(d => d.Subtotal)
                .HasPrecision(18, 2);

            builder.Property(d => d.CreatedBy)
                .HasMaxLength(100);

            builder.Property(d => d.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(d => d.ProductCode)
                .HasDatabaseName("IX_OrderDetails_ProductCode");
        }
    }
}
