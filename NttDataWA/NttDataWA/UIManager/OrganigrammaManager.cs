using System;
using System.Collections;
using System.Web.SessionState;
using System.Xml;
using NttDataWA.DocsPaWR;
using System.Collections.Generic;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{
	/// <summary>
	/// Summary description for OrganigrammaManager.
	/// </summary>
	public class OrganigrammaManager
	{
		#region Declaration

		private string _idAmministrazione = null;

		private ArrayList _listaUO = null;

        private ArrayList _listaUOInReg = null;

		private ArrayList _listaRuoliUO = null;

		private ArrayList _listaUtenti = null;

		private ArrayList _listaRegistri = null;

		private ArrayList _listaFunzioni = null;

		private OrgTipoRuolo[] _listaTipiRuolo = null;	

		private object[] _listaIDParentRicerca = null;
	
		private ArrayList _listaRisultatoRicerca = null;

        private ArrayList _listaRuoliAOO = null;

		private NttDataWA.DocsPaWR.OrgUtente _datiUtente = null;

		private NttDataWA.DocsPaWR.OrgDettagliGlobali _datiUOStampaBuste = null;

		private NttDataWA.DocsPaWR.EsitoOperazione _esitoOperazione = null;

		private NttDataWA.DocsPaWR.OrgUO _datiUO = null;

		private NttDataWA.DocsPaWR.FileDocumento _filePDF = null;

        private ArrayList _listaRuoli = null;

        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();
	
		#endregion

		#region public

		public OrganigrammaManager()
		{
						
		}		

		public string getIDAmministrazione()
		{
			return this._idAmministrazione;
		}

		public ArrayList getListaUO()
		{
			return this._listaUO;
		}

		public ArrayList getListaRuoliUO()
		{
			return this._listaRuoliUO;
		}

        public ArrayList getListaUOInReg()
        {
            return this._listaUOInReg;
        }

		public ArrayList getListaUtenti()
		{
			return this._listaUtenti;
		}

        public ArrayList getListaRuoli()
        {
            return this._listaRuoli;
        }

        public ArrayList getListaRuoliAOO()
        {
            return this._listaRuoliAOO;
        }

		public ArrayList getListaRegistri()
		{
			return this._listaRegistri;
		}

		public ArrayList getListaFunzioni()
		{
			return this._listaFunzioni;
		}		

		public OrgTipoRuolo[] getListaTipiRuolo()
		{
			return this._listaTipiRuolo;
		}

		public object[] getListaIDParentRicerca()
		{
			return this._listaIDParentRicerca;
		}

		public ArrayList getRisultatoRicerca()
		{
			return this._listaRisultatoRicerca;
		}
		
		public void CurrentIDAmm(string codAmm)
		{
			this.GetIDAmm(codAmm);
		}

		public NttDataWA.DocsPaWR.OrgUO getDatiUO()
		{
			return this._datiUO;
		}

		public NttDataWA.DocsPaWR.OrgUtente getDatiUtente()
		{
			return this._datiUtente;
		}

		public NttDataWA.DocsPaWR.OrgDettagliGlobali getDatiUOStampaBuste()
		{
			return this._datiUOStampaBuste;
		}

		public NttDataWA.DocsPaWR.EsitoOperazione getEsitoOperazione()
		{
			return this._esitoOperazione;
		}

		public void DatiUOCorrente(string idUO)
		{
			this.AmmGetDatiUOCorrente(idUO);
		}

		public void ListaUOLivelloZero(string idAmm)
		{
			this.AmmGetListUO("0","0",idAmm);		
		}

		public void ListaUO(string idParent, string livello, string idAmm)
		{
			this.AmmGetListUO(idParent, livello, idAmm);
		}

        public void ListaUOInReg(string idRegistro, string tipoRicerca, string ricerca)
        {
            this.AmmGetListUOInReg(idRegistro, tipoRicerca, ricerca);
        }

        public void ListaRuoliUO(string idUO)
		{
			this.AmmGetListRuoliUO(idUO);
		}
        public void ListaRuoliUORic(string idUO,bool ricorsivo)
        {
            this.AmmGetListRuoliUORic(idUO,ricorsivo);
        }

		public void ListaUtenti(string idRuolo)
		{
			this.AmmGetListUtentiRuolo(idRuolo);
		}

		public void ListaUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
		{
			this.AmmGetListUtenti(idAmm,ricercaPer,testoDaRicercare,IDesclusi);
		}

        public void ListaRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        {
            this.AmmGetListRuoli(idAmm, ricercaPer, testoDaRicercare, idRegistro, IDesclusi);
        }

		public void ListaRegistriRF(string idAmm, string idRuolo, string chaRF)
		{
			this.AmmGetListRegistri(idAmm,idRuolo, chaRF);
		}

		public void ListaRegistriAssRuolo(string idAmm, string idRuolo)
		{
			this.AmmGetListRegistriAssRuolo(idAmm,idRuolo);
		}

		public void ListaFunzioni(string idAmm, string idRuolo)
		{
			this.AmmGetListFunzioni(idAmm,idRuolo);
		}

		public void ListaTipiRuolo(string idAmm)
		{
			this.AmmGetListTipiRuolo(idAmm);
		}

		public void DatiUtente(string idCorrGlob)
		{
			this.AmmGetDatiUtente(idCorrGlob);
		}

		public void InsNuovaUO(NttDataWA.DocsPaWR.OrgUO newUO)
		{
			this.AmmInsNuovaUO(newUO);
		}

		public void ModUO(NttDataWA.DocsPaWR.OrgUO theUO, bool StoricizzUO)
		{
			this.AmmModUO(theUO, StoricizzUO);
		}

        public NttDataWA.DocsPaWR.Amministrazione ModificaUoTIBCO(string oldCodiceUO, NttDataWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            return this.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);
        }

        public void inviaNotificaMail(NttDataWA.DocsPaWR.OrgUO theUO, NttDataWA.DocsPaWR.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO)
        {
            this.AmmInviaNotificaMail(theUO, amm, descrizioneAOO, tipoOperazione, oldCodiceUO);
        }


        public NttDataWA.DocsPaWR.Amministrazione eliminaUoTIBCO(NttDataWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            return this.AmmEliminaUoTIBCO(theUO, out result);
        }


		public void EliminaUO(NttDataWA.DocsPaWR.InfoUtenteAmministratore infoUtente, string idCorrGlob)
		{
            this.AmmEliminaUO(infoUtente, idCorrGlob);
		}

		public void InsNuovoRuolo(NttDataWA.DocsPaWR.OrgRuolo newRuolo, bool computeAtipicita)
		{
			this.AmmInsNuovoRuolo(newRuolo, computeAtipicita);
		}

		public void ModRuolo(NttDataWA.DocsPaWR.OrgRuolo ruolo)
		{
			this.AmmModRuolo(ruolo);
		}

        public void OnlyDisabledRole(NttDataWA.DocsPaWR.OrgRuolo ruolo)
        {
            this.AmmOnlyDisabledRole(ruolo);
        }

		public int EliminaRuolo(NttDataWA.DocsPaWR.OrgRuolo ruolo)
		{
			this.AmmEliminaRuolo(ruolo);
            return 0;
		}

		public void InsRegistri(NttDataWA.DocsPaWR.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo)
		{
			this.AmmInsRegistri(listaRegistri,idUO,idCorrGlobRuolo);
		}

        public void AssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            this.AmmAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
        }

        public void DeleteAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            this.AmmDeleteAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
        }

		public void InsTipoFunzioni(NttDataWA.DocsPaWR.OrgTipoFunzione[] listaFunzioni)
		{
			this.AmmInsTipoFunzioni(listaFunzioni);
		}

		public void InsUtenteInRuolo(string idPeople, string idGruppo,string idAmm, string type)
		{
			this.AmmInsUtenteInRuolo(idPeople,idGruppo, idAmm, type);
		}

		public void EliminaUtenteInRuolo(string idPeople, string idGruppo, string idAmm)
		{
			this.AmmEliminaUtenteInRuolo(idPeople,idGruppo, idAmm);
		}

		public void EliminaADLUtente(string idPeople, string idCorrGlobGruppo)
		{
			this.AmmEliminaADLUtente(idPeople,idCorrGlobGruppo);
		}

		public void VerificaUtenteLoggato(string userId, string idAmm)
		{
			this.AmmVerificaUtenteLoggato(userId, idAmm);
		}

		public void VerificaTrasmRuolo(string idCorrGlobRuolo)
		{
			this.AmmVerificaTrasmRuolo(idCorrGlobRuolo);
		}

		public void RifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
		{
			this.AmmRifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);
		}

		public void SostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
		{
			this.AmmSostituzioneUtente(idPeopleNewUT,idCorrGlobRuolo);
		}

		public void InsTrasmUtente(string idPeople, string idCorrGlobRuolo)
		{
			this.AmmInsTrasmUtente(idPeople,idCorrGlobRuolo);
		}

        public void RicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
		{
            this.AmmRicercaInOrg(tipo, codice, descrizione, idAmm, searchHistoricized, searchByCodeExact);
		}

		public void ListaIDParentRicerca(string IDPartenza, string tipo)
		{
			this.AmmListaIDParentRicerca(IDPartenza,tipo);
		}

		public void EstendeVisibRuolo(string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string idAmm, string livelloRuolo, bool escludiAtipicita)
		{
            this.AmmEstendeVisibRuolo(idRegistro, idCorrGlobRuolo, idGruppo, idCorrGlobUO, idAmm, livelloRuolo, escludiAtipicita);
		}

		public string GetLivelloRuolo(string idCorrGlobRuolo)
		{
			return this.AmmGetLivelloRuolo(idCorrGlobRuolo);
		}

		public void DettagliUOStampaBuste(string idCorrGlob)
		{
			this.AmmGetDatiUOStampaBuste(idCorrGlob);
		}

		public void SpostaRuolo(NttDataWA.DocsPaWR.OrgRuolo ruolo)
		{
            //this.AmmSpostaRuolo(ruolo);
		}

		public void SpostaUO(NttDataWA.DocsPaWR.OrgUO uoDaSpostare, NttDataWA.DocsPaWR.OrgUO uoPadre)
		{
            //this.AmmSpostaUO(uoDaSpostare,uoPadre);
		}
		
        //public ArrayList GetListaUoFiglie(NttDataWA.DocsPaWR.OrgUO uo)
        //{
        //    //ArrayList list= new ArrayList();

        //    //list = docsPaWS.AmmGetListaUoFiglie(uo);	
        //    //return list;
        //}

		public void StampaOrganigramma (XmlDocument xmlDoc)
		{
            docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            this._filePDF = docsPaWS.StampaOrgInPdf(xmlDoc);
		}

		public NttDataWA.DocsPaWR.FileDocumento getFilePDF()
		{
			return this._filePDF;
		}

		public void setSessionFilePDF(NttDataWA.DocsPaWR.FileDocumento filePdf)
		{
			if (System.Web.HttpContext.Current.Session["SESSION_PDF"]==null)
			{			
				System.Web.HttpContext.Current.Session.Add("SESSION_PDF",filePdf);
			}
            if (System.Web.HttpContext.Current.Session["EXPORT_FILE_SESSION"] == null)
            {
                System.Web.HttpContext.Current.Session.Add("EXPORT_FILE_SESSION", filePdf);
            }
		}

		public NttDataWA.DocsPaWR.FileDocumento getSessionFilePDF()
		{
            NttDataWA.DocsPaWR.FileDocumento filePdf = new NttDataWA.DocsPaWR.FileDocumento();

			if (System.Web.HttpContext.Current.Session["SESSION_PDF"]!=null)
			{			
				filePdf = (NttDataWA.DocsPaWR.FileDocumento) System.Web.HttpContext.Current.Session["SESSION_PDF"];
			}
			return filePdf;
		}

		public void releaseSessionFilePDF()
		{
			System.Web.HttpContext.Current.Session.Remove("SESSION_PDF");
		}

        public void PerformUpDown(string idCorrGlobDaSpostare, string idPesoDaSpostare, string idCorrGlobSubisce, string idPesoSubisce)
        {
            //this.AmmPerformUpDown(idCorrGlobDaSpostare, idPesoDaSpostare, idCorrGlobSubisce, idPesoSubisce);
        }

		#endregion

		#region private

		/// <summary>
		/// Prende ID Amministrazione
		/// </summary>
		/// <param name="codAmm"></param>
		private void GetIDAmm(string codAmm)
		{
            this._idAmministrazione = docsPaWS.AmmGetIDAmm(codAmm);			
		}

		/// <summary>
		/// Reperimento dei dati della UO corrente
		/// </summary>
		/// <param name="idUO">system_id della UO</param>
		private void AmmGetDatiUOCorrente(string idUO)
		{
            this._datiUO = docsPaWS.AmmGetDatiUOCorrente(idUO);
		}

		/// <summary>
		/// Lista UO
		/// </summary>
		/// <param name="idParent"></param>
		/// <param name="livello"></param>
		/// <param name="idAmm"></param>
		private void AmmGetListUO(string idParent, string livello, string idAmm)
		{
            OrgUO[] lista = docsPaWS.AmmGetListUO(idParent, livello, idAmm);

            if (lista!=null && lista.Length > 0)
				this._listaUO = new ArrayList(lista);
		}

        /// <summary>
        /// Lista UO in registro
        /// </summary>
        /// <param name="idRegistro"></param>
        private void AmmGetListUOInReg(string idRegistro, string tipoRicerca, string ricerca)
        {

            OrgUO[] lista = docsPaWS.AmmGetListUOInReg(idRegistro, tipoRicerca, ricerca);

            if (lista!=null && lista.Length > 0)
                this._listaUOInReg = new ArrayList(lista);

        }

		/// <summary>
		/// Lista Ruoli
		/// </summary>
		/// <param name="idUO"></param>
		private void AmmGetListRuoliUO(string idUO)
		{

            OrgRuolo[] lista = docsPaWS.AmmGetListRuoliUO(idUO);

            if (lista!=null && lista.Length > 0)
				this._listaRuoliUO = new ArrayList(lista);

		}

        private void AmmGetListRuoliUORic(string idUO,bool ricorsivo)
        {
            OrgRuolo[] lista = docsPaWS.AmmGetListRuoliUORic(idUO, ricorsivo);

            if (lista!=null && lista.Length > 0)
                this._listaRuoliUO = new ArrayList(lista);
        }

		/// <summary>
		/// Lista utenti del ruolo
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListUtentiRuolo(string idRuolo)
		{
			OrgUtente[] lista = docsPaWS.AmmGetListUtentiRuolo(idRuolo);

            if (lista!=null && lista.Length > 0)
				this._listaUtenti = new ArrayList(lista);;
		}

		private void AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
		{

            OrgUtente[] lista = docsPaWS.AmmGetListUtenti(idAmm, ricercaPer, testoDaRicercare, IDesclusi);

            if (lista!=null && lista.Length > 0)
				this._listaUtenti = new ArrayList(lista);
		}

        private void AmmGetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        {
            OrgRuolo[] lista = docsPaWS.AmmGetListRuoli(idAmm, ricercaPer, testoDaRicercare, idRegistro, IDesclusi);

            if (lista!=null && lista.Length > 0)
                this._listaRuoli = new ArrayList(lista);
        }

		/// <summary>
		/// Lista Registri
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
        private void AmmGetListRegistri(string idAmm, string idRuolo, string chaRF)
		{
            OrgRegistro[] lista = docsPaWS.AmmGetListRegistriRF(idAmm, idRuolo, chaRF);

            if (lista!=null && lista.Length > 0)
				this._listaRegistri = new ArrayList(lista);
		}

		/// <summary>
		/// Lista Registri associati al ruolo
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListRegistriAssRuolo(string idAmm, string idRuolo)
		{
            OrgRegistro[] lista = docsPaWS.AmmGetListRegistriAssRuolo(idAmm, idRuolo);

			if(lista!=null && lista.Length>0)
				this._listaRegistri = new ArrayList(lista);

		}

		/// <summary>
		/// Lista Funzioni
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListFunzioni(string idAmm, string idRuolo)
		{

            OrgTipoFunzione[] lista = docsPaWS.AmmGetListFunzioni(idAmm, idRuolo);

			if(lista!=null && lista.Length>0)
				this._listaFunzioni = new ArrayList(lista);
		}

		/// <summary>
		/// Lista Tipi Ruolo
		/// </summary>
		/// <param name="idAmm"></param>
		private void AmmGetListTipiRuolo(string idAmm)
		{

            this._listaTipiRuolo = docsPaWS.AmmGetListTipiRuolo(idAmm);
		}

		private void AmmGetDatiUtente(string idCorrGlob)
		{

            this._datiUtente = docsPaWS.AmmGetDatiUtente(idCorrGlob);
		}

		private void AmmInsNuovaUO(NttDataWA.DocsPaWR.OrgUO newUO)
		{
            this._esitoOperazione = docsPaWS.AmmInsNuovaUO(UIManager.UserManager.GetInfoUser(), newUO);
		}

		private void AmmModUO(NttDataWA.DocsPaWR.OrgUO theUO, bool StoricizzUO)
		{
            this._esitoOperazione = docsPaWS.AmmModUO(theUO, StoricizzUO);
		}

        private NttDataWA.DocsPaWR.Amministrazione AmmModificaUoTIBCO(string oldCodiceUO, NttDataWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            NttDataWA.DocsPaWR.Amministrazione amm = new NttDataWA.DocsPaWR.Amministrazione();
            amm = docsPaWS.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);
            return amm;
        }

        private NttDataWA.DocsPaWR.Amministrazione AmmEliminaUoTIBCO(NttDataWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            NttDataWA.DocsPaWR.Amministrazione amm = new NttDataWA.DocsPaWR.Amministrazione();
            amm = docsPaWS.AmmEliminaUoTIBCO(theUO, out result);
            return amm;
        }



        private void AmmInviaNotificaMail(NttDataWA.DocsPaWR.OrgUO theUO, NttDataWA.DocsPaWR.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO)
        {

            docsPaWS.inviaNotificaMail(theUO, amm, descrizioneAOO, tipoOperazione, oldCodiceUO);
        }

		private void AmmEliminaUO(NttDataWA.DocsPaWR.InfoUtente infoUtente, string idCorrGlob)
		{
            this._esitoOperazione = docsPaWS.AmmEliminaUO(infoUtente, idCorrGlob);
		}

		private void AmmInsNuovoRuolo(NttDataWA.DocsPaWR.OrgRuolo newRuolo, bool computeAtipicita)
		{
            this._esitoOperazione = docsPaWS.AmmInsNuovoRuolo(UIManager.UserManager.GetInfoUser(), newRuolo, computeAtipicita);
		}

		private void AmmModRuolo(NttDataWA.DocsPaWR.OrgRuolo ruolo)
		{
            this._esitoOperazione = docsPaWS.AmmModRuolo(UIManager.UserManager.GetInfoUser(), ruolo);
		}

        private void AmmOnlyDisabledRole(NttDataWA.DocsPaWR.OrgRuolo ruolo)
        {
            this._esitoOperazione = docsPaWS.AmmOnlyDisabledRole(UIManager.UserManager.GetInfoUser(),ruolo);
        }

		private void AmmEliminaRuolo(NttDataWA.DocsPaWR.OrgRuolo ruolo)
		{
            this._esitoOperazione = docsPaWS.AmmEliminaRuolo(UIManager.UserManager.GetInfoUser(), ruolo);
		}

		private void AmmInsRegistri(NttDataWA.DocsPaWR.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo)
		{
            this._esitoOperazione = docsPaWS.AmmInsRegistri(listaRegistri, idUO, idCorrGlobRuolo);
		}

        private void AmmAssociazioneRFRuolo(string idRf,  string idCorrGlobRuolo)
        {
            this._esitoOperazione = docsPaWS.AmmAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
        }

        private void AmmDeleteAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            this._esitoOperazione = docsPaWS.AmmDeleteAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
        }


		private void AmmInsTipoFunzioni(NttDataWA.DocsPaWR.OrgTipoFunzione[] listaFunzioni)
		{
            this._esitoOperazione = docsPaWS.AmmInsTipoFunzioni(UIManager.UserManager.GetInfoUser(),listaFunzioni);
		}

		private void AmmInsUtenteInRuolo(string idPeople, string idGruppo, string idAmm, string type)
		{
            this._esitoOperazione = docsPaWS.AmmInsUtenteInRuolo(UIManager.UserManager.GetInfoUser(), idPeople, idGruppo, idAmm, type);
		}

		private void AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo)
		{
            this._esitoOperazione = docsPaWS.AmmInsTrasmUtente(idPeople, idCorrGlobRuolo);
		}

		private void AmmEliminaUtenteInRuolo(string idPeople, string idGruppo, string idAmm)
		{
            this._esitoOperazione = docsPaWS.AmmEliminaUtenteInRuolo(UIManager.UserManager.GetInfoUser(), idPeople, idGruppo, idAmm);
		}

		private void AmmVerificaUtenteLoggato(string userId, string idAmm)
		{
            this._esitoOperazione = docsPaWS.AmmVerificaUtenteLoggato(userId, idAmm);
		}
		
		private void AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo)
		{
            this._esitoOperazione = docsPaWS.AmmEliminaADLUtente(idPeople, idCorrGlobGruppo);
		}

		private void AmmVerificaTrasmRuolo(string idCorrGlobRuolo)
		{
            this._esitoOperazione = docsPaWS.AmmVerificaTrasmRuolo(idCorrGlobRuolo);
		}

		private void AmmRifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
		{
            this._esitoOperazione = docsPaWS.AmmRifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);
		}

		private void AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
		{
            this._esitoOperazione = docsPaWS.AmmSostituzioneUtente(idPeopleNewUT, idCorrGlobRuolo);
		}

        private void AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
		{
            OrgRisultatoRicerca[] lista = docsPaWS.AmmRicercaInOrg(tipo, codice, descrizione, idAmm, searchHistoricized, searchByCodeExact);

            if (lista!=null && lista.Length > 0)
				this._listaRisultatoRicerca = new ArrayList(lista);

			lista = null;
		}

		private void AmmListaIDParentRicerca(string IDPartenza, string tipo)
		{
            object[] lista = docsPaWS.AmmListaIDParentRicerca(IDPartenza, tipo);


            if (lista != null && lista.Length > 0)
                this._listaIDParentRicerca = lista;

		}

		private void AmmEstendeVisibRuolo(string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string idAmm, string livelloRuolo, bool escludiAtipicita)
		{
            //this._esitoOperazione = docsPaWS.AmmEstendeVisibRuolo(idRegistro, idCorrGlobRuolo, idGruppo, idCorrGlobUO, idAmm, livelloRuolo, escludiAtipicita);	
		}

		private string AmmGetLivelloRuolo(string idCorrGlobRuolo)
		{
			return docsPaWS.GetLivelloTipoRuolo(idCorrGlobRuolo);
		}

		private void AmmGetDatiUOStampaBuste(string idCorrGlob)
		{
            //this._datiUOStampaBuste = docsPaWS.AmmGetDatiUOStampaBuste(idCorrGlob);
		}

       // private void AmmSpostaRuolo(NttDataWA.DocsPaWR.OrgRuolo ruolo)
       // {
       //     this._esitoOperazione = docsPaWS.AmmSpostaRuolo(ruolo);
       // }

       // private void AmmSpostaUO(NttDataWA.DocsPaWR.OrgUO uoDaSpostare, NttDataWA.DocsPaWR.OrgUO uoPadre)
       // {
       //     this._esitoOperazione = ws.AmmSpostaUO(uoDaSpostare,uoPadre);
       //     ws = null;
       // }

       // private void AmmStampaOrganigramma(XmlDocument xmlDoc)
       // {			
       //     UIManager.WebServiceLink ws = new UIManager.WebServiceLink();
       //     this._filePDF = ws.AmmStampaOrganigramma(xmlDoc);
       //     ws = null;
       // }
        
       // public void GetListaRuoliAOO(string idRegistro)
       // {
       //     this.AmmGetListaRuoliAOO(idRegistro);
       // }

       ///// <summary>
       // /// Ritorna tutti i ruoli associati ad una determinata AOO (Registro o RF)
       ///// </summary>
       ///// <param name="idRegistro"></param>
       // private void AmmGetListaRuoliAOO(string idRegistro)
       // {
       //     UIManager.WebServiceLink ws = new UIManager.WebServiceLink();

       //     ArrayList lista = ws.AmmGetListaRuoliAOO(idRegistro);

       //     if (lista.Count > 0)
       //         this._listaRuoliAOO = new ArrayList(lista);

       //     lista = null;

       //     ws = null;
       // }

       // private void AmmPerformUpDown(string idCorrGlobDaSpostare, string idPesoDaSpostare, string idCorrGlobSubisce, string idPesoSubisce)
       // {
       //     UIManager.WebServiceLink ws = new UIManager.WebServiceLink();
       //     this._esitoOperazione = ws.AmmOrdinamento(idCorrGlobDaSpostare, idPesoDaSpostare, idCorrGlobSubisce, idPesoSubisce);
       //     ws = null;
       // }

		#endregion

        public SaveChangesToRoleResponse SaveChangesToRole(SaveChangesToRoleRequest request)
        {
            return docsPaWS.SaveChangesToRole(request);
        }

        public OrgRuolo GetRole(String idCorrGlobali)
        {
            try
            {
                return docsPaWS.GetRole(idCorrGlobali);
                
            }
            catch (Exception e)
            {
                return null;
                
            }
        }

        public bool CheckCodiceUODuplicato(string id, string codice, string idAmm)
        {
            try
            {
                bool result = false;
                DocsPaWebService wws = new DocsPaWebService();
                result = wws.ammCheckCodiceUODuplicato(id, codice, idAmm);
                return result;

            }
            catch (Exception e)
            {
                return false;

            }

        }

        public static OrgRuolo[] AmmGetListaRuoliAOO(string rf) {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            return ws.AmmGetListaRuoliAOO(rf);
        }

	}
}
