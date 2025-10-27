// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Newtonsoft.Json;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.SigilloElettronico
{
    public class SignXMLType : ValueObject
    {
        [Required]
        [JsonPropertyName("fileDaFirmare")]
        public byte[] FileDaFirmare { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        [JsonPropertyName("codiceEnteIPA")]
        public string CodiceEnteIPA { get; init; } = null!;

        [JsonPropertyName("codiceAOOIPA")]
        public string? CodiceAOOIPA { get; init; } = null;

        [JsonPropertyName("marcaTemporale")]
        public sbyte? MarcaTemporale { get; init; } = null;

        [JsonPropertyName("tipoFirma")]
        public sbyte? TipoFirma { get; init; } = null;
    }
}
