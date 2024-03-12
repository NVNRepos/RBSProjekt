using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// constant
const string policy = "defaultPolicy";
var secret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("No JWT Secret in appsettings.json");

// Add services to the container.

builder.Services.AddControllers();

builder.AddServices();

builder.AddDbContext();

builder.AddIdentity();

// Add MediatR (default lifetime is transient)
builder.Services.AddMediatR(opt =>
    opt.RegisterServicesFromAssembly(typeof(API.Mediator.ApiMediator).Assembly));

builder.AddSwaggerConfigurations();


// Add authentication (JWT Baerer)
builder.AddAuthentication(secret);

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

