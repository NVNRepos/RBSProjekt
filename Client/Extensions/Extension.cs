using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace Client.Extensions
{
    public static class Extension
    {

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
    }
}
