// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.SearchIpa.Models
{
    public class AOO
    {
        [JsonProperty("cod_amm")]
        public string CodAmm { get; set; }

        [JsonProperty("cod_uni_aoo")]
        public string CodUniAoo { get; set; }

        [JsonProperty("cod_aoo")]
        public string CodAoo { get; set; }

        [JsonProperty("data_cessazione")]
        public string DataCessazione { get; set; }

        [JsonProperty("des_aoo")]
        public string DesAoo { get; set; }

        [JsonProperty("regione")]
        public string Regione { get; set; }

        [JsonProperty("provincia")]
        public string Provincia { get; set; }

        [JsonProperty("comune")]
        public string Comune { get; set; }

        [JsonProperty("cap")]
        public string Cap { get; set; }

        [JsonProperty("indirizzo")]
        public string Indirizzo { get; set; }

        [JsonProperty("tel")]
        public string Tel { get; set; }

        [JsonProperty("fax")]
        public string Fax { get; set; }

        [JsonProperty("nome_resp")]
        public string NomeResp { get; set; }

        [JsonProperty("cogn_resp")]
        public string CognResp { get; set; }

        [JsonProperty("mail1")]
        public string Mail1 { get; set; }

        [JsonProperty("mail_resp")]
        public string MailResp { get; set; }

        [JsonProperty("tel_resp")]
        public string TelResp { get; set; }

        [JsonProperty("mail2")]
        public string Mail2 { get; set; }


        [JsonProperty("mail3")]
        public string Mail3 { get; set; }
    }
}
