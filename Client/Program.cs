﻿using Client;
using Client.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.RootComponents.Add<App>( "#app" );
builder.RootComponents.Add<HeadOutlet>( "head::after" );

builder.AddHttpConfigurations();

builder.AddStoragedServices(Extension.UserStorageTypeFromInt(
    builder.Configuration.GetValue("StorageType", 0)  
));

builder.AddServices();

//From microsoft template to AuthenticationStateProvider
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
