using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using log4net;

namespace LiveCycle
{
    class DocumentiManager
    {
        //PUBLIC
        private static ILog logger = LogManager.GetLogger(typeof(DocumentiManager));
        public static void compilaCampiDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LiveCycle.ProcessFormInput processFormInput, XmlDocument xmlDoc, ref DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput)
        {
            try
            {
                //Imposto l'oggetto del documento
                inserisciOggetto(processFormInput.schedaDocumentoInput, xmlDoc);

                //Imposto il mittente del documento
                inserisciMittente(processFormInput.schedaDocumentoInput, xmlDoc);
                
                processFormOutput.schedaDocumentoOutput = processFormInput.schedaDocumentoInput;                
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: compilaCampiDocumento()", e);                
            }
        }

        public static void compilaTipologiaDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LiveCycle.ProcessFormInput processFormInput, XmlDocument xmlDoc, ref DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput)
        {
            try
            {
                //Verifico che l'xml contiene un campo per la tipologia di documento
                XmlNode node = xmlDoc.SelectSingleNode("//TipologiaDocumento");
                string tipologiaDocumento = string.Empty;
                DocsPaVO.ProfilazioneDinamica.Templates templateSelezionato = null;
                if (node != null)
                {
                    tipologiaDocumento = node.InnerText;
                    if (!string.IsNullOrEmpty(tipologiaDocumento))
                    {
                        //Recupero il template del documento
                        templateSelezionato = getTemplateDocumento(infoUtente, tipologiaDocumento);
                
                        //In funzione del tipo di oggetto eseguo una compilazione specifica
                        if (templateSelezionato != null && !string.IsNullOrEmpty(templateSelezionato.DESCRIZIONE))
                        {
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in templateSelezionato.ELENCO_OGGETTI)
                            {
                                switch(oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                {
                                    case "CampoDiTesto":
                                        inserisciCampoDiTesto(oggettoCustom,xmlDoc);
                                        break;
                                    case "CasellaDiSelezione":
                                        inserisciCasellaDiSelezione(oggettoCustom, xmlDoc);
                                        break;
                                    case "MenuATendina":
                                        //inserisciMenuATendina();
                                        break;
                                    case "SelezioneEsclusiva":
                                        inserisciSelezioneEsclusiva(oggettoCustom, xmlDoc);
                                        break;
                                    case "Contatore":
                                        //inserisciContatore();
                                        break;
                                    case "Data":
                                        inserisciData(oggettoCustom,xmlDoc);
                                        break;
                                    case "Corrispondente":
                                        //inserisciCorrispondente();
                                        break;
                                }
                            }
                            
                            //Imposto la tipologia di documento
                            DocsPaVO.documento.TipologiaAtto tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                            tipologiaAtto.systemId = templateSelezionato.SYSTEM_ID.ToString();
                            tipologiaAtto.descrizione = templateSelezionato.DESCRIZIONE;
                            processFormInput.schedaDocumentoInput.tipologiaAtto = tipologiaAtto;
                            processFormInput.schedaDocumentoInput.daAggiornareTipoAtto = true;
                            processFormInput.schedaDocumentoInput.template = templateSelezionato;
                        }

                    processFormOutput.schedaDocumentoOutput = processFormInput.schedaDocumentoInput;                    
                    }                    
                }                
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: compilaTipologiaDocumento()", e);
            }
        }

        /*
        public static void compilaCampiNascosti(DocsPaVO.utente.InfoUtente infoUtente, XmlDocument xmlDoc, ref DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput)
        {
            try
            {
                //Codice Classificazione Rapida
                if(xmlDoc.SelectSingleNode("//_codiceClassifica_") != null)
                {
                    processFormOutput.codiceClassificaRapida = xmlDoc.SelectSingleNode("//_codiceClassifica_").InnerText;
                }

                //Codice Modello di Trasmissione
                if (xmlDoc.SelectSingleNode("//_codiceModelloTrasm_") != null)
                {
                    processFormOutput.codiceModelloTramissione = xmlDoc.SelectSingleNode("//_codiceModelloTrasm_").InnerText;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: compilaCampiNascosti()", e);
            }
        }
        */

        /*
        public static void setFirmaDocumento(DocsPaVO.utente.InfoUtente infoUtente, ProcessFormService.PDFSignatureVerificationResult resultSignature, ref DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput)
        {
            try
            {
                if (processFormOutput.schedaDocumentoOutput.documenti != null && processFormOutput.schedaDocumentoOutput.documenti.Count > 0)
                {
                    ((DocsPaVO.documento.Documento)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato = "1";
                    ((DocsPaVO.documento.FileRequest)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato = "1";
                }                
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: setFirmaDocumento()", e);
            }
        }
        */

        public static void setFirmaDocumento(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.LiveCycle.ProcessFormOutput processFormOutput)
        {
            try
            {
                if (processFormOutput.schedaDocumentoOutput.documenti != null && processFormOutput.schedaDocumentoOutput.documenti.Count > 0)
                {
                    ((DocsPaVO.documento.Documento)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato = "1";
                    ((DocsPaVO.documento.FileRequest)processFormOutput.schedaDocumentoOutput.documenti[0]).firmato = "1";
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: setFirmaDocumento()", e);
            }
        }
        
        //PRIVATE
        private static DocsPaVO.ProfilazioneDinamica.Templates getTemplateDocumento(DocsPaVO.utente.InfoUtente infoUtente, string tipologiaDocumento)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList tipologieDocumento = model.getTemplates(infoUtente.idAmministrazione);
                foreach (DocsPaVO.ProfilazioneDinamica.Templates template in tipologieDocumento)
                {
                    if (template.DESCRIZIONE.ToUpper() == tipologiaDocumento.ToUpper())
                        return model.getTemplateById(template.SYSTEM_ID.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: getTemplateDocumento()", e);
                return null;
            }
        }

        private static void inserisciOggetto(DocsPaVO.documento.SchedaDocumento schedaDocumento, XmlDocument xmlDoc)
        {
            try
            {
                if (xmlDoc.SelectSingleNode("//_oggetto_") != null /*&& string.IsNullOrEmpty(schedaDocumento.oggetto.descrizione)*/)
                    schedaDocumento.oggetto.descrizione = xmlDoc.SelectSingleNode("//_oggetto_").InnerText;                
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: inserisciOggetto()", e);
                throw new Exception(e.Message);
            }
        }

        private static void inserisciMittente(DocsPaVO.documento.SchedaDocumento schedaDocumento, XmlDocument xmlDoc)
        {
            try
            {
                if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)) && xmlDoc.SelectSingleNode("//_mittente_") != null /*&& ((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).mittente == null*/)
                {
                    if(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).mittente == null)
                        ((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).mittente = new DocsPaVO.utente.Corrispondente();
                    
                    ((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).mittente.descrizione = xmlDoc.SelectSingleNode("//_mittente_").InnerText;

                    if(string.IsNullOrEmpty(schedaDocumento.interop))
                        ((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).mittente.tipoCorrispondente = "O";
                    
                }
                if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)) && xmlDoc.SelectSingleNode("//_mittente_") != null /*&& ((DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo).mittente == null*/)
                {
                    if (((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).mittente == null)
                        ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).mittente = new DocsPaVO.utente.Corrispondente();
                    
                    ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).mittente.descrizione = xmlDoc.SelectSingleNode("//_mittente_").InnerText;

                    if (string.IsNullOrEmpty(schedaDocumento.interop))
                        ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).mittente.tipoCorrispondente = "O";
                 
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: inserisciMittente()", e);
                throw new Exception(e.Message);
            }
        }

        private static void inserisciCampoDiTesto(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, XmlDocument xmlDoc)
        {
            try
            {
                if (xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE) != null)
                    oggettoCustom.VALORE_DATABASE = xmlDoc.SelectSingleNode("//"+oggettoCustom.DESCRIZIONE).InnerText;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: inserisciCampoDiTesto()", e);
                throw new Exception(e.Message);
            }
        }

        private static void inserisciData(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, XmlDocument xmlDoc)
        {
            try
            {
                if(xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE) != null)
                    oggettoCustom.VALORE_DATABASE = xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE).InnerText;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: inserisciData()", e);
                throw new Exception(e.Message);
            }
        }

        private static void inserisciCasellaDiSelezione(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, XmlDocument xmlDoc)
        {
            try
            {
                if (xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE) != null && !string.IsNullOrEmpty(xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE).InnerText))
                {
                    int valoreSelezionato = Convert.ToInt32(xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE).InnerText);
                    if (valoreSelezionato <= oggettoCustom.ELENCO_VALORI.Count)
                    {
                        oggettoCustom.VALORE_DATABASE = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[valoreSelezionato - 1]).VALORE;
                        oggettoCustom.VALORI_SELEZIONATI[valoreSelezionato - 1] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[valoreSelezionato - 1]).VALORE;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: inserisciCasellaDiSelezione()", e);
                throw new Exception(e.Message);
            }
        }

        private static void inserisciSelezioneEsclusiva(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, XmlDocument xmlDoc)
        {
            try
            {
                if (xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE) != null && !string.IsNullOrEmpty(xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE).InnerText))
                {
                    int valoreSelezionato = Convert.ToInt32(xmlDoc.SelectSingleNode("//" + oggettoCustom.DESCRIZIONE).InnerText);
                    if (valoreSelezionato <= oggettoCustom.ELENCO_VALORI.Count)
                    {
                        oggettoCustom.VALORE_DATABASE = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[valoreSelezionato - 1]).VALORE;
                        oggettoCustom.VALORI_SELEZIONATI[valoreSelezionato - 1] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[valoreSelezionato - 1]).VALORE;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle  - DocumentiManager - metodo: inserisciSelezioneEsclusiva()", e);
                throw new Exception(e.Message);
            }
        }
        
       
    }
}
