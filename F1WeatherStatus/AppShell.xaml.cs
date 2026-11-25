using F1WeatherStatus.Views;

namespace F1WeatherStatus;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rotas empilhadas (não aparecem no menu)
        Routing.RegisterRoute(nameof(EstadoDetalhePage), typeof(EstadoDetalhePage));
        Routing.RegisterRoute(nameof(DetalheCorridaPage), typeof(DetalheCorridaPage));
    }
}
