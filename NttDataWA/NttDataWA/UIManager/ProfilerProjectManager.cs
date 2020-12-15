using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.Web.UI.WebControls;
using System.Collections;

namespace NttDataWA.UIManager
{
    public class ProfilerProjectManager
    {       
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static Templates getTemplateFascById(string idTemplate)
        {
            try
            {
                Templates template = docsPaWS.getTemplateFascById(idTemplate);

                //Se la tipologia è di campi comuni (Iperfascicolo) richiamo il metodo che mi restituisce il temmplate
                //affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
                //di fascicolo associate al ruolo
                if (template != null && template.IPER_FASC_DOC == "1")
                {
                    DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                    template = docsPaWS.getTemplateFascCampiComuniById(UserManager.GetInfoUser(), idTemplate);
                }

                if (template != null)
                    return template;
                else
                    return null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.AssDocFascRuoli getDirittiCampoTipologiaFasc(string idRuolo, string idTemplate, string idOggettoCustom)
        {
            try
            {
                return docsPaWS.getDirittiCampoTipologiaFasc(idRuolo, idTemplate, idOggettoCustom);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getDirittiCampiTipologiaFasc(idRuolo, idTemplate));
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void addNoRightsCustomObject(ArrayList assDocFascRuoli, OggettoCustom oggettoCustom)
        {
            try
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static ArrayList getTipoFascFromRuolo(string idAmministrazione, string idRuolo, string diritti)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getTipoFascFromRuolo(idAmministrazione, idRuolo, diritti));
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Templates getTemplateFascDettagli(string idProject)
        {
            try
            {
                return docsPaWS.getTemplateFascDettagli(idProject);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string verificaOkContatoreFasc(DocsPaWR.Templates template)
        {
            string result = string.Empty;
            int lunghezza = 254;
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
                            result = "il numero massimo di carattere disponibili per il campo: " + oggCustom.DESCRIZIONE + " è stato superato";
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static Templates getAttributiTipoFasc(InfoUtente infoUtente, string idTipoFasc)
        {
            try
            {
                Templates result = docsPaWS.getAttributiTipoFasc(infoUtente, idTipoFasc);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

    }
}