using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cw7
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "dd.MM.yyyy";
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (DateTime.TryParseExact(s, Format, CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime date))
            {
                return date;
            }

            return DateTime.Parse(s,CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value.Hour > 0)
            {
                writer.WriteStringValue(value.ToString("dd.MM.yyyy HH:mm"));
            }
            else
            {
                writer.WriteStringValue(value.ToString("dd.MM.yyyy"));
            }
        }
    }
}
