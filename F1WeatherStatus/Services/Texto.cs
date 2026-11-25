using System.Globalization;

namespace F1WeatherStatus.Services;

/// <summary>Comparações de texto que ignoram maiúsculas e acentos ("sao" = "São").</summary>
public static class Texto
{
    private static readonly CompareInfo Comparador = new CultureInfo("pt-BR").CompareInfo;
    private const CompareOptions Opcoes = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    public static bool Igual(string? a, string? b) =>
        a is not null && b is not null && Comparador.Compare(a.Trim(), b.Trim(), Opcoes) == 0;

    public static bool Contem(string? origem, string? termo) =>
        !string.IsNullOrEmpty(origem) && !string.IsNullOrEmpty(termo)
        && Comparador.IndexOf(origem, termo, Opcoes) >= 0;
}
