using System.Globalization;
using System.Text.Json;
using F1WeatherStatus.Models;
using F1WeatherStatus.Services.Dto;

namespace F1WeatherStatus.Services;

/// <summary>Corridas e resultados de Fórmula 1 pela OpenF1 (gratuita, sem chave).</summary>
public sealed class OpenF1Service : ApiServiceBase, IFormula1Service
{
    private const string BaseUrl = "https://api.openf1.org/v1";

    public OpenF1Service(HttpClient http) : base(http) { }

    public override string NomeApi => "OpenF1";

    public async Task<IReadOnlyList<Corrida>> ObterCorridasAsync(int ano, CancellationToken ct = default)
    {
        var sessoes = await GetListaAsync<OpenF1Sessao>($"{BaseUrl}/sessions?year={ano}&session_name=Race", ct);

        return sessoes
            .Select(s => new Corrida
            {
                SessionKey = s.SessionKey,
                MeetingKey = s.MeetingKey,
                Local = s.Location ?? string.Empty,
                Pais = s.CountryName ?? string.Empty,
                Circuito = s.CircuitShortName ?? string.Empty,
                Data = s.DateStart
            })
            .OrderBy(c => c.Data)
            .ToList();
    }

    public async Task<InfoCircuito?> ObterCircuitoAsync(int meetingKey, CancellationToken ct = default)
    {
        var lista = await GetListaAsync<OpenF1Meeting>($"{BaseUrl}/meetings?meeting_key={meetingKey}", ct);
        var m = lista.FirstOrDefault();
        return m is null
            ? null
            : new InfoCircuito
            {
                NomeOficial = m.OfficialName ?? string.Empty,
                Tipo = m.CircuitType ?? string.Empty,
                ImagemUrl = m.CircuitImage,
                BandeiraUrl = m.CountryFlag
            };
    }

    public async Task<IReadOnlyList<ResultadoPiloto>> ObterResultadoAsync(int sessionKey, CancellationToken ct = default)
    {
        var tarefaResultado = GetListaAsync<OpenF1Resultado>($"{BaseUrl}/session_result?session_key={sessionKey}", ct);
        var tarefaPilotos = GetListaAsync<OpenF1Piloto>($"{BaseUrl}/drivers?session_key={sessionKey}", ct);
        await Task.WhenAll(tarefaResultado, tarefaPilotos);

        var pilotos = tarefaPilotos.Result
            .GroupBy(p => p.DriverNumber)
            .ToDictionary(g => g.Key, g => g.First());

        return tarefaResultado.Result
            .Select(r =>
            {
                pilotos.TryGetValue(r.DriverNumber, out var p);
                return new ResultadoPiloto
                {
                    Posicao = r.Position,
                    Numero = r.DriverNumber,
                    Nome = p?.FullName ?? $"Piloto #{r.DriverNumber}",
                    Sigla = p?.NameAcronym ?? string.Empty,
                    Equipe = p?.TeamName ?? "Equipe não informada",
                    CorEquipe = CorDaEquipe(p?.TeamColour),
                    Voltas = r.NumberOfLaps ?? 0,
                    Situacao = r.Dsq ? "DSQ" : r.Dns ? "DNS" : r.Dnf ? "DNF" : null,
                    Tempo = FormatarTempo(r)
                };
            })
            .OrderBy(r => r.Posicao ?? int.MaxValue)
            .ThenByDescending(r => r.Voltas)
            .ToList();
    }

    private static Color CorDaEquipe(string? hex)
    {
        var limpo = hex?.Trim().TrimStart('#') ?? string.Empty;
        if (limpo.Length != 6 || !limpo.All(Uri.IsHexDigit))
            return Colors.Gray;

        return Color.FromArgb("#" + limpo);
    }

    private static string FormatarTempo(OpenF1Resultado r)
    {
        if (r.Dsq) return "Desclassificado";
        if (r.Dns) return "Não largou";
        if (r.Dnf) return "Abandonou";

        if (r.Position == 1 && r.Duration is { ValueKind: JsonValueKind.Number } duracao)
            return TimeSpan.FromSeconds(duracao.GetDouble()).ToString(@"h\:mm\:ss\.fff", CultureInfo.InvariantCulture);

        if (r.GapToLeader is { } gap)
        {
            if (gap.ValueKind == JsonValueKind.Number)
                return "+" + gap.GetDouble().ToString("F3", CultureInfo.InvariantCulture) + "s";

            if (gap.ValueKind == JsonValueKind.String)
                return (gap.GetString() ?? string.Empty).Replace("LAPS", "voltas").Replace("LAP", "volta");
        }

        return string.Empty;
    }
}
