using System;

namespace SAAdminTool.AdminTool.Gestione_Logs
{
	/// <summary>
	/// Summary description for exportLogSessionManager.
	/// </summary>
	public class exportLogSessionManager
	{
		private const string EXPORT_FILE_SESSION = "EXPORT_FILE_SESSION";

		public exportLogSessionManager()
		{
			
		}

		#region FILE
		/// <summary>
		/// Imposta la sessione dell'export
		/// </summary>
		public void SetSessionExportFile(SAAdminTool.DocsPaWR.FileDocumento file)
		{
			if (System.Web.HttpContext.Current.Session[EXPORT_FILE_SESSION]==null)
			{
				System.Web.HttpContext.Current.Session.Add(EXPORT_FILE_SESSION,file);
			}
		}

		/// <summary>
		/// Recupera l'export in sessione
		/// </summary>
		/// <returns></returns>
		public SAAdminTool.DocsPaWR.FileDocumento GetSessionExportFile()
		{
			DocsPaWR.FileDocumento filePdf = new SAAdminTool.DocsPaWR.FileDocumento();

			if (System.Web.HttpContext.Current.Session[EXPORT_FILE_SESSION]!=null)
			{			
				filePdf = (SAAdminTool.DocsPaWR.FileDocumento) System.Web.HttpContext.Current.Session[EXPORT_FILE_SESSION];
			}
			return filePdf;
		}
		
		/// <summary>
		/// Rilascia la sessione dell'export
		/// </summary>
		public void ReleaseSessionExportFile()
		{
			System.Web.HttpContext.Current.Session.Remove(EXPORT_FILE_SESSION);
		}
		#endregion


	}
}
