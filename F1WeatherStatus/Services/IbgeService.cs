using F1WeatherStatus.Models;

namespace F1WeatherStatus.Services;

/// <summary>Estados e municípios pela API de localidades do IBGE (gratuita, sem chave).</summary>
public sealed class IbgeService : ApiServiceBase, IIbgeService
{
    private const string BaseUrl = "https://servicodados.ibge.gov.br/api/v1/localidades";

    private IReadOnlyList<Estado>? _estados;
    private readonly Dictionary<string, IReadOnlyList<Municipio>> _municipios = new();

    public IbgeService(HttpClient http) : base(http) { }

    public override string NomeApi => "IBGE";

    public async Task<IReadOnlyList<Estado>> ObterEstadosAsync(CancellationToken ct = default)
    {
        // Guarda em memória: várias telas usam a lista de estados.
        return _estados ??= await GetListaAsync<Estado>($"{BaseUrl}/estados?orderBy=nome", ct);
    }

    public async Task<IReadOnlyList<Municipio>> ObterMunicipiosAsync(string siglaEstado, CancellationToken ct = default)
    {
        if (_municipios.TryGetValue(siglaEstado, out var guardados))
            return guardados;

        var lista = await GetListaAsync<Municipio>(
            $"{BaseUrl}/estados/{Uri.EscapeDataString(siglaEstado)}/municipios?orderBy=nome", ct);

        _municipios[siglaEstado] = lista;
        return lista;
    }

    public async Task<Estado?> BuscarEstadoPorNomeAsync(string nome, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return null;

        var estados = await ObterEstadosAsync(ct);
        return estados.FirstOrDefault(e => Texto.Igual(e.Nome, nome) || Texto.Igual(e.Sigla, nome))
               ?? estados.FirstOrDefault(e => Texto.Contem(nome, e.Nome));
    }
}
