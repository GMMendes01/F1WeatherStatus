using F1WeatherStatus.Models;
using F1WeatherStatus.Services;

namespace F1WeatherStatus.Views;

/// <summary>
/// Junta três APIs: IBGE (estado e municípios), Geocoding (nome -> coordenadas)
/// e Open-Meteo (clima nessas coordenadas).
/// </summary>
public partial class EstadoDetalhePage : BasePage, IQueryAttributable
{
    private readonly IIbgeService _ibge;
    private readonly IGeocodingService _geocoding;
    private readonly IClimaService _clima;

    private Estado? _estado;
    private string _municipioInicial = string.Empty;
    private IReadOnlyList<Municipio> _municipios = Array.Empty<Municipio>();

    public EstadoDetalhePage(IIbgeService ibge, IGeocodingService geocoding, IClimaService clima)
    {
        InitializeComponent();
        _ibge = ibge;
        _geocoding = geocoding;
        _clima = clima;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("estado", out var valor) || valor is not Estado estado)
            return;

        _estado = estado;
        _municipioInicial = query.TryGetValue("municipioInicial", out var m) && m is string texto ? texto : string.Empty;

        Title = estado.Sigla;
        LblNome.Text = estado.Nome;
        LblResumo.Text = estado.Resumo;
    }

    protected override async Task CarregarDadosAsync()
    {
        if (_estado is null)
            return;

        _municipios = await _ibge.ObterMunicipiosAsync(_estado.Sigla);
        Filtrar(BuscaMunicipios.Text);

        // Mostra o município do usuário (vindo do IP) ou, senão, a capital.
        var inicial = string.IsNullOrWhiteSpace(_municipioInicial) ? _estado.Capital : _municipioInicial;
        await MostrarClimaAsync(inicial);
    }

    private void OnBuscaTextChanged(object? sender, TextChangedEventArgs e) => Filtrar(e.NewTextValue);

    private void Filtrar(string? texto)
    {
        var termo = texto?.Trim() ?? string.Empty;
        var lista = termo.Length == 0
            ? _municipios
            : _municipios.Where(m => Texto.Contem(m.Nome, termo)).ToList();

        ListaMunicipios.ItemsSource = lista;
        LblTotal.Text = $"{lista.Count} de {_municipios.Count} municípios";
    }

    private async void OnMunicipioSelecionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Municipio municipio)
            return;

        ListaMunicipios.SelectedItem = null;
        await ExecutarAsync(() => MostrarClimaAsync(municipio.Nome));
    }

    private async Task MostrarClimaAsync(string nomeMunicipio)
    {
        if (_estado is null || string.IsNullOrWhiteSpace(nomeMunicipio))
            return;

        // Nome do município (IBGE) -> coordenadas (Geocoding) -> clima (Open-Meteo)
        var lugar = await _geocoding.BuscarAsync(nomeMunicipio, "BR", _estado.Nome);
        if (lugar is null)
        {
            CartaoClima.IsVisible = false;
            await ExibirErroAsync($"Não encontrei as coordenadas de {nomeMunicipio}.");
            return;
        }

        var clima = await _clima.ObterClimaAsync(lugar.Latitude, lugar.Longitude);

        LblLugar.Text = $"Clima em {nomeMunicipio} – {_estado.Sigla}";
        LblTemperatura.Text = $"{clima.Temperatura:F1} °C";
        LblClima.Text = clima.Descricao;
        LblClimaDetalhe.Text =
            $"Sensação {clima.SensacaoTermica:F1} °C · Umidade {clima.Umidade:F0}% · Vento {clima.VelocidadeVento:F0} km/h";
        CartaoClima.IsVisible = true;
    }
}
