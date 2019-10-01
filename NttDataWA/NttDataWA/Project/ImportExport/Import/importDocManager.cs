using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web;
using NttDataWA.DocsPaWR;
using log4net;
using NttDataWA.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Services;


namespace NttDataWA.Project.ImportExport.Import
{
    public class ImportDocManager
    {
        private static DocsPaWebService docsPaWS = new NttDataWA.DocsPaWR.DocsPaWebService();
        private static ILog logger = LogManager.GetLogger(typeof(ImportDocManager));

        /* ABBATANGELI GIANLUIGI
         * Aggiunto come ultimo parametro la cartella root
         * da utilizzare come root per l'acquisizione massiva*/
        public static int checkORCreateDocFolderFasc(Page page, NttDataWA.DocsPaWR.Fascicolo fasc, string absolutePath, NttDataWA.DocsPaWR.FileDocumento fd, string foldName, string type, NttDataWA.DocsPaWR.Folder folder_Root, string componentType)
        {
            int retValue = 0;
            SchedaDocumento scheda = new SchedaDocumento();
            Oggetto ogg = new Oggetto();
            FileRequest fr = null;
            bool daAggiornareUffRef;
            bool inFolder;
            bool fileFormat = false;
            bool fattDaImport = false;

            try
            {
                Folder folderRoot = null;
                if (folder_Root == null)
                {
                    //get folder dal fascicolo Fasc:					
                    folderRoot = NttDataWA.UIManager.ProjectManager.getFolder(page, fasc);
                }
                else
                {
                    folderRoot = NttDataWA.UIManager.ProjectManager.getFolder(page, folder_Root);
                }

                //verifico formato file se chiave attiva su backend
                if (docsPaWS.IsEnabledSupportedFileTypes())
                    fileFormat = true;

                ArrayList splitData = getSplitPath(absolutePath);
                ArrayList rootFolder = findRootFolder(splitData, foldName);

                if ((rootFolder.Count > 0))
                    for (int i = 0; i < rootFolder.Count; i++)
                    {
                        if (type == "DIR")
                        {
                            retValue = checkExsistNode(page, ref folderRoot, rootFolder[i].ToString(), absolutePath, componentType);
                        }

                        if (type == "FILE")
                            if (i != rootFolder.Count - 1)
                                retValue = checkExsistNode(page, ref folderRoot, rootFolder[i].ToString(), absolutePath, componentType);

                            else
                            {
                                string externalExt = getExt(rootFolder[i].ToString());
                                if (verifyFileFormat(page, externalExt, fileFormat))
                                {
                                    if ((externalExt.ToUpper() != "P7M" && externalExt.ToUpper() != "M7M" && externalExt.ToUpper() != "TSD") ||
                                        verifyFileFormat(page, getEnvelopedExt(rootFolder[i].ToString()), fileFormat))
                                    {
                                        if (fd.content.Length > 0)
                                        {
                                            ogg.descrizione = rootFolder[i].ToString();
                                            scheda.oggetto = ogg;
                                            scheda.personale = "0";
                                            scheda.privato = "0";
                                            scheda.userId = UIManager.UserManager.GetInfoUser().userId;
                                            scheda.typeId = "LETTERA";
                                            scheda.tipoProto = "G";
                                            scheda.appId = "ACROBAT";
                                            scheda.idPeople = UIManager.UserManager.GetInfoUser().idPeople;
                                            scheda = docsPaWS.DocumentoAddDocGrigia(scheda, UIManager.UserManager.GetInfoUser(), UIManager.UserManager.GetSelectedRole());
                                            // setto le prop di fileDocumento
                                            fd.length = fd.content.Length;
                                            fd.name = rootFolder[i].ToString();

                                            String message = String.Empty;
                                            //fascicolazione
                                            UIManager.DocumentManager.addDocumentoInFolder(page, scheda.systemId, folderRoot.systemID, false, out inFolder, out message);

                                            //acq img
                                            string errorMessage = string.Empty;
                                            if (scheda.documenti != null && scheda.documenti[0] != null)
                                            {
                                                fr = (NttDataWA.DocsPaWR.FileRequest)scheda.documenti[0];
                                                fr = docsPaWS.DocumentoPutFileImport(fr, fd, UIManager.UserManager.GetInfoUser(), out errorMessage);

                                                if (fr != null)
                                                    retValue = 1;
                                                else  //se l'acquisizione non va a buon fine cestino il documento
                                                {
                                                    if (!string.IsNullOrEmpty(errorMessage) && errorMessage.Equals("NOME_FILE_TROPPO_LUNGO"))
                                                    {
                                                        UIManager.DocumentManager.CestinaDocumento(page, scheda, scheda.tipoProto, "Errore nell'acquisizione del documento da import", out errorMessage);
                                                        retValue = 7;
                                                    }
                                                }
                                                    
                                            }

                                            // controllo fattura
                                            if ((externalExt.ToUpper() == "P7M" || externalExt.ToUpper() == "M7M" || externalExt.ToUpper() == "TSD")
                                                && (getEnvelopedExt(rootFolder[i].ToString())).ToUpper() == "XML")
                                            {
                                                fattDaImport = true;
                                            }
                                            else if(externalExt.ToUpper() == "XML")
                                            {
                                                string xmlFatt = System.Text.Encoding.UTF8.GetString(fd.content);
                                                xmlFatt = xmlFatt.Trim();
                                                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                                                if (xmlFatt.Contains("xml version=\"1.1\""))
                                                {
                                                    logger.Debug("Versione XML 1.1. Provo conversione");
                                                    xmlFatt = xmlFatt.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                                                }
                                                xmlDoc.LoadXml(xmlFatt);
                                                if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                                                    xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                                                {
                                                    fattDaImport = true;
                                                }
                                                else
                                                {
                                                    logger.Debug("Il file xml non è una fattura.");
                                                }
                                            }

                                            if (fattDaImport)
                                            {
                                                // eseguo il metodo esposto che va fino al FattElAttiveDaImport
                                                string retImpFAtt= docsPaWS.FattElAttiveDaImport(scheda.systemId, UIManager.UserManager.GetInfoUser());
                                                if (retImpFAtt == "Fattura non firmata")
                                                {
                                                    retValue = 6;
                                                }
                                            }

                                        }
                                    }
                                    else retValue = 21;
                                }
                                else retValue = 2;
                            }
                    }
            }

            catch (Exception ex) {
                logger.Error("Errore: ", ex);
                retValue = 0; 
            }

            return retValue;
        }

        private static bool verifyFileFormat(Page page, string fileName, bool attivaFF)
        {
            bool result = false;
            if (attivaFF)
            {
                //gestione del fileTypes 
                NttDataWA.DocsPaWR.SupportedFileType filesupp = new SupportedFileType();
                filesupp = docsPaWS.GetSupportedFileType(Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione), fileName);
                if (filesupp != null && filesupp.FileTypeUsed)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else result = true;

            return result;
        }

        private static string getExt(string FileName)
        {
            string result = null;

            /* char[] separatore = null;
             string carattere = ".";
             separatore = carattere.ToCharArray();
             string[] path = FileName.Split(separatore);
             if (path.Length > 0)
             {
                 result = path[1].ToString();
             }*/

            int finalPoint = FileName.LastIndexOf('.');
            int lengthFileName = FileName.Length - 1;
            int finalFileName = lengthFileName - finalPoint;

            if (finalPoint != -1)
            {
                result = FileName.Substring(finalPoint + 1, finalFileName);
            }

            return result;



        }

        private static string getEnvelopedExt(string filename)
        {
            string result = null;

            int finalPoint = filename.LastIndexOf('.');
            int lengthFileName = filename.Length - 1;
            int finalFileName = lengthFileName - finalPoint;

            if (finalPoint != -1)
            {
                result = filename.Substring(finalPoint + 1, finalFileName);
            }
            string tempfilename = filename.Substring(0, lengthFileName - finalFileName);
            while (result.ToUpper() == "P7M" || result.ToUpper() == "M7M" || result.ToUpper() == "TSD")
            {
                finalPoint = tempfilename.LastIndexOf('.');
                lengthFileName = tempfilename.Length - 1;
                finalFileName = lengthFileName - finalPoint;

                if (finalPoint != -1)
                {
                    result = tempfilename.Substring(finalPoint + 1, finalFileName);
                    tempfilename = tempfilename.Substring(0, lengthFileName - finalFileName);
                }
            }
            return result;
        }

        public static ArrayList getSplitPath(string absolutepath)
        {
            ArrayList data = new ArrayList();
            char[] separatore = null;
            string carattere = @"\";
            separatore = carattere.ToCharArray();
            string[] path = absolutepath.Split(separatore);
            // i primi due valori dell'array contentgono
            // pos 0 il drive
            // pos 1 il percorso scelto
            if (path.Length > 2)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    data.Add(path[i].ToString());
                }
            }

            return data;
        }

        public static int checkExsistNode(Page page, ref DocsPaWR.Folder folderRoot, string descFolder, string key = null, string componentType = null)
        {
            bool dacreare = true;
            string idFolderAdded = string.Empty;
            int result = 0;

            for (int i = 0; i < folderRoot.childs.Length; i++)
            {
                if (descFolder == folderRoot.childs[i].descrizione)
                {
                    folderRoot = folderRoot.childs[i];
                    result = 4;
                    dacreare = false;
                }
            }
            if (dacreare)
            {
                ResultCreazioneFolder resultFold = new ResultCreazioneFolder();
                //crea folder
                DocsPaWR.Folder cartella = new Folder();
                cartella.descrizione = descFolder;
                cartella.idFascicolo = folderRoot.idFascicolo;
                cartella.idParent = folderRoot.systemID;
                UIManager.ProjectManager.newFolder(page, ref cartella, UIManager.UserManager.GetInfoUser(), UIManager.UserManager.GetSelectedRole(), out resultFold);

                folderRoot = cartella;

                if (resultFold.ToString() == "OK") result = 3;
                if (resultFold.ToString() == "FOLDER_EXIST") result = 4;

                if (!String.IsNullOrEmpty(componentType) && componentType.Equals(Constans.TYPE_SOCKET))
                    setSessionMapIdFolders(key, cartella.systemID);
                else
                    HttpContext.Current.Session["idFolderAdded"] = idFolderAdded;
            }
            return result;

        }

        public static void setSessionMapIdFolders(String key, String idFolderAdded)
        {
            Dictionary<string, string> mapIdFoldersAdded = (Dictionary<string, string>)HttpContext.Current.Session["mapIdFoldersAdded"];
            if (mapIdFoldersAdded == null)
            {
                mapIdFoldersAdded = new Dictionary<string, string>();
                mapIdFoldersAdded.Add(key, idFolderAdded);
            }
            else
            {
                mapIdFoldersAdded[key] = idFolderAdded;
            }
            HttpContext.Current.Session["mapIdFoldersAdded"] = mapIdFoldersAdded;
        }

        public static void setSessionMapImportStatus(String key, String importStatus)
        {
            Dictionary<string, string> mapImportStatus = (Dictionary<string, string>)HttpContext.Current.Session["mapImportStatus"];
            if (mapImportStatus == null)
            {
                mapImportStatus = new Dictionary<string, string>();
                mapImportStatus.Add(key, importStatus);
            }
            else
            {
                mapImportStatus[key] = importStatus;
            }
            HttpContext.Current.Session["mapImportStatus"] = mapImportStatus;
        }

        public static Dictionary<string, string> getSessionMapIdFolders()
        {
            Dictionary<string, string> mapIdFoldersAdded = (Dictionary<string, string>)HttpContext.Current.Session["mapIdFoldersAdded"];
            if (mapIdFoldersAdded == null)
            {
                mapIdFoldersAdded = new Dictionary<string, string>();
            }
            return mapIdFoldersAdded;
        }

        public static Dictionary<string, string> getSessionMapImportStatus()
        {
            Dictionary<string, string> mapImportStatus = (Dictionary<string, string>)HttpContext.Current.Session["mapImportStatus"];
            if (mapImportStatus == null)
            {
                mapImportStatus = new Dictionary<string, string>();
            }
            return mapImportStatus;
        }

        public static string getImportStatus(string key)
        {
            string importStatus = string.Empty;
            string mapImportStatusJSON = string.Empty;
            string mapIdFoldersAddedJSON = string.Empty;
            Dictionary<string, string> mapImportStatus = null;
            Dictionary<string, string> mapIdFoldersAdded = null;
            List<string> importStatusJSON = new List<string>();
            try
            {
                mapImportStatus = getSessionMapImportStatus();
                mapIdFoldersAdded = getSessionMapIdFolders();
                if (mapImportStatus != null && mapImportStatus.ContainsKey(key))
                {
                    importStatusJSON.Add(mapImportStatus[key]);

                }
                else
                {
                    importStatusJSON.Add(string.Empty);
                }
                if (mapIdFoldersAdded != null && mapIdFoldersAdded.ContainsKey(key))
                {
                    importStatusJSON.Add(mapIdFoldersAdded[key]);
                }
                else
                {
                    importStatusJSON.Add(string.Empty);
                }

                cleanSessionImport(key);

                importStatus = JsonConvert.SerializeObject(importStatusJSON);
            }
            catch (Exception)
            {

            }

            return importStatus;
        }

        public static void cleanSessionImport()
        {
            HttpContext.Current.Session["mapImportStatus"] = null;
            HttpContext.Current.Session["mapIdFoldersAdded"] = null;
        }

        public static void cleanSessionImport(String key)
        {
            Dictionary<string, string> mapImportStatus = (Dictionary<string, string>)HttpContext.Current.Session["mapImportStatus"];
            Dictionary<string, string> mapIdFoldersAdded = (Dictionary<string, string>)HttpContext.Current.Session["mapIdFoldersAdded"];
            if (mapImportStatus != null && mapImportStatus.ContainsKey(key))
            {
                mapImportStatus.Remove(key);
                HttpContext.Current.Session["mapImportStatus"] = mapImportStatus;
            }

            if (mapIdFoldersAdded != null && mapIdFoldersAdded.ContainsKey(key))
            {
                mapIdFoldersAdded.Remove(key);
                HttpContext.Current.Session["mapIdFoldersAdded"] = mapIdFoldersAdded;
            }

        }



        public static ArrayList findRootFolder(ArrayList splitData, string foldName)
        {
            ArrayList result = new ArrayList();

            for (int i = 0; i < splitData.Count; i++)
            {
                if (splitData[i].ToString() == foldName.Split('\\')[foldName.Split('\\').Length - 1])
                {
                    for (int j = i + 1; j < splitData.Count; j++)
                    {
                        result.Add(splitData[j].ToString());
                    }
                    break;
                }
            }
            return result;
        }
    }
}
