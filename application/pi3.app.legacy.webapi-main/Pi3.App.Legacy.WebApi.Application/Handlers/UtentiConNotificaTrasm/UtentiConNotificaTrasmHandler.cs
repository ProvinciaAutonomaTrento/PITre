// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using UtentiConNotificaTrasmRequest = Pi3.App.Legacy.WebApi.Application.Requests.UtentiConNotificaTrasm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UtentiConNotificaTrasm
{
    public class UtentiConNotificaTrasmHandler : IRequestHandler<UtentiConNotificaTrasmRequest, UtentiConNotificaTrasmResult>
    {

        protected readonly ILogger<UtentiConNotificaTrasmHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        private string getFlagNotifica(long idModello, long idPeople)
        {
            string result = "0";
            if (idModello != 0 && this._dbContext.ModelloDestConNotificaEntities.Any(dcn => dcn.ID_MODELLO_MITT_DEST == idModello && dcn.ID_PEOPLE == idPeople))
            {
                result = "1";
            }
            return result;
        }

        private async Task<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> UtentiConNotificaTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, Object[] utentiDaInserire, Object[] utentiDaCancellare, string operazione)
        {
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
            DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utNotifica;
            string commandText = string.Empty;
            modello = objModTrasm;

            try
            {
                foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest ragioneDest in objModTrasm.RAGIONI_DESTINATARI)
                {
                    foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in ragioneDest.DESTINATARI)
                    {
                        // DESTINATARIO RUOLO
                        if (mittDest.CHA_TIPO_MITT_DEST.Equals("D") && mittDest.CHA_TIPO_URP.Equals("R"))
                        {
                            switch (operazione)
                            {
                                case "GET":
                                    var rows = (from p in this._dbContext.PeopleEntities.AsNoTracking()
                                                from pg in this._dbContext.PeopleGroupEntities.AsNoTracking()
                                                from cg in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                                where cg.SYSTEM_ID == mittDest.ID_CORR_GLOBALI &&
                                                cg.ID_GRUPPO == pg.GROUPS_SYSTEM_ID &&
                                                pg.PEOPLE_SYSTEM_ID == p.SYSTEM_ID &&
                                                !pg.DTA_FINE.HasValue
                                                orderby p.VAR_COGNOME ascending
                                                select new
                                                {
                                                    ID = p.SYSTEM_ID,
                                                    CODICE = p.USER_ID,
                                                    UTENTE = p.FULL_NAME,
                                                    RUOLO = cg.VAR_DESC_CORR,
                                                    P_SYSTEM_ID = p.SYSTEM_ID,
                                                    p.VAR_COGNOME
                                                }).AsEnumerable().Select(row => new
                                                {
                                                    row.ID,
                                                    row.CODICE,
                                                    row.UTENTE,
                                                    row.RUOLO,
                                                    FLAG_NOTIFICA = getFlagNotifica(mittDest.SYSTEM_ID, row.P_SYSTEM_ID),
                                                    row.VAR_COGNOME

                                                });

                                    List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm> utenteNotifica = new List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm>();
                                    foreach (var row in rows)
                                    {
                                        utNotifica = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                                        utNotifica.ID_PEOPLE = row.ID.ToString();
                                        utNotifica.CODICE_UTENTE = row.CODICE.ToString();
                                        utNotifica.NOME_COGNOME_UTENTE = row.UTENTE.ToString();
                                        utNotifica.FLAG_NOTIFICA = row.FLAG_NOTIFICA.ToString();
                                        utNotifica.ID_MODELLO_MITT_DEST = mittDest.SYSTEM_ID.ToString();

                                        utenteNotifica.Add(utNotifica);
                                    }
                                    mittDest.UTENTI_NOTIFICA = utenteNotifica.ToArray();
                                    break;

                                case "SET":
                                    if (utentiDaInserire != null && utentiDaInserire.Count() > 0)
                                    {
                                        foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utIns in utentiDaInserire)
                                        {
                                            string? resSystemId = "0";
                                            resSystemId = await this._dbContext.ModelloDestConNotificaEntities.AsNoTracking()
                                                .Where(md => md.ID_PEOPLE == utIns.ID_PEOPLE.AsLong() &&
                                                        md.ID_MODELLO_MITT_DEST == utIns.ID_MODELLO_MITT_DEST.AsLong() &&
                                                        md.ID_MODELLO == mittDest.ID_MODELLO).Select(md => md.SYSTEM_ID.ToString()).FirstOrDefaultAsync();
                                            if (resSystemId == null)
                                            {
                                                ModelloDestConNotificaEntity modelloDestConNotifica = new ModelloDestConNotificaEntity()
                                                {
                                                    ID_MODELLO_MITT_DEST = utIns.ID_MODELLO_MITT_DEST.AsLong(),
                                                    ID_PEOPLE = utIns.ID_PEOPLE.AsLong(),
                                                    ID_MODELLO = mittDest.ID_MODELLO
                                                };

                                                await this._dbContext.ModelloDestConNotificaEntities.AddAsync(modelloDestConNotifica);

                                                int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();
                                            }
                                        }
                                    }

                                    if (utentiDaCancellare != null && utentiDaCancellare.Count() > 0)
                                    {
                                        foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utCanc in utentiDaCancellare)
                                        {
                                            var listForDel = await this._dbContext.ModelloDestConNotificaEntities.
                                                Where(row => row.ID_MODELLO_MITT_DEST == utCanc.ID_MODELLO_MITT_DEST.AsLong() && row.ID_PEOPLE == utCanc.ID_PEOPLE.AsLong()).ToListAsync();

                                            foreach (var el in listForDel)
                                            {
                                                ((DbContext)_dbContext).Remove(el);
                                            }
                                            ((DbContext)_dbContext).SaveChanges();

                                        }
                                    }

                                    if (utentiDaInserire == null && utentiDaCancellare == null)
                                    {
                                        foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                                        {
                                            if (ut.FLAG_NOTIFICA.Equals("1"))
                                            {
                                                ModelloDestConNotificaEntity modelloDestConNotifica = new ModelloDestConNotificaEntity()
                                                {
                                                    ID_MODELLO_MITT_DEST = mittDest.SYSTEM_ID,
                                                    ID_PEOPLE = ut.ID_PEOPLE.AsLong(),
                                                    ID_MODELLO = mittDest.ID_MODELLO
                                                };

                                                await this._dbContext.ModelloDestConNotificaEntities.AddAsync(modelloDestConNotifica);

                                                int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();
                                            }
                                        }
                                    }

                                    break;
                            }
                        }

                        if (mittDest.CHA_TIPO_MITT_DEST.Equals("D") && mittDest.CHA_TIPO_URP.Equals("P"))
                        {

                            switch (operazione)
                            {
                                case "GET":
                                    var rows = (from p in this._dbContext.PeopleEntities.AsNoTracking()
                                                let innerSId = (from c in this._dbContext.CorrGlobaliEntities where c.SYSTEM_ID == mittDest.ID_CORR_GLOBALI select c.ID_PEOPLE).First()
                                                where p.SYSTEM_ID == innerSId && p.DISABLED.Equals("N")
                                                select new
                                                {
                                                    ID = p.SYSTEM_ID,
                                                    CODICE = p.USER_ID,
                                                    UTENTE = p.FULL_NAME,
                                                    RUOLO = "",

                                                }).AsEnumerable().Select(row => new
                                                {
                                                    row.ID,
                                                    row.CODICE,
                                                    row.UTENTE,
                                                    row.RUOLO,
                                                    FLAG_NOTIFICA = getFlagNotifica(mittDest.SYSTEM_ID, row.ID)
                                                });

                                    List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm> utentiConNotificaTrasms = new List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm>();
                                    foreach (var row in rows)
                                    {
                                        utNotifica = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                                        utNotifica.ID_PEOPLE = row.ID.ToString();
                                        utNotifica.CODICE_UTENTE = row.CODICE.ToString();
                                        utNotifica.NOME_COGNOME_UTENTE = row.UTENTE.ToString();
                                        utNotifica.FLAG_NOTIFICA = row.FLAG_NOTIFICA.ToString();
                                        utNotifica.ID_MODELLO_MITT_DEST = mittDest.SYSTEM_ID.ToString();
                                        utentiConNotificaTrasms.Add(utNotifica);
                                    }
                                    mittDest.UTENTI_NOTIFICA = utentiConNotificaTrasms.ToArray();
                                    break;

                                case "SET":
                                    var listForDel = await this._dbContext.ModelloDestConNotificaEntities.
                                     Where(row => row.ID_MODELLO_MITT_DEST == mittDest.SYSTEM_ID).ToListAsync();

                                    foreach (var el in listForDel)
                                    {
                                        ((DbContext)_dbContext).Remove(el);
                                    }
                                    ((DbContext)_dbContext).SaveChanges();

                                    foreach (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm ut in mittDest.UTENTI_NOTIFICA)
                                    {
                                        if (ut.FLAG_NOTIFICA.Equals("1"))
                                        {

                                            ModelloDestConNotificaEntity modelloDestConNotifica = new ModelloDestConNotificaEntity()
                                            {
                                                ID_MODELLO_MITT_DEST = mittDest.SYSTEM_ID,
                                                ID_PEOPLE = ut.ID_PEOPLE.AsLong(),
                                                ID_MODELLO = mittDest.ID_MODELLO
                                            };

                                            await this._dbContext.ModelloDestConNotificaEntities.AddAsync(modelloDestConNotifica);

                                            int rows_affected = await ((DbContext)this._dbContext).SaveChangesAsync();
                                        }
                                    }


                                    break;
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                objModTrasm = modello;
                this._logger.LogWebMethodError(ex);
            }

            return objModTrasm;
        }

        public UtentiConNotificaTrasmHandler(
            ILogger<UtentiConNotificaTrasmHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<UtentiConNotificaTrasmResult> Handle(UtentiConNotificaTrasmRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione output = new();
            try
            {
                output = await this.UtentiConNotificaTrasm(request.objModTrasm,request.utentiDaInserire,request.utentiDaCancellare,request.operazione);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new UtentiConNotificaTrasmResult(output);
        }
    }
}
