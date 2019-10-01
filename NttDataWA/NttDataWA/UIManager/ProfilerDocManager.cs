using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.Web.UI.WebControls;
using DocsPaIntegration.Search;

namespace NttDataWA.UIManager
{
    public class ProfilerDocManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static Templates getTemplateById(string idTemplate)
        {
            try
            {
                Templates template = docsPaWS.getTemplateById(idTemplate);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1")
                {
                    DocsPaWR.InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                    template = docsPaWS.getTemplateCampiComuniById(infoUtente, idTemplate);

                }

                if (template != null)
                {
                    return template;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string CountTipoFromTipologia(Utils.TypeProfiledFields type, string description)
        {
            try
            {
                string result = string.Empty;
                string count = docsPaWS.CountTipoFromTipologia(type.ToString(), description);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<AssDocFascRuoli> getDirittiCampiTipologiaDoc(string idRole, string idTemplate)
        {
            try
            {
                List<AssDocFascRuoli> result = new List<AssDocFascRuoli>();
                result = docsPaWS.getDirittiCampiTipologiaDoc(idRole, idTemplate).ToList<AssDocFascRuoli>();
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void addNoRightsCustomObject(List<AssDocFascRuoli> assDocFascRuoli, OggettoCustom oggettoCustom)
        {
            try
            {
                DocsPaWR.AssDocFascRuoli assDocFascRuolo = assDocFascRuoli.Where(asRuolo => asRuolo.ID_OGGETTO_CUSTOM.Equals(oggettoCustom.SYSTEM_ID.ToString())).FirstOrDefault();
                if (assDocFascRuolo == null)
                {
                    DocsPaWR.AssDocFascRuoli newAssDocFascRuoli = new AssDocFascRuoli();
                    newAssDocFascRuoli.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                    newAssDocFascRuoli.INS_MOD_OGG_CUSTOM = "0";
                    newAssDocFascRuoli.VIS_OGG_CUSTOM = "0";
                    assDocFascRuoli.Add(newAssDocFascRuoli);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static UserControls.Calendar.VisibleTimeModeEnum getVisibleTimeMode(OggettoCustom oggettoCustom)
        {
            switch (oggettoCustom.FORMATO_ORA.ToUpper())
            {
                case "HH":
                    return UserControls.Calendar.VisibleTimeModeEnum.Hours;

                case "HH:MM":
                    return UserControls.Calendar.VisibleTimeModeEnum.Minutes;

                case "HH:MM:SS":
                    return UserControls.Calendar.VisibleTimeModeEnum.Seconds;

                default:
                    return UserControls.Calendar.VisibleTimeModeEnum.Nothing;
            }
        }

        /// <summary>
        /// Funzione per la restituzione della tipologia di un documento senza i campi di profilazione
        /// </summary>
        /// <param name="page">Pagina chiamante</param>
        /// <param name="modelId">Docnumber</param>
        /// /// <param name="modelId">Lista campi visibili dalla griglia</param>
        /// <returns>Nome del modello</returns>
        public static Templates getTemplate(string docNumber)
        {
            try
            {
                return docsPaWS.getTemplate(string.Empty, string.Empty, docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getIdTemplate(string docNumber)
        {
            try
            {
                return docsPaWS.getIdTemplate(docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool verificaCampiObbligatori(DocsPaWR.Templates template)
        {
            try
            {
                if (template != null)
                {
                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggCustom = (DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string verificaOkContatore(DocsPaWR.Templates template, out string customMessage)
        {
            customMessage = string.Empty;
            string result = string.Empty;
            int lunghezza = 254;

            try
            {
                if (template != null)
                {
                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggCustom = (DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
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
                                            //tipoAooRf = "RF";
                                            //result = "Non è stato selezionato alcun RF per il contatore.";
                                            result = "WarningProfilerDocManagerNoRF";
                                        }
                                        else
                                        {
                                            //tipoAooRf = "Registro";
                                            //result = "Non è stato selezionato alcun Registro per il contatore.";
                                            result = "WarningProfilerDocManagerNoRegistry";
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(oggCustom.NUMERO_DI_CARATTERI))
                        {
                            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LENGTH_CAMPI_PROFILATI.ToString()]))
                                lunghezza = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LENGTH_CAMPI_PROFILATI.ToString()]);
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
                                customMessage = "Il numero massimo di caratteri disponibili per il campo: " + oggCustom.DESCRIZIONE + " è stato superato";
                                result = "CUSTOMERROR";
                                break;
                            }
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string VerifyAndSetTipoDoc(DocsPaWR.InfoUtente infoUtente, ref DocsPaWR.SchedaDocumento schedaDocumento)
        {
            try
            {
                string messageResult = string.Empty;

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
                if (message.ToUpper() != "KO" && !string.IsNullOrEmpty(message))
                {
                    messageResult = message;
                }

                //Caso in cui i controlli sui campi profilati sono andati tutti a buon fine
                //Non devo visualizzare nessun resosconto all'utente
                //La scheda documento passata per riferimento a questo metodo diventa la scheda controllata e valorizzata
                if (message.ToUpper() != "KO" && string.IsNullOrEmpty(message))
                {
                    messageResult = string.Empty;
                }
                return messageResult;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void AnnullaContatoreDiRepertorio(string idOggetto, string docNumber)
        {
            try
            {
                docsPaWS.AnnullaContatoreDiRepertorio(idOggetto, docNumber, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void Storicizza(DocsPaWR.Storicizzazione storico)
        {
            try
            {
                docsPaWS.Storicizza(storico);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static Templates getTemplateDettagli(string docNumber)
        {
            try
            {
                return docsPaWS.getTemplateDettagli(docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static SearchOutputRow getSearchOutputRowSelected()
        {
            try
            {
                return (SearchOutputRow)HttpContext.Current.Session["searchOutputRowSelected"];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static OggettoCustom getOggettoById(string idOggetto)
        {
            try
            {
                return docsPaWS.getOggettoById(idOggetto);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        //Tatiana 22/05/2013
        public static Templates getTemplatePerRicerca(string idAmministrazione, string tipoAtto)
        {
            try
            {
                Templates template = docsPaWS.getTemplatePerRicerca(idAmministrazione, tipoAtto);

                //Se la tipologia è di campi comuni (Iperdocumento) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di documento associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1")
                {
                    try
                    {
                        DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                        template = docsPaWS.getTemplateCampiComuniById(UserManager.GetInfoUser(), template.SYSTEM_ID.ToString());
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
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}