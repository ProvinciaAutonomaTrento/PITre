using System;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna
{
    public class CorrispondenteTrasmissioneModel : AbstractRecipient
    {
        [JsonProperty("DescrizioneUtente")]
        public string descrizioneUtente { get; set; }

        [JsonProperty("DescrizioneRuolo")]
        public string descrizioneRuolo { get; set; }

        [JsonProperty("CodiceCorrispondente")]
        public string codiceCorrispondente { get; set; }

        [JsonProperty("DescrizioneUO")]
        public string descrizioneUO { get; set; }

        [JsonProperty("IdUO")]
        public string idUO { get; set; }

        [JsonProperty("Type")]
        public int type { get; set; }

        [JsonProperty("IdRuolo")]
        public string idRuolo { get; set; }

        [JsonProperty("IdUtente")]
        public string idUtente { get; set; }

        [JsonProperty("idCorrispondente")]
        public string idCorrispondente { get; set; }
		
        [JsonProperty("Preferito")]
        public bool preferito { get; set; }

        public CorrispondenteTrasmissioneModel(string descrizioneUtente, string descrizioneRuolo, string codiceCorrispondente, string descrizioneUO, string idUO, int type, string idRuolo, string idUtente, string idCorrispondente, bool preferito=true)
        {
            this.descrizioneUtente = descrizioneUtente;
            this.descrizioneRuolo = descrizioneRuolo;
            this.codiceCorrispondente = codiceCorrispondente;
            this.descrizioneUO = descrizioneUO;
            this.idUO = idUO;
            this.type = type;
            this.idRuolo = idRuolo;
            this.idUtente = idUtente;
            this.idCorrispondente = idCorrispondente;
            this.preferito = preferito;
        }

        public override string getId()
        {
            switch (type)
            {
                case (int)RecipientType.USER:
                    return idUtente;
                case (int)RecipientType.ROLE:
                    return idRuolo;
                case (int)RecipientType.OFFICE:
                    return idUO;
                default:
                    throw new ArgumentException();
            }
        }

		public override string getIdCorrespondant()
		{
			return idCorrispondente;
		}

		public override RecipientType getRecipientType()
        {
            return (RecipientType)type;
        }

        public override string getTitle()
        {
            switch (type)
            {
                case (int)RecipientType.USER:
                    return descrizioneUtente;
                case (int)RecipientType.ROLE:
                    return descrizioneRuolo;
                case (int)RecipientType.OFFICE:
                    return descrizioneUO;
                default:
                    return "";
            }
        }

        public override string getSubtitle()
        {
            // TODO Check : verificare se va mostrata qualche info nella seconda riga, in caso affermativa restituire il valore da visualizzare qui:l'adapter mostrerà l'informazione automaticamente
            return null;
        }

        public override bool isPreferred()
        {
            return preferito;
        }

        public override void setPreferred(bool isFavorite)
        {
            preferito = isFavorite;
        }

        public override void SetIdRuoloDelegato(string roleId)
        {
            throw new NotImplementedException();
        }

        public override string GetIdRuoloDelegato()
        {
            throw new NotImplementedException();
        }
    }
}