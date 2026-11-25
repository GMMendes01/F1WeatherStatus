namespace F1WeatherStatus;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Sempre tema claro (fundo branco), mesmo com o Windows/Android em modo escuro.
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell()) { Title = "F1WeatherStatus" };
    }
}
