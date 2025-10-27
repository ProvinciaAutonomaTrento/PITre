// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorioByIdObjectCustom;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorioNoHTML;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorio
{
    public class GetSegnaturaRepertorioHandler : IRequestHandler<GetSegnaturaRepertorioCommand, GetSegnaturaRepertorioResponse>,
        IRequestHandler<GetSegnaturaRepertorioNoHTMLCommand, GetSegnaturaRepertorioNoHTMLResponse>,
        IRequestHandler<GetSegnaturaRepertorioByIdObjectCustomCommand, GetSegnaturaRepertorioByIdObjectCustomResponse>,
        IRequestHandler<GetSegnaturaRepertorioNoHTMLByIdOggettoCommand, GetSegnaturaRepertorioNoHTMLByIdOggettoResponse>


    {
        #region Public Members
        public GetSegnaturaRepertorioHandler(ILogger<GetSegnaturaRepertorioHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async virtual Task<GetSegnaturaRepertorioResponse> Handle(GetSegnaturaRepertorioCommand request, CancellationToken cancellationToken)
        {
            var segnatura = await this.GetSegnaturaRepertorio(request.Docnumber, request.CodiceAmm, true);
            return new GetSegnaturaRepertorioResponse(segnatura);
        }

        public async virtual Task<GetSegnaturaRepertorioNoHTMLResponse> Handle(GetSegnaturaRepertorioNoHTMLCommand request, CancellationToken cancellationToken)
        {
            var segnatura = await this.GetSegnaturaRepertorio(request.DocNumber, request.CodiceAmm, false);
            return new GetSegnaturaRepertorioNoHTMLResponse(segnatura);
        }

        public async virtual Task<GetSegnaturaRepertorioByIdObjectCustomResponse> Handle(GetSegnaturaRepertorioByIdObjectCustomCommand request, CancellationToken cancellationToken)
        {
            var segnatura = await this.GetSegnaturaRepertorio(request.DocNumber, request.CodiceAmm, true, request.IdObjectCustom);
            return new GetSegnaturaRepertorioByIdObjectCustomResponse(segnatura);
        }

        public async Task<GetSegnaturaRepertorioNoHTMLByIdOggettoResponse> Handle(GetSegnaturaRepertorioNoHTMLByIdOggettoCommand request, CancellationToken cancellationToken)
        {
            var segnatura = await this.GetSegnaturaRepertorio(request.DocNumber, request.CodiceAmm, false, request.IdObjectCustom);
            return new GetSegnaturaRepertorioNoHTMLByIdOggettoResponse(segnatura);
        }

        #endregion
        #region Private Members

        protected readonly ILogger<GetSegnaturaRepertorioHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected virtual async Task<string> GetSegnaturaRepertorio(string Docnumber, string CodiceAmm, bool GenerateHtml, string IdObjectCustom)
        {
            string segnatura = string.Empty;
            string dataCancellazione = string.Empty;

            var oggettoCustom = _dbContext.AssociazioneTemplatesEntities.Join(_dbContext.OggettiCustomEntities, at => at.ID_OGGETTO, oc => oc.SYSTEM_ID, (at, oc) => new { at, oc }).
                Where(w => w.at.DOC_NUMBER == Docnumber && w.oc.REPERTORIO == 1 && w.at.ID_OGGETTO == IdObjectCustom.AsLong()).
                Select(s => new
                {
                    DATA_ANNULLAMENTO = s.at.DTA_ANNULLAMENTO,
                    s.at.VAR_SEGNATURA,

                }).FirstOrDefault();

            if (oggettoCustom != null)
            {
                dataCancellazione = oggettoCustom.VAR_SEGNATURA;

                if (!string.IsNullOrEmpty(oggettoCustom.VAR_SEGNATURA))
                {
                    if (GenerateHtml)
                        if (!string.IsNullOrEmpty(oggettoCustom.DATA_ANNULLAMENTO.ToString()))
                        {
                            segnatura = "<p style=\"color:red;text-decoration:line-through;\">" + oggettoCustom.VAR_SEGNATURA + "<br/>--<br/>" + oggettoCustom.DATA_ANNULLAMENTO + "</p>";
                        }
                        else
                        {
                            segnatura = "<p style=\"color:red;\">" + oggettoCustom.VAR_SEGNATURA + "</p>";
                        }
                    else
                        segnatura = oggettoCustom.VAR_SEGNATURA;
                }
            }


            return (segnatura);
        }

        protected virtual async Task<string> GetSegnaturaRepertorio(string DocNumber, string CodiceAmm, bool GenerateHtml)
        {
            var segnatura = string.Empty;

            var oggettoCustomEntity = await _dbContext.AssociazioneTemplatesEntities
                .Join(_dbContext.OggettiCustomEntities,
                    a => a.ID_OGGETTO,
                    o => o.SYSTEM_ID,
                    (a, o) => new { a, o })
                .Where(j => j.a.DOC_NUMBER == DocNumber && j.o.REPERTORIO == 1)
                .Select(j => new
                {
                    j.a.DOC_NUMBER,
                    j.a.VAR_SEGNATURA,
                    j.a.DTA_ANNULLAMENTO,
                    j.o.CAMPO_COMUNE
                })
                .ToListAsync();

            if (oggettoCustomEntity.Any())
            {
                var repertorio = oggettoCustomEntity.OrderBy(o => o.CAMPO_COMUNE ?? 0).First();
                var dataCancellazione = repertorio.DTA_ANNULLAMENTO;
                segnatura = repertorio.VAR_SEGNATURA ?? string.Empty;

                if (!string.IsNullOrEmpty(segnatura) && GenerateHtml)
                {
                    segnatura = dataCancellazione.HasValue ? "<p style=\"color:red;text-decoration:line-through;\">" + segnatura + "<br/>--<br/>" + dataCancellazione.Value + "</p>"
                        : "<p style=\"color:red;\">" + segnatura + "</p>";
                }
            }

            return segnatura;
        }

        protected virtual Templates GetTemplateDettagli(string DocNumber)
        {
            Templates template = new Templates();
            List<OggettoCustom> listaOggetti = new List<OggettoCustom>();

            long? idTemplate = _dbContext.AssociazioneTemplatesEntities.Where(w => w.DOC_NUMBER == DocNumber).Select(s => s.ID_TEMPLATE).FirstOrDefault();

            if (idTemplate != null)
            {
                var query = _dbContext.AssociazioneTemplatesEntities.
                Join(_dbContext.TipoAttoEntities, at => at.ID_TEMPLATE, ta => ta.SYSTEM_ID, (at, ta) => new { at, ta }).
                Join(_dbContext.OggettiCustomEntities, j => j.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (j, oc) => new { j.at, j.ta, oc }).
                Join(_dbContext.OggettiCustomCompEntities, j => j.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (j, occ) => new { j.at, j.ta, j.oc, occ }).
                Join(_dbContext.TipoOggettoEntities, j => j.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (j, to) => new { j.at, j.ta, j.oc, j.occ, to });

                var dataSet = query.Where(w => w.ta.SYSTEM_ID == idTemplate && w.occ.ID_TEMPLATE == idTemplate && w.at.DOC_NUMBER == DocNumber).Select(s => new
                {
                    SYSTEM_ID_OGG_CUSTOM = query.Select(s => s.at.ID_OGGETTO).Distinct().FirstOrDefault(),
                    SYSTEM_ID_TIPO_ATTO = s.at.SYSTEM_ID,
                    SYSTEM_ID_TEMPLATE = s.at.ID_TEMPLATE,
                    SYSTEM_ID_OGG_CUSTOM_COMP = s.occ.SYSTEM_ID,
                    SYSTEM_ID_TIPO_OGGETTO = s.to.SYSTEM_ID,
                    s.oc.CAMPO_COMUNE,
                    s.oc.REPERTORIO,
                    s.occ.POSIZIONE,
                    s.at.VAR_SEGNATURA,
                    DTA_ANNULLAMENTO = _dbContext.AssociazioneTemplatesEntities.Join(_dbContext.OggettiCustomEntities, at => at.ID_OGGETTO, oc => oc.SYSTEM_ID, (at, oc) => new { at, oc }).
                                               Join(_dbContext.TipoOggettoEntities, j => j.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (j, to) => new { j.at, j.oc, to }).
                                               Where(w => w.at.SYSTEM_ID == s.at.SYSTEM_ID && w.to.DESCRIZIONE != "'CasellaDiSelezione'").Select(s => s.at.DTA_ANNULLAMENTO).FirstOrDefault(),

                }).OrderBy(o => o.POSIZIONE).ToArray();



                for (int i = 0; i < dataSet.Count(); i++)
                {
                    OggettoCustom oggettoCustom = new OggettoCustom();

                    oggettoCustom.SYSTEM_ID = Convert.ToInt32(dataSet[i].SYSTEM_ID_OGG_CUSTOM);
                    oggettoCustom.POSIZIONE = dataSet[i].POSIZIONE.ToString() != null ? dataSet[i].POSIZIONE.ToString() : "";

                    if (!string.IsNullOrEmpty(dataSet[i].CAMPO_COMUNE.ToString()) && dataSet[i].CAMPO_COMUNE.ToString() == "1")
                        oggettoCustom.CAMPO_COMUNE = "1";
                    else
                        oggettoCustom.CAMPO_COMUNE = "0";

                    if (!string.IsNullOrEmpty(dataSet[i].REPERTORIO.ToString()) && dataSet[i].REPERTORIO.ToString() == "1")
                        oggettoCustom.REPERTORIO = "1";
                    else
                        oggettoCustom.REPERTORIO = "0";

                    oggettoCustom.CONSERVAZIONE = "0";

                    if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                        oggettoCustom = this.ControllaCustom(oggettoCustom);

                    oggettoCustom.VAR_SEGNATURA = dataSet[i].VAR_SEGNATURA != null ? dataSet[i].VAR_SEGNATURA : "";

                    listaOggetti.Add(oggettoCustom);
                }

                template.ELENCO_OGGETTI = listaOggetti.ToArray();
            }


            return template;


        }

        protected virtual OggettoCustom ControllaCustom(OggettoCustom OggettoCustom)
        {
            OggettoCustom ogg = OggettoCustom;

            var query = _dbContext.ContCustomDocEntities.Where(w => w.ID_OGG == ogg.SYSTEM_ID).FirstOrDefault();

            if (ogg != null)
            {
                OggettoCustom.DATA_INIZIO = ogg.DATA_INIZIO.ToString();
                OggettoCustom.DATA_FINE = ogg.DATA_FINE.ToString();
            }

            return ogg;
        }



        #endregion
    }
}
