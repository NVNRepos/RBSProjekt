using System.Net.Http.Json;
using Client.Extensions;
using Domain.Tableau;

namespace Client.Services
{
    public class TableauService : ITableauService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public TableauService(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public async Task<EmployeePresentCollection> GetEmployeePresentsAsync()
        {
            var result = await _httpClientFactory.CreateAPIClient()
                .GetAsync($"{Domain.Routes.TABLEAU}");

            if (!result.IsSuccessStatusCode || result.StatusCode == System.Net.HttpStatusCode.NoContent)
                return new EmployeePresentCollection();

            return await result.Content.ReadFromJsonAsync<EmployeePresentCollection>() ?? throw new InvalidDataException();

        }
    }
}
