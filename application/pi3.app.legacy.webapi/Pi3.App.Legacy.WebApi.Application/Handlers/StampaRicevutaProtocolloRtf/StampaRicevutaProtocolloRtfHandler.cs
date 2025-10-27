// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StampaRicevutaProtocolloRtfRequest = Pi3.App.Legacy.WebApi.Application.Requests.StampaRicevutaProtocolloRtf;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.StampaRicevutaProtocolloRtf
{
    public class StampaRicevutaProtocolloRtfHandler : IRequestHandler<StampaRicevutaProtocolloRtfRequest, StampaRicevutaProtocolloRtfResult>
    {
        #region Public Members

        public StampaRicevutaProtocolloRtfHandler(ILogger<StampaRicevutaProtocolloRtfHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<StampaRicevutaProtocolloRtfResult> Handle(StampaRicevutaProtocolloRtfRequest request, CancellationToken cancellationToken)
        {
            FileDocumento fileDocumento = null;
            var schedaDocumento = request.schedaDoc;

            try
            {
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
                var tenantDescription = await _dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idTenant)
                    .Select(a => a.VAR_DESC_AMM)
                    .FirstAsync();

                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
                string rootPath = Path.Combine(
                                repositoryRootPath,
                                "Modelli",
                                tenantCode.ToUpper(),
                                "Ricevute",
                                schedaDocumento.registro.codRegistro,
                                "Ricevuta.rtf"
                                )
                                .PathAsUnixPath();

                var reportData = File.ReadAllText(rootPath);

                var dataProtocollo = await _dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == schedaDocumento.systemId.AsLong())
                    .Select(p => p.DTA_PROTO)
                    .FirstOrDefaultAsync();

                reportData = Regex.Replace(reportData, "#Amministrazione#", tenantDescription, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                reportData = Regex.Replace(reportData, "#Data ora protocollo#", dataProtocollo.AsDateTimeFormat(), RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                reportData = Regex.Replace(reportData, "#Data protocollo#", schedaDocumento.protocollo.dataProtocollazione, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                reportData = Regex.Replace(reportData, "#Numero protocollo#", schedaDocumento.protocollo.numero, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                reportData = Regex.Replace(reportData, "#Segnatura#", schedaDocumento.protocollo.segnatura, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                reportData = Regex.Replace(reportData, "#Oggetto#", schedaDocumento.oggetto.descrizione.Replace("\n", @" \par "), RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

                var descrizione = string.Empty;
                if (schedaDocumento.protocollatore != null)
                {
                    // PROTOCOLLATORE						
                    if (schedaDocumento.protocollatore.utente_idPeople != null && schedaDocumento.protocollatore.utente_idPeople != string.Empty)
                    {
                        descrizione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.ID_PEOPLE == schedaDocumento.protocollatore.utente_idPeople.AsLong())
                            .Select(c => c.VAR_DESC_CORR)
                            .FirstOrDefaultAsync();

                        reportData = Regex.Replace(reportData, "#Protocollatore#", descrizione, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                    }

                    // RUOLO PROTOCOLLATORE					
                    if (schedaDocumento.protocollatore.ruolo_idCorrGlobali != null && schedaDocumento.protocollatore.ruolo_idCorrGlobali != string.Empty)
                    {
                        descrizione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == schedaDocumento.protocollatore.ruolo_idCorrGlobali.AsLong())
                            .Select(c => c.VAR_DESC_CORR)
                            .FirstOrDefaultAsync();

                        reportData = Regex.Replace(reportData, "#Ruolo protocollatore#", descrizione, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                    }

                    // UO PROTOCOLLATORE					
                    if (schedaDocumento.protocollatore.uo_idCorrGlobali != null && schedaDocumento.protocollatore.uo_idCorrGlobali != string.Empty)
                    {
                        descrizione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == schedaDocumento.protocollatore.uo_idCorrGlobali.AsLong())
                            .Select(c => c.VAR_DESC_CORR)
                            .FirstOrDefaultAsync();

                        reportData = Regex.Replace(reportData, "#Uo protocollatore#", descrizione, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                    }
                }

                // NOTE 
                // Reperimento dell'ultima nota visibile a tutti
                var noteDocumento = await _dbContext.NoteEntities.
                         Where(n => n.IDOGGETTOASSOCIATO == schedaDocumento.systemId.AsLong()
                            && (n.TIPOVISIBILITA == "T"))
                         .OrderByDescending(n => n.DATACREAZIONE)
                         .Select(n => n.TESTO)
                         .FirstOrDefaultAsync();
                reportData = Regex.Replace(reportData, "#Note#", noteDocumento != null ?  noteDocumento : string.Empty, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

                //report custom pat momentaneo
                if (schedaDocumento.creatoreDocumento != null && !string.IsNullOrEmpty(schedaDocumento.creatoreDocumento.idCorrGlob_UO))
                {
                    descrizione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == schedaDocumento.creatoreDocumento.idCorrGlob_UO.AsLong())
                            .Select(c => c.VAR_DESC_CORR)
                            .FirstOrDefaultAsync();

                    reportData = Regex.Replace(reportData, "#Uo creatore#", descrizione, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                }
                else
                {
                    reportData = Regex.Replace(reportData, "#Uo creatore#", "", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                }

                var descrizioneMittente = schedaDocumento.protocollo.GetType() == typeof(ProtocolloEntrata) ? 
                    ((ProtocolloEntrata)schedaDocumento.protocollo).mittente.descrizione : (schedaDocumento.protocollo.GetType() == typeof(ProtocolloUscita) ? 
                        ((ProtocolloUscita)schedaDocumento.protocollo).mittente.descrizione : ((ProtocolloInterno)schedaDocumento.protocollo).mittente.descrizione);

                reportData = Regex.Replace(reportData, "#Mittente#", descrizioneMittente, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                reportData = Regex.Replace(reportData, "#Numero Allegati#", schedaDocumento.allegati.Count().ToString(), RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

                fileDocumento = new FileDocumento();
                fileDocumento.content = ToByteArray(reportData);
                fileDocumento.length = fileDocumento.content.Length;
                fileDocumento.contentType = "application/rtf";
                fileDocumento.name = "ricevuta.rtf";
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                fileDocumento = null;
            }

            return new StampaRicevutaProtocolloRtfResult(fileDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<StampaRicevutaProtocolloRtfHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        private byte[] ToByteArray(string str)
        {
            char[] charArray = str.ToCharArray();
            System.Collections.ArrayList byteArr = new System.Collections.ArrayList();

            for (int i = 0; i < charArray.Length; i++)
            {
                if ((int)charArray[i] > 255)
                {
                    string utf = "\\u" + ((int)charArray[i]) + "G";
                    char[] utfChars = utf.ToCharArray();
                    for (int j = 0; j < utfChars.Length; j++)
                    {
                        byteArr.Add((byte)utfChars[j]);
                    }
                }
                else
                {
                    byteArr.Add((byte)charArray[i]);
                }
            }
            byte[] res = (byte[])byteArr.ToArray(typeof(byte));
            return res;
        }
        #endregion
    }
}