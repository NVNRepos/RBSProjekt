using Client;
using Client.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.AddHttpConfigurations();

// Currently only Sessionstorage is suppported
builder.AddStorage(UserStorageType.Session);
builder.AddServices(UserStorageType.Session);

//From microsoft template to AuthenticationStateProvider
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
