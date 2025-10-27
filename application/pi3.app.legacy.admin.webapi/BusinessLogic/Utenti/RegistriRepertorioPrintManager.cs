// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.utente.Repertori;
using DocsPaVO.Validations;

namespace BusinessLogic.Utenti;

public class RegistriRepertorioPrintManager
{
    public static List<RegistroRepertorio> GetRegisteredRegistries(String administrationId)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.GetRegisteredRegisters(administrationId);
    }

    public static RegistroRepertorioSingleSettings GetRegisterSettings(String counterId, String registryId, String rfId, RegistroRepertorio.TipologyKind tipologyKind, RegistroRepertorio.SettingsType settingsType)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.GetRegisterSettings(counterId, registryId, rfId, tipologyKind, settingsType);
    }

    public static bool SaveRuoloRespConservazione(string idGroup, string idUser, string idAmm)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.SaveRuoloRespConservazione(idGroup, idUser, idAmm);
    }

    public static bool SaveRuoloRespConservazioneAoo(string idGroup, string idUser, string idAmm, string idAoo)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.SaveRuoloRespConservazioneAoo(idGroup, idUser, idAmm, idAoo);
    }

    /// <summary>
    /// Metodo per il recupero delle impostazioni minimali relative ad un repertorio
    /// </summary>
    /// <param name="counterId">Id del repertorio</param>
    /// <returns>Lista delle impostazioni minumali</returns>
    public static List<RegistroRepertorioSettingsMinimalInfo> GetSettingsMinimalInfo(String counterId, RegistroRepertorio.TipologyKind tipologyKind, string idAmm)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.GetSettingsMinimalInfo(counterId, tipologyKind, idAmm);

    }

    /// <summary>
    /// Metodo per l'aggiornamento delle impostazioni relative ad una particolare istanza di contatore
    /// </summary>
    /// <param name="counterId">Id del contatore da salvare</param>
    /// <param name="tipologyKind"></param>
    /// <param name="settingsType">Tipologia di impostazioni richiesta (Singola o Globale)</param>
    /// <param name="settings">Impostazioni da salvare</param>
    /// <param name="idAmm">Id dell'amministrazione</param>
    /// <param name="validationResult">Risultato della validazione delle impostazioni</param>
    /// <returns>Esito dell'operazione di aggiornamento</returns>
    public static bool SaveRegisterSettings(String counterId, RegistroRepertorio.SettingsType settingsType, RegistroRepertorio.TipologyKind tipologyKind, RegistroRepertorioSingleSettings settings, String idAmm, out ValidationResultInfo validationResult)
    {
        bool retVal = true;
        ValidationResultInfo validation = new ValidationResultInfo();

        // Prima di proseguire, validazione dei dati se è stata impostata una frequenza di stampa diversa da N
        if (settings.PrintFrequency != RegistroRepertorioSingleSettings.Frequency.N)
        {
            validation = ValidateSettingsData(settings);
            retVal = validation.Value;
        }

        // Se la validazione è passata, salvataggio dei dati
        if (validation.Value)
        {
            // Calcolo della data prevista per la prossima stampa
            settings.DateNextAutomaticPrint = GetNextAutomaticPrintDate(DateTime.Now.GetMaxDate(settings.DateAutomaticPrintStart), settings.PrintFrequency);

            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            // Recupero dello stato del repertorio prima di modificare
            //String lastPrintedNumber, lastNumberToPrint;
            RegistroRepertorioSingleSettings.RepertorioState state = manager.GetState(counterId, settings.RegistryId, settings.RFId);//, out lastPrintedNumber, out lastNumberToPrint);

            // Se è cambiato lo stato, viene cambiato lo stato del repertorio
            if (state != settings.CounterState)
                ChangeRepertorioState(counterId, settings.RegistryId, settings.RFId, idAmm);

            retVal = manager.SaveRegisterSettings(counterId, settingsType, tipologyKind, settings);
        }

        validationResult = validation;
        return retVal;
    }


    /// <summary>
    /// Metodo per la validazione dei dati di un contatore di repertorio
    /// </summary>
    /// <param name="settings">Impostazioni da validare</param>
    /// <returns>Risultato della validazione</returns>
    public static ValidationResultInfo ValidateSettingsData(RegistroRepertorioSingleSettings settings)
    {
        ValidationResultInfo validation = new ValidationResultInfo();

        // La data di fine validità deve essere maggiore della data di oggi
        if (settings.DateAutomaticPrintFinish < DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy")))
        {
            validation.Value = false;
            validation.BrokenRules.Add(new BrokenRule("1", "La data di fine validità deve essere la data di oggi o una data successiva", BrokenRule.BrokenRuleLevelEnum.Error));
        }

        // La data di fine validità deve essere maggiore della data di inizio validità
        if (settings.DateAutomaticPrintFinish < settings.DateAutomaticPrintStart)
        {
            validation.Value = false;
            validation.BrokenRules.Add(new BrokenRule("2", "La data di fine validità deve essere una data successiva a quella di inizio validità", BrokenRule.BrokenRuleLevelEnum.Error));
        }

        // L'intervallo di date deve essere compatibile con la frequenza di stampa (nell'intervallo deve capitare almeno una stampa)
        DateTime nextPrint = GetNextAutomaticPrintDate(new DateTime().GetMaxDate(settings.DateAutomaticPrintStart), settings.PrintFrequency);
        if (nextPrint > settings.DateAutomaticPrintFinish)
        {
            validation.Value = false;
            validation.BrokenRules.Add(new BrokenRule("3", String.Format("Il periodo temporale specificato non è compatibile con la frequenza di stampa. Specificare una data di fine validità pari o successiva a {0} oppure modificare, se possibile, la frequenza di stampa", nextPrint.ToString("dddd, dd MMMM yyyy")), BrokenRule.BrokenRuleLevelEnum.Error));
        }

        // Se è stata selezionata una frequenza di stampa diversa da N devono essere stato selezionato anche un responsabile di stampa 
        // ed un utente di stampa
        if (settings.PrintFrequency != RegistroRepertorioSingleSettings.Frequency.N && (String.IsNullOrEmpty(settings.PrinterRoleRespId) || String.IsNullOrEmpty(settings.PrinterUserRespId)))
        {
            validation.Value = false;
            validation.BrokenRules.Add(new BrokenRule("4", "Per abilitare la stampa automatica è necessario selezionare un ruolo responsabile della stampa del registro di repertorio ed un utente che verrà utilizzato come utente creatore del documento", BrokenRule.BrokenRuleLevelEnum.Error));
        }

        // Restituzione del risultato della validazione
        return validation;

    }

    /// <summary>
    /// Metodo per il calcolo della prossima data di stampa
    /// </summary>
    /// <param name="date">Data a partire dalla quale bisogna calcolare la prossima stampa automatica</param>
    /// <param name="frequency">Frequenza di stampa</param>
    /// <returns>Data prevista per la prossima stampa</returns>
    private static DateTime GetNextAutomaticPrintDate(DateTime date, RegistroRepertorioSingleSettings.Frequency frequency)
    {
        DateTime nextPrint = new DateTime();
        switch (frequency)
        {

            case RegistroRepertorioSingleSettings.Frequency.D:
                nextPrint = date.AddDays(1);
                break;
            case RegistroRepertorioSingleSettings.Frequency.W:
                nextPrint = date.AddDays(7);
                break;
            case RegistroRepertorioSingleSettings.Frequency.FD:
                nextPrint = date.AddDays(15);
                break;
            case RegistroRepertorioSingleSettings.Frequency.M:
                nextPrint = date.AddMonths(1);
                break;
        }

        return nextPrint;
    }

    /// <summary>
    /// Metodo per il cambio dello stato di un repertorio
    /// </summary>
    /// <param name="counterId"></param>
    /// <param name="registryId"></param>
    /// <param name="rfId"></param>
    /// <param name="idAmm"></param>
    /// <returns></returns>
    public static bool ChangeRepertorioState(String counterId, String registryId, String rfId, String idAmm)
    {
        bool retVal = true;
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

        //String lastPrintedNumber, lastNumberToPrint;
        // Recupero dello stato attuale del contatore
        RegistroRepertorioSingleSettings.RepertorioState state = manager.GetState(counterId, registryId, rfId);//, out lastPrintedNumber, out lastNumberToPrint);

        // Recupero dell'id della tipologia associata al contatore
        String tipology = manager.GetCounterTypology(counterId);

        // Cambio dello stato
        if (state == RegistroRepertorioSingleSettings.RepertorioState.O)
            state = RegistroRepertorioSingleSettings.RepertorioState.C;
        else
            state = RegistroRepertorioSingleSettings.RepertorioState.O;

        // Salvataggio del nuovo stato e sospensione / attivazione della tipologia
        Templates template = new Templates() { SYSTEM_ID = Convert.ToInt32(tipology) };
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            // Salvataggio nuovo stato del registro
            retVal = manager.SaveCounterState(counterId, rfId, registryId, state);

            // Abilitazione / disabilitazione tipologia
            if (state == RegistroRepertorioSingleSettings.RepertorioState.O)
                template.IN_ESERCIZIO = "NO";
            else
                template.IN_ESERCIZIO = "SI";

            ProfilazioneDocumenti.messaInEsercizioTemplate(template, idAmm);

            transactionContext.Complete();

        }

        return retVal;

    }

}

public static class DateExtension
{
    /// <summary>
    /// Metodo per il calcolo della data maggiore fra due date
    /// </summary>
    /// <param name="date">Data a cui applicare l'extension method</param>
    /// <param name="dateToCompare">Data da comparare</param>
    /// <returns>La data maggiore</returns>
    public static DateTime GetMaxDate(this DateTime date, DateTime dateToCompare)
    {
        return date > dateToCompare ? date : dateToCompare;
    }
}