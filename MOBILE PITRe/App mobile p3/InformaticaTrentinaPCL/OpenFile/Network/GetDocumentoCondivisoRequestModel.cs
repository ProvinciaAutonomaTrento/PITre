using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetDocumentoCondivisoRequestModel : BaseRequestModel
    {
        public Body body;

        public GetDocumentoCondivisoRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            public string chiaveDoc { get; set; }

            public Body(string chiaveDoc)
            {
                this.chiaveDoc = Uri.UnescapeDataString(chiaveDoc);
            }
        }
    }
}
