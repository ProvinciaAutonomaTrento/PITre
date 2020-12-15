using System;
using System.Collections.Generic;
using System.Linq;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class LibroFirmaResponseModel : DocumentResponseModel
    {
        [JsonProperty(PropertyName = "Elements")]
        public List<SignDocumentModel> elements { get; set; }

        [JsonProperty(PropertyName = "TotalRecordCount")]
        public int totalRecordCount { get; set; }

        [JsonProperty(PropertyName = "ModalitaFirmaParallela")]
        bool modalitaFirmaParallela { get; set; }

        public LibroFirmaResponseModel(List<SignDocumentModel> elements, int totalRecordCount, bool modalitaFirmaParallela)
        {
            this.elements = elements;
            this.totalRecordCount = totalRecordCount;
            this.modalitaFirmaParallela = modalitaFirmaParallela;
        }

        public LibroFirmaResponseModel() { }

        #region DocumentResponseModel
        public override List<AbstractDocumentListItem> GetResults()
        {
            if (null != elements)
                return elements.ToList<AbstractDocumentListItem>();
            else
                return new List<AbstractDocumentListItem>();
        }

        public override int GetTotalRecordCount()
        {
            return totalRecordCount;
        }
        #endregion

        public SignFlowType GetSignFlowType()
        {
            return modalitaFirmaParallela ? SignFlowType.PARALLELA : SignFlowType.ANNIDATA;
        }

        public enum SignFlowType
        {
            PARALLELA,
            ANNIDATA
        }
    }
}
