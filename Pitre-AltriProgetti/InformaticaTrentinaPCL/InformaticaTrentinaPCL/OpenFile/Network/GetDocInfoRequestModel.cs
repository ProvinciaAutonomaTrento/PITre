using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetDocInfoRequestModel : BaseRequestModel
    {
        public Body body;

        public GetDocInfoRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body{
            
            public string idDoc { get; set; } //Obbligatorio per identificare il documento
            public string idEvento { get; set; } //In caso di assenza di idTrasm consente di rimuovere la notifica dalla todolist
            public string idTrasm { get; set; } //Consente di di prelevare informazioni per la trasmissione 

            public Body(string idDoc)
            {
                this.idDoc = idDoc;
            }
        }
    }
}
