using System;
using System.Collections;

namespace DocsPAWA.AdminTool.Manager
{
	/// <summary>
	/// Summary description for AmministrazioneManager.
	/// </summary>
	public class AmministrazioneManager
	{
		#region Declaration

		private ArrayList _listaAmministrazioni = null;
		private ArrayList _listaRagioniTrasm = null;
		private DocsPAWA.DocsPaWR.Menu[] _vociMenu = null;
		private DocsPAWA.DocsPaWR.InfoAmministrazione _amministrazioneCorrente = null;
		private DocsPAWA.DocsPaWR.EsitoOperazione _esitoOperazione = null;

		#endregion

		#region Public
		public AmministrazioneManager()
		{

		}

		public DocsPAWA.DocsPaWR.EsitoOperazione getEsitoOperazione()
		{
			return this._esitoOperazione;
		}

		public void ListaAmministrazioni()
		{
			this.AmmListAmm();
		}

		public void ListaRagioniTrasm(string idAmm)
		{
			this.AmmListRagTrasm(idAmm);
		}

		public ArrayList getListaAmministrazioni()
		{
			return this._listaAmministrazioni;
		}

		public ArrayList getListaRagioniTrasm()
		{
			return this._listaRagioniTrasm;
		}

		public DocsPAWA.DocsPaWR.Menu[] getListaVociMenu()
		{
			return this._vociMenu;
		}

		public void InfoAmmCorrente(string idAmm)
		{
			this.GetInfoAmmCorrente(idAmm);
		}

		public DocsPAWA.DocsPaWR.InfoAmministrazione getCurrentAmm()
		{
			return this._amministrazioneCorrente;
		}

		public DocsPAWA.DocsPaWR.EsitoOperazione Insert(ref DocsPAWA.DocsPaWR.InfoAmministrazione info)
		{
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            return ws.AmmInsertAmm(ref info);
		}

        public bool IsEnabledRF(string idamm)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.IsEnabldRF(idamm);

        }

        public DocsPAWA.DocsPaWR.EsitoOperazione Update(ref DocsPAWA.DocsPaWR.InfoAmministrazione info, bool modFascicolatura, bool modSegnatura, bool modTimbroPdf, bool modProtTit)
		{
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            return ws.AmmUpdateAmm(ref info, modFascicolatura, modSegnatura, modTimbroPdf, modProtTit);
		}

		public DocsPAWA.DocsPaWR.EsitoOperazione Delete(DocsPAWA.DocsPaWR.InfoAmministrazione info)
		{
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            return ws.AmmDeleteAmm(ref info);
		}

        //public DocsPAWA.DocsPaWR.EsitoOperazione Login(string userid, string pwd)
        //{
        //    return LoginAmministrazione(userid, pwd);
        //}

        public DocsPAWA.DocsPaWR.EsitoOperazione Login(DocsPAWA.DocsPaWR.UserLogin userLogin, bool forceLogin, out DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore)
        {
            return LoginAmministrazione(userLogin, forceLogin, out datiAmministratore);
        }

        //public DocsPAWA.DocsPaWR.EsitoOperazione LoginUpdSession(string userid, string sessionID)
        //{
        //    return UpdateLoginAmministrazione(userid, sessionID);
        //}

		public void GetAmmAppartenenza(string userid, string pwd)
		{
			this.GetAmmUtente(userid, pwd);
		}

		public void GetAmmListaVociMenu(string idCorrGlob, string idAmm)
		{
			this.GetListaVociMenu(idCorrGlob, idAmm);
		}

		#endregion

		#region Private

		private void AmmListAmm()
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			ArrayList lista = ws.AmmGetListAmministrazioni();

			if (lista.Count > 0)
				this._listaAmministrazioni = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		private void AmmListRagTrasm(string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			DocsPaWR.OrgRagioneTrasmissione[] ragioni = ws.GetInfoRagioniTrasmissione(idAmm);

			if (ragioni.Length > 0)
				this._listaRagioniTrasm = new ArrayList(ragioni);

			ws = null;
		}

		private void GetInfoAmmCorrente(string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._amministrazioneCorrente = ws.GetInfoAmmCorrente(idAmm);

			ws = null;
		}


        //private DocsPAWA.DocsPaWR.EsitoOperazione LoginAmministrazione(string userid, string pwd)
        //{
        //    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
        //    DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            
        //    esito = ws.Login(userid, pwd);

        //    return esito;
        //}

        private DocsPAWA.DocsPaWR.EsitoOperazione LoginAmministrazione(DocsPAWA.DocsPaWR.UserLogin userLogin, bool forceLogin, out DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore)
		{
			datiAmministratore = null;

			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            
			esito = ws.Login(userLogin, forceLogin, out datiAmministratore);

			return esito;
		}

        //private DocsPAWA.DocsPaWR.EsitoOperazione UpdateLoginAmministrazione(string userid, string sessionID)
        //{
        //    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
        //    DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

        //    esito = ws.UpdateLoginAmministrazione(userid, sessionID);

        //    return esito;
        //}

		private void GetAmmUtente(string userid, string pwd)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._amministrazioneCorrente = ws.GetInfoAmmAppartenenzaUtente(userid, pwd);

			ws = null;
		}

		private void GetListaVociMenu(string idCorrGlob, string idAmm)
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			this._vociMenu = ws.AmmGetListMenuUtenteObj(idCorrGlob, idAmm);
			ws = null;
		}

		#endregion
	}
}
