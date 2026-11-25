using System.Text.Json.Serialization;

namespace F1WeatherStatus.Models;

/// <summary>Município retornado pela API de localidades do IBGE.</summary>
public class Municipio
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
}
