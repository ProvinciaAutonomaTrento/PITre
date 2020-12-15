using System;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class NewMandateRequestModel:BaseRequestModel
    {
        public Body body;

		public NewMandateRequestModel(Body requestBody, string token)
		{
			this.body = requestBody;
			this.token = token;
		}

		public class Body : DelegaDocumentModel
		{

			public Body(DateTime dataDecorrenza, DateTime dataScadenza, string idDelegato, string idRuoloDelegante, string idRuoloDelegato) 
                : base(dataDecorrenza, dataScadenza, idDelegato, idRuoloDelegante, idRuoloDelegato)
			{
			}
		}

    }
}
