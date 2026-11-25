using System.Globalization;

namespace F1WeatherStatus.Models;

/// <summary>Condições atuais e previsão dos próximos dias.</summary>
public class ClimaAtual
{
    public double Temperatura { get; set; }
    public double SensacaoTermica { get; set; }
    public double Umidade { get; set; }
    public double VelocidadeVento { get; set; }
    public int CodigoTempo { get; set; }
    public DateTime Horario { get; set; }
    public List<PrevisaoDia> Previsao { get; set; } = new();

    public string Descricao => DescricaoTempo.Descrever(CodigoTempo);
}

public class PrevisaoDia
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public DateTime Data { get; set; }
    public double Minima { get; set; }
    public double Maxima { get; set; }
    public int CodigoTempo { get; set; }

    public string DiaTexto => Data.Date == DateTime.Today ? "Hoje" : Data.ToString("ddd, dd/MM", PtBr);
    public string Descricao => DescricaoTempo.Descrever(CodigoTempo);
    public string Faixa => $"{Minima:F0}° / {Maxima:F0}°";
}

/// <summary>Clima de um dia específico (passado, hoje ou futuro próximo).</summary>
public class ClimaDia
{
    public DateTime Data { get; set; }
    public double Minima { get; set; }
    public double Maxima { get; set; }
    public double ChuvaMm { get; set; }
    public int CodigoTempo { get; set; }

    public string Descricao => DescricaoTempo.Descrever(CodigoTempo);
    public string Resumo =>
        $"{Descricao} · {Minima:F0}° a {Maxima:F0}° · chuva {ChuvaMm:F1} mm";
}

/// <summary>Traduz os códigos de tempo da OMM (WMO) usados pela Open-Meteo.</summary>
public static class DescricaoTempo
{
    public static string Icone(int codigo) => codigo switch
    {
        0 or 1 => "☀️",
        2 => "⛅",
        3 => "☁️",
        45 or 48 => "🌫️",
        51 or 53 or 55 or 56 or 57 => "🌦️",
        61 or 63 or 65 or 66 or 67 or 80 or 81 or 82 => "🌧️",
        71 or 73 or 75 or 77 or 85 or 86 => "❄️",
        95 or 96 or 99 => "⛈️",
        _ => "🌡️"
    };

    public static string Descrever(int codigo) => codigo switch
    {
        0 => "Céu limpo",
        1 => "Predominantemente limpo",
        2 => "Parcialmente nublado",
        3 => "Nublado",
        45 or 48 => "Neblina",
        51 or 53 or 55 => "Garoa",
        56 or 57 => "Garoa congelante",
        61 => "Chuva fraca",
        63 => "Chuva moderada",
        65 => "Chuva forte",
        66 or 67 => "Chuva congelante",
        71 or 73 or 75 => "Neve",
        77 => "Grãos de neve",
        80 or 81 or 82 => "Pancadas de chuva",
        85 or 86 => "Pancadas de neve",
        95 => "Trovoada",
        96 or 99 => "Trovoada com granizo",
        _ => "Condição desconhecida"
    };
}
