using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione
{
    public sealed class EsibizionePathManager
    {

        public static string RootFolder
        {
            get
            {
                //modifica Lembo 16-11-2012
                //string rootPath = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
                //if (System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"] != null)
                //    rootPath = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
                //string rootPath = DocsPaConsManager.getConservazioneRootPath();
                string rootPath = DocsPaConsManager.getEsibizioneRootPath();
                if (string.IsNullOrEmpty(rootPath))
                    rootPath = @"C:\Sviluppo\docspa.etnoteam.it\DocsPA30\Conservazione";
                return rootPath;
            }
        }

        public static string GetRootFolderAmministrazione(string idIstanza)
        {
            return System.IO.Path.Combine(RootFolder, GetCodiceAmministrazione(idIstanza));
        }

        public static string GetRootPathIstanza(string idIstanza)
        {
            return System.IO.Path.Combine(GetRootFolderAmministrazione(idIstanza), idIstanza);
        }

        public static string GetPathFileZip(string idIstanza)
        {
            return GetRootFolderAmministrazione(idIstanza) + @"\" + idIstanza + ".ZIP";
        }

        public static string GetCodiceAmministrazione(string idIstanza)
        {
            string retValue = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT VAR_CODICE_AMM FROM DPA_AMMINISTRA WHERE SYSTEM_ID = ( SELECT ID_AMM FROM PEOPLE WHERE SYSTEM_ID = ( SELECT ID_PEOPLE FROM DPA_AREA_ESIBIZIONE WHERE SYSTEM_ID = {0} ) )", idIstanza);

                dbProvider.ExecuteScalar(out retValue, commandText);
            }

            return retValue;
        }

        public static string RootFolderUrl
        {
            get
            {
                //se è definita la chiave ESIBIZIONE_DOWNLOAD_URL
                if(!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_DOWNLOAD_URL"]))
                    return System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_DOWNLOAD_URL"];
                //altrimenti restituisco il valore della CONSERVAZIONE
                else
                    return System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_DOWNLOAD_URL"];
                
            }
        }

        public static string GetZipUrl(string idIstanza)
        {
            //se non è definita la chiave dell'esibizione utilizzo quella della conservazione
            string zipUrl = System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_DOWNLOAD_ZIPURL"];
            if(string.IsNullOrEmpty(zipUrl))
                zipUrl = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_DOWNLOAD_ZIPURL"];

            if (String.IsNullOrEmpty(zipUrl))
                return string.Format("{0}/{1}/{2}.zip", RootFolderUrl, GetCodiceAmministrazione(idIstanza), idIstanza);
            else
                return string.Format("{0}/{1}/{2}.zip", zipUrl, GetCodiceAmministrazione(idIstanza), idIstanza);
        }


    }
}
