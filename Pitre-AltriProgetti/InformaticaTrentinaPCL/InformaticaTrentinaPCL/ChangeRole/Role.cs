using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.ChangeRole
{
	public enum RoleType
	{
		SecretaryForStaff,
		ServiceForThePersonal,
		StaffReader
	}

    public class RuoloInfo : AbstractRecipient
	{
        public RuoloInfo(string id, string code, string desc)
		{
            this.descrizione = desc;
            this.codice = code;
			this.id = id;
		}

		[JsonProperty(PropertyName = "descrizione")]
		public string descrizione { get; set; }

		[JsonProperty(PropertyName = "id")]
		public string id { get; set; }

		[JsonProperty(PropertyName = "Codice")]
		public string codice { get; set; }

        //public string ID_Role { get; set; }
        //public RoleType roleType { get; set; }
        //public string description { get; set; }


		protected bool Equals(RuoloInfo other)
		{
			return string.Equals(id, other.id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RuoloInfo) obj);
		}

		public override int GetHashCode()
		{
			return (id != null ? id.GetHashCode() : 0);
		}

		public override string getId()
		{
			return id;
		}

		public override string getIdCorrespondant()
		{
			return getId();
		}

		public override RecipientType getRecipientType()
		{
			return RecipientType.ROLE;
		}

		public override string getTitle()
		{
			return descrizione;
		}

		public override string getSubtitle()
		{
			return "";
		}

		public override bool isPreferred()
		{
			return false;
			/* do nothing: L'informazione non è presente nella risposta, quindi il valore è ignorato durante il rendering */
		}

		public override void setPreferred(bool isFavorite)
		{
			/* do nothing: L'informazione non è presente nella risposta, quindi il valore è ignorato durante il rendering */
		}
        public override string GetIdRuoloDelegato()
        {
            return id;
        }

        public override void SetIdRuoloDelegato(string roleId)
        {
            throw new NotImplementedException();
        }
    }
}

