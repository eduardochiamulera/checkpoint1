using Microsoft.AspNetCore.Identity;

namespace Cursos.Data;

public class SeedData
{
    public static async Task SeedAsync(IServiceProvider services, string password)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = ["Admin", "Instructor", "Student"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@cursos.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            await userManager.CreateAsync(admin, password);
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}