using System;
using System.IO;
using System.Configuration;
using System.Web;

namespace BusinessLogic.AcquisizioneMassiva
{
	/// <summary>
	/// Classe per il reperimento delle impostazioni relative all'AcquisizioneMassiva
	/// </summary>
	public class Configurations
	{
		private const string PATH_ACQUISIZIONE_BATCH="pathAcquisizioneBatch";

		/// <summary>
		/// Reperimento del percorso della cartella in 
		/// cui viene effettuato l'upload dei documenti per l'acquisizione massiva
		/// </summary>
		/// <returns></returns>
		public static string GetPathAcquisizioneBatch()
		{
			// Reperimento della chiave dal web.config
			string path=ConfigurationManager.AppSettings[PATH_ACQUISIZIONE_BATCH];

			if (path==null || path==string.Empty)
				// Se non definita, viene creata una cartella sul documentale
				path=DocumentRootPath + @"\AcquisizioneMassiva\";

			return path;
		}

		/// <summary>
		/// Reperimento dell'url della pagina utilizzata per effettuare
		/// l'upload dei documenti per l'acquisizione massiva
		/// </summary>
		/// <returns></returns>
		public static string GetUrlUploadAcquisizione()
		{
			return RootPath + "UploadPage.aspx";
		}

		/// <summary>
		/// Reperimento percorso radice dell'applicazione web
		/// </summary>
		private static string RootPath
		{
			get
			{
				HttpRequest request=HttpContext.Current.Request;

				string rootPath=request.Url.Scheme + "://" + request.Url.Host;

				if(!request.Url.Port.Equals(80))
					rootPath+= ":" + request.Url.Port;

				rootPath+=request.ApplicationPath + "/AcquisizioneMassiva/";

				return rootPath;
			}
		}

		/// <summary>
		/// Reperimento percorso principale del documentale
		/// </summary>
		/// <returns></returns>
		private static string DocumentRootPath
		{
			get
			{
				return Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
			}
		}
	}
}