using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.WebServices;

namespace VtDocsWS.Manager
{
    public class AdLDocumentManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AdLDocumentManager));

        public static Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloResponse AddDocumentInAdLRuolo(Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloRequest request)
        {
            Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloResponse response = new Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloResponse();
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "AddDocumentInAdLRuolo");

                if (string.IsNullOrEmpty(request.IdProfile) && string.IsNullOrEmpty(request.IdRegistro) && string.IsNullOrEmpty(request.TipoProto))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                try
                {
                    BusinessLogic.Documenti.areaLavoroManager.execAddLavoroRoleMethod(request.IdProfile, request.TipoProto, request.IdRegistro, infoUtente, null);
                }
                catch (Exception ex)
                {
                    throw new PisException("ADL_ERROR");
                }

                response.Success = true;
                response.Result = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
                response.Result = false;
            }

            return response;
        }

        public static Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteResponse AddDocumentInAdlUtente(Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteRequest request)
        {
            Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteResponse response = new Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteResponse();
            try
            {
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "AddDocumentInAdLRuolo");

                if (string.IsNullOrEmpty(request.IdProfile) && string.IsNullOrEmpty(request.IdRegistro) && string.IsNullOrEmpty(request.TipoProto))
                {
                    throw new PisException("MISSING_PARAMETER");
                }

                try
                {
                    BusinessLogic.Documenti.areaLavoroManager.execAddLavoroMethod(request.IdProfile, request.TipoProto, request.IdRegistro, infoUtente, null);
                }
                catch (Exception ex)
                {
                    throw new PisException("ADL_ERROR");
                }

                response.Success = true;
                response.Result = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
                response.Result = false;
            }
            return response;
        }
    }
}