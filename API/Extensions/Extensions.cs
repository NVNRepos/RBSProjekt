using API.Data.Context;
using API.Data.Entities;
using API.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class Extensions
    {

        public static void AddSwaggerConfigurations(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer [jwt]'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, [] } });

            });
        }

        public static void IdentityOptionsSetter(IdentityOptions options)
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 5;
            options.Password.RequiredUniqueChars = 0;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.AllowedUserNameCharacters += " ";
            options.User.RequireUniqueEmail = false;
        }

        public static void SeedDatabase(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProjectContext>();
            if (db is null || db.Employees.Any() || db.Departments.Any() || db.Stamps.Any() || db.Users.Any())
                return;

            Department support = new Department { Name = "Support" };
            Department entwicklung = new Department { Name = "Entwicklung" };

            Employee noel = new Employee
            {
                Name = "von Negri",
                Department = entwicklung,
                FirstName = "Noel",
                Password = "Password"
            };

            Employee chantal = new Employee
            {
                Name = "Kinzelmann",
                Department = entwicklung,
                FirstName = "Chantal",
                Password = "Password"
            };

            Employee daniel = new Employee
            {
                Name = "Vetter-Gindele",
                Department = support,
                FirstName = "Daniel",
                Password = "Password"
            };

            if (!db.Departments.Any())
                db.Departments.AddRange([support, entwicklung]);
            db.SaveChanges();

            if (!db.Employees.Any())
                db.Employees.AddRange([noel, chantal, daniel]);
            db.SaveChanges();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            User noelUser = new User
            {
                UserName = $"{noel.FirstName!} {noel.Name}",
                Email = "n.vonnegri@digital-zeit.de",
                Employee = noel,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            User chantalUser = new User
            {
                UserName = $"{chantal.FirstName!} {chantal.Name}",
                Email = "c.kinzelmann@digital-zeit.de",
                Employee = chantal,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            User danielUser = new User
            {
                UserName = $"{daniel.FirstName!} {daniel.Name}",
                Email = "d.vetter-gindele@digital-zeit.de",
                Employee = daniel,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            Task<IdentityResult> noelResult = userManager.CreateAsync(noelUser, noel.Password);
            noelResult.Wait();

            Task<IdentityResult> chantalResult = userManager.CreateAsync(chantalUser, chantal.Password);
            chantalResult.Wait();

            Task<IdentityResult> danielResult = userManager.CreateAsync(danielUser, daniel.Password);
            danielResult.Wait();
        }
    }
}
