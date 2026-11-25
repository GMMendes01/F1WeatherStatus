using F1WeatherStatus.Models;

namespace F1WeatherStatus.Services;

/// <summary>Contrato comum às APIs de localização por IP (permite polimorfismo).</summary>
public interface ILocalizacaoService
{
    string NomeApi { get; }
    Task<Localizacao> ObterLocalizacaoAsync(CancellationToken ct = default);
}

public interface IClimaService
{
    Task<ClimaAtual> ObterClimaAsync(double latitude, double longitude, CancellationToken ct = default);

    /// <summary>Clima de um dia específico; null se a data estiver fora do alcance da API.</summary>
    Task<ClimaDia?> ObterClimaDiaAsync(double latitude, double longitude, DateTime data, CancellationToken ct = default);
}

public interface IGeocodingService
{
    /// <summary>Busca um lugar pelo nome; null se não encontrar.</summary>
    Task<Lugar?> BuscarAsync(string nome, string? codigoPais = null, string? regiao = null, CancellationToken ct = default);
}

public interface IIbgeService
{
    Task<IReadOnlyList<Estado>> ObterEstadosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Municipio>> ObterMunicipiosAsync(string siglaEstado, CancellationToken ct = default);

    /// <summary>Acha o estado a partir de um nome (ex.: a região devolvida pela API de IP).</summary>
    Task<Estado?> BuscarEstadoPorNomeAsync(string nome, CancellationToken ct = default);
}

public interface IFormula1Service
{
    Task<IReadOnlyList<Corrida>> ObterCorridasAsync(int ano, CancellationToken ct = default);
    Task<IReadOnlyList<ResultadoPiloto>> ObterResultadoAsync(int sessionKey, CancellationToken ct = default);
    Task<InfoCircuito?> ObterCircuitoAsync(int meetingKey, CancellationToken ct = default);
}
