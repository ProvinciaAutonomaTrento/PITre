using System;
using System.Collections;
using System.Data;
using log4net;
using System.Collections.Generic;

namespace BusinessLogic.Documenti
{
    public class TimestampManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(TimestampManager));

        public static DocsPaVO.areaConservazione.OutputResponseMarca executeAndSaveTSR(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.areaConservazione.InputMarca richiesta, DocsPaVO.documento.FileRequest fileRequest)
        {
            // La marca temporale da restituire
            DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = null;

            try
            {
                //Scelta del tipo di implementazione per la richiesta della marca temporale
                string typeName = System.Configuration.ConfigurationManager.AppSettings["TYPE_TSA"];
                logger.Debug("Istanza TYPE_TSA = " + typeName);

                Type instanceType = Type.GetType(typeName, false);
                if (instanceType != null)
                {
                    logger.Debug("Istanza TYPE_TSA = " + typeName + " non nulla");
                    DocsPa_I_TSAuthority.I_TSR_Request instance = (DocsPa_I_TSAuthority.I_TSR_Request)Activator.CreateInstance(instanceType);
                    resultMarca = new DocsPaVO.areaConservazione.OutputResponseMarca();

                    //Ottengo una marca temporale in base alla specifica implementazione settata nel web.config
                    logger.Debug("Richiesta marca temporale");
                    resultMarca = instance.getTimeStamp(richiesta);
                    logger.Debug("Fine richiesta marca temporale");

                    //Genero l'array di byte per il file p7m e TSR
                    byte[] p7m = String_To_Bytes(richiesta.file_p7m);
                    byte[] TSR = Convert.FromBase64String(resultMarca.marca);

                    //Verifico la marca e completo l'oggetto OutputResponseMarca
                    logger.Debug("Verifica marca temporale");
                    resultMarca = VerificaMarca(p7m, TSR);
                    logger.Debug("Fine verifica marca temporale");

                    //Salvo la marca generata sul database
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        try
                        {
                            logger.Debug("Salvataggio TSR");
                            DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                            timestampDoc.saveTSR(infoUtente, resultMarca, fileRequest);
                            transactionContext.Complete();
                            FileManager.processFileInformation(fileRequest, infoUtente);
                            logger.Debug("Fine Salvataggio TSR");
                        }
                        catch (Exception e)
                        {
                            logger.DebugFormat("ERRORE : Salvataggio marca temporale sul database : {0} \r\nstk\r\n{1}" + e.Message,e.StackTrace);
                        }
                    }
                }
                else
                {
                    logger.Debug("Istanza TYPE_TSA = " + typeName + " nulla");
                }
            }
            catch (Exception eMarca)
            {
                logger.DebugFormat("ERRORE : Richiesta marca temporale :{0} \r\nstk{1}\r\n " + eMarca.Message, eMarca.StackTrace);
            }

            return resultMarca;
        }

        public static ArrayList getTimestampsDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                return timestampDoc.getTimestampsDoc(infoUtente, fileRequest);                
            }
            catch (Exception e)
            {
                logger.Debug("ERRORE : Restituzioni marche temporali del documento : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Ritorna il numero di timestamp associati al documento 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public static int getCountTimestampsDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                return timestampDoc.getCountTimestampsDoc(infoUtente, fileRequest.docNumber, fileRequest.versionId);
            }
            catch (Exception e)
            {
                logger.Debug("ERRORE : Restituzione del numero delle marche temporali del documento : " + e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Ritorna il numero di timestamp associati al documento (il documento è identificato tramite DocNumber e VersionID)
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public static int getCountTimestampsDocLite(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, string versionId)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                return timestampDoc.getCountTimestampsDoc(infoUtente, docNumber, versionId);
            }
            catch (Exception e)
            {
                logger.Debug("ERRORE : Restituzione del numero delle marche temporali del documento (LITE) : " + e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Convert hex string to byte_array
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;
            // x variable used to hold byte array element position
            int x = 0;
            // allocate byte array based on half of string length
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting
            //  it to decimal equivalent and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values
            return bytes;
        }

        /// <summary>
        /// Verifica la marca ottenuta in risposta ed estrae tutti i dati dalla marca completando i dati dell'oggetto OutputResponseMarca con quelli letti direttamente dalla marca!
        /// </summary>
        /// <param name="filep7m"></param>
        /// <param name="fileTSR"></param>
        /// <returns></returns>
        private static DocsPaVO.areaConservazione.OutputResponseMarca VerificaMarca(byte[] filep7m, byte[] fileTSR)
        {
            DocsPaVO.areaConservazione.OutputResponseMarca outputMarca = new DocsPaVO.areaConservazione.OutputResponseMarca();
            DigitalSignature.VerifyTimeStamp checkMarca = new DigitalSignature.VerifyTimeStamp();       
            outputMarca = checkMarca.Verify(filep7m, fileTSR);

            return outputMarca;
        }

        /// <summary>
        /// Crea una nuova versione da un documento acquisito, se è associato un TSR, verrà unito al documento per creare un TSD
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileRequest CreateTSDVersion (DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
        {
            byte []  lastTSR = BusinessLogic.Interoperabilità.InteroperabilitaUtils.GetTSRForDocument(infoUtente, fileRequest);
            if (lastTSR == null)
                return null;

            //Conversione in TSD
            DocsPaVO.documento.FileDocumento docFile = FileManager.getFileFirmato(fileRequest, infoUtente, false);
            BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new DigitalSignature.VerifyTimeStamp();
            if (vts.Verify(docFile.content, lastTSR).esito != "OK")
                return null;

            try
            {
                BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.tsd tsdMgr = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.tsd();
                BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile file = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = docFile.content, Name = docFile.name, MessageFileType = BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.fileType.Binary };
                List<BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile> tsrLst = new List<BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile>();
                BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile tsrfile = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = lastTSR, MessageFileType = BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.fileType.Binary };
                tsrLst.Add(tsrfile);
                docFile.content = tsdMgr.create(file, tsrLst.ToArray()).Content;
                docFile.estensioneFile = "tsd";
                docFile.fullName = docFile.fullName + ".tsd";
                docFile.length = (int)docFile.content.Length;
                docFile.name = docFile.name + ".tsd";
                docFile.nomeOriginale = docFile.nomeOriginale + ".tsd";
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore creando il TSD!! {0},{1}", e.Message, e.StackTrace);
                return null;
            }
            DocsPaVO.documento.FileRequest tsdFileReq;
            if (fileRequest.GetType() == typeof(DocsPaVO.documento.Allegato))
            {
                tsdFileReq = new DocsPaVO.documento.Allegato();
                (tsdFileReq as DocsPaVO.documento.Allegato).numeroPagine = (fileRequest as DocsPaVO.documento.Allegato).numeroPagine;
            }
            else
            {
                tsdFileReq = new DocsPaVO.documento.Documento();
            }

            tsdFileReq.docNumber = fileRequest.docNumber;
            tsdFileReq.descrizione = "Versione creata per conversione in TSD";
            tsdFileReq.firmato = "1";
            tsdFileReq = BusinessLogic.Documenti.VersioniManager.addVersion(tsdFileReq, infoUtente, false);
            
              if (tsdFileReq != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", tsdFileReq.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunta al N.ro Doc.: ", tsdFileReq.docNumber, " la Ver. ", tsdFileReq.version+" di tipo TSD"), DocsPaVO.Logger.CodAzione.Esito.OK);
                
              else

                  BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", tsdFileReq.docNumber, string.Format("{0}{1}{2}{3}", "Aggiunta al N.ro Doc.: ", tsdFileReq.docNumber, " la Ver. ", tsdFileReq.version + " di tipo TSD"), DocsPaVO.Logger.CodAzione.Esito.KO);
                
           
            tsdFileReq = BusinessLogic.Documenti.FileManager.putFile(tsdFileReq,docFile, infoUtente);

            List<DocsPaVO.LibroFirma.FirmaElettronica> firmaE = LibroFirma.LibroFirmaManager.GetFirmaElettronicaDaFileRequest(fileRequest);
            bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
            if (isFirmatoElettonicamente)
            {
                string impronta = string.Empty;
                DocsPaDB.Query_DocsPAWS.Documenti docInfoDB = new DocsPaDB.Query_DocsPAWS.Documenti();
                docInfoDB.GetImpronta(out impronta, tsdFileReq.versionId, tsdFileReq.docNumber);

                foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaE)
                {
                    firma.UpdateXml(impronta, tsdFileReq.versionId, tsdFileReq.version);
                    LibroFirma.LibroFirmaManager.InserisciFirmaElettronica(firma);
                }
            }

            return tsdFileReq;
        }
    }
}
