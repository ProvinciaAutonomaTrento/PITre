using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Configuration;
using BusinessLogic.Documenti.DigitalSignature;
using iTextSharp.text.pdf;
using System.Xml;
using System.Text;
using log4net;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using DocsPaVO.documento;
using PDFLinearizator;
using DocsPaUtils.Security;
using DocsPaVO.utente;
using InfoCertSignerConnector;
using InlineConverterEngine.Proxy;

namespace BusinessLogic.Documenti
{
    /// <summary>
    /// </summary>
    public class FileManager
    {

        class PdfInfo
        {
            public string version;
            public bool IsSigned;
            public bool IsPdfA;
            public bool HasJava;
            public string conformance;
            public bool HasBiometricData;

        }

        private static ILog logger = LogManager.GetLogger(typeof(FileManager));


        //Abilitazione Features
        private static bool GestioneTSAttacced = true;
        private static bool GestionePades = true;

        #region Flag DaInviare
        /// <summary>
        /// Setta ad 1 il flag CHA_DA_INVIARE della tabella VERSION
        /// Deve essere invocato prima di aggiungere la nuova versione del documento pdf derivante
        /// dal contenuto del file P7M
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento setFlagDaInviare(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.SetFlagDaInviare(ref schedaDoc);
            return schedaDoc;

            #region Codice Commentato
            /*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try 
			{
				db.openConnection();
				string updateStr = "UPDATE VERSIONS SET CHA_DA_INVIARE = '0' WHERE NOT CHA_DA_INVIARE = '0' AND DOCNUMBER=" + schedaDoc.docNumber;
				logger.Debug(updateStr);
				db.executeNonQuery(updateStr);
				updateStr = "UPDATE VERSIONS SET CHA_DA_INVIARE = '1' WHERE VERSION_ID=" + ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).versionId;
				logger.Debug(updateStr);
				db.executeNonQuery(updateStr);
				db.closeConnection();
				for (int i = 1; i < schedaDoc.documenti.Count; i++) 
				{
					((DocsPaVO.documento.Documento)schedaDoc.documenti[i]).daInviare = "0";
				}
				((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).daInviare = "1";
			}
			catch(Exception e) 
			{
				logger.Debug(e.Message);
				db.closeConnection();
				
				throw new Exception("F_System");
			}
			return schedaDoc;*/
            #endregion
        }
        #endregion




        #region Get File

        /// <summary>
        /// getFile, con dati etichetta, funziona solo se file è pdf.
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento getFileConSegnatura(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente infoutente, string xmlPath, DocsPaVO.documento.labelPdf position, bool convertUsingLifecycle)
        {
            logger.Info("BEGIN");
            bool isAttachment = ((sch.documenti.ToArray())[0] as DocsPaVO.documento.Documento).docNumber.Equals(objFileRequest.docNumber) ? false : true;
            if (sch.documentoPrincipale != null) isAttachment = true;
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            fileDoc.path = objFileRequest.docServerLoc + objFileRequest.path;
            fileDoc.name = objFileRequest.fileName;
            if (ConfigurationManager.AppSettings["documentale"].ToUpper() == "FILENET")
            {
                DocsPaDocumentale.Documentale.DocumentManager FNdoc = new DocsPaDocumentale.Documentale.DocumentManager(infoutente);
                fileDoc.name = FNdoc.GetOriginalFileName(objFileRequest.docNumber, objFileRequest.versionId);
            }
            int indice;
            indice = fileDoc.name.LastIndexOf(@"\");
            if (indice < (fileDoc.name.Length - 1))
            {
                fileDoc.name = fileDoc.name.Substring(indice + 1);
            }

            //modifica
            if (string.IsNullOrEmpty(fileDoc.path))
                fileDoc.fullName = fileDoc.name;
            else
                //fine modifica
                fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;
            fileDoc.contentType = getContentType(fileDoc.name);
            logger.Debug("Full name: " + fileDoc.fullName);
            logger.Debug("idAmministrazione: " + infoutente.idAmministrazione);
            logger.Debug("Library: " + DocsPaDB.Utils.Personalization.getInstance(infoutente.idAmministrazione).getLibrary());

            string docNumber = objFileRequest.docNumber;

            string version_label = objFileRequest.versionLabel;

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoutente);

            if ((fileDoc.fullName.ToUpper().EndsWith("P7M")) || //cades
                (fileDoc.fullName.ToUpper().EndsWith("TSD")) || //timestamp
                (fileDoc.fullName.ToUpper().EndsWith("M7M")) || //timestamp
                (fileDoc.fullName.ToUpper().EndsWith("PDF")))   //pades
            {
                fileDoc = FileManager.getFile(objFileRequest, infoutente, true, false);
                //TODO: if(fileDoc!=null && fileDoc.content!=null) continue;
            }
            else if (!documentManager.GetFile(ref fileDoc, ref objFileRequest))
            {
                logger.Debug("Errore nella gestione del File (getFile)");
                throw new Exception();
            }
            string version_label_allegato = string.Empty;
            if (isAttachment)
                version_label_allegato = (from allegato in (sch.allegati.Cast<DocsPaVO.documento.Allegato>().ToArray()) where allegato.docNumber.Equals(objFileRequest.docNumber) select allegato.versionLabel).FirstOrDefault();
            //Controllo se il file non fosse PDF, se fosse diverso da PDF lo mando a lifecycle per la conversione

            if ((Path.GetExtension(fileDoc.name).ToLowerInvariant() != ".pdf") && (convertUsingLifecycle == true))
            {
                DocsPaVO.documento.FileDocumento lcfileDoc = LiveCycle.LiveCycle.GeneratePDFInSyncMod(fileDoc);
                if (lcfileDoc != null)
                    fileDoc = lcfileDoc;
                else
                    throw new Exception("Non è stato possibile convertire il file con LifeCycle");
            }

            fileDoc = addEtic(ref fileDoc, sch, infoutente, position, version_label_allegato, objFileRequest.versionId);
            fileDoc.contentType = getContentType(fileDoc.name);

            int indice2;
            indice2 = fileDoc.name.LastIndexOf(@"\");
            if (indice2 < (fileDoc.name.Length - 1))
            {
                fileDoc.name = fileDoc.name.Substring(indice2 + 1);
            }
            logger.Info("END");
            return fileDoc;
        }

        public static DocsPaVO.documento.FileDocumento setFileConSegnatura(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente infoutente, string xmlPath, string position, ref DocsPaVO.documento.FileDocumento fileDoc)
        {

            int indice;
            indice = fileDoc.name.LastIndexOf(@"\");
            if (indice < (fileDoc.name.Length - 1))
            {
                fileDoc.name = fileDoc.name.Substring(indice + 1);
            }
            fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;
            fileDoc.contentType = getContentType(fileDoc.name);

            fileDoc = addEtic(ref fileDoc, sch, infoutente, xmlPath, position);
            fileDoc.contentType = getContentType(fileDoc.name);

            int indice2;
            indice2 = fileDoc.name.LastIndexOf(@"\");
            if (indice2 < (fileDoc.name.Length - 1))
            {
                fileDoc.name = fileDoc.name.Substring(indice2 + 1);
            }

            return fileDoc;
        }

        public static DocsPaVO.documento.FileDocumento getVoidFileConSegnatura(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente infoutente, string xmlPath, DocsPaVO.documento.labelPdf position)
        {
            logger.Info("BEGIN");
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            fileDoc = addVoidEtic(ref fileDoc, sch, infoutente, position);
            logger.Info("END");
            return fileDoc;
        }


        private static XmlNode LoadRootXml(XmlDocument xmlDoc)
        {
            string pathCodice = "//labelPdf";
            XmlNode nodoRoot = xmlDoc.SelectSingleNode(pathCodice);
            nodoRoot = nodoRoot.ParentNode;
            return nodoRoot;
        }

        #region load Properties eticPdf from XML
        /// <summary>
        /// Caricamento informazioni per Label PDF
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        //private static void loadXmlLabelProperties(DocsPaVO.documento.FileDocumento file, string xmlPath, string position)
        //{
        //    string _xmlPath = xmlPath;
        //    string delimitatore = "-";

        //    XmlDocument _xml = new XmlDocument();
        //    try
        //    {
        //        _xml.Load(xmlPath);	
        //        //carico info Font
        //        XmlNode NodoFont = _xml.SelectSingleNode(".//font");	
        //        XmlNodeList nodiFont = NodoFont.ChildNodes;

        //        file.LabelPdf.font_type = nodiFont[0].InnerText;
        //        file.LabelPdf.font_color = nodiFont[1].InnerText;
        //        file.LabelPdf.font_size = nodiFont[2].InnerText;
        //        // carico Info Label
        //        XmlNode NodoLabel = _xml.SelectSingleNode("./labelPdf");
        //        XmlNodeList nodiLabel = NodoLabel.ChildNodes;
        //        file.LabelPdf.label_rotation = nodiLabel[2].InnerText;

        //        #region LoadDefaulPosition
        //        // carico le 4 posizioni
        //        // posizione Alto Sinistra
        //        DocsPaVO.documento.position pos_upSx = new DocsPaVO.documento.position();
        //        XmlNode NodoPosition = _xml.SelectSingleNode(".//pos_upSx");
        //        XmlNodeList nodiPosition = NodoPosition.ChildNodes;
        //        pos_upSx.posName="pos_upSx";
        //        pos_upSx.PosX = nodiPosition[0].InnerText;
        //        pos_upSx.PosY = nodiPosition[1].InnerText;
        //        file.LabelPdf.positions.Add(pos_upSx);
        //        // posizione Alto Destra
        //        DocsPaVO.documento.position pos_upDx = new DocsPaVO.documento.position();
        //        NodoPosition = _xml.SelectSingleNode(".//pos_upDx");
        //        nodiPosition = NodoPosition.ChildNodes;
        //        pos_upDx.posName="pos_upDx";
        //        pos_upDx.PosX = nodiPosition[0].InnerText;
        //        pos_upDx.PosY = nodiPosition[1].InnerText;
        //        file.LabelPdf.positions.Add(pos_upDx);
        //        // posizione basso Sinistra
        //        DocsPaVO.documento.position pos_downSx = new DocsPaVO.documento.position();
        //        NodoPosition = _xml.SelectSingleNode(".//pos_downSx");
        //        nodiPosition = NodoPosition.ChildNodes;
        //        pos_downSx.posName="pos_downSx";
        //        pos_downSx.PosX = nodiPosition[0].InnerText;
        //        pos_downSx.PosY = nodiPosition[1].InnerText;
        //        file.LabelPdf.positions.Add(pos_downSx);
        //        // posizione basso Destra
        //        DocsPaVO.documento.position pos_downDx = new DocsPaVO.documento.position();
        //        NodoPosition = _xml.SelectSingleNode(".//pos_downDx");
        //        nodiPosition = NodoPosition.ChildNodes;
        //        pos_downDx.posName="pos_downDx";
        //        pos_downDx.PosX = nodiPosition[0].InnerText;
        //        pos_downDx.PosY = nodiPosition[1].InnerText;
        //        file.LabelPdf.positions.Add(pos_downDx);
        //        #endregion

        //            if ((position==null)||(position==""))
        //            {
        //                //prendo la default su XML
        //                file.LabelPdf.default_position = nodiLabel[3].InnerText;
        //            }
        //            else
        //            {
        //                //verifico le scelte utente
        //                string [] posPers = position.Split(Convert.ToChar(delimitatore));
        //                // è stata scelta una posizione standard
        //                if (posPers.Length == 1)
        //                {
        //                    //forzo la scelta utente con default
        //                    file.LabelPdf.default_position = position;
        //                }
        //                else
        //                {
        //                    file.LabelPdf.default_position = "pos_pers";
        //                    DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
        //                    pos_pers.posName = "pos_pers";
        //                    pos_pers.PosX = posPers[0].ToString();
        //                    pos_pers.PosY = posPers[1].ToString();
        //                    file.LabelPdf.positions.Add(pos_pers);
        //                }
        //            }

        //    }

        //    catch (Exception ex)
        //    {
        //        logger.Debug(ex);
        //        throw ex;
        //    }

        //}
        #endregion

        /// <summary>
        /// Metodo che sfrutta la lettura da DB invece che da file XML: è il nuovo metodo implementato per la versione
        /// che prevede anche il timbro su pdf oltre che la segnatura.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="position"></param>
        /// <param name="Amm"></param>
        private static void loadXmlLabelProperties(DocsPaVO.documento.FileDocumento file, string position, DocsPaVO.amministrazione.InfoAmministrazione Amm)
        {
            string delimitatore = "-";
            try
            {
                //carico info Font
                DocsPaVO.amministrazione.carattere carat = new DocsPaVO.amministrazione.carattere();
                for (int i = 0; i < Amm.Timbro.carattere.Count; i++)
                {
                    carat = (DocsPaVO.amministrazione.carattere)Amm.Timbro.carattere[i];
                    //Se da front-end ho selezionato un tipo di font lo utilizzo altrimenti uso quello
                    //configurato in amministrazione...
                    if (string.IsNullOrEmpty(file.LabelPdf.sel_font))
                    {
                        file.LabelPdf.sel_font = Amm.Timbro_carattere;
                    }
                    //if (carat.id == Amm.Timbro_carattere)
                    if (carat.id == file.LabelPdf.sel_font)
                    {
                        file.LabelPdf.font_type = carat.caratName;
                        file.LabelPdf.font_size = carat.dimensione;
                    }
                }
                DocsPaVO.amministrazione.color colore = new DocsPaVO.amministrazione.color();
                for (int j = 0; j < Amm.Timbro.color.Count; j++)
                {
                    colore = (DocsPaVO.amministrazione.color)Amm.Timbro.color[j];
                    //Se selezionato uso il colore scelto da front-end
                    if (string.IsNullOrEmpty(file.LabelPdf.sel_color))
                    {
                        file.LabelPdf.sel_color = Amm.Timbro_colore;
                    }
                    //if (colore.id == Amm.Timbro_colore)
                    if (colore.id == file.LabelPdf.sel_color)
                    {
                        file.LabelPdf.font_color = colore.colName;
                    }
                }
                file.LabelPdf.label_rotation = Amm.Timbro_rotazione;

                #region LoadDefaulPosition
                // carico le 4 posizioni
                string default_pos = string.Empty;
                DocsPaVO.documento.position pos_upSx = new DocsPaVO.documento.position();
                DocsPaVO.documento.position pos_upDx = new DocsPaVO.documento.position();
                DocsPaVO.documento.position pos_downSx = new DocsPaVO.documento.position();
                DocsPaVO.documento.position pos_downDx = new DocsPaVO.documento.position();
                DocsPaVO.amministrazione.posizione pos = new DocsPaVO.amministrazione.posizione();
                for (int k = 0; k < Amm.Timbro.positions.Count; k++)
                {
                    pos = (DocsPaVO.amministrazione.posizione)Amm.Timbro.positions[k];
                    // posizione Alto Sinistra
                    if (pos.posName == "pos_upSx")
                    {
                        pos_upSx.posName = pos.posName;
                        pos_upSx.PosX = pos.PosX;
                        pos_upSx.PosY = pos.PosY;
                        file.LabelPdf.positions.Add(pos_upSx);
                    }
                    // posizione Alto Destra
                    if (pos.posName == "pos_upDx")
                    {
                        pos_upDx.posName = pos.posName;
                        pos_upDx.PosX = pos.PosX;
                        pos_upDx.PosY = pos.PosY;
                        file.LabelPdf.positions.Add(pos_upDx);
                    }
                    // posizione basso Sinistra
                    if (pos.posName == "pos_downSx")
                    {
                        pos_downSx.posName = pos.posName;
                        pos_downSx.PosX = pos.PosX;
                        pos_downSx.PosY = pos.PosY;
                        file.LabelPdf.positions.Add(pos_downSx);
                    }
                    // posizione basso Destra
                    if (pos.posName == "pos_downDx")
                    {
                        pos_downDx.posName = pos.posName;
                        pos_downDx.PosX = pos.PosX;
                        pos_downDx.PosY = pos.PosY;
                        file.LabelPdf.positions.Add(pos_downDx);
                    }
                    // posizione di default
                    if (pos.id == Amm.Timbro_posizione)
                    {
                        default_pos = pos.posName;
                    }
                }
                #endregion

                if ((position == null) || (position == ""))
                {
                    //prendo la default su XML
                    file.LabelPdf.default_position = default_pos;
                }
                else
                {
                    //verifico le scelte utente
                    string[] posPers = position.Split(Convert.ToChar(delimitatore));
                    // è stata scelta una posizione standard
                    if (posPers.Length == 1)
                    {
                        //forzo la scelta utente con default
                        file.LabelPdf.default_position = position;
                    }
                    else
                    {
                        //prima di passare alle coordinate personalizzate verifico se la x e la y corrispondono ad
                        //una delle coordinate di default...
                        if (posPers[0].ToString() == pos_upSx.PosX)
                        {
                            if (posPers[1].ToString() == pos_upSx.PosY)
                            {
                                file.LabelPdf.default_position = pos_upSx.posName;
                            }
                            else
                            {
                                if (posPers[1].ToString() == pos_downSx.PosY)
                                {
                                    file.LabelPdf.default_position = pos_downSx.posName;
                                }
                                else
                                {
                                    file.LabelPdf.default_position = "pos_pers";
                                    DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
                                    pos_pers.posName = "pos_pers";
                                    pos_pers.PosX = posPers[0].ToString();
                                    pos_pers.PosY = posPers[1].ToString();
                                    file.LabelPdf.positions.Add(pos_pers);
                                }
                            }
                        }
                        else
                        {
                            if (posPers[0].ToString() == pos_upDx.PosX)
                            {
                                if (posPers[1].ToString() == pos_upDx.PosY)
                                {
                                    file.LabelPdf.default_position = pos_upDx.posName;
                                }
                                else
                                {
                                    if (posPers[1].ToString() == pos_downDx.PosY)
                                    {
                                        file.LabelPdf.default_position = pos_downDx.posName;
                                    }
                                    else
                                    {
                                        file.LabelPdf.default_position = "pos_pers";
                                        DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
                                        pos_pers.posName = "pos_pers";
                                        pos_pers.PosX = posPers[0].ToString();
                                        pos_pers.PosY = posPers[1].ToString();
                                        file.LabelPdf.positions.Add(pos_pers);
                                    }
                                }
                            }
                            else
                            {
                                file.LabelPdf.default_position = "pos_pers";
                                DocsPaVO.documento.position pos_pers = new DocsPaVO.documento.position();
                                pos_pers.posName = "pos_pers";
                                pos_pers.PosX = posPers[0].ToString();
                                pos_pers.PosY = posPers[1].ToString();
                                file.LabelPdf.positions.Add(pos_pers);
                            }
                        }

                    }
                }

            }

            catch (Exception ex)
            {
                logger.Debug(ex);
                throw ex;
            }

        }

        /// <summary>
        /// Metodo utilizzato prima dell'aggiunta del timbro sul pdf
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sch"></param>
        /// <param name="utente"></param>
        /// <param name="xmlPath"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileDocumento addEtic(ref DocsPaVO.documento.FileDocumento file, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente utente, string xmlPath, string position)
        {
            //devo ancora valorizzare gli altri campi di labelPdf se necessario...
            DocsPaVO.documento.labelPdf label = new DocsPaVO.documento.labelPdf();
            label.default_position = position;
            return addEtic(ref file, sch, utente, label);
        }

        public static bool IsBlankPdfPage(PdfReader r, int numberPage)
        {
            bool result = false;
            try
            {
                //get the page content
                byte[] bContent = r.GetPageContent(numberPage);
                if (bContent.Length == 0)
                {
                    result = true;
                }
            }
            catch(Exception e)
            {
            //do what you need here
            }
            return result;
        }


        /// <summary>
        /// aggiunge segnatura o timbro ad un file pdf
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileDocumento addEtic(ref DocsPaVO.documento.FileDocumento file, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente utente, DocsPaVO.documento.labelPdf labelPdf, string version_label_allegato = "", string versionId = "")
        {
            int posX = 0;
            int posY = 0;
            byte[] rtn = null;
            PdfInfo info = getPdfInfo(file);

            bool isPades = info.IsSigned;

            // se è pdfa ed è pure firmato non posso fare append perchè la conformità pdfa sarebbe corrotta
            if (info.IsSigned && info.IsPdfA)
                isPades = false;

            //isPades = false;

            // se sono presenti i dati biometrici ritorno il fileinfo senza modificare il file

            bool disabled = false;

            string BE_DO_NOT_LABEL_BIO_PADES = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_DO_NOT_LABEL_BIO_PADES");
            if (!string.IsNullOrEmpty(BE_DO_NOT_LABEL_BIO_PADES))
                if ((BE_DO_NOT_LABEL_BIO_PADES == "1") || (BE_DO_NOT_LABEL_BIO_PADES.ToLower() == "true"))
                    disabled = true;
            if (disabled)
                if (info.HasBiometricData)
                {
                    file.LabelPdf = labelPdf;
                    file.LabelPdf.default_position = "";
                    return file;
                }

            //isPades = false;
            //^^^^^^^^^^^^^^ Interruttore , commentare per attivare lo stamping su firma

            //verranno scritti i file temporanei in un subfolder di REPORTS_PATH
            string basePathFiles = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
            //string fullPath = basePathFiles + @"\EtichettaPdf";
            //string fullPath = basePathFiles.Replace("%DATA", "EtichettaPdf");
            string fullPath = System.IO.Path.Combine(basePathFiles, "EtichettaPdf");
            //standard naming per il file temporaneo
            string fileName = @"\" + Guid.NewGuid() + "_" + utente.userId + ".pdf";
            //combinazione path & filename
            string fullPathFileName = fullPath + fileName;
            System.IO.FileStream fileStream = null;
            System.IO.FileStream fs = null;
            //estraggo la posizione dall'oggetto Label per mantenere inalterato il funzionamento del metodo
            string position = labelPdf.position;
            //aggiungo il carattere ed il colore selezionati all'oggetto label del fileDoc
            if (!string.IsNullOrEmpty(labelPdf.sel_font))
            {
                file.LabelPdf.sel_font = labelPdf.sel_font;
            }
            if (!string.IsNullOrEmpty(labelPdf.sel_color))
            {
                file.LabelPdf.sel_color = labelPdf.sel_color;
            }

            DocsPaVO.amministrazione.InfoAmministrazione currAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione);

            try
            {
                //verifico esistenza directory
                if (!System.IO.Directory.Exists(fullPath))
                {
                    System.IO.Directory.CreateDirectory(fullPath);
                }
                //verifico esistenza file
                if (System.IO.File.Exists(fullPathFileName))
                {
                    System.IO.File.Delete(fullPathFileName);
                }

                if (PdfConverter.CanConvertFile(file.name))
                {
                    // Conversione in pdf del file, 
                    // se la tipologia di file è tra quelle 
                    // per cui è possibile effettuarla
                    ConvertToPdf(utente, file, fullPathFileName);
                }

                //verifico la posizione prescelta
                if ((position == null) || (position == ""))
                {
                    //caricamento preferenze di default di LabelPDF
                    loadXmlLabelProperties(file, null, currAmm);
                }
                else
                {
                    //caricamento preferenze utente per LabelPDF
                    loadXmlLabelProperties(file, position, currAmm);
                }

                System.IO.MemoryStream ms = new System.IO.MemoryStream(file.content, true);

                //Stringa del timbro o della segnatura
                int maxT = 0;
                string maxTimbro = "";
                string escape = "\n";

                //Mev Firma1  < 
                string dati = string.Empty;
                string datiProtRepertorio = string.Empty;
                if (labelPdf.notimbro == false)
                {
                    // visualizza la timbratura se il documento è protocollato

                    //In base all'informazione nel frontEnd decidere se devo caricare la segnatura oppure il Timbro!!!
                    //...mettendo a true l'ultimo parametro carico il timbro!!!
                    dati = GetDatiEtichetta(utente, currAmm, labelPdf, sch, file.signatureResult, version_label_allegato);
                    //rimuovo l'ultimo separatore!!!
                    dati = dati.TrimEnd(Convert.ToChar(escape));

                    // se il documento è repertoriato, visualizza il protocollo di repertorio in alternativa alla segnatura
                    if ((sch.documentoPrincipale != null))
                    {
                        //caso di documento ALLEGATO con documento principale repertoriato e non protocollato
                        if (DocManager.getDettaglio(utente, sch.documentoPrincipale.idProfile, sch.documentoPrincipale.docNumber).protocollo == null)
                            datiProtRepertorio = GetDatiEtichettaProtocolloRepertorio(DocManager.getDettaglio(utente, sch.documentoPrincipale.idProfile, sch.documentoPrincipale.docNumber), currAmm.Codice);
                    }
                    else if(sch.protocollo == null)
                    {
                        //caso di documento repertoriato e non protocollato
                        if(sch.template == null && sch.tipologiaAtto != null && !string.IsNullOrEmpty(sch.tipologiaAtto.systemId))
                        {
                            DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                            DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(sch.docNumber);
                            sch.template = template;
                        }
                        if ((isDocRepertoriato(sch, currAmm.Codice)))
                            datiProtRepertorio = GetDatiEtichettaProtocolloRepertorio(sch, currAmm.Codice);
                    }

                    if (!string.IsNullOrEmpty(datiProtRepertorio))
                        // il protocollo di repertorio viene visualizzato solo se non è attivo la visualizzazione del timbro (orizzontale/verticale)
                        if (!labelPdf.orientamento.ToLower().Equals("orizzontale") && !labelPdf.orientamento.ToLower().Equals("verticale"))
                        dati = datiProtRepertorio;
                    //>

                    // accoda alla label visualizzata le info allegato se è il documento visualizzato è un allegato
                    DocsPaVO.documento.Allegato docAllegato = null;
                    if (isDocAllegato(sch, ref docAllegato))
                        dati += AppendDatiEtichettaAllegato(docAllegato, labelPdf.orientamento);
                }

                string[] rTimbro = dati.Split(Convert.ToChar(escape));

                for (int t = 0; t < rTimbro.Length; t++)
                {
                    if (rTimbro[t].Length > maxT)
                    {
                        maxTimbro = rTimbro[t];
                        maxT = rTimbro[t].Length;
                    }
                    //Rimuovo gli spazi bianchi dai dati del timbro che in caso di orientamento
                    //verticale creano un disallineamento!
                    rTimbro[t] = rTimbro[t].Trim();
                }

                //se dal frontEnd ho valorizzato la rotazione allora la sostituisco a quella configurata
                if (labelPdf.label_rotation != String.Empty)
                    currAmm.Timbro_rotazione = labelPdf.label_rotation;

                //valorizzo i dati da passare al frontEnd
                file.LabelPdf.label_rotation = currAmm.Timbro_rotazione;
                file.LabelPdf.orientamento = labelPdf.orientamento;
                file.LabelPdf.tipoLabel = labelPdf.tipoLabel;

                // posizione 0 del memory stream
                ms.Position = 0;
                iTextSharp.text.pdf.PdfReader prd = null;
                try
                {
                    prd = new PdfReader(ms);
                }
                catch (Exception itextEx)
                {
                    logger.Debug("Itextsharp: impossibile aprire il file - ", itextEx);
                    return file;
                }

                //ABBATANGELI - CODICE DISGUSTOSO IMPOSTO DAL GRUPPO PANZERA-LUCIANI
                for (int i=0;i<rTimbro.Length; i++)
                {
                    if (rTimbro[i].ToUpper().StartsWith("MIBACT|MIBACT_"))
                    {
                        rTimbro[i] = rTimbro[i].Substring(0, 7) + rTimbro[i].Substring(14);
                    }
                }

                // Lista contenente gli indici delle pagine su cui si richiede la stampa del dato
                System.Collections.Generic.Dictionary<int, string[]> pageContentStructure = new System.Collections.Generic.Dictionary<int, string[]>();
                pageContentStructure.Add(1, rTimbro);

                // temporanea CHP
                //labelPdf.digitalSignInfo = new DocsPaVO.documento.labelPdfDigitalSignInfo { printOnLastPage = true, printOnFirstPage = false };

                if (labelPdf.digitalSignInfo != null)
                {
                    //Mev Firma1 < aggiunto parametro dettaglio firma su GetDatiFirmaPerEtichetta e labelPdf
                    string datiFirma = Environment.NewLine + GetDatiFirmaPerEtichetta(file.signatureResult, currAmm.DettaglioFirma, labelPdf, currAmm.IDAmm);
                    //>
                    //Aggiunta informazioni di firma elettronica
                    datiFirma += Environment.NewLine + GetDatiFirmaElettronica(sch.docNumber, versionId, labelPdf, utente);

                    string[] datiFirmaAsArray = datiFirma.Split(Convert.ToChar(escape));

                    if (labelPdf.digitalSignInfo.printOnFirstPage)
                    {
                        // Se si è richiesta la stampa dei dati di firma sulla prima pagina, 
                        // viene accodata alla struttura l'item relativo alla pagina e il contenuto
                        System.Collections.Generic.List<string> content = new System.Collections.Generic.List<string>(pageContentStructure[1]);
                        content.AddRange(datiFirmaAsArray);
                        pageContentStructure[1] = content.ToArray();
                    }

                    if (labelPdf.digitalSignInfo.printOnLastPage)
                    {
                        // Se si è richiesta la stampa dei dati di firma sull'ultima pagina, viene inserita nella struttura l'item relativo alla pagina e il contenuto
                        if (prd.NumberOfPages == 1)
                        {
                            System.Collections.Generic.List<string> content = new System.Collections.Generic.List<string>(pageContentStructure[1]);
                            content.AddRange(datiFirmaAsArray);
                            pageContentStructure[1] = content.ToArray();
                        }
                        else
                            pageContentStructure[prd.NumberOfPages] = datiFirmaAsArray;
                    }

                    file.LabelPdf.digitalSignInfo = labelPdf.digitalSignInfo;
                }

                fs = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.OpenOrCreate);
                iTextSharp.text.pdf.PdfStamper stamp;
                try
                {

                    //if (isPades)
                    //    stamp = new PdfStamper(prd, fs, '\0', true);
                    //else
                    //    stamp = new PdfStamper(prd, fs, '\0', false);

                    if (isPades)
                            stamp = new PdfStamper(prd, fs, '\0', true);
                        else
                            if (info.HasBiometricData && !disabled)
                        {
                            stamp = new PdfStamper(prd, fs, '\0', true);
                        }
                        else
                        {
                            stamp = new PdfStamper(prd, fs, '\0', false);
                        }
                        //stamp = new PdfStamper(prd, fs);
                        //stamp.Writer.Open();
                    }
                    catch (Exception ex)
                {
                    logger.Debug(ex);
                    fs.Close();
                    return file;
                }

                foreach (System.Collections.Generic.KeyValuePair<int, string[]> pair in pageContentStructure)
                {
                    // dimensioni della prima pagina del documento
                    int pageRotation = prd.GetPageRotation(pair.Key);
                    iTextSharp.text.Rectangle rect = null;
                    if (pageRotation == 0)
                    {
                        rect = new iTextSharp.text.Rectangle(prd.GetPageSize(pair.Key));
                    }
                    else
                    {
                        rect = new iTextSharp.text.Rectangle(prd.GetPageSizeWithRotation(pair.Key));

                    }
                    //font di stampa
                    BaseFont bf = castFontTypePreferences(ref file);

                    // verifico le dimensioni massime consentite per la sovrastampa				

                    // calcola l'occupazione in pixel della segnatura
                    int segnaPix = Convert.ToInt32(bf.GetWidthPoint(maxTimbro, Convert.ToInt32(file.LabelPdf.font_size)));

                    // viene allineata la dimensione massima utilizzabile per la sovrastampa
                    // sottraendo la massima occupazione della segnatura con un sfrido di 5 px
                    int maxWidth = (Convert.ToInt32(rect.Width) - segnaPix) - 5;

                    //inserisco le info della pagina PDF sul VO
                    file.LabelPdf.pdfHeight = rect.Height.ToString();
                    file.LabelPdf.pdfWidth = Convert.ToString(maxWidth);

                    #region calcolo con valori percentuali (old)
                    //				//calcolo delle percentuali per le 4 posizioni standard
                    //				int larghezza = Convert.ToInt32(rect.Width);
                    //				int altezza = Convert.ToInt32(rect.Height);
                    //
                    //				int percAltezza = (altezza * 5)/100;
                    //				int percLarghezza = (larghezza * 10) /100;
                    #endregion

                    #region posizionamento con valori in Pixel
                    // posizionamento in Pixel
                    posX = 0;
                    posY = 0;

                    // verifica dimensioni coerenti
                    DocsPaVO.documento.position default_pos = null;

                    for (int i = 0; i < file.LabelPdf.positions.Count; i++)
                    {
                        default_pos = (DocsPaVO.documento.position)file.LabelPdf.positions[i];

                        if (default_pos.posName == file.LabelPdf.default_position)
                        {
                            posX = Convert.ToInt32(default_pos.PosX);
                            posY = Convert.ToInt32(rect.Top) - Convert.ToInt32(default_pos.PosY);

                            // verifico se la prescelta X rientra nel dim del foglio
                            if (posX >= Convert.ToInt32(file.LabelPdf.pdfWidth))
                            {
                                posX = Convert.ToInt32(file.LabelPdf.pdfWidth);
                            }
                            // verifico se la prescelta Y rientra nel dim del foglio
                            if (posY <= 0)
                            {
                                posY = Convert.ToInt32(file.LabelPdf.font_size);
                            }
                            break;
                        }

                    }

                    //////fs = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.OpenOrCreate);
                    //////iTextSharp.text.pdf.PdfStamper stamp = new PdfStamper(prd, fs);

                    //////stamp.Writer.Open();

                    PdfContentByte cb = null;

                    // Gabriele Melini 07-01-2015
                    // INC000000507999
                    // Visualizzazione segnatura su documenti firmati PADES/CADES
                    // CODICE COMMENTATO
                    //if (!isPades)
                    //    cb = stamp.GetOverContent(pair.Key);
                    //else
                    //    cb = new iTextSharp.text.pdf.PdfContentByte(stamp.Writer);
                    cb = stamp.GetOverContent(pair.Key);

                    iTextSharp.text.Color colore = castFontColorPreferences(ref file);
                    //colore
                    cb.SetColorFill(colore);
                    bool showSignatureAsAnnotation = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "VIEW_SIGNATURE_AS_ANNOTATION")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "VIEW_SIGNATURE_AS_ANNOTATION").Equals("1");
                    if (!isPades || !showSignatureAsAnnotation)//dimensione Font solo se non pades 
                    {
                        cb.BeginText();
                        //font
                        cb.SetFontAndSize(bf, Convert.ToInt32(file.LabelPdf.font_size));


                        #region prova barcode
                        //				iTextSharp.text.pdf.Barcode39 br39=new Barcode39();
                        //				br39.TextAlignment=1;
                        //			byte[] br=iTextSharp.text.pdf.Barcode39.GetBarsCode39(sch.docNumber);
                        //					br39.Font=bf;
                        //				cb.AddImage(br39.CreateImageWithBarcode(cb,iTextSharp.text.Color.RED,iTextSharp.text.Color.BLACK),true);
                        //				//cb.ShowTextAligned(1,br39.Code,120,820,0);
                        #endregion
                        //dimensioni pagina
                        iTextSharp.text.Rectangle size = cb.PdfDocument.PageSize;

                        cb.SetTextMatrix(posX, posY);

                        //Come già fatto per la posizione gestire la rotazione in base all'informazione passata dal FrontEnd!
                        rTimbro = pair.Value;

                        writeLabelOnNonSignedPDF(file, ref posX, ref posY, currAmm, rTimbro, segnaPix, default_pos, cb);
                        //cb.ShowText(sch.protocollo.segnatura);
                        cb.EndText();
                    }
                    else   //E' un pades non posso scrivere, al massimo aggungere una annotation
                    {

                        // Gabriele Melini 07-01-2015
                        // INC000000507999
                        // Visualizzazione segnatura su documenti firmati PADES/CADES
                        cb.SetFontAndSize(bf, Convert.ToInt32(file.LabelPdf.font_size));
                        rTimbro = pair.Value;

                        int offset = 0;
                        int AppoX = posX;
                        int AppoY = posY;
                        int k = 0;
                        float rotaz = 0;
                        string Timbro_rotazione = currAmm.Timbro_rotazione;
                        string posName = default_pos.posName;
                        int timbroLength = rTimbro.Length;
                        string font_size = file.LabelPdf.font_size;

                        calcolaRettangoloEtichetta(ref posX, ref posY, segnaPix, ref offset, AppoX, AppoY, k, ref rotaz, Timbro_rotazione, posName, timbroLength, font_size);

                        string segnatura = "";
                        int maxLen = 0;
                        int sizeX = 0;
                        string maxSegnatura = string.Empty;
                        int sizeY = 0;
                        foreach (string lineaTimbro in rTimbro)
                        {
                            if (lineaTimbro.Length > maxLen)
                            {
                                maxLen = lineaTimbro.Length;
                                maxSegnatura = lineaTimbro;
                            }
                            segnatura += lineaTimbro.Trim() + "\r";
                            sizeY += (Convert.ToInt32(file.LabelPdf.font_size) + 2);
                        }

                        maxLen *= 8;
                        sizeX = Convert.ToInt32(bf.GetWidthPoint(maxSegnatura, Convert.ToInt32(file.LabelPdf.font_size)));
                        if (posX + sizeX > Convert.ToInt32(cb.PdfDocument.PageSize.Width))
                            sizeX = Convert.ToInt32(cb.PdfDocument.PageSize.Width) - posX;

                        //iTextSharp.text.Rectangle annoRect = new iTextSharp.text.Rectangle(posX, posY, posX + 195, posY - 80);
                        iTextSharp.text.Rectangle annoRect = new iTextSharp.text.Rectangle(posX, posY, posX + sizeX, posY - sizeY);

                        PdfAnnotation annot = new PdfAnnotation(stamp.Writer, annoRect);
                        annot.Put(iTextSharp.text.pdf.PdfName.SUBTYPE, iTextSharp.text.pdf.PdfName.FREETEXT);
                        annot.BorderStyle = new iTextSharp.text.pdf.PdfBorderDictionary(0, 0);

                        annot.Put(iTextSharp.text.pdf.PdfName.CONTENTS, new iTextSharp.text.pdf.PdfString(segnatura, iTextSharp.text.pdf.PdfObject.TEXT_UNICODE));
                        annot.DefaultAppearanceString = cb;

                        annot.Rotate = pageRotation;

                        annot.Flags = iTextSharp.text.pdf.PdfAnnotation.FLAGS_PRINT | iTextSharp.text.pdf.PdfAnnotation.FLAGS_READONLY | iTextSharp.text.pdf.PdfAnnotation.FLAGS_NOZOOM | iTextSharp.text.pdf.PdfAnnotation.FLAGS_LOCKED;
                        if (labelPdf.digitalSignInfo != null)
                        {
                            if (labelPdf.digitalSignInfo.printOnFirstPage)
                                stamp.AddAnnotation(annot, 1);
                            if (labelPdf.digitalSignInfo.printOnLastPage)
                            {
                                if (prd.NumberOfPages.Equals(1))
                                    stamp.AddAnnotation(annot, 1);
                                else
                                {
                                    stamp.AddAnnotation(annot, pair.Key);
                                }
                            }
                        }
                        else
                        {
                            stamp.AddAnnotation(annot, 1);
                        }
                    }

                    //////stamp.Close();
                    #endregion
                }

                //Rimuovo le pagine bianche generate nel PDF da itext
                /*
                 * TOLTO:SE PADES ANDAVA IN ERRORE
                ArrayList pageToKeap = new ArrayList();
                for (int i = 1; i <= prd.NumberOfPages; i++)
                {
                    if (!IsBlankPdfPage(prd, i))
                        pageToKeap.Add(i);
                }
                if (pageToKeap.Count > 0)
                    prd.SelectPages(pageToKeap);
                */

                prd.Close();
                stamp.Close();


                fileStream = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.Open);
                int fileLength = (int)fileStream.Length;
                rtn = new byte[fileLength];
                fileStream.Read(rtn, 0, fileLength);
                fileStream.Close();
                file.length = fileLength;
                file.fullName = fileName;
                file.name = fileName;
                file.estensioneFile = "PDF";
                file.path = fullPath;
                file.content = rtn;
                return file;
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(fullPathFileName))
                {
                    System.IO.File.Delete(fullPathFileName);
                }
                logger.Debug(ex);
                throw ex;
            }
            finally
            {
                if (System.IO.File.Exists(fullPathFileName))
                {

                    System.IO.File.Delete(fullPathFileName);
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }

                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }



        private static DocsPaVO.documento.FileDocumento addVoidEtic(ref DocsPaVO.documento.FileDocumento file, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente utente, DocsPaVO.documento.labelPdf labelPdf)
        {
            int posX = 0;
            int posY = 0;
            PdfInfo info = getPdfInfo(file);
            bool isPades = info.IsSigned;

            // se è pdfa ed è pure firmato non posso fare append perchè la conformità pdfa sarebbe corrotta
            if (info.IsSigned && info.IsPdfA)
                isPades = false;

            // se sono presenti i dati biometrici ritorno il fileinfo senza modificare il file
            if (info.HasBiometricData)
            {
                file.LabelPdf = labelPdf;
                file.LabelPdf.default_position = "";
                return file;
            }

            //isPades = false;
            //^^^^^^^^^^^^^^ Interruttore , commentare per attivare lo stamping su firma

            byte[] rtn = null;
            //verranno scritti i file temporanei in un subfolder di REPORTS_PATH
            string basePathFiles = System.Configuration.ConfigurationManager.AppSettings["REPORTS_PATH"];
            //string fullPath = basePathFiles + @"\EtichettaPdf";
            string fullPath = basePathFiles.Replace("%DATA", "EtichettaPdf");
            //standard naming per il file temporaneo
            string fileName = @"\" + sch.docNumber + "_" + utente.userId + ".pdf";
            //combinazione path & filename
            string fullPathFileName = fullPath + fileName;
            System.IO.FileStream fileStream = null;
            System.IO.FileStream fs = null;
            //estraggo la posizione dall'oggetto Label per mantenere inalterato il funzionamento del metodo
            string position = null;

            if (labelPdf != null)
            {
                position = labelPdf.position;
                //aggiungo il carattere ed il colore selezionati all'oggetto label del fileDoc
                if (!string.IsNullOrEmpty(labelPdf.sel_font))
                {
                    file.LabelPdf.sel_font = labelPdf.sel_font;
                }
                if (!string.IsNullOrEmpty(labelPdf.sel_color))
                {
                    file.LabelPdf.sel_color = labelPdf.sel_color;
                }
            }
            DocsPaVO.amministrazione.InfoAmministrazione currAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione);

            try
            {
                //verifico esistenza directory
                if (!System.IO.Directory.Exists(fullPath))
                {
                    System.IO.Directory.CreateDirectory(fullPath);
                }
                //verifico esistenza file
                if (System.IO.File.Exists(fullPathFileName))
                {
                    System.IO.File.Delete(fullPathFileName);
                }

                //verifico la posizione prescelta
                if ((position == null) || (position == ""))
                {
                    //caricamento preferenze di default di LabelPDF
                    loadXmlLabelProperties(file, null, currAmm);
                }
                else
                {
                    //caricamento preferenze utente per LabelPDF
                    loadXmlLabelProperties(file, position, currAmm);
                }

                if (file.content == null)
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    FileStream newStream = new FileStream(fullPathFileName, FileMode.Create);
                    PdfWriter.GetInstance(document, newStream);
                    document.Open();
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    document.Close();

                    newStream = new FileStream(fullPathFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    file.content = new byte[newStream.Length];
                    newStream.Read(file.content, 0, file.content.Length);
                    newStream.Flush();
                    newStream.Close();
                    File.Delete(fullPathFileName);
                }

                System.IO.MemoryStream ms = new System.IO.MemoryStream(file.content, true);

                //Stringa del timbro o della segnatura
                int maxT = 0;
                string maxTimbro = "";
                string escape = "\n";
                //In base all'informazione nel frontEnd decidere se devo caricare la segnatura oppure il Timbro!!!
                //...mettendo a true l'ultimo parametro carico il timbro!!!
                string dati = GetDatiEtichetta(utente, currAmm, labelPdf, sch, file.signatureResult);
                //rimuovo l'ultimo separatore!!!
                dati = dati.TrimEnd(Convert.ToChar(escape));
                string[] rTimbro = dati.Split(Convert.ToChar(escape));
                for (int t = 0; t < rTimbro.Length; t++)
                {
                    if (rTimbro[t].Length > maxT)
                    {
                        maxTimbro = rTimbro[t];
                        maxT = rTimbro[t].Length;
                    }
                    //Rimuovo gli spazi bianchi dai dati del timbro che in caso di orientamento
                    //verticale creano un disallineamento!
                    rTimbro[t] = rTimbro[t].Trim();
                }
                //se dal frontEnd ho valorizzato la rotazione allora la sostituisco a quella configurata
                if (labelPdf.label_rotation != String.Empty)
                {
                    currAmm.Timbro_rotazione = labelPdf.label_rotation;
                }
                //valorizzo i dati da passare al frontEnd
                file.LabelPdf.label_rotation = currAmm.Timbro_rotazione;
                file.LabelPdf.orientamento = labelPdf.orientamento;
                file.LabelPdf.tipoLabel = labelPdf.tipoLabel;

                // posizione 0 del memory stream
                ms.Position = 0;
                iTextSharp.text.pdf.PdfReader prd = new PdfReader(ms);

                // dimensioni della prima pagina del documento
                iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(prd.GetPageSize(1));

                //font di stampa
                BaseFont bf = castFontTypePreferences(ref file);

                // verifico le dimensioni massime consentite per la sovrastampa				

                // calcola l'occupazione in pixel della segnatura
                int segnaPix = Convert.ToInt32(bf.GetWidthPoint(maxTimbro, Convert.ToInt32(file.LabelPdf.font_size)));

                //viene allineata la dimensione massima utilizzabile per la sovrastampa
                // sottraendo la massima occupazione della segnatura con un sfrido di 5 px
                int maxWidth = (Convert.ToInt32(rect.Width) - segnaPix) - 5;

                //inserisco le info della pagina PDF sul VO
                file.LabelPdf.pdfHeight = rect.Height.ToString();
                file.LabelPdf.pdfWidth = Convert.ToString(maxWidth);


                #region posizionamento con valori in Pixel
                // posizionamento in Pixel
                posX = 0;
                posY = 0;

                // verifica dimensioni coerenti
                DocsPaVO.documento.position default_pos = null;

                for (int i = 0; i < file.LabelPdf.positions.Count; i++)
                {
                    default_pos = (DocsPaVO.documento.position)file.LabelPdf.positions[i];

                    if (default_pos.posName == file.LabelPdf.default_position)
                    {
                        posX = Convert.ToInt32(default_pos.PosX);
                        posY = Convert.ToInt32(rect.Top) - Convert.ToInt32(default_pos.PosY);

                        // verifico se la prescelta X rientra nel dim del foglio
                        if (posX >= Convert.ToInt32(file.LabelPdf.pdfWidth))
                        {
                            posX = Convert.ToInt32(file.LabelPdf.pdfWidth);
                        }
                        // verifico se la prescelta Y rientra nel dim del foglio
                        if (posY <= 0)
                        {
                            posY = Convert.ToInt32(file.LabelPdf.font_size);
                        }
                        break;
                    }

                }

                fs = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.OpenOrCreate);
                iTextSharp.text.pdf.PdfStamper stamp = new PdfStamper(prd, fs);

                stamp.Writer.Open();
                PdfContentByte cb = null;

                if (!isPades)
                    cb = stamp.GetOverContent(1);
                else
                    cb = new iTextSharp.text.pdf.PdfContentByte(stamp.Writer);

                iTextSharp.text.Color colore = castFontColorPreferences(ref file);
                //colore
                cb.SetColorFill(colore);

                if (!isPades)
                {
                    cb.BeginText();
                    //font
                    cb.SetFontAndSize(bf, Convert.ToInt32(file.LabelPdf.font_size));


                    //dimensioni pagina
                    iTextSharp.text.Rectangle size = cb.PdfDocument.PageSize;

                    cb.SetTextMatrix(posX, posY);

                    //Come già fatto per la posizione gestire la rotazione in base all'informazione passata dal FrontEnd!
                    writeLabelOnNonSignedPDF(file, ref posX, ref posY, currAmm, rTimbro, segnaPix, default_pos, cb);

                    //cb.ShowText(sch.protocollo.segnatura);
                    cb.EndText();
                }
                else
                {
                    int offset = 0;
                    int AppoX = posX;
                    int AppoY = posY;
                    int k = 0;
                    float rotaz = 0;
                    string Timbro_rotazione = currAmm.Timbro_rotazione;
                    string posName = default_pos.posName;
                    int timbroLength = rTimbro.Length;
                    string font_size = file.LabelPdf.font_size;

                    calcolaRettangoloEtichetta(ref posX, ref posY, segnaPix, ref offset, AppoX, AppoY, k, ref rotaz, Timbro_rotazione, posName, timbroLength, font_size);

                    string segnatura = "";
                    int maxLen = 0;
                    foreach (string lineaTimbro in rTimbro)
                    {
                        if (lineaTimbro.Length > maxLen)
                            maxLen = lineaTimbro.Length;
                        segnatura += lineaTimbro + "\r";
                    }

                    maxLen *= 8;

                    iTextSharp.text.Rectangle annoRect = new iTextSharp.text.Rectangle(posX, posY, posX + 195, posY - 80);

                    PdfAnnotation annot = new PdfAnnotation(stamp.Writer, annoRect);
                    annot.Put(iTextSharp.text.pdf.PdfName.SUBTYPE, iTextSharp.text.pdf.PdfName.FREETEXT);
                    annot.BorderStyle = new iTextSharp.text.pdf.PdfBorderDictionary(0, 0);

                    annot.Put(iTextSharp.text.pdf.PdfName.CONTENTS, new iTextSharp.text.pdf.PdfString(segnatura, iTextSharp.text.pdf.PdfObject.TEXT_UNICODE));
                    annot.DefaultAppearanceString = cb;
                    annot.Flags = iTextSharp.text.pdf.PdfAnnotation.FLAGS_PRINT | iTextSharp.text.pdf.PdfAnnotation.FLAGS_READONLY | iTextSharp.text.pdf.PdfAnnotation.FLAGS_NOZOOM | iTextSharp.text.pdf.PdfAnnotation.FLAGS_LOCKED;
                    stamp.AddAnnotation(annot, 1);
                }
                stamp.Close();
                #endregion

                fileStream = new System.IO.FileStream(fullPathFileName, System.IO.FileMode.Open);
                int fileLength = (int)fileStream.Length;
                rtn = new byte[fileLength];
                fileStream.Read(rtn, 0, fileLength);
                fileStream.Flush();
                fileStream.Close();
                file.length = fileLength;
                file.fullName = fileName;
                file.name = fileName;
                file.estensioneFile = "PDF";
                file.path = fullPath;
                file.content = rtn;
                file.contentType = "application/pdf";
                return file;
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(fullPathFileName))
                {
                    System.IO.File.Delete(fullPathFileName);
                }
                logger.Debug(ex);
                throw ex;
            }
            finally
            {
                if (System.IO.File.Exists(fullPathFileName))
                {

                    System.IO.File.Delete(fullPathFileName);
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }

                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }


        private static void writeLabelOnNonSignedPDF(DocsPaVO.documento.FileDocumento file, ref int posX, ref int posY, DocsPaVO.amministrazione.InfoAmministrazione currAmm, string[] rTimbro, int segnaPix, DocsPaVO.documento.position default_pos, PdfContentByte cb)
        {
            int offset = 0;
            int AppoX = posX;
            int AppoY = posY;

            for (int k = 0; k < rTimbro.Length; k++)
            {
                float rotaz = 0;
                string Timbro_rotazione = currAmm.Timbro_rotazione;
                string posName = default_pos.posName;
                int timbroLength = rTimbro.Length;
                string font_size = file.LabelPdf.font_size;

                calcolaRettangoloEtichetta(ref posX, ref posY, segnaPix, ref offset, AppoX, AppoY, k, ref rotaz, Timbro_rotazione, posName, timbroLength, font_size);

                cb.ShowTextAligned(0, rTimbro[k], posX, posY, rotaz);
                //lo valorizzo dopo il primo timbro perchè la prima riga deve partire dalla posizione zero!!!
                offset = (Convert.ToInt32(file.LabelPdf.font_size) + 2);
            }
        }

        

        private static void calcolaRettangoloEtichetta(ref int posX, ref int posY, int segnaPix, ref int offset, int AppoX, int AppoY, int k, ref float rotaz, string Timbro_rotazione, string posName, int timbroLength, string font_size)
        {
            if (Timbro_rotazione != String.Empty)
            {
                rotaz = (float)System.Convert.ToInt16(Timbro_rotazione);
                if (posName == "pos_upSx")
                {
                    switch (Timbro_rotazione)
                    {
                        case "0":
                            posX = posX;
                            posY = posY - offset;
                            break;
                        case "90":
                            posX = posX + offset;
                            posY = AppoY - segnaPix;
                            break;
                        case "180":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = AppoX + segnaPix;
                            posY = AppoY - (offset * (timbroLength - (k + 1)));
                            break;
                        case "270":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = AppoX + (offset * (timbroLength - (k + 1)));
                            posY = posY;
                            break;
                        default:
                            posX = posX;
                            posY = posY - offset;
                            break;
                    }
                }
                if (posName == "pos_upDx")
                {
                    switch (Timbro_rotazione)
                    {
                        case "0":
                            posX = posX;
                            posY = posY - offset;
                            break;
                        case "90":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = AppoX + segnaPix - (offset * (timbroLength - (k + 1)));
                            posY = AppoY - segnaPix;
                            break;
                        case "180":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = AppoX + segnaPix;
                            posY = AppoY - (offset * (timbroLength - (k + 1)));
                            break;
                        case "270":
                            posX = AppoX + segnaPix - (offset * k);
                            posY = posY;
                            break;
                        default:
                            posX = posX;
                            posY = posY - offset;
                            break;
                    }
                }
                if (posName == "pos_downSx")
                {
                    switch (Timbro_rotazione)
                    {
                        case "0":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = posX;
                            posY = AppoY + (offset * (timbroLength - (k + 1)));
                            break;
                        case "90":
                            posX = posX + offset;
                            posY = posY;
                            break;
                        case "180":
                            posX = AppoX + segnaPix;
                            posY = posY + offset;
                            break;
                        case "270":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = AppoX + (offset * (timbroLength - (k + 1)));
                            posY = AppoY + segnaPix;
                            break;
                        default:
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = posX;
                            posY = AppoY + (offset * (timbroLength - (k + 1)));
                            break;
                    }
                }
                if (posName == "pos_downDx")
                {
                    switch (Timbro_rotazione)
                    {
                        case "0":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = posX;
                            posY = AppoY + (offset * (timbroLength - (k + 1)));
                            break;
                        case "90":
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = AppoX + segnaPix - (offset * (timbroLength - (k + 1)));
                            posY = posY;
                            break;
                        case "180":
                            posX = AppoX + segnaPix;
                            posY = posY + offset;
                            break;
                        case "270":
                            posX = AppoX + segnaPix - (offset * k);
                            posY = AppoY + segnaPix;
                            break;
                        default:
                            offset = (Convert.ToInt32(font_size) + 2);
                            posX = posX;
                            posY = AppoY + (offset * (timbroLength - (k + 1)));
                            break;
                    }
                }
                //perchè anche in caso di posizione personale devo sempre andare a capo di una riga
                //se il timbro fosse con orientamento verticale!!!
                if (posName == "pos_pers")
                {
                    posX = posX;
                    posY = posY - offset;
                }
            }
            else
            {
                if (posName == "pos_downDx" || posName == "pos_downSx")
                {
                    offset = (Convert.ToInt32(font_size) + 2);
                    posY = AppoY + (offset * (timbroLength - (k + 1)));
                }
                else
                {
                    posY = posY - offset;
                }
            }
        }


        private static string GetDatiEtichetta(
                        DocsPaVO.utente.InfoUtente utente,
                        DocsPaVO.amministrazione.InfoAmministrazione Amm,
                        DocsPaVO.documento.labelPdf labelInfo,
                        DocsPaVO.documento.SchedaDocumento sch,
                        DocsPaVO.documento.VerifySignatureResult signatureResult,
                        string version_label_allegato = ""
                            )
        {
            string retValue = string.Empty;

            //separatore fra un valore e l'altro del timbro
            string separatore = " ";
            //valore di fine riga per determinare se andare a capo oppure no
            string escape = String.Empty;
            //valore che verrà restituito come output alla richiesta di dati da stampare su pdf!!!
            string Timbro = String.Empty;
            //oggetto all'interno del quale leggere tutti i dati del timbro!!!
            DocsPaVO.amministrazione.InfoAmministrazione currAmm = Amm;
            //parametri per recuperare la/le classifica
            string profile = sch.systemId;
            string people = utente.idPeople;
            string gruppo = utente.idGruppo;
            //è l'ultimo valore sostituito nella fase di creazione del timbro
            string[] lastVal = { "COD_AMM", "COD_REG", "NUM_PROTO", "DATA_COMP", "ORA", "NUM_ALLEG", "CLASSIFICA", "IN_OUT", "COD_UO_PROT", "COD_UO_VIS", "COD_RF_PROT", "COD_RF_VIS" };

            //se da amministrazione ho selezionato il timbro devo abilitare la stampa del medesimo!!!
            //leggendo l'informazione direttamente da amministrazione, solo nel caso in cui non venga
            //modificata la selezione da front-end
            //if (orientamento == String.Empty || orientamento == null)
            //{
            //    if (currAmm.Timbro_orientamento != null && currAmm.Timbro_orientamento != string.Empty && currAmm.Timbro_orientamento != "false")
            //    {
            //        isTimbro = true;
            //    }
            //}
            if (labelInfo.tipoLabel)
            {

                //orientamento del timbro non applicabile alla segnatura
                if (labelInfo.orientamento == String.Empty || labelInfo.orientamento == null)
                {
                    labelInfo.orientamento = currAmm.Timbro_orientamento;
                }
                //nel caso che anche il valore letto in amministrazione sia null di default stampo il timbro in verticale
                if (labelInfo.orientamento.ToLower() == "orizzontale")
                {
                    escape = "";
                }
                else
                {
                    escape = "\n";
                }

                string TimbroIniziale = currAmm.Timbro_pdf;
                string datiTimbro = currAmm.Timbro_pdf;
                string sep = DocsPaDB.Utils.Personalization.getInstance(currAmm.IDAmm).getSeparator();


                //nome amministrazione
                if (datiTimbro.Contains("COD_AMM"))
                {
                    //string codAmm = DocsPaDB.Utils.Personalization.getInstance(sch.registro.idAmministrazione).getCodiceAmministrazione();
                    string codAmm = DocsPaDB.Utils.Personalization.getInstance(currAmm.IDAmm).getCodiceAmministrazione();
                    if (codAmm != String.Empty)
                    {
                        datiTimbro = datiTimbro.Replace("COD_AMM", (codAmm + escape));
                        lastVal[0] = codAmm + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_AMM", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("COD_AMM") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("COD_AMM", escape);
                        datiTimbro = datiTimbro.Replace("COD_AMM", "");
                    }
                }

                //questo è il codice AOO ed è valorizzato solo sui documenti protocollati
                if (datiTimbro.Contains("COD_REG"))
                {

                    // se si tratta di un allegato ad un protocollo lo recupero dal COD_REG del protocollo
                    if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.codRegistro != null))
                    {
                        lastVal[1] = sch.documentoPrincipale.codRegistro + escape;
                        datiTimbro = datiTimbro.Replace("COD_REG", (sch.documentoPrincipale.codRegistro + escape));
                    }
                    //bisogna effettuare un controllo sul fatto che sia valorizzato oppure no!
                    else if (sch.registro != null)
                    {
                        lastVal[1] = sch.registro.codRegistro + escape;
                        datiTimbro = datiTimbro.Replace("COD_REG", (sch.registro.codRegistro + escape));
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_REG", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("COD_REG") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("COD_REG", escape);
                        datiTimbro = datiTimbro.Replace("COD_REG", "");
                    }
                }

                //numero protocollo
                if (datiTimbro.Contains("NUM_PROTO"))
                {
                    //Normalizzo il numero di protocollo secondo lo standard a 7 cifre
                    int MAX_LENGTH = 7;
                    string zeroes = "";
                    string numProto = "";

                    // se si tratta di un allegato ad un protocollo lo recupero dal NUM_PROTO del protocollo
                    if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.numProt != null))
                    {
                        numProto = sch.documentoPrincipale.numProt;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;

                        datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
                        lastVal[2] = numProto + escape;

                    }
                    // se si tratta di un allegato ad un grigio lo recupero dal docnumber del documento principale 
                    else if ((sch.documentoPrincipale != null) && (sch.documentoPrincipale.numProt == null) && sch.documentoPrincipale.docNumber != null)
                    {
                        numProto = sch.documentoPrincipale.docNumber;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;

                        datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
                        lastVal[2] = numProto + escape;

                    }
                    else if (sch.protocollo != null)
                    {
                        numProto = sch.protocollo.numero != null ? sch.protocollo.numero : string.Empty;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;

                        //***********
                        datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
                        lastVal[2] = numProto + escape;
                    }
                    // Per far funzionare il timbro anche con i documenti grigi
                    else if ((sch.documentoPrincipale == null) && (sch.tipoProto == "G"))
                    {
                        numProto = sch.docNumber;
                        for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                        {
                            zeroes = zeroes + "0";
                        }
                        numProto = zeroes + numProto;
                        datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
                        lastVal[2] = "ID: " + numProto + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "NUM_PROTO", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("NUM_PROTO") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("NUM_PROTO", escape);
                        datiTimbro = datiTimbro.Replace("NUM_PROTO", "");
                    }
                }

                //data protocollazione
                if (datiTimbro.Contains("DATA_COMP"))
                {
                    // se si tratta di un allegato ad un protocollo lo recupero dal DATA_COMP del protocollo
                    if (sch.documentoPrincipale != null)
                    {
                        //datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));
                        //protocollo.dataProtocollazione + escape));

                        //PALUMBO: nel caso di documento Protocollato il metodo GetProtoData valorizza la dataApertura con la DataProtocollo
                        if (sch.documentoPrincipale.dataApertura != null)
                            datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.documentoPrincipale.dataApertura + escape));
                        else
                            datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));
                        lastVal[3] = sch.dataCreazione + escape;

                    }
                    // Per far funzionare il timbro anche con i documenti grigi
                    else if ((sch.documentoPrincipale == null) && (sch.tipoProto == "G"))
                    {
                        datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.dataCreazione + escape));

                        lastVal[3] = sch.dataCreazione + escape;
                    }

                    else if (sch.protocollo != null)
                    {
                        datiTimbro = datiTimbro.Replace("DATA_COMP", (sch.protocollo.dataProtocollazione + escape));
                        lastVal[3] = sch.protocollo.dataProtocollazione + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "DATA_COMP", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("DATA_COMP") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("DATA_COMP", escape);
                        datiTimbro = datiTimbro.Replace("DATA_COMP", "");
                    }
                }

                //ora di protocollazione
                if (datiTimbro.Contains("ORA"))
                {
                    //aggiunta dell'ora di protocollazione nel timbro
                    string ora = sch.oraCreazione;
                    if ((ora != null) && (ora != ""))
                    {
                        //se l'ora è nel formato comprensivo dei secondi devo rimuovere i secondi prima di inserire l'ora!
                        if (ora.Length > 5)
                        {
                            ora = ora.Remove((ora.Length - 3), 3);
                        }
                        datiTimbro = datiTimbro.Replace("ORA", (ora + escape));
                        lastVal[4] = ora + escape;
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "ORA", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("ORA") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("ORA", escape);
                        datiTimbro = datiTimbro.Replace("ORA", "");
                    }
                }

                //numero allegati
                if (datiTimbro.Contains("NUM_ALLEG"))
                {
                    datiTimbro = datiTimbro.Replace("NUM_ALLEG", (sch.allegati.Count + escape));
                    //datiTimbro = datiTimbro.Replace("NUM_ALLEG", "");
                    lastVal[5] = System.Convert.ToString(sch.allegati.Count) + escape;
                }

                //classificazione o classificazioni del corrente documento protocollato
                if (datiTimbro.Contains("CLASSIFICA"))
                {
                    string padding = string.Empty;
                    ArrayList classifica = new ArrayList();
                    ArrayList folders = new ArrayList();
                    // Bisogna mettere il controllo su classifica perchè potrebbe non essere ancora assegnata!!!
                    // controllo prima se sono su un allegato ad un protocollo
                    string idprofile = sch.documentoPrincipale != null ? sch.documentoPrincipale.docNumber : profile;
                    classifica = Fascicoli.FascicoloManager.getFascicoliDaDoc(utente, idprofile);

                    string key_beprojectlevel = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_LEVEL");
                    if (!string.IsNullOrEmpty(key_beprojectlevel) && key_beprojectlevel.Equals("1"))
                        folders = Fascicoli.FolderManager.GetFoldersDocument(sch.docNumber);

                    if (classifica != null)
                    {
                        for (int i = 0; i < classifica.Count; i++)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fascicolo = (DocsPaVO.fascicolazione.Fascicolo)classifica[i];
                            if (fascicolo.codice != String.Empty)
                            {
                                string temp = string.Empty;
                                var folder = (from DocsPaVO.fascicolazione.Folder f in folders
                                              where f.idFascicolo == fascicolo.systemID
                                              select f).FirstOrDefault();

                                if (folder != null)
                                {
                                    for (int j = 1; j < folder.codicelivello.Length/4; j++)
                                    {
                                        string val = folder.codicelivello.Substring(j * 4, 4);
                                        temp += string.Format(".{0}",Convert.ToInt32(val));
                                    }
                                    
                                    // Questo approccio genera un errore ma al momento 
                                    // non mi viene in mente niente di più semplice
                                    Timbro += padding + separatore + (i == 0 ? "[" : "; ") + 
                                        GetCodiceFascicolo(currAmm.Fascicolatura, fascicolo.codice, temp) + escape;
                                }
                                else if (!string.IsNullOrEmpty(key_beprojectlevel) && key_beprojectlevel.Equals("1"))
                                {
                                    Timbro += padding + separatore + (i == 0 ? "[" : "; ") + fascicolo.codice + escape;
                                    if (escape != "")
                                        padding = ReturnDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                                }
                                else
                                {
                                    Timbro = Timbro + padding + separatore + (i == 0 ? "[" : "; ") + fascicolo.codice + escape;
                                    //Nel caso di orientamento verticale replico il separatore letto in amministrazione
                                    //altrimenti uso il seguente separatore
                                    padding = " -";
                                    if (escape != "")
                                        padding = ReturnDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                                }
                            }
                        }
                        if (Timbro == string.Empty)
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                            //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                            //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("CLASSIFICA") - 1), 1);
                            //datiTimbro = datiTimbro.Replace("CLASSIFICA", escape);
                            datiTimbro = datiTimbro.Replace("CLASSIFICA", "");
                        }
                        else
                        {
                            Timbro += "]";
                            datiTimbro = datiTimbro.Replace("CLASSIFICA", Timbro);
                            lastVal[6] = Timbro;
                        }
                    }
                }

                //Tipo di protocollo
                if (datiTimbro.Contains("IN_OUT"))
                {
                    string arrPart = "";
                    if (sch.protocollatore != null)
                    {
                        if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
                        {
                            arrPart = "A";
                        }
                        else if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                        {
                            arrPart = "P";
                        }
                        else if (sch.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
                        {
                            arrPart = "I";
                        }
                    }
                    if (arrPart == string.Empty)
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "IN_OUT", lastVal);
                        //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                        //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("IN_OUT") - 1), 1);
                        //datiTimbro = datiTimbro.Replace("IN_OUT", escape);
                        datiTimbro = datiTimbro.Replace("IN_OUT", "");
                    }
                    else
                    {
                        datiTimbro = datiTimbro.Replace("IN_OUT", arrPart + escape);
                        lastVal[7] = arrPart + escape;
                    }
                }


                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (datiTimbro.Contains("COD_UO_PROT"))
                {
                    if (sch.protocollatore != null)
                    {
                        //string uo = getCodiceUO(sch.creatoreDocumento.idCorrGlob_Ruolo);

                        //string uo = getCodiceUO(sch.protocollatore.ruolo_idCorrGlobali);
                        //ABBATANGELI - Necessario per creare il timbro correttamente anche se il ruolo è stato storicizzato
                        string uo = getCodiceUOEnabledAndDisabled(sch.protocollatore.ruolo_idCorrGlobali);
                        //uo = sch.protocollatore.uo_codiceCorrGlobali;
                        if (!string.IsNullOrEmpty(uo))
                        {
                            datiTimbro = datiTimbro.Replace("COD_UO_PROT", (uo + escape));
                            lastVal[8] = uo + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
                    }
                }

                if (datiTimbro.Contains("COD_UO_VIS"))
                {
                    if (utente.idCorrGlobali != string.Empty)
                    {
                        string uo = getCodiceUO(utente.idCorrGlobali);
                        if (!string.IsNullOrEmpty(uo))
                        {
                            datiTimbro = datiTimbro.Replace("COD_UO_VIS", (uo + escape));
                            lastVal[9] = uo + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
                    }
                }

                if (datiTimbro.Contains("COD_RF_PROT"))
                {
                    if (sch.protocollatore != null)
                    {
                        string rf = getCodiceRF(sch.protocollatore.ruolo_idCorrGlobali);
                        if (!string.IsNullOrEmpty(rf))
                        {
                            datiTimbro = datiTimbro.Replace("COD_RF_PROT", (rf + escape));
                            lastVal[10] = rf + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
                    }
                }

                if (datiTimbro.Contains("COD_RF_VIS"))
                {
                    if (utente.idCorrGlobali != string.Empty)
                    {
                        string rf = getCodiceRF(utente.idCorrGlobali);
                        if (!string.IsNullOrEmpty(rf))
                        {
                            datiTimbro = datiTimbro.Replace("COD_RF_VIS", (rf + escape));
                            lastVal[11] = rf + escape;
                        }
                        else
                        {
                            datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
                            datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
                        }
                    }
                    else
                    {
                        datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
                        datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
                    }
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


                Timbro = datiTimbro;

                //nel caso di documento annullato stampo il timbro di annullamento come segue
                if (sch.protocollo != null && sch.protocollo.protocolloAnnullato != null)
                    Timbro = separatore + sch.protocollo.segnatura + "\n" + separatore + "Annullato il: " + sch.protocollo.protocolloAnnullato.dataAnnullamento + "\n" + separatore + "Motivo: " + sch.protocollo.protocolloAnnullato.autorizzazione;

                retValue = string.IsNullOrEmpty(version_label_allegato) ? Timbro : Timbro + " - " + version_label_allegato;
                //retValue = Timbro;
            }
            else
            {

                //nel caso di documento annullato stampo il timbro di annullamento come segue
                if (sch.protocollo != null && sch.protocollo.protocolloAnnullato != null)
                {
                    Timbro = sch.protocollo.segnatura + "\n" + "Annullato il: " + sch.protocollo.protocolloAnnullato.dataAnnullamento + "\n" + "Motivo: " + sch.protocollo.protocolloAnnullato.autorizzazione;
                    retValue = Timbro;
                }
                else
                {
                    //protocolli
                    if (sch != null && (sch.protocollo != null
                     && sch.protocollo.segnatura != null
                     && sch.protocollo.segnatura != ""))
                    {
                        //segnatura in alternativa al timbro ed all'annullamento!!!
                        retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.protocollo.segnatura : sch.protocollo.segnatura + " - " + version_label_allegato;

                        //se siamo in CC aggiungo alla segnatura il protocollo arma(per carabinieri) in stampa A4 segnatura
                        string protoArma = string.Empty;
                        string livTitolario = string.Empty;
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"]))
                        {
                            protoArma = System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"];
                            livTitolario = System.Configuration.ConfigurationManager.AppSettings["ENABLE_LIVELLI_TITOLARIO"];
                            ArrayList fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(utente, profile);

                            string classifica = string.Empty;
                            string sep = ":";
                            string space = " ";
                            //classificazione
                            for (int i = 0; i < fascicolo.Count; i++)
                            {
                                DocsPaVO.fascicolazione.Fascicolo fasc = (DocsPaVO.fascicolazione.Fascicolo)fascicolo[i];
                                if (fasc.codice != String.Empty)
                                {
                                    DocsPaVO.fascicolazione.Classifica[] classif = BusinessLogic.Fascicoli.TitolarioManager.getGerarchia(fasc.idClassificazione, Amm.IDAmm);
                                    classifica = classif[classif.Length - 1].codice;
                                }
                            }

                            retValue += "\n" + livTitolario + sep + classifica + space + protoArma + sep + sch.protocolloTitolario;
                        }
                        //end check carabinieri
                    }
                    //predisposti o grigi
                    else if (sch != null && (sch.protocollo != null
                     && string.IsNullOrEmpty(sch.protocollo.segnatura)
                     && !string.IsNullOrEmpty(sch.systemId))

                        || (sch.protocollo == null
                            && !string.IsNullOrEmpty(sch.systemId)))

                        if (sch.documentoPrincipale != null)
                        {
                            if (sch.documentoPrincipale.segnatura != null)
                                retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.documentoPrincipale.segnatura : sch.documentoPrincipale.segnatura + " - " + version_label_allegato;
                            //retValue = sch.documentoPrincipale.segnatura;
                            else
                                retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString() : sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString() + " - " + version_label_allegato;
                            //retValue = sch.documentoPrincipale.docNumber + "  " + sch.dataCreazione.ToString();
                        }

                        else
                            retValue = string.IsNullOrEmpty(version_label_allegato) ? sch.docNumber + "  " + sch.dataCreazione.ToString() : sch.docNumber + "  " + sch.dataCreazione.ToString() + " - " + version_label_allegato;
                    //retValue = sch.docNumber + "  " + sch.dataCreazione.ToString();

                    //&& supportedType
                    //&& Session["allegato"] == null
                    //)
                    ///

                }
            }

            return retValue;
        }

        private static string GetCodiceFascicolo(string fascicolatura, string codice, string temp)
        {
            int startfasc = fascicolatura.IndexOf("NUM_PROG");
            startfasc = (startfasc == 0 ? 0 : startfasc - 1);
            char cstartfasc = startfasc == 0 ? char.MinValue : fascicolatura[startfasc];

            int index = 0, rep = 0;
            while (index <= startfasc)
            {
                if (fascicolatura[index] == fascicolatura[startfasc])
                    rep++;

                index++;
            }

            int endfasc = fascicolatura.IndexOf("NUM_PROG") + "NUM_PROG".Length;
            endfasc = endfasc >= fascicolatura.Length ? fascicolatura.Length - 1 : endfasc;
            char cendfasc = endfasc == fascicolatura.Length - 1 ? char.MinValue : fascicolatura[endfasc];

            int startcod = 0;
            if (cstartfasc != char.MinValue)
            {
                if (cendfasc == char.MinValue)
                    startcod = codice.LastIndexOf(cstartfasc);
                else
                {
                    for (int i = 0; i < rep; i++)
                    {
                        startcod = codice.IndexOf(cstartfasc, startcod + 1);
                    }
                }
            }

            int endcod = cendfasc == char.MinValue ? codice.Length : codice.IndexOf(cendfasc, startcod + 1);
            return codice.Insert(endcod, temp);
        }

        //Mev Firma1 < aggiunta funzioni gestione MEV Firma1 

        /// <summary>
        /// Determina e un documento è repertoriato 
        /// se definito per il documento
        /// </summary>
        /// <returns>ritorna una stringa rappresentante il protocollo di repertorio</returns>
        private static bool isDocRepertoriato(DocsPaVO.documento.SchedaDocumento sch, string codiceAmministrazione)
        {
            try
            {
                if (sch.template != null)
                    if (sch.template.ELENCO_OGGETTI.Count > 0)
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom objAttrib in sch.template.ELENCO_OGGETTI)
                            if (objAttrib.REPERTORIO == "1") return true;
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Determina e formatta una label contenente le info relative al protocollo di repertorio 
        /// se definito per il documento
        /// </summary>
        /// <returns>ritorna una stringa rappresentante il protocollo di repertorio</returns>
        private static string GetDatiEtichettaProtocolloRepertorio(DocsPaVO.documento.SchedaDocumento sch, string codiceAmministrazione)
        {
            string labelResult = string.Empty;
            try
            {
                //verifica se il documento ha associato un tipo documento 
                if (sch.template != null)
                {
                    // verifica se esistono attributi associati al tipo documento
                    if (sch.template.ELENCO_OGGETTI.Count > 0)
                    {
                        // verifica se almeno un attributo tipo funzione è di tipo repertoriato
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom objAttrib in sch.template.ELENCO_OGGETTI)
                            if (objAttrib.REPERTORIO == "1")
                                return string.Format("{0} - {1}", getFormattedProtocolloDiRepertorio(objAttrib, codiceAmministrazione), sch.template.DESCRIZIONE);
                        return string.Empty;
                    }
                    else return string.Empty;
                }
                else return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Formatta il contatore del protocollo repertoriato 
        /// </summary>
        /// <returns></returns>
        private static string getFormattedProtocolloDiRepertorio(DocsPaVO.ProfilazioneDinamica.OggettoCustom objAtt,
                                                                 string codiceAmministrazione)
        {
            string ProtocolloFormattedResult = string.Empty;
            if (objAtt.VALORE_DATABASE != null && objAtt.VALORE_DATABASE != "")
            {
                ProtocolloFormattedResult = objAtt.FORMATO_CONTATORE;//.Replace("|", "/").Replace("-", "/");
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("ANNO", objAtt.ANNO);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("CONTATORE", objAtt.VALORE_DATABASE);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("COD_AMM", codiceAmministrazione);
                ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("COD_UO", objAtt.CODICE_DB);
                if (!string.IsNullOrEmpty(objAtt.DATA_INSERIMENTO))
                {
                    int fine = objAtt.DATA_INSERIMENTO.LastIndexOf(".");
                    if (fine == -1) fine = objAtt.DATA_INSERIMENTO.LastIndexOf(":");
                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("gg/mm/aaaa hh:mm", objAtt.DATA_INSERIMENTO.Substring(0, fine));
                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("gg/mm/aaaa", objAtt.DATA_INSERIMENTO.Substring(0, 10));
                }

                if (!string.IsNullOrEmpty(objAtt.ID_AOO_RF) && objAtt.ID_AOO_RF != "0")
                {
                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(objAtt.ID_AOO_RF);
                    if (reg != null)
                    {
                        //ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.codRegistro);
                        //ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", reg.codRegistro);
                        if (!string.IsNullOrEmpty(reg.chaRF) && reg.chaRF == "1")
                        {
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.codRegistro);

                            if (!string.IsNullOrEmpty(reg.idAOOCollegata))
                            {
                                DocsPaVO.utente.Registro registro = new DocsPaVO.utente.Registro();
                                DocsPaDB.Query_DocsPAWS.Utenti rub = new DocsPaDB.Query_DocsPAWS.Utenti();
                                rub.GetRegistro(reg.idAOOCollegata, ref registro);
                                if (registro != null)
                                    ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", registro.codRegistro);
                            }
                        }
                        else //se contatore di AOO non ho i dati per ricavare RF perchè non mi viene passato in input. 
                        {
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("RF", reg.codRegistro);
                            ProtocolloFormattedResult = ProtocolloFormattedResult.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
            }
            // codice protocollo di repertorio
            return string.Format("{0}", ProtocolloFormattedResult);
        }
        /// <summary>
        /// Verifica se il documento è un allegato
        /// </summary>
        /// <param name="sch">object documento</param>
        /// <returns></returns>
        private static bool isDocAllegato(DocsPaVO.documento.SchedaDocumento sch, ref DocsPaVO.documento.Allegato retDocAllegato)
        {
            if (sch.documentoPrincipale == null) return false;
            ArrayList allegati = AllegatiManager.getAllegati(sch.documentoPrincipale.docNumber, string.Empty);
            foreach (DocsPaVO.documento.Allegato allegato in allegati.ToArray())
                if (sch.docNumber == allegato.docNumber) { retDocAllegato = allegato; return true; }
            return false;
        }

        /// <summary>
        /// Ritorna le info relative ad un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        private static string AppendDatiEtichettaAllegato(DocsPaVO.documento.Allegato allegato, string orientamento)
        {
            return string.Format("{0}Allegato {1} {2} ({3})", ((orientamento.ToLower() == "verticale") ? "\n" : " - "),
                                                                 DecodeTypeAttachment(allegato.TypeAttachment), allegato.position, allegato.versionLabel);
        }
        /// <summary>
        /// Decodifica tipo allegato
        /// </summary>
        /// <returns></returns>
        private static string DecodeTypeAttachment(int typeAttachment)
        {
            switch (typeAttachment)
            {
                case 1: //allegato tipo utente
                    return "Utente";
                case 2: //allegato tipo PEC
                    return "Pec";
                case 3: //allegato tipo IS
                    return "P.I.Tre.";
                case 4: //allegato tipo SE
                    return "Sist. Esterni";
                default:
                    return string.Empty;
            }
        }
        //>

        /// <summary>
        /// Reperimento dei dati relativi alla firma elettronica
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="versionId"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetDatiFirmaElettronica(string idDocumento, string versionId,DocsPaVO.documento.labelPdf lblPdf, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int maxCharsPerRow = 0;
            if (lblPdf.position.ToUpper().Contains("DX"))
                maxCharsPerRow = 30; // numero di caratteri massimo per riga per allineamento a destra
            else
                maxCharsPerRow = 90; // numero di caratteri massimo per riga per allineamento a sinistra

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool isFirstSigner = true;
            List<DocsPaVO.LibroFirma.FirmaElettronica> listSign = GetElectronicSignatureDocument(idDocumento, versionId, infoUtente);
            if (listSign != null && listSign.Count() > 0)
            { 
                foreach(DocsPaVO.LibroFirma.FirmaElettronica sign in listSign)
                {
                    if (isFirstSigner)
                    {
                        sb.AppendFormat("{0}", FormatLabel("Documento firmato elettronicamente da:", maxCharsPerRow));
                        sb.AppendLine();
                        sb.AppendFormat("{0}", FormatLabel(sign.Firmatario, maxCharsPerRow));
                        sb.AppendLine();
                        sb.AppendFormat("il {0}", FormatLabel(sign.DataApposizione, maxCharsPerRow));
                        isFirstSigner = false;
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.AppendFormat("{0}", FormatLabel(sign.Firmatario, maxCharsPerRow));
                        sb.AppendLine();
                        sb.AppendFormat("il {0}", FormatLabel(sign.DataApposizione, maxCharsPerRow));
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Reperimento dei dati di firma da stampare sull'etichetta pdf
        /// </summary>
        /// <param name="signatureResult"></param>
        /// <returns></returns>
        private static string GetDatiFirmaPerEtichetta(DocsPaVO.documento.VerifySignatureResult signatureResult,
                                                      string dettaglioFirma, DocsPaVO.documento.labelPdf lblPdf,
                                                      string idAmm)
        {
            //PEC Firma - Posizionamento dei dati firma per allineamento a destra/sinistra
            int maxCharsPerRow = 0;
            if (lblPdf.position.ToUpper().Contains("DX"))
                maxCharsPerRow = 30; // numero di caratteri massimo per riga per allineamento a destra
            else
                maxCharsPerRow = 90; // numero di caratteri massimo per riga per allineamento a sinistra

            // Apposizione dati della firma digitale
            if (signatureResult != null && signatureResult.PKCS7Documents != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                //Mev Firma1 < aggiunge l'eventuale dettaglio di firma solo per visualizzazioni dettaglio sintetico e 
                //             se è abilitato il flag di configurazione FE_DETTAGLI_FIRMA
                if (lblPdf.digitalSignInfo.printFormatSign == DocsPaVO.documento.labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Extended)
                {
                    if (getValoreChiaveConfig(idAmm, "FE_DETTAGLI_FIRMA") == "1")
                        if (!string.IsNullOrEmpty(dettaglioFirma))
                        {
                            sb.AppendLine();
                            sb.AppendFormat("{0}", FormatLabel(dettaglioFirma, maxCharsPerRow));
                        }
                }

                bool isFirstSigner = true;
                foreach (DocsPaVO.documento.PKCS7Document signedDoc in signatureResult.PKCS7Documents)
                {
                    foreach (DocsPaVO.documento.SignerInfo signer in signedDoc.SignersInfo)
                    {
                        //gestione firma dettaglio sintetico / dettaglio completo
                        if (lblPdf.digitalSignInfo.printFormatSign == DocsPaVO.documento.labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Short)
                        {
                            if (isFirstSigner)
                            {
                                sb.AppendLine();
                                sb.AppendFormat("Documento firmato digitalmente da: {0}", signer.SubjectInfo.CommonName);
                                isFirstSigner = false;
                            }
                            else sb.AppendFormat(", {0}", signer.SubjectInfo.CommonName);

                            //MEV CONTRO-FIRMATARI:   aggiunta info sintetica controfirmatari
                            if (signer.counterSignatures != null && signer.counterSignatures.Count() > 0)
                                sb.Append(GetDatiControFirmatari(signer));
                                //foreach (DocsPaVO.documento.SignerInfo controfirmatari in signer.counterSignatures)
                                //    sb.AppendFormat(", {0}", controfirmatari.SubjectInfo.CommonName);
                        }
                        else
                        {
                            sb.AppendLine(GetDatiFirmaPerEtichettaInfoFirmaCompleta(signer, maxCharsPerRow));

                            //MEV CONTRO-FIRMATARI: aggiunta info completa controfirmatari
                            if (signer.counterSignatures != null && signer.counterSignatures.Count() > 0)
                            {
                                //foreach (DocsPaVO.documento.SignerInfo controSigner in signer.counterSignatures)
                                //    sb.AppendLine(GetDatiFirmaPerEtichettaInfoFirmaCompleta(controSigner,maxCharsPerRow));
                                    sb.AppendLine(GetDatiControFirmatariCompleta(signer, maxCharsPerRow));
                            }
                        }
                        //>
                    }

                    // gestione del ritorno a capo con allineamento SX/DX relativo al dettaglio sintetico della firma
                    if ((!string.IsNullOrEmpty(sb.ToString())) && (lblPdf.digitalSignInfo.printFormatSign == DocsPaVO.documento.labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Short))
                        sb.Replace(sb.ToString(), FormatLabel(sb.ToString(), maxCharsPerRow));
                }

                return sb.ToString();
            }
            else
                return string.Empty;
        }

        private static string GetDatiControFirmatari(DocsPaVO.documento.SignerInfo signer)
        {
            string result = string.Empty;
            if (signer.counterSignatures != null && signer.counterSignatures.Count() > 0)
            {
                foreach (DocsPaVO.documento.SignerInfo controfirmatari in signer.counterSignatures)
                {
                    result += string.Format(", {0}", controfirmatari.SubjectInfo.CommonName);
                    result += GetDatiControFirmatari(controfirmatari);
                }
            }
            return result;
        }

        private static string GetDatiControFirmatariCompleta(DocsPaVO.documento.SignerInfo signer, int maxCharsPerRow)
        {
            if (signer.counterSignatures != null && signer.counterSignatures.Count() > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (DocsPaVO.documento.SignerInfo controfirmatari in signer.counterSignatures)
                {
                    sb.AppendLine(GetDatiFirmaPerEtichettaInfoFirmaCompleta(controfirmatari, maxCharsPerRow));
                    sb.AppendLine(GetDatiControFirmatariCompleta(controfirmatari, maxCharsPerRow));
                }
                return sb.ToString();
            }
            else
                return string.Empty;
        }
        /// <summary>
        /// Formatta le informazioni relative alla firma completa
        /// </summary>
        private static string GetDatiFirmaPerEtichettaInfoFirmaCompleta(DocsPaVO.documento.SignerInfo signer, int maxCharsPerRow)
        {
            System.Text.StringBuilder sbTextInfoFirma = new StringBuilder();
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("ENTE CERTIFICATORE: {0}", getEnteCertificatore(signer.CertificateInfo.IssuerName)), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("SN CERTIFICATO: {0}", signer.CertificateInfo.SerialNumber), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("VALIDO DA: {0}", signer.CertificateInfo.ValidFromDate), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("VALIDO AL: {0}", signer.CertificateInfo.ValidToDate), maxCharsPerRow));
            sbTextInfoFirma.AppendLine();
            sbTextInfoFirma.AppendFormat(FormatLabel(string.Format("FIRMATARI: {0}", signer.SubjectInfo.CommonName), maxCharsPerRow));

            return sbTextInfoFirma.ToString();
        }


        /// <summary>
        /// Formattazione label su documenti in formato A4 (ritorno a capo) 
        /// </summary>
        /// <param name="text">testo da formattare</param>
        /// <param name="maxCharsPerRow">numero massimo di caratteri per riga</param>
        /// <returns></returns>
        private static string FormatLabel(string text, int maxCharsPerRow)
        {
            string rowWord = string.Empty;
            string prevRowWord = string.Empty;
            string formattedText = string.Empty;
            if (text.Length <= maxCharsPerRow) return text;
            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                prevRowWord = rowWord;
                rowWord += (string.IsNullOrEmpty(prevRowWord)) ? word : string.Format(" {0}", word);
                if (rowWord.Length > maxCharsPerRow)
                {
                    formattedText += prevRowWord + "\r\n";
                    rowWord = word;
                }
            }
            return formattedText + rowWord;
        }
        //>




        //Mev Firma1 < Recupera un valore di configurazione 
        public static string getValoreChiaveConfig(string idAmm, string keyId)
        {
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(idAmm);
            ArrayList lista = new ArrayList(chiaviAmm.ListaChiavi);
            foreach (DocsPaVO.amministrazione.ChiaveConfigurazione itemKeyConfig in lista)
                if (itemKeyConfig.Codice == keyId)
                    return itemKeyConfig.Valore;
            return string.Empty;
        }
        //>

        //Mev Firma1 < aggiunto function getEnteCertificatore()
        /// <summary>
        /// Formatta i dati relativi all'ente certificatore
        /// </summary>
        /// <param name="issuesName">IssuesName con formato generico CN={0},CN={1}, OU={2}, O={3}, C={4}</param>
        /// <returns>Ente certificatore contenete le info di CN, O, C  </returns>
        private static string getEnteCertificatore(string issuerName)
        {
            string enteCertCN = string.Empty;
            string enteCertO = string.Empty;
            string enteCertC = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(issuerName)) return string.Empty; //no issuesName
                string[] issuerNamePars = issuerName.Split(',');
                // recupera CN
                if (issuerName.Contains("CN="))
                    enteCertCN = issuerNamePars.Where(a => a.Contains("CN=")).SingleOrDefault().Split('=')[1];
                // recupera O
                if (issuerName.Contains("O="))
                    enteCertO = string.Format("{0}{1}", (string.IsNullOrEmpty(enteCertCN) ? string.Empty : ", "), issuerNamePars.Where(a => a.Contains("O=")).SingleOrDefault().Split('=')[1]);
                if (issuerName.Contains("C="))
                    enteCertC = string.Format(", {0}", issuerNamePars.Where(a => a.Contains("C=")).SingleOrDefault().Split('=')[1]);
            }
            catch(Exception e)
            {
                logger.Error("Errore in getEnteCertificatore: " + e.Message);
            }
            return string.Format("{0}{1}{2}", enteCertCN, enteCertO, enteCertC);
        }
        //>

        #region UtilsTimbro
        /// <summary>
        /// Questo metodo rimuove i separatori o la descrizione dei campi non valorizzati del timbro
        /// </summary>
        /// <returns></returns>
        private static string RemoveDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int count = 0;
            int start = 0;
            int inizio = 0;
            string specialChar = "#%*@";
            //Uso un ciclo while nel caso ci sia per errore più di un'occorrenza del codice da rimuovere!
            while (currTimbro.Contains(currVal))
            {
                //devo ricalcolare l'indice dei codici precedenti
                int[] ordine = codicePrec(timbro_iniziale);

                if (currVal.Equals("COD_AMM"))
                {
                    if (ordine[0] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
                        count = currTimbro.IndexOf("COD_AMM") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_AMM");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_REG"))
                {
                    if (ordine[1] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
                        count = currTimbro.IndexOf("COD_REG") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_REG");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("NUM_PROTO"))
                {
                    if (ordine[2] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
                        count = currTimbro.IndexOf("NUM_PROTO") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("NUM_PROTO");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("DATA_COMP"))
                {
                    if (ordine[3] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
                        count = currTimbro.IndexOf("DATA_COMP") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("DATA_COMP");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("ORA"))
                {
                    if (ordine[4] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
                        count = currTimbro.IndexOf("ORA") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("ORA");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("NUM_ALLEG"))
                {
                    if (ordine[5] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
                        count = currTimbro.IndexOf("NUM_ALLEG") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("NUM_ALLEG");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("CLASSIFICA"))
                {
                    if (ordine[6] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
                        count = currTimbro.IndexOf("CLASSIFICA") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("CLASSIFICA");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("IN_OUT"))
                {
                    if (ordine[7] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
                        count = currTimbro.IndexOf("IN_OUT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("IN_OUT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (currVal.Equals("COD_UO_PROT"))
                {
                    if (ordine[8] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[8]]) + dati[ordine[8]].Length;
                        count = currTimbro.IndexOf("COD_UO_PROT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_UO_PROT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_UO_VIS"))
                {
                    if (ordine[9] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[9]]) + dati[ordine[9]].Length;
                        count = currTimbro.IndexOf("COD_UO_VIS") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_UO_VIS");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_RF_PROT"))
                {
                    if (ordine[10] >= 0)
                    {
                        start = GetStartIndex(dati, ordine, currTimbro, 10);
                        count = currTimbro.IndexOf("COD_RF_PROT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_RF_PROT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_RF_VIS"))
                {
                    if (ordine[11] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[11]]) + dati[ordine[11]].Length;
                        count = currTimbro.IndexOf("COD_RF_VIS") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_RF_VIS");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            }
            currTimbro = currTimbro.Replace(specialChar, currVal);
            return currTimbro;
        }

        private static int GetStartIndex(string[] dati, int[] ordine, string currTimbro, int index)
        {
            int start = 0;
            if (ordine[index] >= 0)
            {
                if (currTimbro.IndexOf(dati[ordine[index]]) < 0)
                {
                    start = GetStartIndex(dati, ordine, currTimbro, ordine[index]);
                }
                else
                {
                    start = currTimbro.IndexOf(dati[ordine[index]]) + dati[ordine[index]].Length;
                }
            }
            return start;
        }

        /// <summary>
        /// Restituisce il vettore degli indici dei codici precedenti
        /// </summary>
        /// <param name="timbro_iniziale"></param>
        /// <returns></returns>
        private static int[] codicePrec(string timbro_iniziale)
        {
            int[] ordine = new int[12];
            int i = -1;
            while (timbro_iniziale != string.Empty)
            {
                string appo = timbro_iniziale;
                if (timbro_iniziale.StartsWith("COD_AMM"))
                {
                    ordine[0] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
                    i = 0;
                }
                if (timbro_iniziale.StartsWith("COD_REG"))
                {
                    ordine[1] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
                    i = 1;
                }
                if (timbro_iniziale.StartsWith("NUM_PROTO"))
                {
                    ordine[2] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
                    i = 2;
                }
                if (timbro_iniziale.StartsWith("DATA_COMP"))
                {
                    ordine[3] = i;
                    timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
                    i = 3;
                }
                if (timbro_iniziale.StartsWith("ORA"))
                {
                    ordine[4] = i;
                    timbro_iniziale = timbro_iniziale.Replace("ORA", "");
                    i = 4;
                }
                if (timbro_iniziale.StartsWith("NUM_ALLEG"))
                {
                    ordine[5] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
                    i = 5;
                }
                if (timbro_iniziale.StartsWith("CLASSIFICA"))
                {
                    ordine[6] = i;
                    timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
                    i = 6;
                }
                if (timbro_iniziale.StartsWith("IN_OUT"))
                {
                    ordine[7] = i;
                    timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
                    i = 7;
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (timbro_iniziale.StartsWith("COD_UO_PROT"))
                {
                    ordine[8] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
                    i = 8;
                }
                if (timbro_iniziale.StartsWith("COD_UO_VIS"))
                {
                    ordine[9] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
                    i = 9;
                }
                if (timbro_iniziale.StartsWith("COD_RF_PROT"))
                {
                    ordine[10] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
                    i = 10;
                }
                if (timbro_iniziale.StartsWith("COD_RF_VIS"))
                {
                    ordine[11] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
                    i = 11;
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }
            return ordine;
        }

        /// <summary>
        /// Questo metodo restituisce i separatori o la descrizione dei campi richiesti
        /// </summary>
        /// <returns></returns>
        private static string ReturnDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int[] ordine = new int[12];
            int i = -1;
            while (timbro_iniziale != string.Empty)
            {
                string appo = timbro_iniziale;
                if (timbro_iniziale.StartsWith("COD_AMM"))
                {
                    ordine[0] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
                    i = 0;
                }
                if (timbro_iniziale.StartsWith("COD_REG"))
                {
                    ordine[1] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
                    i = 1;
                }
                if (timbro_iniziale.StartsWith("NUM_PROTO"))
                {
                    ordine[2] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
                    i = 2;
                }
                if (timbro_iniziale.StartsWith("DATA_COMP"))
                {
                    ordine[3] = i;
                    timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
                    i = 3;
                }
                if (timbro_iniziale.StartsWith("ORA"))
                {
                    ordine[4] = i;
                    timbro_iniziale = timbro_iniziale.Replace("ORA", "");
                    i = 4;
                }
                if (timbro_iniziale.StartsWith("NUM_ALLEG"))
                {
                    ordine[5] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
                    i = 5;
                }
                if (timbro_iniziale.StartsWith("CLASSIFICA"))
                {
                    ordine[6] = i;
                    timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
                    i = 6;
                }
                if (timbro_iniziale.StartsWith("IN_OUT"))
                {
                    ordine[7] = i;
                    timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
                    i = 7;
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (timbro_iniziale.StartsWith("COD_UO_PROT"))
                {
                    ordine[8] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
                    i = 8;
                }
                if (timbro_iniziale.StartsWith("COD_UO_VIS"))
                {
                    ordine[9] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
                    i = 9;
                }
                if (timbro_iniziale.StartsWith("COD_RF_PROT"))
                {
                    ordine[10] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
                    i = 10;
                }
                if (timbro_iniziale.StartsWith("COD_RF_VIS"))
                {
                    ordine[11] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
                    i = 11;
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }

            int count = 0;
            int start = 0;
            if (currVal.Equals("COD_AMM"))
            {
                if (ordine[0] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
                    count = currTimbro.IndexOf("COD_AMM") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("COD_AMM");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("COD_REG"))
            {
                if (ordine[1] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
                    count = currTimbro.IndexOf("COD_REG") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("COD_REG");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("NUM_PROTO"))
            {
                if (ordine[2] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
                    count = currTimbro.IndexOf("NUM_PROTO") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("NUM_PROTO");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("DATA_COMP"))
            {
                if (ordine[3] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
                    count = currTimbro.IndexOf("DATA_COMP") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("DATA_COMP");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("ORA"))
            {
                if (ordine[4] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
                    count = currTimbro.IndexOf("ORA") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("ORA");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("NUM_ALLEG"))
            {
                if (ordine[5] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
                    count = currTimbro.IndexOf("NUM_ALLEG") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("NUM_ALLEG");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("CLASSIFICA"))
            {
                if (ordine[6] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
                    count = currTimbro.IndexOf("CLASSIFICA") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("CLASSIFICA");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("IN_OUT"))
            {
                if (ordine[7] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
                    count = currTimbro.IndexOf("IN_OUT") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("IN_OUT");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            return currTimbro;
        }

        /// <summary>
        /// Restituisce l'Unità Organizzativa relativa al'id in CorrGlob di un dato Ruolo
        /// </summary>
        /// <param name="idCorrGlob_Ruolo"></param>
        /// <returns></returns>
        private static string getCodiceUO(string idCorrGlob_Ruolo)
        {
            string uo = string.Empty;
            //NB: l'oggetto registri contenuto dentro l'oggetto ruolo NON prevede gli RF ma SOLO i registri
            //    associati a tale ruolo!!!
            DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuolo(idCorrGlob_Ruolo);
            if (ruolo != null)
            {
                uo = ruolo.uo.codice;
            }
            return uo;
        }

        /// <summary>
        /// Restituisce l'Unità Organizzativa relativa al'id in CorrGlob di un dato Ruolo
        /// </summary>
        /// <param name="idCorrGlob_Ruolo"></param>
        /// <returns></returns>
        private static string getCodiceUOEnabledAndDisabled(string idCorrGlob_Ruolo)
        {
            string uo = string.Empty;
            //NB: l'oggetto registri contenuto dentro l'oggetto ruolo NON prevede gli RF ma SOLO i registri
            //    associati a tale ruolo!!!
            DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuoloEnabledAndDisabled(idCorrGlob_Ruolo);
            if (ruolo != null)
            {
                uo = ruolo.uo.codice;
            }
            return uo;
        }


        /// <summary>
        /// Restituisce RF, se esiste, relativo al'id in CorrGlob di un dato Ruolo
        /// </summary>
        /// <param name="idCorrGlob_Ruolo"></param>
        /// <returns></returns>
        private static string getCodiceRF(string idCorrGlob_Ruolo)
        {
            string RF = string.Empty;
            //NB: idCorrGlob_Ruolo in questo caso è equivalente a ruolo.systemId!!!
            //Se invece che "1" passo "null" ottengo anche i registri oltre gli RF.
            ArrayList listaRF = Utenti.RegistriManager.getListaRegistriRfRuolo(idCorrGlob_Ruolo, "1", null);
            if (listaRF.Count > 0)
            {
                DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)listaRF[0];
                if (reg != null)
                {
                    if (reg.chaRF == "1")
                    {
                        RF = reg.codRegistro;
                    }
                }
            }
            return RF;
        }
        #endregion


        #region utilsMethod


        /// <summary>
        /// Reperimento cartella temporanea conversione pdf
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetConvertPdfTempFolder(DocsPaVO.utente.InfoUtente infoUtente)
        {
            string basePath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");

            string codiceAmministrazione = string.Empty;

            using (DocsPaDB.Query_Utils.Utils dbUtils = new DocsPaDB.Query_Utils.Utils())
                codiceAmministrazione = dbUtils.getCodAmm(infoUtente.idAmministrazione);

            const string PARAM = "%DATA";

            if (basePath.Contains(PARAM))
                basePath = basePath.Replace(PARAM, string.Format(@"DPA.Convert\{0}\", codiceAmministrazione));
            else
                basePath = Path.Combine(basePath, string.Format(@"DPA.Convert\{0}\", codiceAmministrazione));

            return basePath;
        }

        private static bool ConvertToPdf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileDocumento originalFileDocument, string outputPdfFile)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            string temporaryFolder = GetConvertPdfTempFolder(infoUtente);

            if (!Directory.Exists(temporaryFolder))
                Directory.CreateDirectory(temporaryFolder);

            string temporaryFilePath = (Path.Combine(temporaryFolder, infoUtente.userId + "_" + Guid.NewGuid() + "_" + originalFileDocument.name));

            if (outputPdfFile.IndexOf(@"\") == -1)
                outputPdfFile = (temporaryFolder + outputPdfFile);

            try
            {
                // Il file da convertire viene salvato nella stessa cartella 
                // del file di output pdf per essere successivamente rimosso
                //if (File.Exists(temporaryFilePath))
                //    File.Delete(temporaryFilePath);


                if (!File.Exists(temporaryFilePath))
                {
                    File.WriteAllBytes(temporaryFilePath, originalFileDocument.content);
                }

                retValue = PdfConverter.Convert(temporaryFilePath, outputPdfFile);

                if (retValue)
                {

                    logger.Debug(string.Format("Documento '{0}' convertito in pdf nel file '{1}'", temporaryFilePath, outputPdfFile));

                    // Lettura del content del file pdf appena creato
                    byte[] newContent = File.ReadAllBytes(outputPdfFile);

                    // Aggiornamento degli attributi chiave relavivamente all'oggetto "FileDocumento"
                    originalFileDocument.content = newContent;
                    originalFileDocument.length = newContent.Length;
                    originalFileDocument.name += ".pdf";
                    originalFileDocument.fullName += ".pdf";
                    originalFileDocument.contentType = "application/pdf";
                    originalFileDocument.estensioneFile = "pdf";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ConvertToPdf: " + ex.Message);
            }

            try
            {
                // Tentativo di cancellazione del file originale temporaneo
                if (File.Exists(temporaryFilePath))
                    File.Delete(temporaryFilePath);

                // Tentativo di cancellazione del file pdf temporaneo
                if (File.Exists(outputPdfFile))
                    File.Delete(outputPdfFile);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore non bloccante in ConvertToPdf, non è stato possibile cancellare il file temporaneo: " + ex.Message);
            }
            logger.Info("END");
            return retValue;
        }

        private static iTextSharp.text.pdf.BaseFont castFontTypePreferences(ref DocsPaVO.documento.FileDocumento file)
        {

            try
            {
                if (file.LabelPdf.font_type == "COURIER_BOLD")
                {
                    return BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, false);
                }
                if (file.LabelPdf.font_type == "COURIER")
                {
                    return BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
                }
                if (file.LabelPdf.font_type == "HELVETICA")
                {
                    return BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                }
                if (file.LabelPdf.font_type == "HELVETICA_BOLD")
                {
                    return BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false);
                }

                if (file.LabelPdf.font_type == "HELVETICA BOLD")
                {
                    return BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false);
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw ex;
            }
            return null;

        }

        private static iTextSharp.text.Color castFontColorPreferences(ref DocsPaVO.documento.FileDocumento file)
        {
            const string RED = "RED";
            const string BLACK = "BLACK";
            iTextSharp.text.Color color = null;
            try
            {
                switch (file.LabelPdf.font_color)
                {
                    case RED:
                        color = new iTextSharp.text.Color(System.Drawing.Color.Red);
                        break;
                    case BLACK:
                        color = new iTextSharp.text.Color(System.Drawing.Color.Black);
                        break;
                    default:
                        color = new iTextSharp.text.Color(System.Drawing.Color.Red);
                        break;
                }
            }
            /*
            try
            {
                if (file.LabelPdf.font_color == "RED")
                {
                    return color = new iTextSharp.text.Color(System.Drawing.Color.Red);

                }
                if (file.LabelPdf.font_color == "BLACK")
                {
                    return color = new iTextSharp.text.Color(System.Drawing.Color.Black);

                }
            }*/
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw ex;
            }
            return color;
        }

        #endregion


        /// <summary>
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <param name="debug"></param>
        public static void saveFile(DocsPaVO.documento.FileDocumento fileDoc)
        {
            try
            {
                FileStream file = File.Create(fileDoc.fullName);
                file.Write(fileDoc.content, 0, fileDoc.length);
                file.Flush();
                file.Close();

            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione del salvataggio del File (saveFile)", e);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Reperimento di un file firmato (.p7m)
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento getFileFirmato(
                DocsPaVO.documento.FileRequest objFileRequest,
                DocsPaVO.utente.InfoUtente objSicurezza, bool conservazione)
        {
            return getFile(objFileRequest, objSicurezza, false, conservazione);
        }

        /// <summary>
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento getFile(
                DocsPaVO.documento.FileRequest objFileRequest,
                DocsPaVO.utente.InfoUtente objSicurezza)
        {

            return getFile(objFileRequest, objSicurezza, true, false);
        }

        /// <summary>
        /// Reperimento path del file acquisito per l'ultima versione del documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string getCurrentVersionFilePath(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
                return dbDocumenti.GetCurrentVersionFilePath(docNumber);
        }

        /// <summary>
        /// Reperimento nome originale del file acquisito per l'ultima versione del documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string getOriginalFileName(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest objFileRequest)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                string OriginalfileName = dbDocumenti.GetNomeOriginale(objFileRequest.versionId, objFileRequest.docNumber);
                if (!string.IsNullOrEmpty(OriginalfileName))
                {
                    return removeIllegalChars(OriginalfileName, false);
                }
                else
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// Reperimento nome originale del file acquisito per l'ultima versione del documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string getFileName(String versionId, String docNumber)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
                return dbDocumenti.GetFileName(versionId, docNumber);
        }

        /// <summary>
        /// Modifica nome originale del file acquisito per l'ultima versione del documento Deve essere cartaceo e scansito
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="fileName"></param>
        public static bool setOriginalFileName(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest objFileRequest, string fileName, bool force = false)
        {
            bool retval = false;
            bool execute = false;
            if (force)
                execute = true;

            if (objFileRequest.cartaceo)
                execute = true;


            if (execute)
            {


                string ofn = getOriginalFileName(infoUtente, objFileRequest);
                //IF decidere se il nome deve avere criteri
                //DPA_IMAGE
                //GUID dell IMAGE e estensione (si pensa TIF)
                using (DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti())
                    dbDocumenti.UpdateComponentsOfn(fileName, objFileRequest.versionId, objFileRequest.docNumber);
                retval = true;
            }
            return retval;
        }


        public static DocsPaVO.documento.FileDocumento getInfoFile(
            DocsPaVO.documento.FileRequest objFileRequest,
            DocsPaVO.utente.InfoUtente objSicurezza
            )
        {
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            fileDoc.path = objFileRequest.docServerLoc + objFileRequest.path;
            fileDoc.name = objFileRequest.fileName;
            int indice;
            //

            if (ConfigurationManager.AppSettings["documentale"].ToUpper() == "FILENET")
            {
                DocsPaDocumentale.Documentale.DocumentManager FNdoc = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
                fileDoc.name = FNdoc.GetOriginalFileName(objFileRequest.docNumber, objFileRequest.versionId);
            }
            fileDoc.nomeOriginale = getOriginalFileName(objSicurezza, objFileRequest);
            indice = fileDoc.name.LastIndexOf(@"\");
            if (indice < (fileDoc.name.Length - 1))
            {
                fileDoc.name = fileDoc.name.Substring(indice + 1);
            }

            //modifica
            if (string.IsNullOrEmpty(fileDoc.path))
                fileDoc.fullName = '\u005C'.ToString() + fileDoc.name;
            else
                //fine modifica
                //fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;
                fileDoc.fullName = objFileRequest.fileName;
            fileDoc.contentType = getContentType(fileDoc.name);
            logger.Debug("Full name: " + fileDoc.fullName);
            logger.Debug("idAmministrazione: " + objSicurezza.idAmministrazione);
            logger.Debug("Library: " + DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());

            string docNumber = objFileRequest.docNumber;
            // string versionId = objFileRequest.versionId;	
            string version_label = objFileRequest.versionLabel;

            return fileDoc;
        }

        /// <summary>
        /// Reperimento del file richiesto e conversione inline in pdf
        /// </summary>
        /// <remarks>
        /// La conversione in pdf è effettuata solo se il convertitore
        /// correntemente impostato supporta il formato del file originale
        /// </remarks>
        /// <param name="objFileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="verificaFileFirmato"></param>
        /// <param name="convertPdfInLine">
        /// Se true, converte il file originale in formato pdf 
        /// (se il formato è supportato dal convertitore corrementente impostato)
        /// </param>
        /// <param name="isConverted">
        /// True se il file è stato convertito in pdf
        /// </param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento getFile(
                            DocsPaVO.documento.FileRequest objFileRequest,
                            DocsPaVO.utente.InfoUtente objSicurezza,
                            bool verificaFileFirmato,
                            bool convertPdfInLine,
                            out bool isConverted)
        {
            logger.Info("BEGIN");
            isConverted = false;
            DocsPaVO.documento.FileDocumento fileDocument = getFile(objFileRequest, objSicurezza, verificaFileFirmato, false);

            if (fileDocument != null && convertPdfInLine && (!string.IsNullOrEmpty(fileDocument.name)) && PdfConverter.CanConvertFile(fileDocument.name))
            {
                // Conversione in pdf del file, 
                // se la tipologia di file è tra quelle 
                // per cui è possibile effettuarla
                string outputfileName = Path.Combine(GetConvertPdfTempFolder(objSicurezza), objSicurezza.userId + "_" + objSicurezza.idPeople + "_" + fileDocument.name + ".pdf");

                isConverted = ConvertToPdf(objSicurezza, fileDocument, outputfileName);
            }
            logger.Info("END");
            return fileDocument;
        }

        /// <summary>
        /// Reperimento del file e gestione del file eml in versione html
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento getFileAsEML(DocsPaVO.documento.FileRequest objFileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            DocsPaVO.documento.FileDocumento fd = getFile(objFileRequest, objSicurezza, true, false);
            if (fd != null && fd.name.ToUpper().EndsWith(".EML"))
            {
                BusinessLogic.Interoperabilità.IMailConnector iMailConnector = new BusinessLogic.Interoperabilità.SvrPosta();
                BusinessLogic.Interoperabilità.CMMsg cmm = iMailConnector.getMessage(fd.content);

                /*
                if (string.IsNullOrEmpty(fd.path))
                {
                    DocsPaDB.Query_DocsPAWS.Documentale docum = new DocsPaDB.Query_DocsPAWS.Documentale();
                    fd.path = docum.GetFileName(objFileRequest.versionId);
                }*/

                //BusinessLogic.Interoperabilità.CMMsg cmm = iMailConnector.getMessage(System.IO.File.ReadAllText(fd.path));
                //string eml = System.IO.File.ReadAllText(fd.path, encoding);
                //BusinessLogic.Interoperabilità.CMMsg cmm1 = iMailConnector.getMessage(eml);
                string testoMessaggio = string.Empty;
                if (!string.IsNullOrEmpty(cmm.HTMLBody))
                {
                    System.Text.StringBuilder str = new StringBuilder();
                    string htmlBody = cmm.HTMLBody;
                    string[] tmp = htmlBody.Split(new string[] { "</body>" }, StringSplitOptions.None);
                    string restOfMessage = string.Empty;
                    if (tmp.Length > 1)
                        restOfMessage = tmp[1];

                    str.AppendFormat("{0}Mail Mittente: {1}</body>{2}", tmp[0], cmm.from, restOfMessage);
                    testoMessaggio = str.ToString();
                }
                else
                    testoMessaggio = string.Format("{0},{1},{2}", "<pre>", cmm.body, "</pre>");
                fd.contentType = "text/html";
                //13/01/2015: non visualizzava correttamente i caratteri accentati
                //fd.content = encoding.GetBytes(testoMessaggio);
                fd.content = System.Text.Encoding.UTF8.GetBytes(testoMessaggio);
            }
            return fd;
        }

        #region Anteprime pdf

        /// <summary>
        /// Restituzione ed eventuale generazione anteprima file pdf
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// <returns>FileDocumentoAnteprima</returns>
        public static DocsPaVO.documento.FileDocumentoAnteprima getPreviewFilePdf(
            DocsPaVO.documento.FileRequest objFileRequest, int pgFrom, int pgTo,
            DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.documento.labelPdf position, 
            DocsPaVO.utente.InfoUtente objSicurezza
            )
        {
            FileDocumentoAnteprima retVal = new FileDocumentoAnteprima();
            AnteprimaPdf anteprima = null;

            
            int numberOfPages = (string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PREVIEW_PG"))?3:int.Parse(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PREVIEW_PG"))); //= 3 ABBATANGELI - Caricare da chiavi di configurazione

            DocsPaDB.Query_DocsPAWS.Documenti qsD = new DocsPaDB.Query_DocsPAWS.Documenti();
            
            //ABBATANGELI - Provo a caricare dal db l'anteprima relativa alla coppia versionId docNumber
            List<AnteprimaPdf> antemprime = qsD.getPdfPreviews(objFileRequest.versionId, objFileRequest.docNumber);
            if (antemprime.Count > 0)
            {
                if (pgTo == 0)
                    anteprima = antemprime.FirstOrDefault(e => e.previewPageFrom == pgFrom);
                else
                    anteprima = antemprime.FirstOrDefault(e => e.previewPageNamber == pgTo);
            }

            retVal.inFileSystem = (anteprima != null && !string.IsNullOrEmpty(anteprima.pathFile));

            //ABBATANGELI - Se non ho già creato l'alteprima o è stata cancellata, la genero
            if (!retVal.inFileSystem || !File.Exists(anteprima.pathFile))
            {
                string newPreviewFileName = objFileRequest.versionId;
                if (pgFrom == 0)
                {
                    if ((pgTo - numberOfPages) <= 0)
                        newPreviewFileName = newPreviewFileName + "_1.pdf";
                    else
                        newPreviewFileName = newPreviewFileName + "_" + (pgTo - numberOfPages).ToString() + ".pdf";
                }
                else
                {
                    newPreviewFileName = newPreviewFileName + "_" + pgFrom.ToString() + ".pdf";
                }

                retVal.Import(getFile(objFileRequest, objSicurezza, true, false));
                retVal.firstPg = pgFrom;
                retVal.lastPg = (pgFrom + (numberOfPages - 1));

                anteprima = MakePreview(ref retVal, newPreviewFileName, objSicurezza);
                if (anteprima != null)
                {
                    anteprima.versionId = objFileRequest.versionId;
                    anteprima.docNumber = objFileRequest.docNumber;
                    qsD.insPdfPreviews(anteprima);

                    retVal.lastPg = anteprima.previewPageNamber;
                }
            }
            else
            {
                //ABBATANGELI - Carico l'anteprima utilizzando i dati provenienti da db
                retVal.Import(getInfoFile(objFileRequest, objSicurezza));
                retVal.firstPg = anteprima.previewPageFrom;
                retVal.lastPg = anteprima.previewPageNamber;
                retVal.totalPg = anteprima.totalPageNumber;
                retVal.content = streamFileFromPath(anteprima.pathFile);
                retVal.length = retVal.content.Length;
            }

            string version_label_allegato = string.Empty;

            //if (retVal.firstPg == 1)
            //{
                FileDocumento fileDoc = new FileDocumento();
                fileDoc.fullName = retVal.fullName;
                fileDoc.path = anteprima.pathFile;
                fileDoc.name = retVal.fullName;
                fileDoc.LabelPdf = position;
                fileDoc.content = retVal.content;

                fileDoc = addEtic(ref fileDoc, sch, objSicurezza, position, version_label_allegato, objFileRequest.versionId);
                if (fileDoc.content != null && fileDoc.content.Count() > 0)
                    retVal.content = fileDoc.content;
            //}
            retVal.contentType = getContentType(fileDoc.name);

            return retVal;
        }

        private static byte[] streamFileFromPath(string path)
        {
            byte[] stream = null;

            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string pathfile = path;
                    if (!string.IsNullOrEmpty(pathfile))
                    {
                        FileInfo file = new FileInfo(pathfile);
                        stream = new byte[file.Length];
                        using (FileStream fs = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                        {
                            fs.Read(stream, 0, stream.Length);
                            fs.Close();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore nel metodo streamFileFromPath: " + e.Message);
            }
            return stream;
        }

        private static AnteprimaPdf MakePreview(ref FileDocumentoAnteprima fda, string previewFileName, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            AnteprimaPdf anteprimaDB = new AnteprimaPdf();

            string fullPathFileName = System.Configuration.ConfigurationManager.AppSettings["PREVIEWS_PATH"].Replace("%DATA", objSicurezza.idAmministrazione + @"\");
            Directory.CreateDirectory(fullPathFileName);
            anteprimaDB.pathFile = fullPathFileName + previewFileName;

            FileStream fs = new FileStream(anteprimaDB.pathFile, System.IO.FileMode.Create);
            //            stamper.FormFlattening = true;

            PdfReader rPdf = new iTextSharp.text.pdf.PdfReader(fda.content);
            anteprimaDB.totalPageNumber = rPdf.NumberOfPages;
            fda.totalPg = anteprimaDB.totalPageNumber;

            PdfImportedPage importedPage = null;

            try
            {
                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                iTextSharp.text.Document sourceDocument = new iTextSharp.text.Document(rPdf.GetPageSizeWithRotation(fda.firstPg));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                PdfCopy pdfCopyProvider = new PdfCopy(sourceDocument, fs);
                sourceDocument.Open();

                // Walk the specified range and add the page copies to the output file:
                int counter = 0;
                int i = fda.firstPg;
                while (i <= fda.lastPg && i <= fda.totalPg)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(rPdf, i);
                    pdfCopyProvider.AddPage(importedPage);
                    i++;
                    counter++;
                }

                rPdf.Close();
                sourceDocument.Close();
                //pdfCopyProvider.Flush();
                pdfCopyProvider.Close();

                fda.content = File.ReadAllBytes(anteprimaDB.pathFile);
                fda.length = fda.content.Length;

                anteprimaDB.previewPageFrom = fda.firstPg;
                anteprimaDB.previewPageNamber = (fda.firstPg + (counter - 1));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return anteprimaDB;
        }

        #endregion

        private static DocsPaVO.documento.FileDocumento getFile(
            DocsPaVO.documento.FileRequest objFileRequest,
            DocsPaVO.utente.InfoUtente objSicurezza,
            bool verificaFileFirmato, bool conservazione)
        {
            logger.Info("BEGIN");
            //logger.DebugFormat("Parametri verificaFileFirmato {0}  | conservazione {1}", verificaFileFirmato, conservazione);
            bool isContainer = false;
            // Verifica se il file è acquisito nell'ambito di un repository di sessione
            if (objFileRequest.repositoryContext != null)
            {
                DocsPaVO.documento.FileDocumento fileDocument = null;
                // Reperimento del file in un documento quando ancora non è stato salvato,
                //// pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
                if (CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione))
                {
                    if (CacheFileManager.isFileInCache(objFileRequest.docNumber))
                    {
                        string path = CacheFileManager.ricercaPathCaching(objFileRequest.docNumber, objFileRequest.versionId, objSicurezza.idAmministrazione);
                        if (!string.IsNullOrEmpty(path))
                            objFileRequest.path = path;
                    }
                }

                SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(objFileRequest.repositoryContext);

                fileDocument = fileManager.GetFile(objFileRequest);

                if (fileDocument != null && fileDocument.name.ToUpper().EndsWith(".P7M") && verificaFileFirmato)
                {
                    // Se file .P7M, viene fatta la verifica della firma digitale del file
                    VerifyFileSignature(fileDocument, null);
                }
                logger.Info("END");
                return fileDocument;
            }

            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            fileDoc.path = objFileRequest.docServerLoc + objFileRequest.path;
            fileDoc.name = objFileRequest.fileName;

            //
            if (ConfigurationManager.AppSettings["documentale"].ToUpper() == "FILENET")
            {
                DocsPaDocumentale.Documentale.DocumentManager FNdoc = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
                fileDoc.name = FNdoc.GetOriginalFileName(objFileRequest.docNumber, objFileRequest.versionId);
            }
            fileDoc.nomeOriginale = getOriginalFileName(objSicurezza, objFileRequest);

            populateNameAndContent(fileDoc);
            logger.Debug("Full name: " + fileDoc.fullName);
            logger.Debug("idAmministrazione: " + objSicurezza.idAmministrazione);
            logger.Debug("Library: " + DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary());

            string docNumber = objFileRequest.docNumber;
            // string versionId = objFileRequest.versionId;	
            string version_label = objFileRequest.versionLabel;

            // Acquisizione del file 
            //// modalita caching
            string messageError = string.Empty;

            if (CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione))
            {
                if (!CacheFileManager.getFile(ref objSicurezza, ref objFileRequest, fileDoc, out messageError))
                {
                    logger.Debug("Errore nella gestione del File (getFile)");
                    throw new Exception(messageError);
                }
            }
            else// modalità classica
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);

                if (!documentManager.GetFile(ref fileDoc, ref objFileRequest))
                {
                    logger.Debug("Errore nella gestione del File (getFile)");
                    throw new Exception();
                }
            }

            if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".TSR") && !conservazione)
            {
                //se il file è .TSR viene verificata la marca temporale
                if (verificaFileFirmato)
                    VerifyFileTimeStamp(fileDoc);
                logger.Info("END");
                return fileDoc;
            }


            if (GestioneTSAttacced)
            {
                //logger.Debug("Gestione sbusto time stamp attiva");
                //logger.DebugFormat("Parametri EXT {0},  VFF {1}  path{2}", fileDoc.name.ToUpper(), verificaFileFirmato,fileDoc.path);
                // Gestione file timestampati

                //Faillace 10/07
                //documentum , in fase di putfile , non da la giusta estensione per i file TSD nella components (sotto investigazione)
                //prendo quindi il nome originale e lo processo
                //fileDoc.name = fileDoc.nomeOriginale;

                if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".TSD") && verificaFileFirmato)
                {
                    EstrazioneTSD(objFileRequest, fileDoc, objSicurezza);
                    //logger.DebugFormat("Sbusto TSD nuova len{0}", fileDoc.length);
                    populateNameAndContent(fileDoc);
                    isContainer = true;
                }
                if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".M7M") && verificaFileFirmato)
                {
                    EstrazioneM7M(objFileRequest, fileDoc, objSicurezza);
                    //logger.DebugFormat("Sbusto M7M nuova len{0}", fileDoc.length);
                    populateNameAndContent(fileDoc);
                    isContainer = true;
                }
            }

            if (fileDoc != null && fileDoc.name.ToUpper().EndsWith(".P7M") && verificaFileFirmato)
            {
                // Se file .P7M, viene fatta la verifica della firma digitale del file
                try
                {
                    VerifyFileSignature(fileDoc, null);
                }
                catch (Exception ex)
                {
                    logger.Debug("Eroore in verifica Firma " + ex.Message);
                }
                //logger.DebugFormat("Sbusto CADES nuova len{0}", fileDoc.length);
                if (fileDoc.signatureResult != null)
                {
                    ArrayList tsAl = TimestampManager.getTimestampsDoc(objSicurezza, objFileRequest);
                    List<DocsPaVO.documento.TSInfo> tsLst = new List<DocsPaVO.documento.TSInfo>();

                    foreach (DocsPaVO.documento.TimestampDoc tsdoc in tsAl)
                    {
                        if (!string.IsNullOrEmpty(tsdoc.TSR_FILE))
                        {
                            DocsPaVO.documento.TSInfo info = new VerifyTimeStamp().getTSCertInfo(tsdoc.TSR_FILE);
                            tsLst.Add(info);
                        }
                        else
                        {
                            logger.Error("La marca è nulla!!! errore nel DB");
                        }
                    }

                    if (tsLst.Count > 0)
                        fileDoc.signatureResult.DocumentTimeStampInfo = tsLst.ToArray();
                }
                logger.Info("END");
                return fileDoc;
            }
            else
            {
                if (fileDoc.name.ToUpper().EndsWith(".PDF") && GestionePades)
                {
                    if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc))
                    {
                        try
                        {
                            BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.VerifyPadesSignature(fileDoc);
                            //E' una firma pades elaboriamo una VSR per il pades

                            //ItextSharp in alcuni casi non restituisce tutte le firme PADES, in questo caso chiamo il servizio esterno
                            if(!BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.CheckNumSignature(fileDoc) && verificaFileFirmato)
                            {
                                VerifySignatureResult resultTemp = VerifyFileSignaturePades_External(fileDoc, DateTime.Now);
                                if (resultTemp != null && resultTemp.PKCS7Documents != null)
                                {
                                    if (resultTemp.PKCS7Documents.Count() > fileDoc.signatureResult.PKCS7Documents.Count())
                                    {
                                        fileDoc.signatureResult = resultTemp;
                                    }
                                    else if (resultTemp.PKCS7Documents.Count() == fileDoc.signatureResult.PKCS7Documents.Count())
                                    {
                                        for(int i =0; i < resultTemp.PKCS7Documents.Count(); i++)
                                        {
                                            if(resultTemp.PKCS7Documents[i].SignersInfo.Count() > fileDoc.signatureResult.PKCS7Documents[i].SignersInfo.Count())
                                            {
                                                fileDoc.signatureResult = resultTemp;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.DebugFormat("Errore nella verifica della firma PADES: {0} \r\n{1}", ex.Message, ex.StackTrace);
                        }
                    }
                    else
                    {
                        if(IsSignedPades(fileDoc))
                        {
                            VerifySignatureResult resultTemp = VerifyFileSignaturePades_External(fileDoc, DateTime.Now);
                            fileDoc.signatureResult = resultTemp;
                        }
                    }
                   
                }
                else
                {
                    if (fileDoc.name.ToUpper().EndsWith(".XML") && verificaFileFirmato)
                    {
                        VerifyFileSignatureXADES(fileDoc, DateTime.MinValue);
                    }
                }
                // verifico l'impronta, solamente se non si è in repository di sessione
                string versionId = objFileRequest.versionId;
                string impronta = "";

                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                if (CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione))
                {
                    DocsPaDB.Query_DocsPAWS.Caching cache = new DocsPaDB.Query_DocsPAWS.Caching();
                    cache.GetImpronta(out impronta, versionId, objFileRequest.docNumber, objSicurezza.idAmministrazione);
                    if (string.IsNullOrEmpty(impronta))
                    {
                        if (verificaPath(objFileRequest.docServerLoc))
                        {
                            logger.Info("END");
                            return fileDoc;
                        }
                    }
                }
                else
                {
                    // verifico l'impronta, solamente se non si è in repository di sessione


                    doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    doc.GetImpronta(out impronta, versionId, objFileRequest.docNumber);

                    //verifico i path dei file per vedere se bisogna fare il controllo sull'impronta !!! ATTENZIONE
                    if (string.IsNullOrEmpty(impronta))
                    {
                        if (verificaPath(objFileRequest.docServerLoc))
                        {
                            logger.Info("END");
                            return fileDoc;
                        }
                    }
                }
                if (
                    impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fileDoc.content)) ||
                    impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDoc.content)) ||
                    isContainer)
                {
                    logger.Info("END");
                    return fileDoc;
                }
                else
                {
                    logger.Info("END");
                    return null;
                }
            }
        }

        private static void populateNameAndContent(DocsPaVO.documento.FileDocumento fileDoc)
        {
            int indice;

            if ((fileDoc != null) && (!String.IsNullOrEmpty(fileDoc.name)))
            {
                indice = fileDoc.name.LastIndexOf(@"\");
                if (indice < (fileDoc.name.Length - 1))
                {
                    fileDoc.name = fileDoc.name.Substring(indice + 1);
                }
                //modifica
                if (string.IsNullOrEmpty(fileDoc.path))
                    fileDoc.fullName = fileDoc.name;
                else
                    //fineModifica
                    fileDoc.fullName = fileDoc.path + '\u005C'.ToString() + fileDoc.name;

                fileDoc.contentType = getContentType(fileDoc.name);
            }
        }

        public static DateTime dataRiferimentoValitaDocumento(
           DocsPaVO.documento.FileRequest objFileRequest,
           DocsPaVO.utente.InfoUtente objSicurezza)
        {
            DateTime referenceDate = DateTime.MinValue;

            //Prendo il TimeStamp
            var ts = BusinessLogic.Documenti.TimestampManager.getTimestampsDoc(objSicurezza, objFileRequest);

            //Se presente almeno uno
            if (ts.Count > 0)
            {
                //Prendo la data temporale piu vecchia (last of)
                DocsPaVO.documento.TimestampDoc timestampDoc = ts[ts.Count - 1] as DocsPaVO.documento.TimestampDoc;
                if (!String.IsNullOrEmpty(timestampDoc.DTA_CREAZIONE))  //controlliamo se esiste al data creazione (deve esistere)
                {
                    logger.DebugFormat("Data di creazione da timestamp [{0}]", timestampDoc.DTA_CREAZIONE);
                    try
                    {
                        referenceDate = DocsPaUtils.Functions.Functions.ToDate(timestampDoc.DTA_CREAZIONE); //La prendiamo come buona.
                    }
                    catch
                    {
                        referenceDate = Convert.ToDateTime(timestampDoc.DTA_CREAZIONE);
                    }
                }
            }

            //Nel caso non fosse presente il timeTimestamp si fa un fallback verso la data di protocollazione
            if (referenceDate == DateTime.MinValue)
            {
                //Reperisco la scheda documento (usando sicurezza blanda)
                DocsPaVO.documento.SchedaDocumento schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(objSicurezza, objFileRequest.docNumber);

                //se scheda esiste e non è null
                if (schDoc != null)
                {
                    if (schDoc.protocollo != null) // ed è presente un protocollo
                    {
                        if (!String.IsNullOrEmpty(schDoc.protocollo.dataProtocollazione))  //e la data di procollazione non è nulla
                            referenceDate = DocsPaUtils.Functions.Functions.ToDate(schDoc.protocollo.dataProtocollazione); //Prendiamo quella data per buona.

                    }
                    if (referenceDate == DateTime.MinValue)
                    {
                        string dataRepertoriazione = new DocsPaDB.Query_DocsPAWS.Documenti().GetDataCreazioneRepertorio(schDoc.docNumber);
                        if (!String.IsNullOrEmpty(dataRepertoriazione))
                        {
                            try
                            {
                                CultureInfo ci = new CultureInfo("it-IT");
                                referenceDate = DateTime.ParseExact(dataRepertoriazione, "dd/MM/yyyy HH:mm", ci.DateTimeFormat);

                            }
                            catch (Exception e)
                            {
                                logger.ErrorFormat("Errore parsing la data [{0}] {1} {2}", dataRepertoriazione, e.Message, e.StackTrace);
                            }
                        }
                    }
                    if (referenceDate == DateTime.MinValue)
                    {
                        try
                        {
                            referenceDate = DocsPaUtils.Functions.Functions.ToDate(schDoc.dataCreazione); //La prendiamo come buona.
                        }
                        catch
                        {
                            referenceDate = Convert.ToDateTime(schDoc.dataCreazione);
                        }
                    }
                }
            }


            //Se tutto fallisce prendiamo la data attuale alle 00.00  
            if (referenceDate == DateTime.MinValue)
                referenceDate = DateTime.Now.Date;

            logger.DebugFormat("referenceDate - {0}", referenceDate.ToString());

            return referenceDate;
        }
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        public static bool VerifyPadesSignature(DocsPaVO.documento.FileDocumento fileDoc)
        {
            SignedDocument si = new SignedDocument();
            VerifyTimeStamp verifyTimeStamp = new VerifyTimeStamp();

            string padesSignAlgorithm = null;
            PdfReader pdfReader = null;
            try
            {
                pdfReader = new PdfReader(fileDoc.content);
            }
            catch
            {
                return false;
            }

            AcroFields af = pdfReader.AcroFields;
            ArrayList signNames = af.GetSignatureNames();

            if (signNames.Count == 0) //Firma non è presente
                return false;

            List<DocsPaVO.documento.SignerInfo> siList = new List<DocsPaVO.documento.SignerInfo>();
            bool verResult = true;


            foreach (string name in signNames)
            {
                List<DocsPaVO.documento.TSInfo> tsLst = new List<DocsPaVO.documento.TSInfo>();
                PdfPKCS7 pk = af.VerifySignature(name);
                padesSignAlgorithm = "PADES " + pk.GetHashAlgorithm();
                byte[] cert = pk.SigningCertificate.GetEncoded();
                DocsPaVO.documento.SignerInfo sinfo = si.GetCertSignersInfo(cert);
                sinfo.SignatureAlgorithm = padesSignAlgorithm;

                if (verResult) //fino a che è true verifica
                    verResult = pk.Verify();

                if (pk.TimeStampToken != null)
                {
                    //Ricavo il certificato
                    ICollection certsColl = pk.TimeStampToken.GetCertificates("COLLECTION").GetMatches(null);
                    DocsPaVO.documento.TSInfo timeStamp = verifyTimeStamp.getTSCertInfo(certsColl);

                    timeStamp.TSdateTime = pk.TimeStampToken.TimeStampInfo.GenTime.ToLocalTime();
                    timeStamp.TSserialNumber = pk.TimeStampToken.TimeStampInfo.SerialNumber.ToString();
                    timeStamp.TSimprint = Convert.ToBase64String(pk.TimeStampToken.TimeStampInfo.TstInfo.MessageImprint.GetEncoded());
                    timeStamp.TSType = DocsPaVO.documento.TsType.PADES;
                    tsLst.Add(timeStamp);
                }
                if (tsLst.Count > 0)
                    sinfo.SignatureTimeStampInfo = tsLst.ToArray();

                siList.Add(sinfo);
            }

            DocsPaVO.documento.VerifySignatureResult result = new DocsPaVO.documento.VerifySignatureResult();

            if (verResult)
            {
                result.StatusCode = 0;
                result.StatusDescription = "La Verifica OK, ma senza controllo CRL";
            }
            else
            {
                result.StatusCode = -1;
                result.StatusDescription = "La Verifica di almeno un firmatario e Fallita";
            }

            List<DocsPaVO.documento.PKCS7Document> pkcsDocs = new List<DocsPaVO.documento.PKCS7Document>();
            if ((fileDoc.signatureResult != null) && (fileDoc.signatureResult.PKCS7Documents != null) && (fileDoc.signatureResult.PKCS7Documents.Length > 0))
            {
                foreach (DocsPaVO.documento.PKCS7Document docs in fileDoc.signatureResult.PKCS7Documents)
                    pkcsDocs.Add(docs);
            }

            pkcsDocs.Add(new DocsPaVO.documento.PKCS7Document { SignersInfo = siList.ToArray(), SignAlgorithm = padesSignAlgorithm, DocumentFileName = fileDoc.nomeOriginale, SignHash = "Non Disponibile per la firma PADES", SignatureType = DocsPaVO.documento.SignType.PADES });
            result.PKCS7Documents = pkcsDocs.ToArray();
            result.FinalDocumentName = fileDoc.name;
            fileDoc.signatureResult = result;

            return false;
        }
        */

        public static bool VerifyFileSignatureXADES(DocsPaVO.documento.FileDocumento fileDoc, DateTime? dataDiRiferimento)
        {
            bool retval = false;
            VerifySignature verifySignature = new VerifySignature();

            string inputDirectory = verifySignature.GetPKCS7InputDirectory();

            // Creazione cartella di appoggio nel caso non esista
            if (!System.IO.Directory.Exists(inputDirectory))
                System.IO.Directory.CreateDirectory(inputDirectory);

            logger.Debug("PKCS7InputDirectory: " + inputDirectory);

            string inputFile = string.Concat(inputDirectory, fileDoc.name);

            // Copia del file firmato dalla cartella del documentale
            // alla cartella di input utilizzata dal ws della verifica
            CopySignedFileToInputFolder(fileDoc, inputFile);
            fileDoc.signatureResult = verifySignature.VerifySignatureXADES_External(fileDoc, dataDiRiferimento.Value);
            try
            {
                // Rimozione del file firmato dalla cartella di input
                File.Delete(inputFile);
            }
            catch
            {
            }
            if (fileDoc.signatureResult != null && fileDoc.signatureResult.StatusCode == 0) //Valido
                retval = true;

            return retval;
        }

        /// <summary>
        /// Verifica firma digitale del file
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <param name="dataDiRiferimento"> data di riferimento , se null non controlla la CLR esterna</param>
        public static bool VerifyFileSignature(DocsPaVO.documento.FileDocumento fileDoc, DateTime? dataDiRiferimento)
        {
            bool retval = false;
            VerifySignature verifySignature = new VerifySignature();



            string inputDirectory = verifySignature.GetPKCS7InputDirectory();

            // Creazione cartella di appoggio nel caso non esista
            if (!System.IO.Directory.Exists(inputDirectory))
                System.IO.Directory.CreateDirectory(inputDirectory);

            logger.Debug("PKCS7InputDirectory: " + inputDirectory);

            string inputFile = string.Concat(inputDirectory, fileDoc.name);

            // Copia del file firmato dalla cartella del documentale
            // alla cartella di input utilizzata dal ws della verifica
            CopySignedFileToInputFolder(fileDoc, inputFile);

            if (dataDiRiferimento == null)
            {
                fileDoc.signatureResult = verifySignature.Verify(fileDoc.name);
            }
            else if (dataDiRiferimento == DateTime.MinValue)  //La MARCA
            {
                fileDoc.signatureResult = verifySignature.VerifyM7M(fileDoc.name);
                try
                {
                    // Rimozione del file firmato dalla cartella di input
                    File.Delete(inputFile);
                }
                catch { }
                if (fileDoc.signatureResult.StatusCode == 0) //Valido
                    retval = true;

                return retval;
            }
            else
            {
                if (fileDoc.name.ToUpper().EndsWith(".XML"))               
                    VerifyFileSignatureXADES(fileDoc, dataDiRiferimento.Value);                
                else
                {
                    fileDoc.signatureResult = verifySignature.Verify_External(fileDoc, dataDiRiferimento.Value);
                    //pezza verrastro
                    fileDoc.name = Path.GetFileNameWithoutExtension(fileDoc.name);
                }

                   
            }

            try
            {
                // Rimozione del file firmato dalla cartella di input
                File.Delete(inputFile);
            }
            catch
            {
            }
            string outputDirectory = verifySignature.GetPKCS7OutputDirectory();
            if (fileDoc.signatureResult.StatusCode == 0) //Valido
                retval = true;

            // Creazione cartella di appoggio nel caso non esista
            if (!System.IO.Directory.Exists(outputDirectory))
                System.IO.Directory.CreateDirectory(outputDirectory);

            logger.Debug("PKCS7OutputDirectory: " + outputDirectory);

            // Il valore di ritorno è 0 solamente se la firma del file è stata verificata
            string outputFileName = string.Empty;

            //INIZIO ABBATANGELI 
            //fileDoc.signatureResult.PKCS7Documents == null
            if (fileDoc.signatureResult.PKCS7Documents != null)
            {

                for (int i = 0; i < fileDoc.signatureResult.PKCS7Documents.Length; i++)
                {
                    // Ricerca nel file firmato del documento originale,
                    // che ha estensione != da ".P7M"
                    DocsPaVO.documento.PKCS7Document innerDocument = fileDoc.signatureResult.PKCS7Documents[i];

                    if (!string.IsNullOrEmpty(innerDocument.DocumentFileName) && !innerDocument.DocumentFileName.ToUpper().EndsWith(".P7M"))
                    {
                        fileDoc.name = innerDocument.DocumentFileName;
                        outputFileName = string.Concat(outputDirectory, fileDoc.name);
                        fileDoc.content = null;
                        // Lettura del contenuto del file originale
                        fileDoc.content = GetOutputFileContent(outputFileName);
                        fileDoc.length = fileDoc.content.Length;
                        fileDoc.contentType = getContentType(fileDoc.name);
                        fileDoc.fullName = string.Concat(fileDoc.path, '\u005C'.ToString(), fileDoc.name);

                        try
                        {
                            // Rimozione del file creato nella cartella di output
                            File.Delete(outputFileName);
                        }
                        catch
                        {
                        }

                        break;
                    }
                    innerDocument = null;
                }

            //FINE ABBATANGELI
            }

            verifySignature = null;

            if (dataDiRiferimento == null)
            {
                if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc))
                    BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.VerifyPadesSignature(fileDoc);
            }

            return retval;
        }

        public static VerifySignatureResult VerifyFileSignaturePades_External(DocsPaVO.documento.FileDocumento fileDoc, DateTime? dataDiRiferimento)
        {
            bool retval = false;
            VerifySignature verifySignature = new VerifySignature();
            VerifySignatureResult signResult;


            string inputDirectory = verifySignature.GetPKCS7InputDirectory();

            // Creazione cartella di appoggio nel caso non esista
            if (!System.IO.Directory.Exists(inputDirectory))
                System.IO.Directory.CreateDirectory(inputDirectory);

            logger.Debug("PKCS7InputDirectory: " + inputDirectory);

            string inputFile = string.Concat(inputDirectory, fileDoc.name);

            // Copia del file firmato dalla cartella del documentale
            // alla cartella di input utilizzata dal ws della verifica
            CopySignedFileToInputFolder(fileDoc, inputFile);
            try
            {
                signResult = verifySignature.Verify_External(fileDoc, dataDiRiferimento.Value);
                if (signResult != null && signResult.PKCS7Documents != null && signResult.PKCS7Documents.Count() > 0)
                {
                    signResult.PKCS7Documents[0].SignAlgorithm = "PADES" + signResult.PKCS7Documents[0].SignAlgorithm;
                    signResult.PKCS7Documents[0].SignatureType = DocsPaVO.documento.SignType.PADES;
                }
                else
                {
                    signResult = fileDoc.signatureResult;
                }
            }
            catch
            {
                // Rimozione del file firmato dalla cartella di input
                File.Delete(inputFile);
                signResult = fileDoc.signatureResult;
            }
            return signResult;
        }


        /// <summary>
        ///  Copia fisica del file firmato (.P7M) nella cartella in cui viene
        ///  fatta la verifica della firma digitale dal ws esterno
        /// </summary>
        /// <param name="inputFile"></param>
        private static void CopySignedFileToInputFolder(DocsPaVO.documento.FileDocumento fileDoc, string inputFile)
        {
            FileStream stream = new FileStream(inputFile, FileMode.Create, FileAccess.Write);
            stream.Write(fileDoc.content, 0, fileDoc.content.Length);
            stream.Flush();
            stream.Close();
            stream = null;
        }

        /// <summary>
        /// Lettura e restituzione del contenuto del file originale
        /// estratto dal file firmato (dalla cartella di output utilizzata
        /// dal ws esterno che effettua la verififica della firma digitale)
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        private static byte[] GetOutputFileContent(string outputFileName)
        {
            FileStream stream = new FileStream(outputFileName, FileMode.Open, FileAccess.Read);
            byte[] retValue = new byte[stream.Length];
            stream.Read(retValue, 0, retValue.Length);
            stream.Flush();
            stream.Close();
            stream = null;
            return retValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool verificaPath(string path)
        {
            char[] separator = { ';' };
            string pathString = "";

            // chiedere a Gennaro...
            if (System.Configuration.ConfigurationManager.AppSettings["verifica_path_file"] != null)
            {
                pathString = System.Configuration.ConfigurationManager.AppSettings["verifica_path_file"];

                if (pathString == null || pathString.Equals(""))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            logger.Debug(pathString);
            String[] pathFile = pathString.Split(separator);

            for (int i = 0; i < pathFile.Length; i++)
            {
                if (path.ToUpper().Equals(pathFile[i].ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string getContentType(string fileName)
        {
            string[] extArr = fileName.Split('.');
            string ext = extArr[extArr.Length - 1].ToLower();
            //string contentType = DocsPaUtils.Configuration.DocumentTypeManager.GetValue(ext);
            string contentType = null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            ArrayList contents = new ArrayList();
            doc.GetApplicazioni(ext, contents);
            if (contents != null && contents.Count > 0)
            {
                DocsPaVO.documento.Applicazione appl = (DocsPaVO.documento.Applicazione)contents[0];
                if (appl != null)
                {
                    contentType = appl.mimeType;
                }
            }

            if (contentType == null)
            {
                contentType = "application/x-" + ext;
            }

            return contentType;
        }
        #endregion

        #region Put File

        /// <summary>
        /// Codice ROBUSTO per Leggere porzioni di XML anche corrotte o non well formed.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private static string StrongXmlReadElementText(string element, byte[] metadata)
        {
            string xmlstring = System.Text.ASCIIEncoding.ASCII.GetString(metadata);
            int conf = xmlstring.IndexOf("<" + element);
            if (conf == -1)
                return null;
            using (MemoryStream xmlfile = new MemoryStream(metadata))
            {
                xmlfile.Position = conf;
                using (XmlTextReader xt = new XmlTextReader(xmlfile))
                {
                    xt.Namespaces = false;
                    try
                    {
                        //retry
                        int count = 0;
                        while (true)
                        {
                            count++;
                            xt.Read();
                            if (xt.NodeType == XmlNodeType.Text)
                                return xt.Value;

                            if (count > 100)
                                return null;
                        }
                    }
                    catch { return null; }
                }
            }
        }


        private static PdfInfo getPdfInfo(DocsPaVO.documento.FileDocumento fileDoc)
        {
            return getPdfInfo(fileDoc.content);
        }
  
        private static PdfInfo getPdfInfo(byte[] content)
        {
            PdfInfo retval = new PdfInfo();
            try
            {
                iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(content);
                retval.version = "1." + r.PdfVersion;
                iTextSharp.text.pdf.AcroFields af = r.AcroFields;
                if (!String.IsNullOrEmpty(r.JavaScript))
                    retval.HasJava = true;

                //get metadata per controllo conformità e PDFa


                if (r.Metadata != null)
                {


                    try
                    {

                        string part = StrongXmlReadElementText("pdfaid:part", r.Metadata);
                        if (!string.IsNullOrEmpty(part))
                        {
                            retval.IsPdfA = true;
                            string conf = StrongXmlReadElementText("pdfaid:conformance", r.Metadata);
                            if (!string.IsNullOrEmpty(conf))
                                retval.conformance = String.Format("PDF/A {0}{1}", part, conf);

                        }
                        string PdfE = StrongXmlReadElementText("pdfe:ISO_PDFEVersion", r.Metadata);
                        if (!string.IsNullOrEmpty(PdfE))
                            retval.conformance = PdfE;


                        string pdfX = StrongXmlReadElementText("pdfxid:GTS_PDFXVersion", r.Metadata);
                        if (!string.IsNullOrEmpty(pdfX))
                        {
                            retval.conformance = pdfX;
                            string pdfVT = StrongXmlReadElementText("pdfvtid:GTS_PDFVTVersion", r.Metadata);
                            retval.conformance += "|" + pdfVT;
                        }
                    }
                    catch
                    {

                    }
                }
                if (af != null)
                {
                    System.Collections.ArrayList sigs = af.GetSignatureNames();
                    //controllo se firmato
                    if (sigs.Count > 0)
                        retval.IsSigned = true;

                    //controllo dati biometrici e grafometrici
                    foreach (string name in sigs)
                    {
                        bool hasBio = af.GetSignatureDictionary(name).Contains(new iTextSharp.text.pdf.PdfName("Prop_BiometricData"));
                        if (hasBio)
                        {
                            retval.HasBiometricData = true;
                            break;
                        }
                    }
                }
                return retval;
            }
            catch
            {
                return retval;
            }
        }

        /*
        /// <summary>
        /// Ritorna true se in un file PDF sono presenti delle firme pades
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        private static bool IsPdfPades(DocsPaVO.documento.FileDocumento fileDoc)
        {
            try
            {
                int numSig = 0;
                iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(fileDoc.content);
                iTextSharp.text.pdf.AcroFields af = r.AcroFields;
                if (af != null)
                {
                    numSig = af.GetSignatureNames().Count;
                    if (numSig > 0)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="isProtocollo"></param>
        /// <param name="fileName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, bool isProtocollo, string fileName, out string errorMessage, byte[] fileContent)
        {
            errorMessage = string.Empty;
            bool accepted = false;

            if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                FileInfo fileInfo;
                try
                {
                    fileInfo = new FileInfo(fileName);
                }
                catch (PathTooLongException)
                {
                    string name = Path.GetFileName(fileName);
                    fileInfo = new FileInfo(name);
                }
                string extension = fileInfo.Extension.Replace(".", string.Empty);
                if (extension.Contains(" "))
                    extension = extension.Replace(" ", "");
                bool Validation = false;
                if (!string.IsNullOrEmpty(extension))
                {
                    // Reperimento di tutti i formati di file accettati dall'amministrazione
                    DocsPaVO.FormatiDocumento.SupportedFileType[] acceptedFormats = FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(infoUtente.idAmministrazione));

                    DocsPaVO.FormatiDocumento.SupportedFileType selectedFormat = null;

                    foreach (DocsPaVO.FormatiDocumento.SupportedFileType format in acceptedFormats)
                    {
                        accepted = (format.FileTypeUsed && format.FileExtension.ToUpper().Equals(extension.ToUpper()));

                        if (accepted)
                        {
                            Validation = format.FileTypeValidation;
                            selectedFormat = format;
                            break;
                        }
                    }

                    if (accepted)
                    {
                        //con la carta di identità del documento il controllo non è piu bloccante.
                        /*
                        if (Validation)
                        {
                            if (fileContent != null)
                            {
                                logger.Debug(String.Format("Validazione Start {0}, len {1}", fileName, fileContent.Length));
                                Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
                                string fileExtension = ff.FileType(fileContent);
                                logger.Debug(String.Format("Validazione End {0}, Declared :{1}  Found:{2}", fileName, extension, fileExtension));
                                //if (!extension.ToLower().Contains(fileExtension.ToLower()))
                                if (!fileExtension.ToLower().Contains(extension.ToLower()))
                                {
                                    errorMessage = string.Format(string.Format("Il formato file dichiarato '{0}' Non è quello riscontrato '{1}'", extension.ToLower(), fileExtension.ToLower()));
                                    logger.Debug(errorMessage);
                                    accepted = false;
                                }

                            }
                        }
                        */
                        // Verifica se il formato è valido rispetto al tipo documento 
                        if ((selectedFormat.DocumentType == DocsPaVO.FormatiDocumento.DocumentTypeEnum.Grigio && isProtocollo)
                            || (selectedFormat.DocumentType == DocsPaVO.FormatiDocumento.DocumentTypeEnum.Protocollo && !isProtocollo))
                        {
                            accepted = false;

                            errorMessage = string.Format("Il formato file '{0}' non è valido per un {1}", extension, (isProtocollo ? "protocollo" : "documento grigio"));
                        }
                    }
                    else
                    {
                        errorMessage = string.Format("Il formato file '{0}' non è tra quelli accettati in amministrazione", extension);
                    }
                }
            }
            else
            {
                accepted = true;
            }

            return accepted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <param name="fileName"></]param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, string fileName, out string errorMessage)
        {
            bool isProtocollo = (Documenti.DocManager.GetTipoDocumento(docNumber) != "G");

            return IsFileAccepted(infoUtente, isProtocollo, fileName, out errorMessage, null);
        }

        /// <summary>
        /// Verifica se il documento è tra quelli accettati dall'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="isProtocollo"></param>
        /// <param name="fileName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, bool isProtocollo, DocsPaVO.documento.FileDocumento fileDocument, out string errorMessage, bool checkSign = true)
        {
            string fileName = string.Empty;
            if (!string.IsNullOrEmpty(fileDocument.fullName))
                fileName = fileDocument.fullName;
            else
                fileName = fileDocument.name;
            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(fileName);
            }
            catch(PathTooLongException)
            {
                string name = Path.GetFileName(fileName);
                fileInfo = new FileInfo(name);
            }
            string extension = fileInfo.Extension.Replace(".", string.Empty);
            //è un p7m? lo dovrei sbustare...
            //se trovate bachi, corregeteli senza troppa esitazione...
            //30/11/2012 afaillace.. aggiunto blocco
            byte[] content;
            if (extension.ToUpper().EndsWith("P7M".ToUpper()))
            {
                //devo duplicare il filedocumento perchè non voglio modificare quello di input.
                DocsPaVO.documento.FileDocumento verFileDocument = new DocsPaVO.documento.FileDocumento();
                verFileDocument.content = fileDocument.content;
                verFileDocument.fullName = fileDocument.fullName;
                verFileDocument.name = fileDocument.name;
                try
                {
                    if (checkSign)
                    {
                        VerifyFileSignature(verFileDocument, null);
                        if (verFileDocument.signatureResult != null)
                        {

                            if (
                                (verFileDocument.signatureResult.ErrorMessages != null) &&
                                (verFileDocument.signatureResult.ErrorMessages.Length > 0)
                                )
                            {
                                string errRetval = "\r\n";
                                foreach (string s in verFileDocument.signatureResult.ErrorMessages)
                                    errRetval += s + " \r\n";

                                //throw new Exception("Il documento firmato ha i seguenti problemi:" + errRetval +"\r\n non è possobile verificarlo");
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    logger.Debug("Il documento firmato è danneggiato, non è più possibile verificarlo: " + ex.Message);
                }
                fileName = verFileDocument.fullName;
                content = verFileDocument.content;
            }
            else
            {
                content = fileDocument.content;
            }
            //FineBlocco


            return IsFileAccepted(infoUtente, isProtocollo, fileName, out errorMessage, content);
        }

        /// <summary>
        /// Verifica se il documento è tra quelli accettati dall'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docNumber"></param>
        /// <param name="fileDocument"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool IsFileAccepted(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, DocsPaVO.documento.FileDocumento fileDocument, out string errorMessage, bool checkSign = true)
        {
            bool isProtocollo = (Documenti.DocManager.GetTipoDocumento(docNumber) != "G");

            return IsFileAccepted(infoUtente, isProtocollo, fileDocument, out errorMessage, checkSign);
        }

        /// <summary>
        /// Verifica se il contenuto del file è conforme alla sua estenzione
        /// </summary>
        /// <param name="fileDocument"></param>
        /// <returns></returns>
        public static bool IsValidFileContent(DocsPaVO.documento.FileDocumento fileDocument)
        {
            bool isValid = false;

            string fileName = string.Empty;

            if (!string.IsNullOrEmpty(fileDocument.fullName))
                fileName = fileDocument.fullName;
            else
                fileName = fileDocument.name;

            logger.DebugFormat("Validazione Start {0}", fileName);
            Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
            string fileExtension = ff.FileType(fileDocument.content);

            string extension = System.IO.Path.GetExtension(fileName).Replace(".", string.Empty);

            //normalizzo l'estensine per file strani html /htm tif/tiff etc etc
            extension = normalizeDblExtensions(extension);

            logger.Debug(String.Format("Validazione End {0}, Declared :[{1}]  Found:[{2}]", fileName, extension, fileExtension));
            isValid = fileExtension.ToLower().Contains(extension.ToLower());

            return isValid;
        }

        /// <summary>
        /// Estrae le estensioni dal nome
        /// </summary>
        /// <param name="infile"></param>
        /// <returns></returns>
        static string getExts(string infile)
        {
            string fname = System.IO.Path.GetFileNameWithoutExtension(infile);

            if (fname.Contains('.'))
                fname = fname.Remove(fname.IndexOf('.'));

            string extname = infile.Replace(fname, string.Empty);
            return extname;
        }

        private static bool isScannedDocument(DocsPaVO.documento.FileDocumento fileDocumento)
        {
            //prima di tutto deve essere cartaceo
            if (!fileDocumento.cartaceo)
                return false;

            string fileName = fileDocumento.name;
            fileName = fileName.Replace(getExts(fileName), string.Empty);
            bool isScannedFileName = false;
            //poi il nome deve almeno avere un valore
            if (!string.IsNullOrEmpty(fileName))
            {
                //Testo il nome con il guid.tiff (nuova versione degli smart client)
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
                isScannedFileName = guidRegEx.IsMatch(fileName);
                if (isScannedFileName)
                    return true;

                //nel caso testo il pattern con la vecchia nomenclatura Activex dpascanXXXX.tif
                isScannedFileName = false;
                guidRegEx = new Regex(@"^(\{{0,1}dpascan([0-9]){4})$");
                isScannedFileName = guidRegEx.IsMatch(fileName);
                if (isScannedFileName)
                    return true;

                //Per i file provenienti da ACQ MASSIVA
                isScannedFileName = false;
                guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})_ACQMASSIVA$");
                isScannedFileName = guidRegEx.IsMatch(fileName);
                if (isScannedFileName)
                    return true;


            }
            return false;
        }

        /// <summary>
        /// FAILLAX 29 luglio 2013 (fa caldo)
        /// Funzioncina porcata che serve a fileFinder per normalizzare l'estensione di certi file con doppie estensioni
        /// come gli htm/html i tif/tiff, l'input viene confrontato con degli schemi classici e portati al default in lowercase
        /// </summary>
        /// <param name="inExt"></param>
        /// <returns></returns>
        public static string normalizeDblExtensions(string inExt)
        {
            if (inExt.ToLower() == "html")
            {
                logger.DebugFormat("{0} -> htm", inExt);
                return "htm";
            }
            if (inExt.ToLower() == "tif")
            {
                logger.DebugFormat("{0} -> tif", inExt);
                return "tiff";
            }
            if (inExt.ToLower() == "jpeg")
            {
                logger.DebugFormat("{0} -> jpg", inExt);
                return "jpg";
            }
            return inExt;
        }

        public static string removeIllegalChars(string filename, bool normalizeDotsAndSpacesToo)
        {

            if (string.IsNullOrEmpty(filename))
                return filename;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
                filename = filename.Replace(c.ToString(), "_");

            if (normalizeDotsAndSpacesToo)
            {
                filename = filename.Replace(".", "_");
                filename = filename.Replace(" ", "_");
            }
            return filename;
        }


        public static string Truncate(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static string setFileNameForScanneddocuments(DocsPaVO.utente.InfoUtente objSicurezza, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento)
        {
            string FileName = string.Empty;
            string ext = System.IO.Path.GetExtension(fileDocumento.name);
            DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglio(objSicurezza, fileRequest.docNumber, fileRequest.docNumber);
            DocsPaVO.documento.Allegato all = null;
            bool isallegato = isDocAllegato(sd, ref all);

            if (!isallegato)
            {
                FileName = "Documento_Principale_";
                if (sd.protocollo != null)
                    FileName += removeIllegalChars(sd.protocollo.segnatura, true) + ext;
                else
                    FileName += fileRequest.docNumber + ext;
            }
            else
            {  //è un allegato
                string descAllegato = Truncate(fileRequest.descrizione, 115);// tronco a 115
                // L'allegato è sempre un documento grigio
                FileName += removeIllegalChars(descAllegato, true) + "_" + fileRequest.docNumber + ext;
            }
            return FileName;

        }

        /// <summary>
        /// Legge un bytearray base64 (il content), e lo trasforma in un bytearray sbustato, eseguendo una lettura linea per linea (per evitare problemi di memoria)
        /// </summary>
        /// <param name="inBase64Bytes"> file base 64 in formato byte[]</param>
        /// <returns></returns>
        static byte[] readBase64(byte[] inBase64Bytes)
        {
            MemoryStream msIn = new MemoryStream(inBase64Bytes);
            MemoryStream msOut = new MemoryStream();
            StreamReader sr = new StreamReader(msIn);
            while (true)
            {
                string line = sr.ReadLine();
                if (String.IsNullOrEmpty(line))
                    break;

                byte[] temp = null;

                try
                {
                    temp = Convert.FromBase64String(line);
                }
                catch
                {
                    return null;
                }
                msOut.Write(temp, 0, temp.Length);
            }
            return msOut.ToArray();
        }

        private static string ConvertAccentedString(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Inserimento file nel documentale.
        /// Restituisce l'esito dell'operazione insieme 
        /// all'eventuale messaggio di errore.
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool putFile(ref DocsPaVO.documento.FileRequest fileRequest,
                                    DocsPaVO.documento.FileDocumento fileDoc,
                                    DocsPaVO.utente.InfoUtente objSicurezza,
                                    out string errorMessage, bool processFileInfo = true)
        {
            return putFile(ref fileRequest, fileDoc, objSicurezza, true, out errorMessage, processFileInfo);
        }

        public static bool putFileRicevuteIs(ref DocsPaVO.documento.FileRequest fileRequest,
                            DocsPaVO.documento.FileDocumento fileDoc,
                            DocsPaVO.utente.InfoUtente objSicurezza,
                            out string errorMessage)
        {
            return putFileRicevuteIs(ref fileRequest, fileDoc, objSicurezza, true, out errorMessage);
        }

        public static bool putFileRicevuteIs(ref DocsPaVO.documento.FileRequest fileRequest,
                            DocsPaVO.documento.FileDocumento fileDoc,
                            DocsPaVO.utente.InfoUtente objSicurezza,
                            bool verifyFileFormat,
                            out string errorMessage)
        {
            logger.Info("BEGIN");
            bool retValue = true;
            errorMessage = string.Empty;
            String tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
            bool signTypeChecked = false;

            fileRequest.fileName = fileDoc.name;

            //fix per il salvataggio nella components
            if (String.IsNullOrEmpty(fileDoc.fullName))
                fileDoc.fullName = fileDoc.name;

            // Verifica se il file è acquisito nell'ambito di un repository di sessione
            if (fileRequest.repositoryContext != null)
            {
                bool isProtocollo = false;

                // Determina la tipologia del documento 
                if (fileRequest is DocsPaVO.documento.Documento)
                    isProtocollo = (!fileRequest.repositoryContext.IsDocumentoGrigio);
                else if (fileRequest is DocsPaVO.documento.Allegato)
                    // L'allegato è sempre un documento grigio
                    isProtocollo = false;

                    SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(fileRequest.repositoryContext);

                    // Inserimento di un file in un documento quando ancora non è stato salvato,
                    // pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
                    fileManager.SetFile(fileRequest, fileDoc);

                    // Aggiornamento oggetto FileRequest
                    fileRequest.fileName = fileDoc.name;
                    fileRequest.fileSize = fileDoc.content.Length.ToString();
                    fileRequest.subVersion = "A";
                    fileRequest.path = fileDoc.path;
            }
            else
            {
                // Creazione contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    //controllo se il file è già stato acquistito per gestire la concorrenza
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);


                    if (string.IsNullOrEmpty(fileRequest.fileName))
                        fileRequest.fileName = fileDoc.name;

                    //string estensione = fileDoc.estensioneFile;
                    string estensione = "";
 
                    estensione = Path.GetExtension(fileDoc.name);
                    if (estensione.StartsWith("."))
                        estensione = estensione.Substring(1);

                    fileRequest.firmato = "0";
                    tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;

                    fileRequest.tipoFirma = tipoFirma;

                    if (estensione.IndexOf(".") != -1)
                    {
                        string[] extSplit = estensione.Split('.');
                        estensione = extSplit[extSplit.Length - 1];
                    }

                    //se nomeOriginale non è stato impostato lo metto io riprendendolo dal nome
                    if (String.IsNullOrEmpty(fileDoc.nomeOriginale))
                    {
                        fileDoc.nomeOriginale = fileDoc.name;
                    }

                    if (retValue)
                    {
  
                    if (!documentManager.PutFile(fileRequest, fileDoc, estensione))
                    {
                        retValue = false;
                        errorMessage = "Non è stato possibile acquisire il documento. Er su PF <BR><BR>Ripetere l'operazione di acquisizione.";
                        logger.Error(errorMessage.Replace("<BR", ""));
                    }
                       
                        documentManager = null;
                        if (retValue)
                            transactionContext.Complete();

                        // Ulteriore verifica tramite impronta della corretta acq
                        if (!doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId))
                        {
                            errorMessage = "Non è stato possibile acquisire il documento Er su CAF. <BR><BR>Ripetere l'operazione di acquisizione.";
                            logger.Error(errorMessage.Replace("<BR", ""));
                            //08-02-2016: Commentato il cestinaDocumento perchè se falisce l'acquisizione cestinava il protocollo
                            //doc.CestinaDocumento(objSicurezza.idPeople, fileRequest.docNumber, fileRequest.versionId);
                            retValue = false;
                        }
                    }
                }
            }
 
            logger.Info("END");

            //Funzioni Accessorie da effetuare una volta inserito il file.
            //se retvalue è false non vengono effettuate in quanto è fallito l'inserimento

            return retValue;
        }
        /// <summary>
        /// Inserimento file nel documentale.
        /// Restituisce l'esito dell'operazione insieme 
        /// all'eventuale messaggio di errore.
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="verifyFileFormat"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool putFile(ref DocsPaVO.documento.FileRequest fileRequest,
                                    DocsPaVO.documento.FileDocumento fileDoc,
                                    DocsPaVO.utente.InfoUtente objSicurezza,
                                    bool verifyFileFormat,
                                    out string errorMessage, bool processFileInfo = true)
        {
            logger.Info("BEGIN");
            bool retValue = true;
            errorMessage = string.Empty;
            String tipoFirma =  DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
            bool signTypeChecked = false;

            if (fileRequest.repositoryContext == null)
            {
                // Verifica stato di consolidamento del documento, solamente se non si sta acquisendo il file nel repository context
                retValue = DocumentConsolidation.CanExecuteAction(objSicurezza, fileRequest.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.ModifyVersions);
            }

            if (!retValue)
            {
                errorMessage = "Il documento risulta in stato consolidato, impossibile acquisire il file";
            }
            else
            {
                //controllo se doc in cestino
                string incestino = string.Empty;
                if (fileRequest != null && !string.IsNullOrEmpty(fileRequest.docNumber))
                    incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(fileRequest.docNumber);

                //Verifico se non posso acquisire perchè è attivo un processo di firma per il documento principale o allegato e non sono il titolare del passo in esecuzione
                if (!LibroFirma.LibroFirmaManager.CanExecuteAction(fileRequest, objSicurezza))
                {
                    errorMessage = "il documento principale o l'allegato è in libro firma.";
                    throw new Exception("Non è possibile acquisire poichè il documento principale o l'allegato è in libro firma");
                }

                if (!string.IsNullOrEmpty(incestino) && incestino == "1")
                    throw new Exception("Il documento è stato rimosso, non è più possibile modificarlo");

                if (retValue)
                {
                    //tolgo nel nome del file i caratteri accentati
                    string nameWithoutAccentedString = ConvertAccentedString(fileDoc.name);
                    fileDoc.name = nameWithoutAccentedString;
                    fileRequest.fileName = nameWithoutAccentedString;

                    //fix per il salvataggio nella components
                    if (String.IsNullOrEmpty(fileDoc.fullName))
                        fileDoc.fullName = fileDoc.name;


                    //Gestione base64
                    if (fileDoc.name.ToUpper().EndsWith("P7M"))
                    {
                        byte[] deb64Content = readBase64(fileDoc.content);
                        if (deb64Content != null)
                        {
                            fileDoc.content = deb64Content;
                            fileDoc.length = fileDoc.content.Length;
                        }
                    }

                    // Verifica se il file è acquisito nell'ambito di un repository di sessione
                    if (fileRequest.repositoryContext != null)
                    {
                        bool isProtocollo = false;

                        // Determina la tipologia del documento 
                        if (fileRequest is DocsPaVO.documento.Documento)
                            isProtocollo = (!fileRequest.repositoryContext.IsDocumentoGrigio);
                        else if (fileRequest is DocsPaVO.documento.Allegato)
                            // L'allegato è sempre un documento grigio
                            isProtocollo = false;

                        // Verifica se il formato documento è tra quelli accettati dall'amministrazione
                        if (verifyFileFormat)
                            retValue = IsFileAccepted(objSicurezza, isProtocollo, fileDoc, out errorMessage, processFileInfo);

                        if (retValue)
                        {
                            SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(fileRequest.repositoryContext);

                            // Inserimento di un file in un documento quando ancora non è stato salvato,
                            // pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
                            fileManager.SetFile(fileRequest, fileDoc);

                            // Aggiornamento oggetto FileRequest
                            fileRequest.fileName = fileDoc.name;
                            fileRequest.fileSize = fileDoc.content.Length.ToString();
                            fileRequest.subVersion = "A";
                            fileRequest.path = fileDoc.path;
                        }
                    }
                    else
                    {
                        bool scanned = isScannedDocument(fileDoc);
                        if (scanned)
                            fileDoc.name = setFileNameForScanneddocuments(objSicurezza, fileRequest, fileDoc);

                        // Verifica se il formato documento è tra quelli accettati dall'amministrazione
                        if (verifyFileFormat)
                            retValue = IsFileAccepted(objSicurezza, fileRequest.docNumber, fileDoc, out errorMessage, processFileInfo);

                        if (retValue)
                        {
                            // Creazione contesto transazionale
                            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                            {
                                //controllo se il file è già stato acquistito per gestire la concorrenza
                                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                                //la stringa isFilePresent vale:
                                //"1" se il file è stato già acquisito;
                                //"0" se il file ancora non è stato acquisito
                                bool isFilePresent = doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId);

                                if (isFilePresent)
                                {
                                    errorMessage = "Impossibile acquisire il file perchè risulta già acquisito";
                                    retValue = false;
                                }
                                else if (fileDoc.content.Length == 0)
                                {
                                    errorMessage = "Impossibile acquisire il file perchè è di 0 byte";
                                    retValue = false;
                                }
                                else
                                {
                                    DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);

                                    if (GestioneTSAttacced)
                                    {
                                        //Gestione timestamped file
                                        if ((fileDoc.name.ToUpper().EndsWith("M7M")) ||
                                            (fileDoc.name.ToUpper().EndsWith("TSD")))
                                        {
                                            string extension = fileDoc.name.ToUpper();
                                            if (extension.EndsWith("M7M"))
                                            {
                                                //GestioneM7M(fileRequest, fileDoc, objSicurezza);
                                                DigitalSignature.PKCS_Utils.m7m m7mhandler = new DigitalSignature.PKCS_Utils.m7m();
                                                m7mhandler.explode(fileDoc.content);
                                                AggiuntaEVerificaMarca(fileRequest, objSicurezza, m7mhandler.Data.Content, m7mhandler.TSR);
                                            }

                                            if (extension.EndsWith("TSD"))
                                            {
                                                //GestioneTSD(fileRequest, fileDoc, objSicurezza);

                                                DigitalSignature.PKCS_Utils.tsd tsdhandler = new DigitalSignature.PKCS_Utils.tsd();
                                                tsdhandler.explode(fileDoc.content);
                                                AggiuntaEVerificaMarca(fileRequest, objSicurezza, tsdhandler.Data.Content, tsdhandler.TSR);


                                            }

                                        }
                                    }


                                    if (string.IsNullOrEmpty(fileRequest.fileName))
                                        fileRequest.fileName = fileDoc.name;

                                    //string estensione = fileDoc.estensioneFile;
                                    string estensione = "";
                                    if (string.IsNullOrEmpty(estensione))
                                    {
                                        if (
                                            fileDoc.name.ToUpper().EndsWith("P7M") ||
                                            fileDoc.name.ToUpper().EndsWith("TSD") ||
                                            fileDoc.name.ToUpper().EndsWith("M7M")
                                            )
                                        {
                                            estensione = fileDoc.name.Substring(fileDoc.name.IndexOf(".") + 1);
                                        }
                                        else
                                        {
                                            estensione = Path.GetExtension(fileDoc.name);
                                            if (estensione.StartsWith("."))
                                                estensione = estensione.Substring(1);
                                        }
                                    }

                                    if (
                                        fileDoc.name.ToUpper().EndsWith("P7M") ||
                                        fileDoc.name.ToUpper().EndsWith("TSD") ||
                                        fileDoc.name.ToUpper().EndsWith("M7M") ||
                                        fileRequest.firmato == "1"
                                        )
                                    {
                                        while (
                                            fileDoc.name.ToUpper().EndsWith("P7M") ||
                                            fileDoc.name.ToUpper().EndsWith("TSD") ||
                                            fileDoc.name.ToUpper().EndsWith("M7M")
                                            )
                                        {
                                            if (estensione.LastIndexOf(".") > -1)
                                            {
                                                // Mod. Lembo per ticket: while breaker
                                                if (!estensione.ToUpper().EndsWith("P7M") &&
                                                    !estensione.ToUpper().EndsWith("M7M") &&
                                                    !estensione.ToUpper().EndsWith("TSD"))
                                                    break;

                                                estensione = estensione.Remove(estensione.LastIndexOf("."));
                                                if (estensione.ToUpper().EndsWith("P7M"))
                                                {
                                                    fileRequest.firmato = "1";
                                                    tipoFirma = DocsPaVO.documento.TipoFirma.CADES;
                                                    signTypeChecked = true;
                                                }
                                                if (estensione.ToUpper().EndsWith("TSD"))
                                                {
                                                    tipoFirma = DocsPaVO.documento.TipoFirma.TSD;
                                                    signTypeChecked = true;
                                                }
                                            }
                                            else
                                                break;
                                        }
                                        //LULUCIANI: PUò ACCADERE CHE IL NOME DEL FILE CONTENGA "." QUESTO FA Sì CHE L'ESTENSIONE 
                                        //RISULTI SPORCA, PER EVITARE CIò ALLA FINE DEL PRECDENTE whilE CHIAMO COMUNQUE IL METODO

                                        string estensione2 = Path.GetExtension(estensione);
                                        if (!string.IsNullOrEmpty(estensione2))
                                            estensione = estensione2;


                                        if (estensione.StartsWith("."))
                                            estensione = estensione.Substring(1);

                                        if (fileDoc.name.ToUpper().EndsWith("P7M"))
                                        {
                                            fileRequest.firmato = "1";
                                            tipoFirma = DocsPaVO.documento.TipoFirma.CADES;
                                            signTypeChecked = true;
                                        }
                                    }
                                    //In caso di XML, verifico se è firmato XADES
                                    else if (estensione.ToUpper().EndsWith("XML") && IsSignedXades(fileDoc))
                                    {
                                        fileRequest.firmato = "1";
                                        tipoFirma = DocsPaVO.documento.TipoFirma.XADES.ToString();
                                        signTypeChecked = true;
                                    }
                                    else
                                    {
                                        fileRequest.firmato = "0";
                                        tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
                                    }


                                    //test se pades
                                    if (GestionePades)
                                    {
                                        if (estensione.ToUpper().EndsWith("PDF"))
                                        {
                                            if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc))
                                            {
                                                fileRequest.firmato = "1";
                                                if (!signTypeChecked)
                                                    tipoFirma = DocsPaVO.documento.TipoFirma.PADES;
                                            }
                                            else //Verifico la firma pades con ASPOSE
                                            {
                                                if(IsSignedPades(fileDoc))
                                                {
                                                    fileRequest.firmato = "1";
                                                    if (!signTypeChecked)
                                                        tipoFirma = DocsPaVO.documento.TipoFirma.PADES;
                                                }
                                            }
                                        }
                                    }

                                    if (fileRequest.tipoFirma == DocsPaVO.documento.TipoFirma.ELETTORNICA)
                                    {
                                        switch (tipoFirma)
                                        {
                                            case (DocsPaVO.documento.TipoFirma.CADES):
                                                tipoFirma = DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA;
                                                break;
                                            case (DocsPaVO.documento.TipoFirma.XADES):
                                                tipoFirma = DocsPaVO.documento.TipoFirma.PADES_ELETTORNICA;
                                                break;
                                            case (DocsPaVO.documento.TipoFirma.TSD):
                                                tipoFirma = DocsPaVO.documento.TipoFirma.PADES_ELETTORNICA;
                                                break;
                                            case (DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA):
                                                tipoFirma = DocsPaVO.documento.TipoFirma.ELETTORNICA;
                                                break;
                                        }
                                    }
                                    fileRequest.tipoFirma = tipoFirma;

                                    if (estensione.IndexOf(".") != -1)
                                    {
                                        string[] extSplit = estensione.Split('.');
                                        estensione = extSplit[extSplit.Length - 1];
                                    }

                                    //se nomeOriginale non è stato impostato lo metto io riprendendolo dal nome
                                    if (String.IsNullOrEmpty(fileDoc.nomeOriginale))
                                    {
                                        fileDoc.nomeOriginale = fileDoc.name;
                                    }

                                    if (retValue)
                                    {
                                        bool cacheAttivo= CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione);
                                        if (cacheAttivo)
                                        {
                                            logger.Debug("eseguo il putfile della cache");
                                            if (!CacheFileManager.PutFile(objSicurezza, fileRequest, fileDoc, estensione, out errorMessage))
                                            {
                                                if (errorMessage == string.Empty)
                                                    errorMessage = "Non è stato possibile acquisire il documento. Er su CM <BR><BR>Ripetere l'operazione di acquisizione.";

                                                retValue = false;
                                            }
                                        }
                                        else
                                        {
                                            if (!documentManager.PutFile(fileRequest, fileDoc, estensione))
                                            {
                                                retValue = false;
                                                errorMessage = "Non è stato possibile acquisire il documento. Er su PF <BR><BR>Ripetere l'operazione di acquisizione.";
                                                logger.Error(errorMessage.Replace ("<BR",""));
                                            }
                                            else
                                            {
                                                DocsPaDocumentale.Documentale.FullTextSearchManager fullTextManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(objSicurezza);
                                                retValue = fullTextManager.SetDocumentAsIndexed(fileRequest.docNumber);

                                                if (!retValue)
                                                {
                                                    errorMessage = "Non è stato possibile acquisire il documento. Er su Idx <BR><BR>Ripetere l'operazione di acquisizione.";
                                                    logger.Error(errorMessage.Replace("<BR", ""));
                                                }
                                            }
                                          }
                                        documentManager = null;

                                        if (retValue)
                                            transactionContext.Complete();

                                        if (!cacheAttivo)
                                        {
                                            // Ulteriore verifica tramite impronta della corretta acq
                                            if (!doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId))
                                            {
                                                errorMessage = "Non è stato possibile acquisire il documento Er su CAF. <BR><BR>Ripetere l'operazione di acquisizione.";
                                                logger.Error(errorMessage.Replace("<BR", ""));
                                                //08-02-2016: Commentato il cestinaDocumento perchè se falisce l'acquisizione cestinava il protocollo
                                                //doc.CestinaDocumento(objSicurezza.idPeople, fileRequest.docNumber, fileRequest.versionId);
                                                retValue = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            logger.Info("END");
            
            //Funzioni Accessorie da effetuare una volta inserito il file.
            //se retvalue è false non vengono effettuate in quanto è fallito l'inserimento
            if (retValue)
            {
                //Crea il fileInfo per la carta di identità del documento.
                if (processFileInfo)
                    processFileInformation(fileRequest, objSicurezza);

                //estrae dalla fattura PA gli eventuali allegati contenuti nell'XML e li inserisce come allegati del documento
                addAllegatiFatturaPA(fileRequest, fileDoc, objSicurezza);

                //Inserisce i file nell'aera di spool per l'indicizzazione
                sendFileToIndexer(fileRequest, fileDoc, objSicurezza);
            }
            return retValue;
        }

        public static bool putFileBypassaControlli(ref DocsPaVO.documento.FileRequest fileRequest,
                                    DocsPaVO.documento.FileDocumento fileDoc,
                                    DocsPaVO.utente.InfoUtente objSicurezza,
                                    bool verifyFileFormat,
                                    out string errorMessage, bool processFileInfo = true)
        {
            logger.Info("BEGIN");
            bool retValue = true;
            errorMessage = string.Empty;
            String tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
            bool signTypeChecked = false;


            if (retValue)
            {
                //tolgo nel nome del file i caratteri accentati
                string nameWithoutAccentedString = ConvertAccentedString(fileDoc.name);
                fileDoc.name = nameWithoutAccentedString;
                fileRequest.fileName = nameWithoutAccentedString;

                //fix per il salvataggio nella components
                if (String.IsNullOrEmpty(fileDoc.fullName))
                    fileDoc.fullName = fileDoc.name;
                //Gestione base64
                if (fileDoc.name.ToUpper().EndsWith("P7M"))
                {
                    byte[] deb64Content = readBase64(fileDoc.content);
                    if (deb64Content != null)
                    {
                        fileDoc.content = deb64Content;
                        fileDoc.length = fileDoc.content.Length;
                    }
                }
                // Verifica se il file è acquisito nell'ambito di un repository di sessione
                if (fileRequest.repositoryContext != null)
                {
                    bool isProtocollo = false;

                    // Determina la tipologia del documento 
                    if (fileRequest is DocsPaVO.documento.Documento)
                        isProtocollo = (!fileRequest.repositoryContext.IsDocumentoGrigio);
                    else if (fileRequest is DocsPaVO.documento.Allegato)
                        // L'allegato è sempre un documento grigio
                        isProtocollo = false;
                    // Verifica se il formato documento è tra quelli accettati dall'amministrazione
                    if (verifyFileFormat)
                        retValue = IsFileAccepted(objSicurezza, isProtocollo, fileDoc, out errorMessage, processFileInfo);
                    if (retValue)
                    {
                        SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(fileRequest.repositoryContext);

                        // Inserimento di un file in un documento quando ancora non è stato salvato,
                        // pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
                        fileManager.SetFile(fileRequest, fileDoc);
                        // Aggiornamento oggetto FileRequest
                        fileRequest.fileName = fileDoc.name;
                        fileRequest.fileSize = fileDoc.content.Length.ToString();
                        fileRequest.subVersion = "A";
                        fileRequest.path = fileDoc.path;
                    }
                }
                else
                {
                    bool scanned = isScannedDocument(fileDoc);
                    if (scanned)
                        fileDoc.name = setFileNameForScanneddocuments(objSicurezza, fileRequest, fileDoc);

                    // Verifica se il formato documento è tra quelli accettati dall'amministrazione
                    if (verifyFileFormat)
                        retValue = IsFileAccepted(objSicurezza, fileRequest.docNumber, fileDoc, out errorMessage, processFileInfo);
                    if (retValue)
                    {
                        // Creazione contesto transazionale
                        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                        {
                            //controllo se il file è già stato acquistito per gestire la concorrenza
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                            //la stringa isFilePresent vale:
                            //"1" se il file è stato già acquisito;
                            //"0" se il file ancora non è stato acquisito
                            bool isFilePresent = doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId);
                            if (isFilePresent)
                            {
                                errorMessage = "Impossibile acquisire il file perchè risulta già acquisito";
                                retValue = false;
                            }
                            else if (fileDoc.content.Length == 0)
                            {
                                errorMessage = "Impossibile acquisire il file perchè è di 0 byte";
                                retValue = false;
                            }
                            else
                            {
                                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
                                if (GestioneTSAttacced)
                                {
                                    //Gestione timestamped file
                                    if ((fileDoc.name.ToUpper().EndsWith("M7M")) ||
                                        (fileDoc.name.ToUpper().EndsWith("TSD")))
                                    {
                                        string extension = fileDoc.name.ToUpper();
                                        if (extension.EndsWith("M7M"))
                                        {
                                            //GestioneM7M(fileRequest, fileDoc, objSicurezza);
                                            DigitalSignature.PKCS_Utils.m7m m7mhandler = new DigitalSignature.PKCS_Utils.m7m();
                                            m7mhandler.explode(fileDoc.content);
                                            AggiuntaEVerificaMarca(fileRequest, objSicurezza, m7mhandler.Data.Content, m7mhandler.TSR);
                                        }

                                        if (extension.EndsWith("TSD"))
                                        {
                                            //GestioneTSD(fileRequest, fileDoc, objSicurezza);

                                            DigitalSignature.PKCS_Utils.tsd tsdhandler = new DigitalSignature.PKCS_Utils.tsd();
                                            tsdhandler.explode(fileDoc.content);
                                            AggiuntaEVerificaMarca(fileRequest, objSicurezza, tsdhandler.Data.Content, tsdhandler.TSR);
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(fileRequest.fileName))
                                    fileRequest.fileName = fileDoc.name;
                                string estensione = "";
                                if (string.IsNullOrEmpty(estensione))
                                {
                                    if (
                                        fileDoc.name.ToUpper().EndsWith("P7M") ||
                                        fileDoc.name.ToUpper().EndsWith("TSD") ||
                                        fileDoc.name.ToUpper().EndsWith("M7M")
                                        )
                                    {
                                        estensione = fileDoc.name.Substring(fileDoc.name.IndexOf(".") + 1);
                                    }
                                    else
                                    {
                                        estensione = Path.GetExtension(fileDoc.name);
                                        if (estensione.StartsWith("."))
                                            estensione = estensione.Substring(1);
                                    }
                                }

                                if (
                                    fileDoc.name.ToUpper().EndsWith("P7M") ||
                                    fileDoc.name.ToUpper().EndsWith("TSD") ||
                                    fileDoc.name.ToUpper().EndsWith("M7M") ||
                                    fileRequest.firmato == "1"
                                    )
                                {
                                    while (
                                        fileDoc.name.ToUpper().EndsWith("P7M") ||
                                        fileDoc.name.ToUpper().EndsWith("TSD") ||
                                        fileDoc.name.ToUpper().EndsWith("M7M")
                                        )
                                    {
                                        if (estensione.LastIndexOf(".") > -1)
                                        {
                                            // Mod. Lembo per ticket: while breaker
                                            if (!estensione.ToUpper().EndsWith("P7M") &&
                                                !estensione.ToUpper().EndsWith("M7M") &&
                                                !estensione.ToUpper().EndsWith("TSD"))
                                                break;

                                            estensione = estensione.Remove(estensione.LastIndexOf("."));
                                            if (estensione.ToUpper().EndsWith("P7M"))
                                            {
                                                fileRequest.firmato = "1";
                                                tipoFirma = DocsPaVO.documento.TipoFirma.CADES;
                                                signTypeChecked = true;
                                            }
                                            if (estensione.ToUpper().EndsWith("TSD"))
                                            {
                                                tipoFirma = DocsPaVO.documento.TipoFirma.TSD;
                                                signTypeChecked = true;
                                            }
                                        }
                                        else
                                            break;
                                    }
                                    string estensione2 = Path.GetExtension(estensione);
                                    if (!string.IsNullOrEmpty(estensione2))
                                        estensione = estensione2;
                                    if (estensione.StartsWith("."))
                                        estensione = estensione.Substring(1);
                                    if (fileDoc.name.ToUpper().EndsWith("P7M"))
                                    {
                                        fileRequest.firmato = "1";
                                        tipoFirma = DocsPaVO.documento.TipoFirma.CADES;
                                        signTypeChecked = true;
                                    }
                                }
                                //In caso di XML, verifico se è firmato XADES
                                else if (estensione.ToUpper().EndsWith("XML") && IsSignedXades(fileDoc))
                                {
                                    fileRequest.firmato = "1";
                                    tipoFirma = DocsPaVO.documento.TipoFirma.XADES.ToString();
                                    signTypeChecked = true;
                                }
                                else
                                {
                                    fileRequest.firmato = "0";
                                    tipoFirma = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;
                                }
                                //test se pades
                                if (GestionePades)
                                {
                                    if (estensione.ToUpper().EndsWith("PDF"))
                                    {
                                        if (BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDoc))
                                        {
                                            fileRequest.firmato = "1";
                                            if (!signTypeChecked)
                                                tipoFirma = DocsPaVO.documento.TipoFirma.PADES;
                                        }
                                    }
                                }

                                if (fileRequest.tipoFirma == DocsPaVO.documento.TipoFirma.ELETTORNICA)
                                {
                                    switch (tipoFirma)
                                    {
                                        case (DocsPaVO.documento.TipoFirma.CADES):
                                            tipoFirma = DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA;
                                            break;
                                        case (DocsPaVO.documento.TipoFirma.XADES):
                                            tipoFirma = DocsPaVO.documento.TipoFirma.PADES_ELETTORNICA;
                                            break;
                                        case (DocsPaVO.documento.TipoFirma.TSD):
                                            tipoFirma = DocsPaVO.documento.TipoFirma.PADES_ELETTORNICA;
                                            break;
                                        case (DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA):
                                            tipoFirma = DocsPaVO.documento.TipoFirma.ELETTORNICA;
                                            break;
                                    }
                                }
                                fileRequest.tipoFirma = tipoFirma;

                                if (estensione.IndexOf(".") != -1)
                                {
                                    string[] extSplit = estensione.Split('.');
                                    estensione = extSplit[extSplit.Length - 1];
                                }

                                //se nomeOriginale non è stato impostato lo metto io riprendendolo dal nome
                                if (String.IsNullOrEmpty(fileDoc.nomeOriginale))
                                {
                                    fileDoc.nomeOriginale = fileDoc.name;
                                }

                                if (retValue)
                                {
                                    bool cacheAttivo = CacheFileManager.isActiveCaching(objSicurezza.idAmministrazione);
                                    if (cacheAttivo)
                                    {
                                        logger.Debug("eseguo il putfile della cache");
                                        if (!CacheFileManager.PutFile(objSicurezza, fileRequest, fileDoc, estensione, out errorMessage))
                                        {
                                            if (errorMessage == string.Empty)
                                                errorMessage = "Non è stato possibile acquisire il documento. Er su CM <BR><BR>Ripetere l'operazione di acquisizione.";

                                            retValue = false;
                                        }
                                    }
                                    else
                                    {
                                        if (!documentManager.PutFile(fileRequest, fileDoc, estensione))
                                        {
                                            retValue = false;
                                            errorMessage = "Non è stato possibile acquisire il documento. Er su PF <BR><BR>Ripetere l'operazione di acquisizione.";
                                            logger.Error(errorMessage.Replace("<BR", ""));
                                        }
                                        else
                                        {
                                            DocsPaDocumentale.Documentale.FullTextSearchManager fullTextManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(objSicurezza);
                                            retValue = fullTextManager.SetDocumentAsIndexed(fileRequest.docNumber);

                                            if (!retValue)
                                            {
                                                errorMessage = "Non è stato possibile acquisire il documento. Er su Idx <BR><BR>Ripetere l'operazione di acquisizione.";
                                                logger.Error(errorMessage.Replace("<BR", ""));
                                            }
                                        }
                                    }
                                    documentManager = null;

                                    if (retValue)
                                        transactionContext.Complete();

                                    if (!cacheAttivo)
                                    {
                                        // Ulteriore verifica tramite impronta della corretta acq
                                        if (!doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId))
                                        {
                                            errorMessage = "Non è stato possibile acquisire il documento Er su CAF. <BR><BR>Ripetere l'operazione di acquisizione.";
                                            logger.Error(errorMessage.Replace("<BR", ""));
                                            retValue = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            logger.Info("END");
            if (retValue)
            {
                //Crea il fileInfo per la carta di identità del documento.
                if (processFileInfo)
                    processFileInformation(fileRequest, objSicurezza);

                //estrae dalla fattura PA gli eventuali allegati contenuti nell'XML e li inserisce come allegati del documento
                addAllegatiFatturaPA(fileRequest, fileDoc, objSicurezza);

                //Inserisce i file nell'aera di spool per l'indicizzazione
                sendFileToIndexer(fileRequest, fileDoc, objSicurezza);
            }
            return retValue;
        }
        
        private static bool IsSignedPades(FileDocumento fileDoc)
        {
            bool result = false;
            try
            {
                ConvertEngineWS converter = new ConvertEngineWS();
                string converterEngineUrl = System.Configuration.ConfigurationManager.AppSettings["INLINE_CONVERTER_URL"];
                if(!string.IsNullOrEmpty(converterEngineUrl))
                {
                    // Impostazione dell'url del convertitore
                    converter.Url = converterEngineUrl;
                    try
                    {
                        // Conversione del file
                        result = converter.IsPdfPades(fileDoc.content);
                    }
                    catch (Exception e)
                    {
                        // Recupero dell'eccezione originale e sua scrittura nel log
                        ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);

                        logger.Debug("Eccezione durante la conversione con il motore esterno.", originalException);

                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in IsSignedPadesAspose: " + e.Message);
                result = false;
            }

            return result;
        }

        private static bool IsSignedXades(FileDocumento fileDoc)
        {
            bool result = false;
            XmlDocument Xmlfile = new XmlDocument();
            XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.content));
            tr.XmlResolver = null;
            try
            {
                Xmlfile.Load(tr);
                XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
                if (signature != null && signature.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel metodo IsSignedXades " + e.Message);
                return false;
            }
            finally
            {
                tr.Close();
            }

            return result;
        }

        private static void sendFileToIndexer(FileRequest fileRequest, FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            logger.Debug("Start");
 
            string folder = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_INDEXER_SUBMIT_FOLDER");
            if (string.IsNullOrEmpty(folder))
                return;


            //scrivo su file solo se esiste il content
            if (fileDoc.content != null)
            {
                try
                {
                    Uri u = new Uri(folder);

                    if (u.Scheme.ToLower() == "file") //nel caso fosse una cartella locale
                    {
                        if (!string.IsNullOrEmpty(u.LocalPath))
                        {
                            string lpath = u.LocalPath;
                            if (!Directory.Exists(lpath))
                                Directory.CreateDirectory(lpath);

                            // Creazione nome file
                            string fileName = string.Empty;
                            string estensione = Path.GetExtension(fileDoc.nomeOriginale);

                            if (!string.IsNullOrEmpty(fileRequest.fileName))
                            {
                                fileName = fileRequest.fileName;

                                string extensions = string.Empty;

                                while (!string.IsNullOrEmpty(Path.GetExtension(fileName)))
                                {
                                    extensions = Path.GetExtension(fileName) + extensions;

                                    fileName = Path.GetFileNameWithoutExtension(fileName);
                                }

                                fileName = string.Concat(fileRequest.versionId, extensions);
                            }
                            else
                                fileName = string.Format("{0}.{1}", fileRequest.versionId, estensione);


                            if (!string.IsNullOrEmpty ( fileRequest.versionId))
                                File.WriteAllBytes(Path.Combine(lpath, fileName), fileDoc.content);

                        }
                    }
                    else if (u.Scheme.ToLower() == "svc")   //nel caso fosse un servizio svc
                    {
                        throw new NotImplementedException("Funzionalità di invio a WS non implmentata");
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Errore, copiando il file per l'indicizzazione ", e);
                }
            }
        }

        private static void addAllegatiFatturaPA(FileRequest fileRequest, FileDocumento fileDoc,  DocsPaVO.utente.InfoUtente objSicurezza)
        {
            logger.Debug("Start");
         
            bool disabled = true;
            string config = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROCESS_FATTURAPA");
            if (!string.IsNullOrEmpty(config))
                if ((config == "1") || (config.ToLower() == "true"))
                    disabled = false;
            if (disabled)
                return;
          

            XmlParsing.FatturaPA.FatturaPAManager pam = new XmlParsing.FatturaPA.FatturaPAManager();
            try
            {

                byte[] content = fileDoc.content;
                if (fileDoc.name.ToUpper().EndsWith("P7M"))
                { //gestire il file p7m  .. nel caso la fattura fosse in un container P7m , sbusto all'infinito fino a che non arrivo al content vero e proprio
                    do 
                    {
                        try
                        {
                            content = Documenti.DigitalSignature.PKCS_Utils.Pkcs.extractSignedContent(content);
                        }
                        catch  //mi sta dando ecccezione, vuol dire che sono arrivato al file.. esco dal ciclo..
                        {
                            break;
                        }
                    } while (true);

                }

                //prima di tutto controlliamo se è una fattura PA
                if (pam.isFatturaPA(content))
                {
                    logger.Debug("L'allegato risulta essere una fattura PA");
                    string docnumPrinc = string.Empty;
                    //reperisco una scheda documento dal documumber del filerequest
                    SchedaDocumento sdFile = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(objSicurezza, fileRequest.docNumber, fileRequest.docNumber);
                    
                    //Verifico se la scheda documento si riferisce al documento principale o ad un suo eventuale allegato..
                    // se fosse il documento principale reperisco dalle l'informazione ricavata il docnumber che in ogni modo dovrà essere lo stesso del filerequest
                    if (sdFile.documentoPrincipale == null) //doc principale
                    {
                        docnumPrinc = sdFile.docNumber;
                    }
                    else //Allegato
                    {
                        //nel caso fosse un allegato prendo il documber dalla struttura docummento principale e da li reperisco la scheda documento del documento principale.
                        docnumPrinc = sdFile.documentoPrincipale.docNumber;
                        sdFile = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(objSicurezza, docnumPrinc, docnumPrinc);
                    }


                    //Questa chiamata estrae alla fattura PA gli eventuali allegati..
                    XmlParsing.FatturaPA.FatturaPAManager.allegati[] allegatiFatturaPA = pam.getAllegatiFromFatturaPA(content);

                    //nel caso non ci fossero allegati o la chiamata fosse fallita il risultato sarà null quindi non è necessario proseguire oltre.
                    if (allegatiFatturaPA != null)
                    {

                        List<String> hashAllegati = new List<string>();
                        //Mi ciclo gli allegati della scheda doumento per salvarmi gli hash, ma lo faccio solo se ho realmente degli allegati in SD
                        ArrayList allArray = BusinessLogic.Documenti.AllegatiManager.getAllegati(docnumPrinc, string.Empty);
                        if (allArray.Count > 0)
                        {
                            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
                            {
                                foreach (DocsPaVO.documento.Allegato all in allArray)
                                {
                                    string imp = "";
                                    doc.GetImpronta(out imp, all.versionId, all.docNumber);
                                    hashAllegati.Add(imp.ToLower());
                                }
                            }
                        }

                        //Per ogni allegato in fattura, controllo se quell'HASH non sia già presente nella mia lista.. se è presente annullo il contenutoAttachment facendo 
                        //saltare l'aquisizione e l'aggiunta dell'allegato.
                        if (allArray.Count > 0)
                        {
                            foreach (XmlParsing.FatturaPA.FatturaPAManager.allegati allPA in allegatiFatturaPA)
                            {
                                string hashAllegato = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(allPA.contenutoAttachment);
                                if (hashAllegati.Contains(hashAllegato.ToLower()))
                                    allPA.contenutoAttachment = null;
                            }
                        }

                        int contatore = 0;
                        string nomeFattura = fileDoc.nomeOriginale;
                        if (!string.IsNullOrEmpty(nomeFattura))
                            nomeFattura = Path.GetFileNameWithoutExtension(nomeFattura);
                        else
                            nomeFattura = "FatturaPA";

                        //per ogni allegato in fattura pa....
                        foreach (XmlParsing.FatturaPA.FatturaPAManager.allegati a in allegatiFatturaPA)
                        {
                            //controllo se il contenuto non fosse null... 
                            //magari è stato posto a null perchè l'allegato è già presente, 
                            //magari è posto a null perchè all'allegato realmente non esiste o l'xml è corrotto
                            //se è null salto a quello successvio
                            if (a.contenutoAttachment == null)
                                continue;

                            //creo il nome dal nome attachment e dal suo formato (estensione)
                            
                            //Gestione delle varie casitiche.
                            string nomeAll = "Attachment.bin";
                            if (string.IsNullOrEmpty(a.formatoAttachment))
                            {
                                if (String.IsNullOrEmpty(Path.GetExtension(a.nomeAttachment)))
                                {
                                    a.formatoAttachment = "bin";
                                }
                                else
                                {
                                    a.formatoAttachment = Path.GetExtension(a.nomeAttachment);
                                    a.nomeAttachment = Path.GetFileNameWithoutExtension(a.nomeAttachment);
                                }
                                a.formatoAttachment  = a.formatoAttachment.Replace (".","");
                                nomeAll = String.Format("{0}.{1}", a.nomeAttachment, a.formatoAttachment);
                            }
                            else
                            {
                                nomeAll = String.Format("{0}.{1}", a.nomeAttachment, a.formatoAttachment);
                            }

                            DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                            try
                            {
                                string descBreveAtt = string.Format("{0}_All {1}",nomeFattura, contatore++);
                                if (string.IsNullOrEmpty(a.descrizioneAttachment))
                                    all.descrizione = descBreveAtt;
                                else
                                    all.descrizione = String.Format ("{0}: {1}", descBreveAtt , a.descrizioneAttachment);

                                all.docNumber = docnumPrinc;
                                all.fileName = nomeAll;
                                all.version = "0";
                                all.numeroPagine = 1;

                                DocsPaVO.documento.Allegato allIns = null;
                                //aggiungo un allegato in DPA
                                allIns = AllegatiManager.aggiungiAllegato(objSicurezza, all);
                                BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(all.versionId, all.docNumber, "D");
                            }
                            catch (Exception e)
                            {
                                logger.Debug("Errore creando l'allegato per la fattura PA  ", e);
                                return;
                            }

                            try
                            {
                                string err = null;

                                DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                                fdAll.content = a.contenutoAttachment;
                                fdAll.length = a.contenutoAttachment.Length;

                                fdAll.name = all.fileName;
                                fdAll.bypassFileContentValidation = true;
                                fdAll.fullName = nomeAll;
                                fdAll.name = nomeAll;
                                fdAll.nomeOriginale = nomeAll;
                                fdAll.estensioneFile = Path.GetExtension (nomeAll) ;
                                fdAll.contentType = getContentType(nomeAll);

                                DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(objSicurezza, docnumPrinc);
                                DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                                fRAll = (DocsPaVO.documento.FileRequest)all;

                                if (fdAll.content.Length > 0)
                                {
                                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, objSicurezza, out err))
                                    {
                                        logger.Debug("Errore durante la putfile aggiungendo l'allegato per la fattura PA");
                                        AllegatiManager.rimuoviAllegato(all, objSicurezza);
                                        //  BusinessLogic.interoperabilita.InteroperabilitaManager.deleteNotifica(daticert.docnumber);
                                        throw new Exception(err);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                logger.Debug("Errore aggiungendo il contenuto all'allegato per la fattura PA  ", e);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore rilvenado se il file è una FatturaPA, ok non fa nulla", e);
                return;
            }
        }

        public static string getEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") ||
                    items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M"))
                    )
                {
                    retValue = items[i];
                    break;
                }
            }
            return retValue;
        }

        public static string removeLastExtension(string fileName)
        {
            return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
        }

        public static void EstrazioneTSD(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            EstrazioneTSContainer(fileRequest, fileDoc, objSicurezza, new DigitalSignature.PKCS_Utils.tsd());
        }

        public static void EstrazioneM7M(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            EstrazioneTSContainer(fileRequest, fileDoc, objSicurezza, new DigitalSignature.PKCS_Utils.m7m());            //AggiuntaEVerificaMarca(fileRequest, objSicurezza, m7mhandler.Data.Content, m7mhandler.TSR);
        }

        private static void EstrazioneTSContainer(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza, DigitalSignature.PKCS_Utils.ITimeStampedContainer TScontainer)
        {
            TScontainer.explode(fileDoc.content);
            fileDoc.content = TScontainer.Data.Content;

            if (!String.IsNullOrEmpty(fileDoc.name))
                fileDoc.name = removeLastExtension(fileDoc.name);
            if (!String.IsNullOrEmpty(fileDoc.fullName))
                fileDoc.fullName = removeLastExtension(fileDoc.fullName);
            if (!String.IsNullOrEmpty(fileDoc.nomeOriginale))
                fileDoc.nomeOriginale = removeLastExtension(fileDoc.nomeOriginale);
            if (!String.IsNullOrEmpty(fileRequest.fileName))
                fileRequest.fileName = removeLastExtension(fileRequest.fileName);
            if (!String.IsNullOrEmpty(fileRequest.path))
                fileRequest.path = removeLastExtension(fileRequest.path);

            fileRequest.fileSize = TScontainer.Data.Content.Length.ToString();
            fileDoc.estensioneFile = Path.GetExtension(fileDoc.fullName).Replace(".", "");
        }

        private static void AggiuntaEVerificaMarca(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza, byte[] Content, List<DigitalSignature.PKCS_Utils.CryptoFile> TSRLst)
        {
            foreach (DigitalSignature.PKCS_Utils.CryptoFile cf in TSRLst)
            {
                DigitalSignature.VerifyTimeStamp checkMarca = new DigitalSignature.VerifyTimeStamp();
                DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = checkMarca.Verify(Content, cf.Content);

                if (resultMarca.esito == "OK")
                {
                    DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                    timestampDoc.saveTSR(objSicurezza, resultMarca, fileRequest);
                }
            }
        }



        ////modifica

        // public static bool putFileCache(ref DocsPaVO.documento.FileRequest fileRequest,
        //                            DocsPaVO.documento.FileDocumento fileDoc,
        //                            DocsPaVO.utente.InfoUtente objSicurezza,
        //                            bool verifyFileFormat,
        //                            out string errorMessage) 
        //    {
        //    bool retValue = true;
        //    errorMessage = string.Empty;

        //    //controllo se doc in cestino
        //    string incestino = string.Empty;
        //    if (fileRequest != null && !string.IsNullOrEmpty(fileRequest.docNumber))
        //        incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(fileRequest.docNumber);

        //    if (!string.IsNullOrEmpty(incestino) && incestino == "1")
        //        throw new Exception("Il documento è stato rimosso, non è più possibile modificarlo");

        //    if (retValue)
        //    {
        //        // Verifica se il file è acquisito nell'ambito di un repository di sessione
        //        if (fileRequest.repositoryContext != null)
        //        {
        //            bool isProtocollo = false;

        //            // Determina la tipologia del documento 
        //            if (fileRequest is DocsPaVO.documento.Documento)
        //                isProtocollo = (!fileRequest.repositoryContext.IsDocumentoGrigio);
        //            else if (fileRequest is DocsPaVO.documento.Allegato)
        //                // L'allegato è sempre un documento grigio
        //                isProtocollo = false;

        //            // Verifica se il formato documento è tra quelli accettati dall'amministrazione
        //            if (verifyFileFormat)
        //                retValue = IsFileAccepted(objSicurezza, isProtocollo, fileDoc, out errorMessage);

        //            if (retValue)
        //            {
        //                SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(fileRequest.repositoryContext);

        //                // Inserimento di un file in un documento quando ancora non è stato salvato,
        //                // pertanto è disponibile un repository temporaneo valido solamente nell'ambito dell'inserimento
        //                fileManager.SetFile(fileRequest, fileDoc);

        //                // Aggiornamento oggetto FileRequest
        //                fileRequest.fileName = fileDoc.name;
        //                fileRequest.fileSize = fileDoc.content.Length.ToString();
        //                fileRequest.subVersion = "A";
        //                fileRequest.path = fileDoc.path;
        //            }
        //        }
        //        else
        //        {
        //            // Verifica se il formato documento è tra quelli accettati dall'amministrazione
        //            if (verifyFileFormat)
        //                retValue = IsFileAccepted(objSicurezza, fileRequest.docNumber, fileDoc, out errorMessage);

        //            if (retValue)
        //            {
        //                // Creazione contesto transazionale
        //                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        //                {
        //                    //controllo se il file è già stato acquistito per gestire la concorrenza
        //                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

        //                    //la stringa isFilePresent vale:
        //                    //"1" se il file è stato già acquisito;
        //                    //"0" se il file ancora non è stato acquisito
        //                    bool isFilePresent = doc.CheckAcquisizioneFile(fileRequest.docNumber, fileRequest.versionId);

        //                    if (fileDoc.content.Length == 0 || isFilePresent)
        //                    {
        //                        retValue = false;
        //                    }
        //                    else
        //                    {
        //                        string estensione = string.Empty;

        //                        try
        //                        {
        //                            string nomeFile = fileRequest.fileName;
        //                            estensione = fileDoc.estensioneFile;
        //                            if (!(estensione != null && !estensione.Equals("")))
        //                            {
        //                                //estensione = fileDoc.name.Substring(fileDoc.name.LastIndexOf(".")+1);
        //                                //nel caso di p7m deve prendere tutto ciò che viene dopo il primo punto, anche se parte del nome del file dopo
        //                                // in un metodo più interno si fanno i dovuto controlli
        //                                if (fileDoc.name.ToUpper().EndsWith("P7M"))
        //                                {
        //                                    estensione = fileDoc.name.Substring(fileDoc.name.IndexOf(".") + 1);
        //                                }
        //                                else
        //                                {
        //                                    estensione = Path.GetExtension(fileDoc.name);
        //                                    if (estensione.StartsWith("."))
        //                                        estensione = estensione.Substring(1);
        //                                }
        //                            }
        //                            estensione = estensione.ToUpper();

        //                            //Verifico se è un file firmato
        //                            if (estensione.EndsWith("P7M"))
        //                            {
        //                                if (fileRequest.fileName == null || fileRequest.fileName.Equals(""))
        //                                    fileRequest.fileName = fileRequest.versionId + estensione;
        //                                fileRequest.fileName = getEstensioneFileReq(fileRequest.fileName, estensione);
        //                            }
        //                            else
        //                            {
        //                                char[] dot = { '.' };
        //                                string[] parts = estensione.Split(dot);
        //                                estensione = parts[parts.Length - 1];
        //                            }

        //                            modificaEstensione(ref fileRequest, estensione, objSicurezza);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            logger.Debug("Errore nella gestione dell'inserimento del File (putFile)", e);

        //                            errorMessage = "Errore nella gestione dell'inserimento del File";
        //                            retValue = false;
        //                        }

        //                        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);

        //                        if (retValue)
        //                        {
        //                        }

        //                        if (retValue)
        //                        {
        //                            DocsPaDocumentale.Documentale.FullTextSearchManager fullTextManager = new DocsPaDocumentale.Documentale.FullTextSearchManager(objSicurezza);
        //                            retValue = fullTextManager.SetDocumentAsIndexed(fileRequest.docNumber);

        //                            if (!retValue)
        //                                errorMessage = "Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione.";
        //                        }

        //                        documentManager = null;

        //                        if (retValue)
        //                            transactionContext.Complete();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return retValue;
        //}

        //private static void cancellaFile(string[] infoFileDelete, double filesize)
        //{
        //    FileInfo file = new FileInfo(infoFileDelete[1]);

        //    if (double.Parse(infoFileDelete[4]) <= filesize)
        //    {
        //        File.Delete(infoFileDelete[1]);

        //        DocsPaDB.Query_DocsPAWS.Documentale query = new DocsPaDB.Query_DocsPAWS.Documentale();

        //        if (query.deleteCaching(infoFileDelete[0], infoFileDelete[3], infoFileDelete[2]))
        //            return (filesize - file.Length);
        //    }
        //    return filesize;

        //}

        //private static ArrayList verificaCaching()
        //{
        //    DocsPaDB.Query_DocsPAWS.Documentale query = new DocsPaDB.Query_DocsPAWS.Documentale();
        //    return query.ricercaDocumemtoInCache("1", "");
        //}

        //private static double getFolderSize(string physicalPath)
        //{
        //    double dblDirSize = 0;
        //    DirectoryInfo objDirInfo = new DirectoryInfo(physicalPath);
        //    Array arrChildFiles = objDirInfo.GetFiles();
        //    Array arrSubFolders = objDirInfo.GetDirectories();
        //    foreach (FileInfo objChildFile in arrChildFiles)
        //    {
        //        dblDirSize += objChildFile.Length;
        //    }
        //    foreach (DirectoryInfo objChildFolder in arrSubFolders)
        //    {
        //        dblDirSize += getFolderSize(objChildFolder.FullName);
        //    }
        //    return dblDirSize;
        //}

        //fine modifica


        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileRequest putFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            return putFile(fileRequest, fileDoc, objSicurezza, true);
        }

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDoc"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="verifyFileFormat"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileRequest putFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.utente.InfoUtente objSicurezza, bool verifyFileFormat)
        {
            DocsPaVO.documento.FileRequest retValue = fileRequest;

            string errorMessage;

            if (!putFile(ref retValue, fileDoc, objSicurezza, verifyFileFormat, out errorMessage))
                throw new ApplicationException(errorMessage);

            return retValue;
        }

        /// <summary>
        /// Inserimento di un file nel documento richiesto ignorando
        /// i criteri di security.
        /// NB: Qualsiasi utente potrà fare l'inserimento di un file
        /// in un documento.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="docNumber"></param>
        /// <param name="numTotaleAllegati"></param>
        /// <param name="numDiAllegati"></param>
        /// <param name="numPagineAllegato"></param>
        /// <param name="infoUtente"></param>
        /// <param name="pathFilename"></param>
        /// <param name="fileReq"></param>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        public static string PutFileBatchNoSecurity(
                string fileName,
                string docNumber,
                string numTotaleAllegati,
                string numDiAllegati,
                string numPagineAllegato,
                DocsPaVO.utente.InfoUtente infoUtente,
                string pathFilename,
                ref DocsPaVO.documento.FileRequest fileReq,
                ref DocsPaVO.documento.FileDocumento fileDoc)
        {
            logger.Debug("PutFileBatchNoSecurity");

            string errorMessage;

            // Verifica se il formato documento è tra quelli accettati dall'amministrazione
            if (IsFileAccepted(infoUtente, docNumber, fileName, out errorMessage))
            {
                DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);

                if (schedaDocumento.checkOutStatus != null)
                {
                    return string.Format("Il documento con identificativo {0}  risulta bloccato dall'utente {1}", schedaDocumento.docNumber, schedaDocumento.checkOutStatus.UserName);
                }
                else if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.protocolloAnnullato != null &&
                schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento != null)
                {
                    return string.Format("Il documento con identificativo {0} risulta annullato.", schedaDocumento.docNumber);
                }
                else
                {
                    return FileManager.PutFileBatch(schedaDocumento, infoUtente, fileName, docNumber, numTotaleAllegati,
                        numDiAllegati, numPagineAllegato, pathFilename, ref fileReq, ref fileDoc);
                }
            }
            else
            {
                return errorMessage;
            }
        }

        public static string PutFileBatch(string fileName, string docNumber, string numTotaleAllegati,
            string numDiAllegati, string numPagineAllegato, DocsPaVO.utente.InfoUtente infoUtente, string pathFilename,
            ref DocsPaVO.documento.FileRequest fileReq, ref DocsPaVO.documento.FileDocumento fileDoc)
        {
            logger.Debug("PutFileBatch");

            string errorMessage;

            // Verifica se il formato documento è tra quelli accettati dall'amministrazione
            if (IsFileAccepted(infoUtente, docNumber, fileName, out errorMessage))
            {
                DocsPaVO.documento.SchedaDocumento SchedaDocumento = new DocsPaVO.documento.SchedaDocumento();
                SchedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, docNumber, docNumber);

                return FileManager.PutFileBatch(SchedaDocumento, infoUtente, fileName, docNumber, numTotaleAllegati,
                    numDiAllegati, numPagineAllegato, pathFilename, ref fileReq, ref fileDoc);
            }
            else
            {
                return errorMessage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <param name="fileName"></param>
        /// <param name="docNumber"></param>
        /// <param name="numTotaleAllegati"></param>
        /// <param name="numDiAllegati"></param>
        /// <param name="numPagineAllegato"></param>
        /// <param name="pathFileName"></param>
        /// <param name="fileReq"></param>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        private static string PutFileBatch(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, string fileName, string docNumber, string numTotaleAllegati,
                                            string numDiAllegati, string numPagineAllegato, string pathFileName,
                                            ref DocsPaVO.documento.FileRequest fileReq, ref DocsPaVO.documento.FileDocumento fileDoc)
        {
            if (numDiAllegati != "0")
            {
                DocsPaVO.documento.Allegato allegato = new DocsPaVO.documento.Allegato();
                allegato.numeroPagine = Int32.Parse(numPagineAllegato);

                if (numDiAllegati.Length > 1)
                {
                    allegato.descrizione = "Allegato " + numDiAllegati;
                }
                else
                {
                    allegato.descrizione = "Allegato 0" + numDiAllegati;
                }

                allegato.docNumber = docNumber;
                allegato.version = "0";

                allegato.subVersion = "";
                fileReq = allegato;


            }
            else
            {
                fileReq.version = "1";
                fileReq.subVersion = "!";
                fileReq.versionLabel = fileReq.version;
            }


            try
            {
                fileReq.autore = schedaDocumento.autore;
                fileReq.daAggiornareFirmatari = false;
                fileReq.dataInserimento = schedaDocumento.dataCreazione;
                fileReq.docNumber = docNumber;
                fileReq.fileName = fileName;
                fileReq.idPeople = infoUtente.idPeople;
                fileDoc.name = fileName;
                fileDoc.fullName = pathFileName;

                FileInfo finfo = new FileInfo(pathFileName);
                string ext = finfo.Extension;

                switch (ext.ToLower())
                {
                    case ".pdf":
                        fileDoc.contentType = "APPLICATION/PDF";
                        break;
                    default:
                        fileDoc.contentType = "IMAGE/TIFF";
                        break;
                }

                System.IO.FileInfo fi = new FileInfo(pathFileName);
                fileDoc.length = (int)fi.Length;
                fileDoc.content = new byte[fileDoc.length];

                FileStream fs = new FileStream(pathFileName, FileMode.Open);
                fs.Read(fileDoc.content, 0, fileDoc.length);
                fs.Close();

                return "Y";
            }
            catch (Exception e)
            {
                logger.Debug("Errore nell'acquisizione batch del documento", e);
                return "N";
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="estensione"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static string getEstensioneFileReq(string fileName, string estensione)
        {
            logger.Debug("getEstensioneFileReq");
            //si verifica se l'estensione del file request va bene:
            // string tempEst = estensione.Substring(0, estensione.LastIndexOf("."));
            string tempEst = string.Empty;
            if (estensione.LastIndexOf(".") != -1)
                tempEst = estensione.Substring(0, estensione.LastIndexOf("."));
            else tempEst = estensione;
            string res = "";

            if (fileName.EndsWith(tempEst))
            {
                res = fileName;
            }
            else
            {
                res = fileName.Substring(0, fileName.IndexOf(".")) + "." + tempEst;
            }

            logger.Debug("Estensione file req=" + res);

            return res;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="estensione"></param>
        /// <param name="objSicurezza"></param>
        private static void modificaEstensione(ref DocsPaVO.documento.FileRequest fileRequest, string estensione, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            try
            {
                logger.Debug("modificaEstensione");
                fileRequest.applicazione = getApplicazione(estensione);
                bool daInviare = false;

                if (fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Documento)))
                {
                    if (((DocsPaVO.documento.Documento)fileRequest).daInviare != null && ((DocsPaVO.documento.Documento)fileRequest).daInviare.Equals("1"))
                    {
                        daInviare = true;
                    }
                }

                bool allegato = fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato));
                string docNumber = fileRequest.docNumber;
                string version_id = fileRequest.versionId;
                string library = DocsPaDB.Utils.Personalization.getInstance(objSicurezza.idAmministrazione).getLibrary();
                string version = fileRequest.version;
                string subVersion = fileRequest.subVersion;
                string versionLabel = fileRequest.versionLabel;

                if (allegato)
                {
                    BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(objSicurezza, (DocsPaVO.documento.Allegato)fileRequest);
                }
                else
                {
                    BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, objSicurezza, daInviare);
                }

                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
                if (!documentManager.ModifyExtension(ref fileRequest, docNumber, version_id, version, subVersion, versionLabel))
                {
                    throw new Exception("Errore nella modifica dell'estensione del nome file.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel metodo modificaEstensione della classe FileManager.cs : " + ex.Message);
            }

            //DocsPaWS.Utils.ErrorHandler.checkPCDOperation(docObj,"Errore nel lock del documento");
            logger.Debug("Fine modificaEstensione");
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileRequest"></param>
        /// <param name="debug"></param>
        private static void setImg(DocsPaVO.documento.FileRequest fileRequest)
        {
            // TODO: CHA_IMG = 0 quando creo una nuova versione
            if (Int32.Parse(fileRequest.version) > 0)
            {
                /*string updateString = 
                    "UPDATE PROFILE SET CHA_IMG = '1' WHERE CHA_IMG = '0' AND DOCNUMBER = " + fileRequest.docNumber;
                logger.Debug(updateString);
                db.executeNonQuery(updateString);*/
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                doc.SetImg(fileRequest.docNumber);
            }
        }
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="estensione"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ArrayList getApplicazioni(string estensione)
        {
            logger.Debug("getApplicazioni");

            System.Collections.ArrayList res = new System.Collections.ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            res = doc.GetApplicazioni(estensione, res);

            return res;

            #region Codice Commentato
            /*try
			{
				db.openConnection();
				string query="SELECT SYSTEM_ID,APPLICATION,DESCRIPTION, DEFAULT_EXTENSION FROM APPS ";
				if(estensione!=null){
				   query += " WHERE UPPER(DEFAULT_EXTENSION)='"+estensione.ToUpper()+"'";
				}
				logger.Debug(query);
				db.fillTable(query,ds,"APP");
				
				foreach(System.Data.DataRow dr in ds.Tables["APP"].Rows)
				{
					DocsPaVO.documento.Applicazione appl=new DocsPaVO.documento.Applicazione();
					appl.systemId= dr["SYSTEM_ID"].ToString();
					appl.descrizione= dr["DESCRIPTION"].ToString();
					appl.application= dr["APPLICATION"].ToString();
					appl.estensione= dr["DEFAULT_EXTENSION"].ToString();
				    res.Add(appl);
				}
				db.closeConnection();

				return res;
			}
			catch(Exception e)
			{
				logger.Debug(e.ToString());
				db.closeConnection();
			}
			
			return null;*/
            #endregion
        }

        /// <summary>
        /// </summary>
        /// <param name="estensione"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.Applicazione getApplicazione(string estensione)
        {
            logger.Debug("getApplicazione");

            DocsPaVO.documento.Applicazione res = new DocsPaVO.documento.Applicazione();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.GetExt(estensione, ref res);

            return res;

            #region Codice Commentato
            /*try
			{
				db.openConnection();
				string labelString="SELECT SYSTEM_ID, DEFAULT_EXTENSION FROM APPS ORDER BY SYSTEM_ID DESC";
				logger.Debug(labelString);
				db.fillTable(labelString,dataSet,"APPS");
				string lastSysId=dataSet.Tables["APPS"].Rows[0]["SYSTEM_ID"].ToString();
				System.Data.DataRow[] extRows = dataSet.Tables["APPS"].Select("DEFAULT_EXTENSION='"+estensione.ToUpper()+"'");
				
				if(extRows.Length==0)
				{
					int sysId=Int32.Parse(lastSysId)+1;
					string insertString="INSERT INTO APPS (SYSTEM_ID,";
					insertString=insertString+"APPLICATION,DESCRIPTION,FILING_SCHEME,DEFAULT_EXTENSION) VALUES (";
					insertString=insertString+sysId+",";
					insertString=insertString+"'GEN_"+estensione.ToUpper()+"','GEN_"+estensione.ToUpper()+"',2,'"+estensione.ToUpper()+"')";
					string insertString = obj.insertApp(sysId, estensione.ToUpper());
					logger.Debug(insertString);
					db.insertLocked(insertString,"APPS");
					res.systemId=sysId.ToString();
					res.estensione=estensione;

				}
				else 
				{
					res.estensione = estensione;
					res.systemId = extRows[0]["SYSTEM_ID"].ToString();
				}

				db.closeConnection();
				logger.Debug("Fine getApplicazione");
				
				return res;
			}
			catch(Exception e)
			{
				logger.Debug(e.Message);
				db.closeConnection();
				
				throw new Exception("F_System");
			}*/
            #endregion
        }

        #region Conversione Pdf lato server
        //FAILLACE: Codice commentato, in quanto la funzione sotto provvedere a svolgere le due funzioni
        /*
        public static byte[] creaXmlCOnversionePdfServer(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            xw.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xw.WriteStartElement("root");
            //Info Documento
            xw.WriteElementString("DST", infoUtente.dst);
            xw.WriteElementString("ID_AMM", infoUtente.idAmministrazione);
            xw.WriteElementString("ID_CORR_GLOBALI", infoUtente.idCorrGlobali);
            xw.WriteElementString("ID_GRUPPO", infoUtente.idGruppo);
            xw.WriteElementString("ID_PEOPLE", infoUtente.idPeople);
            xw.WriteElementString("SEDE", infoUtente.sede);
            xw.WriteElementString("USERID", infoUtente.userId);
            xw.WriteElementString("URLWA", infoUtente.urlWA);

            //Documento
            //Recupero scheda documento
            DocsPaVO.documento.SchedaDocumento schDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, objServerPdfConversion.idProfile, objServerPdfConversion.docNumber);
            xw.WriteElementString("AUTORE", schDoc.autore);
            xw.WriteElementString("DATA_INSERIMENTO", schDoc.dataCreazione);
            xw.WriteElementString("DESCRIZIONE", "Versione convertita in PDF");
            xw.WriteElementString("DOCNUMBER", schDoc.docNumber);
            xw.WriteElementString("ID_PROFILE", schDoc.systemId);
            // Recupero urlWs
            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            string httpRootPath = request.Url.Scheme + "://" + request.Url.Host;
            string UrlWs = httpRootPath + System.Web.HttpContext.Current.Request.ApplicationPath + "/DocsPaWS.asmx";
            string machineName = System.Web.HttpContext.Current.Server.MachineName;
            UrlWs = UrlWs.Replace("localhost", machineName);
            xw.WriteElementString("URL_WS", UrlWs);
            xw.WriteElementString("FILE_NAME", getFileNameFromPath(objServerPdfConversion.fileName));

            //Getsione nome del file originale
            DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)schDoc.documenti[0];
            string originalFileName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, fr);
            if (string.IsNullOrEmpty(originalFileName))
                originalFileName = fr.fileName;

            xw.WriteElementString("ORIGINAL_FILE_NAME", BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, fr));

            xw.WriteEndElement();
            sw.Flush();
            xw.Flush();
            xw.Close();
            // Gabriele Melini 15-04-2014
            // fix per bug lettere accentate nell'OFN
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] xmlByteArray = encoding.GetBytes(sw.ToString());
            return xmlByteArray;
        }
        */
        #region MEV 1.5 F02_01
        public static byte[] creaXmlConversionePdfServer(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion, HttpContext context)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            xw.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xw.WriteStartElement("root");
            //Info Documento
            xw.WriteElementString("DST", infoUtente.dst);
            xw.WriteElementString("ID_AMM", infoUtente.idAmministrazione);
            xw.WriteElementString("ID_CORR_GLOBALI", infoUtente.idCorrGlobali);
            xw.WriteElementString("ID_GRUPPO", infoUtente.idGruppo);
            xw.WriteElementString("ID_PEOPLE", infoUtente.idPeople);
            xw.WriteElementString("SEDE", infoUtente.sede);
            xw.WriteElementString("USERID", infoUtente.userId);
            xw.WriteElementString("URLWA", infoUtente.urlWA);
            if (infoUtente.delegato != null && !string.IsNullOrEmpty(infoUtente.delegato.idPeople))
            {
                xw.WriteElementString("ID_PEOPLE_DELEGATO", infoUtente.delegato.idPeople);
            }

            //Documento
            //Recupero scheda documento
            DocsPaVO.documento.SchedaDocumento schDoc;
            if (context !=null)
                schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, objServerPdfConversion.docNumber);
            else
                schDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, objServerPdfConversion.idProfile, objServerPdfConversion.docNumber);

            xw.WriteElementString("AUTORE", schDoc.autore);
            xw.WriteElementString("DATA_INSERIMENTO", schDoc.dataCreazione);
            xw.WriteElementString("DESCRIZIONE", "Versione convertita in PDF");
            xw.WriteElementString("DOCNUMBER", schDoc.docNumber);
            xw.WriteElementString("ID_PROFILE", schDoc.systemId);
            string machineName;
            string UrlWs;
            //in base all'esistenza o meno del context, mi comporto come la versione vecchia della funzione o quella nuova.
            if (context != null)
            {
                // Recupero urlWs
                string httpRootPath = context.Request.Url.Scheme + "://" + context.Request.Url.Host;
                logger.DebugFormat("httpRootPath: {0}", httpRootPath);
                UrlWs = httpRootPath + context.Request.ApplicationPath + "/DocsPaWS.asmx";
                machineName = context.Server.MachineName;
            }
            else
            {
                // Recupero urlWs
                System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
                string httpRootPath = request.Url.Scheme + "://" + request.Url.Host;
                logger.DebugFormat("httpRootPath: {0}", httpRootPath);
                UrlWs = httpRootPath + System.Web.HttpContext.Current.Request.ApplicationPath + "/DocsPaWS.asmx";
                machineName = System.Web.HttpContext.Current.Server.MachineName;
            }

            logger.DebugFormat("UrlWs: {0}", UrlWs);
            logger.DebugFormat("machineName: {0}", machineName);
            logger.DebugFormat("localhost {0}, URL_WS {1},  FILE_NAME {2} ", machineName, UrlWs, getFileNameFromPath(objServerPdfConversion.fileName));

            UrlWs = UrlWs.Replace("localhost", machineName);
            xw.WriteElementString("URL_WS", UrlWs);
            xw.WriteElementString("FILE_NAME", getFileNameFromPath(objServerPdfConversion.fileName));

            //Getsione nome del file originale
            DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)schDoc.documenti[0];
            string originalFileName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, fr);
            if (string.IsNullOrEmpty(originalFileName))
                originalFileName = fr.fileName;

            logger.DebugFormat("ORIGINAL_FILE_NAME {0}", originalFileName);

            //converto da UTF8 a base64, cosi' ho le accentate pure in ascii... 
            originalFileName = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(originalFileName));
            xw.WriteElementString("ORIGINAL_FILE_NAME", originalFileName);

            xw.WriteEndElement();
            sw.Flush();
            xw.Flush();
            xw.Close();
            // Gabriele Melini 15-04-2014
            // fix per bug lettere accentate nell'OFN
            //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            
            
            // Faillace 4-6-14 :  da indicazioni di luca, pare che UTF8 non funzioni bene e blocchi la conversione, 
            // mettiamo ASCII, ma il nome file originale lo codifichiamo base64
            // peraltro l'intestazione dell'XML è utf8---> bha
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] xmlByteArray = encoding.GetBytes(sw.ToString());
            return xmlByteArray;
        }
        #endregion

        public static string getFileNameFromPath(string path)
        {
            string fileNameWithExt = string.Empty;
            string[] arrayFileNameWithExt = path.Split('\\');
            if (arrayFileNameWithExt.Length > 0)
            {
                fileNameWithExt = arrayFileNameWithExt[arrayFileNameWithExt.Length - 1];
                //string[] arrayFileName = fileNameWithExt.Split('.');
                //if (arrayFileName.Length > 0)
                //{
                //    fileName = arrayFileName[0];
                //    return fileName;          
                //}
            }
            return fileNameWithExt;
        }

        public static string getExtFileFromPath(string path)
        {
            string ext = string.Empty;
            string[] arrayFileNameWithExt = path.Split('\\');
            if (arrayFileNameWithExt.Length > 0)
            {
                string fileNameWithExt = arrayFileNameWithExt[arrayFileNameWithExt.Length - 1];

                string[] arrayFileName = fileNameWithExt.Split('.');
                if (arrayFileName.Length > 0)
                {
                    ext = arrayFileName[arrayFileName.Length - 1];
                }
            }
            return ext;
        }
        #endregion Conversione Pdf lato server

        /// <summary>
        /// Verifica marca temporale
        /// </summary>
        /// <param name="fileDoc"></param>
        private static void VerifyFileTimeStamp(DocsPaVO.documento.FileDocumento fileDoc)
        {
            VerifyTimeStamp verifyTimestamp = new VerifyTimeStamp();
            DocsPaVO.areaConservazione.OutputResponseMarca marca = verifyTimestamp.Verify(fileDoc.content);
            fileDoc.timestampResult = marca;
            fileDoc.content = marca.DecryptedTSR.content;
            fileDoc.contentType = marca.DecryptedTSR.contentType;
            fileDoc.length = marca.DecryptedTSR.length;
        }
        #region FileInfo / Carta Identità documento
        /// <summary>
        /// Esegue i test per il FileInfo e memorizza nella components i dati aggiornati nell'apposito campo flag
        /// </summary>
        /// <param name="fileRequest">il filerequest in input</param>
        /// 
        /*
         * 
        
         select *  from (
         select *  from components where file_info LIKE '%8'
         WHERE ROWNUM <= 100;

         * 
         */
        public static DocsPaVO.documento.FileInformation getFileFileInformation(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
            string fileInfoMask = docs.GetFileInfoMask(fileRequest.versionId, fileRequest.docNumber);
            return DocsPaVO.documento.FileInformation.decodeMask(fileInfoMask);
        }


        public static void processFileInformation(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            logger.DebugFormat("processFileInformation");
            bool disabled = true;
            string config = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROCESS_FILEINFO");
            if (!string.IsNullOrEmpty(config))
                if ((config == "1") || (config.ToLower() == "true"))
                    disabled = false;
            if (disabled)
                return;

            if (fileRequest.repositoryContext != null)
                return;

            if (String.IsNullOrEmpty(fileRequest.docNumber))
                return;
            logger.DebugFormat("processFileInformation2");
            if (objSicurezza != null && string.IsNullOrEmpty(objSicurezza.dst))
            {
                logger.DebugFormat("Ricavo Token");
                objSicurezza.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                logger.DebugFormat("Token ricavato");
            }
            logger.DebugFormat("processFileInformation3");
            string fileNameBack = fileRequest.fileName;
            DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
            string fileInfoMask = docs.GetFileInfoMask(fileRequest.versionId, fileRequest.docNumber);
            logger.DebugFormat("GET FileinfoMASK for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
            DocsPaVO.documento.FileInformation fileInfo = DocsPaVO.documento.FileInformation.decodeMask(fileInfoMask);

            /*
            // Setto tutto a untested (vecchio disabled)
            if (disabled)
            {
                fileInfo.Status = DocsPaVO.documento.FileInformation.VerifyStatus.Untested;
                docs.UpdateComponentsFileInfo(DocsPaVO.documento.FileInformation.encodeMask(fileInfo), fileRequest.versionId, fileRequest.docNumber);
                return;
            }
            */
            logger.DebugFormat("processFileInformation4");
            // Modifica per evitare errori col documentale Documentum.
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            fileRequest.version = documentale.GetVersionFromVersionId(fileRequest.versionId);
            fileRequest.versionLabel = fileRequest.version;

            //1) prelevo il file
            DocsPaVO.documento.FileDocumento filedoc = getFile(fileRequest, objSicurezza);
            if (!String.IsNullOrEmpty(filedoc.name))
            {
                if (Path.GetExtension(filedoc.name).ToLowerInvariant().Contains("pdf"))
                {
                    fileInfo.PdfVer = Encoding.UTF8.GetString(filedoc.content, 5, 3);
                    logger.DebugFormat("Estenzione PDF, versione ricavata {0}", fileInfo.PdfVer);
                }
            }
            DocsPaVO.documento.FileDocumento filedocfirmato = getFileFirmato(fileRequest, objSicurezza, false);
            Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
            string fileExtension = ff.FileType(filedoc.content);
            logger.DebugFormat("processFileInformation5");
            if ((fileExtension.ToUpperInvariant().Contains("PDF/")) && (!String.IsNullOrEmpty(fileInfo.PdfVer)))
                fileInfo.PdfVer += "§" + fileExtension;

            logger.DebugFormat("SAUTILS fileExtension for file is {0}", fileExtension);

            if (ff.isExecutable(fileExtension))
            {
                fileInfo.NoMacroOrExe = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                logger.DebugFormat("SAUTILS Says file has macros");
            }
            else
            {
                fileInfo.NoMacroOrExe = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                logger.DebugFormat("SAUTILS Says file has NO macros");
            }
            bool fileformatOK = IsValidFileContent(filedoc);
            if (fileformatOK)
            {
                fileInfo.FileFormatOK = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                logger.DebugFormat("SAUTILS Says file has good Extension");
            }
            else
            {
                fileInfo.FileFormatOK = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                logger.DebugFormat("SAUTILS Says file has BAD Extension");
            }
            ArrayList tsAl = TimestampManager.getTimestampsDoc(objSicurezza, fileRequest);
            string extOFN = string.Empty;
            try
            {
                extOFN = System.IO.Path.GetExtension(filedocfirmato.nomeOriginale);
                logger.DebugFormat("OFN ext: {0}", extOFN);
            }
            catch { }

            //Test sull' Impronta
            string impronta;

            logger.DebugFormat("processFileInformation");

            docs.GetImpronta(out impronta, fileRequest.versionId, fileRequest.docNumber);
            if (string.IsNullOrEmpty(impronta))
            {
                //Sul DB non è stato salvata l'impronta/ metto untested perchè non posso testare
                fileInfo.FileHashOK = DocsPaVO.documento.FileInformation.VerifyStatus.Untested;
            }
            else
            {

                //ho un impronta, testo sia SHA1 che SHA256 non si sa mai
                if (
                    impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(filedocfirmato.content)) ||
                    impronta.Equals(DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(filedocfirmato.content))
                    )
                {
                    //impronta valida, metto a valid il flag
                    fileInfo.FileHashOK = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                }
                else
                {
                    //impronta nonvalida, metto a invalid il flag
                    fileInfo.FileHashOK = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                }
            }

            //controllo TS
            if (tsAl.Count == 0)     //TS non presenti
                fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;

            foreach (DocsPaVO.documento.TimestampDoc tsInfo in tsAl)
            {

                BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new Documenti.DigitalSignature.VerifyTimeStamp();
                DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = vts.Verify(filedocfirmato.content, Convert.FromBase64String(tsInfo.TSR_FILE));
                logger.DebugFormat("Found TS: result  {0}", resultMarca.esito);
                if (resultMarca.esito == "OK")
                {
                    fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                }
                else
                {
                    //gestire se è TSD o M7M, il controllo non va fatto dato che sarà errato 
                    if ((extOFN.ToUpperInvariant().Contains("TSD") || extOFN.ToUpperInvariant().Contains("M7M")))
                    {

                        resultMarca = vts.Verify(BusinessLogic.Documenti.DigitalSignature.Helpers.sbustaFileTimstamped(filedocfirmato.content), Convert.FromBase64String(tsInfo.TSR_FILE));
                        logger.DebugFormat("Found [TSD] TS: result  {0}", resultMarca.esito);
                        if (resultMarca.esito == "OK")
                        {
                            fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                            if (Convert.ToDateTime(tsInfo.DTA_SCADENZA) < System.DateTime.Now)
                            {
                                fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Expired;
                                logger.Debug("Found [TSD] TS: result  Expired");
                            }
                        }
                        else
                        {
                            fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;
                            logger.Debug("Found [TSD] TS: result  NA: bad TS");
                        }
                        break;
                    }
                    fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                    logger.Debug("Found  TS: result  Invalid bad TS");
                    break;
                }

                if (Convert.ToDateTime(tsInfo.DTA_SCADENZA) < System.DateTime.Now)
                {
                    fileInfo.TimeStampStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Expired;
                    logger.Debug("Found TS: result  Expired");
                }

                break;      //solo il primo ovvero l'ultima marca
            }

            if (fileInfo.CheckRefDate == DateTime.MinValue)
                fileInfo.CheckRefDate = DateTime.Now;

            // già stato controllato... non rifaccio il controllo
            if ((fileInfo.Signature != DocsPaVO.documento.FileInformation.VerifyStatus.Valid) &&
               (fileInfo.CrlStatus != DocsPaVO.documento.FileInformation.VerifyStatus.Valid))
            {
                logger.DebugFormat("Firma non valida o non verificata");
                if (fileRequest.firmato == "1")
                {
                    logger.DebugFormat("Firmato, verifica schedulata");
                    fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
                    fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
                    fileInfo.CheckRefDate = DateTime.MinValue; //rimetto il minvalue, dato che la data la metto alla fine della verifica crl
                }
                else
                {
                    logger.DebugFormat("Non Firmato");
                    fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;
                    fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.NotApplicable;
                }
            }

            //foto formato in amm
            try
            {
                string estensione = Path.GetExtension(filedoc.name).ToUpperInvariant();

                //tolgo il punto davanti all'aestensione nel caso esista.
                if (estensione.StartsWith(".")) estensione = estensione.Substring(1);

                DocsPaVO.FormatiDocumento.SupportedFileType[] fileTypes = FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(objSicurezza.idAmministrazione));
                DocsPaVO.FormatiDocumento.SupportedFileType FileType = (from fileType in fileTypes where fileType.FileExtension.ToUpper().Equals(estensione) select fileType).FirstOrDefault();
                if (FileType.FileTypePreservation)
                    fileInfo.Preservable = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                else
                    fileInfo.Preservable = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;

                if (FileType.FileTypeSignature)
                    fileInfo.Signable = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                else
                    fileInfo.Signable = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;

                if (fileInfo.AdminRefDate == DateTime.MinValue)
                    fileInfo.AdminRefDate = DateTime.Now;
            }
            catch (Exception e)
            {
                logger.DebugFormat("Errore reperendo il formato dall'amministrazione {0} stack {1}", e.Message, e.StackTrace);
            }

            fileInfo.setGlobalStatus();
            string fileInfoStr = DocsPaVO.documento.FileInformation.encodeMask(fileInfo);
            logger.DebugFormat("SET FileinfoMASK for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
            docs.UpdateComponentsFileInfo(fileInfoStr, fileRequest.versionId, fileRequest.docNumber);

            fileRequest.fileName = fileNameBack;

            //Se sto acquisendo l'ultima versione, aggiorno l'informazione sul file acquisito in DPA_INFO_FILE
            bool isUltimaVersione = fileRequest.versionId.Equals(VersioniManager.getLatestVersionID(fileRequest.docNumber, objSicurezza));
            if (isUltimaVersione)
            {
                fileRequest.dataAcquisizione = docs.GetDataAcquisizioneFile(fileRequest.versionId);
                UpdateInfoFileAcquisito(fileInfo, fileRequest, filedoc.nomeOriginale, fileExtension, objSicurezza);
            }

        }


        public static bool processFileInformationCRL(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            string fileNameBack = fileRequest.fileName;

            if (objSicurezza != null && string.IsNullOrEmpty(objSicurezza.dst))
            {
                logger.DebugFormat("Ricavo Token");
                objSicurezza.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                logger.DebugFormat("Token ricavato");
            }

            // Modifica per evitare errori col documentale Documentum.
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            fileRequest.version = documentale.GetVersionFromVersionId(fileRequest.versionId);
            fileRequest.versionLabel = fileRequest.version;

            DocsPaVO.documento.FileDocumento filedocfirmato = getFileFirmato(fileRequest, objSicurezza, false);
            DateTime dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fileRequest, objSicurezza);
            //verifica Esterna
            VerifyFileSignature(filedocfirmato, dataRif);

            bool retval = processFileInformationCRLUpdate(fileRequest, objSicurezza, filedocfirmato,dataRif);
            fileRequest.fileName = fileNameBack;
            return retval;
        }


        public static bool processFileToSetSignType(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            string fileNameBack = fileRequest.fileName;

            if (objSicurezza != null && string.IsNullOrEmpty(objSicurezza.dst))
            {
                logger.DebugFormat("Ricavo Token");
                objSicurezza.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                logger.DebugFormat("Token ricavato");
            }

            // Modifica per evitare errori col documentale Documentum.
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            fileRequest.version = documentale.GetVersionFromVersionId(fileRequest.versionId);
            fileRequest.versionLabel = fileRequest.version;

            bool retval = processFileToSetSignTypeUpdate(fileRequest, objSicurezza);
            fileRequest.fileName = fileNameBack;
            return retval;
        }


        public static bool processFileToSetSignTypeUpdate(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool processed = false;
            string tipoFirma = string.Empty;
            bool electornicallySigned = false;
            string extension = string.Empty;
            try
            {
                logger.DebugFormat("----- INIZIO processFileToSetSignTypeUpdate docNumber: {0} versionId: {1} ", fileRequest.docNumber, fileRequest.versionId);

                if (fileRequest.firmato != "1")
                    logger.DebugFormat("----- processFileToSetSignTypeUpdate file non firmato impossibile proseguire docNumber: {0} versionId: {1} ", fileRequest.docNumber, fileRequest.versionId);
                else
                {
                    if (infoUtente != null && string.IsNullOrEmpty(infoUtente.dst))
                    {
                        logger.DebugFormat("Ricavo Token");
                        infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                        logger.DebugFormat("Token ricavato");
                    }

                    electornicallySigned = BusinessLogic.LibroFirma.LibroFirmaManager.IsElectronicallySigned(fileRequest.docNumber, fileRequest.versionId);

                    extension = Path.GetExtension(fileRequest.path).ToUpper();

                    switch (extension)
                    {
                        case(".P7M"):
                            tipoFirma = TipoFirma.CADES;
                            break;
                        case (".TSD"):
                            tipoFirma = TipoFirma.TSD;
                            break;
                        case (".XML"):
                            tipoFirma = TipoFirma.XADES;
                            break;
                        case (".PDF"):
                            if (BusinessLogic.Documenti.FileManager.IsPdfPades(fileRequest, infoUtente))
                            {
                                tipoFirma = TipoFirma.PADES;
                            }
                            break;
                        default:
                            tipoFirma = TipoFirma.NESSUNA_FIRMA;
                            break;
                    }
                    logger.DebugFormat("----- processFileToSetSignTypeUpdate aggiorno il valore per il seguente documento docNumber: {0} versionId: {1} estensione: {2} tipofirma: {3} path {4}", fileRequest.docNumber, fileRequest.versionId, extension, fileRequest.path);
                    DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                    processed = docs.UpdateComponentsChaTipoFirma(fileRequest.versionId, fileRequest.docNumber, tipoFirma, electornicallySigned);
                }
                logger.DebugFormat("----- FINE processFileToSetSignTypeUpdate docNumber: {0} versionId: {1}  processed: {2}", fileRequest.docNumber, fileRequest.versionId, processed);                
            }
            catch (Exception e)
            {
                logger.DebugFormat("Errore settando il tipo firma CHA_TIPO_FIRMA {0} {1}", e.Message, e.StackTrace);
                processed = false;
            }
            return processed;
        }

        public static bool processFileInformationCRLUpdate(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza, DocsPaVO.documento.FileDocumento filedocfirmato, DateTime DataDiRiferimentoCRL)
        {
            if (fileRequest.firmato == "1")
            {
                try
                {
                    if (objSicurezza != null && string.IsNullOrEmpty(objSicurezza.dst))
                    {
                        logger.DebugFormat("Ricavo Token");
                        objSicurezza.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                        logger.DebugFormat("Token ricavato");
                    }

                    DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                    string fileInfoMask = docs.GetFileInfoMask(fileRequest.versionId, fileRequest.docNumber);
                    logger.DebugFormat("GET FileinfoMASK CRL for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
                    DocsPaVO.documento.FileInformation fileInfo = DocsPaVO.documento.FileInformation.decodeMask(fileInfoMask);

                    //controllo già effettuato in precedenza, non lo rifaccio..
                    if ((fileInfo.Signature != DocsPaVO.documento.FileInformation.VerifyStatus.Valid) ||
                        (fileInfo.CrlStatus != DocsPaVO.documento.FileInformation.VerifyStatus.Valid))
                    {
                        if (filedocfirmato.signatureResult.StatusCode == -100)
                        {
                            //server sta giu o non o funzionante
                            logger.DebugFormat("Errore verificando la firma -100");
                            fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
                            fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.InProgress;
                        }
                        else
                        {
                            logger.DebugFormat("Status verificando la firma {0}", filedocfirmato.signatureResult.StatusCode);
                            bool hasErrs = false;
                            if (filedocfirmato.signatureResult.ErrorMessages != null)
                                if (filedocfirmato.signatureResult.ErrorMessages.Length != 0)
                                    hasErrs = true;

                            if ((filedocfirmato.signatureResult.StatusCode != -1) && (hasErrs == false))
                            {
                                fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                                logger.DebugFormat("firma OK");
                            }
                            else
                            {
                                fileInfo.Signature = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                                logger.DebugFormat("firma NON OK");
                            }
                            //controllo CRL:
                            if (revokedCertArePresent(filedocfirmato))
                            {
                                logger.DebugFormat("ATTENZIONE Sono presenti Certificati revocati");
                                fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Invalid;
                            }
                            else
                            {
                                logger.DebugFormat("NON Sono presenti Certificati revocati");
                                fileInfo.CrlStatus = DocsPaVO.documento.FileInformation.VerifyStatus.Valid;
                            }
                        }

                        fileInfo.setGlobalStatus();

                        if (fileInfo.CheckRefDate == DateTime.MinValue)
                            fileInfo.CheckRefDate = DateTime.Now;

                        //if (fileInfo.CrlRefDate == DateTime.MinValue)
                        fileInfo.CrlRefDate = DataDiRiferimentoCRL;


                        fileInfoMask = DocsPaVO.documento.FileInformation.encodeMask(fileInfo);
                        logger.DebugFormat("SET FileinfoMASK CRL for doc: {0} ver: {1}  mask: {2}", fileRequest.docNumber, fileRequest.versionId, fileInfoMask);
                        docs.UpdateComponentsFileInfo(fileInfoMask, fileRequest.versionId, fileRequest.docNumber);

                    }
                }
                catch (Exception e)
                {
                    logger.DebugFormat("Errore settando la CRL per la FileInformation {0} {1}", e.Message, e.StackTrace);
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool revokedCertArePresent(DocsPaVO.documento.FileDocumento filedoc)
        {
            foreach (DocsPaVO.documento.PKCS7Document p7md in filedoc.signatureResult.PKCS7Documents)
            {
                foreach (DocsPaVO.documento.SignerInfo siinfo in p7md.SignersInfo)
                {
                    if (siinfo.CertificateInfo.RevocationDate != DateTime.MinValue)
                        return true;
                }
            }
            return false;
        }

        public static string getExtCheckErrors(DocsPaVO.documento.FileDocumento filedoc)
        {
            string retval = "";
            if (filedoc.signatureResult.ErrorMessages.Length > 0)
            {
                string errs = "";
                foreach (string err in filedoc.signatureResult.ErrorMessages)
                {
                    if (err.Contains("EXTCHECK"))
                    {
                        if (err.Contains("-"))
                            errs += err.Split('-')[0] + ":";
                        else
                            errs += err + ":";
                    }
                }
                if (errs.Length > 0)
                    retval += " :" + errs;
            }
            return retval;
        }
        #endregion


        #region HSMSignature

        public static string HSM_RequestCertificateJson(String AliasCertificato, String DominioCertificato)
        {
            logger.Debug("inizio");
            return BusinessLogic.Documenti.DigitalSignature.RemoteSignature.GetHSMCertificateList(AliasCertificato, DominioCertificato);
        }


        public static bool HSM_RequestOTP(String AliasCertificato, String DominioCertificato)
        {
            logger.Debug("inizio");
            try
            {
                return BusinessLogic.Documenti.DigitalSignature.RemoteSignature.RichiediOTP(AliasCertificato, DominioCertificato);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore invocando il metodo RichiediOTP {0} {1}", ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente">infoutente</param>
        /// <param name="fr">filerequest</param>
        /// <param name="cofirma">richiedo cofirma</param>
        /// <param name="timestamp">richiedo timestamp</param>
        /// <param name="tipoFirma">CADES/PADES</param>
        /// <param name="AliasCertificato">Alias del certificato</param>
        /// <param name="DominioCertificato">Dominio del Certificato</param>
        /// <param name="OtpFirma">Otp della Firma</param>
        /// <param name="PinCertificato">Pin del certificato</param>
        /// <returns></returns>
        public static bool HSM_Sign(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, bool cofirma, bool timestamp, string tipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf)
        {

            try
            {
                RemoteSignature.SignType tipo = (RemoteSignature.SignType)Enum.Parse(typeof(RemoteSignature.SignType), tipoFirma);
                BusinessLogic.Documenti.DigitalSignature.RemoteSignature rs = new RemoteSignature(AliasCertificato, DominioCertificato, PinCertificato, tipo, timestamp, cofirma);

                DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
                if (ConvertPdf)
                {
                    fd = BusinessLogic.LiveCycle.LiveCycle.GeneratePDFInSyncMod(fd);
                    fr.fileName = fd.name;
                }

                byte[] content = fd.content;
                byte[] signed = rs.Sign(content, OtpFirma);
                bool signResult = false;
                if ((tipo == RemoteSignature.SignType.PADES))
                    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(signed, cofirma, ref fr, infoUtente);
                else   //cades non cofirmato
                    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), cofirma, ref fr, infoUtente);

                return signResult;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore invocando il metodo RemoteSignature {0} {1}", ex.Message, ex.StackTrace);
                return false;
            }

        }

        public static bool HSM_Sign(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, bool cofirma, bool timestamp, string tipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf, out FirmaResult firmaResult)
        {
            firmaResult = new FirmaResult();
            try
            {
                RemoteSignature.SignType tipo = (RemoteSignature.SignType)Enum.Parse(typeof(RemoteSignature.SignType), tipoFirma);
                BusinessLogic.Documenti.DigitalSignature.RemoteSignature rs = new RemoteSignature(AliasCertificato, DominioCertificato, PinCertificato, tipo, timestamp, cofirma);

                DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
                if (ConvertPdf)
                {
                    fd = BusinessLogic.LiveCycle.LiveCycle.GeneratePDFInSyncMod(fd);
                    fr.fileName = fd.name;
                }

                byte[] content = fd.content;
                byte[] signed = rs.Sign(content, OtpFirma);
                bool signResult = false;
                if ((tipo == RemoteSignature.SignType.PADES))
                    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(signed, cofirma, ref fr, infoUtente, ConvertPdf);
                else   //cades non cofirmato
                    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), cofirma, ref fr, infoUtente);

                return signResult;
            }
            catch (Exception ex)
            {
                string id = ExtractFromString(ex.Message, "#CODE", "#MESSAGE");
                firmaResult.esito = GetEsitoFirma(id);
                firmaResult.errore = ExtractFromString(ex.Message, "#MESSAGE", "#TYPE");
                logger.ErrorFormat("Errore invocando il metodo RemoteSignature {0} {1}", ex.Message, ex.StackTrace);
                return false;
            }
        }

        private static EsitoFirma GetEsitoFirma(string id)
        {
            EsitoFirma esito = new EsitoFirma();
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            esito = documenti.GetMessaggioEsitoFirma(id);              
            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente">infoutente</param>
        /// <param name="content">byte content</param>
        /// <param name="cofirma">richiedo cofirma</param>
        /// <param name="timestamp">richiedo timestamp</param>
        /// <param name="tipoFirma">CADES/PADES</param>
        /// <param name="AliasCertificato">Alias del certificato</param>
        /// <param name="DominioCertificato">Dominio del Certificato</param>
        /// <param name="OtpFirma">Otp della Firma</param>
        /// <param name="PinCertificato">Pin del certificato</param>
        /// <returns></returns>
        public static byte[] HSM_SignContent(DocsPaVO.utente.InfoUtente infoUtente, byte[] content, bool cofirma, bool timestamp, string tipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf)
        {
            byte[] signed = null;
            RemoteSignature.SignType tipo;
            BusinessLogic.Documenti.DigitalSignature.RemoteSignature remoteSignature = null;
            HSMPin hsmPin = new HSMPin();

            try
            {
                hsmPin.pin = PinCertificato;
                hsmPin.idPeople = infoUtente.idPeople;
                hsmPin.idAmministrazione = infoUtente.idAmministrazione;
                hsmPin.alias = AliasCertificato;

                memorizesPin(hsmPin);

                tipo = (RemoteSignature.SignType)Enum.Parse(typeof(RemoteSignature.SignType), tipoFirma);
                remoteSignature = new RemoteSignature(AliasCertificato, DominioCertificato, PinCertificato, tipo, timestamp, cofirma);

                signed = remoteSignature.Sign(content, OtpFirma);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("HSM_SignContent: Errore invocando il metodo RemoteSignature {0} {1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (remoteSignature != null) remoteSignature = null;
            }

            return signed;
        }

        public static byte[] HSM_AutomaticSignature(InfoUtente infoUtente, String inputPath, String outputPath, bool timeStamp)
        {
            String memorizedDecryptedPin = null;
            CryptoString crypto = null;
            byte[] signedContent = null;
            HSMPin hsmPin = null;
            HSMParameters hsmParameters = null;

            try
            {
                hsmPin = Utenti.UserManager.selectHSMPin(infoUtente.idPeople, infoUtente.idAmministrazione);

                if (hsmPin == null || String.IsNullOrEmpty(hsmPin.pin) || String.IsNullOrEmpty(hsmPin.alias))
                    throw new Exception("HSMPIN NOT FOUND - idPeople: " + infoUtente.idPeople + " - idAmm: " + infoUtente.idAmministrazione + " - inputPath: " + inputPath);
                else
                {
                    crypto = new CryptoString((infoUtente.idPeople));
                    memorizedDecryptedPin = crypto.Decrypt(hsmPin.pin).Replace("\0", String.Empty);
                }

                hsmParameters = Utenti.UserManager.selectHSMAmministrationParameters(infoUtente.idAmministrazione);

                if(hsmParameters == null)
                    throw new Exception("HSMPARAMETERS AMMINISTRATION NOT FOUND - idPeople: " + infoUtente.idPeople + " - idAmm: " + infoUtente.idAmministrazione + " - inputPath: " + inputPath);

                if (!timeStamp)
                {
                    logger.Debug("Marcatura temporale NON ATTIVA");
                    hsmParameters.tsaurl = string.Empty;
                    hsmParameters.tsauser = string.Empty;
                    hsmParameters.tsapassword = string.Empty;
                }

                int result = InfoCertSigner.CADESSingleSigner(hsmPin.alias, hsmParameters.dominio, memorizedDecryptedPin, hsmParameters.tsaurl, hsmParameters.tsauser, hsmParameters.tsapassword, hsmParameters.userapplicativa, hsmParameters.passwordapplicativa, inputPath, outputPath, hsmParameters.serverurl);

                if (result == 0)
                {
                    signedContent = File.ReadAllBytes(outputPath);
                }
                else
                    throw new Exception("CADESSingleSigner NOT Signed - idPeople: " + infoUtente.idPeople + " - idAmm: " + infoUtente.idAmministrazione + " - inputPath: " + inputPath);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("HSM_AutomaticSignature: Exception {0} {1}", ex.Message, ex.StackTrace);
            }

            return signedContent;
        }

        /// <summary>
        /// Pin: viene memorizzato per gestire la firma automatica INFOCERT
        /// </summary>
        /// <param name="idPeople">oggetto infoutente</param>
        /// <param name="pin">pin inserito dall`utente</param>
        private static void memorizesPin(HSMPin hsmPin)
        {
            HSMPin memorizedHSMPin = null;

            try
            {
                memorizedHSMPin = Utenti.UserManager.selectHSMPin(hsmPin.idPeople, hsmPin.idAmministrazione);

                if (memorizedHSMPin == null)
                    Utenti.UserManager.insertHSMPin(hsmPin);
                else
                    Utenti.UserManager.updateHSMPin(hsmPin);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("memorizesPin: Exception {0} {1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (memorizedHSMPin != null) memorizedHSMPin = null;
            }
        }

        /// <summary>
        /// Apro una sessione multisign,inserisce dentro i file
        /// </summary>
        /// <param name="infoUtente">oggetto infoutente</param>
        /// <param name="fileRequestList">array di filerequest</param>
        /// <param name="cofirma">opzione cofirma</param>
        /// <param name="timestamp">opzione timestamp</param>
        /// <param name="tipoFirma">PADES/CASES</param>
        /// <returns>token di sessione multisign</returns>
        public static string HSM_OpenMultiSignSession(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest[] fileRequestList, bool cofirma, bool timestamp, string tipoFirma)
        {
            logger.Debug("inizio");
            RemoteSignature.SignType tipo = (RemoteSignature.SignType)Enum.Parse(typeof(RemoteSignature.SignType), tipoFirma);

            BusinessLogic.Documenti.DigitalSignature.RemoteSignature.MultiSign ms = new RemoteSignature.MultiSign(cofirma, timestamp, tipo);
            logger.DebugFormat("Sessione HSM aperta, token: {0}", ms.SessionToken);
            List<string> tokenList = new List<string>();
            foreach (DocsPaVO.documento.FileRequest fr in fileRequestList)
            {
                if (fr != null)
                {
                    DocsPaVO.documento.FileDocumento fd = null;
                    try
                    {
                        fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
                    }
                    catch (Exception e)
                    {
                        logger.ErrorFormat("Errore reperendo il file{0} {1}", e.Message, e.StackTrace);
                    }

                    if (fd != null)
                    {
                        try
                        {
                            string hash = ms.Put(fd, fr.versionId);
                            tokenList.Add(hash + "§" + fr.docNumber);
                        }
                        catch (Exception e)
                        {
                            logger.ErrorFormat("Errore inserendolo in sessione il file{0} {1}", e.Message, e.StackTrace);
                        }
                    }
                }
            }
            string retval = ms.SessionToken;
            foreach (string toks in tokenList)
                retval += "|" + toks;

            return retval;
        }

        /// <summary>
        /// Firma, reperimento file e chiusura sessione
        /// </summary>
        /// <param name="infoUtente">oggetto infoutente</param>
        /// <param name="fileRequestList">lista di filerequest</param>
        /// <param name="MultiSignToken">Token tornato dalla HSM_OpenMultiSignSession</param>
        /// <param name="AliasCertificato">alias certificato</param>
        /// <param name="DominioCertificato">dominio certificato</param>
        /// <param name="OtpFirma">Otp di firma</param>
        /// <param name="PinCertificato">Pin del certificato</param>
        /// <returns>valore firmati tutti ok, o errore</returns>
        public static FirmaResult[] HSM_SignMultiSignSession(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool cofirma)
        {
            logger.Debug("inizio");
            string[] tokenLst = MultiSignToken.Split('|');
            string sessionToken = tokenLst[0];
            List<FirmaResult> retval = new List<FirmaResult>();
            BusinessLogic.Documenti.DigitalSignature.RemoteSignature.MultiSign ms = new RemoteSignature.MultiSign(AliasCertificato, DominioCertificato, sessionToken, cofirma);
            //bool cofirma = ms.Cofirma;
            bool result;
            string error = string.Empty;
            EsitoFirma esitoFirma;
            try
            {
                result = ms.Sign(PinCertificato, OtpFirma);
            }
            catch(Exception e)
            {
                result = false;
                error = e.Message;
            }   
            if (result)
            {
                foreach (string fileSession in tokenLst)
                {
                    
                    string[] hashLst = fileSession.Split('§');
                    if (hashLst.Length != 2) //non ci sta la coppia , o è il primo o ci sono problemi
                        continue;

                    string filehash = hashLst[0];
                    string docNumber = hashLst[1];
                    //seleziono l'oggetto FR dal docnumber del token
                    DocsPaVO.documento.FileRequest fr = (from a in fileRequestList where a.docNumber == docNumber select a).FirstOrDefault() as DocsPaVO.documento.FileRequest;
                    FirmaResult firmres = new FirmaResult { fileRequest = fr };
                    
                    // Non viene settato nel FE
                    fr.inLibroFirma = BusinessLogic.LibroFirma.LibroFirmaManager.IsDocInLibroFirma(fr.docNumber);
                
                    //non trovato
                    if (fr == null)
                    {
                        logger.Debug("FR è null.. male male male!");
                        firmres.errore = "false: Filerequest nullo";
                    }
                    else
                    {
                        byte[] signed = ms.Get(filehash);
                        if (signed != null)
                        {
                            bool signResult = false;
                            
                            //if ((ms.TipoFirma == RemoteSignature.SignType.PADES) || ms.Cofirma)  //cades cofirmato, o pades, non gradico venga cambiata l'ext
                            //    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), true, ref fr, infoUtente, true);
                            //else   //cades non cofirmato
                            //    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), false, ref fr, infoUtente, true);
                            if (ms.TipoFirma == RemoteSignature.SignType.PADES)
                            {
                                signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(signed, ms.Cofirma, ref fr, infoUtente);
                            }
                            else
                            {
                                signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(signed), ms.Cofirma, ref fr, infoUtente);
                            }
                            if (!signResult)
                            {
                                firmres.errore = "false: Errore creando la nuova versione firmata";
                                if (fr.inLibroFirma)
                                {
                                    string[] splitMsg = firmres.errore.Split(':');
                                    BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fr.docNumber, splitMsg[1].ToString());
                                }
                            }
                            else
                            {
                                string method = "DOC_SIGNATURE";
                                string description = "Il documento è stato firmato digitalmente HSM CADES";
                                if (ms.TipoFirma == RemoteSignature.SignType.PADES)
                                {
                                    method = "DOC_SIGNATURE_P";
                                    description = "Il documento è stato firmato digitalmente HSM PADES";
                                }
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, fr.docNumber,
                                    description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0");

                                firmres.errore = "true: Versione creata con successo";
                             
                                //MEV LIBRO FIRMA EMANUELA 12-06-2015: Se il documento è in libro firma aggiorno la data di esecuzione
                                if (fr.inLibroFirma)
                                {
                                    BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(fr.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                                    BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fr.docNumber, description, infoUtente);
                                }
                            }                       
                        }
                        else
                        {
                            firmres.errore = "false: Errore firmando remotamente il documento";
                        }
                    }
                    retval.Add(firmres);
                }
                ms.CloseSession();
            }
            else
            {
                //andato male.. CIAO CIAO
                logger.Debug("La firma multipla è andata male... esco e chiudo la sessione");
                ms.CloseSession();
                string id = ExtractFromString(error, "#CODE", "#MESSAGE");
                esitoFirma = GetEsitoFirma(id);
                error = ExtractFromString(error, "#MESSAGE", "#TYPE");
                retval.Add(new FirmaResult { errore = string.IsNullOrEmpty(error) ? "false: La firma multipla è fallita globalmente" : error, esito =  esitoFirma});

                foreach (FileRequest fr in fileRequestList)
                {
                    fr.inLibroFirma = BusinessLogic.LibroFirma.LibroFirmaManager.IsDocInLibroFirma(fr.docNumber);
                    if (fr.inLibroFirma)
                    {
                        string msg = string.IsNullOrEmpty(error) ? "Errore durante la procedura di firma" : error;
                        BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaErroreEsitoFirma(fr.docNumber, msg);
                    }
                }
            }
            return retval.ToArray();
        }

        private static string ExtractFromString(string text, string startString, string endString)
        {
            string result = text;
            int indexStart = 0, indexEnd = 0;
            indexStart = text.IndexOf(startString);
            indexEnd = text.IndexOf(endString);
            if (indexStart > 0 && indexEnd > 0)
            {
                indexStart += startString.Length;
                try
                {
                    result = text.Substring(indexStart, indexEnd - indexStart);
                }
                catch(Exception e)
                {
                    return result;
                }
            }
            return result;
        }
        #endregion
        #region RemotePdfStamping
        public static bool RemotePdfStamp(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr,String StampText)
        {
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
            if (fd.content == null)
            {
                logger.Debug("Lo il reperimento del documento è andato male content==null esco.");
                return false;
            }

            BusinessLogic.Documenti.DigitalSignature.RemotePdfStamper remoteStamp = new RemotePdfStamper(fd.content, StampText);
            if (remoteStamp.StampedPDF == null)
            {
                logger.Debug("Lo stamping remoto non ha restituito nulla.");
                return false;
            }
            bool signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(remoteStamp.StampedPDF), true, ref fr, infoUtente);
            return signResult;
        }

        public static bool RemotePdfStamp(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, int Page, int LeftX, int LeftY, int RightX, int RightY, String StampText)
        {
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);

            BusinessLogic.Documenti.DigitalSignature.RemotePdfStamper remoteStamp = new RemotePdfStamper(Page,LeftX,LeftY ,RightX ,RightY, fd.content, StampText);
            if (fd.content == null)
            {
                logger.Debug("Lo il reperimento del documento è andato male content==null esco.");
                return false;
            }

            if (remoteStamp.StampedPDF == null)
            {
                logger.Debug("Lo stamping remoto con parametri non ha restituito nulla.");
                return false;
            }
            bool signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmato(Convert.ToBase64String(remoteStamp.StampedPDF), true, ref fr, infoUtente);
            return signResult;
        }


        #endregion

        #region ElectronicSignature

        public static List<DocsPaVO.LibroFirma.FirmaElettronica> GetElectronicSignatureDocument(string docnumber, string versionId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.LibroFirma.FirmaElettronica> listElectronicSignature = new List<DocsPaVO.LibroFirma.FirmaElettronica>();
            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                listElectronicSignature = documenti.GetElectronicSignatureDocument(docnumber, versionId, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in BusinessLogic.Documenti.FileManager  - metodo: GetElectronicSignatureDocument ", ex);
                return null;
            }
            return listElectronicSignature;
        }

        #endregion

        /// <summary>
        /// Linearizza un file PDF nel caso non lo fosse già
        /// </summary>
        /// <param name="contentFile">byte[]</param>
        /// /// <returns>byte[]</returns>
        public static FileDocumento LinearizzePDFContent(FileDocumento contentFile)
        {
            FileDocumento tempFile = contentFile;

            PDFLinearizator.ConvertedPDF cpdf = PDFLinearizator.PDFLinearizator.LinearizePDFfromContent(contentFile.content, contentFile.nomeOriginale);

            if (cpdf != null && cpdf.ConvertedContent != null && cpdf.ConvertedContent.Length > 0)
            {
                tempFile.content = cpdf.ConvertedContent;
                tempFile.length = tempFile.content.Length;
            }
            else
                tempFile = contentFile;

            return tempFile;
        }

        /// <summary>
        /// Linearizza un file PDF nel caso non lo fosse già
        /// </summary>
        /// <param name="contentFile">byte[]</param>
        /// /// <returns>byte[]</returns>
        public static FileDocumentoAnteprima LinearizzePDFContent(FileDocumentoAnteprima contentFile)
        {
            FileDocumentoAnteprima tempFile = contentFile;

            PDFLinearizator.ConvertedPDF cpdf = PDFLinearizator.PDFLinearizator.LinearizePDFfromContent(contentFile.content, contentFile.nomeOriginale);

            if (cpdf != null && cpdf.ConvertedContent != null && cpdf.ConvertedContent.Length > 0)
            {
                tempFile.content = cpdf.ConvertedContent;
                tempFile.length = tempFile.content.Length;
            }
            else
                tempFile = contentFile;

            return tempFile;
        }

        /// <summary>
        /// Controlla la firma pades
        /// </summary>
        /// <param name="objFileRequest"></param>
        /// <param name="objSicurezza"></param>
        /// /// <returns>Boolean</returns>
        public static Boolean IsPdfPades(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Boolean isPades = false;
            FileDocumento fileDocumento = null;
            String extension = Path.GetExtension(fileRequest.fileName);
            if (fileRequest != null && extension != null && extension.ToLowerInvariant() == ".pdf")
            {
                fileDocumento = FileManager.getFile(fileRequest, infoUtente, false, false);
                isPades = BusinessLogic.Documenti.DigitalSignature.Pades_Utils.Pades.IsPdfPades(fileDocumento);
            }

            return isPades;
        }

        public static void InsertInfoFile(string idProfile, string versionId, string idDocumentoPrincipale)
        {
            try
            {
                InfoFile infoFile = new InfoFile();
                infoFile.IdProfile = idProfile;
                infoFile.VersionId = versionId;
                infoFile.IdDocumentoPrincipale = idDocumentoPrincipale;

                //Inserisco il record in DPA_INFO_FILE per registrare le informazioni del file acquisito
                DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                docs.InsertInfoFile(infoFile);

            }
            catch(Exception e)
            {
                logger.Error("Errore in InsertInfoFile " + e.Message);
            }
        }

        /// <summary>
        /// Aggiorna l'infoFile a seguito di una acquisizione
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileRequest"></param>
        /// <param name="fileExtFileTypeFinder"></param>
        /// <param name="objSicurezza"></param>
        public static void UpdateInfoFileAcquisito(DocsPaVO.documento.FileInformation fileInfo, FileRequest fileRequest, string varNomeOriginale, string fileExtFileTypeFinder, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                bool notifica = false;
                InfoFile infoFile = new InfoFile();
                infoFile.IdProfile = fileRequest.docNumber;
                infoFile.VersionId = fileRequest.versionId;
                infoFile.NomeFile = varNomeOriginale;
                infoFile.Estensione = getEstensioneIntoSignedFile(fileRequest.fileName);
                if (fileInfo.FileFormatOK.Equals(FileInformation.VerifyStatus.Invalid))
                    infoFile.EstensioneConforme = false;

                if (fileInfo.NoMacroOrExe.Equals(FileInformation.VerifyStatus.Invalid))
                {
                    infoFile.Conforme = false;
                    if (fileExtFileTypeFinder.ToLower().Contains("+macro") || fileExtFileTypeFinder.ToLower().Contains("docm") ||
                        fileExtFileTypeFinder.ToLower().Contains("xlsm") || fileExtFileTypeFinder.ToLower().Contains("pptm"))
                    {
                        infoFile.ContieneMacro = true;
                    }
                    infoFile.ContieneForms = fileExtFileTypeFinder.ToLower().Contains("+forms");
                    infoFile.ContieneJavascript = fileExtFileTypeFinder.ToLower().Contains("+javascript");

                    if(infoFile.ContieneMacro)
                        infoFile.DescrizioneInfoFile += "MACRO";

                    if (infoFile.ContieneForms)
                        infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "FormPDF" : ",FormPDF";

                    if (infoFile.ContieneJavascript)
                        infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "JAVASCRIPT" : ",JAVASCRIPT";
                }

                if (!infoFile.EstensioneConforme)
                    infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "NON_CONFORME" : ",NON_CONFORME";
                //infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "Estensione non conforme al formato di file" : ",Estensione non conforme al formato di file";

                infoFile.DataAcquisizione = fileRequest.dataAcquisizione;

                DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                if (!infoFile.Conforme)
                {
                    infoFile.IdDocumentoPrincipale = docs.GetIdDocumentoPrincipale(infoFile.IdProfile);
                    notifica = true;
                }

                //Aggiorno le informazione del file acquisito
                docs.UpdateInfoFile(infoFile, notifica);
            }
            catch (Exception e)
            {
                logger.Error("Errore in UpdateInfoFileAcquisito: " + e.Message);
            }
        }

        /// <summary>
        /// Svuota la info file del documento per ultima versione del file non acquisita
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="infoUtente"></param>
        public static void UpdateInfoFileNonAcquisito(FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                InfoFile infoFile = new InfoFile();
                infoFile.IdProfile = fileRequest.docNumber;
                infoFile.VersionId = fileRequest.versionId;

                //Aggiorno le informazione del file acquisito
                DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                docs.UpdateInfoFile(infoFile);
            }
            catch (Exception e)
            {
                logger.Error("Errore in UpdateInfoFileNonAcquisito: " + e.Message);
            }
        }

        /// <summary>
        /// Aggiorno l'infoFile con l'ultima versione del documento
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="infoUtente"></param>
        public static void UpdateInfoFileConUltimaVersione(string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                //Estraggo l'ultima versione del documento
                Documento[] documenti = BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(infoUtente, docnumber);
                if (documenti != null && documenti.Count() > 0)
                {
                    FileRequest newFileRequest = documenti[0];
                    if (Convert.ToInt32(newFileRequest.fileSize) > 0)
                    {
                        FileInformation fileInfo = getFileFileInformation(newFileRequest, infoUtente);

                        //1) prelevo il file
                        DocsPaVO.documento.FileDocumento filedoc = getFile(newFileRequest, infoUtente);
                        Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
                        string fileExtension = ff.FileType(filedoc.content);

                        //Aggiorno le informazione del file acquisito
                        UpdateInfoFileAcquisito(fileInfo, newFileRequest, filedoc.nomeOriginale, fileExtension, infoUtente);
                    }
                    else
                    {
                        //Se l'ultima versione non ha il file acquisito, svuoto la riga
                        UpdateInfoFileNonAcquisito(newFileRequest, infoUtente);
                    }
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in UpdateInfoFileConUltimaVersione " + e.Message);
            }
        }
    }
}
