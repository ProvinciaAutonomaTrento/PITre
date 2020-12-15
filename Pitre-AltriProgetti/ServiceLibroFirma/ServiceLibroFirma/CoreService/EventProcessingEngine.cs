using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.LibroFirma;
using DocsPaDB.Query;
using DocsPaUtils;

namespace ServiceLibroFirma.CoreService
{
    class EventProcessingEngine
    {
        #region private field

        private List<Evento> _listEvents;
        private DataLayerEvents _dataLayersEvents = new DataLayerEvents();
        #endregion

        #region public property

        #endregion

        #region public methods

        /// <summary>
        /// Popola la lista degli eventi da processare
        /// </summary>
        /// <returns></returns>
        public void ListToBeProcessed(string[] listOfEventType)
        {
            this._listEvents = _dataLayersEvents.SelectLibroFirmaEventsToBeProcessed(listOfEventType);
        }

        /// <summary>
        /// Delete the closed manual process
        /// </summary>
        /// <returns></returns>
        public void CloseManualProcess()
        {
            _dataLayersEvents.DeleteMAnualProcess();
        }

        /// <summary>
        /// Delete the closed manual process
        /// </summary>
        /// <returns></returns>
        public void ClearDatabase()
        {
            _dataLayersEvents.DeleteAllIgnoredEvent();
        }

        /// <summary>
        /// Creates the list of notifications associated with the event
        /// </summary>
        public void ElaborateListOfEvents(string eventWithoutActor)
        {
            DocsPaVO.utente.Utente utente = null;
            List<string> listElaborati = new List<string>();

            foreach (Evento eventInSession in _listEvents)
            {
                PassoFirma passo = _dataLayersEvents.GetPassoDiFirma(eventInSession, eventWithoutActor);

                if (!string.IsNullOrEmpty(passo.idPasso))
                {
                    LF_Services.ClosePassoAndGetNextRequest getRequest = new LF_Services.ClosePassoAndGetNextRequest();
                    LF_Services.AddElementoInLFRequest addRequest = new LF_Services.AddElementoInLFRequest();

                    if (utente == null || utente.IdPeople != eventInSession.idUtente || utente.Idruolo != eventInSession.idRuolo)
                        utente = ServiceUser(eventInSession.idUtente, eventInSession.idRuolo, passo.isAutomatico);

                    getRequest.UserName = utente.Userid;
                    getRequest.CodeAdm = utente.CodAmm;
                    getRequest.CodeRoleLogin = utente.CodRuolo;
                    getRequest.AuthenticationToken = utente.AuthenticationToken; // "SSO=TEST";
                    getRequest.Delegato = eventInSession.Delegato;

                    getRequest.IdIstanzaPasso = passo.idPasso;
                    getRequest.IdIstanzaProcesso = passo.idProcesso;
                    getRequest.IdVersione = passo.VersionId;
                    getRequest.IdDocumento = passo.idDocumeto;
                    getRequest.OrdinePasso = passo.numeroSequenza;
                    getRequest.DataEsecuzione = eventInSession.DataInserimento;
                    getRequest.DocAll = passo.docAll;

                    LF_Services.LibroFirmaClient LF_Client = new LF_Services.LibroFirmaClient();

                    LF_Services.ClosePassoAndGetNextResponse getResponse = LF_Client.ClosePassoAndGetNext(getRequest);

                    IstanzaPassoDiFirma nextPasso = convertIstanzaPasso(getResponse.IstanzaPasso);
                    if (nextPasso != null)
                    {
                        addRequest.UserName = utente.Userid;
                        addRequest.CodeAdm = utente.CodAmm;
                        addRequest.CodeRoleLogin = utente.CodRuolo;
                        addRequest.AuthenticationToken = utente.AuthenticationToken;
                        addRequest.Delegato = eventInSession.Delegato;

                        addRequest.IdPassoPrecedente = passo.idPasso;
                        addRequest.IdPasso = nextPasso.idIstanzaPasso;
                        addRequest.Modalita = "A";

                        LF_Services.AddElementoInLFResponse addResponse = LF_Client.AddElementoInLF(addRequest);
                    }
                }
                else
                {
                    //Verifico se ci sono eventi di interruzione processo
                }

                listElaborati.Add(eventInSession.idEvento);
            }

            if (listElaborati.Count>0) // && 1==2)
                _dataLayersEvents.DeleteEvent(listElaborati);
        }

        private DocsPaVO.utente.Utente ServiceUser(string idPeople, string idRuolo, bool isUtenteAutomatico)
        {
            DocsPaVO.utente.Utente utente;
            if(isUtenteAutomatico)
                utente = _dataLayersEvents.GetLoggedAutomaticUser(idPeople, idRuolo);
            else
                utente = _dataLayersEvents.GetLoggedUser(idPeople, idRuolo);

            TokenServices.GetAuthenticationTokenRequest tokenRequest = new TokenServices.GetAuthenticationTokenRequest();
            TokenServices.GetAuthenticationTokenResponse tokenResponse = new TokenServices.GetAuthenticationTokenResponse();
            TokenServices.TokenClient tokenClient = new TokenServices.TokenClient();

            tokenRequest.CodeAdm = utente.CodAmm;
            tokenRequest.UserName = utente.Userid;

            tokenResponse = tokenClient.GetAuthenticationToken(tokenRequest);
            utente.AuthenticationToken = tokenResponse.AuthenticationToken;

            logWriter.addLog(logWriter.INFO, (!string.IsNullOrEmpty(utente.AuthenticationToken) ? "Loggato come " + utente.Userid : "Login fallita con idPeople " + idPeople + " e idRuolo " + idRuolo));

            return utente;
        }

        private IstanzaPassoDiFirma convertIstanzaPasso(LF_Services.IstanzaPassoFirma istanzaP)
        {
            IstanzaPassoDiFirma retVal = null;
            
            if (istanzaP != null)
            {
                retVal = new IstanzaPassoDiFirma();
                retVal.CodiceTipoEvento = istanzaP.CodiceTipoEvento;
                retVal.dataEsecuzione = istanzaP.dataEsecuzione;
                retVal.dataScadenza = istanzaP.dataScadenza;
                retVal.descrizioneStatoPasso = istanzaP.descrizioneStatoPasso;
                retVal.idIstanzaPasso = istanzaP.idIstanzaPasso;
                retVal.idIstanzaProcesso = istanzaP.idIstanzaProcesso;
                retVal.idNotificaEffettuata = istanzaP.idNotificaEffettuata;
                retVal.idPasso = istanzaP.idPasso;
                if (istanzaP.statoPasso == TipoStatoPasso.NEW.ToString())
                    retVal.statoPasso = TipoStatoPasso.NEW;
                if (istanzaP.statoPasso == TipoStatoPasso.STUCK.ToString())
                    retVal.statoPasso = TipoStatoPasso.STUCK;
                if (istanzaP.statoPasso == TipoStatoPasso.LOOK.ToString())
                    retVal.statoPasso = TipoStatoPasso.LOOK;
                if (istanzaP.statoPasso == TipoStatoPasso.CLOSE.ToString())
                    retVal.statoPasso = TipoStatoPasso.CLOSE;
                retVal.IdTipoEvento = istanzaP.IdTipoEvento;
                retVal.motivoRespingimento = istanzaP.motivoRespingimento;
                retVal.numeroSequenza = istanzaP.numeroSequenza;
                retVal.IdRuoloCoinvolto = istanzaP.IdRuoloCoinvolto;
                retVal.TipoFirma = istanzaP.TipoFirma;
                retVal.IdUtenteCoinvolto = istanzaP.IdUtenteCoinvolto;
            }

            return retVal;
        }


        #endregion
    }
}

