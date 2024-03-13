using System.Net.Http.Headers;
using Client.Services;
namespace Client.Authorization
{
    /// <summary>
    /// Like angular http interceptor 
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IUserManager _userManager;

        public AuthenticationHandler(IConfiguration configuration, IUserManager userManager)
            => (_configuration, _userManager) = (configuration, userManager);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var jwt = await _userManager.GetJwtAsync();
            bool isToServer = request.RequestUri?.AbsoluteUri.StartsWith(_configuration[ClientDefaults.SERVER_URL] ?? string.Empty) ?? false;
            //check
            if (isToServer && !string.IsNullOrEmpty(jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
