// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects;
using Pi3.Core.Extensions;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente;
using System.Globalization;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SalvaModificaStato;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using System.Xml;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject
{
    // Richiede libreria MediatR
    public class CreateDocumentAndAddInProjectCommandHandler : IRequestHandler<CreateDocumentAndAddInProjectCommand, CreateDocumentAndAddInProjectCommandResponse>
    {
        #region Public Members

        public CreateDocumentAndAddInProjectCommandHandler(
            ILogger<CreateDocumentAndAddInProjectCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService loggerService,
            ICAdESService cAdESService,
            IPAdESService pAdESService,
            IAggregazioneDocumentaleRepository aggDocRepository,
            IDocumentBlobRepository blobRepository,
            IHttpContextAccessor httpContextAccessor,
            IFileValidatorService fileValidatorService,
            INotaRepository notaRepository,
            ITrasmissioneRepository trasmissioneRepository
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._documentoAmministrativo = documentoAmministrativoRepository;
            this._loggerService = loggerService;
            this._aggDocRepository = aggDocRepository;
            this._blobRepository = blobRepository;
            this._cAdESService = cAdESService;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;
            this._notaRepository = notaRepository;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<CreateDocumentAndAddInProjectCommandResponse> Handle(CreateDocumentAndAddInProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("CreateDocumentAndAddInProject - START");

            CreateDocumentAndAddInProjectCommandResponse response = new CreateDocumentAndAddInProjectCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                bool daRepertoriare = false;
                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }
                if (request == null || request.Document == null)
                {
                    throw new RestException("REQUIRED_DOCUMENT");
                }
                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new RestException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new RestException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new RestException("REQUIRED_REGISTER");
                }

                if (request != null && request.Document != null && request.Document.MainDocument != null && request.Document.MainDocument.Content != null && request.Document.MainDocument.Content.Length > 0)
                {
                    var maxfilesize = DBUtils.maxFileSizePermitted(_pi3DbContext);
                    if (maxfilesize > 0 && request.Document.MainDocument.Content.Length > maxfilesize)
                    {
                        throw new RestException("FILE_SIZE_EXCEEDED");
                    }

                    using var stream = new MemoryStream(request.Document.MainDocument.Content);
                    var validateResult = await this._fileValidatorService.Validate(new FileToValidate()
                    {
                        Name = request.Document.MainDocument.Name,
                        Stream = stream
                    });

                    if (!validateResult.FormatIsAdmitted)
                        throw new RestException("FILE_FORMAT_NOT_ADMITTED");
                }

                #region controlli sender rec
                if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                    request.Document.Sender.CorrespondentType = "O";

                if (request.Document.MultipleSenders != null)
                {
                    for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                    {
                        Correspondent c = request.Document.MultipleSenders[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.Recipients != null)
                {
                    for (int i = 0; i < request.Document.Recipients.Length; i++)
                    {
                        Correspondent c = request.Document.Recipients[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.RecipientsCC != null)
                {
                    for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                    {
                        Correspondent c = request.Document.RecipientsCC[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }
                #endregion

                #endregion

                #region implementazione
                var id_amm = infoUtente.idAmministrazione;
                #region prelievo fascicolo
                string idfascicoloPISInpunt = "";
                if (!string.IsNullOrWhiteSpace(request.CodeProject))
                {
                    idfascicoloPISInpunt = (from a in _pi3DbContext.ProjectEntities where a.VAR_CODICE.ToUpper() == request.CodeProject.ToUpper() && a.ID_TITOLARIO.ToString() == request.ClassificationSchemeId select a.SYSTEM_ID).FirstOrDefault().ToString();
                }
                else
                {
                    idfascicoloPISInpunt = request.IdProject;
                }

                if (string.IsNullOrWhiteSpace(idfascicoloPISInpunt) || idfascicoloPISInpunt == "0") throw new RestException("PROJECT_NOT_FOUND");

                //Controllo visibilit� fascicolo
                var securityRigths = await _pi3DbContext.GetSecurityRights(idfascicoloPISInpunt, infoUtente.idPeople, infoUtente.idGruppo);
                switch (securityRigths)
                {
                    case SecurityRightTypesEnum.Deny:
                        throw new RestException("PROJECT_NOT_VISIBLE");
                    case SecurityRightTypesEnum.Read:
                        //cerco una trasmissione pending al ruolo con workflow
                        var trasmPendente = await DBUtils.GetTrasmPendenteConWorkflowFascicolo(idfascicoloPISInpunt, infoUtente.idCorrGlobali, infoUtente.idPeople, _pi3DbContext);
                        if (trasmPendente != 0)
                        {
                            //Accetta
                            var fascAggregate = await _trasmissioneRepository.Get(infoUtente.idAmministrazione, trasmPendente.ToString());
                            fascAggregate.Accetta(infoUtente.idGruppo.ToString(), infoUtente.idPeople, new Accetta()
                            {
                                Data = DateTime.Now,
                                //Note = new TextValue(trasmUtente.noteAccettazione), //Non ce le metto
                                IdDelegato = null
                            });
                            await _trasmissioneRepository.Update(fascAggregate);
                            break;
                        }
                        else
                            throw new RestException("PROJECT_NOT_EDITABLE");
                    case SecurityRightTypesEnum.Write:
                    case SecurityRightTypesEnum.FullControl:
                        break;
                    default:
                        throw new RestException("PROJECT_NOT_FOUND");
                }


                Project fascicoloPISInput = null;
                try
                {
                    fascicoloPISInput = DBUtils.getProjectFromDB(idfascicoloPISInpunt, _pi3DbContext);
                }
                catch (Exception ex) { throw new RestException("PROJECT_NOT_FOUND"); }
                if (fascicoloPISInput == null) throw new RestException("PROJECT_NOT_FOUND");

                //Controllo se il fascicolo � chiuso 
                if (!string.IsNullOrEmpty(fascicoloPISInput.ClosureDate))
                    throw new RestException("PROJECT_CLOSED");

                #endregion
                #region Creazione Documento
                //TODO: gestione protocolli, predisposti, tipologie, note, file
                bool privato = request != null && request.Document != null && request.Document.PrivateDocument;
                bool personale = request != null && request.Document != null && request.Document.PersonalDocument;
                DocsPaVO.utente.Registro registro = null;
                var tipologiaFlusso = RestUtils.AsTipologiaFlusso(request.Document.DocumentType);
                bool predisponi = false;
                if (request.Document.Predisposed
                    && request.Document.DocumentType.ToUpper() != "G")
                {
                    predisponi = true;
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) || !string.IsNullOrEmpty(request.CodeRegister))
                {
                    registro = DBUtils.getRegistroByCodAOO(request.CodeRegister, infoUtente.idAmministrazione, _pi3DbContext);
                    if (registro == null) throw new RestException("REGISTER_NOT_FOUND");
                }

                //gestione registro per creazione NP
                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("G") && string.IsNullOrWhiteSpace(request.CodeRegister)))
                {
                    var idRuoloInUO = await this._pi3DbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == infoUtente.idGruppo.AsLong()).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                    var registri = await GetListaRegistriAttiviByRuolo(idRuoloInUO);
                    if (registri.Count() == 1)
                        registro = registri[0];
                    else
                        throw new RestException("REQUIRED_REGISTER");
                }

                var aggregate = new DocumentoAmministrativo(infoUtente.idAmministrazione,
                                        DateTime.Now,
                                        new OggettoDelDocumento()
                                        {
                                            Id = null,
                                            Descrizione = new TextValue(request.Document.Object)
                                        },
                                        registro == null ? null : new DatiRegistro() { IdRegistro = registro.systemId },
                                        predisponi ? null : tipologiaFlusso,
                                        privato ? TipologieVisibilitaEnum.Privata : (personale ? TipologieVisibilitaEnum.Personale : TipologieVisibilitaEnum.Gerarchica));



                #region AGGIUNTA PROFILAZIONE DIN
                DocsPaVO.ProfilazioneDinamica.Templates? template = null;
                long idOggettoRepertorio = 0;
                long idTipoAtto = 0;
                if (request != null && request.Document != null && request.Document.Template != null &&
                    (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                {
                    if (!string.IsNullOrEmpty(request.Document.Template.Id))
                    {
                        template = DBUtils.GetTemplateById(request.Document.Template.Id, _pi3DbContext);
                    }
                    else if (!string.IsNullOrEmpty(request.Document.Template.Name))
                    {
                        template = DBUtils.GetTemplateByDescrizione(request.Document.Template.Name, id_amm, _pi3DbContext);
                    }

                    if (template == null)
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                    else
                    {
                        idTipoAtto = template.ID_TIPO_ATTO.AsLong();
                        idOggettoRepertorio = template.ELENCO_OGGETTI
                        .Where(o => o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore")
                            && o.REPERTORIO.Equals("1")
                            && o.CONTATORE_DA_FAR_SCATTARE
                            && string.IsNullOrEmpty(o.VALORE_DATABASE))
                        .Select(o => o.SYSTEM_ID)
                        .FirstOrDefault();

                        if (idOggettoRepertorio != null && idOggettoRepertorio != 0)
                            daRepertoriare = true;

                        var listaTemplate = DBUtils.GetListTemplatesLiteByRoleWithReadWriteRights(infoUtente.idAmministrazione, infoUtente.idGruppo, _pi3DbContext);
                        var inListaTemp = listaTemplate?.Select(t => t.system_id).Contains(template.SYSTEM_ID.ToString()) ?? false;
                        if (listaTemplate == null || !inListaTemp)
                        {
                            throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                        }
                        File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);
                        template = (await this._mediator.Send(new GetTemplateFromPisVisibilityCommand(request.Document.Template, template, false, infoUtente.idGruppo, "D", "", infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF))).output;

                        if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))

                        {

                            var templateRequest = request.Document.Template;
                            aggregate.AddProfile(template.SYSTEM_ID.ToString(), new TextValue(template.DESCRIZIONE));
                            var tempPis = DBUtils.GetDocumentTemplateByIdTemplate(template.SYSTEM_ID.ToString(), infoUtente.idGruppo, _pi3DbContext);



                            var reqFields = templateRequest.Fields != null ? templateRequest.Fields.Select(f => f.Name) : new List<string>();

                            var tempFields = new List<Field>();
                            if (templateRequest.Fields != null)
                            {
                                foreach (var f in templateRequest.Fields)
                                {
                                    var corrFieldInTemp = template.ELENCO_OGGETTI.Where(o => o.DESCRIZIONE == f.Name).FirstOrDefault();
                                    if (corrFieldInTemp == null)
                                    {
                                        continue;
                                    }
                                    var type = tempPis.Fields.Where(fl => fl.Name == f.Name).Select(fl => fl.Type).FirstOrDefault();
                                    if (type == "MultipleChoise")
                                    {
                                        // Normalizzazione multiplechoice 
                                        List<string> values = new();
                                        if (corrFieldInTemp != null)
                                        {
                                            var multipleChoice = corrFieldInTemp.ELENCO_VALORI.Select(o => f.MultipleChoice.Contains(o.VALORE) ? o.VALORE : string.Empty).ToArray();
                                            tempFields.Add(new()
                                            {
                                                Value = f.Value,
                                                Id = f.Id,
                                                MultipleChoice = multipleChoice,
                                                Name = f.Name
                                            });
                                        }
                                        else
                                        {
                                            tempFields.Add(f);
                                        }
                                    }
                                    else
                                    {
                                        tempFields.Add(f);
                                    }
                                }
                            }

                            var fields = tempFields.Union(template.ELENCO_OGGETTI.Where(f => !reqFields.Contains(f.DESCRIZIONE)).Select(o => new Field()
                            {
                                CodeRegisterOrRF = (!string.IsNullOrEmpty(o.ID_AOO_RF) && !o.ID_AOO_RF.Equals("0")) ? DBUtils.getRegistro(o.ID_AOO_RF, this._pi3DbContext).codRegistro : string.Empty,
                                Value = o.VALORE_DATABASE,
                                CounterToTrigger = o.CONTATORE_DA_FAR_SCATTARE,
                                MultipleChoice = o.VALORI_SELEZIONATI,
                                Name = o.DESCRIZIONE

                            }).ToList()).ToList();

                            foreach (var field in fields)
                            {
                                // linearizzo gli oggetti da elenco_oggetti a field 
                                var editabile = (from a in template.ELENCO_OGGETTI
                                                 where a.DESCRIZIONE.ToUpper() == field.Name.ToUpper()
                                                 //&& DBUtils.GetRightsCustomObjectDoc(this._pi3DbContext, template.SYSTEM_ID, infoUtente.idGruppo, a.SYSTEM_ID) == ("INSERT_AND_MODIFY")
                                                 select new Field()
                                                 {
                                                     CodeRegisterOrRF = (!string.IsNullOrEmpty(a.ID_AOO_RF) && !a.ID_AOO_RF.Equals("0")) ? DBUtils.getRegistro(a.ID_AOO_RF, this._pi3DbContext).codRegistro : string.Empty,
                                                     Value = a.VALORE_DATABASE,
                                                     CounterToTrigger = a.CONTATORE_DA_FAR_SCATTARE,
                                                     MultipleChoice = a.VALORI_SELEZIONATI,
                                                     Name = a.DESCRIZIONE,
                                                     Id = a.SYSTEM_ID.ToString(),
                                                     Type = DBUtils.GetFieldTypeFromCustomObjDoc(this._pi3DbContext, a)

                                                 }).FirstOrDefault();
                                string dirittiSulCampo = DBUtils.GetRightsCustomObjectDoc(this._pi3DbContext, template.SYSTEM_ID, infoUtente.idGruppo, editabile.Id.AsLong());


                                if (editabile != null && string.IsNullOrWhiteSpace(editabile.Id) && dirittiSulCampo != "INSERT_AND_MODIFY" &&
                                    (!string.IsNullOrWhiteSpace(field.Value) || field.CounterToTrigger || field.MultipleChoice != null))
                                {
                                    throw new RestException("TEMPLATE_FIELD_NOT_ROLE_EDITABLE");
                                }

                                if (string.Equals(editabile.Name, field.Name, StringComparison.OrdinalIgnoreCase))

                                {
                                    var oggetto = this._pi3DbContext.OggettiCustomEntities.AsNoTracking().Where(a => a.SYSTEM_ID == editabile.Id.AsLong()).Join(
                                        this._pi3DbContext.TipoOggettoEntities.AsNoTracking(),
                                        a => a.ID_TIPO_OGGETTO,
                                        c => c.SYSTEM_ID,
                                        (a, c) => new
                                        {
                                            a.RESET_ANNO,
                                            c.DESCRIZIONE,
                                            a.REPERTORIO
                                        }).FirstOrDefault();


                                    switch (editabile.Type)

                                    {
                                        case "SubCounter":
                                        case "Counter":
                                            DocsPaVO.utente.Registro reg = null;
                                            bool resettaContatoreInizioAnno = false;
                                            if (!string.IsNullOrEmpty(field.CodeRegisterOrRF))
                                            {
                                                reg = DBUtils.getRegistroByCodAOO(field.CodeRegisterOrRF, infoUtente.idAmministrazione, this._pi3DbContext);
                                            }
                                            if (reg == null)
                                            {
                                                throw new RestException("REGISTER_NOT_FOUND");
                                            }
                                            resettaContatoreInizioAnno = oggetto.RESET_ANNO == "SI";

                                            aggregate.AddProfileField(
                                            template.SYSTEM_ID.ToString(),
                                            editabile.Id,
                                            new TextValue(editabile.Name),
                                            oggetto.DESCRIZIONE,
                                            new ContatoreRepertorioFieldValue(reg.systemId,
                                            field.CounterToTrigger,
                                            resettaContatoreInizioAnno));

                                            break;
                                        case "MultipleChoise":
                                            aggregate.AddProfileField(
                                                template.SYSTEM_ID.ToString(),
                                                editabile.Id,
                                                new TextValue(editabile.Name),
                                                oggetto.DESCRIZIONE,
                                                new ElementFieldMultiValue(field.MultipleChoice.Select(s => new TextValue(s)).ToArray()));
                                            break;

                                        default:
                                            aggregate.AddProfileField(
                                            template.SYSTEM_ID.ToString(),
                                            editabile.Id,
                                            new TextValue(editabile.Name),
                                            oggetto.DESCRIZIONE,
                                            new ElementFieldSingleValue(new TextValue(field.Value)));
                                            break;
                                    }
                                }
                            }

                        }

                        var idDiagramma = DBUtils.GetDiagrammaAssociato(template.SYSTEM_ID.ToString(), this._pi3DbContext);
                        if (idDiagramma != 0)
                        {
                            setStatoDiagrammaIniziale = true;
                            diagramma = (await this._mediator.Send(new GetDiagrammaByIdCommand()
                            {
                                IdDiagramma = idDiagramma.ToString()
                            })).Output;

                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count() > 0)
                                {
                                    foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                    {
                                        if (stato.STATO_INIZIALE == true) statoIniziale = stato;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion


                DocsPaVO.utente.Corrispondente mittente = null;
                // scheda documento introdotta solo per reg
                DocsPaVO.documento.SchedaDocumento schedaDoc = new();
                #region Controlli non grigio settaggio mittente
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) &&
                    !request.Document.DocumentType.ToUpper().Equals("G"))
                {
                    if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                    {
                        if (!request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            //Mittente non presente
                            throw new RestException("REQUIRED_SENDER");
                        }
                        else
                        {
                            var mittResp = await this._mediator.Send(new RisolviCorrispondenteCommand()
                            {
                                SearchKey = ruolo.uo.systemId,
                                InfoUtente = infoUtente
                            });
                            mittente = mittResp.Corrispondente;
                        }
                    }
                    else
                    {
                        if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                        {
                            mittente = DBUtils.GetCorrespondentFromPisNewInsertOccasionale(request.Document.Sender, infoUtente, this._pi3DbContext);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                            {
                                var mittResp = await this._mediator.Send(new RisolviCorrispondenteCommand()
                                {
                                    SearchKey = request.Document.Sender.Id,
                                    InfoUtente = infoUtente
                                });
                                mittente = mittResp.Corrispondente;
                                if (mittente == null)
                                    //Mittente non trovato
                                    throw new RestException("SENDER_NOT_FOUND");
                            }
                        }
                    }

                    if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                        {
                            //Destinatario non presente
                            throw new RestException("REQUIRED_RECIPIENT");
                        }
                    }

                    if (string.IsNullOrEmpty(request.CodeRegister))
                    {
                        //Registro mancante
                        throw new RestException("REQUIRED_REGISTER");
                    }
                    else
                    {
                        DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(request.CodeRegister, infoUtente.idAmministrazione, this._pi3DbContext);
                        if (reg == null)
                        {
                            //Registro mancante
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            schedaDoc.registro = reg;
                        }
                    }

                    if (!string.IsNullOrEmpty(request.CodeRF))
                    {
                        DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(request.CodeRF, infoUtente.idAmministrazione, this._pi3DbContext);
                        if (reg != null)
                        {
                            schedaDoc.id_rf_prot = reg.systemId;
                            schedaDoc.id_rf_invio_ricevuta = reg.systemId;
                            schedaDoc.cod_rf_prot = reg.codRegistro;
                        }
                        else
                        {
                            //RF non trovato
                            throw new RestException("RF_NOT_FOUND");
                        }
                    }
                }

                #endregion

                if (request.Document.Predisposed
                    && request.Document.DocumentType.ToUpper() != "G")
                {
                    aggregate.Predisponi((TipologiaFlussoEnum)tipologiaFlusso);
                    predisponi = true;
                }
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) &&
                    !request.Document.DocumentType.ToUpper().Equals("G"))
                {

                    switch (tipologiaFlusso)
                    {
                        case TipologiaFlussoEnum.E:

                            DateTime? arrivalDate = null;
                            if (!string.IsNullOrWhiteSpace(request.Document.ArrivalDate))
                                arrivalDate = this.ParseDate(request.Document.ArrivalDate);
                            if (!request.Document.Predisposed)
                            {
                                aggregate.AssignProtocolloMittente(new ProtocolloMittente()
                                {
                                    Data = !string.IsNullOrEmpty(request.Document.DataProtocolSender) ? request.Document.DataProtocolSender.Trim().AsDateTime() : null,
                                    DataArrivo = !string.IsNullOrEmpty(request.Document.ArrivalDate) ? arrivalDate : null,
                                    Segnatura = !string.IsNullOrEmpty(request.Document.ProtocolSender) ? request.Document.ProtocolSender : null,
                                });
                            }
                            else
                            {
                                aggregate.AssignProtocolloMittente(new ProtocolloMittente()
                                {
                                    DataArrivo = !string.IsNullOrEmpty(request.Document.ArrivalDate) ? arrivalDate : null
                                });
                            }

                            if (!string.IsNullOrEmpty(request.Document.MeansOfSending))
                            {
                                var listaMezziSpedizione = DBUtils.ListaMezziSpedizione(infoUtente.idAmministrazione, true, this._pi3DbContext);
                                foreach (var mezzo in listaMezziSpedizione)
                                {
                                    if (mezzo.Descrizione.ToUpper().Equals(request.Document.MeansOfSending.ToUpper()))
                                    {
                                        aggregate.AssignMezzoSpedizione(mezzo.IDSystem, new Core.SeedWork.TextValue(mezzo.Descrizione!));
                                    }
                                }
                            }
                            aggregate.AssignMittente(new Mittente(new PG()
                            {
                                DenominazioneUfficio = new TextValue(mittente.descrizione),
                            },
                                mittente.systemId));

                            if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                            {
                                foreach (var mm in request.Document.MultipleSenders)
                                {
                                    aggregate.AddMittenteMultiplo(new Mittente(
                                        new PG()
                                        {
                                            DenominazioneUfficio = new TextValue(mm.Description)
                                        },
                                        mm.Id));
                                }
                            }


                            break;
                        case TipologiaFlussoEnum.U:
                        case TipologiaFlussoEnum.I:

                            if (mittente != null)
                            {
                                aggregate.AssignMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(mittente.descrizione),
                                },
                                    mittente.systemId));
                            }
                            if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                            {
                                foreach (Correspondent corrTemp in request.Document.Recipients)
                                {
                                    DocsPaVO.utente.Corrispondente corrBySys = null;

                                    // Verifica se occasionale
                                    if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                    {
                                        corrBySys = DBUtils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente, this._pi3DbContext);
                                    }
                                    else
                                    {
                                        var recResp = await this._mediator.Send(new RisolviCorrispondenteCommand()
                                        {
                                            SearchKey = corrTemp.Id,
                                            InfoUtente = infoUtente
                                        });
                                        corrBySys = recResp.Corrispondente;

                                        if (corrBySys == null)
                                        {
                                            throw new RestException("RECIPIENT_NOT_FOUND");
                                        }

                                    }

                                    // TO DO tipoCorrispondente F
                                    if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                    {
                                        var corrResp = await this._mediator.Send(new GetCorrispondentiByCodListaCommand()
                                        {
                                            CodiceLista = corrBySys.codiceRubrica,
                                            InfoUtente = infoUtente,

                                        });
                                        var corrByLista = corrResp.Corrispondenti;

                                        foreach (DocsPaVO.utente.Corrispondente d in corrByLista)
                                        {
                                            aggregate.AddDestinatario(new Destinatario(
                                               new PG()
                                               {
                                                   DenominazioneUfficio = new TextValue(d.descrizione),
                                                   IndirizziDigitaliDiRiferimento = new List<string>() { d.email }
                                               },
                                               d.systemId)
                                            { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
                                        }
                                    }
                                    else
                                    {
                                        aggregate.AddDestinatario(new Destinatario(
                                               new PG()
                                               {
                                                   DenominazioneUfficio = new TextValue(corrBySys.descrizione),
                                                   IndirizziDigitaliDiRiferimento = new List<string>() { corrBySys.email }
                                               },
                                               corrBySys.systemId)
                                        { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
                                    }

                                }
                            }
                            if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                            {
                                foreach (Correspondent corrTemp in request.Document.RecipientsCC)
                                {
                                    DocsPaVO.utente.Corrispondente corrBySys = null;

                                    // Verifica se occasionale
                                    if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                                    {
                                        corrBySys = DBUtils.GetCorrespondentFromPisNewInsertOccasionale(corrTemp, infoUtente, this._pi3DbContext);
                                    }
                                    else
                                    {
                                        var recResp = await this._mediator.Send(new RisolviCorrispondenteCommand()
                                        {
                                            SearchKey = corrTemp.Id,
                                            InfoUtente = infoUtente
                                        });
                                        corrBySys = recResp.Corrispondente;

                                        if (corrBySys == null)
                                        {
                                            throw new RestException("RECIPIENT_NOT_FOUND");
                                        }

                                    }

                                    // TO DO tipoCorrispondente F
                                    if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                                    {
                                        var corrResp = await this._mediator.Send(new GetCorrispondentiByCodListaCommand()
                                        {
                                            CodiceLista = corrBySys.codiceRubrica,
                                            InfoUtente = infoUtente,

                                        });
                                        var corrByLista = corrResp.Corrispondenti;

                                        foreach (DocsPaVO.utente.Corrispondente d in corrByLista)
                                        {
                                            aggregate.AddDestinatarioCc(new Destinatario(
                                               new PG()
                                               {
                                                   DenominazioneUfficio = new TextValue(d.descrizione),
                                                   IndirizziDigitaliDiRiferimento = new List<string>() { d.email }
                                               },
                                               d.systemId)
                                            { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
                                        }
                                    }
                                    else
                                    {
                                        aggregate.AddDestinatarioCc(new Destinatario(
                                               new PG()
                                               {
                                                   DenominazioneUfficio = new TextValue(corrBySys.descrizione),
                                                   IndirizziDigitaliDiRiferimento = new List<string>() { corrBySys.email }
                                               },
                                               corrBySys.systemId)
                                        { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
                                    }

                                }
                            }

                            break;
                    }

                    if (!request.Document.Predisposed)
                    {
                        aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
                        {
                            DatiRegistro = new DatiRegistro() { IdRegistro = !string.IsNullOrEmpty(schedaDoc.id_rf_prot) ? schedaDoc.id_rf_prot : schedaDoc.registro.systemId }
                        });
                    }
                }

                #region Gestione linked docs
                if (request.Document.ParentDocument != null)
                {
                    aggregate.AddRelatedElement(request.Document.ParentDocument.Id);
                }
                if (request.Document.LinkedDocuments != null && request.Document.LinkedDocuments.Length > 0)
                {
                    foreach (LinkedDocument ld in request.Document.LinkedDocuments)
                    {
                        if (ld.LinkType == "PARENT")
                        {
                            aggregate.AddRelatedElement(ld.Id);
                        }
                        else if (ld.LinkType == "LINKED")
                        {
                            aggregate.AddRelatedElement(ld.Id, false);
                        }
                    }
                }
                #endregion

                await _documentoAmministrativo.Add(aggregate);
                #region Aggiunta note
                if (aggregate != null && !string.IsNullOrWhiteSpace(aggregate.Id) && request != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                {
                    foreach (Note nota in request.Document.Note)
                    {
                        var aggregateNota = new Pi3.Core.AggregateModels.NotaAggregate.Nota(idTenant, DateTime.Now, new TextValue(nota.Description), null,
                        new AutoreNota()
                        {
                            IdUtente = infoUtente.idPeople,
                            IdRuolo = infoUtente.idGruppo,
                        },
                        aggregate.Id.ToString(),
                        TipiOggettoEnum.Documento,
                        TipoAccessoNotaEnum.Pubblica,
                        null
                        );

                        await this._notaRepository.Add(aggregateNota);
                    }
                }
                #endregion



                if (aggregate != null && !string.IsNullOrWhiteSpace(aggregate.Id))
                {
                    if (setStatoDiagrammaIniziale)
                    {
                        await this._mediator.Send(new SalvaModificaStatoCommand()
                        {
                            DocNumber = aggregate.Id,
                            IdStato = statoIniziale.SYSTEM_ID.ToString(),
                            Diagramma = diagramma,
                            IdUtente = infoUtente.idPeople,
                            User = infoUtente,
                            dataScadenza = string.Empty
                        });
                    }

                    var aggregateResponse = await _documentoAmministrativo.Get(idTenant, aggregate.Id, new Core.SeedWork.ILoadBehavior[1]
                   {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadMittentiDestinatari = true,
                            LoadNote = true,
                            LoadProfilesMetadata = true,
                            LoadProfiles = true
                        }
                   });

                    response.Document = RestUtils.getDocFromAggregate(aggregateResponse, _pi3DbContext);
                    if (daRepertoriare && idOggettoRepertorio != 0)
                    {
                        var segnaturaRepertorio = await this._pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Where(a => a.DOC_NUMBER == aggregate.Id && a.ID_TEMPLATE == idTipoAtto && a.ID_OGGETTO == idOggettoRepertorio)
                        .Select(a => a.VAR_SEGNATURA)
                        .FirstAsync();

                        await this._loggerService.LogOK("DOCUMENTO_REPERTORIATO", aggregate.Id,
                        string.Format(Resources.LogRepertoriatoDocumento, segnaturaRepertorio), null, infoUtente.codWorkingApplication);
                    }
                    await _loggerService.LogOK("DOCUMENTOADDDOCGRIGIA",
                    aggregate.Id, string.Format("PIS REST: Creazione del documento ID: {0}, Tipo documento {1}, Predisposto {2}", aggregate.Id, response.Document.DocumentType, predisponi.ToString()),
                    null, infoUtente.codWorkingApplication);
                }
                response.Document.ParentDocument = request.Document.ParentDocument;
                #endregion
                #region Upload file principale
                if (response.Document != null && !string.IsNullOrWhiteSpace(response.Document.Id) && request.Document.MainDocument != null && request.Document.MainDocument.Content != null)
                {
                    try
                    {
                        TipoFirmaEnum tipoFirma = await GetTipoFirmaFile(request.Document.MainDocument);

                        var fileAggregate = new DocumentBlob(infoUtente.idAmministrazione, DateTime.Now, new Core.SeedWork.TextValue(request.Document.MainDocument.Name));

                        fileAggregate.UploadStream(new MemoryStream(request.Document.MainDocument.Content), request.Document.MainDocument.Name);
                        fileAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);
                        await _blobRepository.Add(fileAggregate);
                        var versioneAttuale = (from a in _pi3DbContext.ComponentEntities where a.DOCNUMBER == aggregate.Id.AsLong() orderby a.VERSION_ID descending select a).FirstOrDefault();
                        aggregate.AssignDocumentBlobRef(new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                        {
                            IdBlob = fileAggregate.Id,
                            Hash = fileAggregate.Hash,
                            HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                            CreationDate = fileAggregate.CreationDate,
                            ContentType = fileAggregate.ContentType,
                            FileSize = fileAggregate.FileSize,
                            FileName = fileAggregate.FileName,
                            TipoFirma = tipoFirma
                        }, new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior() { CreateNewVersion = false, Name = new Core.SeedWork.TextValue("documento principale"), IdVersion = versioneAttuale?.VERSION_ID.ToString() });

                        await _documentoAmministrativo.Update(aggregate);
                        await _loggerService.LogOK("DOCUMENTOPUTFILE",
                        aggregate.Id, $"PIS REST:Acquisito documento per l'id {aggregate.Id} tramite PIS",
                        null, infoUtente.codWorkingApplication);


                    }
                    catch (Exception exfile)
                    {
                        throw new RestException("Errore creazione file", exfile.ToString());
                    }
                }
                #endregion
                #region Aggiunta in fascicolo
                string idfascicolo = "";

                long idSottoFascicolo = 0;

                long idfascPrincipale;

                string idTitAttivo = null;

                var titolari = DBUtils.getTitolariUtilizzabili(infoUtente.idAmministrazione, _pi3DbContext);
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            idTitAttivo = tempTit.ID;
                            break;
                        }
                    }
                }

                long idCartellaPrincipale = 0;

                AggregazioneDocumentale? fascicolo = null;

                if (!string.IsNullOrWhiteSpace(request.IdProject))
                {
                    var rigaFasc = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.SYSTEM_ID == request.IdProject.AsLong() select a).FirstOrDefault();
                    if (rigaFasc == null || rigaFasc.SYSTEM_ID < 1) throw new RestException("PROJECT_NOT_FOUND");
                    var idfasctemp = rigaFasc.SYSTEM_ID;
                    if (rigaFasc.CHA_TIPO_PROJ == "T")
                    {
                        idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.ID_PARENT == rigaFasc.SYSTEM_ID select a.SYSTEM_ID).FirstOrDefault();
                    }

                    //var idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.SYSTEM_ID == request.IdProject.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                    idfascicolo = idfasctemp.ToString();
                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");

                    idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();




                    fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString());
                }
                else if (request.CodeProject.IndexOf("//") > -1)
                {
                    string separatore = "//";
                    // MODIFICA per inserimento in sottocartelle
                    string[] separatoreAr = new string[] { separatore };

                    string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                    var rigaFasc = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.SYSTEM_ID == request.IdProject.AsLong() select a).FirstOrDefault();
                    if (rigaFasc == null || rigaFasc.SYSTEM_ID < 1) throw new RestException("PROJECT_NOT_FOUND");
                    //idfascPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == pathCartelle[0] && a.ID_TITOLARIO == idTitAttivo.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                    idfascPrincipale = rigaFasc.SYSTEM_ID;
                    if (rigaFasc.CHA_TIPO_PROJ == "T")
                    {
                        idfascPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.ID_PARENT == rigaFasc.SYSTEM_ID select a.SYSTEM_ID).FirstOrDefault();
                    }

                    idfascicolo = idfascPrincipale.ToString();

                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");

                    var idCartellaPrincipale2 = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();


                    var CartellaPrincipale2 = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale2.ToString(), new ILoadBehavior[1] { new GetAggregatoDocumentaleLoadBehavior() { BypassSecurityCheck = true, LoadFolderHierarchy = true } });

                    if (pathCartelle.Length > 1)
                    {
                        var sottocartelle = await (from a in _pi3DbContext.ProjectEntities
                                                   where a.ID_FASCICOLO == idfascPrincipale
                                                   select
                                                   new Folder()
                                                   {
                                                       Id = a.SYSTEM_ID.ToString(),
                                                       Description = a.DESCRIPTION,
                                                       IdParent = a.ID_PARENT.ToString(),
                                                       IdProject = a.ID_FASCICOLO.ToString()


                                                   }).ToListAsync();
                        idSottoFascicolo = CartellaPrincipale2.Id.AsLong();
                        try
                        {
                            for (int i = 1; i < pathCartelle.Length; i++)
                            {

                                var folderSelezionato = ((from f in sottocartelle where f.IdParent == idSottoFascicolo.ToString() && f.Description == pathCartelle[i] select f).ToList().First());
                                idSottoFascicolo = folderSelezionato.Id.AsLong();
                            }

                        }
                        catch (Exception e)
                        {
                            throw new Exception("Cartella non trovata nel fascicolo");
                        }


                        fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale2.ToString(), new ILoadBehavior[1] {
                                new GetAggregatoDocumentaleLoadBehavior()
                                {
                                    BypassSecurityCheck = true,
                                    LoadFolderHierarchy = true,
                                }
                            });

                    }
                    else
                    {

                        var idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == request.CodeProject && a.ID_TITOLARIO == idTitAttivo.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                        idfascicolo = idfasctemp.ToString();
                        if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");


                        idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();

                        fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString());
                    }

                }
                else
                {
                    //var idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == request.CodeProject && a.ID_TITOLARIO == idTitAttivo.AsLong() select a.SYSTEM_ID).FirstOrDefault();
                    var rigaFasc = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.VAR_CODICE == request.CodeProject && a.ID_TITOLARIO == idTitAttivo.AsLong() select a).FirstOrDefault();
                    if (rigaFasc == null || rigaFasc.SYSTEM_ID < 1) throw new RestException("PROJECT_NOT_FOUND");
                    var idfasctemp = rigaFasc.SYSTEM_ID;
                    if (rigaFasc.CHA_TIPO_PROJ == "T")
                    {
                        idfasctemp = (from a in _pi3DbContext.ProjectEntities where a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.CHA_TIPO_FASCICOLO == "G" && a.ID_PARENT == rigaFasc.SYSTEM_ID select a.SYSTEM_ID).FirstOrDefault();
                    }

                    idfascicolo = idfasctemp.ToString();
                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");


                    idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();

                    fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString());
                }


                if (fascicolo == null) throw new RestException("PROJECT_NOT_FOUND");
                // Controllo visibilit� fascicolo
                try
                {
                    await _pi3DbContext.AssertSecurityRights(idfascicolo, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }


                if (idSottoFascicolo > 0)
                {
                    fascicolo.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = aggregate.Id.ToString() }, idSottoFascicolo.ToString());
                }
                else
                {
                    fascicolo.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = aggregate.Id.ToString() });

                }
                await _aggDocRepository.Update(fascicolo);
                await _loggerService.LogOK("DOCADDINFASC",
                        aggregate.Id.ToString(), $"PIS REST: Documento {aggregate.Id.ToString()} inserito nel fascicolo {fascicolo.DatiRegistrazione.Codice}",
                        null, infoUtente.codWorkingApplication);

                #endregion

                #endregion

                //response.Document.Template = DBUtils.GetDocumentTemplateByIdTemplate(request.Document.Template != null ? request.Document.Template.Id : "0",infoUtente.idGruppo,this._pi3DbContext);
                if (template != null && template.SYSTEM_ID != null)
                    response.Document.Template = DBUtils.GetDocumentTemplateByIdTemplate(template.SYSTEM_ID.ToString(), infoUtente.idGruppo, this._pi3DbContext);

                if (setStatoDiagrammaIniziale)
                {
                    response.Document.Template.StateDiagram.StateOfDiagram[0] = DBUtils.GetStateOfDiagram(statoIniziale, diagramma.SYSTEM_ID.ToString());
                }
                response.Code = CreateDocumentResponseCode.OK;

                _logger.LogInformation("end CreateDocumentAndAddInProject");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione CreateDocumentAndAddInProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateDocumentAndAddInProjectCommandResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione CreateDocumentAndAddInProject");
                response = new CreateDocumentAndAddInProjectCommandResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }


        #endregion

        #region Private Members
        protected readonly INotaRepository _notaRepository;
        protected readonly ILogger<CreateDocumentAndAddInProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativo;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;
        protected readonly IDocumentBlobRepository _blobRepository;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ICAdESService _cAdESService;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;


        private async Task<List<DocsPaVO.utente.Registro>> GetListaRegistriAttiviByRuolo(long idRuolo)
        {
            List<DocsPaVO.utente.Registro> registers = new List<DocsPaVO.utente.Registro>();
            var query = this._pi3DbContext.RegistroEntities.
                    Join(
                        this._pi3DbContext.RuoloRegistroEntities,
                        r => r.SYSTEM_ID,
                        rr => rr.ID_REGISTRO,
                        (r, rr) => new
                        {
                            registro = r,
                            ruolo = rr
                        })
                    .Join(this._pi3DbContext.AmministraEntities, g => g.registro.ID_AMM, a => a.SYSTEM_ID, (g, a) => new
                    {
                        g.registro,
                        g.ruolo,
                        amministra = a
                    })
                    .Where(w => "0".Equals(w.registro.CHA_RF) && w.ruolo.ID_RUOLO_IN_UO == idRuolo && !string.IsNullOrEmpty(w.registro.CHA_STATO) && w.registro.CHA_STATO.Equals("A"))
                    .OrderBy(j => j.registro.CHA_STATO)
                    .ThenByDescending(j => j.ruolo.CHA_PREFERITO)
                    .ThenBy(o => o.registro.VAR_PREG != null)
                    .ThenBy(o => o.registro.VAR_CODICE)
                    .ThenBy(o => o.registro.VAR_DESC_REGISTRO)
                    .Select(s => new
                    {
                        s.registro.SYSTEM_ID,
                        s.registro.VAR_CODICE,
                        s.registro.NUM_RIF,
                        s.registro.VAR_DESC_REGISTRO,
                        s.registro.VAR_EMAIL_REGISTRO,
                        s.registro.CHA_STATO,
                        s.registro.ID_AMM,
                        s.amministra.VAR_CODICE_AMM,
                        s.registro.DIRITTO_RUOLO_AOO,
                        s.registro.INVIO_RICEVUTA_MANUALE,
                        s.registro.FLAG_WSPIA,
                        s.registro.DTA_OPEN,
                        s.registro.DTA_CLOSE,
                        s.registro.DTA_ULTIMO_PROTO,
                        s.registro.ID_RUOLO_AOO,
                        s.registro.ID_RUOLO_RESP,
                        s.registro.ID_PEOPLE_AOO,
                        s.registro.CHA_AUTO_INTEROP,
                        s.registro.VAR_PREG,
                        s.registro.ANNO_PREG,
                        s.registro.VAR_CODICE_IPA
                    });

            var elementi = query.ToList();
            foreach (var item in elementi)
            {
                DocsPaVO.utente.Registro reg = new()
                {
                    systemId = item.SYSTEM_ID.ToString(),
                    codRegistro = item.VAR_CODICE,
                    codice = item.NUM_RIF?.ToString(),
                    descrizione = item.VAR_DESC_REGISTRO,
                    email = item.VAR_EMAIL_REGISTRO,
                    stato = item.CHA_STATO,
                    idAmministrazione = item.ID_AMM?.ToString(),
                    codAmministrazione = item.VAR_CODICE_AMM?.ToString(),
                    dataApertura = item.DTA_OPEN.HasValue ? item.DTA_OPEN.Value.ToString("dd/MM/yyyy") : String.Empty,
                    dataChiusura = item.DTA_CLOSE.HasValue ? item.DTA_CLOSE.Value.ToString("dd/MM/yyyy") : String.Empty,
                    dataUltimoProtocollo = item.DTA_ULTIMO_PROTO.HasValue ? item.DTA_ULTIMO_PROTO.Value.ToString("dd/MM/yyyy") : String.Empty,
                    idRuoloAOO = item.ID_RUOLO_AOO?.ToString(),
                    idRuoloResp = item.ID_RUOLO_RESP?.ToString(),
                    idUtenteAOO = item.ID_PEOPLE_AOO?.ToString(),
                    autoInterop = item.CHA_AUTO_INTEROP,
                    Diritto_Ruolo_AOO = item.DIRITTO_RUOLO_AOO.ToString(),
                    invioRicevutaManuale = item.INVIO_RICEVUTA_MANUALE?.ToString() ?? "0",
                    FlagWspia = item.FLAG_WSPIA?.ToString() ?? "0",
                    flag_pregresso = "1".Equals(item.VAR_PREG),
                    anno_pregresso = String.IsNullOrEmpty(item.VAR_PREG) ? null : item.ANNO_PREG?.ToString(),
                    codiceIpa = item.VAR_CODICE_IPA
                };
                registers.Add(reg);
            }

            return registers;
        }
        protected DateTime? ParseDate(string arrivalDate)
        {
            DateTime dateVal;

            // Pattern di validit� per una data valida
            string pattern = "dd/MM/yyyy HH:mm:ss";
            string pattern2 = "dd/MM/yyyy HH:mm";

            try
            {
                if (!DateTime.TryParseExact(arrivalDate, pattern, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateVal))
                {
                    if (!DateTime.TryParseExact(arrivalDate, pattern2, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateVal))
                    {
                        throw new Exception(Resources.InvalidFormatDtError);
                    }
                }
            }
            catch
            {
                throw new Exception(Resources.InvalidFormatDtError);
            }

            return dateVal;
        }

        private async Task<TipoFirmaEnum> GetTipoFirmaFile(File fileDoc)
        {
            TipoFirmaEnum tipoFirmaEnum = TipoFirmaEnum.Nessuna;

            if (fileDoc.Name.ToUpper().EndsWith("P7M"))
            {
                tipoFirmaEnum = TipoFirmaEnum.Cades;
            }
            if (fileDoc.Name.ToUpper().EndsWith("TSD"))
            {
                tipoFirmaEnum = TipoFirmaEnum.Tsd;
            }
            if (fileDoc.Name.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(fileDoc.Content)))
            {
                tipoFirmaEnum = TipoFirmaEnum.Pades;
            }
            if (fileDoc.Name.ToUpper().EndsWith("XML") && await IsSignedXades(fileDoc))
            {
                tipoFirmaEnum = TipoFirmaEnum.Xades;
            }

            return tipoFirmaEnum;
        }
        protected virtual async Task<bool> IsSignedXades(File fileDoc)
        {
            bool result = false;
            XmlDocument Xmlfile = new XmlDocument();
            XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.Content));
            tr.XmlResolver = null;
            try
            {
                Xmlfile.Load(tr);
                XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
                if (signature != null && signature.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Errore nel metodo IsSignedXades " + e.Message);
                result = false;
            }
            finally
            {
                tr.Close();
            }

            return result;
        }

        #endregion
    }

}