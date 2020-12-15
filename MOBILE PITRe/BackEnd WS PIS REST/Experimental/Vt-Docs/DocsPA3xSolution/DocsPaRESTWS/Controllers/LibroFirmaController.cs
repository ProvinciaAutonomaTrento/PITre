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
    public class LibroFirmaController : ApiController
    {
        /// <summary>
        /// Elementi del LibroFirma
        /// </summary>
        /// <param name="request">
        /// Oggetto LibroFirmaRequest, popolato nei campi:
        /// RequestedPage (int): pagina richiesta
        /// PageSize (int): dimensione della pagina
        /// Oggetto (string): (facoltativo) filtro ricerca, oggetto del documento
        /// Proponente (string): (facoltativo) filtro ricerca, Proponente del documento
        /// Note (string): (facoltativo) filtro ricerca, Note del documento
        /// IdDocumento (string): (facoltativo) filtro ricerca, id del documento
        /// NumProto (string): (facoltativo) filtro ricerca, numero di protocollo del documento
        /// NumAnnoProto (string): (facoltativo) filtro ricerca, numero e anno di protocollo del documento, formato num-anno (ex. 123-2018)
        /// DataDa (string): (facoltativo) filtro ricerca, formato gg/mm/aaaa, limite inferiore della data creazione del documento
        /// DataA (string): (facoltativo) filtro ricerca, formato gg/mm/aaaa, limite superiore della data creazione del documento
        /// DataProtoDa (string): (facoltativo) filtro ricerca, formato gg/mm/aaaa, limite inferiore della data di protocollazione del documento
        /// DataProtoA (string): (facoltativo) filtro ricerca, formato gg/mm/aaaa, limite superiore della data di protocollazione del documento
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice zero, restituisce un array di LibroFirmaElement, e il numero totale di elementi nel LibroFirma corrispondenti alla ricerca.
        /// </returns>
        /// <remarks>Metodo utilizzato per ottenere gli elementi del libro firma, ed anche per effettuare delle ricerche tra gli stessi.</remarks>
        [Route("LibroFirmaElements")]
        [ResponseType(typeof(LibroFirmaResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> getLibroFirmaElements(LibroFirmaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LibroFirmaResponse retval = await MobileManager.GetLibroFirmaElements(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Cambio Stato di un elemento del Libro Firma
        /// </summary>
        /// <param name="request">
        /// Oggetto LibroFirmaCambiaStatoRequest, campi da valorizzare (i valori dell'oggetto elemento dovrebbero provenire da una precedente ricerca):
        /// elemento.IdElemento : Id dell'elemento da firmare
        /// elemento.DataAccettazione
        ///  elemento.StatoFirma : stato della firma attuale
        ///   elemento.IdIstanzaProcesso 
        ///   elemento.MotivoRespingimento
        /// nuovostato (string): stato in cui si vuole far andare il documento. Valori ammessi: PROPOSTO, DA_FIRMARE, DA_RESPINGERE, FIRMATO, RESPINTO, INTERROTTO, NO_COMPETENZA
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// </returns>
        /// <remarks>Metodo per il cambio dello stato di un documento nel libro Firma</remarks>
        [Route("CambiaStatoElemento")]
        [ResponseType(typeof(LibroFirmaResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> CambiaStatoElementoLibroFirma(LibroFirmaCambiaStatoRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LibroFirmaResponse retval = await MobileManager.CambiaStatoElementoLibroFirma(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Cambio di stato di più elementi, ognuno con il suo stato, in Librofirma
        /// </summary>
        /// <param name="request">
        /// Oggetto LFCambiaStatiRequest. Contiene un array di oggetti ElementToChange da valorizzare nei seguenti campi:
        /// elemento.IdElemento : Id dell'elemento da firmare
        /// elemento.DataAccettazione
        /// elemento.StatoFirma : stato della firma attuale
        /// elemento.IdIstanzaProcesso 
        /// elemento.MotivoRespingimento
        /// nuovostato (string): stato in cui si vuole far andare il documento. Valori ammessi: PROPOSTO, DA_FIRMARE, DA_RESPINGERE, FIRMATO, RESPINTO, INTERROTTO, NO_COMPETENZA
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice 0, restituisce una serie di elementi ElementResult, contenenti l'esito per ogni elemento della richiesta.
        /// </returns>
        /// <remarks>Metodo per il cambio di stato di più elementi, ognuno con il suo stato, nel libro firma</remarks>
        [Route("CambiaStatoElementiMultipli")]
        [ResponseType(typeof(LFCambiaStatiResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> CambiaStatoElementiMultipli(LFCambiaStatiRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LFCambiaStatiResponse retval = await MobileManager.LFCambiaStati(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Respingi elementi del libro firma
        /// </summary>
        /// <param name="request">
        /// Oggetto LibroFirmaRequest, popolato nei campi:
        /// RequestedPage (int): pagina richiesta
        /// PageSize (int): dimensione della pagina
        /// Testo (string): (facoltativo) testo da ricercare, se omesso non stringe la ricerca degli elementi
        /// TipoRicerca (string): (facoltativo) indica in quale campo viene effettuata la ricerca del testo inserito. Valori ammessi "OGGETTO", "NOTE", "PROPONENTE". Se omesso o differente, la ricerca viene effettuata nell'oggetto.
        /// </param>        
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice zero, restituisce un array di LibroFirmaElement, e il numero totale di elementi nel LibroFirma corrispondenti ai parametri inseriti in ingresso.
        /// </returns>
        /// <remarks>Metodo utilizzato per respingere tutti gli elementi del libro firma che sono nello stato DA RESPINGERE. Al termine del processo di firma, restuirà gli elementi relativi ai parametri inseriti</remarks>
        [Route("RespingiElementi")]
        [ResponseType(typeof(LibroFirmaResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> RespingiSelezionatiElementiLf(LibroFirmaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LibroFirmaResponse retval = await MobileManager.RespingiSelezionatiElementiLf(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Firma elementi in Libro Firma
        /// </summary>
        /// <param name="request">
        /// Oggetto LibroFirmaRequest, popolato nei campi:
        /// RequestedPage (int): pagina richiesta
        /// PageSize (int): dimensione della pagina
        /// Testo (string): (facoltativo) testo da ricercare, se omesso non stringe la ricerca degli elementi
        /// TipoRicerca (string): (facoltativo) indica in quale campo viene effettuata la ricerca del testo inserito. Valori ammessi "OGGETTO", "NOTE", "PROPONENTE". Se omesso o differente, la ricerca viene effettuata nell'oggetto.
        /// </param>        
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice zero, restituisce un array di LibroFirmaElement, e il numero totale di elementi nel LibroFirma corrispondenti ai parametri inseriti in ingresso.
        /// </returns>
        /// <remarks>Metodo utilizzato per firmare tutti gli elementi del libro firma che sono nello stato DA FIRMARE. Al termine del processo di firma, restuirà gli elementi relativi ai parametri inseriti</remarks>
        [Route("FirmaElementi")]
        [ResponseType(typeof(LibroFirmaResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> FirmaSelezionatiElementiLf(LibroFirmaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LibroFirmaResponse retval = await MobileManager.FirmaSelezionatiElementiLf(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Verifica presenza elemento Cades
        /// </summary>
        /// <returns>Valore booleano. True se presente almeno un elemento da firmare CADES</returns>
        /// <remarks>Metodo per verificare se in coda di firma è presente un elemento da firmare con firma CADES</remarks>
        [Route("ElementoCades")]
        [ResponseType(typeof(bool))]
        [HttpGet]
        public async Task<HttpResponseMessage> ExistsElementWithSignCades()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.ExistsElementWithSignCades(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Verifica presenza elemento Pades
        /// </summary>
        /// <returns>Valore booleano. True se presente almeno un elemento da firmare PADES</returns>
        /// <remarks>Metodo per verificare se in coda di firma è presente un elemento da firmare con firma PADES</remarks>
        [Route("ElementoPades")]
        [ResponseType(typeof(bool))]
        [HttpGet]
        public async Task<HttpResponseMessage> ExistsElementWithSignPades()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.ExistsElementWithSignPades(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Firma elementi LibroFirma con HSM 
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
        /// Se il code è 0, restituisce una lista di oggetti InfoDocFirmaHSM contenenti l’id del documento, lo status (OK/KO) e l’eventuale messaggio di errore.
        /// </returns>
        /// <remarks>Metodo per effettuare la firma HSM degli elementi del Libro Firma che sono in stato DA_FIRMARE</remarks>
        [Route("FirmaElementiHSM")]
        [ResponseType(typeof(HSMMultiSignResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> HsmMultiSign(HSMSignRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            HSMMultiSignResponse retval = await MobileManager.HsmMultiSign(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Autorizzazione Libro Firma 
        /// </summary>
        /// <returns>Valore booleano: se true, l'utente è abilitato all'utilizzo del libro firma</returns>
        /// <remarks>Metodo per verificare se un utente è autorizzato all'utilizzo del Libro Firma</remarks>
        [Route("AutorizzazioneLibroFirma")]
        [ResponseType(typeof(bool))]
        [HttpGet]
        public async Task<HttpResponseMessage> LibroFirmaIsAuthorized()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.LibroFirmaIsAutorized(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

    }
}
