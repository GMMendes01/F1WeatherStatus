using F1WeatherStatus.Services;
using F1WeatherStatus.Views;
using Microsoft.Extensions.Logging;

namespace F1WeatherStatus;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Um único HttpClient compartilhado por todos os serviços.
        var http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("F1WeatherStatus/1.0");
        builder.Services.AddSingleton(http);

        // Polimorfismo: duas implementações de ILocalizacaoService.
        // A ordem de registro define a ordem de tentativa (ip-api.io primeiro, ipwho.is de reserva).
        builder.Services.AddSingleton<ILocalizacaoService, IpApiIoService>();
        builder.Services.AddSingleton<ILocalizacaoService, IpWhoIsService>();
        builder.Services.AddSingleton<GerenciadorLocalizacao>();

        builder.Services.AddSingleton<IClimaService, OpenMeteoService>();
        builder.Services.AddSingleton<IGeocodingService, GeocodingService>();
        builder.Services.AddSingleton<IIbgeService, IbgeService>();
        builder.Services.AddSingleton<IFormula1Service, OpenF1Service>();

        // Páginas (o Shell cria cada uma pela injeção de dependência).
        builder.Services.AddTransient<LocalizacaoPage>();
        builder.Services.AddTransient<ClimaPage>();
        builder.Services.AddTransient<EstadosPage>();
        builder.Services.AddTransient<EstadoDetalhePage>();
        builder.Services.AddTransient<CorridasPage>();
        builder.Services.AddTransient<DetalheCorridaPage>();
        builder.Services.AddTransient<RankingPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
