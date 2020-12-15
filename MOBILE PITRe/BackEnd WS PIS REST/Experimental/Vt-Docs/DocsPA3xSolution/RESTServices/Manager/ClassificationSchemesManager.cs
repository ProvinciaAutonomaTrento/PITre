using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Threading.Tasks;
using RESTServices.Model.Domain;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;
using System.Collections;

namespace RESTServices.Manager
{
    public class ClassificationSchemesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ClassificationSchemesManager));

        public static async Task<GetClassificationSchemeResponse> getActiveClassificationScheme(string token)
        {
            logger.Info("begin getActiveClassificationScheme");
            GetClassificationSchemeResponse response = new GetClassificationSchemeResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                ClassificationScheme classificationSchemeResponse = new ClassificationScheme();
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            classificationSchemeResponse.Active = true;
                            classificationSchemeResponse.Description = tempTit.Descrizione;
                            classificationSchemeResponse.Id = tempTit.ID;
                            break;
                        }
                    }
                }
                else
                {
                    //Titolario attivo non trovato
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                response.ClassificationScheme = classificationSchemeResponse;
                response.Code = GetClassificationSchemeResponseCode.OK;

                logger.Info("end getActiveClassificationScheme");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getActiveClassificationScheme: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetClassificationSchemeResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getActiveClassificationScheme: " + e);
                response = new GetClassificationSchemeResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetClassificationSchemeResponse> getClassificationSchemeById(string token, string idClassScheme)
        {
            logger.Info("begin getClassificationSchemeById");
            GetClassificationSchemeResponse response = new GetClassificationSchemeResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                if (string.IsNullOrEmpty(idClassScheme))
                {
                    throw new RestException("ID_CLASSIFICATIONSCHEME_REQUIRED");
                }

                ClassificationScheme classificationSchemeResponse = new ClassificationScheme();

                DocsPaVO.amministrazione.OrgTitolario titolario = null;

                try
                {
                    titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(idClassScheme);
                }
                catch
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                if (titolario != null)
                {

                    if (titolario.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                    {
                        classificationSchemeResponse.Active = true;
                    }
                    else
                    {
                        classificationSchemeResponse.Active = false;
                    }
                    classificationSchemeResponse.Description = titolario.Descrizione;
                    classificationSchemeResponse.Id = titolario.ID;
                }
                else
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                response.ClassificationScheme = classificationSchemeResponse;

                response.Code = GetClassificationSchemeResponseCode.OK;

                logger.Info("end getClassificationSchemeById");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getClassificationSchemeById: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetClassificationSchemeResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getClassificationSchemeById: " + e);
                response = new GetClassificationSchemeResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }

        public static async Task<GetAllClassificationSchemesResponse> getAllClassificationSchemes(string token)
        {
            logger.Info("begin getAllClassificationSchemes");
            GetAllClassificationSchemesResponse response = new GetAllClassificationSchemesResponse();
            try
            {
                #region controllo Token
                if (string.IsNullOrWhiteSpace(token))
                {
                    logger.Error("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = Utils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = BusinessLogic.Utenti.UserManager.getUtente(authInfoArray[5], authInfoArray[4]);
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(authInfoArray[0]);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];               

                #endregion
                ClassificationScheme[] classificationSchemesResponse = null;

                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATIONSCHEMES_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    int i = 0;
                    classificationSchemesResponse = new ClassificationScheme[titolari.Count];
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        ClassificationScheme tempOrg = new ClassificationScheme();
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            tempOrg.Active = true;
                        }
                        else
                        {
                            tempOrg.Active = false;
                        }
                        tempOrg.Description = tempTit.Descrizione;
                        tempOrg.Id = tempTit.ID;
                        classificationSchemesResponse[i] = tempOrg;
                        i++;
                    }
                }
                else
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATIONSCHEMES_NOT_FOUND");
                }

                response.ClassificationSchemes = classificationSchemesResponse;

                response.Code = GetAllClassificationSchemesResponseCode.OK;

                logger.Info("end getAllClassificationSchemes");
            }
            catch (RestException pisEx)
            {
                logger.ErrorFormat("Eccezione getAllClassificationSchemes: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response = new GetAllClassificationSchemesResponse();
                response.Code = GetAllClassificationSchemesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                logger.Error("eccezione getAllClassificationSchemes: " + e);
                response = new GetAllClassificationSchemesResponse();
                response.Code = GetAllClassificationSchemesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }
            return response;
        }
    }
}