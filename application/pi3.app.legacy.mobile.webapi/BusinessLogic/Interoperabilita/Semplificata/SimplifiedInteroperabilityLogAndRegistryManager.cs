// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.Logger;
using Serilog;

namespace BusinessLogic.Interoperabilita.Semplificata;

public class SimplifiedInteroperabilityLogAndRegistryManager
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(SimplifiedInteroperabilityLogAndRegistryManager));

    /// <summary>
    /// Metodo per l'inserimento di un item nel log
    /// </summary>
    /// <param name="profileId">Id del documento</param>
    /// <param name="isErrorMessage">Flag utilizzato per indicare se si desidera inserire un errore</param>
    /// <param name="text">Testo del messaggio da loggare</param>
    /// <returns>Esito dell'operazione di inserimento</returns>
    public static bool InsertItemInLog(String profileId, bool isErrorMessage, String text)
    {
        bool retVal = false;
        using (SimplifiedInteroperabilityRegistryAndLogDbManager dbManager = new SimplifiedInteroperabilityRegistryAndLogDbManager())
        {
            retVal = dbManager.InsertItemInLog(new InteroperabilityLogItem()
            {
                ProfileId = String.IsNullOrEmpty(profileId) ? 0 : Int32.Parse(profileId),
                IsErrorMessage = isErrorMessage,
                LogMessage = text
            });

        }
        if (isErrorMessage)
        {
            logger.Error("Messaggio {0}relativo al documento con id {0} con testo '{1}' {2}",
                isErrorMessage ? "di eccezione " : "",
                profileId,
                text,
                retVal ? "inserite correttamento" : "non inserito");
        }
        else
        {
            logger.Debug("Messaggio {0}relativo al documento con id {0} con testo '{1}' {2}",
                isErrorMessage ? "di eccezione " : "",
                profileId,
                text,
                retVal ? "inserite correttamento" : "non inserito");
        }

        return retVal;
    }

}
