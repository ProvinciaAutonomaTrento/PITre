// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFolder;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliDaCodice;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Repository;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithFromPrevious
{
    public class CreateDocumentWithFromPreviousCommandHandler : IRequestHandler<CreateDocumentWithFromPreviousCommand, CreateDocumentWithFromPreviousCommandResponse>
    {
        public CreateDocumentWithFromPreviousCommandHandler(
            INotaRepository notaRepository,
            IAggregazioneDocumentaleRepository _aggDocRepository,
            IConfigurationService configurationService, ILogger<CreateDocumentWithFromPreviousCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor,
            IPi3DbContext pi3DbContext, IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            ICAdESService cAdESService,
            IPAdESService pAdESService,
            IWebMethodLoggerService loggerService, IDocumentBlobRepository blobRepository,
            IFileValidatorService fileValidatorService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._documentoAmministrativo = documentoAmministrativoRepository;
            this._loggerService = loggerService;
            this._blobRepository = blobRepository;
            this._notaRepository = notaRepository;
            this._configurationService = configurationService;
            this._aggDocRepository = _aggDocRepository;
            this._cAdESService = cAdESService;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<CreateDocumentWithFromPreviousCommandResponse> Handle(CreateDocumentWithFromPreviousCommand request, CancellationToken cancellationToken)
        {

            CreateDocumentWithFromPreviousCommandResponse response = new();
            try
            {
                #region token e autenticazione
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request.FromPregressi)
                {
                    (var key, var keyFound) = await this._configurationService.TryGetValue<string>(infoUtente.idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");

                    if (!string.IsNullOrEmpty(key) && "TRUE".Equals(key))
                    {
                        throw new Exception(Resources.ClassRequired);
                    }
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
                #endregion

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


                #region implementazione

                #region Creazione Documento
                bool predisponi = false;
                if (request.Document.Predisposed
                    && request.Document.DocumentType.ToUpper() != "G")
                {
                    predisponi = true;
                }
                bool daRepertoriare = false;
                var setStatoDiagrammaIniziale = false;
                //TODO: gestione protocolli, predisposti, tipologie, note, file
                bool privato = request != null && request.Document != null && request.Document.PrivateDocument;
                bool personale = request != null && request.Document != null && request.Document.PersonalDocument;
                DocsPaVO.utente.Registro registro = null;
                var tipologiaFlusso = RestUtils.AsTipologiaFlusso(request.Document.DocumentType);
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



                #region PROFILAZIONE DIN
                long idOggettoRepertorio = 0;
                long idTipoAtto = 0;
                DocsPaVO.ProfilazioneDinamica.Templates? template = null;
                if (request != null && request.Document != null && request.Document.Template != null &&
                    (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                {
                    if (!string.IsNullOrEmpty(request.Document.Template.Id))
                    {
                        template = DBUtils.GetTemplateById(request.Document.Template.Id, _pi3DbContext);
                    }
                    else if (!string.IsNullOrEmpty(request.Document.Template.Name))
                    {
                        //template = DBUtils.GetTemplateByDescrizione(request.Document.Template.Name, request.Document.Template.Id, _pi3DbContext);
                        template = DBUtils.GetTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione, _pi3DbContext);
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
                                            if (reg == null && string.IsNullOrEmpty(field.CodeRegisterOrRF) && field.CounterToTrigger)
                                            {
                                                throw new RestException("REGISTER_NOT_FOUND");
                                            }
                                            resettaContatoreInizioAnno = oggetto.RESET_ANNO == "SI";

                                            aggregate.AddProfileField(
                                            template.SYSTEM_ID.ToString(),
                                            editabile.Id,
                                            new TextValue(editabile.Name),
                                            oggetto.DESCRIZIONE,
                                            new ContatoreRepertorioFieldValue(
                                                reg != null ? reg.systemId : "",
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
                                IndirizziDigitaliDiRiferimento = new List<string>() { mittente.email }
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
                                               d.systemId) { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
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
                                               corrBySys.systemId) { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
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
                                               d.systemId) { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
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
                                               corrBySys.systemId) { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
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

                //if (request.Document.ArrivalDate is not null)
                //{
                //    var docNum = aggregate.Id.AsLong(); //id del documento da aggiungere in profile
                //    var profileToUpdate = _pi3DbContext.ProfileEntities.SingleOrDefault(p => p.DOCNUMBER == docNum);
                //    if (profileToUpdate is not null)
                //    {
                //        var versionForUpdate = _pi3DbContext.VersionEntities.SingleOrDefault(v => v.DOCNUMBER == docNum);
                //        if (versionForUpdate is not null)
                //        {
                //            profileToUpdate.DTA_PROTO_IN = versionForUpdate.DTA_ARRIVO;
                //            await ((DbContext)this._pi3DbContext).SaveChangesAsync();
                //        }
                //    }
                //}

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



                #endregion


                if (aggregate != null && !string.IsNullOrWhiteSpace(aggregate.Id))
                {
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
                    var note = DBUtils.getNoteOggetto(aggregate.Id, _pi3DbContext);
                    if (note != null)
                    {
                        response.Document.Note = note.ToArray();
                    }
                    response.Document.Template = DBUtils.getTemplateFromDocumentId(aggregate.Id, _pi3DbContext);
                    response.Document.ParentDocument = await DBUtils.getParentDocInfoFromDocId(aggregate.Id, _pi3DbContext);
                    var childDocs = await DBUtils.getChildDocsInfoFromDocId(aggregate.Id, _pi3DbContext);
                    if (childDocs != null && childDocs.Any())
                        response.Document.LinkedDocuments = childDocs.ToArray();

                    await _loggerService.LogOK("DOCUMENTOADDDOCGRIGIA",
                        aggregate.Id, string.Format("PIS REST: Creazione del documento ID: {0}, Tipo documento {1}, Predisposto {2}", aggregate.Id, response.Document.DocumentType, predisponi.ToString()),
                        null, infoUtente.codWorkingApplication);
                    if (daRepertoriare && idOggettoRepertorio != 0)
                    {
                        var segnaturaRepertorio = await this._pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Where(a => a.DOC_NUMBER == aggregate.Id && a.ID_TEMPLATE == idTipoAtto && a.ID_OGGETTO == idOggettoRepertorio)
                        .Select(a => a.VAR_SEGNATURA)
                        .FirstAsync();

                        await this._loggerService.LogOK("DOCUMENTO_REPERTORIATO", aggregate.Id,
                        string.Format(Resources.LogRepertoriatoDocumento, segnaturaRepertorio), null, infoUtente.codWorkingApplication);
                    }
                }

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

                (string? valPiano, bool found) = await this._configurationService.TryGetValue<string>(infoUtente.idAmministrazione, "ENABLE_PIANO_CONSERVAZIONE");

                if ("1".Equals(valPiano) && request.Document.Template != null && !string.IsNullOrEmpty(request.Document.Template.Id))
                {
                    try
                    {
                        var filters = new Dictionary<string, string>();
                        filters.Add("DOCUMENT_TEMPLATE_ID", template.SYSTEM_ID.ToString());
                        var risultatiRic = DBUtils.SearchArchivePlansRest(filters, "0", "20", this._pi3DbContext);
                        if (risultatiRic != null && risultatiRic.Count > 0)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                            var respFasc = (await this._mediator.Send(new FascicolazioneGetListaFascicoliDaCodiceCommand()
                            {
                                InfoUtente = infoUtente,
                                CodiceFascicolo = risultatiRic.First().CodiceClassificazione,
                                Registro = null,
                                EnableProfilazione = false,
                                EnableUffRef = false,
                                InsRic = "I"
                            }));

                            if (respFasc != null && respFasc.Output != null && respFasc.Output.Count() > 0)
                            {
                                if (respFasc.Output.Count() == 1)
                                {
                                    AggregazioneDocumentale? fascDaRep = null;
                                    try
                                    {
                                        fascicolo = (DocsPaVO.fascicolazione.Fascicolo)respFasc.Output.First();
                                        DocsPaVO.fascicolazione.Folder folder = (await this._mediator.Send(new FascicolazioneGetFolderCommand()
                                        {
                                            IdGruppo = infoUtente.idGruppo,
                                            IdPeople = infoUtente.idPeople,
                                            Fascicolo = new()
                                            {
                                                systemID = fascicolo.systemID
                                            }
                                        })).Output;
                                        bool isSottofascicolo = folder.idParent != folder.idFascicolo;

                                        fascDaRep = await _aggDocRepository.Get(infoUtente.idAmministrazione, folder.systemID);

                                        if (fascDaRep == null) throw new RestException("PROJECT_NOT_FOUND");

                                        if (isSottofascicolo)
                                        {
                                            fascDaRep.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = aggregate.Id }, folder.systemID);
                                        }
                                        else
                                        {
                                            fascDaRep.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = aggregate.Id });
                                        }


                                        await _aggDocRepository.Update(fascDaRep);

                                        await _loggerService.LogKO("DOCADDINFASC",
                                        aggregate.Id, $"PIS REST: Documento {aggregate.Id} inserito nel fascicolo {fascDaRep.DatiRegistrazione?.Codice}",
                                        null, infoUtente.codWorkingApplication);
                                    }
                                    catch (Exception ex)
                                    {
                                        await _loggerService.LogKO("DOCADDINFASC",
                                        aggregate.Id, $"PIS REST: Documento {aggregate.Id} inserito nel fascicolo {fascicolo.codice}",
                                        null, infoUtente.codWorkingApplication);
                                    }

                                }
                                else
                                {
                                    //Fascicoli multipli
                                    throw new RestException("MULTIPLE");
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(exception: ex, message: string.Format(Resources.ConsError, ex));
                    }
                }
                #endregion

                response.Code = CreateDocumentWithFromPreviousResponseCode.OK;



            }
            catch (RestException pisEx)
            {
                response = new();
                response.Code = CreateDocumentWithFromPreviousResponseCode.PIS_ERROR;
                response.ErrorMessage = pisEx.Description;
                response.Ex = pisEx;
            }
            catch (Exception e)
            {
                response = new();
                response.Code = CreateDocumentWithFromPreviousResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;
                response.Ex = e;

            }

            return response;
        }




        #region Private Members
        protected readonly INotaRepository _notaRepository;
        protected readonly ILogger<CreateDocumentWithFromPreviousCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativo;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IDocumentBlobRepository _blobRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;
        protected readonly ICAdESService _cAdESService;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;

        protected DateTime? ParseDate(string arrivalDate)
        {
            DateTime dateVal;

            // Pattern di validità per una data valida
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
