using System.Globalization;

namespace F1WeatherStatus.Models;

/// <summary>Localização aproximada obtida a partir do endereço IP.</summary>
public class Localizacao
{
    public string Ip { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Regiao { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string CodigoPais { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string FusoHorario { get; set; } = string.Empty;
    public string Provedor { get; set; } = string.Empty;

    /// <summary>Nome da API que respondeu (ip-api.io ou ipwho.is).</summary>
    public string Fonte { get; set; } = string.Empty;

    public string CidadeCompleta =>
        string.Join(", ", new[] { Cidade, Regiao, Pais }.Where(p => !string.IsNullOrWhiteSpace(p)));

    public string Coordenadas =>
        string.Format(CultureInfo.InvariantCulture, "{0:F4}, {1:F4}", Latitude, Longitude);
}
