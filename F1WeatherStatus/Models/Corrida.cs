using System.Globalization;

namespace F1WeatherStatus.Models;

/// <summary>Dados do circuito vindos da OpenF1 (endpoint meetings).</summary>
public class InfoCircuito
{
    public string NomeOficial { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public string? BandeiraUrl { get; set; }
}

/// <summary>Uma corrida (sessão "Race") de Fórmula 1.</summary>
public class Corrida
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public int SessionKey { get; set; }
    public int MeetingKey { get; set; }
    public string Local { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Circuito { get; set; } = string.Empty;
    public DateTimeOffset Data { get; set; }

    public string Titulo => $"{Pais} – {Local}";
    public string DataTexto => Data.ToLocalTime().ToString("dd/MM/yyyy HH:mm", PtBr);
    public bool JaAconteceu => Data < DateTimeOffset.Now;
    public string Situacao => JaAconteceu ? "Realizada" : "Em breve";
}
