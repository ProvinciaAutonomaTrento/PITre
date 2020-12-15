using System;
using System.Collections;

namespace SAAdminTool.AdminTool.Manager
{
	/// <summary>
	/// Summary description for TipiRuoloManager.
	/// </summary>
	public class TipiRuoloManager
	{
		#region Declaration				

		private string _idAmministrazione = null;

		private ArrayList _listaTipoRuoloUtenti = null;		

		private SAAdminTool.DocsPaWR.EsitoOperazione _esitoOperazione = null;

		#endregion

		#region public

		public TipiRuoloManager()
		{

		}

		public string getIDAmministrazione()
		{
			return this._idAmministrazione;
		}

		public ArrayList getListaTipoRuoloUtenti()
		{
			return this._listaTipoRuoloUtenti;
		}

		public void CurrentIDAmm(string codAmm)
		{
			this.GetIDAmm(codAmm);
		}

		public SAAdminTool.DocsPaWR.EsitoOperazione getEsitoOperazione()
		{
			return this._esitoOperazione;
		}

		public void ListaTipoRuoloUtenti(string codTipoRuolo, string codAmm)
		{
			this.CurrentIDAmm(codAmm);
			if(this._idAmministrazione!=null && this._idAmministrazione!=string.Empty)
				this.AmmGetListTipoRuoloUtenti(codTipoRuolo, this._idAmministrazione);
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
		/// Lista tipo ruolo con utenti
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idRuolo"></param>
		private void AmmGetListTipoRuoloUtenti(string codTipoRuolo, string idAmm)
		{					
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			ArrayList lista = ws.AmmGetListTipoRuoloUtenti(codTipoRuolo,idAmm);			

			if(lista.Count>0)
				this._listaTipoRuoloUtenti = new ArrayList(lista);

			lista = null;

			ws = null;
		}

		#endregion
	}
}
