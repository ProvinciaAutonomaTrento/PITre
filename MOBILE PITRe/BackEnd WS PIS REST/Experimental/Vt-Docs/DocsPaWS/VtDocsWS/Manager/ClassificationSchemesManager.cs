using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.WebServices;
using System.Collections;
using log4net;

namespace VtDocsWS.Manager
{
    public class ClassificationSchemesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ClassificationSchemesManager));

        public static Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeResponse GetActiveClassificationScheme(Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeRequest request)
        {
            Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeResponse response = new Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetActiveClassificationScheme");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                Domain.ClassificationScheme classificationSchemeResponse = new Domain.ClassificationScheme();

                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
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
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }

                response.ClassificationScheme = classificationSchemeResponse;
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesResponse GetAllClassificationSchemes(Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesRequest request)
        {
            Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesResponse response = new Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetAllClassificationSchemes");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                Domain.ClassificationScheme[] classificationSchemesResponse = null;

                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                }
                catch
                {
                    //Titolari non trovati
                    throw new PisException("CLASSIFICATIONSCHEMES_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    int i = 0;
                    classificationSchemesResponse = new Domain.ClassificationScheme[titolari.Count];
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        Domain.ClassificationScheme tempOrg = new Domain.ClassificationScheme();
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
                    throw new PisException("CLASSIFICATIONSCHEMES_NOT_FOUND");
                }

                response.ClassificationSchemes = classificationSchemesResponse;

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdResponse GetClassificationSchemeById(Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdRequest request)
        {
            Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdResponse response = new Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetClassificationSchemeById");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request != null && string.IsNullOrEmpty(request.IdClassificationScheme))
                {
                    throw new PisException("ID_CLASSIFICATIONSCHEME_REQUIRED");
                }

                Domain.ClassificationScheme classificationSchemeResponse = new Domain.ClassificationScheme();

                DocsPaVO.amministrazione.OrgTitolario titolario = null;

                try
                {
                    titolario = BusinessLogic.Amministrazione.TitolarioManager.getTitolarioById(request.IdClassificationScheme);
                }
                catch
                {
                    //Titolari non trovati
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
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
                    throw new PisException("CLASSIFICATION_NOT_FOUND");
                }

                response.ClassificationScheme = classificationSchemeResponse;

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }
    }
}