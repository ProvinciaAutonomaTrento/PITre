using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna
{
    public class ModelliTrasmissioneModel : AbstractRecipient
    {
		[JsonProperty("Codice")]
		public string codice { get; set; }

		[JsonProperty("Id")]
        public string id { get; set; }

        public ModelliTrasmissioneModel()
        {
        }

        public ModelliTrasmissioneModel(string codice, string id)
        {
            this.codice = codice;
            this.id = id;
        }

        public override string getId()
        {
            return id;
        }

        public override string getIdCorrespondant()
        {
            //TODO Check
            throw  new ArgumentException();
        }

        public override RecipientType getRecipientType()
        {
            return RecipientType.MODEL;
        }

        public override string getTitle()
        {
            return codice;
        }

        public override string getSubtitle()
        {
            //TODO Check
            return null;
        }

        public override bool isPreferred()
        {
            return false;
        }

        public override void setPreferred(bool isFavorite)
        {
            //TODO Check
            throw new NotImplementedException();
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
