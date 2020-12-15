using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using BusinessLogic.utenti;
using DocsPaVO.utente.Repertori;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using BusinessLogic.Stampe;
using System.Collections;
using DocsPaVO.Modelli;
using DocsPaVO.ProfilazioneDinamica;
using BusinessLogic.ProfilazioneDinamica;
using System.Threading;

namespace BusinessLogic.utenti
{
    /// <summary>
    /// Questa classe fornisce metodi per il coordinamento delle azioni riguardanti le stampe dei registri di repertorio
    /// </summary>
    public class RegistriRepertorioPrintManager
    {
        /// <summary>
        /// Metodo per il recupero dell'anagrafica dei registri di repertorio registrati per una data amministrazione
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione di cui recuperare l'anagrafica dei repertori</param>
        /// <returns>Lista dei registri di repertori registrati per la specifica amministrazione</returns>
        public static List<RegistroRepertorio> GetRegisteredRegistries(String administrationId)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.GetRegisteredRegisters(administrationId);

        }

        /// <summary>
        /// Metodo per il recupero delle impostazioni relative ad un contatore di repertorio
        /// </summary>
        /// <param name="counterId">Id del contatore di cui recuperare le impostazioni</param>
        /// <returns>Registro di repertorio con le impostazioni</returns>
        public static RegistroRepertorioSingleSettings GetRegisterSettings(String counterId, String registryId, String rfId, RegistroRepertorio.TipologyKind tipologyKind, RegistroRepertorio.SettingsType settingsType)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.GetRegisterSettings(counterId, registryId, rfId, tipologyKind, settingsType);
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
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static void GeneratePrintRepertorio(String counterId, String registryId, String rfId, InfoUtente userInfo, Ruolo role)
        {
            System.Threading.Mutex mutex = null;
            StampaRepertorio printer = new StampaRepertorio();
            try
            {
                // Creazione o reperimento del mutex
                mutex = CreateMutex(counterId);
                mutex.WaitOne();
                printer.GeneratePrintRepertorio(counterId, registryId, rfId, userInfo, role);
            }
            finally
            {
                //Rilascio il mutex
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex = null;
                }
            }

        }

        private static Mutex CreateMutex(string counterId)
        {
            bool createdNew;
            string mutexName = string.Format("DPAStampaRepertori-{0}", counterId);
            Mutex m = new Mutex(false, mutexName, out createdNew);
            
            if (!createdNew)
            {
                m.WaitOne();
                // the mutex already exists, we hold a reference to the mutex, but we
                // don't own it (we need to call WaitOne).
            }
            return m;
        }

        public static void GeneratePrintByYear(string counterId, string year)
        {
            StampaRepertorio printer = new StampaRepertorio();
            printer.GeneratePrintByYear(counterId, year);

        }

        public static string GeneratePrintByDocumentId(string idDoc)
        {
            StampaRepertorio printer = new StampaRepertorio();
            string newIdDoc = printer.GeneratePrintByDocumentId(idDoc);
            return newIdDoc;
        }

        public static string GeneratePrintByRanges(string anno, string counterId, string numRepStart, string numRepEnd, string idRegistro, string dataStampa, bool ultimastampa)
        {
            StampaRepertorio printer = new StampaRepertorio();
            string newIdDoc = printer.GeneratePrintByRanges(anno,counterId,numRepStart,numRepEnd,idRegistro,dataStampa,ultimastampa);
            return newIdDoc;
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
            if (settings.DateAutomaticPrintFinish < DateTime.Now)
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
        /// Metodo che restituisce i registri di repertorio di cui si è Repsonsabile e Stampatore o solo Responsabile o Stampatore
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public static ArrayList GetRegistriesWithAooOrRf(string idRoleResp, string idRolePrinter)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.GetRegistriesWithAooOrRf(idRoleResp, idRolePrinter);
        }

        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Repsonsabile e Stampatore o solo Responsabile o Stampatore (compresi i ruoli superiori)
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public static ArrayList GetRegistriesWithAooOrRfSup(string idRoleResp, string idRolePrinter)
        {
            
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            // INTEGRAZIONE PITRE-PARER
            // MEV Policy e responsabile conservazione
            // Req. F03_01 - Responsabile conservazione
            // Il responsabile della conservazione deve avere visibilità su tutti i registri di repertorio
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();

            string idAmm = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idRoleResp).idAmministrazione;
            string idRoleRespCons = c.GetIdRoleResponsabileConservazione(idAmm);

            if (idRoleResp.Equals(idRoleRespCons) || idRolePrinter.Equals(idRoleRespCons))
                return manager.GetRegistriesRespCons(idAmm);
            else
                return manager.GetRegistriesWithAooOrRfSup(idRoleResp, idRolePrinter);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static bool AssignDocumentVisibilityToResponsable(InfoUtente userInfo, String docNumber, String counterId, String rfId, String registryId, out int rightsOut)
        {
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(userInfo);
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager repManager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();


            // Recupero dell'id del ruolo responsabile e dei diritti da assegnargli
            RegistroRepertorioSingleSettings.ResponsableRight rights = RegistroRepertorioSingleSettings.ResponsableRight.R;
            String responsableIdGroup = repManager.GetResponsableRoleId(counterId, rfId, registryId, out rights);

            bool retVal = true;
            if (!String.IsNullOrEmpty(responsableIdGroup))
                retVal = docManager.AddPermissionToRole(new DirittoOggetto()
                {
                    accessRights = rights == RegistroRepertorioSingleSettings.ResponsableRight.R ? 45 : 63,
                    personorgroup = responsableIdGroup,
                    tipoDiritto = TipoDiritto.TIPO_ACQUISITO,
                    idObj = docNumber,
                    soggetto = new Ruolo() { tipoCorrispondente = "R", idGruppo = responsableIdGroup }
                });
            rightsOut = (rights == RegistroRepertorioSingleSettings.ResponsableRight.R ? 45 : 63);
            return retVal;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="docNumber"></param>
        /// <param name="counterId"></param>
        /// <param name="rfId"></param>
        /// <param name="registryId"></param>
        /// <param name="responsableIdGroup"></param>
        /// <returns></returns>
        public static bool AssignDocumentVisibilityToRole(InfoUtente userInfo, String docNumber, String counterId, String rfId, String registryId, String roleGroupId, int rights)
        {
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(userInfo);
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager repManager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            bool retVal = true;
            if (!String.IsNullOrEmpty(roleGroupId))
                retVal = docManager.AddPermissionToRole(new DirittoOggetto()
                {
                    accessRights = rights,
                    personorgroup = roleGroupId,
                    tipoDiritto = TipoDiritto.TIPO_ACQUISITO,
                    idObj = docNumber,
                    soggetto = new Ruolo() { tipoCorrispondente = "R", idGruppo = roleGroupId }
                });

            return retVal;

        }

        /// <summary>
        /// Metodo per la generazione dell'elenco dei registri da stampare
        /// </summary>
        /// <returns>Lista dei registri da stampare</returns>
        public static List<RegistroRepertorioPrint> GetRegistersToPrint(bool repairBrokenPrint)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            // Recupero delle stampe che devono partire oggi (e per cui non sia passato il termine di stampa automatica)
            List<RegistroRepertorioPrint> registeries = manager.GetRegistersToPrint(repairBrokenPrint);

            // Aggiornamento delle date previste per le prossime stampe automatiche dei registri
            foreach (var reg in registeries)
                manager.SaveNextAutomaticPrintDate(reg.CounterId, reg.RegistryId, reg.RFId, GetNextAutomaticPrintDate(DateTime.Now, reg.PrintFrequency));

            // Restituzione dei repertori da stampare
            return registeries;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public static List<RepertorioPrintRange> GetRepertoriPrintRanges(String counterId, String registryId, String rfId, bool repairBrokenPrint)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.GetRepertoriPrintRanges(counterId, registryId, rfId, repairBrokenPrint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public static bool ExistsRepertoriToPrint(String counterId, String registryId, String rfId)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.ExistsRepertoriToPrint(counterId, registryId, rfId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public static RegistroRepertorioSingleSettings.RepertorioState GetRepertorioState(String counterId, String registryId, String rfId)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.GetState(counterId, registryId, rfId);
        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// MEV Policy e responsabile conservazione
        /// Aggiorna il ruolo responsabile della conservazione per l'amministrazione selezionata
        /// </summary>
        /// <param name="idGroup"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool SaveRuoloRespConservazione(string idGroup, string idUser, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            return manager.SaveRuoloRespConservazione(idGroup, idUser, idAmm);
        }

        public static string GetResponsableRoleIdFromIdTemplate(string idTemplate, string rfId, string registryId, out string idRespCorr)
        {
            string retval="";
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
            RegistroRepertorioSingleSettings.ResponsableRight resprights;
            retval = manager.GetResponsableRoleIdFromIdTemplate(idTemplate, rfId, registryId, out resprights, out idRespCorr);
            return retval;
        }

    }

    /// <summary>
    /// Extension method per l'oggetto DateTime che consente di calcolare la maggiore fra due date
    /// </summary>
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
}
