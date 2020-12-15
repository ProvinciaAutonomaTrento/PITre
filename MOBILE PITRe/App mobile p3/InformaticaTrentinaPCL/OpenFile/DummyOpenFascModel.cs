using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.OpenFile.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class DummyOpenFascModel : WS, IOpenFascModel
    {
        public DummyOpenFascModel()
        {
        }

        public async Task<GetFascRicercaResponseModel> GetFascRicerca(GetFascRicercaRequestModel request)
        {
            GetFascRicercaResponseModel response;
            try
            {
                await Task.Delay(1000);
                List<DocInfo> documenti = new List<DocInfo>();
                int pageSize = request.body.pageSize;
                int page = request.body.requestedPage;
                int start = page * pageSize;
                for (int i = start; i < start+pageSize; i++)
                {
                    documenti.Add(new DocInfo("accessRights", true, "01/01/2017", "01/01/2017",
                        new List<string>() {"descr fasc 1", "desc fasc 2"},
                        new List<string>() {"destinatario 1", "destinatario 2"},
                        false, "id_doc_1", "id_doc_pricipale_1", false, true, "mittente", "note", "Oggetto "+(i+1),
                        "originalFileName", "segnatura", "tipoProto"));
                }
               
                RicercaDocumentModel infoElement = new RicercaDocumentModel("id_0011", 2, "Note Fascicolo", "Tipo proto", "Segnatura Fascicolo", "extension", "oggetto del documento");
                List<ResultFascRicerca> results = new List<ResultFascRicerca>() {
                    new ResultFascRicerca(documenti,infoElement)
                };

                response = new GetFascRicercaResponseModel(results, results.Count+1010);
                response.Code = 0;
            }
            catch (Exception e)
            {
                response = new GetFascRicercaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
