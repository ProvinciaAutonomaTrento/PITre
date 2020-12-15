using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.ProfilazioneDinamica;
using System.Collections;
using BusinessLogic.ProfilazioneDinamica;
using DocsPaVO.PrjDocImport;
using System.IO;
using BusinessLogic.Import.Template.Builders;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using DocsPaVO.amministrazione;
using BusinessLogic.Amministrazione;
using BusinessLogic.Utenti;
using log4net;

namespace BusinessLogic.Import.Template
{
    public class TemplateManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(TemplateManager));
        private InfoUtente _infoUtente;
        private Ruolo _role;
        private bool _isEnabledSmistamento;
        private Templates _template;
        private TemplateType _type;
        private delegate void PlaceholderHandler(SchedaDocumento sd,TemplateBuilder builder,DocumentRowData rowData,string label, string[] values);


        public TemplateManager(InfoUtente infoUtente,Ruolo role,bool isEnabledSmistamento,string templateName,TemplateType type)
        {
                logger.Debug("TemplateManager begin");
                if (string.IsNullOrEmpty(templateName)) throw new Exception("Non è stato specificato nessun template");
                List<Templates> value = new List<Templates>();
                ArrayList tempList = ProfilazioneDocumenti.getTemplates(infoUtente.idAmministrazione);
                foreach (object obj in tempList)
                {
                    string descrizione = ((Templates)obj).DESCRIZIONE;
                    if (templateName.ToUpper().Equals(descrizione.ToUpper()))
                    {
                        _template = ProfilazioneDocumenti.getTemplateById("" + ((Templates)obj).SYSTEM_ID);
                    }
                }
                if (_template == null)
                {
                    logger.Debug("Nessun template associato al nome " + templateName);
                    throw new Exception("Nessun template associato al nome " + templateName);
                }
                _type = type;
                _infoUtente = infoUtente;
                _role = role;
                _isEnabledSmistamento = isEnabledSmistamento;
        }

        public byte[] BuildDocumentFromTemplate(DocumentRowData rowData,SchedaDocumento schedaDocumento)
        {
            logger.Debug("BuildDocumentFromTemplate begin");
                    string path = _template.PATH_MODELLO_STAMPA_UNIONE;
                    if(string.IsNullOrEmpty(path)) path=_template.PATH_MODELLO_1;
                    if (string.IsNullOrEmpty(path))
                    {
                        logger.Debug("Nessun modello associato al documento");
                        throw new Exception("Nessun modello associato al documento");
                    }
                    try
                    {
                        byte[] input = GetFileContent(path);
                        TemplateBuilder tempBuilder = TemplateBuilder.Instances[_type];
                        HandleCampiComuni(_infoUtente,tempBuilder,schedaDocumento);
                        foreach (ProfilationFieldInformation temp in rowData.DocumentProfilationData)
                        {
                            GetPlaceHolderHandler(_template, temp.Label).Invoke(schedaDocumento,tempBuilder,rowData, temp.Label, temp.Values);
                        }
                        return tempBuilder.ReplacePlaceholders(input);
                    }
                    catch (Exception e)
                    {
                        logger.Debug("Errore durante la creazione del documento: "+e);
                        throw new Exception("Impossibile creare il file rtf associato");
                    }
        }

        private byte[] GetFileContent(string filePath)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(filePath).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        private PlaceholderHandler GetPlaceHolderHandler(Templates temp,string oggettoCustomName){
            OggettoCustom oggCustom = temp.getOggettoCustom(oggettoCustomName);
            if (oggCustom == null) return HandleTextPlaceholder;
            if ("Corrispondente".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO)) return HandleCorrispondentePlaceholder;
            if ("Contatore".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO)) return HandleContatorePlaceholder;
            if ("Link".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO))
            {
                if ("INTERNO".Equals(oggCustom.TIPO_LINK))
                {
                    if ("DOCUMENTO".Equals(oggCustom.TIPO_OBJ_LINK))
                    {
                        return HandleInternalLinkDocPlaceholder;
                    }
                    else
                    {
                        return HandleInternalLinkFascPlaceholder;
                    }
                }
                else
                {
                    return HandleExternalLinkPlaceholder;
                }
            }
            if (oggCustom != null && "OggettoEsterno".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO))
            {
                return HandleOggettoEsterno;
            }
            return HandleTextPlaceholder;
        }

        private void HandleContatorePlaceholder(SchedaDocumento sd, TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
             OggettoCustom oggCust = sd.template.getOggettoCustom(label);
             string value="";
             if (!string.IsNullOrEmpty(oggCust.FORMATO_CONTATORE))
             {
                 value = oggCust.FORMATO_CONTATORE;
                 value = value.Replace("ANNO", oggCust.ANNO);
                 value = value.Replace("CONTATORE", oggCust.VALORE_DATABASE);
                 string codiceAmministrazione = AmministraManager.AmmGetInfoAmmCorrente(_infoUtente.idAmministrazione).Codice;
                 value = value.Replace("COD_AMM", codiceAmministrazione);
                 value = value.Replace("COD_UO", oggCust.CODICE_DB);
                 int fine = oggCust.DATA_INSERIMENTO.LastIndexOf(".");
                 value = value.Replace("gg/mm/aaaa hh:mm", oggCust.DATA_INSERIMENTO.Substring(0, fine));
                 value = value.Replace("gg/mm/aaaa", oggCust.DATA_INSERIMENTO.Substring(0, 10));
                 if (!string.IsNullOrEmpty(oggCust.ID_AOO_RF) && oggCust.ID_AOO_RF != "0")
                 {
                     Registro reg = RegistriManager.getRegistro(oggCust.ID_AOO_RF);
                     if (reg != null)
                     {
                         value = value.Replace("RF", reg.codRegistro);
                         value = value.Replace("AOO", reg.codRegistro);
                     }
                 }
             }
             else
             {
                 value = oggCust.VALORE_DATABASE;
             }
             builder.AddTextPlaceholder(label, value);
        }

        private void HandleCorrispondentePlaceholder(SchedaDocumento sd,TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
            try
            {
                OggettoCustom oggCust = sd.template.getOggettoCustom(label);
                string idAmm = ImportUtils.GetAdministrationId(rowData.AdminCode);
                Corrispondente corr = ImportUtils.FindCorrispondente(values[0], oggCust, _infoUtente,_role, rowData.RFCode != null ? rowData.RFCode : String.Empty, rowData.RegCode != null ? rowData.RegCode : String.Empty, idAmm, _isEnabledSmistamento);
                if (corr != null)
                {
                    Corrispondente corrInd = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(corr.systemId);
                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(corr, corrInd, builder);
                    builder.AddTextPlaceholder(label, corrInfo.Descrizione);
                    builder.AddTextPlaceholder(label + "$indirizzo", corrInfo.Indirizzo);
                    builder.AddTextPlaceholder(label + "$telefono", corrInfo.Telefono);
                    builder.AddTextPlaceholder(label + "$indirizzo$telefono", corrInfo.IndirizzoTelefono);
                }
            }
            catch (Exception e) { }
        }

        private void HandleTextPlaceholder(SchedaDocumento sd, TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
            builder.AddTextPlaceholder(label, values, TextPosition.VERTICAL);
        }

        private void HandleExternalLinkPlaceholder(SchedaDocumento sd, TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
            if (values.Length < 2) return;
            builder.AddLinkPlaceholder(label, values[0], values[1]);
        }

        private void HandleInternalLinkDocPlaceholder(SchedaDocumento sd, TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
            if (values.Length < 2) return;

            BaseInfoDoc infoDoc = GetInfoDocumento(values[1]);
            if (infoDoc == null) return;
            bool acquisito = infoDoc.HaveFile;
            if (!acquisito)
            {
                builder.AddLinkPlaceholder(label, values[0], FELink + "visualizzaOggetto.aspx?idAmministrazione=" + _infoUtente.idAmministrazione + "&tipoOggetto=D&idObj=" + values[1]);
            }
            else
            {
                builder.AddLinkPlaceholder(label, values[0], FELink + "visualizzaLink.aspx?docNumber=" + values[1]);
            }
        }

        private void HandleInternalLinkFascPlaceholder(SchedaDocumento sd, TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
            if (values.Length < 2) return;
            Fascicolo fasc=GetInfoFascicolo(values[1]);
            if (fasc != null)
            {
                builder.AddLinkPlaceholder(label, values[0], FELink + "visualizzaOggetto.aspx?idAmministrazione=" + _infoUtente.idAmministrazione + "&tipoOggetto=F&idObj=" + fasc.codice);
            }
        }

        private void HandleOggettoEsterno(SchedaDocumento sd, TemplateBuilder builder, DocumentRowData rowData, string label, string[] values)
        {
            if (values.Length < 2) return;
            string value=values[0]+" - "+values[1];
            builder.AddTextPlaceholder(label, value);
        }

        private BaseInfoDoc GetInfoDocumento(string idDoc)
        {
            BaseInfoDoc infoDoc = null;
            try
            {
                List<BaseInfoDoc> infos = DocManager.GetBaseInfoForDocument(idDoc, null, null);
                if (infos.Count == 0) return null;
                infoDoc = infos[0];
            }
            catch (Exception e) { }
            if (infoDoc == null) return null;
            string errorMessage = "";
            int result = DocManager.VerificaACL("D", idDoc, _infoUtente, out errorMessage);
            if (result != 2) return null;
            return infoDoc;
        }

        private Fascicolo GetInfoFascicolo(string idFascicolo)
        {

            Fascicolo fasc = null;
            try
            {
                BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(idFascicolo, _infoUtente);
            }
            catch (Exception e) { }
            if (fasc == null) return null;
            String errorMessage = "";
            int result = DocManager.VerificaACL("F", fasc.systemID, _infoUtente, out errorMessage);
            if (result != 2) return null;
            return fasc;
        }

        private string FELink
        {
            get
            {
                return _infoUtente.urlWA+"/";
            }
        }

        private void HandleCampiComuni(InfoUtente infoUtente, TemplateBuilder builder, SchedaDocumento schedaDocumento)
        {
            try
            {
                logger.Debug("HandleCampiComuni begin");
                DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                DocsPaVO.amministrazione.OrgDettagliGlobali dettCorr = new DocsPaVO.amministrazione.OrgDettagliGlobali();
                DocsPaDB.Query_DocsPAWS.Utenti obj = new DocsPaDB.Query_DocsPAWS.Utenti();
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                if (infoAmm != null)
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.AMMINISTRAZIONE, infoAmm.Descrizione);
                logger.Debug("add oggetto");
                // OGGETTO
                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.OGGETTO, schedaDocumento.oggetto.descrizione);
                logger.Debug("add data creazione");
                // DATA CREAZIONE
                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DATA_CREAZIONE, schedaDocumento.dataCreazione);

                // ID DOCUMENTO
                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.ID_DOCUMENTO, schedaDocumento.docNumber);

                // NOTE 
                // Reperimento dell'ultima nota visibile a tutti
                logger.Debug("add note");
                string testoNote = string.Empty;

                foreach (DocsPaVO.Note.InfoNota nota in BusinessLogic.Note.NoteManager.GetNote(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, schedaDocumento.systemId), null))
                {
                    if (nota.TipoVisibilita == DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti)
                    {
                        testoNote = nota.Testo;
                        break;
                    }
                }

                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NOTE, testoNote);
                logger.Debug("fine add note");
                // TIPOLOGIA
                if (schedaDocumento.tipologiaAtto != null)
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TIPOLOGIA, schedaDocumento.tipologiaAtto.descrizione);

                if (schedaDocumento.creatoreDocumento != null)
                {
                    // CREATORE					
                    if (schedaDocumento.creatoreDocumento.idPeople != null && schedaDocumento.creatoreDocumento.idPeople != string.Empty)
                    {
                        corr = obj.GetCorrispondenteBySystemID(obj.GetIDUtCorr(schedaDocumento.creatoreDocumento.idPeople));
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CREATORE, corr.descrizione);
                    }

                    // RUOLO CREATORE				
                    if (schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != null && schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != string.Empty)
                    {
                        corr = obj.GetCorrispondenteBySystemID(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.RUOLO_CREATORE, corr.descrizione);
                    }
                    else
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.RUOLO_CREATORE, string.Empty);

                    // UO CREATORE				
                    if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
                    {
                        //descrizione
                        corr = obj.GetCorrispondenteBySystemID(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_CREATORE, corr.descrizione);

                        // UO PADRE
                        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                        string idCorrGlobali = string.Empty;
                        if (schedaDocumento.protocollatore != null)
                            idCorrGlobali = schedaDocumento.protocollatore.uo_idCorrGlobali;
                        else
                            idCorrGlobali = schedaDocumento.creatoreDocumento.idCorrGlob_UO;

                        int idParent = amm.AmmListaIDParentRicercaUO(Convert.ToInt32(idCorrGlobali));
                        if (idParent != 0)
                        {
                            corr = obj.GetCorrispondenteBySystemID(Convert.ToString(idParent));
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_PADRE, corr.descrizione);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.COD_UO_PADRE, corr.codiceRubrica);

                            //dettagli
                            dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(corr.systemId);
                            if (dettCorr != null)
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_PADRE, dettCorr.Indirizzo);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_PADRE, dettCorr.Citta);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_PADRE, dettCorr.Cap);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_PADRE, dettCorr.Provincia);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_PADRE, dettCorr.Telefono1);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_PADRE, dettCorr.Telefono2);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_PADRE, dettCorr.Fax);
                            }
                        }
                        else
                        {
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.COD_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_PADRE, string.Empty);
                        }

                        //dettagli
                        dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                        if (dettCorr != null)
                        {
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_CREATORE, dettCorr.Indirizzo);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_CREATORE, dettCorr.Citta);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_CREATORE, dettCorr.Cap);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NAZIONE_UO_CREATORE, dettCorr.Nazione);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_CREATORE, dettCorr.Provincia);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_CREATORE, dettCorr.Telefono1);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_CREATORE, dettCorr.Telefono2);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_CREATORE, dettCorr.Fax);
                        }
                    }
                    else
                    {
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NAZIONE_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_CREATORE, string.Empty);
                    }
                }
                logger.Debug("add ruolo resp UO");
                //Ruolo responsabile della UO
                if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
                {
                    // ruolo responsabile UO
                    OrgRuolo ruoloResp = Amministrazione.OrganigrammaManager.AmmGetRuoloResponsabileUO(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                    if (ruoloResp != null)
                    {
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.COD_RESP_UO, ruoloResp.CodiceRubrica);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESC_RESP_UO, ruoloResp.Descrizione);
                        if (ruoloResp.Utenti != null && ruoloResp.Utenti.Count > 0)
                        {
                            string listaUtenti = string.Empty;

                            for (int i = 0; i < ruoloResp.Utenti.Count; i++)
                            {
                                DocsPaVO.amministrazione.OrgUtente ut = ((DocsPaVO.amministrazione.OrgUtente)ruoloResp.Utenti[i]);
                                listaUtenti += ut.Nome + " " + ut.Cognome + ",";
                            }
                            if (listaUtenti != "")
                                listaUtenti = listaUtenti.Substring(0, listaUtenti.Length - 1);

                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.LISTA_UTENTE_RESP_UO, listaUtenti);
                        }
                        else
                        {
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty);
                        }
                    }
                    else
                    {
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.COD_RESP_UO, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESC_RESP_UO, string.Empty);
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty);
                    }

                    //lista utenti associati al ruolo
                    //ArrayList listaUtenti = Amministrazione.OrganigrammaManager.GetListUtentiRuolo(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                }
                logger.Debug("add classificazione doc");
                // Classificazioni del documento
                using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoliDb = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                {
                    string classifiche = string.Empty;

                    foreach (DocsPaVO.fascicolazione.Fascicolo item in fascicoliDb.GetFascicoliDaDoc(infoUtente, schedaDocumento.systemId))
                    {
                        if (classifiche != string.Empty)
                            classifiche += builder.NewLine();
                        classifiche += item.codice;
                    }

                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CLASSIFICHE, classifiche);
                }
                logger.Debug("add campi doc protocollato");
                // campi del documento protocollato
                if (schedaDocumento.protocollo != null)
                {
                    // NUMERO PROTOCOLLO
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NUM_PROTOCOLLO, schedaDocumento.protocollo.numero);

                    // SEGNATURA
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.SEGNATURA, schedaDocumento.protocollo.segnatura);

                    // DATA PROTOCOLLO
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DATA_PROTOCOLLO, schedaDocumento.protocollo.dataProtocollazione);

                    // DATA ORA PROTOCOLLO
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DATA_ORA_PROTOCOLLO, BusinessLogic.Documenti.ProtoManager.getDataOraProtocollo(schedaDocumento.docNumber));

                    // REGISTRO
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.REGISTRO, schedaDocumento.registro.descrizione);

                    // CODICE REGISTRO
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CODICE_REGISTRO, schedaDocumento.registro.codRegistro);

                    // NUMERO ALLEGATI
                    ArrayList listaAllegati = BusinessLogic.Documenti.AllegatiManager.getAllegati(schedaDocumento.docNumber, string.Empty);
                    if (listaAllegati != null)
                        builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NUM_ALLEGATI, Convert.ToString(listaAllegati.Count));

                    if (schedaDocumento.protocollatore != null)
                    {
                        // PROTOCOLLATORE						
                        if (schedaDocumento.protocollatore.utente_idPeople != null && schedaDocumento.protocollatore.utente_idPeople != string.Empty)
                        {
                            corr = obj.GetCorrispondenteBySystemID(obj.GetIDUtCorr(schedaDocumento.protocollatore.utente_idPeople));
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROTOCOLLATORE, corr.descrizione);
                        }

                        // RUOLO PROTOCOLLATORE					
                        if (schedaDocumento.protocollatore.ruolo_idCorrGlobali != null && schedaDocumento.protocollatore.ruolo_idCorrGlobali != string.Empty)
                        {
                            corr = obj.GetCorrispondenteBySystemID(schedaDocumento.protocollatore.ruolo_idCorrGlobali);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.RUOLO_PROTOCOLLATORE, corr.descrizione);
                        }

                        // UO PROTOCOLLATORE					
                        if (schedaDocumento.protocollatore.uo_idCorrGlobali != null && schedaDocumento.protocollatore.uo_idCorrGlobali != string.Empty)
                        {
                            //descrizione
                            corr = obj.GetCorrispondenteBySystemID(schedaDocumento.protocollatore.uo_idCorrGlobali);
                            builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_PROTOCOLLATORE, corr.descrizione);

                            //dettagli
                            dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(schedaDocumento.protocollatore.uo_idCorrGlobali);
                            if (dettCorr != null)
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_PROT, dettCorr.Indirizzo);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_PROT, dettCorr.Citta);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_PROT, dettCorr.Cap);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NAZIONE_UO_PROT, dettCorr.Nazione);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_PROT, dettCorr.Provincia);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_PROT, dettCorr.Telefono1);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_PROT, dettCorr.Telefono2);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_PROT, dettCorr.Fax);
                            }

                            // UO PADRE
                            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            int idParent = amm.AmmListaIDParentRicercaUO(Convert.ToInt32(schedaDocumento.protocollatore.uo_idCorrGlobali));
                            if (idParent != 0)
                            {
                                corr = obj.GetCorrispondenteBySystemID(Convert.ToString(idParent));
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_PADRE, corr.descrizione);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.COD_UO_PADRE, corr.codiceRubrica);

                                //dettagli
                                dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(corr.systemId);
                                if (dettCorr != null)
                                {
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_PADRE, dettCorr.Indirizzo);
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_PADRE, dettCorr.Citta);
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_PADRE, dettCorr.Cap);
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_PADRE, dettCorr.Provincia);
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_PADRE, dettCorr.Telefono1);
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_PADRE, dettCorr.Telefono2);
                                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_PADRE, dettCorr.Fax);
                                }
                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.COD_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_PADRE, string.Empty);
                            }
                        }
                    }

                    if (schedaDocumento.tipoProto != null)
                    {
                        string listaDestinatari = string.Empty;
                        string listaDestinatariIndirizzi = string.Empty;
                        string listaDestinatariTelefono = string.Empty;
                        string listaDestinatariIndirizzoTelefono = string.Empty;
                        string listaMittentiMultipli = string.Empty;
                        string listaMittentiMultipliIndirizzo = string.Empty;
                        string listaMittentiMultipliTelefono = string.Empty;
                        string listaMittentiMultipliIndirizzoTelefono = string.Empty;
                        string mittenteIndirizzo = string.Empty;
                        string mittenteTelefono = string.Empty;
                        string mittenteIndirizzoTelefono = string.Empty;
                        // Protocollo in INGRESSO (Arrivo)
                        if (schedaDocumento.tipoProto.Equals("A"))
                        {
                            DocsPaVO.documento.ProtocolloEntrata prot = new DocsPaVO.documento.ProtocolloEntrata();
                            prot = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                            // MITTENTE
                            if (prot.mittente != null)
                            {
                                DocsPaVO.utente.Corrispondente mittIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                                CorrispondenteInfo mittInfo = new CorrispondenteInfo(prot.mittente, mittIndirizzo, builder);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE, mittInfo.Descrizione);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO, mittInfo.Indirizzo);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_TELEFONO, mittInfo.Telefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittInfo.IndirizzoTelefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);


                            }
                            if (prot.mittenti != null && prot.mittenti.Count > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente mittMult in prot.mittenti)
                                {
                                    if (listaMittentiMultipli != string.Empty)
                                        listaMittentiMultipli += builder.NewLine();
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(mittMult.systemId);
                                    CorrispondenteInfo mittMultInfo = new CorrispondenteInfo(mittMult, corrIndirizzo, builder);
                                    listaMittentiMultipli += mittMultInfo.Descrizione;

                                    if (listaMittentiMultipliIndirizzo != string.Empty)
                                            listaMittentiMultipliIndirizzo += builder.NewLine();
                                    listaMittentiMultipliIndirizzo += mittMultInfo.Indirizzo;

                                    if (listaMittentiMultipliTelefono != string.Empty)
                                            listaMittentiMultipliTelefono += builder.NewLine();
                                    listaMittentiMultipliTelefono += mittMultInfo.Telefono;

                                    if (listaMittentiMultipliIndirizzoTelefono != string.Empty)
                                            listaMittentiMultipliIndirizzoTelefono += builder.NewLine();
                                    listaMittentiMultipliIndirizzoTelefono += mittMultInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI, listaMittentiMultipli);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, listaMittentiMultipliIndirizzo);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, listaMittentiMultipliTelefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, listaMittentiMultipliIndirizzoTelefono);


                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, string.Empty);
                            }

                            if (prot.ufficioReferente != null)
                            {
                                // CODICE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica);

                                // DESCRIZIONE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione);
                            }
                        }

                        // Protocollo in USCITA (Partenza)
                        if (schedaDocumento.tipoProto.Equals("P"))
                        {
                            DocsPaVO.documento.ProtocolloUscita prot = new DocsPaVO.documento.ProtocolloUscita();
                            prot = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                            // MITTENTE
                            if (prot.mittente != null)
                            {

                                DocsPaVO.utente.Corrispondente mittIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                                CorrispondenteInfo mittInfo = new CorrispondenteInfo(prot.mittente, mittIndirizzo, builder);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE, mittInfo.Descrizione);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO, mittInfo.Indirizzo);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_TELEFONO, mittInfo.Telefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittInfo.IndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            // DESTINATARI								
                            if (prot.destinatari != null && prot.destinatari.Count > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatari)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo destInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += destInfo.Descrizione;

                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += destInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += destInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += destInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI, listaDestinatari);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);

                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            logger.Debug("add destinatari CC");
                            // DESTINATARI CC
                            listaDestinatari = string.Empty;
                            listaDestinatariIndirizzi = string.Empty;
                            listaDestinatariTelefono = string.Empty;
                            listaDestinatariIndirizzoTelefono = string.Empty;
                            if (prot.destinatariConoscenza != null && prot.destinatariConoscenza.Count > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatariConoscenza)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += corrInfo.Descrizione;
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += corrInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += corrInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += corrInfo.IndirizzoTelefono;
                                }

                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC, listaDestinatari);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            if (prot.ufficioReferente != null)
                            {
                                // CODICE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica);

                                // DESCRIZIONE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione);
                            }
                        }
                        logger.Debug("check protocollo interno");
                        // Protocollo INTERNO
                        if (schedaDocumento.tipoProto.Equals("I"))
                        {
                            DocsPaVO.documento.ProtocolloInterno prot = new DocsPaVO.documento.ProtocolloInterno();
                            prot = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;

                            // MITTENTE					
                            if (prot.mittente != null)
                            {

                                DocsPaVO.utente.Corrispondente mittIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                                CorrispondenteInfo mittInfo = new CorrispondenteInfo(prot.mittente, mittIndirizzo, builder);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE, mittInfo.Descrizione);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO, mittInfo.Indirizzo);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_TELEFONO, mittInfo.Telefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittInfo.IndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            // DESTINATARI								
                            if (prot.destinatari != null && prot.destinatari.Count > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatari)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += corrInfo.Descrizione;
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += corrInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += corrInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += corrInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI, listaDestinatari);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            logger.Debug("add destinatari CC");
                            // DESTINATARI CC
                            listaDestinatari = string.Empty;
                            listaDestinatariIndirizzi = string.Empty;
                            listaDestinatariIndirizzoTelefono = string.Empty;
                            listaDestinatariTelefono = string.Empty;
                            if (prot.destinatariConoscenza != null && prot.destinatariConoscenza.Count > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatariConoscenza)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += corrInfo.Descrizione;
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += corrInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += corrInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += corrInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC, listaDestinatari);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            if (prot.ufficioReferente != null)
                            {
                                // CODICE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica);

                                // DESCRIZIONE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione);
                            }
                        }
                    }
                }
                else
                {
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NUM_PROTOCOLLO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.SEGNATURA, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DATA_PROTOCOLLO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.REGISTRO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CODICE_REGISTRO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROTOCOLLATORE, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.RUOLO_PROTOCOLLATORE, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.UO_PROTOCOLLATORE, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.INDIRIZZO_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CITTA_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.CAP_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.NAZIONE_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.PROVINCIA_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL1_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.TEL2_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.FAX_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(BusinessLogic.Modelli.BaseDocModelProcessor.DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);
                }
                logger.Debug("Fine HandleCampiComuni");
            }
            catch (Exception e)
            {
                logger.Debug("Eccezione in HandleCampiComuni: "+e);
            }
        }
    }
}
