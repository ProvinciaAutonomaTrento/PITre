// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Chilkat;
using DocsPaVO.ProfilazioneDinamicaLite;
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
using GetReportPregressiRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportPregressi;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetReportPregressi
{
    public class GetReportPregressiHandler : IRequestHandler<GetReportPregressiRequest, GetReportPregressiResult>
    {
        #region Public Members

        public GetReportPregressiHandler(IPi3DbContext dbContext, ILogger<GetReportPregressiHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetReportPregressiResult> Handle(GetReportPregressiRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi output = null;

            try
            {
                output = await this.GetReports(request.sysId,request.getItems);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }

            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetReportPregressiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        private async Task<DocsPaVO.Import.Pregressi.ReportPregressi> GetReports(string sysId, bool getItems)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi output = null;
            try
            {
                var preg = await this._dbContext.PregressoEntities.AsNoTracking().Where(p => p.SYSTEM_ID == sysId.AsLong()).Select(
                                p => new
                                {
                                    p.SYSTEM_ID,
                                    p.ID_AMM,
                                    p.ID_UTENTE_CREATORE,
                                    p.ID_RUOLO_CREATORE,
                                    DATA_ESECUZIONE = p.DATA_ESECUZIONE,
                                    DATA_FINE = p.DATA_FINE,
                                    p.NUM_DOC
                                }).FirstOrDefaultAsync();

                if (preg != null)
                {
                    output = new DocsPaVO.Import.Pregressi.ReportPregressi()
                    {
                        systemId = preg.SYSTEM_ID.ToString(),
                        idAmm = preg.ID_AMM != null ? preg.ID_AMM.ToString() : string.Empty,
                        idUtenteCreatore = preg.ID_UTENTE_CREATORE != null ? preg.ID_UTENTE_CREATORE.ToString() : string.Empty,
                        idRuoloCreatore = preg.ID_RUOLO_CREATORE != null ? preg.ID_RUOLO_CREATORE.ToString() : string.Empty,
                        dataEsecuzione = preg.DATA_ESECUZIONE != null ? preg.DATA_ESECUZIONE.AsDateFormat() : string.Empty,
                        dataFine = preg.DATA_FINE != null ? preg.DATA_FINE.AsDateFormat() : string.Empty,
                        numDoc = preg.NUM_DOC != null ? preg.NUM_DOC.ToString() : "0"
                    };
                    output.itemPregressi = new List<DocsPaVO.Import.Pregressi.ItemReportPregressi>();
                    if (getItems)
                    {
                        var items = await this._dbContext.AssPregressiEntities.AsNoTracking()
                            .Where(a => a.ID_PREGRESSO == sysId.AsLong() && a.ESITO != null).Select(a => new
                            {
                                a.SYSTEM_ID,
                                a.ID_PREGRESSO,
                                a.ID_REGISTRO,
                                a.ID_DOCUMENTO,
                                a.ID_UTENTE,
                                a.ID_RUOLO,
                                a.TIPO_OPERAZIONE,
                                a.DATA,
                                a.ERRORE,
                                a.ESITO,
                                a.ID_NUM_PROTO_EXCEL
                            }).ToListAsync();
                        items = items.AsQueryable().OrderBy(a => a.DATA).ToList();

                        foreach (var i in items)
                        {
                            DocsPaVO.Import.Pregressi.ItemReportPregressi item = new DocsPaVO.Import.Pregressi.ItemReportPregressi()
                            {
                                systemId = i.SYSTEM_ID.ToString(),
                                idPregresso = i.ID_PREGRESSO != null ? i.ID_PREGRESSO.ToString() : string.Empty,
                                idRegistro = i.ID_REGISTRO != null ? i.ID_REGISTRO.ToString() : string.Empty,
                                idDocumento = i.ID_DOCUMENTO != null ? i.ID_DOCUMENTO.ToString() : string.Empty,
                                idUtente = i.ID_UTENTE != null ? i.ID_UTENTE.ToString() : string.Empty,
                                idRuolo = i.ID_RUOLO != null ? i.ID_RUOLO.ToString() : string.Empty,
                                tipoOperazione = i.TIPO_OPERAZIONE ?? string.Empty,
                                data = i.DATA != null ? i.DATA.AsDateFormat() : string.Empty,
                                errore = i.ERRORE ?? string.Empty,
                                esito = i.ESITO ?? string.Empty,
                                idNumProtocolloExcel = i.ID_NUM_PROTO_EXCEL ?? string.Empty,
                                valoriProfilati = new(),
                                ProjectCodes = new string[0]
                            };

                            item.Allegati = await GetAllegatiByIdItem(item.systemId);

                            output.itemPregressi.Add(item);

                        }
                    }

                }
            }
            catch(Exception ex)
            {
                output = null;
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            
            return output;
        }

        private async Task<List<DocsPaVO.Import.Pregressi.Allegati>> GetAllegatiByIdItem(string idItem)
        {
            List<DocsPaVO.Import.Pregressi.Allegati> output = new();
            try
            {

                var allegati = await (from a in this._dbContext.AssAllegatoEntities.AsNoTracking()
                                      where a.ID_ITEM == idItem.AsLong()
                                      orderby a.SYSTEM_ID
                                      select new
                                      {
                                          a.SYSTEM_ID,
                                          a.ID_ITEM,
                                          a.ERRORE,
                                          a.ESITO
                                      }).ToListAsync();


                if (allegati.Count > 0)
                    output = new List<DocsPaVO.Import.Pregressi.Allegati>();
                foreach (var a in allegati)
                {
                    DocsPaVO.Import.Pregressi.Allegati allegato = new DocsPaVO.Import.Pregressi.Allegati()
                    {
                        systemId = a.SYSTEM_ID.ToString()
                    };
                    allegato.idItem = a.ID_ITEM != null ? a.ID_ITEM.ToString() : string.Empty;
                    allegato.idItem = a.ERRORE;
                    allegato.idItem = a.ESITO;

                    output.Add(allegato);

                }
            }
            catch(Exception ex)
            {
                output = null;
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return output;
        }

        #endregion
    }
}