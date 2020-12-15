using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class FavoritesRequestModel : BaseRequestModel
    {
        public Body body;

        public FavoritesRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            public bool soloPers { get; set; }
            public string tipoPref { get; set; }

            public Body(bool soloPers)
            {
                this.soloPers = soloPers;

            }
        }
    }
}
