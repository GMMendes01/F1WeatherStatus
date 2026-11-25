using F1WeatherStatus.Models;
using F1WeatherStatus.Services;

namespace F1WeatherStatus.Views;

public partial class LocalizacaoPage : BasePage
{
    private readonly GerenciadorLocalizacao _localizacao;
    private readonly IIbgeService _ibge;
    private readonly IClimaService _clima;

    private Localizacao? _atual;
    private Estado? _estado;

    public LocalizacaoPage(GerenciadorLocalizacao localizacao, IIbgeService ibge, IClimaService clima)
    {
        InitializeComponent();
        _localizacao = localizacao;
        _ibge = ibge;
        _clima = clima;
    }

    protected override Task CarregarDadosAsync() => AtualizarAsync();

    private async Task AtualizarAsync()
    {
        // 1) IP -> coordenadas e região
        var local = await _localizacao.ObterAsync();
        _atual = local;

        LblIp.Text = local.Ip;
        LblCidade.Text = local.CidadeCompleta;
        LblCoordenadas.Text = local.Coordenadas;
        LblFuso.Text = local.FusoHorario;
        LblProvedor.Text = local.Provedor;
        LblFonte.Text = local.Fonte;
        CartaoLocal.IsVisible = true;
        BtnVerClima.IsEnabled = true;

        // 2) e 3) dependem do resultado do IP e rodam juntos; uma falha não derruba a outra.
        await Task.WhenAll(CarregarEstadoAsync(local), CarregarClimaAsync(local));
    }

    private async Task CarregarEstadoAsync(Localizacao local)
    {
        _estado = null;
        CartaoEstado.IsVisible = false;

        if (!string.Equals(local.CodigoPais, "BR", StringComparison.OrdinalIgnoreCase))
            return; // o IBGE só conhece estados brasileiros

        try
        {
            _estado = await _ibge.BuscarEstadoPorNomeAsync(local.Regiao);
        }
        catch (ApiException)
        {
            return;
        }

        if (_estado is null)
            return;

        LblEstado.Text = $"{_estado.Nome} ({_estado.Sigla})";
        LblEstadoResumo.Text = _estado.Resumo;
        CartaoEstado.IsVisible = true;
    }

    private async Task CarregarClimaAsync(Localizacao local)
    {
        CartaoClima.IsVisible = false;

        try
        {
            var clima = await _clima.ObterClimaAsync(local.Latitude, local.Longitude);
            LblTemperatura.Text = $"{clima.Temperatura:F1} °C";
            LblClima.Text = clima.Descricao;
            LblClimaDetalhe.Text =
                $"Sensação {clima.SensacaoTermica:F1} °C · Umidade {clima.Umidade:F0}% · Vento {clima.VelocidadeVento:F0} km/h";
            CartaoClima.IsVisible = true;
        }
        catch (ApiException)
        {
            // O clima é um complemento: sem ele a tela continua útil.
        }
    }

    private async void OnAtualizarClicked(object? sender, EventArgs e) =>
        await ExecutarAsync(AtualizarAsync);

    // Pilha: abre o detalhe do estado (IBGE + clima da capital + municípios).
    private async void OnVerEstadoClicked(object? sender, EventArgs e)
    {
        if (_estado is null)
            return;

        await ExecutarAsync(() => NavegarParaAsync(nameof(EstadoDetalhePage),
            new Dictionary<string, object>
            {
                ["estado"] = _estado,
                ["municipioInicial"] = _atual?.Cidade ?? string.Empty
            }));
    }

    // Abas: leva as coordenadas para a tela de Clima.
    private async void OnVerClimaClicked(object? sender, EventArgs e)
    {
        if (_atual is null)
            return;

        var parametros = new Dictionary<string, object>
        {
            ["lat"] = _atual.Latitude,
            ["lon"] = _atual.Longitude,
            ["local"] = _atual.CidadeCompleta
        };

        await ExecutarAsync(() => NavegarParaAsync("//inicio/clima", parametros));
    }
}
