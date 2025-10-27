// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
    public class ApparenceType : ValueObject
    {
        [JsonPropertyName("image")]
        public string? Image { get; init; } = null;

        [JsonPropertyName("imageBin")]
        public byte[]? ImageBin { get; init; } = null;

        [JsonPropertyName("imageOnly")]
        public bool? ImageOnly { get; init; } = null;

        [Required]
        [JsonPropertyName("leftx")]
        public int LeftX { get; init; }

        [Required]
        [JsonPropertyName("lefty")]
        public int LeftY { get; init; }

        [JsonPropertyName("location")]
        public string? Location { get; init; } = null!;

        [Required]
        [JsonPropertyName("page")]
        public int Page { get; init; }

        [JsonPropertyName("reason")]
        public string? Reason { get; init; } = null!;

        [Required]
        [JsonPropertyName("rightx")]
        public int RightX { get; init; }

        [Required]
        [JsonPropertyName("righty")]
        public int RightY { get; init; }

        [Required(AllowEmptyStrings = false)]
        [JsonPropertyName("testo")]
        public string Testo { get; init; } = null!;

        [JsonPropertyName("bScaleFont")]
        public bool? ScaleFont { get; init; } = null;

        [JsonPropertyName("bShowDateTime")]
        public bool? ShowDateTime { get; init; } = null;

        [JsonPropertyName("resizeMode")]
        public int? ResizeMode { get; init; } = null;

        [JsonPropertyName("preservePDFA")]
        public bool? PreservePDFA { get; init; } = null;
    }
}
