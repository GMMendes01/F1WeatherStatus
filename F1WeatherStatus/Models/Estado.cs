using System.Text.Json.Serialization;

namespace F1WeatherStatus.Models;

/// <summary>Unidade da Federação retornada pela API de localidades do IBGE.</summary>
public class Estado
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sigla")]
    public string Sigla { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("regiao")]
    public Regiao Regiao { get; set; } = new();

    public string RegiaoTexto => $"Região {Regiao.Nome}";
    public string Capital => Capitais.De(Sigla);
    public string Resumo => $"{RegiaoTexto} · Capital: {Capital}";
}

public class Regiao
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sigla")]
    public string Sigla { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
}
