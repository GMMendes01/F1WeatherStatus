namespace F1WeatherStatus.Views;

/// <summary>
/// Página base (herança) de todas as telas do app. Centraliza:
/// carregamento inicial, indicador de "carregando", exibição de erros e navegação.
/// </summary>
public abstract class BasePage : ContentPage
{
    private bool _jaCarregou;
    private bool _carregando;

    /// <summary>Ligado ao indicador de carregamento das telas.</summary>
    public bool Carregando
    {
        get => _carregando;
        private set
        {
            _carregando = value;
            OnPropertyChanged();
        }
    }

    protected BasePage()
    {
        // Permite que o XAML de cada tela faça binding em Carregando.
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_jaCarregou)
            return;

        _jaCarregou = true;
        await ExecutarAsync(CarregarDadosAsync);
    }

    /// <summary>Cada tela sobrescreve para buscar seus dados na primeira exibição.</summary>
    protected virtual Task CarregarDadosAsync() => Task.CompletedTask;

    /// <summary>Executa uma ação mostrando o indicador e tratando qualquer erro.</summary>
    protected async Task ExecutarAsync(Func<Task> acao)
    {
        if (Carregando)
            return;

        try
        {
            Carregando = true;
            await acao();
        }
        catch (Exception ex)
        {
            await ExibirErroAsync(ex.Message);
        }
        finally
        {
            Carregando = false;
        }
    }

    protected Task ExibirErroAsync(string mensagem) =>
        DisplayAlertAsync("Ops!", mensagem, "OK");

    /// <summary>Navegação em pilha (ou absoluta, com "//") pelo Shell.</summary>
    protected static Task NavegarParaAsync(string rota, IDictionary<string, object>? parametros = null) =>
        parametros is null
            ? Shell.Current.GoToAsync(rota)
            : Shell.Current.GoToAsync(rota, parametros);
}
