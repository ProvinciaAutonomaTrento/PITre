using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using DocsPaVO.Mobile.Responses;
using DocsPaRESTWS.Model;
using DocsPaRESTWS.Manager;

namespace DocsPaRESTWS.Controllers
{
    [EnableCors("*", "*", "*")]
    public class FirmaHSMController : ApiController
    {
        /// <summary>
        /// Firma HSM
        /// </summary>
        /// <param name="request">
        /// Oggetto HSMSignRequest, deve essere popolato nei seguenti campi:
        /// IdDoc: (stringa) id del documento da firmare  
        /// cofirma: (bool) se viene richiesta cofirma  
        /// timestamp: (bool) se viene richiesto timestamp
        /// TipoFirma: (string) tipo della firma, valori ammessi "CADES" o "PADES"
        /// AliasCertificato: (string) Alias del certificato
        /// DominioCertificato: (string) Dominio del certificato
        /// OtpFirma: (string) One time password della firma  
        /// PinCertificato: (string) Pin del certificato  
        /// ConvertPdf: (bool) richiesta di coversione in PDF 
        /// </param>
        /// <returns>
        /// Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// 2 - Errore durante l'operazione di firma
        /// </returns>
        /// <remarks>Metodo per effettuare la firma HSM</remarks>
        [Route("FirmaHSM")]
        [ResponseType(typeof(HSMSignResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> HSMSign(HSMSignRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            HSMSignResponse retval = await MobileManager.hsmSign(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Richiesta One Time Password
        /// </summary>
        /// <param name="request">
        /// Oggetto HSMSignRequest, deve essere popolato nei seguenti campi:
        /// AliasCertificato: (string) Alias del certificato
        /// DominioCertificato: (string) Dominio del certificato
        /// </param>
        /// <returns>
        /// Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// </returns>
        /// <remarks>Metodo per la richiesta di OTP per firma HSM</remarks>
        [Route("RichiestaOTP")]
        [ResponseType(typeof(HSMSignResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> HSMSignOTP(HSMSignRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            HSMSignResponse retval = await MobileManager.hsmRequestOTP(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Verifica di firma di un documento
        /// </summary>
        /// <param name="request">
        /// Oggetto HSMSignRequest, deve essere popolato nei seguenti campi:
        /// IdDoc: (stringa) id del documento da firmare  
        /// </param>
        /// <returns>
        /// Code:
        /// 0 - Il documento è firmato
        /// 1 - Errore generico
        /// 2 - Il documento non è firmato
        /// </returns>
        [Route("Verifica")]
        [ResponseType(typeof(HSMSignResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> hsmVerifySign(HSMSignRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            HSMSignResponse retval = await MobileManager.hsmVerifySign(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Dettagli di firma di un documento
        /// </summary>
        /// <param name="request">
        /// Oggetto HSMSignRequest, deve essere popolato nei seguenti campi:
        /// IdDoc: (stringa) id del documento da firmare  
        /// </param>
        /// <returns>
        /// Code:
        /// 0 - OK
        /// 1 - Errore Generico
        /// 2 - Il documento non è firmato
        /// In caso di codice 0, restituisce un oggetto InfoDocFirmato che contiene le informazioni sulla firma del documento
        /// </returns>
        /// <remarks>Metodo per il prelievo delle informazioni sulla firma di un documento</remarks>
        [Route("DettagliDocumento")]
        [ResponseType(typeof(HSMSignResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> hsmInfoSign(HSMSignRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            HSMSignResponse retval = await MobileManager.hsmInfoSign(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Informazioni sul certificato associato ad un utente
        /// </summary>
        /// <returns>
        /// Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice 0, restituisce un oggetto Memento, che contiene al suo interno le informazioni su Alias e Dominio del certificato.
        /// </returns>
        /// <remarks>Metodo per avere informazioni sul certificato associato all'utente</remarks>
        [Route("InfoMemento")]
        [ResponseType(typeof(HSMSignResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> hsmInfoMemento()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            HSMSignResponse retval = await MobileManager.hsmGetMementoForUser(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Verifica abilitazione utente per OTP
        /// </summary>
        /// <returns>
        /// Valore booleano: true se abilitato, falso altrimenti.
        /// </returns>
        /// <remarks>Metodo per verificare se l'utente è abilitato all'utilizzo della One Time Password</remarks>
        [Route("OTPAbilitato")]
        [ResponseType(typeof(bool))]
        [HttpGet]
        public async Task<HttpResponseMessage> isAllowedOTP()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.isAllowedOTP(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}
