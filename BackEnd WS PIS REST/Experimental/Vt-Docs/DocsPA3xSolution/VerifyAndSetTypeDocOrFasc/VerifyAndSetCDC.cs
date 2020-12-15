using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.ProfilazioneDinamica;
using System.Text.RegularExpressions;

namespace VerifyAndSetTypeDocOrFasc
{
    class VerifyAndSetCDC : VerifyAndSetManager
    {
        public override string verifyTipoDoc(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            string message = string.Empty;
            //Viene effettuato il controllo solo se si tratta di un protocollo in entrata
            if (schedaDocumento != null &&
                schedaDocumento.protocollo != null &&
                schedaDocumento.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata)
                )
            {
                DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare = schedaDocumento;

                //Controllo Preventivo
                if (schedaDocumentoDaVerificare != null &&
                    schedaDocumentoDaVerificare.template != null &&
                    schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("CONTROLLO PREVENTIVO SCCLA")
                    )
                {
                    ControlloTipologiaDocControlloPreventivo(ref schedaDocumentoDaVerificare, ref message, infoUtente);

                    //Se i controlli sono andati tutti a buon fine aggiorno la scheda documento
                    if (string.IsNullOrEmpty(message))
                        schedaDocumento = schedaDocumentoDaVerificare;
                }

                /****************************************************************/
                /****************************************************************/
                /****************************************************************/
                //Modifica Iacozzilli Giordano 23/05/2012
                //AGGIUNGO IL CONTROLLO AL TIPO DOCUMENTO "CONTROLLO SUCCESSIVO SCCLA"
                if (schedaDocumentoDaVerificare != null &&
                    schedaDocumentoDaVerificare.template != null &&
                    schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("CONTROLLO SUCCESSIVO SCCLA")
                    )
                {
                    /******************************************************************************************* */
                    /******************************************************************************************* */
                    /******************************************************************************************* */
                    ControlloTipologiaDocControlloSuccessivo(ref schedaDocumentoDaVerificare, ref message, infoUtente);

                    //Se i controlli sono andati tutti a buon fine aggiorno la scheda documento
                    if (string.IsNullOrEmpty(message))
                        schedaDocumento = schedaDocumentoDaVerificare;
                }
                /****************************************************************/
                /****************************************************************/
                //FINE
                /****************************************************************/

                //Controllo Pensioni
                if (schedaDocumentoDaVerificare != null &&
                    schedaDocumentoDaVerificare.template != null &&
                    (schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("PENSIONI CIVILI SCCLA") ||
                    schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("PENSIONI MILITARI SCCLA"))
                    )
                {
                    ControlloTipologiaDocPensioni(ref schedaDocumentoDaVerificare, ref message);

                    //Se i controlli sono andati tutti a buon fine aggiorno la scheda documento
                    if (string.IsNullOrEmpty(message))
                        schedaDocumento = schedaDocumentoDaVerificare;
                }

                //Controllo Preventivo SRC
                if (schedaDocumentoDaVerificare != null &&
                    schedaDocumentoDaVerificare.template != null &&
                    schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("CONTROLLO PREVENTIVO SRC")
                    )
                {
                    ControlloTipologiaDocControlloPreventivoSRC(ref schedaDocumentoDaVerificare, ref message, infoUtente);

                    //Se i controlli sono andati tutti a buon fine aggiorno la scheda documento
                    if (string.IsNullOrEmpty(message))
                        schedaDocumento = schedaDocumentoDaVerificare;
                }
            }

            //Viene effettuato il controllo sempre indipendentemente se un grigio o un protocollo
            if (schedaDocumento != null)
            {
                DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare = schedaDocumento;

                //Controllo Pareri SCCLA
                if (schedaDocumentoDaVerificare != null &&
                    schedaDocumentoDaVerificare.template != null &&
                    schedaDocumentoDaVerificare.template.DESCRIZIONE.ToUpper().Equals("PARERI SCCLA")
                    )
                {
                    ControlloPareriSCCLA(ref schedaDocumentoDaVerificare, ref message, infoUtente);
                    //Se i controlli sono andati tutti a buon fine aggiorno la scheda documento
                    if (string.IsNullOrEmpty(message))
                        schedaDocumento = schedaDocumentoDaVerificare;
                }
            }

            return message;
        }

        public override string verifyTipoFasc(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            return string.Empty;
        }

        private void ControlloTipologiaDocControlloPreventivo(ref DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare, ref string message, DocsPaVO.utente.InfoUtente infoUtente)
        {
            OggettoCustom[] elencoOggetti = (OggettoCustom[])schedaDocumentoDaVerificare.template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom));

            OggettoCustom dtaScadenzaControllo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SCADENZA CONTROLLO")).FirstOrDefault();
            OggettoCustom dtaScadenzaAmministrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SCADENZA AMMINISTRAZIONE")).FirstOrDefault();
            OggettoCustom stato = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("STATO")).FirstOrDefault();
            OggettoCustom registroFoglio = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("REGISTRO-FOGLIO")).FirstOrDefault();
            OggettoCustom dtaRestAmministrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RESTITUZ. AMMINISTRAZIONE")).FirstOrDefault();
            OggettoCustom dtaInvioDeferimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA INVIO DEFERIMENTO")).FirstOrDefault();
            OggettoCustom dtaRitornoSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SEZIONE")).FirstOrDefault();
            OggettoCustom numeroOsservazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO OSSERVAZIONE")).FirstOrDefault();
            OggettoCustom dtaOsservazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA OSSERVAZIONE")).FirstOrDefault();
            OggettoCustom dtaRichiestaRitiro = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RICHIESTA RITIRO")).FirstOrDefault();
            OggettoCustom dtaDelibera = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DELIBERA")).FirstOrDefault();
            OggettoCustom esitoAdunanza = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ESITO ADUNANZA")).FirstOrDefault();
            OggettoCustom dtaDecreto = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DECRETO")).FirstOrDefault();
            OggettoCustom dtaRitornoPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRitornoSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom trasmessoPerCompetenza = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TRASMESSO PER COMPETENZA")).FirstOrDefault();
            OggettoCustom dtaRegistrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA REGISTRAZIONE")).FirstOrDefault();
            OggettoCustom numDelibera = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. DELIBERA")).FirstOrDefault();

            OggettoCustom numPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom numSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom numRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM RILIEVO")).FirstOrDefault();

            OggettoCustom dtaPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RILIEVO")).FirstOrDefault();

            OggettoCustom elencoTrasmPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom elencoTrasmSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom numElencoRilievi = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO ELENCO RILIEVI")).FirstOrDefault();

            OggettoCustom dtaElencoTrasmPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaElencoTrasmSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaElencoRilievi = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO RILIEVI")).FirstOrDefault();

            OggettoCustom elencoTrasmVersoRagioneria = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. VERSO RAGIONERIA")).FirstOrDefault();
            OggettoCustom dtaTrasmVersoRagioneria = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA TRASM. VERSO RAGIONERIA")).FirstOrDefault();
            OggettoCustom numElencoTrasm = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO ELENCO TRASM")).FirstOrDefault();
            OggettoCustom dtaElencoTrasmissione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO TRASMISSIONE")).FirstOrDefault();

            //Controllo validità Date
            VerifyDate(elencoOggetti, ref message);
            if (!String.IsNullOrEmpty(message))
                return;

            //Reset di campi calcolati
            if (numElencoTrasm != null)
                numElencoTrasm.VALORE_DATABASE = string.Empty;
            if (dtaElencoTrasmissione != null)
                dtaElencoTrasmissione.VALORE_DATABASE = string.Empty;

            //Se la data di arrivo non è valorizzata, viene impostata con la data di creazione
            if (string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
            {
                if (schedaDocumentoDaVerificare.protocollo != null && schedaDocumentoDaVerificare.protocollo.dataProtocollazione != null)
                {
                    ((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo = schedaDocumentoDaVerificare.protocollo.dataProtocollazione;
                }
                else
                {
                    ((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo = System.DateTime.Now.ToString("dd/MM/yyyy");
                }
            }

            //*******************************************************************************************************************
            //MEV IACOZZILLI GIORDANO 09/07/2012
            //DEVO AGGIUNGERE AI 2 CONTROLLI SOTTO
            //LA IF SUL TIPO DECRETO cipe_40

            //Data primo rilievo <= data arrivo + 60 
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
            {
                OggettoCustom Tipologie_4_control = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TIPOLOGIA")).FirstOrDefault();

                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataArrivo;
                if (Tipologie_4_control.VALORE_DATABASE == "CIPE_40")
                    dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo).AddDays(40);
                else
                    dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo).AddDays(60);

                if (dataPrimoRilievo > dataArrivo)
                    message += "\\nLa -Data Primo Rilievo- risulta successiva alla scadenza dei termini previsti per il controllo";
            }

            //MEV IACOZZILLI GIORDANO 09/07/2012
            //Va aggiunto il controllo sulla data primo rilievo <= alla data protocollo +60
            //Data primo rilievo <= data protocollo + 60 
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                  !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
            {
                OggettoCustom Tipologie_4_control = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TIPOLOGIA")).FirstOrDefault();
                DateTime dataProtocollo;
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                if (Tipologie_4_control.VALORE_DATABASE == "CIPE_40")
                    dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(40);
                else
                    dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60);

                if (dataPrimoRilievo > dataProtocollo)
                    message += "\\nLa -Data Primo Rilievo- risulta successiva alla scadenza dei termini previsti per il controllo";
            }

            //*******************************************************************************************************************
            //FINE
            //*******************************************************************************************************************

            //Data primo rilievo >= data protocollo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                if (dataPrimoRilievo < dataProtocollo)
                    message += "\\nLa -Data Primo Rilievo- risulta minore della data di protocollo";
            }

            //CONTROLLO DATA SCADENZA CONTROLLO - Senza Rilievo

            /****************************************************************/
            /****************************************************************/
            /****************************************************************/
            //Modifica Iacozzilli Giordano 23/05/2012
            //
            //Quando il valore della combo tipologia è CIPE_40 allora la data scadenza sarà + 40 gg,
            //Nuova specoifica del 05/07/2012, nel caso di CIPE_40 devo dare la scadenza admin a + 20 gg.
            //altrimenti rimane +60 gg.
            //OLD CODE:
            //if (dtaScadenzaControllo != null)
            //{
            //    if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
            //    {
            //        if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
            //            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60).ToShortDateString();
            //        else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
            //            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60).ToShortDateString();
            //    }
            //}
            //NEW CODE:
            if (dtaScadenzaControllo != null)
            {
                OggettoCustom Tipologie = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TIPOLOGIA")).FirstOrDefault();

                if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                {
                    if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                        if (Tipologie.VALORE_DATABASE == "CIPE_40")
                        {
                            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(40).ToShortDateString();
                            dtaScadenzaAmministrazione.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(20).ToShortDateString();
                        }
                        else
                            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60).ToShortDateString();
                    else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                        if (Tipologie.VALORE_DATABASE == "CIPE_40")
                        {
                            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(40).ToShortDateString();
                            dtaScadenzaAmministrazione.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(20).ToShortDateString();
                        }
                        else
                            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60).ToShortDateString();
                }
            }

            //CONTROLLO DATA SCADENZA CONTROLLO - Con Rilievo senza rispota a riliveo

            //**********************************************************************************
            //MEV Iacozzilli Giordano 11/07/2012
            //**********************************************************************************
            //Aggiungo le if sul CIPE_40
            if (dtaScadenzaControllo != null && dtaPrimoRilievo != null && dtaRitornoPrimoRilievo != null)
            {
                if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                {
                    if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                    {
                        OggettoCustom Tipologie = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TIPOLOGIA")).FirstOrDefault();

                        if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                            if (Tipologie.VALORE_DATABASE == "CIPE_40")
                                dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60).ToShortDateString();
                            else
                                dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(90).ToShortDateString();

                        else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                            if (Tipologie.VALORE_DATABASE == "CIPE_40")
                                dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60).ToShortDateString();
                            else
                                dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(90).ToShortDateString();
                    }
                }
            }

            //CONTROLLO DATA SCADENZA CONTROLLO - Con Rilievo e rispota a riliveo

            //**********************************************************************************
            //MEV Iacozzilli Giordano 11/07/2012
            //**********************************************************************************
            //Aggiungo le if sul CIPE_40

            if (dtaScadenzaControllo != null && dtaPrimoRilievo != null && dtaRitornoPrimoRilievo != null)
            {
                if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                {
                    DateTime datePrimoRilevo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                    DateTime dateRitornoPrimoRilevo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
                    int diffDate = dateRitornoPrimoRilevo.Subtract(datePrimoRilevo).Days;
                    OggettoCustom Tipologie = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TIPOLOGIA")).FirstOrDefault();

                    if (diffDate >= 30)
                    {
                        if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                        {
                            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                                    if (Tipologie.VALORE_DATABASE == "CIPE_40")
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60).ToShortDateString();
                                    else
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(90).ToShortDateString();

                                else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                                    if (Tipologie.VALORE_DATABASE == "CIPE_40")
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60).ToShortDateString();
                                    else
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(90).ToShortDateString();
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                        {
                            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                                    if (Tipologie.VALORE_DATABASE == "CIPE_40")
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(40 + diffDate).ToShortDateString();
                                    else
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60 + diffDate).ToShortDateString();
                                else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                                    if (Tipologie.VALORE_DATABASE == "CIPE_40")
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(40 + diffDate).ToShortDateString();
                                    else
                                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60 + diffDate).ToShortDateString();
                            }
                        }

                    }
                }
            }

            //CONTROLLO DATA SCADENZA AMMINISTRAZIONE

            //**********************************************************************************
            //MEV Iacozzilli Giordano 11/07/2012
            //**********************************************************************************
            //Aggiungo le if sul CIPE_40

            if (dtaScadenzaAmministrazione != null && dtaPrimoRilievo != null)
            {
                if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE))
                {
                    OggettoCustom Tipologie = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TIPOLOGIA")).FirstOrDefault();
                    if (Tipologie.VALORE_DATABASE == "CIPE_40")
                        dtaScadenzaAmministrazione.VALORE_DATABASE = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE).AddDays(20).ToShortDateString();
                    else
                        dtaScadenzaAmministrazione.VALORE_DATABASE = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE).AddDays(30).ToShortDateString();
                }
            }


            //CONTROLLO STATO
            //"In esame"
            if (registroFoglio != null && string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) &&
                //dtaRitornoPrimoRilievo != null && string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
                 dtaRestAmministrazione != null && string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                //dtaInvioSezione != null && string.IsNullOrEmpty(dtaInvioSezione.VALORE_DATABASE) &&
                //dtaRitornoSezione != null && string.IsNullOrEmpty(dtaRitornoSezione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "In esame";
            }

            //"Registrato senza rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaOsservazione != null && string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato senza rilievo";
            }

            //"Registrato a seguito di rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo";
            }

            //"Registrato a seguito di rilievo a vuoto"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaOsservazione != null && !string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo a vuoto";
            }

            //"Annullato"
            if (schedaDocumentoDaVerificare.protocollo.protocolloAnnullato != null)
            {
                stato.VALORE_DATABASE = "Annullato";
            }

            //"Restituito all'amministrazione"
            if (dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE))
            {
                stato.VALORE_DATABASE = "Restituito all'amministrazione";
            }

            //"Ammesso al visto della sezione"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaInvioDeferimento != null && !string.IsNullOrEmpty(dtaInvioDeferimento.VALORE_DATABASE) &&
                dtaDelibera != null && !string.IsNullOrEmpty(dtaDelibera.VALORE_DATABASE) &&
                dtaRitornoSezione != null && !string.IsNullOrEmpty(dtaRitornoSezione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Ammesso al visto della sezione";
            }

            //"Trasmesso per competenza"
            //if (trasmessoPerCompetenza != null && trasmessoPerCompetenza.VALORE_DATABASE.ToUpper().Equals("SI"))
            //{
            //    stato.VALORE_DATABASE = "Trasmesso per competenza";
            //}

            //"Non luogo a deliberare deciso dalla sezione"
            if (esitoAdunanza != null && esitoAdunanza.VALORE_DATABASE.ToUpper().Equals("NON LUOGO A DELIBERARE"))
            {
                stato.VALORE_DATABASE = "Non luogo a deliberare deciso dalla sezione";
            }

            //"Ricusazione del visto dalla sezione"
            if (esitoAdunanza != null && esitoAdunanza.VALORE_DATABASE.ToUpper().Equals("RICUSAZIONE VISTO"))
            {
                stato.VALORE_DATABASE = "Ricusazione del visto della sezione";
            }

            //CONTROLLO DATE
            //"Data decreto" <= "Data arrivo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                if (dataDecreto > dataArrivo)
                    message += "\\nLa -Data decreto- è maggiore della -Data arrivo-";
            }

            //"Data decreto" <= "Data protocollo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                if (dataDecreto > dataProtocollo)
                    message += "\\nLa -Data decreto- è maggiore della -Data protocollo-";
            }

            //"Data arrivo" <= "Data primo rilievo"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                if (dataArrivo > dataRilievo)
                    message += "\\nLa -Data arrivo- è maggiore della -Data primo rilievo-";
            }

            //"Data primo rilievo" <= "Data ritorno primo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
               dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataRitornoRilievo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
                if (dataRilievo > dataRitornoRilievo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data ritorno primo Rilievo-";
            }

            //"Data secondo rilievo" <= "Data ritorno secondo rilievo"
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataSecondoRilievo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                DateTime dataRitornoSecondoRilievo = Convert.ToDateTime(dtaRitornoSecondoRilievo.VALORE_DATABASE);
                if (dataSecondoRilievo > dataRitornoSecondoRilievo)
                    message += "\\nLa -Data secondo rilievo- è maggiore della -Data ritorno secondo rilievo-";
            }

            //"Data decreto" <= "Data arrivo"
            //Già fatto sopra

            //"Data arrivo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataArrivo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data arrivo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data protocollo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione) &&
                dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataProtocollo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data protocollo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data ritorno primo rilievo" (valorizzata), "Data primo rilievo" (valorizzata)
            if (dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno primo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //"Data ritorno secondo rilievo" (valorizzata), "Data secondo rilievo" (valorizzata)
            if (dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno secondo rilievo- è valorizzata ma la -Data secondo rilievo- no";
            }

            //"Data primo rilievo" <= "Data secondo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataSecondo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataSecondo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data secondo Rilievo-";
            }

            //"Data secondo rilievo" (Valorizzata), "Data primo rilievo" (non valorizzata)
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data secondo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //Data Registrazione (non valorizzata) assume il valore della data inserimento del campo "Registro-Foglio"
            if (dtaRegistrazione != null && string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO))
                )
            {
                dtaRegistrazione.VALORE_DATABASE = System.DateTime.Now.ToString("dd/MM/yyyy");
            }

            //Data Registrazione - da subordinare alla effettiva modifica della data e ai diritti sul campo
            if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) && !string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber))
            {
                DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = DBManagerCDC.getDirittiCampo(infoUtente.idGruppo, schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString());
                if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1")
                {
                    string dataRegistrazioneDB = DBManagerCDC.getValoreCampoDB(dtaRegistrazione, schedaDocumentoDaVerificare.docNumber);
                    if (dataRegistrazioneDB != null && dataRegistrazioneDB != dtaRegistrazione.VALORE_DATABASE)
                    {
                        //Data registrazione (valorizzata) <= della data attuale
                        if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE))
                        {
                            DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                            DateTime dataAttuale = Convert.ToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));
                            if (dataRegistrazione > dataAttuale)
                                message += "\\nLa -Data Registrazione- è maggiore della data attuale";
                        }

                        //Data registrazione (valorizzata) >= della maggiore data registrazione per l'RF in questione
                        if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                            registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                            !string.IsNullOrEmpty(registroFoglio.ID_AOO_RF)
                            )
                        {
                            DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                            DateTime maxDataRegistrazione = DBManagerCDC.getMaxDataRegistrazione(schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString(), registroFoglio.ID_AOO_RF);
                            if (dataRegistrazione < maxDataRegistrazione)
                                message += "\\nLa -Data Registrazione- è minore della maggiore Data Registrazione esistente per questo RF";
                        }
                    }
                }
            }

            //Controllo per evitare che siano valorizzate le date registrazione e trasmissione verso la ragioneria
            //prima dell'effettiva registrazione del decreto
            if (registroFoglio != null &&
                (string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) && !registroFoglio.CONTATORE_DA_FAR_SCATTARE) &&
                    (
                    (dtaRegistrazione != null && !String.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)) ||
                    (elencoTrasmVersoRagioneria != null && !String.IsNullOrEmpty(elencoTrasmVersoRagioneria.VALORE_DATABASE)) ||
                    (dtaTrasmVersoRagioneria != null && !String.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE))
                    )
                )
            {
                message += "\\nRisultano valorizzati valorizzati i seguenti dati : \\n-DATA REGISTRAZIONE \\n-ELENCO TRASM VERSO RAGIONERIA \\n-DATA TRASM VERSO RAGIONERIA \\nma non ci sono gli estremi di registrazione";
            }

            //Num Rilievo
            if (numPrimoRilievo != null && !string.IsNullOrEmpty(numPrimoRilievo.VALORE_DATABASE) && numRilievo != null)
                numRilievo.VALORE_DATABASE = numPrimoRilievo.VALORE_DATABASE;

            if (numSecondoRilievo != null && !string.IsNullOrEmpty(numSecondoRilievo.VALORE_DATABASE) && numRilievo != null)
                numRilievo.VALORE_DATABASE = numSecondoRilievo.VALORE_DATABASE;

            //Data Rilievo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && dtaRilievo != null)
                dtaRilievo.VALORE_DATABASE = dtaPrimoRilievo.VALORE_DATABASE;

            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) && dtaRilievo != null)
                dtaRilievo.VALORE_DATABASE = dtaSecondoRilievo.VALORE_DATABASE;

            //Numero Elenco Rilievi
            if (elencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(elencoTrasmPrimoRilievo.VALORE_DATABASE) && numElencoRilievi != null)
                numElencoRilievi.VALORE_DATABASE = elencoTrasmPrimoRilievo.VALORE_DATABASE;

            if (elencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(elencoTrasmSecondoRilievo.VALORE_DATABASE) && numElencoRilievi != null)
                numElencoRilievi.VALORE_DATABASE = elencoTrasmSecondoRilievo.VALORE_DATABASE;

            //Data Elenco Rilievi
            if (dtaElencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(dtaElencoTrasmPrimoRilievo.VALORE_DATABASE) && dtaElencoRilievi != null)
                dtaElencoRilievi.VALORE_DATABASE = dtaElencoTrasmPrimoRilievo.VALORE_DATABASE;

            if (dtaElencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(dtaElencoTrasmSecondoRilievo.VALORE_DATABASE) && dtaElencoRilievi != null)
                dtaElencoRilievi.VALORE_DATABASE = dtaElencoTrasmSecondoRilievo.VALORE_DATABASE;

            //Data rilievo valorizzata e data trasm verso ragioneria non valorizzata
            if (dtaRilievo != null && !string.IsNullOrEmpty(dtaRilievo.VALORE_DATABASE) &&
                dtaTrasmVersoRagioneria != null && string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE) &&
                numElencoRilievi != null &&
                dtaElencoTrasmissione != null &&
                numElencoTrasm != null
                )
            {
                numElencoTrasm.VALORE_DATABASE = numElencoRilievi.VALORE_DATABASE;
                dtaElencoTrasmissione.VALORE_DATABASE = dtaRilievo.VALORE_DATABASE;
            }

            //Data rilievo non valorizzata e data trasm verso ragioneria valorizzata
            if (dtaRilievo != null && string.IsNullOrEmpty(dtaRilievo.VALORE_DATABASE) &&
                dtaTrasmVersoRagioneria != null && !string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE) &&
                elencoTrasmVersoRagioneria != null &&
                dtaElencoTrasmissione != null &&
                numElencoTrasm != null
                )
            {
                numElencoTrasm.VALORE_DATABASE = elencoTrasmVersoRagioneria.VALORE_DATABASE;
                dtaElencoTrasmissione.VALORE_DATABASE = dtaTrasmVersoRagioneria.VALORE_DATABASE;
            }

            //Data rilievo valorizzata e data trasm verso ragioneria valorizzata
            if (dtaRilievo != null && !string.IsNullOrEmpty(dtaRilievo.VALORE_DATABASE) &&
                dtaTrasmVersoRagioneria != null && !string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE) &&
                dtaElencoTrasmissione != null &&
                numElencoTrasm != null &&
                elencoTrasmVersoRagioneria != null &&
                numElencoRilievi != null
            )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaRilievo.VALORE_DATABASE);
                DateTime dataTrasmVersoRagioneria = Convert.ToDateTime(dtaTrasmVersoRagioneria.VALORE_DATABASE);
                if (dataRilievo > dataTrasmVersoRagioneria)
                {
                    numElencoTrasm.VALORE_DATABASE = numElencoRilievi.VALORE_DATABASE;
                    dtaElencoTrasmissione.VALORE_DATABASE = dtaRilievo.VALORE_DATABASE;
                }
                else if (dataRilievo <= dataTrasmVersoRagioneria)
                {
                    numElencoTrasm.VALORE_DATABASE = elencoTrasmVersoRagioneria.VALORE_DATABASE;
                    dtaElencoTrasmissione.VALORE_DATABASE = dtaTrasmVersoRagioneria.VALORE_DATABASE;
                }
            }

            //Controllo Valorizzazione Campi
            if (numPrimoRilievo != null && !string.IsNullOrEmpty(numPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Primo Rilievo deve essere valorizzata";
            }

            if (numSecondoRilievo != null && !string.IsNullOrEmpty(numSecondoRilievo.VALORE_DATABASE) &&
               dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Secondo Rilievo deve essere valorizzata";
            }

            if (numeroOsservazione != null && !string.IsNullOrEmpty(numeroOsservazione.VALORE_DATABASE) &&
               dtaOsservazione != null && string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE))
            {
                message += "\\nLa -Data Osservazione deve essere valorizzata";
            }

            if (elencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(elencoTrasmPrimoRilievo.VALORE_DATABASE) &&
               dtaElencoTrasmPrimoRilievo != null && string.IsNullOrEmpty(dtaElencoTrasmPrimoRilievo.VALORE_DATABASE))
            {
                message += "\\nLa -Data Elenco Trasm. Primo rilievo deve essere valorizzata";
            }

            if (elencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(elencoTrasmSecondoRilievo.VALORE_DATABASE) &&
                dtaElencoTrasmSecondoRilievo != null && string.IsNullOrEmpty(dtaElencoTrasmSecondoRilievo.VALORE_DATABASE))
            {
                message += "\\nLa -Data Elenco Trasm. Secondo rilievo deve essere valorizzata";
            }

            if (elencoTrasmVersoRagioneria != null && !string.IsNullOrEmpty(elencoTrasmVersoRagioneria.VALORE_DATABASE) &&
               dtaTrasmVersoRagioneria != null && string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE))
            {
                message += "\\nLa -Data Trasm. Verso Ragioneria deve essere valorizzata";
            }

            if (numDelibera != null && !string.IsNullOrEmpty(numDelibera.VALORE_DATABASE) &&
               dtaDelibera != null && string.IsNullOrEmpty(dtaDelibera.VALORE_DATABASE))
            {
                message += "\\nLa -Data Delibera deve essere valorizzata";
            }

            //Se è valorizzata la data di registrazione  non può essere valorizzata la data di restituzione amministrazione e viceversa
            if (((dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)) || (registroFoglio.CONTATORE_DA_FAR_SCATTARE)) && dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE))
            {
                message +=
                    "\\nNon è possibile registrare un decreto restituito oppure restituire un decreto registrato";
            }

            //La data di primo rilievo non può essere maggiore della data di scadenza controllo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && dtaScadenzaControllo != null && !string.IsNullOrEmpty(dtaScadenzaControllo.VALORE_DATABASE))
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataScadenzaControllo = Convert.ToDateTime(dtaScadenzaControllo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataScadenzaControllo)
                    message += "\\nLa -Data Primo Rilievo- non può essere maggiore della data di scadenza controllo";

            }

            //Aggiungo la visibilità di questo documento ad uno specifico ruolo, il tutto è fatto tramite una stored
            if (!string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber) &&
                registroFoglio != null &&
                registroFoglio.CONTATORE_DA_FAR_SCATTARE &&
                string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)
                )
            {
                DBManagerCDC.setVisiblitaDocumento(schedaDocumentoDaVerificare.docNumber);
            }
            else
            {
                if (string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber) &&
                registroFoglio != null &&
                registroFoglio.CONTATORE_DA_FAR_SCATTARE &&
                string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)
                )
                {
                    message += "\\nLa Registrazione non può essere contestuale alla protocollazione";
                }
            }

            //Se i controlli sono andati tutti a buon fine aggiorno i campi
            if (string.IsNullOrEmpty(message))
                schedaDocumentoDaVerificare.template.ELENCO_OGGETTI = new System.Collections.ArrayList(elencoOggetti);


        }

        private void ControlloTipologiaDocControlloSuccessivo(ref DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare, ref string message, DocsPaVO.utente.InfoUtente infoUtente)
        {
            OggettoCustom[] elencoOggetti = (OggettoCustom[])schedaDocumentoDaVerificare.template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom));
            OggettoCustom stato = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("STATO")).FirstOrDefault();
            OggettoCustom registroFoglio = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("REGISTRO-FOGLIO")).FirstOrDefault();
            OggettoCustom dtaRestAmministrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RESTITUZ. AMMINISTRAZIONE")).FirstOrDefault();
            OggettoCustom dtaInvioDeferimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA INVIO DEFERIMENTO")).FirstOrDefault();
            OggettoCustom dtaRitornoSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SEZIONE")).FirstOrDefault();
            OggettoCustom numeroOsservazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO OSSERVAZIONE")).FirstOrDefault();
            OggettoCustom dtaOsservazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA OSSERVAZIONE")).FirstOrDefault();
            OggettoCustom dtaRichiestaRitiro = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RICHIESTA RITIRO")).FirstOrDefault();
            OggettoCustom dtaDelibera = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DELIBERA")).FirstOrDefault();
            OggettoCustom esitoAdunanza = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ESITO ADUNANZA")).FirstOrDefault();
            OggettoCustom dtaDecreto = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DECRETO")).FirstOrDefault();
            OggettoCustom dtaRitornoPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRitornoSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SECONDO RILIEVO")).FirstOrDefault();

            OggettoCustom dtaRegistrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA REGISTRAZIONE")).FirstOrDefault();
            OggettoCustom numDelibera = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. DELIBERA")).FirstOrDefault();

            OggettoCustom numPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom numSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom numRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM RILIEVO")).FirstOrDefault();

            OggettoCustom dtaPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RILIEVO")).FirstOrDefault();

            OggettoCustom elencoTrasmPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom elencoTrasmSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom numElencoRilievi = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO ELENCO RILIEVI")).FirstOrDefault();

            OggettoCustom dtaElencoTrasmPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaElencoTrasmSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaElencoRilievi = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO RILIEVI")).FirstOrDefault();

            OggettoCustom elencoTrasmVersoRagioneria = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. VERSO RAGIONERIA")).FirstOrDefault();
            OggettoCustom dtaTrasmVersoRagioneria = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA TRASM. VERSO RAGIONERIA")).FirstOrDefault();
            OggettoCustom numElencoTrasm = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO ELENCO TRASM")).FirstOrDefault();
            OggettoCustom dtaElencoTrasmissione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO TRASMISSIONE")).FirstOrDefault();

            //Reset di campi calcolati
            if (numElencoTrasm != null)
                numElencoTrasm.VALORE_DATABASE = string.Empty;
            if (dtaElencoTrasmissione != null)
                dtaElencoTrasmissione.VALORE_DATABASE = string.Empty;

            //Se la data di arrivo non è valorizzata, viene impostata con la data di creazione
            if (string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
            {
                if (schedaDocumentoDaVerificare.protocollo != null && schedaDocumentoDaVerificare.protocollo.dataProtocollazione != null)
                {
                    ((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo = schedaDocumentoDaVerificare.protocollo.dataProtocollazione;
                }
                else
                {
                    ((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo = System.DateTime.Now.ToString("dd/MM/yyyy");
                }
            }


            //Data primo rilievo >= data protocollo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                if (dataPrimoRilievo < dataProtocollo)
                    message += "\\nLa -Data Primo Rilievo- risulta minore della data di protocollo";
            }


            //CONTROLLO STATO
            //"In esame"
            if (registroFoglio != null && string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) &&
                 dtaRestAmministrazione != null && string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "In esame";
            }

            //"Registrato senza rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaOsservazione != null && string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato senza rilievo";
            }

            //"Registrato a seguito di rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo";
            }

            //"Registrato a seguito di rilievo a vuoto"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaOsservazione != null && !string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo a vuoto";
            }

            //"Annullato"
            if (schedaDocumentoDaVerificare.protocollo.protocolloAnnullato != null)
            {
                stato.VALORE_DATABASE = "Annullato";
            }

            //"Restituito all'amministrazione"
            if (dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE))
            {
                stato.VALORE_DATABASE = "Restituito all'amministrazione";
            }

            //"Ammesso al visto della sezione"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaInvioDeferimento != null && !string.IsNullOrEmpty(dtaInvioDeferimento.VALORE_DATABASE) &&
                dtaDelibera != null && !string.IsNullOrEmpty(dtaDelibera.VALORE_DATABASE) &&
                dtaRitornoSezione != null && !string.IsNullOrEmpty(dtaRitornoSezione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Ammesso al visto della sezione";
            }

            //"Trasmesso per competenza"
            //if (trasmessoPerCompetenza != null && trasmessoPerCompetenza.VALORE_DATABASE.ToUpper().Equals("SI"))
            //{
            //    stato.VALORE_DATABASE = "Trasmesso per competenza";
            //}

            //"Non luogo a deliberare deciso dalla sezione"
            if (esitoAdunanza != null && esitoAdunanza.VALORE_DATABASE.ToUpper().Equals("NON LUOGO A DELIBERARE"))
            {
                stato.VALORE_DATABASE = "Non luogo a deliberare deciso dalla sezione";
            }

            //"Ricusazione del visto dalla sezione"
            if (esitoAdunanza != null && esitoAdunanza.VALORE_DATABASE.ToUpper().Equals("RICUSAZIONE VISTO"))
            {
                stato.VALORE_DATABASE = "Ricusazione del visto della sezione";
            }

            //CONTROLLO DATE
            //"Data decreto" <= "Data arrivo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                if (dataDecreto > dataArrivo)
                    message += "\\nLa -Data decreto- è maggiore della -Data arrivo-";
            }

            //"Data decreto" <= "Data protocollo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                if (dataDecreto > dataProtocollo)
                    message += "\\nLa -Data decreto- è maggiore della -Data protocollo-";
            }

            //"Data arrivo" <= "Data primo rilievo"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                if (dataArrivo > dataRilievo)
                    message += "\\nLa -Data arrivo- è maggiore della -Data primo rilievo-";
            }

            //"Data primo rilievo" <= "Data ritorno primo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
               dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataRitornoRilievo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
                if (dataRilievo > dataRitornoRilievo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data ritorno primo Rilievo-";
            }

            /****************************************************************/
            /****************************************************************/
            //Modifica Iacozzilli Giordano 23/05/2012
            //"Data primo rilievo" <= "data registrazione"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
              dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)
               )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                if (dataRilievo > dataRegistrazione)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data Registrazione -";
            }
            //FINE
            /****************************************************************/
            /****************************************************************/

            /****************************************************************/
            /****************************************************************/
            //Modifica Iacozzilli Giordano 23/05/2012
            // "Data primo rilievo" <= "Data restituzione amministrazione"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
              dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
               )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataRestAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataRilievo > dataRestAmministrazione)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data Restituzione Amministrazione -";
            }
            //FINE
            /****************************************************************/
            /****************************************************************/


            //"Data secondo rilievo" <= "Data ritorno secondo rilievo"
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataSecondoRilievo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                DateTime dataRitornoSecondoRilievo = Convert.ToDateTime(dtaRitornoSecondoRilievo.VALORE_DATABASE);
                if (dataSecondoRilievo > dataRitornoSecondoRilievo)
                    message += "\\nLa -Data secondo rilievo- è maggiore della -Data ritorno secondo rilievo-";
            }

            //"Data decreto" <= "Data arrivo"
            //Già fatto sopra

            //"Data arrivo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataArrivo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data arrivo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data protocollo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione) &&
                dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataProtocollo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data protocollo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data ritorno primo rilievo" (valorizzata), "Data primo rilievo" (valorizzata)
            if (dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno primo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //"Data ritorno secondo rilievo" (valorizzata), "Data secondo rilievo" (valorizzata)
            if (dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno secondo rilievo- è valorizzata ma la -Data secondo rilievo- no";
            }

            //"Data primo rilievo" <= "Data secondo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataSecondo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataSecondo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data secondo Rilievo-";
            }

            //"Data secondo rilievo" (Valorizzata), "Data primo rilievo" (non valorizzata)
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data secondo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //Data Registrazione (non valorizzata) assume il valore della data inserimento del campo "Registro-Foglio"
            if (dtaRegistrazione != null && string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO))
                )
            {
                dtaRegistrazione.VALORE_DATABASE = System.DateTime.Now.ToString("dd/MM/yyyy");
            }

            //Data Registrazione - da subordinare alla effettiva modifica della data e ai diritti sul campo
            if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) && !string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber))
            {
                DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = DBManagerCDC.getDirittiCampo(infoUtente.idGruppo, schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString());
                if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1")
                {
                    string dataRegistrazioneDB = DBManagerCDC.getValoreCampoDB(dtaRegistrazione, schedaDocumentoDaVerificare.docNumber);
                    if (dataRegistrazioneDB != null && dataRegistrazioneDB != dtaRegistrazione.VALORE_DATABASE)
                    {
                        //Data registrazione (valorizzata) <= della data attuale
                        if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE))
                        {
                            DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                            DateTime dataAttuale = Convert.ToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));
                            if (dataRegistrazione > dataAttuale)
                                message += "\\nLa -Data Registrazione- è maggiore della data attuale";
                        }

                        //Data registrazione (valorizzata) >= della maggiore data registrazione per l'RF in questione
                        if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                            registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                            !string.IsNullOrEmpty(registroFoglio.ID_AOO_RF)
                            )
                        {
                            DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                            DateTime maxDataRegistrazione = DBManagerCDC.getMaxDataRegistrazione(schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString(), registroFoglio.ID_AOO_RF);
                            if (dataRegistrazione < maxDataRegistrazione)
                                message += "\\nLa -Data Registrazione- è minore della maggiore Data Registrazione esistente per questo RF";
                        }
                    }
                }
            }

            //Controllo per evitare che siano valorizzate le date registrazione e trasmissione verso la ragioneria
            //prima dell'effettiva registrazione del decreto
            if (registroFoglio != null &&
                (string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) && !registroFoglio.CONTATORE_DA_FAR_SCATTARE) &&
                    (
                    (dtaRegistrazione != null && !String.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)) ||
                    (elencoTrasmVersoRagioneria != null && !String.IsNullOrEmpty(elencoTrasmVersoRagioneria.VALORE_DATABASE)) ||
                    (dtaTrasmVersoRagioneria != null && !String.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE))
                    )
                )
            {
                message += "\\nRisultano valorizzati valorizzati i seguenti dati : \\n-DATA REGISTRAZIONE \\n-ELENCO TRASM VERSO RAGIONERIA \\n-DATA TRASM VERSO RAGIONERIA \\nma non ci sono gli estremi di registrazione";
            }

            //Num Rilievo
            if (numPrimoRilievo != null && !string.IsNullOrEmpty(numPrimoRilievo.VALORE_DATABASE) && numRilievo != null)
                numRilievo.VALORE_DATABASE = numPrimoRilievo.VALORE_DATABASE;

            if (numSecondoRilievo != null && !string.IsNullOrEmpty(numSecondoRilievo.VALORE_DATABASE) && numRilievo != null)
                numRilievo.VALORE_DATABASE = numSecondoRilievo.VALORE_DATABASE;

            //Data Rilievo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && dtaRilievo != null)
                dtaRilievo.VALORE_DATABASE = dtaPrimoRilievo.VALORE_DATABASE;

            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) && dtaRilievo != null)
                dtaRilievo.VALORE_DATABASE = dtaSecondoRilievo.VALORE_DATABASE;

            //Numero Elenco Rilievi
            if (elencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(elencoTrasmPrimoRilievo.VALORE_DATABASE) && numElencoRilievi != null)
                numElencoRilievi.VALORE_DATABASE = elencoTrasmPrimoRilievo.VALORE_DATABASE;

            if (elencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(elencoTrasmSecondoRilievo.VALORE_DATABASE) && numElencoRilievi != null)
                numElencoRilievi.VALORE_DATABASE = elencoTrasmSecondoRilievo.VALORE_DATABASE;

            //Data Elenco Rilievi
            if (dtaElencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(dtaElencoTrasmPrimoRilievo.VALORE_DATABASE) && dtaElencoRilievi != null)
                dtaElencoRilievi.VALORE_DATABASE = dtaElencoTrasmPrimoRilievo.VALORE_DATABASE;

            if (dtaElencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(dtaElencoTrasmSecondoRilievo.VALORE_DATABASE) && dtaElencoRilievi != null)
                dtaElencoRilievi.VALORE_DATABASE = dtaElencoTrasmSecondoRilievo.VALORE_DATABASE;





            //Controllo Valorizzazione Campi
            if (numPrimoRilievo != null && !string.IsNullOrEmpty(numPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Primo Rilievo deve essere valorizzata";
            }

            if (numSecondoRilievo != null && !string.IsNullOrEmpty(numSecondoRilievo.VALORE_DATABASE) &&
               dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Secondo Rilievo deve essere valorizzata";
            }

            if (numeroOsservazione != null && !string.IsNullOrEmpty(numeroOsservazione.VALORE_DATABASE) &&
               dtaOsservazione != null && string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE))
            {
                message += "\\nLa -Data Osservazione deve essere valorizzata";
            }

            if (elencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(elencoTrasmPrimoRilievo.VALORE_DATABASE) &&
               dtaElencoTrasmPrimoRilievo != null && string.IsNullOrEmpty(dtaElencoTrasmPrimoRilievo.VALORE_DATABASE))
            {
                message += "\\nLa -Data Elenco Trasm. Primo rilievo deve essere valorizzata";
            }

            if (elencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(elencoTrasmSecondoRilievo.VALORE_DATABASE) &&
                dtaElencoTrasmSecondoRilievo != null && string.IsNullOrEmpty(dtaElencoTrasmSecondoRilievo.VALORE_DATABASE))
            {
                message += "\\nLa -Data Elenco Trasm. Secondo rilievo deve essere valorizzata";
            }

            if (elencoTrasmVersoRagioneria != null && !string.IsNullOrEmpty(elencoTrasmVersoRagioneria.VALORE_DATABASE) &&
               dtaTrasmVersoRagioneria != null && string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE))
            {
                message += "\\nLa -Data Trasm. Verso Ragioneria deve essere valorizzata";
            }

            if (numDelibera != null && !string.IsNullOrEmpty(numDelibera.VALORE_DATABASE) &&
               dtaDelibera != null && string.IsNullOrEmpty(dtaDelibera.VALORE_DATABASE))
            {
                message += "\\nLa -Data Delibera deve essere valorizzata";
            }

            //Se è valorizzata la data di registrazione  non può essere valorizzata la data di restituzione amministrazione e viceversa
            if (((dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)) || (registroFoglio.CONTATORE_DA_FAR_SCATTARE)) && dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE))
            {
                message +=
                    "\\nNon è possibile registrare un decreto restituito oppure restituire un decreto registrato";
            }



            //Aggiungo la visibilità di questo documento ad uno specifico ruolo, il tutto è fatto tramite una stored
            if (!string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber) &&
                registroFoglio != null &&
                registroFoglio.CONTATORE_DA_FAR_SCATTARE &&
                string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)
                )
            {
                DBManagerCDC.setVisiblitaDocumento(schedaDocumentoDaVerificare.docNumber);
            }
            else
            {
                if (string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber) &&
                registroFoglio != null &&
                registroFoglio.CONTATORE_DA_FAR_SCATTARE &&
                string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)
                )
                {
                    message += "\\nLa Registrazione non può essere contestuale alla protocollazione";
                }
            }

            //Se i controlli sono andati tutti a buon fine aggiorno i campi
            if (string.IsNullOrEmpty(message))
                schedaDocumentoDaVerificare.template.ELENCO_OGGETTI = new System.Collections.ArrayList(elencoOggetti);


        }

        private void ControlloTipologiaDocControlloPreventivoSRC(ref DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare, ref string message, DocsPaVO.utente.InfoUtente infoUtente)
        {
            OggettoCustom[] elencoOggetti = (OggettoCustom[])schedaDocumentoDaVerificare.template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom));

            OggettoCustom dtaScadenzaControllo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SCADENZA CONTROLLO")).FirstOrDefault();
            OggettoCustom dtaScadenzaAmministrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SCADENZA AMMINISTRAZIONE")).FirstOrDefault();
            OggettoCustom stato = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("STATO")).FirstOrDefault();
            OggettoCustom registroFoglio = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("REGISTRO-FOGLIO")).FirstOrDefault();
            OggettoCustom dtaRestAmministrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RESTITUZ. AMMINISTRAZIONE")).FirstOrDefault();
            OggettoCustom dtaInvioDeferimento = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA INVIO DEFERIMENTO")).FirstOrDefault();
            OggettoCustom dtaRitornoSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SEZIONE")).FirstOrDefault();
            OggettoCustom numeroOsservazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO OSSERVAZIONE")).FirstOrDefault();
            OggettoCustom dtaOsservazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA OSSERVAZIONE")).FirstOrDefault();
            OggettoCustom dtaRichiestaRitiro = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RICHIESTA RITIRO")).FirstOrDefault();
            OggettoCustom dtaDelibera = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DELIBERA SEZIONE")).FirstOrDefault();
            OggettoCustom esitoAdunanza = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ESITO ADUNANZA")).FirstOrDefault();
            OggettoCustom dtaDecreto = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DECRETO")).FirstOrDefault();
            OggettoCustom dtaRitornoPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRitornoSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom trasmessoPerCompetenza = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TRASMESSO PER COMPETENZA")).FirstOrDefault();
            OggettoCustom dtaRegistrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA REGISTRAZIONE")).FirstOrDefault();
            OggettoCustom numDelibera = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. DELIBERA")).FirstOrDefault();

            OggettoCustom numPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom numSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM. SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom numRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUM RILIEVO")).FirstOrDefault();

            OggettoCustom dtaPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RILIEVO")).FirstOrDefault();

            OggettoCustom elencoTrasmPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom elencoTrasmSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom numElencoRilievi = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO ELENCO RILIEVI")).FirstOrDefault();

            OggettoCustom dtaElencoTrasmPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaElencoTrasmSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaElencoRilievi = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO RILIEVI")).FirstOrDefault();

            OggettoCustom elencoTrasmVersoRagioneria = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("ELENCO TRASM. VERSO RAGIONERIA")).FirstOrDefault();
            OggettoCustom dtaTrasmVersoRagioneria = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA TRASM. VERSO RAGIONERIA")).FirstOrDefault();
            OggettoCustom numElencoTrasm = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO ELENCO TRASM")).FirstOrDefault();
            OggettoCustom dtaElencoTrasmissione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA ELENCO TRASMISSIONE")).FirstOrDefault();

            //Reset di campi calcolati
            if (numElencoTrasm != null)
                numElencoTrasm.VALORE_DATABASE = string.Empty;
            if (dtaElencoTrasmissione != null)
                dtaElencoTrasmissione.VALORE_DATABASE = string.Empty;

            //Se la data di arrivo non è valorizzata, viene impostata con la data di creazione
            if (string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
            {
                if (schedaDocumentoDaVerificare.protocollo != null && schedaDocumentoDaVerificare.protocollo.dataProtocollazione != null)
                {
                    ((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo = schedaDocumentoDaVerificare.protocollo.dataProtocollazione;
                }
                else
                {
                    ((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo = System.DateTime.Now.ToString("dd/MM/yyyy");
                }
            }

            //Data primo rilievo < data arrivo + 60 
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo).AddDays(60);
                if (dataPrimoRilievo > dataArrivo)
                    message += "\\nLa -Data Primo Rilievo- risulta successiva alla scadenza dei termini previsti per il controllo";
            }

            //Data primo rilievo >= data protocollo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                if (dataPrimoRilievo < dataProtocollo)
                    message += "\\nLa -Data Primo Rilievo- risulta minore della data di protocollo";
            }

            //CONTROLLO DATA SCADENZA CONTROLLO - Senza Rilievo
            if (dtaScadenzaControllo != null)
            {
                if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                {
                    if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60).ToShortDateString();
                    else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                        dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60).ToShortDateString();
                }
            }

            //CONTROLLO DATA SCADENZA CONTROLLO - Con Rilievo senza rispota a riliveo
            if (dtaScadenzaControllo != null && dtaPrimoRilievo != null && dtaRitornoPrimoRilievo != null)
            {
                if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                {
                    if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(90).ToShortDateString();
                        else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                            dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(90).ToShortDateString();
                    }
                }
            }

            //CONTROLLO DATA SCADENZA CONTROLLO - Con Rilievo e rispota a riliveo
            if (dtaScadenzaControllo != null && dtaPrimoRilievo != null && dtaRitornoPrimoRilievo != null)
            {
                if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                {
                    DateTime datePrimoRilevo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                    DateTime dateRitornoPrimoRilevo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
                    int diffDate = dateRitornoPrimoRilevo.Subtract(datePrimoRilevo).Days;

                    if (diffDate >= 30)
                    {
                        if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                        {
                            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                                    dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(90).ToShortDateString();
                                else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                                    dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(90).ToShortDateString();
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE))
                        {
                            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo))
                                    dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime((((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)).AddDays(60 + diffDate).ToShortDateString();
                                else if (!string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione))
                                    dtaScadenzaControllo.VALORE_DATABASE = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione).AddDays(60 + diffDate).ToShortDateString();
                            }
                        }

                    }
                }
            }

            //CONTROLLO DATA SCADENZA AMMINISTRAZIONE
            if (dtaScadenzaAmministrazione != null && dtaPrimoRilievo != null)
            {
                if (!string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE))
                {
                    dtaScadenzaAmministrazione.VALORE_DATABASE = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE).AddDays(30).ToShortDateString();
                }
            }

            //CONTROLLO STATO
            //"In esame"
            if (registroFoglio != null && string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) &&
                //dtaRitornoPrimoRilievo != null && string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
                 dtaRestAmministrazione != null && string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                //dtaInvioSezione != null && string.IsNullOrEmpty(dtaInvioSezione.VALORE_DATABASE) &&
                //dtaRitornoSezione != null && string.IsNullOrEmpty(dtaRitornoSezione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "In esame";
            }

            //"Registrato senza rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaOsservazione != null && string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato senza rilievo";
            }

            //"Registrato a seguito di rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo";
            }

            //"Registrato a seguito di rilievo a vuoto"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaOsservazione != null && !string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo a vuoto";
            }

            //"Annullato"
            if (schedaDocumentoDaVerificare.protocollo.protocolloAnnullato != null)
            {
                stato.VALORE_DATABASE = "Annullato";
            }

            //"Restituito all'amministrazione"
            if (dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE))
            {
                stato.VALORE_DATABASE = "Restituito all'amministrazione";
            }

            //"Ammesso al visto della sezione"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaInvioDeferimento != null && !string.IsNullOrEmpty(dtaInvioDeferimento.VALORE_DATABASE) &&
                dtaDelibera != null && !string.IsNullOrEmpty(dtaDelibera.VALORE_DATABASE) &&
                dtaRitornoSezione != null && !string.IsNullOrEmpty(dtaRitornoSezione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Ammesso al visto della sezione";
            }

            ////"Trasmesso per competenza"
            //if (trasmessoPerCompetenza != null && trasmessoPerCompetenza.VALORE_DATABASE.ToUpper().Equals("SI"))
            //{
            //    stato.VALORE_DATABASE = "Trasmesso per competenza";
            //}

            //"Non luogo a deliberare deciso dalla sezione"
            if (esitoAdunanza != null && esitoAdunanza.VALORE_DATABASE.ToUpper().Equals("NON LUOGO A DELIBERARE"))
            {
                stato.VALORE_DATABASE = "Non luogo a deliberare deciso dalla sezione";
            }

            //"Ricusazione del visto dalla sezione"
            if (esitoAdunanza != null && esitoAdunanza.VALORE_DATABASE.ToUpper().Equals("RICUSAZIONE VISTO"))
            {
                stato.VALORE_DATABASE = "Ricusazione del visto della sezione";
            }

            //CONTROLLO DATE
            //"Data decreto" <= "Data arrivo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                if (dataDecreto > dataArrivo)
                    message += "\\nLa -Data decreto- è maggiore della -Data arrivo-";
            }

            //"Data decreto" <= "Data protocollo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                if (dataDecreto > dataProtocollo)
                    message += "\\nLa -Data decreto- è maggiore della -Data protocollo-";
            }

            //"Data arrivo" <= "Data primo rilievo"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                if (dataArrivo > dataRilievo)
                    message += "\\nLa -Data arrivo- è maggiore della -Data primo rilievo-";
            }

            //"Data primo rilievo" <= "Data ritorno primo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
               dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataRitornoRilievo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
                if (dataRilievo > dataRitornoRilievo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data ritorno primo Rilievo-";
            }

            //"Data secondo rilievo" <= "Data ritorno secondo rilievo"
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataSecondoRilievo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                DateTime dataRitornoSecondoRilievo = Convert.ToDateTime(dtaRitornoSecondoRilievo.VALORE_DATABASE);
                if (dataSecondoRilievo > dataRitornoSecondoRilievo)
                    message += "\\nLa -Data secondo rilievo- è maggiore della -Data ritorno secondo rilievo-";
            }

            //"Data decreto" <= "Data arrivo"
            //Già fatto sopra

            //"Data arrivo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataArrivo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data arrivo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data protocollo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione) &&
                dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataProtocollo = Convert.ToDateTime(((DocsPaVO.documento.ProtocolloEntrata)schedaDocumentoDaVerificare.protocollo).dataProtocollazione);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaRestAmministrazione.VALORE_DATABASE);
                if (dataProtocollo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data protocollo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data ritorno primo rilievo" (valorizzata), "Data primo rilievo" (valorizzata)
            if (dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno primo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //"Data ritorno secondo rilievo" (valorizzata), "Data secondo rilievo" (valorizzata)
            if (dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno secondo rilievo- è valorizzata ma la -Data secondo rilievo- no";
            }

            //"Data primo rilievo" <= "Data secondo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataSecondo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataSecondo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data secondo Rilievo-";
            }

            //"Data secondo rilievo" (Valorizzata), "Data primo rilievo" (non valorizzata)
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data secondo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //Data Registrazione (non valorizzata) assume il valore della data inserimento del campo "Registro-Foglio"
            if (dtaRegistrazione != null && string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO))
                )
            {
                dtaRegistrazione.VALORE_DATABASE = System.DateTime.Now.ToString("dd/MM/yyyy");
            }

            //Data Registrazione - da subordinare alla effettiva modifica della data e ai diritti sul campo
            if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) && !string.IsNullOrEmpty(schedaDocumentoDaVerificare.docNumber))
            {
                DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = DBManagerCDC.getDirittiCampo(infoUtente.idGruppo, schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString());
                if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1")
                {
                    string dataRegistrazioneDB = DBManagerCDC.getValoreCampoDB(dtaRegistrazione, schedaDocumentoDaVerificare.docNumber);
                    if (dataRegistrazioneDB != null && dataRegistrazioneDB != dtaRegistrazione.VALORE_DATABASE)
                    {
                        //Data registrazione (valorizzata) <= della data attuale
                        if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE))
                        {
                            DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                            DateTime dataAttuale = Convert.ToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));
                            if (dataRegistrazione > dataAttuale)
                                message += "\\nLa -Data Registrazione- è maggiore della data attuale";
                        }

                        //Data registrazione (valorizzata) >= della maggiore data registrazione per l'RF in questione
                        if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                            registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                            !string.IsNullOrEmpty(registroFoglio.ID_AOO_RF)
                            )
                        {
                            DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                            DateTime maxDataRegistrazione = DBManagerCDC.getMaxDataRegistrazione(schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString(), registroFoglio.ID_AOO_RF);
                            if (dataRegistrazione < maxDataRegistrazione)
                                message += "\\nLa -Data Registrazione- è minore della maggiore Data Registrazione esistente per questo RF";
                        }
                    }
                }
            }

            //Controllo per evitare che siano valorizzate le date registrazione e trasmissione verso la ragioneria
            //prima dell'effettiva registrazione del decreto
            if (registroFoglio != null &&
                (string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) && !registroFoglio.CONTATORE_DA_FAR_SCATTARE) &&
                    (
                    (dtaRegistrazione != null && !String.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)) ||
                    (elencoTrasmVersoRagioneria != null && !String.IsNullOrEmpty(elencoTrasmVersoRagioneria.VALORE_DATABASE)) ||
                    (dtaTrasmVersoRagioneria != null && !String.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE))
                    )
                )
            {
                message += "\\nRisultano valorizzati valorizzati i seguenti dati : \\n-DATA REGISTRAZIONE \\n-ELENCO TRASM VERSO RAGIONERIA \\n-DATA TRASM VERSO RAGIONERIA \\nma non ci sono gli estremi di registrazione";
            }

            //Num Rilievo
            if (numPrimoRilievo != null && !string.IsNullOrEmpty(numPrimoRilievo.VALORE_DATABASE) && numRilievo != null)
                numRilievo.VALORE_DATABASE = numPrimoRilievo.VALORE_DATABASE;

            if (numSecondoRilievo != null && !string.IsNullOrEmpty(numSecondoRilievo.VALORE_DATABASE) && numRilievo != null)
                numRilievo.VALORE_DATABASE = numSecondoRilievo.VALORE_DATABASE;

            //Data Rilievo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && dtaRilievo != null)
                dtaRilievo.VALORE_DATABASE = dtaPrimoRilievo.VALORE_DATABASE;

            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) && dtaRilievo != null)
                dtaRilievo.VALORE_DATABASE = dtaSecondoRilievo.VALORE_DATABASE;

            //Numero Elenco Rilievi
            if (elencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(elencoTrasmPrimoRilievo.VALORE_DATABASE) && numElencoRilievi != null)
                numElencoRilievi.VALORE_DATABASE = elencoTrasmPrimoRilievo.VALORE_DATABASE;

            if (elencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(elencoTrasmSecondoRilievo.VALORE_DATABASE) && numElencoRilievi != null)
                numElencoRilievi.VALORE_DATABASE = elencoTrasmSecondoRilievo.VALORE_DATABASE;

            //Data Elenco Rilievi
            if (dtaElencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(dtaElencoTrasmPrimoRilievo.VALORE_DATABASE) && dtaElencoRilievi != null)
                dtaElencoRilievi.VALORE_DATABASE = dtaElencoTrasmPrimoRilievo.VALORE_DATABASE;

            if (dtaElencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(dtaElencoTrasmSecondoRilievo.VALORE_DATABASE) && dtaElencoRilievi != null)
                dtaElencoRilievi.VALORE_DATABASE = dtaElencoTrasmSecondoRilievo.VALORE_DATABASE;

            //Data rilievo valorizzata e data trasm verso ragioneria non valorizzata
            if (dtaRilievo != null && !string.IsNullOrEmpty(dtaRilievo.VALORE_DATABASE) &&
                dtaTrasmVersoRagioneria != null && string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE) &&
                numElencoRilievi != null &&
                dtaElencoTrasmissione != null &&
                numElencoTrasm != null
                )
            {
                numElencoTrasm.VALORE_DATABASE = numElencoRilievi.VALORE_DATABASE;
                dtaElencoTrasmissione.VALORE_DATABASE = dtaRilievo.VALORE_DATABASE;
            }

            //Data rilievo non valorizzata e data trasm verso ragioneria valorizzata
            if (dtaRilievo != null && string.IsNullOrEmpty(dtaRilievo.VALORE_DATABASE) &&
                dtaTrasmVersoRagioneria != null && !string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE) &&
                elencoTrasmVersoRagioneria != null &&
                dtaElencoTrasmissione != null &&
                numElencoTrasm != null
                )
            {
                numElencoTrasm.VALORE_DATABASE = elencoTrasmVersoRagioneria.VALORE_DATABASE;
                dtaElencoTrasmissione.VALORE_DATABASE = dtaTrasmVersoRagioneria.VALORE_DATABASE;
            }

            //Data rilievo valorizzata e data trasm verso ragioneria valorizzata
            if (dtaRilievo != null && !string.IsNullOrEmpty(dtaRilievo.VALORE_DATABASE) &&
                dtaTrasmVersoRagioneria != null && !string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE) &&
                dtaElencoTrasmissione != null &&
                numElencoTrasm != null &&
                elencoTrasmVersoRagioneria != null &&
                numElencoRilievi != null
            )
            {
                DateTime dataRilievo = Convert.ToDateTime(dtaRilievo.VALORE_DATABASE);
                DateTime dataTrasmVersoRagioneria = Convert.ToDateTime(dtaTrasmVersoRagioneria.VALORE_DATABASE);
                if (dataRilievo > dataTrasmVersoRagioneria)
                {
                    numElencoTrasm.VALORE_DATABASE = numElencoRilievi.VALORE_DATABASE;
                    dtaElencoTrasmissione.VALORE_DATABASE = dtaRilievo.VALORE_DATABASE;
                }
                else if (dataRilievo <= dataTrasmVersoRagioneria)
                {
                    numElencoTrasm.VALORE_DATABASE = elencoTrasmVersoRagioneria.VALORE_DATABASE;
                    dtaElencoTrasmissione.VALORE_DATABASE = dtaTrasmVersoRagioneria.VALORE_DATABASE;
                }
            }

            //Controllo Valorizzazione Campi
            if (numPrimoRilievo != null && !string.IsNullOrEmpty(numPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Primo Rilievo deve essere valorizzata";
            }

            if (numSecondoRilievo != null && !string.IsNullOrEmpty(numSecondoRilievo.VALORE_DATABASE) &&
               dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Secondo Rilievo deve essere valorizzata";
            }

            if (numeroOsservazione != null && !string.IsNullOrEmpty(numeroOsservazione.VALORE_DATABASE) &&
               dtaOsservazione != null && string.IsNullOrEmpty(dtaOsservazione.VALORE_DATABASE))
            {
                message += "\\nLa -Data Osservazione deve essere valorizzata";
            }

            if (elencoTrasmPrimoRilievo != null && !string.IsNullOrEmpty(elencoTrasmPrimoRilievo.VALORE_DATABASE) &&
               dtaElencoTrasmPrimoRilievo != null && string.IsNullOrEmpty(dtaElencoTrasmPrimoRilievo.VALORE_DATABASE))
            {
                message += "\\nLa -Data Elenco Trasm. Primo rilievo deve essere valorizzata";
            }

            if (elencoTrasmSecondoRilievo != null && !string.IsNullOrEmpty(elencoTrasmSecondoRilievo.VALORE_DATABASE) &&
                dtaElencoTrasmSecondoRilievo != null && string.IsNullOrEmpty(dtaElencoTrasmSecondoRilievo.VALORE_DATABASE))
            {
                message += "\\nLa -Data Elenco Trasm. Secondo rilievo deve essere valorizzata";
            }

            if (elencoTrasmVersoRagioneria != null && !string.IsNullOrEmpty(elencoTrasmVersoRagioneria.VALORE_DATABASE) &&
               dtaTrasmVersoRagioneria != null && string.IsNullOrEmpty(dtaTrasmVersoRagioneria.VALORE_DATABASE))
            {
                message += "\\nLa -Data Trasm. Verso Ragioneria deve essere valorizzata";
            }

            if (numDelibera != null && !string.IsNullOrEmpty(numDelibera.VALORE_DATABASE) &&
               dtaDelibera != null && string.IsNullOrEmpty(dtaDelibera.VALORE_DATABASE))
            {
                message += "\\nLa -Data Delibera deve essere valorizzata";
            }

            //Se è valorizzata la data di registrazione  non può essere valorizzata la data di restituzione amministrazione e viceversa
            if (((dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE)) || (registroFoglio.CONTATORE_DA_FAR_SCATTARE)) && dtaRestAmministrazione != null && !string.IsNullOrEmpty(dtaRestAmministrazione.VALORE_DATABASE))
            {
                message +=
                    "\\nNon è possibile registrare un decreto restituito oppure restituire un decreto registrato";
            }

            //La data di primo rilievo non può essere maggiore della data di scadenza controllo
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) && dtaScadenzaControllo != null && !string.IsNullOrEmpty(dtaScadenzaControllo.VALORE_DATABASE))
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataScadenzaControllo = Convert.ToDateTime(dtaScadenzaControllo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataScadenzaControllo)
                    message += "\\nLa -Data Primo Rilievo- non può essere maggiore della data di scadenza controllo";

            }

            //Se i controlli sono andati tutti a buon fine aggiorno i campi
            if (string.IsNullOrEmpty(message))
                schedaDocumentoDaVerificare.template.ELENCO_OGGETTI = new System.Collections.ArrayList(elencoOggetti);
        }

        private void ControlloTipologiaDocPensioni(ref DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare, ref string message)
        {
            OggettoCustom[] elencoOggetti = (OggettoCustom[])schedaDocumentoDaVerificare.template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom));

            OggettoCustom stato = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("STATO")).FirstOrDefault();
            OggettoCustom registroFoglio = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("REGISTRO-FOGLIO")).FirstOrDefault();
            OggettoCustom dtaRilievoAVuoto = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RILIEVO A VUOTO")).FirstOrDefault();
            OggettoCustom dtaRichiestaRitiro = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RICHIESTA RITIRO")).FirstOrDefault();
            OggettoCustom dtaProtoRestAmministrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA PROTO RESTITUZ. AMMINISTRAZIONE")).FirstOrDefault();
            OggettoCustom dtaDecreto = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DECRETO")).FirstOrDefault();
            OggettoCustom dtaPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRitornoPrimoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO PRIMO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRitornoSecondoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SECONDO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaTerzoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA TERZO RILIEVO")).FirstOrDefault();
            OggettoCustom dtaRitornoTerzoRilievo = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO TERZO RILIEVO")).FirstOrDefault();
            OggettoCustom decretoAnnullato = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DECRETO ANNULLATO")).FirstOrDefault();
            OggettoCustom dtaInvioSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA INVIO SEZIONE")).FirstOrDefault();
            OggettoCustom dtaRitornoSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA RITORNO SEZIONE")).FirstOrDefault();
            OggettoCustom dtaDeliberaSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA DELIBERA SEZIONE")).FirstOrDefault();
            OggettoCustom trasmessoPerCompetenza = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("TRASMESSO PER COMPETENZA")).FirstOrDefault();
            OggettoCustom rispostaSezione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("RISPOSTA SEZIONE")).FirstOrDefault();
            OggettoCustom numeroDecretoRiproposto = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("NUMERO DECRETO RIPROPOSTO")).FirstOrDefault();
            OggettoCustom dtaRegistrazione = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DATA REGISTRAZIONE")).FirstOrDefault();

            //CONTROLLO STATO
            //"In esame"
            if (registroFoglio != null && string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO) &&
                dtaProtoRestAmministrazione != null && string.IsNullOrEmpty(dtaProtoRestAmministrazione.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "In esame";
            }

            //"Registrato senza rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato senza rilievo";
            }

            //"Registrato a seguito di rilievo"
            if (registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Registrato a seguito di rilievo";
            }

            //"Registrato a seguito di rilievo a vuoto"
            if (dtaRilievoAVuoto != null && !string.IsNullOrEmpty(dtaRilievoAVuoto.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO))
                )
            {
                stato.VALORE_DATABASE = "Registrato senza rilievo";
            }

            //"Annullato"
            if (schedaDocumentoDaVerificare.protocollo.protocolloAnnullato != null)
            {
                stato.VALORE_DATABASE = "Annullato";
            }

            //"Restituito all'amministrazione"
            if (dtaProtoRestAmministrazione != null && !string.IsNullOrEmpty(dtaProtoRestAmministrazione.VALORE_DATABASE) &&
                dtaRichiestaRitiro != null && string.IsNullOrEmpty(dtaRichiestaRitiro.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Restituito all'Amm.";
            }

            //"Ritirato"
            if (dtaProtoRestAmministrazione != null && !string.IsNullOrEmpty(dtaProtoRestAmministrazione.VALORE_DATABASE) &&
                dtaRichiestaRitiro != null && !string.IsNullOrEmpty(dtaRichiestaRitiro.VALORE_DATABASE)
                )
            {
                stato.VALORE_DATABASE = "Ritirato";
            }

            //"Ammesso al visto della sezione"
            if (dtaInvioSezione != null && !string.IsNullOrEmpty(dtaInvioSezione.VALORE_DATABASE) &&
                dtaDeliberaSezione != null && !string.IsNullOrEmpty(dtaDeliberaSezione.VALORE_DATABASE) &&
                dtaRitornoSezione != null && !string.IsNullOrEmpty(dtaRitornoSezione.VALORE_DATABASE) &&
                rispostaSezione != null && rispostaSezione.VALORE_DATABASE.ToUpper().Equals("AMMESSO ALLA REGISTRAZIONE")
                )
            {
                stato.VALORE_DATABASE = "Ammesso al visto della sezione";
            }

            //"Trasmesso per competenza"
            if (trasmessoPerCompetenza != null && trasmessoPerCompetenza.VALORE_DATABASE.ToUpper().Equals("SI"))
            {
                stato.VALORE_DATABASE = "Trasmesso per competenza";
            }

            //"Non luogo a deliberare deciso dalla sezione"
            if (rispostaSezione != null && rispostaSezione.VALORE_DATABASE.ToUpper().Equals("NON LUOGO A DELIBERARE"))
            {
                stato.VALORE_DATABASE = "Non luogo a deliberare deciso dalla sezione";
            }

            //"Ricusazione del visto dalla sezione"
            if (rispostaSezione != null && rispostaSezione.VALORE_DATABASE.ToUpper().Equals("RICUSAZIONE VISTO"))
            {
                stato.VALORE_DATABASE = "Ricusazione del visto della sezione";
            }

            //"Non ancora riproposto dopo rilievo"
            if ((
                (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)) ||
                (dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)) ||
                (dtaTerzoRilievo != null && string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE))
                ) &&
                (numeroDecretoRiproposto != null && string.IsNullOrEmpty(numeroDecretoRiproposto.VALORE_DATABASE))
                )
            {
                stato.VALORE_DATABASE = "Nono ancora riproposto dopo rilievo";
            }

            //"Annullato e sostituito dall'amministrazione
            if (numeroDecretoRiproposto != null && !string.IsNullOrEmpty(numeroDecretoRiproposto.VALORE_DATABASE))
            {
                stato.VALORE_DATABASE = "Annullato e sostituito dall'amministrazione";
            }

            //CONTROLLO DATE
            //"Data decreto" <= "Data arrivo"
            if (dtaDecreto != null && !string.IsNullOrEmpty(dtaDecreto.VALORE_DATABASE) &&
               schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo)
                )
            {
                DateTime dataDecreto = Convert.ToDateTime(dtaDecreto.VALORE_DATABASE);
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                if (dataDecreto > dataArrivo)
                    message += "\\nLa -Data decreto- è maggiore della -Data arrivo-";
            }

            //"Data arrivo" <= "Data primo rilievo"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                if (dataArrivo > dataPrimoRilievo)
                    message += "\\nLa -Data arrivo- è maggiore della -Data primo rilievo-";
            }

            //"Data primo rilievo" <= "Data ritorno primo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataRitornoRilievo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataRitornoRilievo)
                    message += "\\nLa -Data primo rilievo- è maggiore della -Data ritorno primo rilievo-";
            }

            ////"Data ritorno primo rilievo" <= "Data secondo rilievo"
            //if (dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
            //    dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
            //    )
            //{
            //    DateTime dataRitornoRilievo = Convert.ToDateTime(dtaRitornoPrimoRilievo.VALORE_DATABASE);
            //    DateTime dataSecondoRilievo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
            //    if (dataRitornoRilievo > dataSecondoRilievo)
            //        message += "\\nLa -Data ritorno primo rilievo- è maggiore della -Data secondo rilievo-";
            //}

            ////"Data secondo rilievo" <= "Data ritorno secondo rilievo"
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataSecondoRilievo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                DateTime dataRitornoSecondoRilievo = Convert.ToDateTime(dtaRitornoSecondoRilievo.VALORE_DATABASE);
                if (dataSecondoRilievo > dataRitornoSecondoRilievo)
                    message += "\\nLa -Data secondo rilievo- è maggiore della -Data ritorno secondo rilievo-";
            }

            //////"Data ritorno secondo rilievo" <= "Data terzo rilievo"
            //if (dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE) &&
            //    dtaTerzoRilievo != null && !string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE)
            //    )
            //{
            //    DateTime dataRitornoSecondoRilievo = Convert.ToDateTime(dtaRitornoSecondoRilievo.VALORE_DATABASE);
            //    DateTime dataTerzoRilievo = Convert.ToDateTime(dtaTerzoRilievo.VALORE_DATABASE);
            //    if (dataRitornoSecondoRilievo > dataTerzoRilievo)
            //        message += "\\nLa -Data ritorno secondo rilievo- è maggiore della -Data terzo rilievo-";
            //}

            ////"Data terzo rilievo" <= "Data ritorno terzo rilievo"
            if (dtaTerzoRilievo != null && !string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE) &&
                dtaRitornoTerzoRilievo != null && !string.IsNullOrEmpty(dtaRitornoTerzoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataTerzoRilievo = Convert.ToDateTime(dtaTerzoRilievo.VALORE_DATABASE);
                DateTime dataRitornoTerzoRilievo = Convert.ToDateTime(dtaRitornoTerzoRilievo.VALORE_DATABASE);
                if (dataTerzoRilievo > dataRitornoTerzoRilievo)
                    message += "\\nLa -Data terzo rilievo- è maggiore della -Data ritorno terzo rilievo-";
            }

            //"Data decreto" <= "Data arrivo"
            //Già fatto sopra

            //"Data arrivo" <= "Data restituzione amministrazione"
            if (schedaDocumentoDaVerificare.documenti != null && schedaDocumentoDaVerificare.documenti.Count > 0 &&
               !string.IsNullOrEmpty(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo) &&
                dtaProtoRestAmministrazione != null && !string.IsNullOrEmpty(dtaProtoRestAmministrazione.VALORE_DATABASE)
                )
            {
                DateTime dataArrivo = Convert.ToDateTime(((DocsPaVO.documento.Documento)schedaDocumentoDaVerificare.documenti[0]).dataArrivo);
                DateTime dataRestituzioneAmministrazione = Convert.ToDateTime(dtaProtoRestAmministrazione.VALORE_DATABASE);
                if (dataArrivo > dataRestituzioneAmministrazione)
                    message += "\\nLa -Data arrivo- è maggiore della -Data restituzione amministrazione-";
            }

            //"Data ritorno primo rilievo" (valorizzata), "Data primo rilievo" (valorizzata)
            if (dtaRitornoPrimoRilievo != null && !string.IsNullOrEmpty(dtaRitornoPrimoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno primo rilievo- è valorizzata, ma la -Data primo rilievo- no";
            }

            //"Data ritorno secondo rilievo" (valorizzata), "Data secondo rilievo" (valorizzata)
            if (dtaRitornoSecondoRilievo != null && !string.IsNullOrEmpty(dtaRitornoSecondoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data Ritorno secondo rilievo- è valorizzata ma la -Data secondo rilievo- no";
            }

            //"Data ritorno terzo rilievo" (valorizzata), "Data terzo rilievo" (valorizzata)
            if (dtaRitornoTerzoRilievo != null && !string.IsNullOrEmpty(dtaRitornoTerzoRilievo.VALORE_DATABASE) &&
                dtaTerzoRilievo != null && string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data ritorno terzo rilievo- è valorizzata ma la -Data terzo rilievo- no";
            }

            //"Data primo rilievo" <= "Data secondo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataSecondo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataSecondo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data secondo Rilievo-";
            }

            //"Data secondo rilievo" <= "Data terzo rilievo"
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaTerzoRilievo != null && !string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataSecondoRilievo = Convert.ToDateTime(dtaSecondoRilievo.VALORE_DATABASE);
                DateTime dataTerzoRilievo = Convert.ToDateTime(dtaTerzoRilievo.VALORE_DATABASE);
                if (dataSecondoRilievo > dataTerzoRilievo)
                    message += "\\nLa -Data secondo rilievo- è maggione della -Data terzo Rilievo-";
            }

            //"Data primo rilievo" <= "Data terzo rilievo"
            if (dtaPrimoRilievo != null && !string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE) &&
                dtaTerzoRilievo != null && !string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE)
                )
            {
                DateTime dataPrimoRilievo = Convert.ToDateTime(dtaPrimoRilievo.VALORE_DATABASE);
                DateTime dataTerzoRilievo = Convert.ToDateTime(dtaTerzoRilievo.VALORE_DATABASE);
                if (dataPrimoRilievo > dataTerzoRilievo)
                    message += "\\nLa -Data primo rilievo- è maggione della -Data terzo Rilievo-";
            }

            //"Data secondo rilievo" (Valorizzata), "Data primo rilievo" (non valorizzata)
            if (dtaSecondoRilievo != null && !string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data secondo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //"Data terzo rilievo" (valorizzata), "Data secondo rilievo" (non valorizzata)
            if (dtaTerzoRilievo != null && !string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE) &&
                dtaSecondoRilievo != null && string.IsNullOrEmpty(dtaSecondoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data terzo rilievo- è valorizzata ma la -Data secondo rilievo- no";
            }

            //"Data terzo rilievo" (valorizzata), "Data primo rilievo" (non valorizzata)
            if (dtaTerzoRilievo != null && !string.IsNullOrEmpty(dtaTerzoRilievo.VALORE_DATABASE) &&
                dtaPrimoRilievo != null && string.IsNullOrEmpty(dtaPrimoRilievo.VALORE_DATABASE)
                )
            {
                message += "\\nLa -Data terzo rilievo- è valorizzata ma la -Data primo rilievo- no";
            }

            //EVENTUALMENTE DA FARE
            //"Numero secondo rilievo" (Valorizzata), "Numero primo rilievo" (non valorizzata)

            //"Numero terzo rilievo" (valorizzata), "Numenro secondo rilievo" (non valorizzata)

            //"Numnero terzo rilievo" (valorizzata), "Numero primo rilievo" (non valorizzata)

            //Data Registrazione (non valorizzata) assume il valore della data inserimento del campo "Registro-Foglio"
            if (dtaRegistrazione != null && string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO))
                )
            {
                dtaRegistrazione.VALORE_DATABASE = System.DateTime.Now.ToString("dd/MM/yyyy");
            }

            //Data registrazione (valorizzata) <= della data attuale
            if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE))
            {
                DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                DateTime dataAttuale = Convert.ToDateTime(System.DateTime.Now.ToString("dd/MM/yyyy"));
                if (dataRegistrazione > dataAttuale)
                    message += "\\nLa -Data Registrazione- è maggiore della data attuale";
            }

            //Data registrazione (valorizzata) >= della maggiora data registrazione per l'RF in questione
            if (dtaRegistrazione != null && !string.IsNullOrEmpty(dtaRegistrazione.VALORE_DATABASE) &&
                registroFoglio != null && (registroFoglio.CONTATORE_DA_FAR_SCATTARE || !string.IsNullOrEmpty(registroFoglio.DATA_INSERIMENTO)) &&
                !string.IsNullOrEmpty(registroFoglio.ID_AOO_RF)
                )
            {
                DateTime dataRegistrazione = Convert.ToDateTime(dtaRegistrazione.VALORE_DATABASE);
                DateTime maxDataRegistrazione = DBManagerCDC.getMaxDataRegistrazione(schedaDocumentoDaVerificare.template.SYSTEM_ID.ToString(), dtaRegistrazione.SYSTEM_ID.ToString(), registroFoglio.ID_AOO_RF);
                if (dataRegistrazione < maxDataRegistrazione)
                    message += "\\nLa -Data Registrazione- è minore della maggiore Data Registrazione esistente per questo RF";
            }

            //Se i controlli sono andati tutti a buon fine aggiorno i campi
            if (string.IsNullOrEmpty(message))
                schedaDocumentoDaVerificare.template.ELENCO_OGGETTI = new System.Collections.ArrayList(elencoOggetti);
        }

        private void ControlloPareriSCCLA(ref DocsPaVO.documento.SchedaDocumento schedaDocumentoDaVerificare, ref string message, DocsPaVO.utente.InfoUtente infoUtente)
        {
            OggettoCustom[] elencoOggetti = (OggettoCustom[])schedaDocumentoDaVerificare.template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom));

            OggettoCustom Decreti = elencoOggetti.Where(oggetto => oggetto.DESCRIZIONE.ToUpper().Equals("DECRETI ESAMINATI")).FirstOrDefault();

            if (Decreti != null && !String.IsNullOrEmpty(Decreti.VALORE_DATABASE) && !IsNumber(Decreti.VALORE_DATABASE))
            {
                message += "\\n Il campo Decreti Esaminati ammette solo valori numerici validi";
            }
        }

        private bool IsNumber(string text)
        {
            bool result = true;
            if (text.IndexOf('0').Equals(0))
            {
                result = false;
            }
            else
            {
                Regex objNotNaturalPattern = new Regex("[^0-9]");
                Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
                result = !objNotNaturalPattern.IsMatch(text) && objNaturalPattern.IsMatch(text);
            }

            return result;
        }

        private void VerifyDate(OggettoCustom[] elencoOggetti, ref string message)
        {
            OggettoCustom[] date = elencoOggetti.Where(oggetto => oggetto.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("DATA")).ToArray<OggettoCustom>();

            foreach (OggettoCustom data in date)
            {
                try
                {
                    if (!string.IsNullOrEmpty(data.VALORE_DATABASE))
                        Convert.ToDateTime(data.VALORE_DATABASE);
                }
                catch (Exception ex)
                {
                    message += "\\nLa data " + data.DESCRIZIONE + " non è valida";
                }
            }
        }
    }
}
