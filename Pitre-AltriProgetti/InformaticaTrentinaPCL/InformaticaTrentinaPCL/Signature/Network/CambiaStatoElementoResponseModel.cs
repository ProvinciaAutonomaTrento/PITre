using System;
using System.Collections.Generic;
using System.Linq;
using InformaticaTrentinaPCL.Home.MVPD;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class CambiaStatoElementoResponseModel: DocumentResponseModel
    {

        [JsonProperty(PropertyName = "Elementi")]
        public List<ChangeStateElement> Elementi { get; set; }


        public CambiaStatoElementoResponseModel()
        {
           
        } 

        public CambiaStatoElementoResponseModel(List<ChangeStateElement> risultati, int totalRecordCount)
        {
            this.Elementi = risultati;
        }

        #region DocumentResponseModel

        public override List<AbstractDocumentListItem> GetResults()
        {
            //todo da capire come cambia il metodo
            //if (null != Elementi)
                //return Elementi.ToList<AbstractDocumentListItem>();

            return new List<AbstractDocumentListItem>();
        }

        public override int GetTotalRecordCount()
        {
            throw new NotImplementedException();
        }

        #endregion


        public class ChangeStateElement
        {
            public SignDocumentModel Elemento { get; set; }
            public string esito { get; set; }

        }
    }
}
