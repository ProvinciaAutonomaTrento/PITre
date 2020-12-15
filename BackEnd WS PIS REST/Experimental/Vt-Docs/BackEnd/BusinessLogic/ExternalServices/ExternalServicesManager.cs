using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.ExternalServices;

namespace BusinessLogic.ExternalServices
{
    public class ExternalServicesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaDB.Query_DocsPAWS.Fatturazione));

        public List<Servizio> getServizi()
        {
            List<Servizio> retList = new List<Servizio>();
         
            try {
                
                DocsPaDB.Query_DocsPAWS.ExternalServices externalServices = new DocsPaDB.Query_DocsPAWS.ExternalServices();
                List<Servizio> servizi = externalServices.getServizi();

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }

            return retList;
        }

        public List<Servizio> getServizi(string docNumber)
        {
            List<Servizio> retList = new List<Servizio>();

            try
            {

                DocsPaDB.Query_DocsPAWS.ExternalServices externalServices = new DocsPaDB.Query_DocsPAWS.ExternalServices();
                List<Servizio> servizi = externalServices.getServizi(docNumber);

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }

            return retList;
        }

        public static string AvviaAzioniEsterne(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, int diagramId)
        {
            string esitoAzione = "-1";

            DocsPaDB.Query_DocsPAWS.ExternalServices externalServices = new DocsPaDB.Query_DocsPAWS.ExternalServices();

            try
            {
                List<Servizio> servizi = new List<Servizio>();

                try
                {
                    servizi = externalServices.getServizi(docNumber);
                }
                catch (Exception e)
                { 
                    //Database disallineato
                }

                if (servizi.Count > 0)
                {
                    foreach (Servizio servizio in servizi)
                    {
                        if (servizio.getDescrizione() == "SERVIZIO FATTURAZIONE")
                        {
                            esitoAzione = InviaASdi(infoUtente, docNumber, diagramId.ToString(), servizio.getCodiceEsecutore());
                        }
                        else
                            esitoAzione = (EseguiAzione(servizio) ? "1" : "-1");
                    }
                }
                else
                {
                    esitoAzione = "0";
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in servizi esterni  - metodo: AvviaAzioniEsterne", e);
            }

            return esitoAzione;
        }

        private static bool EseguiAzione(Servizio servizio)
        {
            bool retVal = false;

            string parametri;

            foreach (Dictionary<String, String> parametro in servizio.getParametri())
            {
                                
            }

            return retVal;
        }

        private static string InviaASdi(DocsPaVO.utente.InfoUtente infoUtente, string docnumber, string diagramId, string codEsecutore)
        {
            logger.Debug("BEGIN");
            string retVal = "-1";
            string identificativo_Invio = string.Empty;
            string dbKey = (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SDI_VIA_PEC"));
            bool PEC_ENABLED = (string.IsNullOrEmpty(dbKey) || dbKey == "0"?false:true) ;

            try
            {
                string fileNameSdi = getNomeSDI(codEsecutore, docnumber);
                DocsPaVO.documento.SchedaDocumento schedaDoc = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                DocsPaVO.documento.FileRequest docPrincipale = (DocsPaVO.documento.FileRequest) schedaDoc.documenti[0];
                DocsPaVO.documento.FileDocumento fileDoc = Documenti.FileManager.getFile(docPrincipale, infoUtente);

                string nomeOld = fileDoc.name;
                string extOriginalName = System.IO.Path.GetExtension(fileDoc.nomeOriginale).ToLower();

                fileDoc.nomeOriginale = fileNameSdi + ".xml";

                if (!extOriginalName.ToUpper().Equals("XML") && !extOriginalName.ToUpper().Equals(".XML"))
                    fileDoc.nomeOriginale += extOriginalName; 
                
                fileDoc.name = fileDoc.nomeOriginale;

                bool prosegui = true;
                if (PEC_ENABLED)
                {
                    #region OLD CODE
                    //prosegui = docPrincipale.firmato.Equals("1");
                    #endregion

                    // Gabriele Melini 28-02-2017
                    // Prima dell'invio a SDI se il file non è firmato devo firmarlo utilizzando il servizio
                    // di firma HSM automatica
                    if (!docPrincipale.firmato.Equals("1"))
                    {
                        logger.Debug("FILE DA FIRMARE");
                        string rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");

                        string inputPath = System.IO.Path.Combine(rootPath, string.Format(@"SignXML\Input\{0}\{1}", docPrincipale.versionId, fileDoc.nomeOriginale));
                        string outputPath = System.IO.Path.Combine(rootPath, string.Format(@"SignXML\Output\{0}\{1}.P7M", docPrincipale.versionId, fileDoc.nomeOriginale));

                        if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(inputPath)))
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(inputPath));
                        if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(outputPath)))
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputPath));

                        // Al momento il firmatario coincide con l'utente responsabile della conservazione
                        // in futuro andrebbe gestito un po' meglio
                        Conservazione.ConservazioneManager cman = new Conservazione.ConservazioneManager();
                        string idUtente = cman.GetIdUtenteRespConservazione(infoUtente.idAmministrazione);
                        string idRuolo = cman.GetIdRuoloRespConservazione(infoUtente.idAmministrazione);

                        if(string.IsNullOrEmpty(idUtente) || string.IsNullOrEmpty(idRuolo)) 
                        {
                            prosegui = false;
                        }
                        else
                        {
                            DocsPaVO.utente.InfoUtente signer = new DocsPaVO.utente.InfoUtente();
                            DocsPaVO.utente.Utente u = BusinessLogic.Utenti.UserManager.getUtenteById(idUtente);
                            DocsPaVO.utente.Ruolo r = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idRuolo);

                            signer = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);
                            if (signer.dst == null)
                            {
                                signer.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                            }

                            // Scrivo su file system il file da firmare
                            System.IO.File.WriteAllBytes(inputPath, fileDoc.content);

                            // Chiamo il servizio di firma automatica
                            byte[] signedContent = Documenti.FileManager.HSM_AutomaticSignature(signer, inputPath, outputPath);

                            if (signedContent != null)
                            {
                                logger.Debug("signed content length: " + signedContent.Length);

                                // Rimuovo il file dal path temporaneo
                                try
                                {
                                    if (System.IO.File.Exists(inputPath))
                                    {
                                        System.IO.File.Delete(inputPath);
                                    }
                                }
                                catch (Exception exc)
                                {
                                    logger.ErrorFormat("Rimozione file temporanei non riuscita: {0}", exc.Message);
                                }

                                // Aggiungo la versione
                                prosegui = Documenti.SignedFileManager.AppendDocumentoFirmato(signedContent, false, ref docPrincipale, infoUtente);
                                if (prosegui)
                                {
                                    // Ricarico il file firmato
                                    schedaDoc = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                                    fileDoc = BusinessLogic.Documenti.FileManager.getFileFirmato((DocsPaVO.documento.FileRequest)schedaDoc.documenti[0], infoUtente, false);
                                }
                            }
                            else
                            {
                                logger.Debug("Firma automatica non riuscita!");
                                prosegui = false;
                            }
                        }

                        
                    }
                }

                if (docPrincipale != null && prosegui)
                    {
                        logger.Debug("INVIO A SDI");                
                        identificativo_Invio = SdiConnector.Connector.SendFileToSdi(fileDoc);
                        logger.Debug("RESULT: " + identificativo_Invio);


                        if (PEC_ENABLED)
                        {
                            //if (!string.IsNullOrEmpty(identificativo_Invio))
                            //if (string.IsNullOrEmpty(identificativo_Invio))
                            //{
                                logger.Debug("INVIO VIA PEC");
                                string retvalPec = SdiConnector.Connector.SendFileToSdiViaPec(fileDoc, docnumber);
                                logger.Debug("RESULT: " + retvalPec);
                                string[] pecResp = retvalPec.Split(':');
                                if (pecResp[0].ToLower() == "false")
                                {
                                    // se va in errore annullo l'identificativo invio
                                    identificativo_Invio = string.Empty;
                                }
                            //}
                        }


                    fileDoc.name = nomeOld;

                    if (!string.IsNullOrEmpty(identificativo_Invio))
                    {
                        addNota(infoUtente, schedaDoc, fileNameSdi);

                        Documenti.FileManager.setOriginalFileName(infoUtente, docPrincipale, fileDoc.nomeOriginale, true);

                        if (Fatturazione.FatturazioneManager.PutProfileFattura(docnumber, identificativo_Invio, diagramId))
                            retVal = "1";
                        else
                            retVal = "0";
                    }
                    else
                    {
                        retVal = "-2";
                    }
                }
                else
                {
                    retVal = "-3";
                }
            }
            catch (Exception ex)
            {
                retVal = "-1";
            }

            return retVal;
        }

        private static void addNota(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string identificativo_Invio)
        {
            DocsPaVO.Note.AssociazioneNota oggettoAssociato = new DocsPaVO.Note.AssociazioneNota();
            oggettoAssociato.TipoOggetto = DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento;
            oggettoAssociato.Id = schedaDoc.systemId;

            DocsPaVO.Note.InfoNota new_note = new DocsPaVO.Note.InfoNota();
            new_note.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
            new_note.Testo = identificativo_Invio;
            new_note.DataCreazione = System.DateTime.Now;
            new_note.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
            new_note.UtenteCreatore.IdUtente = infoUtente.idPeople;
            new_note.UtenteCreatore.DescrizioneUtente = infoUtente.userId;

            new_note.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
            //invoiceName_note.UtenteCreatore.DescrizioneRuolo = ruolo.descrizione;

            DocsPaVO.Note.InfoNota invoiceName_note = BusinessLogic.Note.NoteManager.InsertNota(infoUtente, oggettoAssociato, new_note);
            schedaDoc.noteDocumento.Insert(0,invoiceName_note);
        }

        private static string getNomeSDI(string codEsec, string docNum)
        {
            string nomeFileSDI = string.Empty; //IT00513990010_AB006

            if (string.IsNullOrEmpty(codEsec))
            {
                codEsec = "XX00000000000";
            }
            else if (codEsec.Length < 13)
            {
                string nazione = codEsec.Substring(0, 2);
                string temp = "000000000" + codEsec.Substring(2);
                codEsec = nazione + temp.Substring(temp.Length - (codEsec.Length - 2), 11);
            }

            string univocoInvio = getDocNumberSDI(docNum);

            nomeFileSDI = codEsec + "_" + univocoInvio;

            return nomeFileSDI;
        }

        private static string getDocNumberSDI(string docNumb)
        {
            int value = Convert.ToInt32(docNumb);
            string result = string.Empty;
            char[] baseChars = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
            int targetBase = baseChars.Length;

            do
            {
                result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            if (string.IsNullOrEmpty(result))
            {
                result = "00000";
            }
            else
            {
                if (result.Length < 5)
                {
                    string temp = "00000" + result;
                    result = temp.Substring(result.Length);
                }
            }

            return result;
        }
    }
}
