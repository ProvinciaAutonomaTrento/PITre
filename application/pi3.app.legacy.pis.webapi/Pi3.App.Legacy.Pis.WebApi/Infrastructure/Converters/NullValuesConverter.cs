// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Converters
{
    public class NullValuesConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;  // Restituisce il valore predefinito per il tipo T
            }

            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                // Gestisce i valori booleani
                return (T)(object)reader.GetBoolean();
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (typeToConvert == typeof(int))
                {
                    // Gestisce i valori interi
                    return (T)(object)reader.GetInt32();
                }
                else if (typeToConvert == typeof(long))
                {
                    // Gestisce i valori long
                    return (T)(object)reader.GetInt64();
                }
            }

            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            //if (value == null)
            //{
            //    writer.WriteNullValue();
            //    return;
            //}

            // Gestisce i valori booleani
            if (value is bool boolValue)
            {
                writer.WriteBooleanValue(boolValue);
                return;
            }

            // Gestisce i valori interi
            if (value is int intValue)
            {
                writer.WriteNumberValue(intValue);
                return;
            }

            // Gestisce i valori long
            if (value is long longValue)
            {
                writer.WriteNumberValue(longValue);
                return;
            }


            // Gestisce gli oggetti annidati e altri tipi
            JsonSerializer.Serialize(writer, value, options);
        }

       
    }
}
