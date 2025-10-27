// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.FriendApplication;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportSchedaDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.ReportSchedaDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReportSchedaDoc
{
    public class ReportSchedaDocHandler : IRequestHandler<ReportSchedaDocRequest, ReportSchedaDocResult>
    {
        #region Public Members

        public ReportSchedaDocHandler(ILogger<ReportSchedaDocHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<ReportSchedaDocResult> Handle(ReportSchedaDocRequest request, CancellationToken cancellationToken)
        {
            FileDocumento fileDocumento = null;
            var schedaDocumento = request.schedaDoc;
            try
            {
                var reportData = schedaDocumento.protocollo == null ? Resources.schedaProfiloDoc : Resources.schedaDocRtf;

                //Ultima nota documento
                var idCorrGlobali = request.infoUtente.idCorrGlobali.AsLong();
                var idUser = request.infoUtente.idPeople.AsLong();
                var idGruppo = request.infoUtente.idGruppo.AsLong();
                var noteDocumento = await _dbContext.NoteEntities.
                         Where(n => n.IDOGGETTOASSOCIATO == schedaDocumento.systemId.AsLong()
                            && (n.TIPOVISIBILITA == "T"
                            || (n.TIPOVISIBILITA == "F" && this._dbContext.RuoloRegistroEntities.AsNoTracking().Any(r => r.ID_REGISTRO == n.IDRFASSOCIATO && r.ID_RUOLO_IN_UO == idCorrGlobali))
                            || (n.TIPOVISIBILITA == "P" && n.IDUTENTECREATORE == idUser)
                            || (n.TIPOVISIBILITA == "R" && n.IDRUOLOCREATORE == idGruppo))
                         )
                         .OrderByDescending(n => n.DATACREAZIONE)
                         .Select(n => n.TESTO)
                         .FirstOrDefaultAsync();

                if (schedaDocumento.protocollo == null)
                {
                    var paroleChiavi = string.Empty;
                    if(schedaDocumento.paroleChiave != null && schedaDocumento.paroleChiave.Count() > 0)
                    {
                        schedaDocumento.paroleChiave.ForEach(p =>
                        {
                            paroleChiavi += (!string.IsNullOrEmpty(paroleChiavi) ? "; " : string.Empty) + p.descrizione;
                        });
                    }

                    reportData = reportData.Replace("XIDDOC", schedaDocumento.docNumber);
                    reportData = reportData.Replace("XDTACREAZIONE", schedaDocumento.dataCreazione);
                    reportData = reportData.Replace("XOGG", schedaDocumento.oggetto.descrizione);
                    reportData = reportData.Replace("XPAROLECHIAVI", paroleChiavi);
                    reportData = reportData.Replace("XTIPODOC", schedaDocumento.tipologiaAtto != null ? schedaDocumento.tipologiaAtto.descrizione : string.Empty);
                    reportData = reportData.Replace("XNOTE", noteDocumento != null ? noteDocumento : string.Empty);
                }
                else
                {
                    var mittDest = string.Empty;
                    var protMitt = string.Empty;
                    if (schedaDocumento.protocollo.GetType().Equals(typeof(ProtocolloEntrata)))
                    {
                        mittDest = ((ProtocolloEntrata)schedaDocumento.protocollo).mittente.descrizione;
                        protMitt = ((ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente + ((ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente;
                    }
                    else
                    {
                        var dest = ((ProtocolloUscita)schedaDocumento.protocollo).destinatari;
                        for (int i = 0; i < dest.Count(); i++)
                        {
                            mittDest = mittDest + (dest[i]).descrizione;
                            if (i < (dest.Count() - 1))
                            {
                                mittDest = mittDest + " \\par ";
                            }
                        }
                        var destCC = ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
                        if (destCC != null && destCC.Count() > 0)
                        {
                            mittDest = mittDest + " \\par ";
                            for (int i = 0; i < destCC.Count(); i++)
                            {
                                mittDest = mittDest + (destCC[i]).descrizione;
                                if (i < (destCC.Count() - 1))
                                {
                                    mittDest = mittDest + " \\par ";
                                }
                            }
                        }
                    }
                    //classifica
                    var classifica = string.Empty;
                    var codice = string.Empty;
                    var fascicoli = (await _mediator.Send(new Requests.FascicolazioneGetFascicoliDaDoc(request.infoUtente, schedaDocumento.docNumber))).output;
                    for (int i = 0; i < fascicoli.Count(); i++)
                    {
                        //questo serve in caso nel codice fascicolo sia utilizzato come separatore il back slash "\", solo cos� � stampato nel file rtf...
                        codice = (fascicoli[i]).codice.Replace(@"\", @"{\rtlch\fcs1 \af0\afs20 \ltrch\fcs0 \fs20\lang1040\langfe1033\langnp1040\insrsid4612691 \\}");
                        classifica = classifica + " " + codice;
                        if (i < (fascicoli.Count() - 1))
                        {
                            classifica = classifica + " \\par";
                        }
                    }

                    reportData = reportData.Replace("XNOME_REGISTRO", schedaDocumento.registro != null ? schedaDocumento.registro.descrizione : string.Empty);
                    reportData = reportData.Replace("XCODICE_REGISTRO", schedaDocumento.registro != null ? schedaDocumento.registro.codRegistro : string.Empty);
                    reportData = reportData.Replace("XNUM_PR", schedaDocumento.protocollo.numero);
                    reportData = reportData.Replace("XDTA_PR", schedaDocumento.protocollo.dataProtocollazione);
                    reportData = reportData.Replace("XA/P", schedaDocumento.tipoProto);
                    reportData = reportData.Replace("XMITT/DEST", mittDest);
                    reportData = reportData.Replace("XPROT_MITT", protMitt);
                    reportData = reportData.Replace("XCLASSIFICA", classifica);
                    reportData = reportData.Replace("XRISPOSTA", "");
                    reportData = reportData.Replace("XOGGETTO", schedaDocumento.oggetto.descrizione);
                    reportData = reportData.Replace("XNOTE", noteDocumento != null ? noteDocumento : string.Empty);
                }

                fileDocumento = new FileDocumento();
                fileDocumento.content = Encoding.UTF8.GetBytes(reportData);
                fileDocumento.length = fileDocumento.content.Length;
                fileDocumento.contentType = "application/rtf";
                fileDocumento.name = "report.rtf";
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                fileDocumento = null;
            }

            return new ReportSchedaDocResult(fileDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ReportSchedaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}