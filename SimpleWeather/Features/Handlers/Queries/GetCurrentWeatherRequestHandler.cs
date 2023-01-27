using MediatR;
using Microsoft.EntityFrameworkCore;
using SimpleWeather.Core;
using SimpleWeather.Data;
using SimpleWeather.Features.Requests.Queries;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;

namespace SimpleWeather.Features.Handlers.Queries
{
    public class GetCurrentWeatherRequestHandler : IRequestHandler<GetCurrentWeatherRequest, ResultResponse<RootObject>>
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IUserAccessor _userAccessor;
        private readonly DataContext _context;

        public GetCurrentWeatherRequestHandler(IWeatherRepository weatherRepository, IUserAccessor userAccessor, DataContext context)
        {
            _weatherRepository = weatherRepository;
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<ResultResponse<RootObject>> Handle(GetCurrentWeatherRequest request, CancellationToken cancellationToken)
        {
            var va = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            var query = await _weatherRepository.GetWeatherForecast(request.Location);

            if (query is null) return ResultResponse<RootObject>.Failure("Not Found");

            return ResultResponse<RootObject>.Success(query);
        }
    }
}
