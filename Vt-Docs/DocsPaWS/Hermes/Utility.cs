using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.fascicolazione;
using log4net;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;
using DocsPaVO.ProfilazioneDinamica;


namespace DocsPaWS.Hermes
{
    public class Utility
    {
        private static ILog logger = LogManager.GetLogger(typeof(Hermes));

        public DocsPaVO.utente.Utente Login(DocsPaVO.utente.UserLogin userLogin)
        {
            DocsPaVO.utente.Utente utente = null;
            string idWebSession = string.Empty;
            string ipAddress = string.Empty;
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            try
            {
                utente = BusinessLogic.Utenti.Login.loginMethod(userLogin, out loginResult, true, idWebSession, out ipAddress);
            }
            catch (Exception e)
            {
                logger.Debug("Hermes -  metodo: Login.", e);
            }

            return utente;
        }

        public bool Logoff(DocsPaVO.utente.Utente utente)
        {
            try
            {
                utente = BusinessLogic.Utenti.UserManager.getUtente(utente.idPeople);
                BusinessLogic.Utenti.Login.logoff(utente.userId, utente.idAmministrazione, utente.sessionID, utente.dst);
                BusinessLogic.UserLog.UserLog.WriteLog(utente.userId, null, null, utente.idAmministrazione, "LOGOFF", null, null, DocsPaVO.Logger.CodAzione.Esito.OK, null);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Hermes - metodo:  Logoff.", e);
                return false;
            }
        }

        public Registro[] RegistroGetRegistriRuolo(string idRuolo)
        {

            Registro[] registri = null;
            try
            {

                registri = (Registro[])BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(idRuolo).ToArray(typeof(Registro));
            }
            catch (Exception e)
            {
                logger.Debug("Hermes  - metodo: RegistroGetRegistriRuolo", e);
            }

            return registri;
        }

        public OrgTitolario GetTitolarioAttivo(string idAmministrazione)
        {
            OrgTitolario titolarioAttivo = null;
            try
            {
                OrgTitolario[] titolari = null;
                titolari = (OrgTitolario[])BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(idAmministrazione).ToArray(typeof(OrgTitolario));
                foreach (OrgTitolario ti in titolari)
                {
                    if (ti.Stato.Equals(OrgStatiTitolarioEnum.Attivo))
                    {
                        titolarioAttivo = ti;
                        break;
                    }
                }

                return titolarioAttivo;
            }
            catch (Exception e)
            {
                logger.Debug("Hermes -  metodo:  GetTitolarioAttivo", e);
                return null; ;
            }
        }

        public Classificazione FascicolazioneGetClassificazione(string idAmministrazione, string idGruppo, string idPeople, Registro registro, string codiceClassifica, string idTitolario)
        {
            Classificazione nodo = null;

            try
            {
                Classificazione[] result = (Classificazione[])BusinessLogic.Fascicoli.TitolarioManager.getTitolario2(idAmministrazione, idGruppo, idPeople, registro, codiceClassifica, false, idTitolario).ToArray(typeof(Classificazione));
                if (result.Length > 0)
                    nodo = result[0];

            }
            catch (Exception e)
            {
                logger.Debug("Hermes  - metodo: FascicolazioneGetClassificazione", e);
            }

            return nodo;

        }

        public string FascicolazioneGetFascNumRif(string idTitolario, string idRegistro)
        {

            string numFascicolo = string.Empty;
            try
            {
                numFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(idTitolario, idRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Hermes -  metodo: FascicolazioneGetFascNumRif", e);
            }
            return numFascicolo;
        }

        public Fascicolo FascicolazioneNewFascicolo(Classificazione classificazione, Fascicolo fascicolo, bool enableUffRef, InfoUtente infoUtente, Ruolo ruolo)
        {
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = null;
            DocsPaVO.fascicolazione.ResultCreazioneFascicolo resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK;
            try
            {
                objFascicolo = BusinessLogic.Fascicoli.FascicoloManager.newFascicolo(classificazione, fascicolo, infoUtente, ruolo, enableUffRef, out resultCreazione);
                if (objFascicolo != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLAZIONENEWFASCICOLO", objFascicolo.systemID, string.Format("{0} {1}", "Cod. Fascicolo:", objFascicolo.codice), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR;
                logger.Debug("Hermes  - metodo: FascicolazioneNewFascicolo", e);
                objFascicolo = null;
            }

            return objFascicolo;
        }

        public OggettoCustom salvaValoreOggetto(OggettoCustom _oggettoCustom, string _valore)
        {
            switch (_oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto": _oggettoCustom.VALORE_DATABASE = _valore; break;
                case "Data": _oggettoCustom.VALORE_DATABASE = _valore; break;
                default: _oggettoCustom.VALORE_DATABASE = _valore; break;
            }
            return _oggettoCustom;
        }


        public Fascicolo[] FascicolazioneGetListaFascicoli(DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, InfoUtente infoUtente)
        {
            Fascicolo[] result = null;
            try
            {
                result =(Fascicolo[]) BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoli(infoUtente, classificazione, listaFiltri, enableUfficioRef, enableProfilazione, childs, null, null, String.Empty).ToArray(typeof(Fascicolo));
            }
            catch (Exception e)
            {
                logger.Debug("Hermes  - metodo: FascicolazioneGetListaFascicoli", e);
                result = null;
            }

            return result;
        }



    }
}