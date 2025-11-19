// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.SearchIpa.Models.Response
{
    public class EnteResponse
    {
        public Result result { get; set; }
        public List<Data> data { get; set; }

    }

    public class Data
    {
        [JsonProperty("tipo_entita")]
        public string TipoEntita { get; set; }

        [JsonProperty("cod_amm")]
        public string CodAmm { get; set; }

        [JsonProperty("des_amm")]
        public string DesAmm { get; set; }

        [JsonProperty("cod_entita")]
        public string CodEntita { get; set; }

        public List<OU> OU { get; set; }


    }

    public class OU
    {
        [JsonProperty("desc_ou")]
        public string DescOu { get; set; }

        [JsonProperty("stato_canale")]
        public string StatoCanale { get; set; }

        [JsonProperty("cod_uni_ou")]
        public string CodUniOu { get; set; }
    }
}
