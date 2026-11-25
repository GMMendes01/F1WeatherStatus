using System.Net;
using System.Text.Json;

namespace F1WeatherStatus.Services;

/// <summary>Erro amigável de uma API, exibido ao usuário.</summary>
public class ApiException : Exception
{
    public ApiException(string mensagem, Exception? interna = null) : base(mensagem, interna) { }
}

/// <summary>
/// Classe base abstrata (herança) para todos os serviços que consomem APIs REST.
/// Centraliza a requisição HTTP, a leitura do JSON e o tratamento de erros.
/// </summary>
public abstract class ApiServiceBase
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected ApiServiceBase(HttpClient http)
    {
        Http = http;
    }

    protected HttpClient Http { get; }

    /// <summary>Nome da API, usado nas mensagens de erro.</summary>
    public abstract string NomeApi { get; }

    /// <summary>Faz GET e converte o JSON para o tipo informado.</summary>
    protected async Task<T> GetAsync<T>(string url, CancellationToken ct = default)
    {
        using var resposta = await EnviarAsync(url, ct);
        GarantirSucesso(resposta);
        return await LerJsonAsync<T>(resposta, ct);
    }

    /// <summary>Faz GET de uma lista; 404 é tratado como lista vazia.</summary>
    protected async Task<List<T>> GetListaAsync<T>(string url, CancellationToken ct = default)
    {
        using var resposta = await EnviarAsync(url, ct);
        if (resposta.StatusCode == HttpStatusCode.NotFound)
            return new List<T>();

        GarantirSucesso(resposta);
        return await LerJsonAsync<List<T>>(resposta, ct);
    }

    /// <summary>Faz GET e devolve o JSON bruto, para respostas de formato variável.</summary>
    protected async Task<JsonDocument> GetDocumentoAsync(string url, CancellationToken ct = default)
    {
        using var resposta = await EnviarAsync(url, ct);
        GarantirSucesso(resposta);
        await using var fluxo = await resposta.Content.ReadAsStreamAsync(ct);
        return await JsonDocument.ParseAsync(fluxo, cancellationToken: ct);
    }

    private async Task<HttpResponseMessage> EnviarAsync(string url, CancellationToken ct)
    {
        try
        {
            return await Http.GetAsync(url, ct);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new ApiException($"{NomeApi} demorou demais para responder.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"Não foi possível conectar a {NomeApi}. Verifique a internet.", ex);
        }
    }

    private void GarantirSucesso(HttpResponseMessage resposta)
    {
        if (!resposta.IsSuccessStatusCode)
            throw new ApiException($"{NomeApi} respondeu com erro {(int)resposta.StatusCode} ({resposta.ReasonPhrase}).");
    }

    private async Task<T> LerJsonAsync<T>(HttpResponseMessage resposta, CancellationToken ct)
    {
        try
        {
            await using var fluxo = await resposta.Content.ReadAsStreamAsync(ct);
            var dados = await JsonSerializer.DeserializeAsync<T>(fluxo, OpcoesJson, ct);
            return dados ?? throw new ApiException($"{NomeApi} retornou uma resposta vazia.");
        }
        catch (JsonException ex)
        {
            throw new ApiException($"{NomeApi} retornou dados em formato inesperado.", ex);
        }
    }
}
