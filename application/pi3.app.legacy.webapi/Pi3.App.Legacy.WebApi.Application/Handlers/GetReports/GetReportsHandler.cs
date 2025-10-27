// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetReportsRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReports;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetReports
{
    public class GetReportsHandler : IRequestHandler<GetReportsRequest, GetReportsResult>
    {
        #region Public Members

        public GetReportsHandler(ILogger<GetReportsHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;

        }

        public async Task<GetReportsResult> Handle(GetReportsRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.Import.Pregressi.ReportPregressi> output = new();
            try
            {
                var query = (from p in this._dbContext.PregressoEntities.AsNoTracking()
                             from a in this._dbContext.AssPregressiEntities.AsNoTracking()
                             where p.ID_AMM == request.infoUtente.idAmministrazione.AsLong() && p.SYSTEM_ID == a.ID_PREGRESSO
                             select new
                             {
                                 a,
                                 p
                             });
                var groups = query.GroupBy(r => new
                {
                    r.p.SYSTEM_ID,
                    r.p.ID_AMM,
                    r.p.ID_UTENTE_CREATORE,
                    r.p.ID_RUOLO_CREATORE,
                    r.p.DATA_ESECUZIONE,
                    r.p.DATA_FINE,
                    r.p.NUM_DOC,
                    r.p.DESCRIZIONE

                });
                foreach(var group in groups)
                {
                    long countElaborati = 0;
                    long countErrori = 0;
                    foreach(var item in group) 
                    {
                        if (item.a.ESITO != null)
                        {
                            if (item.a.ESITO.Equals("W") || item.a.ESITO.Equals("E"))
                            {
                                countElaborati++;
                                countErrori++;
                            }
                            else if (item.a.ESITO.Equals("S"))
                            {
                                countElaborati++;
                            }
                        }
                    }
                    DocsPaVO.Import.Pregressi.ReportPregressi rp = new DocsPaVO.Import.Pregressi.ReportPregressi();
                    rp.itemPregressi = new List<DocsPaVO.Import.Pregressi.ItemReportPregressi>();
                    rp.systemId = group.Key.SYSTEM_ID.ToString();
                    rp.idAmm = group.Key.ID_AMM != null ? group.Key.ID_AMM.ToString() : string.Empty;
                    rp.idUtenteCreatore = group.Key.ID_UTENTE_CREATORE != null ? group.Key.ID_UTENTE_CREATORE.ToString() : string.Empty;
                    rp.idRuoloCreatore = group.Key.ID_RUOLO_CREATORE != null ? group.Key.ID_RUOLO_CREATORE.ToString() : string.Empty;
                    rp.dataEsecuzione = group.Key.DATA_ESECUZIONE != null ? group.Key.DATA_ESECUZIONE.AsDateFormat() : null;
                    rp.dataFine = group.Key.DATA_FINE != null ? group.Key.DATA_FINE.AsDateFormat() : null;
                    rp.numDoc = group.Key.NUM_DOC != null ? group.Key.NUM_DOC.ToString() : "0";
                    rp.descrizione = group.Key.DESCRIZIONE ?? string.Empty;
                    rp.numeroElaborati = countElaborati.ToString();
                    rp.inError = countErrori.ToString();


                    if (request.getItems)
                    {
                        var items = await (from a in this._dbContext.AssPregressiEntities.AsNoTracking()
                         where a.ID_PREGRESSO == rp.systemId.AsLong() && a.ESITO != null
                         select new DocsPaVO.Import.Pregressi.ItemReportPregressi()
                         {
                             systemId = a.SYSTEM_ID.ToString(),
                             idPregresso = a.ID_PREGRESSO != null ? a.ID_PREGRESSO.ToString() : string.Empty,
                             idRegistro = a.ID_REGISTRO != null ? a.ID_REGISTRO.ToString() : string.Empty,
                             idDocumento = a.ID_DOCUMENTO != null ? a.ID_DOCUMENTO.ToString() : string.Empty,
                             idUtente = a.ID_UTENTE != null ? a.ID_UTENTE.ToString() : string.Empty,
                             idRuolo = a.ID_RUOLO != null ? a.ID_RUOLO.ToString() : string.Empty,
                             tipoOperazione = a.TIPO_OPERAZIONE ?? string.Empty,
                             data = a.DATA != null ? a.DATA.AsDateFormat() : null,
                             errore = a.ERRORE,
                             esito = a.ESITO,
                             idNumProtocolloExcel = a.ID_NUM_PROTO_EXCEL != null ? a.ID_NUM_PROTO_EXCEL.ToString() : string.Empty
                         }).ToListAsync();
                        rp.itemPregressi = items.OrderBy(i => i.data).ToList();
                    }
                    output.Add(rp);
                }

                output = output.OrderByDescending(i => i.systemId.AsLong()).ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetReportsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}