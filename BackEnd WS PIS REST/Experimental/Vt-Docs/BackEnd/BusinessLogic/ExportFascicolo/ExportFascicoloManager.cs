using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using System.Collections;
using log4net;

namespace BusinessLogic.ExportFascicolo
{
    /// <summary>
    /// Questa classe mette a disposizione metodi per l'esportazione di
    /// un fascicolo, dei suoi sottofascicoli e dei documenti in esso
    /// contenuti.
    /// </summary>
    public class ExportFascicoloManager
    {
        /// <summary>
        /// Metodo costruttore privato per impedire istanziazione oggetto.
        /// </summary>
        private ExportFascicoloManager()
        { }

        private ILog logger = LogManager.GetLogger(typeof(ExportFascicoloManager));

        /// <summary>
        /// Reperimento dei metadati di un fascicolo, compresi di eventuali sottofascicoli e documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static DocsPaVO.ExportFascicolo.MetaInfoFascicolo GetInfoFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string idFascicolo)
        {
            try
            {
                // Creo un oggetto per memorizzare i metadati relativi al fascicolo con id pari a idFascicolo
                DocsPaVO.ExportFascicolo.MetaInfoFascicolo retValue = new DocsPaVO.ExportFascicolo.MetaInfoFascicolo();

                // Richiedo alla logica di business informazioni sul fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idFascicolo, infoUtente);

                // Memorizzo l'id del fascicolo
                retValue.Id = fascicolo.systemID;
                // Memorizzo il nome da attribuire al fascicolo 
                retValue.Nome = GetValidName(fascicolo.codice);

                // Recupero le informazioni sui documenti contenuti in fascicolo
                List<DocsPaVO.ExportFascicolo.MetaInfoDocumento> documenti = GetInfoDocumenti(infoUtente, fascicolo);
                
                //Decommentare per generare il report navigabile dell'export fascicolo
                //documenti.Add(new DocsPaVO.ExportFascicolo.MetaInfoDocumento { IsAllegato = false, IsProtocollo = false, Id = "OFN-FASCID:" + idFascicolo, Nome = retValue.Nome + "_OFN_Index" });

                // Salvo le informazioni sui documenti
                retValue.Documenti = documenti.ToArray();

                // Reperimento metadati dei sottofascicoli
                DocsPaVO.fascicolazione.Folder folder = BusinessLogic.Fascicoli.FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);

                List<DocsPaVO.ExportFascicolo.MetaInfoFascicolo> folders = new List<DocsPaVO.ExportFascicolo.MetaInfoFascicolo>();

                // Per ogni sottofascicolo...
                foreach (DocsPaVO.fascicolazione.Folder child in folder.childs)
                    // ...richiedo le informazioni sul sottofascicolo
                    folders.Add(CreateInfoFascicoloForFolder(child, infoUtente));

                // folders conterrà le informazioni su tutti i sottofascicoli di 
                // 'fascicolo' e sui documenti in essi contenuti
                retValue.Fascicoli = folders.ToArray();

                // Restituisco i metadati relativi al fascicolo
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Funzione per il reperimento delle informazioni sui documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        private static List<DocsPaVO.ExportFascicolo.MetaInfoDocumento> GetInfoDocumenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            return GetInfoDocumenti(infoUtente, BusinessLogic.Fascicoli.FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, fascicolo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        private static string getOriginalFileName(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            string versID = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNumber, infoUtente);
            DocsPaVO.documento.FileRequest fileRq = new DocsPaVO.documento.FileRequest { docNumber = docNumber, versionId = versID };
            string retval =BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, fileRq);
            retval = System.IO.Path.GetFileNameWithoutExtension(retval);
            if (String.IsNullOrEmpty(retval))
                return string.Empty;
            else
                return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        private static string getFileName(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            string versID = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNumber, infoUtente);
            DocsPaVO.documento.FileRequest fileRq = new DocsPaVO.documento.FileRequest { docNumber = docNumber, versionId = versID };
            string retval = BusinessLogic.Documenti.FileManager.getFileName(versID, docNumber);
            if (String.IsNullOrEmpty(retval))
                return string.Empty;
            else
                return retval;
        }

        private static string cleanOriginalFileNameDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento documento)
        {

            string nomeDocumento = GetNomeDocumento(documento);
            string originalFileName = getOriginalFileName(infoUtente, documento.docNumber);
            string nameSepa = "_";
            if (originalFileName.Contains(nomeDocumento))
                originalFileName = originalFileName.Replace(nomeDocumento, String.Empty);

            if (originalFileName.EndsWith("_"))
                originalFileName = originalFileName.Remove(originalFileName.Length - 1);

            string nome = String.Format("{0}{1}{2}",nomeDocumento, nameSepa, originalFileName);
            return nome;
        }


        private static string cleanOriginalFileNameAllegato(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento documento, DocsPaVO.documento.Allegato allegato)
        {

            string nomeDocumento = GetNomeDocumento(documento);
            string nomeAllegato = GetNomeDocumento(allegato);
            string originalFileName = getOriginalFileName(infoUtente, allegato.docNumber);
            string nameSepa = "_";
            if (originalFileName.Contains(nomeDocumento))
                originalFileName= originalFileName.Replace(nomeDocumento, String.Empty);

            if (originalFileName.Contains(nomeAllegato))
                originalFileName = originalFileName.Replace(nomeAllegato, String.Empty);

            if (originalFileName.EndsWith("_"))
                originalFileName = originalFileName.Remove(originalFileName.Length - 1);

            string AllSepa = "-All-";

            string nome = String.Format("{0}{1}{2}", String.Format("{0}{1}{2}", nomeDocumento, AllSepa, nomeAllegato), nameSepa, originalFileName);
            return nome;
        }


        /// <summary>
        /// Reperimento del TSR del docNumber all'ultima versione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumer"></param>
        /// <returns></returns>
        private static string getTimeStampForDocNumber (DocsPaVO.utente.InfoUtente infoUtente,string docNumer)
        {

            string retval = null;
            
            string versione = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNumer, infoUtente);
            FileRequest frTS = new FileRequest { docNumber = docNumer,versionId = versione};
            ArrayList tsAL = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc(infoUtente, frTS);
            foreach (DocsPaVO.documento.TimestampDoc ts  in tsAL)
            {
                if (!string.IsNullOrEmpty(ts.TSR_FILE))
                {
                    byte[] tsrBin = Convert.FromBase64String(ts.TSR_FILE);
                    byte[] fileContent = GetFileDocumento(infoUtente, docNumer).FileContent;
                    bool match = new Documenti.DigitalSignature.VerifyTimeStamp().machTSR(tsrBin, fileContent);
                    
                //Luluciani: per evitare problema Export tsr non valide  Zanotti 
                    // if (match)  
                        retval = ts.TSR_FILE;
                }
                break;
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static List<DocsPaVO.ExportFascicolo.MetaInfoDocumento> GetInfoDocumenti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder)
        {
            // Reperimento dei metadati dei documenti contenuti nel fascicolo
            List<DocsPaVO.ExportFascicolo.MetaInfoDocumento> documenti = new List<DocsPaVO.ExportFascicolo.MetaInfoDocumento>();
            List<String> lstTSR = new List<string>();
            
            
            //Mi ciclo i documenti per cercare eventuali timestamp e li metto in lista
            foreach (DocsPaVO.documento.InfoDocumento documento in BusinessLogic.Fascicoli.FolderManager.getDocumenti(infoUtente.idGruppo, infoUtente.idPeople, folder))
            {
                string strTSR = getTimeStampForDocNumber(infoUtente, documento.idProfile);
                if (strTSR != null)
                {
                    lstTSR.Add(strTSR.ToLowerInvariant());
                    documenti.Add(
                    new DocsPaVO.ExportFascicolo.MetaInfoDocumento
                    {
                        Id = "TSR_" + documento.idProfile,
                        Nome = cleanOriginalFileNameDocumento(infoUtente, documento) + ".TSR",
                        FullName = getFileName(infoUtente, documento.docNumber) + ".TSR",
                        IsAllegato = false,
                        IsProtocollo = false
                    });
                }

                // Richiedo gli allegati del documento
                foreach (DocsPaVO.documento.Allegato allegato in BusinessLogic.Documenti.AllegatiManager.getAllegati(documento.docNumber, string.Empty))
                {
                    //relativo timestamp
                    strTSR = getTimeStampForDocNumber(infoUtente, allegato.docNumber);
                    if (strTSR != null)
                    {
                        lstTSR.Add(strTSR.ToLowerInvariant());
                        documenti.Add(
                        new DocsPaVO.ExportFascicolo.MetaInfoDocumento
                        {
                            Id = "TSR_" + allegato.docNumber,
                            Nome = cleanOriginalFileNameAllegato(infoUtente, documento, allegato),
                            FullName = getFileName(infoUtente, allegato.docNumber),
                            IsAllegato = false,
                            IsProtocollo = false
                        });
                    }
                }
            }


            foreach (DocsPaVO.documento.InfoDocumento documento in BusinessLogic.Fascicoli.FolderManager.getDocumenti(infoUtente.idGruppo, infoUtente.idPeople, folder))
            {
                bool toAdd = true;
                if (System.IO.Path.GetExtension(getOriginalFileName(infoUtente, documento.docNumber)).ToLowerInvariant().Contains("tsr"))
                {
                    string base64TSR = Convert.ToBase64String(GetFile(infoUtente, documento.docNumber).content).ToLowerInvariant();
                    if (lstTSR.Contains(base64TSR))
                    {
                        toAdd = false;
                    }
                }

                if (toAdd)
                {
                    documenti.Add(
                    new DocsPaVO.ExportFascicolo.MetaInfoDocumento
                    {
                        Id = documento.idProfile,
                        Nome = cleanOriginalFileNameDocumento(infoUtente, documento),
                        FullName = getFileName(infoUtente, documento.docNumber),
                        IsAllegato = documento.allegato,
                        IsProtocollo = (documento.tipoProto != "G")
                    });
                }
               // Richiedo gli allegati del documento
                foreach (DocsPaVO.documento.Allegato allegato in BusinessLogic.Documenti.AllegatiManager.getAllegati(documento.docNumber, string.Empty))
                {

                    toAdd = true;

                    if (System.IO.Path.GetExtension(allegato.fileName).ToLowerInvariant().Contains("tsr"))
                    {
                        string base64TSR = Convert.ToBase64String(GetFile(infoUtente, allegato.docNumber).content).ToLowerInvariant();
                        if (lstTSR.Contains(base64TSR))
                        {
                            toAdd = false;
                        }
                    }

                    if (toAdd)
                    {
                        documenti.Add(
                        new DocsPaVO.ExportFascicolo.MetaInfoDocumento
                        {
                            Id = allegato.docNumber,
                            IsAllegato = true,
                            IsProtocollo = false,
                            FullName = getFileName(infoUtente, allegato.docNumber),
                            Nome = cleanOriginalFileNameAllegato(infoUtente, documento, allegato),
                        });
                    }
                }
            }
            return documenti;
        }

        /// <summary>
        /// Funzione per il repriemento delle informazioni sui sottofascicoli
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static DocsPaVO.ExportFascicolo.MetaInfoFascicolo CreateInfoFascicoloForFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.InfoUtente infoUtente)
        {
            // Salvo le metainformaizoni di folder
            DocsPaVO.ExportFascicolo.MetaInfoFascicolo infoFascicolo = new DocsPaVO.ExportFascicolo.MetaInfoFascicolo
            {
                Id = folder.systemID,
                Nome = GetValidName(folder.descrizione)
            };

            // Memorizzo le informazioni sui documenti presenti all'interno di fascicolo
            infoFascicolo.Documenti = GetInfoDocumenti(infoUtente, folder).ToArray();

            List<DocsPaVO.ExportFascicolo.MetaInfoFascicolo> folders = new List<DocsPaVO.ExportFascicolo.MetaInfoFascicolo>();
            // Per ogni sottofascicolo di folder...
            foreach (DocsPaVO.fascicolazione.Folder childFolder in folder.childs)
                // ...ricavo le informazioni sui sottofascicoli e sui 
                // documenti in essi contenuti...
                folders.Add(CreateInfoFascicoloForFolder(childFolder, infoUtente));

            // Memorizzo le informazioni sui sottofascicoli
            infoFascicolo.Fascicoli = folders.ToArray();

            // Restituisco le metainformazioni su fascicolo e sui suoi sottofascicoli
            return infoFascicolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.ExportFascicolo.ContentDocumento GetFileDocumento(DocsPaVO.utente.InfoUtente infoUtente, string numDocumento)
        {

            //gestione TSR
            if (numDocumento.StartsWith ("TSR_"))
            {
                //reperisco il docnumber
                string docNum = numDocumento.Split('_')[1];
                string tsrBase64 = getTimeStampForDocNumber(infoUtente, docNum);
                DocsPaVO.documento.FileDocumento fdTsr = GetFile(infoUtente, docNum);
        
                if (!String.IsNullOrEmpty(tsrBase64))
                {
                    return new DocsPaVO.ExportFascicolo.ContentDocumento
                    {
                        FileContent = Convert.FromBase64String (tsrBase64),
                        MimeType = "application/timestamp-reply",
                        FileExtension = System.IO.Path.GetExtension(fdTsr.name) + ".TSR"
                    };
                }
            }

            // Download del file associato al documento
            DocsPaVO.documento.FileDocumento fileDocumento = GetFile(infoUtente, numDocumento);

            if (fileDocumento != null)
            {
                return new DocsPaVO.ExportFascicolo.ContentDocumento
                {
                    FileContent = fileDocumento.content,
                    MimeType = fileDocumento.contentType,
                    FileExtension = System.IO.Path.GetExtension(fileDocumento.name)
                };
            }
            else
                return null;
        }

        /// <summary>
        /// Reperimento file associato al documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileDocumento GetFile(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
            DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
            
            DocsPaVO.documento.FileDocumento retVal = null;

            try
            {
                if (IsFileAcquired(fileRequest))
                {
                    //retVal = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);

                    //prelevo il file firmato se presente e non lo sbustato
                    retVal = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);
                    // Download del file associato al documento
                 
                }
            }
            catch (Exception ex) { }

            return retVal;

        }

        private static string GetMetaInfoDocumentoOFN(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.ExportFascicolo.MetaInfoFascicolo fascicolo,ref string report,string path)
        {
            int iterator = 0;
            for (iterator=0;iterator< fascicolo.Documenti.Length;iterator++)
            {
                DocsPaVO.ExportFascicolo.MetaInfoDocumento documentoNext = null;
                DocsPaVO.ExportFascicolo.MetaInfoDocumento documento = fascicolo.Documenti[iterator];

                if ((iterator+1) <fascicolo.Documenti.Length)
                    documentoNext = fascicolo.Documenti[iterator+1];

                if (!documento.Id.StartsWith("OFN-FASCID:"))
                {
                    
                    string ofn = string.Empty;
                    string ext = string.Empty;
                    string versID = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(documento.Id, infoUtente);
                    DocsPaVO.documento.FileRequest fileRq = new DocsPaVO.documento.FileRequest { docNumber = documento.Id, versionId = versID };
                    ofn = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, fileRq);
                    DocsPaVO.documento.FileDocumento fd = GetFile(infoUtente, documento.Id);

                    //prendiamo l'impronta per sapere se è aquisito
                    string impronta = null; ;
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    doc.GetImpronta(out impronta, versID, documento.Id);
                    if (String.IsNullOrEmpty(impronta)) //non aquisito
                        continue;
                    
                    string filepath = BusinessLogic.Documenti.FileManager.getCurrentVersionFilePath(infoUtente, documento.Id);
                    if (String.IsNullOrEmpty(filepath)) //non aquisito
                        continue;

                    ext = System.IO.Path.GetExtension(filepath);
                    
                    if (string.IsNullOrEmpty(ofn))
                        ofn = documento.Nome;

                    

                    string nomeFile = path+"\\" + documento.Nome + ext;
                    if (nomeFile.StartsWith("\\"))
                        nomeFile = nomeFile.Substring(1);

                    string nomeOri = path + "\\" + ofn;
                    if (nomeOri.StartsWith("\\"))
                        nomeOri = nomeOri.Substring(1);

     

                    if (documento.IsAllegato)   
                        report += "<li>";

                    report += string.Format("<a href=\"{0}\" TITLE={2}>{1}</a> <br/>\r\n", nomeFile, nomeOri,documento.Nome);

                    if (documento.IsAllegato) 
                        report += "</li>";


                    //Gestione indentatura per il file html
                    if (documentoNext != null)
                    {
                        if (documento.IsAllegato)
                        {
                            if (!documentoNext.IsAllegato)
                                report += "</ul>";
                        }
                        else
                        {
                            if (documentoNext.IsAllegato)
                                report += "<ul>";
                        }
                    }
                    else
                    {
                        if (documento.IsAllegato)
                            report += "</ul>";
                    }
                }
            }

            foreach (DocsPaVO.ExportFascicolo.MetaInfoFascicolo sottoFascicoli in fascicolo.Fascicoli)
            {
                string nomesottofascicolo = path + "\\" + sottoFascicoli.Nome;
                if (nomesottofascicolo.StartsWith("\\"))
                    nomesottofascicolo = nomesottofascicolo.Substring(1);

                report += "<p><h2>Sottofascicolo "+ nomesottofascicolo + "</h2></p>";
                GetMetaInfoDocumentoOFN(infoUtente, sottoFascicoli, ref report, path + "\\" + sottoFascicoli.Nome);
            }

            return report;
        }

        public static DocsPaVO.ExportFascicolo.ContentDocumento GeneraReportOfn(DocsPaVO.utente.InfoUtente infoUtente, string IdFascicolo)
        {
            DocsPaVO.ExportFascicolo.MetaInfoFascicolo fascicolo = BusinessLogic.ExportFascicolo.ExportFascicoloManager.GetInfoFascicolo(infoUtente, IdFascicolo);
            string style = "<style TYPE=\"text/css\"><!-- ul {  margin-top: 0px;  margin-bottom: 0px;  padding-top: 0px;  padding-bottom: 0px;} --></style>";

            string report = String.Format ( "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><title>Indice Fascicolo {0} </title> {1} </head><body><p><h1>Contenuto Fascicolo {0}</h1></p>\r\n",fascicolo.Nome ,style);
            report = GetMetaInfoDocumentoOFN(infoUtente, fascicolo, ref report,null);
            report += "</body></html>";
            DocsPaVO.ExportFascicolo.ContentDocumento contentDocumento = new DocsPaVO.ExportFascicolo.ContentDocumento { MimeType = "text/html", FileExtension = ".htm", FileContent = System.Text.ASCIIEncoding.ASCII.GetBytes(report) };
            return contentDocumento;
        }

        /// <summary>
        /// Reperimento del nome del documento adatto per il file system
        /// </summary>
        /// <param name="documento"></param>
        /// 
        /// <returns></returns>
        private static string GetNomeDocumento(DocsPaVO.documento.InfoDocumento documento)
        {
            // Se il documento è un protocollo...
            if (!String.IsNullOrEmpty (documento.segnatura))
                // ...il nome viene ricavato dalla segnatura
                return GetValidName(documento.segnatura);
            else
                // ...altrimenti se è un grigio il nome è pari all'id del documento
                return GetValidName(documento.docNumber);
                
        }

        private static string GetNomeDocumento(DocsPaVO.documento.Allegato allegato)
        {
            return GetValidName(allegato.docNumber);
        }

        /// <summary>
        /// Questa funzione si occupa di sostituire i caratteri non validi
        /// per il salvataggio di un file su file system con un carattere -
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetValidName(string name)
        {
            string retVal = name;

            // Sostituisco tutti i caratteri \
            retVal = retVal.Replace("\\", "_");

            // Sostituisco tutti i caratteri /
            retVal = retVal.Replace("/", "_");

            // Sostituisco tutti i caratteri :
            retVal = retVal.Replace(":", "_");

            // Sostituisco tutti i caratteri *
            retVal = retVal.Replace("*", "_");

            // Sostituisco tutti i caratteri ?
            retVal = retVal.Replace("?", "_");

            // Sostituisco tutti i caratteri "
            retVal = retVal.Replace("\"", "_");

            // Sostituisco tutti i caratteri <
            retVal = retVal.Replace("<", "_");

            // Sostituisco tutti i caratteri >
            retVal = retVal.Replace(">", "_");

            // Sostituisco tutti i caratteri |
            retVal = retVal.Replace("|", "_");

            return retVal;

        }

        /// <summary>
        /// Verifica se il documento risulta acquisito per la versione richiesta
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private static bool IsFileAcquired(DocsPaVO.documento.FileRequest fileRequest)
        {
            return !string.IsNullOrEmpty(fileRequest.fileName) &&
                    fileRequest.fileSize != "0";
        }
    }
}
