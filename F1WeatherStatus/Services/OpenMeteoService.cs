using System.Globalization;
using F1WeatherStatus.Models;
using F1WeatherStatus.Services.Dto;

namespace F1WeatherStatus.Services;

/// <summary>Clima atual e previsão de 7 dias pela Open-Meteo (gratuita, sem chave).</summary>
public sealed class OpenMeteoService : ApiServiceBase, IClimaService
{
    public OpenMeteoService(HttpClient http) : base(http) { }

    public override string NomeApi => "Open-Meteo";

    public async Task<ClimaAtual> ObterClimaAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        var url = FormattableString.Invariant(
            $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}") +
            "&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m" +
            "&daily=weather_code,temperature_2m_max,temperature_2m_min" +
            "&timezone=auto&forecast_days=7";

        var r = await GetAsync<OpenMeteoResposta>(url, ct);
        var atual = r.Current ?? throw new ApiException("Open-Meteo não retornou o clima atual.");

        var clima = new ClimaAtual
        {
            Temperatura = atual.Temperature,
            SensacaoTermica = atual.ApparentTemperature,
            Umidade = atual.RelativeHumidity,
            VelocidadeVento = atual.WindSpeed,
            CodigoTempo = atual.WeatherCode,
            Horario = DateTime.TryParse(atual.Time, CultureInfo.InvariantCulture, DateTimeStyles.None, out var h)
                ? h : DateTime.Now
        };

        if (r.Daily is { } d)
        {
            for (var i = 0; i < d.Time.Count; i++)
            {
                if (!DateTime.TryParse(d.Time[i], CultureInfo.InvariantCulture, DateTimeStyles.None, out var dia))
                    continue;

                clima.Previsao.Add(new PrevisaoDia
                {
                    Data = dia,
                    CodigoTempo = Valor(d.WeatherCode, i) ?? -1,
                    Maxima = Valor(d.TemperatureMax, i) ?? 0,
                    Minima = Valor(d.TemperatureMin, i) ?? 0
                });
            }
        }

        return clima;
    }

    public async Task<ClimaDia?> ObterClimaDiaAsync(double latitude, double longitude, DateTime data,
        CancellationToken ct = default)
    {
        var hoje = DateTime.Today;
        if (data.Date > hoje.AddDays(15))
            return null; // previsão só alcança ~16 dias

        // Dias recentes ainda não estão no arquivo histórico: usa a API de previsão (que guarda ~3 meses).
        var historico = data.Date < hoje.AddDays(-7);
        if (historico && data.Date < hoje.AddYears(-80))
            return null;

        var baseUrl = historico
            ? "https://archive-api.open-meteo.com/v1/archive"
            : "https://api.open-meteo.com/v1/forecast";
        var dia = data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var url = FormattableString.Invariant($"{baseUrl}?latitude={latitude}&longitude={longitude}") +
                  "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum" +
                  $"&timezone=auto&start_date={dia}&end_date={dia}";

        var r = await GetAsync<OpenMeteoResposta>(url, ct);
        if (r.Daily is not { Time.Count: > 0 } d)
            return null;

        return new ClimaDia
        {
            Data = data.Date,
            CodigoTempo = Valor(d.WeatherCode, 0) ?? -1,
            Maxima = Valor(d.TemperatureMax, 0) ?? 0,
            Minima = Valor(d.TemperatureMin, 0) ?? 0,
            ChuvaMm = Valor(d.PrecipitationSum, 0) ?? 0
        };
    }

    private static T? Valor<T>(List<T?> lista, int i) where T : struct =>
        i < lista.Count ? lista[i] : null;
}
