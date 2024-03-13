using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Client.Authorization;
using Client.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Client.Extensions
{
    public static class Extension
    {

        public static void AddHttpConfigurations(this WebAssemblyHostBuilder builder){
            builder.Services.AddTransient<AuthenticationHandler>();
            builder.Services.AddHttpClient( ClientDefaults.SERVER_API ).ConfigureHttpClient(
                c => c.BaseAddress = new Uri( builder.Configuration[ClientDefaults.SERVER_URL] ?? string.Empty ) )
                .AddHttpMessageHandler<AuthenticationHandler>();    
        }

        public static void AddStoragedServices(this WebAssemblyHostBuilder builder, UserStorageType storageType){
            switch (storageType)
            {
                case UserStorageType.Session:
                    builder.Services.AddBlazoredSessionStorageAsSingleton();
                    builder.Services.AddScoped<IUserManager, UserSessionStorageManager>();
                    builder.Services.AddSingleton<AuthenticationStateProvider, SessionAuthenticationStateProvider>();
                    break;
                case UserStorageType.Local:
                    builder.Services.AddBlazoredLocalStorageAsSingleton();
                    builder.Services.AddScoped<IUserManager, UserLocalStorageManager>();
                    builder.Services.AddSingleton<AuthenticationStateProvider, LocalAuthenticationStateProvider>();
                    break;
                default: throw new System.Diagnostics.UnreachableException();
            }
        }

        public static void AddServices(this WebAssemblyHostBuilder builder){
            builder.Services.AddScoped<ToastNotification>();
            builder.Services.AddTransient<IStampService, StampService>();
            builder.Services.AddTransient<ITableauService, TableauService>();
        }

        /// <summary>
        /// Sets an item in the client session storage
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="sessionStorageService">Session storage</param>
        /// <param name="key">Session storage key</param>
        /// <param name="item">Item</param>
        /// <returns>Task</returns>
        public static async Task SaveItemEncryptedAsync<T>(this ISessionStorageService sessionStorageService, string key, T item)
        {
            string itemJson = JsonSerializer.Serialize(item);
            var itemJsonByte = Encoding.UTF8.GetBytes(itemJson);
            var base64Json = Convert.ToBase64String(itemJsonByte);
            await sessionStorageService.SetItemAsync(key, base64Json);
        }

        /// <summary>
        /// Read an item for client session storage
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="sessionStorageService">Session storage</param>
        /// <param name="key">Session storage key</param>
        /// <returns>Task with result <typeparamref name="T"/>?</returns>
        public static async Task<T?> ReadEncryptedItemAsync<T>(this ISessionStorageService sessionStorageService, string key)
        {
            string base64Json = await sessionStorageService.GetItemAsync<string>(key);
            var itemJsonBytes = Convert.FromBase64String(base64Json);
            var itemJson = Encoding.UTF8.GetString(itemJsonBytes);
            var item = JsonSerializer.Deserialize<T>(itemJson);
            return item;
        }

        /// <summary>
        /// Sets an item in the client session storage
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="sessionStorageService">Session storage</param>
        /// <param name="key">Session storage key</param>
        /// <param name="item">Item</param>
        /// <returns>Task</returns>
        public static async Task SaveItemEncryptedAsync<T>(this ILocalStorageService sessionStorageService, string key, T item)
        {
            string itemJson = JsonSerializer.Serialize(item);
            var itemJsonByte = Encoding.UTF8.GetBytes(itemJson);
            var base64Json = Convert.ToBase64String(itemJsonByte);
            await sessionStorageService.SetItemAsync(key, base64Json);
        }

        /// <summary>
        /// Read an item for client session storage
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="sessionStorageService">Session storage</param>
        /// <param name="key">Session storage key</param>
        /// <returns>Task with result <typeparamref name="T"/>?</returns>
        public static async Task<T?> ReadEncryptedItemAsync<T>(this ILocalStorageService sessionStorageService, string key)
        {
            string? base64Json = await sessionStorageService.GetItemAsync<string>(key);
            var itemJsonBytes = Convert.FromBase64String(base64Json ?? string.Empty);
            var itemJson = Encoding.UTF8.GetString(itemJsonBytes);
            var item = JsonSerializer.Deserialize<T>(itemJson);
            return item;
        }

        public static string GetDisplayName(this ClaimsPrincipal principal)
            => principal.Claims.First(c => c.Type == ClaimTypes.UserData)?.Value ?? (principal.Identity?.Name ?? string.Empty);

        public static HttpClient CreateAPIClient(this IHttpClientFactory factory)
            => factory.CreateClient(ClientDefaults.SERVER_API);

        public static UserStorageType UserStorageTypeFromInt(int type)
            => type switch{
                1 => UserStorageType.Local,
                _ => UserStorageType.Session
            };
    }

    public enum UserStorageType{
        Session,
        Local
    }
}
