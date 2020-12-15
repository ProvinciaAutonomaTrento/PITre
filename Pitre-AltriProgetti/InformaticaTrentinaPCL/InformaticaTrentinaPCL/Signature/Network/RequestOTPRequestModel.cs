using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Signature.Network
{
    public class RequestOTPRequestModel : BaseRequestModel
    {
        public Body body;

        public RequestOTPRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }
        public class Body
        {
            [JsonProperty("AliasCertificato")]
            public string aliasCertificato { get; set; }

            [JsonProperty("cofirma")]
            public bool cofirma { get; set; }

            [JsonProperty("ConvertPdf")]
            public bool convertPdf { get; set; }

            [JsonProperty("DominioCertificato")]
            public string dominioCertificato { get; set; }

            [JsonProperty("IdDoc")]
            public string idDoc { get; set; }

            [JsonProperty("OtpFirma")]
            public string otpFirma { get; set; }

            [JsonProperty("PinCertificato")]
            public string pinCertificato { get; set; }

            [JsonProperty("timestamp")]
            public bool timestamp { get; set; }

            [JsonProperty("TipoFirma")]
            public string tipoFirma { get; set; }

            public Body(string aliasCertificato, string dominioCertificato, string otpFirma, string pinCertificato, string tipoFirma, bool? cofirma)
            {
                this.aliasCertificato = aliasCertificato;
                this.dominioCertificato = dominioCertificato;
                this.otpFirma = otpFirma;
                this.pinCertificato = pinCertificato;
                this.tipoFirma = tipoFirma;
                if (cofirma != null)
                    this.cofirma = (bool)cofirma;
            }
        }
    }
}
