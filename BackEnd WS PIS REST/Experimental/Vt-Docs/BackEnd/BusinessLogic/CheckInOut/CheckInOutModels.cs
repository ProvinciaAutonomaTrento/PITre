using System;
using System.IO;
using System.Collections;
using DocsPaVO.FormatiDocumento;
using log4net;

namespace BusinessLogic.CheckInOut
{
	/// <summary>
	/// Classe per la gestione dei modelli documenti predefiniti
	/// </summary>
	public sealed class CheckInOutModels
	{
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutModels));
		private const string DEFAULT_MODEL_FILE_NAME="model";

		private CheckInOutModels()
		{
		}

		#region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <returns></returns>
        public static string[] GetDocumentModelTypes(int idAdmin)
        {
            return FormatiDocumento.DocumentModelsManager.GetFileTypes(idAdmin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public static string[] GetDocumentModelTypes(string admin)
        {
            return FormatiDocumento.DocumentModelsManager.GetFileTypes(admin);
        }

        /// <summary>
        /// Reperimento modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] GetDocumentModelContent(int idAdmin, string fileName)
        {
            string fileType = GetFileType(fileName);

            return FormatiDocumento.DocumentModelsManager.GetModelFile(idAdmin, fileType);
        }

        /// <summary>
        /// Reperimento modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] GetDocumentModelContent(string admin, string fileName)
        {
            string fileType = GetFileType(fileName);

            return FormatiDocumento.DocumentModelsManager.GetModelFile(admin, fileType);
        }

		#endregion

        #region Private methods

        /// <summary>
        /// Reperimento della tipologia del file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetFileType(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            return fileInfo.Extension.Replace(".", "");
        }

        #endregion
	}
}
