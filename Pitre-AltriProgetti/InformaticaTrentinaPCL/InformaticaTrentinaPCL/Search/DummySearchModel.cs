using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Search.MVP;

using TestRefit.Network;

namespace InformaticaTrentinaPCL.Search
{
    public class DummySearchModel : WS, ISearchModel
    {
        private readonly int NUMBER_OF_PAGES_TO_SHOW = 5;

        public DummySearchModel()
        {
        }

        public async Task<RicercaResponseModel> GetRicercaElementList(RicercaRequestModel request)
        {
            RicercaResponseModel response;
            try
            {
                await Task.Delay(1000);
                int totalRecordCount = NetworkConstants.DEFAULT_LIST_PAGE_SIZE * NUMBER_OF_PAGES_TO_SHOW;
                response = new RicercaResponseModel("", CreateListaModelliDelega(), totalRecordCount);

            }
            catch (Exception e)
            {
                response = new RicercaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        private List<RicercaDocumentModel> CreateListaModelliDelega()
        {
            List<RicercaDocumentModel> list = new List<RicercaDocumentModel>();
            for (int i = 0; i < NetworkConstants.DEFAULT_LIST_PAGE_SIZE; i++)
            {
                list.Add(
                    new RicercaDocumentModel("id", 0, "note", "tipoProto", "segnatura" + i, "extension", "oggtto documento " + i));

            }
            return list;
        }
    }
}
