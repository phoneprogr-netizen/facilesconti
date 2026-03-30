using FacileSconti.Domain.Entities;
using FacileSconti.Domain.Enums;
using FacileSconti.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FacileSconti.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db)
    {
        foreach (var role in new[] { "Admin", "Customer", "EndUser" })
        {
            if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
        }

        var admin = await EnsureUserAsync(
            userManager,
            "admin@facilesconti.local",
            "Admin123!",
            "System",
            "Admin",
            UserType.Admin,
            "Admin");

        var customerUser = await EnsureUserAsync(
            userManager,
            "cliente.demo@facilesconti.local",
            "Cliente123!",
            "Mario",
            "Rossi",
            UserType.Customer,
            "Customer");

        await EnsureUserAsync(
            userManager,
            "utente.demo@facilesconti.local",
            "Utente123!",
            "Luca",
            "Verdi",
            UserType.EndUser,
            "EndUser");

        var business = await db.CustomerBusinesses.FirstOrDefaultAsync(x => x.OwnerUserId == customerUser.Id);
        if (business is null)
        {
            business = new CustomerBusiness
            {
                Name = "Pizzeria Bella Napoli",
                VatNumber = "IT12345678901",
                FiscalCode = "BLLNPL80A01H501X",
                Email = customerUser.Email ?? "cliente.demo@facilesconti.local",
                Phone = "+39 06 1234567",
                Address = "Via Roma 10",
                City = "Roma",
                Province = "RM",
                Description = "Pizzeria artigianale con forno a legna.",
                OwnerUserId = customerUser.Id,
                CreatedBy = admin.Id
            };
            db.CustomerBusinesses.Add(business);
            await db.SaveChangesAsync();
        }

        var category = await db.CouponCategories.OrderBy(c => c.Id).FirstOrDefaultAsync();
        if (category is null) return;

        if (!await db.Coupons.AnyAsync(c => c.CustomerBusinessId == business.Id && c.Slug == "pizza-2x1-serale"))
        {
            db.Coupons.AddRange(
                new Coupon
                {
                    CustomerBusinessId = business.Id,
                    CouponCategoryId = category.Id,
                    Title = "Pizza 2x1 serale",
                    Slug = "pizza-2x1-serale",
                    ShortDescription = "Tutte le sere, seconda pizza omaggio.",
                    FullDescription = "Valido da lunedì a giovedì su prenotazione.",
                    OriginalPrice = 24,
                    DiscountedPrice = 12,
                    CouponType = CouponType.Free,
                    Status = CouponStatus.Active,
                    ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
                    ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                    IsFeatured = true,
                    IsBoostedInHome = false,
                    CreatedBy = admin.Id
                },
                new Coupon
                {
                    CustomerBusinessId = business.Id,
                    CouponCategoryId = category.Id,
                    Title = "Menu pranzo -20%",
                    Slug = "menu-pranzo-20",
                    ShortDescription = "Sconto immediato sul menu pranzo.",
                    FullDescription = "Valido tutti i giorni feriali dalle 12:00 alle 15:00.",
                    OriginalPrice = 20,
                    DiscountedPrice = 16,
                    CouponType = CouponType.Free,
                    Status = CouponStatus.Active,
                    ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)),
                    ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                    IsFeatured = true,
                    IsBoostedInHome = true,
                    CreatedBy = admin.Id
                });

            await db.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string firstName,
        string lastName,
        UserType userType,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserType = userType,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Impossibile creare l'utente demo '{email}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role)) await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
