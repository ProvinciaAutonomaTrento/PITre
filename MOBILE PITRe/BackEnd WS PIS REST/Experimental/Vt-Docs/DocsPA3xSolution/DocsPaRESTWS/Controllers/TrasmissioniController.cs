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
    public class TrasmissioniController : ApiController
    {
        /// <summary>
        /// Trasmissione Vista
        /// </summary>
        /// <param name="request">
        /// Oggetto request, popolato nel campo:
        /// IdTrasmissione - id della trasmissione per la quale si vuole impostare la data di visualizzazione</param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// 2 - Errore nell'impostazione della data di visualizzazione</returns>
        /// <remarks>Metodo per inserire la data di visualizzazione di una trasmissione</remarks>
        [Route("TrasmissioneVista")]
        [ResponseType(typeof(AccettaRifiutaTrasmResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> setDataVista_SP(TrasmVistaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            AccettaRifiutaTrasmResponse retval = await MobileManager.setDataVistaSP_TV(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Accettazione o Rifiuto di una trasmissione
        /// </summary>
        /// <param name="request">
        /// Oggetto request popolato nei campi:
        /// Action: (string) identifica l'azione. "ACCETTA" per accettare, altrimenti rifiuta. 
        /// Note: (string) Note di accettazione o rifiuto. In caso di rifiuto le note sono obbligatorie.
        /// IdTrasmissioneUtente: (string) l'id della trasmissione utente sulla quale effettuare l'azione. Ha priorità rispetto a idTrasmissione.
        /// IdTrasmissione: (string) nel caso non sia disponibile l'id della trasmissione utente, esso viene ricavato a partire dall'id Trasmissione.
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// 2 - Errore durante l'esecuzione della trasmissione
        /// </returns>
        /// <remarks>Metodo per accettare o rifiutare una trasmissione. Le note sono facoltative in caso di accettazione, obbligatorie in caso di rifiuto.
        /// E' richiesto un solo parametro tra idTrasmissioneUtente e idTrasmissione. Il primo ha priorità ed è preferibile perché più veloce.</remarks>
        [Route("AccettaRifiutaTrasm")]
        [ResponseType(typeof(AccettaRifiutaTrasmResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> AccettaRifiutaTrasm(AccettaRifiutaTrasmRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            AccettaRifiutaTrasmResponse retval = await MobileManager.accettaRifiutaTrasm(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        ///<summary>Smistamento di una trasmissione</summary>
        ///<param name="request">Oggetto request </param>
        ///<returns>Codice: 0 - OK 1 - Errore generico 2 - Errore durante l'esecuzione della trasmissione</returns>
        ///<remarks>Metodo per smistare una trasmissione ricevuta. Lo smistamento accetta la trasmissione ricevuta ed effettua una nuova trasmissione verso un ruolo selezionato.<br/>
        ///E' richiesto un solo parametro tra idTrasmissioneUtente e idTrasmissione. Il primo ha priorità ed è preferibile perché più veloce.<br/>
        ///E' possibile trasmettere tramite modello indicando idModelloTrasm, oppure tramite trasmissione diretta ad un solo ruolo.<br/>
        ///Se presente, l'idModelloTrasm ha priorità rispetto alla trasmissione diretta.<br/>
        ///I parametri IdTrasmissioneUtente, IdTrasmissione, HasWorkflow, IdEvento, IdDoc e IdFasc sono da prelevare dalla todoList, il resto delle informazioni per la trasmissione seguente, sono da selezionare tramite maschera di trasmissione<br/><br/>
        ///Oggetto request popolato nei campi: <br/>
        ///IdTrasmissioneUtente: (string) l'id della trasmissione utente sulla quale effettuare l'azione. Ha priorità rispetto a idTrasmissione.<br/>
        ///IdTrasmissione: (string) nel caso non sia disponibile l'id della trasmissione utente, esso viene ricavato a partire dall'id Trasmissione.<br/>
        ///HasWorkflow: (boolean) Trasmissione ricevuta ha workflow <br/>
        ///IdEvento: (string) Id dell'evento che ha generato la notifica <br/>
        ///IdDoc: (string) Id del documento da spedire<br/>
        ///IdFasc: (string) Id del fascicolo da spedire (solo un parametro tra idDoc e IdFasc. IdDoc ha priorità)<br/>
        ///IdModelloTrasm: (string) id del modello di trasmissione<br/>
        ///Note: (string) note della trasmissione <br/>
        ///Ragione: (string) nome della ragione di trasmissione (COMPETENZA, CONOSCENZA, INOLTRO ETC.)<br/>
        ///Note: (string) note della trasmissione <br/>
        ///IdDestinatario: (string) id del destinatario della trasmissione<br/>
        ///CodiceDestinatario: (string) Codice del destinatario della trasmissione<br/>
        ///Notify: (bool) indica se deve essere inviata la notifica (true in quasi tutti i casi)<br/>
        ///TipoTrasmissione: (string) se popolata con "T" è una trasmissione di tipo a TUTTI, altrimenti è singola.      </remarks>
        [Route("Smista")]
        [ResponseType(typeof(AccettaRifiutaTrasmResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> Smista(SmistaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            AccettaRifiutaTrasmResponse retval = await MobileManager.smista(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista dei modelli di trasmissione
        /// </summary>
        /// <param name="fascicoli">(bool) indica se i modelli di trasmissione riguardano i fascicoli o meno</param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice 0, restituisce una lista di oggetti ModelloTrasm
        /// </returns>
        /// <remarks>Metodo per il prelievo della lista dei modelli di trasmissione disponibili per l'utente, discriminando se essi siano per documenti o fascicoli</remarks>
        [Route("ListaModelliTrasmissione")]
        [ResponseType(typeof(ListaModelliTrasmResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> listaModelliTrasm(bool fascicoli)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaModelliTrasmResponse retval = await MobileManager.getListaModelliTrasm(token, fascicoli);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Trasmissione con modello
        /// </summary>
        /// <param name="request">
        /// Oggetto request, popolato nei campi:
        /// IdDoc: (string) Id del documento da spedire
        /// IdFasc: (string) Id del fascicolo da spedire (solo un parametro tra idDoc e IdFasc. IdDoc ha priorità)
        /// IdModelloTrasm: (string) id del modello di trasmissione
        /// Note: (string) note della trasmissione 
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore Generico</returns>
        /// <remarks>Metodo per l'esecuzione di una trasmissione mediante un modello di trasmissione</remarks>
        [Route("EseguiTrasmissioneModello")]
        [ResponseType(typeof(EseguiTrasmResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> eseguiTrasmModello(EseguiTrasmRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            EseguiTrasmResponse retval = await MobileManager.eseguiTrasm(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Trasmissione
        /// </summary>
        /// <param name="request">
        /// Oggetto Request, popolato nei campi:
        /// IdDoc: (string) Id del documento da spedire
        /// IdFasc: (string) Id del fascicolo da spedire (solo un parametro tra idDoc e IdFasc. IdDoc ha priorità)
        /// Ragione: (string) nome della ragione di trasmissione (COMPETENZA, CONOSCENZA, INOLTRO ETC.)
        /// Note: (string) note della trasmissione 
        /// IdDestinatario: (string) id del destinatario della trasmissione
        /// CodiceDestinatario: (string) Codice del destinatario della trasmissione
        ///  Notify: (bool) indica se deve essere inviata la notifica (true in quasi tutti i casi)
        ///  TipoTrasmissione: (string) se popolata con "T" è una trasmissione di tipo a TUTTI, altrimenti è singola.    
        /// </param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore Generico</returns>
        /// <remarks>Metodo per eseguire una trasmissione diretta verso un utente o un ruolo.</remarks>
        [Route("EseguiTrasmissione")]
        [ResponseType(typeof(EseguiTrasmResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> eseguiTrasmDiretta(EseguiTrasmDirettaRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            EseguiTrasmResponse retval = await MobileManager.eseguiTrasmDiretta(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista corrispondenti veloce
        /// </summary>
        /// <param name="request">
        /// Oggetto request, popolato nei campi:
        /// descrizione - parte del nome del corrispondente da ricercare
        /// ragione - valori ammessi "con" e "comp"
        /// </param>
        /// <returns>
        /// Code:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di code 0, restituisce la lista dei corrispondenti
        /// </returns>
        /// <remarks>Metodo utilizzato per effettuare una ricerca veloce dei corrispondenti</remarks>
        [Route("ListaCorrispondenti")]
        [ResponseType(typeof(RicercaSmistamentoResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> ListaCorrVeloce(GetListaCorrVeloceRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            RicercaSmistamentoResponse retval = await MobileManager.GetListaCorrispondentiVeloce(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista dei ruoli di un utente
        /// </summary>
        /// <param name="idUtente">id dell'utente del quale si vogliono conoscere i ruoli.</param>
        /// <returns>Lista dei ruoli associati ad un utente.</returns>
        /// <remarks>Metodo utilizzato per ottenere la lista dei ruoli associati ad un determinato utente</remarks>
        [Route("ListaRuoliUtente")]
        [ResponseType(typeof(ListaRuoliByUserResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getListaRuoli(string idUtente)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaRuoliByUserResponse retval = await MobileManager.getListaRuoliUser(token, idUtente);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Lista preferiti
        /// </summary>
        /// <param name="soloPers">valore booleano che indica se prelevare solo le persone</param>
        /// <param name="tipoPref">Tipo preferito, A per assegnatari, D per deleghe</param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice zero, restituisce anche la lista dei corrispondenti preferiti da un utente.</returns>
        /// <remarks>Metodo utilizzato per il prelievo dei preferiti di un utente</remarks>
        [Route("ListaPreferiti")]
        [ResponseType(typeof(ListaPreferitiResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> ListaPreferiti(string soloPers = "false", string tipoPref = "A")
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaPreferitiResponse retval = await MobileManager.PrefMobile_getList(token, soloPers, tipoPref);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Inserimento Preferito
        /// </summary>
        /// <param name="infoPref">Oggetto InfoPreferito, popolato nei campi 
        /// IdCorrespondent: il suo id corrispondente, 
        /// IdInternal: il suo idPeople o IdGruppo, 
        /// DescCorrespondent: la sua descrizione,
        /// TipoURP: U per Unità organizzativa, R per ruolo, P per persona
        /// TipoPref: A per assegna, D per delega
        /// </param>
        /// <returns>Valore booleano che testimonia l'avvenuto inserimento o l'errore</returns>
        /// <remarks>Metodo utilizzato per l'inserimento di un corrispondente nei preferiti</remarks>
        [Route("Preferito")]
        [ResponseType(typeof(bool))]
        [HttpPut]
        public async Task<HttpResponseMessage> NuovoPreferito(DocsPaVO.Mobile.InfoPreferito infoPref)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.PrefMobile_insert(token, infoPref);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Cancellazione preferito
        /// </summary>
        /// <param name="infoPref">Oggetto InfoPreferito, popolato nei campi 
        /// SystemId: l'id all'interno della tabella dei preferiti,
        /// IdCorrespondent: il suo id corrispondente, 
        /// IdInternal: il suo idPeople o IdGruppo</param>
        /// <returns>Valore booleano che testimonia l'avvenuto cancellamento o l'errore</returns>
        /// <remarks>Metodo utilizzato per la cancellazione di un utente dai preferiti</remarks>
        [Route("Preferito")]
        [ResponseType(typeof(bool))]
        [HttpDelete]
        public async Task<HttpResponseMessage> RimuoviPreferito(DocsPaVO.Mobile.InfoPreferito infoPref)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            bool retval = await MobileManager.PrefMobile_delete(token, infoPref);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        ///  Lista delle ragioni di trasmissione per un documento
        /// </summary>
        /// <param name="idObject">Id dell'oggetto da trasmettere</param>
        /// <param name="docFasc">D se è un documento, F se fascicolo. Se omesso, default è D</param>
        /// <returns>Codice:
        /// 0 - OK
        /// 1 - Errore generico
        /// In caso di codice zero, restituisce anche la lista delle ragioni di trasmissione per un documento.</returns>
        /// <remarks>Metodo per il prelievo delle ragioni di trasmissione disponibili per un determinato documento</remarks>
        [Route("ListaRagioni")]
        [ResponseType(typeof(ListaRagioniResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> ListaRagioni(string idObject, string docFasc)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ListaRagioniResponse retval = await MobileManager.getListaRagioni(token,idObject,docFasc);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}