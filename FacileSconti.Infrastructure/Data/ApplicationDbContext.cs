using FacileSconti.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<CustomerBusiness> CustomerBusinesses => Set<CustomerBusiness>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<CustomerContract> CustomerContracts => Set<CustomerContract>();
    public DbSet<ContractFeature> ContractFeatures => Set<ContractFeature>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponCategory> CouponCategories => Set<CouponCategory>();
    public DbSet<CouponImage> CouponImages => Set<CouponImage>();
    public DbSet<CouponDownload> CouponDownloads => Set<CouponDownload>();
    public DbSet<CouponView> CouponViews => Set<CouponView>();
    public DbSet<CouponRedemption> CouponRedemptions => Set<CouponRedemption>();
    public DbSet<CouponPaymentConfig> CouponPaymentConfigs => Set<CouponPaymentConfig>();
    public DbSet<HomeBoostPlan> HomeBoostPlans => Set<HomeBoostPlan>();
    public DbSet<CustomerBoostActivation> CustomerBoostActivations => Set<CustomerBoostActivation>();
    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();
    public DbSet<PublicPage> PublicPages => Set<PublicPage>();
    public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
