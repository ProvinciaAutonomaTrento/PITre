// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using StackExchange.Redis;
using System.Globalization;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocument
{
    // Richiede libreria MediatR
    public class EditDocumentCommandHandler : IRequestHandler<EditDocumentCommand, EditDocumentCommandResponse>
    {
        #region Public Members

        public EditDocumentCommandHandler(ILogger<EditDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IDocumentoAmministrativoRepository daRepository, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._darepository = daRepository;
            this._loggerService = loggerService;
        }

        public async Task<EditDocumentCommandResponse> Handle(EditDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditDocument - START");

            EditDocumentCommandResponse response = new EditDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request.Document == null || string.IsNullOrEmpty(request.Document.Id))
                {
                    throw new RestException("REQUIRED_DOCUMENT");
                }
                #endregion

                #region implementazione
                // Controllo visibilit� documento
                try
                {
                    await _pi3DbContext.AssertSecurityRights(request.Document.Id, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                //TODO
                var aggregate = await _darepository.Get(infoUtente.idAmministrazione, request.Document.Id,
                    new ILoadBehavior[1]{ new Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories.GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadMittentiDestinatari = true,
                    LoadNote =true,
                    LoadProfilesMetadata =true,
                    LoadProfiles = true
                }}) ?? throw new RestException("DOCUMENT_NOT_FOUND");

                if (aggregate.InRecycleBin) throw new RestException("DOCUMENT_NOT_FOUND");

                //modifica oggetto del documento
                if (!string.IsNullOrWhiteSpace(request.Document.Object) && (aggregate.OggettoDelDocumento.Descrizione.Value != request.Document.Object))
                {
                    aggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                    {
                        Descrizione = new TextValue(request.Document.Object),
                        Id = null
                    });
                }


                //VerificaETrasformaDestinatari(aggregate, request);

                string segnaturaRepertorio = string.Empty;
                long? idOggetto = default;



                if (request is not null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                {
                    var template = new Template();

                    string idTemplate = string.Empty;

                    if (request.Document.Template.Id != null)
                    {
                        idTemplate = request.Document.Template.Id;
                    }
                    else if (request.Document.Template.Name != null)
                    {
                        idTemplate = (from a in _pi3DbContext.TipoAttoEntities where a.VAR_DESC_ATTO.ToUpper() == request.Document.Template.Name.ToUpper() && a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.ABILITATO_SI_NO == 1 select a.SYSTEM_ID).FirstOrDefault().ToString();
                    }
                    if (string.IsNullOrWhiteSpace(idTemplate) || idTemplate == "0") throw new RestException("TEMPLATE_NOT_FOUND");

                    if (aggregate.Profiles == null || aggregate.Profiles[0] is null || !aggregate.Profiles[0].Id.Equals(idTemplate))
                    {
                        throw new RestException("TEMPLATE_NOT_VALID");
                    }
                    //TODO: cambiato la logica
                    //if (!DBUtils.IsTemplateEditable(idTemplate, infoUtente.idGruppo, _pi3DbContext))
                    //{
                    //    throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                    //}
                    var templateDiritti = DBUtils.GetDocumentTemplateByIdTemplate(idTemplate, infoUtente.idGruppo, _pi3DbContext);

                    if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                    {
                        template = request.Document.Template;

                        bool daRepertoriare = default;



                        DocsPaVO.ProfilazioneDinamica.Templates templates = new();
                        if (!string.IsNullOrEmpty(request.Document.Template.Id))
                        {
                            templates = DBUtils.GetTemplateById(request.Document.Template.Id, _pi3DbContext);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Template.Name))
                            {
                                templates = DBUtils.GetTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione, this._pi3DbContext);
                            }
                        }
                        templates = (await this._mediator.Send(new GetTemplateFromPisVisibilityCommand(request.Document.Template, templates, false, infoUtente.idGruppo, "D", "", infoUtente, null, null, null, true))).output;


                        foreach (var oggettoCustom in templates.ELENCO_OGGETTI)
                        {
                            var editabile = templateDiritti.Fields.Where(a => a.Name.ToUpper() == oggettoCustom.DESCRIZIONE.ToUpper() && a.Rights == ("INSERT_AND_MODIFY")).FirstOrDefault();

                            if (editabile is null || string.IsNullOrWhiteSpace(editabile.Id))
                            {
                                throw new RestException("TEMPLATE_FIELD_NOT_ROLE_EDITABLE");
                            }

                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Contatore":
                                case "ContatoreSottocontatore":
                                    aggregate.ChangeProfileFieldValue(
                                    templates.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                    break;
                                case "CasellaDiSelezione":
                                    aggregate.ChangeProfileFieldValue(
                                    templates.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                    break;
                                default:
                                    aggregate.ChangeProfileFieldValue(
                                    templates.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                    break;
                            }
                        }
                    }
                }

                DocsPaVO.utente.Corrispondente mittentePaVO = null;
                DocsPaVO.documento.SchedaDocumento schedaDoc = new();
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
                            mittentePaVO = mittResp.Corrispondente;
                        }
                    }
                    else
                    {
                        if (request.Document.Sender != null &&
                            !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) &&
                            request.Document.Sender.CorrespondentType.Equals("O") &&
                            string.IsNullOrEmpty(request.Document.Sender.Id))
                        {
                            mittentePaVO = DBUtils.GetCorrespondentFromPisNewInsertOccasionale(request.Document.Sender, infoUtente, this._pi3DbContext);
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
                                mittentePaVO = mittResp.Corrispondente;
                                if (mittentePaVO == null)
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
                var tipologiaFlusso = RestUtils.AsTipologiaFlusso(request.Document.DocumentType);

                if (request.Document.Predisposed)
                {
                    aggregate.Predisponi((TipologiaFlussoEnum)tipologiaFlusso);
                }
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) &&
                    !request.Document.DocumentType.ToUpper().Equals("G"))
                {

                    switch (tipologiaFlusso)
                    {
                        case TipologiaFlussoEnum.E:

                            var arrivalDate = this.ParseDate(request.Document.ArrivalDate);
                            if (!request.Document.Predisposed)
                            {
                                aggregate.AssignProtocolloMittente(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente()
                                {
                                    Data = !string.IsNullOrEmpty(request.Document.DataProtocolSender) ? request.Document.DataProtocolSender.Trim().AsDateTime() : null,
                                    DataArrivo = !string.IsNullOrEmpty(request.Document.ArrivalDate) ? arrivalDate : null,
                                    Segnatura = !string.IsNullOrEmpty(request.Document.ProtocolSender) ? request.Document.ProtocolSender : null,
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

                            if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                            {

                                var mittentiMultipli = aggregate.MittentiMultipli?.ToList();
                                if (mittentiMultipli != null && mittentiMultipli.Any())
                                {
                                    mittentiMultipli.ForEach(mm =>
                                    {
                                        aggregate.RemoveMittenteMultiplo(mm);
                                    });

                                }

                                foreach (var mm in request.Document.MultipleSenders)
                                {
                                    //bool inList = aggregate.MittentiMultipli.Any(x => mm.Id != null && x.Id == mm.Id);
                                    //if (!inList)
                                    aggregate.AddMittenteMultiplo(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente(
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

                                        for (int i = corrByLista.Count - 1; i >= 0; i--)
                                        {
                                            aggregate.RemoveDestinatario(aggregate.Destinatari[i]);
                                        }
                                        if (request.Document.Recipients != null)
                                        {
                                            foreach (var recipient in request.Document.Recipients)
                                            {
                                                aggregate.AddDestinatario(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario(
                                                    new PG()
                                                    {
                                                        DenominazioneUfficio = new TextValue(recipient.Description),
                                                        IndirizziDigitaliDiRiferimento = new List<string>() { recipient.Email }
                                                    }, recipient.Id)
                                                { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = aggregate.Destinatari.Count - 1; i >= 0; i--)
                                        {
                                            var pis = aggregate.Destinatari[i];

                                            // Verifica se c'� una corrispondenza tra pis.Id e PaVo.systemId
                                            var corrispondenza = corrBySys.systemId == pis.Id;

                                            // Se c'� una corrispondenza, rimuovi l'elemento
                                            if (corrispondenza != null)
                                            {
                                                aggregate.RemoveDestinatario(pis);
                                            }
                                        }

                                        aggregate.AddDestinatario(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario(
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

                                        for (int i = corrByLista.Count - 1; i >= 0; i--)
                                        {
                                            aggregate.RemoveDestinatarioCc(aggregate.DestinatariCc[i]);
                                        }
                                        if (request.Document.RecipientsCC != null)
                                        {
                                            foreach (var recipient in request.Document.RecipientsCC)
                                            {
                                                aggregate.AddDestinatarioCc(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario(
                                                    new PG()
                                                    {
                                                        DenominazioneUfficio = new TextValue(recipient.Description),
                                                        IndirizziDigitaliDiRiferimento = new List<string>() { recipient.Email }
                                                    }, recipient.Id)
                                                { MezzoDiSpedizione = corrBySys.canalePref?.typeId });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = aggregate.DestinatariCc.Count - 1; i >= 0; i--)
                                        {
                                            var pis = aggregate.DestinatariCc[i];

                                            // Verifica se c'� una corrispondenza tra pis.Id e PaVo.systemId
                                            var corrispondenza = corrBySys.systemId == pis.Id;

                                            // Se c'� una corrispondenza, rimuovi l'elemento
                                            if (corrispondenza != null)
                                            {
                                                aggregate.RemoveDestinatarioCc(pis);
                                            }
                                        }

                                        aggregate.AddDestinatarioCc(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario(
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


                //modifica mittente
                if (mittentePaVO != null)
                {
                    //var mittente = DBUtils.GetCorrespondentFromDB(request.Document.Sender.Id, _pi3DbContext);
                    //if (mittente == null || string.IsNullOrEmpty(mittente.Id)) { throw new RestException("CORRESPONDENT_NOT_FOUND"); }
                    if (aggregate.Mittente == null || (aggregate.Mittente != null && aggregate.Mittente.Id != mittentePaVO.systemId))
                        aggregate.ChangeMittente(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente(new PG()
                        {
                            DenominazioneUfficio = new TextValue(mittentePaVO.descrizione),
                            IndirizziDigitaliDiRiferimento = new List<string>() { mittentePaVO.email }
                        }, mittentePaVO.systemId));
                    //else
                    //    aggregate.AssignMittente(new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente(mittente.Id));
                }


                var errors = aggregate.GetErrors();
                foreach (var error in errors)
                {
                    _loggerService.LogKO(error.Message);
                }
                await _darepository.Update(aggregate);

                //costruzione dell'oggetto da restituire
                if (aggregate is not null && !string.IsNullOrWhiteSpace(aggregate.Id))
                {
                    var aggregateResponse = await _darepository.Get(infoUtente.idAmministrazione, aggregate.Id, new Core.SeedWork.ILoadBehavior[1]
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
                    //TODO: eventualmente aggiungere i campi mancanti
                    var note = DBUtils.getNoteOggetto(aggregate.Id, _pi3DbContext);
                    if (note != null)
                    {
                        response.Document.Note = note.ToArray();
                    }
                    response.Document.Template = DBUtils.getTemplateFromDocumentId(aggregate.Id, _pi3DbContext);
                    if (idOggetto is not null)
                    {
                        response.Document.Template.Fields.FirstOrDefault(o => o.Id.Equals(idOggetto.ToString())).Value = segnaturaRepertorio;

                    }

                    response.Document.ParentDocument = await DBUtils.getParentDocInfoFromDocId(aggregate.Id, _pi3DbContext);
                    var childDocs = await DBUtils.getChildDocsInfoFromDocId(aggregate.Id, _pi3DbContext);
                    if (childDocs != null && childDocs.Any())
                        response.Document.LinkedDocuments = childDocs.ToArray();
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                await _loggerService.LogOK("MODIFIEDOBJECTDOC", aggregate.Id, $"PIS REST:Modificato il documento con id {aggregate.Id} tramite PIS");


                #endregion

                response.Code = GetDocumentResponseCode.OK;

                _logger.LogInformation("end EditDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione EditDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new EditDocumentCommandResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione EditDocument");
                response = new EditDocumentCommandResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members
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
                        throw new Exception("Formato Data non corretto");
                    }
                }
            }
            catch
            {
                throw new Exception("Formato Data non corretto");
            }

            return dateVal;
        }


        protected readonly ILogger<EditDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _darepository;
        protected readonly IWebMethodLoggerService _loggerService;

        #endregion
    }

}