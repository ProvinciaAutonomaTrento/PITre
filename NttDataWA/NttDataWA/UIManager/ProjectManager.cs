using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web.UI;
using System.IO;
using System.Collections;


namespace NttDataWA.UIManager
{
    public class ProjectManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static Fascicolo[] GetProjectFromCodeNoSecurity(string projectCode, string idAdm, string idClassificationScheme, bool onlyGenerals)
        {
            try
            {
                DocsPaWR.Fascicolo[] result = docsPaWS.GetFascicoloDaCodiceNoSecurity(projectCode, idAdm, idClassificationScheme, onlyGenerals);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce un'oggetto fascicolo
        /// </summary>
        /// <param name="registry">oggetto registro</param>
        /// <param name="codeProject">codice del nodo del titolario</param>
        /// <returns></returns>
        public static DocsPaWR.Fascicolo GetProjectByCode(Registro registry, string codeProject)
        {
            try
            {
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                //bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //        && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice(infoUtente, codeProject, registry, false, enableProfilazione);

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Fascicolo GetProjectByCodeRedAndClassScheme(Registro registry, string codeProject, string idTitolario)
        {
            try
            {
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, codeProject, registry, enableUfficioRef, enableProfilazione, idTitolario);

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce la classificazione da un idClassificazione
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static Fascicolo getClassificazioneById(string idClassificazione, InfoUtente infoutente)
        {
            Fascicolo fascicolo = null;
            try
            {
                fascicolo = docsPaWS.FascicolazioneGetFascicoloById(idClassificazione, infoutente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fascicolo;
        }

        /// <summary>
        /// restituisce la classificazione da un idClassificazione senza security
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static Fascicolo getClassificazioneById(string idClassificazione)
        {
            Fascicolo fascicolo = null;
            try
            {
                fascicolo = docsPaWS.FascicolazioneGetFascicoloByIdNoSecurity(idClassificazione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fascicolo;
        }

        public static Folder[] GetFoldersDocument(string idDocumento)
        {
            Folder[] retValue = null;
            try
            {
                retValue = docsPaWS.FascicolazioneGetFoldersDocument(idDocumento);

                if (retValue == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;

            //DocsPaWR.Fascicolo[] result = docsPaWS.GetFascicoloDaCodiceNoSecurity(projectCode, idAdm, idClassificationScheme, onlyGenerals);
            //DocsPaWR.Folder[] result = null;
            //return result;
        }

        public static DocsPaWR.FascicolazioneClassifica[] getGerarchia(Page page, string idClassificazione, string idAmm)
        {
            try
            {
                DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetGerarchia(idClassificazione, idAmm);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Fascicolo getFascicoloInClassifica(Page page, string codFascicolo, string idRegistro, string idTitolario, string systemId)
        {
            DocsPaWR.Fascicolo result = null;

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                bool enableUfficioRef = (Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString())) != null
                    && Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString()).Equals("1");

                //bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                result = docsPaWS.FascicolazioneGetFascicoloInClassifica(infoUtente, codFascicolo, idRegistro, enableUfficioRef, idTitolario, enableProfilazione, systemId);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static Folder getFolder(Page page, Fascicolo fasc)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                Folder result = docsPaWS.FascicolazioneGetFolder(infoUtente.idPeople, infoUtente.idGruppo, fasc);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Folder getFolder(Page page, Folder fold)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                Folder result = docsPaWS.FascicolazioneGetFolderAndChild(infoUtente.idPeople, infoUtente.idGruppo, fold);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Folder getFolder(Page page, string idFolder)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                Folder result = docsPaWS.FascicolazioneGetFolderById(infoUtente.idPeople, infoUtente.idGruppo, idFolder);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //catch (System.Web.Services.Protocols.SoapException es)
            //{
            //    ErrorManager.redirect(page, es);
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void removeFolderSelezionato(Page page)
        {
            try
            {
                removeFolderSelezionato();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void removeFolderSelezionato()
        {
            try
            {
                HttpContext.Current.Session.Remove("fascDocumenti.FolderSel");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.ValidationResultInfo deleteDocFromProject(Page page, DocsPaWR.Folder folder, string idProfile, string fascRapida, DocsPaWR.Fascicolo fasc, out string msg)
        {
            DocsPaWR.ValidationResultInfo result = null;
            msg = string.Empty;
            try
            {
                result = docsPaWS.FascicolazioneDeleteDocFromProject(UserManager.GetInfoUser(), idProfile, folder, fascRapida, fasc, out msg);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }




        //Laura 12 Marzo
        public static DocsPaWR.Fascicolo getFascicoloSelezionatoFascRapida(Page page)
        {
            return (DocsPaWR.Fascicolo)getFascicoloSelezionatoFascRapida();
        }


        public static DocsPaWR.Fascicolo getFascicoloSelezionatoFascRapida()
        {
            return (DocsPaWR.Fascicolo)HttpContext.Current.Session["FascicoloSelezionatoFascRapida"];
        }

        public static DocsPaWR.Folder getFolderSelezionato(Page page)
        {
            try
            {
                return (DocsPaWR.Folder)HttpContext.Current.Session["fascDocumenti.FolderSel"];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Fascicolo getFascicoloById(Page page, string idFascicolo)
        {
            DocsPaWR.Fascicolo result = null;

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                result = docsPaWS.FascicolazioneGetFascicoloById(idFascicolo, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// restituisce il fascicolo 
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static Fascicolo getFascicoloById(string idFascicolo, InfoUtente infoUtente)
        {
            DocsPaWR.Fascicolo result = null;

            try
            {
                result = docsPaWS.FascicolazioneGetFascicoloById(idFascicolo, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //Laura 13 Marzo
        private static void SetSessionValue(string sessionKey, object value)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = value;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.Folder[] getListaFolderDaCodiceFascicolo(Page page, string codFascicolo, string descrFolder, DocsPaWR.Registro reg)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                //bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
                bool enableUfficioRef = (Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString())) != null
                    && Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString()).Equals("1");

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Folder[] result = docsPaWS.FascicolazioneGetListaFolderDaCodice(infoUtente, codFascicolo, descrFolder, reg, enableUfficioRef, enableProfilazione);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setCodiceFascRapida(Page page, string codiceFascRapida)
        {
            try
            {
                page.Session["CodiceFascRapida"] = codiceFascRapida;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setDescrizioneFascRapida(Page page, string descrizioneFascRapida)
        {
            try
            {
                page.Session["DescrizioneFascRapida"] = descrizioneFascRapida;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.Fascicolo getFascicoloDaCodice(Page page, string codFascicolo)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                //bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableUfficioRef = (Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString()) != null
                   && Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString()).Equals("1"));



                bool enableProfilazione = false;

                if (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()] != null && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice(infoUtente, codFascicolo, RegistryManager.GetRegistryInSession(), enableUfficioRef, enableProfilazione);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Fascicolo getFascicoloDaCodice(Page page, string codFascicolo, string idTitolario)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, codFascicolo, UserManager.getRegistroSelezionato(page), enableUfficioRef, enableProfilazione, idTitolario);

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPaWR.Fascicolo[] getListaFascicoliDaCodice(Page page, string codFascicolo, DocsPaWR.Registro reg, string insRic)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                //bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableUfficioRef = (Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString()) != null
                    && Utils.InitConfigurationKeys.GetValue("0", DBKeys.ENABLE_UFFICIO_REF.ToString()).Equals("1"));

                bool enableProfilazione = false;

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo[] result = docsPaWS.FascicolazioneGetListaFascicoliDaCodice(infoUtente, codFascicolo, reg, enableUfficioRef, enableProfilazione, insRic);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce un'elenco di tipologie utilizzazbili da un ruolo
        /// </summary>
        /// <param name="_idAmministrazione"></param>
        /// <param name="_idGruppo"></param>
        /// <param name="_Diritti"></param>
        /// <returns></returns>
        public static Templates[] getTipologiaFascicoloByRuolo(string _idAmministrazione, string _idGruppo, string _Diritti)
        {
            Templates[] template = null;
            try
            {
                template = docsPaWS.getTipoFascFromRuolo(_idAmministrazione, _idGruppo, _Diritti);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return template;
        }

        /// <summary>
        /// Creazione di un nuovo fascicolo
        /// </summary>
        /// <param name="classificazione"></param>
        /// <param name="fascicolo"></param>
        /// <param name="infoutente"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public static Fascicolo newFascicolo(FascicolazioneClassificazione classificazione, Fascicolo fascicolo, DocsPaWR.InfoUtente infoutente, DocsPaWR.Ruolo ruolo)
        {
            Fascicolo result = null;
            try
            {
                bool enableUfficioRef = false;
                //enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                //Laura 9 Aprile commentato perchè nel caso in cui la chiave non è presente nella ricerca rischio di non avere risultati
                //ABBATANGELI GIANLUIGI - Se non trovo la chiave di configurazione CODICE_APPLICAZIONE, imposto il codice applicazione di default a ____
                //if (string.IsNullOrEmpty(fascicolo.codiceApplicazione))
                //    fascicolo.codiceApplicazione = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);

                if (string.IsNullOrEmpty(fascicolo.codiceApplicazione))
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]))
                        fascicolo.codiceApplicazione = System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString();

                //ABBATANGELI GIANLUIGI - TEST
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                ResultCreazioneFascicolo resultcreazione;
                result = docsPaWS.FascicolazioneNewFascicolo(classificazione, fascicolo, infoutente, ruolo, enableUfficioRef, out resultcreazione);
                setEsitoCreazioneFascicolo(resultcreazione.ToString());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// setta la varibile di sessione per l'esito della creazione del fascicolo
        /// </summary>
        /// <param name="codice"></param>
        private static void setEsitoCreazioneFascicolo(string codice)
        {
            try
            {
                HttpContext.Current.Session.Add("ESITOCREAZIONEFASCICOLO", codice);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// restituisce l'esito della creazione del fascicolo
        /// </summary>
        /// <returns></returns>
        public static string getEsitoCreazioneFascicolo()
        {
            try
            {
                string esito = HttpContext.Current.Session["ESITOCREAZIONEFASCICOLO"].ToString();
                HttpContext.Current.Session.Remove("ESITOCREAZIONEFASCICOLO");
                return esito;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce il nodo per consentire l'inserimento del fascicolo
        /// </summary>
        /// <param name="_idAmministrazione"></param>
        /// <param name="_idGruppo"></param>
        /// <param name="registro"></param>
        /// <param name="_codClassifica"></param>
        /// <param name="_getFigli"></param>
        /// <param name="_idPeople"></param>
        /// <param name="_idTitolario"></param>
        /// <returns></returns>
        public static FascicolazioneClassificazione[] getFascicolazioneClassificazione(string _idAmministrazione,
            string _idGruppo, string _idPeople, Registro registro, string _codClassifica, bool _getFigli, string _idTitolario)
        {
            DocsPaWR.FascicolazioneClassificazione[] result = null;
            try
            {
                result = docsPaWS.FascicolazioneGetTitolario2(_idAmministrazione,
                    _idGruppo, _idPeople,
                    registro, _codClassifica, _getFigli, _idTitolario);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// restituisce il numero del fascicolo
        /// </summary>
        /// <param name="_idNodoSelezionato"></param>
        /// <param name="_idRegistro"></param>
        /// <returns></returns>
        public static string getNumeroFascicolo(string _idNodoSelezionato, string _idRegistro)
        {
            try
            {
                return docsPaWS.FascicolazioneGetFascNumRif(_idNodoSelezionato, _idRegistro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

        }

        /// <summary>
        /// restituisce il fascicolo creato
        /// </summary>
        /// <returns></returns>
        public static Fascicolo getProjectInSession()
        {
            try
            {
                return (Fascicolo)HttpContext.Current.Session["FascicoloCreato"];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// setat il fascicolo creato
        /// </summary>
        /// <param name="fascicolo"></param>
        public static void setProjectInSession(Fascicolo fascicolo)
        {
            try
            {
                HttpContext.Current.Session["FascicoloCreato"] = fascicolo;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);

            }
        }

        //Laura 19 Marzo
        public static void setProjectInSessionForRicFasc(string codClassifica)
        {
            try
            {
                HttpContext.Current.Session["ProjectForRicFasc"] = codClassifica;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);

            }
        }


        public static string getProjectInSessionForRicFasc()
        {
            try
            {
                if (HttpContext.Current.Session["ProjectForRicFasc"] != null)
                    return HttpContext.Current.Session["ProjectForRicFasc"].ToString();
                else
                    return "";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        //Fine Laura 19 Marzo
        /// <summary>
        /// rimuove dalla sessione il fascicolo creato
        /// </summary>
        public static void removeProjectInSession()
        {
            try
            {
                HttpContext.Current.Session.Remove("FascicoloCreato");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// salva le modifiche del fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns>null se non è avvenuto il salvataggio del fascicolo</returns>
        public static Fascicolo setFascicolo(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            try
            {
                if (!docsPaWS.FascicolazioneSetFascicolo(infoUtente, ref fascicolo))
                {
                    fascicolo = null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return fascicolo;
        }

        /// <summary>
        /// cancella un fascicolo e tutti i suoi figli
        /// </summary>
        /// <param name="infoutente"></param>
        /// <param name="fascicolo"></param>
        public static bool deleteFascicoloAndFolder(Fascicolo fascicolo, InfoUtente infoutente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.FascicolazioneEliminaFascicolo(fascicolo, infoutente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// aggiunge un fascicolo in Area di Lavoro
        /// </summary>
        /// <param name="infoutente"></param>
        /// <param name="fascicolo"></param>
        public static bool addFascicoloInAreaDiLavoro(Fascicolo fascicolo, InfoUtente infoutente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoExecAddLavoro(null, null, fascicolo, infoutente, (fascicolo.idRegistroNodoTit != null ? fascicolo.idRegistroNodoTit : ""));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// rimuovi il fascicolo dall'aria di lavoro
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool eliminaFascicoloDaAreaDiLavoro(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoCancellaAreaLavoro(infoUtente.idPeople, infoUtente.idCorrGlobali, null, fascicolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static Fascicolo getFascicoloById(string idFascicolo)
        {
            try
            {
                DocsPaWR.Fascicolo result = null;
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                result = docsPaWS.FascicolazioneGetFascicoloById(idFascicolo, infoUtente);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


            public static Fascicolo getFascicoloByIdNoSecurity(string idFascicolo)
        {
            try
            {
                DocsPaWR.Fascicolo result = null;
                InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                result = docsPaWS.FascicolazioneGetFascicoloByIdNoSecurity(idFascicolo);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        

        public static string[] getListaFascicoliPagingCustomRicVelolce(InfoUtente infoUtente, FascicolazioneClassificazione classificazione, DocsPaWR.Registro registro, FiltroRicerca[] filtriRicerca, bool childs, int requestedPage, out int numTotPage, out int nRec, int pageSize, bool getSystemIdList, out SearchResultInfo[] idProjectList, byte[] datiExcel, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security)
        {
            nRec = 0;
            numTotPage = 0;

            string[] result = null;
            // Lista dei system id dei fascicoli
            SearchResultInfo[] idProjects = null;

            try
            {
                //bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                //    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
                bool enableUfficioRef = true; //DA RIVEDERE CHIAVE NON PRESENTE 

                bool enableProfilazione = false;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
                {
                    enableProfilazione = true;
                }

                SearchObject[] value = docsPaWS.FascicolazioneGetListaFascicoliPagingCustom(infoUtente, classificazione, registro, filtriRicerca, enableUfficioRef, enableProfilazione, childs, requestedPage, pageSize, getSystemIdList, datiExcel, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security, out numTotPage, out nRec, out idProjects);

                if (value != null && value.Length > 0)
                    result = new string[value.Length];
                {
                    int i = 0;
                    foreach (SearchObject prj in value)
                    {
                        string description = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;
                        string code = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
                        result[i] = description + " (" + code + ")";
                        i++;
                    }
                }

                if (result == null)
                {
                    //throw new Exception();
                }

                // Salvataggio della lista dei system id dei fascicoli
                idProjectList = idProjects;


                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            idProjectList = null;
            return null;
        }

        public static FiltroRicerca[] addToArrayFiltroRicerca(FiltroRicerca[] array, FiltroRicerca nuovoElemento)
        {
            try
            {
                FiltroRicerca[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new FiltroRicerca[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new FiltroRicerca[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce una folder
        /// </summary>
        /// <param name="idFolder"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static Folder getFolder(string idFolder, InfoUtente infoUtente)
        {
            Folder _folder = null;
            try
            {
                _folder = docsPaWS.FascicolazioneGetFolderById(infoUtente.idPeople, infoUtente.idGruppo, idFolder);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return _folder;
        }

        /// <summary>
        /// rimuove un documento dal fasciolo selezionato
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="fascRapida"></param>
        /// <returns></returns>
        public static ValidationResultInfo deleteDocFromFolder(Folder folder, InfoUtente infoUtente, string idProfile, string fascRapida, out string msg)
        {
            ValidationResultInfo result = null;
            msg = string.Empty;
            try
            {
                result = docsPaWS.FascicolazioneDeleteDocFromFolder(infoUtente, idProfile, folder, fascRapida, out msg);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static bool InsertDocumentInStorageArea(String idProfile, SchedaDocumento selectedDocument, InfoUtente infoUtente)
        {
            try
            {
                // Eventuale messaggio da restituire
                bool result = false;
                // Se si sta creando la prima istanza di conservazione...
                if (DocumentManager.isPrimaConservazione(
                    infoUtente.idPeople,
                    infoUtente.idGruppo) == 1)
                {
                    // Viene visualizzato un messaggio all'utente
                    // toReturn = "E' stata creata una nuova istanza di conservazione.";
                }
                int conservationAreaId = DocumentManager.addAreaConservazione(
                   idProfile,
                    string.Empty,
                    selectedDocument.docNumber,
                    infoUtente,
                    "D");

                // Se l'identificativo non è -1 vengono aggiornati i dati sulla conservazione
                if (conservationAreaId > -1)
                {
                    int size_xml = DocumentManager.getItemSize(
                        selectedDocument,
                        conservationAreaId.ToString());

                    int doc_size = int.Parse(selectedDocument.documenti[0].fileSize);

                    int numeroAllegati = selectedDocument.allegati.Length;
                    string fileName = selectedDocument.documenti[0].fileName;
                    string tipoFile = Path.GetExtension(fileName);
                    int size_allegati = 0;
                    for (int i = 0; i < selectedDocument.allegati.Length; i++)
                        size_allegati = size_allegati + int.Parse(selectedDocument.allegati[i].fileSize);

                    int total_size = size_allegati + doc_size + size_xml;

                    if (DocumentManager.insertSizeInItemCons(conservationAreaId.ToString(), total_size))
                    {
                        result = DocumentManager.updateItemsConservazione(
                            tipoFile,
                           numeroAllegati.ToString(),
                           conservationAreaId.ToString());
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Inserimento di un singolo documento in area conservazione nel rispetto dei vincoli della dimensione istanze
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="selectedDocument"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool InsertDocumentInStorageArea_WithConstraint(String idProfile, SchedaDocumento selectedDocument, InfoUtente infoUtente)
        {
            try
            {
                // Eventuale messaggio da restituire
                bool result = false;
                // Se si sta creando la prima istanza di conservazione...
                if (DocumentManager.isPrimaConservazione(
                    infoUtente.idPeople,
                    infoUtente.idGruppo) == 1)
                {
                    // Viene visualizzato un messaggio all'utente
                    // toReturn = "E' stata creata una nuova istanza di conservazione.";
                }

                // Controllo Rispetto dei Vincoli dell'istanza
                #region Vincoli Istanza di Conservazione
                // Variabili di controllo per violazione dei vincoli sulle istanze
                bool numDocIstanzaViolato = false;
                bool dimIstanzaViolato = false;
                int TotalSelectedDocumentSize = 0;
                int currentDimOfDocs = 0;
                int indexOfCurrentDoc = 0;

                // Get valori limiti per le istanze di conservazione
                int DimMaxInIstanza = 0;
                int numMaxDocInIstanza = 0;
                int TolleranzaPercentuale = 0;
                try
                {
                    InfoUtente infoUt = UserManager.GetInfoUser();
                    DimMaxInIstanza = DocumentManager.getDimensioneMassimaIstanze(infoUt.idAmministrazione);
                    numMaxDocInIstanza = DocumentManager.getNumeroDocMassimoIstanze(infoUt.idAmministrazione);
                    TolleranzaPercentuale = DocumentManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
                }
                catch (Exception ex)
                {
                }

                TotalSelectedDocumentSize = DocumentManager.GetTotalDocumentSize(selectedDocument);
                // Dimensione documenti raggiunta
                currentDimOfDocs = TotalSelectedDocumentSize + currentDimOfDocs;
                // Numero di documenti raggiunti
                indexOfCurrentDoc = indexOfCurrentDoc + 1;

                numDocIstanzaViolato = DocumentManager.isVincoloNumeroDocumentiIstanzaViolato(indexOfCurrentDoc, numMaxDocInIstanza);
                dimIstanzaViolato = DocumentManager.isVincoloDimensioneIstanzaViolato(currentDimOfDocs, DimMaxInIstanza, TolleranzaPercentuale);

                double DimensioneMassimaConsentitaPerIstanza = 0;
                DimensioneMassimaConsentitaPerIstanza = DimMaxInIstanza - ((DimMaxInIstanza * TolleranzaPercentuale) / 100);

                int DimMaxConsentita = 0;
                DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

                if (numDocIstanzaViolato || dimIstanzaViolato)
                {
                    // Azzero le due variabili
                    currentDimOfDocs = 0;
                    indexOfCurrentDoc = 0;
                }
                #endregion

                int conservationAreaId = DocumentManager.addAreaConservazione_WithConstraint(
                   idProfile,
                    string.Empty,
                    selectedDocument.docNumber,
                    infoUtente,
                    "D",
                    numDocIstanzaViolato,
                    dimIstanzaViolato,
                    DimMaxConsentita,
                    numMaxDocInIstanza,
                    TotalSelectedDocumentSize
                    );

                // Se l'identificativo non è -1 vengono aggiornati i dati sulla conservazione
                if (conservationAreaId > -1)
                {
                    int size_xml = DocumentManager.getItemSize(
                        selectedDocument,
                        conservationAreaId.ToString());

                    int doc_size = int.Parse(selectedDocument.documenti[0].fileSize);

                    int numeroAllegati = selectedDocument.allegati.Length;
                    string fileName = selectedDocument.documenti[0].fileName;
                    string tipoFile = Path.GetExtension(fileName);
                    int size_allegati = 0;
                    for (int i = 0; i < selectedDocument.allegati.Length; i++)
                        size_allegati = size_allegati + int.Parse(selectedDocument.allegati[i].fileSize);

                    int total_size = size_allegati + doc_size + size_xml;

                    if (DocumentManager.insertSizeInItemCons(conservationAreaId.ToString(), total_size))
                    {
                        result = DocumentManager.updateItemsConservazione(
                            tipoFile,
                           numeroAllegati.ToString(),
                           conservationAreaId.ToString());
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool RemoveDocumentFromStorageArea(SchedaDocumento documento, InfoUtente infoutente)
        {
            // Eventuale messaggio da mostrare
            bool result = false;
            try
            {
                if (DocumentManager.canDeleteAreaConservazione(
                    documento.systemId,
                    infoutente.idPeople,
                    infoutente.idGruppo))
                {
                    result = DocumentManager.eliminaDaAreaConservazione(
                             documento.systemId,
                             null,
                             null,
                             false,
                             String.Empty);
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// restituisce la tipologia dei fascicoli dato un ruolo
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="idRuolo"></param>
        /// <param name="diritti"></param>
        /// <returns></returns>
        public static Templates[] getTipoFascFromRuolo(string idAmministrazione, string idRuolo, string diritti)
        {
            Templates[] result = null;
            try
            {
                result = docsPaWS.getTipoFascFromRuolo(idAmministrazione, idRuolo, diritti);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static Templates getTemplateFascById(string idTemplate, InfoUtente infoutente)
        {
            Templates template = null;
            try
            {
                template = docsPaWS.getTemplateFascById(idTemplate);
                if (template != null && template.IPER_FASC_DOC.Equals("1"))
                    template = ProjectManager.getTemplateFascCampiComuniById(idTemplate, infoutente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return template;
        }

        /// <summary>
        /// Se la tipologia è di campi comuni (Iperfascicolo) richiamo il metodo che mi restituisce il temmplate
        /// affinchè vengano visualizzati solo i campi comuni sui quali si ha visibilità rispetto alle tipologie
        /// di fascicolo associate al ruolo
        /// </summary>
        /// <param name="idTemplate"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static Templates getTemplateFascCampiComuniById(string idTemplate, InfoUtente infoutente)
        {
            Templates template = null;
            try
            {
                template = docsPaWS.getTemplateFascCampiComuniById(infoutente, idTemplate);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return template;
        }

        public static AssDocFascRuoli[] getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate)
        {
            AssDocFascRuoli[] result = null;
            try
            {
                result = docsPaWS.getDirittiCampiTipologiaFasc(idRuolo, idTemplate);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static bool CheckRevocationAcl()
        {
            return ProjectManager.CheckRevocationAcl(UIManager.ProjectManager.getProjectInSession());
        }

        public static bool CheckRevocationAcl(Fascicolo Prj)
        {
            try
            {
                if (Prj.systemID != null && !string.IsNullOrEmpty(Prj.systemID))
                {
                    DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                    // Il metodo verificaACL viene chiamato anche per acl dei fascicoli.
                    string errorMessage = "";
                    int result = DocumentManager.verifyDocumentACL("F", Prj.systemID, UserManager.GetInfoUser(), out errorMessage);

                    // Imposta lo stato della revoca dell'acl
                    // ************************************
                    // *  CASI:
                    // *  result = 0 --> ACL rimossa
                    // *  result = 1 --> documento rimosso
                    // *  result =2 --> documento "normale"
                    // *  result -1 --> errore generico
                    // **************************************
                    return (result == 0);
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static FascicoloDiritto[] getListaVisibilitaSemplificata(InfoFascicolo infoFasc, bool cercaRimossi, string rootFolder)
        {
            try
            {
                FascicoloDiritto[] result = null;
                result = docsPaWS.FascicolazioneGetVisibilitaSemplificata(infoFasc, cercaRimossi, rootFolder);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetRootFolderFasc(Fascicolo fasc)
        {
            try
            {
                string result = string.Empty;
                result = docsPaWS.getRootFolderFasc(fasc.systemID);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static InfoFascicolo getInfoFascicoloDaFascicolo(Fascicolo fasc)
        {
            try
            {
                InfoFascicolo infoFasc = new InfoFascicolo();

                infoFasc.idFascicolo = fasc.systemID;
                infoFasc.descrizione = fasc.descrizione;
                infoFasc.idClassificazione = fasc.idClassificazione;
                infoFasc.codice = fasc.codice;
                // non so dove trova queste info ?
                // infoFasc.codRegistro =
                // infoFasc.idRegistro = 	

                if (fasc.stato != null && fasc.stato.Equals("A"))
                {
                    infoFasc.apertura = fasc.apertura;
                }

                return infoFasc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Tab GetProjectTab(string projectId, DocsPaWR.InfoUtente infoUser)
        {
            try
            {
                return docsPaWS.GetProjectTab(projectId, infoUser);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Stato getStatoFasc(Fascicolo prj)
        {
            try
            {
                return docsPaWS.getStatoFasc(prj.systemID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void updateFolder(Page page, Folder folder)
        {
            try
            {
                bool result = docsPaWS.FascicolazioneModifyFolder(UserManager.GetInfoUser(), folder);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return;
        }

        public static bool CanRemoveFascicolo(Page page, string project_Id, out string nFasc)
        {
            bool result = false;
            nFasc = "";

            try
            {
                result = docsPaWS.FascicolazioneCanRemoveFascicolo(project_Id, out nFasc);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool CanRemoveFascicoloPrincipale(Page page, string project_Id, out string nDoc)
        {
            bool result = false;
            nDoc = "";
            try
            {
                result = docsPaWS.FascicolazioneCanRemoveFascicoloPrincipale(project_Id, out nDoc);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static void delFolder(Page page, Folder fold)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                bool result = docsPaWS.FascicolazioneDelFolder(fold, infoUtente);

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DettItemsConservazione[] getStoriaConsFasc(string idProject)
        {
            try
            {
                DettItemsConservazione[] dettItemsCons = null;

                dettItemsCons = docsPaWS.gettDettaglioConsFasc(idProject);
                if (dettItemsCons == null)
                    throw new Exception();
                return dettItemsCons;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool newFolder(Page page, ref Folder folder, InfoUtente infoUtente, Ruolo ruolo, out DocsPaWR.ResultCreazioneFolder result)
        {
            result = ResultCreazioneFolder.GENERIC_ERROR;

            try
            {
                folder = docsPaWS.FascicolazioneNewFolder(folder, infoUtente, ruolo, out result);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return (result == ResultCreazioneFolder.OK);
        }

        public static Folder[] getFolderByDescrizione(Page page, string idFascicolo, string descFolder)
        {
            Folder[] result = null;
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                result = docsPaWS.FascicolazioneGetFolderByDescr(infoUtente.idPeople, infoUtente.idGruppo, idFascicolo, descFolder);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool GetIfDocumentiCountVisibleIsEgualNotVisible(Page page, Folder folder)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                bool result = docsPaWS.FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisible(infoUtente, folder);
                if (result && folder.childs.Length > 0)
                {
                    foreach (Folder subfolder in folder.childs)
                    {
                        Folder subfolder2 = ProjectManager.getFolder(page, subfolder);
                        bool subresult = docsPaWS.FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisible(infoUtente, subfolder2);
                        if (!subresult)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        internal static void MoveFolder(Page page, string folderId, string parentId)
        {
            try
            {
                docsPaWS.FascicolazioneMoveFolder(folderId, parentId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static SearchObject[] getListaFascicoliPagingCustom(FascicolazioneClassificazione classificazione, Registro registro, FiltroRicerca[] filtriRicerca, bool childs, int requestedPage, out int numTotPage, out int nRec, int pageSize, bool getSystemIdList, out SearchResultInfo[] idProjectList, byte[] datiExcel, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security)
        {
            nRec = 0;
            numTotPage = 0;

            // Lista dei system id dei fascicoli
            SearchResultInfo[] idProjects = null;

            try
            {
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamicaFasc.ToString()] == "1")
                {
                    enableProfilazione = true;
                }

                InfoUtente infoUtente = UserManager.GetInfoUser();
                SearchObject[] result = docsPaWS.FascicolazioneGetListaFascicoliPagingCustom(infoUtente, classificazione, registro, filtriRicerca, enableUfficioRef, enableProfilazione, childs, requestedPage, pageSize, getSystemIdList, datiExcel, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security, out numTotPage, out nRec, out idProjects);

                if (result == null)
                {
                    //throw new Exception();
                }

                // Salvataggio della lista dei system id dei fascicoli
                idProjectList = idProjects;

                return result;
            }
            catch (Exception es)
            {
                UIManager.AdministrationManager.DiagnosticError(es);
            }

            idProjectList = null;

            return null;
        }

        //Laura 13 Marzo
        public static void setFiltroRicFasc(Page page, FiltroRicerca[][] filtroRicerca)
        {
            try
            {
                page.Session["ricFasc.listaFiltri"] = filtroRicerca;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            //SetSessionValue("ricFasc.listaFiltri", filtroRicerca);
        }



        public static FiltroRicerca[][] getFiltriRicFasc(Page page)
        {
            try
            {
                return (FiltroRicerca[][])GetSessionValue("ricFasc.listaFiltri");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            //SetSessionValue("ricFasc.listaFiltri", filtroRicerca);
        }


        private static Object GetSessionValue(string sessionKey)
        {
            return System.Web.HttpContext.Current.Session[sessionKey];
        }



        public static void setClassificazioneSelezionata(Page page, FascicolazioneClassificazione classificazioneSelezionata)
        {
            //page.Session["ClassificazioneSelezionata"] = classificazioneSelezionata;
            SetSessionValue("ClassificazioneSelezionata", classificazioneSelezionata);
        }
        public static FascicolazioneClassificazione getClassificazioneSelezionata(Page page)
        {
            //return (FascicolazioneClassificazione)page.Session["ClassificazioneSelezionata"];
            return (FascicolazioneClassificazione)GetSessionValue("ClassificazioneSelezionata");
        }

        //Laura 13 Marzo
        public static Hashtable getHashFascicoli(Page page)
        {
            return (Hashtable)page.Session["docClassifica.HashFascicoli"];
        }

        public static void setHashFascicoli(Page page, Hashtable fascicoli)
        {
            page.Session["docClassifica.HashFascicoli"] = fascicoli;
        }

        //Laura 19 Marzo
        public static void removeHashFascicoli(Page page)
        {
            page.Session.Remove("docClassifica.HashFascicoli");
        }

        public static Fascicolo[] getListaFascicoliPaging(Page page, FascicolazioneClassificazione classificazione, DocsPaWR.Registro registro, FiltroRicerca[] filtriRicerca, bool childs, int requestedPage, out int numTotPage, out int nRec, int pageSize,
            bool getSystemIdList, out SearchResultInfo[] idProjectList, byte[] datiExcel)
        {
            nRec = 0;
            numTotPage = 0;

            // Lista dei system id dei fascicoli
            SearchResultInfo[] idProjects = null;

            try
            {
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                InfoUtente infoUtente = UserManager.GetInfoUser();
                Fascicolo[] result = docsPaWS.FascicolazioneGetListaFascicoliPaging(infoUtente, classificazione, registro, filtriRicerca,
                                                enableUfficioRef, enableProfilazione, childs, requestedPage, pageSize, getSystemIdList, datiExcel, out numTotPage, out nRec, out idProjects);

                if (result == null)
                {
                    //throw new Exception();
                }

                // Salvataggio della lista dei system id dei fascicoli
                idProjectList = idProjects;

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			}
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            idProjectList = null;

            return null;
        }



        public static string getFirstDayOfWeek()
        {
            return docsPaWS.getFirstDayOfWeek();
        }

        public static string getLastDayOfWeek()
        {
            return docsPaWS.getLastDayOfWeek();
        }

        public static string getFirstDayOfMonth()
        {
            return docsPaWS.getFirstDayOfMonth();
        }

        public static string getLastDayOfMonth()
        {
            return docsPaWS.getLastDayOfMonth();
        }

        public static string toDay()
        {
            return docsPaWS.toDay();
        }

        public static DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario2(Page page, string codClassifica, bool getFigli, string idTitolario)
        {
            try
            {

                InfoUtente infoUtente = UserManager.GetInfoUser();
                Registro registro = UserManager.getRegistroSelezionato(page);

                DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, codClassifica, getFigli, idTitolario);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario2(Page page, string codClassifica, DocsPaWR.Registro regNodo, bool getFigli, string idTitolario)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();

                DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, regNodo, codClassifica, getFigli, idTitolario);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }



        //Laura 19 Marzo
        public static void setFascicoloSelezionatoFascRapida(DocsPaWR.Fascicolo fascicoloSelezionato)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["FascicoloSelezionatoFascRapida"] = fascicoloSelezionato;
        }


        public static void setFascicoloSelezionatoFascRapida(Page page, DocsPaWR.Fascicolo fascicoloSelezionato)
        {
            setFascicoloSelezionatoFascRapida(fascicoloSelezionato);
        }


        public static void removeFascicoloSelezionatoFascRapida()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("FascicoloSelezionatoFascRapida");
        }

        public static void removeFascicoloSelezionatoFascRapida(Page page)
        {
            removeFascicoloSelezionatoFascRapida();
        }


        public static Hashtable getHashFascicoliSelezionati(Page page)
        {
            return page.Session["docClassifica.HashFascicoliSelezionati"] as Hashtable;
        }

        public static void setHashFascicoliSelezionati(Page page, Hashtable fascicoliSelezionati)
        {
            page.Session["docClassifica.HashFascicoliSelezionati"] = fascicoliSelezionati;
        }

        public static Fascicolo GetFascicolo(InfoFascicolo info)
        {
            try
            {

                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
                Fascicolo result = docsPaWS.FascicolazioneGetDettaglioFascicolo(UserManager.GetInfoUser(), info, enableUfficioRef);
                return result;
            }

            catch (Exception es)
            {
                return null;
            }
        }

        public static DocsPaWR.DocumentoLogDocumento[] getStoriaLog(string idOggetto, string varOggetto)
        {
            try
            {
                DocsPaWR.DocumentoLogDocumento[] result = docsPaWS.DocumentoGetListaLog(idOggetto, varOggetto);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        public static SearchObject[] getListaDocumentiPagingCustom(
                        Folder folder,
                        DocsPaWR.FiltroRicerca[][] filtriRicerca,
                        int numPage,
                        out int numTotPage,
                        out int nRec,
                        bool compileIdProfileList,
                        bool showGridPersonalization,
                        bool export,
                        Field[] visibleFieldsTemplate,
                        String[] documentsSystemId,
                        int pageSize,
                        DocsPaWR.FiltroRicerca[][] filtriRicercaOrdinamento,
                        out SearchResultInfo[] idProfiles)
        {
            nRec = 0;
            numTotPage = 0;

            SearchResultInfo[] idProfileList = null;

            SearchObject[] retValue = null;

            try
            {
                retValue = docsPaWS.FascicolazioneGetDocumentiPagingWithFiltersCustom(UIManager.UserManager.GetInfoUser(), folder, filtriRicerca, numPage, compileIdProfileList, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, pageSize, filtriRicercaOrdinamento, out numTotPage, out nRec, out idProfileList);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            idProfiles = idProfileList;

            return retValue;
        }

        public static string GetAccessRightFascBySystemID(string idFasc)
        {
            try
            {
                return docsPaWS.getAccessRightFascBySystemID(idFasc, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        public static void removeFiltroRicFasc(Page page)
        {
            RemoveSessionValue("ricFasc.listaFiltri");
            RemoveSessionValue("classificaSelezionata");
            RemoveSessionValue("filtroProfDinamica");
        }

        //Laura 25 Marzo
        public static int getCountDocumentiInFolder(DocsPaWR.Folder fasc)
        {
            int numDoc = 0;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            SearchResultInfo[] idProfileList = null;
            numDoc = docsPaWS.getCountDocumentiInFolderCustom(infoUtente, fasc, null, out idProfileList);

            return numDoc;
        }

        public static Fascicolo getFascicolo(Page page, string idFascicolo)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                Fascicolo result = docsPaWS.FascicolazioneGetFascicoloById(idFascicolo, infoUtente);

                if (result == null)
                {
                    ScriptManager.RegisterStartupScript(page, page.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + page.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPaWR.FascicolazioneClassifica[] getGerarchiaDaCodice2(Page page, string codClassificazione, string idAmm, string idTitolario, Registro reg)
        {
            try
            {
                DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetGerarchiaDaCodice2(codClassificazione, reg, idAmm, idTitolario);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

          
        }

        public static string[] getIdDocumentiFromFascicolo(string idProject)
        {
            try
            {
                SearchResultInfo[] fasc = docsPaWS.getDocumentiInFascicolo(idProject);
                string[] result = new string[fasc.Length];
                for (int i = 0; i < fasc.Length; i++)
                {
                    result[i] = Convert.ToString(fasc[i].Id);
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void addFascicoloInAreaDiLavoro(Page page, DocsPaWR.Fascicolo fascicolo)
        {
            try
            {

                bool result = docsPaWS.DocumentoExecAddLavoro(null, null, fascicolo, UserManager.GetInfoUser(), (fascicolo.idRegistroNodoTit != null ? fascicolo.idRegistroNodoTit : ""));

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void eliminaFascicoloDaAreaDiLavoro(Page page, DocsPaWR.Fascicolo fascicolo)
        {
            try
            {
                Utente utente = UserManager.GetUserInSession();
                Ruolo ruolo = RoleManager.GetRoleInSession();
                InfoUtente infoUtente = UserManager.GetInfoUser();
                bool result = docsPaWS.DocumentoCancellaAreaLavoro(infoUtente.idPeople, infoUtente.idCorrGlobali, null, fascicolo);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static int isFascInADLRole(string idProject, Page page)
        {
            try
            {
                int retValue = 0;
                InfoUtente infoUtente = UserManager.GetInfoUser();
                retValue = docsPaWS.isFascInADLRole(idProject, infoUtente.idCorrGlobali);
                return retValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        /// <summary>
        /// aggiunge un fascicolo in Area di Lavoro ruolo
        /// </summary>
        /// <param name="infoutente"></param>
        /// <param name="fascicolo"></param>
        public static bool addFascicoloInAreaDiLavoroRole(Fascicolo fascicolo, InfoUtente infoutente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoExecAddLavoroRole(null, null, fascicolo, infoutente, (fascicolo.idRegistroNodoTit != null ? fascicolo.idRegistroNodoTit : ""));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// rimuovi il fascicolo dall'aria di lavoro role
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool eliminaFascicoloDaAreaDiLavoroRole(Fascicolo fascicolo, InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.DocumentoCancellaAreaLavoro("0", infoUtente.idCorrGlobali, null, fascicolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        public static FiltroRicerca[][] getFiltroRicFasc()
        {
            return (FiltroRicerca[][])GetSessionValue("ricFasc.listaFiltri");
        }

        public static FiltroRicerca[][] getMemoriaFiltriRicFasc()
        {
            return (FiltroRicerca[][])GetSessionValue("MemoriaFiltriRicFasc");
        }

        public static bool getAllClassValue(Page page)
        {
            bool retValue;
            try
            {
                retValue = (bool)GetSessionValue("AllClassValue");
            }
            catch
            {
                retValue = false;
            }
            return retValue;
        }

        public static DocsPaWR.InfoScarto getIstanzaScarto(Page page)
        {
            return (DocsPaWR.InfoScarto)GetSessionValue("ItemScarto");
        }

        public static void setFascicoloSelezionato(DocsPaWR.Fascicolo fascicoloSelezionato)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["FascicoloSelezionato"] = fascicoloSelezionato;
        }

        /// <summary>
        /// Metodo per verificare se un fascicolo è generale
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="idFascicolo"></param>
        /// <returns>True se il fascicolo è generale.</returns>
        public static bool IsFascicoloGenerale(InfoUtente userInfo, String idFascicolo)
        {
            bool res = false;

            try
            {
                res = docsPaWS.IsFascicoloGenerale(userInfo, idFascicolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return res;
        }


        internal static string getAccessRightFascBySystemID(string p, InfoUtente infoUtente)
        {
            string res = string.Empty;

            try
            {
                res = docsPaWS.getAccessRightFascBySystemID(p, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return res;
        }

        //Lnr 20/05/2013
        public static string getDescrizioneFascRapida(Page page)
        {
            return (string)page.Session["DescrizioneFascRapida"];
        }

        public static bool editingFascACL(DocsPaWR.FascicoloDiritto fascDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EditingFascACL(fascDiritto, personOrGroup, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public static bool ripristinaFascACL(DocsPaWR.FascicoloDiritto fascDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RipristinaFascACL(fascDiritto, personOrGroup, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// restituisce la tipologia dei fascicoli dato un ID AMMINISTRAZIONE versione Archivio
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static Templates[] ARCHIVE_getTipoFasc(string idAmministrazione)
        {
            Templates[] result = null;
            try
            {
                result = docsPaWS.getTipoFasc(idAmministrazione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        //Tatiana 15/05/2013
        public static DocsPaWR.FileDocumento reportTitolario(DocsPaWR.Registro registro)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                DocsPaWR.FileDocumento result = docsPaWS.ReportTitolario(infoUtente, registro);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            catch (Exception es)
            {
                UIManager.AdministrationManager.DiagnosticError(es);
                return null;
            }
        }
        //Tatiana
        public static DocsPaWR.FileDocumento reportFascette(string codiceFacicolo, DocsPaWR.Registro registro)
        {
            try
            {
                if (codiceFacicolo.Equals(""))
                    return null;  // gestire msg errore
                else
                {
                    Fascicolo fascicolo = getFascicoloDaCodice(null, codiceFacicolo);

                    if (fascicolo != null)
                    {
                        return docsPaWS.reportFascetteFascicolo(fascicolo, UserManager.GetInfoUser());
                    }
                    else
                    {//gestire msg errore
                        //page.ClientScript.RegisterStartupScript(page.GetType(), "init", "<script>alert('Fascicolo non trovato');</script>");
                    }
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                UIManager.AdministrationManager.DiagnosticError(es);
                return null;
            }

            return null;
        }
		
		public static bool InsertDescrizioneFascicolo(DocsPaWR.DescrizioneFascicolo descFasc, out ResultDescrizioniFascicolo resultInsertDescrizioniFascicolo)
        {
            resultInsertDescrizioniFascicolo = ResultDescrizioniFascicolo.OK;
            try
            {
                return docsPaWS.InsertDescrizioneFascicolo(descFasc, UserManager.GetInfoUser(), out resultInsertDescrizioniFascicolo);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                UIManager.AdministrationManager.DiagnosticError(es);
                return false;
            }
        }

        public static List<DescrizioneFascicolo> GetListDescrizioniFascicolo(List<FiltroDescrizioniFascicolo> filters, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            numTotPage = 0;
            nRec = 0;
            return docsPaWS.GetListDescrizioniFascicolo(filters.ToArray(), UserManager.GetInfoUser(), numPage, pageSize, out numTotPage, out nRec).ToList();
        }

        public static bool AggiornaDescrizioneFascicolo(DescrizioneFascicolo descFasc, out ResultDescrizioniFascicolo resultUpdateDescrizioniFascicolo)
        {
            resultUpdateDescrizioniFascicolo = ResultDescrizioniFascicolo.OK;
            try
            {
                return docsPaWS.AggiornaDescrizioneFascicolo(descFasc, UserManager.GetInfoUser(), out resultUpdateDescrizioniFascicolo);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                UIManager.AdministrationManager.DiagnosticError(es);
                return false;
            }
        }

        public static bool EliminaDescrizioneFascicolo(string systemId)
        {
            try
            {
                return docsPaWS.EliminaDescrizioneFascicolo(systemId, UserManager.GetInfoUser());
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                UIManager.AdministrationManager.DiagnosticError(es);
                return false;
            }
        }

        public static DocsPaWR.StoricoProfilati[] getStoriaProfilatiFasc(DocsPaWR.Templates template, string idProject)
        {
            try
            {
                DocsPaWR.StoricoProfilati[] result = docsPaWS.FascicoloGetListaStoricoProfilati(template.SYSTEM_ID.ToString(), idProject);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool ExistsTrasmPendenteConWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.ExistsTrasmPendenteConWorkflowFascicolo(idProject, idRuoloInUO, idPeople, infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public static bool ExistsTrasmPendenteSenzaWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople)
        {
            bool result = false;
            InfoUtente infoUtente = UserManager.GetInfoUser();
            try
            {
                return docsPaWS.ExistsTrasmPendenteSenzaWorkflowFascicolo(idProject, idRuoloInUO, idPeople, infoUtente);
            }
            catch (Exception e)
            {

            }

            return result;
        }
    }
}
