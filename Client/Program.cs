using Blazored.SessionStorage;
using Client;
using Client.Authentication;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddHttpClient( ClientDefaults.SERVER_API ).ConfigureHttpClient(
    c => c.BaseAddress = new Uri( builder.Configuration[ClientDefaults.SERVER_URL] ?? string.Empty ) )
    .AddHttpMessageHandler<AuthenticationHandler>();
builder.Services.AddBlazoredSessionStorageAsSingleton();
//DO NOT CHANGE THIS LINE. There are downcastings
builder.Services.AddSingleton<AuthenticationStateProvider, ClientAuthenticationStateProvider>();
builder.Services.AddScoped<IUserManager, UserSessionStorageManager>();

builder.Services.AddScoped<ToastNotification>();
builder.Services.AddTransient<IStampService, StampService>();
builder.Services.AddTransient<ITableauService, TableauService>();


//From microsoft template to AuthenticationStateProvider
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
