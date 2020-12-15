using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale.Interfaces
{
    /// <summary>
    /// Interfaccia per la gestione dell'organigramma nel documentale
    /// </summary>
    public interface IOrganigrammaManager
    {
        /// <summary>
        /// Inserimento nuovo ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="computeAtipicita">Flag che indica se bisogna calcolare l'atipicità a seguito dell'estensione dei diritti</param>
        /// <returns></returns>
        EsitoOperazione InserisciRuolo(OrgRuolo ruolo, bool computeAtipicita);

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        EsitoOperazione ModificaRuolo(OrgRuolo ruolo);

        /// <summary>
        /// Informa l'amministratore se ci sono documenti associati a questo ruolo(può essere solo disabilitato)
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo);

        /// <summary>
        /// Cancellazione di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        EsitoOperazione EliminaRuolo(OrgRuolo ruolo);

        /// <summary>
        /// Spostamento di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        EsitoOperazione SpostaRuolo(OrgRuolo ruolo);

        /// <summary>
        /// Impostazione di un ruolo come predefinito
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo);

        /// <summary>
        /// Inserimento di un nuovo utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        EsitoOperazione InserisciUtente(OrgUtente utente);

        /// <summary>
        /// Modifica dei dati di un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        EsitoOperazione ModificaUtente(OrgUtente utente);

        /// <summary>
        /// Elimina un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        EsitoOperazione EliminaUtente(OrgUtente utente);

        /// <summary>
        /// Inserimento di un utente in un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo);

        /// <summary>
        /// Elminiazione di un utente da un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo);

        /// <summary>
        /// Copia visibilità da ruolo a ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="copyVisibility"></param>
        /// <returns></returns>
        EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility);

        /// <summary>
        /// Storicizzazione di un ruolo
        /// </summary>
        /// <param name="role">Ruolo da storicizzare</param>
        /// <returns>Ruolo storicizzato. Attenzione! Consultare la documentazione delle implementazioni per avere informazioni su come di comporta l'algoritmo di storicizzazione</returns>
        OrgRuolo HistoricizeRole(OrgRuolo role);

        /// <summary>
        /// Metodo per l'estensione di visibilità ai ruoli superiori di un ruolo
        /// </summary>
        /// <param name="idAmm">Id dell'amministrazione</param>
        /// <param name="idGroup">Id del gruppo di cui estendere la visibilità</param>
        /// <param name="extendScope">Scope di estensione</param>
        /// <param name="copyIdToTempTable">True se bisogna copiare gli id id dei documenti e fascicoli in una tabella tamporanea per l'allineamento asincrono della visibilità</param>
        /// <returns>Esito dell'operazione</returns>
        EsitoOperazione ExtendVisibilityToHigherRoles(
            String idAmm,
            String idGroup,
            DocsPaVO.amministrazione.SaveChangesToRoleRequest.ExtendVisibilityOption extendScope);

        /// <summary>
        /// Metodo per il calcolo dell'atipicità su sottoposti ed eventualmente sugli oggetti del ruolo
        /// </summary>
        /// <param name="ruolo">Ruolo interessato dal calcolo dell'atipicità</param>
        /// <param name="idTipoRuoloVecchio">Id dell'eventuale vecchio tipo ruolo del ruolo</param>
        /// <param name="idVecchiaUo">Id della eventuale vecchia UO cui apparteneva il ruolo</param>
        /// <param name="calcolaSuiSottoposti">True se è richiesto il calcolo dell'atipicità sui sottoposti</param>
        /// <returns></returns>
        EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, String idTipoRuoloVecchio, String idVecchiaUo, bool calcolaSuiSottoposti);

    }
}