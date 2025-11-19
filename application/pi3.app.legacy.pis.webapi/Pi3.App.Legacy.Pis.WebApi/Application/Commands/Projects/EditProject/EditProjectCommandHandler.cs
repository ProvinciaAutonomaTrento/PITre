// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Utilities.IO;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFascicoloById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateFascDettagli;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using StackExchange.Redis;
using System.Collections;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditProject
{
    // Richiede libreria MediatR
    public class EditProjectCommandHandler : IRequestHandler<EditProjectCommand, EditProjectCommandResponse>
    {
        #region Public Members

        public EditProjectCommandHandler(IAggregazioneDocumentaleRepository aggregazioneDocumentale,ILogger<EditProjectCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._aggregazioneDocumentale = aggregazioneDocumentale;
        }

        public async Task<EditProjectCommandResponse> Handle(EditProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("EditProject - START");

            EditProjectCommandResponse response = new EditProjectCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);

                if (request.Project == null)
                {
                    throw new RestException("REQUIRED_PROJECT");
                }
                #endregion

                #region implementazione

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                await _pi3DbContext.AssertSecurityRights(request.Project.Id, infoUtente.idPeople, infoUtente.idGruppo,SecurityRightTypesEnum.Write);

                try
                {
                    fascicolo = (await this._mediator.Send(new FascicolazioneGetFascicoloByIdCommand()
                    {
                        IdFascicolo = request.Project.Id,
                        InfoUtente = infoUtente
                    })).Output;

                    if (fascicolo != null)
                    {
                        fascicolo.cartaceo = request.Project.Paper;
                        if (request.Project.Controlled)
                        {
                            fascicolo.controllato = "1";
                        }
                        else
                        {
                            fascicolo.controllato = "0";
                        }

                        fascicolo.descrizione = request.Project.Description;
                        fascicolo.dtaLF = request.Project.CollocationDate;
                        fascicolo.idUoLF = request.Project.PhysicsCollocation;
                        if (request.Project.Private)
                        {
                            fascicolo.privato = "1";
                        }
                        else
                        {
                            fascicolo.privato = "0";
                        }

                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (request != null && request.Project != null && request.Project.Template != null && (!string.IsNullOrEmpty(request.Project.Template.Id) || (!string.IsNullOrEmpty(request.Project.Template.Name))))
                        {
                            if (!string.IsNullOrEmpty(request.Project.Template.Id))
                            {
                                template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                                {
                                    IdTemplate = request.Project.Template.Id.ToString()
                                })).Output;
                            }
                            else if (!string.IsNullOrEmpty(request.Project.Template.Name))
                            {
                                var tipoFascSid = await this._pi3DbContext.TipoFascEntities.AsNoTracking()
                                    .Where(tf => tf.VAR_DESC_FASC.ToUpper().Equals(request.Project.Template.Name.ToUpper()))
                                    .Select(t => t.SYSTEM_ID).FirstOrDefaultAsync();
                                if(tipoFascSid != null)
                                {
                                    template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                                    {
                                        IdTemplate = tipoFascSid.ToString()
                                    })).Output;
                                }
                            }
                            var controlloVisibilita = DBUtils.GetRuoliTipoFasc(infoUtente,template.SYSTEM_ID.ToString(),this._pi3DbContext);
                            if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                            {
                                throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                            }
                            if (template != null)
                            {
                                fascicolo.template = (await this._mediator.Send(new GetTemplateFascDettagliCommand()
                                {
                                    IdProject = fascicolo.systemID
                                })).Output;

                                if (fascicolo.template != null)
                                {
                                    List<DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue> oldOggCustom = new();
                                    template = fascicolo.template;
                                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg1 in fascicolo.template.ELENCO_OGGETTI)
                                    {
                                        DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue oldObjText = new DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue();
                                        oldObjText.IDTemplate = fascicolo.template.SYSTEM_ID.ToString();
                                        oldObjText.ID_Doc_Fasc = fascicolo.systemID;
                                        oldObjText.ID_Oggetto = ogg1.SYSTEM_ID.ToString();
                                        oldObjText.Valore = ogg1.VALORE_DATABASE;
                                        oldObjText.Tipo_Ogg_Custom = ogg1.TIPO.DESCRIZIONE_TIPO;
                                        oldObjText.ID_People = infoUtente.idPeople;
                                        oldObjText.ID_Ruolo_In_UO = infoUtente.idCorrGlobali;
                                        oldOggCustom.Add(oldObjText);
                                    }
                                    template.OLD_OGG_CUSTOM = oldOggCustom.ToArray();
                                }
                                fascicolo.template = (await this._mediator.Send(new GetTemplateFromPisVisibilityCommand(request.Project.Template, template, false, infoUtente.idGruppo, "P", "", infoUtente))).output;

                            }
                            else
                            {
                                throw new RestException("TEMPLATE_NOT_FOUND");
                            }
                        }

                        fascicolo = await this.UpdateFascicolo(fascicolo,infoUtente);

                        if (template != null)
                        {
                            var idFolderPricipal = await this._pi3DbContext.ProjectEntities.Where(p => p.ID_PARENT == fascicolo.systemID.AsLong() && p.ID_FASCICOLO == fascicolo.systemID.AsLong()).Select(p => p.SYSTEM_ID).FirstAsync();

                            var aggregate = await this._aggregazioneDocumentale.Get(infoUtente.idAmministrazione, idFolderPricipal.ToString(), new ILoadBehavior[1]
                            {
                                new GetAggregatoDocumentaleLoadBehavior()
                                {

                                    BypassSecurityCheck = false,
                                    LoadFolderHierarchy = false,
                                    LoadProfiles = true
                                }
                            });

                            if (aggregate.Profiles == null || aggregate.Profiles.Count == 0)
                            {
                                // INSERIMENTO
                                aggregate.AddProfile(template.SYSTEM_ID.ToString(), new TextValue(template.DESCRIZIONE));
                                for (int i = 0; i < template.ELENCO_OGGETTI.Count(); i++)
                                {
                                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                    {
                                        case "Contatore":
                                        case "ContatoreSottocontatore":
                                            aggregate.AddProfileField(
                                            fascicolo.template.SYSTEM_ID.ToString(),
                                            oggettoCustom.SYSTEM_ID.ToString(),
                                            new TextValue(oggettoCustom.DESCRIZIONE),
                                            oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                            new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                            break;
                                        case "CasellaDiSelezione":
                                            aggregate.AddProfileField(
                                            fascicolo.template.SYSTEM_ID.ToString(),
                                            oggettoCustom.SYSTEM_ID.ToString(),
                                            new TextValue(oggettoCustom.DESCRIZIONE),
                                            oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                            new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                            break;
                                        default:
                                            aggregate.AddProfileField(
                                            fascicolo.template.SYSTEM_ID.ToString(),
                                            oggettoCustom.SYSTEM_ID.ToString(),
                                            new TextValue(oggettoCustom.DESCRIZIONE),
                                            oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                            new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                            break;
                                    }
                                }
                                Array.Clear(template.OLD_OGG_CUSTOM);

                            }
                            else
                            {
                                //AGGIORNAMENTO
                                foreach (var oggettoCustom in fascicolo.template.ELENCO_OGGETTI)
                                {
                                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                    {
                                        case "Contatore":
                                        case "ContatoreSottocontatore":
                                            aggregate.ChangeProfileFieldValue(
                                            fascicolo.template.SYSTEM_ID.ToString(),
                                            oggettoCustom.SYSTEM_ID.ToString(),
                                            new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                            break;
                                        case "CasellaDiSelezione":
                                            aggregate.ChangeProfileFieldValue(
                                            fascicolo.template.SYSTEM_ID.ToString(),
                                            oggettoCustom.SYSTEM_ID.ToString(),
                                            new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                            break;
                                        default:
                                            aggregate.ChangeProfileFieldValue(
                                            fascicolo.template.SYSTEM_ID.ToString(),
                                            oggettoCustom.SYSTEM_ID.ToString(),
                                            new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                            break;
                                    }
                                }
                            }
                            await this._aggregazioneDocumentale.Update(aggregate);

                        }
                    }
                    else
                    {
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Errore nell'aggiornamento dei dati del fascicolo");
                }
                if (fascicolo != null)
                {
                    Project responseProject = DBUtils.getProjectFromDB(fascicolo.systemID, this._pi3DbContext);
                    if (responseProject != null)
                    {
                        response.Project = responseProject;
                    }
                    else
                    {
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }


                #endregion

                response.Code = GetProjectResponseCode.OK;

                _logger.LogInformation("end EditProject");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione EditProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new EditProjectCommandResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "eccezione EditProject");
                response = new EditProjectCommandResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentale;

        private async Task<DocsPaVO.fascicolazione.Fascicolo> UpdateFascicolo(DocsPaVO.fascicolazione.Fascicolo nodoFascicolo, InfoUtente infoUtente)
        {
            try
            {
                if (nodoFascicolo != null && nodoFascicolo.systemID != null)
                {
                    if (nodoFascicolo.descrizione.Contains("�"))
                        nodoFascicolo.descrizione = nodoFascicolo.descrizione.Replace("�", "&ordm;");
                    var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                    var idFolderPricipal = await this._pi3DbContext.ProjectEntities.Where(p => p.ID_PARENT == nodoFascicolo.systemID.AsLong() && p.ID_FASCICOLO == nodoFascicolo.systemID.AsLong()).Select(p => p.SYSTEM_ID).FirstAsync();
                    var aggregate = await _aggregazioneDocumentale.Get(idTenant, idFolderPricipal.ToString(),
                    new ILoadBehavior[1]
                    {
                                new GetAggregatoDocumentaleLoadBehavior()
                                {
                                    BypassSecurityCheck = true, 
                                    LoadFolderHierarchy = true
                                }
                         });

                    aggregate.ChangeDescription(new TextValue(nodoFascicolo.descrizione));

                    await this._aggregazioneDocumentale.Update(aggregate);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                nodoFascicolo = null;
            }
            if(nodoFascicolo == null)
            {
                throw new Exception(Resource.CampoDescMsg);
            }
            return nodoFascicolo;
        }

        #endregion
    }

}