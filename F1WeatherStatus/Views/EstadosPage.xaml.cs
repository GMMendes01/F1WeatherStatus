using F1WeatherStatus.Models;
using F1WeatherStatus.Services;

namespace F1WeatherStatus.Views;

public partial class EstadosPage : BasePage
{
    private readonly IIbgeService _ibge;
    private IReadOnlyList<Estado> _todos = Array.Empty<Estado>();

    public EstadosPage(IIbgeService ibge)
    {
        InitializeComponent();
        _ibge = ibge;
    }

    protected override async Task CarregarDadosAsync()
    {
        _todos = await _ibge.ObterEstadosAsync();
        Filtrar(BuscaEstados.Text);
    }

    private void OnBuscaTextChanged(object? sender, TextChangedEventArgs e) => Filtrar(e.NewTextValue);

    private void Filtrar(string? texto)
    {
        var termo = texto?.Trim() ?? string.Empty;

        var lista = termo.Length == 0
            ? _todos
            : _todos.Where(e => Texto.Contem(e.Nome, termo) || Texto.Contem(e.Sigla, termo)
                                || Texto.Contem(e.Regiao.Nome, termo) || Texto.Contem(e.Capital, termo))
                    .ToList();

        ListaEstados.ItemsSource = lista;
        LblTotal.Text = $"{lista.Count} de {_todos.Count} estados";
    }

    // Pilha: abre o detalhe do estado escolhido.
    private async void OnEstadoSelecionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Estado estado)
            return;

        ListaEstados.SelectedItem = null;

        await ExecutarAsync(() => NavegarParaAsync(nameof(EstadoDetalhePage),
            new Dictionary<string, object> { ["estado"] = estado }));
    }
}
