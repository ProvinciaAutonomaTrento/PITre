using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Delega
{
    public class DummyDelegaModel : WS, IListaDelegheModel
    {
        private readonly int NUMBER_OF_PAGES_TO_SHOW = 5;

        public DummyDelegaModel()
        {
        }

        public async Task<DelegaListResponseModel> GetListaModelliDelega(DelegaListRequestModel request)
        {
            DelegaListResponseModel response;
            try
            {
                await Task.Delay(1000);
                int totalRecordCount = NetworkConstants.DEFAULT_LIST_PAGE_SIZE * NUMBER_OF_PAGES_TO_SHOW;
                response = new DelegaListResponseModel(this.CreateListaModelliDelega(), totalRecordCount);
            }
            catch (Exception e)
            {
                response = new DelegaListResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        private List<DelegaDocumentModel> CreateListaModelliDelega()
        {
            List<DelegaDocumentModel> list = new List<DelegaDocumentModel>();
            for (int i = 0; i < NetworkConstants.DEFAULT_LIST_PAGE_SIZE; i++)
            {
                list.Add(
                    new DelegaDocumentModel(DateTime.Parse("2016-09-27T08:00:00"), DateTime.Parse("2016-09-30T08:00:00"), "0", 2, "Rehders Swen", "14914333",
                                            "TUTTI", new InfoDelegaModel("14922313" + i, "14914333" + i, "Rehders Swen", "", "TUTTI", "", "", "0", "", "", "",
                                                                         DateTime.Parse("2015-04-20T17:59:00").ToReadableString(), DateTime.Parse("2015-04-20T17:59:00").ToReadableString(),
                                                                        "", "0", "0", "", ""), "14922313" + i, "0", "Mapelli Patrizio", "mapellip"));
            }
            return list;
        }

        public async Task<DoRevokeResponseModel> DoRevoke(DoRevokeRequestModel request)
        {
            DoRevokeResponseModel response;
            try
            {
                await Task.Delay(1000);
                response = new DoRevokeResponseModel();
            }
            catch (Exception e)
            {
                response = new DoRevokeResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
