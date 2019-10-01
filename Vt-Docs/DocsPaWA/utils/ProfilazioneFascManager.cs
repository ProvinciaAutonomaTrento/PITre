using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using System.Data;
using System.Linq;

namespace DocsPAWA
{
    public class ProfilazioneFascManager
    {
        private static DocsPaWebService docsPaWS = ProxyManager.getWS();

        public static ArrayList getTemplatesFasc(string idAmministrazione, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTemplatesFasc(idAmministrazione));
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
                return docsPaWS.getOggettoFascById(idOggetto);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Templates eliminaOggettoCustomTemplateFasc(Templates template, int oggettoCustom, Page page)
        {
            try
            {
                return docsPaWS.eliminaOggettoCustomTemplateFasc(template, oggettoCustom);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool salvaTemplateFasc(InfoUtente infoUtente, Templates template, string idAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.salvaTemplateFasc(infoUtente,template,idAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }            
        }

        public static bool aggiornaTemplateFasc(Templates template, Page page)
        {
            try
            {
                return docsPaWS.aggiornaTemplateFasc(template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static bool isValueInUseFasc(string idOggetto, string idTemplate, string valoreOggettoDB, Page page)
        {
            try
            {
                return docsPaWS.isValueInUseFasc(idOggetto, idTemplate, valoreOggettoDB);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static bool disabilitaTemplateFasc(Templates template, string idAmministrazione, string codiceAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.disabilitaTemplateFasc(template,idAmministrazione,codiceAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static Templates getTemplateFascById(string idTemplate, Page page)
        {
            try
            {
                Templates template = docsPaWS.getTemplateFascById(idTemplate);

                //Se la tipologia è di campi comuni (Iperfascicolo) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di fascicolo associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1" && page != null)
                {
                    try
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                        template = docsPaWS.getTemplateFascCampiComuniById(UserManager.getInfoUtente(page), idTemplate);
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

        public static Templates spostaOggettoFasc(Templates template, int oggettoSelezionato, string spostamento, Page page)
        {
            try
            {
                return docsPaWS.spostaOggettoFasc(template, oggettoSelezionato, spostamento);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void messaInEserizioTemplateFasc(Templates template, string idAmministrazione, Page page)
        {
            try
            {
                docsPaWS.messaInEserizioTemplateFasc(template, idAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList getTipoFasc(string idAmministrazione, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTipoFasc(idAmministrazione));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }           
        }

        public static ArrayList getTipoFascFromRuolo(string idAmministrazione, string idRuolo, string diritti, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTipoFascFromRuolo(idAmministrazione, idRuolo, diritti));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }            
        }

        public static Templates getTemplateFascDettagli(string idProject, Page page)
        {
            try
            {
                return docsPaWS.getTemplateFascDettagli(idProject);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getRuoliTipoFasc(string idTipoFasc, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getRuoliTipoFasc(idTipoFasc));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaAssociazioneFascRuoli(Object[] assFascRuoli, Page page)
        {
            try
            {
                docsPaWS.salvaAssociazioneFascRuoli(assFascRuoli);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static Templates impostaCampiComuniFasc(Templates modello, Object[] campiComuni, Page page)
        {
            try
            {
                return docsPaWS.impostaCampiComuniFasc(modello, campiComuni);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isInUseCampoComuneFasc(string idTemplate, string idCampoComune, Page page)
        {
            try
            {
                return docsPaWS.isInUseCampoComuneFasc(idTemplate, idCampoComune);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static int countFascTipoFasc(string tipo_fasc, string codiceAmm, Page page)
        {
            try
            {
                return docsPaWS.countFascTipoFasc(tipo_fasc, codiceAmm);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return 0;
            }
        }

        public static void updatePrivatoTipoFasc(int systemId_template, string privato, Page page)
        {
            try
            {
                docsPaWS.updatePrivatoTipoFasc(systemId_template, privato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void updateMesiConsTipoFasc(int systemId_template, string mesi, Page page)
        {
            try
            {
                docsPaWS.updateMesiConsTipoFasc(systemId_template, mesi);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getIdModelliTrasmAssociatiFasc(idTipoFasc, idDiagramma, idStato));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaAssociazioneModelliFasc(string idTipoFasc, string idDiagramma, Object[] modelliSelezionati, string idStato, Page page)
        {
            try
            {
                docsPaWS.salvaAssociazioneModelliFasc(idTipoFasc, idDiagramma, modelliSelezionati, idStato);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }

        }

        public static void updateScadenzeTipoFasc(int systemId_template, string scadenza, string preScadenza, Page page)
        {
            try
            {
                docsPaWS.updateScadenzeTipoFasc(systemId_template, scadenza, preScadenza);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static string getIdTemplateFasc(string idProject, Page page)
        {
            try
            {
                return docsPaWS.getIdTemplateFasc(idProject);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static string verificaOkContatoreFasc(DocsPAWA.DocsPaWR.Templates template)
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
                            result = "il numero massimo di carattere disponibili per il campo: " + oggCustom.DESCRIZIONE+" è stato superato";
                            break;
                        }
                    }
                }
            }
            return result;
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

        public static Templates aggiungiOggettoCustomTemplateFasc(OggettoCustom oggettoCustom, Templates template, Page page)
        {
            try
            {
                return docsPaWS.aggiungiOggettoCustomTemplateFasc(oggettoCustom, template);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getDirittiCampiTipologiaFasc(idRuolo, idTemplate));
                return result;                
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void salvaDirittiCampiTipologiaFasc(ArrayList listaDirittiCampiSelezionati, Page page)
        {
            try
            {
                DocsPaWR.AssDocFascRuoli[] listaDirittiCampiSelezionati_1 = new DocsPAWA.DocsPaWR.AssDocFascRuoli[listaDirittiCampiSelezionati.Count];
                listaDirittiCampiSelezionati.CopyTo(listaDirittiCampiSelezionati_1);

                docsPaWS.salvaDirittiCampiTipologiaFasc(listaDirittiCampiSelezionati_1);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void estendiDirittiCampiARuoliFasc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {

            try
            {
                DocsPaWR.AssDocFascRuoli[] listaDirittiCampiSelezionati_1 = new DocsPAWA.DocsPaWR.AssDocFascRuoli[listaDirittiCampiSelezionati.Count];
                listaDirittiCampiSelezionati.CopyTo(listaDirittiCampiSelezionati_1);

                DocsPaWR.Ruolo[] listaRuoli_1 = new DocsPAWA.DocsPaWR.Ruolo[listaRuoli.Count];
                listaRuoli.CopyTo(listaRuoli_1);

                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                docsPaWS.estendiDirittiCampiARuoliFasc(listaDirittiCampiSelezionati_1, listaRuoli_1);
            }
            catch (Exception ex)
            {
                //ErrorManager.redirect(page, ex);
            }
        }

        public static DocsPaWR.AssDocFascRuoli getDirittiCampoTipologiaFasc(string idRuolo, string idTemplate, string idOggettoCustom, Page page)
        {
            try
            {
                return docsPaWS.getDirittiCampoTipologiaFasc(idRuolo, idTemplate, idOggettoCustom);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void estendiDirittiRuoloACampiFasc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
        {
            try
            {
                DocsPaWR.OggettoCustom[] listaCampi_1 = new DocsPAWA.DocsPaWR.OggettoCustom[listaCampi.Count];
                listaCampi.CopyTo(listaCampi_1);

                DocsPaWR.AssDocFascRuoli[] listaDirittiRuoli_1 = new DocsPAWA.DocsPaWR.AssDocFascRuoli[listaDirittiRuoli.Count];
                listaDirittiRuoli.CopyTo(listaDirittiRuoli_1);

                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                docsPaWS.estendiDirittiRuoloACampiFasc(listaDirittiRuoli_1, listaCampi_1);
            }
            catch (Exception ex)
            {
                //ErrorManager.redirect(page, ex);
            }
        }

        public static ArrayList getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom, Page page)
        {
            try
            {
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                ArrayList result = new ArrayList(docsPaWS.getRuoliFromOggettoCustomFasc(idTemplate, idOggettoCustom));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Templates getAttributiTipoFasc(InfoUtente infoUtente, string idTipoFasc, Page page)
        {
            try
            {
                Templates result = docsPaWS.getAttributiTipoFasc(infoUtente,idTipoFasc);
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static  UserControls.Calendar.VisibleTimeModeEnum getVisibleTimeMode(OggettoCustom oggettoCustom)
        {
            switch (oggettoCustom.FORMATO_ORA.ToUpper())
            {
                case"HH":
                    return UserControls.Calendar.VisibleTimeModeEnum.Hours;
                    break;

                case"HH:MM":
                    return UserControls.Calendar.VisibleTimeModeEnum.Minutes;
                    break;

                case"HH:MM:SS":
                    return UserControls.Calendar.VisibleTimeModeEnum.Seconds;
                    break;

                default:
                    return UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                    break;
            }
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

        public static void verificaCampiPersonalizzati(Page page, DocsPAWA.DocsPaWR.Fascicolo fascicolo, Microsoft.Web.UI.WebControls.TreeView Folders,  bool editMode)
        {
            System.Web.UI.WebControls.DropDownList ddl_tipoFasc = (System.Web.UI.WebControls.DropDownList)page.FindControl("ddl_tipologiaFasc");
            System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati = (System.Web.UI.WebControls.ImageButton)page.FindControl("img_btnDettagliProf");
            System.Web.UI.WebControls.TextBox txt_fascdesc = (System.Web.UI.WebControls.TextBox)page.FindControl("txt_fascdesc");

            //Fascicolo senza tipologia (Apro la lista documenti in fascicolo)
            if (fascicolo != null && (fascicolo.template == null || fascicolo.template.SYSTEM_ID == 0) && string.IsNullOrEmpty(ddl_tipoFasc.SelectedValue))
            {
                ddl_tipoFasc.Enabled = editMode;
                btn_CampiPersonalizzati.Enabled = editMode;

                page.Session["ListaDocs-CampiProf"] = "ListaDocs";
                string urlFormattata = System.Text.RegularExpressions.Regex.Replace(txt_fascdesc.Text, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
                string newUrl = "tabPulsantiDoc.aspx?idFolder=" + getSelectedNodeFolder(page, Folders).ID.ToString() + "&AclRevocata=" + GetControlAclFascicolo(page).AclRevocata.ToString();// + "&codFasc=" + urlFormattata;
                page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='" + newUrl + "';", true);                       
            }

            //Fascicolo con tipologia (Apro i campi profilati del fascicolo)
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" && fascicolo.tipo.Equals("P"))
            {
                //Fasciolo con template salvato
                if (fascicolo != null && fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
                {
                    ddl_tipoFasc.Enabled = false;
                    btn_CampiPersonalizzati.Enabled = true;

                    System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem(fascicolo.template.DESCRIZIONE, fascicolo.template.SYSTEM_ID.ToString());
                    if (!ddl_tipoFasc.Items.Contains(item))
                    {
                        ddl_tipoFasc.Items.Add(item);
                        ddl_tipoFasc.SelectedValue = item.Value;
                    }

                    page.Session["Template"] = fascicolo.template;
                    page.Session["ListaDocs-CampiProf"] = "CampiProf";
                    string newUrl = "tabPulsantiDoc.aspx?tipoFascicolo=" + fascicolo.tipo + "&codTipologiaFasc=" + ddl_tipoFasc.SelectedValue.ToString() + "&editMode=" + editMode.ToString() + "&AclRevocata=" + GetControlAclFascicolo(page).AclRevocata.ToString();// + "&codFasc=" + txt_fascdesc.Text;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='" + newUrl + "';", true);                                                                            
                }

                //Fasciolo con template appena selezionato
                if (fascicolo != null && fascicolo.template != null && fascicolo.template.SYSTEM_ID == 0 && !string.IsNullOrEmpty(ddl_tipoFasc.SelectedValue))
                {
                    ddl_tipoFasc.Enabled = true;
                    btn_CampiPersonalizzati.Enabled = true;

                    page.Session["ListaDocs-CampiProf"] = "CampiProf";
                    string newUrl = "tabPulsantiDoc.aspx?tipoFascicolo=" + fascicolo.tipo + "&codTipologiaFasc=" + ddl_tipoFasc.SelectedValue.ToString() + "&editMode=" + editMode.ToString() + "&AclRevocata=" + GetControlAclFascicolo(page).AclRevocata.ToString();// + "&codFasc=" + txt_fascdesc.Text;
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='" + newUrl + "';", true);                                                                            
                }
            }
            else
            {
                if (fascicolo != null)
                {
                    string newUrl = "tabPulsantiDoc.aspx?tipoFascicolo=" + fascicolo.tipo + "&editMode=" + editMode.ToString() + "&AclRevocata=" + GetControlAclFascicolo(page).AclRevocata.ToString();
                    page.ClientScript.RegisterStartupScript(page.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='" + newUrl + "';", true);
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

        private static Microsoft.Web.UI.WebControls.TreeNode getSelectedNodeFolder(Page page, Microsoft.Web.UI.WebControls.TreeView Folders)
        {
            Microsoft.Web.UI.WebControls.TreeNode nodeToSelect;
            if (page.Session["fascDocumenti.nodoSelezionato"] != null)
            {
                nodeToSelect = (Microsoft.Web.UI.WebControls.TreeNode)page.Session["fascDocumenti.nodoSelezionato"];
            }
            else
            {
                if (Folders.Nodes.Count > 0)
                {
                    nodeToSelect = Folders.GetNodeFromIndex(Folders.SelectedNodeIndex);
                }
                else
                {
                    nodeToSelect = null;
                }
                page.Session["fascDocumenti.nodoSelezionato"] = nodeToSelect;
            }

            return nodeToSelect;
        }

        private static DocsPAWA.fascicolo.AclFascicolo GetControlAclFascicolo(Page page)
        {
            return (DocsPAWA.fascicolo.AclFascicolo)page.FindControl("aclFascicolo");
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
                toReturn = docsPaWS.GetFascModelNameById(modelId);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);

            }

            return toReturn;
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
                ObjType = ObjectType.Folder,
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
                ObjType = ObjectType.Folder
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
                ObjType = ObjectType.Folder
            });
        }


        #endregion


        internal static void updateTemplateStruttura(int idtipofascicolo, int idtemplate, Page pagina)
        {
            try
            {
                docsPaWS.SaveStrutturaTemplateRelation(idtipofascicolo, int.MinValue, idtemplate);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(pagina, ex);
            }
        }
    }
}
