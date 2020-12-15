using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class SearchMandateAssignee : AbstractRecipient
    {

        [JsonProperty("Descrizione")]
        public string descrizione { get; set; }

        [JsonProperty("Dst")]
        public string dst { get; set; }

        [JsonProperty("IdAmministrazione")]
        public string idAmministrazione { get; set; }

        [JsonProperty("IdPeople")]
        public string idPeople { get; set; }

        [JsonProperty(PropertyName = "Ruoli")]
        public List<RuoloInfo> ruoli { get; set; }

        [JsonProperty("Token")]
        public string token { get; set; }

        [JsonProperty("UserId")]
        public string userId { get; set; }

        [JsonProperty("Preferito")]
        public bool preferito { get; set; }

        public string IdRuoloDelegato;

        public SearchMandateAssignee()
        {
        }

        public SearchMandateAssignee(string descrizione, string dst, string idAmministrazione, string idPeople, List<RuoloInfo> ruoli, string token, string userId, bool preferito)
        {
            this.descrizione = descrizione;
            this.dst = dst;
            this.idAmministrazione = idAmministrazione;
            this.idPeople = idPeople;
            this.ruoli = ruoli;
            this.token = token;
            this.userId = userId;
            this.preferito = preferito;
        }

        public override string getId()
        {
            return idPeople;
        }

        public override string getIdCorrespondant()
        {
            //TODO
            throw new ArgumentException();
        }

        public override RecipientType getRecipientType()
        {
            return RecipientType.USER;
        }

        public override string getTitle()
        {
            return descrizione;
        }

        public override string getSubtitle()
        {
            return ruoli[0].descrizione;
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
            this.IdRuoloDelegato = roleId;
        }

        public override string GetIdRuoloDelegato()
        {
            return IdRuoloDelegato;
        }
    }
}
