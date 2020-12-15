using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.OpenFile.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class DummyOpenDocumentModel : WS, IOpenDocumentModel
    {
        public DummyOpenDocumentModel()
        {
        }

        public async Task<GetDocInfoResponseModel> GetDocInfo(GetDocInfoRequestModel request)
        {
            GetDocInfoResponseModel response;
            try
            {
                await Task.Delay(1000);
                List<DocInfo> allegati = new List<DocInfo>() {
                    new DocInfo("accessRights",true,"01/01/2017","01/01/2017",new List<string>() { "descr fasc 1", "desc fasc 2" }, new List<string>() { "destinatario 1", "destinatario 2" },
                                false,"id_doc_1","id_doc_pricipale_1",false,true,"mittente","note","oggetto","originalFileName","segnatura","tipoProto")
                };
                DocInfo docInfo = new DocInfo("accessRights", true, "01/01/2017", "01/01/2017", new List<string>() { "descr fasc 1", "desc fasc 2" }, new List<string>() { "destinatario 1", "destinatario 2" },
                                              false, "id_doc_1", "id_doc_pricipale_1", false, true, "mittente", "note", "oggetto", "originalFileName", "segnatura", "tipoProto");
                TrasmInfo trasmInfo = new TrasmInfo(true, "01/01/2017", true, "idTrasm", "idTrasmUtente", "mittente", "noteGenerali", "noteIndividuali", "ragione", false);

                response = new GetDocInfoResponseModel(allegati, docInfo, trasmInfo);

            }
            catch (Exception e)
            {
                response = new GetDocInfoResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<GetFileResponseModel> GetFile(GetFileRequestModel request)
        {
            GetFileResponseModel response;
            try
            {
                await Task.Delay(1000);

                FileInfo fileInfo = new FileInfo("contentFile","contentType","extensionFile","fullName",123456,"name","originalFileName","path");
                response = new GetFileResponseModel(fileInfo);

            }
            catch (Exception e)
            {
                response = new GetFileResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

    }
}
