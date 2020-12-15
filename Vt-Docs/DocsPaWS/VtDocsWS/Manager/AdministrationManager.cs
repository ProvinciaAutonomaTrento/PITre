using log4net;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VtDocsWS.Manager
{
    public class AdministrationManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AdministrationManager));

        public static Services.Administration.GetAdministrations.GetAdministrationsResponse GetAdministrations(Services.Administration.GetAdministrations.GetAdministrationsRequest request)
        {
            Services.Administration.GetAdministrations.GetAdministrationsResponse response = new Services.Administration.GetAdministrations.GetAdministrationsResponse();

            try
            {
                if (request != null && (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password)))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                ArrayList administrations = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioniByUser(request.UserName, true);

                if (administrations == null)
                {
                    //Utente non trovato
                    throw new PisException("ADMINISTRATIONS_NO_EXIST");
                }
                else
                {
                    List<Domain.Administration> adm = new List<Domain.Administration>();
                    foreach (DocsPaVO.utente.Amministrazione a in administrations)
                    {
                        adm.Add(new Domain.Administration()
                        {
                            Id = a.systemId,
                            Code = a.codice,
                            Description = a.descrizione,
                            Email = a.email,
                            Library = a.libreria
                        });
                    }
                    response.Administrations = adm.ToArray();
                }

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
