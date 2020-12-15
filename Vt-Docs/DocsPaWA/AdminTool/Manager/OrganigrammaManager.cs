using System;
using System.Collections;
using System.Web.SessionState;
using System.Xml;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;

namespace Amministrazione.Manager
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

		private ArrayList _listaTipiRuolo = null;	

		private ArrayList _listaIDParentRicerca = null;
	
		private ArrayList _listaRisultatoRicerca = null;

        private ArrayList _listaRuoliAOO = null;

		private DocsPAWA.DocsPaWR.OrgUtente _datiUtente = null;

		private DocsPAWA.DocsPaWR.OrgDettagliGlobali _datiUOStampaBuste = null;

		private DocsPAWA.DocsPaWR.EsitoOperazione _esitoOperazione = null;

		private DocsPAWA.DocsPaWR.OrgUO _datiUO = null;

		private DocsPAWA.DocsPaWR.FileDocumento _filePDF = null;

        private ArrayList _listaRuoli = null;
	
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

		public ArrayList getListaTipiRuolo()
		{
			return this._listaTipiRuolo;
		}

		public ArrayList getListaIDParentRicerca()
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

		public DocsPAWA.DocsPaWR.OrgUO getDatiUO()
		{
			return this._datiUO;
		}

		public DocsPAWA.DocsPaWR.OrgUtente getDatiUtente()
		{
			return this._datiUtente;
		}

		public DocsPAWA.DocsPaWR.OrgDettagliGlobali getDatiUOStampaBuste()
		{
			return this._datiUOStampaBuste;
		}

		public DocsPAWA.DocsPaWR.EsitoOperazione getEsitoOperazione()
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

		public void InsNuovaUO(DocsPAWA.DocsPaWR.OrgUO newUO)
		{
			this.AmmInsNuovaUO(newUO);
		}

		public void ModUO(DocsPAWA.DocsPaWR.OrgUO theUO, bool StoricizzUO)
		{
			this.AmmModUO(theUO, StoricizzUO);
		}

        public DocsPAWA.DocsPaWR.Amministrazione ModificaUoTIBCO(string oldCodiceUO, DocsPAWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            return this.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);
        }

        public void inviaNotificaMail(DocsPAWA.DocsPaWR.OrgUO theUO, DocsPAWA.DocsPaWR.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO)
        {
            this.AmmInviaNotificaMail(theUO, amm, descrizioneAOO, tipoOperazione, oldCodiceUO);
        }


        public DocsPAWA.DocsPaWR.Amministrazione eliminaUoTIBCO(DocsPAWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            return this.AmmEliminaUoTIBCO(theUO, out result);
        }


		public void EliminaUO(DocsPAWA.DocsPaWR.InfoUtenteAmministratore infoUtente, string idCorrGlob)
		{
            this.AmmEliminaUO(infoUtente, idCorrGlob);
		}

		public void InsNuovoRuolo(DocsPAWA.DocsPaWR.OrgRuolo newRuolo, bool computeAtipicita)
		{
			this.AmmInsNuovoRuolo(newRuolo, computeAtipicita);
		}

		public void ModRuolo(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			this.AmmModRuolo(ruolo);
		}

        public void OnlyDisabledRole(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
        {
            this.AmmOnlyDisabledRole(ruolo);
        }

		public int EliminaRuolo(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			this.AmmEliminaRuolo(ruolo);
            return 0;
		}

		public void InsRegistri(DocsPAWA.DocsPaWR.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo)
		{
			this.AmmInsRegistri(listaRegistri,idUO,idCorrGlobRuolo);
		}

        public bool ExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPAWA.DocsPaWR.RightRuoloMailRegistro[] listaRegistri, string idCorrGlobRuolo, string idGruppo)
        {
            return this.AmmExistsPassiFirmaByRuoloTitolareAndRegistro(listaRegistri, idCorrGlobRuolo, idGruppo);
        }

        public bool ExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            return this.AmmExistsPassiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
        }

        public bool InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, InfoUtente infoUtente)
        {
            return this.AmmInvalidaProcessiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente);
        }

        public bool InvalidaProcessiRegistriCoinvolti(DocsPAWA.DocsPaWR.RightRuoloMailRegistro[] listaRegistri, string idCorrGlobRuolo, string idGruppo, InfoUtente infoUtente)
        {
            return this.AmmInvalidaProcessiRegistriCoinvolti(listaRegistri, idCorrGlobRuolo, idGruppo, infoUtente);
        }

        public void AssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            this.AmmAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
        }

        public void DeleteAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            this.AmmDeleteAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
        }

		public void InsTipoFunzioni(DocsPAWA.DocsPaWR.OrgTipoFunzione[] listaFunzioni)
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

        public void VerificaUtenteRespStampeRep(string userId, string roleId, string idAmm)
        {
            this.AmmVerificaUtenteRespStampeRep(userId, roleId, idAmm);
        }

		public void VerificaTrasmRuolo(string idCorrGlobRuolo)
		{
			this.AmmVerificaTrasmRuolo(idCorrGlobRuolo);
		}

		public void RifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
		{
			this.AmmRifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);
		}

        public void AccettaTrasmConWF(string idCorrGlobRuolo)
        {
            this.AmmAccettaTrasmConWF(idCorrGlobRuolo);
        }

        public void AccettaTrasmConWFUtente(string idPeople, string idCorrGlobaliRuolo)
        {
            this.AmmAccettaTrasmConWFUtente(idPeople, idCorrGlobaliRuolo);
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

		public void SpostaRuolo(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			this.AmmSpostaRuolo(ruolo);
		}

		public void SpostaUO(DocsPAWA.DocsPaWR.OrgUO uoDaSpostare, DocsPAWA.DocsPaWR.OrgUO uoPadre)
		{
			this.AmmSpostaUO(uoDaSpostare,uoPadre);
		}
		
		public ArrayList GetListaUoFiglie(DocsPAWA.DocsPaWR.OrgUO uo)
		{
			ArrayList list= new ArrayList();

			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			list = ws.AmmGetListaUoFiglie(uo);	
			ws = null;
			return list;
		}

		public void StampaOrganigramma (XmlDocument xmlDoc)
		{
			this.AmmStampaOrganigramma(xmlDoc);
		}

		public DocsPAWA.DocsPaWR.FileDocumento getFilePDF()
		{
			return this._filePDF;
		}

		public void setSessionFilePDF(DocsPAWA.DocsPaWR.FileDocumento filePdf)
		{
			if (System.Web.HttpContext.Current.Session["SESSION_PDF"]==null)
			{			
				System.Web.HttpContext.Current.Session.Add("SESSION_PDF",filePdf);
			}
		}

		public DocsPAWA.DocsPaWR.FileDocumento getSessionFilePDF()
		{
            DocsPAWA.DocsPaWR.FileDocumento filePdf = new DocsPAWA.DocsPaWR.FileDocumento();

			if (System.Web.HttpContext.Current.Session["SESSION_PDF"]!=null)
			{			
				filePdf = (DocsPAWA.DocsPaWR.FileDocumento) System.Web.HttpContext.Current.Session["SESSION_PDF"];
			}
			return filePdf;
		}

		public void releaseSessionFilePDF()
		{
			System.Web.HttpContext.Current.Session.Remove("SESSION_PDF");
		}

        public void PerformUpDown(string idCorrGlobDaSpostare, string idPesoDaSpostare, string idCorrGlobSubisce, string idPesoSubisce)
        {
            this.AmmPerformUpDown(idCorrGlobDaSpostare, idPesoDaSpostare, idCorrGlobSubisce, idPesoSubisce);
        }

		#endregion

		#region private

		/// <summary>
		/// Prende ID Amministrazione
		/// </summary>
		/// <param name="codAmm"></param>
		private void GetIDAmm(string codAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();			
			this._idAmministrazione = ws.AmmGetIDAmm(codAmm);			
			ws = null;
		}

		/// <summary>
		/// Reperimento dei dati della UO corrente
		/// </summary>
		/// <param name="idUO">system_id della UO</param>
		private void AmmGetDatiUOCorrente(string idUO)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			this._datiUO = ws.AmmGetDatiUOCorrente(idUO);

			ws = null;
		}

		/// <summary>
		/// Lista UO
		/// </summary>
		/// <param name="idParent"></param>
		/// <param name="livello"></param>
		/// <param name="idAmm"></param>
		private void AmmGetListUO(string idParent, string livello, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListUO(idParent,livello,idAmm);			

			if(lista.Count>0)
				this._listaUO = new ArrayList(lista);

			lista = null;

			ws = null;
		}

        /// <summary>
        /// Lista UO in registro
        /// </summary>
        /// <param name="idRegistro"></param>
        private void AmmGetListUOInReg(string idRegistro, string tipoRicerca, string ricerca)
        {
            ArrayList lista = new ArrayList();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            lista = ws.AmmGetListUOInReg(idRegistro, tipoRicerca, ricerca);
            
            if (lista.Count > 0)
                this._listaUOInReg = new ArrayList(lista);

            lista = null;

            ws = null;
        }

		/// <summary>
		/// Lista Ruoli
		/// </summary>
		/// <param name="idUO"></param>
		private void AmmGetListRuoliUO(string idUO)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListRuoliUO(idUO);			

			if(lista.Count>0)
				this._listaRuoliUO = new ArrayList(lista);

			lista = null;

			ws = null;
		}

        private void AmmGetListRuoliUORic(string idUO,bool ricorsivo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            ArrayList lista = ws.AmmGetListRuoliUORic(idUO, ricorsivo);

            if (lista.Count > 0)
                this._listaRuoliUO = new ArrayList(lista);

            lista = null;

            ws = null;
        }

		/// <summary>
		/// Lista utenti del ruolo
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListUtentiRuolo(string idRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListUtentiRuolo(idRuolo);			

			if(lista.Count>0)
				this._listaUtenti = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		private void AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListUtenti(idAmm,ricercaPer,testoDaRicercare,IDesclusi);			

			if(lista.Count>0)
				this._listaUtenti = new ArrayList(lista);

			lista = null;

			ws = null;
		}

        private void AmmGetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            ArrayList lista = ws.AmmGetListRuoli(idAmm, ricercaPer, testoDaRicercare, idRegistro, IDesclusi);

            if (lista.Count > 0)
                this._listaRuoli = new ArrayList(lista);

            lista = null;

            ws = null;
        }

		/// <summary>
		/// Lista Registri
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
        private void AmmGetListRegistri(string idAmm, string idRuolo, string chaRF)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListRegistriRF(idAmm, idRuolo, chaRF);

			if(lista.Count>0)
				this._listaRegistri = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		/// <summary>
		/// Lista Registri associati al ruolo
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListRegistriAssRuolo(string idAmm, string idRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListRegistriAssRuolo(idAmm, idRuolo);

			if(lista.Count>0)
				this._listaRegistri = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		/// <summary>
		/// Lista Funzioni
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListFunzioni(string idAmm, string idRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListFunzioni(idAmm, idRuolo);

			if(lista.Count>0)
				this._listaFunzioni = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		/// <summary>
		/// Lista Tipi Ruolo
		/// </summary>
		/// <param name="idAmm"></param>
		private void AmmGetListTipiRuolo(string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            this._listaTipiRuolo = ws.AmmGetListTipiRuolo(idAmm);

			ws = null;
		}

		private void AmmGetDatiUtente(string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			this._datiUtente = ws.AmmGetDatiUtente(idCorrGlob);

			ws = null;
		}

		private void AmmInsNuovaUO(DocsPAWA.DocsPaWR.OrgUO newUO)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsNuovaUO(newUO);

			ws = null;
		}

		private void AmmModUO(DocsPAWA.DocsPaWR.OrgUO theUO, bool StoricizzUO)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmModUO(theUO, StoricizzUO);

			ws = null;
		}

        private DocsPAWA.DocsPaWR.Amministrazione AmmModificaUoTIBCO(string oldCodiceUO, DocsPAWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            DocsPAWA.DocsPaWR.Amministrazione amm = new DocsPAWA.DocsPaWR.Amministrazione();   
            amm = ws.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);
            ws = null;
            return amm;
        }

        private DocsPAWA.DocsPaWR.Amministrazione AmmEliminaUoTIBCO(DocsPAWA.DocsPaWR.OrgUO theUO, out bool result)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            DocsPAWA.DocsPaWR.Amministrazione amm = new DocsPAWA.DocsPaWR.Amministrazione();
            amm = ws.AmmEliminaUoTIBCO(theUO, out result);
            ws = null;
            return amm;
        }



        private void AmmInviaNotificaMail(DocsPAWA.DocsPaWR.OrgUO theUO, DocsPAWA.DocsPaWR.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            ws.inviaNotificaMail(theUO, amm, descrizioneAOO, tipoOperazione, oldCodiceUO);
            ws = null;
        }

		private void AmmEliminaUO(DocsPAWA.DocsPaWR.InfoUtente infoUtente, string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmEliminaUO(infoUtente, idCorrGlob);

			ws = null;
		}

		private void AmmInsNuovoRuolo(DocsPAWA.DocsPaWR.OrgRuolo newRuolo, bool computeAtipicita)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsNuovoRuolo(newRuolo, computeAtipicita);
			ws = null;
		}

		private void AmmModRuolo(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmModRuolo(ruolo);
			ws = null;
		}

        private void AmmOnlyDisabledRole(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmOnlyDisabledRole(ruolo);
            ws = null;
        }

		private void AmmEliminaRuolo(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmEliminaRuolo(ruolo);
			ws = null;
		}

		private void AmmInsRegistri(DocsPAWA.DocsPaWR.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsRegistri(listaRegistri, idUO, idCorrGlobRuolo);
			ws = null;
		}

        private bool AmmExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPAWA.DocsPaWR.RightRuoloMailRegistro[] listaRegistri, string idCorrGlobRuolo, string idGruppo)
        {
            bool result = false;
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            result = ws.AmmExistsPassiFirmaByRuoloTitolareAndRegistro(listaRegistri, idCorrGlobRuolo, idGruppo);
            ws = null;
            return result;
        }

        private bool AmmExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            bool result = false;
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            result = ws.AmmExistsPassiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
            ws = null;
            return result;
        }

        private bool AmmInvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, InfoUtente infoUtente)
        {
            bool result = false;
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            result = ws.AmmInvalidaProcessiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente);
            ws = null;
            return result;
        }


        private bool AmmInvalidaProcessiRegistriCoinvolti(DocsPAWA.DocsPaWR.RightRuoloMailRegistro[] listaRegistri, string idCorrGlobRuolo, string idGruppo, InfoUtente infoUtente)
        {
            bool result = false;
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            result = ws.AmmInvalidaProcessiRegistriCoinvolti(listaRegistri, idCorrGlobRuolo, idGruppo, infoUtente);
            ws = null;
            return result;
        }

        private void AmmAssociazioneRFRuolo(string idRf,  string idCorrGlobRuolo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmAssociazioneRFRuolo(idRf,idCorrGlobRuolo);
            ws = null;
        }

        private void AmmDeleteAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmDeleteAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
            ws = null;
        }


		private void AmmInsTipoFunzioni(DocsPAWA.DocsPaWR.OrgTipoFunzione[] listaFunzioni)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsTipoFunzioni(listaFunzioni);
			ws = null;
		}

		private void AmmInsUtenteInRuolo(string idPeople, string idGruppo, string idAmm, string type)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsUtenteInRuolo(idPeople, idGruppo, idAmm, type);
			ws = null;
		}

		private void AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsTrasmUtente(idPeople, idCorrGlobRuolo);
			ws = null;
		}

		private void AmmEliminaUtenteInRuolo(string idPeople, string idGruppo, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmEliminaUtenteInRuolo(idPeople, idGruppo, idAmm);
			ws = null;
		}

		private void AmmVerificaUtenteLoggato(string userId, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmVerificaUtenteLoggato(userId,idAmm);
			ws = null;
		}

        private void AmmVerificaUtenteRespStampeRep(string userId, string roleId, string idAmm)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmVerificaUtenteRespStampeRep(userId, roleId, idAmm);
            ws = null;
        }
		
		private void AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmEliminaADLUtente(idPeople,idCorrGlobGruppo);
			ws = null;
		}

		private void AmmVerificaTrasmRuolo(string idCorrGlobRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmVerificaTrasmRuolo(idCorrGlobRuolo);
			ws = null;
		}

		private void AmmRifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmRifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);
			ws = null;
		}

        private void AmmAccettaTrasmConWF(string idCorrGlobRuolo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmAccettaTrasmConWFRuolo(idCorrGlobRuolo);
            ws = null;
        }

        private void AmmAccettaTrasmConWFUtente(string idPeople, string idCorrGlobaliRuolo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmAccettaTrasmConWFUtente(idPeople, idCorrGlobaliRuolo);
            ws = null;
        }

        private void AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmSostituzioneUtente(idPeopleNewUT, idCorrGlobRuolo);
			ws = null;
		}

        private void AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            ArrayList lista = ws.AmmRicercaInOrg(tipo, codice, descrizione, idAmm, searchHistoricized, searchByCodeExact);			

			if(lista.Count>0)
				this._listaRisultatoRicerca = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		private void AmmListaIDParentRicerca(string IDPartenza, string tipo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList list = ws.AmmListaIDParentRicerca(IDPartenza,tipo);			

			if(list.Count>0)
				this._listaIDParentRicerca = list;

			list = null;

			ws = null;
		}

		private void AmmEstendeVisibRuolo(string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string idAmm, string livelloRuolo, bool escludiAtipicita)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmEstendeVisibRuolo(idRegistro, idCorrGlobRuolo, idGruppo, idCorrGlobUO, idAmm, livelloRuolo, escludiAtipicita);
			ws = null;			
		}

		private string AmmGetLivelloRuolo(string idCorrGlobRuolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			return ws.GetLivelloTipoRuolo(idCorrGlobRuolo);
		}

		private void AmmGetDatiUOStampaBuste(string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			this._datiUOStampaBuste = ws.AmmGetDatiUOStampaBuste(idCorrGlob);

			ws = null;
		}

		private void AmmSpostaRuolo(DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmSpostaRuolo(ruolo);
			ws = null;
		}

		private void AmmSpostaUO(DocsPAWA.DocsPaWR.OrgUO uoDaSpostare, DocsPAWA.DocsPaWR.OrgUO uoPadre)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmSpostaUO(uoDaSpostare,uoPadre);
			ws = null;
		}

		private void AmmStampaOrganigramma(XmlDocument xmlDoc)
		{			
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._filePDF = ws.AmmStampaOrganigramma(xmlDoc);
			ws = null;
		}
        
        public void GetListaRuoliAOO(string idRegistro)
        {
            this.AmmGetListaRuoliAOO(idRegistro);
        }

       /// <summary>
        /// Ritorna tutti i ruoli associati ad una determinata AOO (Registro o RF)
       /// </summary>
       /// <param name="idRegistro"></param>
        private void AmmGetListaRuoliAOO(string idRegistro)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            ArrayList lista = ws.AmmGetListaRuoliAOO(idRegistro);

            if (lista.Count > 0)
                this._listaRuoliAOO = new ArrayList(lista);

            lista = null;

            ws = null;
        }

        private void AmmPerformUpDown(string idCorrGlobDaSpostare, string idPesoDaSpostare, string idCorrGlobSubisce, string idPesoSubisce)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmOrdinamento(idCorrGlobDaSpostare, idPesoDaSpostare, idCorrGlobSubisce, idPesoSubisce);
            ws = null;
        }

		#endregion

        public SaveChangesToRoleResponse SaveChangesToRole(SaveChangesToRoleRequest request)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.SaveChangesToRole(request);
        }

        public OrgRuolo GetRole(String idCorrGlobali)
        {
            try
            {
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                return ws.GetRole(idCorrGlobali);
                
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

        #region Libro firma

        public bool SostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople)
        {
            return this.AmmSostituisciUtentePassiCorrelati(idRuolo, idOldPeople, idNewPeople);
        }

        private bool AmmSostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmSostituisciUtentePassiCorrelati(idRuolo, idOldPeople, idNewPeople);
        }

        public bool StoricizzaRuoloPassiCorrelati(string idRuoloOld, string idRuoloNew)
        {
            return this.AmmStoricizzaRuoloPassiCorrelati(idRuoloOld, idRuoloNew);
        }

        private bool AmmStoricizzaRuoloPassiCorrelati(string idRuoloOld, string idRuoloNew)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmStoricizzaRuoloPassiCorrelati(idRuoloOld, idRuoloNew);
        }

        public struct SoggettoInModifica
        {
            public const string UTENTE = "U";
            public const string DISABILITA_UTENTE = "UD";
            public const string ULTIMO_UTENTE = "UURC"; //L'utente che si stà rimuovendo è l'ultimo ed è coinvolto
            public const string ULTIMO_UTENTE_RUOLO = "UURNC"; //L'utente che si stà rimuovendo non è coinvolto in processi/istanze
            public const string ULTIMO_UTENTE_E_RUOLO = "UURCNC"; //L'utente che si stà rimuovendo è l'ultimo e ci sono processi in cui è coinvolti e altri no
            public const string RUOLO = "R";
        }

        public struct TipoOperazione
        {
            public const string SPOSTA_RUOLO = "sposta_ruolo";
            public const string MODIFICA_RUOLO_CON_STORICIZZAZIONE = "modifica_ruolo_con_storicizzazione";
            public const string MODIFICA_RUOLO_CON_DISABILITAZIONE_TRASM = "modifica_con_disabilitazione_trasm";
            public const string CANCELLAZIONE_RUOLO = "cancellazione_ruolo";
        }
        #endregion

    }
}
