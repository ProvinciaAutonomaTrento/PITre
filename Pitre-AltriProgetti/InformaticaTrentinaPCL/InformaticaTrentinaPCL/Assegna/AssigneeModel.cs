using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assign
{
    public class AssigneeModel : AbstractRecipient
	{
		public string ID_Assignable { get; set; }
		public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
		public AssignableTypeEnum AssignableType { get; set; }
        public string IdRuoloDelegato { get; set; }

        public AssigneeModel(string ID_Assignable, string imageUrl, string name, string role, AssignableTypeEnum assignableType)
		{
            this.ID_Assignable = ID_Assignable;
			this.ImageUrl = imageUrl;
			this.Name = name;
			this.Role = role;
			this.AssignableType = assignableType;
		}

		public enum AssignableTypeEnum
		{
			MODELS,
			FAVORITES
		}


		public override string getId()
		{
			return ID_Assignable;
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
			return Name;
		}

		public override string getSubtitle()
		{
			return Role;
		}

		public override bool isPreferred()
		{
			return false;
		}

		public override void setPreferred(bool isFavorite)
		{
			throw new NotImplementedException();
		}

        public override void SetIdRuoloDelegato(string roleId)
        {
            IdRuoloDelegato = roleId;
        }

        public override string GetIdRuoloDelegato()
        {
            throw new NotImplementedException();
        }
    }
}
