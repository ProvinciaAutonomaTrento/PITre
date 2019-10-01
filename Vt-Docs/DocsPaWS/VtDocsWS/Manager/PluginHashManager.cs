using DocsPaVO.Plugin;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.WebServices;

namespace VtDocsWS.Manager
{
    public class PluginHashManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(PluginHashManager));

        public static Services.PluginHash.GetHashMail.GetHashMailResponse GetHashMail(Services.PluginHash.GetHashMail.GetHashMailRequest request)
        {
            Services.PluginHash.GetHashMail.GetHashMailResponse response = new Services.PluginHash.GetHashMail.GetHashMailResponse();

            DocsPaVO.utente.InfoUtente infoUtente = null;

            //Inizio controllo autenticazione utente
            infoUtente = Utils.CheckAuthentication(request, "GetHashMail");

            if (string.IsNullOrEmpty(request.HashFile))
            {
                throw new PisException("MISSING_PARAMETER");
            }

            try
            {
                DpaPluginHash dpaPluginHash = BusinessLogic.Plugin.PluginHashBL.GetHashMail(request.HashFile);
                response = new Services.PluginHash.GetHashMail.GetHashMailResponse()
                {
                    DpaPluginHash = dpaPluginHash != null ? new Domain.DpaPluginHash()
                    {
                        DataElaborazione = dpaPluginHash.dataElaborazione,
                        HashFile = dpaPluginHash.hashFile,
                        Utente = BusinessLogic.Utenti.UserManager.getUtenteById(dpaPluginHash.idPeople),
                        IdProfile = dpaPluginHash.idProfile,
                        SystemId = dpaPluginHash.systemId,
                        AccessRight = BusinessLogic.Documenti.DocManager.getAccessRightDocBySystemID(dpaPluginHash.idProfile, infoUtente)
                    } : null
                };
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

        public static Services.PluginHash.NewHashMail.NewHashMailResponse NewHashMail(Services.PluginHash.NewHashMail.NewHashMailRequest request)
        {
            Services.PluginHash.NewHashMail.NewHashMailResponse response = new Services.PluginHash.NewHashMail.NewHashMailResponse();

            DocsPaVO.utente.InfoUtente infoUtente = null;

            //Inizio controllo autenticazione utente
            infoUtente = Utils.CheckAuthentication(request, "GetHashMail");

            if (infoUtente == null)
            {
                //Utente non trovato
                throw new PisException("USER_NO_EXIST");
            }

            if (string.IsNullOrEmpty(request.HashFile) || string.IsNullOrEmpty(request.IdProfile))
            {
                throw new PisException("MISSING_PARAMETER");
            }

            try
            {
                response.Result = BusinessLogic.Plugin.PluginHashBL.NewHashMail(request.IdProfile, infoUtente.idPeople, request.HashFile);
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
                response.Result = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Result = false;
                response.Success = false;
            }

            return response;
        }
    }
}