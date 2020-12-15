using System;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.UIManager
{
	/// <summary>
	/// Summary description for exportDatiSessionManager.
	/// </summary>
	public class exportDatiSessionManager
	{
		private const string EXPORT_FILE_SESSION = "EXPORT_FILE_SESSION";

		public exportDatiSessionManager()
		{
			
		}

		#region FILE
		/// <summary>
		/// Imposta la sessione dell'export
		/// </summary>
		public void SetSessionExportFile(FileDocumento file)
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
		public FileDocumento GetSessionExportFile()
		{
			FileDocumento filePdf = new FileDocumento();

			if (System.Web.HttpContext.Current.Session[EXPORT_FILE_SESSION]!=null)
			{			
				filePdf = (FileDocumento) System.Web.HttpContext.Current.Session[EXPORT_FILE_SESSION];
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
