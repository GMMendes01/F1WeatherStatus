using F1WeatherStatus.Models;

namespace F1WeatherStatus.Services;

/// <summary>
/// Tenta cada <see cref="ILocalizacaoService"/> registrado, na ordem, até um responder.
/// Não conhece as classes concretas: trabalha só com a interface (polimorfismo).
/// </summary>
public sealed class GerenciadorLocalizacao
{
    private readonly IReadOnlyList<ILocalizacaoService> _provedores;

    public GerenciadorLocalizacao(IEnumerable<ILocalizacaoService> provedores)
    {
        _provedores = provedores.ToList();
    }

    public async Task<Localizacao> ObterAsync(CancellationToken ct = default)
    {
        var falhas = new List<string>();

        foreach (var provedor in _provedores)
        {
            try
            {
                var localizacao = await provedor.ObterLocalizacaoAsync(ct);
                localizacao.Fonte = provedor.NomeApi;
                return localizacao;
            }
            catch (ApiException ex)
            {
                falhas.Add(ex.Message);
            }
        }

        throw new ApiException("Nenhuma API de localização respondeu:\n" + string.Join("\n", falhas));
    }
}
