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
    public class RicercaController : ApiController
    {
        /// <summary>
        /// Ricerche Salvate
        /// </summary>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice 0, restituisce la lista delle ricerche salvate per l'utente</returns>
        /// <remarks>Metodo per il prelievo della lista delle ricerche salvate per un utente</remarks>
        [Route("RicercheSalvate")]
        [ResponseType(typeof(GetRicSalvateResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getRicSalvate()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            GetRicSalvateResponse retval = await MobileManager.getRicSalvate(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Ricerca
        /// </summary>
        /// <param name="request">
        /// Oggetto RicercaRequest, popolato nei campi:
        /// PageSize: numero di elementi da visualizzare 
        /// RequestedPage: numero della pagina richiesta
        /// Per le ricerche salvate:
        /// IdRicercaSalvata: id della ricerca salvata
        /// TipoRicercaSalvata: tipo della ricerca salvata. Valore ammesso "F" per indicare i fascicoli, altrimenti cerca nei documenti.
        /// 
        /// Per le ricerche non salvate:
        /// Text: testo da ricercare
        /// DataDa: limite temporale inferiore per la data di creazione del fascicolo o del documento. Formato dd/mm/yyyy
        /// DataA: limite temporale superiore per la data di creazione del fascicolo o del documento. Formato dd/mm/yyyy
        /// DataProtoDa: limite temporale inferiore per la data protocollo. Formato dd/mm/yyyy
        /// DataProtoA: limite temporale superioreper la data protocollo. Formato dd/mm/yyyy
        /// IdDocumento : Id del documento da ricercare
        /// NumProto: numero del protocollo da ricercare
        /// TipoRicerca: tipo della ricerca da effettuare, valori ammessi: 
        /// - "DOC" per cercare tra i documenti
        /// - "FASC" per cercare tra i fascicoli
        /// - "ALL" per cercare sia i documenti che i fascicoli
        /// - "ADL_DOC" per cercare tra i documenti dell'area di lavoro
        /// - "ADL_FASC" per cercare tra i fascicoli dell'area di lavoro
        /// - "ADL_ALL" per cercare sia documenti che fascicoli in area di lavoro
        /// 
        /// Per le ricerche di documento all'interno di un fascicolo o di una sottocartella:
        /// IdFasc: id del fascicolo
        /// IdParentFolder: id della sottocartella
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore Generico
        /// In caso di codice zero, restituisce una lista di oggetti RicercaElement che contengono le informazioni degli oggetti cercati</returns>
        /// <remarks>Metodo per eseguire la ricerca di documenti e fascicoli, anche per quelli in area di lavoro.</remarks>
        [Route("Ricerca")]
        [ResponseType(typeof(RicercaResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> ricerca(RicercaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            RicercaResponse retval = await MobileManager.ricerca(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;

        }

        /// <summary>
        /// Ricerca all'interno di un fascicolo
        /// </summary>
        /// <param name="IdFascicolo">Id del fascicolo</param>
        /// <param name="PageSize">Numero di elementi per pagina</param>
        /// <param name="RequestedPage">Pagina richiesta</param>
        /// <param name="TestoDaCercare">Testo da cercare nell'oggetto del documento</param>
        /// <returns>Codice
        /// 0 - OK
        /// 1 - System Error
        /// In caso di codice zero, restituisce la lista dei documenti all'interno di un fascicolo e delle sue sottocartelle
        /// </returns>
        /// <remarks>Metodo utilizzato per la ricerca dei documenti in un fascicolo e nelle sue sottocartelle.</remarks>
        [Route("FascRicerca")]
        [ResponseType(typeof(RicercaFascResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> ricercaFasc(string IdFascicolo, int PageSize, int RequestedPage, string TestoDaCercare = null)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            RicercaFascResponse retval = await MobileManager.getDocsInFasc(token, IdFascicolo, PageSize, RequestedPage,TestoDaCercare);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;

        }
    }
}
