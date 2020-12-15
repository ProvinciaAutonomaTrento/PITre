using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class CondividiRequestModel : BaseRequestModel
    {
        public Body body;

        public CondividiRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {

            public string idPeople { get; set; }
            public string idDocumento { get; set; }

            public Body(string idPeople, string idDocumento){
                this.idPeople = idPeople;
                this.idDocumento = idDocumento;
            }
        }
    }
}
