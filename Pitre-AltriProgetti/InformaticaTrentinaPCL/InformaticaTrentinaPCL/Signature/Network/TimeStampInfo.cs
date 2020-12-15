using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Signature.Network
{
    public class TimeStampInfo
    {
        [JsonProperty("TSANameIssuer")]
        public string TsaNameIssuer { get; set; }

        [JsonProperty("TSANameSubject")]
        public string TsaNameSubject { get; set; }

        [JsonProperty("TSdateTime")]
        public DateTime TSdateTime { get; set; }

        [JsonProperty("TSimprint")]
        public string TSimprint { get; set; }

        [JsonProperty("TSserialNumber")]
        public string TSserialNumber { get; set; }

        [JsonProperty("dataFineValiditaCert")]
        public DateTime DataFineValiditaCert { get; set; }

        [JsonProperty("dataInizioValiditaCert")]
        public DateTime DataInizioValiditaCert { get; set; }

        [JsonProperty("TSType")]
        public long TsType { get; set; }

        public TimeStampInfo()
        {
        }
    }
}
