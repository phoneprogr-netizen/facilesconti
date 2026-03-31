using FacileSconti.Application.Interfaces;
using FacileSconti.Domain.Entities;
using FacileSconti.Infrastructure.Data;
using FacileSconti.Infrastructure.Options;
using FacileSconti.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FacileSconti.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<TurboSmtpOptions>(configuration.GetSection(TurboSmtpOptions.SectionName));
        services.AddHttpClient(nameof(TurboSmtpEmailService));

        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IEmailService, TurboSmtpEmailService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddSingleton<IQrCodeService, QrCodeService>();

        return services;
    }
}
