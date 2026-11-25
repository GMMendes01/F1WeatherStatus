namespace F1WeatherStatus.Models;

/// <summary>Capitais dos estados (a API de estados do IBGE não informa a capital).</summary>
public static class Capitais
{
    private static readonly Dictionary<string, string> PorSigla = new()
    {
        ["AC"] = "Rio Branco", ["AL"] = "Maceió", ["AP"] = "Macapá", ["AM"] = "Manaus",
        ["BA"] = "Salvador", ["CE"] = "Fortaleza", ["DF"] = "Brasília", ["ES"] = "Vitória",
        ["GO"] = "Goiânia", ["MA"] = "São Luís", ["MT"] = "Cuiabá", ["MS"] = "Campo Grande",
        ["MG"] = "Belo Horizonte", ["PA"] = "Belém", ["PB"] = "João Pessoa", ["PR"] = "Curitiba",
        ["PE"] = "Recife", ["PI"] = "Teresina", ["RJ"] = "Rio de Janeiro", ["RN"] = "Natal",
        ["RS"] = "Porto Alegre", ["RO"] = "Porto Velho", ["RR"] = "Boa Vista", ["SC"] = "Florianópolis",
        ["SP"] = "São Paulo", ["SE"] = "Aracaju", ["TO"] = "Palmas"
    };

    public static string De(string sigla) =>
        PorSigla.TryGetValue(sigla, out var capital) ? capital : string.Empty;
}
