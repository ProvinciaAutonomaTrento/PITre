using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class SearchMandateAssigneeRequestModel : BaseRequestModel
    {
        public Body body;

        public SearchMandateAssigneeRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            public string descrizione;
            public int numMaxResult;

            public Body(string descrizione)
            {
                this.descrizione = descrizione;
                this.numMaxResult = NetworkConstants.NUM_MAX_RESULT;
            }
        }
    }
}
