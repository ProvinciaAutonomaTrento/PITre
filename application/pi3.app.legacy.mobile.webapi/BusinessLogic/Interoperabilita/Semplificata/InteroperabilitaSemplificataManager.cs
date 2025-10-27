// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Interoperabilita.Semplificata.Exceptions;
using DocsPaVO.Interoperabilita.Semplificata;
using Serilog;

namespace BusinessLogic.Interoperabilita.Semplificata;

public class InteroperabilitaSemplificataManager
{
    private static ILogger logger = Log.ForContext(typeof(InteroperabilitaSemplificataManager));

    public const String InteroperabilityCode = "SIMPLIFIEDINTEROPERABILITY";

    /// <summary>
    /// Metodo per verificare se un documento è stato ricevuto per IS
    /// </summary>
    /// <param name="documentId">Id del documento da verificare</param>
    /// <returns>Esito della verifica</returns>
    public static bool IsDocumentReceivedWithIS(String documentId)
    {
        bool retVal = false;
        using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interopDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
        {
            retVal = interopDb.IsDocumentReceivedWithIS(documentId);
        }

        return retVal;
    }

    /// <summary>
    /// Metodo utilizzato per verificare se, per l'amministrazione corrente è attiva la funzionalità di
    /// interoperabilità semplificata
    /// </summary>
    /// <param name="adminId">Id dell'amministrazione</param>
    /// <returns>Flag che indica lo stato di attivazione dell'interoperabilità</returns>
    public static bool IsEnabledSimplifiedInteroperability(String adminId)
    {
        string enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(adminId, "INTEROP_SERVICE_ACTIVE");
        if (String.IsNullOrEmpty(enabled))
            enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_ACTIVE");

        return enabled == "1";

    }

    public static InteroperabilitySettings LoadSettings(String registryId)
    {
        logger.Debug("Caricamento impostazioni di interoperabilità semplificata per il registro con id {0}",
            registryId);

        InteroperabilitySettings loadedData = null;
        using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interoperabilitaSemplificataDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
        {
            loadedData = interoperabilitaSemplificataDb.LoadSettings(registryId);
        }

        logger.Debug("Caricamento impostazioni di interoperabilità semplificata per il registro con id {0}, {1}",
            registryId,
            loadedData != null ? "effettuato con successo" : "non riuscito.");

        return loadedData;

    }

    public static bool SaveSettings(InteroperabilitySettings interoperabilitySettings)
    {
        logger.Debug("Salvataggio impostazioni di interoperabilità semplificata per il registro con id {0}",
            interoperabilitySettings.RegistryId);

        // Validazione dei dati prima di salvare
        ValidateData(interoperabilitySettings);

        bool saved = false;
        using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interoperabilitaSemplificataDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
        {
            saved = interoperabilitaSemplificataDb.SaveSettings(interoperabilitySettings);
        }

        logger.Debug("Salvataggio impostazioni di interoperabilità semplificata per il registro con id {0}, {1}",
            interoperabilitySettings.RegistryId,
            saved ? "effettuato con successo" : "non riuscito.");

        return saved;

    }

    private static void ValidateData(InteroperabilitySettings interoperabilitySettings)
    {
        SimplifiedInteroperabilitySaveSettingsException exc = new SimplifiedInteroperabilitySaveSettingsException();

        // Se è attiva l'interoperabilità ma non c'è nè un ruolo nè un utente impostato per la creazione del
        // predisposto
        if (interoperabilitySettings.IsEnabledInteroperability && interoperabilitySettings.RoleId == 0 &&
            interoperabilitySettings.UserId == 0)
            exc.AddInvalidField("Specificare ruolo e utente da utilizzare per la creazione del documento predisposto");

        // Se c'è almeno un avviso, viene lanciata l'eccezione
        if (exc.GetInvalidFields().Count > 0)
            throw exc;
    }

    /// <summary>
    /// Metodo per la verifica di abilitazione di un oggetto all'IS
    /// </summary>
    /// <param name="objectId">Id dell'oggetto</param>
    /// <param name="rf">Booleano che indica se si desidera verificare un RF</param>
    /// <returns>Esito della verifica</returns>
    public static bool IsElementInteroperable(String objectId, bool rf)
    {
        logger.Debug("Caricamento stato di abilitazione dell'{0} con id {1} all'interoperabilità semplificata",
            rf ? "RF" : "UO",
            objectId);

        bool retVal = false;
        using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interoperabilitaSemplificataDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
        {
            retVal = interoperabilitaSemplificataDb.IsElementInteroperable(objectId, rf);
        }

        logger.Debug("Caricamento stato di abilitazione dell'{0} con id {1} all'interoperabilità semplificata, {2}",
            rf ? "RF" : "UO",
            objectId,
            retVal ? "effettuato con successo" : "non riuscito.");

        return retVal;

    }

}
