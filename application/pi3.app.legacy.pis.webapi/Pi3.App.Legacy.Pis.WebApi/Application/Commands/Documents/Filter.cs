// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public enum FilterTypeEnum
    {
        String,
        Bool,
        Number,
        Date
    }

    public class FilterTypeConverter : JsonConverter<FilterTypeEnum>
    {
        public override FilterTypeEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string enumString = reader.GetString();

                // Confronta le stringhe e converte al valore enum corretto
                if (string.Equals(enumString, "bool", StringComparison.OrdinalIgnoreCase))
                    return FilterTypeEnum.Bool; if (string.Equals(enumString, "boolean", StringComparison.OrdinalIgnoreCase))
                    return FilterTypeEnum.Bool;
                else if (string.Equals(enumString, "string", StringComparison.OrdinalIgnoreCase))
                    return FilterTypeEnum.String;
                else if (string.Equals(enumString, "number", StringComparison.OrdinalIgnoreCase))
                    return FilterTypeEnum.Number;
                else if (string.Equals(enumString, "date", StringComparison.OrdinalIgnoreCase))
                    return FilterTypeEnum.Date;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                // Se è un numero, proviamo a interpretarlo come un intero
                int enumInt = reader.GetInt32();
                if (Enum.IsDefined(typeof(FilterTypeEnum), enumInt))
                    return (FilterTypeEnum)enumInt;
            }

            // Valore di default se non viene riconosciuto
            return FilterTypeEnum.String;
        }

        public override void Write(Utf8JsonWriter writer, FilterTypeEnum value, JsonSerializerOptions options)
        {
            // Puoi decidere come scrivere l'output (stringa o numero)
            writer.WriteNumberValue((int)value);
        }
    }

    /// <summary>
    /// Filtro di ricerca
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Nome del filtro
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del filtro
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Nel caso di template popolare il template con i campi di interesse
        /// </summary>
        public Template Template
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del filtro
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo filtro
        /// </summary>
        public FilterTypeEnum Type
        {
            get;
            set;
        }
    }
}
