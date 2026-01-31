using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartMeal.Infrastructure.Http;

public static class JsonConfiguration
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented = false
    };
}
