// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using BusinessLogic.RubricaComune;
using DocsPaVO.amministrazione;
using DocsPaVO.fascicolazione;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.LibroFirma;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.rubrica;
using DocsPaVO.RubricaComune;
using DocsPaVO.Settings;
using DocsPaVO.utente;
using DocsPaVO.utente.Repertori.RequestAndResponse;
using DocsPaVO.Validations;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Admin.WebApi.Infrastructure.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using ProspettiRiepilogativi;
using Serilog.Context;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaWS
{
    public class DocsPaWS : IDocsPaWS
    {
        private Serilog.ILogger logger = Serilog.Log.ForContext(typeof(DocsPaWS));

        private ISpreadsheetService spreadsheetService;
        private readonly IServiceProvider serviceProvider;
        private IConfiguration _configuration;
        private IReportGeneratorService _reportGeneratorService;
        protected IRubricaComuneService _rubricaComuneService;

        public DocsPaWS(ISpreadsheetService spreadsheetService, IServiceProvider serviceProvider, IConfiguration configuration, IReportGeneratorService reportGeneratorService, IRubricaComuneService rubricaComuneService)
        {
            this.spreadsheetService = spreadsheetService; // spreadsheetService;
            this.serviceProvider = serviceProvider;
            this._configuration = configuration;
            this._reportGeneratorService = reportGeneratorService;
            this._rubricaComuneService = rubricaComuneService;
        }

        public virtual int DO_GetIdAmmByCodAmm(string codAmm)
        {
            int result = 0;
            try
            {
                ProspettiRiepilogativi.Model objM = new ProspettiRiepilogativi.Model();
                result = objM.DO_GetIdAmmByCodice(codAmm);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Errore nel recupero della connectionString - metodo: DO_GetDBType");
            }
            return result;
        }

        public void deleteConfigurazioneCache(string idAmministrazione)
        {
            try
            {
                BusinessLogic.Documenti.CacheFileManager.deleteConfigurazioneCache(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method deleteconfigurazionecache errore: " + e.Message);
            }
        }

        public DocsPaVO.Caching.CacheConfig getConfigurazioneCache(string idAmministrazione)
        {
            DocsPaVO.Caching.CacheConfig config = null;
            try
            {
                config = BusinessLogic.Documenti.CacheFileManager.GetInstance(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method getconfigurazionecache errore: " + e.Message);
            }

            return config;
        }

        public bool AmmCheckSessionID(string sessionID)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();

                bool result = false;
                try
                {
                    BusinessLogic.Amministrazione.AmministraManager amministrazione = new BusinessLogic.Amministrazione.AmministraManager();
                    result = amministrazione.CheckSessionID(sessionID);
                }
                catch (Exception e)
                {
                    logger.Debug(e, "Errore durante il controllo SessionID amministratore.");
                }

                return result;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public DocsPaVO.Conservazione.Policy GetPolicyById(string idPolicy)
        {
            try
            {
                DocsPaVO.Conservazione.Policy result = null;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.GetPolicyById(idPolicy);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: GetPolicyById");

                throw e;
            }
        }

        [XmlInclude(typeof(DocsPaVO.utente.Registro))]
        public DocsPaVO.utente.Registro[] GetRfByIdAmm(int idAmministrazione, string tipo)
        {
            try
            {
                Registro[] result = null;

                result = BusinessLogic.Amministrazione.RegistroManager.GetRfByIdAmm(idAmministrazione, tipo);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: GetTypeDocumentsWithDiagramByIdAmm");

                throw e;
            }
        }

        public string GetIdRuoloRespConservazione(string idAmm, string idAOO = null)
        {
            string result = string.Empty;

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager manager = new BusinessLogic.Conservazione.ConservazioneManager();
                result = manager.GetIdRuoloRespConservazione(idAmm, idAOO);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Errore in DocsPaWS.asmx - metodo: GetRuoloRespConservazione");
            }

            return result;
        }

        public string GetIdUtenteRespConservazione(string idAmm, string idAOO)
        {
            string result = string.Empty;

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager manager = new BusinessLogic.Conservazione.ConservazioneManager();
                result = manager.GetIdUtenteRespConservazione(idAmm, idAOO);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Errore in DocsPaWS.asmx - metodo: GetRuoloRespConservazione");
            }

            return result;
        }

        public bool GetStatoAttivazione(string idAmm)
        {
            bool result = false;

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager cons = new BusinessLogic.Conservazione.ConservazioneManager();
                result = cons.GetStatoAttivazione(idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetStatoAttivazione", ex);
            }

            return result;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.Conservazione.PARER.PolicyPARER))]
        //[XmlInclude(typeof(DocsPaVO.Conservazione.PARER.PolicyPARER))]
        //public virtual ArrayList GetListaPolicyPARER(string idAmm, string tipo)

        public virtual IList<DocsPaVO.Conservazione.PARER.PolicyPARER> GetListaPolicyPARER(string idAmm, string tipo)
        {
            ArrayList result = new ArrayList();

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.getListaPolicy(idAmm, tipo);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetListaPolicyPARER - ", ex);
                result = null;
            }

            return result.OfType<DocsPaVO.Conservazione.PARER.PolicyPARER>().ToList();
        }

        public bool SetStatoAttivazione(string idAmm, string stato)
        {
            bool result = false;

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager cons = new BusinessLogic.Conservazione.ConservazioneManager();
                result = cons.SetStatoAttivazione(idAmm, stato);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SetStatoAttivazione", ex);
            }

            return result;
        }

        public bool IsLogAttivatoAlert(string idAmm, string codice)
        {
            try
            {
                return BusinessLogic.Conservazione.AmmConservazioneManager.IsLogAttivato(idAmm, codice);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx - Metodo: IsLogAttivatoAlert");

            }

        }

        public DocsPaVO.amministrazione.AlertConservazione GetAlertValues(string idAmm)
        {
            DocsPaVO.amministrazione.AlertConservazione alertData = new DocsPaVO.amministrazione.AlertConservazione();
            try
            {
                alertData = BusinessLogic.Conservazione.AmmConservazioneManager.GetGestioneAlert(idAmm);
            }
            catch (Exception ex)
            {
                alertData = null;
                throw new ApplicationException("Errore in DocsPaWS.asmx - metodo GetGestioneAlert ", ex);
            }

            return alertData;
        }

        public bool SaveAlertValues(string idAmm, DocsPaVO.amministrazione.AlertConservazione param)
        {
            bool retVal = false;
            try
            {
                retVal = BusinessLogic.Conservazione.AmmConservazioneManager.SaveGestioneAlert(idAmm, param);
            }
            catch (Exception ex)
            {
                retVal = false;
                throw new ApplicationException("Errore in DocsPaWS.asmx - metodo SaveAlertValues ", ex);
            }

            return retVal;
        }

        public bool SetMailStruttura(string idAmm, DocsPaVO.Conservazione.PARER.Mailbox mail)
        {
            bool result = false;

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager cons = new BusinessLogic.Conservazione.ConservazioneManager();
                result = cons.SetMailStruttura(idAmm, mail);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SetMailStruttura", ex);
            }

            return result;
        }

        public DocsPaVO.Conservazione.PARER.Mailbox GetMailStruttura(string idAmm)
        {
            DocsPaVO.Conservazione.PARER.Mailbox result = new DocsPaVO.Conservazione.PARER.Mailbox();

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager cons = new BusinessLogic.Conservazione.ConservazioneManager();
                result = cons.GetMailStruttura(idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetMailStruttura", ex);
                result = null;
            }

            return result;
        }

        public DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyResponse ReportMonitoraggioPolicy(DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyRequest request)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();

                return BusinessLogic.Conservazione.PARER.ReportManager.ReportMonitoraggioPolicy(request, spreadsheetService, configurationService, _reportGeneratorService);
            }
        }

        public DocsPaVO.FormatiDocumento.SupportedFileType[] GetSupportedFileTypesPreservation(int idAmministrazione)
        {
            try
            {
                return BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileTypesPreservation(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetSupportedFileTypes", e);

                throw e;
            }
        }

        public bool InsertNewPolicyPARER(DocsPaVO.Conservazione.PARER.PolicyPARER policy, InfoUtenteAmministratore utente)
        {
            bool result = false;

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.InsertNewPolicy(policy);


                if (result)
                {
                    string tipo = string.Empty;
                    if (policy.tipo.Equals("D"))
                        tipo = "Documenti";
                    if (policy.tipo.Equals("S"))
                        tipo = "Stampe";
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERNEW", policy.id, string.Format("Creazione nuova Policy {2} {0} da parte dell'utente {1}", policy.codice, utente.userId, tipo), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    string tipo = string.Empty;
                    if (policy.tipo.Equals("D"))
                        tipo = "Documenti";
                    if (policy.tipo.Equals("S"))
                        tipo = "Stampe";
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERNEW", "", string.Format("Creazione nuova Policy {2} {0} da parte dell'utente {1}", policy.codice, utente.userId, tipo), DocsPaVO.Logger.CodAzione.Esito.KO);
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: InsertNewPolicyPARER - ", ex);
                result = false;
            }

            return result;
        }

        public bool UpdatePolicyPARER(DocsPaVO.Conservazione.PARER.PolicyPARER policy, InfoUtenteAmministratore utente)
        {
            bool result = false;

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.UpdatePolicy(policy);


                if (result)
                {
                    string tipo = string.Empty;
                    if (policy.tipo.Equals("D"))
                        tipo = "Documenti";
                    if (policy.tipo.Equals("S"))
                        tipo = "Stampe";
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERMOD", policy.id, string.Format("Modifica Policy {2} {0} da parte dell'utente {1}", policy.codice, utente.userId, tipo), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    string tipo = string.Empty;
                    if (policy.tipo.Equals("D"))
                        tipo = "Documenti";
                    if (policy.tipo.Equals("S"))
                        tipo = "Stampe";
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERMOD", policy.id, string.Format("Modifica Policy {2} {0} da parte dell'utente {1}", policy.codice, utente.userId, tipo), DocsPaVO.Logger.CodAzione.Esito.KO);
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx . metodo: UpdatePolicyPARER - ", ex);
                result = false;
            }

            return result;
        }

        public string GetCountDocumentiFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy, string idAmm)
        {
            string result = string.Empty;

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.GetCountDocumentiFromPolicy(policy, idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetCountDocumentiFromPolicy - ", ex);
            }

            return result;
        }

        public DocsPaVO.DiagrammaStato.DiagrammaStato getDiagrammaById(string idDiagramma)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getDiagrammaById", e);
                return null;
            }
        }

        public DocsPaVO.Conservazione.PARER.PolicyPARER GetPolicyPARERById(string idPolicy)
        {
            DocsPaVO.Conservazione.PARER.PolicyPARER result = new DocsPaVO.Conservazione.PARER.PolicyPARER();

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.GetPolicyById(idPolicy);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetPolicyPARERById - ", ex);
                result = null;
            }

            return result;
        }

        [XmlInclude(typeof(DocsPaVO.ProfilazioneDinamicaLite.TemplateLite))]
        public DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] GetTypeDocumentsWithDiagramByIdAmm(int idAmministrazione, string type)
        {
            try
            {
                TemplateLite[] result = null;
                if (type.Equals("D"))
                {
                    result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLite(idAmministrazione.ToString());
                }
                else
                {
                    result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getListTemplatesLite(idAmministrazione.ToString());
                }
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetTypeDocumentsWithDiagramByIdAmm", e);

                throw e;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTitolario))]
        //public virtual ArrayList getTitolariUtilizzabili(string idAmministrazione)

        public virtual IList<OrgTitolario> getTitolariUtilizzabili(string idAmministrazione)
        {
            ArrayList titolari = new ArrayList();
            try
            {
                titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(idAmministrazione);
                return titolari.OfType<OrgTitolario>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTitolariUtilizzabili", e);
                return titolari.OfType<OrgTitolario>().ToList();
            }
        }

        [XmlInclude(typeof(DocsPaVO.fascicolazione.Fascicolo))]
        public DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurityConservazione(string codiceFasc, string idAmm, string idTitolario, bool soloGenerali, bool isRicFasc, string idRegistro)
        {
            try
            {
                Fascicolo[] result = null;
                result = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurityConservazione(codiceFasc, idAmm, idTitolario, soloGenerali, isRicFasc, idRegistro);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetFascicoloDaCodiceNoSecurityConservazione", e);
                return null;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public bool InserisciPolicyConservazione(DocsPaVO.Conservazione.Policy policy)
        {
            try
            {
                bool result = false;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.InsertNewPolicy(policy);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: InserisciPolicyConservazione", e);

                throw e;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public bool SvuotaCachePolicy(string idAmm, string tipo)
        {
            try
            {
                bool result = false;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.SvuotaCachePolicy(idAmm, tipo);

                return result;

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SvuotaCachePolicy", ex);

                throw ex;
            }

        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public bool ModifyPolicyConservazione(DocsPaVO.Conservazione.Policy policy)
        {
            try
            {
                bool result = false;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.ModifyNewPolicy(policy);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ModifyNewPolicy", e);

                throw e;
            }
        }

        public RegisteredRegistriRepertorioResponse GetRegisteredRegistries(RegisteredRegistriRepertorioRequest request)
        {
            RegisteredRegistriRepertorioResponse response = new RegisteredRegistriRepertorioResponse();
            try
            {
                response.RegistriRepertorio = BusinessLogic.Utenti.RegistriRepertorioPrintManager.GetRegisteredRegistries(request.AdministrationId);

            }
            catch (Exception e)
            {
                logger.Debug(String.Format("Errore durante il recupero dell'anagrafica dei registri di repertorio definiti per l'amministrazione {0}", request.AdministrationId), e);
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

            return response;
        }

        public RegistroRepertorioSettingsResponse GetRegisterSettings(RegistroRepertorioSettingsRequest request)
        {
            RegistroRepertorioSettingsResponse response = new RegistroRepertorioSettingsResponse();
            try
            {
                response.RegistroRepertorioSingleSettings = BusinessLogic.Utenti.RegistriRepertorioPrintManager.GetRegisterSettings(request.CounterId, request.RegistryId, request.RfId, request.TipologyKind, request.SettingsType);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il recupero delle impostazioni relative ad un registro di repertorio", e);
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

            return response;
        }

        public Utente[] getUserInRoleByIdCorrGlobali(string idCorrGlobale)
        {
            try
            {
                Utente[] result = null;

                result = BusinessLogic.Utenti.UserManager.getUserInRoleByIdCorrGlobali(idCorrGlobale);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ModifyNewPolicy", e);

                throw e;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public bool SavePeriodPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            try
            {
                bool result = false;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.SavePeriodPolicy(policy);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: SavePeriodPolicy", e);

                throw e;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public DocsPaVO.Conservazione.Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            try
            {
                DocsPaVO.Conservazione.Policy[] result = null;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.GetListaPolicy(idAmm, tipo);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetListaPolicy", e);

                throw e;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Conservazione.Policy))]
        public bool DeletePolicy(string idPolicy)
        {
            try
            {
                bool result = false;

                result = BusinessLogic.Conservazione.Policy.PolicyManager.DeletePolicy(idPolicy);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DeletePolicy", e);

                throw e;
            }
        }

        public DocsPaVO.Conservazione.PARER.EsecuzionePolicy GetInfoEsecuzionePolicy(string idPolicy, string tipo)
        {
            DocsPaVO.Conservazione.PARER.EsecuzionePolicy result = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.GetInfoEsecuzionePolicy(idPolicy, tipo);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetInfoEsecuzionePolicy");
                result = null;
            }

            return result;
        }

        public bool DeletePolicyPARER(string idPolicy, InfoUtenteAmministratore utente)
        {
            bool result = false;

            try
            {
                DocsPaVO.Conservazione.PARER.PolicyPARER policy = BusinessLogic.Conservazione.PARER.PolicyPARERManager.GetPolicyById(idPolicy);
                string codPolicy = policy.codice;
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.DeletePolicy(idPolicy);

                if (result)
                {
                    string tipo = string.Empty;
                    if (policy.tipo.Equals("D"))
                        tipo = "Documenti";
                    if (policy.tipo.Equals("S"))
                        tipo = "Stampe";
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERDEL", idPolicy, string.Format("Eliminazione Policy {2} {0} da parte dell'utente {1}", codPolicy, utente.userId, tipo), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    string tipo = string.Empty;
                    if (policy.tipo.Equals("D"))
                        tipo = "Documenti";
                    if (policy.tipo.Equals("S"))
                        tipo = "Stampe";
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERDEL", idPolicy, string.Format("Eliminazione Policy {2} {0} da parte dell'utente {1}", codPolicy, utente.userId, tipo), DocsPaVO.Logger.CodAzione.Esito.KO);
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: DeletePolicyPARER - ", ex);
            }

            return result;
        }

        public bool UpdateStatoPolicy(ArrayList lista, InfoUtenteAmministratore utente)
        {
            bool result = false;

            try
            {
                var converted = new ArrayList();
                foreach (XmlNode[] modelloXml in lista)
                {

                    var elemento = new DocsPaVO.Conservazione.PARER.PolicyPARER();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                           
                            case "id":
                                elemento.id = field.InnerText;
                                break;
                            case "tipo":
                                elemento.tipo = field.InnerText;
                                break;
                            case "codice":
                                elemento.codice = field.InnerText;
                                break;
                            case "descrizione":
                                elemento.descrizione = field.InnerText;
                                break;
                            case "isAttiva":
                                elemento.isAttiva = field.InnerText;
                                break;
                            case "arrivo":
                                elemento.arrivo = bool.Parse(field.InnerText);
                                break;
                            case "partenza":
                                elemento.partenza = bool.Parse(field.InnerText);
                                break;
                            case "interno":
                                elemento.interno = bool.Parse(field.InnerText);
                                break;
                            case "grigio":
                                elemento.grigio = bool.Parse(field.InnerText);
                                break;
                            case "notificaMail":
                                elemento.notificaMail = field.InnerText;
                                break;
                        }
                    }
                    converted.Add(elemento);
                }
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.UpdateStatoPolicy(converted, utente);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: UpdateStatoPolicy - ", ex);
            }

            return result;
        }

        public bool SaveRuoloRespConservazione(string idGroup, string idUser, string idAmm)
        {
            bool result = false;

            try
            {
                result = BusinessLogic.Utenti.RegistriRepertorioPrintManager.SaveRuoloRespConservazione(idGroup, idUser, idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SaveRuoloRespConservazione", ex);
            }

            return result;
        }

        public string GetStampaRegistroValues(string idAmm)
        {
            string retval = string.Empty;

            try
            {
                retval = BusinessLogic.Conservazione.AmmConservazioneManager.GetStampaRegistroValues(idAmm);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx - metodo: GetStampaRegistroValues", e);
            }

            return retval;

        }

        public bool SaveStampaRegistroValues(string idAmm, string disabled, string printFreq, string printHour)
        {
            bool retval = false;

            try
            {
                retval = BusinessLogic.Conservazione.AmmConservazioneManager.SaveStampaRegistroValues(idAmm, disabled, printFreq, printHour);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx - metodo: SaveStampaRegistroValues", e);
            }

            return retval;

        }

        public void WriteLog(InfoUtenteAmministratore infoUtente, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, bool esitoOperazione)
        {
            if (infoUtente != null)
            {
                if (esitoOperazione)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, ID_Oggetto, Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, WebMethodName, ID_Oggetto, Var_desc_Oggetto, DocsPaVO.Logger.CodAzione.Esito.KO);
            }
        }

        public string GetStampaRegistroOraStampa(string idAmm)
        {
            string retVal = string.Empty;

            try
            {
                retVal = BusinessLogic.Conservazione.AmmConservazioneManager.GetStampaRegistroOraStampa(idAmm);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx - metodo: GetStampaRegistroOraStampa", e);
            }

            return retVal;

        }

        public string getIdAmmByCod(string codiceAmministrazione)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getIdAmmByCod(codiceAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: aggiungiOggettoValoreOggettoCustom", e);
                return null;
            }
        }

        public DocsPaVO.LibroFirma.ProcessoFirma[] GetProcessiDiFirmaByIdAmm(string idAmministrazione)
        {
            List<DocsPaVO.LibroFirma.ProcessoFirma> listaProcessiDiFirma = new List<DocsPaVO.LibroFirma.ProcessoFirma>();
            try
            {
                listaProcessiDiFirma = BusinessLogic.LibroFirma.LibroFirmaManager.GetProcessiDiFirmaByIdAmm(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetProcessiDiFirmaByIdAmm", e);
            }
            return listaProcessiDiFirma.ToArray();
        }

        public bool IsConsolidationEnabled()
        {
            try
            {
                return BusinessLogic.Documenti.DocumentConsolidation.IsConfigEnabled();
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public DocsPaVO.FormatiDocumento.SupportedFileType[] GetSupportedFileTypes(int idAmministrazione)
        {
            try
            {
                return BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileTypes(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetSupportedFileTypes", e);

                throw e;
            }
        }

        public DocsPaVO.amministrazione.OrgTipoFunzione AmmGetTipoFunzioneByCod(string codice, bool fillFunzioniElementari)
        {

            DocsPaVO.amministrazione.OrgTipoFunzione retVal = new DocsPaVO.amministrazione.OrgTipoFunzione();

            try
            {
                retVal = BusinessLogic.Amministrazione.TipiFunzioneManager.GetTipoFunzioneByCod(codice, fillFunzioniElementari);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: AmmGetTipoFunzioneByCod", ex);
            }

            return retVal;
        }

        public DocsPaVO.amministrazione.OrgFunzioneAnagrafica AmmGetFunzioneAnagrafica(string codice)
        {

            DocsPaVO.amministrazione.OrgFunzioneAnagrafica retVal = new DocsPaVO.amministrazione.OrgFunzioneAnagrafica();

            try
            {
                retVal = BusinessLogic.Amministrazione.FunzioniManager.GetAnagraficaPerReport(codice);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: AmmGetFunzioneAnagrafica", ex);
            }

            return retVal;

        }

        public DocsPaVO.documento.FileDocumento AmmGetReportFunzioni(string tipoReport, string formato, string idFunzione, string idAmm)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();

                DocsPaVO.documento.FileDocumento retVal = new DocsPaVO.documento.FileDocumento();

                try
                {
                    retVal = BusinessLogic.Amministrazione.TipiFunzioneManager.ReportFunzioni(tipoReport, formato, idFunzione, idAmm, spreadsheetService, configurationService, _reportGeneratorService);
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetReportFunzioni", ex);
                    retVal = null;
                }

                return retVal;
            }
        }

        [return: XmlArray()]
        public DocsPaVO.amministrazione.OrgTipoFunzione[] AmmGetTipiFunzione(bool fillFunzioniElementari, string idAmm)
        {
            DocsPaVO.amministrazione.OrgTipoFunzione[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiFunzioneManager.GetTipiFunzione(fillFunzioniElementari, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetTipiFunzione", e);
            }

            return retValue;
        }

        public DocsPaVO.amministrazione.OrgTipoFunzione AmmGetTipoFunzione(string idTipoFunzione, bool fillFunzioniElementari)
        {
            DocsPaVO.amministrazione.OrgTipoFunzione retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiFunzioneManager.GetTipoFunzione(idTipoFunzione, fillFunzioniElementari);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetTipoFunzione", e);
            }

            return retValue;
        }

        [return: XmlArray()]
        public DocsPaVO.amministrazione.OrgFunzione[] AmmGetFunzioni(string idTipoFunzoine)
        {
            DocsPaVO.amministrazione.OrgFunzione[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.FunzioniManager.GetFunzioni(idTipoFunzoine);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetFunzioni", e);
            }

            return retValue;
        }

        [return: XmlArray()]
        public DocsPaVO.amministrazione.OrgFunzioneAnagrafica[] AmmGetFunzioniAnagrafica()
        {
            DocsPaVO.amministrazione.OrgFunzioneAnagrafica[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.FunzioniManager.GetFunzioniAnagrafica();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetFunzioniAnagrafica", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmInsertTipoFunzione(ref DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiFunzioneManager.InsertTipoFunzione(tipoFunzione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertTipoFunzione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateTipoFunzione(ref DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiFunzioneManager.UpdateTipoFunzione(tipoFunzione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateTipoFunzione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDeleteTipoFunzione(DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiFunzioneManager.DeleteTipoFunzione(tipoFunzione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteTipoFunzione", e);
            }

            return retValue;
        }

        //public ArrayList getListaTemi()

        public IList<string> getListaTemi()
        {
            ArrayList result = new ArrayList();
            try
            {
                BusinessLogic.Amministrazione.AmministraManager am = new BusinessLogic.Amministrazione.AmministraManager();
                result = am.getListaTemi();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS  - metodo: getListaTemi", e);
            }
            return result.OfType<string>().ToList();
        }

        public string getCssAmministrazione(string idAmm)
        {
            string result = string.Empty;
            try
            {
                BusinessLogic.Utenti.UserManager um = new BusinessLogic.Utenti.UserManager();
                result = um.getCssAmministrazione(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS  - metodo: getCssAmministrazione", e);
            }
            return result;
        }

        public string getSegnAmm(string idAmm)
        {
            string result = string.Empty;
            try
            {
                BusinessLogic.Utenti.UserManager um = new BusinessLogic.Utenti.UserManager();
                result = um.getSegnAmm(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS  - metodo: getSegnAmm", e);
            }
            return result;
        }

        public string getpath(string type)
        {
            string serverPath = "";
            try
            {

                switch (type)
                {
                    case "Amministrazione":
                        serverPath = DocsPaVO.Settings.AppSettings.Instance.ADMIN_IMAGES_ROOT_PATH;
                        break;

                    case "FrontEnd":
                        serverPath = DocsPaVO.Settings.AppSettings.Instance.IMAGES_ROOT_PATH;
                        break;

                    case "LoginFE":
                        serverPath = DocsPaVO.Settings.AppSettings.Instance.IMAGES_PATH;
                        break;

                    case "Scrittura":
                        serverPath = DocsPaVO.Settings.AppSettings.Instance.READ_IMAGES_ROOT_PATH;
                        break;

                    case "Css":
                        serverPath = DocsPaVO.Settings.AppSettings.Instance.CSS_ROOT_PATH;
                        break;
                }
                return serverPath;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: existFile", e);
                return serverPath;
            }
        }

        public bool setTemaAmministrazione(string idAmm, int valore)
        {
            bool result = false;
            BusinessLogic.Amministrazione.AmministraManager am = new BusinessLogic.Amministrazione.AmministraManager();
            result = am.setTemaAmministrazione(idAmm, valore);

            return result;
        }

        public bool setColoreSegnatura(string idAmm, int valore)
        {
            bool result = false;
            BusinessLogic.Amministrazione.AmministraManager am = new BusinessLogic.Amministrazione.AmministraManager();
            result = am.setColoreSegnatura(idAmm, valore);

            return result;
        }

        public void clearHashTableChiaviConfig(string idAmm)
        {
            DocsPaUtils.Configuration.InitConfigurationKeys.remove(idAmm);
        }

        public bool EnableJavaAppletOption()
        {
            bool retVal = false;
            try
            {
                retVal = DocsPaDB.Query_DocsPAWS.SmartClient.EnableJavaAppletOption();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: EnableJavaAppletOption", e);
                retVal = false;
            }

            return retVal;
        }

        public string isEnableProtocolloTitolario()
        {
            return DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario();
        }

        public virtual bool isEnableRiferimentiMittente()
        {
            return DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableRiferimentiMittente();
        }

        //[SoapInclude(typeof(InfoUtente))]
        //      [SoapInclude(typeof(InfoUtenteAmministratore))]
        //      [SoapInclude(typeof(ModelProcessorInfo))]
        //public DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors(DocsPaVO.utente.InfoUtente infoUtente)
        public DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors(InfoUtenteAmministratore infoUtente)
        {
            SetUserId(infoUtente);
            return BusinessLogic.Modelli.ModelliManager.GetModelProcessors(infoUtente);
        }

        public bool testConnessione(OrgRegistro.MailRegistro mailRegistro, string tipoConnessione, out string messaggioDiErrore)
        {
            messaggioDiErrore = string.Empty;
            try
            {
                BusinessLogic.Interoperabilita.SvrPosta sv = new BusinessLogic.Interoperabilita.SvrPosta(mailRegistro.Provider);
                return sv.provaConnessione(mailRegistro, out messaggioDiErrore, tipoConnessione);
            }
            catch (Exception e)
            {
                logger.Debug("errore nel web method test connessione - errore: " + e.Message);
            }

            messaggioDiErrore = "";
            return false;
        }

        public string VerificaDocErrati(string idAmm)
        {
            string retvalue = string.Empty;
            try
            {
                retvalue = BusinessLogic.Documenti.DocManager.VerificaDocErrati(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo VerificaDocErrati", e);
                throw e;
            }
            return retvalue;
        }

        public DocsPaVO.amministrazione.VisualizzaStatoDoc GetInfoDocument(string docnumber, string numProto, string anno, string idAmm, string codiceRegistro)
        {
            VisualizzaStatoDoc info = null;
            try
            {
                info = BusinessLogic.Amministrazione.AmministraManager.GetInfoDocument(docnumber, numProto, anno, idAmm, codiceRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetInfoDocument - ", e);
                info = null;
            }
            return info;
        }

        public DocsPaVO.LibroFirma.IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso, InfoUtenteAmministratore infoUtente)
        {
            try
            {
                return BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(idIstanzaProcesso, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetIstanzaProcessoDiFirmaByIdIstanzaProcesso", e);
                return null;
            }
        }

        public DocsPaVO.Import.Pregressi.EsitoImportPregressi importPregresso(InfoUtenteAmministratore infoUtente, byte[] dati, bool isAdministration)
        {
            SetUserId(infoUtente);
            DocsPaVO.Import.Pregressi.EsitoImportPregressi esitoImport = new DocsPaVO.Import.Pregressi.EsitoImportPregressi();
            try
            {
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                string name = "importPreg" + DateTime.Now.Ticks.ToString() + ".xls";

                esitoImport = BusinessLogic.Amministrazione.ImportPregressiManager.CheckFileData(dati, name, serverPath, infoUtente, isAdministration);

                return esitoImport;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: importPregresso", e);
                esitoImport = null;
            }

            return esitoImport;
        }

        // da togliere, va al documentale e carica una marea di roba
        private bool CreaDocumentiImportPregresso(InfoUtenteAmministratore infoUtente, DocsPaVO.Import.Pregressi.ReportPregressi report)
        {
            BusinessLogic.Amministrazione.ImportPregressiManager.CreaDocumenti(infoUtente, report, this._rubricaComuneService);
            return true;
        }

        private delegate bool asyncDelegate(InfoUtenteAmministratore infoUtente, DocsPaVO.Import.Pregressi.ReportPregressi report);

        private void asyncImport(IAsyncResult ar)
        {
        }

        public bool asyncImportPregresso(InfoUtenteAmministratore infoUtente, DocsPaVO.Import.Pregressi.EsitoImportPregressi esitoPregressi, string descrizione)
        {
            bool esito = false;
            SetUserId(infoUtente);
            try
            {
                //Andrea - parametro descrizione del report
                DocsPaVO.Import.Pregressi.ReportPregressi report = BusinessLogic.Amministrazione.ImportPregressiManager.ImportaPregresso(infoUtente, esitoPregressi, descrizione);

                if (report.itemPregressi.Count > 0)
                {
                    asyncDelegate mDelegate = new asyncDelegate(CreaDocumentiImportPregresso);
                    IAsyncResult result = mDelegate.BeginInvoke(infoUtente, report, new AsyncCallback(asyncImport), null);

                    mDelegate.EndInvoke(result);

                    esito = true;
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: asyncImportPregresso", e);
            }

            return esito;
        }

        public DocsPaVO.documento.FileDocumento ExportReportExcel(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr)
        {
            DocsPaVO.documento.FileDocumento result = null;
#if false
			BusinessLogic.ExportDati.ExportDatiManager manager = new BusinessLogic.ExportDati.ExportDatiManager();
			result = manager.ExportReportInError(repInErr);

#endif
            return result;
        }

        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.ReportPregressi))]
        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.ItemReportPregressi))]
        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.EsitoImportPregressi))]
        public DocsPaVO.Import.Pregressi.ReportPregressi GetReportPregressi(string sysId, bool getItems)
        {
            try
            {
                DocsPaVO.Import.Pregressi.ReportPregressi result = null;

#if false
				result = BusinessLogic.Import.ImportPregressi.PregressiManager.GetReportPregressi(sysId, getItems);

#endif
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetReportPregressi", e);

                throw e;
            }
        }

        // da togliere, va alla generazione in excel
        public DocsPaVO.documento.FileDocumento ExportPregressiExcel(DocsPaVO.Import.Pregressi.ReportPregressi report, InfoUtenteAmministratore infoUtente)
        {

            DocsPaVO.documento.FileDocumento result = null;
            BusinessLogic.ExportDati.ExportDatiManager manager = new BusinessLogic.ExportDati.ExportDatiManager();
#if false
			result = manager.ExportReportPregresso(report, infoUtente);

#endif
            return result;
        }

        public DocsPaVO.utente.Registro GetRegistroBySistemId(string idRegistro)
        {
            DocsPaVO.utente.Registro result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Utenti.RegistriManager.getRegistro(idRegistro);
#if TRACE_WS
				pt.WriteLogTracer("FascicolazioneNewTitolario");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetRegistroBySistemId", e);
                result = null;
            }

            return result;
        }

        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.ReportPregressi))]
        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.ItemReportPregressi))]
        public DocsPaVO.Import.Pregressi.ReportPregressi[] GetReports(bool getItems, InfoUtenteAmministratore infoUtente, bool daAmministrazione)
        {
            try
            {
                DocsPaVO.Import.Pregressi.ReportPregressi[] result = null;

                result = BusinessLogic.Import.ImportPregressi.PregressiManager.GetReports(getItems, infoUtente, daAmministrazione);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetReports", e);

                throw e;
            }
        }

        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.ReportPregressi))]
        [XmlInclude(typeof(DocsPaVO.Import.Pregressi.ItemReportPregressi))]
        public bool DeleteReportById(string idReport)
        {
            try
            {
                bool result = false;

                result = BusinessLogic.Import.ImportPregressi.PregressiManager.DeleteReport(idReport);

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DeleteReportById", e);

                throw e;
            }
        }

        public Disservizio getInfoDisservizio()
        {
            return BusinessLogic.Amministrazione.AmministraManager.getDisservizio();
        }

        public bool updateDisservizio(Disservizio disservizio)
        {
            if (BusinessLogic.Amministrazione.AmministraManager.updateDisservizio(disservizio))
                //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
                return this.setStatoAccettazioneDisservizio("F");
            else
                return false;
        }

        public bool creaDisservizio(Disservizio disservizio)
        {
            if (BusinessLogic.Amministrazione.AmministraManager.creaDisservizio(disservizio))
                //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
                return this.setStatoAccettazioneDisservizio("F");
            else
                return false;
        }

        public bool setStatoNotificaDisservizio(string systemId, string stato)
        {
            if (BusinessLogic.Amministrazione.AmministraManager.setStatoNotificaDisservizio(systemId, stato))
            {
                //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
                if (stato == "2")
                    return this.setStatoAccettazioneDisservizio("F");
                else
                    return this.setStatoAccettazioneDisservizio("N");

            }
            else
                return false;

        }

        public bool cambiaStatoDisservizio(string stato, string system_id)
        {
            if (BusinessLogic.Amministrazione.AmministraManager.cambiaStatoDisservizio(stato, system_id))
                return this.setStatoAccettazioneDisservizio("F");
            else
                return false;
        }

        public bool deleteDisservizio(string system_id)
        {
            if (BusinessLogic.Amministrazione.AmministraManager.deleteDisservizio(system_id))
                //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
                return this.setStatoAccettazioneDisservizio("F");
            else
                return false;
        }

        public virtual DataSet getListeDistribuzioneAmm(string idAmm)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.getListe(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getListeDistribuzione", e);
                return null;
            }
        }

        public virtual string getCodiceLista(string idLista)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.getCodiceLista(idLista);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getCodiceLista", e);
                return null;
            }
        }

        public virtual DataSet getCorrispondentiLista(string codiceLista)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiLista(codiceLista);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getCorrispondentiLista", e);
                return null;
            }
        }

        public virtual void deleteListaDistribuzione(string codiceLista)
        {
            try
            {
                BusinessLogic.Rubrica.ListeDistribuzione.deleteLista(codiceLista);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: deleteListaDistribuzione", e);
            }
        }

        public virtual string getNomeLista(string codiceLista, string idAmm)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.getNomeLista(codiceLista, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getNomeLista", e);
                return null;
            }
        }

        public virtual bool isUniqueNomeLista(string nomeLista, string idAmm)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.isUniqueNomeLista(nomeLista, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isUniqueNomeLista", e);
                return false;
            }
        }


        public virtual void modificaLista(object dsCorrLista, string idLista, string nomeLista, string codiceLista)
        {
            try
            {
                var dsCorrispondenti = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("ID_DPA_CORR");
                dt.Columns.Add("VAR_DESC_CORR");
                dt.Columns.Add("VAR_COD_RUBRICA");
                dt.Columns.Add("CHA_TIPO_IE");
                dt.Columns.Add("CHA_DISABLED_TRASM");

                var datasetNode = ((XmlNode)(dsCorrLista as XmlNode[])[1]).SelectSingleNode("NewDataSet");
                if (datasetNode != null)
                {
                    var childNodes = datasetNode.SelectNodes("ResultTable");
                    foreach (XmlNode node in childNodes)
                    {
                        DataRow row = dt.NewRow();
                        row[0] = node.SelectSingleNode("ID_DPA_CORR")?.InnerText;
                        row[1] = node.SelectSingleNode("VAR_DESC_CORR")?.InnerText;
                        row[2] = node.SelectSingleNode("VAR_COD_RUBRICA")?.InnerText;
                        row[3] = node.SelectSingleNode("CHA_TIPO_IE")?.InnerText;
                        row[4] = node.SelectSingleNode("CHA_DISABLED_TRASM")?.InnerText;
                        dt.Rows.Add(row);
                    }
                }

                dsCorrispondenti.Tables.Add(dt);

                //BusinessLogic.Rubrica.ListeDistribuzione.modificaLista(dsCorrLista, idLista, nomeLista, codiceLista);
                BusinessLogic.Rubrica.ListeDistribuzione.modificaLista(dsCorrispondenti, idLista, nomeLista, codiceLista);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: modificaLista", e);
            }
        }



        public virtual bool isUniqueCod(string codLista, string idAmm)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.isUniqueCod(codLista, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isUniqueCod", e);
                return false;
            }
        }

        public virtual void salvaLista(object dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm)
        {
            SetUserId(idUtente);
            try
            {
                var dsCorrispondenti = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("ID_DPA_CORR");
                dt.Columns.Add("VAR_DESC_CORR");
                dt.Columns.Add("VAR_COD_RUBRICA");
                dt.Columns.Add("CHA_TIPO_IE");
                dt.Columns.Add("CHA_DISABLED_TRASM");

                var datasetNode = ((XmlNode)(dsCorrLista as XmlNode[])[1]).SelectSingleNode("NewDataSet");
                if (datasetNode != null)
                {
                    var childNodes = datasetNode.SelectNodes("ResultTable");
                    foreach (XmlNode node in childNodes)
                    {
                        DataRow row = dt.NewRow();
                        row[0] = node.SelectSingleNode("ID_DPA_CORR")?.InnerText;
                        row[1] = node.SelectSingleNode("VAR_DESC_CORR")?.InnerText;
                        row[2] = node.SelectSingleNode("VAR_COD_RUBRICA")?.InnerText;
                        row[3] = node.SelectSingleNode("CHA_TIPO_IE")?.InnerText;
                        row[4] = node.SelectSingleNode("CHA_DISABLED_TRASM")?.InnerText;
                        dt.Rows.Add(row);
                    }
                }

                dsCorrispondenti.Tables.Add(dt);
                BusinessLogic.Rubrica.ListeDistribuzione.salvaLista(dsCorrispondenti, nomeLista, codiceLista, idUtente, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaLista", e);
            }
        }

        public string ExportAmministrazioni()
        {
            string result = null;

            try
            {
                BusinessLogic.AmministrazioneXml.AmministrazioniXml amministrazioniXml = new BusinessLogic.AmministrazioneXml.AmministrazioniXml();
                XmlDocument doc;
                doc = amministrazioniXml.ExportStructureXmlDocument();
                if (doc != null)
                {
                    result = doc.OuterXml;
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'esportazione della struttura delle amministrazioni.", exception);
                result = null;
            }

            return result;
        }

        public DocsPaVO.amministrazione.EsitoOperazione LoginAmministratoreProfilato(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore datiAmministratore)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            datiAmministratore = null;
            SetUserId(userLogin.UserName);
            try
            {
                esito = BusinessLogic.Amministrazione.AmministraManager.LoginAmministratoreProfilato(userLogin, forceLogin, out datiAmministratore);
                if (esito.Codice == 0)
                    BusinessLogic.UserLog.UserLog.WriteLog(datiAmministratore, "AMM_LOGIN", datiAmministratore.idPeople, "Accesso ad amministrazione di " + datiAmministratore.userId + " da IP " + userLogin.IPAddress, DocsPaVO.Logger.CodAzione.Esito.OK);
                else
                {
                    if (datiAmministratore == null)
                    {
                        datiAmministratore = new InfoUtenteAmministratore();
                        datiAmministratore.idPeople = "0";
                        datiAmministratore.userId = userLogin.UserName;
                        datiAmministratore.idGruppo = "0";
                        datiAmministratore.idAmministrazione = "0";
                        datiAmministratore.delegato = null;
                    }
                    BusinessLogic.UserLog.UserLog.WriteLog(datiAmministratore, "AMM_LOGIN", datiAmministratore.idPeople, "Accesso ad amministrazione fallito di " + datiAmministratore.userId + " da IP " + userLogin.IPAddress, DocsPaVO.Logger.CodAzione.Esito.KO);
                }
            }
            catch (Exception e)
            {
                if (datiAmministratore == null)
                {
                    datiAmministratore = new InfoUtenteAmministratore();
                    datiAmministratore.idPeople = "0";
                    datiAmministratore.userId = userLogin.UserName;
                    datiAmministratore.idGruppo = "0";
                    datiAmministratore.idAmministrazione = "0";
                    datiAmministratore.delegato = null;
                }
                BusinessLogic.UserLog.UserLog.WriteLog(datiAmministratore, "AMM_LOGIN", datiAmministratore.idPeople, "Accesso ad amministrazione fallito di " + datiAmministratore.userId + " da IP " + userLogin.IPAddress, DocsPaVO.Logger.CodAzione.Esito.KO);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: LoginAmministratoreProfilato ", e);
                esito.Codice = 1;
                esito.Descrizione = "errore di sistema!";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.InfoAmministrazione AmmGetInfoAmmCorrente(string idAmm)
        {
            DocsPaVO.amministrazione.InfoAmministrazione retValue = null;
            try
            {
                retValue = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetInfoAmmCorrente - ", e);
            }
            return retValue;
        }

        public DocsPaVO.amministrazione.InfoAmministrazione GetInfoAmmAppartenenzaUtente(string userid, string pwd)
        {
            DocsPaVO.amministrazione.InfoAmministrazione result = new DocsPaVO.amministrazione.InfoAmministrazione();
            try
            {
                result = BusinessLogic.Amministrazione.AmministraManager.GetInfoAmmAppartenenzaUtente(userid, pwd);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetInfoAmmAppartenenzaUtente ", e);
            }
            return result;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.Menu))]
        //public virtual ArrayList AmmGetListMenuUtente(string idAmm, string idCorrGlob)

        public virtual IList<DocsPaVO.amministrazione.Menu> AmmGetListMenuUtente(string idAmm, string idCorrGlob)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.UtenteManager.AmmGetListMenuUtente(idAmm, idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListMenuUtente - ", e);
            }

            return retValue.OfType<DocsPaVO.amministrazione.Menu>().ToList();
        }

        public string GetXMLLog(string codAmm)
        {
            string result = "";
            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                result = log.ReadWSLog(codAmm);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS GetXMLLog: ", exception);
            }
            return result;
        }

        public bool SetXMLLog(string stream, string codAmm, InfoUtenteAmministratore infoUtente)
        {
            bool result = false;
            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                string idAmm = log.GetAdminByName(codAmm);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMM_MOD_ATTIVAZIONE_LOG", idAmm, "Modifica in attivazione/disabilitazione log", DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
                result = log.WriteWSLog(stream, codAmm);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS SetXMLLog: ", exception);
            }
            return result;
        }

        public string VerificaArchivioLogPath()
        {
            string result = string.Empty;

            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                result = log.VerificaArchivioLogPath();
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS VerificaArchivioPath: ", exception);
            }

            return result;
        }

        public string ContaLog(string codAmm, string type)
        {
            string result = "";

            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                log.ContaArchivioLog(out result, codAmm, type);

            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS ContaLog: ", exception);
            }

            return result;
        }

        public bool StampaPDFLog(string codAmm, string type)
        {
            bool result = false;

            try
            {
                BusinessLogic.Report.ReportLog rpl = new BusinessLogic.Report.ReportLog();
                result = rpl.RiversaInStorico(codAmm, type);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS StampaPDFLog: ", exception);
            }

            return result;
        }

        public string[] GetFilesLog(string codAmm)
        {
            string[] files = null;

            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                files = log.ListaFilesLog(codAmm);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS GetFilesLog: ", exception);
            }
            return files;
        }

        public Assertion[] GetListAssertion(string idAmm)
        {
            List<Assertion> assertions = BusinessLogic.Amministrazione.AssertionEventManager.GetListAssertionsEvent(idAmm);
            return assertions.ToArray();
        }

        public int InsertAssertionEvent(Assertion newAssertion)
        {
            return BusinessLogic.Amministrazione.AssertionEventManager.InsertAssertionEvent(newAssertion);
        }

        public int UpdateAssertionEvent(Assertion assertion)
        {
            return BusinessLogic.Amministrazione.AssertionEventManager.UpdateAssertionEvent(assertion);
        }

        public virtual DocsPaVO.documento.FileDocumento ExportLog(string codAmm, string type, string exportType, string title, string user, string data_a, string data_da, string oggetto, string azione, string esito, int tabelle)
        {
            SetUserId(user);
            DocsPaVO.documento.FileDocumento file = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                BusinessLogic.Report.ReportLog manager = new BusinessLogic.Report.ReportLog();
                file = (DocsPaVO.documento.FileDocumento)manager.GeneraFile(codAmm, type, exportType, title, user, data_a, data_da, oggetto, azione, esito, tabelle);
#if TRACE_WS
				pt.WriteLogTracer("ExportRicercaFasc");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ExportLog", e);
                file = null;
            }
            return file;
        }

        public virtual string findRecords(string idAmm)
        {
            string result = "";
            try
            {
                result = BusinessLogic.Amministrazione.AmministraManager.findRecords(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: findRecords", e);
                result = null;
            }
            return result;
        }

        public string GetXMLLogFiltrato(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, string type, int table)
        {
            string result = "";
            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                result = log.GetXmlLogFiltrato(dataDa, dataA, user, oggetto, azione, codAmm, esito, type, table);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS GetXMLLogFiltrato: ", exception);
            }

            return result;
        }

        public string GetXMLLogAmm(string codAmm)
        {
            string result = "";
            try
            {
                BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();
                result = log.ReadWSLogAmm(codAmm);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS GetXMLLogAmm: ", exception);
            }
            return result;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.MezzoSpedizione))]
        //public ArrayList AmmListaMezzoSpedizione(string idAmm, bool vediTutti)

        public IList<MezzoSpedizione> AmmListaMezzoSpedizione(string idAmm, bool vediTutti)
        {
            ArrayList retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(idAmm, vediTutti);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetMezzoSpedizione", e);
            }

            return retValue?.OfType<MezzoSpedizione>().ToList();
        }

        public DocsPaVO.amministrazione.MezzoSpedizione AmmGetMezzoSpedizione(string idMezzoSpedizione)
        {
            DocsPaVO.amministrazione.MezzoSpedizione retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.MezziSpedizioneManager.GetMezzoSpedizione(idMezzoSpedizione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetMezzoSpedizione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmInsertMezzoSpedizione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.MezzoSpedizione m_sped, string idAmm)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.MezziSpedizioneManager.InsertMezzoSpedizione(m_sped);
                //if (retValue != null)
                //    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWTIPORUOLO", tipoRuolo.IDTipoRuolo, "Creazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWTIPORUOLO", tipoRuolo.IDTipoRuolo, "Creazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertTipoRuolo", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateMezzoSpedizione(ref DocsPaVO.amministrazione.MezzoSpedizione m_sped)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.MezziSpedizioneManager.UpdateMezzoSpedizione(m_sped);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateMezzoSpedizione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDeleteMezzoSpedizione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.MezzoSpedizione m_sped, string idAmm)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.MezziSpedizioneManager.DeleteMezzoSpedizione(m_sped);
                //if (retValue != null)
                //    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDELTIPORUOLO", tipoRuolo.IDTipoRuolo, "Cancellazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                retValue.Value = false;
                //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDELTIPORUOLO", tipoRuolo.IDTipoRuolo, "Cancellazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                //logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteTipoRuolo", e);
            }
            return retValue;
        }

        //public ArrayList getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag)

        public IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag)
        {

            var result = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliByDdlAmmPaging(idAmm, nPagina, filtriRicerca, out numTotPag);
            return result?.OfType<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione>().ToList();
        }

        public virtual string getModelloSystemId()
        {
            string systemId = string.Empty;
            try
            {
                systemId = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloSystemId();
                return systemId;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getModelloSystemId", e);
                return null;
            }
        }

        public string getSeRegistroObbl(string idAmm, string nome)
        {
            string retValue = string.Empty;
            retValue = BusinessLogic.Amministrazione.AmministraManager.getSeRegistroObbl(idAmm, nome);
            return retValue;
        }

        public virtual string salvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione, InfoUtenteAmministratore infoUtente)
        {
            SetUserId(infoUtente);
            string result = string.Empty;
            try
            {
                result = BusinessLogic.Trasmissioni.ModelliTrasmissioni.salvaModello(modelloTrasmissione);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "TRASMISSIONEINSMOD", modelloTrasmissione.SYSTEM_ID.ToString(), "Inserimento nuovo modello trasm: " + modelloTrasmissione.NOME + " da utente " + infoUtente.userId, DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaModello", e);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "TRASMISSIONEINSMOD", modelloTrasmissione.SYSTEM_ID.ToString(), "Inserimento nuovo modello trasm: " + modelloTrasmissione.NOME + " da utente " + infoUtente.userId, DocsPaVO.Logger.CodAzione.Esito.KO);
            }
            return result;
        }

        public virtual void CancellaModello(string idAmm, string idModello)
        {

            try
            {
                BusinessLogic.Trasmissioni.ModelliTrasmissioni.CancellaModello(idAmm, idModello);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: CancellaModello", e);
            }
        }

        public virtual bool existRf(string idAmministrazione)
        {
            try
            {
                return BusinessLogic.Utenti.RegistriManager.existRf(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: existRf", e);
                return false;
            }
        }

        public virtual string getNews(string idAmm)
        {
            string retValue = string.Empty;
            try
            {
                retValue = BusinessLogic.Amministrazione.AmministraManager.getNews(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getNews - ", e);
            }
            return retValue;
        }

        public virtual bool setNews(string idAmm, string news, bool enable_news)
        {
            bool retValue = false;
            try
            {
                retValue = BusinessLogic.Amministrazione.AmministraManager.setNews(idAmm, news, enable_news);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: setNews - ", e);
            }
            return retValue;
        }

        public virtual bool importaOggettario(byte[] dati, string nomeFile, string codiceAmm, bool update, ref int oggInseriti, ref int oggAggiornati, ref int oggErrati)
        {
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                bool result = BusinessLogic.Oggettario.OggettarioManager.importaOggettario(dati, nomeFile, serverPath, codiceAmm, update, ref oggInseriti, ref oggAggiornati, ref oggErrati, this.spreadsheetService);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: importaOggettario", e);
                return false;
            }
        }

        //[return: XmlArray()]
        //public virtual ArrayList getLogImportOggettario()

        public virtual IList<string> getLogImportOggettario()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                logFile = BusinessLogic.Oggettario.OggettarioManager.getLogImportOggettario(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getLogImportOggettario", e);
                return logFile.OfType<string>().ToList();
            }
        }

        // da togliere, va al documentale
        public virtual EsitoOperazione CopyVisibility(InfoUtenteAmministratore infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            try
            {
                return BusinessLogic.Amministrazione.OrganigrammaManager.CopyVisibility(infoUtente, copyVisibility);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: CopyVisibility", e);
                return null;
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.utente.ExtApplication))]
        //public virtual ArrayList GetExtApplications()
        public virtual IList<ExtApplication> GetExtApplications()
        {
            ArrayList result = null;
            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = DocsPaDB.Query_DocsPAWS.GestioneApplicazioni.GetExtApplications();
#if TRACE_WS
				pt.WriteLogTracer("GetApplicazioni");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetExtApplications", e);

                result = null;
            }
            return result?.OfType<ExtApplication>().ToList();
        }

        public virtual DocsPaVO.utente.Utente getUtenteById(string idPeople)
        {
            try
            {
                return BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getUtenteById", e);
                throw new Exception(e.Message);
            }
        }

        public DocsPaVO.Validations.ValidationResultInfo ExtAppDeleteUte(string idApplicazione, string idUtente)
        {

            ValidationResultInfo retValue = new DocsPaVO.Validations.ValidationResultInfo { Value = false };

            try
            {
                if (DocsPaDB.Query_DocsPAWS.GestioneApplicazioni.DeleteRelExtApp_Ute(idApplicazione, idUtente))
                {
                    retValue.Value = true;
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateTipoRuolo", e);
                retValue.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule { Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error, Description = e.Message });
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo ExtAppAddUte(string idApplicazione, string idUtente)
        {

            ValidationResultInfo retValue = new DocsPaVO.Validations.ValidationResultInfo { Value = false };

            try
            {
                if (DocsPaDB.Query_DocsPAWS.GestioneApplicazioni.CreateRelExtApp_Ute(idApplicazione, idUtente))
                {
                    retValue.Value = true;
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateTipoRuolo", e);
                retValue.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule { Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error, Description = e.Message });
            }

            return retValue;
        }

        public virtual DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteBySystemId(string system_id)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            try
            {
                logger.Debug("WS > AddressbookGetCorrispondenteBySystemId");
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(system_id);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AddressbookGetCorrispondenteBySystemId - ", e);
            }
            return corr;
        }

        public virtual DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteBySystemIdDisabled(string system_id)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            try
            {
                logger.Debug("WS > AddressbookGetCorrispondenteBySystemId");
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(system_id);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AddressbookGetCorrispondenteBySystemIdDisabled - ", e);
            }
            return corr;
        }

        public virtual DocsPaVO.utente.Ruolo getRuoloByIdGruppo(string idGruppo)
        {
            try
            {
                return BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoloByIdGruppo", e);
                throw new Exception(e.Message);
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.Qualifica.Qualifica))]
        public List<DocsPaVO.Qualifica.Qualifica> GetQualifiche(int id_amm)
        {
            List<DocsPaVO.Qualifica.Qualifica> list = new List<DocsPaVO.Qualifica.Qualifica>();

            try
            {
                list = BusinessLogic.Utenti.QualificheManager.GetQualifiche(id_amm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetQualifiche", e);
            }

            return list;
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.Qualifica.PeopleGroupsQualifiche))]
        public List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> GetPeopleGroupsQualifiche(String idAmm, String idUo, String idGruppo, String idPeople)
        {
            List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> list = new List<DocsPaVO.Qualifica.PeopleGroupsQualifiche>();

            try
            {
                list = BusinessLogic.Utenti.QualificheManager.GetPeopleGroupsQualifiche(idAmm, idUo, idGruppo, idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetPeopleGroupsQualifiche", e);
            }

            return list;
        }

        public DocsPaVO.Validations.ValidationResultInfo InsertPeopleGroupsQual(DocsPaVO.Qualifica.PeopleGroupsQualifiche pgq)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Utenti.QualificheManager.InsertPeopleGroupsQual(pgq);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: InsertPeopleGroupsQual", e);
            }

            return retValue;
        }

        public int GetCountProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            try
            {
                return BusinessLogic.LibroFirma.LibroFirmaManager.GetCountProcessiDiFirmaByUtenteTitolare(idUtenteCoinvolto, idRuoloCoinvolto);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetCountProcessiDiFirmaByUtenteTitolare", e);
                return 0;
            }
        }

        public int GetCountIstanzeProcessiDiFirmaByUtenteCoinvolto(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            try
            {
                return BusinessLogic.LibroFirma.LibroFirmaManager.GetCountIstanzaProcessiDiFirmaByUtenteTitolare(idUtenteCoinvolto, idRuoloCoinvolto);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetCountIstanzaProcessiDiFirmaByUtenteTitolare", e);
                return 0;
            }
        }

        // anche questo finisce per giocare con il documentale e as400, a un certo punto usa una libreria InteroperabilityController
        public bool InvalidaPassiCorrelatiTitolare(string idRuolo, string idPeople, string tipoTick, InfoUtenteAmministratore infoUtente)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();

                bool result = false;
                try
                {
                    result = BusinessLogic.LibroFirma.LibroFirmaManager.InvalidaPassiCorrelatiTitolare(idRuolo, idPeople, tipoTick, infoUtente, spreadsheetService, configurationService, _reportGeneratorService, _rubricaComuneService);
                }
                catch (Exception e)
                {
                    logger.Error("Errore in DocsPaWS.asmx  - metodo: InvalidaPassiCorrelatiByUtenteCoinvolto", e);
                }
                return result;
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRegistro))]
        //public virtual ArrayList AmmGetListRegistriAssRuolo(string idAmm, string idRuolo)
        public virtual IList<OrgRegistro> AmmGetListRegistriAssRuolo(string idAmm, string idRuolo)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListRegistriAssRuolo(idAmm, idRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRegistriAssRuolo - ", e);
            }

            return retValue.OfType<OrgRegistro>().ToList();
        }

        public bool importOrganigramma(InfoUtenteAmministratore infoUtente, byte[] dati)
        {
            SetUserId(infoUtente);
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT.PathAsUnixPath();
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                bool result = BusinessLogic.Amministrazione.OrganigrammaManager.importOrganigramma(dati, "importazioneOrganigramma.xlsx", serverPath, infoUtente, this.spreadsheetService, this._rubricaComuneService);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: importaOrganigramma" + e.Message);
                return false;
            }
        }

        public DocsPaVO.RubricaComune.ElementoRubricaUO GetElementoRubricaUO(InfoUtenteAmministratore infoUtente, string idUo)
        {
            SetUserId(infoUtente);
            return BusinessLogic.RubricaComune.RubricaServices.GetElementoRubricaUO(infoUtente, idUo);
        }

        public DocsPaVO.RubricaComune.ElementoRubricaRF GetElementoRubricaRF(InfoUtenteAmministratore infoUtente, String idRf)
        {
            SetUserId(infoUtente);
            return BusinessLogic.RubricaComune.RubricaServices.GetElementoRubricaRF(infoUtente, idRf);
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRegistro))]
        //public System.Collections.ArrayList AmmGetRegistri(string codiceAmministrazione, string chaRF)
        public IList<OrgRegistro> AmmGetRegistri(string codiceAmministrazione, string chaRF)
        {
            ArrayList retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.GetRegistri(codiceAmministrazione, chaRF);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetRegistri", e);


            }

            return retValue?.OfType<OrgRegistro>().ToList();
        }

        [XmlInclude(typeof(DocsPaVO.amministrazione.CasellaRegistro))]
        public DocsPaVO.amministrazione.CasellaRegistro[] AmmGetMailRegistro(string idRegistro)
        {
            DocsPaVO.amministrazione.CasellaRegistro[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.GetMailRegistro(idRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetMailRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.RubricaComune.ConfigurazioniRubricaComune GetConfigurazioniRubricaComune(InfoUtenteAmministratore infoUtente)
        {
            SetUserId(infoUtente);
            return BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente);
        }

        //[return: XmlArray()]
        //public virtual ArrayList getLogImportOrganigramma()
        public virtual IList<string> getLogImportOrganigramma()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT.PathAsUnixPath();

                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath.PathAsUnixPath(), DateTime.Now.ToString("yyyyMMdd"));

                logFile = BusinessLogic.Amministrazione.OrganigrammaManager.getLogImportOrganigramma(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getLogImportOrganigramma", e);
                return logFile.OfType<string>().ToList();
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUO))]
        //public virtual ArrayList AmmGetListUO(string idParent, string livello, string idAmm)
        public virtual IList<OrgUO> AmmGetListUO(string idParent, string livello, string idAmm)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListUo(idParent, livello, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListUO - ", e);
            }

            return retValue.OfType<OrgUO>().ToList();
        }

        //[return: XmlArray()]
        //public virtual ArrayList AmmListaIDParentRicerca(string IDPartenza, string tipo)
        public virtual IList<int> AmmListaIDParentRicerca(string IDPartenza, string tipo)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.AmmListaIDParentRicerca(IDPartenza, tipo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmListaIDParentRicerca - ", e);
            }

            return retValue.OfType<int>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        //		public virtual ArrayList AmmGetListRuoliUO(string idUO)
        public virtual IList<OrgRuolo> AmmGetListRuoliUO(string idUO)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUo(idUO);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRuoliUO - ", e);
            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRisultatoRicerca))]
        //		public virtual ArrayList AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
        public virtual IList<OrgRisultatoRicerca> AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.AmmRicercaInOrg(tipo, codice, descrizione, idAmm, searchHistoricized, searchByCodeExact);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmRicercaInOrg - ", e);
            }

            return retValue.OfType<OrgRisultatoRicerca>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTipoRuolo))]
        //public virtual ArrayList AmmGetListTipiRuolo(string idAmm)
        public virtual IList<OrgTipoRuolo> AmmGetListTipiRuolo(string idAmm)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListTipiRuolo(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListTipiRuolo - ", e);
            }

            return retValue.OfType<OrgTipoRuolo>().ToList();
        }

        public virtual DocsPaVO.amministrazione.OrgDettagliGlobali AmmGetDatiStampaBuste(string idCorrGlob)
        {
            DocsPaVO.amministrazione.OrgDettagliGlobali dettagli = new DocsPaVO.amministrazione.OrgDettagliGlobali();
            try
            {
                dettagli = BusinessLogic.Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetDatiStampaBuste - ", e);
            }

            return dettagli;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTipoFunzione))]
        //		public virtual ArrayList AmmGetListFunzioni(string idAmm, string idRuolo)
        public virtual IList<OrgTipoFunzione> AmmGetListFunzioni(string idAmm, string idRuolo)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListFunzioni(idAmm, idRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListFunzioni - ", e);
            }

            return retValue.OfType<OrgTipoFunzione>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRegistro))]
        //		public virtual ArrayList AmmGetListRegistriRF(string idAmm, string idRuolo, string chaRF)
        public virtual IList<OrgRegistro> AmmGetListRegistriRF(string idAmm, string idRuolo, string chaRF)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListRegistriRF(idAmm, idRuolo, chaRF);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRegistriRF - ", e);

            }

            return retValue.OfType<OrgRegistro>().ToList();
        }

        public virtual DocsPaVO.amministrazione.OrgUtente AmmGetDatiUtente(string idCorrGlob)
        {
            DocsPaVO.amministrazione.OrgUtente datiUtente = new DocsPaVO.amministrazione.OrgUtente();
            try
            {
                datiUtente = BusinessLogic.Amministrazione.OrganigrammaManager.GetDatiUtente(idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetDatiUtente - ", e);
            }

            return datiUtente;
        }

        public virtual void resetHashTable()
        {
            BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.resetHashTable();
        }

        public virtual DataSet isCorrInListaDistr(string idCorr)
        {
            try
            {
                return BusinessLogic.Rubrica.ListeDistribuzione.isCorrInListaDistr(idCorr);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isCorrInListaDistr", e);
                return null;
            }
        }

        public DocsPaVO.LibroFirma.ProcessoFirma[] GetProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            List<DocsPaVO.LibroFirma.ProcessoFirma> listaProcessiDiFirma = new List<DocsPaVO.LibroFirma.ProcessoFirma>();
            numTotPage = 0;
            nRec = 0;
            try
            {
                listaProcessiDiFirma = BusinessLogic.LibroFirma.LibroFirmaManager.GetProcessiDiFirmaByTitolarePaging(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetProcessiDiFirmaByTitolarePaging ", e);
            }
            return listaProcessiDiFirma.ToArray();
        }

        public string GetDescDiagrammiByIdProcesso(string idProcesso)
        {
            string result = string.Empty;
            try
            {
                result = BusinessLogic.LibroFirma.LibroFirmaManager.GetDescDiagrammiByIdProcesso(idProcesso);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetDescDiagrammiByIdProcesso", e);
            }
            return result;
        }

        [XmlInclude(typeof(DocsPaVO.amministrazione.SaveChangesToRoleReport))]
        public SaveChangesToRoleResponse SaveChangesToRole(SaveChangesToRoleRequest request)
        {
            return BusinessLogic.Amministrazione.OrganigrammaManager.SaveChangesToRole(request, _rubricaComuneService);
        }

        public bool AmmStoricizzaRuoloPassiCorrelati(string idRuoloOld, string idRuoloNew)
        {
            bool result = false;
            try
            {
                result = BusinessLogic.LibroFirma.LibroFirmaManager.AmmStoricizzaRuoloPassiCorrelati(idRuoloOld, idRuoloNew);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: AmmStoricizzaRuoloPassiCorrelati", e);
            }
            return result;
        }

        [Obsolete]
        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmSpostaRuolo(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmSpostaRuolo(infoUtente, ruolo);
                //if (esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSPOSTAROLE", ruolo.IDCorrGlobale, "Spostamento ruolo " + ruolo.Codice + " - " + ruolo.Descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, ruolo.IDAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSPOSTAROLE", ruolo.IDCorrGlobale, "Spostamento ruolo " + ruolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, ruolo.IDAmministrazione);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmSpostaRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        //[return: XmlArray]
        //[return: XmlArrayItem(typeof(string))]
        //		public virtual ArrayList GetListaParentUo(string idUo, string livelloUo)
        public virtual IList<string> GetListaParentUo(string idUo, string livelloUo)
        {
            ArrayList listaUo = new ArrayList();
            try
            {
                listaUo = BusinessLogic.Utenti.addressBookManager.GetParentUo(idUo, livelloUo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetListaParentUo", e);
            }
            return listaUo.OfType<string>().ToList();
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmVerificaUtenteLoggato(string userId, string idAmm)
        {
            SetUserId(userId);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmVerificaUtenteLoggato(userId, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmVerificaUtenteLoggato - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public int GetCountProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            int result = 0;
            try
            {
                result = BusinessLogic.LibroFirma.LibroFirmaManager.GetCountProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetCountProcessiDiFirmaByTitolare", e);
            }
            return result;
        }

        public int GetCountIstanzaProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            int result = 0;
            try
            {
                result = BusinessLogic.LibroFirma.LibroFirmaManager.GetCountIstanzaProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetCountIstanzaProcessiDiFirmaByTitolare", e);
            }
            return result;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUtenteInRuolo(InfoUtenteAmministratore infoUtente, string idPeople, string idGruppo, string idAmm)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idGruppo);
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUtenteInRuolo(infoUtente, idPeople, idGruppo);
                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
                string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.GetCodAmmById(idAmm);
                //if (esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISASSRUOLOUSER", idPeople, "Disassociazione ruolo " + ruolo.codiceRubrica + "(" + ruolo.descrizione + ")" + " utente " + utente.userId + "(" + utente.descrizione + ")", DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISASSRUOLOUSER", idPeople, "Disassociazione ruolo " + idGruppo + " utente " + idPeople, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaUtenteInRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaADLUtente(idPeople, idCorrGlobGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaADLUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmVerificaTrasmRuolo(string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmVerificaTrasmRuolo(idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmVerificaTrasmRuolo - ", e);
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmRifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmRifiutaTrasmConWF(idCorrGlobRuolo, idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmRifiutaTrasmConWF - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsUtenteInRuolo(InfoUtenteAmministratore infoUtente, string idPeople, string idGruppo, string idAmm, string type)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsUtenteInRuolo(infoUtente, idPeople, idGruppo);
                //if (esito.Codice == 0)
                //{
                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idGruppo);
                string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.GetCodAmmById(idAmm);

                if (type.Equals("newUser"))
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMASSRUOLOUSER", idPeople, "Associazione ruolo " + ruolo.codice + "(" + ruolo.descrizione + ") utente " + utente.userId + "(" + utente.descrizione + ")", DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSPOSTAUSER", idPeople, "Spostamento utente " + utente.userId + " - " + utente.descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
                //}
            }
            catch (Exception e)
            {
                if (type.Equals("newUser"))
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMASSRUOLOUSER", idPeople, "Associazione ruolo " + idGruppo + " utente " + idPeople, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSPOSTAUSER", idPeople, "Spostamento utente " + idPeople, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsUtenteInRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsTrasmUtente(idPeople, idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsTrasmUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgNodoTitolario))]
        //public virtual ArrayList AmmGetNodiTitolario_InRuolo(string idGruppo)
        public virtual IList<OrgNodoTitolario> AmmGetNodiTitolario_InRuolo(string idGruppo)
        {
            ArrayList retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TitolarioManager.GetNodiTitolario_InRuolo(idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetNodiTitolario_InRuolo", e);
            }

            return retValue?.OfType<OrgNodoTitolario>().ToList();
        }

        public string filtroRicercaTitAmm(string codice, string descrizione, string codAmm, string idRegistro)
        {
            string result = "";
            try
            {
                BusinessLogic.AmministrazioneXml.TitolarioXml obj = new BusinessLogic.AmministrazioneXml.TitolarioXml();
                result = obj.filtroRicTitAmm(codice, descrizione, codAmm, idRegistro);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS filtroRicercaTitAmm: ", exception);
            }
            return result;
        }

        // questo va a finire nel documentale
        public DocsPaVO.amministrazione.EsitoOperazione AmmExtendToChildNodes(InfoUtenteAmministratore infoUtente, string idNodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario, string idAmm, string idRegistro, bool check)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            logger.Debug("Inizio procedura di inserimento o eliminazione visibilità a tutti i nodi titolario figli di: " + idNodoTitolario);

            try
            {
                esito = BusinessLogic.Amministrazione.TitolarioManager.AmmExtendToChildNodes(infoUtente, idNodoTitolario, ruoloTitolario, idAmm, idRegistro, check);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmExtendToChildNodes", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore durante la procedura di inserimento dei diritti sui nodi del titolario";
            }

            logger.Debug("Fine procedura di inserimento visibilità");

            return esito;
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.amministrazione.EsitoOperazione))]
        public virtual DocsPaVO.amministrazione.EsitoOperazione[] AmmUpdateRuoliTitolario(
            InfoUtenteAmministratore infoUtente,
            string idTitolario,
            DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario,
            DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolarioDisattiva,
            string idAmm,
            bool allTitolario,
            string idRegistro)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TitolarioManager.UpdateRuoliTitolario(infoUtente, idTitolario, ruoliTitolario, ruoliTitolarioDisattiva, allTitolario, idAmm, idRegistro);
                if (retValue != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMVISNODO", idTitolario, "Imposta visibilita nodoTitolario " + idTitolario, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMVISNODO", idTitolario, "Imposta visibilita nodoTitolario " + idTitolario, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateRuoliTitolario", e);
            }

            return retValue;
        }

        public DocsPaVO.amministrazione.PasswordConfigurations AdminGetPasswordConfigurations(InfoUtenteAmministratore infoUtente, int idAmministrazione)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.AdminPasswordConfig.GetPasswordConfigurations(infoUtente, idAmministrazione);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx  - metodo: AdminGetPasswordConfigurations", e);
            }
        }

        public bool AdminSavePasswordConfigurations(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.PasswordConfigurations configurations)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.AdminPasswordConfig.SavePasswordConfigurations(infoUtente, configurations);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx  - metodo: AdminSavePasswordConfigurations", e);
            }
        }

        public void AdminExpireAllPassword(InfoUtenteAmministratore infoUtente, int idAmministrazione)
        {
            SetUserId(infoUtente);
            try
            {
                BusinessLogic.Amministrazione.AdminPasswordConfig.ExpireAllPassword(infoUtente, idAmministrazione);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                        "MODUSER", infoUtente.idCorrGlobali,
                        string.Format("Richiesta di invalidare tutte le password da parte dell'amministratore {0}",
                            infoUtente.userId),
                            DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                        "MODUSER", infoUtente.idCorrGlobali,
                        string.Format("Errore nella richiesta di invalidare tutte le password da parte dell'amministratore {0}",
                            infoUtente.userId),
                            DocsPaVO.Logger.CodAzione.Esito.KO);
                throw new ApplicationException("Errore in DocsPaWS.asmx  - metodo: AdminExpireAllPassword", e);
            }
        }

        public virtual void salvaAssociazioneModelli(string idTipoDoc, string idDiagramma, ArrayList modelliSelezionati, string idStato)
        {
            try
            {
                var converted = new ArrayList();
                foreach (XmlNode[] modelloXml in modelliSelezionati)
                {

                    var elemento = new DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                            case "ID_TIPO_DOC":
                                elemento.ID_TIPO_DOC = field.InnerText;
                                break;
                            case "ID_DIAGRAMMA":
                                elemento.ID_DIAGRAMMA = field.InnerText;
                                break;
                            case "ID_TEMPLATE":
                                elemento.ID_TEMPLATE = field.InnerText;
                                break;
                            case "ID_STATO":
                                elemento.ID_STATO = field.InnerText;
                                break;
                            case "TRASM_AUT":
                                elemento.TRASM_AUT = field.InnerText;
                                break;
                        }
                    }
                    converted.Add(elemento);
                }

                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaAssociazioneModelli(idTipoDoc, idDiagramma, converted /*modelliSelezionati*/, idStato);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaAssociazioneModelli", e);
            }

        }

        //public virtual ArrayList getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca)
        public virtual IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca)
        {
            ArrayList listaModelli = new ArrayList();
            try
            {
                listaModelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliByAmmConRicerca(idAmm, codiceRicerca, tipoRicerca);
                return listaModelli.OfType<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getModelliByAmmConRicerca", e);
                return null;
            }
        }

        //[return: XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm))]
        //public virtual ArrayList getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato)
        public virtual IList<DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm> getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getIdModelliTrasmAssociati(idTipoDoc, idDiagramma, idStato);
                return result.OfType<DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getIdModelliTrasmAssociati", e);
                return null;
            }
        }

        //public virtual ArrayList getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo)
        public virtual IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo)
        {
            ArrayList listaModelli = new ArrayList();
            try
            {
                listaModelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliAssDiagrammi(idTipo, idDiagramma, stato, idAmm, selezionati, tipo);
                return listaModelli.OfType<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getModelliAssDiagrammi", e);
                return null;
            }
        }

        //public virtual ArrayList getModelliByAmmLite(string idAmm)
        public virtual IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliByAmmLite(string idAmm)
        {
            ArrayList listaModelli = new ArrayList();
            try
            {
                listaModelli = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelliByAmmLite(idAmm);
                return listaModelli.OfType<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getModelliByAmm", e);
                return null;
            }
        }

        //        public virtual ArrayList getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca)
        public virtual IList<DocsPaVO.utente.Ruolo> getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca)
        {
            ArrayList listaRuoli = new ArrayList();
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliByAmm(idAmm, codiceRicerca, tipoRicerca);
                return result.OfType<DocsPaVO.utente.Ruolo>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoliByAmm", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //public virtual ArrayList getTemplatesArchivioDeposito(DocsPaVO.utente.InfoUtente infoUtente, string idAmm, bool seRepertorio)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.Templates> getTemplatesArchivioDeposito(InfoUtenteAmministratore infoUtente, string idAmm, bool seRepertorio)
        {
            ArrayList result = null;
            try
            {
                SetUserId(infoUtente);
                result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplatesArchivioDeposito(idAmm, seRepertorio);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplatesArchivioDeposito", e);
                result = null;
            }
            return result?.OfType<DocsPaVO.ProfilazioneDinamica.Templates>().ToList();
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateById(string idTemplate)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplateById", e);
                return null;
            }
        }

        public bool UpdateAssociazioneTemplateContestoProcedurale(string idTipoAtto, string idContestoProcedurale, InfoUtenteAmministratore infoUente)
        {
            return BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.UpdateAssociazioneTemplateContestoProcedurale(idTipoAtto, idContestoProcedurale);
        }

        public bool InsertContestoProcedurale(DocsPaVO.FlussoAutomatico.ContestoProcedurale contesto, InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.InsertContestoProcedurale(contesto);
        }

        public List<DocsPaVO.FlussoAutomatico.ContestoProcedurale> GetListContestoProcedurale(InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.GetListContestoProcedurale();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli))]
        //        public virtual ArrayList getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliFromOggettoCustomDoc(idTemplate, idOggettoCustom);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoliFromOggettoCustomDoc", e);
                return null;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.OggettoCustom getOggettoById(string idOggetto)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getOggettoById(idOggetto);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getOggettoById", e);
                throw e;
            }
        }

        public DocsPaVO.ProfilazioneDinamica.Contatore[] GetValuesContatoriDoc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.GetValuesContatoriDoc(oggettoCustom);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel - metodo: GetValuesContatoriDoc", ex);
                return null;
            }
        }

        public void DeleteValueContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.DeleteValueContatoreDoc(contatore);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel - metodo: deleteValueContatoreDoc", ex);
            }
        }

        public DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse GetCustomHistoryList(DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest request)
        {
            DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse response = new DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse();
            try
            {
                switch (request.ObjType)
                {
                    case DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest.ObjectType.Document:
                        response = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.GetCustomHistoryList(request);
                        break;
                    case DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest.ObjectType.Folder:
                        response = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.GetCustomHistoryList(request);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {

            }

            return response;
        }

        public DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse ActiveSelectiveHistory(DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest request)
        {
            // Risultato dell'elaborazione
            DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse retVal = new DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse();

            try
            {
                switch (request.ObjType)
                {
                    case DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest.ObjectType.Document:
                        retVal = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.ActiveSelectiveHistory(request);
                        break;
                    case DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest.ObjectType.Folder:
                        retVal = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.ActiveSelectiveHistory(request);
                        break;

                }
            }
            catch (Exception e)
            {

            }

            return retVal;
        }

        public bool isInUseCampoComuneDoc(string idTemplate, string idCampoComune)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.isInUseCampoComuneDoc(idTemplate, idCampoComune);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isInUseCampoComuneDoc", e);
                return false;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates eliminaOggettoCustomTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoCustom)
        {
            try
            {
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoC = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoCustom];
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.eliminaOggettoCustomDaDB(oggettoC, template);
                template.ELENCO_OGGETTI.RemoveAt(oggettoCustom);

                for (int i = oggettoCustom; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    oggettoC = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                    if (oggettoC.POSIZIONE != null && oggettoC.POSIZIONE != "")
                    {
                        int posizione = Convert.ToInt32(oggettoC.POSIZIONE);
                        posizione--;
                        oggettoC.POSIZIONE = Convert.ToString(posizione);
                        BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.aggiornaPosizione(oggettoC, template);
                    }
                }
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: eliminaOggettoCustomTemplate", e);
                return null;
            }
        }

        public virtual int countDocTipoDoc(string tipo_atto, string codiceAmm)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.countDocTipoDoc(tipo_atto, codiceAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: countDocTipoDoc", e);
                return 0;
            }
        }

        public virtual void updatePrivatoTipoDoc(int systemId_template, string privato)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdatePrivatoTipoDoc(systemId_template, privato);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updatePrivatoTipoDoc", e);
            }
        }

        public virtual void updateMesiConsTipoDoc(int systemId_template, string mesiCons)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdateMesiConsTipoDoc(systemId_template, mesiCons);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updateMesiCosnTipoDoc", e);
            }
        }

        public virtual void updateInvioConsTipoDoc(int systemId_template, string invioCons)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdateInvioConsTipoDoc(systemId_template, invioCons);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: updateInvioConsTipoDoc", ex);
            }
        }

        public virtual void updateConsolidamentoOggCustom(int systemId, string consolida, string systemId_template)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdateConsolidaCampo(systemId, consolida, systemId_template);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: updateConsolidamentoOggCustom", ex);
            }
        }

        public virtual void updateConservazioneOggCustom(int systemId, string conserva, string systemId_template)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdateConservaCampo(systemId, conserva, systemId_template);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: updateConservazioneOggCustom", ex);
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates aggiungiOggettoCustomTemplate(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            try
            {
                template.ELENCO_OGGETTI.Add(oggettoCustom);
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: aggiungiOggettoCustomTemplate", e);
                return null;
            }
        }

        public virtual bool isValueInUse(string idOggetto, string idTemplate, string valoreOggettoDB)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.isValueInUse(idOggetto, idTemplate, valoreOggettoDB);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isValueInUse", e);
                return false;
            }
        }

        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli))]
        //public virtual ArrayList getRuoliTipoDoc(string idTipoDoc)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliTipoDoc(string idTipoDoc)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(idTipoDoc);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoliTipoDoc", e);
                return null;
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.documento.TipologiaAtto))]
        //public virtual ArrayList GetTipologiaAttoProfDin(string idAmministrazione)
        public virtual IList<DocsPaVO.documento.TipologiaAtto> GetTipologiaAttoProfDin(string idAmministrazione)
        {
            ArrayList result = null;

            try
            {
#if TRACE_WS
					DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Documenti.DocManager.getTipologiaAtto(idAmministrazione);
#if TRACE_WS
					pt.WriteLogTracer("DocumentoGetTipologiaAtto");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DocumentoGetTipologiaAtto", e);
                result = null;
            }
            return result?.OfType<DocsPaVO.documento.TipologiaAtto>().ToList();
        }

        public virtual bool salvaTemplate(InfoUtenteAmministratore infoUtente, DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            SetUserId(infoUtente);
            bool retValue = false;
            try
            {
                retValue = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaTemplate(template, idAmministrazione);
                if (retValue)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWDOC", template.SYSTEM_ID.ToString(), "Creazione tipo documento " + template.DESCRIZIONE, DocsPaVO.Logger.CodAzione.Esito.OK, idAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWDOC", template.SYSTEM_ID.ToString(), "Creazione tipo documento " + template.DESCRIZIONE, DocsPaVO.Logger.CodAzione.Esito.KO, idAmministrazione);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaTemplate", e);
                retValue = false;
            }
            return retValue;
        }

        public virtual bool aggiornaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.aggiornaTemplate(template);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: aggiornaTemplate", e);
                return false;
            }
        }

        public virtual void messaInEserizioTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.messaInEsercizioTemplate(template, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: messaInEserizioTemplate", e);
            }
        }

        public bool UpdateIsTypeInstance(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            bool result = false;
            try
            {
                result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdateIsTypeInstance(template, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: UpdateIsTypeInstance", e);
            }
            return result;
        }

        // sistemare il Server.MapPath
        public virtual bool disabilitaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string codiceAmministrazione)
        {
            try
            {
                string serverPath = ""; // this.Server.MapPath("xml");
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.disabilitaTemplate(template, idAmministrazione, serverPath, codiceAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: disabilitaTemplate", e);
                return false;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates spostaOggetto(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoSelezionato, string spostamento)
        {
            try
            {
                if (spostamento.Equals("UP"))
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    oggettoCustom_1.POSIZIONE = (Convert.ToInt32(oggettoCustom_1.POSIZIONE) - 1).ToString();
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato - 1];
                    oggettoCustom_2.POSIZIONE = (Convert.ToInt32(oggettoCustom_2.POSIZIONE) + 1).ToString();
                    //update posizione
                    BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.aggiornaPosizioni(oggettoCustom_1, oggettoCustom_2, template);
                    template.ELENCO_OGGETTI.Reverse(oggettoSelezionato - 1, 2);
                }
                if (spostamento.Equals("DOWN"))
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    oggettoCustom_1.POSIZIONE = (Convert.ToInt32(oggettoCustom_1.POSIZIONE) + 1).ToString();
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato + 1];
                    oggettoCustom_2.POSIZIONE = (Convert.ToInt32(oggettoCustom_2.POSIZIONE) - 1).ToString();
                    //update posizione
                    BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.aggiornaPosizioni(oggettoCustom_1, oggettoCustom_2, template);
                    template.ELENCO_OGGETTI.Reverse(oggettoSelezionato, 2);
                }
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: spostaOggetto", e);
                return null;
            }
        }

        public virtual void updateScadenzeTipoDoc(int systemId_template, string scadenza, string preScadenza)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.updateScadenzeTipoDoc(systemId_template, scadenza, preScadenza);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updateScadenzeTipoDoc", e);
            }
        }

        public virtual bool associaTipoDocDiagramma(string idTipoDoc, string idDiagramma)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.associaTipoDocDiagramma(idTipoDoc, idDiagramma);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: associaTipoDocDiagramma", e);
                return false;
            }
        }

        private void SetUserId(InfoUtenteAmministratore infoUtente)
        {
            if (infoUtente != null) SetUserId(infoUtente.userId);
        }

        // controlla la compatibilità di LogicalThreadContext su dotnet.core
        private void SetUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) //LogicalThreadContext.Properties["userId"] = userId.ToUpper();
                LogContext.PushProperty("userId", userId.ToUpper());
        }

        public virtual int getDiagrammaAssociato(string idTipoDoc)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(idTipoDoc);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getDiagrammaAssociato", e);
                return 0;
            }
        }

        public virtual bool isDocumentiInStatoFinale(string idDiagramma, string idTemplate)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.isDocumentiInStatoFinale(idDiagramma, idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isDocumentiInStatoFinale", e);
                return false;
            }
        }

        public virtual void disassociaTipoDocDiagramma(string idTipoDoc)
        {
            try
            {
                BusinessLogic.DiagrammiStato.DiagrammiStato.disassociaTipoDocDiagramma(idTipoDoc);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: disassociaTipoDocDiagramma", e);
            }
        }

        public virtual void salvaDirittiCampiTipologiaDoc(ArrayList listaDirittiCampiSelezionati)
        {
            try
            {
                var converted = new ArrayList();

                foreach (XmlNode[] ruolo in listaDirittiCampiSelezionati)
                {
                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in ruolo)
                    {
                        switch (field.Name)
                        {
                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "ID_OGGETTO_CUSTOM":
                                elemento.ID_OGGETTO_CUSTOM = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                            case "INS_MOD_OGG_CUSTOM":
                                elemento.INS_MOD_OGG_CUSTOM = field.InnerText;
                                break;
                            case "VIS_OGG_CUSTOM":
                                elemento.VIS_OGG_CUSTOM = field.InnerText;
                                break;
                            case "ANNULLA_REPERTORIO":
                                elemento.ANNULLA_REPERTORIO = field.InnerText;
                                break;
                        }
                    }
                    converted.Add(elemento);
                }

                //BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaDirittiCampiTipologiaDoc(listaDirittiCampiSelezionati);
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaDirittiCampiTipologiaDoc(converted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaDirittiCampiTipologiaDoc", e);
            }

        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli))]
        //		public virtual ArrayList getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(idRuolo, idTemplate);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getDirittiCampiTipologiaDoc", e);
                return null;
            }

        }

        public virtual void estendiDirittiCampiARuoliDoc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {
            try
            {
                var listaDirittiCampiSelezionatiConverted = new ArrayList();
                var listaRuoliConverted = new ArrayList();

                foreach (XmlNode[] campo in listaDirittiCampiSelezionati)
                {
                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in campo)
                    {
                        switch (field.Name)
                        {

                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "ID_OGGETTO_CUSTOM":
                                elemento.ID_OGGETTO_CUSTOM = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                            case "INS_MOD_OGG_CUSTOM":
                                elemento.INS_MOD_OGG_CUSTOM = field.InnerText;
                                break;
                            case "VIS_OGG_CUSTOM":
                                elemento.VIS_OGG_CUSTOM = field.InnerText;
                                break;
                            case "ANNULLA_REPERTORIO":
                                elemento.ANNULLA_REPERTORIO = field.InnerText;
                                break;
                        }
                    }
                    listaDirittiCampiSelezionatiConverted.Add(elemento);
                }

                foreach (XmlNode[] ruolo in listaRuoli)
                {
                    var elemento = new Ruolo
                    {
                        idGruppo = ruolo.Where(f => f.Name == "idGruppo").Select(f => f.InnerText).FirstOrDefault()
                    };

                    listaRuoliConverted.Add(elemento);
                }

                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.estendiDirittiCampiARuoliDoc(listaDirittiCampiSelezionatiConverted, listaRuoliConverted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: estendiDirittiCampiARuoli", e);
            }

        }

        public virtual byte[] GetFileFromPath(string path)
        {
            byte[] result;
            try
            {
                result = BusinessLogic.Modelli.ModelliManager.GetFileFromPath(path.PathAsUnixPath());
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca del file: " + e.Message);
                throw e;
            }
            return result;
        }

        public virtual void salvaAssociazioneModelliFasc(string idTipoFasc, string idDiagramma, ArrayList modelliSelezionati, string idStato)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaAssociazioneModelliFasc(idTipoFasc, idDiagramma, modelliSelezionati, idStato);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaAssociazioneModelliFasc", e);
            }

        }

        //[return: XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm))]
        //public virtual ArrayList getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato)

        public virtual IList<DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm> getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getIdModelliTrasmAssociatiFasc(idTipoFasc, idDiagramma, idStato);
                return result.OfType<DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getIdModelliTrasmAssociatiFasc", e);
                return null;
            }
        }

        // DocsPaVO.ProfilazioneDinamica.Templates template
        //public virtual ArrayList getTemplatesFasc(string idAmministrazione)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.Templates> getTemplatesFasc(string idAmministrazione)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplatesFasc(idAmministrazione);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.Templates>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplatesFasc", e);
                return null;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascById(string idTemplate)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplateFascById", e);
                return null;
            }
        }

        public DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniFasc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.impostaCampiComuniFasc(modello, campiComuni);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: impostaCampiComuniFasc", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli))]
        //public virtual ArrayList getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getRuoliFromOggettoCustomFasc(idTemplate, idOggettoCustom);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoliFromOggettoCustomFasc", e);
                return null;
            }

        }

        public EsitoOperazione SaveStrutturaTemplateRelation(int idtipofascicolo, int idtitolario, int idtemplate)
        {
            EsitoOperazione result = new EsitoOperazione() { Codice = 0, Descrizione = "Esito positivo" };

            try
            {
                BusinessLogic.Amministrazione.StruttureSottofascicoliManager.saveStrutturaSottofascicoli(idtipofascicolo, idtitolario, idtemplate);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SaveStrutturaSottofascicoli", ex);
                result.Codice = -1;
                result.Descrizione = ex.Message;
            }

            return result;
        }

        public bool isInUseCampoComuneFasc(string idTemplate, string idCampoComune)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.isInUseCampoComuneFasc(idTemplate, idCampoComune);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isInUseCampoComuneFasc", e);
                return false;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates eliminaOggettoCustomTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoCustom)
        {
            try
            {
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoC = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoCustom];
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.eliminaOggettoCustomDaDBFasc(oggettoC, template);
                template.ELENCO_OGGETTI.RemoveAt(oggettoCustom);

                for (int i = oggettoCustom; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    oggettoC = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                    if (oggettoC.POSIZIONE != null && oggettoC.POSIZIONE != "")
                    {
                        int posizione = Convert.ToInt32(oggettoC.POSIZIONE);
                        posizione--;
                        oggettoC.POSIZIONE = Convert.ToString(posizione);
                        BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.aggiornaPosizioneFasc(oggettoC, template);
                    }
                }
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: eliminaOggettoCustomTemplateFasc", e);
                return null;
            }
        }

        public virtual int countFascTipoFasc(string tipo_fasc, string codiceAmm)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.countFascTipoFasc(tipo_fasc, codiceAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: countFascTipoFasc", e);
                return 0;
            }
        }

        public virtual void updatePrivatoTipoFasc(int systemId_template, string privato)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.UpdatePrivatoTipoFasc(systemId_template, privato);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updatePrivatoTipoFasc", e);
            }
        }

        public virtual void updateMesiConsTipoFasc(int systemId_template, string mesiCons)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.UpdateMesiConsTipoFasc(systemId_template, mesiCons);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updateMesiCosnTipoFasc", e);
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates aggiungiOggettoCustomTemplateFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            try
            {
                template.ELENCO_OGGETTI.Add(oggettoCustom);
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: aggiungiOggettoCustomTemplateFasc", e);
                return null;
            }
        }

        public virtual bool isValueInUseFasc(string idOggetto, string idTemplate, string valoreOggettoDB)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.isValueInUseFasc(idOggetto, idTemplate, valoreOggettoDB);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isValueInUse", e);
                return false;
            }
        }

        [return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli))]
        //public virtual ArrayList getRuoliTipoFasc(string idTipoFasc)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliTipoFasc(string idTipoFasc)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getRuoliTipoFasc(idTipoFasc);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoliTipoDoc", e);
                return null;
            }
        }

        public virtual bool salvaTemplateFasc(InfoUtenteAmministratore infoUtente, DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            SetUserId(infoUtente);
            bool retValue = false;
            try
            {
                retValue = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaTemplateFasc(template, idAmministrazione);
                if (retValue)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWFASC", template.SYSTEM_ID.ToString(), "Creazione tipo fascicolo " + template.DESCRIZIONE, DocsPaVO.Logger.CodAzione.Esito.OK, idAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWFASC", template.SYSTEM_ID.ToString(), "Creazione tipo fascicolo " + template.DESCRIZIONE, DocsPaVO.Logger.CodAzione.Esito.KO, idAmministrazione);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaTemplateFasc", e);
                retValue = false;
            }
            return retValue;
        }

        public virtual bool aggiornaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            try
            {
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.aggiornaTemplateFasc(template);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: aggiungiOggettoValoreOggettoCustom", e);
                return false;
            }
        }

        public virtual void messaInEserizioTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.messaInEsercizioTemplateFasc(template, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: messaInEserizioTemplateFasc", e);
            }
        }

        // sistemare il Server.MapPath
        public virtual bool disabilitaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string codiceAmministrazione)
        {
            try
            {
                string serverPath = ""; // this.Server.MapPath("xml");
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.disabilitaTemplateFasc(template, idAmministrazione, serverPath, codiceAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: disabilitaTemplateFasc", e);
                return false;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascCampiComuniById(InfoUtenteAmministratore infoUtente, string idTemplate)
        {
            try
            {
                SetUserId(infoUtente);
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascCampiComuniById(infoUtente, idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplateFascCampiComuniById", e);
                return null;
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates spostaOggettoFasc(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoSelezionato, string spostamento)
        {
            try
            {
                if (spostamento.Equals("UP"))
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    oggettoCustom_1.POSIZIONE = (Convert.ToInt32(oggettoCustom_1.POSIZIONE) - 1).ToString();
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato - 1];
                    oggettoCustom_2.POSIZIONE = (Convert.ToInt32(oggettoCustom_2.POSIZIONE) + 1).ToString();
                    //update posizione
                    BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.aggiornaPosizioniFasc(oggettoCustom_1, oggettoCustom_2, template);
                    template.ELENCO_OGGETTI.Reverse(oggettoSelezionato - 1, 2);
                }
                if (spostamento.Equals("DOWN"))
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato];
                    oggettoCustom_1.POSIZIONE = (Convert.ToInt32(oggettoCustom_1.POSIZIONE) + 1).ToString();
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2 = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[oggettoSelezionato + 1];
                    oggettoCustom_2.POSIZIONE = (Convert.ToInt32(oggettoCustom_2.POSIZIONE) - 1).ToString();
                    //update posizione
                    BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.aggiornaPosizioniFasc(oggettoCustom_1, oggettoCustom_2, template);
                    template.ELENCO_OGGETTI.Reverse(oggettoSelezionato, 2);
                }
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: spostaOggettoFasc", e);
                return null;
            }
        }

        public virtual void updateScadenzeTipoFasc(int systemId_template, string scadenza, string preScadenza)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.updateScadenzeTipoFasc(systemId_template, scadenza, preScadenza);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updateScadenzeTipoFasc", e);
            }
        }

        public virtual void salvaDirittiCampiTipologiaFasc(ArrayList listaDirittiCampiSelezionati)
        {
            try
            {
                var converted = new ArrayList();

                foreach (XmlNode[] ruolo in listaDirittiCampiSelezionati)
                {
                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in ruolo)
                    {
                        switch (field.Name)
                        {

                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "ID_OGGETTO_CUSTOM":
                                elemento.ID_OGGETTO_CUSTOM = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                            case "INS_MOD_OGG_CUSTOM":
                                elemento.INS_MOD_OGG_CUSTOM = field.InnerText;
                                break;
                            case "VIS_OGG_CUSTOM":
                                elemento.VIS_OGG_CUSTOM = field.InnerText;
                                break;
                        }
                    }
                    converted.Add(elemento);
                }
                //BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaDirittiCampiTipologiaFasc(listaDirittiCampiSelezionati);
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaDirittiCampiTipologiaFasc(converted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaDirittiCampiTipologiaFasc", e);
            }
        }

        public virtual void salvaAssociazioneFascRuoli(ArrayList assFascRuoli)
        {
            try
            {
                var converted = new ArrayList();

                foreach (XmlNode[] ruolo in assFascRuoli)
                {
                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in ruolo)
                    {
                        switch (field.Name)
                        {
                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "DIRITTI_TIPOLOGIA":
                                elemento.DIRITTI_TIPOLOGIA = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                        }
                    }
                    converted.Add(elemento);
                }
                //BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaAssociazioneFascRuoli(assFascRuoli);
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaAssociazioneFascRuoli(converted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaAssociazioneDocRuoli", e);
            }

        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli))]
        //		public virtual ArrayList getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getDirittiCampiTipologiaFasc(idRuolo, idTemplate);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getDirittiCampiTipologiaFasc", e);
                return null;
            }
        }

        public virtual void estendiDirittiCampiARuoliFasc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {
            try
            {
                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.estendiDirittiCampiARuoliFasc(listaDirittiCampiSelezionati, listaRuoli);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: estendiDirittiCampiARuoliFasc", e);
            }

        }

        public DocsPaVO.Validations.ValidationResultInfo InsertQual(DocsPaVO.Qualifica.Qualifica qual)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Utenti.QualificheManager.InsertQual(qual);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: InsertQual", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo UpdateQual(String idQualifica, String descrizione)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Utenti.QualificheManager.UpdateQual(idQualifica, descrizione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: UpdateQual", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo DeleteQual(String idQualifica, int idAmministrazione)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Utenti.QualificheManager.DeleteQual(idQualifica, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DeleteQual", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateMessageNotificaRagioneTrasmissione(string codiceRagione, string idAmministrazione, string msgNotificaDocumenti, string msgNotificaFascioli, bool allRagioniDoc, bool allRagioniFasc)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.UpdateMessageNotificaRagioneTrasmissione(codiceRagione, idAmministrazione, msgNotificaDocumenti, msgNotificaFascioli, allRagioniDoc, allRagioniFasc);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateMsgNotificaRagioneTrasmissione", e);
            }

            return retValue;
        }

        public DocsPaVO.amministrazione.OrgRagioneTrasmissione AmmGetRagioneTrasmissione(string idRagione)
        {
            DocsPaVO.amministrazione.OrgRagioneTrasmissione retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.GetRagioneTrasmissione(idRagione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetRagioneTrasmissione", e);
            }

            return retValue;
        }

        [return: XmlArray()]
        public DocsPaVO.amministrazione.OrgRagioneTrasmissione[] AmmGetRagioniTrasmissione(string idAmministrazione)
        {
            DocsPaVO.amministrazione.OrgRagioneTrasmissione[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.GetRagioniTrasmissione(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetRagioniTrasmissione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmInsertRagioneTrasmissione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione, string idAmm)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.InsertRagioneTrasmissione(ragione);
                if (retValue != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWTRASM", ragione.ID, "Creazione nuova ragione di trasmissione " + ragione.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWTRASM", ragione.ID, "Creazione nuova ragione di trasmissione " + ragione.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertRagioneTrasmissione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateRagioneTrasmissione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione, string idAmm)
        {
            SetUserId(infoUtente);
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.UpdateRagioneTrasmissione(ragione);
                if (retValue != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMMODTRASM", ragione.ID, "Modifica ragione di trasmissione " + ragione.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMMODTRASM", ragione.ID, "Modifica ragione di trasmissione " + ragione.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateRagioneTrasmissione", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDeleteRagioneTrasmissione(ref DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.DeleteRagioneTrasmissione(ragione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteRagioneTrasmissione", e);
            }

            return retValue;
        }

        public virtual bool DeleteModelloStampaRicevuta(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRegistro registro)
        {
            SetUserId(infoUtente);
            bool retValue = false;
            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.DeleteModelloStampaRicevuta(infoUtente, registro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DeleteModelloStampaRicevuta ", e);
            }
            return retValue;
        }

        public virtual bool ContainsModelloStampaRicevuta(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRegistro objRegistro)
        {
            SetUserId(infoUtente);
            bool retValue = false;
            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.ContainsModelloStampaRicevuta(infoUtente, objRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ContainsModelloStampaRicevuta ", e);
            }
            return retValue;
        }

        public virtual bool SaveModelloStampaRicevuta(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRegistro registro, byte[] modelContent, bool modelPdf)
        {
            SetUserId(infoUtente);
            bool retValue = false;
            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.SaveModelloStampaRicevuta(infoUtente, registro, modelContent, modelPdf);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: SaveModelloStampaRicevuta ", e);
            }
            return retValue;
        }

        public bool AmmInvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, InfoUtenteAmministratore infoUtente)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();

                return BusinessLogic.LibroFirma.LibroFirmaManager.InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente, spreadsheetService, configurationService, _reportGeneratorService, _rubricaComuneService);
            }
        }

        [XmlInclude(typeof(DocsPaVO.utente.Corrispondente))]

        public virtual DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteByCodRubricaIE(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, InfoUtenteAmministratore u)
        {
            SetUserId(u);
            DocsPaVO.utente.Corrispondente corr = null;

            try
            {
                logger.Debug("WS > AddressbookGetCorrispondenteByCodRubricaIE");
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(codice, tipoIE, u, this._rubricaComuneService);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AddressbookGetCorrispondenteByCodRubricaIE - ", e);
            }
            return corr;
        }

        public bool AmmSbloccoCasella(string email, string idRegistro)
        {
            return BusinessLogic.Amministrazione.AmministraManager.SbloccoCasella(email, idRegistro);
        }

        public virtual DocsPaVO.utente.Ruolo getRuoloById(string idCorrGlobali)
        {
            try
            {
                return BusinessLogic.Utenti.UserManager.getRuoloById(idCorrGlobali);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoloById", e);
                throw new Exception(e.Message);
            }
        }

        public GetUsersInRoleMinimalInfoResponse GetUsersInRoleMinimalInfo(GetUsersInRoleMinimalInfoRequest request)
        {
            GetUsersInRoleMinimalInfoResponse response = new GetUsersInRoleMinimalInfoResponse();
            try
            {
                response = new GetUsersInRoleMinimalInfoResponse() { Users = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(request.RoleId) };

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il recupero delle informazioni minimali sugli utenti del ruolo", e);
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

            return response;
        }

        public string GetXMLUOSmistamento(string idRegistro)
        {
            string result = "";
            try
            {
                result = BusinessLogic.Amministrazione.RegistroManager.GetXMLUOSmistamento(idRegistro);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS GetXMLUOSmistamento: ", exception);
            }
            return result;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        //public virtual ArrayList AmmGetListaRuoliAOO(string idRegistro)
        public virtual IList<DocsPaVO.amministrazione.OrgRuolo> AmmGetListaRuoliAOO(string idRegistro)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListaRuoliAOO(idRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListaRuoliAOO - ", e);

            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        //public virtual ArrayList AmmGetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        public virtual IList<OrgRuolo> AmmGetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoli(idAmm, ricercaPer, testoDaRicercare, idRegistro, IDesclusi);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRuoli - ", e);
            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmAssociazioneRFRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        //public virtual ArrayList AmmGetListRuoliUORic(string idUO, bool ricorsivo)
        public virtual IList<OrgRuolo> AmmGetListRuoliUORic(string idUO, bool ricorsivo)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUoRic(idUO, ricorsivo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRuoliUORic - ", e);
            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmDeleteAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmDeleteAssociazioneRFRuolo(idRf, idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteAssociazioneRFRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDelRightMailRegistro(string idRegistro, string idRuoloInAoo, string indirizzoEmail)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.DeleteRightRuoloMailRegistro(idRegistro, idRuoloInAoo, indirizzoEmail);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDelRightMailRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.amministrazione.OrgRegistro AmmGetRegistro(string idRegistro)
        {
            DocsPaVO.amministrazione.OrgRegistro retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.GetRegistro(idRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetRegistro", e);
            }

            return retValue;
        }

        public bool IsEnabledInteropInterna()
        {
            try
            {
                return BusinessLogic.Interoperabilita.InteroperabilitaUtils.InteropInternaEnabled;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: IsEnabledInteropInterna", e);

                throw e;
            }
        }

        public bool AmmExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            return BusinessLogic.LibroFirma.LibroFirmaManager.ExistsPassiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmInsertRegistro(ref DocsPaVO.amministrazione.OrgRegistro registro, InfoUtenteAmministratore infoUtente)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.InsertRegistro(registro);
                if (retValue != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWAOO", registro.IDRegistro, "Creazione nuova AOO " + registro.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, registro.IDAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWAOO", registro.IDRegistro, "Creazione nuova AOO " + registro.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, registro.IDAmministrazione);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateRegistro(ref DocsPaVO.amministrazione.OrgRegistro registro)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.UpdateRegistro(registro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDeleteRegistro(ref DocsPaVO.amministrazione.OrgRegistro registro)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.DeleteRegistro(registro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmCanDeleteRegistro(DocsPaVO.amministrazione.OrgRegistro registro)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.CanDeleteRegistro(registro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmCanDeleteRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmInsRightMailRegistro(System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailRegistro)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.InsertRightRuoloMailRegistro(rightRuoloMailRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsRightMailRegistro", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDeleteMailRegistro(string idRegistro, string casella)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.DeleteMailRegistro(idRegistro, casella);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteMailRegistro", e);
            }

            return retValue;
        }

        public bool IsEnabledRF(string idAmm)
        {
            try
            {
                return BusinessLogic.Utenti.RegistriManager.enabledRF(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: IsEnabledSupportedFileTypes", e);

                throw e;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTipoRuolo))]
        //      [SoapInclude(typeof(DocsPaVO.amministrazione.OrgTipoRuolo))]
        //      public ArrayList AmmGetTipiRuolo(string codiceAmministrazione)
        public IList<OrgTipoRuolo> AmmGetTipiRuolo(string codiceAmministrazione)
        {
            ArrayList retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiRuoloManager.GetTipiRuolo(codiceAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetTipiRuolo", e);
            }

            return retValue?.OfType<OrgTipoRuolo>().ToList();
        }

        public DocsPaVO.amministrazione.OrgTipoRuolo AmmGetTipoRuolo(string idTipoRuolo)
        {
            DocsPaVO.amministrazione.OrgTipoRuolo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiRuoloManager.GetTipoRuolo(idTipoRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetTipoRuolo", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmInsertTipoRuolo(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo, string idAmm)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.TipiRuoloManager.InsertTipoRuolo(tipoRuolo);
                if (retValue != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWTIPORUOLO", tipoRuolo.IDTipoRuolo, "Creazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWTIPORUOLO", tipoRuolo.IDTipoRuolo, "Creazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertTipoRuolo", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateTipoRuolo(ref DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiRuoloManager.UpdateTipoRuolo(tipoRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateTipoRuolo", e);
            }

            return retValue;
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmDeleteTipoRuolo(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo, string idAmm)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.TipiRuoloManager.DeleteTipoRuolo(tipoRuolo);
                if (retValue != null)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDELTIPORUOLO", tipoRuolo.IDTipoRuolo, "Cancellazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDELTIPORUOLO", tipoRuolo.IDTipoRuolo, "Cancellazione tipo ruolo " + tipoRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteTipoRuolo", e);
            }

            return retValue;
        }

        public string AmmGetIDAmm(string codAmm)
        {
            string retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(codAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetIDAmm - ", e);
            }

            return retValue;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        //public virtual ArrayList AmmGetListTipoRuoloUtenti(string codTipoRuolo, string idAmm)
        public virtual IList<OrgRuolo> AmmGetListTipoRuoloUtenti(string codTipoRuolo, string idAmm)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.TipiRuoloManager.ListTipoRuoloUtenti(codTipoRuolo, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListTipoRuoloUtenti - ", e);
            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.SistemaEsterno))]
        //public virtual ArrayList AmmGetSistemiEsterni(string idAmm)
        public virtual IList<SistemaEsterno> AmmGetSistemiEsterni(string idAmm)
        {
            var result = BusinessLogic.Amministrazione.SistemiEsterni.getSistemiEsterni(idAmm);
            return result.OfType<SistemaEsterno>().ToList();
        }

        public virtual string ctrlInserimentoSistemaEsterno(string idAmm, string codUtente, string codRuolo)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.ctrlInserimentoSistemaEsterno(idAmm, codUtente, codRuolo);
        }

        public virtual DocsPaVO.utente.UnitaOrganizzativa GetHubSistemiEsterni(string codice, string idAmm)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.getHubSistemaEsterno(codice, idAmm);
        }

        public virtual bool setVisibilityHubSysExt(string idHub)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.setVisibilityHubSysExt(idHub);
        }

        public virtual DocsPaVO.utente.TipoRuolo AmmGetTipoRuoloByCode(string idAmm, string codice)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.getTipoRuoloByCode(idAmm, codice);
        }

        public virtual bool AmmInsSysExtAfterAssoc(InfoUtenteAmministratore infut, string idAmm, string codUtente, string codRuolo, string descrizione)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.InsSysExtAfterAssoc(infut, idAmm, codUtente, codRuolo, descrizione);
        }

        public virtual bool AmmModDescTknTimeForExtSys(string desc, string tknTime, string idSysExt)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.ModificaDescTokenSistemaEsterno(desc, tknTime, idSysExt);
        }

        public virtual bool DeleteExternalSystem(DocsPaVO.amministrazione.SistemaEsterno ExtSys, InfoUtenteAmministratore infout)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.deleteExtSys(ExtSys, infout);
        }

        public virtual bool AmmModAllowedPISforExtSys(string methods, string idSysExt)
        {
            return BusinessLogic.Amministrazione.SistemiEsterni.ModificaMetodiPermessiSistemaEsterno(methods, idSysExt);
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.MetodoPIS))]
        //public virtual ArrayList AmmGetPISMethods()
        public virtual IList<MetodoPIS> AmmGetPISMethods()
        {
            var result = BusinessLogic.Amministrazione.SistemiEsterni.getPISMethods();
            return result.OfType<MetodoPIS>().ToList();
        }

        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologia(InfoUtenteAmministratore infoUtente, string idOggetto, string id_Aoo_RF, string annoDa, string annoA, string numeroDa, string numeroA, bool sbloccati, string IdAmministrazione)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.DocumentoStatoFinaleManager.GetDocumentiStatoFinale(infoUtente, idOggetto, id_Aoo_RF, annoDa, annoA, numeroDa, numeroA, sbloccati, IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinalePerContatore(InfoUtenteAmministratore infoUtente, string idTemplates, string idOggetto, string id_Aoo_RF, string anno, string numero, bool sbloccati, string IdAmministrazione)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.DocumentoStatoFinaleManager.GetDocumentiStatoFinale(infoUtente, idTemplates, idOggetto, id_Aoo_RF, anno, numero, sbloccati, IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologiaDocumento(InfoUtenteAmministratore infoUtente, string idTemplate, string anno, bool sbloccati, string IdAmministrazione)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.DocumentoStatoFinaleManager.GetDocumentiStatoFinale(infoUtente, idTemplate, anno, sbloccati, IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(
            InfoUtenteAmministratore infoUtente,
            string idDocumento, string anno, string idRegistro, bool sbloccati, string IdTipologia, string IdAmministrazione, bool Protocollati)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.DocumentoStatoFinaleManager.GetDocumentiStatoFinale(infoUtente, idDocumento, anno, idRegistro, sbloccati, IdTipologia, Protocollati, IdAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public virtual DocsPaVO.ProfilazioneDinamica.Templates getTemplateCampiComuniById(InfoUtenteAmministratore infoUtente, string idTemplate)
        {
            try
            {
                SetUserId(infoUtente);
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateCampiComuniById(infoUtente, idTemplate);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplateCampiComuniById", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.documento.DirittoOggetto))]
        //public virtual ArrayList DocumentoGetVisibilita(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, bool cercaRimossi)
        public virtual IList<DocsPaVO.documento.DirittoOggetto> DocumentoGetVisibilita(InfoUtenteAmministratore infoUtente, string idProfile, bool cercaRimossi)
        {
            SetUserId(infoUtente);
            ArrayList objListaDiritti = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                objListaDiritti = BusinessLogic.Utenti.DirittiManager.getListaDiritti(infoUtente, idProfile, cercaRimossi);
#if TRACE_WS
				pt.WriteLogTracer("DocumentoGetVisibilita");
#endif

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DocumentoGetVisibilita", e);
                objListaDiritti = null;
            }

            return objListaDiritti.OfType<DocsPaVO.documento.DirittoOggetto>().ToList();
        }


        public bool ModificaDocumentoStatoFinale(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale[] infoModificheList)
        {
            SetUserId(infoUtente);
            bool retValue = false;

            string idFirstDocument = string.Empty;

            if (infoModificheList.Length > 0)
                idFirstDocument = infoModificheList[0].IdDocumento;

            try
            {
                retValue = BusinessLogic.Amministrazione.DocumentoStatoFinaleManager.ModificaDocumentoStatoFinale(infoUtente, infoModificheList);
            }
            catch (Exception ex)
            {
                retValue = false;
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
            finally
            {
                BusinessLogic.UserLog.UserLog.WriteLog(
                    infoUtente,
                    "MODIFICADOCSTATOFINALE",
                    idFirstDocument,
                    string.Format("N° Doc.: {0}{1}Modificati i diritti di lettura / scrittura sul documento in stato finale", idFirstDocument, Environment.NewLine),
                    (retValue ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO));
            }

            return retValue;
        }

        public virtual DocsPaVO.documento.SchedaDocumento DocumentoGetDettaglioDocumentoNoSecurity(InfoUtenteAmministratore infoutente, string idProfile, string docNumber)
        {
            SetUserId(infoutente);
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            string VarDescOggetto = string.Empty;

            try
            {
                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoutente, idProfile);

                if (schedaDocumento != null)
                {
                    if (schedaDocumento.protocollo != null)
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.:", schedaDocumento.docNumber, "Segnatura: ", schedaDocumento.protocollo.segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.:", schedaDocumento.docNumber + " Creato il " + schedaDocumento.dataCreazione);

                    BusinessLogic.UserLog.UserLog.WriteLog(infoutente, "DOCUMENTOGETDETTAGLIODOCUMENTO", schedaDocumento.systemId, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK);
                }
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoutente, "DOCUMENTOGETDETTAGLIODOCUMENTO", docNumber, VarDescOggetto, DocsPaVO.Logger.CodAzione.Esito.KO);

                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DocumentoGetDettaglioDocumento", e);
                schedaDocumento = null;
            }

            return schedaDocumento;
        }

        public DataTable GetStruttureSottofascicoli(string idamministrazione)
        {
            DataTable result = null;

            try
            {
                result = BusinessLogic.Amministrazione.StruttureSottofascicoliManager.getStruttureSottofascicoli(idamministrazione);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetStruttureSottofascicoli", ex);
            }

            return result;
        }

        public EsitoOperazione DeleteStrutturaSottofascicoli(int idtemplate, int idtipofascicolo, int idtitolario, string idAmm)
        {
            EsitoOperazione result = new EsitoOperazione() { Codice = 0, Descrizione = "Esito positivo" };

            try
            {
                DataTable relation = BusinessLogic.Amministrazione.StruttureSottofascicoliManager.getTemplateStrutturaRelation(idtemplate, idtipofascicolo, idtitolario);
                if (relation.Rows.Count > 0)
                    throw new Exception("Impossibile cancellare l'elemento perchè risulta collegato");

                BusinessLogic.Amministrazione.StruttureSottofascicoliManager.DeleteStrutturaSottofascicoli(idtemplate, idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SaveStrutturaSottofascicoli", ex);
                result.Codice = -1;
                result.Descrizione = ex.Message;
            }

            return result;
        }

        public DataTable GetStruttureSottofascicoliById(string idamm, int idfascicolo, int idtitolario, int idtemplate)
        {
            DataTable result = null;

            try
            {
                result = BusinessLogic.Amministrazione.StruttureSottofascicoliManager.getStruttureSottofascicoli(idamm, idfascicolo, idtitolario, idtemplate);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetStruttureSottofascicoli", ex);
            }

            return result;
        }

        public EsitoOperazione SaveStrutturaSottofascicoli(int id, DataTable data, string name, string idAmm)
        {
            EsitoOperazione result = new EsitoOperazione() { Codice = 0, Descrizione = "Esito positivo" };

            try
            {
                BusinessLogic.Amministrazione.StruttureSottofascicoliManager.saveStrutturaSottofascicoli(id, data, name, idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SaveStrutturaSottofascicoli", ex);
                result.Codice = -1;
                result.Descrizione = ex.Message;
            }

            return result;
        }

        public bool IsNodoStrutturaInFascicoliEmpty(string strutturaid, string nodename, string idamm, out int numfascicoli)
        {
            bool result = false;
            numfascicoli = 0;

            try
            {
                result = BusinessLogic.Amministrazione.StruttureSottofascicoliManager.IsNodoStrutturaInFascicoliEmpty(strutturaid, nodename, out numfascicoli, idamm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SaveStrutturaSottofascicoli", ex);
            }

            return result;
        }

        public virtual DocsPaVO.amministrazione.OrgTitolario getTitolarioById(string idTitolario)
        {
            DocsPaVO.amministrazione.OrgTitolario titolario;
            try
            {
                titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(idTitolario);
                return titolario;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTitolarioById", e);
                return null;
            }
        }

        public virtual void setEtichetteTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            try
            {
                BusinessLogic.Amministrazione.TitolarioManager.setEtichetteTitolario(titolario);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: setEtichetteTitolario", e);
            }
        }

        public bool importIndice(byte[] dati, DocsPaVO.amministrazione.OrgTitolario titolario, InfoUtenteAmministratore infoUtente)
        {
            SetUserId(infoUtente);
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                bool result = BusinessLogic.Amministrazione.TitolarioManager.importaIndice(dati, "importazioneIndice.xls", serverPath, infoUtente, titolario);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: importIndice", e);
                return false;
            }
        }

        public bool importTitolario(byte[] dati, DocsPaVO.amministrazione.OrgTitolario titolario, InfoUtenteAmministratore infoUtente)
        {
            try
            {
                SetUserId(infoUtente);
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                bool result = BusinessLogic.Amministrazione.TitolarioManager.importaTitolario(dati, "importazioneTitolario.xls", serverPath, infoUtente, titolario, DocsPaVO.Settings.AppSettings.Instance.DS_PROVIDER,
                    DocsPaVO.Settings.AppSettings.Instance.DS_EXTENDED_PROPERTIES, spreadsheetService);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: importTitolario", e);
                return false;
            }
        }

        public DocsPaVO.fascicolazione.VoceIndiceSistematico existVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            try
            {
                return BusinessLogic.Fascicoli.IndiceSistematico.existVoceIndice(voceIndice);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: existVoceIndice", e);
                return null;
            }
        }

        public void addNuovaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            try
            {
                BusinessLogic.Fascicoli.IndiceSistematico.addNuovaVoceIndice(voceIndice);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: addNuovaVoceIndice", e);
            }
        }

        public bool isAssociataVoce(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice, bool suSingoloNodo)
        {
            try
            {
                return BusinessLogic.Fascicoli.IndiceSistematico.isAssociataVoce(voceIndice, suSingoloNodo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isAssociataVoce", e);
                return false;
            }
        }

        public void removeVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            try
            {
                BusinessLogic.Fascicoli.IndiceSistematico.removeVoceIndice(voceIndice);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: removeVoceIndice", e);
            }
        }

        public void associaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            try
            {
                BusinessLogic.Fascicoli.IndiceSistematico.associaVoceIndice(voceIndice);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: associaVoceIndice", e);
            }
        }

        public void dissociaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice)
        {
            try
            {
                BusinessLogic.Fascicoli.IndiceSistematico.dissociaVoceIndice(voceIndice);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: dissociaVoceIndice", e);
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.fascicolazione.VoceIndiceSistematico))]
        //public ArrayList getIndiceByIdAmm(string idAmm)
        public IList<VoceIndiceSistematico> getIndiceByIdAmm(string idAmm)
        {
            try
            {
                var result = BusinessLogic.Fascicoli.IndiceSistematico.getIndiceByIdAmm(idAmm);
                return result.OfType<VoceIndiceSistematico>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getIndiceByIdAmm", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.fascicolazione.VoceIndiceSistematico))]
        //		public ArrayList getIndiceByIdProject(string idProject)
        public IList<VoceIndiceSistematico> getIndiceByIdProject(string idProject)
        {
            try
            {
                var result = BusinessLogic.Fascicoli.IndiceSistematico.getIndiceByIdProject(idProject);
                return result.OfType<VoceIndiceSistematico>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getIndiceByIdProject", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //public virtual ArrayList getLogImportIndice()
        public virtual IList<string> getLogImportIndice()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                string serverPath = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                logFile = BusinessLogic.Amministrazione.TitolarioManager.getLogImportIndice(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getLogImportIndice", e);
                return logFile.OfType<string>().ToList();
            }
        }

        //[return: XmlArray()]
        //public virtual ArrayList getLogImportTitolario()
        public virtual IList<string> getLogImportTitolario()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];

                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                logFile = BusinessLogic.Amministrazione.TitolarioManager.getLogImportTitolario(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getLogImportTitolario", e);
                return logFile.OfType<string>().ToList();
            }
        }

        public bool isEnableIndiceSistematico()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["INDICE_SISTEMATICO"] != null && System.Configuration.ConfigurationManager.AppSettings["INDICE_SISTEMATICO"] == "1")
                return true;
            else
                return false;
        }

        public bool existTitolarioInDef(string codiceAmm)
        {
            try
            {
                bool result = BusinessLogic.Amministrazione.TitolarioManager.existTitolarioInDef(codiceAmm);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: existTitolarioInDef", e);
                return false;
            }
        }

        public virtual string isEnableContatoreTitolario()
        {
            return DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableContatoreTitolario();
        }

        public virtual int getContatoreProtTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            try
            {
                return BusinessLogic.Amministrazione.TitolarioManager.getContatoreProtTitolario(nodoTitolario);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getContatoreProtTitolario", e);
                return 0;
            }
        }

        public bool SaveTitolario(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            try
            {
                SetUserId(infoUtente);
                bool result = BusinessLogic.Amministrazione.TitolarioManager.SaveTitolario(infoUtente, titolario);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: saveTitolario", e);
                return false;
            }
        }

        public bool attivaTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            SetUserId(infoUtente);
            try
            {
                bool result = BusinessLogic.Amministrazione.TitolarioManager.attivaTitolario(infoUtente, titolario);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: attivaTitolario", e);
                return false;
            }
        }

        public bool deleteTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            SetUserId(infoUtente);
            try
            {
                bool result = BusinessLogic.Amministrazione.TitolarioManager.deleteTitolario(infoUtente, titolario);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: deleteTitolario", e);
                return false;
            }
        }

        public bool copiaTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            SetUserId(infoUtente);
            try
            {
                return BusinessLogic.Amministrazione.TitolarioManager.CopiaTitolario(infoUtente, titolario, idRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: copiaTitolario", e);
                return false;
            }
        }

        public virtual DocsPaVO.documento.FileDocumento ExportTitolarioInExcel(DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                string serverPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                BusinessLogic.ExportDati.ExportDatiManager exportDati = new BusinessLogic.ExportDati.ExportDatiManager();
                DocsPaVO.documento.FileDocumento fileDocumento = exportDati.ExportTitolarioInExcel(serverPath, titolario, idRegistro, this.spreadsheetService);
                return fileDocumento;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ExportTitolario", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgTitolario))]
        //public virtual ArrayList getTitolari(string idAmministrazione)
        public virtual IList<OrgTitolario> getTitolari(string idAmministrazione)
        {
            ArrayList titolari = new ArrayList();
            try
            {
                titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolari(idAmministrazione);
                return titolari.OfType<OrgTitolario>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTitolari", e);
                return titolari.OfType<OrgTitolario>().ToList();
            }
        }

        public virtual List<DispositivoStampaEtichetta> AmministrazioneGetDispositivoStampaEtichetta()
        {
            List<DispositivoStampaEtichetta> objListaDispositivi = null;
            try
            {
                objListaDispositivi = BusinessLogic.Amministrazione.AmministraManager.GetDispositiviStampaEtichetta();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmministrazioneGetDispositivoStampaEtichetta", e);
                objListaDispositivi = null;
            }
            return objListaDispositivi;
        }

        public bool EnableHTML5SocketOption()
        {
            bool retVal = false;
            try
            {
                retVal = DocsPaDB.Query_DocsPAWS.SmartClient.EnableHTML5SocketOption();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: EnableJavaAppletOption", e);
                retVal = false;
            }

            return retVal;
        }

        public bool AdminIsSupportedPasswordConfig()
        {
            try
            {
                return BusinessLogic.Amministrazione.AdminPasswordConfig.IsSupportedPasswordConfig();
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx  - metodo: AdminIsSupportedPasswordConfig", e);
            }
        }

        public DocsPaVO.Ldap.LdapConfig GetLdapConfig(InfoUtenteAmministratore infoUtente, string idAmministrazione)
        {
            try
            {
                SetUserId(infoUtente);

                return BusinessLogic.Ldap.LdapConfigurationsServices.GetLdapConfig(infoUtente, idAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUtente))]
        //public virtual ArrayList AmmGetListUtentiInAmmRic(string idAmm, string ricercaPer, string testoDaRicercare)
        public virtual IList<OrgUtente> AmmGetListUtentiInAmmRic(string idAmm, string ricercaPer, string testoDaRicercare)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListUtenti(idAmm, ricercaPer, testoDaRicercare);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListUtenti - ", e);
            }

            return retValue.OfType<OrgUtente>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.utente.Amministrazione))]
        //public virtual ArrayList amministrazioneGetAmministrazioniByUser(string userId, bool controllo, out string returnMsg)
        public virtual IList<Amministrazione> amministrazioneGetAmministrazioniByUser(string userId, bool controllo, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                var result = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioniByUser(userId, controllo);
                return result.OfType<Amministrazione>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: amministrazioneGetAmministrazioniByUser", e);
                returnMsg = e.ToString();
            }

            return null;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsUtente(InfoUtenteAmministratore infoUtente, string idAmm, DocsPaVO.amministrazione.OrgUtente utente)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsNuovoUtente(infoUtente, utente);
                //if(esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUSER", utente.IDPeople, "Creazione utente " + utente.Codice + " - " + utente.Nome + " " + utente.Cognome, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUSER", utente.IDPeople, "Creazione utente " + utente.Codice + " - " + utente.Nome + " " + utente.Cognome, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public Utente GetUtenteAutomatico(string idAmministrazione)
        {
            return BusinessLogic.Utenti.UserManager.GetUtenteAutomatico(idAmministrazione);
        }

        public virtual string GetPasswordUtenteMultiAmm(string userId)
        {
            string password = string.Empty;

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.GetPasswordUtenteMultiAmm(userId);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetPasswordUtenteMultiAmm", e);
            }

            return password;
        }

        public virtual bool SetPasswordUtenteMultiAmm(string userId, string password)
        {
            bool resultValue = false;

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.SetPasswordUtenteMultiAmm(userId, password);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: SetPasswordUtenteMultiAmm", e);
            }

            return resultValue;
        }

        public virtual bool SetUtenteVerticale(string userId, string idAmm, bool isVerticale)
        {
            bool resultValue = false;

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.SetUtenteVerticale(userId, idAmm, isVerticale);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: SetUtenteVerticale", e);
            }

            return resultValue;
        }

        public virtual bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
        {
            bool resultValue = false;

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.ModificaPasswordUtenteMultiAmm(userId, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ModificaPasswordUtenteMultiAmm", e);
            }

            return resultValue;
        }

        public virtual bool IsUtenteVerticale(string idPeople)
        {
            bool resultValue = false;

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.IsUtenteVerticale(idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: IsUtenteVerticale", e);
            }

            return resultValue;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmModUtente(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUtente utente)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmModUtente(infoUtente, utente);

                // Se l'esito è positivo
                if (esito.Codice == 0)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                        "MODUSER", utente.IDCorrGlobale,
                        string.Format("Profilo dell'utente {0} modificato dall'amministratore {1}",
                            utente.UserId, infoUtente.userId),
                            DocsPaVO.Logger.CodAzione.Esito.OK);
                if (esito.Codice == 4)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                        "MODUSER", utente.IDCorrGlobale,
                        string.Format("Password dell'utente {0} modificata dall'amministratore {1}",
                            utente.UserId, infoUtente.userId),
                            DocsPaVO.Logger.CodAzione.Esito.OK);
                    esito.Codice = 0;
                }
                if (esito.Codice == 5)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                        "MODUSER", utente.IDCorrGlobale,
                        string.Format("Impostazione smart client dell'utente {0} modificata dall'amministratore {1}",
                            utente.UserId, infoUtente.userId),
                            DocsPaVO.Logger.CodAzione.Esito.OK);
                    esito.Codice = 0;
                }
                if (esito.Codice == 6)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                        "MODUSER", utente.IDCorrGlobale,
                        string.Format("Impostazione smart client e password dell'utente {0} modificate dall'amministratore {1}",
                            utente.UserId, infoUtente.userId),
                            DocsPaVO.Logger.CodAzione.Esito.OK);
                    esito.Codice = 0;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmModUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmVerificaEliminazioneUtente(DocsPaVO.amministrazione.OrgUtente utente)
        {
            SetUserId(utente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmVerificaEliminazioneUtente(utente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmVerificaEliminazioneUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        private void SetUserId(OrgUtente infoUtente)
        {
            if (infoUtente != null) SetUserId(infoUtente.UserId);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUtente(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUtente utente)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUtente(infoUtente, utente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public string GetFormatoDominio(string idAmm)
        {
            string retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetFormatoDominio(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetFormatoDominio - ", e);
            }

            return retValue;
        }

        public virtual bool AmmCheckRegAssUtente(string idCorrGlob)
        {
            bool esito = false;
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmCheckRegAssUtente(idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmCheckRegAssUtente - ", e);
            }

            return esito;
        }

        public virtual bool AmmCheckMenuAssUtente(string idCorrGlob)
        {
            bool esito = false;
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmCheckMenuAssUtente(idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmCheckMenuAssUtente - ", e);
            }

            return esito;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        //public virtual ArrayList AmmGetListRuoliUtente(string idPeople)
        public virtual IList<OrgRuolo> AmmGetListRuoliUtente(string idPeople)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListRuoliUtente(idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRuoliUtente - ", e);
            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        public virtual bool AmmCheckRespAOO(string idPeople, string idGruppo)
        {
            bool esito = false;
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmCheckRespAOO(idPeople, idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmCheckRespAOO - ", e);
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaRegistriUtenteAdmin(string idCorrGlob)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmEliminaRegistriUtenteAdmin(idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaRegistriUtenteAdmin - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaMenuUtenteAdmin(string idCorrGlob)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmEliminaMenuUtenteAdmin(idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaMenuUtenteAdmin - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual bool importaUtenti(InfoUtenteAmministratore infoUtente, byte[] dati, string nomeFile, string codiceAmm, bool update, ref int utInseriti, ref int utAggiornati)
        {
            SetUserId(infoUtente);
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;
                //serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                bool result = BusinessLogic.Utenti.UserManager.importaUtenti(infoUtente, dati, nomeFile, codiceAmm, update, ref utInseriti, ref utAggiornati, this.spreadsheetService, serverPath);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: importaUtenti");
                return false;
            }
        }

        public DocsPaVO.Ldap.LdapSyncronizationResponse GetLdapSyncResponse(InfoUtenteAmministratore infoUtente, int idHistoryItem)
        {
            try
            { 
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = serverPath.PathAsUnixPath();
                SetUserId(infoUtente);
                return BusinessLogic.Ldap.LdapUserSyncronizationServices.GetLdapSyncResponse(infoUtente, idHistoryItem, serverPath);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public void SaveLdapConfig(InfoUtenteAmministratore infoUtente, string idAmministrazione, DocsPaVO.Ldap.LdapConfig ldapInfo)
        {
            SetUserId(infoUtente);
            try
            {
                BusinessLogic.Ldap.LdapConfigurationsServices.SaveLdapConfig(infoUtente, idAmministrazione, ldapInfo);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public DocsPaVO.Ldap.LdapSyncronizationResponse SyncronizeLdapUsers(DocsPaVO.Ldap.LdapSyncronizationRequest request)
        {
            try
            {
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = serverPath.PathAsUnixPath();

                return BusinessLogic.Ldap.LdapUserSyncronizationServices.SyncronizeLdapUsers(request, serverPath);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public DocsPaVO.Ldap.LdapSyncronizationHistoryItem[] GetLdapSyncHistory(InfoUtenteAmministratore infoUtente, string idAmministrazione)
        {
            try
            {
                SetUserId(infoUtente);
                return BusinessLogic.Ldap.LdapUserSyncronizationServices.GetLdapSyncHistory(infoUtente, idAmministrazione);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        public void DeleteLdapSyncHistoryItem(InfoUtenteAmministratore infoUtente, int idHistoryItem)
        {
            SetUserId(infoUtente);
            try
            {
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = serverPath.PathAsUnixPath();

                BusinessLogic.Ldap.LdapUserSyncronizationServices.DeleteLdapSyncHistoryItem(infoUtente, idHistoryItem, serverPath);
            }
            catch (Exception ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
        }

        //[return: XmlArray()]
        //public virtual ArrayList getLogImportUtenti()
        public virtual IList<string> getLogImportUtenti()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                // PALUMBO: modificato per ovviare al blocco dell'accesso su macchina 20 in scrittura
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["LOG_PATH"];
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                logFile = BusinessLogic.Utenti.UserManager.getLogImportUtenti(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: getLogImportUtenti");
                return logFile.OfType<string>().ToList();
            }
        }

        public virtual string ElencoUtentiConnessi(string codiceAmm)
        {
            string result = null;

            try
            {
                result = BusinessLogic.Utenti.Login.ElencoUtentiConnessi(codiceAmm);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore durante la lettura degli utenti connessi.");
                result = null;
            }

            return result;
        }

        public virtual int NumUtentiConnessi(string codiceAmm)
        {
            int result = 0;

            try
            {
                result = BusinessLogic.Utenti.Login.NumUtentiConnessi(codiceAmm);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore durante la lettura del numero di utenti connessi.");
                result = 0;
            }
            return result;
        }

        public virtual bool DisconnettiUtente(string userId, string codiceAmm)
        {
            bool result = true; // Presume successo

            try
            {
                BusinessLogic.Utenti.Login.disconnettiUtente(userId, codiceAmm);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore durante la disconnessione dell'utente.");
                result = false;
            }

            return result;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsMenuUtenteAdmin(DocsPaVO.amministrazione.Menu[] listaMenu, string idCorrGlob, string idAmm)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmInsMenuUtenteAdmin(listaMenu, idCorrGlob, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: AmmInsMenuUtenteAdmin - ");
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRegistro))]
        //public virtual ArrayList AmmGetListRegistriUtente(string idAmm, string idCorrGlob)
        public virtual IList<OrgRuolo> AmmGetListRegistriUtente(string idAmm, string idCorrGlob)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.UtenteManager.AmmGetListRegistriUtente(idAmm, idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListRegistriUtente - ", e);
            }

            return retValue.OfType<OrgRuolo>().ToList();
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsRegistriUtenteAdmin(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idCorrGlob)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmInsRegistriUtenteAdmin(listaRegistri, idCorrGlob);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsRegistriUtenteAdmin - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmImpostaRuoloPreferito(InfoUtenteAmministratore infoUtente, string idPeople, string idGruppo)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmImpostaRuoloPreferito(infoUtente, idPeople, idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmImpostaRuoloPreferito - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public string GetLabelTipoDocumento(string typeId)
        {
            return BusinessLogic.Documenti.DocManager.GetLabelTipoDocumento(typeId);
        }

        public InteroperabilitySettings LoadSimplifiedInteroperabilitySettings(String registryId)
        {
            try
            {
                return BusinessLogic.Interoperabilita.Semplificata.InteroperabilitaSemplificataManager.LoadSettings(registryId);
            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(new Exception("Errore durante il caricamento delle impostazioni."));
            }

        }

        public bool SaveSimplifiedInteroperabilitySettings(InteroperabilitySettings interoperabilitySettings)
        {
            try
            {
                return BusinessLogic.Interoperabilita.Semplificata.InteroperabilitaSemplificataManager.SaveSettings(interoperabilitySettings);
            }
            catch (BusinessLogic.Interoperabilita.Semplificata.Exceptions.SimplifiedInteroperabilitySaveSettingsException ex)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(new Exception("Errore durante il salvataggio delle impostazioni."));
            }

        }

        public virtual bool AmmVerificaGestioneChiavi(string idPeople)
        {
            bool esito = false;
            try
            {
                esito = BusinessLogic.Amministrazione.UtenteManager.AmmVerificaGestioneChiavi(idPeople);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmVerificaGestioneChiavi - ", e);
            }

            return esito;
        }

        public bool SaveElementoRubricaRF(RaggruppamentoFunzionale rf, String idRegistro)
        {
            return BusinessLogic.Utenti.RegistriManager.SaveRaggruppamentoFunzionaleRC(rf, idRegistro);
        }

        public String GetRoleDescriptionByIdGroup(String idGroup)
        {
            try
            {
                return BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdGroup(idGroup);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "errore in GetRoleDescriptionByIdGroup");
            }

            return String.Empty;
        }

        public DocsPaVO.Validations.ValidationResultInfo AdminChangePassword(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.ChangeAdminPassword(user, oldPassword);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx  - metodo: AdminChangePassword", e);
            }
        }

        public bool setStatoAccettazioneDisservizio(string stato)
        {
            //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
            return BusinessLogic.Amministrazione.AmministraManager.setStatoAccettazioneDisservizio(stato);
        }

        public string getApplicationName()
        {
            string retValue = string.Empty;
            retValue = BusinessLogic.Amministrazione.AmministraManager.getApplicationName();
            return retValue;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.utente.Amministrazione))]
        //[SoapInclude(typeof(DocsPaVO.utente.Amministrazione))]
        public virtual IList<Amministrazione> amministrazioneGetAmministrazioni(out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                var result = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni();
                return result.OfType<Amministrazione>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: amministrazioneGetAmministrazioni", e);
                returnMsg = e.ToString();
            }

            return null;
        }

        public virtual DocsPaVO.amministrazione.InfoAmministrazione[] AmmGetListAmministrazioni()
        {
            DocsPaVO.amministrazione.InfoAmministrazione[] retValue = null;
            try
            {
                retValue = BusinessLogic.Amministrazione.AmministraManager.AmmGetListAmministrazioni();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListAmministrazioni - ", e);
            }

            return retValue;
        }

        [XmlInclude(typeof(DocsPaVO.documento.EtichettaInfo))]
        //        public virtual DocsPaVO.documento.EtichettaInfo[] getEtichetteDocumenti(DocsPaVO.utente.InfoUtente infoUtente, string idAmm)
        public virtual DocsPaVO.documento.EtichettaInfo[] getEtichetteDocumenti(InfoUtenteAmministratore infoUtente, string idAmm)
        {
            SetUserId(infoUtente);
            DocsPaVO.documento.EtichettaInfo[] etichettaInfo = null;
            try
            {
                etichettaInfo = BusinessLogic.Documenti.EtichetteManager.GetInstance(infoUtente, idAmm);
            }
            catch (Exception e)
            {
                throw e;
            }
            return etichettaInfo;
        }

        //      [return: XmlArray()]
        //      [return: XmlArrayItem(typeof(DocsPaVO.amministrazione.ChiaveConfigurazione))]
        //      [XmlInclude(typeof(DocsPaVO.amministrazione.ChiaveConfigurazione))] //Aggiunto
        //[SoapInclude(typeof(DocsPaVO.amministrazione.ChiaveConfigurazione[]))]
        //      [SoapInclude(typeof(DocsPaVO.amministrazione.ChiaveConfigurazione))]
        public virtual IList<ChiaveConfigurazione> getListaChiaviConfig(string idAmm)
        //public virtual ArrayList getListaChiaviConfig(string idAmm)
        {
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(idAmm);
            //ArrayList lista = new ArrayList(chiaviAmm.ListaChiavi);
            List<ChiaveConfigurazione> lista = chiaviAmm.ListaChiavi.OfType<ChiaveConfigurazione>().ToList();

            return lista;
        }

        //[return: XmlArray()]
        public DocsPaVO.amministrazione.OrgRagioneTrasmissione[] AmmGetInfoRagioniTrasmissione(string idAmministrazione)
        {
            DocsPaVO.amministrazione.OrgRagioneTrasmissione[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RagioniTrasmissioneManager.GetInfoRagioniTrasmissione(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetInfoRagioniTrasmissione", e);
            }

            return retValue;
        }

        //public List<DocsPaVO.documento.FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumento(string idAmm, DocsPaVO.utente.InfoUtente infoutente)
        public List<DocsPaVO.documento.FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumento(string idAmm, InfoUtenteAmministratore infoutente)
        {
            return BusinessLogic.Amministrazione.AmministraManager.GetFascicolazioneTipiDocumento(idAmm, infoutente);
        }

        public bool IsEnabledSupportedFileTypes()
        {
            try
            {
                return BusinessLogic.FormatiDocumento.Configurations.SupportedFileTypesEnabled;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: IsEnabledSupportedFileTypes", e);

                throw e;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUtente))]
        //[SoapInclude(typeof(DocsPaVO.amministrazione.OrgUtente))]
        public virtual IList<OrgUtente> AmmGetListUtentiInAmm(string idAmm)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListUtenti(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListUtenti - ", e);
            }

            return retValue.OfType<OrgUtente>().ToList();
        }

        public virtual int NumUtentiAttivi(string codiceAmm)
        {
            int result = 0;

            try
            {
                result = BusinessLogic.Utenti.Login.NumUtentiAttivi(codiceAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la lettura del numero di utenti connessi.", e);
                result = 0;
            }
            return result;
        }

        public virtual bool getValueInteropNoMail()
        {
            return BusinessLogic.Interoperabilita.InteroperabilitaUtils.InteropIntNoMail;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //public virtual ArrayList getTipoFasc(string idAmministrazione)
        public virtual IList<DocsPaVO.ProfilazioneDinamica.Templates> getTipoFasc(string idAmministrazione)
        {
            ArrayList result = null;

            try
            {
                result = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTipoFasc(idAmministrazione);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.Templates>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTipoFasc", e);
                result = null;
            }
            return result?.OfType<DocsPaVO.ProfilazioneDinamica.Templates>().ToList();
        }

        public DataSet GetAmmRightMailRegistro(string idRegistro, string idRuoloInUO)
        {
            try
            {
                return BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(idRegistro, idRuoloInUO);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetAmmRightMailRegistro", e);
            }

            return new DataSet();
        }

        public string RegistriInAmm(string codAmm)
        {
            string result = "";
            try
            {
                BusinessLogic.AmministrazioneXml.TitolarioXml obj = new BusinessLogic.AmministrazioneXml.TitolarioXml();
                result = obj.registriInAmm(codAmm);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS RegistriInAmm: ", exception);
            }
            return result;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgNodoTitolario))]
        //public virtual ArrayList AmmGetNodiTitolario(string codiceAmministrazione, string idNodoTitolarioParent, string idRegistro)
        public virtual IList<OrgNodoTitolario> AmmGetNodiTitolario(string codiceAmministrazione, string idNodoTitolarioParent, string idRegistro)
        {
            ArrayList retValue = null;

            try
            {
                if (idRegistro == null)
                    idRegistro = string.Empty;

                retValue = BusinessLogic.Amministrazione.TitolarioManager.GetNodiTitolario(codiceAmministrazione, idNodoTitolarioParent, idRegistro);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetNodiTitolario", e);
            }

            return retValue.OfType<OrgNodoTitolario>().ToList();
        }

        //public ArrayList getResponsabiliCons(string idAmm)
        public IList<object> getResponsabiliCons(string idAmm)
        {
            try
            {
                BusinessLogic.Conservazione.ConservazioneManager cons = new BusinessLogic.Conservazione.ConservazioneManager();
                var result = cons.getResponsabiliCons(idAmm);
                return result.OfType<object>().ToList();
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: getResponsabiliCons", ex);
                return null;
            }
        }

        public DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER[] GetListaPolicyFascicoliPARER(string idAmm)
        {
            try
            {
                return (DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER[])BusinessLogic.Conservazione.PARER.PolicyPARERManager.getListaPolicy(idAmm, "F").ToArray(typeof(DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER));
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetListaPolicyFascicoliPARER - ", ex);
                return null;
            }
        }

        public virtual string findFontColor(string idAmm)
        {
            string result = "";
            try
            {
                result = BusinessLogic.Amministrazione.AmministraManager.findFontColor(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: findFontColor", e);
                result = "";
            }
            return result;
        }

        public DocsPaVO.CheckInOut.CheckOutStatus[] GetCheckOutAdminDocuments(InfoUtenteAmministratore adminInfoUtente, string idAdministration)
        {
            SetUserId(adminInfoUtente);
            // TODO: Gestione log
            DocsPaVO.CheckInOut.CheckOutStatus[] retValue = null;

            try
            {
                retValue = BusinessLogic.CheckInOut.CheckInOutAdminServices.GetCheckOutDocuments(adminInfoUtente, idAdministration);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retValue;
        }

        public bool CanForceUndoCheckOutAdminDocument(InfoUtenteAmministratore adminInfoUtente)
        {
            SetUserId(adminInfoUtente);
            bool retValue = false;

            try
            {
                retValue = BusinessLogic.CheckInOut.CheckInOutAdminServices.CanForceUndoCheckOut(adminInfoUtente);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retValue;
        }

        public virtual IList<DocsPaVO.ProfilazioneDinamica.Templates> getTemplates(string idAmministrazione)
        //[XmlInclude(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //[SoapInclude(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //public virtual ArrayList getTemplates(string idAmministrazione)
        {
            try
            {
                var result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplates(idAmministrazione);
                return result.OfType<DocsPaVO.ProfilazioneDinamica.Templates>().ToList();
                //return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getTemplates", e);
                return null;
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.DiagrammaStato))]
        public virtual IList<DocsPaVO.DiagrammaStato.DiagrammaStato> getDiagrammi(string idAmm)
        {
            try
            {
                var result = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammi(idAmm);
                return result.OfType<DocsPaVO.DiagrammaStato.DiagrammaStato>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getDiagrammi", e);
                return null;
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.trasmissione.RagioneTrasmissione))]
        //public virtual ArrayList TrasmissioneGetRagioniSysExt(DocsPaVO.trasmissione.Diritti diritti)
        public virtual IList<DocsPaVO.trasmissione.RagioneTrasmissione> TrasmissioneGetRagioniSysExt(DocsPaVO.trasmissione.Diritti diritti)
        {
            ArrayList result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Trasmissioni.RagioniManager.getListaRagioni(diritti, false, true);
#if TRACE_WS
				pt.WriteLogTracer("TrasmissioneGetRagioni");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneGetRagioniSysExt", e);
                result = null;
            }

            return result?.OfType<DocsPaVO.trasmissione.RagioneTrasmissione>().ToList();
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.trasmissione.RagioneTrasmissione))]
        //public virtual ArrayList TrasmissioneGetRagioni(DocsPaVO.trasmissione.Diritti diritti, bool flgDaRicercaTrasm)
        public virtual IList<DocsPaVO.trasmissione.RagioneTrasmissione> TrasmissioneGetRagioni(DocsPaVO.trasmissione.Diritti diritti, bool flgDaRicercaTrasm)
        {
            ArrayList result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Trasmissioni.RagioniManager.getListaRagioni(diritti, flgDaRicercaTrasm);
#if TRACE_WS
				pt.WriteLogTracer("TrasmissioneGetRagioni");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneGetRagioni", e);
                result = null;
            }

            return result?.OfType<DocsPaVO.trasmissione.RagioneTrasmissione>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.Deleghe.InfoDelega))]
        //public virtual ArrayList DelegaGetLista(InfoUtenteAmministratore infoUtente, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe)
        public virtual IList<DocsPaVO.Deleghe.InfoDelega> DelegaGetLista(InfoUtenteAmministratore infoUtente, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe)
        {
            SetUserId(infoUtente);
            ArrayList deleghe = null;
            numDeleghe = 0;
            try
            {
                deleghe = BusinessLogic.Deleghe.DelegheManager.getListaDeleghe(infoUtente, tipoDelega, statoDelega, idAmm, out numDeleghe);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetDelegheAssegnate", e);
            }
            return deleghe?.OfType<DocsPaVO.Deleghe.InfoDelega>().ToList();
        }

        //[XmlInclude(typeof(DocsPaVO.amministrazione.MailProvider))]
        public DocsPaVO.amministrazione.MailProvider[] AmmGetProviderList()
        {
            DocsPaVO.amministrazione.MailProvider[] retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.GetProviderList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetMailRegistro", e);
            }

            return retValue;
        }

        public bool IsEnabledMultiMail(string idAmm)
        {
            try
            {
                return BusinessLogic.Interoperabilita.InteroperabilitaUtils.MultiMailEnabled(idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: IsEnabledMultiMail", e);
                throw e;
            }
        }

        public bool LogoutAmministratore(DocsPaVO.amministrazione.InfoUtenteAmministratore adminUser)
        {
            SetUserId(adminUser);
            bool result = false;
            try
            {
                BusinessLogic.Amministrazione.AmministraManager manager = new BusinessLogic.Amministrazione.AmministraManager();
                result = manager.LogoutAmministratore(adminUser);
                if (result)
                    BusinessLogic.UserLog.UserLog.WriteLog(adminUser, "AMM_LOGOFF", adminUser.idPeople, "Uscita da amministrazione di " + adminUser.userId, DocsPaVO.Logger.CodAzione.Esito.OK);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(adminUser, "AMM_LOGOFF", adminUser.idPeople, "Uscita fallita da amministrazione di " + adminUser.userId, DocsPaVO.Logger.CodAzione.Esito.KO);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(adminUser, "AMM_LOGOFF", adminUser.idPeople, "Uscita fallita da amministrazione di " + adminUser.userId, DocsPaVO.Logger.CodAzione.Esito.KO);
                logger.Debug("Errore durante la Logout amministratore.", e);
            }

            return result;
        }

        [XmlInclude(typeof(DocsPaVO.documento.EtichettaInfo))]
        public virtual bool setEtichetteDocumenti(InfoUtenteAmministratore infoUtente, string idAmm, DocsPaVO.documento.EtichettaInfo[] etichette)
        {
            SetUserId(infoUtente);
            bool success = false;
            try
            {
                success = BusinessLogic.Documenti.EtichetteManager.SetInstance(infoUtente, idAmm, etichette);
                return success;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmUpdateAmm(ref DocsPaVO.amministrazione.InfoAmministrazione info, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, bool modFascicolatura, bool modSegnatura, bool modTimbroPdf, bool modProtTit)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = null;
            try
            {
                esito = BusinessLogic.Amministrazione.AmministraManager.UpdateAmministrazione(info, infoUtente);
                if (esito != null)
                {
                    if (modFascicolatura)
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSEGNFASC", info.IDAmm, "Modifica fascicolatura di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, info.IDAmm);
                    if (modSegnatura)
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMFORMATOSEGN", info.IDAmm, "Modifica segnatura di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, info.IDAmm);
                    if (modTimbroPdf)
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMFORMATOTIMBRO", info.IDAmm, "Modifica timbro di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, info.IDAmm);
                    if (modProtTit)
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FORMATOPROTTIT", info.IDAmm, "Modifica protocollo titolario di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, info.IDAmm);
                }
            }
            catch (Exception e)
            {
                if (modFascicolatura)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSEGNFASC", info.IDAmm, "Modifica fascicolatura di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, info.IDAmm);
                if (modSegnatura)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMFORMATOSEGN", info.IDAmm, "Modifica segnatura di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, info.IDAmm);
                if (modTimbroPdf)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMFORMATOTIMBRO", info.IDAmm, "Modifica timbro di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, info.IDAmm);
                if (modProtTit)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FORMATOPROTTIT", info.IDAmm, "Modifica protocollo titolario di AOO " + info.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, info.IDAmm);

                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateAmm - ", e);
            }

            return esito;
        }

        public List<string> GetIdRuoliProcessiUltimoUtente(string idPeople)
        {
            try
            {
                return BusinessLogic.LibroFirma.LibroFirmaManager.GetIdRuoliProcessiUltimoUtente(idPeople);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetIdRuoliProcessiUltimoUtente", e);
            }
            return null;
        }

        public GetSettingsMinimalInfoResponse GetSettingsMinimalInfo(GetSettingsMinimalInfoRequest request)
        {
            GetSettingsMinimalInfoResponse response = new GetSettingsMinimalInfoResponse();
            try
            {
                response.Settings = BusinessLogic.Utenti.RegistriRepertorioPrintManager.GetSettingsMinimalInfo(request.CounterId, request.TipologyKind, request.idAmm);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il recupero delle informazioni minimali relative alle impostazioni ad un registro di repertorio", e);
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

            return response;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovaUO(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUO nuovaUO)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            SetUserId(infoUtente);
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsNuovaUO(nuovaUO);
                if (esito.Codice != 1 || esito.Codice != 2 || esito.Codice != 3)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUO", "0", "Creazione UO " + nuovaUO.Codice + " - " + nuovaUO.Descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, nuovaUO.IDAmministrazione);
                else
                {
                    string report = string.Empty;
                    switch (esito.Codice)
                    {
                        case 1:
                            report = "Errore in creazione UO " + nuovaUO.Descrizione + ": codice già presente in amministrazione";
                            break;

                        case 2:
                            report = "Errore in creazione UO " + nuovaUO.Descrizione + ": errore generico";
                            break;

                        case 3:
                            report = "Errore in creazione UO " + nuovaUO.Descrizione + ": codice già presente in altra amministrazione";
                            break;
                    }
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUO", "0", "Errore in creazione UO " + nuovaUO.Descrizione, DocsPaVO.Logger.CodAzione.Esito.KO, nuovaUO.IDAmministrazione);
                }
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUO", "0", "Errore in creazione UO " + nuovaUO.Descrizione, DocsPaVO.Logger.CodAzione.Esito.KO, nuovaUO.IDAmministrazione);
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: AmmInsNuovaUO - ");
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public string GetMailPrincipaleRegistro(string idRegistro)
        {
            string casellaPrincipale = string.Empty;
            return BusinessLogic.Amministrazione.RegistroManager.GetMailPrincipaleRegistro(idRegistro);
        }

        public DocsPaVO.Validations.ValidationResultInfo ForceUndoCheckOutAdminDocument(InfoUtenteAmministratore adminInfoUtente, DocsPaVO.CheckInOut.CheckOutStatus checkOutAdminStatus)
        {
            SetUserId(adminInfoUtente);
            // TODO: Gestione log
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.CheckInOut.CheckInOutAdminServices.ForceUndoCheckOut(adminInfoUtente, checkOutAdminStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retValue;
        }

        public virtual DocsPaVO.utente.Amministrazione AmmModificaUoTIBCO(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            //Andrea - Fatturazione elettronica
            DocsPaVO.utente.Amministrazione retVal = new Amministrazione();

            string fatturazioneElettronicaExternalService = string.Empty;
            //Chiave per abilitare l'utilizzo della fatturazione su DB Schema esterno (PITRE_FATT)
            fatturazioneElettronicaExternalService = DocsPaVO.Settings.AppSettings.Instance.FATTURAZIONE_ELETTRONICA_EXTERNAL_SERVICE;

            if (!string.IsNullOrEmpty(fatturazioneElettronicaExternalService) && fatturazioneElettronicaExternalService.Equals("1"))
            {
                //La chiave per Fatturazione elettronica è attiva
                string fatturazioneElettronicaWebReferenceUrl = string.Empty;
                //Chiave per identificare URL del servizio di Fatturazione elettronica
                fatturazioneElettronicaWebReferenceUrl = DocsPaVO.Settings.AppSettings.Instance.FATTURAZIONE_ELETTRONICA_WEB_REFERENCE_URL;
                if (!string.IsNullOrEmpty(fatturazioneElettronicaWebReferenceUrl))
                {
                    try
                    {
                        // commentato adesso, tanto non passa per qui
                        //vedi anche AmmEliminaUoTIBCO che è pure commentato
                        //FatturazioneElettronicaServiceConsumer.FatturazioneElettronicaConsumer consumer = new FatturazioneElettronicaServiceConsumer.FatturazioneElettronicaConsumer(fatturazioneElettronicaWebReferenceUrl);
                        //retVal = consumer.ModificaUoSysExternal(oldCodiceUO, theUO, out result);
                        retVal = null;
                        result = false;
                    }
                    catch (Exception e)
                    {
                        //Il costruttore ha rilanciato un'eccezione in quanto la url non è valida
                        logger.Debug("Error costruttore FatturazioneElettronicaConsumer, Message: {0}, StackTrace: {1} ", e.Message, e.StackTrace);
                        result = false;
                        retVal = null;
                    }
                }
                else
                {
                    result = false;
                    retVal = null;
                }
            }
            else
                retVal = BusinessLogic.Amministrazione.OrganigrammaManager.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);
            //return BusinessLogic.Amministrazione.OrganigrammaManager.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);

            return retVal;
        }

        public bool IsPianoConservazioneAcquisito(string idTitolario, string idRegistro, InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.IsPianoConservazioneAcquisito(idTitolario, idRegistro, infoUtente);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovoRuolo(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo newRuolo, bool computeAtipicita)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsNuovoRuolo(infoUtente, newRuolo, computeAtipicita);
                //if (esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWROLE", newRuolo.IDCorrGlobale, "Creazione ruolo " + newRuolo.Codice + " - " + newRuolo.Descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, newRuolo.IDAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWROLE", newRuolo.IDCorrGlobale, "Creazione ruolo " + newRuolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, newRuolo.IDAmministrazione);
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: AmmInsNuovoRuolo - ");
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        /// <summary>
        /// Estra la lista delle tipologie fascicolo legate all'id nodo di classifica selezionato
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<DocsPaVO.PianoConservazione> GetPianoConservazioneByIdClassificazione(string idClassificazione, InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.GetPianoConservazioneByIdClassificazione(idClassificazione, infoUtente);
        }

        /// <summary>
        /// Inserisci una o più caselle di posta associate al registro/RF
        /// </summary>
        /// <param name="idRegistro">id del registro</param>
        /// <param name="caselle">Caselle di posta da aggiornare</param>
        /// <returns></returns>
        public DocsPaVO.Validations.ValidationResultInfo AmmInsertMailRegistro(string idRegistro, CasellaRegistro[] caselle, bool insertInteropInt)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.InsertMailRegistro(idRegistro, caselle, insertInteropInt);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertMailRegistro", e);
            }
            return retValue;
        }

        /// <summary>
        /// Verifica l'esistenza di passi di firma in cui sono coinvolti il ruolo ed i registri visibili al ruolo non inclusi nella lista in input e che verranno quindi rimmossi tra le visibilita del ruolo
        /// </summary>
        /// <param name="rightRuoloMailReg"></param>
        /// <param name="idRuoloInUO"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public bool AmmExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo)
        {
            return BusinessLogic.LibroFirma.LibroFirmaManager.ExistsPassiFirmaByRuoloTitolareAndRegistro(rightRuoloMailReg, idRuoloInUO, idGruppo);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsRegistri(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsRegistri(listaRegistri, idUO, idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsRegistri - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual int getDiagrammaAssociatoFasc(string idTipoFasc)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(idTipoFasc);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getDiagrammaAssociatoFasc", e);
                return 0;
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuoloTitolario))]
        public IList<DocsPaVO.amministrazione.OrgRuoloTitolario> AmmGetRuoliInTitolario(string idNodoTitolario, string idRegistro, string codiceRicerca, string tipoRicerca)
        {
            ArrayList retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TitolarioManager.GetRuoliInTitolario(idNodoTitolario, idRegistro, codiceRicerca, tipoRicerca);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetRuoliInTitolario", e);
            }

            return retValue.OfType<OrgRuoloTitolario>().ToList();
        }

        /// <summary>
        /// Metodo per la verifica di abilitazione di un oggetto all'IS
        /// </summary>
        /// <param name="objectId">Id dell'oggetto</param>
        /// <param name="rf">Booleano che indica se si desidera verificare un RF</param>
        /// <returns>Esito della verifica</returns>
        public bool IsElementInteroperableWithSimplifiedInteroperability(String objectId, bool rf)
        {
            try
            {
                return BusinessLogic.Interoperabilita.Semplificata.InteroperabilitaSemplificataManager.IsElementInteroperable(objectId, rf);
            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(new Exception(String.Format("Errore durante la verifica di abilitazione dell'{0} {1} all'interoperabilità semplificata.",
                    rf ? "RF" : "UO",
                    objectId)));
            }
        }

        //[XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        //public virtual ArrayList rubricaGetElementiRubrica(DocsPaVO.rubrica.ParametriRicercaRubrica qc, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        public virtual IList<DocsPaVO.rubrica.ElementoRubrica> rubricaGetElementiRubrica(DocsPaVO.rubrica.ParametriRicercaRubrica qc, InfoUtenteAmministratore u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        {
            SetUserId(u);
            ArrayList objElementiRubrica = null;
            try
            {
                logger.Debug("WS > rubricaGetElementiRubrica");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                if (qc.caller != null && qc.caller.IdUtente == null)
                    qc.caller.IdUtente = u.idPeople;
                objElementiRubrica = ccs.Search(qc, smistamentoRubrica, this._rubricaComuneService);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: rubricaGetElementiRubrica - ");
            }
            return objElementiRubrica.OfType<DocsPaVO.rubrica.ElementoRubrica>().ToList();
        }

        public virtual bool isFiltroAooEnabled()
        {
            bool filtroAOO = false;
            if (DocsPaVO.Settings.AppSettings.Instance.NO_FILTRO_AOO != null && DocsPaVO.Settings.AppSettings.Instance.NO_FILTRO_AOO == "1")
                filtroAOO = true;
            return filtroAOO;
        }

        //[XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual void rubricaCheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, InfoUtenteAmministratore u)
        {
            SetUserId(u);
            try
            {
                logger.Debug("WS > rubricaCheckChildrenExistence");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                ccs.CheckChildrenExistence(ref ers, u.idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in DocsPaWS.asmx  - metodo: rubricaCheckChildrenExistence - ");
            }
        }


        //[WebMethod]
        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.utente.Amministrazione))]
        //public virtual ArrayList GetDatiAmministrazioneByUserAdministrator(string userId, bool controllo, out string returnMsg)
        public virtual IList<DocsPaVO.utente.Amministrazione> GetDatiAmministrazioneByUserAdministrator(string userId, bool controllo, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.GetDatiAmministrazioneByUserAdministrator(userId, controllo)
                    .OfType<DocsPaVO.utente.Amministrazione>()
                    .ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetDatiAmministrazioneByUserAdministrator", e);
                returnMsg = e.ToString();
            }

            return null;
        }

        public InfoUtenteAmministratore GetDatiAmministratore(string userId, string idAmm)
        {
            return BusinessLogic.Amministrazione.AmministraManager.GetDatiAmministratore(userId, idAmm);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsTipoFunzioni(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTipoFunzione[] listaFunzioni)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            ArrayList listaFunzioniAttuali = new ArrayList();

            try
            {
                listaFunzioniAttuali = BusinessLogic.Amministrazione.OrganigrammaManager.GetListFunzioni(listaFunzioni[0].IDAmministrazione, listaFunzioni[0].Associato);
            }
            catch (Exception e)
            {
                // Errore nel reperimento delle funzioni attuali. The show must go on.
                logger.Warning(e, "Errore nel reperimento funzioni ruolo attuali.");
            }

            try
            {
                // aggiornamento funzioni associate
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsTipoFunzioni(listaFunzioni);

                // scrittura log eventi in base alla modifica delle funzioni rispetto alle precedenti
                // funzioni attivate
                foreach (OrgTipoFunzione funzione in listaFunzioni)
                {
                    bool exists = false;
                    foreach (OrgTipoFunzione funzioneAttuale in listaFunzioniAttuali)
                    {
                        if (!String.IsNullOrEmpty(funzioneAttuale.Associato) && funzioneAttuale.Codice.Equals(funzione.Codice))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMASSFUNZRUOLO", funzione.Associato, "Associazione funzione a ruolo " + funzione.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, funzione.IDAmministrazione);
                }

                // funzioni disattivate
                foreach (OrgTipoFunzione funzioneAttuale in listaFunzioniAttuali)
                {
                    bool removed = true;
                    foreach (OrgTipoFunzione funzione in listaFunzioni)
                    {
                        if (String.IsNullOrEmpty(funzioneAttuale.Associato) || funzioneAttuale.Codice.Equals(funzione.Codice))
                        {
                            removed = false;
                            break;
                        }
                    }

                    if (removed)
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISASSFUNZRUOLO", funzioneAttuale.Associato, "Disassociazione funzione a ruolo " + funzioneAttuale.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, listaFunzioni[0].IDAmministrazione);
                }

                //if (esito.Codice == 0)
                //{
                // BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMASSFUNZRUOLO", listaFunzioni[0].Associato, "Associazione funzioni a ruolo " + listaFunzioni[0].Codice, DocsPaVO.Logger.CodAzione.Esito.OK, listaFunzioni[0].IDAmministrazione);
                // BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISASSFUNZRUOLO", listaFunzioni[0].Associato, "Disassociazione funzioni a ruolo " + listaFunzioni[0].Codice, DocsPaVO.Logger.CodAzione.Esito.OK, listaFunzioni[0].IDAmministrazione);
                //}
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMASSFUNZRUOLO", listaFunzioni[0].Associato, "Associazione funzioni a ruolo", DocsPaVO.Logger.CodAzione.Esito.KO, listaFunzioni[0].IDAmministrazione);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISASSFUNZRUOLO", listaFunzioni[0].Associato, "Disassociazione funzioni a ruolo", DocsPaVO.Logger.CodAzione.Esito.KO, listaFunzioni[0].IDAmministrazione);
                logger.Error(e, "Errore in DocsPaWS.asmx  - metodo: AmmInsTipoFunzioni - ");
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByID(string idAmm, string idModello)
        {

            try
            {
                return BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(idAmm, idModello);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getModelloByID", e);
                return null;
            }
        }

        //[XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual DocsPaVO.rubrica.ElementoRubrica rubricaGetElementoRubricaSimpleBySystemId(string systemId, InfoUtenteAmministratore u)
        {
            SetUserId(u);
            try
            {
                logger.Debug("WS > rubricaGetElementoRubricaSimpleBySystemId");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                return ccs.SearchSingleSimpleBySystemId(systemId);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: rubricaGetElementoRubricaSimpleBySystemId - ", e);
                return null;
            }
        }

        public bool isCorrispondenteValid(string userId)
        {
            try
            {
                SetUserId(userId);
                return BusinessLogic.Utenti.addressBookManager.isCorrispondenteValido(userId);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Errore in DocsPaWS.asmx  - metodo: isCorrispondenteValid", e);
            }
        }

        public virtual bool isEnableConversionePdfLatoServer()
        {
            return BusinessLogic.Documenti.DocManager.isEnabledConversionePdfServer();
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmModUO(DocsPaVO.amministrazione.OrgUO theUO, bool StoricizzUO)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmModUO(theUO, StoricizzUO);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmModUO - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        //[XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual void rubricaCheckChildrenExistenceEx(ref DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, InfoUtenteAmministratore u)
        {
            SetUserId(u);
            try
            {
                logger.Debug("WS > rubricaCheckChildrenExistenceEx");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                ccs.CheckChildrenExistence(ref ers, checkUo, checkRuoli, checkUtenti, u.idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: rubricaCheckChildrenExistenceEx - ", e);
            }
        }

        //[XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        //public virtual ArrayList rubricaGetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        public IList<DocsPaVO.rubrica.ElementoRubrica> rubricaGetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, InfoUtenteAmministratore u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        {
            SetUserId(u);
            ArrayList objElementiRubrica = null;

            try
            {
                logger.Debug("WS > rubricaGetRootItems");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                objElementiRubrica = ccs.GetRootItems(tipoIE, smistamentoRubrica);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: rubricaGetRootItems - ", e);
            }
            return objElementiRubrica?.OfType<DocsPaVO.rubrica.ElementoRubrica>().ToList();
        }

        public virtual void salvaAssociazioneDocRuoli(ArrayList assDocRuoli)
        {
            try
            {
                var converted = new ArrayList();
                foreach (XmlNode[] ruolo in assDocRuoli)
                {
                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in ruolo)
                    {
                        switch (field.Name)
                        {
                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                            case "DIRITTI_TIPOLOGIA":
                                elemento.DIRITTI_TIPOLOGIA = field.InnerText;
                                break;
                        }
                    }
                    converted.Add(elemento);
                }

                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaAssociazioneDocRuoli(converted);
                //BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaAssociazioneDocRuoli(assDocRuoli);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaAssociazioneDocRuoli", e);
            }

        }

        /// <summary>
        /// GIUGNO 2008 - Adamo
        /// MODELLI DI TRASMISSIONE:
        /// Gestione della notifica trasmissione degli utenti inserito nei modelli di trasmissione
        /// </summary> 
        /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>
        /// <param name="operazione">Tipo di operazione da effettuare sul db:   'GET' = reperimento dati,   'SET' = modifica dei dati </param>
        /// <returns>L'oggetto stesso passato come parametro. Oggetto NULL se ci sono state eccezioni nel metodo</returns>
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione UtentiConNotificaTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, ArrayList utentiDaInserire, ArrayList utentiDaCancellare, string operazione)
        {
            try
            {
                return BusinessLogic.Trasmissioni.ModelliTrasmissioni.UtentiConNotificaTrasm(objModTrasm, utentiDaInserire, utentiDaCancellare, operazione);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: UtentiConNotificaTrasm", e);
                objModTrasm = null;
                return objModTrasm;
            }
        }

        public virtual bool isUniqueNameDiagramma(string nomeDiagramma)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.isUniqueNameDiagramma(nomeDiagramma);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: isUniqueNameDiagramma", e);
                return false;
            }
        }

        public virtual void salvaDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg, string idAmm)
        {
            try
            {
                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaDiagramma(dg, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaDiagramma", e);
            }
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.utente.Ruolo))]
        //public virtual ArrayList GetListaRuoliUtente(string idPeople)
        public virtual IList<DocsPaVO.utente.Ruolo> GetListaRuoliUtente(string idPeople)
        {
            ArrayList listaRuoli = null;
            try
            {
                listaRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetListaRuoliUtente", e);
            }
            return listaRuoli.OfType<Ruolo>().ToList();
        }

        public virtual DocsPaVO.documento.FileDocumento AmmExportTabellaConfineIntegrazioni(string idAmministrazione)
        {
            try
            {
                BusinessLogic.ExportDati.ExportDatiManager exportDati = new BusinessLogic.ExportDati.ExportDatiManager();
                return exportDati.ExportPianoCons_IntegrPIS(idAmministrazione, this.spreadsheetService);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmExportTabellaConfineIntegrazioni", e);
                return null;
            }
        }

        /// <summary>
        /// Inserimento di una nuova amministrazione
        /// </summary>
        /// <param name="info"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsertAmm(ref DocsPaVO.amministrazione.InfoAmministrazione info, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = null;
            SetUserId(infoUtente);
            try
            {
                esito = BusinessLogic.Amministrazione.AmministraManager.InsertAmministrazione(info, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertAmm - ", e);
            }

            return esito;
        }

        public bool SetFascicolazioneTipiDocumento(List<DocsPaVO.documento.FascicolazioneTipiDocumento> listTipiDoc, string idAmm, InfoUtenteAmministratore infoutente)
        {
            return BusinessLogic.Amministrazione.AmministraManager.SetFascicolazioneTipiDocumento(listTipiDoc, idAmm, infoutente);
        }

        /// <summary>
        /// Cancellazione di un'amministrazione esistente
        /// </summary>
        /// <param name="info"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmDeleteAmm(ref DocsPaVO.amministrazione.InfoAmministrazione info, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = null;

            try
            {
                esito = BusinessLogic.Amministrazione.AmministraManager.DeleteAmministrazione(info, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmDeleteAmm - ", e);
            }

            return esito;
        }

        /// <summary>
        /// Estrae per la tipologia documento in input il piano di conservazione(tipologia fascicolo) associato
        /// </summary>
        /// <param name="idTipoAtto"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoAtto(string idTipoAtto, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.GetPianoConservazioneByIdTipoAtto(idTipoAtto);
        }

        /// <summary>
        /// Estra la lista delle tipologie fascicolo legate al codice nodo di classifica selezionato
        /// </summary>
        /// <param name="codiceClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<DocsPaVO.PianoConservazione> GetPianoConservazioneByCodiceClassificazione(string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmministrazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.GetPianoConservazioneByCodiceClassificazione(codiceClassificazione, registro, idAmministrazione, infoUtente);
        }

        /// <summary>
        /// Estrae per l'ex tipologia fascicolo in input il piano di conservazione(tipologia fascicolo) associato
        /// </summary>
        /// <param name="idTipoFasc"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoFasc(string idTipoFasc, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.GetPianoConservazioneByIdTipoFasc(idTipoFasc);
        }

        /// <summary>
        /// Associa ad una tipologia documento il piano di conservazione(tipologia fascicolo) in input
        /// </summary>
        /// <param name="idPianoConservazione"></param>
        /// <param name="idTipoAtto"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool InsertPianoConservazioneTipoAtto(string idPianoConservazione, string idTipoAtto, string idAmministrazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.InsertPianoConservazioneTipoAtto(idPianoConservazione, idTipoAtto, idAmministrazione);
        }

        /// <summary>
        /// Modifica il piano di conservazione (tipologia fascicolo) associato ad una tipologia documento
        /// </summary>
        /// <param name="idTipoAtto"></param>
        /// <param name="idPianoConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool UpdatePianoConservazioneTipoAtto(string idTipoAtto, string idPianoConservazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.UpdatePianoConservazioneTipoAtto(idTipoAtto, idPianoConservazione);
        }

        public virtual DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteByIdPeople(string idPeople, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.amministrazione.InfoUtenteAmministratore u)
        {
            SetUserId(u);
            DocsPaVO.utente.Corrispondente corr = null;

            try
            {
                logger.Debug("WS > AddressbookGetCorrispondenteByIdPeople");
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(idPeople, tipoIE, u);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AddressbookGetCorrispondenteByIdPeople - ", e);
            }
            return corr;
        }

        public virtual void salvaModelli(byte[] dati, string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                string serverPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "REPOSITORY_ROOT_PATH"); 
                //string serverPath = this.Server.MapPath("xml");
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaModelli(dati, nomeProfilo, codiceAmministrazione, nomeFile, estensione, serverPath, template);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: salvaModelli", e);
            }
        }

        /// <summary>
        /// Importazione della tabella di confine delle integrazioni
        /// </summary>
        /// <param name="dati"></param>
        /// <param name="idTitolario"></param>
        /// <param name="idAmm"></param>
        /// <param name="idRegistro"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool AmmImportTabellaConfineIntegrazioni(byte[] dati, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {

            string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
            serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.ImportPianoCons_IntegrPIS(dati, infoUtente, this.spreadsheetService, serverPath);
        }


        /// <summary>
        /// Associa ad un ex tipologia fascicolo il piano di conservazione(tipologia fascicolo) in input
        /// </summary>
        /// <param name="idPianoConservazione"></param>
        /// <param name="idTipoFasc"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool InsertPianoConservazioneTipoFasc(string idPianoConservazione, string idTipoFasc, string idAmministrazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.InsertPianoConservazioneTipoFasc(idPianoConservazione, idTipoFasc, idAmministrazione);
        }

        [XmlInclude(typeof(DocsPaVO.utente.Corrispondente))]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        //public virtual ArrayList AddressbookGetListaCorrispondenti_Aut(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        public virtual IList<DocsPaVO.utente.Corrispondente> AddressbookGetListaCorrispondenti_Aut(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        {
            ArrayList result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Utenti.addressBookManager.getListaCorrispondentiAutProtInt(queryCorrispondente);
#if TRACE_WS
				pt.WriteLogTracer("AddressbookGetListaCorrispondenti");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AddressbookGetListaCorrispondenti", e);
                result = null;
            }

            return result.OfType<DocsPaVO.utente.Corrispondente>().ToArray();
        }

        //[WebMethod]
        //[XmlInclude(typeof(DocsPaVO.utente.Corrispondente))]
        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        //public virtual ArrayList AddressbookGetListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        //        public virtual IList<Corrispondente> AddressbookGetListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        public virtual IList<Ruolo> AddressbookGetListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        {
            ArrayList result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(queryCorrispondente);
#if TRACE_WS
				pt.WriteLogTracer("AddressbookGetListaCorrispondenti");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AddressbookGetListaCorrispondenti", e);
                result = null;
            }

            return result.OfType<Ruolo>().ToArray();
        }

        public virtual void inviaNotificaMail(DocsPaVO.amministrazione.OrgUO theUO, DocsPaVO.utente.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO)
        {
            BusinessLogic.Amministrazione.OrganigrammaManager.inviaNotificaMail(theUO, amm, descrizioneAOO, tipoOperazione, oldCodiceUO);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmSpostaUO(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUO uoDaSpostare, DocsPaVO.amministrazione.OrgUO uoPadre)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmSpostaUO(uoDaSpostare, uoPadre);
                //if(esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSPOSTAUO", uoDaSpostare.IDCorrGlobale, "Spostamento UO " + uoDaSpostare.Codice + " sotto " + uoPadre.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, uoDaSpostare.IDAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMSPOSTAUO", uoDaSpostare.IDCorrGlobale, "Spostamento UO " + uoDaSpostare.Codice + " sotto " + uoPadre.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, uoDaSpostare.IDAmministrazione);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmSpostaUO - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUtente))]
        //public virtual ArrayList AmmGetListUtentiRuolo(string idRuolo)
        public virtual IList<DocsPaVO.amministrazione.OrgUtente> AmmGetListUtentiRuolo(string idRuolo)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListUtentiRuolo(idRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListUtentiRuolo - ", e);
            }

            return retValue.OfType<OrgUtente>().ToList();
        }

        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUtente))]
        //public virtual ArrayList AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
        public virtual IList<DocsPaVO.amministrazione.OrgUtente> AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
        {
            ArrayList retValue = new ArrayList();

            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetListUtenti(idAmm, ricercaPer, testoDaRicercare, IDesclusi);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetListUtenti - ", e);
            }

            return retValue.OfType<OrgUtente>().ToList();
        }

        public string GetCodLiv(string codliv, string livello, string codAmm, string idTitolario, string idRegistro)
        {
            string result = "";
            try
            {
                BusinessLogic.AmministrazioneXml.TitolarioXml obj = new BusinessLogic.AmministrazioneXml.TitolarioXml();
                result = obj.getCodLiv(codliv, livello, codAmm, idTitolario, idRegistro);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS GetCodLiv: ", exception);
            }
            return result;
        }

        /// <summary>
        /// Inserimento di un nuovo nodo di titolario.
        /// Per default, vengono ereditati il registro associato
        /// e la visibilità dei ruoli del nodo padre (se presente).
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmInsertTitolario(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, string idAmm)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;
            SetUserId(infoUtente);
            try
            {
                retValue = BusinessLogic.Amministrazione.TitolarioManager.InsertNodoTitolario(infoUtente, ref nodoTitolario);
                if (retValue.Codice != -1)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWNODO", nodoTitolario.ID, "Creazione nodo titolario " + nodoTitolario.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWNODO", nodoTitolario.ID, "Creazione nodo titolario " + nodoTitolario.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmInsertTitolario", e);
                retValue.Codice = 1;
                retValue.Descrizione = "si è verificato un errore";
            }

            return retValue;
        }

        /// <summary>
        /// Estra il dettaglio del singolo piano di conservazione (tipologia fascicolo) tramite l'id Piano conservazione
        /// </summary>
        /// <param name="idPianoConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.PianoConservazione GetPianoConservazioneById(string idPianoConservazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.GetPianoConservazioneById(idPianoConservazione, infoUtente);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmOnlyDisabledRole(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmOnlyDisabledRole(infoUtente, ruolo);
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMONLYDISABLEDROLE", ruolo.IDCorrGlobale, "Disabilitazione ruolo " + ruolo.Codice, DocsPaVO.Logger.CodAzione.Esito.OK, ruolo.IDAmministrazione);
            }
            catch (Exception e)
            {
                esito.Codice = 10;
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmOnlyDisabledRole - ", e);
                esito.Descrizione = "si è verificato un errore";
            }
            return esito;
        }

        public DocsPaVO.utente.RoleHistoryResponse GetRoleHistory(DocsPaVO.utente.RoleHistoryRequest request)
        {
            try
            {
                return BusinessLogic.Utenti.UserManager.GetRoleHistory(request);

            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

        }

        public virtual string AmmGetVisibNodoTit_InRuolo(string idNodoTitolario, string idGruppo)
        {
            string retValue = string.Empty;
            try
            {
                retValue = BusinessLogic.Amministrazione.TitolarioManager.AmmGetVisibilitaNodoTit_InRuolo(idNodoTitolario, idGruppo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmGetVisibilitaNodoTit_InRuolo", e);
            }
            return retValue;
        }

        public DocsPaVO.LibroFirma.IstanzaProcessoDiFirma[] GetIstanzeProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> listaProcessiDiFirma = new List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma>();
            numTotPage = 0;
            nRec = 0;
            try
            {
                listaProcessiDiFirma = BusinessLogic.LibroFirma.LibroFirmaManager.GetIstanzaProcessiDiFirmaByTitolarePaging(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: GetIstanzeProcessiDiFirmaByTitolare", e);
            }
            return listaProcessiDiFirma.ToArray();
        }

        public OrgRuolo GetRole(string idCorrGlobRuolo)
        {
            return BusinessLogic.Amministrazione.OrganigrammaManager.GetRole(idCorrGlobRuolo);
        }


        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUO(InfoUtenteAmministratore infoUtente, string idCorrGlob)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUO(infoUtente, idCorrGlob, _rubricaComuneService);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaUO - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.utente.Amministrazione AmmEliminaUoTIBCO(DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            //Andrea - Fatturazione elettronica
            DocsPaVO.utente.Amministrazione retVal = new Amministrazione();

            string fatturazioneElettronicaExternalService = string.Empty;
            //Chiave per abilitare l'utilizzo della fatturazione su DB Schema esterno (PITRE_FATT)
            fatturazioneElettronicaExternalService = System.Configuration.ConfigurationManager.AppSettings["FATTURAZIONE_ELETTRONICA_EXTERNAL_SERVICE"];

            if (!string.IsNullOrEmpty(fatturazioneElettronicaExternalService) && fatturazioneElettronicaExternalService.Equals("1"))
            {
                //La chiave per Fatturazione elettronica è attiva
                string fatturazioneElettronicaWebReferenceUrl = string.Empty;
                //Chiave per identificare URL del servizio di Fatturazione elettronica
                fatturazioneElettronicaWebReferenceUrl = System.Configuration.ConfigurationManager.AppSettings["FATTURAZIONE_ELETTRONICA_WEB_REFERENCE_URL"];
                if (!string.IsNullOrEmpty(fatturazioneElettronicaWebReferenceUrl))
                {
                    try
                    {
                        // commentato per adesso, come in AmmModificaUoTIBCO
                        //FatturazioneElettronicaServiceConsumer.FatturazioneElettronicaConsumer consumer = new FatturazioneElettronicaServiceConsumer.FatturazioneElettronicaConsumer(fatturazioneElettronicaWebReferenceUrl);
                        //retVal = consumer.EliminaUoSysExternal(theUO, out result);
                        //return consumer.EliminaUoSysExternal(theUO, out result);
                        retVal = null;
                        result = false;
                    }
                    catch (Exception e)
                    {
                        //Il costruttore ha rilanciato un'eccezione in quanto la url non è valida
                        logger.Debug("Error costruttore FatturazioneElettronicaConsumer, Message: {0}, StackTrace: {1} ", e.Message, e.StackTrace);
                        result = false;
                        retVal = null;
                        //return null;
                    }
                }
                else
                {
                    result = false;
                    retVal = null;
                    //return null;
                }
            }
            else
                retVal = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUoTIBCO(theUO, out result);
            //return BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUoTIBCO(theUO, out result);

            return retVal;
            //End Andrea - Fatturazione elettronica.

            //Prima della modifica Fatturazione per lo schema PITRE_FATT:
            //return BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUoTIBCO(theUO, out result);
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEliminaRuolo(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaRuolo(infoUtente, ruolo);
                //if (esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISABILROLE", ruolo.IDCorrGlobale, "Disabilitazione ruolo " + ruolo.Codice + " - " + ruolo.Descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, ruolo.IDAmministrazione);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISABILROLE", ruolo.IDCorrGlobale, "Disabilitazione ruolo " + ruolo.Codice, DocsPaVO.Logger.CodAzione.Esito.KO, ruolo.IDAmministrazione);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEliminaRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmVerificaUtenteRespStampaRep(string userId, string roleId, string idAmm)
        {
            SetUserId(userId);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmVerificaUtenteRespStampaRep(userId, roleId, idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: AmmVerificaUtenteRespStampaRep - ", ex);
                esito.Codice = 1;
                esito.Descrizione = "Si è verificato un errore";
            }

            return esito;
        }

        public virtual void estendiDirittiRuoloACampiDoc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
        {
            try
            {

                var listaDirittiRuoliConverted = new ArrayList();
                foreach (XmlNode[] modelloXml in listaDirittiRuoli)
                {

                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "ID_OGGETTO_CUSTOM":
                                elemento.ID_OGGETTO_CUSTOM = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                            case "DIRITTI_TIPOLOGIA":
                                elemento.DIRITTI_TIPOLOGIA = field.InnerText;
                                break;
                            case "INS_MOD_OGG_CUSTOM":
                                elemento.INS_MOD_OGG_CUSTOM = field.InnerText;
                                break;
                            case "VIS_OGG_CUSTOM":
                                elemento.VIS_OGG_CUSTOM = field.InnerText;
                                break;
                            case "ANNULLA_REPERTORIO":
                                elemento.ANNULLA_REPERTORIO = field.InnerText;
                                break;





                        }
                    }
                    listaDirittiRuoliConverted.Add(elemento);
                }


                var listaCampiConverted = new ArrayList();
                foreach (XmlNode[] modelloXml in listaCampi)
                {

                    var elemento = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                            case "SYSTEM_ID":
                                elemento.SYSTEM_ID = (int)field.InnerText.AsLong();
                                break;


                        }
                    }
                    listaCampiConverted.Add(elemento);
                }
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.estendiDirittiRuoloACampiDoc(listaDirittiRuoliConverted, listaCampiConverted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: estendiDirittiRuoloACampiDoc", e);
            }

        }

        public virtual DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma[] getRuoliStatiDiagramma(int idDiagramma)
        {
            try
            {
                return BusinessLogic.DiagrammiStato.DiagrammiStato.getRuoliStatiDiagramma(idDiagramma).ToArray();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: getRuoliTipoDoc", e);
                return null;
            }
        }


        public virtual bool ModifyRuoloStatiDiagramma(DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma[] listAssRoleStatiDia)
        {
            return BusinessLogic.DiagrammiStato.DiagrammiStato.ModifyRuoloStatiDiagramma(new
                                                            List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma>(listAssRoleStatiDia));
        }

        public DocsPaVO.Validations.ValidationResultInfo AmmUpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.RegistroManager.UpdateMailRegistro(idRegistro, caselle);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateMailRegistro", e);
            }

            return retValue;
        }



        public virtual string GetLivelloTipoRuolo(string idCorrGlobRuolo)
        {
            string retValue = string.Empty;
            try
            {
                retValue = BusinessLogic.Amministrazione.OrganigrammaManager.GetLivelloTipoRuolo(idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetLivelloTipoRuolo - ", e);
            }

            return retValue;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmEstendeVisibRuolo(InfoUtenteAmministratore infoUtente, string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string idAmm, string livelloRuolo, bool escludiAtipicita)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEstendeVisibRuolo(infoUtente, idRegistro, idCorrGlobRuolo, idGruppo, idCorrGlobUO, idAmm, livelloRuolo, escludiAtipicita);
                //if (esito.Codice == 3 || esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMVISRUOLO", idCorrGlobRuolo, "Imposta visibilita al ruolo " + idGruppo, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMVISRUOLO", idCorrGlobRuolo, "Imposta visibilita al ruolo " + idGruppo, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmEstendeVisibRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }


        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmAbilitaUtente(InfoUtenteAmministratore infoUtente, string idPeople, string idAmm)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
                string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.GetCodAmmById(idAmm);

                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmAbilitaUtente(infoUtente, idPeople);
                if (esito.Codice == 9)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMABILUSER", idPeople, "Abilitazione utente " + utente.userId + " - " + utente.descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMABILUSER", idPeople, "Abilitazione utente " + idPeople, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Error("Errore in DocsPaWS.asmx  - metodo: AmmAbilitaUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmDisabilitaUtente(InfoUtenteAmministratore infoUtente, string idPeople, string idAmm)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmDisabilitaUtente(infoUtente, idPeople);
                DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
                string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.GetCodAmmById(idAmm);
                //if (esito.Codice == 0)
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISABILUSER", idPeople, "Disabilitazione utente " + utente.userId + " - " + utente.descrizione, DocsPaVO.Logger.CodAzione.Esito.OK, idAmm);
            }
            catch (Exception e)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMDISABILUSER", idPeople, "Disabilitazione utente " + idPeople, DocsPaVO.Logger.CodAzione.Esito.KO, idAmm);
                logger.Error("Errore in DocsPaWS.asmx  - metodo: AmmDisabilitaUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        [XmlInclude(typeof(DocsPaVO.Report.PrintReportObjectTransformationRequest))]
        public DocsPaVO.Report.PrintReportResponse GetReportRegistry(string contextName)
        {
            DocsPaVO.Report.PrintReportResponse response = null;

            try
            {
                response = BusinessLogic.Reporting.ReportGeneratorCommand.GetRegisteredReport(contextName);
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la generazione dell'anagrafica dei report registrati nel sistema.", e);
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);

            }

            return response;
        }

        [XmlInclude(typeof(DocsPaVO.Report.PrintReportRequestDataset))]
        public DocsPaVO.Report.PrintReportResponse GenerateReport(DocsPaVO.Report.PrintReportRequest request)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();
                DocsPaVO.Report.PrintReportResponse response = null;


                try
                {
                    response = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request, spreadsheetService, configurationService, _reportGeneratorService);
                }
                catch (Exception e)
                {
                    logger.Debug("Errore durante la generazione del file di report {0}.", request.ReportKey, e);
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);

                }

                return response;
            }
        }


        [XmlInclude(typeof(DocsPaVO.rubrica.ElementoRubrica))]
        public virtual DocsPaVO.rubrica.ElementoRubrica rubricaGetElementoRubrica(string cod, InfoUtenteAmministratore u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, string condRegistri)
        {
            SetUserId(u);
            try
            {
                logger.Debug("WS > rubricaGetElementoRubrica");
                BusinessLogic.Rubrica.DPA3_RubricaSearchAgent ccs = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(u);
                return ccs.SearchSingle(cod, smistamentoRubrica, condRegistri, this._rubricaComuneService);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: rubricaGetElementoRubrica - ", e);
                return null;
            }
        }
        public bool ImportPianoConservazione(byte[] dati, string idTitolario, string idAmm, string idRegistro, InfoUtenteAmministratore infoUtente)
        {
            try
            {

                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));
                return BusinessLogic.PianoConservazione.PianoConservazioneManager.ImportPianoConservazione(dati, idTitolario, idAmm, idRegistro, infoUtente, this.spreadsheetService, serverPath);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ExportPianoConservazione", e);
                return false;
            }

        }

        public virtual DocsPaVO.documento.FileDocumento ExportPianoConservazione(DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro)
        {
            try
            {
                string serverPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));

                BusinessLogic.ExportDati.ExportDatiManager exportDati = new BusinessLogic.ExportDati.ExportDatiManager();
                return exportDati.ExportPianoConservazione(titolario, idRegistro, this.spreadsheetService, serverPath);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: ExportPianoConservazione", e);
                return null;
            }
        }

        //public virtual ArrayList GetLogImportPianoConservazione()
        public virtual IList<string> GetLogImportPianoConservazione()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));
                logFile = BusinessLogic.PianoConservazione.PianoConservazioneManager.GetLogImportPianoConservazione(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetLogImportPianoConservazione", e);
                return logFile.OfType<string>().ToList();
            }
        }

        //public virtual ArrayList GetLogImportTabellaConfineIntegrazioni()
        public virtual IList<string> GetLogImportTabellaConfineIntegrazioni()
        {
            ArrayList logFile = new ArrayList();

            try
            {
                string serverPath = _configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
                serverPath = System.IO.Path.Combine(serverPath, DateTime.Now.ToString("yyyyMMdd"));
                logFile = BusinessLogic.PianoConservazione.PianoConservazioneManager.GetLogImportTabellaConfine(serverPath);
                return logFile.OfType<string>().ToList();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetLogImportTabellaConfineIntegrazioni", e);
                return logFile.OfType<string>().ToList();
            }
        }

        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.trasmissione.RagioneTrasmissione))]
        public virtual IList<DocsPaVO.trasmissione.RagioneTrasmissione> TrasmissioneGetRagioniATutti(DocsPaVO.trasmissione.Diritti diritti)
        {
            ArrayList result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Trasmissioni.RagioniManager.getListaRagioniATutti(diritti);
#if TRACE_WS
				pt.WriteLogTracer("TrasmissioneGetRagioni");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneGetRagioniATutti", e);
                result = null;
            }


            return result?.OfType<DocsPaVO.trasmissione.RagioneTrasmissione>().ToList();

        }


        public bool CheckConnection(out string exceptionMessage)
        {
            bool result = true; // Presume successo
            exceptionMessage = null;

            try
            {
                result = BusinessLogic.Amministrazione.UtenteManager.Checkconnection();

            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message + " " + ex.StackTrace;
                result = false;
            }

            return result;
        }

        //public virtual ArrayList DO_GetRegistriAoo(int idAmm)
        public virtual IList<PR_Registro> DO_GetRegistriAoo(int idAmm)
        {
            ArrayList result = new ArrayList();
            try
            {
                ProspettiRiepilogativi.Model objM = new ProspettiRiepilogativi.Model();
                result = objM.DO_GetRegistriAoo(idAmm);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel recupero della connectionString - metodo: DO_GetDBType", ex);
            }
            return result.OfType<PR_Registro>().ToList(); ;
        }

        public DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse FindAndReplaceRoleInModelliTrasmissione(DocsPaVO.Modelli_Trasmissioni.FindAndReplaceRequest request)
        {
            DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse response = null;

            try
            {
                response = BusinessLogic.Trasmissioni.ModelliTrasmissioni.FindAndReplaceRoleInModelliTrasmissione(request);
            }
            catch (Exception e)
            {
                DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

            return response;
        }

        public virtual void estendiDirittiRuoloACampiFasc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
        {
            try
            {
                var listaDirittiRuoliConverted = new ArrayList();
                foreach (XmlNode[] modelloXml in listaDirittiRuoli)
                {

                    var elemento = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                            case "ID_TIPO_DOC_FASC":
                                elemento.ID_TIPO_DOC_FASC = field.InnerText;
                                break;
                            case "ID_OGGETTO_CUSTOM":
                                elemento.ID_OGGETTO_CUSTOM = field.InnerText;
                                break;
                            case "ID_GRUPPO":
                                elemento.ID_GRUPPO = field.InnerText;
                                break;
                            case "DIRITTI_TIPOLOGIA":
                                elemento.DIRITTI_TIPOLOGIA = field.InnerText;
                                break;
                            case "INS_MOD_OGG_CUSTOM":
                                elemento.INS_MOD_OGG_CUSTOM = field.InnerText;
                                break;
                            case "VIS_OGG_CUSTOM":
                                elemento.VIS_OGG_CUSTOM = field.InnerText;
                                break;
                            case "ANNULLA_REPERTORIO":
                                elemento.ANNULLA_REPERTORIO = field.InnerText;
                                break;
                        }
                    }
                    listaDirittiRuoliConverted.Add(elemento);
                }

                var listaCampiConverted = new ArrayList();
                foreach (XmlNode[] modelloXml in listaCampi)
                {

                    var elemento = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                            case "SYSTEM_ID":
                                elemento.SYSTEM_ID = (int)field.InnerText.AsLong();
                                break;

                        }
                    }
                    listaCampiConverted.Add(elemento);
                }

                BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.estendiDirittiRuoloACampiFasc(listaDirittiRuoliConverted, listaCampiConverted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: estendiDirittiRuoloACampiFasc", e);
            }

        }

        public SaveRegistroRepertorioSettingsResponse SaveRegisterSettings(SaveRegistroRepertorioSettingsRequest request)
        {
            SaveRegistroRepertorioSettingsResponse response = new SaveRegistroRepertorioSettingsResponse();
            try
            {
                ValidationResultInfo validationResult;
                bool result = BusinessLogic.Utenti.RegistriRepertorioPrintManager.SaveRegisterSettings(request.CounterId, request.Settings, request.TipologyKind, request.SettingsToSave, request.IdAdmin, out validationResult);

                response.ValidationResult = validationResult;
                response.SaveChangesResult = result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il salvataggio delle modifiche ad un registro di repertorio", e);
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

            return response;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmUpdateTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            SetUserId(infoUtente);
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;

            try
            {
                retValue = BusinessLogic.Amministrazione.TitolarioManager.UpdateNodoTitolario(infoUtente, nodoTitolario);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmUpdateTitolario", e);
                retValue.Codice = 1;
                retValue.Descrizione = "si è verificato un errore";
            }

            return retValue;
        }

        public DocsPaVO.documento.FileDocumento GetReportProcessiFirma(string idRuolo, string idUtente, string formato)
        {
            DocsPaVO.documento.FileDocumento retVal = new DocsPaVO.documento.FileDocumento();

            try
            {

                using (var scope = serviceProvider.CreateScope())
                {

                    var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();
                    retVal = BusinessLogic.LibroFirma.LibroFirmaManager.GetReportProcessiFirma(idRuolo, idUtente, formato, spreadsheetService, configurationService, _reportGeneratorService);

                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetReportProcessiFirma", ex);
                retVal = null;
            }

            return retVal;
        }

        public virtual bool DelegaVerificaUnicaAssegnataAmm(DocsPaVO.Deleghe.InfoDelega delega)
        {
            bool result = false;
            try
            {
                result = BusinessLogic.Deleghe.DelegheManager.verificaUnicaDelegaAmm(delega);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DelegaVerificaUnicaAssegnata", e);
            }
            return result;
        }

        public virtual bool DelegaCreaNuova(InfoUtenteAmministratore infoUtente, DocsPaVO.Deleghe.InfoDelega delega)
        {
            SetUserId(infoUtente);
            bool result = false;
            try
            {
                result = BusinessLogic.Deleghe.DelegheManager.creaNuovaDelega(infoUtente, delega);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DelegaCreaNuova", e);
            }
            return result;
        }

        public virtual bool DelegaRevoca(InfoUtenteAmministratore infoUtente, DocsPaVO.Deleghe.InfoDelega[] listaDeleghe, out string msg)
        {
            SetUserId(infoUtente);
            bool result = false;
            msg = string.Empty;
            try
            {
                result = BusinessLogic.Deleghe.DelegheManager.revocaDelega(infoUtente, listaDeleghe, out msg);
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il web method DelegaRevoca", e);
            }
            return result;
        }

        public virtual bool DelegaModificaAmm(DocsPaVO.Deleghe.InfoDelega delegaOld, DocsPaVO.Deleghe.InfoDelega delegaNew, string tipoDelega, string dataScadenzaOld, string dataDecorrenzaOld)
        {
            bool result = false;
            try
            {
                result = BusinessLogic.Deleghe.DelegheManager.modificaDelegaAmm(delegaOld, delegaNew, tipoDelega, dataScadenzaOld, dataDecorrenzaOld);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: DelegaModificaAmm", e);
            }
            return result;
        }

        public virtual ArrayList UtenteGetRegistriWithRf(string idCorrGlobali, string all, string idAooColl)
        {
            ArrayList result = null;

            try
            {
                result = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(idCorrGlobali, all, idAooColl);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: UtenteGetRegistriWithRf", e);

                result = null;
            }
            return result;
        }

        public DocsPaVO.Validations.ValidationResultInfo SaveSupportedFileType(ref DocsPaVO.FormatiDocumento.SupportedFileType fileType)
        {
            try
            {
                return BusinessLogic.FormatiDocumento.SupportedFormatsManager.SaveFileType(ref fileType);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: SaveSupportedFileType", e);

                throw e;
            }
        }

        public virtual void eliminaModelli(string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            try
            {
                //string serverPath = System.Configuration.ConfigurationManager.AppSettings["MODELS_ROOT_PATH"];
                string serverPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH"); ;
                //string serverPath = this.Server.MapPath("xml");
                BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.eliminaModelli(nomeProfilo, codiceAmministrazione, nomeFile, estensione, serverPath, template);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: eliminaModelli", e);
            }
        }
        public DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniDoc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni)
        {
            try
            {
                var listaCampiConverted = new ArrayList();
                foreach (XmlNode[] modelloXml in campiComuni)
                {

                    var elemento = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    foreach (var field in modelloXml)
                    {
                        switch (field.Name)
                        {
                            case "SYSTEM_ID":
                                elemento.SYSTEM_ID = (int)field.InnerText.AsLong();
                                break;
                            case "ID_AOO_RF":
                                elemento.ID_AOO_RF =field.InnerText;
                                break;
                        }
                    }
                    listaCampiConverted.Add(elemento);
                }
                return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.impostaCampiComuniDoc(modello, listaCampiConverted);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: impostaCampiComuniDoc", e);
                return null;
            }
        }
        public bool SalvaCessioneDirittiSuModelliTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objTrasm)
        {
            try
            {
                return BusinessLogic.Trasmissioni.ModelliTrasmissioni.SalvaCessioneDirittiSuModelliTrasm(objTrasm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: CessioneDirittiSuModelliTrasm", e);
                return false;
            }
        }

        public bool ExistsTrasmissioniPendentiConWorkflowUtente(string idRuoloInUO, string idPeople)
        {
            return BusinessLogic.Amministrazione.AmministraManager.ExistsTrasmissioniPendentiConWorkflowUtente(idRuoloInUO, idPeople);
        }

        public DocsPaVO.documento.FileDocumento GetReportTrasmissioniPendentiUtente(string idPeople, string idCorrGlobaliRuolo, string formato)
        {
            DocsPaVO.documento.FileDocumento retVal = new DocsPaVO.documento.FileDocumento();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {

                    var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();
                    retVal = BusinessLogic.Amministrazione.AmministraManager.GetReportTrasmissioniPendentiUtente(idPeople, idCorrGlobaliRuolo, formato, spreadsheetService, configurationService, _reportGeneratorService);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: GetReportTrasmissioniPendentiUtente", ex);
                retVal = null;
            }

            return retVal;
        }

        public virtual DocsPaVO.amministrazione.EsitoOperazione AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmSostituzioneUtente(idPeopleNewUT, idCorrGlobRuolo);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmSostituzioneUtente - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public bool AmmSostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = false;
            try
            {
                result = BusinessLogic.LibroFirma.LibroFirmaManager.AmmSostituisciUtentePassiCorrelati(idRuolo, idOldPeople, idNewPeople);
            }
            catch (Exception e)
            {
                logger.Error("Errore in DocsPaWS.asmx  - metodo: InvalidaPassiCorrelatiByUtenteCoinvolto", e);
            }
            return result;
        }

        public DocsPaVO.amministrazione.EsitoOperazione AmmAccettaTrasmConWFRuolo(string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmAccettaTrasmConWFRuolo(idCorrGlobRuolo, _rubricaComuneService);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: AmmAccettaTrasmConWFRuolo - ", e);
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore";
            }

            return esito;
        }

        public bool StoricizzaPianoConservazione(string idTitolario, string idRegistro, InfoUtenteAmministratore infoUtente)
        {
            return BusinessLogic.PianoConservazione.PianoConservazioneManager.StoricizzaPianoConservazione(idTitolario, idRegistro, infoUtente);
        }

        public DocsPaVO.documento.FileDocumento ExportDettaglioPolicyPARER(string[] idPolicy, string formato, string tipo)
        {

            DocsPaVO.documento.FileDocumento result = new DocsPaVO.documento.FileDocumento();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();
                    result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.ExportPolicy(idPolicy, formato, tipo, spreadsheetService, configurationService, _reportGeneratorService);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: ExportDettaglioPolicyPARER", ex);
                result = null;
            }

            return result;
        }

        public DocsPaVO.Validations.ValidationResultInfo RemoveSupportedFileType(ref DocsPaVO.FormatiDocumento.SupportedFileType fileType)
        {
            try
            {
                return BusinessLogic.FormatiDocumento.SupportedFormatsManager.RemoveFileType(ref fileType);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: RemoveSupportedFileType", e);

                throw e;
            }
        }

        public DocsPaVO.Validations.ValidationResultInfo updateChiaveConfig(InfoUtenteAmministratore infoUtente, ChiaveConfigurazione chiave)
        {
            DocsPaVO.Validations.ValidationResultInfo retValue = DocsPaUtils.Configuration.ChiaviConfigManager.UpdateChiaveConfigurazione(chiave);
            if (infoUtente != null)
            {
                if (retValue.Value)
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMMODCHIAVI", infoUtente.idAmministrazione, string.Format("{0}{1}{2}{3}", "Modifica della chiave ", chiave.Codice, " - Nuovo valore : ", chiave.Valore), DocsPaVO.Logger.CodAzione.Esito.OK);
                else
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMMODCHIAVI", infoUtente.idAmministrazione, string.Format("{0}{1}{2}{3}", "Errore nella modifica della chiave ", chiave.Codice, " - Nuovo valore : ", chiave.Valore), DocsPaVO.Logger.CodAzione.Esito.KO);
            }
            return retValue;
        }
        public virtual DocsPaVO.fascicolazione.Fascicolo FascicolazioneGetFascicoloByIdNoSecurity(string idFascicolo)
        {
            DocsPaVO.fascicolazione.Fascicolo result = null;

            try
            {
                //gestione tracer
#if TRACE_WS
				DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
#endif
                result = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloByIdNoSecurity(idFascicolo);
#if TRACE_WS
				pt.WriteLogTracer("FascicolazioneGetFascicoloDaCodice");
#endif
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: FascicolazioneGetFascicoloByIdNoSecurity", e);
                result = null;
            }

            return result;
        }


        public DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER GetPolicyFascicoliPARERById(string idPolicy)
        {
            DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER result = new DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER();

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.GetPolicyFascicoliById(idPolicy);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: GetPolicyFascicoliPARERById - ", ex);
                result = null;
            }

            return result;
        }
        public bool UpdateStatoPolicyFascicoli(DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER[] listaPolicy, InfoUtenteAmministratore infoUtente)
        {
            bool result = false;

            try
            {
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.UpdateStatoPolicyFascicoli(listaPolicy.ToList(), infoUtente);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: UpdateStatoPolicyFascicoli - ", ex);
            }

            return result;
        }

        public bool DeletePolicyFascicoliPARER(string idPolicy, InfoUtenteAmministratore utente)
        {
            bool result = false;

            try
            {
                var policy = BusinessLogic.Conservazione.PARER.PolicyPARERManager.GetPolicyFascicoliById(idPolicy);
                result = BusinessLogic.Conservazione.PARER.PolicyPARERManager.DeletePolicyFascicoli(policy);

                if (result)
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERDEL", idPolicy, string.Format("Eliminazione Policy fascicoli {0} da parte dell'utente {1}", policy.codice, utente.userId), DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    BusinessLogic.UserLog.UserLog.WriteLog(utente, "AMMPOLICYPARERDEL", idPolicy, string.Format("Eliminazione Policy fascicoli {0} da parte dell'utente {1}", policy.codice, utente.userId), DocsPaVO.Logger.CodAzione.Esito.KO);
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: DeletePolicyPARER - ", ex);
            }

            return result;
        }


        public void updateDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg)
        {
            try
            {
                BusinessLogic.DiagrammiStato.DiagrammiStato.updateDiagramma(dg);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: updateDiagramma", e);
            }
        }

        public bool ammCheckCodiceUODuplicato(string id, string codice, string idAmm)
        {
            bool result = false;
            result = BusinessLogic.Amministrazione.OrganigrammaManager.ammCheckCodiceUODuplicato(id, codice, idAmm);
            return result;
        }

        public bool SaveRuoloRespConservazioneAOO(string idGruppo, string idUtente, string idAmm, string idAOO)
        {
            bool result = false;

            try
            {
                result = BusinessLogic.Utenti.RegistriRepertorioPrintManager.SaveRuoloRespConservazioneAoo(idGruppo, idUtente, idAmm, idAOO);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWS.asmx - metodo: SaveRuoloRespConservazioneAOO", ex);
            }

            return result;
        }

    }

}



