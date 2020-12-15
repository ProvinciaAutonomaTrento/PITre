using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega
{
    public class InfoPreferito : AbstractRecipient
    {
        [JsonProperty("DescCorrespondent")]
        public string descCorrespondent { get; set; }

        [JsonProperty("IdCorrespondent")]
        public string idCorrespondent { get; set; } //IdCorrispondente 

        [JsonProperty("IdInternal")]
        public string idInternal { get; set; }

        [JsonProperty("IdPeopleOwner")]
        public string idPeopleOwner { get; set; }

        //[JsonProperty("TipoPref")]
        //public string tipoPref { get; set; }

        [JsonProperty("TipoURP")]
        public string tipoURP { get; set; }

        public string IdRuoloDelegato;
        private bool isFavorite = true;
        
        public InfoPreferito()
        {
        }

        public InfoPreferito(string descCorrespondent, string idCorrespondent, string idInternal, string idPeopleOwner, string tipoURP)
        {
            this.descCorrespondent = descCorrespondent;
            this.idCorrespondent = idCorrespondent;
            this.idInternal = idInternal;
            this.idPeopleOwner = idPeopleOwner;
            this.tipoURP = tipoURP;
        }

        public override string getId()
        {
            return idInternal; //TO CHECK 
        }

        public override RecipientType getRecipientType()
        {
            switch (tipoURP)
            {
                case "P":
                    return RecipientType.USER;
                case "R":
                    return RecipientType.ROLE;
                case "U":
                    return RecipientType.OFFICE;
                default:
                    throw new ArgumentException();
            }

        }

        public override string getIdCorrespondant()
        {
            return idCorrespondent;
        }

        public override string getTitle()
        {
            return descCorrespondent; //TO CHECK 
        }

        public override string getSubtitle()
        {
            return null; //TO CHECK 
        }

        public override bool isPreferred()
        {
            return isFavorite;
        }

        public override void setPreferred(bool isFavorite)
        {
            this.isFavorite = isFavorite;
        }

        public override void SetIdRuoloDelegato(string roleId)
        {
            this.IdRuoloDelegato = roleId;
        }

        public override string GetIdRuoloDelegato()
        {
            return IdRuoloDelegato;
        }
    }
}
