namespace F1WeatherStatus.Models;

/// <summary>Escolhe uma cor conforme a temperatura: frio = azul, calor = vermelho.</summary>
public static class CorTemperatura
{
    private const double Frio = 0;    // °C ou menos: azul
    private const double Calor = 35;  // °C ou mais: vermelho

    public static Color Cor(double graus)
    {
        var t = Math.Clamp((graus - Frio) / (Calor - Frio), 0, 1);
        // Matiz de 220° (azul) até 0° (vermelho), passando por ciano, verde, amarelo e laranja.
        var matiz = (1 - t) * 220 / 360.0;
        return Color.FromHsla(matiz, 0.60, 0.45);
    }

    /// <summary>Versão bem clara da cor, para fundos de cartão.</summary>
    public static Color Fundo(double graus)
    {
        var c = Cor(graus);
        return Color.FromHsla(c.GetHue(), 0.60, 0.95);
    }
}
