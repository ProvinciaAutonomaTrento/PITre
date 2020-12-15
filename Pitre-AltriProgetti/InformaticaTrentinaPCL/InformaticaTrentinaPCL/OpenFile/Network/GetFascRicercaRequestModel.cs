using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetFascRicercaRequestModel : BaseRequestModel
    {
        public Body body;

        public GetFascRicercaRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            public string idFascicolo { get; set; }
            public int pageSize { get; set; }
            public int requestedPage { get; set; }
            public string searchString { get; set; }

            public Body(string idFascicolo, int pageSize, int requestedPage, string searchString = null)
            {
                this.idFascicolo = idFascicolo;
                this.pageSize = pageSize;
                this.requestedPage = requestedPage;
                this.searchString = searchString;
            }
        }
    }
}
