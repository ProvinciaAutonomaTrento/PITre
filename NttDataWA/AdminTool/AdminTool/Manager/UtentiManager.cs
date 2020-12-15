using System;
using System.Collections;
using System.Collections.Generic;

namespace Amministrazione.Manager
{
	/// <summary>
	/// Summary description for UtentiManager.
	/// </summary>
	public class UtentiManager
	{
		#region Declaration

		private string _idAmministrazione = null;

		private ArrayList _listaUtenti = null;

		private ArrayList _ruoliUtente = null;

		private ArrayList _listaRegistriUtente = null;
        private ArrayList _listaMenuUtente = null;

		private SAAdminTool.DocsPaWR.OrgUtente _datiUtente = null;

		private SAAdminTool.DocsPaWR.EsitoOperazione _esitoOperazione = null;

		#endregion

		#region Public

		public string getIDAmministrazione()
		{
			return this._idAmministrazione;
		}		

		public ArrayList getListaUtenti()
		{
			return this._listaUtenti;
		}

		public ArrayList getRuoliUtente()
		{
			return this._ruoliUtente;
		}

		public ArrayList getRegistriUtente()
		{
			return this._listaRegistriUtente;
		}

		public void CurrentIDAmm(string codAmm)
		{
			this.GetIDAmm(codAmm);
		}

		public SAAdminTool.DocsPaWR.OrgUtente getDatiUtente()
		{
			return this._datiUtente;
		}

		public SAAdminTool.DocsPaWR.EsitoOperazione getEsitoOperazione()
		{
			return this._esitoOperazione;
		}

		public void ListaUtenti(string idAmm)
		{
			this.AmmGetListUtenti(idAmm);
		}

		public void ListaUtenti(string idAmm, string ricercaPer, string testoDaRicercare)
		{
			this.AmmGetListUtenti(idAmm,ricercaPer,testoDaRicercare);
		}

		public void DatiUtente(string idCorrGlob)
		{
			this.AmmGetDatiUtente(idCorrGlob);
		}

		public void RuoliUtente(string idPeople)
		{
			this.AmmGetRuoliUtente(idPeople);
		}

		public void RegistriUtente(string idCorrGlob, string idAmm)
		{
			this.AmmGetRegistriUtente(idCorrGlob, idAmm);
		}

		public void EliminaRegistriUtente(string idCorrGlob)
		{
			this.AmmEliminaRegistriUtente(idCorrGlob);
		}

        public void ModUtente(SAAdminTool.DocsPaWR.OrgUtente utente, string idAmministrazione)
        {
            this.AmmModUtente(utente, idAmministrazione);
		}

		public void InsUtente(SAAdminTool.DocsPaWR.OrgUtente utente, string idAmm)
		{
			this.AmmInsUtente(utente, idAmm);
		}

		public void EliminaUtente(SAAdminTool.DocsPaWR.OrgUtente utente)
		{
			this.AmmEliminaUtente(utente);
		}

		public void VerificaEliminazioneUtente(SAAdminTool.DocsPaWR.OrgUtente utente)
		{
			this.AmmVerificaEliminazioneUtente(utente);
		}

		public void DisabilitaUtente(string idPeople, string idAmm)
		{
			this.AmmDisabilitaUtente(idPeople, idAmm);
		}

		public void AbilitaUtente(string idPeople, string idAmm)
		{
			this.AmmAbilitaUtente(idPeople, idAmm);
		}

		public void ImpostaRuoloPreferito(string idPeople, string idGruppo)
		{
			this.AmmImpostaRuoloPreferito(idPeople,idGruppo);
		}	

		public void InsRegistriUtente(SAAdminTool.DocsPaWR.OrgRegistro[] listaRegistri, string idCorrGlob)
		{
			this.AmmInsRegistriUtente(listaRegistri,idCorrGlob);
		}
		
		public bool EsistonoRegistriAssociati(string idCorrGlob)
		{
			return this.AmmCheckRegAssUtente(idCorrGlob);
		}

      public bool VerificaSeRespAOO(string idpeople, string idGruppo)
      {
         return this.AmmIsUtenteRespAOO(idpeople, idGruppo);
      }

      public string[] getAmmRespAOO(string idpeople)
      {
          return this.AmmGetUtenteResp(idpeople);
      }


      public List<SAAdminTool.DocsPaWR.Qualifica> GetQualifiche(int id_amm)
      {
          return this.AmmGetQualifiche(id_amm);
      }

      public SAAdminTool.DocsPaWR.ValidationResultInfo InsertQual(SAAdminTool.DocsPaWR.Qualifica qual)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = ws.InsertQual(qual);
          return retValue;
      }

      public SAAdminTool.DocsPaWR.ValidationResultInfo UpdateQual(String idQualifica, String descrizione)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = ws.UpdateQual(idQualifica, descrizione);
          return retValue;
      }

      public SAAdminTool.DocsPaWR.ValidationResultInfo DeleteQual(String idQualifica, int idAmministrazione)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = ws.DeleteQual(idQualifica, idAmministrazione);
          return retValue;
      }

      public List<SAAdminTool.DocsPaWR.PeopleGroupsQualifiche> GetPeopleGroupsQualifiche(String idAmm, String idUo, String idGruppo, String idPeople)
      {
          return this.AmmGetPeopleGroupsQualifiche(idAmm, idUo, idGruppo, idPeople);
      }

      public SAAdminTool.DocsPaWR.ValidationResultInfo InsertPeopleGroupsQual(SAAdminTool.DocsPaWR.PeopleGroupsQualifiche pgq)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = ws.InsertPeopleGroupsQual(pgq);
          return retValue;
      }

      public SAAdminTool.DocsPaWR.ValidationResultInfo DeletePeopleGroups(String idPeopleGroups)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          SAAdminTool.DocsPaWR.ValidationResultInfo retValue = ws.DeletePeopleGroups(idPeopleGroups);
          return retValue;
      }

        //SABRINA

        public ArrayList getMenuUtente()
        {
            return this._listaMenuUtente;
        }
        public void InsMenuUtente(SAAdminTool.DocsPaWR.Menu[] listaMenu, string idCorrGlob,string idAmm)
        {
            this.AmmInsMenuUtente(listaMenu, idCorrGlob, idAmm);
        }

        public bool EsistonoMenuAssociati(string idCorrGlob)
        {
            return this.AmmCheckMenuAssUtente(idCorrGlob);
        }


        public void MenuUtente(string idCorrGlob, string idAmm)
        {
            this.AmmGetMenuUtente(idCorrGlob, idAmm);
        }

        public void EliminaMenuUtente(string idCorrGlob)
        {
            this.AmmEliminaMenuUtente(idCorrGlob);
        }

        public string GetFormatDominio(string idAmministrazione)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.GetFormatoDominio(idAmministrazione);
        }       

		#endregion

		#region Private

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
		/// Lista utenti
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListUtenti(string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListUtenti(idAmm);			

			if(lista.Count>0)
				this._listaUtenti = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		/// <summary>
		/// Lista utenti con ricerca
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="ricercaPer"></param>
		/// <param name="testoDaRicercare"></param>
		private void AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListUtenti(idAmm,ricercaPer,testoDaRicercare);			

			if(lista.Count>0)
				this._listaUtenti = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		/// <summary>
		/// Dati utente
		/// </summary>
		/// <param name="idCorrGlob"></param>
		private void AmmGetDatiUtente(string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			this._datiUtente = ws.AmmGetDatiUtente(idCorrGlob);

			ws = null;
		}

		/// <summary>
		/// Ruoli utente
		/// </summary>
		/// <param name="idPeople"></param>
		private void AmmGetRuoliUtente(string idPeople)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListRuoliUtente(idPeople);			

			if(lista.Count>0)
				this._ruoliUtente = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		/// <summary>
		/// Registri utente
		/// </summary>
		/// <param name="idCorrGlob"></param>
		private void AmmGetRegistriUtente(string idCorrGlob, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

			ArrayList lista = ws.AmmGetListRegistriUtente(idCorrGlob, idAmm);			

			if(lista.Count>0)
				this._listaRegistriUtente = new ArrayList(lista);

			lista = null;

			ws = null;
		}

        private void AmmModUtente(SAAdminTool.DocsPaWR.OrgUtente utente, string idAmminstrazione)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmModUtente(utente, idAmminstrazione);

			ws = null;
		}

		private void AmmInsUtente(SAAdminTool.DocsPaWR.OrgUtente utente, string idAmm)
		{
            //string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsUtente(utente, idAmm);

			ws = null;
		}

		private void AmmVerificaEliminazioneUtente(SAAdminTool.DocsPaWR.OrgUtente utente)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmVerificaEliminazioneUtente(utente);

			ws = null;
		}

		private void AmmEliminaUtente(SAAdminTool.DocsPaWR.OrgUtente utente)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmEliminaUtente(utente);

			ws = null;
		}

		private void AmmDisabilitaUtente(string idPeople, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmDisabilitaUtente(idPeople, idAmm);

			ws = null;
		}

		private void AmmAbilitaUtente(string idPeople, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmAbilitaUtente(idPeople, idAmm);

			ws = null;
		}

		private void AmmImpostaRuoloPreferito(string idPeople, string idGruppo)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmImpostaRuoloPreferito(idPeople,idGruppo);

			ws = null;
		}

		private void AmmInsRegistriUtente(SAAdminTool.DocsPaWR.OrgRegistro[] listaRegistri, string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmInsRegistriUtente(listaRegistri,idCorrGlob);

			ws = null;
		}

		private void AmmEliminaRegistriUtente(string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._esitoOperazione = ws.AmmEliminaRegistriUtente(idCorrGlob);

			ws = null;
		}

		private bool AmmCheckRegAssUtente(string idCorrGlob)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			return ws.AmmCheckRegAssUtente(idCorrGlob);			
		}

      private bool AmmIsUtenteRespAOO(string idpeople, string idGruppo)
      {
         AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
         return ws.AmmIsUtenteRespAOO(idpeople, idGruppo);
      }

      private string[] AmmGetUtenteResp(string idpeople)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          return ws.getUtenteRespAOO(idpeople);
      }


      private List<SAAdminTool.DocsPaWR.Qualifica> AmmGetQualifiche(int id_amm)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          List<SAAdminTool.DocsPaWR.Qualifica> qualifiche = new List<SAAdminTool.DocsPaWR.Qualifica>(ws.GetQualifiche(id_amm));
          return qualifiche;
      }

      private List<SAAdminTool.DocsPaWR.PeopleGroupsQualifiche> AmmGetPeopleGroupsQualifiche(String idAmm, String idUo, String idGruppo, String idPeople)
      {
          AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
          List<SAAdminTool.DocsPaWR.PeopleGroupsQualifiche> pgqs = new List<SAAdminTool.DocsPaWR.PeopleGroupsQualifiche>(ws.GetPeopleGroupsQualifiche(idAmm, idUo, idGruppo, idPeople));
          return pgqs;
      }      

      

      //SABRINA  -- voci di menù

        private void AmmGetMenuUtente(string idCorrGlob, string idAmm)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            ArrayList lista = ws.AmmGetListMenuUtente(idCorrGlob, idAmm);

            if (lista.Count > 0)
                this._listaMenuUtente = new ArrayList(lista);

            lista = null;

            ws = null;
        }
        private void AmmInsMenuUtente(SAAdminTool.DocsPaWR.Menu[] listaMenu, string idCorrGlob, string idAmm)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmInsMenuUtente(listaMenu, idCorrGlob, idAmm);

            ws = null;
        }

        private void AmmEliminaMenuUtente(string idCorrGlob)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._esitoOperazione = ws.AmmEliminaMenuUtente(idCorrGlob);

            ws = null;
        }

        private bool AmmCheckMenuAssUtente(string idCorrGlob)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmCheckMenuAssUtente(idCorrGlob);
        }

       


        //FINE SABRINA

		#endregion	
	
        public bool AmmVerificaGestioneChiavi(string idPeople)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmVerificaGestioneChiavi(idPeople);
        }
	}
}
