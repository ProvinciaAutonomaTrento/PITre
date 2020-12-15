using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaRuoliUserRequestModel : BaseRequestModel
    {
        public Body body;
    
        public ListaRuoliUserRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            public string idUtente { get; set; }

            public Body(string idUtente)
            {
                this.idUtente = idUtente;
            }

        }
    }
}
