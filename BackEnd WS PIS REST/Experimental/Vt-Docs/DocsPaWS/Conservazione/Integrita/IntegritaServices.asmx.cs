using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaVO.utente;
using DocsPaVO.documento;

namespace DocsPaWS.Conservazione.Integrita
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/Conservazione/IntegritaServices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class IntegritaServices : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetHashDocumentoResponse[] GetHashDocumenti(GetHashDocumentoRequest[] request)
        {
            List<GetHashDocumentoResponse> retval = new List<GetHashDocumentoResponse>();

            foreach (GetHashDocumentoRequest r in request)
                retval.Add(GetHashDocumento(r));

            return retval.ToArray();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public GetHashDocumentoResponse GetHashDocumento(GetHashDocumentoRequest request)
        {
            GetHashDocumentoResponse response = new GetHashDocumentoResponse();
            response.Success = false;
            response.HashAlgo = GetHashDocumentoResponse.AlgoritomoHash.none;
            try
            {
                // Hash repository
                // ottenuto da GetFileFirmato con applicazione algoritmo di hashing (256 oppure no)
                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(request.IdPeople);
                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, null);
                SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity (infoUtente,request.IdDocumento );
                FileRequest fr = sd.documenti[0] as FileRequest;
                if (fr != null)
                {
                    FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
                    string improntaDalDB = string.Empty;
                    using (DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti())
                    {

                        documentiDb.GetImpronta(out improntaDalDB, fr.versionId, request.IdDocumento);
                    }
                    string improntaDalFile = string.Empty;
                    improntaDalFile = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fd.content);
                    response.HashAlgo = GetHashDocumentoResponse.AlgoritomoHash.SHA256;
                    if (improntaDalFile != improntaDalDB)
                    {
                        improntaDalFile = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fd.content);
                        response.HashAlgo = GetHashDocumentoResponse.AlgoritomoHash.SHA1;
                    }
                    response.HashDatabase = improntaDalDB;
                    response.HashRepository = improntaDalFile;
                    response.Success = true;
                }   

                // Hash database
                // facciamo una query scalare

               
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
