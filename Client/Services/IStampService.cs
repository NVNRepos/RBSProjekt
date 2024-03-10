using Domain;

namespace Client.Services {
    public interface IStampService {

        public Task<Domain.Terminal.StampResponseModel> Stamp( StampType stampType );

        public Task<Domain.Terminal.StampResponseModel> Stamp(uint emplyoeeId, StampType stampType );

    }
}
