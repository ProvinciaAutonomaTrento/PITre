// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetTitolario2;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateByDescrizione;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateFascDettagli;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchProjects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetFiltersProjectsFromPis
{
    public class GetFiltersProjectsFromPisCommandHandler : IRequestHandler<GetFiltersProjectsFromPisCommand, GetFiltersProjectsFromPisCommandResponse>
    {
        public GetFiltersProjectsFromPisCommandHandler(ILogger<GetFiltersProjectsFromPisCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }


        public async Task<GetFiltersProjectsFromPisCommandResponse> Handle(GetFiltersProjectsFromPisCommand request, CancellationToken cancellationToken)
        {

            DocsPaVO.utente.Registro registro = null;
            DocsPaVO.filtri.FiltroRicerca[][] qV = new DocsPaVO.filtri.FiltroRicerca[1][];
            qV[0] = new DocsPaVO.filtri.FiltroRicerca[1];
            DocsPaVO.filtri.FiltroRicerca[] fVList = new DocsPaVO.filtri.FiltroRicerca[0];
            DocsPaVO.filtri.FiltroRicerca fV1 = null;
            bool classificazione = false;
            string codiceClassificazione = string.Empty;
            string idTitolario = string.Empty;
            DocsPaVO.fascicolazione.Classificazione objClassificazione = null;
            bool filterFound = false;
            var filters = request.Filters;
            var infoUtente = request.InfoUtente;

            foreach (Filter fil in filters)
            {
                if (fil != null && !string.IsNullOrEmpty(fil.Value))
                {
                    if (fil.Name.ToUpper().Equals("YEAR"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "ANNO_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CREATION_DATE_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CREAZIONE_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CREATION_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CREAZIONE_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CLOSING_DATE_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CHIUSURA_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CLOSING_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CHIUSURA_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("OPENING_DATE_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "APERTURA_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("OPENING_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "APERTURA_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("STATE"))
                    {
                        filterFound = true;
                        if (fil.Value.ToUpper().Equals("O"))
                        {
                            fil.Value = "A";
                        }
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "STATO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("TYPE_PROJECT"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "TIPO_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (fil.Name.ToUpper().Equals("PROJECT_CODE"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CODICE_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (fil.Name.ToUpper().Equals("PROJECT_NUMBER"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "NUMERO_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("PROJECT_DESCRIPTION"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "TITOLO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CLASSIFICATION_CODE"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CODICE_CLASSIFICA";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                        classificazione = true;
                        codiceClassificazione = fil.Value;
                    }

                    if (fil.Name.ToUpper().Equals("CLASSIFICATION_SCHEME"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "ID_TITOLARIO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                        idTitolario = fil.Value;
                    }

                    if (fil.Name.ToUpper().Equals("REGISTER"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            registro = DBUtils.getRegistroByCodAOO(fil.Value, infoUtente.idAmministrazione, this._pi3DbContext);
                        }
                    }

                    if (fil.Name.ToUpper().Equals("SUBPROJECT"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "SOTTOFASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("TEMPLATE"))
                    {
                        // viene cercato solo l'id del template con TIPOLOGIA_FASCICOLO
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;

                        int tempIdTemplate = 0;
                        if (!int.TryParse(fil.Value, out tempIdTemplate))
                        {
                            template = (await this._mediator.Send(new GetTemplateByDescrizioneCommand()
                            {
                                Desc = fil.Value
                            })).Output;
                            if (template != null)
                                tempIdTemplate = template.SYSTEM_ID;
                        }

                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "TIPOLOGIA_FASCICOLO";
                        //fV1.valore = fil.Value;
                        fV1.valore = tempIdTemplate.ToString();
                        fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PROFILAZIONE_DINAMICA";
                        fV1.valore = "Profilazione Dinamica";

                        if (fil.Template != null)
                        {
                            if (!string.IsNullOrEmpty(fil.Template.Id))
                            {
                                template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                                {
                                   IdTemplate = fil.Template.Id
                                })).Output;

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(fil.Template.Name))
                                {
                                    template = (await this._mediator.Send(new GetTemplateByDescrizioneCommand()
                                    {
                                        Desc = fil.Template.Name
                                    })).Output;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(fil.Value))
                            {
                                int templateNum = 0;

                                if (Int32.TryParse(fil.Value, out templateNum))
                                {
                                    template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                                    {
                                        IdTemplate = templateNum.ToString()
                                    })).Output;

                                }
                                else
                                {
                                    template = (await this._mediator.Send(new GetTemplateByDescrizioneCommand()
                                    {
                                        Desc = fil.Value
                                    })).Output;

                                }
                            }
                        }



                        if (template != null)
                        {
                            if (fil.Template != null)
                            {
                                fV1.template = DBUtils.GetTemplateFromPis(fil.Template, template, true, infoUtente, this._pi3DbContext);
                                fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                        else
                        {
                            //Template non trovato
                            throw new RestException("TEMPLATE_NOT_FOUND");
                        }
                    }

                    if (fil.Name.ToUpper().Equals("TEMPLATE_EXTRACTION"))
                    {
                        filterFound = true;
                    }

                    if (fil.Name.ToUpper().Equals("REGISTER_EXTRACTION"))
                    {
                        filterFound = true;
                    }

                    if (!filterFound)
                        throw new RestException("FILTER_NOT_FOUND");
                }
            }

            if (classificazione)
            {
                if (registro != null && !string.IsNullOrEmpty(codiceClassificazione) && !string.IsNullOrEmpty(idTitolario))
                {
                    var listaClassifiche = (await this._mediator.Send(new FascicolazioneGetTitolario2Command()
                    {
                        IdAmministrazione = infoUtente.idAmministrazione,
                        IdGruppo = infoUtente.idGruppo,
                        IdPeople = infoUtente.idPeople,
                        Registro = registro,
                        CodiceClassifica = codiceClassificazione,
                        GetFigli = false,
                        IdTitolario = idTitolario
                    })).Output;

                    if (listaClassifiche != null && listaClassifiche.Count() > 0)
                    {
                        objClassificazione = (DocsPaVO.fascicolazione.Classificazione)listaClassifiche[0];
                    }
                }
                else
                {
                    throw new RestException("MISSING_PATAMETERS_CLASSIFICATION");
                }
            }

            fV1 = new DocsPaVO.filtri.FiltroRicerca();
            fV1.nomeCampo = "CREATION_DATE";
            fV1.argomento = "ORDER_DIRECTION";
            fV1.valore = "DESC";
            fVList = RestUtils.AddToArrayFiltroRicerca(fVList, fV1);

            qV[0] = fVList;

            return new() 
            {
                Output = qV,
                Registro = registro,
                ObjClassificazione = objClassificazione
            };
            

        }

        #region Private Members
        protected readonly ILogger<GetFiltersProjectsFromPisCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;


        #endregion
    }
}
