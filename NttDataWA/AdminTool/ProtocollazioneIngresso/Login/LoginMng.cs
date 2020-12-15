using System;

namespace ProtocollazioneIngresso.Login
{
	/// <summary>
	/// Summary description for LoginMng.
	/// </summary>
	public class LoginMng
	{
		private const string AUT_PROT_INGRESSO="PROTOCOLLAZIONE_INGRESSO";

		private System.Web.UI.Page _page=null;
		
		public LoginMng(System.Web.UI.Page page)
		{
			this._page=page;
		}

		public SAAdminTool.DocsPaWR.Utente GetUtente()
		{		
			return SAAdminTool.UserManager.getUtente(this._page);
		}

		public SAAdminTool.DocsPaWR.Ruolo GetRuolo() 
		{
			return SAAdminTool.UserManager.getRuolo(this._page);
		}

		public SAAdminTool.DocsPaWR.InfoUtente GetInfoUtente() 
		{
			return SAAdminTool.UserManager.getInfoUtente(this.GetUtente(),this.GetRuolo());
		}

		/// <summary>
		/// Verifica se l'utente corrente possiede
		/// le autorizzazioni minime per utilizzare
		/// la protocollazione in ingresso 
		/// </summary>
		/// <returns></returns>
		public bool IsUtenteAutorizzato()
		{
			return this.IsEnabledButtonProtIngresso();
		}

        /// <summary>
        /// Verifica se l'utente corrente possiede
        /// le autorizzazioni minime per utilizzare
        /// la protocollazione in uscita
        /// </summary>
        /// <returns></returns>
        public bool IsUtenteAutorizzatoUscita()
        {
            return this.IsEnabledButtonProtUscita();
        }

		/// <summary>
		/// Verifica se l'utente è abilitato alla protocollazione in ingresso in base a:
        /// - valore del tag "PROTO_SEMPLIFICATO_ENABLED" del web.config,
		/// - se true, si verifica l'abilitazione della funzione 
		///   di protocollazione in ingresso semplificata
		/// </summary>
		/// <returns></returns>
		private bool IsEnabledButtonProtIngresso()
		{
			bool retValue=false;

            string configValue = SAAdminTool.ConfigSettings.getKey(SAAdminTool.ConfigSettings.KeysENUM.PROTO_SEMPLIFICATO_ENABLED);
			
			if (configValue!=null && configValue!=string.Empty)
				retValue=Convert.ToBoolean(configValue);

            if (retValue)
            {
                //retValue=SAAdminTool.UserManager.ruoloIsAutorized(this._page,"PROTO_IN");
                retValue = SAAdminTool.UserManager.ruoloIsAutorized(this._page, "PROTO_IN_SEMPL");
            }
			return retValue;
		}

        /// <summary>
        /// Verifica se l'utente è abilitato alla protocollazione in ingresso in base a:
        /// - valore del tag "PROTOUSCITA_ENABLED" del web.config,
        /// - se true, si verifica l'abilitazione della funzione 
        ///   di protocollazione in uscita
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledButtonProtUscita()
        {
            bool retValue = false;

            string configValue = SAAdminTool.ConfigSettings.getKey(SAAdminTool.ConfigSettings.KeysENUM.PROTO_SEMPLIFICATO_ENABLED);

            if (configValue != null && configValue != string.Empty)
                retValue = Convert.ToBoolean(configValue);

            if (retValue)
            {
                //retValue = SAAdminTool.UserManager.ruoloIsAutorized(this._page, "PROTO_OUT");
                retValue = SAAdminTool.UserManager.ruoloIsAutorized(this._page, "PROTO_OUT_SEMPL");
            }

            return retValue;
        }
	}
}
