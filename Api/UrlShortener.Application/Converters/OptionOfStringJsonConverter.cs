using LanguageExt;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace UrlShortener.Application.Converters;

public class OptionOfStringJsonConverter : JsonConverter<Option<string>>
{
    public override Option<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, Option<string> value, JsonSerializerOptions options)
    {
        value.IfSome(innerValue => writer.WriteStringValue(innerValue));
    }
}