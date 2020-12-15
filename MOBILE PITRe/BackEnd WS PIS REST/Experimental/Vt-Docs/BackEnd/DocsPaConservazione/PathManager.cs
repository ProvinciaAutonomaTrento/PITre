using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PathManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static string RootFolder
        {
            get
            {
                //modifica Lembo 16-11-2012
                //string rootPath = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
                //if (System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
                //    rootPath = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
                string rootPath = DocsPaConsManager.getConservazioneRootPath();
                if(string.IsNullOrEmpty(rootPath))
                    rootPath = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
                return rootPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string GetRootFolderAmministrazione(string idIstanza)
        {
            return System.IO.Path.Combine(RootFolder, GetCodiceAmministrazione(idIstanza));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetRootPathIstanza(string idIstanza)
        {
            return System.IO.Path.Combine(GetRootFolderAmministrazione(idIstanza), idIstanza);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetFolderChiusura(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            return GetRootPathIstanza(idIstanza) + @"\Chiusura";
        }

        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetFolderSchemi(string idIstanza)
        {
            return GetRootPathIstanza(idIstanza) + @"\Schemi";
        }

        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetFolderReadme(string idIstanza)
        {
            return GetRootPathIstanza(idIstanza) + @"\Readme";
        }

        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetFolderStaticFiles(string idIstanza)
        {
            return GetRootPathIstanza(idIstanza) + @"\static";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetPathFileChiusura(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            return GetFolderChiusura(infoUtente, idIstanza) + @"\file_chiusura.xml";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetPathFileChiusuraP7M(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            return GetFolderChiusura(infoUtente, idIstanza) + @"\file_chiusura.XML.P7M";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetPathFileChiusuraTSR(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            return GetFolderChiusura(infoUtente, idIstanza) + @"\file_chiusura.TSR";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="progressivoMarca"></param>
        /// <returns></returns>
        public static string GetPathFileChiusuraTSR(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza, string progressivoMarca)
        {
            if (string.IsNullOrEmpty(progressivoMarca))
                return GetPathFileChiusuraTSR(infoUtente, idIstanza);
            else
                return GetFolderChiusura(infoUtente, idIstanza) + string.Format(@"\file_chiusura_{0}.TSR", progressivoMarca);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetPathFileZip(string idIstanza)
        {
            return GetRootFolderAmministrazione(idIstanza) + @"\" + idIstanza + ".ZIP";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetPathFileIndexHtml(string idIstanza)
        {
            return GetRootPathIstanza(idIstanza) + @"\index.html";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetCodiceAmministrazione(string idIstanza)
        {
            string retValue = string.Empty;
            
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT VAR_CODICE_AMM FROM DPA_AMMINISTRA WHERE SYSTEM_ID = ( SELECT ID_AMM FROM PEOPLE WHERE SYSTEM_ID = ( SELECT ID_PEOPLE FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = {0} ) )", idIstanza);

                dbProvider.ExecuteScalar(out retValue, commandText);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento url della cartella in cui vengono memorizzati i file istanza di conservazione
        /// </summary>
        /// <returns></returns>
        public static string RootFolderUrl
        {
            get
            {
                //Undo modifica Lembo 16-11-2012
                
                return System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_DOWNLOAD_URL"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetZipUrl(string idIstanza)
        {
            string zipUrl = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_DOWNLOAD_ZIPURL"];
            if (String.IsNullOrEmpty (zipUrl))
                return string.Format("{0}/{1}/{2}.zip", RootFolderUrl, GetCodiceAmministrazione(idIstanza), idIstanza);
            else
                return string.Format("{0}/{1}/{2}.zip", zipUrl, GetCodiceAmministrazione(idIstanza), idIstanza);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetIndexUrl(string idIstanza)
        {
            return String.Format("{0}/index.html", GetUrlIstanza(idIstanza));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetUrlIstanza(string idIstanza)
        {
            return string.Format("{0}/{1}/{2}", RootFolderUrl, GetCodiceAmministrazione(idIstanza), idIstanza);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static string GetUrlFileChiusuraP7M(string idIstanza)
        {
            return String.Format("{0}/Chiusura/file_chiusura.XML.P7M", GetUrlIstanza(idIstanza));
        }


        public static string GetUrlFileChiusuraTSR(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza, string progressivoMarca)
        {
            if (!string.IsNullOrEmpty (progressivoMarca ))
                return string.Format("{0}/Chiusura/file_chiusura_{1}.TSR", GetUrlIstanza(idIstanza),progressivoMarca);
            else
                return string.Format("{0}/Chiusura/file_chiusura.TSR", GetUrlIstanza(idIstanza));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string NormalizeString(string value)
        {
            return value.Replace(" ", string.Empty);
        }
    }
}
