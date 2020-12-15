using System;
using System.Collections.Generic;
using System.Linq;
using InformaticaTrentinaPCL.Home.MVPD;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class ADLListResponseModel : DocumentResponseModel
    {

	    [JsonProperty(PropertyName = "Risultati")]
	    public List<AdlDocumentModel> elements { get; set; }

	    [JsonProperty(PropertyName = "TotalRecordCount")]
	    public int totalRecordCount { get; set; }

	    public ADLListResponseModel(List<AdlDocumentModel> risultati, int totalRecordCount)
	    {
		    this.elements = risultati;
		    this.totalRecordCount = totalRecordCount;
	    }

	    public ADLListResponseModel() { }

	    
	    #region DocumentResponseModel
	    
	    public override List<AbstractDocumentListItem> GetResults()
	    {
		    if (null != elements)
			    return elements.ToList<AbstractDocumentListItem>();
		    
			return new List<AbstractDocumentListItem>();
	    }

	    public override int GetTotalRecordCount()
	    {
		    return totalRecordCount;
	    }
	    
	    #endregion
	}
}
