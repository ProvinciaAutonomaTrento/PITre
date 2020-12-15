using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BusinessLogic.Modelli
{
    /// <summary>
    /// BaseClass per l'elaborazione dei modelli documenti
    /// </summary>
    public abstract class BaseDocModelProcessor : IModelProcessor
    {
        /// <summary>
        /// Identifica il tipo di modello per la prima versione del documento 
        /// </summary>
        public const string MODEL_VERSION_1 = "1";

        /// <summary>
        /// Identifica il tipo di modello dalla seconda versione del documento in poi
        /// </summary>
        public const string MODEL_VERSION_2 = "2";

        /// <summary>
        /// Identifica il tipo di modello per l'allegato del documento
        /// </summary>
        public const string MODEL_ATTATCHMENT = "A";

        /// <summary>
        /// Identifica il tipo di modello per la stampa della ricevuta di protocollo
        /// </summary>
        public const string MODEL_STAMPA_RICEVUTA = "STAMPA_RICEVUTA";

        /// <summary>
        /// Struttura delle chiavi comuni ai documenti
        /// </summary>
        public struct DocumentCommonFields
        {
            public const string OGGETTO = "Oggetto";
            public const string DATA_CREAZIONE = "Data creazione";
            public const string ID_DOCUMENTO = "ID Documento";
            public const string NOTE = "Note";
            public const string TIPOLOGIA = "Tipologia";
            public const string CREATORE = "Creatore";
            public const string RUOLO_CREATORE = "Ruolo creatore";
            public const string UO_CREATORE = "UO creatore";
            public const string INDIRIZZO_UO_CREATORE = "Indirizzo uo creatore";
            public const string CITTA_UO_CREATORE = "Citta' uo creatore";
            public const string CAP_UO_CREATORE = "Cap uo creatore";
            public const string NAZIONE_UO_CREATORE = "Nazione uo creatore";
            public const string PROVINCIA_UO_CREATORE = "Provincia uo creatore";
            public const string TEL1_UO_CREATORE = "Telefono1 uo creatore";
            public const string TEL2_UO_CREATORE = "Telefono2 uo creatore";
            public const string FAX_UO_CREATORE = "Fax uo creatore";
            public const string NUM_PROTOCOLLO = "Numero protocollo";
            public const string SEGNATURA = "Segnatura";
            public const string DATA_PROTOCOLLO = "Data protocollo";
            public const string REGISTRO = "Registro";
            public const string CODICE_REGISTRO = "Codice registro";
            public const string PROTOCOLLATORE = "Protocollatore";
            public const string RUOLO_PROTOCOLLATORE = "Ruolo protocollatore";
            public const string UO_PROTOCOLLATORE = "UO protocollatore";
            public const string INDIRIZZO_UO_PROT = "Indirizzo uo prot";
            public const string CITTA_UO_PROT = "Citta' uo prot";
            public const string CAP_UO_PROT = "Cap uo prot";
            public const string NAZIONE_UO_PROT = "Nazione uo prot";
            public const string PROVINCIA_UO_PROT = "Provincia uo prot";
            public const string TEL1_UO_PROT = "Telefono1 uo prot";
            public const string TEL2_UO_PROT = "Telefono2 uo prot";
            public const string FAX_UO_PROT = "Fax uo prot";
            public const string MITTENTE = "Mittente";
            public const string MITTENTE_INDIRIZZO = "Mittente$indirizzo";
            public const string MITTENTE_TELEFONO = "Mittente$telefono";
            public const string MITTENTE_INDIRIZZO_TELEFONO = "Mittente$indirizzo$telefono";
            public const string MITTENTI_MULTIPLI = "Mittenti_multipli";
            public const string MITTENTI_MULTIPLI_INDIRIZZO = "Mittenti_multipli$indirizzo";
            public const string MITTENTI_MULTIPLI_TELEFONO = "Mittenti_multipli$telefono";
            public const string MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO = "Mittenti_multipli$indirizzo$telefono";
            public const string DESTINATARI = "Destinatari";
            public const string DESTINATARI_INDIRIZZO = "Destinatari$indirizzo";
            public const string DESTINATARI_TELEFONO = "Destinatari$telefono";
            public const string DESTINATARI_INDIRIZZO_TELEFONO = "Destinatari$indirizzo$telefono";
            public const string DESTINATARI_CC = "Destinatari CC";
            public const string DESTINATARI_CC_INDIRIZZO = "Destinatari CC$indirizzo";
            public const string DESTINATARI_CC_TELEFONO = "Destinatari CC$telefono";
            public const string DESTINATARI_CC_INDIRIZZO_TELEFONO = "Destinatari CC$indirizzo$telefono";
            public const string COD_RESP_UO = "Codice Responsabile UO";
            public const string DESC_RESP_UO = "Descrizione Responsabile UO";
            public const string LISTA_UTENTE_RESP_UO = "Lista Utenti Ruolo Responsabile UO";
            public const string AMMINISTRAZIONE = "Amministrazione";
            public const string DATA_ORA_PROTOCOLLO = "Data Ora Protocollo";
            public const string NUM_ALLEGATI = "Numero Allegati";
            public const string UFF_REF_DESC = "Descrizione Ufficio Referente";
            public const string UFF_REF_COD = "Codice Ufficio Referente";
            public const string UO_PADRE = "Uo superiore";
            public const string COD_UO_PADRE = "Codice Uo superiore";
            public const string INDIRIZZO_UO_PADRE = "Indirizzo uo superiore";
            public const string CITTA_UO_PADRE = "Citta' uo superiore";
            public const string CAP_UO_PADRE = "Cap uo superiore";
            public const string PROVINCIA_UO_PADRE = "Provincia uo superiore";
            public const string TEL1_UO_PADRE = "Telefono1 uo superiore";
            public const string TEL2_UO_PADRE = "Telefono2 uo superiore";
            public const string FAX_UO_PADRE = "Fax uo superiore";
            public const string CLASSIFICHE = "Classifiche";
            
        }

        /// <summary>
        /// Elaborazione del modello richiesto
        /// </summary>
        /// <param name="request">
        /// Dati di richiesta del modello
        /// </param>
        /// <returns></returns>
        public abstract DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        protected abstract DocsPaVO.Modelli.ModelKeyValuePair[] GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento);

        /// <summary>
        /// Reperimento della scheda documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual DocsPaVO.documento.SchedaDocumento GetDocument(DocsPaVO.utente.InfoUtente infoUtente, string id)
        {
            // Reperimento scheda documento
            DocsPaVO.documento.SchedaDocumento document = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, id, id);

            if (document == null)
                throw new ApplicationException(string.Format("Nessun documento trovato con id {0}", id));

            return document;
        }

        /// <summary>
        /// Inizializzazione valori dei campi comuni
        /// </summary>
        /// <param name="keyValues"></param>
        protected virtual void InitCommonFields(ArrayList keyValues)
        {
            this.AppendKeyValue(DocumentCommonFields.OGGETTO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DATA_CREAZIONE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.ID_DOCUMENTO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NOTE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TIPOLOGIA, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.RUOLO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CITTA_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CAP_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NAZIONE_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL1_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL2_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.FAX_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NUM_PROTOCOLLO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.SEGNATURA, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DATA_PROTOCOLLO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.REGISTRO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CODICE_REGISTRO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROTOCOLLATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.RUOLO_PROTOCOLLATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UO_PROTOCOLLATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CITTA_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CAP_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NAZIONE_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL1_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL2_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.FAX_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.COD_RESP_UO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESC_RESP_UO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.AMMINISTRAZIONE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DATA_ORA_PROTOCOLLO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NUM_ALLEGATI, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UFF_REF_COD, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CLASSIFICHE, string.Empty, keyValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDocumento"></param>
        protected virtual void FetchCommonFields(ArrayList keyValues, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {


            //Campi profilati di tipo Corrispondente
            for (int i = 0; i < keyValues.Count; i++)
            {
                //Campi indirizzo, telefono e inidirizzo+telefono per il Corrispondente profilato
                string[] corrProf = (string[])keyValues[i];

                if (corrProf != null && corrProf.Length > 2 && corrProf[2] == "Corrispondente")
                {

                    string field = corrProf[0] + "$indirizzo";
                    AppendKeyValue(field, corrProf[3], keyValues);
                    string field1 = corrProf[0] + "$telefono";
                    AppendKeyValue(field1, corrProf[6], keyValues);
                    string field2 = corrProf[0] + "$indirizzo$telefono";
                    AppendKeyValue(field2, corrProf[7], keyValues);
                }
            }
            // Inizializzazione campi comuni
            this.InitCommonFields(keyValues);
            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
            DocsPaDB.Query_DocsPAWS.Utenti obj = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.amministrazione.OrgDettagliGlobali dettCorr = new DocsPaVO.amministrazione.OrgDettagliGlobali();
            DocsPaVO.amministrazione.OrgRuolo ruoloResp = new DocsPaVO.amministrazione.OrgRuolo();

            // DESCRIZIONE AMMINISTRAZIONE
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
            if (infoAmm != null)
                AppendKeyValue(DocumentCommonFields.AMMINISTRAZIONE, infoAmm.Descrizione, keyValues);

            // OGGETTO
            AppendKeyValue(DocumentCommonFields.OGGETTO, schedaDocumento.oggetto.descrizione, keyValues);

            // DATA CREAZIONE
            AppendKeyValue(DocumentCommonFields.DATA_CREAZIONE, schedaDocumento.dataCreazione, keyValues);

            // ID DOCUMENTO
            AppendKeyValue(DocumentCommonFields.ID_DOCUMENTO, schedaDocumento.docNumber, keyValues);

            // NOTE 
            // Reperimento dell'ultima nota visibile a tutti
            string testoNote = string.Empty;

            foreach (DocsPaVO.Note.InfoNota nota in BusinessLogic.Note.NoteManager.GetNote(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, schedaDocumento.systemId), null))
            {
                if (nota.TipoVisibilita == DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti)
                {
                    testoNote = nota.Testo;
                    break;
                }
            }
                                                    
            AppendKeyValue(DocumentCommonFields.NOTE, testoNote, keyValues);

            // TIPOLOGIA
            if (schedaDocumento.tipologiaAtto != null)
                AppendKeyValue(DocumentCommonFields.TIPOLOGIA, schedaDocumento.tipologiaAtto.descrizione, keyValues);

            if (schedaDocumento.creatoreDocumento != null)
            {
                // CREATORE					
                if (schedaDocumento.creatoreDocumento.idPeople != null && schedaDocumento.creatoreDocumento.idPeople != string.Empty)
                {
                    corr = obj.GetCorrispondenteBySystemID(obj.GetIDUtCorr(schedaDocumento.creatoreDocumento.idPeople));
                    AppendKeyValue(DocumentCommonFields.CREATORE, corr.descrizione, keyValues);
                }

                // RUOLO CREATORE				
                if (schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != null && schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != string.Empty)
                {
                    corr = obj.GetCorrispondenteBySystemID(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo);
                    AppendKeyValue(DocumentCommonFields.RUOLO_CREATORE, corr.descrizione, keyValues);
                }
                else
                    AppendKeyValue(DocumentCommonFields.RUOLO_CREATORE, string.Empty, keyValues);

                // UO CREATORE				
                if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
                {
                    //descrizione
                    corr = obj.GetCorrispondenteBySystemID(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                    AppendKeyValue(DocumentCommonFields.UO_CREATORE, corr.descrizione, keyValues);

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
                        AppendKeyValue(DocumentCommonFields.UO_PADRE, corr.descrizione, keyValues);
                        AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, corr.codiceRubrica, keyValues);

                        //dettagli
                        dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(corr.systemId);
                        if (dettCorr != null)
                        {
                            AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE, dettCorr.Indirizzo, keyValues);
                            AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, dettCorr.Citta, keyValues);
                            AppendKeyValue(DocumentCommonFields.CAP_UO_PADRE, dettCorr.Cap, keyValues);
                            AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, dettCorr.Provincia, keyValues);
                            AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, dettCorr.Telefono1, keyValues);
                            AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, dettCorr.Telefono2, keyValues);
                            AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, dettCorr.Fax, keyValues);
                        }
                    }
                    else
                    {
                        AppendKeyValue(DocumentCommonFields.UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.CAP_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, string.Empty, keyValues);
                        AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, string.Empty, keyValues);
                    }

                    //dettagli
                    dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                    if (dettCorr != null)
                    {
                        AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_CREATORE, dettCorr.Indirizzo, keyValues);
                        AppendKeyValue(DocumentCommonFields.CITTA_UO_CREATORE, dettCorr.Citta, keyValues);
                        AppendKeyValue(DocumentCommonFields.CAP_UO_CREATORE, dettCorr.Cap, keyValues);
                        AppendKeyValue(DocumentCommonFields.NAZIONE_UO_CREATORE, dettCorr.Nazione, keyValues);
                        AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_CREATORE, dettCorr.Provincia, keyValues);
                        AppendKeyValue(DocumentCommonFields.TEL1_UO_CREATORE, dettCorr.Telefono1, keyValues);
                        AppendKeyValue(DocumentCommonFields.TEL2_UO_CREATORE, dettCorr.Telefono2, keyValues);
                        AppendKeyValue(DocumentCommonFields.FAX_UO_CREATORE, dettCorr.Fax, keyValues);
                    }
                }
                else
                {
                    AppendKeyValue(DocumentCommonFields.UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.CITTA_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.CAP_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.NAZIONE_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.TEL1_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.TEL2_UO_CREATORE, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.FAX_UO_CREATORE, string.Empty, keyValues);
                }
            }

            //Ruolo responsabile della UO
            if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
            {
                // ruolo responsabile UO
                ruoloResp = Amministrazione.OrganigrammaManager.AmmGetRuoloResponsabileUO(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                if (ruoloResp != null)
                {
                    AppendKeyValue(DocumentCommonFields.COD_RESP_UO, ruoloResp.CodiceRubrica, keyValues);
                    AppendKeyValue(DocumentCommonFields.DESC_RESP_UO, ruoloResp.Descrizione, keyValues);
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

                        AppendKeyValue(DocumentCommonFields.LISTA_UTENTE_RESP_UO, listaUtenti, keyValues);
                    }
                    else
                    {
                        AppendKeyValue(DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty, keyValues);
                    }
                }
                else
                {
                    AppendKeyValue(DocumentCommonFields.COD_RESP_UO, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.DESC_RESP_UO, string.Empty, keyValues);
                    AppendKeyValue(DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty, keyValues);
                }

                //lista utenti associati al ruolo
                //ArrayList listaUtenti = Amministrazione.OrganigrammaManager.GetListUtentiRuolo(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
            }

            // Classificazioni del documento
            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoliDb = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                string classifiche = string.Empty;

                foreach (DocsPaVO.fascicolazione.Fascicolo item in fascicoliDb.GetFascicoliDaDoc(infoUtente, schedaDocumento.systemId))
                {
                    if (classifiche != string.Empty)
                        classifiche += Environment.NewLine;
                    classifiche += item.codice;
                }

                AppendKeyValue(DocumentCommonFields.CLASSIFICHE, classifiche, keyValues);
            }

            // campi del documento protocollato
            if (schedaDocumento.protocollo != null)
            {
                // NUMERO PROTOCOLLO
                AppendKeyValue(DocumentCommonFields.NUM_PROTOCOLLO, schedaDocumento.protocollo.numero, keyValues);

                // SEGNATURA
                AppendKeyValue(DocumentCommonFields.SEGNATURA, schedaDocumento.protocollo.segnatura, keyValues);

                // DATA PROTOCOLLO
                AppendKeyValue(DocumentCommonFields.DATA_PROTOCOLLO, schedaDocumento.protocollo.dataProtocollazione, keyValues);

                // DATA ORA PROTOCOLLO
                AppendKeyValue(DocumentCommonFields.DATA_ORA_PROTOCOLLO, BusinessLogic.Documenti.ProtoManager.getDataOraProtocollo(schedaDocumento.docNumber), keyValues);

                // REGISTRO
                AppendKeyValue(DocumentCommonFields.REGISTRO, schedaDocumento.registro.descrizione, keyValues);

                // CODICE REGISTRO
                AppendKeyValue(DocumentCommonFields.CODICE_REGISTRO, schedaDocumento.registro.codRegistro, keyValues);

                // NUMERO ALLEGATI
                ArrayList listaAllegati = BusinessLogic.Documenti.AllegatiManager.getAllegati(schedaDocumento.docNumber, string.Empty);
                if (listaAllegati != null)
                    AppendKeyValue(DocumentCommonFields.NUM_ALLEGATI, Convert.ToString(listaAllegati.Count), keyValues);

                if (schedaDocumento.protocollatore != null)
                {
                    // PROTOCOLLATORE						
                    if (schedaDocumento.protocollatore.utente_idPeople != null && schedaDocumento.protocollatore.utente_idPeople != string.Empty)
                    {
                        corr = obj.GetCorrispondenteBySystemID(obj.GetIDUtCorr(schedaDocumento.protocollatore.utente_idPeople));
                        AppendKeyValue(DocumentCommonFields.PROTOCOLLATORE, corr.descrizione, keyValues);
                    }

                    // RUOLO PROTOCOLLATORE					
                    if (schedaDocumento.protocollatore.ruolo_idCorrGlobali != null && schedaDocumento.protocollatore.ruolo_idCorrGlobali != string.Empty)
                    {
                        corr = obj.GetCorrispondenteBySystemID(schedaDocumento.protocollatore.ruolo_idCorrGlobali);
                        AppendKeyValue(DocumentCommonFields.RUOLO_PROTOCOLLATORE, corr.descrizione, keyValues);
                    }

                    // UO PROTOCOLLATORE					
                    if (schedaDocumento.protocollatore.uo_idCorrGlobali != null && schedaDocumento.protocollatore.uo_idCorrGlobali != string.Empty)
                    {
                        //descrizione
                        corr = obj.GetCorrispondenteBySystemID(schedaDocumento.protocollatore.uo_idCorrGlobali);
                        AppendKeyValue(DocumentCommonFields.UO_PROTOCOLLATORE, corr.descrizione, keyValues);

                        //dettagli
                        dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(schedaDocumento.protocollatore.uo_idCorrGlobali);
                        if (dettCorr != null)
                        {
                            AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PROT, dettCorr.Indirizzo, keyValues);
                            AppendKeyValue(DocumentCommonFields.CITTA_UO_PROT, dettCorr.Citta, keyValues);
                            AppendKeyValue(DocumentCommonFields.CAP_UO_PROT, dettCorr.Cap, keyValues);
                            AppendKeyValue(DocumentCommonFields.NAZIONE_UO_PROT, dettCorr.Nazione, keyValues);
                            AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PROT, dettCorr.Provincia, keyValues);
                            AppendKeyValue(DocumentCommonFields.TEL1_UO_PROT, dettCorr.Telefono1, keyValues);
                            AppendKeyValue(DocumentCommonFields.TEL2_UO_PROT, dettCorr.Telefono2, keyValues);
                            AppendKeyValue(DocumentCommonFields.FAX_UO_PROT, dettCorr.Fax, keyValues);
                        }

                        // UO PADRE
                        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                        int idParent = amm.AmmListaIDParentRicercaUO(Convert.ToInt32(schedaDocumento.protocollatore.uo_idCorrGlobali));
                        if (idParent != 0)
                        {
                            corr = obj.GetCorrispondenteBySystemID(Convert.ToString(idParent));
                            AppendKeyValue(DocumentCommonFields.UO_PADRE, corr.descrizione, keyValues);
                            AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, corr.codiceRubrica, keyValues);

                            //dettagli
                            dettCorr = Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(corr.systemId);
                            if (dettCorr != null)
                            {
                                AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE, dettCorr.Indirizzo, keyValues);
                                AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, dettCorr.Citta, keyValues);
                                AppendKeyValue(DocumentCommonFields.CAP_UO_PADRE, dettCorr.Cap, keyValues);
                                AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, dettCorr.Provincia, keyValues);
                                AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, dettCorr.Telefono1, keyValues);
                                AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, dettCorr.Telefono2, keyValues);
                                AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, dettCorr.Fax, keyValues);
                            }
                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.CAP_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, string.Empty, keyValues);
                        }
                    }
                }

                if (schedaDocumento.tipoProto != null)
                {
                    string listaDestinatari = string.Empty;
                    string listaDestinatariIndirizzi= string.Empty;
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
                            mittenteIndirizzo += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita;
                            mittenteTelefono += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;
                            mittenteIndirizzoTelefono += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita + 
                                Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2; 
                            AppendKeyValue(DocumentCommonFields.MITTENTE, prot.mittente.descrizione, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, mittenteIndirizzo, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO,mittenteTelefono , keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO,mittenteIndirizzoTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                            

                        }
                        if (prot.mittenti != null && prot.mittenti.Count > 0)
                        {
                            foreach (DocsPaVO.utente.Corrispondente mittMult in prot.mittenti)
                            {
                                if (listaMittentiMultipli != string.Empty)
                                    listaMittentiMultipli += Environment.NewLine;
                                listaMittentiMultipli += mittMult.descrizione;

                                DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(mittMult.systemId);
                                if (corrIndirizzo != null)
                                {
                                    if (listaMittentiMultipliIndirizzo != string.Empty)
                                        listaMittentiMultipliIndirizzo += Environment.NewLine;
                                    listaMittentiMultipliIndirizzo += mittMult.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita;

                                    if (listaMittentiMultipliTelefono != string.Empty)
                                        listaMittentiMultipliTelefono += Environment.NewLine;
                                    listaMittentiMultipliTelefono += mittMult.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;

                                    if (listaMittentiMultipliIndirizzoTelefono != string.Empty)
                                        listaMittentiMultipliIndirizzoTelefono += Environment.NewLine;
                                    listaMittentiMultipliIndirizzoTelefono += mittMult.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                    + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita +
                                    Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;
                                }
                            }
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI, listaMittentiMultipli, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, listaMittentiMultipliIndirizzo, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, listaMittentiMultipliTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, listaMittentiMultipliIndirizzoTelefono, keyValues);


                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }

                        if (prot.ufficioReferente != null)
                        {
                            // CODICE UFFICIO REFERENTE
                            AppendKeyValue(DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica, keyValues);

                            // DESCRIZIONE UFFICIO REFERENTE
                            AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione, keyValues);
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
                            mittenteIndirizzo += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita;
                            
                            
                            mittenteTelefono += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;
                            mittenteIndirizzoTelefono += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita +
                                Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;

                            AppendKeyValue(DocumentCommonFields.MITTENTE, prot.mittente.descrizione, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, mittenteIndirizzo, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, mittenteTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittenteIndirizzoTelefono, keyValues);
                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.MITTENTE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }
                        // DESTINATARI								
                        if (prot.destinatari != null && prot.destinatari.Count > 0)
                        {
                            foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatari)
                            {
                                if (listaDestinatari != string.Empty)
                                    listaDestinatari += Environment.NewLine;
                                listaDestinatari += utCorr.descrizione;

                                DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                if (corrIndirizzo != null)
                                {
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += Environment.NewLine;
                                    listaDestinatariIndirizzi += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += Environment.NewLine;
                                    listaDestinatariTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += Environment.NewLine;
                                    listaDestinatariIndirizzoTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                    + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita +
                                    Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;
                                }
                            }
                            
                            AppendKeyValue(DocumentCommonFields.DESTINATARI, listaDestinatari, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, listaDestinatariIndirizzi, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, listaDestinatariTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono, keyValues);

                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }
                        // DESTINATARI CC
                        listaDestinatari = string.Empty;
                        listaDestinatariIndirizzi = string.Empty;
                        listaDestinatariTelefono = string.Empty;
                        listaDestinatariIndirizzoTelefono = string.Empty;
                        if (prot.destinatariConoscenza != null && prot.destinatariConoscenza.Count > 0)
                        {
                            foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatariConoscenza)
                            {
                                if (listaDestinatari != string.Empty)
                                    listaDestinatari += Environment.NewLine;
                                listaDestinatari += utCorr.descrizione;

                                DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                if (corrIndirizzo != null)
                                {
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += Environment.NewLine;
                                    listaDestinatariIndirizzi += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += Environment.NewLine;
                                    listaDestinatariTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += Environment.NewLine;
                                    listaDestinatariIndirizzoTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita +
                                        Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;
                                }
                            }

                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, listaDestinatari, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, listaDestinatariIndirizzi, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, listaDestinatariTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono, keyValues);
                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }
                        if (prot.ufficioReferente != null)
                        {
                            // CODICE UFFICIO REFERENTE
                            AppendKeyValue(DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica, keyValues);

                            // DESCRIZIONE UFFICIO REFERENTE
                            AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione, keyValues);
                        }
                    }

                    // Protocollo INTERNO
                    if (schedaDocumento.tipoProto.Equals("I"))
                    {
                        DocsPaVO.documento.ProtocolloInterno prot = new DocsPaVO.documento.ProtocolloInterno();
                        prot = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;

                        // MITTENTE					
                        if (prot.mittente != null)
                        {

                            DocsPaVO.utente.Corrispondente mittIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                            mittenteIndirizzo += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita;
                            mittenteTelefono += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;
                            mittenteIndirizzoTelefono += prot.mittente.descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita +
                                Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;

                            AppendKeyValue(DocumentCommonFields.MITTENTE, prot.mittente.descrizione, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, mittenteIndirizzo, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, mittenteTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittenteIndirizzoTelefono, keyValues);
                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.MITTENTE, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }
                        // DESTINATARI								
                        if (prot.destinatari != null && prot.destinatari.Count > 0)
                        {
                            foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatari)
                            {
                                if (listaDestinatari != string.Empty)
                                    listaDestinatari += Environment.NewLine;
                                listaDestinatari += utCorr.descrizione;

                                DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                if (corrIndirizzo != null)
                                {
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += Environment.NewLine;
                                    listaDestinatariIndirizzi += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += Environment.NewLine;
                                    listaDestinatariTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += Environment.NewLine;
                                    listaDestinatariIndirizzoTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                    + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita +
                                    Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;
                                }
                            }

                            AppendKeyValue(DocumentCommonFields.DESTINATARI, listaDestinatari, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, listaDestinatariIndirizzi, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, listaDestinatariTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono, keyValues);
                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }

                        // DESTINATARI CC
                        listaDestinatari = string.Empty;
                        listaDestinatariIndirizzi = string.Empty;
                        listaDestinatariIndirizzoTelefono = string.Empty;
                        listaDestinatariTelefono = string.Empty;
                        if (prot.destinatariConoscenza != null && prot.destinatariConoscenza.Count > 0)
                        {
                            foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatariConoscenza)
                            {
                                if (listaDestinatari != string.Empty)
                                    listaDestinatari += Environment.NewLine;
                                listaDestinatari += utCorr.descrizione;

                                DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                                if (corrIndirizzo != null)
                                {
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += Environment.NewLine;
                                    listaDestinatariIndirizzi += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += Environment.NewLine;
                                    listaDestinatariTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += Environment.NewLine;
                                    listaDestinatariIndirizzoTelefono += utCorr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                                        + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita +
                                        Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;
                                }
                            }

                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, listaDestinatari, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, listaDestinatariIndirizzi, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, listaDestinatariTelefono, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono, keyValues);
                        }
                        else
                        {
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
                            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                        }
                        if (prot.ufficioReferente != null)
                        {
                            // CODICE UFFICIO REFERENTE
                            AppendKeyValue(DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica, keyValues);

                            // DESCRIZIONE UFFICIO REFERENTE
                            AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione, keyValues);
                        }
                    }
                }
            }
            else
            {
                AppendKeyValue(DocumentCommonFields.NUM_PROTOCOLLO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.SEGNATURA, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DATA_PROTOCOLLO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.REGISTRO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.CODICE_REGISTRO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.PROTOCOLLATORE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.RUOLO_PROTOCOLLATORE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.UO_PROTOCOLLATORE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.CITTA_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.CAP_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.NAZIONE_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.TEL1_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.TEL2_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.FAX_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            }
        }

        /// <summary>
        /// Aggiunge chiavi (comune)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        /// <param name="listaChiaviValori"></param>
        /// <returns></returns>
        private void AppendKeyValue(string key, string description, ArrayList keyValues)
        {
            bool found = false;

            foreach (Array item in keyValues)
            {
                if (item.GetValue(0).ToString().ToUpper().Equals(key.ToUpper()))
                {
                    item.SetValue(description, 1);
                    found = true;
                    break;
                }
            }

            if (!found)
                keyValues.Add(new string[2] 
                            { 
                                key, 
                                (string.IsNullOrEmpty(description) ? string.Empty : description)
                            });
        }
    }
}
