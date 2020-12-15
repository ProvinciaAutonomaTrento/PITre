using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaRagioniRequestModel : BaseRequestModel
    {
        public Body body;

        public ListaRagioniRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {

            public string idObject { get; set; }
            public string docFasc { get; set; }

            public Body(string idObject, string docFasc)
            {

                this.idObject = idObject;
                this.docFasc = docFasc;
            }

        }
    }
}
