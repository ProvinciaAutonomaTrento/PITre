// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.SearchIpa.Models
{
    public class Result
    {
        [JsonProperty("cod_err")]
        public string CodErr { get; set; }

        [JsonProperty("desc_err")]
        public string DescErr { get; set; }

        [JsonProperty("num_items")]
        public int NumItems { get; set; }
    }
}
