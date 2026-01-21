using FindMyMeds.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FindMyMeds.Infrastructure.Seed
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var users = new[]
            {
                new { Name = "Mhmd",  Email = "mhmd@gmail.com",  Password = "Mhmd@123" },
                new { Name = "Sara",  Email = "sara@gmail.com",  Password = "Sara@123" },
                new { Name = "Ali",   Email = "ali@gmail.com",   Password = "Ali@123" },
                new { Name = "Maya",  Email = "maya@gmail.com",  Password = "Maya@123" },
                new { Name = "Jawad", Email = "jawad@gmail.com", Password = "Jawad@123" }
            };

            foreach (var u in users)
            {
                var existingUser = await userManager.FindByEmailAsync(u.Email);
                if (existingUser != null)
                    continue;

                var user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, u.Password);
                if (!result.Succeeded)
                    continue;
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
