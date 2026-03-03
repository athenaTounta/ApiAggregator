
using System.Text.Json.Serialization;

public class WeatherResponse
{
    public WeatherDescription[]? Weather { get; set; }
    public WeatherData? Main { get; set; }
    public long Dt { get; set; }
    public string? Name { get; set; }
}

public class WeatherData
{
    public decimal Temp { get; set; }
    [JsonPropertyName("feels_like")]
    public decimal FeelsLike { get; set; }
    public int Humidity { get; set; }
}

public class WeatherDescription
{
    public string? Main { get; set; }
    public string? Description { get; set; }
}
