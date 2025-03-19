using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace demo.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            // check if exist
            if (context.Events.Any())
            {
                return;   
            }

            // add some examples
            context.Events.AddRange(
                new Event
                {
                    Title = "Technical semniar",
                    Description = "Discuss web3 development",
                    Date = DateTime.Now.AddDays(7),
                    Time = DateTime.Now.Date.AddHours(14),
                    Location = "101"
                },
                new Event
                {
                    Title = "Team building activity",
                    Description = "Outdoor training and team work activities",
                    Date = DateTime.Now.AddDays(14),
                    Time = DateTime.Now.Date.AddHours(9),
                    Location = "city park"
                },
                new Event
                {
                    Title = "Project kick-off",
                    Description = "new project kickoff meeting",
                    Date = DateTime.Now.AddDays(3),
                    Time = DateTime.Now.Date.AddHours(10),
                    Location = "102"
                }
            );

            // create demo user
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            if (userManager.FindByEmailAsync(adminUser.Email).Result == null)
            {
                await userManager.CreateAsync(adminUser, "Admin123!");
            }

            await context.SaveChangesAsync();
        }
    }
}