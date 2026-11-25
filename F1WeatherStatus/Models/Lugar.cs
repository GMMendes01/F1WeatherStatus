using System.Globalization;

namespace F1WeatherStatus.Models;

/// <summary>Resultado da busca de um lugar pelo nome (geocodificação).</summary>
public class Lugar
{
    public string Nome { get; set; } = string.Empty;
    public string Regiao { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string Coordenadas =>
        string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}", Latitude, Longitude);
}
