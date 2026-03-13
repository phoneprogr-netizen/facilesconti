using FacileSconti.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FacileSconti.Infrastructure.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ShortDescription).HasMaxLength(400).IsRequired();
        builder.HasOne(x => x.PaymentConfig).WithOne(x => x.Coupon).HasForeignKey<CouponPaymentConfig>(x => x.CouponId);
    }
}

public class CustomerBusinessConfiguration : IEntityTypeConfiguration<CustomerBusiness>
{
    public void Configure(EntityTypeBuilder<CustomerBusiness> builder)
    {
        builder.ToTable("CustomerBusinesses");
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.VatNumber).IsUnique();
        builder.HasOne(x => x.OwnerUser).WithOne(x => x.CustomerBusiness).HasForeignKey<CustomerBusiness>(x => x.OwnerUserId);
    }
}

public class CustomerContractConfiguration : IEntityTypeConfiguration<CustomerContract>
{
    public void Configure(EntityTypeBuilder<CustomerContract> builder)
    {
        builder.ToTable("CustomerContracts");
        builder.Property(x => x.AgreedPrice).HasColumnType("decimal(18,2)");
        builder.HasOne(x => x.CustomerBusiness).WithMany(x => x.Contracts).HasForeignKey(x => x.CustomerBusinessId);
        builder.HasOne(x => x.SubscriptionPlan).WithMany(x => x.Contracts).HasForeignKey(x => x.SubscriptionPlanId);
    }
}

public class CouponDownloadConfiguration : IEntityTypeConfiguration<CouponDownload>
{
    public void Configure(EntityTypeBuilder<CouponDownload> builder)
    {
        builder.ToTable("CouponDownloads");
        builder.HasIndex(x => x.UniqueCode).IsUnique();
        builder.HasOne(x => x.EndUser).WithMany(x => x.CouponDownloads).HasForeignKey(x => x.EndUserId);
    }
}

public class PaymentRecordConfiguration : IEntityTypeConfiguration<PaymentRecord>
{
    public void Configure(EntityTypeBuilder<PaymentRecord> builder)
    {
        builder.ToTable("PaymentRecords");
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
    }
}

public class PublicPageConfiguration : IEntityTypeConfiguration<PublicPage>
{
    public void Configure(EntityTypeBuilder<PublicPage> builder)
    {
        builder.ToTable("PublicPages");
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
