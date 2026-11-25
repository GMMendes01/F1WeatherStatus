using F1WeatherStatus.Models;
using F1WeatherStatus.Services;

namespace F1WeatherStatus.Views;

/// <summary>
/// Junta quatro APIs: OpenF1 (corrida e resultado), Geocoding (onde fica o circuito),
/// Open-Meteo (clima no dia da corrida) e a API de IP (distância até você).
/// </summary>
public partial class DetalheCorridaPage : BasePage, IQueryAttributable
{
    private readonly IFormula1Service _f1;
    private readonly IGeocodingService _geocoding;
    private readonly IClimaService _clima;
    private readonly GerenciadorLocalizacao _localizacao;
    private Corrida? _corrida;

    public DetalheCorridaPage(IFormula1Service f1, IGeocodingService geocoding, IClimaService clima,
        GerenciadorLocalizacao localizacao)
    {
        InitializeComponent();
        _f1 = f1;
        _geocoding = geocoding;
        _clima = clima;
        _localizacao = localizacao;
    }

    /// <summary>Recebe a corrida enviada pela tela anterior da pilha.</summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("corrida", out var valor) || valor is not Corrida corrida)
            return;

        _corrida = corrida;
        Title = corrida.Local;
        LblTitulo.Text = corrida.Titulo;
        LblCircuito.Text = $"Circuito: {corrida.Circuito}";
        LblData.Text = $"Largada: {corrida.DataTexto}";
    }

    protected override async Task CarregarDadosAsync()
    {
        if (_corrida is null)
            return;

        // Os extras rodam em paralelo com o resultado e nunca impedem a lista de aparecer.
        var extras = CarregarExtrasAsync(_corrida);
        var circuitoInfo = CarregarCircuitoAsync(_corrida);

        var resultado = await _f1.ObterResultadoAsync(_corrida.SessionKey);
        ListaResultado.ItemsSource = resultado;

        if (resultado.Count > 0)
        {
            LblResumo.Text = $"{resultado.Count} pilotos classificados";
        }
        else
        {
            LblResumo.Text = string.Empty;
            LblVazio.Text = _corrida.JaAconteceu
                ? "O resultado desta corrida ainda não foi publicado."
                : "Esta corrida ainda não aconteceu.";
        }

        await extras;
        await circuitoInfo;
    }

    // OpenF1 (meetings): imagem do traçado, tipo de circuito e nome oficial do GP.
    private async Task CarregarCircuitoAsync(Corrida corrida)
    {
        try
        {
            var info = await _f1.ObterCircuitoAsync(corrida.MeetingKey);
            if (info is null)
                return;

            LblOficial.Text = info.NomeOficial;
            LblOficial.IsVisible = info.NomeOficial.Length > 0;
            LblTipoCircuito.Text = $"Tipo de circuito: {info.Tipo}";
            LblTipoCircuito.IsVisible = info.Tipo.Length > 0;

            if (!string.IsNullOrWhiteSpace(info.ImagemUrl))
            {
                ImgCircuito.Source = ImageSource.FromUri(new Uri(info.ImagemUrl));
                ImgCircuito.IsVisible = true;
            }
        }
        catch (ApiException)
        {
            // Complemento opcional.
        }
    }

    private async Task CarregarExtrasAsync(Corrida corrida)
    {
        try
        {
            // Nome do circuito (OpenF1) -> coordenadas (Geocoding). Tenta do mais específico ao mais geral.
            Lugar? circuito = null;
            foreach (var nome in new[] { corrida.Local, corrida.Circuito, corrida.Pais })
            {
                if (string.IsNullOrWhiteSpace(nome))
                    continue;

                circuito = await _geocoding.BuscarAsync(nome);
                if (circuito is not null)
                    break;
            }

            if (circuito is null)
                return;

            var tarefaClima = _clima.ObterClimaDiaAsync(circuito.Latitude, circuito.Longitude, corrida.Data.UtcDateTime);
            var tarefaUsuario = ObterLocalizacaoUsuarioAsync();
            await Task.WhenAll(tarefaClima, tarefaUsuario);

            var climaDia = tarefaClima.Result;
            var usuario = tarefaUsuario.Result;

            if (climaDia is null)
            {
                LblIcone.Text = "🌡️";
                LblTemperaturaCorrida.Text = "--";
                LblDescricaoCorrida.Text = "Indisponível";
                LblClimaCorrida.Text = $"Clima de {corrida.Data.ToLocalTime():dd/MM/yyyy} indisponível para esta data.";
            }
            else
            {
                var media = (climaDia.Minima + climaDia.Maxima) / 2;
                var cor = CorTemperatura.Cor(media);
                LblIcone.Text = DescricaoTempo.Icone(climaDia.CodigoTempo);
                LblTemperaturaCorrida.Text = $"{climaDia.Minima:F0}° a {climaDia.Maxima:F0}°C";
                LblTemperaturaCorrida.TextColor = cor;
                LblDescricaoCorrida.Text = climaDia.Descricao;
                LblClimaTitulo.TextColor = cor;
                CartaoExtras.Stroke = new SolidColorBrush(cor);
                CartaoExtras.BackgroundColor = CorTemperatura.Fundo(media);
                LblClimaCorrida.Text =
                    $"{circuito.Nome} · {corrida.Data.ToLocalTime():dd/MM/yyyy} · chuva {climaDia.ChuvaMm:F1} mm";
            }

            LblDistancia.Text = usuario is null
                ? string.Empty
                : $"Distância até você ({usuario.Cidade}): " +
                  $"{Geo.DistanciaKm(usuario.Latitude, usuario.Longitude, circuito.Latitude, circuito.Longitude):N0} km";

            CartaoExtras.IsVisible = true;
        }
        catch (ApiException)
        {
            // Complemento opcional: sem ele a tela continua mostrando o resultado.
        }
    }

    private async Task<Localizacao?> ObterLocalizacaoUsuarioAsync()
    {
        try
        {
            return await _localizacao.ObterAsync();
        }
        catch (ApiException)
        {
            return null;
        }
    }
}
