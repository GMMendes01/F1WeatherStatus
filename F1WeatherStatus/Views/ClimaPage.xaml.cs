using System.Globalization;
using F1WeatherStatus.Models;
using F1WeatherStatus.Services;

namespace F1WeatherStatus.Views;

public partial class ClimaPage : BasePage, IQueryAttributable
{
    private const double LatitudePadrao = -23.5505;
    private const double LongitudePadrao = -46.6333;

    private readonly IClimaService _clima;
    private readonly GerenciadorLocalizacao _localizacao;
    private readonly IGeocodingService _geocoding;
    private string _nomeLocal = "São Paulo (padrão)";
    private double _latitude = LatitudePadrao;
    private double _longitude = LongitudePadrao;

    public ClimaPage(IClimaService clima, GerenciadorLocalizacao localizacao, IGeocodingService geocoding)
    {
        InitializeComponent();
        _clima = clima;
        _localizacao = localizacao;
        _geocoding = geocoding;
    }

    /// <summary>Recebe lat/lon enviados pela tela de Localização.</summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("lat", out var lat) && lat is double latitude &&
            query.TryGetValue("lon", out var lon) && lon is double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
            _nomeLocal = query.TryGetValue("local", out var nome) && nome is string texto && texto.Length > 0
                ? texto
                : "Local informado";

            _ = ExecutarAsync(ConsultarAsync);
        }
    }

    protected override Task CarregarDadosAsync() => ConsultarAsync();

    // Nome da cidade -> coordenadas (Geocoding) -> clima (Open-Meteo)
    private async void OnBuscarCidadeClicked(object? sender, EventArgs e)
    {
        var nome = EntCidade.Text?.Trim();
        if (string.IsNullOrEmpty(nome))
        {
            await ExibirErroAsync("Digite o nome de uma cidade.");
            return;
        }

        await ExecutarAsync(async () =>
        {
            var lugar = await _geocoding.BuscarAsync(nome);
            if (lugar is null)
            {
                await ExibirErroAsync($"Não encontrei a cidade \"{nome}\".");
                return;
            }

            _latitude = lugar.Latitude;
            _longitude = lugar.Longitude;
            _nomeLocal = string.Join(", ", new[] { lugar.Nome, lugar.Regiao, lugar.Pais }
                .Where(p => !string.IsNullOrWhiteSpace(p)));
            await ConsultarAsync();
        });
    }

    private async void OnMinhaLocalizacaoClicked(object? sender, EventArgs e)
    {
        await ExecutarAsync(async () =>
        {
            var local = await _localizacao.ObterAsync();
            _latitude = local.Latitude;
            _longitude = local.Longitude;
            _nomeLocal = local.CidadeCompleta;
            await ConsultarAsync();
        });
    }

    private async Task ConsultarAsync()
    {
        var clima = await _clima.ObterClimaAsync(_latitude, _longitude);
        Exibir(clima);
    }

    private void Exibir(ClimaAtual clima)
    {
        LblLocal.Text = _nomeLocal;
        LblTemperatura.Text = $"{clima.Temperatura:F1} °C";
        LblTemperatura.TextColor = CorTemperatura.Cor(clima.Temperatura);
        CartaoClima.Stroke = new SolidColorBrush(CorTemperatura.Cor(clima.Temperatura));
        CartaoClima.BackgroundColor = CorTemperatura.Fundo(clima.Temperatura);
        LblDescricao.Text = clima.Descricao;
        LblDetalhes.Text =
            $"Sensação {clima.SensacaoTermica:F1} °C · Umidade {clima.Umidade:F0}% · Vento {clima.VelocidadeVento:F0} km/h";
        LblAtualizado.Text = $"Horário local da medição: {clima.Horario:dd/MM HH:mm}";

        CartaoClima.IsVisible = true;
        LblTituloPrevisao.IsVisible = clima.Previsao.Count > 0;
        BindableLayout.SetItemsSource(ListaPrevisao, clima.Previsao);
    }

}
