using System.Net.Http.Json;
using Client.Extensions;
using Domain;
using Domain.Terminal;

namespace Client.Services {
    public class StampService : IStampService {

        private readonly IHttpClientFactory _httpClientFactory;

        public StampService(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public async Task<StampResponseModel> Stamp( StampType stampType ) {
            var result = await _httpClientFactory.CreateAPIClient()
                .PostAsJsonAsync( $"{Routes.TERMINAL}/{Routes.STAMP}", stampType );

            return (await result.Content.ReadFromJsonAsync<StampResponseModel>()) ?? throw new InvalidDataException();

        }

        public async Task<StampResponseModel> Stamp(uint emplyoeeId, StampType stampType)
        {
            var result = await _httpClientFactory.CreateAPIClient()
                .PostAsJsonAsync($"{Routes.TERMINAL}/{Routes.STAMP}/{emplyoeeId}",stampType);
            return (await result.Content.ReadFromJsonAsync<StampResponseModel>()) ?? throw new InvalidDataException();
        }
    }
}
