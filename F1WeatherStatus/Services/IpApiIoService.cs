using System.Text.Json;
using F1WeatherStatus.Models;

namespace F1WeatherStatus.Services;

/// <summary>
/// Localização pela ip-api.io. A API exige chave paga: preencha <see cref="ChaveApi"/>
/// para usá-la. Sem chave, o serviço falha de propósito e o app usa a reserva (ipwho.is).
/// </summary>
public sealed class IpApiIoService : ApiServiceBase, ILocalizacaoService
{
    private const string ChaveApi = "";

    public IpApiIoService(HttpClient http) : base(http) { }

    public override string NomeApi => "ip-api.io";

    public async Task<Localizacao> ObterLocalizacaoAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ChaveApi))
            throw new ApiException("ip-api.io: chave de API não configurada.");

        using var doc = await GetDocumentoAsync(
            $"https://ip-api.io/json?api_key={Uri.EscapeDataString(ChaveApi)}", ct);

        var raiz = doc.RootElement;
        // Alguns planos devolvem os dados dentro de "location"; outros, na raiz.
        var local = raiz.TryGetProperty("location", out var l) && l.ValueKind == JsonValueKind.Object ? l : raiz;

        var latitude = Numero(local, "latitude");
        var longitude = Numero(local, "longitude");
        if (latitude is null || longitude is null)
            throw new ApiException("ip-api.io não informou as coordenadas.");

        return new Localizacao
        {
            Ip = Texto(raiz, "ip"),
            Cidade = Texto(local, "city"),
            Regiao = Texto(local, "region_name", "region"),
            Pais = Texto(local, "country_name", "country"),
            CodigoPais = Texto(local, "country_code"),
            Latitude = latitude.Value,
            Longitude = longitude.Value,
            FusoHorario = Texto(local, "time_zone", "timezone"),
            Provedor = Texto(raiz, "organisation", "isp", "org")
        };
    }

    private static string Texto(JsonElement e, params string[] nomes)
    {
        foreach (var nome in nomes)
            if (e.TryGetProperty(nome, out var v) && v.ValueKind == JsonValueKind.String)
                return v.GetString() ?? string.Empty;
        return string.Empty;
    }

    private static double? Numero(JsonElement e, string nome) =>
        e.TryGetProperty(nome, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : null;
}
