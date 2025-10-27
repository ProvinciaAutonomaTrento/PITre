// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.filtri;
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FindAndReplaceRoleInModelliTrasmissioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.FindAndReplaceRoleInModelliTrasmissione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FindAndReplaceRoleInModelliTrasmissione
{
    public class FindAndReplaceRoleInModelliTrasmissioneHandler : IRequestHandler<FindAndReplaceRoleInModelliTrasmissioneRequest, FindAndReplaceRoleInModelliTrasmissioneResult>
    {

        protected readonly ILogger<FindAndReplaceRoleInModelliTrasmissioneHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;





        private async Task<FindAndReplaceResponse> FindAndReplaceRuoli(
            FindAndReplaceRequest request)
        {
            FindAndReplaceResponse response = new FindAndReplaceResponse();

            switch (request.Operation)
            {
                case FindAndReplaceRequest.FindAndReplaceEnum.Find:  // Ricerca
                    // Ricerca di tutti i modelli che soddisfano i criteri di ricerca passati ed estrazione
                    // di una collection con i dati di interesse e verifica della possibilità di effettuare l'operazione
                    // di sostituzione sui modelli individuati
                    response.Models = await this.FindModelliTrasmissione(request.SearchFilters, request.UserInfo, request.IsAdministrator);
                    this.AnalyzeModelliTrasmissione(response.Models, request.RoleToReplace);
                    break;
                case FindAndReplaceRequest.FindAndReplaceEnum.Replace:
                    // Applicazione delle azioni richieste e restituzione di un report
                    // con l'esito effettivo dell'azione
                    response.Models = await this.ExecuteFindAndReplace(request);
                    break;
                default:
                    break;
            }

            return response;
        }

        private void AnalyzeModelliTrasmissione(ModelloTrasmissioneSearchResultCollection modelloTrasmissioneSearchResultCollection, DocsPaVO.rubrica.ElementoRubrica roleToReplace)
        {
            // Il modello potrebbe subire modifiche se il ruolo da sostituire compare fra i destinatari
            foreach (var modello in modelloTrasmissioneSearchResultCollection)
            {
                bool containsCorr = modello.Destinatari.Where(e => e.Corrispondente.systemId == roleToReplace.systemId).Count() > 0;
                modello.Message = String.Format("Il modello {0} modifiche in quanto il ruolo da sostituire{1}compare fra i destinatari.",
                    containsCorr ? "potrebbe subire" : "non subirà", containsCorr ? " " : " non ");

                modello.SyntheticResult = containsCorr ?
                    ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.OK :
                    ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.KO;

            }
        }

        private class ModelloTrasmInfo
        {
            public long IdModello { get; set; }
            public string? Nome { get; set; }   
            public long Idmittdest { get; set; }   
            public string? Tipotrasmissione { get; set; }   
            public long Idcorrispondente { get; set; }   
            public long? Numerolivello { get; set; }   
            public string? Mittdest { get; set; }   
            public long? IdPeople { get; set; }   
            public string? VarNoteGenerali { get; set; }   
            public string? CharTipoOg { get; set; }   
            public long? IdRegistro { get; set; }   
        }

        private async Task<ModelloTrasmissioneSearchResultCollection> FindModelliTrasmissione(FiltroRicerca[] searchFilters, DocsPaVO.utente.InfoUtente userInfo, bool administrator)
        {
            ModelloTrasmissioneSearchResultCollection retVal = new ModelloTrasmissioneSearchResultCollection();
            IQueryable<ModelloTrasmInfo> query;
            if (administrator)
            {
                query = (from a in this._dbContext.ModelloTrasmEntities.AsNoTracking()
                         from b in this._dbContext.ModelloMittDestEntities.AsNoTracking()
                         from c in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                         from d in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                         from e in this._dbContext.TipoRuoloEntities.AsNoTracking()
                         where b.CHA_TIPO_URP.Equals("R") &&
                         a.ID_AMM == userInfo.idAmministrazione.AsLong() &&
                         a.SYSTEM_ID == b.ID_MODELLO &&
                         b.ID_RAGIONE == c.SYSTEM_ID &&
                         b.ID_CORR_GLOBALI == d.SYSTEM_ID &&
                         d.ID_TIPO_RUOLO == e.SYSTEM_ID
                         orderby a.SYSTEM_ID
                         select new ModelloTrasmInfo()
                         {
                             IdModello = a.SYSTEM_ID,
                             Nome = a.NOME,
                             Idmittdest = b.SYSTEM_ID,
                             Tipotrasmissione = c.CHA_TIPO_DEST,
                             Idcorrispondente = b.ID_CORR_GLOBALI,
                             Numerolivello = e.NUM_LIVELLO,
                             Mittdest = b.CHA_TIPO_MITT_DEST,
                             IdPeople = d.ID_PEOPLE,
                             VarNoteGenerali = a.VAR_NOTE_GENERALI,
                             CharTipoOg = a.CHA_TIPO_OGGETTO,
                             IdRegistro = a.ID_REGISTRO
                         });
                
            }
            else
            {

                var subQuery = await (from mt in this._dbContext.ModelloTrasmEntities.AsNoTracking()
                               from md in this._dbContext.ModelloMittDestEntities.AsNoTracking()
                               let subCondition = (from assDiag in this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                                  where md.ID_MODELLO == assDiag.ID_MOD_TRASM
                                                  select assDiag.ID_MOD_TRASM).ToList()
                               where mt.SYSTEM_ID == md.ID_MODELLO &&
                               mt.ID_AMM == userInfo.idAmministrazione.AsLong() &&
                               (mt.ID_PEOPLE == null || mt.ID_PEOPLE == userInfo.idPeople.AsLong()) &&
                               md.CHA_TIPO_MITT_DEST != null && md.CHA_TIPO_MITT_DEST.Equals("M")  && 
                               mt.SINGLE.Equals("0")  &&
                               (md.ID_CORR_GLOBALI == userInfo.idCorrGlobali.AsLong() || md.ID_CORR_GLOBALI == 0) &&
                               !(subCondition.Any())
                               select mt.SYSTEM_ID).ToListAsync();

                query = (from a in this._dbContext.ModelloTrasmEntities.AsNoTracking()
                        from b in this._dbContext.ModelloMittDestEntities.AsNoTracking()
                        from c in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                        from d in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        from e in this._dbContext.TipoRuoloEntities.AsNoTracking()
                        where b.CHA_TIPO_URP.Equals("R") &&
                        a.SYSTEM_ID == b.ID_MODELLO &&
                        b.ID_RAGIONE == c.SYSTEM_ID &&
                        b.ID_CORR_GLOBALI == d.SYSTEM_ID &&
                        d.ID_TIPO_RUOLO == e.SYSTEM_ID &&
                        subQuery.Contains(a.SYSTEM_ID)
                        orderby a.SYSTEM_ID
                        select new ModelloTrasmInfo()
                        {
                            IdModello = a.SYSTEM_ID,
                            Nome = a.NOME,
                            Idmittdest = b.SYSTEM_ID,
                            Tipotrasmissione = c.CHA_TIPO_DEST,
                            Idcorrispondente = b.ID_CORR_GLOBALI,
                            Numerolivello = e.NUM_LIVELLO,
                            Mittdest = b.CHA_TIPO_MITT_DEST,
                            IdPeople = d.ID_PEOPLE,
                            VarNoteGenerali = a.VAR_NOTE_GENERALI,
                            CharTipoOg = a.CHA_TIPO_OGGETTO,
                            IdRegistro = a.ID_REGISTRO


                        });



            }
            var predicate = PredicateBuilder.New<ModelloTrasmInfo>();
            bool inFilter = false;
            foreach (DocsPaVO.filtri.FiltroRicerca filtro in searchFilters)
                {
                // Parsing del filtro di ricerca e aggiunta della condizione di filtro
                    listaArgomentiModelliTrasmissione filter =
                        (listaArgomentiModelliTrasmissione)
                        Enum.Parse(typeof(listaArgomentiModelliTrasmissione), filtro.argomento, true);
                inFilter = true;
                    switch (filter)
                    {
                        // Codice modello
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO:
                            if (!String.IsNullOrEmpty(filtro.valore))
                            {
                                string cond = filtro.valore.Substring(filtro.valore.IndexOf("_") + 1);
                                predicate = predicate.And(mi => mi.IdModello.ToString().Contains(cond.ToUpper().Replace("'", "''")));
                            }
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.DESCRIZIONE_MODELLO:
                            if (String.IsNullOrEmpty(filtro.valore))
                            {
                                predicate = predicate.And(mi => mi.IdPeople == null);
                            }
                            else 
                            {
                                predicate = predicate.And(mi => mi.Nome != null && mi.Nome.ToUpper().Contains(filtro.valore.Replace("'", "''").ToUpper()));
                            }
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.RUOLI_DISABLED_RIC_TRASM: //TESTA BENE
                            predicate = predicate.And(mi => 
                            (from mtd in this._dbContext.ModelloMittDestEntities
                            from c in this._dbContext.CorrGlobaliEntities
                            where mi.IdModello == mtd.ID_MODELLO &&
                            (
                            mtd.CHA_TIPO_URP != null && mtd.CHA_TIPO_URP.Equals("R") && 
                            mtd.CHA_TIPO_MITT_DEST != null && mtd.CHA_TIPO_MITT_DEST.Equals("D") &&
                            mtd.ID_CORR_GLOBALI == c.SYSTEM_ID &&
                            c.CHA_DISABLED_TRASM != null && c.CHA_DISABLED_TRASM.Equals("1")
                            )
                            select 'x')
                            .Any());
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.NOTE:
                            predicate = predicate.And(
                                mi => mi.VarNoteGenerali != null && mi.VarNoteGenerali.ToUpper().Contains(filtro.valore.Replace("'", "''").ToUpper()));
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.TIPO_TRASMISSIONE:
                            predicate = predicate.And(
                                    mi => mi.CharTipoOg != null && mi.CharTipoOg.ToUpper().Equals(filtro.valore.ToUpper()));
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.ID_REGISTRO:
                            predicate = predicate.And(
                                        mi => mi.IdRegistro != null && mi.IdRegistro == filtro.valore.AsLong());
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.ID_RAGIONE_TRASMISSIONE:
                            predicate = predicate.And(mi => (
                                from mtd in this._dbContext.ModelloMittDestEntities
                                from c in this._dbContext.CorrGlobaliEntities
                                where mi.IdModello == mtd.ID_MODELLO &&
                                mtd.ID_RAGIONE == filtro.valore.AsLong()
                                select 'x'
                                ).Any());
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_CORR_PER_VISIBILITA:
                            predicate = predicate.And(mi => (
                              from mtd in this._dbContext.ModelloMittDestEntities
                              from c in this._dbContext.CorrGlobaliEntities
                              where mi.IdModello == mtd.ID_MODELLO &&
                              ((c.VAR_CODICE!= null ? c.VAR_CODICE.ToUpper().Equals(filtro.valore.ToUpper()) : false )&&
                              c.SYSTEM_ID == mtd.ID_CORR_GLOBALI &&
                              (mtd.CHA_TIPO_MITT_DEST != null ? mtd.CHA_TIPO_MITT_DEST.ToUpper().Equals("M") : false))
                              select 'x'
                              ).Any());
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_CORR_PER_DESTINATARIO:
                            predicate = predicate.And(mi => (
                                from mtd in this._dbContext.ModelloMittDestEntities
                                from c in this._dbContext.CorrGlobaliEntities
                                where mi.IdModello == mtd.ID_MODELLO &&
                                ((c.VAR_CODICE != null ? c.VAR_CODICE.ToUpper().Equals(filtro.valore.ToUpper()) : false) &&
                                c.SYSTEM_ID == mtd.ID_CORR_GLOBALI &&
                                (mtd.CHA_TIPO_MITT_DEST != null ? mtd.CHA_TIPO_MITT_DEST.ToUpper().Equals("D") : false))
                                select 'x'
                                ).Any());
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.RUOLI_DEST_DISABLED:
                            predicate = predicate.And(mi => (
                                from mtd in this._dbContext.ModelloMittDestEntities
                                from c in this._dbContext.CorrGlobaliEntities
                                where mi.IdModello == mtd.ID_MODELLO &&
                                ((mtd.CHA_TIPO_URP != null ? mtd.CHA_TIPO_URP.Equals("R") : false) &&
                                (mtd.CHA_TIPO_MITT_DEST != null ? mtd.CHA_TIPO_MITT_DEST.Equals("D") : false) &&
                                mtd.ID_CORR_GLOBALI == c.SYSTEM_ID &&
                                !c.DTA_FINE.HasValue)
                                select 'x'
                            ).Any());
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_UTENTE:
                            predicate = predicate.And(mi => (
                                from mtd in this._dbContext.ModelloMittDestEntities
                                from c in this._dbContext.CorrGlobaliEntities   
                                where mi.IdModello == mtd.ID_MODELLO &&
                                mi.IdPeople != null
                                select 'x'
                            ).Any());
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_AMMINISTRATORE:
                            predicate = predicate.And(mi => (
                                from mtd in this._dbContext.ModelloMittDestEntities
                                from c in this._dbContext.CorrGlobaliEntities
                                where mi.IdModello == mtd.ID_MODELLO &&
                                mi.IdPeople != null
                                select 'x'
                            ).Any());
                            break;
                        default:
                            break;
                    }
                }
            List<ModelloTrasmInfo>? rows = null;
            if (inFilter)
            {
                rows = await query.Where(predicate).ToListAsync();
            }
            else
            {
                rows = await query.ToListAsync();

            }
            if (rows != null)
            {
                retVal = await this.AnalyzeDataSetAndCreateModelliTrasmissioneCollection(rows);

            }
            return retVal;

        }


        private async Task LoadMittentModelloFindAndReplace(ModelloTrasmissioneSearchResult modelToUpdate)
        {


            var innerCondition = await this._dbContext.ModelloMittDestEntities.AsNoTracking().Where(md => (md.CHA_TIPO_MITT_DEST != null ? md.CHA_TIPO_MITT_DEST.Equals("M") : false) && md.ID_MODELLO == modelToUpdate.IdModello.AsLong()).Select(md => md.ID_CORR_GLOBALI).ToListAsync();

            var query = (from cg in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                         from tr in this._dbContext.TipoRuoloEntities.AsNoTracking()
                         where cg.ID_TIPO_RUOLO == tr.SYSTEM_ID && innerCondition.Contains(cg.SYSTEM_ID)
                         select new
                         {
                             numerolivello = tr.NUM_LIVELLO,
                             system_id = cg.SYSTEM_ID
                         });

            foreach (var mitt in query)
                modelToUpdate.Mittenti.Add(new MittenteLite()
                {
                    Id = mitt.system_id.ToString(),
                    Livel = mitt.numerolivello != null ? ((long) mitt.numerolivello).ToString() : string.Empty
                });

        }


        private async Task<ModelloTrasmissioneSearchResultCollection> AnalyzeDataSetAndCreateModelliTrasmissioneCollection(List<ModelloTrasmInfo> dataSet)
        {
            ModelloTrasmissioneSearchResultCollection retVal = new ModelloTrasmissioneSearchResultCollection();
            ModelloTrasmissioneSearchResult? model = null;

            foreach (var dataRow in dataSet)
            {
                // Se IdModello della riga corrente è diverso da quello del modello in costruzione,
                // vuol dire che si sta analizzando un nuovo modello
                if (model == null || dataRow.IdModello.ToString() != model.IdModello)
                {
                    model = new ModelloTrasmissioneSearchResult(
                        dataRow.IdModello.ToString(),
                        dataRow.Nome != null ? dataRow.Nome.ToString() : string.Empty);


                    model.Destinatari.Add(new DestinatarioLite()
                    {
                        Id = dataRow.Idmittdest.ToString(),
                        TipoTrasmissione = dataRow.Tipotrasmissione != null ? dataRow.Tipotrasmissione.ToString() : string.Empty,
                        Corrispondente = new DocsPaVO.utente.Ruolo(
                                dataRow.Idcorrispondente.ToString(),
                                String.Empty,
                                String.Empty,
                                dataRow.Numerolivello != null ? ((long)dataRow.Numerolivello).ToString() : string.Empty,
                                String.Empty,
                                null,
                                null)
                    });

                    // Recupero dei  mittenti per il modello
                    await this.LoadMittentModelloFindAndReplace(model);

                    retVal.Add(model);

                }
                else
                {

                    model.Destinatari.Add(new DestinatarioLite()
                    {
                        Id = dataRow.Idmittdest.ToString(),
                        TipoTrasmissione = dataRow.Tipotrasmissione != null ? dataRow.Tipotrasmissione.ToString() : string.Empty,
                        Corrispondente = new DocsPaVO.utente.Ruolo(
                            dataRow.Idcorrispondente.ToString(),
                            String.Empty,
                            String.Empty,
                            dataRow.Numerolivello != null ? ((long)dataRow.Numerolivello).ToString() : string.Empty,
                            String.Empty,
                            null,
                            null)
                    });

                }
            }

            return retVal;

        }

        private async Task<ModelloTrasmissioneSearchResultCollection> ExecuteFindAndReplace(FindAndReplaceRequest request)
        {

            var oldRole = (await this._mediator.Send(new GetRuoloById((request.RoleToReplace.systemId)))).Output;
            var newRole = (await this._mediator.Send(new GetRuoloById((request.NewRole.systemId)))).Output;


            var users = await this.getListUtentiRuolo(newRole, true);

            int newRoleLivel, roleToReplaceLivel;
            newRoleLivel = Int32.Parse(newRole.livello);
            roleToReplaceLivel = Int32.Parse(oldRole.livello);

            foreach (var model in request.Models)
            {
                // Verifica della possibilità di eseguire l'azione
                String message = String.Empty;
                bool canContinue = this.CanExecuteReplace(model.Destinatari, model.Mittenti, request.NewRole.systemId, newRoleLivel, roleToReplaceLivel, request.RoleToReplace.systemId, model.IdModello, out message);

                if (!canContinue)
                {
                    model.Message = message;
                    model.SyntheticResult = ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.KO;
                }
                else
                {
                    await this.ReplaceDestinatariInModelloTrasmissione(model.IdModello, request.RoleToReplace.systemId, request.NewRole.systemId,
                        model.Destinatari.Where(e => e.Corrispondente.systemId == request.RoleToReplace.systemId).First(),
                        users, request.CopyNotes);

                    // Impostazione messaggio di successo
                    model.Message = "Modello di trasmissione modificato con successo";
                    model.SyntheticResult = ModelloTrasmissioneSearchResult.ModelloTrasmissioneSearchResultSynthetic.OK;
                }

            }

            return request.Models;



        }


        private async Task ReplaceDestinatariInModelloTrasmissione(String idModello, String idCorrGlobaleOldRole, String idCorrGlobaleNewRole, DestinatarioLite destinatario, List<InfoUtentiRuolo> users, bool copyNotes)
        {
            
                // Eliminazione delle informazioni relative ai destinatari, legati al ruolo da sostituire, da notificare 
                var infoDestEntities = await this._dbContext.ModelloDestConNotificaEntities.Where(row => row.ID_MODELLO_MITT_DEST == destinatario.Id.AsLong() && row.ID_MODELLO == idModello.AsLong()).ToListAsync();
                this._dbContext.ModelloDestConNotificaEntities.RemoveRange(infoDestEntities);
                await ((DbContext)this._dbContext).SaveChangesAsync();


                // Inserimento delle informazioni relative agli utenti legati al ruolo con cui sostituire, da notificare
                foreach (var user in users)
                {


                    ModelloDestConNotificaEntity entity = new ModelloDestConNotificaEntity() {
                        ID_MODELLO_MITT_DEST = destinatario.Id.AsLong(),
                        ID_PEOPLE = user.SystemId ,
                        ID_MODELLO = idModello.AsLong()

                    };

                    this._dbContext.ModelloDestConNotificaEntities.Add(entity);

                }

                await ((DbContext)this._dbContext).SaveChangesAsync();


                // Aggiornamento dell'id del ruolo destinatario della trasmissione

                var entToUpdate = this._dbContext.ModelloMittDestEntities.FirstOrDefault( mod => mod.SYSTEM_ID == destinatario.Id.AsLong() && mod.ID_CORR_GLOBALI == idCorrGlobaleOldRole.AsLong() );

                if(entToUpdate != null)
                {
                    entToUpdate.ID_CORR_GLOBALI = idCorrGlobaleNewRole.AsLong();
                    if (!copyNotes)
                    {
                        entToUpdate.VAR_NOTE_SING = string.Empty;

                    }
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }




        }



        private bool CanExecuteReplace(List<DestinatarioLite> destinatari, List<MittenteLite> senderRoles, string newRoleCorrId, int newRoleLivel, int roleToReplaceLivel, string roleToReplaceCorrId, String idModello, out string message)
        {
            StringBuilder mex = new StringBuilder();
            bool retVal = true;

            // Se il ruolo non compare nella lista dei destinatari -> ko
            if (destinatari.Where(e => e.Corrispondente.systemId == roleToReplaceCorrId).Count() == 0)
            {
                retVal = false;
                mex.AppendLine("Il ruolo da sostituire non compare nella lista dei destinatari");
            }

            // Se fra i destinatari compare anche il ruolo con cui sostituire -> ko
            if (destinatari.Where(e => e.Corrispondente.systemId == newRoleCorrId).Count() > 0)
            {
                retVal = false;
                mex.AppendLine("Il ruolo da utilizzare per la sostituzione compare nella lista dei destinatari");
            }

            if (retVal)
            {
                // Recupero del destinatario da sostituire
                DestinatarioLite dest = destinatari.Where(e => e.Corrispondente.systemId == roleToReplaceCorrId).First();

                // Se la ragione di trasmissione prevede delle restrizioni che non vengono rispettate dal ruolo con cui sostituire e dal ruolo mittente -> ko
                if (dest.TipoTrasmissione != "T")
                {
                    bool res = true;
                    String details = String.Empty;

                    switch (dest.TipoTrasmissione)
                    {
                        case "S":       // Sottoposti -> il livello del ruolo da utilizzare per la sostituzione deve essere superiore al numero di livello dei ruoli mittente
                            res = senderRoles.Where(e => newRoleLivel < Int32.Parse(e.Livel)).Count() != senderRoles.Count;
                            details = "Il ruolo da utilizzare per la sostituzione deve essere gerarchicamente superiore ai ruoli mittenti";
                            break;

                        case "P":       // Parilivello -> il livello del ruolo da utilizzare per la sostituzione deve essere uguale al numero di livello dei ruoli mittente
                            res = senderRoles.Where(e => newRoleLivel == Int32.Parse(e.Livel)).Count() != senderRoles.Count;
                            details = "Il ruolo da utilizzare per la sostituzione deve essere gerarchicamente un parilivello dei ruoli mittenti";
                            break;

                        case "I":       // Inferiori -> il livello del ruolo da utilzzare per la sostituzione deve essere inferiore al numero di livello dei ruoli mittente
                            res = senderRoles.Where(e => newRoleLivel > Int32.Parse(e.Livel)).Count() != senderRoles.Count;
                            details = "Il ruolo da utilizzare per la sostituzione deve essere gerarchicamente inferiore ai ruoli mittenti";
                            break;

                    }

                    retVal &= !res;
                    mex.AppendFormat(res ? "Violazione delle restrizioni imposte dalla ragione di trasmissione utilizzata. {0}" : String.Empty, details);

                }

            }

            message = mex.ToString();
            return retVal;

        }



        private class InfoUtentiRuolo
        {
            public long SystemId { get; set; }
            public string? UserId { get; set; }
        }

        private async Task<List<InfoUtentiRuolo>> getListUtentiRuolo(DocsPaVO.utente.Ruolo ruolo, bool appartenente)
        {
            List<InfoUtentiRuolo> result = new();

            if (appartenente)
            {
                result = await (from a in this._dbContext.PeopleEntities.AsNoTracking()
                                from b in this._dbContext.PeopleGroupEntities.AsNoTracking()
                                where a.SYSTEM_ID == b.PEOPLE_SYSTEM_ID &&
                                !b.DTA_FINE.HasValue && 
                                b.GROUPS_SYSTEM_ID == ruolo.idGruppo.AsLong() && 
                                a.ID_AMM == ruolo.idAmministrazione.AsLong()
                                orderby a.SYSTEM_ID
                                select new InfoUtentiRuolo()
                                {
                                    SystemId = a.SYSTEM_ID,
                                    UserId = a.USER_ID
                                }).ToListAsync();
                result = result.Distinct().ToList();
            }
            else
            {
                List<long> innerQuery = await (from a in this._dbContext.PeopleEntities.AsNoTracking()
                                  from b in this._dbContext.PeopleGroupEntities.AsNoTracking()
                                  where a.SYSTEM_ID == b.PEOPLE_SYSTEM_ID &&
                                  !b.DTA_FINE.HasValue &&
                                  b.GROUPS_SYSTEM_ID == ruolo.idGruppo.AsLong()
                                  select a.SYSTEM_ID).ToListAsync();



                result = await (from a in this._dbContext.PeopleEntities.AsNoTracking()
                                where !innerQuery.Contains(a.SYSTEM_ID) &&
                                a.ID_AMM == ruolo.idAmministrazione.AsLong()
                                select new InfoUtentiRuolo()
                                {
                                    SystemId = a.SYSTEM_ID,
                                    UserId = a.USER_ID
                                }).ToListAsync();
                result = result.Distinct().ToList();

            }


            return result;
        }


        public FindAndReplaceRoleInModelliTrasmissioneHandler(
            ILogger<FindAndReplaceRoleInModelliTrasmissioneHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
        }


        public async Task<FindAndReplaceRoleInModelliTrasmissioneResult> Handle(FindAndReplaceRoleInModelliTrasmissioneRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse output = new FindAndReplaceResponse();
            try
            {
                output = await this.FindAndReplaceRuoli(request.request);
            }
            catch( Exception ex )
            {
                this._logger.LogWebMethodError(ex);
            }

            return new FindAndReplaceRoleInModelliTrasmissioneResult(output);
        }
    }
}
