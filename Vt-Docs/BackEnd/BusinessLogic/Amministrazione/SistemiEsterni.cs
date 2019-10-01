using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections;

namespace BusinessLogic.Amministrazione
{
    public class SistemiEsterni
    {
        private static ILog logger = LogManager.GetLogger(typeof(SistemiEsterni));

        public static ArrayList getSistemiEsterni(string idAmm)
        {
            ArrayList retval = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            retval = DBAmm.getSistemiEsterni(idAmm);

            return retval;
        }

        public static ArrayList getPISMethods()
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return DBAmm.getPISMethods();
        }

        public static bool ModificaMetodiPermessiSistemaEsterno(string metodi, string idSysExt)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.ModificaMetodiPermessiSistemaEsterno(metodi, idSysExt);
        }

        public static bool ModificaDescTokenSistemaEsterno(string descrizione, string token, string idSysExt)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.ModificaDescTokenSistemaEsterno(descrizione, token, idSysExt);
        }

        public static DocsPaVO.utente.TipoRuolo getTipoRuoloByCode(string idAmm, string codice)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.getTipoRuoloByCode(codice, idAmm);
        }

        public static bool InsSysExtAfterAssoc(DocsPaVO.utente.InfoUtente infut, string idAmm, string codUtente, string codRuolo, string descrizione)
        {
            bool retval = false;
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(codUtente, idAmm);
            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codRuolo);

            BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsUtenteInRuolo(infut, utente.idPeople, ruolo.idGruppo);
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            retval = DBAmm.insNuovoSistemaEsterno(codRuolo, codUtente, ruolo.systemId, idAmm, descrizione);

            if (retval)
            {
                retval = DBAmm.setSystemUser(utente.idPeople);
            }
            return retval;
        }

        public static DocsPaVO.amministrazione.SistemaEsterno getSistemaEsterno(string idAmm, string codApplicazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBAmm.getSistemaEsterno(idAmm, codApplicazione);
        }

        public static DocsPaVO.amministrazione.SistemaEsterno getSistemaEsternoByUserid(string idAmm, string userId)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBamm.getSistemaEsternoByUserID(idAmm, userId);
        }

        public static DocsPaVO.utente.UnitaOrganizzativa getHubSistemaEsterno(string codice, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.utente.UnitaOrganizzativa retval = DBamm.getUOByCodAndIdAmm(codice, idAmm);
            if (retval != null && string.IsNullOrEmpty(retval.systemId))
                retval = null;
            return retval;
        }

        public static string ctrlInserimentoSistemaEsterno(string idAmm, string codUtente, string codRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBamm.ctrlInserimentoSistemaEsterno(idAmm, codUtente, codRuolo);
        }

        public static bool setVisibilityHubSysExt(string idHub)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione DBamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return DBamm.setVisibilityHubSysExt(idHub);
        }

        public static bool cessioneProprieta(string idOggetto, string idRuoloDest, string idUtenteDest, string idSysExt, string idUtSysExt)
        {

            DocsPaDB.Query_DocsPAWS.Trasmissione DBtrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaDB.Query_DocsPAWS.Utenti DBUt = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = DBUt.getRuoloByCodice(idSysExt);
            return DBtrasm.cessioneProprietaSistemaEsterno(idOggetto, idRuoloDest, idUtenteDest, ruolo.idGruppo, idUtSysExt);
        }

        public static bool deleteExtSys(DocsPaVO.amministrazione.SistemaEsterno sysExt, DocsPaVO.utente.InfoUtente infoUt)
        {
            bool retval = false;

            DocsPaDB.Query_DocsPAWS.Amministrazione DBAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaDB.Query_DocsPAWS.Utenti DbUt = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Amministrazione infoAmm = DBAmm.getInfoAmministrazione(sysExt.idAmministrazione);
            DocsPaVO.utente.Utente utenteNorm = DbUt.getUtenteByCodice(sysExt.UserIdAssociato, infoAmm.codice);
            DocsPaVO.amministrazione.OrgUtente utenteOrg = new DocsPaVO.amministrazione.OrgUtente();
            utenteOrg.IDPeople = utenteNorm.idPeople;
            utenteOrg.UserId = utenteNorm.userId;

            DocsPaVO.amministrazione.EsitoOperazione esito1 = OrganigrammaManager.AmmDisabilitaUtente(infoUt, utenteNorm.idPeople);
            retval = (esito1.Codice == 1 ? false : true);
            // Per eliminare utente e ruolo non vanno presi i metodi in amministrazione, ma quelli in organigramma...
            // Imitare il comportamento di gestioneUtenti.
            // 1. Dissociare utente da ruoli
            // 2. Eliminare utente
            // 3. Eliminare ruolo
            //retval = DBAmm.AmmEliminaUtente(utenteOrg);
            //// eliminazione ruolo (dopo utente)
            logger.Debug("Codice esito cancellazione utente: " + esito1.Codice);
            if (!string.IsNullOrEmpty(esito1.Descrizione))
            {
                logger.Debug(esito1.Descrizione);
            }
            if (retval)
            {
                DocsPaVO.amministrazione.OrgRuolo ruolo = DBAmm.GetRole(sysExt.idRuoloAssociato);
                //string delRuoloResult = DBAmm.AmmEliminaRuolo(ruolo);
                DocsPaVO.amministrazione.EsitoOperazione esito2 = OrganigrammaManager.AmmEliminaRuolo(infoUt, ruolo);
                logger.Debug("Codice esito cancellazione ruolo: " + esito2.Codice);
                if (!string.IsNullOrEmpty(esito2.Descrizione))
                {
                    logger.Debug(esito2.Descrizione);
                }
                retval = DBAmm.delExtSys(sysExt);
            }
            return retval;
        }

        public static bool AggiornaDirittiSistemaEsterno(string thing, string idGroup, string idPeople, string tipoDirittoAttuale, string tipoDirittoToSet, string accessRightsAttuale, string accessRightsLettura, string accessRightsToSet)
        {
            bool retValue = false;
            DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            string condizione = "THING = " + thing + " AND PERSONORGROUP IN (" + idGroup + "," + idPeople + ") AND ACCESSRIGHTS IN (" + accessRightsAttuale + "," + accessRightsLettura + ")";
            // Prelevamento dello stato del documento
            DocsPaVO.DiagrammaStato.Stato stato = DiagrammiStato.DiagrammiStato.getStatoDoc(thing);

            // Se lo stato è stato reperito correttamente ed è in stato finale, i diritti vanno impostati a 45
            // altrimenti si considera come documento non nello stato finale
            if (stato != null &&
                stato.STATO_FINALE)
            {
                // un documento in stato finale non può essere riportato a accessrights = 63
                retValue = obj.impostaDirittoEAggiornaAccessrights(tipoDirittoToSet, condizione, "45");
            }
            else
                retValue = obj.impostaDirittoEAggiornaAccessrights(tipoDirittoToSet, condizione, accessRightsToSet);

            retValue = obj.AggDirittiSysExtInDelSec(thing, idPeople, idGroup, accessRightsAttuale);

            return retValue;
        }

        public static bool CessioneTotaleDirittiSysExt(string idOggetto, string idUtSysExt, string idSysExt, string accessRights)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return obj.CessioneTotaleDirittiSysExt(idOggetto, idUtSysExt, idSysExt, accessRights);
        }

        public static bool CleanRightsExtSysAfterCreation(string idOggetto, string idUtSysExt, string idSysExt)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return obj.CleanRightsExtSysAfterCreation(idOggetto, idUtSysExt, idSysExt);

        }

        #region APSS - Delibere e determine - SOLR
        public static ArrayList APSSgetDelDetDaPubbl(string ogg_custom, string statiDiagramma, string templates, string dataUltimaEsecuzione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList retval = dbAmm.APSSgetDelDetDaPubbl(ogg_custom, statiDiagramma, templates, dataUltimaEsecuzione);

            return retval;
        }

        public static string APSSgetLastExecutionDate(string tipologie)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.APSSgetLastExecutionDate(tipologie);
        }


        public static DocsPaVO.ExternalServices.PubblicazioneAPSS APSSGetPubbByDocId(string idDocument)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.APSSGetPubbByDocId(idDocument);
        }

        public static bool APSSUpdateResultPubbInTable(DocsPaVO.ExternalServices.PubblicazioneAPSS pubb)
        {

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            try
            {
                // bisogna gestire il KO e il rifiuto della trasmissione in caso di ripubblicazione
                if (pubb.PublishResult == "KO")
                {
                    // Utenza codificata.
                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente("UTPUB", BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm("APSS"));
                    DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople)[0];
                    DocsPaVO.utente.InfoUtente infoUt = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                    //DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                    //documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUt, pubb.IdProfile, pubb.IdProfile);
                    //foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in documento.template.ELENCO_OGGETTI)
                    //{
                    //    if (ogg.DESCRIZIONE.ToUpper() == "ESITO PUBBLICAZIONE")
                    //    {
                    //        ogg.VALORE_DATABASE = "01/01/1900";
                    //    }
                    //}
                    //BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaInserimentoUtenteProfDim(infoUt, documento.template, pubb.IdProfile);
                    dbAmm.APSSUpdateEsitoPubb(pubb.IdProfile, "01/01/1900");

                    if (!string.IsNullOrEmpty(pubb.IdSingleTrasm))
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente[] ArrayTrasmUtente = BusinessLogic.Trasmissioni.TrasmManager.getTrasmissioneUtenteInRuolo(infoUt, pubb.IdSingleTrasm, utente);
                        if (ArrayTrasmUtente == null || ArrayTrasmUtente.Length < 1)
                        {
                            throw new Exception("Trasmissione relativa alla ripubblicazione non trovata");
                        }
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente = ArrayTrasmUtente[0];
                        trasmUtente.tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.RIFIUTO;
                        //trasmUtente.dataAccettata = DateTime.Now.ToString("dd/MM/yyyy");
                        if (!string.IsNullOrEmpty(pubb.PublishReason))
                            trasmUtente.noteRifiuto = "Ripubblicazione non effettuata per la seguente ragione: " + pubb.PublishReason;
                        else
                            trasmUtente.noteRifiuto = "Ripubblicazione non effettuata";

                        string idTrasm = BusinessLogic.Amministrazione.SistemiEsterni.APSSgetIdTrasmFromTUtente(trasmUtente.systemId);
                        string errore = "", mode = "", idObj = "";
                        bool effettuata = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmUtente, idTrasm, ruolo, infoUt, out errore, out mode, out idObj);
                        if (!effettuata)
                        {
                            throw new Exception("Errore nell'accettazione : " + errore);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }

            return dbAmm.APSSUpdateResultPubbInTable(pubb);
        }

        public static bool APSSInsertInPubTable(DocsPaVO.ExternalServices.PubblicazioneAPSS pubb)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.APSSInsertInPubTable(pubb);
        }

        public static string APSSgetIdTrasmFromTUtente(string idTrasmUtente)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione dbTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            // il metodo è stato sviluppato per un'altra funzione, ma fa quello che mi serve.
            return dbTrasm.GetInRispostaA(idTrasmUtente);
        }

        public static ArrayList APSSgetDelDetDaRiPubbl(string ogg_custom, string statiDiagramma, string templates, string dataUltimaEsecuzione, string tipoevento)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList retval = dbAmm.APSSgetDelDetDaRiPubbl(ogg_custom, statiDiagramma, templates, dataUltimaEsecuzione, tipoevento);

            return retval;
        }

        public static bool APSSCtrlAttachExt(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.APSSCtrlAttachExt(idDoc);
        }

        public static bool APSSUpdateEsitoPubb(string idDoc, string dataesito)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.APSSUpdateEsitoPubb(idDoc, dataesito);
        }
        #endregion
        #region Fattura Elettronica
        public static bool FattElEsitoNotifica(string idDoc, string esito)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.FattElEsitoNotifica(idDoc, esito);
        }

        public static string FattElCtrlDupl(string idTibco, string idsdi, out string passo, out string esito)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            string idFattura = dbAmm.FattElCtrlDupl(idTibco, idsdi, out passo, out esito);
            return idFattura;
        }

        public static bool FattElInsTabellaLog(string idDoc, string idTibco, string idsdi)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.FattElInsTabellaLog(idDoc, idTibco, idsdi);
        }

        public static bool FattElUpdTabellaLog(string idTibco, string idsdi, string passo, string esito)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.FattElUpdTabellaLog(idTibco, idsdi, passo, esito);
        }

        public static bool FattElEstrAllegatiFE(DocsPaVO.documento.SchedaDocumento doc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUt)
        {
            bool retVal = true;
            try
            {
                DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)doc.documenti[0];
                DocsPaVO.documento.FileDocumento fileDocumento = null;

                fileDocumento = BusinessLogic.Documenti.FileManager.getFile(versione, infoUt);
                if (fileDocumento != null && !string.IsNullOrEmpty(fileDocumento.fullName) && fileDocumento.fullName.ToUpper().EndsWith("XML"))
                {

                    string stringaXml = Encoding.UTF8.GetString(fileDocumento.content);

                    stringaXml = stringaXml.Trim();
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    if (stringaXml.Contains("xml version=\"1.1\""))
                    {
                        logger.Debug("Versione XML 1.1. Provo conversione");
                        stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                    }
                    xmlDoc.LoadXml(stringaXml);
                    if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                    {
                        System.Xml.XmlNodeList listanodi = xmlDoc.DocumentElement.SelectNodes("//*[name()='Allegati']");
                        System.Xml.XmlNode nodoNome = null;
                        System.Xml.XmlNode nodoContent = null;
                        DocsPaVO.documento.FileRequest allegato = null;
                        DocsPaVO.documento.FileDocumento fileAllegato = null;
                        string erroreMessage = "";
                        bool caricaAllegato = true;
                        if (listanodi != null && listanodi.Count > 0)
                        {
                            foreach (System.Xml.XmlNode nodo in listanodi)
                            {
                                nodoNome = null;
                                nodoContent = null;
                                caricaAllegato = true;

                                foreach (System.Xml.XmlNode nodo1 in nodo.ChildNodes)
                                {
                                    //Console.WriteLine(nodo1.Name);
                                    if (nodo1.Name.ToUpper() == "NOMEATTACHMENT") nodoNome = nodo1;
                                    if (nodo1.Name.ToUpper() == "ATTACHMENT") nodoContent = nodo1;
                                }


                                //byte[] contentNormal = Convert.FromBase64String(nodoContent.InnerXml);
                                foreach (DocsPaVO.documento.Allegato alltempx1 in doc.allegati)
                                {
                                    if (alltempx1.descrizione.ToUpper() == nodoNome.InnerXml.ToUpper()) caricaAllegato = false;
                                }
                                if (caricaAllegato)
                                {
                                    allegato = new DocsPaVO.documento.Allegato
                                    {
                                        docNumber = doc.systemId,
                                        descrizione = nodoNome.InnerXml
                                    };
                                    allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUt, ((DocsPaVO.documento.Allegato)allegato));

                                    fileAllegato = new DocsPaVO.documento.FileDocumento
                                    {
                                        name = nodoNome.InnerXml,
                                        fullName = nodoNome.InnerXml,
                                        content = Convert.FromBase64String(nodoContent.InnerXml),
                                        length = Convert.FromBase64String(nodoContent.InnerXml).Length,
                                        bypassFileContentValidation = true
                                    };


                                    if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileAllegato, infoUt, out erroreMessage))
                                    {
                                        throw new Exception("Errore nella creazione del file");
                                    }
                                }
                                else
                                {
                                    logger.Debug("Allegato già presente");
                                }

                            }
                        }
                    }
                    else
                    {
                        retVal = false;
                        logger.Debug("Il file non è una fattura valida");
                        logger.Debug("Controllo negli allegati");
                        retVal = FattElEstraiAllegatiDaAllegato(doc, infoUt);
                    }
                }
                else
                {
                    retVal = false;
                    logger.Debug("Il file non è un XML. Estrazione allegati non effettuata.");
                    logger.Debug("Controllo negli allegati");
                    retVal = FattElEstraiAllegatiDaAllegato(doc, infoUt);
                }

            }
            catch (Exception ex)
            {
                retVal = false;
                logger.Error(ex);
            }

            if (!retVal) logger.Debug("Allegati non trovati ne in file principale ne in allegati.");

            return retVal;
        }

        public static bool FattElEstraiAllegatiDaAllegato(DocsPaVO.documento.SchedaDocumento doc, DocsPaVO.utente.InfoUtente infoUt)
        {
            bool retVal = false;
            DocsPaVO.documento.FileDocumento fileAllFatt = null;
            string stringaXml = null;
            System.Xml.XmlDocument xmlDoc = null;
            try
            {
                foreach (DocsPaVO.documento.Allegato allFatt in doc.allegati)
                {
                    if (!string.IsNullOrEmpty(allFatt.fileName) && allFatt.fileName.ToUpper().Contains(".XML"))
                    {
                        fileAllFatt = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)allFatt, infoUt);

                        if (fileAllFatt != null && !string.IsNullOrEmpty(fileAllFatt.fullName) && fileAllFatt.fullName.ToUpper().EndsWith("XML"))
                        {
                            stringaXml = "";
                            stringaXml = Encoding.UTF8.GetString(fileAllFatt.content);

                            stringaXml = stringaXml.Trim();
                            xmlDoc = new System.Xml.XmlDocument();
                            if (stringaXml.Contains("xml version=\"1.1\""))
                            {
                                logger.Debug("Versione XML 1.1. Provo conversione");
                                stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                            }
                            try
                            {
                                xmlDoc.LoadXml(stringaXml);
                            }
                            catch (Exception bomUTF8)
                            {
                                string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                                if (stringaXml.StartsWith(byteOrderMarkUtf8))
                                {
                                    stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                                }
                                xmlDoc.LoadXml(stringaXml);
                            }
                            if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                            {
                                System.Xml.XmlNodeList listanodi = xmlDoc.DocumentElement.SelectNodes("//*[name()='Allegati']");
                                System.Xml.XmlNode nodoNome = null;
                                System.Xml.XmlNode nodoContent = null;
                                DocsPaVO.documento.FileRequest allegato = null;
                                DocsPaVO.documento.FileDocumento fileAllegato = null;
                                string erroreMessage = "";
                                bool caricaAllegato = true;
                                foreach (System.Xml.XmlNode nodo in listanodi)
                                {
                                    nodoNome = null;
                                    nodoContent = null;
                                    caricaAllegato = true;

                                    foreach (System.Xml.XmlNode nodo1 in nodo.ChildNodes)
                                    {
                                        //Console.WriteLine(nodo1.Name);
                                        if (nodo1.Name.ToUpper() == "NOMEATTACHMENT") nodoNome = nodo1;
                                        if (nodo1.Name.ToUpper() == "ATTACHMENT") nodoContent = nodo1;
                                    }


                                    //byte[] contentNormal = Convert.FromBase64String(nodoContent.InnerXml);
                                    foreach (DocsPaVO.documento.Allegato alltempx1 in doc.allegati)
                                    {
                                        if (alltempx1.descrizione.ToUpper() == nodoNome.InnerXml.ToUpper()) caricaAllegato = false;
                                    }
                                    if (caricaAllegato)
                                    {
                                        allegato = new DocsPaVO.documento.Allegato
                                        {
                                            docNumber = doc.systemId,
                                            descrizione = nodoNome.InnerXml
                                        };
                                        allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUt, ((DocsPaVO.documento.Allegato)allegato));

                                        fileAllegato = new DocsPaVO.documento.FileDocumento
                                        {
                                            name = nodoNome.InnerXml,
                                            fullName = nodoNome.InnerXml,
                                            content = Convert.FromBase64String(nodoContent.InnerXml),
                                            length = Convert.FromBase64String(nodoContent.InnerXml).Length,
                                            bypassFileContentValidation = true
                                        };


                                        if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileAllegato, infoUt, out erroreMessage))
                                        {
                                            throw new Exception("Errore nella creazione del file");
                                        }
                                        else
                                        {
                                            retVal = true;
                                        }
                                    }
                                    else
                                    {
                                        logger.Debug("Allegato già presente");
                                    }

                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                logger.Error(ex);
            }


            return retVal;
        }


        public static bool FattElCtrlPresenzaFattura(string idAmm)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_FATTURA_ASSOC_DA_PEC")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_FATTURA_ASSOC_DA_PEC") == "1")
            {
                DocsPaVO.ProfilazioneDinamica.Templates tipoFattura = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione("fattura elettronica", idAmm);
                if (tipoFattura != null)
                {
                    if (!string.IsNullOrEmpty(tipoFattura.CHA_ASSOC_MANUALE) && tipoFattura.CHA_ASSOC_MANUALE == "1")
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        public static bool FattElDaPEC(string idDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;
            DocsPaVO.documento.FileDocumento fileAllFatt = null;
            string stringaXml = null;
            System.Xml.XmlDocument xmlDoc = null;

            try
            {
                DocsPaVO.documento.SchedaDocumento doc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDoc);
                foreach (DocsPaVO.documento.Allegato allFatt in doc.allegati)
                {
                    if (!string.IsNullOrEmpty(allFatt.fileName) && allFatt.fileName.ToUpper().Contains(".XML"))
                    {
                        fileAllFatt = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)allFatt, infoUtente);

                        if (fileAllFatt != null && !string.IsNullOrEmpty(fileAllFatt.fullName) && fileAllFatt.fullName.ToUpper().EndsWith("XML"))
                        {
                            stringaXml = "";
                            stringaXml = Encoding.UTF8.GetString(fileAllFatt.content);

                            stringaXml = stringaXml.Trim();
                            xmlDoc = new System.Xml.XmlDocument();
                            if (stringaXml.Contains("xml version=\"1.1\""))
                            {
                                logger.Debug("Versione XML 1.1. Provo conversione");
                                stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                            }
                            try
                            {
                                xmlDoc.LoadXml(stringaXml);
                            }
                            catch (Exception bomUTF8)
                            {
                                string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                                if (stringaXml.StartsWith(byteOrderMarkUtf8))
                                {
                                    stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                                }
                                xmlDoc.LoadXml(stringaXml);
                            }
                            if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                            {
                                System.Xml.XmlNodeList fatture = xmlDoc.DocumentElement.SelectNodes("//*[name()='FatturaElettronicaBody']");
                                if (fatture.Count > 1)
                                {
                                    throw new Exception("Lotto di fatture. Associazione a fattura elettronica interrotta.");
                                }
                                else
                                {
                                    #region Associazione dei campi
                                    DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione("fattura elettronica", infoUtente.idAmministrazione);
                                    if (template != null && template.CHA_ASSOC_MANUALE == "1")
                                    {
                                        string notePerElaborazioneXML = "";
                                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                        {
                                            try
                                            {

                                                if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC))
                                                {
                                                    bool associaSecondo = false;
                                                    string[] mappings = oggettoCustom.CAMPO_XML_ASSOC.Split('<');
                                                    string[] mappingXml = mappings[0].Split('>');
                                                    string mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                    for (int i = 1; i < mappingXml.Length; i++)
                                                    {
                                                        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                    }
                                                    string valore = "";
                                                    try
                                                    {
                                                        System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                        valore = node.InnerXml; // valore dell'xml estratto
                                                    }
                                                    catch (Exception nodo)
                                                    {
                                                        if (mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                                                            associaSecondo = true;
                                                        else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                                        {
                                                            valore = "";
                                                        }
                                                        else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_NOTE_VER_FIRMA_1"))
                                                        {
                                                            //valore = "Verifica firma digitale effettuata da SDI, secondo le Specifiche tecniche operative delle Regole tecniche di cui all’allegato B del D.M. n. 55 del 3 aprile 2013 e ss.mm.ii.";
                                                            valore = "";
                                                        }
                                                        else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_IDSDI"))
                                                        {
                                                            valore = "";
                                                        }
                                                        else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_VERSIONE_1"))
                                                        {
                                                            System.Xml.XmlElement root = xmlDoc.DocumentElement;
                                                            valore = root.Attributes["versione"].Value;
                                                        }
                                                        else
                                                        {
                                                            notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                            //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                            //{
                                                            //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                            //}
                                                        }
                                                    }

                                                    if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CODFISC_1"))
                                                    {
                                                        if (!Char.IsLetter(valore[0]) || !Char.IsLetter(valore[1]) || !Char.IsLetter(valore[2]))
                                                            valore = "";
                                                    }

                                                    if (associaSecondo)
                                                    {
                                                        int associaSecondoI = 1;
                                                        while (string.IsNullOrEmpty(valore) && mappings.Length > associaSecondoI && !string.IsNullOrEmpty(mappings[associaSecondoI]))
                                                        {
                                                            mappingXml = mappings[associaSecondoI].Split('>');
                                                            mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                            for (int i = 1; i < mappingXml.Length; i++)
                                                            {
                                                                mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                            }
                                                            valore = "";
                                                            try
                                                            {
                                                                System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                                valore = node.InnerXml; // valore dell'xml estratto
                                                            }
                                                            catch (Exception nodo)
                                                            {

                                                            }
                                                            associaSecondoI++;
                                                        }
                                                        if (string.IsNullOrEmpty(valore))
                                                        {
                                                            notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                            //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                            //{
                                                            //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                            //}

                                                        }
                                                    }

                                                    // Estrazione dei campi CDATA
                                                    if (valore.Contains("<![CDATA["))
                                                    {
                                                        valore = valore.Replace("<![CDATA[", "");
                                                        valore = valore.Replace("]]>", "");
                                                    }

                                                    oggettoCustom.VALORE_DATABASE = valore;

                                                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CampoDiTesto")
                                                    {
                                                        if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("MULTINODE"))
                                                        {
                                                            string separatore = oggettoCustom.OPZIONI_XML_ASSOC.Split('>')[1];

                                                            System.Xml.XmlNodeList multinode = xmlDoc.SelectNodes(mappingElemento);
                                                            if (multinode.Count > 1)
                                                            {
                                                                valore = "";
                                                                foreach (System.Xml.XmlNode nodoX in multinode)
                                                                {
                                                                    if (!valore.Contains(nodoX.InnerXml))
                                                                    {
                                                                        valore += nodoX.InnerXml + separatore;
                                                                    }
                                                                }
                                                            }
                                                            if (valore.Length > 180) valore = valore.Substring(0, 180);

                                                        }
                                                        #region Fornitore fattura elettronica
                                                        if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                                        {
                                                            if (string.IsNullOrEmpty(valore))
                                                            {
                                                                string mappingNome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Nome";
                                                                mappingXml = mappingNome.Split('>');
                                                                mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                                for (int i = 1; i < mappingXml.Length; i++)
                                                                {
                                                                    mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                                }
                                                                valore = "";
                                                                try
                                                                {
                                                                    System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                                    valore = node.InnerXml; // valore dell'xml estratto
                                                                }
                                                                catch (Exception nodo)
                                                                {
                                                                    notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                                    //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                    //{
                                                                    //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                                    //}
                                                                }

                                                                string mappingCognome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Cognome";
                                                                mappingXml = mappingCognome.Split('>');
                                                                mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                                for (int i = 1; i < mappingXml.Length; i++)
                                                                {
                                                                    mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                                }
                                                                try
                                                                {
                                                                    System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                                    valore += (" " + node.InnerXml); // valore dell'xml estratto
                                                                }
                                                                catch (Exception nodo)
                                                                {
                                                                    notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                                    //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                    //{
                                                                    //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                                    //}
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                        oggettoCustom.VALORE_DATABASE = valore;
                                                    }
                                                    if (!string.IsNullOrEmpty(valore))
                                                    {
                                                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggettoCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
                                                        {
                                                            string[] valoriAssociati1 = oggettoCustom.OPZIONI_XML_ASSOC.Split('>');
                                                            bool trovato = false;
                                                            for (int i = 0; i < valoriAssociati1.Length; i++)
                                                            {
                                                                string[] valoriAssociati2 = valoriAssociati1[i].Split('<');
                                                                if (valoriAssociati2[1] == valore)
                                                                {
                                                                    oggettoCustom.VALORE_DATABASE = valoriAssociati2[0];
                                                                    trovato = true;
                                                                }
                                                            }
                                                            if (!trovato)
                                                            {
                                                                notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                                //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                //{
                                                                //    throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non ha un campo associato configurato");
                                                                //}
                                                            }

                                                        }
                                                        else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Data")
                                                        {
                                                            string conversione = oggettoCustom.OPZIONI_XML_ASSOC;
                                                            try
                                                            {

                                                                //DateTime dtp = DateTime.ParseExact(valore, conversione, System.Globalization.CultureInfo.InvariantCulture);
                                                                DateTime dtp = DateTime.ParseExact(valore.Substring(0, conversione.Length), conversione, System.Globalization.CultureInfo.InvariantCulture);

                                                                oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy");
                                                                if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_ORA))
                                                                {
                                                                    oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy " + oggettoCustom.FORMATO_ORA);
                                                                }
                                                            }
                                                            catch (Exception exData)
                                                            {
                                                                oggettoCustom.VALORE_DATABASE = "";
                                                                notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                                //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                //{
                                                                //    throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non è compatibile con la stringa di conversione " + conversione);
                                                                //}
                                                            }

                                                        }
                                                        else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                                                        {
                                                            #region Da rifare con metodi da Frontend
                                                            DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                                                            //filtriRic.doUo = true;
                                                            //filtriRic.doRuoli = true;
                                                            filtriRic.doRubricaComune = true;
                                                            //filtriRic.doUtenti = true;
                                                            //filtriRic.doRF = true;
                                                            filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                                            filtriRic.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                                                            filtriRic.caller.IdRuolo = infoUtente.idGruppo;
                                                            filtriRic.caller.IdUtente = infoUtente.idPeople;
                                                            filtriRic.caller.filtroRegistroPerRicerca = string.Empty;
                                                            string tipoRicerca = oggettoCustom.OPZIONI_XML_ASSOC.Split('§')[0];
                                                            //if (tipoRicerca == "CODE")
                                                            //    filtriRic.codice = valore;
                                                            switch (tipoRicerca)
                                                            {
                                                                case "CODE":
                                                                    filtriRic.codice = valore;
                                                                    break;
                                                                case "DESCRIZIONE":
                                                                    filtriRic.descrizione = valore;
                                                                    break;
                                                                case "PIVA":
                                                                    filtriRic.partitaIva = valore;
                                                                    break;
                                                                case "CF":
                                                                    filtriRic.codiceFiscale = valore;
                                                                    break;
                                                                case "MAIL":
                                                                    filtriRic.email = valore;
                                                                    break;
                                                            }
                                                            BusinessLogic.Rubrica.DPA3_RubricaSearchAgent corrSearcher = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);

                                                            // DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
                                                            DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                                                            ArrayList objElementiRubrica = corrSearcher.Search(filtriRic, smistamentoRubrica);
                                                            if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                                                            {
                                                                string sysId = "";

                                                                if (!string.IsNullOrEmpty(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId))
                                                                {
                                                                    sysId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                                                                }
                                                                else
                                                                {
                                                                    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice);

                                                                    if ((corr == null) || (corr != null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica)))
                                                                    {
                                                                        bool rubricaComuneAbilitata = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
                                                                        if (rubricaComuneAbilitata)
                                                                        {
                                                                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice, infoUtente);
                                                                        }

                                                                        if (corr != null)
                                                                            sysId = corr.systemId;
                                                                    }
                                                                }
                                                                oggettoCustom.VALORE_DATABASE = sysId;
                                                            }
                                                            else
                                                            {
                                                                oggettoCustom.VALORE_DATABASE = string.Empty;
                                                                //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                //    throw new Exception("Elaborazione XML: Errore nell'associazione del campo " + oggettoCustom.DESCRIZIONE + ", Corrispondente non trovato con i dati presenti nello XML.");
                                                                //else
                                                                notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore è presente nel file XML ma non è stato possibile associarlo automaticamente ad un corrispondente.  L’eventuale integrazione avviene manualmente. ";
                                                            }
                                                            #endregion
                                                        }
                                                        else if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {
                                                        notePerElaborazioneXML += oggettoCustom.DESCRIZIONE + ". ";
                                                    }

                                                }
                                            }
                                            catch (Exception ex1end)
                                            {
                                                //if (e.Message.Equals("Corrispondente non trovato con i dati presenti nello XML."))
                                                //{
                                                //    throw new Exception("Corrispondente non trovato con i dati presenti nello XML.");
                                                //}
                                                //if (e.Message.Contains("Elaborazione XML:"))
                                                //{
                                                //    throw new Exception(e.Message);
                                                //}
                                                //else
                                                //{
                                                oggettoCustom.VALORE_DATABASE = string.Empty;
                                                oggettoCustom.VALORI_SELEZIONATI = null;
                                                //}
                                            }

                                            //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI" && (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) || ((oggettoCustom.VALORI_SELEZIONATI == null || oggettoCustom.VALORI_SELEZIONATI.Count < 1) && oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")))
                                            //{
                                            //    //throw new PisException("FIELD_REQUIRED");
                                            //    throw new Exception("Elaborazione XML: Valore assente per il campo obbligatorio " + oggettoCustom.DESCRIZIONE);
                                            //}
                                            // QUA DEVE FINIRE
                                        }


                                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom notePerXML in template.ELENCO_OGGETTI)
                                        {
                                            if (notePerXML.DESCRIZIONE.ToUpper() == "NOTE RELATIVE ALL'ELABORAZIONE XML")
                                            {
                                                if (!string.IsNullOrEmpty(notePerElaborazioneXML))
                                                {
                                                    notePerElaborazioneXML = "Mancanti: " + notePerElaborazioneXML;
                                                    if (notePerElaborazioneXML.Length > 220)
                                                    {
                                                        notePerElaborazioneXML = notePerElaborazioneXML.Substring(0, 220) + "...";
                                                    }
                                                    notePerXML.VALORE_DATABASE = notePerElaborazioneXML;
                                                }
                                                else
                                                    notePerXML.VALORE_DATABASE = "Elaborazione avvenuta con successo";
                                            }
                                        }

                                        doc.template = template;
                                        doc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                        doc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                                        doc.tipologiaAtto.descrizione = template.DESCRIZIONE;
                                        doc.daAggiornareTipoAtto = true;

                                        bool daaggiornTemp = false;
                                        doc = BusinessLogic.Documenti.DocSave.save(infoUtente, doc, false, out daaggiornTemp, null);

                                    }
                                    else
                                    {
                                        throw new Exception("Template Fattura non trovato");
                                    }
                                    #endregion

                                    #region Caricamento allegati fattura se presenti
                                    System.Xml.XmlNodeList listanodi = xmlDoc.DocumentElement.SelectNodes("//*[name()='Allegati']");
                                    System.Xml.XmlNode nodoNome = null;
                                    System.Xml.XmlNode nodoContent = null;
                                    DocsPaVO.documento.FileRequest allegato = null;
                                    DocsPaVO.documento.FileDocumento fileAllegato = null;
                                    string erroreMessage = "";
                                    bool caricaAllegato = true;
                                    if (listanodi != null && listanodi.Count > 0)
                                    {
                                        foreach (System.Xml.XmlNode nodo in listanodi)
                                        {
                                            nodoNome = null;
                                            nodoContent = null;
                                            caricaAllegato = true;

                                            foreach (System.Xml.XmlNode nodo1 in nodo.ChildNodes)
                                            {
                                                //Console.WriteLine(nodo1.Name);
                                                if (nodo1.Name.ToUpper() == "NOMEATTACHMENT") nodoNome = nodo1;
                                                if (nodo1.Name.ToUpper() == "ATTACHMENT") nodoContent = nodo1;
                                            }


                                            //byte[] contentNormal = Convert.FromBase64String(nodoContent.InnerXml);
                                            foreach (DocsPaVO.documento.Allegato alltempx1 in doc.allegati)
                                            {
                                                if (alltempx1.descrizione.ToUpper() == nodoNome.InnerXml.ToUpper()) caricaAllegato = false;
                                            }
                                            if (caricaAllegato)
                                            {
                                                allegato = new DocsPaVO.documento.Allegato
                                                {
                                                    docNumber = doc.systemId,
                                                    descrizione = nodoNome.InnerXml
                                                };
                                                allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));

                                                fileAllegato = new DocsPaVO.documento.FileDocumento
                                                {
                                                    name = nodoNome.InnerXml,
                                                    fullName = nodoNome.InnerXml,
                                                    content = Convert.FromBase64String(nodoContent.InnerXml),
                                                    length = Convert.FromBase64String(nodoContent.InnerXml).Length,
                                                    bypassFileContentValidation = true
                                                };


                                                if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileAllegato, infoUtente, out erroreMessage))
                                                {
                                                    throw new Exception("Errore nella creazione del file");
                                                }
                                                else
                                                {
                                                    retVal = true;
                                                }
                                            }
                                            else
                                            {
                                                logger.Debug("Allegato già presente");
                                            }

                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = false;
            }

            return retVal;
        }

        public static string FattElAttiveDaImport(string idDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string retVal = "";
            DocsPaVO.documento.FileDocumento fileAllFatt = null;
            string stringaXml = null;
            System.Xml.XmlDocument xmlDoc = null;
            bool tempBool = false;
            bool isLottoAttivo = false;
            DocsPaVO.ExternalServices.FornitoreFattAttiva fornitoreFA = null;
            string firmaCX = "";

            try
            {
                DocsPaVO.documento.SchedaDocumento doc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDoc);
                //foreach (DocsPaVO.documento.Allegato allFatt in doc.allegati)
                foreach (DocsPaVO.documento.Documento allFatt in doc.documenti)
                {
                    if (!string.IsNullOrEmpty(allFatt.fileName) && allFatt.fileName.ToUpper().Contains(".XML"))
                    {
                        fileAllFatt = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)allFatt, infoUtente);

                        if (fileAllFatt != null && !string.IsNullOrEmpty(fileAllFatt.fullName) && fileAllFatt.fullName.ToUpper().Contains("XML"))
                        {
                            if (allFatt.fileName.ToUpper().Contains(".P7M"))
                                firmaCX = "C";
                            stringaXml = "";
                            stringaXml = Encoding.UTF8.GetString(fileAllFatt.content);

                            stringaXml = stringaXml.Trim();
                            xmlDoc = new System.Xml.XmlDocument();
                            if (stringaXml.Contains("xml version=\"1.1\""))
                            {
                                logger.Debug("Versione XML 1.1. Provo conversione");
                                stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                            }
                            try
                            {
                                xmlDoc.LoadXml(stringaXml);
                            }
                            catch (Exception bomUTF8)
                            {
                                string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                                if (stringaXml.StartsWith(byteOrderMarkUtf8))
                                {
                                    stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                                }
                                xmlDoc.LoadXml(stringaXml);
                            }
                            if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                            {
                                // controllo se la fattura è di uno dei fornitori in DB
                                bool isFattLottoAttivo = false;

                                

                                string mappFornitore = string.Format("//*[name()='{0}']/*[name()='{1}']/*[name()='{2}']/*[name()='{3}']", "CedentePrestatore", "DatiAnagrafici", "IdFiscaleIVA", "IdCodice");
                                string codFornitore = (xmlDoc.DocumentElement.SelectSingleNode(mappFornitore)).InnerXml;

                                if (!string.IsNullOrEmpty(codFornitore))
                                {
                                    DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                                    ArrayList fornitori = dbAmm.FattElAttive_getCodFornitore(infoUtente.idAmministrazione);
                                    //if (fornitori.Contains(codFornitore))
                                    //    isFattLottoAttivo = true;                                       
                                    foreach (DocsPaVO.ExternalServices.FornitoreFattAttiva forn in fornitori)
                                    {
                                        if (!string.IsNullOrEmpty(forn.CodFornitore) && forn.CodFornitore.ToUpper() == codFornitore.ToUpper())
                                        {
                                            isFattLottoAttivo = true;
                                            fornitoreFA = forn;
                                        }
                                    }
                                }

                                if (!isFattLottoAttivo)
                                {
                                    logger.Error("Fornitore della fattura o lotto attivo non corrispondente: " + codFornitore);
                                    throw new Exception("Codice fornitore non corrispondente");
                                }
                                if (string.IsNullOrEmpty(firmaCX) && stringaXml.ToUpper().Contains("X509CERTIFICATE"))
                                {
                                    firmaCX = "X";
                                }

                                if (string.IsNullOrEmpty(firmaCX))
                                {
                                    string tempOutX5 = "";
                                    System.Xml.XmlElement root = xmlDoc.DocumentElement;
                                    string valore = root.Attributes["versione"].Value;
                                    if (valore.ToUpper() != "FPR" && valore.ToUpper() != "FPR12")
                                    {
                                        logger.Info("FATTURA NON PRIVATA - Errore controllo firma");
                                        BusinessLogic.Documenti.DocManager.CestinaDocumento(infoUtente, doc, null, "Fattura non firmata", out tempOutX5);
                                        throw new Exception("Fattura non firmata");
                                    }
                                    else
                                        logger.Info("FATTURA PRIVATA - Bypass controllo firma");
                                   
                                }

                                System.Xml.XmlNodeList fatture = xmlDoc.DocumentElement.SelectNodes("//*[name()='FatturaElettronicaBody']");
                                if (fatture.Count > 1)
                                {
                                    //throw new Exception("Lotto di fatture. Associazione a fattura elettronica interrotta.");
                                    isLottoAttivo = true;
                                }

                                #region Associazione dei campi
                                DocsPaVO.ProfilazioneDinamica.Templates template = null;
                                if (!isLottoAttivo)
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione("fattura elettronica attiva", infoUtente.idAmministrazione);
                                }
                                else { template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione("lotto di fatture attive", infoUtente.idAmministrazione); }

                                string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                                ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                                if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                                {
                                    throw new Exception("Ruolo non ha diritti su template");
                                }


                                if (template != null)
                                {
                                    string notePerElaborazioneXML = "";
                                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                    {
                                        try
                                        {

                                            if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC))
                                            {
                                                bool associaSecondo = false;
                                                string[] mappings = oggettoCustom.CAMPO_XML_ASSOC.Split('<');
                                                string[] mappingXml = mappings[0].Split('>');
                                                string mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                for (int i = 1; i < mappingXml.Length; i++)
                                                {
                                                    mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                }
                                                string valore = "";
                                                try
                                                {
                                                    System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                    valore = node.InnerXml; // valore dell'xml estratto
                                                }
                                                catch (Exception nodo)
                                                {
                                                    if (mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                                                        associaSecondo = true;
                                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                                    {
                                                        valore = "";
                                                    }
                                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                                    {
                                                        valore = "";
                                                    }
                                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_NOTE_VER_FIRMA_1"))
                                                    {
                                                        //valore = "Verifica firma digitale effettuata da SDI, secondo le Specifiche tecniche operative delle Regole tecniche di cui all’allegato B del D.M. n. 55 del 3 aprile 2013 e ss.mm.ii.";
                                                        valore = "";
                                                    }
                                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                                    {
                                                        valore = "999";
                                                    }
                                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_IDSDI"))
                                                    {
                                                        valore = "";
                                                    }
                                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_VERSIONE_1"))
                                                    {
                                                        System.Xml.XmlElement root = xmlDoc.DocumentElement;
                                                        valore = root.Attributes["versione"].Value;
                                                    }
                                                    else
                                                    {
                                                        notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                        //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                        //{
                                                        //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                        //}
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CODFISC_1"))
                                                {
                                                    if (!Char.IsLetter(valore[0]) || !Char.IsLetter(valore[1]) || !Char.IsLetter(valore[2]))
                                                        valore = "";
                                                }

                                                if (associaSecondo)
                                                {
                                                    int associaSecondoI = 1;
                                                    while (string.IsNullOrEmpty(valore) && mappings.Length > associaSecondoI && !string.IsNullOrEmpty(mappings[associaSecondoI]))
                                                    {
                                                        mappingXml = mappings[associaSecondoI].Split('>');
                                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                        for (int i = 1; i < mappingXml.Length; i++)
                                                        {
                                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                        }
                                                        valore = "";
                                                        try
                                                        {
                                                            System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                            valore = node.InnerXml; // valore dell'xml estratto
                                                        }
                                                        catch (Exception nodo)
                                                        {

                                                        }
                                                        associaSecondoI++;
                                                    }
                                                    if (string.IsNullOrEmpty(valore))
                                                    {
                                                        notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                        //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                        //{
                                                        //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                        //}

                                                    }
                                                }

                                                // Estrazione dei campi CDATA
                                                if (valore.Contains("<![CDATA["))
                                                {
                                                    valore = valore.Replace("<![CDATA[", "");
                                                    valore = valore.Replace("]]>", "");
                                                }

                                                oggettoCustom.VALORE_DATABASE = valore;

                                                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CampoDiTesto")
                                                {
                                                    if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("MULTINODE"))
                                                    {
                                                        string separatore = oggettoCustom.OPZIONI_XML_ASSOC.Split('>')[1];

                                                        System.Xml.XmlNodeList multinode = xmlDoc.SelectNodes(mappingElemento);
                                                        if (multinode.Count > 1)
                                                        {
                                                            valore = "";
                                                            foreach (System.Xml.XmlNode nodoX in multinode)
                                                            {
                                                                if (!valore.Contains(nodoX.InnerXml))
                                                                {
                                                                    valore += nodoX.InnerXml + separatore;
                                                                }
                                                            }
                                                        }
                                                        if (valore.Length > 180) valore = valore.Substring(0, 180);

                                                    }
                                                    #region Cliente fattura elettronica
                                                    if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                                    {
                                                        if (string.IsNullOrEmpty(valore))
                                                        {
                                                            string mappingNome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Nome";
                                                            mappingXml = mappingNome.Split('>');
                                                            mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                            for (int i = 1; i < mappingXml.Length; i++)
                                                            {
                                                                mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                            }
                                                            valore = "";
                                                            try
                                                            {
                                                                System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                                valore = node.InnerXml; // valore dell'xml estratto
                                                            }
                                                            catch (Exception nodo)
                                                            {
                                                                notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                                //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                //{
                                                                //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                                //}
                                                            }

                                                            string mappingCognome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Cognome";
                                                            mappingXml = mappingCognome.Split('>');
                                                            mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                            for (int i = 1; i < mappingXml.Length; i++)
                                                            {
                                                                mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                            }
                                                            try
                                                            {
                                                                System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                                valore += (" " + node.InnerXml); // valore dell'xml estratto
                                                            }
                                                            catch (Exception nodo)
                                                            {
                                                                notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                                //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                                //{
                                                                //    throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                                                //}
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    #region Partita IVA CessionarioCommittente
                                                    if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                                    {
                                                        // CessionarioCommittente>DatiAnagrafici>CodiceFiscale
                                                        if (string.IsNullOrEmpty(valore) || valore == "999")
                                                        {
                                                            string mappingNome = "CessionarioCommittente>DatiAnagrafici>CodiceFiscale";
                                                            mappingXml = mappingNome.Split('>');
                                                            mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                            for (int i = 1; i < mappingXml.Length; i++)
                                                            {
                                                                mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                            }
                                                            try
                                                            {
                                                                System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                                valore = node.InnerXml; // valore dell'xml estratto
                                                            }
                                                            catch (Exception nodo)
                                                            {
                                                                notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";

                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    oggettoCustom.VALORE_DATABASE = valore;
                                                }
                                                if (!string.IsNullOrEmpty(valore))
                                                {
                                                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggettoCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
                                                    {
                                                        string[] valoriAssociati1 = oggettoCustom.OPZIONI_XML_ASSOC.Split('>');
                                                        bool trovato = false;
                                                        for (int i = 0; i < valoriAssociati1.Length; i++)
                                                        {
                                                            string[] valoriAssociati2 = valoriAssociati1[i].Split('<');
                                                            if (valoriAssociati2[1] == valore)
                                                            {
                                                                oggettoCustom.VALORE_DATABASE = valoriAssociati2[0];
                                                                trovato = true;
                                                            }
                                                        }
                                                        if (!trovato)
                                                        {
                                                            notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                            //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                            //{
                                                            //    throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non ha un campo associato configurato");
                                                            //}
                                                        }

                                                    }
                                                    else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Data")
                                                    {
                                                        string conversione = oggettoCustom.OPZIONI_XML_ASSOC;
                                                        try
                                                        {

                                                            //DateTime dtp = DateTime.ParseExact(valore, conversione, System.Globalization.CultureInfo.InvariantCulture);
                                                            DateTime dtp = DateTime.ParseExact(valore.Substring(0, conversione.Length), conversione, System.Globalization.CultureInfo.InvariantCulture);

                                                            oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy");
                                                            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_ORA))
                                                            {
                                                                oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy " + oggettoCustom.FORMATO_ORA);
                                                            }
                                                        }
                                                        catch (Exception exData)
                                                        {
                                                            oggettoCustom.VALORE_DATABASE = "";
                                                            notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                            //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                            //{
                                                            //    throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non è compatibile con la stringa di conversione " + conversione);
                                                            //}
                                                        }

                                                    }
                                                    else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                                                    {
                                                        #region Da rifare con metodi da Frontend
                                                        DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                                                        //filtriRic.doUo = true;
                                                        //filtriRic.doRuoli = true;
                                                        filtriRic.doRubricaComune = true;
                                                        //filtriRic.doUtenti = true;
                                                        //filtriRic.doRF = true;
                                                        filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                                        filtriRic.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                                                        filtriRic.caller.IdRuolo = infoUtente.idGruppo;
                                                        filtriRic.caller.IdUtente = infoUtente.idPeople;
                                                        filtriRic.caller.filtroRegistroPerRicerca = string.Empty;
                                                        string tipoRicerca = oggettoCustom.OPZIONI_XML_ASSOC.Split('§')[0];
                                                        //if (tipoRicerca == "CODE")
                                                        //    filtriRic.codice = valore;
                                                        switch (tipoRicerca)
                                                        {
                                                            case "CODE":
                                                                filtriRic.codice = valore;
                                                                break;
                                                            case "DESCRIZIONE":
                                                                filtriRic.descrizione = valore;
                                                                break;
                                                            case "PIVA":
                                                                filtriRic.partitaIva = valore;
                                                                break;
                                                            case "CF":
                                                                filtriRic.codiceFiscale = valore;
                                                                break;
                                                            case "MAIL":
                                                                filtriRic.email = valore;
                                                                break;
                                                        }
                                                        BusinessLogic.Rubrica.DPA3_RubricaSearchAgent corrSearcher = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);

                                                        // DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
                                                        DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                                                        ArrayList objElementiRubrica = corrSearcher.Search(filtriRic, smistamentoRubrica);
                                                        if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                                                        {
                                                            string sysId = "";

                                                            if (!string.IsNullOrEmpty(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId))
                                                            {
                                                                sysId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                                                            }
                                                            else
                                                            {
                                                                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice);

                                                                if ((corr == null) || (corr != null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica)))
                                                                {
                                                                    bool rubricaComuneAbilitata = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
                                                                    if (rubricaComuneAbilitata)
                                                                    {
                                                                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice, infoUtente);
                                                                    }

                                                                    if (corr != null)
                                                                        sysId = corr.systemId;
                                                                }
                                                            }
                                                            oggettoCustom.VALORE_DATABASE = sysId;
                                                        }
                                                        else
                                                        {
                                                            oggettoCustom.VALORE_DATABASE = string.Empty;
                                                            //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                                            //    throw new Exception("Elaborazione XML: Errore nell'associazione del campo " + oggettoCustom.DESCRIZIONE + ", Corrispondente non trovato con i dati presenti nello XML.");
                                                            //else
                                                            notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore è presente nel file XML ma non è stato possibile associarlo automaticamente ad un corrispondente.  L’eventuale integrazione avviene manualmente. ";
                                                        }
                                                        #endregion
                                                    }
                                                    else if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                                    {

                                                    }
                                                }
                                                else
                                                {
                                                    notePerElaborazioneXML += oggettoCustom.DESCRIZIONE + ". ";
                                                }

                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC))
                                            {
                                                if (oggettoCustom.OPZIONI_XML_ASSOC.ToUpper() == "FATT_EL_ATT_AUTOREPERTORIAZIONE" && fornitoreFA != null && !string.IsNullOrEmpty(fornitoreFA.IdRegistro))
                                                {
                                                    if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")))
                                                    {
                                                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(fornitoreFA.IdRegistro);

                                                        if (reg != null)
                                                        {
                                                            oggettoCustom.ID_AOO_RF = reg.systemId;
                                                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex1end)
                                        {
                                            //if (e.Message.Equals("Corrispondente non trovato con i dati presenti nello XML."))
                                            //{
                                            //    throw new Exception("Corrispondente non trovato con i dati presenti nello XML.");
                                            //}
                                            //if (e.Message.Contains("Elaborazione XML:"))
                                            //{
                                            //    throw new Exception(e.Message);
                                            //}
                                            //else
                                            //{
                                            oggettoCustom.VALORE_DATABASE = string.Empty;
                                            oggettoCustom.VALORI_SELEZIONATI = null;
                                            //}
                                        }

                                        //if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI" && (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) || ((oggettoCustom.VALORI_SELEZIONATI == null || oggettoCustom.VALORI_SELEZIONATI.Count < 1) && oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")))
                                        //{
                                        //    //throw new PisException("FIELD_REQUIRED");
                                        //    throw new Exception("Elaborazione XML: Valore assente per il campo obbligatorio " + oggettoCustom.DESCRIZIONE);
                                        //}
                                        // QUA DEVE FINIRE
                                    }


                                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom notePerXML in template.ELENCO_OGGETTI)
                                    {
                                        if (notePerXML.DESCRIZIONE.ToUpper() == "NOTE RELATIVE ALL'ELABORAZIONE XML")
                                        {
                                            if (!string.IsNullOrEmpty(notePerElaborazioneXML))
                                            {
                                                notePerElaborazioneXML = "Mancanti: " + notePerElaborazioneXML;
                                                if (notePerElaborazioneXML.Length > 220)
                                                {
                                                    notePerElaborazioneXML = notePerElaborazioneXML.Substring(0, 220) + "...";
                                                }
                                                notePerXML.VALORE_DATABASE = notePerElaborazioneXML;
                                            }
                                            else
                                                notePerXML.VALORE_DATABASE = "Elaborazione avvenuta con successo";
                                        }
                                    }

                                    doc.template = template;
                                    doc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                    doc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                                    doc.tipologiaAtto.descrizione = template.DESCRIZIONE;
                                    doc.daAggiornareTipoAtto = true;

                                    bool daaggiornTemp = false;
                                    doc = BusinessLogic.Documenti.DocSave.save(infoUtente, doc, false, out daaggiornTemp, null);

                                }
                                else
                                {
                                    throw new Exception("Template Fattura non trovato");
                                }
                                #endregion

                                #region Caricamento allegati fattura se presenti
                                System.Xml.XmlNodeList listanodi = xmlDoc.DocumentElement.SelectNodes("//*[name()='Allegati']");
                                System.Xml.XmlNode nodoNome = null;
                                System.Xml.XmlNode nodoContent = null;
                                DocsPaVO.documento.FileRequest allegato = null;
                                DocsPaVO.documento.FileDocumento fileAllegato = null;
                                string erroreMessage = "";
                                bool caricaAllegato = true;
                                foreach (System.Xml.XmlNode nodo in listanodi)
                                {
                                    nodoNome = null;
                                    nodoContent = null;
                                    caricaAllegato = true;

                                    foreach (System.Xml.XmlNode nodo1 in nodo.ChildNodes)
                                    {
                                        //Console.WriteLine(nodo1.Name);
                                        if (nodo1.Name.ToUpper() == "NOMEATTACHMENT") nodoNome = nodo1;
                                        if (nodo1.Name.ToUpper() == "ATTACHMENT") nodoContent = nodo1;
                                    }


                                    //byte[] contentNormal = Convert.FromBase64String(nodoContent.InnerXml);
                                    foreach (DocsPaVO.documento.Allegato alltempx1 in doc.allegati)
                                    {
                                        if (alltempx1.descrizione.ToUpper() == nodoNome.InnerXml.ToUpper()) caricaAllegato = false;
                                    }
                                    if (caricaAllegato)
                                    {
                                        allegato = new DocsPaVO.documento.Allegato
                                        {
                                            docNumber = doc.systemId,
                                            descrizione = nodoNome.InnerXml
                                        };
                                        allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));

                                        fileAllegato = new DocsPaVO.documento.FileDocumento
                                        {
                                            name = nodoNome.InnerXml,
                                            fullName = nodoNome.InnerXml,
                                            content = Convert.FromBase64String(nodoContent.InnerXml),
                                            length = Convert.FromBase64String(nodoContent.InnerXml).Length,
                                            bypassFileContentValidation = true
                                        };


                                        if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileAllegato, infoUtente, out erroreMessage))
                                        {
                                            throw new Exception("Errore nella creazione del file");
                                        }
                                        else
                                        {
                                            retVal = "OK";
                                        }
                                    }
                                    else
                                    {
                                        logger.Debug("Allegato già presente");
                                    }

                                }
                                #endregion

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = ex.Message;
            }

            return retVal;
        }

        public static ArrayList FattElAttiveGetFornitori(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.FattElAttive_getCodFornitore(idAmm);
        }

        public static bool FattElAttive_InsertInLogPIS(string idProfile, string iduo, string coduo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.FattElAttive_InsertIntoLogPIS(idProfile, iduo, coduo);
        }

        public static bool FattElAttive_UpSecProprietario(string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.FattElAttive_UpSecProprietario(idProfile);
        }
        #endregion

        #region Integrazione CDS
        public static ArrayList CDS_getLogEvents(string lastLog, string idTipoCDS, string idOggAppliant, string idOggLocat)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList retval = dbAmm.CDS_getLogEvents(lastLog, idTipoCDS, idOggAppliant, idOggLocat);

            return retval;
        }

        public static string CDS_getLastLogID()
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            string retval = dbAmm.CDS_getLastLogId();
            return retval;
        }

        public static bool CDS_InsertEventInTable(DocsPaVO.ExternalServices.EventoCDS evento)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool retval = dbAmm.CDS_InsertEventInTable(evento);
            return retval;
        }

        #endregion

        #region Albo Telematico
        public static ArrayList Albo_getDocsDaNotificare(string idDocMinimo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList retval = dbAmm.Albo_getDocsDaNotificare(idDocMinimo);
            ArrayList retval2 = new ArrayList();
            bool errorepubb = false;
            bool cambiaStato = true;
            foreach (DocsPaVO.ExternalServices.PubblicazioneAlbo pubbAlbo in retval)
            {
                errorepubb = false;
                if (pubbAlbo.statoDoc.ToUpper() == "DA PUBBLICARE")
                {
                    int durata = 0;
                    if (!Int32.TryParse(pubbAlbo.durata, out durata))
                    {
                        pubbAlbo.errore = "Durata in formato non corretto";
                        errorepubb = true;
                        logger.Error("Durata in formato non corretto");
                    }
                    else if (!string.IsNullOrEmpty(pubbAlbo.extDocPrincipale) && !pubbAlbo.extDocPrincipale.ToUpper().Equals("PDF"))
                    {
                        pubbAlbo.errore = "File Principale in formato non corretto";
                        errorepubb = true;
                        logger.Error("File Principale in formato non corretto");
                    }
                    else if (Int32.Parse(pubbAlbo.numAllNonValidi) > 0)
                    {
                        pubbAlbo.errore = "Presente almeno un allegato di formato non valido.";
                        errorepubb = true;
                        logger.Error("Presente almeno un allegato di formato non valido.");
                    }
                    //else if (pubbAlbo.userRuoloCodice.Length > 15)
                    //{
                    //    // La coda di Albo Telematico da errore se il codice ruolo è maggiore di 15 caratteri
                    //    pubbAlbo.errore = "Codice del ruolo troppo lungo.";
                    //    errorepubb = true;
                    //    logger.Error("Codice del ruolo troppo lungo.");
                    //}
                    if (errorepubb && cambiaStato)
                    {
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(pubbAlbo.userId, BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(pubbAlbo.codiceAmm));
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(pubbAlbo.userRuoloCodice);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(pubbAlbo.idDiagramma);
                        if (diagramma != null)
                        {
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if (stato.DESCRIZIONE.ToUpper().Equals("DA PUBBLICARE ERRORE"))
                                    {
                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(pubbAlbo.idDocumento, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                    }
                                }
                            }
                        }
                    }

                }
                else if (pubbAlbo.statoDoc.ToUpper() == "DA ANNULLARE")
                {
                    int durata = 0;
                    string descStato = string.Empty;



                    if (!Int32.TryParse(pubbAlbo.durata, out durata))
                    {
                        pubbAlbo.errore = "Durata in formato non corretto";
                        errorepubb = true;
                        descStato = "DA ANNULLARE ERRORE";
                        logger.Error("Durata in formato non corretto");
                    }
                    else if (durata > 20)
                    {
                        // CONDIZIONE DA RIVEDERE. QUANTI GIORNI DOPO LA SCADENZA
                    }
                    else if (string.IsNullOrEmpty(pubbAlbo.dataPubb))
                    {
                        pubbAlbo.errore = "Data pubblicazione assente";
                        errorepubb = true;
                        descStato = "DA ANNULLARE ERRORE";
                        logger.Error("Data pubblicazione assente");
                    }
                    else
                    {
                        DateTime now = Convert.ToDateTime(DateTime.Now.ToString("d", new System.Globalization.CultureInfo("it-IT")),
                        new System.Globalization.CultureInfo("it-IT"));
                        DateTime dataPubb = new DateTime();
                        dataPubb = Convert.ToDateTime(pubbAlbo.dataPubb, new System.Globalization.CultureInfo("it-IT"));
                        dataPubb = dataPubb.Add(TimeSpan.FromDays(1));
                        TimeSpan ts = now - dataPubb;
                        int differenceInDays = ts.Days;
                        if (differenceInDays > 5) // non posso annullare il documento
                        {
                            pubbAlbo.errore = "Passati più di 5 giorni dalla pubblicazione";
                            errorepubb = true;
                            descStato = "PUBBLICATO - IMPOSSIBILE ANNULLARE - SUPERATA LA DATA CONSENTITA";
                            logger.Error("Passati più di 5 giorni dalla pubblicazione");
                        }
                    }

                    if (errorepubb && cambiaStato)
                    {
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(pubbAlbo.userId, BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(pubbAlbo.codiceAmm));
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(pubbAlbo.userRuoloCodice);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(pubbAlbo.idDiagramma);
                        if (diagramma != null)
                        {
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if (stato.DESCRIZIONE.ToUpper().Equals(descStato))
                                    {
                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(pubbAlbo.idDocumento, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                    }
                                }
                            }
                        }
                    }

                }
                else if (pubbAlbo.statoDoc.ToUpper() == "DA REVOCARE")
                {
                    int durata = 0;
                    string descStato = string.Empty;



                    if (!Int32.TryParse(pubbAlbo.durata, out durata))
                    {
                        pubbAlbo.errore = "Durata in formato non corretto";
                        errorepubb = true;
                        descStato = "DA REVOCARE ERRORE";
                        logger.Error("Durata in formato non corretto");
                    }
                    else if (durata > 0)
                    {
                        pubbAlbo.errore = "PUBBLICATO - IMPOSSIBILE REVOCARE DOC SENZA REVOCA";
                        errorepubb = true;
                        descStato = "PUBBLICATO - IMPOSSIBILE REVOCARE DOC SENZA REVOCA";
                        logger.Error("PUBBLICATO - IMPOSSIBILE REVOCARE DOC SENZA REVOCA");
                    }
                    if (errorepubb && cambiaStato)
                    {
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(pubbAlbo.userId, BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(pubbAlbo.codiceAmm));
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(pubbAlbo.userRuoloCodice);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(pubbAlbo.idDiagramma);
                        if (diagramma != null)
                        {
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if (stato.DESCRIZIONE.ToUpper().Equals(descStato))
                                    {
                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(pubbAlbo.idDocumento, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                    }
                                }
                            }
                        }
                    }
                }

                retval2.Add(pubbAlbo);
            }
            return retval2;
        }

        public static ArrayList Albo_UNITN_getDocsDaNotificare(string idDocMinimo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList retval = dbAmm.Albo_UTN_getDocsDaNotificare(idDocMinimo);
            ArrayList retval2 = new ArrayList();
            bool errorepubb = false;
            bool cambiaStato = true;
            foreach (DocsPaVO.ExternalServices.PubblicazioneAlbo pubbAlbo in retval)
            {
                errorepubb = false;
                if (pubbAlbo.statoDoc.ToUpper() == "DA PUBBLICARE")
                {
                    //int durata = 0;
                    //if (!Int32.TryParse(pubbAlbo.durata, out durata))
                    //{
                    //    pubbAlbo.errore = "Durata in formato non corretto";
                    //    errorepubb = true;
                    //    logger.Error("Durata in formato non corretto");
                    //}
                    //else 
                    if (!string.IsNullOrEmpty(pubbAlbo.extDocPrincipale) && !pubbAlbo.extDocPrincipale.ToUpper().Equals("PDF"))
                    {
                        pubbAlbo.errore = "File Principale in formato non corretto";
                        errorepubb = true;
                        logger.Error("File Principale in formato non corretto");
                    }
                    else if (Int32.Parse(pubbAlbo.numAllNonValidi) > 0)
                    {
                        pubbAlbo.errore = "Presente almeno un allegato di formato non valido.";
                        errorepubb = true;
                        logger.Error("Presente almeno un allegato di formato non valido.");
                    }
                    //else if (pubbAlbo.userRuoloCodice.Length > 15)
                    //{
                    //    // La coda di Albo Telematico da errore se il codice ruolo è maggiore di 15 caratteri
                    //    pubbAlbo.errore = "Codice del ruolo troppo lungo.";
                    //    errorepubb = true;
                    //    logger.Error("Codice del ruolo troppo lungo.");
                    //}
                    if (errorepubb && cambiaStato)
                    {
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(pubbAlbo.userId, BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(pubbAlbo.codiceAmm));
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(pubbAlbo.userRuoloCodice);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(pubbAlbo.idDiagramma);
                        if (diagramma != null)
                        {
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if (stato.DESCRIZIONE.ToUpper().Equals("DA PUBBLICARE ERRORE"))
                                    {
                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(pubbAlbo.idDocumento, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                    }
                                }
                            }
                        }
                    }

                }
                else if (pubbAlbo.statoDoc.ToUpper() == "DA ANNULLARE")
                {
                    int durata = 0;
                    string descStato = string.Empty;



                    //if (!Int32.TryParse(pubbAlbo.durata, out durata))
                    //{
                    //    pubbAlbo.errore = "Durata in formato non corretto";
                    //    errorepubb = true;
                    //    descStato = "DA ANNULLARE ERRORE";
                    //    logger.Error("Durata in formato non corretto");
                    //}
                    //else if (durata > 20)
                    //{
                    //    // CONDIZIONE DA RIVEDERE. QUANTI GIORNI DOPO LA SCADENZA
                    //}
                    //else if (string.IsNullOrEmpty(pubbAlbo.dataPubb))
                    //{
                    //    pubbAlbo.errore = "Data pubblicazione assente";
                    //    errorepubb = true;
                    //    descStato = "DA ANNULLARE ERRORE";
                    //    logger.Error("Data pubblicazione assente");
                    //}
                    //else
                    //{
                    //    DateTime now = Convert.ToDateTime(DateTime.Now.ToString("d", new System.Globalization.CultureInfo("it-IT")),
                    //    new System.Globalization.CultureInfo("it-IT"));
                    //    DateTime dataPubb = new DateTime();
                    //    dataPubb = Convert.ToDateTime(pubbAlbo.dataPubb, new System.Globalization.CultureInfo("it-IT"));
                    //    dataPubb = dataPubb.Add(TimeSpan.FromDays(1));
                    //    TimeSpan ts = now - dataPubb;
                    //    int differenceInDays = ts.Days;
                    //    if (differenceInDays > 5) // non posso annullare il documento
                    //    {
                    //        pubbAlbo.errore = "Passati più di 5 giorni dalla pubblicazione";
                    //        errorepubb = true;
                    //        descStato = "PUBBLICATO - IMPOSSIBILE ANNULLARE - SUPERATA LA DATA CONSENTITA";
                    //        logger.Error("Passati più di 5 giorni dalla pubblicazione");
                    //    }
                    //}

                    if (errorepubb && cambiaStato)
                    {
                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(pubbAlbo.userId, BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(pubbAlbo.codiceAmm));
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(pubbAlbo.userRuoloCodice);
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(pubbAlbo.idDiagramma);
                        if (diagramma != null)
                        {
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if (stato.DESCRIZIONE.ToUpper().Equals(descStato))
                                    {
                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(pubbAlbo.idDocumento, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                    }
                                }
                            }
                        }
                    }

                }

                retval2.Add(pubbAlbo);
            }
            return retval2;
        }

        #endregion

        #region Controllo stampe repertorio

        public static ArrayList Ctrl_stmp_rep_errori()
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbamm.Ctrl_stmp_rep_errori();
        }

        public static ArrayList Ctrl_stmp_rep_file()
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbamm.Ctrl_stampa_rep_file();
        }

        #endregion

        #region Migrazione File System

        public static ArrayList MIGR_FS_GetListMIGRFileInfo(string minVersionId, string maxVersionId)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbamm.MIGR_FS_GetListMIGRFileInfo(minVersionId, maxVersionId);
        }

        public static bool MIGR_FS_insertDataInLogTable(DocsPaVO.ExternalServices.MIGR_File_Info migr_file_info)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbamm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaDB.Query_DocsPAWS.Documenti dbDocs = new DocsPaDB.Query_DocsPAWS.Documenti();

            if (string.IsNullOrEmpty(migr_file_info.ChaErrore))
            {
                if (!dbDocs.UpdateComponentsPath(migr_file_info.PathNew, migr_file_info.VersionId, migr_file_info.Docnumber))
                {
                    migr_file_info.ChaErrore = "E";
                    migr_file_info.MessaggioLog = "Errore in update della components";
                }
            }

            return dbamm.MIGR_FS_InsertInLog(migr_file_info);
        }

        public static byte[] MIGR_FS_GetFile(string docnumber, string versionId, string version, string versionLabel, string password, string idUtente, string idRuolo)
        {
            if (password == "MIGR_FS_asmxAffollato_2016")
            {
                DocsPaVO.utente.InfoUtente infoUt = new DocsPaVO.utente.InfoUtente();
                infoUt = BusinessLogic.Utenti.UserManager.GetInfoUtente(BusinessLogic.Utenti.UserManager.getUtenteById(idUtente), BusinessLogic.Utenti.UserManager.getRuoloById(idRuolo));
                infoUt.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUt);
                return docManager.GetFile(docnumber, version, versionId, versionLabel);
            }
            else return null;
        }


        #endregion

        #region CapServices

        public static bool CAPRefactorTrasmissioni(string idOpportunity, string notaDaIns)
        {
            bool retVal = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retVal = dbAmm.CAPRefactorTrasmissioni(idOpportunity, notaDaIns);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = false;
            }

            return retVal;
        }

        public static DocsPaVO.utente.Utente CAPGetUserByEmail(string email)
        {
            DocsPaVO.utente.Utente retval = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utAmm = new DocsPaDB.Query_DocsPAWS.Utenti();
                retval = utAmm.GetUtenteByEmailNoIdAmm(email.ToUpper());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retval = null;
            }
            return retval;
        }

        public static ArrayList CAPGetDocInFolder(string idFolder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli dbFa = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return dbFa.getIdDocSottoFasc(idFolder);
        }

        public static ArrayList CAPGetOpportunitiesPending(string typePrefix, string idPeople, string pendAppr, string idOpp)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.CAPgetOpportunitiesPending(typePrefix, idPeople, pendAppr, idOpp);
        }

        public static ArrayList CAPGetOppApprovals(string idOpp)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.CAPGetOppApprovals(idOpp);
        }

        public static bool CAPCtrlContribution(string idOpp, string splitChar)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.CAPCtrlContribution(idOpp, splitChar);
        }

        #endregion

        #region CERCA.TRE
        public static System.Data.DataTable C3GetDocs(string fromTime, string toTime, string optionTime)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.C3GetDocs(fromTime, toTime, optionTime);
        }

        public static System.Data.DataTable C3GetAllByIdDoc(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.C3GetAllegatiByDocID(idDoc);
        }

        public static System.Data.DataTable C3GetDocsMod(string fromTime, string toTime, string optionTime)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            System.Data.DataTable tabMod1 = dbAmm.C3GetDocsMod(fromTime, toTime, optionTime);
            ArrayList modificati = new ArrayList();
            System.Data.DataTable retval = null;
            //System.Data.DataRow rTemp;
            if (tabMod1 != null && tabMod1.Rows != null && tabMod1.Rows.Count > 0)
            {
                retval = tabMod1.Clone();
                retval.Rows.Clear();

                foreach (System.Data.DataRow r1 in tabMod1.Rows)
                {
                    if (!modificati.Contains(r1["SYSTEM_ID"].ToString()))
                    {
                        modificati.Add(r1["SYSTEM_ID"].ToString());
                        //rTemp.ItemArray = r1.ItemArray;
                        //retval.Rows.Add(rTemp);
                        retval.ImportRow(r1);
                    }
                }
            }

            System.Data.DataTable tabMod2 = dbAmm.C3GetDocsModAll(fromTime, toTime, optionTime);
            if (tabMod2 != null && tabMod2.Rows != null && tabMod2.Rows.Count > 0)
            {
                if (retval == null)
                {
                    retval = tabMod2.Clone();
                    retval.Rows.Clear();
                }
                foreach (System.Data.DataRow r2 in tabMod2.Rows)
                {
                    if (!modificati.Contains(r2["SYSTEM_ID"].ToString()))
                    {
                        modificati.Add(r2["SYSTEM_ID"].ToString());
                        //rTemp.ItemArray = r2.ItemArray;
                        //retval.Rows.Add(rTemp);
                        retval.ImportRow(r2);
                    }
                }
            }
            System.Data.DataTable tabMod3 = dbAmm.C3GetDocsModLastEditDate(fromTime, toTime, optionTime);
            if (tabMod3 != null && tabMod3.Rows != null && tabMod3.Rows.Count > 0)
            {
                if (retval == null)
                {
                    retval = tabMod3.Clone();
                    retval.Rows.Clear();
                }
                foreach (System.Data.DataRow r3 in tabMod3.Rows)
                {
                    if (!modificati.Contains(r3["SYSTEM_ID"].ToString()))
                    {
                        modificati.Add(r3["SYSTEM_ID"].ToString());
                        //rTemp.ItemArray = r2.ItemArray;
                        //retval.Rows.Add(rTemp);
                        retval.ImportRow(r3);
                    }
                }
            }

            //return dbAmm.C3GetDocsMod(fromTime, toTime, optionTime);
            return retval;
        }
        public static System.Data.DataTable C3GetDocsModAll(string fromTime, string toTime, string optionTime)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.C3GetDocsModAll(fromTime, toTime, optionTime);
        }
        #endregion

        #region MIBACT Bacheca
        public static ArrayList MIBACT_BACHECA_getDocsDaNotificare(string statoInvia, string statoAggiorna, string campoNCirc)
        {
            DocsPaDB.Query_DocsPAWS.Integrazioni intDB = new DocsPaDB.Query_DocsPAWS.Integrazioni();
            return intDB.MIBACT_BACHECA_getDocsDaNotificare(statoInvia, statoAggiorna, campoNCirc);
        }

        public static ArrayList MIBACT_BACHECA_GetFileInfoDoc(string idDoc)
        {
            DocsPaDB.Query_DocsPAWS.Integrazioni intDB = new DocsPaDB.Query_DocsPAWS.Integrazioni();
            return intDB.MIBACT_BACHECA_GetFileInfoDoc(idDoc);
        }
        #endregion

        #region Big File FTP
        public static bool BigFilesFTP_InsertIntoTable(DocsPaVO.ExternalServices.FileFtpUpInfo infoFile)
        {
            DocsPaDB.Query_DocsPAWS.Integrazioni intDb = new DocsPaDB.Query_DocsPAWS.Integrazioni();
            return intDb.BigFilesFTP_InsertIntoTable(infoFile);
        }

        public static ArrayList BigFilesFTP_GetFilesToTransfer()
        {
            DocsPaDB.Query_DocsPAWS.Integrazioni intDb = new DocsPaDB.Query_DocsPAWS.Integrazioni();
            return intDb.BigFilesFTP_GetFilesToTransfer();
        }

        public static bool BigFilesFTP_updateTable(DocsPaVO.ExternalServices.FileFtpUpInfo infoFile)
        {
            DocsPaDB.Query_DocsPAWS.Integrazioni intDb = new DocsPaDB.Query_DocsPAWS.Integrazioni();
            return intDb.BigFilesFTP_updateTable(infoFile);
        }

        public static DocsPaVO.ExternalServices.FileFtpUpInfo BigFileFTP_GetInfoFileFTP(string idQueue, string idDocument)
        {
            DocsPaDB.Query_DocsPAWS.Integrazioni intDb = new DocsPaDB.Query_DocsPAWS.Integrazioni();
            return intDb.BigFilesFTP_GetInfoFile(idQueue, idDocument);

        }



        #endregion

    }
}
