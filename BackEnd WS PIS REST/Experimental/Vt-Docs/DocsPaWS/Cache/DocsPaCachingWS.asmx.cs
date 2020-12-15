using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;
//using Microsoft.Web.Services3;
//using Microsoft.Web.Services3.Design;
//using Microsoft.Web.Services3.Diagnostics.Configuration;
using BusinessLogic.Documenti;
using log4net;

namespace DocsPaWS.Cache
{
    /// <summary>
    /// Summary description for DocsPaCachingWS
    /// </summary>
    [WebService(Namespace = "http://valueteam.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class DocsPaCachingWS : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaCachingWS));

        protected static string path;
        public static string Path { get { return path; } }

        public DocsPaCachingWS()
        {
            path = this.Server.MapPath("");
            InitializeComponent();
        }
      
        private void InitializeComponent()
        {
        }


        [WebMethod()]
        public bool copyFileInCache(byte[] stream, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            return BusinessLogic.Documenti.CacheFileManager.copyFileInCacheConLimite(stream, fileRequest, infoUtente);
        }

        [WebMethod]
        public DocsPaVO.Caching.InfoFileCaching[] ricercaDocumemtoInCacheDaTrasferire(string statoAggiornamento, string idAmministrazione)
        {
            DocsPaVO.Caching.InfoFileCaching[] Info = null;
            try
            {
            Info= CacheFileManager.ricercaDocumemtoInCacheDaTrasferire(statoAggiornamento, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method ricercadocumentoincache errore: " + e.Message);
                Info = new DocsPaVO.Caching.InfoFileCaching[0];
            }

            return Info;
        }

        [WebMethod]
        public DocsPaVO.utente.InfoUtente ricercaInfoUtente(string userid )
        {
            DocsPaVO.utente.InfoUtente info = null;
            try
            {
            info = CacheFileManager.ricercaInfoUtente(userid);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method ricercainfoutente errore: " + e.Message);
            }

            return info;
        }

        [WebMethod]
        public string GetDocPathAdvanced(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {

            string path = string.Empty;
            try
            {
            if(fileRequest != null && objSicurezza != null)
                path = BusinessLogic.Documenti.CacheFileManager.GetDocPathAdvanced(fileRequest, objSicurezza);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method GetDocPathAdvanced errore: " + e.Message);
            }

            
            return path;
        }
        [WebMethod]
        public virtual DocsPaVO.Caching.CacheConfig getConfigurazioneCache(string idAmministrazione)
        {
            DocsPaVO.Caching.CacheConfig info = null;

            try{
             info =CacheFileManager.getConfigurazioneCache(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method getconfigurazionecache errore: " + e.Message);
            }

            
            return info;
        }

        [WebMethod]
        public bool updateComponents(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool retval = false;
            try
            {
            retval = CacheFileManager.aggiornamentoComponents(info);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method updatecomponents errore: " + e.Message);
            }

            
            return retval;
           
        }
        [WebMethod]
        public bool updateCache(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool retval = false;
            try
            {
                retval = CacheFileManager.updateCaching(info);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method updatecache errore: " + e.Message);
            }


            return retval;
       

        }

      
        [WebMethod()]
        public bool insertCache(DocsPaVO.Caching.InfoFileCaching fileInfo)
        {
            bool retval = false;
            try
            {
            retval = CacheFileManager.inserimentoCaching(fileInfo);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method insertcache errore: " + e.Message);
            }


            return retval;
        }

        [WebMethod()]
        public bool insertCacheConStream(DocsPaVO.Caching.InfoFileCaching fileInfo, byte[] stream)
        {
            bool retval = false;
            try
            {
                retval = CacheFileManager.insertCacheConStream(fileInfo, stream);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method GetDocPathAdvanced errore: " + e.Message);
            }


            return retval;
        }
        

        [WebMethod]
        public string ricercaPathComponents(string docNumber, string versionId)
        {
            string path = string.Empty;
            try
            {
            path =CacheFileManager.ricercaPathComponents(docNumber, versionId);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method ricercaPathcomponets errore: " + e.Message);
            }

            return path;
        }

       


        [WebMethod]
        public byte[] streamFileDallaCache(string path)
        {

            byte[] stream = null;
            try
            {
            stream= CacheFileManager.streamFileDallaCache(path);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method streamfiledallacache errore: " + e.Message);
            }

            return stream;
        }



        [WebMethod]
        public bool cancellaFileTrasferitoInCache(string pathDestinazione)
        {
            bool retval = false;
            try
            {
            retval = CacheFileManager.cancellaFileTrasferitoInCache(pathDestinazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method cancellafiletrasferitoincahe errore: " + e.Message);
            }

            return retval;
        }


        [WebMethod]
        public DocsPaVO.Caching.InfoFileCaching[] getDocumentiInCache(string aggiornato, string idAmministrazione)
        {
            DocsPaVO.Caching.InfoFileCaching[] Info = null;
            try
            { 
             Info = CacheFileManager.GetDocumentiInCache(aggiornato, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method getDocumentiIncache errore: " + e.Message);
                Info = new DocsPaVO.Caching.InfoFileCaching[0];
            }

            return Info;
        }

        [WebMethod]
        public void deleteInfoFileInCache(DocsPaVO.Caching.InfoFileCaching info)
        {
            try
            {
            CacheFileManager.deleteInfoFileInCache(info);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method deleteInfofileIncache errore: " + e.Message);
            }

            
        }

        [WebMethod]
        public DocsPaVO.Caching.InfoFileCaching getFileComponents(string docNumber, string versionId, string idAmministrazione)
        {
            DocsPaVO.Caching.InfoFileCaching Info = null;
            try
            {
            Info= CacheFileManager.getFileDaComponents(docNumber,versionId,idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method getFilecomponents errore: " + e.Message);
            }

            return Info;
        }

        [WebMethod]
        public DocsPaVO.Caching.InfoFileCaching getFileCache(string docNumber, string versionId, string idAmministrazione)
        {

            DocsPaVO.Caching.InfoFileCaching info = null;
            try{
            info = CacheFileManager.getFileDaCache(docNumber, versionId, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method getfilecache errore: " + e.Message);
            }

            return info;
        }

        [WebMethod]
        public bool inCache(string docNumber, string versionId, string idAmministrazione)
        {
            bool retval = false;
            try 
            {
            DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest();
            fr.versionId = versionId;
            fr.docNumber = docNumber;
            retval= CacheFileManager.verificaEsistenzaFileInCache(fr, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method incache errore: " + e.Message);
            }

            return retval;
        }

        [WebMethod]
        public bool setConfigurazioneCache(DocsPaVO.Caching.CacheConfig info)
        {
            bool retval = false;
            try
            {
                retval = CacheFileManager.setConfigurazioneCache(info);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method setconfigurazionecache errore: " + e.Message);
            }

            return retval;
        }

        [WebMethod]
        public void deleteConfigurazioneCache(string idAmministrazione)
        {
            try
            {
            CacheFileManager.deleteConfigurazioneCache(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method deleteconfigurazionecache errore: " + e.Message);
            }

            
        }

        [WebMethod]
        public string ricercaPathInCache(DocsPaVO.Caching.InfoFileCaching info)
        {
            string path = string.Empty;
            try
            {
                path = CacheFileManager.ricercaPathCaching(info.DocNumber.ToString(), info.Version_id.ToString(), info.idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method ricercapathincache errore: " + e.Message);
            }

            return path;
            
        }

        [WebMethod]
        public DocsPaVO.Caching.InfoFileCaching massimaVersioneDelDocumento(string docNumber)
        {
            DocsPaVO.Caching.InfoFileCaching info = null;
            try
            {
                info = CacheFileManager.massimaVersioneDelDocumento(docNumber);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method massimaversionedeldocumento errore: " + e.Message);
            }

            return info;
        }

        [WebMethod]
        public bool writeOnServer(byte[] stream,string pathFile)
        {
            bool retval = false;
            try
            {
            retval = CacheFileManager.writeOnServer(stream, pathFile);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method writeonserver errore: " + e.Message);
            }

            return retval;

        }



    }

}