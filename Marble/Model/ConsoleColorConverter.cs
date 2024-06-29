using System.Text.Json.Serialization;
using System.Text.Json;

namespace Maiswan.Marble;

public class ConsoleColorConverter : JsonConverter<ConsoleColor>
{
    public override ConsoleColor Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => (ConsoleColor)Enum.Parse(typeof(ConsoleColor), reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        ConsoleColor consoleColor,
        JsonSerializerOptions options) => throw new NotImplementedException();
}
