using F1WeatherStatus.Models;

namespace F1WeatherStatus.Views;

/// <summary>Ranking de títulos mundiais de pilotos e de equipes (construtores).</summary>
public partial class RankingPage : BasePage
{
    public RankingPage()
    {
        InitializeComponent();
        Mostrar(equipes: false);
    }

    private void OnPilotosClicked(object? sender, EventArgs e) => Mostrar(equipes: false);

    private void OnEquipesClicked(object? sender, EventArgs e) => Mostrar(equipes: true);

    private void Mostrar(bool equipes)
    {
        ListaRanking.ItemsSource = HistoricoCampeoes.Ranking(equipes ? HistoricoCampeoes.Equipes : HistoricoCampeoes.Pilotos);
        LblExplicacao.Text = equipes
            ? "Títulos de construtores, de 1958 a 2025."
            : "Títulos mundiais de pilotos, de 1950 a 2025.";

        // O botão da lista atual fica cheio; o outro, apagado.
        BtnPilotos.Opacity = equipes ? 0.5 : 1;
        BtnEquipes.Opacity = equipes ? 1 : 0.5;
    }
}
