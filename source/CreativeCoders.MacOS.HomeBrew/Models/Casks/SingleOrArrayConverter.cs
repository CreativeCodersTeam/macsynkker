using System.Text.Json;
using System.Text.Json.Serialization;

namespace CreativeCoders.MacOS.HomeBrew.Models.Casks;

/// <summary>
/// Deserializes a JSON value that may be either a single string or an array of strings into <c>string[]</c>.
/// Homebrew's JSON output uses both forms interchangeably for fields like <c>trash</c>, <c>quit</c>, etc.
/// </summary>
public class SingleOrArrayConverter : JsonConverter<string[]?>
{
    /// <inheritdoc />
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
                return [reader.GetString()!];

            case JsonTokenType.StartArray:
                var items = new List<string>();

                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        items.Add(reader.GetString()!);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }

                return items.ToArray();

            default:
                reader.Skip();
                return null;
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, string[]? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();

        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }
}
