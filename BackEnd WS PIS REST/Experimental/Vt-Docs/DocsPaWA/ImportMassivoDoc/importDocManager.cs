using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web;
using DocsPAWA.DocsPaWR;


namespace DocsPAWA
{
    public class importDocManager
    {
        private static DocsPaWebService docsPaWS = ProxyManager.getWS();

        /* ABBATANGELI GIANLUIGI
         * Aggiunto come ultimo parametro la cartella root
         * da utilizzare come root per l'acquisizione massiva*/
        public static int checkORCreateDocFolderFasc(Page page, DocsPAWA.DocsPaWR.Fascicolo fasc, string absolutePath, DocsPAWA.DocsPaWR.FileDocumento fd, string foldName, string type, DocsPAWA.DocsPaWR.Folder folder_Root)
        {
            int retValue = 0;
            DocsPaWR.SchedaDocumento scheda = new SchedaDocumento();
            DocsPaWR.Oggetto ogg = new Oggetto();
            DocsPaWR.FileRequest fr = null;
            bool daAggiornareUffRef;
            bool inFolder;
            bool fileFormat = false;

            try
            {
                DocsPaWR.Folder folderRoot = null;
                if (folder_Root == null)
                {
                    //get folder dal fascicolo Fasc:					
                    folderRoot = FascicoliManager.getFolder(page, fasc);
                }
                else
                {
                    folderRoot = FascicoliManager.getFolder(page, folder_Root);
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
                            retValue = checkExsistNode(page, ref folderRoot, rootFolder[i].ToString());
                        }

                        if (type == "FILE")
                            if (i != rootFolder.Count - 1)
                                retValue = checkExsistNode(page, ref folderRoot, rootFolder[i].ToString());

                            else
                                if (verifyFileFormat(page, getExt(rootFolder[i].ToString()), fileFormat))
                                {
                                    if (fd.content.Length > 0)
                                    {
                                        ogg.descrizione = rootFolder[i].ToString();
                                        scheda.oggetto = ogg;
                                        scheda.personale = "0";
                                        scheda.privato = "0";
                                        scheda.userId = UserManager.getInfoUtente(page).userId;
                                        scheda.typeId = "LETTERA";
                                        scheda.tipoProto = "G";
                                        scheda.appId = "ACROBAT";
                                        scheda.idPeople = UserManager.getInfoUtente(page).idPeople;
                                        scheda = docsPaWS.DocumentoAddDocGrigia(scheda, UserManager.getInfoUtente(page), UserManager.getRuolo(page));
                                        // setto le prop di fileDocumento
                                        fd.length = fd.content.Length;
                                        fd.name = rootFolder[i].ToString();

                                        String message = String.Empty;
                                        //fascicolazione
                                        DocumentManager.addDocumentoInFolder(page, scheda.systemId, folderRoot.systemID, false, out inFolder, out message);

                                        //acq img
                                        if (scheda.documenti != null && scheda.documenti[0] != null)
                                        {
                                            fr = (DocsPAWA.DocsPaWR.FileRequest)scheda.documenti[0];
                                            fr = docsPaWS.DocumentoPutFile(fr, fd, UserManager.getInfoUtente(page));

                                            if (fr != null) retValue = 1;
                                        }

                                    }
                                }
                                else retValue = 2;
                    }
            }

            catch (Exception ex) { retValue = 0; }

            return retValue;
        }
        
        private static bool verifyFileFormat(Page page, string fileName, bool attivaFF)
        {
            bool result = false;
            if (attivaFF)
            {
                //gestione del fileTypes 
                DocsPAWA.DocsPaWR.SupportedFileType filesupp = new SupportedFileType();
                filesupp = docsPaWS.GetSupportedFileType(Convert.ToInt32(UserManager.getInfoUtente(page).idAmministrazione), fileName);
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
            int lengthFileName = FileName.Length-1;
            int finalFileName = lengthFileName-finalPoint;

            if (finalPoint != -1)
            {
                result = FileName.Substring(finalPoint+1, finalFileName);
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

        public static int checkExsistNode(Page page, ref DocsPaWR.Folder folderRoot, string descFolder)
        {
            bool dacreare = true;

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
                FascicoliManager.newFolder(page, ref cartella, UserManager.getInfoUtente(page), UserManager.getRuolo(page), out resultFold);

                folderRoot = cartella;

                if (resultFold.ToString() == "OK") result = 3;
                if (resultFold.ToString() == "FOLDER_EXIST") result = 4;
            }
            return result;

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
