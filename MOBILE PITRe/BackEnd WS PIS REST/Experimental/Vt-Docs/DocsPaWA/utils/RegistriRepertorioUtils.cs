using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;
using System.Collections;

namespace DocsPAWA.utils
{
    /// <summary>
    /// Questa classe fornisce dei metodi di supporto per la gestione dei registri di repertorio
    /// </summary>
    public class RegistriRepertorioUtils
    {
        private static DocsPaWebService _docsPaWS = new DocsPaWebService();

        static RegistriRepertorioUtils()
        {
            _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
        }

        /// <summary>
        /// Metodo per il recupero dell'anagrafica dei registri di repertorio registrati per una data amministrazione
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione di cui recuperare l'anagrafica</param>
        /// <returns>Collection con le informazioni sui registri di repertorio registrati per una data amministrazione</returns>
        public static RegistroRepertorio[] GetRegisteredRegistries(String administrationId)
        {
            return _docsPaWS.GetRegisteredRegistries(new RegisteredRegistriRepertorioRequest() { AdministrationId = administrationId }).RegistriRepertorio;
            
        }

        public static RegistroRepertorioSingleSettings GetRegisterSettings(String counterId, String registryId, String rfId, TipologyKind tipologyKind, SettingsType settingsType)
        {
            return _docsPaWS.GetRegisterSettings(new RegistroRepertorioSettingsRequest() { CounterId = counterId, RegistryId = registryId, RfId = rfId, TipologyKind = tipologyKind, SettingsType = settingsType }).RegistroRepertorioSingleSettings;
        }

        public static RegistroRepertorioSettingsMinimalInfo[] GetSettingsMinimalInfo(String counterId, TipologyKind tipologyKind, string idAmm)
        {
            return _docsPaWS.GetSettingsMinimalInfo(new GetSettingsMinimalInfoRequest() { CounterId = counterId, TipologyKind = tipologyKind, idAmm=idAmm }).Settings;
        }

        /// <summary>
        /// Metodo per l'aggiornamento delle impostazioni relative ad un registro di repòertorio
        /// </summary>
        /// <param name="settings">Impostazioni da salvare</param>
        /// <param name="counterId">Id del contatore</param>
        /// <param name="validationResult">Eventuali errori di validazione, consultarli se il metodo restituisce false</param>
        public static bool SaveRegisterSettings(RegistroRepertorioSingleSettings settings, String counterId, TipologyKind tipologyKind, SettingsType settingsType, out ValidationResultInfo validationResult, String idAmm)
        {
            SaveRegistroRepertorioSettingsResponse response = _docsPaWS.SaveRegisterSettings(new SaveRegistroRepertorioSettingsRequest() { CounterId = counterId, Settings = settingsType, SettingsToSave = settings, TipologyKind = tipologyKind, IdAdmin = idAmm });
            validationResult = response.ValidationResult;

            return response.SaveChangesResult;
        }

        /// <summary>
        /// Registro di repertorio di cui si stanno modificando le impostazioni
        /// </summary>
        public static RegistroRepertorioSingleSettings CounterSettings
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["CounterSettings"] as RegistroRepertorioSingleSettings;
            }

            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("RegistroRepertorioSettings");
                CallContextStack.CurrentContext.ContextState["CounterSettings"] = value;
            }

        }

        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Responsabile e Stampatore o solo Responsabile o Stampatore
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public static RegistroRepertorio[] GetRegistriesWithAooOrRf(string idRoleResp, string idRolePrinter)
        {
            return _docsPaWS.GetRegistriesWithAooOrRf(idRoleResp, idRolePrinter);
        }

        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Responsabile e Stampatore o solo Responsabile o Stampatore (compresi i ruoli superiori)
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public static RegistroRepertorio[] GetRegistriesWithAooOrRfSup(string idRoleResp, string idRolePrinter)
        {
            return _docsPaWS.GetRegistriesWithAooOrRfSup(idRoleResp, idRolePrinter);
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
                retVal = _docsPaWS.GeneratePrintRepertorio(
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
                DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(e);

            }
            return retVal;
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
            return _docsPaWS.ChangeRepertorioState(
                new ChangeRepertorioStateRequest()
                {
                    CounterId = counterId,
                    IdAmm = idAmm,
                    RegistryId = registryId,
                    RfId = rfId
                }).ChangeStateResult;
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
            return _docsPaWS.GetRepertoriPrintRanges(
                new GetRepertoriPrintRangesRequest()
                    {
                        CounterId = counterId,
                        RegistryId = registryId,
                        RfId = rfId
                    },false).Ranges;
        }

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

            return _docsPaWS.GetRepertorioState(
                new GetRepertorioStateRequest()
                    {
                        CounterId = counterId,
                        RegistryId = registryId,
                        RfId = rfId
                    }).State;
        }

    }
}
