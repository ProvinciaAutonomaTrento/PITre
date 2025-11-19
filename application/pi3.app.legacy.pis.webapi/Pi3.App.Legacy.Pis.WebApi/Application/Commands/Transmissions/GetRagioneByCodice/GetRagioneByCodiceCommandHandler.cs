// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Collections;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetRagioneByCodice
{
    public class GetRagioneByCodiceCommandHandler : IRequestHandler<GetRagioneByCodiceCommand, GetRagioneByCodiceCommandResponse>
    {
        public GetRagioneByCodiceCommandHandler(
            ILogger<GetRagioneByCodiceCommandHandler> logger,
            IPi3DbContext pi3DbContext
            )
        {
            this._logger = logger;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetRagioneByCodiceCommandResponse> Handle(GetRagioneByCodiceCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione? ragioneTrasmissione = null;
            var codice = request.Codice;
            var idAmm = request.IdAmm;
            try
            {

                var rag = this._pi3DbContext.RagioneTrasmissioneEntities.AsNoTracking().Where(t => 
                t.VAR_DESC_RAGIONE == codice && 
                t.ID_AMM == idAmm.AsLong()).Select(t => new
                {
                    t.SYSTEM_ID,
                    t.VAR_DESC_RAGIONE,
                    t.CHA_TIPO_RAGIONE,
                    t.CHA_TIPO_DIRITTI,
                    t.CHA_RISPOSTA,
                    t.CHA_TIPO_DEST,
                    t.VAR_NOTE,
                    t.CHA_EREDITA,
                    t.CHA_TIPO_RISPOSTA,
                    t.VAR_NOTIFICA_TRASM,
                    t.VAR_TESTO_MSG_NOTIFICA_DOC,
                    t.VAR_TESTO_MSG_NOTIFICA_FASC,
                    t.CHA_CEDE_DIRITTI,
                    t.CHA_MANTIENI_LETT,
                    t.CHA_MANTIENI_SCRITT
                }).ToList();

                if (rag != null && rag.Count() > 0)
                {
                    foreach (var r in rag)
                    {
                        ragioneTrasmissione = new()
                        {
                            systemId = r.SYSTEM_ID.ToString(),
                            descrizione = r.VAR_DESC_RAGIONE,
                            tipo = r.CHA_TIPO_RAGIONE,
                            tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)this.GetKeyFromVal(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, r.CHA_TIPO_DIRITTI),
                            risposta = r.CHA_RISPOSTA,
                            tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)this.GetKeyFromVal(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, r.CHA_TIPO_DEST),
                            note = r.VAR_NOTE,
                            eredita = r.CHA_EREDITA,
                            tipoRisposta = r.CHA_TIPO_RISPOSTA,
                            notifica = r.VAR_NOTIFICA_TRASM,
                            testoMsgNotificaDoc = r.VAR_TESTO_MSG_NOTIFICA_DOC,
                            testoMsgNotificaFasc = r.VAR_TESTO_MSG_NOTIFICA_FASC,
                            prevedeCessione = r.CHA_CEDE_DIRITTI ?? "N",
                            mantieniLettura = r.CHA_MANTIENI_LETT ?? "false",
                            mantieniScrittura = r.CHA_MANTIENI_SCRITT ?? "false"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ragioneTrasmissione = null;
                this._logger.LogError(exception : ex, message : ex.Message);
            }

            return new()
            {
                Output  = ragioneTrasmissione
            };
        }


        #region Private Members

        protected readonly ILogger<GetRagioneByCodiceCommandHandler> _logger;
        protected readonly IPi3DbContext _pi3DbContext;


        private object GetKeyFromVal(Hashtable ht, string val)
        {
            object res = null;
            IEnumerator keys = ht.Keys.GetEnumerator();

            while (keys.MoveNext())
            {
                if (((string)ht[keys.Current]).Equals(val))
                {
                    res = keys.Current;
                }
            }

            return res;
        }
        #endregion
    }
}
