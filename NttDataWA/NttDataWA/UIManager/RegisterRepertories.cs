using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class RegisterRepertories
    {

        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryType"></param>
        /// <param name="aooRfId"></param>
        /// <returns></returns>
        public static RepertorioState GetRepertorioState(String counterId, String registryType, String aooRfId)
        {
            String registryId = String.Empty, rfId = String.Empty;
            switch (registryType)
            {
                case "A":
                    registryId = aooRfId;
                    break;
                case "R":
                    rfId = aooRfId;
                    break;
            }

            return docsPaWS.GetRepertorioState(
                new GetRepertorioStateRequest()
                {
                    CounterId = counterId,
                    RegistryId = registryId,
                    RfId = rfId
                }).State;
        }

        /// <summary>
        /// Metodo per il cambio di stato di un repertorio
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool ChangeRepertorioState(String counterId, String registryId, String rfId, String idAmm)
        {

            return docsPaWS.ChangeRepertorioState(
                new ChangeRepertorioStateRequest()
                {
                    CounterId = counterId,
                    IdAmm = idAmm,
                    RegistryId = registryId,
                    RfId = rfId
                }).ChangeStateResult;

        }

        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Responsabile e Stampatore o solo Responsabile o Stampatore
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public static RegistroRepertorio[] GetRegistriesWithAooOrRf(string idRoleResp, string idRolePrinter)
        {
            try
            {
                return docsPaWS.GetRegistriesWithAooOrRf(idRoleResp, idRolePrinter);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo per la generazione di una stampa di repertorio
        /// </summary>
        /// <param name="role"></param>
        /// <param name="userInfo"></param>
        /// <param name="rfId"></param>
        /// <param name="registryId"></param>
        /// <param name="counterId"></param>
        /// <returns></returns>
        public static SchedaDocumento GeneratePrintRepertorio(Ruolo role, InfoUtente userInfo, String rfId, String registryId, String counterId)
        {
            SchedaDocumento retVal = null;

            try
            {
                retVal = docsPaWS.GeneratePrintRepertorio(
                    new GeneratePrintRepertorioRequest()
                    {
                        CounterId = counterId,
                        RegistryId = registryId,
                        RfId = rfId,
                        Role = role,
                        UserInfo = userInfo
                    }).Document;
            }
            catch (Exception e)
            {
                SoapExceptionParser.ThrowOriginalException(e);

            }
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public static RepertorioPrintRange[] GetRepertoriPrintRanges(String counterId, String registryId, String rfId)
        {
            return docsPaWS.GetRepertoriPrintRanges(
                new GetRepertoriPrintRangesRequest()
                {
                    CounterId = counterId,
                    RegistryId = registryId,
                    RfId = rfId
                }, false).Ranges;
        }

        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Responsabile e Stampatore o solo Responsabile o Stampatore (compresi i ruoli superiori)
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public static RegistroRepertorio[] GetRegistriesWithAooOrRfSup(string idRoleResp, string idRolePrinter)
        {
            return docsPaWS.GetRegistriesWithAooOrRfSup(idRoleResp, idRolePrinter);
        }

    }
}