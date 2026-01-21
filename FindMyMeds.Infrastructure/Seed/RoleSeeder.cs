using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FindMyMeds.Infrastructure.Seed
{
    public static class RoleSeeder
    {
        private static readonly string[] Roles =
        {
            "Admin",
            "Pharmacy",
            "User"
        };

        public static async Task SeedAsync(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
