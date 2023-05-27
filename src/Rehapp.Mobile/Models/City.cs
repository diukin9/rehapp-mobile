using System.Text.Json.Serialization;

namespace Rehapp.Mobile.Models;

public class City
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; } = null!;

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

