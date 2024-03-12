using System.Text;
using API.Data.Context;
using API.Data.Entities;
using API.Data.Identity;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class Extensions
    {

        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAuthorizationService, AuthorizationService>();
            builder.Services.AddSingleton<ITimeProviderService, TimeProviderService>();
            builder.Services.AddTransient<ITerminalService, TerminalService>();
            builder.Services.AddTransient<ITableauService, TableauService>();
        }

        public static void AddDbContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ProjectContext>(
                opt =>
            {
                opt.UseSqlServer(
                connectionString: builder.Configuration.GetConnectionString("Default"));
            });
        }

        public static void AddIdentity(this WebApplicationBuilder builder){
            builder.Services.AddIdentity<User, IdentityRole>(IdentityOptionsSetter)
            .AddEntityFrameworkStores<ProjectContext>()
            .AddDefaultTokenProviders();
        }


        public static void AddAuthentication(this WebApplicationBuilder builder, string secret){
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = new TimeSpan(0, 0, 5)
                };
            });
        }

        public static void AddSwaggerConfigurations(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
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

        private static void IdentityOptionsSetter(IdentityOptions options)
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
                Email = $"{noel.Name}@{noel.FirstName!}.de",
                Employee = noel,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            User chantalUser = new User
            {
                UserName = $"{chantal.FirstName!} {chantal.Name}",
                Email = $"{chantal.Name}@{chantal.FirstName!}.de",
                Employee = chantal,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            User danielUser = new User
            {
                UserName = $"{daniel.FirstName!} {daniel.Name}",
                Email = $"{daniel.Name}@{daniel.FirstName!}.de",
                Employee = daniel,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            Task<IdentityResult> noelResult = userManager.CreateAsync(noelUser, noel.Password);
            noelResult.GetAwaiter().GetResult();

            Task<IdentityResult> chantalResult = userManager.CreateAsync(chantalUser, chantal.Password);
            chantalResult.GetAwaiter().GetResult();

            Task<IdentityResult> danielResult = userManager.CreateAsync(danielUser, daniel.Password);
            danielResult.GetAwaiter().GetResult();
        }
    }
}
