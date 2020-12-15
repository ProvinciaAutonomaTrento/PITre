using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi avanzati per i documenti dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IDocumentsAdvanced
    {
        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentResponse RemoveDocument(VtDocsWS.Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedResponse SendDocumentAdvanced(VtDocsWS.Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedRequest request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaResponse FatturaEsitoNotifica(VtDocsWS.Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaRequest request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse NuovaFattura(VtDocsWS.Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaRequest request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response CaricaLottoInPi3(VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Request request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.C3GetDocs.C3GetDocsResponse C3GetDocs(VtDocsWS.Services.DocumentsAdvanced.C3GetDocs.C3GetDocsRequest request);

        [OperationContract]
        bool BachecaCircolarePubblicata(VtDocsWS.Services.DocumentsAdvanced.CircolarePubblicata.CircolarePubblicataRequest request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response NuovoLottoAttivo(VtDocsWS.Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Request request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse NuovaFatturaAttiva(VtDocsWS.Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaRequest request);

        [OperationContract]
        VtDocsWS.Services.DocumentsAdvanced.GetModifiedDocumentAdv.GetModifiedDocumentAdvResponse GetModifiedDocumentAdv(VtDocsWS.Services.DocumentsAdvanced.GetModifiedDocumentAdv.GetModifiedDocumentAdvRequest request);


    }
}