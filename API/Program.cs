using System.Text;
using API.Data.Context;
using API.Data.Entities;
using API.Data.Identity;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// constant
const string policy = "defaultPolicy";

// Add services to the container.

builder.Services.AddControllers();

// Add Services
builder.Services.AddTransient<IAuthorizationService, AuthorizationService>();
builder.Services.AddSingleton<ITimeProviderService, TimeProviderService>();
builder.Services.AddTransient<ITerminalService, TerminalService>();
builder.Services.AddTransient<ITableauService, TableauService>();

// Add DbContext
builder.Services.AddDbContext<ProjectContext>(
     opt =>
     {
         opt.UseSqlServer(
         connectionString: builder.Configuration.GetConnectionString("Default"));
     });

builder.Services.AddIdentity<User, IdentityRole>(Extensions.IdentityOptionsSetter)
    .AddEntityFrameworkStores<ProjectContext>()
    .AddDefaultTokenProviders();

// Add MediatR (default lifetime is transient)
builder.Services.AddMediatR(opt =>
    opt.RegisterServicesFromAssembly(typeof(API.Mediator.ApiMediator).Assembly));

builder.Services.AddSwaggerConfigurations();


var secret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("No JWT Secret in appsettings.json");

// Add authentication (JWT Baerer)
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

// Add cross origin ressource sharing
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(policy, p =>
    {
        p.AllowAnyHeader();
        p.AllowAnyMethod();
        p.AllowAnyOrigin();
        //p.WithOrigins( "google.com" );
    });
});

var app = builder.Build();

app.SeedDatabase();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt => opt.DocumentTitle = "Debugger Station");
}

app.UseHttpsRedirection();

// Use cross orgin ressource sharing
app.UseCors(policy);

// First Authentication
app.UseAuthentication();
// Then Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

