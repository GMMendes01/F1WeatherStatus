using F1WeatherStatus.Models;
using F1WeatherStatus.Services.Dto;

namespace F1WeatherStatus.Services;

/// <summary>Localização pela ipwho.is (gratuita, sem chave). Usada como reserva.</summary>
public sealed class IpWhoIsService : ApiServiceBase, ILocalizacaoService
{
    public IpWhoIsService(HttpClient http) : base(http) { }

    public override string NomeApi => "ipwho.is";

    public async Task<Localizacao> ObterLocalizacaoAsync(CancellationToken ct = default)
    {
        var r = await GetAsync<IpWhoIsResposta>("https://ipwho.is/", ct);

        if (!r.Success)
            throw new ApiException($"ipwho.is: {r.Message ?? "consulta não realizada"}.");

        return new Localizacao
        {
            Ip = r.Ip ?? string.Empty,
            Cidade = r.City ?? string.Empty,
            Regiao = r.Region ?? string.Empty,
            Pais = r.Country ?? string.Empty,
            CodigoPais = r.CountryCode ?? string.Empty,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            FusoHorario = r.Timezone?.Id ?? string.Empty,
            Provedor = r.Connection?.Isp ?? string.Empty
        };
    }
}
