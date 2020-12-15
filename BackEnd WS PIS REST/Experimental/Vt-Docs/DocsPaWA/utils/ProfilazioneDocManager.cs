using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using System.Data;
using DocsPaIntegration.Search;
using System.Linq;
using System.Collections.Generic;

namespace DocsPAWA
{
    public class ProfilazioneDocManager
    {
        private static DocsPaWebService docsPaWS = ProxyManager.getWS();

        public static ArrayList getTipologiaAttoProfDin(string idAmministrazione, Page page)
        {
            try
            {
                return new ArrayList(docsPaWS.GetTipologiaAttoProfDin(idAmministrazione));
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getIdModelliTrasmAssociati(idTipoDoc, idDiagramma, idStato));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static OggettoCustom getOggettoById(string idOggetto, Page page)
        {
            try
            {
                return docsPaWS.getOggettoById(idOggetto);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaAssociazioneModelli(string idTipoDoc, string idDiagramma, Object[] modelliSelezionati, string idStato, Page page)
        {
            try
            {
                docsPaWS.salvaAssociazioneModelli(idTipoDoc, idDiagramma, modelliSelezionati, idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList getTipiOggetto(Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTipiOggetto());
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getTipiDocumento(string idAmministrazione, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTipiDocumento(idAmministrazione));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool salvaTemplate(InfoUtente infoUtente, Templates template, string idAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.salvaTemplate(infoUtente,template,idAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }            
        }

        public static Templates eliminaOggettoCustomTemplate(Templates template, int oggettoCustom, Page page)
        {
            try
            {
                return docsPaWS.eliminaOggettoCustomTemplate(template, oggettoCustom);                
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Templates aggiungiOggettoCustomTemplate(OggettoCustom oggettoCustom, Templates template, Page page)
        {
            try
            {
                return docsPaWS.aggiungiOggettoCustomTemplate(oggettoCustom, template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool aggiornaTemplate(Templates template, Page page)
        {
            try
            {
                return docsPaWS.aggiornaTemplate(template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static Templates getTemplateById(string idTemplate, Page page)
        {
            try
            {
                Templates template = docsPaWS.getTemplateById(idTemplate);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                        template = docsPaWS.getTemplateCampiComuniById(UserManager.getInfoUtente(page), idTemplate);
                    }
                    catch (Exception e)
                    {
                        //In questo caso vuol dire che provengo da amministrazione e l'infoUtente non esiste
                        //quindi il template non va filtrato per visibilità
                    }
                }

                if (template != null)
                    return template;
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getTemplates(string idAmministrazione, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTemplates(idAmministrazione));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Templates getTemplateDettagli(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getTemplateDettagli(docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool disabilitaTemplate(Templates template, string idAmministrazione, string codiceAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.disabilitaTemplate(template, idAmministrazione, codiceAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static bool UpdateIsTypeInstance(Templates template, string idAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.UpdateIsTypeInstance(template, idAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static void messaInEserizioTemplate(Templates template, string idAmministrazione, Page page)
        {
            try
            {
                docsPaWS.messaInEserizioTemplate(template, idAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static Templates spostaOggetto(Templates template, int oggettoSelezionato, string spostamento, Page page)
        {
            try
            {
                return docsPaWS.spostaOggetto(template, oggettoSelezionato, spostamento);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Templates getTemplatePerRicerca(string idAmministrazione, string tipoAtto, Page page)
        {
            try
            {
                Templates template = docsPaWS.getTemplatePerRicerca(idAmministrazione, tipoAtto);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                        template = docsPaWS.getTemplateCampiComuniById(UserManager.getInfoUtente(page), template.SYSTEM_ID.ToString());
                    }
                    catch (Exception e)
                    {
                        //In questo caso vuol dire che provengo da amministrazione e l'infoUtente non esiste
                        //quindi il template non va filtrato per visibilità
                    }
                }

                if (template != null)
                    return template;
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static string getIdTemplate(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getIdTemplate(docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isValueInUse(string idOggetto, string idTemplate, string valoreOggettoDB, Page page)
        {
            try
            {
                return docsPaWS.isValueInUse(idOggetto, idTemplate, valoreOggettoDB);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static void salvaModelli(byte[] dati, string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, Templates template, Page page)
        {
            try
            {
                docsPaWS.salvaModelli(dati, nomeProfilo, codiceAmministrazione, nomeFile, estensione, template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void eliminaModelli(string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, Templates template, Page page)
        {
            try
            {
                docsPaWS.eliminaModelli(nomeProfilo, codiceAmministrazione, nomeFile, estensione, template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateScadenzeTipoDoc(int systemId_template, string scadenza, string preScadenza, Page page)
        {
            try
            {
                docsPaWS.updateScadenzeTipoDoc(systemId_template, scadenza, preScadenza);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updatePrivatoTipoDoc(int systemId_template, string privato, Page page)
        {
            try
            {
                docsPaWS.updatePrivatoTipoDoc(systemId_template, privato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateMesiConsTipoDoc(int systemId_template, string mesiCons, Page page)
        {
            try
            {
                docsPaWS.updateMesiConsTipoDoc(systemId_template, mesiCons);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateInvioConsTipoDoc(int systemId_template, string invioCons, Page page)
        {
            try
            {
                docsPaWS.updateInvioConsTipoDoc(systemId_template, invioCons);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateConsolidamentoOggetto(int systemId, string consolida, string systemId_template, Page page)
        {
            try
            {
                docsPaWS.updateConsolidamentoOggCustom(systemId, consolida, systemId_template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateConservazioneOggetto(int systemId, string conserva, string systemId_template, Page page)
        {
            try
            {
                docsPaWS.updateConservazioneOggCustom(systemId, conserva, systemId_template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static int countDocTipoDoc(string tipo_atto, string codiceAmm, Page page)
        {
            try
            {
                return docsPaWS.countDocTipoDoc(tipo_atto, codiceAmm);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return 0;
            }
        }

        public static ArrayList getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getRuoliByAmm(idAmm, codiceRicerca, tipoRicerca));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaAssociazioneDocRuoli(Object[] assDocRuoli, Page page)
        {
            try
            {
                docsPaWS.salvaAssociazioneDocRuoli(assDocRuoli);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }

        }
        
        public static ArrayList getRuoliTipoDoc(string idTipoDoc, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getRuoliTipoDoc(idTipoDoc));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getRuoliTipoAtto(string idTipoDoc, string testo, string tipoRicerca, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getRuoliTipoAtto(idTipoDoc, testo, tipoRicerca));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isDocRepertoriato(string docNumber, string idTipoAtto, Page page)
        {
            try
            {
                return docsPaWS.isDocRepertoriato(docNumber, idTipoAtto);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static ArrayList getTemplatesArchivioDeposito(InfoUtente infoUtente, string idAmm, bool seRepertorio, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTemplatesArchivioDeposito(infoUtente, idAmm, seRepertorio));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }            
        }

        public static string verificaOkContatore(DocsPAWA.DocsPaWR.Templates template)
        {
            string result = string.Empty;
            int lunghezza = 254;
            if (template != null)
            {
                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                    if (oggCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORE"))
                    {
                        //Con incremento differito selezionato
                        if (oggCustom.TIPO_CONTATORE.Equals("R") || oggCustom.TIPO_CONTATORE.Equals("A"))
                        {
                            if (oggCustom.CONTATORE_DA_FAR_SCATTARE)
                            {
                                if (oggCustom.ID_AOO_RF.Equals(""))
                                {
                                    string tipoAooRf = string.Empty;
                                    if (oggCustom.TIPO_CONTATORE.Equals("R"))
                                    {
                                        tipoAooRf = "RF";
                                    }
                                    else
                                    {
                                        tipoAooRf = "Registro";
                                    }
                                    result = "Non è stato selezionato alcun " + tipoAooRf + " per il contatore.";
                                    break;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(oggCustom.NUMERO_DI_CARATTERI))
                    {
                        if (!string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.LENGTH_CAMPI_PROFILATI)))
                            lunghezza = int.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.LENGTH_CAMPI_PROFILATI));
                        else
                            lunghezza = 254;
                    }
                    else
                    {
                        lunghezza = int.Parse(oggCustom.NUMERO_DI_CARATTERI);
                    }
                    if (oggCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CAMPODITESTO"))
                    {
                        if (oggCustom.VALORE_DATABASE.Length > lunghezza)
                        {
                            result = "Il numero massimo di caratteri disponibili per il campo: " + oggCustom.DESCRIZIONE + " è stato superato";
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static void verificaCampiPersonalizzati(Page page, DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            System.Web.UI.WebControls.DropDownList ddl_tipoAtto = (System.Web.UI.WebControls.DropDownList)page.FindControl("ddl_tipoAtto");
            System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati = (System.Web.UI.WebControls.ImageButton)page.FindControl("btn_CampiPersonalizzati");
            System.Web.UI.WebControls.ImageButton btn_salva_P = (System.Web.UI.WebControls.ImageButton)page.FindControl("btn_salva_P");
            System.Web.UI.WebControls.ImageButton btn_protocolla_P = (System.Web.UI.WebControls.ImageButton)page.FindControl("btn_protocolla_P");
            System.Web.UI.WebControls.ImageButton btn_protocollaGiallo_P = (System.Web.UI.WebControls.ImageButton)page.FindControl("btn_protocollaGiallo_P");
            System.Web.UI.WebControls.ImageButton btn_delTipologyDoc = (System.Web.UI.WebControls.ImageButton)page.FindControl("btn_delTipologyDoc");
            bool isAutorizedDelTipology = UserManager.ruoloIsAutorized(page, "ELIMINA_TIPOLOGIA_DOC");

            //E' stata selezionato l'item vuoto del menu a tendina tipi documenti
            if (ddl_tipoAtto.SelectedIndex == 0)
            {
                //Documento con una tipologia su cui non si dispone il diritto di inserimento
                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.docNumber) && schedaDocumento.tipologiaAtto != null && !string.IsNullOrEmpty(schedaDocumento.tipologiaAtto.systemId))
                {
					/* ABBATANGELI GIANLUIGI - TEST PROFILAZIONE
                     * templateDelDoc impegna il database nel caricamento dei campi profilati quindi
                     * prima di eseguire docsPaWS.getTemplateDettagli(schedaDocumento.docNumber)
                     * verifico se sono in grado di reperire il template dall'oggetto schedaDocumento 
                     * passato come parametro
                     * 
                    DocsPaWR.Templates templateDelDoc = docsPaWS.getTemplateDettagli(schedaDocumento.docNumber);
					* */
                    DocsPaWR.Templates templateDelDoc = (schedaDocumento.template == null ? docsPaWS.getTemplateDettagli(schedaDocumento.docNumber) : schedaDocumento.template);
                    if (templateDelDoc != null && templateDelDoc.SYSTEM_ID != 0)
                    {
                        schedaDocumento.template = templateDelDoc;
                        page.Session.Add("template", schedaDocumento.template);
                        ddl_tipoAtto.Items.Clear();
                        System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
                        item.Text = schedaDocumento.template.DESCRIZIONE;
                        item.Value = schedaDocumento.template.SYSTEM_ID.ToString();
                        ddl_tipoAtto.Items.Add(item);
                        ddl_tipoAtto.SelectedItem.Value = schedaDocumento.template.SYSTEM_ID.ToString();
                        ddl_tipoAtto.Enabled = false;
                        btn_CampiPersonalizzati.Visible = true;
                        if (isAutorizedDelTipology)
                            btn_delTipologyDoc.Visible = true;

                        page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                        return;
                    }
                }
                else
                {
                    page.Session.Remove("template");
                    schedaDocumento.template = null;
                    ddl_tipoAtto.Enabled = true;
                    btn_CampiPersonalizzati.Visible = false;
                    if (isAutorizedDelTipology)
                        btn_delTipologyDoc.Visible = false;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=0';", true);
                    return;
                }
            }

            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            //Controllo se provengo da una form pdf processata da livecycle
            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO)
            {
                if (currentContext.ContextState.ContainsKey("ProcessPdfLiveCycle") && schedaDocumento.template != null)
                {
                    DocsPAWA.DocsPaWR.Templates template = schedaDocumento.template;
                    if (template != null && template.ELENCO_OGGETTI.Length != 0)
                    {
                        ddl_tipoAtto.SelectedValue = template.SYSTEM_ID.ToString();
                        impostaOpzioniTipologia(page, template);
                        ddl_tipoAtto.Enabled = true;
                        btn_CampiPersonalizzati.Visible = true;
                        page.Session.Add("template", template);
                        if (btn_salva_P != null)
                            btn_salva_P.Enabled = true;
                        if (isAutorizedDelTipology)
                            btn_delTipologyDoc.Visible = false;
                        currentContext.ContextState.Remove("ProcessPdfLiveCycle");
                        page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                        return;
                    }
                }
            }

            ////Documento non esistente, ma si sta creando come risposta ad un documento
            //if (schedaDocumento != null && string.IsNullOrEmpty(schedaDocumento.docNumber) && page.Session["tipoAttoRipropostoTesto"] != null && page.Session["tipoAttoRipropostoId"] != null)
            //{
            //    ddl_tipoAtto.Enabled = true;
            //    btn_CampiPersonalizzati.Visible = true;
            //    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            //    return;
            //}


            //Documento non esistente e tipologia di documento non esistente
            if (schedaDocumento != null && string.IsNullOrEmpty(schedaDocumento.docNumber) && schedaDocumento.template == null)
            {
                page.Session.Remove("template");
                ddl_tipoAtto.Enabled = true;
                btn_CampiPersonalizzati.Visible = false;
                if (isAutorizedDelTipology)
                    btn_delTipologyDoc.Visible = false;
                return;
            }

            //Documento non esistente (o riproposto) e tipologia di documento esistente
            if (schedaDocumento != null && string.IsNullOrEmpty(schedaDocumento.docNumber) && schedaDocumento.template != null)
            {
                if (page.Session["template"] == null || (page.Session["docRiproposto"] != null && page.Session["docRiproposto"].ToString().ToLower().Equals("true")))
                {
                    page.Session.Add("template", schedaDocumento.template);
                    impostaOpzioniTipologia(page, schedaDocumento.template);
                    ddl_tipoAtto.Enabled = true;
                    btn_CampiPersonalizzati.Visible = true;
                    if (isAutorizedDelTipology)
                        btn_delTipologyDoc.Visible = false;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                }
                return;
            }

            //Documento esistente e tipologia di documento non esistente
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.docNumber) && schedaDocumento.template == null)
            {
                page.Session.Remove("template");
                ddl_tipoAtto.Enabled = true;
                btn_CampiPersonalizzati.Visible = false;
                if (isAutorizedDelTipology)
                    btn_delTipologyDoc.Visible = false;
                return;
            }

            //Documento esistente e tipologia di documento esistente
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.docNumber) && schedaDocumento.template != null)
            {
				/* ABBATANGELI GIANLUIGI - TEST PROFILAZIONE
                 * templateDelDoc impegna il database nel caricamento dei campi profilati che sono già stati caricati
                 * dall'oggetto schedaDocumento passato come parametro
                 * 
                //Documento esistente e tipologia documento appena selezionata
                DocsPaWR.Templates templateDelDoc = docsPaWS.getTemplateDettagli(schedaDocumento.docNumber);
                
                 * */
                DocsPaWR.Templates templateDelDoc = schedaDocumento.template;
                //Documento esistente e tipologia documento appena selezionata
                //DocsPaWR.Templates templateDelDoc = docsPaWS.getTemplateDettagli(schedaDocumento.docNumber);
                if (templateDelDoc != null && templateDelDoc.SYSTEM_ID == 0)
                {
                    page.Session.Add("template", schedaDocumento.template);
                    impostaOpzioniTipologia(page, schedaDocumento.template);
                    if (DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, page) != null)
                    {
                        ddl_tipoAtto.Enabled = false;
                        if (isAutorizedDelTipology)
                            btn_delTipologyDoc.Visible = true;
                    }
                    else
                    {
                        ddl_tipoAtto.Enabled = true;
                        if (isAutorizedDelTipology)
                            btn_delTipologyDoc.Visible = false;
                    }
                    btn_CampiPersonalizzati.Visible = true;

                    if (btn_salva_P != null && btn_protocolla_P != null && btn_protocollaGiallo_P != null)
                    {
                        string pulsanti = btn_salva_P.Enabled + "-" + btn_protocolla_P.Enabled + "-" + btn_protocollaGiallo_P.Enabled;
                        if (page.Session["refreshDxPageProf"] != null && Convert.ToBoolean(page.Session["refreshDxPageProf"]) == true)
                        {
                            page.Session.Remove("refreshDxPageProf");
                            page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1&pulsanti=" + pulsanti + "';", true);
                        }
                    }
                    else
                    {
                        if (page.Session["refreshDxPageProf"] != null && Convert.ToBoolean(page.Session["refreshDxPageProf"]) == true)
                        {
                            page.Session.Remove("refreshDxPageProf");
                            page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                        }
                    }
                    return;
                }

                //Provengo da una ricerca documenti
                if (page.Session["template"] == null || SiteNavigation.CallContextStack.CallerContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_DOCUMENTI)
                {
                    page.Session.Remove("templateRiproposto");
                    page.Session.Add("template", schedaDocumento.template);
                    ddl_tipoAtto.Enabled = false;
                    btn_CampiPersonalizzati.Visible = true;
                    if (isAutorizedDelTipology)
                        btn_delTipologyDoc.Visible = true;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                    return;
                }

                //Protocollo in modifica
                if (page.Session["isDocModificato"] != null && page.Session["isDocModificato"].ToString().ToLower().Equals("true"))
                {
                    ddl_tipoAtto.Enabled = false;
                    btn_CampiPersonalizzati.Visible = true;
                    if (isAutorizedDelTipology)
                        btn_delTipologyDoc.Visible = true;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                    return;
                }

                //Protocollo annnullato
                if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.protocolloAnnullato != null)
                {
                    ddl_tipoAtto.Enabled = false;
                    btn_CampiPersonalizzati.Visible = true;
                    if (isAutorizedDelTipology)
                        btn_delTipologyDoc.Visible = true;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
                    return;
                }
            }

            #region Codice Commentato
            ////Tipologia di documento non selezionata
            //if (ddl_tipoAtto.SelectedValue.Equals(""))
            //{
            //    //Il documento esiste e ha dei campi profilati o ha una tipologia associata
            //    if (schedaDocumento != null && 
            //        !string.IsNullOrEmpty(schedaDocumento.docNumber) &&
            //        schedaDocumento.template != null &&
            //        schedaDocumento.template.SYSTEM_ID != 0
            //        )
            //    {

            //        ddl_tipoAtto.Items.Add("");
            //        System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
            //        ddl_tipoAtto.Items.Add(schedaDocumento.template.DESCRIZIONE);
            //        ddl_tipoAtto.SelectedIndex = ddl_tipoAtto.Items.Count - 1;
            //        ddl_tipoAtto.SelectedItem.Value = schedaDocumento.template.SYSTEM_ID.ToString();

            //        ddl_tipoAtto.Enabled = false;
            //        btn_CampiPersonalizzati.Visible = true;
            //        page.Session.Add("template", schedaDocumento.template);
            //        page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            //        return;
            //    }
            //    else
            //    {
            //        page.Session.Remove("template");
            //        btn_CampiPersonalizzati.Visible = false;
            //        return;
            //    }
            //}

            ////Il documento esiste e ha dei campi profilati o ha una tiologia associata
            //if (schedaDocumento != null && schedaDocumento.docNumber != null && schedaDocumento.docNumber != "")
            //{
            //    DocsPaWR.Templates template = null;
            //    if (ddl_tipoAtto.SelectedValue != null && ddl_tipoAtto.SelectedValue != "")
            //        template = docsPaWS.getTemplateById(ddl_tipoAtto.SelectedValue);
            //    //Tipologia con campi associati
            //    if (template != null && template.ELENCO_OGGETTI.Length != 0)
            //    {
            //        //Nel caso la system_id è uguale a zero vuol dire che non c'è template associato a questo docNumber
            //        DocsPaWR.Templates templateDelDoc = docsPaWS.getTemplateDettagli(schedaDocumento.docNumber);
            //        if (templateDelDoc != null && templateDelDoc.SYSTEM_ID != 0)
            //        {
            //            ddl_tipoAtto.Items.Add("");
            //            ddl_tipoAtto.Items.Add(templateDelDoc.DESCRIZIONE);
            //            ddl_tipoAtto.SelectedIndex = ddl_tipoAtto.Items.Count - 1;
            //            ddl_tipoAtto.SelectedItem.Value = schedaDocumento.template.SYSTEM_ID.ToString();
            //            ddl_tipoAtto.Enabled = false;
            //            btn_CampiPersonalizzati.Visible = true;
            //            DocsPaWR.Templates templateInSess;
            //            if (page.Session["template"] != null)
            //            {
            //                templateInSess = (DocsPAWA.DocsPaWR.Templates)page.Session["template"];
            //                if (templateDelDoc.SYSTEM_ID == templateInSess.SYSTEM_ID)
            //                {
            //                    page.Session.Add("template", templateInSess);
            //                }
            //                else
            //                    page.Session.Add("template", templateDelDoc);
            //            }
            //            else
            //                page.Session.Add("template", templateDelDoc);

            //            page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            //            return;
            //        }
            //        else
            //        {
            //            btn_CampiPersonalizzati.Visible = true;
            //            DocsPaWR.Templates templateInSess;
            //            if (page.Session["template"] != null)
            //            {
            //                templateInSess = (DocsPAWA.DocsPaWR.Templates)page.Session["template"];
            //                if (template.SYSTEM_ID == templateInSess.SYSTEM_ID)
            //                {
            //                    page.Session.Add("template", templateInSess);
            //                }
            //                else
            //                    page.Session.Add("template", template);
            //            }
            //            else
            //                page.Session.Add("template", template);

            //            page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            //            return;                        
            //        }
            //    }

            //    //Disabilito la combo dei tipi atto anche se la tipologia non ha campi ma il documento è in uno stato
            //    if (DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, page) != null)
            //        ddl_tipoAtto.Enabled = false;
            //}

            ////Il documento è in creazione ed è stata selezionata una tipologia di documento
            //if (!ddl_tipoAtto.SelectedValue.Equals(""))
            //{
            //    string idTemplate = ddl_tipoAtto.SelectedItem.Value;
            //    DocsPaWR.Templates template = docsPaWS.getTemplateById(idTemplate);

            //    if (page.Session["template"] == null ||
            //         (page.Session["template"] != null && !(ddl_tipoAtto.SelectedItem.Text.ToUpper()).Equals(((DocsPaWR.Templates)page.Session["template"]).DESCRIZIONE.ToUpper()))
            //        )
            //    {
            //        page.Session.Add("template", template);
            //    }

            //    impostaOpzioniTipologia(page, template);

            //    if (template != null && template.ELENCO_OGGETTI.Length != 0)
            //    {
            //        if (page.Session["docRiproposto"] != null && page.Session["docRiproposto"].ToString().ToLower().Equals("true"))
            //            page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            //        return;
            //    }
            //} 
            #endregion Codice Commmentato
        }
        
        private static void impostaOpzioniTipologia(Page page, Templates template)
        {
            SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;
            
            //Impostazione modello di trasmissione rapida
            if (!string.IsNullOrEmpty(template.CODICE_MODELLO_TRASM) &&
                context != null &&
                context.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO &&
                template != null
                )
            {
                System.Web.UI.WebControls.DropDownList ddl_tmpl = (System.Web.UI.WebControls.DropDownList)page.FindControl("ddl_tmpl");
                DocsPaWR.ModelloTrasmissione modello = ModelliTrasmManager.getModelloByCodice(UserManager.getRegistroSelezionato(page).idAmministrazione, template.CODICE_MODELLO_TRASM, page);
                if (modello != null)
                {
                    System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
                    item.Value = modello.SYSTEM_ID.ToString();
                    item.Text = modello.NOME;
                    if (ddl_tmpl.Items.Contains(item))
                    {
                        ddl_tmpl.SelectedValue = modello.SYSTEM_ID.ToString();
                        page.Session.Add("Modello", modello);
                    }
                }
            }

            //Impostazione codice di classificazione rapida
            if (!string.IsNullOrEmpty(template.CODICE_CLASSIFICA) &&
                context != null &&
                context.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO &&
                template != null
                )
            {
                System.Web.UI.WebControls.TextBox txt_CodFascicolo = (System.Web.UI.WebControls.TextBox)page.FindControl("txt_CodFascicolo");
                System.Web.UI.WebControls.TextBox txt_DescFascicolo = (System.Web.UI.WebControls.TextBox)page.FindControl("txt_DescFascicolo");

                Fascicolo[] listaFasc = FascicoliManager.getListaFascicoliDaCodice(page, template.CODICE_CLASSIFICA, UserManager.getRegistroSelezionato(page), "I");
                if (listaFasc != null && listaFasc.Length >= 1)
                {
                    txt_DescFascicolo.Text = listaFasc[0].descrizione;
                    txt_CodFascicolo.Text = listaFasc[0].codice;
                    FascicoliManager.setFascicoloSelezionatoFascRapida(page, listaFasc[0]);
                }
            }
        }

        public static bool verificaCampiObbligatori(DocsPAWA.DocsPaWR.Templates template)
        {
            if (template != null)
            {
                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    DocsPaWR.OggettoCustom oggCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                    switch (oggCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CasellaDiSelezione":
                            bool selezione = false;
                            for (int j = 0; j < oggCustom.VALORI_SELEZIONATI.Length; j++)
                            {
                                if (oggCustom.VALORI_SELEZIONATI[j] != null)
                                    selezione = true;
                            }
                            if (oggCustom.CAMPO_OBBLIGATORIO == "SI" && !selezione)
                                return true;
                            break;
                        case "SelezioneEsclusiva":
                            if (oggCustom.CAMPO_OBBLIGATORIO == "SI" && (oggCustom.VALORE_DATABASE == "" || oggCustom.VALORE_DATABASE == "-1"))
                                return true;
                            break;
                        case "Link":
                            if (oggCustom.CAMPO_OBBLIGATORIO == "SI" && (oggCustom.VALORE_DATABASE == ""))
                                return true;
                            break;
                        default:
                            if (oggCustom.CAMPO_OBBLIGATORIO == "SI" && oggCustom.VALORE_DATABASE == "")
                                return true;
                            break;
                    }
                }
            }
            return false;
        }

        public static ArrayList getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getDirittiCampiTipologiaDoc(idRuolo, idTemplate));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaDirittiCampiTipologiaDoc(ArrayList listaDirittiCampiSelezionati, Page page)
        {
            try
            {
                DocsPaWR.AssDocFascRuoli[] listaDirittiCampiSelezionati_1 = new DocsPAWA.DocsPaWR.AssDocFascRuoli[listaDirittiCampiSelezionati.Count];
                listaDirittiCampiSelezionati.CopyTo(listaDirittiCampiSelezionati_1);

                docsPaWS.salvaDirittiCampiTipologiaDoc(listaDirittiCampiSelezionati_1);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void estendiDirittiCampiARuoliDoc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {

            try
            {
                DocsPaWR.AssDocFascRuoli[] listaDirittiCampiSelezionati_1 = new DocsPAWA.DocsPaWR.AssDocFascRuoli[listaDirittiCampiSelezionati.Count];
                listaDirittiCampiSelezionati.CopyTo(listaDirittiCampiSelezionati_1);

                DocsPaWR.Ruolo[] listaRuoli_1 = new DocsPAWA.DocsPaWR.Ruolo[listaRuoli.Count];
                listaRuoli.CopyTo(listaRuoli_1);

                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                docsPaWS.estendiDirittiCampiARuoliDoc(listaDirittiCampiSelezionati_1, listaRuoli_1);
            }
            catch (Exception ex)
            {
                //ErrorManager.redirect(page, ex);
            }
        }

        public static DocsPaWR.AssDocFascRuoli getDirittiCampoTipologiaDoc(string idRuolo, string idTemplate, string idOggettoCustom, Page page)
        {
            try
            {
                return docsPaWS.getDirittiCampoTipologiaDoc(idRuolo, idTemplate, idOggettoCustom);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void estendiDirittiRuoloACampiDoc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
        {
            try
            {
                DocsPaWR.OggettoCustom[] listaCampi_1 = new DocsPAWA.DocsPaWR.OggettoCustom[listaCampi.Count];
                listaCampi.CopyTo(listaCampi_1);

                DocsPaWR.AssDocFascRuoli[] listaDirittiRuoli_1 = new DocsPAWA.DocsPaWR.AssDocFascRuoli[listaDirittiRuoli.Count];
                listaDirittiRuoli.CopyTo(listaDirittiRuoli_1);

                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                docsPaWS.estendiDirittiRuoloACampiDoc(listaDirittiRuoli_1, listaCampi_1);
            }
            catch (Exception ex)
            {
                //ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom, Page page)
        {
            try
            {
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                ArrayList result = new ArrayList(docsPaWS.getRuoliFromOggettoCustomDoc(idTemplate, idOggettoCustom));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isInUseCampoComuneDoc(string idTemplate, string idCampoComune, Page page)
        {
            try
            {
                return docsPaWS.isInUseCampoComuneDoc(idTemplate, idCampoComune);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static Templates impostaCampiComuniDoc(Templates modello, Object[] campiComuni, Page page)
        {
            try
            {
                return docsPaWS.impostaCampiComuniDoc(modello, campiComuni);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static UserControls.Calendar.VisibleTimeModeEnum getVisibleTimeMode(OggettoCustom oggettoCustom)
        {
            UserControls.Calendar.VisibleTimeModeEnum ret = UserControls.Calendar.VisibleTimeModeEnum.Nothing;

            switch (oggettoCustom.FORMATO_ORA.ToUpper())
            {
                case "HH":
                    ret = UserControls.Calendar.VisibleTimeModeEnum.Hours;
                    break;

                case "HH:MM":
                    ret = UserControls.Calendar.VisibleTimeModeEnum.Minutes;
                    break;

                case "HH:MM:SS":
                    ret = UserControls.Calendar.VisibleTimeModeEnum.Seconds;
                    break;
            }

            return ret;
        }

        public static string VerifyAndSetTipoDoc(DocsPaWR.InfoUtente infoUtente, ref DocsPaWR.SchedaDocumento schedaDocumento, Page page)
        {
            string messageResult = string.Empty;
            try
            {
                string message = docsPaWS.VerifyAndSetTipoDoc(infoUtente, ref schedaDocumento);

                //Se il message è KO ci sono stati problemi nella funzione di verifica e va controllato il log del backend
                //Ma le altre operazioni devono continuare
                //I campi di profilazione e la scheda documento non sono stati modificati
                if (message.ToUpper() == "KO")
                {
                    messageResult = string.Empty;
                }

                //Caso i cui i controlli sui campi profilati hanno dato errori
                //Gli errori vanno mostrati a video all'utente 
                //I campi di profilazione e la scheda documento non sono stati modificati
                if(message.ToUpper() != "KO" && !string.IsNullOrEmpty(message))
                {
                    //Visualizzo all'utente il resoconto degli errori sui controlli
                    //page.ClientScript.RegisterStartupScript(page.GetType(), "resocontoControlliCampiProfilazione", "alert('" + message + "');", true);
                    messageResult = message;
                }

                //Caso in cui i controlli sui campi profilati sono andati tutti a buon fine
                //Non devo visualizzare nessun resosconto all'utente
                //La scheda documento passata per riferimento a questo metodo diventa la scheda controllata e valorizzata
                if (message.ToUpper() != "KO" && string.IsNullOrEmpty(message))
                {
                    messageResult = string.Empty;
                    page.Session["template"] = schedaDocumento.template;
                }

                // Assegnazione della nuova scheda documento in sessione
                DocumentManager.setDocumentoSelezionato(schedaDocumento);
                DocumentManager.setDocumentoInLavorazione(schedaDocumento);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);                
                return string.Empty;
            }

            return messageResult;
        }

        public static void setSearchOutputRowSelected(SearchOutputRow row)
        {
            HttpContext.Current.Session["searchOutputRowSelected"] = row;
        }

        public static SearchOutputRow getSearchOutputRowSelected()
        {
            return (SearchOutputRow) HttpContext.Current.Session["searchOutputRowSelected"];
        }

        public static void removeSearchOutputRowSelected()
        {
            HttpContext.Current.Session["searchOutputRowSelected"] = null;
        }

        public static void addNoRightsCustomObject(ArrayList assDocFascRuoli, OggettoCustom oggettoCustom)
        {
            DocsPaWR.AssDocFascRuoli[] assDocFascRuoliArray = (DocsPaWR.AssDocFascRuoli[])assDocFascRuoli.ToArray(typeof(AssDocFascRuoli));
            DocsPaWR.AssDocFascRuoli assDocFascRuolo = assDocFascRuoliArray.Where(asRuolo => asRuolo.ID_OGGETTO_CUSTOM.Equals(oggettoCustom.SYSTEM_ID.ToString())).FirstOrDefault();
            if (assDocFascRuolo == null)
            {
                DocsPaWR.AssDocFascRuoli newAssDocFascRuoli = new AssDocFascRuoli();
                newAssDocFascRuoli.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                newAssDocFascRuoli.INS_MOD_OGG_CUSTOM = "0";
                newAssDocFascRuoli.VIS_OGG_CUSTOM = "0";
                assDocFascRuoli.Add(newAssDocFascRuoli);
            }
            
        }

        public static DocsPAWA.DocsPaWR.Contatore[] GetValuesContatoriDoc(Page page, OggettoCustom oggettoCustom)
        {
            try
            {
                DocsPAWA.DocsPaWR.Contatore[] result = docsPaWS.GetValuesContatoriDoc(oggettoCustom);
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void SetValuesContatoreDoc(Page page, Contatore contatore)
        {
            try
            {
                docsPaWS.SetValuesContatoreDoc(contatore);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);                
            }
        }

        public static void DeleteValueContatoreDoc(Page page, Contatore contatore)
        {
            try
            {
                docsPaWS.DeleteValueContatoreDoc(contatore);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void InsertValuesContatoreDoc(Page page, Contatore contatore)
        {
            try
            {
                docsPaWS.InsertValuesContatoreDoc(contatore);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        /// <summary>
        /// Funzione per la restituzione del nome di un template a partire dal suo id
        /// </summary>
        /// <param name="page">Pagina chiamante</param>
        /// <param name="modelId">Id del modello di cui ricavare il nome</param>
        /// <returns>Nome del modello</returns>
        public static String GetModelNameById(Page page, String modelId)
        {
            String toReturn = String.Empty;

            try
            {
                toReturn = docsPaWS.GetDocModelNameById(modelId);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
 
            }

            return toReturn;
        }

        /// <summary>
        /// Funzione per la restituzione del template con solo i campi visibili popolati
        /// </summary>
        /// <param name="page">Pagina chiamante</param>
        /// <param name="modelId">Docnumber</param>
        /// /// <param name="modelId">Lista campi visibili dalla griglia</param>
        /// <returns>Nome del modello</returns>
        public static Templates getTemplateDettagliFilterObjects(string docNumber, string idTipoAtto, string[] visibleFields, Page page)
        {
            try
            {
                return docsPaWS.getTemplateDettagliFilterObjects(docNumber, idTipoAtto, visibleFields);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per la restituzione della tipologia di un documento senza i campi di profilazione
        /// </summary>
        /// <param name="page">Pagina chiamante</param>
        /// <param name="modelId">Docnumber</param>
        /// /// <param name="modelId">Lista campi visibili dalla griglia</param>
        /// <returns>Nome del modello</returns>
        public static Templates getTemplate(string docNumber, Page page)
        {
            try
            {
                return docsPaWS.getTemplate(string.Empty, string.Empty, docNumber);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Templates getTemplatePerRicercaById(string idAtto, Page page)
        {
            try
            {
                Templates template = docsPaWS.getTemplatePerRicercaById(idAtto);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                        template = docsPaWS.getTemplateCampiComuniById(UserManager.getInfoUtente(page), template.SYSTEM_ID.ToString());
                    }
                    catch (Exception e)
                    {
                        //In questo caso vuol dire che provengo da amministrazione e l'infoUtente non esiste
                        //quindi il template non va filtrato per visibilità
                    }
                }

                if (template != null)
                    return template;
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        #region Abilitazione / Disabilitazione storcico campi profilati

        /// <summary>
        /// Metodo per l'attivazione dello storico campi profilati per determinati campi di una tipologia documentale
        /// </summary>
        /// <param name="activeAllFields">True se bisogna attivare lo storico su tutti i campi</param>
        /// <param name="customObjects">Lista dei campi per cui attivare lo storico</param>
        /// <param name="templateId">Id della tipologia cui appartengono i campi di cui attivare lo storico</param>
        /// <returns>Risultato dell'attivazione</returns>
        public static SelectiveHistoryResponse ActiveSelectiveHistory(bool activeAllFields, CustomObjHistoryState[] customObjects, String templateId)
        {
            return docsPaWS.ActiveSelectiveHistory(new SelectiveHistoryRequest()
                {
                    ActiveAllFields = activeAllFields,
                    CustomObjects = customObjects,
                    ObjType = ObjectType.Document,
                    TemplateId = templateId
                });
        }

        /// <summary>
        /// Metodo per la disattivazione dello storico campi profilati per tutti i campi di una tipologia
        /// </summary>
        /// <param name="templateId">Id della tipologia</param>
        /// <returns>Risultato della disabilitazione</returns>
        public static SelectiveHistoryResponse DeactivateAllHistory(String templateId)
        {
            return docsPaWS.DeactivateAllHistory(new SelectiveHistoryRequest()
                {
                    TemplateId = templateId,
                    ObjType = ObjectType.Document
                });
        }

        /// <summary>
        /// Metodo per il recupero della lista dello stato di attivazione dello storico relativo ai campi
        /// profilati di una tipologia
        /// </summary>
        /// <param name="templateId">Id della tipologia di cui recuperare la lista degli stati di attivazione dello storico</param>
        /// <returns>Lista degli stati di abilitazione</returns>
        public static SelectiveHistoryResponse GetCustomHistoryList(String templateId)
        {
            return docsPaWS.GetCustomHistoryList(new SelectiveHistoryRequest()
                {
                    TemplateId = templateId,
                    ObjType = ObjectType.Document
                });
        }


        #endregion

        #region Contesto Procedurale

        public static List<ContestoProcedurale> GetListContestoProcedurale(InfoUtente infoUtente)
        {
            return docsPaWS.GetListContestoProcedurale(infoUtente).ToList();
        }

        public static bool UpdateAssociazioneTemplateContestoProcedurale(string idTipoAtto, string idContesto, InfoUtente infoUtente)
        {
            return docsPaWS.UpdateAssociazioneTemplateContestoProcedurale(idTipoAtto, idContesto, infoUtente);
        }

        public static bool InsertContestoProcedurale(ContestoProcedurale contesto, InfoUtente infoUtente)
        {
            return docsPaWS.InsertContestoProcedurale(contesto, infoUtente);
        }

        #endregion

        public static void AnnullaContatoreDiRepertorio(string idOggetto, string docNumber, Page page)
        {
            try
            {
                docsPaWS.AnnullaContatoreDiRepertorio(idOggetto, docNumber, UserManager.getInfoUtente(page));
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);                
            }
        }

        public static void Storicizza(DocsPaWR.Storicizzazione storico, Page page)
        {
            try
            {
                docsPaWS.Storicizza(storico);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        /// <summary>
        /// Funzione per la rimozione di una tipologia per un documento
        /// </summary>
        /// <param name="page">InfoDocumento</param>
        /// <param name="modelId">SchedaDocumento</param>
        /// <param name="modelId">Pagina chiamante</param>
        public static string RemoveTipologyDoc(InfoUtente info, SchedaDocumento schedaDocumento, Page page)
        {
            string msg = string.Empty;

            try
            {
                /*
                //La rimozione della tipologia è possibile solo se non sono presenti campi contatore oppure questi non sono valorizzati
                if (schedaDocumento != null && schedaDocumento.template != null && schedaDocumento.template.ELENCO_OGGETTI != null)
                {
                    foreach(OggettoCustom ogg in schedaDocumento.template.ELENCO_OGGETTI)
                    {
                        if (ogg.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("Contatore") || ogg.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("ContatoreSottocontatore"))
                        {
                            if(!String.IsNullOrEmpty(ogg.VALORE_DATABASE) || !String.IsNullOrEmpty(ogg.VALORE_SOTTOCONTATORE))
                                msg = "Non è possibile eliminare la tipologia perchè contiene dei contatori valorizzati.";
                        }
                    }
                }
                
                if(String.IsNullOrEmpty(msg))
                    docsPaWS.RemoveTipologyDoc(info, schedaDocumento);
                */
                msg = docsPaWS.RemoveTipologyDoc(info, schedaDocumento);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return string.Empty;
            }

            return msg;
        }
    }
}
