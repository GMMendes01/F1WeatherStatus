using F1WeatherStatus.Models;
using F1WeatherStatus.Services.Dto;

namespace F1WeatherStatus.Services;

/// <summary>
/// Converte nomes de lugares em coordenadas pela Open-Meteo Geocoding (gratuita, sem chave).
/// É a ponte entre as APIs: IBGE/OpenF1 trazem nomes; o clima precisa de coordenadas.
/// </summary>
public sealed class GeocodingService : ApiServiceBase, IGeocodingService
{
    public GeocodingService(HttpClient http) : base(http) { }

    public override string NomeApi => "Open-Meteo Geocoding";

    public async Task<Lugar?> BuscarAsync(string nome, string? codigoPais = null, string? regiao = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return null;

        var url = "https://geocoding-api.open-meteo.com/v1/search" +
                  $"?name={Uri.EscapeDataString(nome)}&count=10&language=pt&format=json";
        if (!string.IsNullOrWhiteSpace(codigoPais))
            url += $"&countryCode={Uri.EscapeDataString(codigoPais)}";

        var resposta = await GetAsync<GeocodingResposta>(url, ct);
        var itens = resposta.Results ?? new List<GeocodingItem>();
        if (itens.Count == 0)
            return null;

        // Vários lugares podem ter o mesmo nome: prefere o que está no estado/região pedido.
        var escolhido = (string.IsNullOrWhiteSpace(regiao)
                            ? null
                            : itens.FirstOrDefault(i => Texto.Igual(i.Admin1, regiao)))
                        ?? itens[0];

        return new Lugar
        {
            Nome = escolhido.Name ?? nome,
            Regiao = escolhido.Admin1 ?? string.Empty,
            Pais = escolhido.Country ?? string.Empty,
            Latitude = escolhido.Latitude,
            Longitude = escolhido.Longitude
        };
    }
}
