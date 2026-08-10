using Microsoft.AspNetCore.Identity;
using PostControl.Web.Models;

namespace PostControl.Web.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Создаём роли
        if (!await roleManager.RoleExistsAsync("Employee"))
            await roleManager.CreateAsync(new IdentityRole("Employee"));

        if (!await roleManager.RoleExistsAsync("Guard"))
            await roleManager.CreateAsync(new IdentityRole("Guard"));

        // Тестовый сотрудник
        var employee = await userManager.FindByNameAsync("TEST001");
        if (employee == null)
        {
            employee = new ApplicationUser
            {
                UserName = "TEST001",
                Email = "employee@test.com",
                FullName = "Иванов Иван",
                CardId = "TEST001",
                AccessLevel = "employee",
                TimeWindows = "09:00-18:00"
            };
            await userManager.CreateAsync(employee, "Password123!");
            await userManager.AddToRoleAsync(employee, "Employee");
        }

        // Тестовый охранник
        var guard = await userManager.FindByNameAsync("GUARD001");
        if (guard == null)
        {
            guard = new ApplicationUser
            {
                UserName = "GUARD001",
                Email = "guard@test.com",
                FullName = "Охрана Петров",
                CardId = "GUARD001",
                AccessLevel = "guard",
                TimeWindows = "00:00-23:59"
            };
            await userManager.CreateAsync(guard, "Guard123!");
            await userManager.AddToRoleAsync(guard, "Guard");
        }
    }
}