using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetFileRequestModel : BaseRequestModel
    {
        public Body body;

        public GetFileRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body{
            public string idDoc { get; set; }

            public Body(string idDoc)
            {
                this.idDoc = idDoc;
            }
        }
    }
}
