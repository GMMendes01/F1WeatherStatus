using F1WeatherStatus.Models;
using F1WeatherStatus.Services;

namespace F1WeatherStatus.Views;

public partial class CorridasPage : BasePage
{
    private const int PrimeiroAnoOpenF1 = 2023;

    private readonly IFormula1Service _f1;
    private readonly bool _inicializado;

    public CorridasPage(IFormula1Service f1)
    {
        InitializeComponent();
        _f1 = f1;

        for (var ano = DateTime.Now.Year; ano >= PrimeiroAnoOpenF1; ano--)
            PickerAno.Items.Add(ano.ToString());

        PickerAno.SelectedIndex = 0;
        _inicializado = true;
    }

    protected override Task CarregarDadosAsync() => CarregarCorridasAsync();

    private async void OnAnoAlterado(object? sender, EventArgs e)
    {
        // Ignora a seleção inicial feita no construtor (o OnAppearing já carrega).
        if (_inicializado)
            await ExecutarAsync(CarregarCorridasAsync);
    }

    private async Task CarregarCorridasAsync()
    {
        if (PickerAno.SelectedItem is not string texto || !int.TryParse(texto, out var ano))
            return;

        ListaCorridas.ItemsSource = await _f1.ObterCorridasAsync(ano);
    }

    // Navegação em PILHA: empilha a tela de detalhe levando a corrida escolhida.
    private async void OnCorridaSelecionada(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Corrida corrida)
            return;

        ListaCorridas.SelectedItem = null;

        await ExecutarAsync(() => NavegarParaAsync(nameof(DetalheCorridaPage),
            new Dictionary<string, object> { ["corrida"] = corrida }));
    }
}
