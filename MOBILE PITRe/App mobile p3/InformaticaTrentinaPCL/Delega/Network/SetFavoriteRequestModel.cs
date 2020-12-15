using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class SetFavoriteRequestModel : BaseRequestModel
    {
        public Body body;
        public bool isFavorite;
        public SetFavoriteRequestModel(Body body, bool isFavorite, string token)
        {
            this.body = body;
            this.isFavorite = isFavorite;
            this.token = token;
        }

        public class Body : InfoPreferito
        {

            public static Body CreateBodyForPersona(string name, string id){
                return new Body(name, null, id, null, NetworkConstants.TYPE_FAVORITE_URP_P);
            }

            public static Body CreateBodyForRuolo(){
                //TODO
                return new Body();
            }

            public static Body CrateBodyForUnitaOrganizzativa(){
                //TODO  
                return new Body();

            }

            private Body(string descCorrespondent, string idCorrespondent, string idInternal, string idPeopleOwner, string tipoURP)
                : base(descCorrespondent, idCorrespondent, idInternal, idPeopleOwner, tipoURP)
            {
            }

            public Body() : base()
            {

            }
        }
    }
}
