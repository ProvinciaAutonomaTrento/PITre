using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;

namespace SAAdminTool.AdminTool.Gestione_Logs
{
	/// <summary>
	/// Summary description for exportLogManager.
	/// </summary>
	public class exportLogManager
	{		
		#region VAR
		private string _soggettoExport = string.Empty;
		private string _tipologiaExport = string.Empty;		
		private string _titolo = string.Empty;
        private string _codAmm = string.Empty;
        private string _user = string.Empty;
        private string _data_a = string.Empty;
        private string _data_da = string.Empty;
        private string _oggetto = string.Empty;
        private string _azione = string.Empty;
        private string _esito = string.Empty;
        private int _tabelle = 0;

		DocsPaWR.FileDocumento _file = new SAAdminTool.DocsPaWR.FileDocumento();
		#endregion

		#region COSTRUTTORE
		public exportLogManager()
		{

		}

		public exportLogManager(
			string soggettoExport, 
			string tipologiaExport,
			string titolo,
			string codAmm,
            string user,
            string data_a,
            string data_da,
            string oggetto,
            string azione,
            string esito,
            int tabelle)
		{
			this._soggettoExport   = soggettoExport;   // Amministrazione o FE
			this._tipologiaExport = tipologiaExport; // PDF o XLS
			this._titolo		  = titolo;
			this._codAmm		  = codAmm;
            this._user = user;
            this._data_a = data_a;
            this._data_da = data_da;
            this._oggetto = oggetto;
            this._azione = azione;
            this._esito = esito;
            this._tabelle = tabelle;
		}

		#endregion

		#region EXPORT
		public SAAdminTool.DocsPaWR.FileDocumento Export()
		{
            this.setLog();
			return _file;
		}

		#endregion
		
		#region LOG

		/// <summary>
		/// 
		/// </summary>
		private void setLog()
		{
			AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            this._file = ws.ExportLog(this._codAmm, this._soggettoExport, this._tipologiaExport, this._titolo, this._user, this._data_a, this._data_da, this._oggetto, this._azione, this._esito, this._tabelle);
			ws = null;
			if(this._file!=null)
			{
				exportLogSessionManager session = new exportLogSessionManager();
				session.SetSessionExportFile(this._file);
			}
		}

		#endregion

        #region FILTRI
        /// <summary>
        /// Reperisce i filtri impostati dall'utente per i log
        /// </summary>
        //private void getFiltriDoc()
        //{
        //    if (DocumentManager.getFiltroRicDoc(null) != null)
        //        this._lstFiltri = DocumentManager.getFiltroRicDoc(null);
        //    else
        //        this._lstFiltri = DocumentManager.getMemoriaFiltriRicDoc(null);
        //}

        /// <summary>
        /// Reperisce i filtri impostati dall'utente per i fascicoli
        /// </summary>
        //private void getFiltriFasc()
        //{
        //    if (FascicoliManager.getFiltroRicFasc(null) != null)
        //        this._lstFiltri = FascicoliManager.getFiltroRicFasc(null);
        //    else
        //        this._lstFiltri = FascicoliManager.getMemoriaFiltriRicFasc(null);
        //}

        /// <summary>
        /// Reperisce i filtri impostati dall'utente per le trasmissioni
        /// </summary>
        //private void getFiltriTrasm()
        //{
        //    this._lstFiltriTrasm = DocumentManager.getFiltroRicTrasm(null);
        //}

        /// <summary>
        ///  Reperisce il codice del fascicolo
        /// </summary>
        //private void getFascDocumenti()
        //{
        //    this._folder = FascicoliManager.getFolderSelezionato();
        //    //this._fascicolo = FascicoliManager.getFascicoloSelezionato(null);
        //    SAAdminTool.DocsPaWR.Fascicolo fascicolo = null;
        //    fascicolo = FascicoliManager.getFascicoloSelezionato(null);
        //    this.codFasc = fascicolo.codice;
        //}
        #endregion

	}
}
