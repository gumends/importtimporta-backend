using Domain.Models.Frete;

namespace Application.Interfaces.Services;

public interface IFreteService
{
    Task<CotarFreteResponse> CotarAsync(CotarFreteRequest request, CancellationToken cancellationToken = default);
}
