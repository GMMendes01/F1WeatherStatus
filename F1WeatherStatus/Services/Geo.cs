namespace F1WeatherStatus.Services;

public static class Geo
{
    /// <summary>Distância em km entre dois pontos (fórmula de Haversine).</summary>
    public static double DistanciaKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double raioTerraKm = 6371.0;
        static double Rad(double g) => g * Math.PI / 180.0;

        var dLat = Rad(lat2 - lat1);
        var dLon = Rad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return raioTerraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
