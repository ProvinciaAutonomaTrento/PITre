// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetRagione
{
    public class GetRagioneCommandHandler : IRequestHandler<GetRagioneCommand, GetRagioneCommandResponse>
    {
        public GetRagioneCommandHandler(
            ILogger<GetRagioneCommandHandler> logger,
            IPi3DbContext dbContext
            ) 
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        public async Task<GetRagioneCommandResponse> Handle(GetRagioneCommand request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.trasmissione.RagioneTrasmissione> res = new();

            try
            {

                List<RagTrasmDTO> query = null;

                switch (request.TipoDest)
                {
                    case "COMPETENZA":
                        query = await (from a in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                                       from b in this._dbContext.AmministraEntities.AsNoTracking()
                                       where a.ID_AMM == request.IdAmm.AsLong() &&
                                       a.ID_AMM == b.SYSTEM_ID &&
                                       a.SYSTEM_ID == b.ID_RAGIONE_COMPETENZA
                                       select new RagTrasmDTO()
                                       {
                                           SystemId = a.SYSTEM_ID,
                                           VarDescRagione = a.VAR_DESC_RAGIONE,
                                           ChaTipoRagione = a.CHA_TIPO_RAGIONE,
                                           ChaTipoDiritti = a.CHA_TIPO_DIRITTI,
                                           ChaRisposta = a.CHA_RISPOSTA,
                                           ChaTipoDest = a.CHA_TIPO_DEST,
                                           VarNote = a.VAR_NOTE,
                                           ChaEredita = a.CHA_EREDITA,
                                           ChaTipoRisposta = a.CHA_TIPO_RISPOSTA,
                                           VarNotificaTrasm = a.VAR_NOTIFICA_TRASM,
                                           VarTestoMsgNotificaDoc = a.VAR_TESTO_MSG_NOTIFICA_DOC,
                                           VarTestoMsgNotificaFasc = a.VAR_TESTO_MSG_NOTIFICA_FASC,
                                           ChaCedeDiritti = a.CHA_CEDE_DIRITTI,
                                           ChaMantieniLett = a.CHA_MANTIENI_LETT
                                       }).ToListAsync();

                        break;
                    case "CONOSCENZA":
                        query = await (from a in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                                       from b in this._dbContext.AmministraEntities.AsNoTracking()
                                       where a.ID_AMM == request.IdAmm.AsLong() &&
                                       a.ID_AMM == b.SYSTEM_ID &&
                                       a.SYSTEM_ID == b.ID_RAGIONE_CONOSCENZA
                                       select new RagTrasmDTO()
                                       {
                                           SystemId = a.SYSTEM_ID,
                                           VarDescRagione = a.VAR_DESC_RAGIONE,
                                           ChaTipoRagione = a.CHA_TIPO_RAGIONE,
                                           ChaTipoDiritti = a.CHA_TIPO_DIRITTI,
                                           ChaRisposta = a.CHA_RISPOSTA,
                                           ChaTipoDest = a.CHA_TIPO_DEST,
                                           VarNote = a.VAR_NOTE,
                                           ChaEredita = a.CHA_EREDITA,
                                           ChaTipoRisposta = a.CHA_TIPO_RISPOSTA,
                                           VarNotificaTrasm = a.VAR_NOTIFICA_TRASM,
                                           VarTestoMsgNotificaDoc = a.VAR_TESTO_MSG_NOTIFICA_DOC,
                                           VarTestoMsgNotificaFasc = a.VAR_TESTO_MSG_NOTIFICA_FASC,
                                           ChaCedeDiritti = a.CHA_CEDE_DIRITTI,
                                           ChaMantieniLett = a.CHA_MANTIENI_LETT
                                       }).ToListAsync();


                        break;
                    case "REFERENTE":
                        query = await (from a in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                                       from b in this._dbContext.AmministraEntities.AsNoTracking()
                                       where a.ID_AMM == request.IdAmm.AsLong() &&
                                       a.ID_AMM == b.SYSTEM_ID &&
                                       a.SYSTEM_ID == b.ID_RAGIONE_REFERENTE
                                       select new RagTrasmDTO()
                                       {
                                           SystemId = a.SYSTEM_ID,
                                           VarDescRagione = a.VAR_DESC_RAGIONE,
                                           ChaTipoRagione = a.CHA_TIPO_RAGIONE,
                                           ChaTipoDiritti = a.CHA_TIPO_DIRITTI,
                                           ChaRisposta = a.CHA_RISPOSTA,
                                           ChaTipoDest = a.CHA_TIPO_DEST,
                                           VarNote = a.VAR_NOTE,
                                           ChaEredita = a.CHA_EREDITA,
                                           ChaTipoRisposta = a.CHA_TIPO_RISPOSTA,
                                           VarNotificaTrasm = a.VAR_NOTIFICA_TRASM,
                                           VarTestoMsgNotificaDoc = a.VAR_TESTO_MSG_NOTIFICA_DOC,
                                           VarTestoMsgNotificaFasc = a.VAR_TESTO_MSG_NOTIFICA_FASC,
                                           ChaCedeDiritti = a.CHA_CEDE_DIRITTI,
                                           ChaMantieniLett = a.CHA_MANTIENI_LETT
                                       }).ToListAsync();
                        break;
                    case "TO":
                        query = await (from a in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                                       from b in this._dbContext.AmministraEntities.AsNoTracking()
                                       where a.ID_AMM == request.IdAmm.AsLong() &&
                                       a.ID_AMM == b.SYSTEM_ID &&
                                       a.SYSTEM_ID == b.ID_RAGIONE_TO
                                       select new RagTrasmDTO()
                                       {
                                           SystemId = a.SYSTEM_ID,
                                           VarDescRagione = a.VAR_DESC_RAGIONE,
                                           ChaTipoRagione = a.CHA_TIPO_RAGIONE,
                                           ChaTipoDiritti = a.CHA_TIPO_DIRITTI,
                                           ChaRisposta = a.CHA_RISPOSTA,
                                           ChaTipoDest = a.CHA_TIPO_DEST,
                                           VarNote = a.VAR_NOTE,
                                           ChaEredita = a.CHA_EREDITA,
                                           ChaTipoRisposta = a.CHA_TIPO_RISPOSTA,
                                           VarNotificaTrasm = a.VAR_NOTIFICA_TRASM,
                                           VarTestoMsgNotificaDoc = a.VAR_TESTO_MSG_NOTIFICA_DOC,
                                           VarTestoMsgNotificaFasc = a.VAR_TESTO_MSG_NOTIFICA_FASC,
                                           ChaCedeDiritti = a.CHA_CEDE_DIRITTI,
                                           ChaMantieniLett = a.CHA_MANTIENI_LETT
                                       }).ToListAsync();
                        break;
                    case "CC":
                        query = await (from a in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                                       from b in this._dbContext.AmministraEntities.AsNoTracking()
                                       where a.ID_AMM == request.IdAmm.AsLong() &&
                                       a.ID_AMM == b.SYSTEM_ID &&
                                       a.SYSTEM_ID == b.ID_RAGIONE_CC
                                       select new RagTrasmDTO()
                                       {
                                           SystemId = a.SYSTEM_ID,
                                           VarDescRagione = a.VAR_DESC_RAGIONE,
                                           ChaTipoRagione = a.CHA_TIPO_RAGIONE,
                                           ChaTipoDiritti = a.CHA_TIPO_DIRITTI,
                                           ChaRisposta = a.CHA_RISPOSTA,
                                           ChaTipoDest = a.CHA_TIPO_DEST,
                                           VarNote = a.VAR_NOTE,
                                           ChaEredita = a.CHA_EREDITA,
                                           ChaTipoRisposta = a.CHA_TIPO_RISPOSTA,
                                           VarNotificaTrasm = a.VAR_NOTIFICA_TRASM,
                                           VarTestoMsgNotificaDoc = a.VAR_TESTO_MSG_NOTIFICA_DOC,
                                           VarTestoMsgNotificaFasc = a.VAR_TESTO_MSG_NOTIFICA_FASC,
                                           ChaCedeDiritti = a.CHA_CEDE_DIRITTI,
                                           ChaMantieniLett = a.CHA_MANTIENI_LETT
                                       }).ToListAsync();
                        break;
                }
                Hashtable tipoDirittoStringa = new Hashtable()
                {
                    {DocsPaVO.trasmissione.TipoDiritto.READ,"R" },
                    {DocsPaVO.trasmissione.TipoDiritto.WRITE,"W" },
                    {DocsPaVO.trasmissione.TipoDiritto.NONE, "N" },
                };

                Hashtable tipoGerarchiaStringa = new Hashtable()
                {
                    {DocsPaVO.trasmissione.TipoGerarchia.INFERIORE,"I"},
                    {DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE,"S"},
                    {DocsPaVO.trasmissione.TipoGerarchia.TUTTI,"T"},
                    {DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO,"P"}
                };
                DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
                res = new();

                query.ForEach(row =>
                {
                    objRagione = new();

                    objRagione.systemId = row.SystemId.ToString();
                    objRagione.descrizione = row.VarDescRagione;
                    objRagione.tipo = row.ChaTipoRagione;
                    objRagione.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)this.GetKeyFromVal(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, row.ChaTipoDiritti);
                    objRagione.risposta = row.ChaRisposta;
                    objRagione.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)this.GetKeyFromVal(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, row.ChaTipoDest);
                    objRagione.note = row.VarNote;
                    objRagione.eredita = row.ChaEredita;
                    objRagione.tipoRisposta = row.ChaTipoRisposta;
                    objRagione.notifica = row.VarNotificaTrasm;
                    objRagione.testoMsgNotificaDoc = row.VarTestoMsgNotificaDoc;
                    objRagione.testoMsgNotificaFasc = row.VarTestoMsgNotificaFasc;
                    objRagione.prevedeCessione = row.ChaCedeDiritti != null ? row.ChaCedeDiritti : "N";
                    objRagione.mantieniLettura = row.ChaMantieniLett != null ? row.ChaMantieniLett : "false";
                    objRagione.mantieniScrittura = "false";
                    res.Add(objRagione);

                });



            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new()
            {
                Output = res.FirstOrDefault()
            };
        }

        #region Private Members
        protected readonly ILogger<GetRagioneCommandHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


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

        private class RagTrasmDTO
        {
            public long? SystemId { get; set; }
            public string? VarDescRagione { get; set; }
            public string? ChaTipoRagione { get; set; }
            public string? ChaTipoDiritti { get; set; }
            public string? ChaRisposta { get; set; }
            public string? ChaTipoDest { get; set; }

            public string? VarNote { get; set; }

            public string? ChaEredita { get; set; }
            public string? ChaTipoRisposta { get; set; }
            public string? VarNotificaTrasm { get; set; }
            public string? VarTestoMsgNotificaDoc { get; set; }
            public string? VarTestoMsgNotificaFasc { get; set; }
            public string? ChaCedeDiritti { get; set; }
            public string? ChaMantieniLett { get; set; }


        }
        #endregion
    }
}
