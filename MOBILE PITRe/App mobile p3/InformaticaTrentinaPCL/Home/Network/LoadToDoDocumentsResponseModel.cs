using System;
using System.Collections.Generic;
using System.Linq;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class LoadToDoDocumentsResponseModel : DocumentResponseModel
    {
        [JsonProperty(PropertyName = "TotalRecordCount")]
        public int totalRecordCount { get; set; }

        [JsonProperty(PropertyName = "Elements")]
        public List<ToDoDocumentModel> elements { get; set; }

        public LoadToDoDocumentsResponseModel() { }
        public LoadToDoDocumentsResponseModel(List<ToDoDocumentModel> list, int totalRecordCount)
        {
            this.elements = list;
            this.totalRecordCount = totalRecordCount;
        }

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

    }
}
