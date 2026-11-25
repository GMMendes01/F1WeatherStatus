using System.Text.Json;
using System.Text.Json.Serialization;

namespace F1WeatherStatus.Services.Dto;

// Classes que espelham o JSON das APIs. Ficam separadas dos modelos do app
// para que mudanças no formato das APIs não se espalhem pelas telas.

// ---------- ipwho.is ----------
public sealed class IpWhoIsResposta
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("ip")] public string? Ip { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("region")] public string? Region { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }
    [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
    [JsonPropertyName("latitude")] public double Latitude { get; set; }
    [JsonPropertyName("longitude")] public double Longitude { get; set; }
    [JsonPropertyName("timezone")] public IpWhoIsFuso? Timezone { get; set; }
    [JsonPropertyName("connection")] public IpWhoIsConexao? Connection { get; set; }
}

public sealed class IpWhoIsFuso
{
    [JsonPropertyName("id")] public string? Id { get; set; }
}

public sealed class IpWhoIsConexao
{
    [JsonPropertyName("isp")] public string? Isp { get; set; }
}

// ---------- Open-Meteo ----------
public sealed class OpenMeteoResposta
{
    [JsonPropertyName("current")] public OpenMeteoAtual? Current { get; set; }
    [JsonPropertyName("daily")] public OpenMeteoDiario? Daily { get; set; }
}

public sealed class OpenMeteoAtual
{
    [JsonPropertyName("time")] public string? Time { get; set; }
    [JsonPropertyName("temperature_2m")] public double Temperature { get; set; }
    [JsonPropertyName("relative_humidity_2m")] public double RelativeHumidity { get; set; }
    [JsonPropertyName("apparent_temperature")] public double ApparentTemperature { get; set; }
    [JsonPropertyName("weather_code")] public int WeatherCode { get; set; }
    [JsonPropertyName("wind_speed_10m")] public double WindSpeed { get; set; }
}

public sealed class OpenMeteoDiario
{
    [JsonPropertyName("time")] public List<string> Time { get; set; } = new();
    [JsonPropertyName("weather_code")] public List<int?> WeatherCode { get; set; } = new();
    [JsonPropertyName("temperature_2m_max")] public List<double?> TemperatureMax { get; set; } = new();
    [JsonPropertyName("temperature_2m_min")] public List<double?> TemperatureMin { get; set; } = new();
    [JsonPropertyName("precipitation_sum")] public List<double?> PrecipitationSum { get; set; } = new();
}

// ---------- Open-Meteo Geocoding ----------
public sealed class GeocodingResposta
{
    // Quando nada é encontrado, a API omite "results".
    [JsonPropertyName("results")] public List<GeocodingItem>? Results { get; set; }
}

public sealed class GeocodingItem
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("latitude")] public double Latitude { get; set; }
    [JsonPropertyName("longitude")] public double Longitude { get; set; }
    [JsonPropertyName("admin1")] public string? Admin1 { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }
}

// ---------- OpenF1 ----------
public sealed class OpenF1Sessao
{
    [JsonPropertyName("session_key")] public int SessionKey { get; set; }
    [JsonPropertyName("meeting_key")] public int MeetingKey { get; set; }
    [JsonPropertyName("location")] public string? Location { get; set; }
    [JsonPropertyName("country_name")] public string? CountryName { get; set; }
    [JsonPropertyName("circuit_short_name")] public string? CircuitShortName { get; set; }
    [JsonPropertyName("date_start")] public DateTimeOffset DateStart { get; set; }
}

public sealed class OpenF1Meeting
{
    [JsonPropertyName("meeting_official_name")] public string? OfficialName { get; set; }
    [JsonPropertyName("circuit_type")] public string? CircuitType { get; set; }
    [JsonPropertyName("circuit_image")] public string? CircuitImage { get; set; }
    [JsonPropertyName("country_flag")] public string? CountryFlag { get; set; }
}

public sealed class OpenF1Resultado
{
    [JsonPropertyName("position")] public int? Position { get; set; }
    [JsonPropertyName("driver_number")] public int DriverNumber { get; set; }
    [JsonPropertyName("number_of_laps")] public int? NumberOfLaps { get; set; }
    [JsonPropertyName("dnf")] public bool Dnf { get; set; }
    [JsonPropertyName("dns")] public bool Dns { get; set; }
    [JsonPropertyName("dsq")] public bool Dsq { get; set; }

    // Podem vir como número, texto ("+1 LAP"), lista (classificação) ou null.
    [JsonPropertyName("duration")] public JsonElement? Duration { get; set; }
    [JsonPropertyName("gap_to_leader")] public JsonElement? GapToLeader { get; set; }
}

public sealed class OpenF1Piloto
{
    [JsonPropertyName("driver_number")] public int DriverNumber { get; set; }
    [JsonPropertyName("full_name")] public string? FullName { get; set; }
    [JsonPropertyName("name_acronym")] public string? NameAcronym { get; set; }
    [JsonPropertyName("team_name")] public string? TeamName { get; set; }
    [JsonPropertyName("team_colour")] public string? TeamColour { get; set; }
}
