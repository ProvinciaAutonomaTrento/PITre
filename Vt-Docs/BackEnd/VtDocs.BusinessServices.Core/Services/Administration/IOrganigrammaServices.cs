using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Administration
{
    /// <summary>
    /// Interfaccia per la gestione dei servizi legati all'organigramma dell'amministrazione
    /// </summary>
    public interface IOrganigrammaServices : VtDocs.BusinessServices.IBusinessService
    {
        /// <summary>
        /// Servizio per il reperimento dei dati di un ruolo in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloResponse GetRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRuoloRequest request);

        /// <summary>
        /// Reperimento delle unità organizzative dell'amministrazione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaResponse GetOrganigramma(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetOrganigrammaRequest request);

        /// <summary>
        /// Servizio di ricerca di elementi in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaResponse Ricerca(VtDocs.BusinessServices.Entities.Administration.Organigramma.RicercaRequest request);

        /// <summary>
        /// Servizio per il reperimento dei responsabili per un utente in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliResponse GetResponsabili(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetResponsabiliRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dati relativi posizione di un utente in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteResponse GetSchedaUtente(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetSchedaUtenteRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dati utente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtenteResponse GetUtente(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtenteRequest request);

        /// <summary>
        /// Servizio per il reperimento degli utenti superiori in organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriResponse GetUtentiSuperiori(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiSuperioriRequest request);

        /// <summary>
        /// Servizio per il reperimento delle UO contenenti almeno un utente con la qualifica specificata
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetListUOByQualificheUtenteResponse GetListUOByQualificheUtente(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetListUOByQualificheUtenteRequest request);

        /// <summary>
        /// Servizio per il reperimento degli utenti in ogranigramma a partire da una UO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiInOrganigrammaResponse GetUtentiInOrganigramma(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiInOrganigrammaRequest request);

        /// <summary>
        /// Servizio per il reperimento degli utenti in organigramma che hanno impostata una determinata qualifica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiConQualificaResponse GetUtentiConQualifica(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUtentiConQualificaRequest request);

        /// <summary>
        /// Servizio per l'inserimento di una qualifica ad un utente in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneQualificaResponse AddAssociazioneQualifica(VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneQualificaRequest request);

        /// <summary>
        /// Servizio per la rimozione dell'associazione di una qualifica ad un utente in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneQualificaResponse RemoveAssociazioneQualifica(VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneQualificaRequest request);

        /// <summary>
        /// Servizio per l'inserimento di un utente in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneUtenteRuoloResponse AddAssociazioneUtenteRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.AddAssociazioneUtenteRuoloRequest request);

        /// <summary>
        /// Servizio per la rimozione di un utente da un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneUtenteRuoloResponse RemoveAssociazioneUtenteRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.RemoveAssociazioneUtenteRuoloRequest request);

        /// <summary>
        /// Servizio per il reperimento delle UO superiori
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUOSuperioriResponse GetUOSuperiori(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetUOSuperioriRequest request);

        /// <summary>
        /// Servizio per il reperimento dei registri associati ad un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRegistriRuoloResponse GetRegistriRuolo(VtDocs.BusinessServices.Entities.Administration.Organigramma.GetRegistriRuoloRequest request);
    }
}
