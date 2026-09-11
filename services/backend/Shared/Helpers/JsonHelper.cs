using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shared.Helpers;

public static class JsonHelper
{
    public static string? ToStringJson(this JsonDocument? jsonDocument)
    {
        if (jsonDocument == null)
        {
            return null;
        }

        return jsonDocument.RootElement.ToString();
    }

    public static bool IsValidJsonString<T>(this string value)
    {
        try
        {
            var jsonObject = JsonConvert.DeserializeObject<T>(value);
            return jsonObject != null;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsInvalidJsonString<T>(this string value)
    {
        return !IsValidJsonString<T>(value);
    }

    public static string? Serialize(object? item)
    {
        if (item == null)
        {
            return null;
        }

        return JsonSerializer.Serialize(item, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
    }

    public static string? Serialize(object? item, JsonSerializerOptions jsonSerializerOptions)
    {
        return item == null ? null : JsonSerializer.Serialize(item, jsonSerializerOptions);
    }

    public static string? PrettySerialize(object? item)
    {
        if (item == null)
        {
            return null;
        }

        return JsonSerializer.Serialize(item, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    public static T? Deserialize<T>(string json, bool isPropertyNameCaseInsensitive = true)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = isPropertyNameCaseInsensitive,
        });
    }
}