// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO;
using DocsPaVO.addressbook;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.Note;
using DocsPaVO.PrjDocImport;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.rubrica;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Data;
using ImportProjectRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportProject;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportProject
{
    public class ImportProjectHandler : IRequestHandler<ImportProjectRequest, ImportProjectResult>
    {
        #region Public Members

        public ImportProjectHandler(ILogger<ImportProjectHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext,
            ISpreadsheetService spreadsheetService,
            IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;

            InitializeMapper();
        }

        public async Task<ImportProjectResult> Handle(ImportProjectRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.PrjDocImport.ImportResult toReturn;
            DocsPaVO.PrjDocImport.ProjectRowData rowData = request.rowData;
            string serverPath = request.serverPath;
            DocsPaVO.utente.InfoUtente userInfo = request.userInfo;
            DocsPaVO.utente.Ruolo role = request.role;
            bool isEnabledSmistamento = request.isEnabledSmistamento;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {

                // Per prima cosa si verifica se l'utente pu� creare fascicoli
                bool canCreateFuntion = role.funzioni.Where(e => e.codice == "FASC_NUOVO").FirstOrDefault() != null;
                if (!canCreateFuntion)
                    toReturn = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = ErrorDescription.RuoloNonAbilitato
                    };
                else
                {
                    toReturn = await this.ExecuteImportProject(rowData, isEnabledSmistamento, role, idTenant);
                    // Impostazione del numero ordinale
                }
                toReturn.Ordinal = rowData.OrdinalNumber;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                toReturn = new DocsPaVO.PrjDocImport.ImportResult()
                {
                    Outcome = DocsPaVO.PrjDocImport.ImportResult.OutcomeEnumeration.KO,
                    Message = ErrorDescription.GenericErrorMessage,
                };

                toReturn.OtherInformation.Add(ex.Message);
            }

            return new ImportProjectResult(toReturn);
        }



        #endregion

        #region Private members
        private async Task<ImportResult> ExecuteImportProject(ProjectRowData rowData, bool isEnabledSmistamento, Ruolo role, long idTenant)
        {
            // L'oggetto da restituire
            ImportResult toReturn;
            // Lista temporanea dei problemi
            List<string> tempProblems;
            // Il risultato della validazione
            bool validationResult;
            // Il nodo titolario in cui creare il fascicolo
            OrgNodoTitolario titolarioNode;
            // Il registro
            Registro registry;
            // L'oggetto classificazione
            DocsPaVO.fascicolazione.Classificazione classification;
            // L'oggetto con la descrizione del fascicolo
            DocsPaVO.fascicolazione.Fascicolo project;
            // Identificativo del registro da utilizzare per la creazione del fascicolo
            String registryId;
            DocsPaVO.PianoConservazione pianoConservazione = null;
            // Creazione dell'oggetto da restituire
            toReturn = new ImportResult();

            // 1. Validazione dei dati
            var enablePianoConservazione = await this._configurationService.GetValue<string>(idTenant.ToString(), "ENABLE_PIANO_CONSERVAZIONE");
            validationResult = this.CheckDataValidity(rowData, out tempProblems, enablePianoConservazione);

            // Aggiunta dei problemi alla lista dei problemi
            toReturn.OtherInformation.AddRange(tempProblems);

            // Se i dati non sono validi, il risultato � negativo
            if (!validationResult)
            {
                toReturn.Outcome = ImportResult.OutcomeEnumeration.KO;
                toReturn.Message = ErrorDescription.NotValidData;
            }
            else
            {
                string adminCode = rowData.AdminCode;
                long adminIdAsLong = await this._dbContext.AmministraEntities.Where(x => x.VAR_CODICE_AMM.ToUpper().Equals(adminCode.ToUpper())).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();

                string adminId = adminIdAsLong.ToString();
                if (string.IsNullOrEmpty(adminId))
                    throw new AdminNotFoundPi3Exception(adminCode);

                //long adminIdAsLong = adminId.AsLong();
                // Altrimenti si procede con le operazioni di creazione
                // 2. Prelevamento del nodo titolario di interesse
                titolarioNode = await this.GetTitolarioNodeObject(rowData, adminIdAsLong);
                registryId = await this._dbContext.RegistroEntities
                    .Where(x => x.ID_AMM == adminIdAsLong && x.VAR_CODICE.ToUpper().Equals(rowData.RegistryCode))
                    .Select(x => x.SYSTEM_ID.ToString())
                    .FirstOrDefaultAsync() ?? string.Empty;
                classification = await this.GetClassificationObject(rowData, titolarioNode, registryId.AsLong());

                //Estrazione del piano di conservazione
                if (!string.IsNullOrEmpty(rowData.TipologiaFascicolo))
                    pianoConservazione = await this.GetPianoConservazione(classification.systemID, rowData.TipologiaFascicolo);

                tempProblems = new List<string>();
                // Creazione dell'oggetto fascicolo
                (project, tempProblems) = await this.CreateProjectObject(
                    classification,
                    pianoConservazione,
                    rowData,
                    titolarioNode,
                    registryId,
                    role,
                    tempProblems,
                    isEnabledSmistamento,
                    adminIdAsLong);

                // Aggiunta degli eventuali errori alla lista dei dettagli
                // del seguente risultato
                toReturn.OtherInformation.AddRange(tempProblems);



                // Creazione del fascicolo
                //this.CreateProject(classification, project, role);
                var resultCreazione = await this._mediator.Send(new Requests.FascicolazioneNewFascicolo(classification, project, new InfoUtente(), role, false));

                switch (resultCreazione.resultCreazione)
                {
                    case ResultCreazioneFascicolo.FASCICOLO_GIA_PRESENTE:
                        throw new GeneralImportException(
                            String.Format(ErrorDescription.ErrorProjectAlreadyExists,
                                 project.codice));
                        break;
                    case ResultCreazioneFascicolo.FORMATO_FASCICOLATURA_NON_PRESENTE:
                        throw new GeneralImportException(
                           String.Format(ErrorDescription.ErrorProjectFascNotFound,
                                project.codice));
                        break;
                    case ResultCreazioneFascicolo.GENERIC_ERROR:
                        throw new GeneralImportException(ErrorDescription.CreatePrjGenericError);
                        break;
                }

                // Trasmissione del fascicolo
                if (rowData.TransmissionModelCode != null)
                {
                    tempProblems = await this.TransmitProject(
                        project,
                        rowData,
                        new InfoUtente(),
                        role);

                    // Aggiunta degli eventuali errori alla lista dei dettagli
                    // del risultato
                    toReturn.OtherInformation.AddRange(tempProblems);
                }

                // Salvataggio del fascicolo nell'area di lavoro
                if (rowData.InWorkingArea)
                {
                    tempProblems = await this.SaveProjectInWorkingArea(project, role);

                    // Aggiunta degli eventuali problemi alla lista
                    // dei dettagli del risultato
                    toReturn.OtherInformation.AddRange(tempProblems);

                }

                // Se si sono verificati errori durante l'elaborazione,
                // il risultato � un Fallimento
                // Altrimenti il risultato � positivo
                toReturn.Outcome = toReturn.OtherInformation.Count > 0 ? ImportResult.OutcomeEnumeration.Warnings : ImportResult.OutcomeEnumeration.OK;

                // Impostazione del messaggio
                toReturn.Message = String.Format(Resources.ResultCreation,
                        project.codice);

                // Impostazione dell'ordinale
                toReturn.Ordinal = rowData.OrdinalNumber;
            }

            // Restituzione del risultato
            return toReturn;
        }

        private async Task<List<string>> SaveProjectInWorkingArea(DocsPaVO.fascicolazione.Fascicolo project, Ruolo role)
        {
            // L'eventuale messaggio di errore da restituire
            List<string> toReturn = new List<string>();

            // Se il ruolo attuale non � abilitato alla creazione del fascicolo
            // non si pu� procedere
            Funzione canAddInADL = role.funzioni.Where(e => e.codice == "FASC_ADD_ADL").FirstOrDefault();

            if (canAddInADL == null)
                toReturn.Add(ErrorDescription.RoleNotEnabledAddinADL);
            else
            {
                bool addedInADL = (await this._mediator.Send(new Requests.DocumentoExecAddLavoro(null, null, project, new InfoUtente(), project.idRegistro))).output;
                if(!addedInADL)
                    toReturn.Add(ErrorDescription.ErrorAddInADL);
            }

            // Restituzione dell'eventuale errore
            return toReturn;
        }

        private async Task<List<string>> TransmitProject(DocsPaVO.fascicolazione.Fascicolo fascicolo, ProjectRowData rowData, InfoUtente infoUtente, Ruolo role)
        {

            #region Dichiarazione variabili
            // La lista dei problemi
            List<String> problems = new List<string>();
            // Il modello di trasmissione da utilizzare per inviare il fascicolo
            ModelloTrasmissione transmModel = null;
            // Il risultato dell'operazione di invio
            bool trasmRes;
            // Un valore utilizzato per tenere traccia del fatto che si �
            // verificata un'eccezione
            bool haveException = false;
            // L'indice dell'ultimo carattere '_'
            int lastUnderscore;
            #endregion

            // Per ogni codice di modello di trasmissione...
            foreach (string modelCode in rowData.TransmissionModelCode)
            {
                // Azzeramento del flag eccezione
                haveException = false;
                // Azzeramento del flag trasmRes
                trasmRes = true;

                // Prelevamento indice dell'ultimo _
                lastUnderscore = modelCode.LastIndexOf('_');

                // ...si prova a reperire il modello di trasmissione
                // tramite il suo id
                transmModel = (await this._mediator.Send(new Requests.getModelloByID(role.idAmministrazione, modelCode.Substring(lastUnderscore + 1)))).output;

                // Se trasmModel � valorizzato e non si riferisce a fascicoli
                if (transmModel != null && transmModel.CHA_TIPO_OGGETTO != "F")
                    // Si segnala il problema all'utente
                    problems.Add(
                        String.Format(
                            ErrorDescription.TransModelNotAdmitted,
                            modelCode.Trim()));

                // ...se il modello � per fascicoli si procede all'invio
                // altrimenti non si pu� utilizzare il modello
                else
                    trasmRes = await this.TrasmissioneExecuteTrasmFascDaModello(fascicolo, transmModel);

                if (!trasmRes)
                {
                    problems.Add(
                        String.Format(
                            ErrorDescription.TransmissionError,
                            fascicolo.descrizione, modelCode.Trim()));
                }
            }

            // Restituzione dell'insieme dei problemi
            return problems;
        }

        private async Task<bool> TrasmissioneExecuteTrasmFascDaModello(DocsPaVO.fascicolazione.Fascicolo fascicolo, ModelloTrasmissione modello)
        {

            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idUserDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            //trasmissione.infoDocumento = DocumentManager.getInfoDocumento(scheda);
            DocsPaVO.fascicolazione.Fascicolo fasc = (await this._mediator.Send(new Requests.FascicolazioneGetFascicoloById(fascicolo.systemID, new InfoUtente()))).output;
            trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fasc);

            var peopleEntity = await this._dbContext.PeopleEntities
                .Where(x => x.SYSTEM_ID == idUser)
                .FirstOrDefaultAsync();


            var infoRoleEntity = await this._dbContext.PeopleGroupEntities
                .Join(this._dbContext.CorrGlobaliEntities, a => a.GROUPS_SYSTEM_ID, b => b.ID_GRUPPO, (a, b) => new { a, b })
                .Join(this._dbContext.TipoRuoloEntities, j1 => j1.b.ID_TIPO_RUOLO, c => c.SYSTEM_ID, (j1, c) => new { a = j1.a, b = j1.b, c })
                .Join(this._dbContext.CorrGlobaliEntities, j2 => j2.b.ID_UO, d => d.SYSTEM_ID, (j2, d) => new { a = j2.a, b = j2.b, c = j2.c, d })
                .Where(x => x.a.DTA_FINE == null && x.b.ID_GRUPPO == idGroup)
                .Select(x => new InfoRoleEntity
                {
                    PEOPLE_SYSTEM_ID = x.a.PEOPLE_SYSTEM_ID,
                    SYSTEM_ID = x.b.SYSTEM_ID,
                    ID_GRUPPO = x.b.ID_GRUPPO,
                    NUM_LIVELLO = x.c.NUM_LIVELLO,
                    ID_REGISTRO = x.b.ID_REGISTRO,
                    VAR_CODICE = x.c.VAR_CODICE,
                    VAR_DESC_RUOLO = x.c.VAR_DESC_RUOLO,
                    NUM_LIVELLO_UO = x.d.NUM_LIVELLO,
                    ID_UO = x.b.ID_UO,
                    VAR_COD_RUBRICA = x.b.VAR_COD_RUBRICA ?? string.Empty,
                    ID_AMM = x.b.ID_AMM,
                    VAR_DESC_CORR = x.b.VAR_DESC_CORR ?? string.Empty,
                    CHA_PREFERITO = x.a.CHA_PREFERITO ?? string.Empty,
                    CHA_RIFERIMENTO = x.b.CHA_RIFERIMENTO ?? string.Empty,
                    CHA_RESPONSABILE = x.b.CHA_RESPONSABILE ?? string.Empty,
                    CHA_SEGRETARIO = x.b.CHA_SEGRETARIO ?? string.Empty
                })
                .FirstOrDefaultAsync();

            trasmissione.utente = this._mapper.Map<Utente>(peopleEntity);
            //var ruolo = (await this._mediator.Send(new Requests.GetListaRuoliUtente(idUser.ToString()))).output[0];
            trasmissione.ruolo = await this.GetRuoloUtente(infoRoleEntity);

            trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

            List<TrasmissioneSingola> trasmSingList = new List<TrasmissioneSingola>();

            
            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count(); i++)
            {
                DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                    DocsPaVO.utente.Corrispondente corr = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, new InfoUtente(), string.Empty, false))).output;

                    DocsPaVO.trasmissione.RagioneTrasmissione ragione = (await this._mediator.Send(new Requests.getRagioneById(mittDest.ID_RAGIONE.ToString()))).output;

                    TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                    string errorString = string.Empty;

                    (trasmSing, errorString) = (await this.GetTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM));

                    if (trasmissione.trasmissioniSingole != null)
                    {
                        // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                        for (int z = 0; z < trasmissione.trasmissioniSingole.Count(); z++)
                        {
                            DocsPaVO.trasmissione.TrasmissioneSingola ts = trasmissione.trasmissioniSingole[i];
                            if (ts.corrispondenteInterno.systemId != null && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                            {
                                if (ts.daEliminare)
                                    (trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            }
                        }
                    }
                    if(!string.IsNullOrEmpty(errorString))
                        trasmissione.listaDestinatariNonRaggiungibili.Add(errorString);

                    if (trasmSing != null)
                        trasmSingList.Add(trasmSing);
                }
            }

            trasmissione.trasmissioniSingole = trasmSingList.ToArray();

            string notify = (!string.IsNullOrEmpty(trasmissione.NO_NOTIFY) && trasmissione.NO_NOTIFY.Equals("1")) ? "0" : "1";

            if (!string.IsNullOrEmpty(idUserDelegato))
                trasmissione.delegato = idUserDelegato;

            var trasmissioneResult = (await this._mediator.Send(new Requests.TrasmissioneSaveExecuteTrasm(string.Empty, trasmissione, new InfoUtente()))).output;


            /* NON SERVE - LO FA GIA' L'HANDLER
            if (trasmissione != null)
            {
                if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                    {
                        string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        string desc = "Trasmesso Fascicolo ID: " + trasmissione.infoFascicolo.idFascicolo.ToString();
                        BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                    }
                }
            }
            */

            return trasmissioneResult != null;
        }

        private async Task<(TrasmissioneSingola, string)> GetTrasmissioneSingola(Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, RagioneTrasmissione ragione, string note, string tipoTrasm)
        {
            string errorString = string.Empty;
            //Controllo se il ruolo � disabilitato alla ricezione delle trasmissioni
            if (corr != null && corr.tipoCorrispondente == "R")
            {
                DocsPaVO.utente.Corrispondente corrispondente = (await this._mediator.Send(new Requests.GetRuoloById(corr.systemId))).Output;
                if (corrispondente != null && corrispondente.disabledTrasm)
                    return (null, string.Empty);
            }

            List<TrasmissioneUtente> trasmUtenteList = new List<TrasmissioneUtente>();

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;

                List<DocsPaVO.utente.Corrispondente> listaUtenti = await this.GetUtenti(corr);

                if (listaUtenti.Count == 0)
                {
                    errorString = string.Format(Resources.NoUsersInRole, corr.codiceCorrispondente, corr.descrizione);
                    trasmissioneSingola = null;
                }
                //ciclo per utenti se dest � gruppo o ruolo
                for (int i = 0; i < listaUtenti.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    trasmUtenteList.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                trasmUtenteList.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = trasmissione.ruolo;
                qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                List<Ruolo> ruoli = (await this._mediator.Send(new Requests.AddressbookGetRuoliRiferimentoAutorizzati(qca, theUo))).output.ToList();

                if (ruoli == null || ruoli.Count == 0)
                {
                    //Popola una lista con tutti i destinatari non raggiungibili
                    errorString = string.Format(Resources.NoUsersInRole, corr.codiceCorrispondente, corr.descrizione);
                }

                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    (trasmissioneSingola, errorString) = await this.GetTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);

                trasmissioneSingola.trasmissioneUtente = trasmUtenteList.ToArray();
                return (trasmissioneSingola, errorString);
            }

            trasmissioneSingola.trasmissioneUtente = trasmUtenteList.ToArray();
            return (trasmissioneSingola, errorString);
        }

        private async Task<List<DocsPaVO.utente.Corrispondente>> GetUtenti(DocsPaVO.utente.Corrispondente corr)
        {
            //costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = corr.idAmministrazione;
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.fineValidita = true;

            return (await this._mediator.Send(new Requests.AddressbookGetListaCorrispondenti(qco))).output.ToList();
        }
        private async Task<Ruolo> GetRuoloUtente(InfoRoleEntity infoRoleEntity)
        {
            var ruolo = new Ruolo()
            {
                systemId = infoRoleEntity.SYSTEM_ID.ToString(),
                descrizione = infoRoleEntity.VAR_DESC_CORR,
                codice = infoRoleEntity.VAR_CODICE,
                livello = infoRoleEntity.NUM_LIVELLO.ToString() ?? string.Empty,
                idGruppo = infoRoleEntity.ID_GRUPPO.ToString() ?? string.Empty,
                tipoRuolo = new TipoRuolo()
                {
                    codice = infoRoleEntity.VAR_CODICE,
                    descrizione = infoRoleEntity.VAR_DESC_RUOLO
                },
                codiceRubrica = infoRoleEntity.VAR_COD_RUBRICA,
                idRegistro = infoRoleEntity.ID_REGISTRO.ToString() ?? string.Empty,
                idAmministrazione = infoRoleEntity.ID_AMM.ToString() ?? string.Empty,
                tipoCorrispondente = "R",
                Responsabile = infoRoleEntity.CHA_RESPONSABILE == "1",
                Segretario = infoRoleEntity.CHA_SEGRETARIO == "1",
                selezionato = infoRoleEntity.CHA_PREFERITO == "1"
            };

            var uoEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.SYSTEM_ID == infoRoleEntity.ID_UO)
                .Select(c => c)
                .FirstAsync();

            ruolo.uo = this._mapper.Map<DocsPaVO.utente.UnitaOrganizzativa>(uoEntity);

            var idParent = uoEntity.ID_PARENT;

            while (idParent != null && idParent != 0)
            {
                var uoParentEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.SYSTEM_ID == idParent)
                .Select(c => c)
                .FirstAsync();

                ruolo.uo.parent = this._mapper.Map<DocsPaVO.utente.UnitaOrganizzativa>(uoParentEntity);

                idParent = uoParentEntity.ID_PARENT;
            }

            var registriEntity = await this._dbContext.RegistroEntities.AsNoTracking()
                .Join(this._dbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == infoRoleEntity.SYSTEM_ID)
                .Select(j => new
                {
                    REGISTRO = j.r,
                    j.rr.CHA_PREFERITO,
                    j.r.VAR_PREG
                })
                .OrderBy(j => j.REGISTRO.CHA_STATO)
                .ThenByDescending(j => j.CHA_PREFERITO)
                .ThenBy(j => j.REGISTRO.VAR_CODICE)
                .ThenBy(j => j.REGISTRO.VAR_DESC_REGISTRO)
                .ToListAsync();

            ruolo.registri = new Registro[registriEntity.Count];

            for (int reg = 0; reg < registriEntity.Count; reg++)
            {
                ruolo.registri[reg] = (
                    this._mapper.Map<DocsPaVO.utente.Registro>(registriEntity[reg].REGISTRO));
            }

            var funzioniEntity = await this._dbContext.FunzioneEntities.AsNoTracking()
                .Join(this._dbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                .Join(this._dbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                .Where(j => j.r.ID_RUOLO_IN_UO == infoRoleEntity.SYSTEM_ID)
                .Select(j => new
                {
                    j.f.SYSTEM_ID,
                    j.f.COD_FUNZIONE,
                    j.f.VAR_DESC_FUNZIONE,
                    j.f.ID_TIPO_FUNZIONE,
                    j.t.VAR_COD_TIPO,
                    j.t.VAR_DESC_TIPO_FUN
                })
                .ToListAsync();

            ruolo.funzioni = new Funzione[funzioniEntity.Count()];

            for (int i = 0; i < funzioniEntity.Count(); i++)
            {
                ruolo.funzioni[i] = new Funzione()
                {
                    systemId = funzioniEntity[i].SYSTEM_ID.ToString(),
                    descrizione = funzioniEntity[i].VAR_DESC_FUNZIONE ?? string.Empty,
                    codice = funzioniEntity[i].COD_FUNZIONE ?? string.Empty,
                    idTipoFunzione = funzioniEntity[i].ID_TIPO_FUNZIONE?.ToString() ?? string.Empty,
                    codTipoFunzione = funzioniEntity[i].VAR_COD_TIPO ?? string.Empty,
                    descTipoFunzione = funzioniEntity[i].VAR_DESC_TIPO_FUN ?? string.Empty
                };
            }
            return ruolo;
        }
        private async Task<(DocsPaVO.fascicolazione.Fascicolo, List<string>)> CreateProjectObject(DocsPaVO.fascicolazione.Classificazione classification, DocsPaVO.PianoConservazione pianoConservazione, ProjectRowData rowData, OrgNodoTitolario titolarioNode, String registryId, Ruolo role, List<string> profilationProblems, bool isEnabledSmistamento, long adminIdAsLong)
        {
            #region Variabili
            // La lista di stringhe con i problemi emersi durante
            // la compilazione dei dati relativi alla profilazione del 
            // documento
            List<String> profilationProblem = new List<string>();
            // L'oggetto da restituire
            DocsPaVO.fascicolazione.Fascicolo project = null;
            #endregion

            // Creazione dell'oggetto fascicolo
            project = new DocsPaVO.fascicolazione.Fascicolo();
            // Impostazione della descrizione
            project.descrizione = rowData.Description;
            // Impostazione del codice da attribuire al fascicolo
            project.codUltimo = classification.codUltimo;
            // Impostazione del flag cartaceo
            project.cartaceo = false;
            // Impostazione del valore privato
            project.privato = "0";

            // Impostazione delle note
            if (rowData.Note != null)
                project.noteFascicolo = new InfoNota[1] { rowData.Note };

            // Impostazione della data di apertura
            //if (String.IsNullOrEmpty(rowData.CreationDate))
            project.apertura = DateTime.Today.ToString("dd/MM/yyyy");
            //else
            //    project.apertura = rowData.CreationDate;

            // Impostazione dell'id del registro del nodo titolario
            project.idRegistroNodoTit = titolarioNode.IDRegistroAssociato;

            // Impostazione dell'id del registro
            if (!string.IsNullOrEmpty(titolarioNode.IDRegistroAssociato))
                project.idRegistro = registryId;

            // Compilazione dei dati relativi alla profilazione
            if (!String.IsNullOrEmpty(rowData.ProjectTipology))
                profilationProblem = await CompileProfilationFields(
                    rowData,
                    project,
                    role,
                    isEnabledSmistamento,
                    adminIdAsLong);

            project.idUoLF = string.Empty;
            project.dtaLF = (await _dbContext.GetSystemDateTime()).AsDateTimeFormat();
            project.descrizioneUOLF = string.Empty;
            project.cartaceo = false;

            project.pianoConservazione = pianoConservazione;
            // Restituzione dell'oggetto creato
            return (project, profilationProblem);
        }
        private async Task<List<string>> CompileProfilationFields(ProjectRowData rowData, DocsPaVO.fascicolazione.Fascicolo project, Ruolo role, bool isEnabledSmistamento, long adminIdAsLong)
        {
            #region Dichiarazione variabili
            // La lista dei template
            List<Templates> templates = null;
            // Il template 
            Templates template = null;
            // L'id dell'amministrazione
            string adminID = String.Empty;
            // I diritti di visibilit� sui campi della tipologia
            List<AssDocFascRuoli> visibilityRights = null;
            // La lista degli errori emersi durante la compilazione dei
            // dati profilati
            List<string> toReturn = new List<string>();
            #endregion

            // 1. Calcolo dell'id dell'amministrazione - Preso gi� prima
            adminID = adminIdAsLong.ToString();

            // 2. Prelevamento della lista dei template filtrando per il piano di conservazione (tipologia fascicolo) eventualmente selezionato
            // Se da questa estrazione non trovo tipologie allora procedo con il passo 3 estraendo tutte le tipologie visibili al ruolo
            if (project.pianoConservazione != null && !string.IsNullOrEmpty(project.pianoConservazione.SystemId))
                templates = (await this._mediator.Send(new Requests.GetTipologiaFascicoloByRuoloAndPianoConservazione(adminID, role.idGruppo, "2", project.pianoConservazione.SystemId))).output.ToList();

            // 3. Prelevamento della lista dei template creati per l'amministrazione
            if (templates == null || templates.Count > 0)
                templates = (await this._mediator.Send(new Requests.getTipoFascFromRuolo(adminID, role.idGruppo, "2"))).output.ToList();

            // 4. Ricerca del template con nome uguale a quello richiesto
            foreach (Templates temp in templates)
                if (temp.DESCRIZIONE.ToUpper().TrimStart().TrimEnd() == rowData.ProjectTipology.ToUpper().TrimStart().TrimEnd())
                    template = (await this._mediator.Send(new Requests.getTemplateFascById(temp.SYSTEM_ID.ToString()))).output;

            // Se il template non � stato recuperato con successo, 
            // eccezione
            if (template == null)
                throw new TemplateNotFoundPi3Exception(rowData.ProjectTipology);

            // Altrimenti si procede con la compilazione dei campi profilati
            // Prelevamento dei diritti di visibilit� sui campi della tipologia
            visibilityRights = (await this._mediator.Send(new Requests.getDirittiCampiTipologiaFasc(
                role.idGruppo,
                template.SYSTEM_ID.ToString()))).output.ToList();

            // Se tutto � andato bene, si pu� procedere alla compilazione dei campi
            // profilati
            Templates t = new Templates();

            (t, toReturn) = await CompileProfilationObjects(
                    rowData,
                    role,
                    template,
                    visibilityRights,
                    adminID,
                    isEnabledSmistamento);

            project.template = t;

            return toReturn;
        }
        private async Task<(Templates, List<string>)> CompileProfilationObjects(ProjectRowData rowData, Ruolo role, Templates template, List<AssDocFascRuoli> visibilityRights, string adminID, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili
            // La lista degli eventuali errori
            List<string> toReturn = new List<string>();
            // La lista dei valori associati ad una determinata
            // etichetta
            string[] fieldValues = null;
            // I diritti associati ad un determinato campo
            AssDocFascRuoli rights = null;
            #endregion

            // Compilazione degli oggetti del template
            // Per ogni oggetto del template
            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
            {
                // Recupero delle informazioni sui diritti relativi
                // all'oggetto obj
                rights = visibilityRights.Where(
                    e => e.ID_OGGETTO_CUSTOM == obj.SYSTEM_ID.ToString()).FirstOrDefault();

                // Recupero dei dati per la compilazione dell'oggetto
                // dal dizionario dei dati di profilazione contenuti nella
                // riga del foglio excel in esame
                fieldValues = rowData.GetProfilationField(obj.DESCRIZIONE.ToUpper());

                // Se fieldValues � valorizzato...
                if (fieldValues != null)
                {
                    // ...compilazione del campo profilato
                    toReturn.AddRange(await this.CompileField(
                        obj,
                        rights,
                        fieldValues,
                        role,
                        new InfoUtente(),
                        rowData.RFCode != null ? rowData.RFCode : String.Empty,
                        adminID,
                        rowData.RegistryCode != null ? rowData.RegistryCode : String.Empty,
                        isEnabledSmistamento));

                }
            }

            return (template, toReturn);
        }
        private async Task<IEnumerable<string>> CompileField(OggettoCustom customObject, AssDocFascRuoli rights, String[] fieldValues, Ruolo role, InfoUtente userInfo, string RFCode, string administrationId, string registryCode, bool isEnabledSmistamento)
        {
            #region variabili
            // Il registro
            Registro registry;
            // Il system id dell'RF
            string rfSyd = String.Empty;
            // Il system id del registro
            string registrySyd = String.Empty;
            // Tipologia di utente da ricercare
            TipoUtente userType;
            // La lista dei problemi emersi durante la compilazione del campo
            List<String> toReturn = new List<string>(); ;
            // Oggetto in cui depositare le informazioni sul campo da selezionare
            ValoreOggetto objectValue;
            #endregion

            switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
            {
                case "CASELLADISELEZIONE":
                    // Nel caso della casella di selezione � possibile che sia selezionato
                    // pi� di un valore
                    // Se il ruolo pu� modificare il campo...
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                        // ...vengono impostati i valori
                        // Per ogni stringa contenuta all'interno dei valori selezionati,
                        // bisogna ricercare l'oggetto con le informazioni sull'opzione da selezionare
                        // ricavandone la posizione ed inserendo tale descrizione nella stessa posizione
                        // ma nell'array VALORI_SELEZIONATI
                        foreach (string selectedValue in fieldValues)
                        {
                            objectValue = ((ValoreOggetto[])customObject.ELENCO_VALORI.ToArray<ValoreOggetto>()).Where(
                                e => e.VALORE.ToUpper().Equals(selectedValue.ToUpper())).FirstOrDefault();

                            // Se il valore non � stato reperito correttamente, viene lanciata una eccezione
                            if (objectValue == null)
                                throw new ValueNotValidPi3Exception(selectedValue);

                            customObject.VALORI_SELEZIONATI[Array.IndexOf(customObject.ELENCO_VALORI, objectValue)] = selectedValue;
                        }
                    else
                        // ...altrimenti non si pu� impostare il valore. Si procede quindi
                        // all'aggiunta di un messaggio di avviso alla lista dei "warnings"
                        toReturn.Add(String.Format(Resources.NoRigthsOnEditObject,
                            customObject.DESCRIZIONE));
                    break;

                case "CORRISPONDENTE":
                    // Se il ruolo possiede i diritti di modifica sul campo...
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                    {
                        DocsPaVO.utente.Corrispondente corr = await this.FindCorrispondente(fieldValues[0], customObject, userInfo, role, RFCode, registryCode, administrationId, isEnabledSmistamento);
                        customObject.VALORE_DATABASE = corr.systemId;
                    }
                    else
                        // Altrimenti si aggiunge un messaggio alla lista dei warnings
                        toReturn.Add(String.Format(Resources.NoRigthsOnEditObject,
                            customObject.DESCRIZIONE));
                    break;

                case "CONTATORE":
                case "CONTATORESOTTOCONTATORE":
                    // Reperimento dei dati sul registro
                    registry = await this.GetRegistroByCodAOO(fieldValues[0].ToUpper(), administrationId.AsLong());

                    if (registry == null)
                        throw new GeneralImportException(String.Format(ErrorDescription.ErrorFindingRegisters, fieldValues[0]));

                    switch (customObject.TIPO_CONTATORE.ToUpper())
                    {
                        case "A":   // Contatore di AOO
                            customObject.ID_AOO_RF = registry.systemId;
                            break;
                        case "R":   // Contatore di RF
                            // Se il registro non � un registro di RF, eccezione
                            if (!(registry.chaRF == "1"))
                                throw new GeneralImportException(String.Format(
                                    ErrorDescription.RegisterErrorNoRF,
                                    fieldValues[0]));

                            // Impostazione dell'id registro
                            customObject.ID_AOO_RF = registry.systemId;
                            break;
                    }
                    // Se il contatore � abilitato allo scatto differito...
                    if (customObject.CONTA_DOPO == "0")
                        // Se il ruolo ha diritti di modifica sul contatore...
                        if (rights.INS_MOD_OGG_CUSTOM == "1")
                            // Il contatore deve scattare
                            customObject.CONTATORE_DA_FAR_SCATTARE = true;
                    break;
                case "LINK":
                    if (fieldValues.Length < 2) throw new GeneralImportException(ErrorDescription.ErrorLinkObjectNotValid);
                    if ("INTERNO".Equals(customObject.TIPO_LINK))
                    {
                        if ("DOCUMENTO".Equals(customObject.TIPO_OBJ_LINK))
                        {
                            InfoDocumento infoDoc = null;
                            infoDoc = await this.GetInfoDocumento(fieldValues[1]);
                            if (infoDoc == null) throw new DocumentNotFoundPi3Exception(fieldValues[1]);
                            var verificaACLResult = (await this._mediator.Send(new Requests.VerificaACL("D", infoDoc.idProfile, userInfo)));
                            string errorMessage = verificaACLResult.errorMessage;
                            int result = verificaACLResult.output;
                            if (result != 2)
                                throw new GeneralImportException(String.Format(ErrorDescription.NoRightsForDoc, fieldValues[1]));
                        }
                        else
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = null;
                            fasc = (await this._mediator.Send(new Requests.FascicolazioneGetFascicoloDaCodice(userInfo, fieldValues[1], null, false, true))).output;
                            if (fasc == null) throw new ProjectNotFoundPi3Exception(fieldValues[1]);
                            var verificaACLResult = (await this._mediator.Send(new Requests.VerificaACL("F", fasc.systemID, userInfo)));
                            string errorMessage = verificaACLResult.errorMessage;
                            int result = verificaACLResult.output;
                            if (result != 2)
                                throw new GeneralImportException(String.Format(ErrorDescription.NoRightsForProj, fieldValues[1]));
                        }
                    }
                    customObject.VALORE_DATABASE = fieldValues[0] + "||||" + fieldValues[1];
                    break;
                case "OGGETTOESTERNO":
                    if (fieldValues.Length < 2) throw new GeneralImportException(ErrorDescription.ErrorExtObjectNotValid);
                    customObject.MANUAL_INSERT = true;
                    customObject.CODICE_DB = fieldValues[0];
                    customObject.VALORE_DATABASE = fieldValues[1];
                    break;
                default:
                    // In tutti gli altri casi il valore � uno solo
                    // Se il ruolo ha diritti di modofica, viene impostato
                    // il valore altrimenti viene inserito un messaggio nella
                    // lista dei warning
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                        customObject.VALORE_DATABASE = fieldValues[0];
                    else
                        // ...altrimenti non si pu� impostare il valore. Si procede quindi
                        // all'aggiunta di un messaggio di avviso alla lista dei "warnings"
                        toReturn.Add(String.Format(Resources.NoRigthsOnEditObject,
                            customObject.DESCRIZIONE));
                    break;

            }
            return toReturn;
        }
        private async Task<InfoDocumento> GetInfoDocumento(string idProfile)
        {
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            string chaCestino = string.Empty;

            if (string.IsNullOrEmpty(idProfile))
                return null;

            InfoDocumento infoDoc = (await this._mediator.Send(new Requests.GetInfoDocumento(new InfoUtente(), idProfile, idProfile))).output;

            //???? si dovrebbero popolare i mittenti e i destinatari ma nel vecchio be cerca una Table "CORRISPONDENTI" che non viene popolata da nessuna parte.. Che ci metto?
            return infoDoc;
        }
        private async Task<DocsPaVO.utente.Corrispondente> FindCorrispondente(string codice, OggettoCustom customObject, InfoUtente userInfo, Ruolo role, string RFCode, string registryCode, string administrationId, bool isEnabledSmistamento)
        {
            Registro registry;
            string rfSyd = string.Empty;
            string registrySyd = string.Empty;
            TipoUtente userType;

            if (String.IsNullOrEmpty(codice))
                throw new GeneralImportException(ErrorDescription.NotValidCorrCode);

            if (!String.IsNullOrEmpty(RFCode))
            {
                registry = await this.GetRegistroByCodAOO(RFCode.ToUpper(), administrationId.AsLong());
                if (registry == null)
                    throw new RegisterNotFoundPi3Exception(RFCode);

                if (registry.chaRF.Trim() != "1")
                    throw new GeneralImportException(String.Format(ErrorDescription.RegisterErrorNoRF, RFCode));

                rfSyd = registry.systemId;
            }

            registry = await this.GetRegistroByCodAOO(registryCode.ToUpper(), administrationId.AsLong());

            registrySyd = registry.systemId;


            switch (customObject.TIPO_RICERCA_CORR.ToUpper())
            {
                case "INTERNI":
                    userType = TipoUtente.INTERNO;
                    break;

                case "ESTERNI":
                    userType = TipoUtente.ESTERNO;
                    break;

                default:
                    userType = TipoUtente.GLOBALE;
                    break;
            }

            // Impostazione del corrispondente
            return await GetCorrispondenteByCode(
                ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                codice,
                role,
                registrySyd,
                rfSyd,
                isEnabledSmistamento,
                userType);
        }
        private async Task<DocsPaVO.utente.Corrispondente> GetCorrispondenteByCode(ParametriRicercaRubrica.CallType callType, string corrCode, Ruolo role, string registrySyd,
            string rfSyd, bool isEnabledSmistamento, TipoUtente userTypeForProject)
        {
            #region Dichiarazione Variabili
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            // L'oggetto utilizzato per memorizzare i parametri di ricerca
            ParametriRicercaRubrica searchParameters;
            // L'oggetto per memorizzare le impostazioni sullo smistamento
            SmistamentoRubrica smistamentoRubrica;
            // La lista degli elementi resttiuiti dalla ricerca
            List<ElementoRubrica> corrList;
            // Il corrispondente da restituire
            DocsPaVO.utente.Corrispondente toReturn = null;
            #endregion

            #region Impostazione parametri di ricerca
            // Creazione oggetto per la memorizzazione dei parametri di ricerca
            searchParameters = new ParametriRicercaRubrica();
            // Impostazione del call type
            searchParameters.calltype = callType;
            // Impostazione del codice da ricercare
            searchParameters.codice = corrCode;
            // Impostazione del flag per la ricerca del codice esatta
            searchParameters.queryCodiceEsatta = true;
            // Creazione del caller
            searchParameters.caller = new ParametriRicercaRubrica.CallerIdentity();
            // Impostazione del calltype
            searchParameters.caller.IdRuolo = role.systemId;
            // Impostazione dell'id utente
            searchParameters.caller.IdUtente = idUser;
            // Impostazione dell'id registro
            searchParameters.caller.IdRegistro = registrySyd;
            // Impostazione del filtro registro per la ricerca
            searchParameters.caller.filtroRegistroPerRicerca = registrySyd;
            // La ricerca va effettuata su Uffici, Utenti, Ruoli, RF
            searchParameters.doUo = true;
            searchParameters.doUtenti = true;
            searchParameters.doRuoli = true;
            searchParameters.doRF = true;
            bool abilitazioneRubricaComune = (await this._mediator.Send(new Requests.GetConfigurazioniRubricaComune(new InfoUtente()))).output.GestioneAbilitata;
            searchParameters.doRubricaComune = abilitazioneRubricaComune;
            #endregion

            #region Impostazione parametri per smistamento
            // Creazione oggetto per parametri smistamento
            smistamentoRubrica = new SmistamentoRubrica();
            // Abilitazione smistamento
            smistamentoRubrica.smistamento = isEnabledSmistamento ? "1" : "0";
            // Impostazione calltype
            smistamentoRubrica.calltype = callType;
            // Impostazione informazioni sull'utente
            smistamentoRubrica.infoUt = new InfoUtente();
            // Impostazione ruolo
            smistamentoRubrica.ruoloProt = role;
            // Impostazione dell'id del registro
            smistamentoRubrica.idRegistro = registrySyd;
            #endregion


            #region Impostazione parametri dipendenti dal contesto
            // Impostazione parametri dipendenti dal contesto
            switch (callType)
            {
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN:
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = abilitazioneRubricaComune;
                    searchParameters.doRubricaComune = true;
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;
                    break;
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;
                    break;
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT:
                    searchParameters.doListe = true;
                    searchParameters.doRubricaComune = abilitazioneRubricaComune;
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;
                    break;
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;
                    break;
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
                    searchParameters.doListe = true;
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.INTERNO;
                    break;
                case ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST:
                    if (!String.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = true;
                    searchParameters.tipoIE = userTypeForProject;
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    break;
            }
            #endregion

            corrList = (await this._mediator.Send(new Requests.rubricaGetElementiRubrica(searchParameters, new InfoUtente(), smistamentoRubrica))).output.ToList();

            // Se non sono stati restituiti corrispondenti eccezione
            if (corrList == null || corrList.Count == 0)
                throw new CorrespondentNotFoundPi3Exception(corrCode);

            // Se sono stati restituiti pi� corrispondenti, ambiguit�
            if (corrList.Count > 1)
                throw new GeneralImportException(String.Format(ErrorDescription.MoreThanOneCorrFoundError, corrCode, corrList.Count));

            /* Rubrica comune - serve questo pezzo?
            ElementoRubrica temp = (ElementoRubrica)corrList[0];
            if (temp.isRubricaComune)
            {
                Corrispondente tempCorr = BusinessLogic.RubricaComune.RubricaServices.UpdateCorrispondente(userInfo, temp.codice);
                toReturn = tempCorr;
            }*/

            toReturn = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteBySystemId(corrList[0].systemId))).output;

            if (toReturn == null)
                throw new GeneralImportException(string.Format(ErrorDescription.ErrorFindingCorr, corrCode));

            return toReturn;
        }
        private async Task<Registro> GetRegistroByCodAOO(string codAoo, long administrationId)
        {
            Registro result = null;
            RegistroEntity registroEntity = await this._dbContext.RegistroEntities
                .Where(x => x.VAR_CODICE.ToUpper().Equals(codAoo.ToUpper()) && x.ID_AMM == administrationId)
                .FirstOrDefaultAsync();

            if (registroEntity != null)
            {
                result = this._mapper.Map<Registro>(registroEntity);
                result.codAmministrazione = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == result.systemId.AsLong()).Select(x => x.VAR_CODICE_AMM).FirstOrDefaultAsync();
            }

            return result;
        }
        private async Task<OrgNodoTitolario> GetTitolarioNodeObject(ProjectRowData rowData, long adminIdAsLong)
        {

            var titolarioEntity = await this._dbContext.ProjectEntities
                .Join(this._dbContext.AmministraEntities, p => p.ID_AMM, a => a.SYSTEM_ID, (p, a) => new { p, a })
                .Where(x => x.p.ID_AMM == adminIdAsLong && x.p.ID_TITOLARIO == 0 && x.p.ID_PARENT == 0 && x.p.CHA_STATO.Equals("A") && x.p.VAR_CODICE.Equals("T"))
                .Select(x => new
                {
                    p = x.p,
                    VAR_CODICE_AMM = x.a.VAR_CODICE_AMM
                })
            .FirstOrDefaultAsync();

            OrgTitolario titolario = null;
            if (titolarioEntity == null)
                throw new TitolarioNotFoundPi3Exception();

            titolario = _mapper.Map<OrgTitolario>(titolarioEntity.p);
            titolario.CodiceAmministrazione = titolarioEntity.VAR_CODICE_AMM ?? string.Empty;

            string regCode = rowData.RegistryCode;
            if (String.IsNullOrEmpty(regCode))
                throw new RegisterNotFoundPi3Exception(string.Empty);

            OrgNodoTitolario titolarioNode = null;
            long idReg = await this._dbContext.RegistroEntities
                .Where(x => x.ID_AMM == adminIdAsLong && x.VAR_CODICE.ToUpper().Equals(regCode.ToUpper()))
                .Select(x => x.SYSTEM_ID)
                .FirstOrDefaultAsync();

            long idTitolario = titolario.ID.AsLong();
            string codeNode = rowData.NodeCode;
            var titolarioNodeEntity = await this._dbContext.ProjectEntities
                .Join(this._dbContext.AmministraEntities, p => p.ID_AMM, a => a.SYSTEM_ID, (p, a) => new { p, a })
                .Where(x => x.p.VAR_CODICE.Equals(codeNode) && x.p.CHA_TIPO_PROJ.Equals("T") && x.p.ID_AMM == adminIdAsLong &&
                    (x.p.ID_REGISTRO == idReg || x.p.ID_REGISTRO == null) && x.p.ID_TITOLARIO == idTitolario)
                .Select(x => new
                {
                    p = x.p,
                    VAR_CODICE_AMM = x.a.VAR_CODICE_AMM
                })
                .FirstOrDefaultAsync();

            if (titolarioNodeEntity == null)
                throw new TitolarioNodeNotFoundPi3Exception(codeNode);

            titolarioNode = _mapper.Map<OrgNodoTitolario>(titolarioNodeEntity.p);
            titolarioNode.CodiceAmministrazione = titolarioNodeEntity.VAR_CODICE_AMM ?? string.Empty;

            return titolarioNode;
        }
        private async Task<PianoConservazione> GetPianoConservazione(string idClassificazione, string tipologiaFascicolo)
        {
            // Piano di conservazione da restituire
            DocsPaVO.PianoConservazione pianoConservazione = null;

            List<DocsPaVO.PianoConservazione> piano = (await this._mediator.Send(new Requests.GetPianoConservazioneByIdClassificazione(idClassificazione, null))).output;
            if (piano != null && piano.Any())
                pianoConservazione = (from p in piano where p.TipologiaFascicolo.Trim().ToLower().Equals(tipologiaFascicolo.Trim().ToLower()) select p).FirstOrDefault();

            if (pianoConservazione == null)
                throw new PianoConservazioneNotFoundPi3Exception(tipologiaFascicolo);

            // Restituzione delle informazioni sul piano di conservazione
            return pianoConservazione;
        }
        private async Task<DocsPaVO.fascicolazione.Classificazione> GetClassificationObject(ProjectRowData rowData, OrgNodoTitolario titolarioNode, long idReg)
        {
            DocsPaVO.fascicolazione.Classificazione classification = new DocsPaVO.fascicolazione.Classificazione();

            // Impostazione del codice nodo
            classification.codice = rowData.NodeCode;

            long idTitolario = titolarioNode.ID.AsLong();

            if (String.IsNullOrEmpty(rowData.ProjectNumber))
            {
                var predicate = PredicateBuilder.New<RegFascEntity>();
                predicate = predicate.And(x => x.ID_TITOLARIO == idTitolario);
                if (!string.IsNullOrEmpty(titolarioNode.IDRegistroAssociato))
                    predicate = predicate.And(x => x.ID_REGISTRO == idReg);
                else
                    predicate = predicate.And(x => x.ID_REGISTRO == null);
                classification.codUltimo = await this._dbContext.RegFascEntities.Where(predicate).Select(x => x.NUM_RIF.ToString()).FirstOrDefaultAsync() ?? string.Empty;
            }
            else
                classification.codUltimo = rowData.ProjectNumber;

            // Impostazione descrizione
            classification.descrizione = rowData.Description;

            // Impostazione del system id
            classification.systemID = titolarioNode.ID;

            // Impostazione del codice di livello
            classification.varcodliv1 = titolarioNode.CodiceLivello;

            // Restituzine dell'oggetto classificazione
            return classification;
        }
        private bool CheckDataValidity(ProjectRowData rowData, out List<string> notValidData, string enablePianoConservazione)
        {
            // La lista con gli errori da restituire
            List<String> problems = new List<string>();

            // Il risultato del controllo di validit�
            bool validationResult = true;

            // L'ordinale � obbligatorio
            if (String.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                validationResult = false;
                problems.Add(ErrorDescription.RequiredOrdinal);
            }

            // Il codice amministrazione � obbligatorio
            if (String.IsNullOrEmpty(rowData.AdminCode))
            {
                validationResult = false;
                problems.Add(ErrorDescription.RequiredAdminCode);
            }

            // Il codice registro � obbligatorio
            if (String.IsNullOrEmpty(rowData.RegistryCode))
            {
                validationResult = false;
                problems.Add(ErrorDescription.RequiredRegCode);
            }

            // La descrizione � obbligatoria
            if (String.IsNullOrEmpty(rowData.Description))
            {
                validationResult = false;
                problems.Add(ErrorDescription.RequiredDescription);
            }

            // Il nodo di titolario
            if (String.IsNullOrEmpty(rowData.NodeCode))
            {
                validationResult = false;
                problems.Add(ErrorDescription.RequiredCodeNode);
            }

            // Tipologia fascicolo piano conservazione
            if (String.IsNullOrEmpty(rowData.TipologiaFascicolo))
            {
                //Se � attiva la chiave del piano di conservazione controllo l'obbligatoriet�
                if (!string.IsNullOrEmpty(enablePianoConservazione) && enablePianoConservazione.Equals("1"))
                {
                    validationResult = false;
                    problems.Add(ErrorDescription.RequiredPrjTipology);
                }
            }

            // Impostazione della lista di problemi di vlaidazione
            notValidData = problems;

            // Restituzione del risultato della validazione
            return validationResult;
        }

        protected readonly ILogger<ImportProjectHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected ISpreadsheetService _spreadsheetService;
        protected IConfiguration _configuration;
        protected readonly string _logPath;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProjectEntity, OrgTitolario>()
                    .ForMember(dest => dest.ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.Commento, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.DataAttivazione, src => src.MapFrom(opt => opt.DTA_ATTIVAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.DataCessazione, src => src.MapFrom(opt => opt.DTA_CESSAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.MaxLivTitolario, src => src.MapFrom(opt => opt.MAX_LIV_TIT))
                    .ForMember(dest => dest.EtichettaTit, src => src.MapFrom(opt => opt.ET_TITOLARIO))
                    .ForMember(dest => dest.EtichettaLiv1, src => src.MapFrom(opt => opt.ET_LIVELLO1))
                    .ForMember(dest => dest.EtichettaLiv2, src => src.MapFrom(opt => opt.ET_LIVELLO2))
                    .ForMember(dest => dest.EtichettaLiv3, src => src.MapFrom(opt => opt.ET_LIVELLO3))
                    .ForMember(dest => dest.EtichettaLiv4, src => src.MapFrom(opt => opt.ET_LIVELLO4))
                    .ForMember(dest => dest.EtichettaLiv5, src => src.MapFrom(opt => opt.ET_LIVELLO5))
                    .ForMember(dest => dest.EtichettaLiv6, src => src.MapFrom(opt => opt.ET_LIVELLO6))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => GetDescriptionTitolario(opt)))
                    .ForMember(dest => dest.Stato, src => src.MapFrom(opt => GetStateTitolario(opt)));

                cfg.CreateMap<ProjectEntity, OrgNodoTitolario>()
                    .ForMember(dest => dest.ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.bloccaTipoFascicolo, src => src.MapFrom(opt => opt.CHA_BLOCCA_FASC))
                    .ForMember(dest => dest.CodiceLivello, src => src.MapFrom(opt => opt.VAR_COD_LIV1))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.ID_TipoFascicolo, src => src.MapFrom(opt => opt.ID_TIPO_FASC))
                    .ForMember(dest => dest.IDParentNodoTitolario, src => src.MapFrom(opt => opt.ID_PARENT))
                    .ForMember(dest => dest.IDRegistroAssociato, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.Livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.NumeroMesiConservazione, src => src.MapFrom(opt => opt.NUM_MESI_CONSERVAZIONE == null ? 0 : Convert.ToInt32(opt.NUM_MESI_CONSERVAZIONE)))
                    .ForMember(dest => dest.note, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.numProtoTit, src => src.MapFrom(opt => opt.NUM_PROT_TIT))
                    .ForMember(dest => dest.contatoreAttivo, src => src.MapFrom(opt => opt.CHA_CONTA_PROT_TIT))
                    .ForMember(dest => dest.bloccaNodiFigli, src => src.MapFrom(opt => opt.CHA_BLOCCA_FIGLI))
                    .ForMember(dest => dest.dataCreazione, src => src.MapFrom(opt => opt.DTA_CREAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.CreazioneFascicoliAbilitata, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_RW) && opt.CHA_RW.Equals("W")));

                cfg.CreateMap<RegistroEntity, Registro>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codRegistro, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.NUM_RIF))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_OPEN.AsDateTimeFormat()))
                    .ForMember(dest => dest.dataChiusura, src => src.MapFrom(opt => opt.DTA_CLOSE.AsDateTimeFormat()))
                    .ForMember(dest => dest.dataUltimoProtocollo, src => src.MapFrom(opt => opt.DTA_ULTIMO_PROTO.AsDateTimeFormat()))
                    .ForMember(dest => dest.idRuoloAOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idUtenteAOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => opt.CHA_DISABILITATO))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => opt.VAR_PREG.Equals("1")))
                    .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => opt.ANNO_PREG))
                    .ForMember(dest => dest.Diritto_Ruolo_AOO, src => src.MapFrom(opt => opt.DIRITTO_RUOLO_AOO));

                cfg.CreateMap<ProfileEntity, InfoDocumento>()
                     .ForMember(dest => dest.idProfile, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.DOCNUMBER))
                     .ForMember(dest => dest.tipoProto, opt => opt.MapFrom(src => src.CHA_TIPO_PROTO))
                     .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.ID_REGISTRO ?? null))
                     .ForMember(dest => dest.oggetto, opt => opt.MapFrom(src => src.VAR_PROF_OGGETTO.Replace("<", "&lt;").Replace(">", "&gt;")))
                     .ForMember(dest => dest.evidenza, opt => opt.MapFrom(src => src.CHA_EVIDENZA))
                     .ForMember(dest => dest.privato, opt => opt.MapFrom(src => src.CHA_PRIVATO))
                     .ForMember(dest => dest.personale, opt => opt.MapFrom(src => src.CHA_PERSONALE))
                     .ForMember(dest => dest.numProt, opt => opt.MapFrom(src => src.NUM_PROTO))
                     .ForMember(dest => dest.dataApertura, opt => opt.MapFrom(src => src.DTA_PROTO.AsDateFormat() ?? src.CREATION_DATE.AsDateFormat()))
                     .ForMember(dest => dest.segnatura, opt => opt.MapFrom(src => src.VAR_SEGNATURA))
                     .ForMember(dest => dest.dataAnnullamento, opt => opt.MapFrom(src => src.DTA_ANNULLA.AsDateFormat() ?? null))
                     .ForMember(dest => dest.acquisitaImmagine, opt => opt.MapFrom(src => src.EXT))
                     .ForMember(dest => dest.idTipoAtto, opt => opt.MapFrom(src => src.ID_TIPO_ATTO))
                     .ForMember(dest => dest.allegato, opt => opt.MapFrom(src => src.ID_DOCUMENTO_PRINCIPALE > 0));

                cfg.CreateMap<PeopleEntity, DocsPaVO.utente.Utente>()
                     .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                     .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_COGNOME + " " + src.VAR_NOME))
                     .ForMember(dest => dest.telefono, opt => opt.MapFrom(src => src.VAR_TELEFONO))
                     .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.EMAIL_ADDRESS))
                     .ForMember(dest => dest.notifica, opt => opt.MapFrom(src => src.CHA_NOTIFICA))
                     .ForMember(dest => dest.amministratore, opt => opt.MapFrom(src => src.CHA_AMMINISTRATORE.Equals("1")))
                     .ForMember(dest => dest.assegnante, opt => opt.MapFrom(src => src.CHA_AMMINISTRATORE.Equals("1")))
                     .ForMember(dest => dest.assegnatario, opt => opt.MapFrom(src => src.CHA_AMMINISTRATORE.Equals("1")))
                     .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.notificaConAllegato, opt => opt.MapFrom(src => src.CHA_NOTIFICA_CON_ALLEGATO))
                     .ForMember(dest => dest.sede, opt => opt.MapFrom(src => src.VAR_SEDE))
                     .ForMember(dest => dest.matricola, opt => opt.MapFrom(src => src.MATRICOLA))
                     .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => "P"))
                     .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.VAR_COGNOME))
                     .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.VAR_NOME));

                cfg.CreateMap<CorrGlobaliEntity, DocsPaVO.utente.UnitaOrganizzativa>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.codiceRubrica, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.interoperante, src => src.MapFrom(opt => opt.CHA_PA == "1"))
                    .ForMember(dest => dest.codiceAOO, src => src.MapFrom(opt => opt.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL))
                    .ForMember(dest => dest.tipoIE, src => src.MapFrom(opt => opt.CHA_TIPO_IE))
                    .ForMember(dest => dest.tipoCorrispondente, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.classificaUO, src => src.MapFrom(opt => opt.CLASSIFICA_UO));

            });

            _mapper = configuration.CreateMapper();
        }
        private string GetDescriptionTitolario(ProjectEntity entity)
        {
            switch (entity.CHA_STATO)
            {
                case "D":
                    return entity.DESCRIPTION + Resources.TitolarioInDefinizione;
                case "A":
                    return entity.DESCRIPTION + Resources.TitolarioAttivo;
                case "C":
                    return entity.DESCRIPTION + string.Format(Resources.TitolarioChiuso, entity.DTA_ATTIVAZIONE.AsDateFormat(), entity.DTA_CESSAZIONE.AsDateFormat());
                default:
                    return "";
            }
        }
        private OrgStatiTitolarioEnum GetStateTitolario(ProjectEntity entity)
        {
            switch (entity.CHA_STATO)
            {
                case "D":
                    return OrgStatiTitolarioEnum.InDefinizione;
                case "A":
                    return OrgStatiTitolarioEnum.Attivo;
                case "C":
                    return OrgStatiTitolarioEnum.Chiuso;
                default:
                    return OrgStatiTitolarioEnum.InDefinizione;
            }
        }
        #endregion
    }

    internal class InfoRoleEntity
    {
        public long? PEOPLE_SYSTEM_ID { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_GRUPPO { get; set; }
        public long? NUM_LIVELLO { get; set; }
        public long? ID_REGISTRO { get; set; }
        public string VAR_CODICE { get; set; }
        public string VAR_DESC_RUOLO { get; set; }
        public long? NUM_LIVELLO_UO { get; set; }
        public long? ID_UO { get; set; }
        public string VAR_COD_RUBRICA { get; set; }
        public long? ID_AMM { get; set; }
        public string VAR_DESC_CORR { get; set; }
        public string CHA_PREFERITO { get; set; }
        public string CHA_RIFERIMENTO { get; set; }
        public string CHA_RESPONSABILE { get; set; }
        public string CHA_SEGRETARIO { get; set; }
    }
}