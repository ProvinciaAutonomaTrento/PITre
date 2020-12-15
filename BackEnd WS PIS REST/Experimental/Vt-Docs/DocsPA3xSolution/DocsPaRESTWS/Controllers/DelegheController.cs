using DocsPaRESTWS.Manager;
using DocsPaRESTWS.Model;
using DocsPaVO.Mobile.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace DocsPaRESTWS.Controllers
{
    [EnableCors("*", "*", "*")]
    public class DelegheController : ApiController
    {
        /// <summary>
        /// Creazione Delega
        /// </summary>
        /// <param name="delega">L'oggetto Delega va popolato nei seguenti campi:
        /// IdRuoloDelegante: può essere popolato con la stringa "TUTTI" per generare una delega per tutti i ruoli. Altrimenti può essere lasciato vuoto: la delega verrà creata per il ruolo con il quale si è fatto l'accesso.
        /// IdDelegato: id dell'utente che viene delegato.
        /// IdRuoloDelegato: id del ruolo che viene delegato. Se omesso sarà delegato il ruolo preferito dell'utente.
        /// DataDecorrenza: parametro data per indicare l'inizio della delega
        /// DataScadenza: parametro data per indicare la fine della delega
        /// </param>
        /// <returns>
        /// Restituisce un Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// 2 - Delega non creata per un errore di creazione
        /// 3 - Nell'arco di tempo selezionato sono presenti delle deleghe sovrapposte
        /// </returns>
        /// <remarks>Metodo utilizzato per la creazione di una delega, fornendo il periodo, e l'id del delegato.</remarks>
        [Route("Delega")]
        [ResponseType(typeof(CreaDelegaResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> creaDelega(DocsPaVO.Mobile.Delega delega)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            CreaDelegaResponse retval = await MobileManager.creaDelega(token, delega);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista delle deleghe assegnate
        /// </summary>
        /// <param name="statoDelega">
        /// statoDelega(string): stato delle deleghe ricercate, valori ammessi
        /// "T" - tutte
        /// "A" - attiva
        /// "I" - impostata
        /// "S" - scaduta
        /// "N" - Attive e impostate
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice zero, restituisce una lista di oggetti Delega, e il numero totale degli elementi presenti.
        /// </returns>
        /// <remarks>Metodo per il prelievo delle deleghe impostate dall'utente verso altri ruoli.</remarks>
        [Route("Deleghe")]
        [ResponseType(typeof(DelegheResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> listaDeleghe(string statoDelega)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            DelegheResponse retval = await MobileManager.listaDelegheAssegnate(token, statoDelega);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Creazione di una delega con un modello
        /// </summary>
        /// <param name="request">Oltre al periodo indicato da DataDecorrenza e DataScadenza, bisogna indicare l'id del modello</param>
        /// <returns>
        /// Restituisce Code:
        /// 0 - OK
        /// 1 - Errore durante la creazione
        /// 2 - Errore Generico
        /// </returns>
        /// <remarks>Metodo che permette la creazione di una delega partendo da un modello preconfigurato</remarks>
        [Route("DelegaDaModello")]
        [ResponseType(typeof(CreaDelegaDaModelloResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> creaDelegaDaModello(CreaDelegaDaModelloRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            CreaDelegaDaModelloResponse retval = await MobileManager.creaDelegaDaModello(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista dei modelli per deleghe
        /// </summary>
        /// <returns>
        /// Restituisce un Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di Code 0, restituisce anche la lista dei modelli di delega
        /// </returns>
        /// <remarks>Metodo per il prelievo della lista dei modelli di delega disponibili per l'utente.</remarks>
        [Route("ModelliDelega")]
        [ResponseType(typeof(ListaModelliDelegaResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getListaModelliDelega()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaModelliDelegaResponse retval = await MobileManager.getListaModelliDelega(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista dei tipi ruolo
        /// </summary>
        /// <returns>
        /// Restituisce un Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di Code 0, restituisce anche la lista dei tipi ruolo
        /// </returns>
        /// <remarks>Metodo per il prelievo della lista dei tipi ruolo.</remarks>
        [Route("ListaTipiRuolo")]
        [ResponseType(typeof(ListaTipiRuoloResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getListaTipiRuolo()
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaTipiRuoloResponse retval = await MobileManager.getListaRuoli(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista degli utenti a partire da un tipo ruolo
        /// </summary>
        /// <param name="codiceTipoRuolo">Codice del tipo ruolo desiderato</param>
        /// <returns>
        /// Restituisce un Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di Code 0, restituisce anche la lista degli utenti a partire dal codice del tipo ruolo
        /// </returns>
        /// <remarks>Metodo per il prelievo della lista degli utenti a partire da un tipo ruolo.</remarks>
        [Route("ListaUtenti")]
        [ResponseType(typeof(ListaUtentiResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getListaUtenti(string codiceTipoRuolo)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaUtentiResponse retval = await MobileManager.getListaUtenti(token, codiceTipoRuolo);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Ricerca degli utenti
        /// </summary>
        /// <param name="descrizione">Parte del nome di un utente</param>
        /// <param name="numMaxResult">Massimo numero dei risultati da restituire</param>
        /// <returns>
        /// Restituisce un Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di Code 0, restituisce anche una lista degli utenti che corrispondono alla descrizione fornita, ed i loro ruoli.
        /// </returns>
        /// <remarks>Metodo per la ricerca di un numero determinato di utenti, fornendo una parte del loro nome. Restituisce anche i ruoli</remarks>
        [Route("RicercaUtenti")]
        [ResponseType(typeof(RicercaUtentiWithRolesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> ricercaUtenti(string descrizione, int numMaxResult)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            RicercaUtentiWithRolesResponse retval = await MobileManager.ricercaUtenti(token, descrizione, numMaxResult);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Revoca Delega
        /// </summary>
        /// <param name="delega">L'oggetto Delega va popolato nei seguenti campi:
        /// IdDelega: Id della delega
        /// DataDecorrenza: parametro data per indicare l'inizio della delega
        /// DataScadenza: parametro data per indicare la fine della delega
        /// </param>
        /// <returns>Valore booleano che indica l'avvenuta revoca</returns>
        /// <remarks>Metodo utilizzato per la revoca di una delega</remarks>
        [Route("RevocaDelega")]
        [ResponseType(typeof(bool))]
        [HttpPost]
        public async Task<HttpResponseMessage> revocaDelega(DocsPaVO.Mobile.Delega delega)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.revocaDelega(token, delega);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}
