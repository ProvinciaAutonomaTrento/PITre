using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.Signature.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Home
{
    public class DummyDocumentListModel : WS, IDocumentListModel
    {

        private readonly int NUMBER_OF_PAGES_TO_SHOW = 5;
        public DummyDocumentListModel()
        {
        }

        public async Task<LoadToDoDocumentsResponseModel> LoadToDoDocuments(LoadToDoDocumentsRequestModel request)
        {
            LoadToDoDocumentsResponseModel response;
            try
            {
                await Task.Delay(3000);
                List<ToDoDocumentModel> documents = new List<ToDoDocumentModel>();

                for (int i = 0; i < NetworkConstants.DEFAULT_LIST_PAGE_SIZE; i++)
                {
                    documents.Add(
                        new ToDoDocumentModel("6865" + i, "2135258" + i, "Oggetto " + (i + 1), "G", null, (i + 1) + "/08/2017", 2, null, "Donaubauer Carmen (Energy and Utility Controller)", "T. HUMAN RESOURCES - DE", false, "-", "68464345484" + i)
                    );
                }
                int totalRecordCount = NetworkConstants.DEFAULT_LIST_PAGE_SIZE * NUMBER_OF_PAGES_TO_SHOW;
                response = new LoadToDoDocumentsResponseModel(documents, totalRecordCount);
            }
            catch (Exception e)
            {
                response = new LoadToDoDocumentsResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<ADLListResponseModel> LoadADLDocuments(ADLListRequestModel request)
        {
            ADLListResponseModel response;
            try
            {
                await Task.Delay(3000);
                List<AdlDocumentModel> documents = new List<AdlDocumentModel>();

                for (int i = 0; i < NetworkConstants.DEFAULT_LIST_PAGE_SIZE; i++)
                {
                    documents.Add(
                        new AdlDocumentModel("id_" + i, 0, "Note " + i, "Tipo proto " + i, "Segnatura " + i, "word", "oggetto " + i, "01/01/2017")
                    );
                }
                int totalRecordCount = NetworkConstants.DEFAULT_LIST_PAGE_SIZE * NUMBER_OF_PAGES_TO_SHOW;

                response = new ADLListResponseModel(documents, totalRecordCount);
            }

            catch (Exception e)
            {
                response = new ADLListResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<LibroFirmaResponseModel> LoadSignDocuments(LibroFirmaRequestModel request)
        {
            LibroFirmaResponseModel response;
            try
            {
                await Task.Delay(3000);
                List<SignDocumentModel> documents = new List<SignDocumentModel>();
                for (int i = 0; i < NetworkConstants.DEFAULT_LIST_PAGE_SIZE; i++)
                {
                    documents.Add(
                        new SignDocumentModel("Id_element_" + i, "Stato firma", "Tipo firma", "Modalità", string.Format("{0}/09/2017", i + 10),
                                              "Ruolo proponente", "Utente proponente",
                                              new SignDocumentInfoModel("Id_document_element_" + 1, "Original file name", "Oggetto", "Note", false, false,
                                                                        string.Format("{0}/09/2017", i + 11), "Tipo proto", "Segnatura", string.Format("{0}/09/2017", i + 11),
                                                                        "Mittente", new List<string>(), new List<string>(), new List<string>(), false, false, "Access rights", "Id_element_" + i),
                                              "Note", "Id_processo_" + i, "Motivo respingimento", "Id_istanza_passo_" + i, "Id_trasm_singola" + i, "Id_utente_titolare" + i, "Id_ruolo_titolare" + i,
                                              string.Format("{0}/09/2017", i + 11), 1024, "Errore firma", false, 0, "Id_" + i, 0)
                    );
                }

                int totalRecordCount = NetworkConstants.DEFAULT_LIST_PAGE_SIZE * NUMBER_OF_PAGES_TO_SHOW;
                response = new LibroFirmaResponseModel(documents, totalRecordCount, true);
            }
            catch (Exception e)
            {
                response = new LibroFirmaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<CondividiResponseModel> Condividi(CondividiRequestModel request)
        {
            CondividiResponseModel response;
            try
            {
                response = new CondividiResponseModel("bUVKogpoXqsUYtdyo7+xbrhrpGZxlpsqbqPKYzKn7bU8faiBd2CpusHLbdkLYQzggHbdNw3Q9ms=");
            }

            catch (Exception e)
            {
                response = new CondividiResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<GetDocumentoCondivisoResponseModel> GetDocumentoCondiviso(GetDocumentoCondivisoRequestModel request)
        {
            GetDocumentoCondivisoResponseModel response;
            try
            {
                await Task.Delay(1000);
                List<DocInfo> allegati = new List<DocInfo>() {
                    new DocInfo("accessRights",true,"01/01/2017","01/01/2017",new List<string>() { "descr fasc 1", "desc fasc 2" }, new List<string>() { "destinatario 1", "destinatario 2" },
                                false,"id_doc_1","id_doc_pricipale_1",false,true,"mittente","note","oggetto","originalFileName","segnatura","tipoProto")
                };
                DocInfo docInfo = new DocInfo("accessRights", true, "01/01/2017", "01/01/2017", new List<string>() { "descr fasc 1", "desc fasc 2" }, new List<string>() { "destinatario 1", "destinatario 2" },
                                              false, "id_doc_1", "id_doc_pricipale_1", false, true, "mittente", "note", "oggetto", "originalFileName", "segnatura", "tipoProto");

                response = new GetDocumentoCondivisoResponseModel(allegati, docInfo);

            }
            catch (Exception e)
            {
                response = new GetDocInfoResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public Task<SignDocumentResponseModel> SignDocumentsHSM(SignDocumentRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<CambiaStatoElementoResponseModel> ChangeDocumentsState(CambiaStatoElementoRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<RespingiElementiResponseModel> RejectDocuments(LibroFirmaRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<FirmaElementiResponseModel> SignDocuments(LibroFirmaRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}