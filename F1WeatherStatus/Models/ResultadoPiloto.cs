namespace F1WeatherStatus.Models;

/// <summary>Linha do resultado final de uma corrida.</summary>
public class ResultadoPiloto
{
    public int? Posicao { get; set; }
    public int Numero { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string Equipe { get; set; } = string.Empty;
    public Color CorEquipe { get; set; } = Colors.Gray;
    public int Voltas { get; set; }
    public string Tempo { get; set; } = string.Empty;

    /// <summary>DNF, DNS ou DSQ quando o piloto não terminou.</summary>
    public string? Situacao { get; set; }

    public string PosicaoTexto => Posicao?.ToString() ?? Situacao ?? "-";
    public string Detalhe => $"#{Numero} · {Equipe} · {Voltas} voltas";
}
