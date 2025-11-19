// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SetDataVistaSP
{
    public class SetDataVistaSPCommandHandler : IRequestHandler<SetDataVistaSPCommand, SetDataVistaSPCommandResponse>
    {

        public SetDataVistaSPCommandHandler(
            ILogger<SetDataVistaSPCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IConfigurationService configurationService,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
        }


        public async Task<SetDataVistaSPCommandResponse> Handle(SetDataVistaSPCommand request, CancellationToken cancellationToken)
        {
            bool result = false;
            string objId = request.DocNumber;
            string objType = request.DocOrFasc;


            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
            var idDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser).ToString();

            long idGroupAsLong = idGroup.AsLong();
            long idUserAsLong = idUser.AsLong();
            long idDelegatoAsLong = idDelegato.AsLong();

            try
            {
                string setDataVistaGrdKey = await this._configurationService.GetValue<string>("BE_SET_DATA_VISTA_GRD");

                if (setDataVistaGrdKey.Equals("2"))
                    result = await this.SPsetDataVista_V2(idUserAsLong, idGroupAsLong, idDelegatoAsLong, objId.AsLong(), objType);
                else
                    result = await this.SPsetDataVista(idUserAsLong, idGroupAsLong, idDelegatoAsLong, objId.AsLong(), objType);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new() 
            {
                Output = result 
            };
        }


        #region Private Members 

        protected readonly ILogger<SetDataVistaSPCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IPi3DbContext _dbContext;

        private async Task<bool> SPsetDataVista(long idUser, long idGroup, long idDelegato, long objId, string objType)
        {
            bool result = false;
            var q1 = this._dbContext.TrasmissioneEntities.Join(this._dbContext.TrasmSingolaEntities, t => t.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (t, ts) => new { t, ts });
            var q2 = q1.Join(this._dbContext.RagioneTrasmissioneEntities, q1 => q1.ts.ID_RAGIONE, rt => rt.SYSTEM_ID, (q1, rt) => new { q1.t, q1.ts, rt });

            long idGroupFromCorrGlobali = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
            long idPeopleFromCorrGlobali = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_PEOPLE == idUser).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

            //1. Cursore -> lista documenti/fascicoli su cui ciclare
            switch (objType)
            {
                case "F":
                    q2 = q2.Where(x => x.t.DTA_INVIO != null && x.t.ID_PROJECT == objId &&
                    (x.ts.ID_CORR_GLOBALE == idGroupFromCorrGlobali || x.ts.ID_CORR_GLOBALE == idPeopleFromCorrGlobali));
                    break;
                case "D":
                    q2 = q2.Where(x => x.t.DTA_INVIO != null && x.t.ID_PROFILE == objId &&
                    (x.ts.ID_CORR_GLOBALE == idGroupFromCorrGlobali || x.ts.ID_CORR_GLOBALE == idPeopleFromCorrGlobali));
                    break;
            }

            var objList = q2.Select(x => new
            {
                SYSTEM_ID_TS = x.ts.SYSTEM_ID,
                CHA_TIPO_TRASM = x.ts.CHA_TIPO_TRASM,
                CHA_TIPO_RAGIONE = x.rt.CHA_TIPO_RAGIONE,
                CHA_TIPO_DEST = x.ts.CHA_TIPO_DEST
            });

            foreach (var obj in objList)
            {
                switch (obj.CHA_TIPO_RAGIONE)
                {
                    case "N": //Senza workflow
                    case "I":
                        //Setto la data vista
                        var trasmUtenteNoWF = await this._dbContext.TrasmUtenteEntities.Where(x => x.DTA_VISTA == null && x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE == idUser).FirstOrDefaultAsync();
                        if (trasmUtenteNoWF == null)
                            this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Trasmissione a utente con id {0} per la trasmissione singola {1} non trovata o già accettata", idUser, obj.SYSTEM_ID_TS);
                        else
                        {
                            trasmUtenteNoWF.DTA_VISTA = trasmUtenteNoWF.DTA_VISTA.HasValue ? trasmUtenteNoWF.DTA_VISTA : DateTime.Now;
                            trasmUtenteNoWF.CHA_VISTA = "1";
                            trasmUtenteNoWF.CHA_IN_TODOLIST = "0";
                            if (idDelegato != 0) //delega
                            {
                                trasmUtenteNoWF.CHA_VISTA_DELEGATO = "1";
                                trasmUtenteNoWF.ID_PEOPLE_DELEGATO = idDelegato;
                            }
                            await ((Pi3DbContext)_dbContext).SaveChangesAsync();

                            //copio la notifica nello storico
                            var notify = await this._dbContext.NotifyEntities
                                .Where(x => x.ID_SPECIALIZED_OBJECT == obj.SYSTEM_ID_TS && (x.ID_GROUP_RECEIVER == idGroup || x.ID_GROUP_RECEIVER == 0) && x.ID_PEOPLE_RECEIVER == idUser)
                                .FirstOrDefaultAsync();

                            if (notify == null)
                                throw new NotifyNotFoundException(obj.SYSTEM_ID_TS, idGroup, idUser);

                            var notifyHistoryToInsert = new NotifyHistoryEntity()
                            {
                                ID_NOTIFY = notify.SYSTEM_ID,
                                ID_EVENT = notify.ID_EVENT,
                                DESC_PRODUCER = notify.DESC_PRODUCER,
                                ID_PEOPLE_RECEIVER = notify.ID_PEOPLE_RECEIVER,
                                ID_GROUP_RECEIVER = notify.ID_GROUP_RECEIVER,
                                TYPE_NOTIFY = notify.TYPE_NOTIFY,
                                DTA_NOTIFY = notify.DTA_NOTIFY,
                                FIELD_1 = notify.FIELD_1,
                                FIELD_2 = notify.FIELD_2,
                                FIELD_3 = notify.FIELD_3,
                                FIELD_4 = notify.FIELD_4,
                                MULTIPLICITY = notify.MULTIPLICITY,
                                SPECIALIZED_FIELD = notify.SPECIALIZED_FIELD,
                                TYPE_EVENT = notify.TYPE_EVENT,
                                DOMAINOBJECT = notify.DOMAINOBJECT,
                                ID_OBJECT = notify.ID_OBJECT,
                                ID_SPECIALIZED_OBJECT = notify.ID_SPECIALIZED_OBJECT,
                                DTA_EVENT = notify.DTA_EVENT,
                                READ_NOTIFICATION = notify.READ_NOTIFICATION
                            };

                            await this._dbContext.NotifyHistoryEntities.AddAsync(notifyHistoryToInsert);
                            int NotifyHistoryRowsIns = await ((Pi3DbContext)_dbContext).SaveChangesAsync();

                            if (NotifyHistoryRowsIns > 0)
                            {
                                //elimino la notifica
                                this._dbContext.NotifyEntities.Remove(notify);
                                int notifyRowsDeleted = await ((DbContext)_dbContext).SaveChangesAsync();
                                if (notifyRowsDeleted == 0)
                                    this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Delete nella DPA_NOTIFY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                            }
                            else
                            {
                                this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Insert nella DPA_NOTIFY_HISTORY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                            }

                            //update della DPA_TODOLIST
                            var toDoListEntity = this._dbContext.ToDoListEntities
                                .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE_DEST == idUser);

                            switch (objType)
                            {
                                case "F":
                                    toDoListEntity = toDoListEntity.Where(x => x.ID_PROJECT == objId);
                                    break;
                                case "D":
                                    toDoListEntity = toDoListEntity.Where(x => x.ID_PROFILE == objId);
                                    break;
                            }

                            var toDoListItem = await toDoListEntity.FirstOrDefaultAsync();

                            if (toDoListItem == null)
                                throw new ToDoListItemNotFountException(obj.SYSTEM_ID_TS, idUser);

                            toDoListItem.DTA_VISTA = DateTime.Now;
                            int toDoListRowUpdated = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                            if (toDoListRowUpdated == 0)
                                this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Update della DPA_TODOLIST fallito per la riga con id trasmissione singola {0}, oggetto {1}, id utente {2}", obj.SYSTEM_ID_TS, objId, idUser);

                            //read_notification = 1
                            var notificationToSetRead = await this._dbContext.NotifyEntities
                                .Where(x => x.ID_SPECIALIZED_OBJECT == obj.SYSTEM_ID_TS && x.ID_PEOPLE_RECEIVER == idUser)
                                .FirstOrDefaultAsync();

                            if (notificationToSetRead == null)
                                throw new NotifyNotFoundException(obj.SYSTEM_ID_TS, idUser);

                            notificationToSetRead.READ_NOTIFICATION = "1";
                            int notificationToSetReadUpdated = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                            if (notificationToSetReadUpdated == 0)
                                this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Delete nella DPA_NOTIFY fallito per la notifica con ID {0}", notificationToSetRead.SYSTEM_ID);

                            if (!string.IsNullOrEmpty(obj.CHA_TIPO_TRASM) && obj.CHA_TIPO_TRASM.Equals("S")
                                && !string.IsNullOrEmpty(obj.CHA_TIPO_DEST) && obj.CHA_TIPO_DEST.Equals("R"))
                            {
                                var trasmUtenteOtherUsers = await this._dbContext.TrasmUtenteEntities
                                    .Where(x => !x.DTA_VISTA.HasValue && x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE != idUser &&
                                    this._dbContext.TrasmUtenteEntities.Any(t => t.ID_TRASM_SINGOLA == x.ID_TRASM_SINGOLA && t.ID_PEOPLE == idUser))
                                    .ToListAsync();

                                trasmUtenteOtherUsers.ForEach(x =>
                                {
                                    x.CHA_VISTA = "1";
                                    x.CHA_IN_TODOLIST = "0";
                                    if (idDelegato == 0)
                                    {
                                        x.CHA_VISTA_DELEGATO = "1";
                                        x.ID_PEOPLE_DELEGATO = idDelegato;
                                    }
                                });
                                await ((Pi3DbContext)_dbContext).SaveChangesAsync();

                                var otherUsersList = trasmUtenteOtherUsers.Select(x => x.ID_PEOPLE).ToList();
                                var notifyListOtherUsers = await this._dbContext.NotifyEntities
                                .Where(x => x.ID_SPECIALIZED_OBJECT == obj.SYSTEM_ID_TS && (x.ID_GROUP_RECEIVER == idGroup || x.ID_GROUP_RECEIVER == 0)
                                        && otherUsersList.Contains(x.ID_PEOPLE_RECEIVER))
                                .ToListAsync();

                                notifyListOtherUsers.ForEach(async notify =>
                                {
                                    var notifyHistoryToInsert = new NotifyHistoryEntity()
                                    {
                                        ID_NOTIFY = notify.SYSTEM_ID,
                                        ID_EVENT = notify.ID_EVENT,
                                        DESC_PRODUCER = notify.DESC_PRODUCER,
                                        ID_PEOPLE_RECEIVER = notify.ID_PEOPLE_RECEIVER,
                                        ID_GROUP_RECEIVER = notify.ID_GROUP_RECEIVER,
                                        TYPE_NOTIFY = notify.TYPE_NOTIFY,
                                        DTA_NOTIFY = notify.DTA_NOTIFY,
                                        FIELD_1 = notify.FIELD_1,
                                        FIELD_2 = notify.FIELD_2,
                                        FIELD_3 = notify.FIELD_3,
                                        FIELD_4 = notify.FIELD_4,
                                        MULTIPLICITY = notify.MULTIPLICITY,
                                        SPECIALIZED_FIELD = notify.SPECIALIZED_FIELD,
                                        TYPE_EVENT = notify.TYPE_EVENT,
                                        DOMAINOBJECT = notify.DOMAINOBJECT,
                                        ID_OBJECT = notify.ID_OBJECT,
                                        ID_SPECIALIZED_OBJECT = notify.ID_SPECIALIZED_OBJECT,
                                        DTA_EVENT = notify.DTA_EVENT,
                                        READ_NOTIFICATION = notify.READ_NOTIFICATION
                                    };

                                    await this._dbContext.NotifyHistoryEntities.AddAsync(notifyHistoryToInsert);

                                    int notifyHistoryRowsIns = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                                    if (notifyHistoryRowsIns > 0)
                                    {
                                        //elimino la notifica
                                        this._dbContext.NotifyEntities.Remove(notify);
                                        int notifyRowsDeleted = await ((DbContext)_dbContext).SaveChangesAsync();
                                        if (notifyRowsDeleted == 0)
                                            this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Delete nella DPA_NOTIFY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                                    }
                                    else
                                    {
                                        this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Insert nella DPA_NOTIFY_HISTORY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                                    }
                                });
                            }
                        }

                        break;
                    case "W": //con workflow
                        var trasmUtenteWF = await this._dbContext.TrasmUtenteEntities
                            .Where(x => !x.DTA_VISTA.HasValue && x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE == idUser).FirstOrDefaultAsync();
                        if (trasmUtenteWF == null)
                            this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Trasmissione a utente con id {0} per la trasmissione singola {1} non trovata o già accettata", idUser, obj.SYSTEM_ID_TS);

                        else
                        {
                            trasmUtenteWF.CHA_VISTA = "1";
                            trasmUtenteWF.DTA_VISTA = trasmUtenteWF.DTA_VISTA.HasValue ? trasmUtenteWF.DTA_VISTA : DateTime.Now; ;

                            if (idDelegato != 0) //delega
                            {
                                trasmUtenteWF.CHA_VISTA_DELEGATO = "1";
                                trasmUtenteWF.ID_PEOPLE_DELEGATO = idDelegato;
                            }
                            await ((Pi3DbContext)_dbContext).SaveChangesAsync();

                            //rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
                            var trasmUtenteAccRif = await this._dbContext.TrasmUtenteEntities
                                .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.DTA_VISTA.HasValue
                                    && (x.CHA_ACCETTATA == "1" || x.CHA_RIFIUTATA == "1"))
                                .FirstOrDefaultAsync();

                            if (trasmUtenteAccRif != null)
                            {
                                trasmUtenteAccRif.CHA_IN_TODOLIST = "0";
                                int toDoListRowUpdated = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                                if (toDoListRowUpdated == 0)
                                    this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Update della DPA_TODOLIST per CHA_IN_TODOLIST = 0 fallito per la riga con id trasmissione singola {0}, oggetto {1}, id utente {2}", obj.SYSTEM_ID_TS, objId, idUser);

                                var toDoListEntityAccRif = this._dbContext.ToDoListEntities
                                    .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE_DEST == idUser);

                                switch (objType)
                                {
                                    case "F":
                                        toDoListEntityAccRif = toDoListEntityAccRif.Where(x => x.ID_PROJECT == objId);
                                        break;
                                    case "D":
                                        toDoListEntityAccRif = toDoListEntityAccRif.Where(x => x.ID_PROFILE == objId);
                                        break;
                                }

                                var toDoListItemAccRif = await toDoListEntityAccRif.FirstOrDefaultAsync();

                                if (toDoListItemAccRif == null)
                                    throw new ToDoListItemNotFountException(obj.SYSTEM_ID_TS, idUser);

                                toDoListItemAccRif.DTA_VISTA = DateTime.Now;
                                toDoListRowUpdated = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                                if (toDoListRowUpdated == 0)
                                    this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Update della DPA_TODOLIST fallito per la riga con id trasmissione singola {0}, oggetto {1}, id utente {2}", obj.SYSTEM_ID_TS, objId, idUser);

                                //elimino la notifica
                                var idPeopleReceiverList = await this._dbContext.TrasmUtenteEntities
                                    .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS
                                        && x.DTA_VISTA.HasValue
                                        && (x.CHA_ACCETTATA == "1" || x.CHA_RIFIUTATA == "1"))
                                    .Select(x => x.ID_PEOPLE).ToListAsync();

                                var notifyList = await this._dbContext.NotifyEntities
                                    .Where(x => x.ID_SPECIALIZED_OBJECT == obj.SYSTEM_ID_TS && (x.ID_GROUP_RECEIVER == idGroup || x.ID_GROUP_RECEIVER == 0)
                                        && idPeopleReceiverList.Contains(x.ID_PEOPLE_RECEIVER)
                                        && (x.ID_GROUP_RECEIVER == idGroup || x.ID_GROUP_RECEIVER == 0))
                                    .ToListAsync();

                                notifyList.ForEach(async notify =>
                                {
                                    var notifyHistoryToInsert = new NotifyHistoryEntity()
                                    {
                                        ID_NOTIFY = notify.SYSTEM_ID,
                                        ID_EVENT = notify.ID_EVENT,
                                        DESC_PRODUCER = notify.DESC_PRODUCER,
                                        ID_PEOPLE_RECEIVER = notify.ID_PEOPLE_RECEIVER,
                                        ID_GROUP_RECEIVER = notify.ID_GROUP_RECEIVER,
                                        TYPE_NOTIFY = notify.TYPE_NOTIFY,
                                        DTA_NOTIFY = notify.DTA_NOTIFY,
                                        FIELD_1 = notify.FIELD_1,
                                        FIELD_2 = notify.FIELD_2,
                                        FIELD_3 = notify.FIELD_3,
                                        FIELD_4 = notify.FIELD_4,
                                        MULTIPLICITY = notify.MULTIPLICITY,
                                        SPECIALIZED_FIELD = notify.SPECIALIZED_FIELD,
                                        TYPE_EVENT = notify.TYPE_EVENT,
                                        DOMAINOBJECT = notify.DOMAINOBJECT,
                                        ID_OBJECT = notify.ID_OBJECT,
                                        ID_SPECIALIZED_OBJECT = notify.ID_SPECIALIZED_OBJECT,
                                        DTA_EVENT = notify.DTA_EVENT,
                                        READ_NOTIFICATION = notify.READ_NOTIFICATION
                                    };

                                    await this._dbContext.NotifyHistoryEntities.AddAsync(notifyHistoryToInsert);

                                    int notifyHistoryRowsIns = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                                    if (notifyHistoryRowsIns > 0)
                                    {
                                        //elimino la notifica
                                        this._dbContext.NotifyEntities.Remove(notify);
                                        int notifyRowsDeleted = await ((DbContext)_dbContext).SaveChangesAsync();
                                        if (notifyRowsDeleted == 0)
                                            this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Delete nella DPA_NOTIFY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                                    }
                                    else
                                    {
                                        this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Insert nella DPA_NOTIFY_HISTORY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                                    }
                                });
                            }
                        }
                        break;
                }
            }
            return result;
        }

        private async Task<bool> SPsetDataVista_V2(long idUser, long idGroup, long idDelegato, long objId, string objType)
        {
            bool result = false;
            var q1 = this._dbContext.TrasmissioneEntities.Join(this._dbContext.TrasmSingolaEntities, t => t.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (t, ts) => new { t, ts });
            var q2 = q1.Join(this._dbContext.RagioneTrasmissioneEntities, q1 => q1.ts.ID_RAGIONE, rt => rt.SYSTEM_ID, (q1, rt) => new { q1.t, q1.ts, rt });

            long idGroupFromCorrGlobali = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
            long idPeopleFromCorrGlobali = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_PEOPLE == idUser).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

            //1. Cursore -> lista documenti/fascicoli su cui ciclare
            switch (objType)
            {
                case "F":
                    q2 = q2.Where(x => x.t.DTA_INVIO != null && x.t.ID_PROJECT == objId &&
                    (x.ts.ID_CORR_GLOBALE == idGroupFromCorrGlobali || x.ts.ID_CORR_GLOBALE == idPeopleFromCorrGlobali));
                    break;
                case "D":
                    q2 = q2.Where(x => x.t.DTA_INVIO != null && x.t.ID_PROFILE == objId &&
                    (x.ts.ID_CORR_GLOBALE == idGroupFromCorrGlobali || x.ts.ID_CORR_GLOBALE == idPeopleFromCorrGlobali));
                    break;
            }

            var objList = q2.Select(x => new
            {
                SYSTEM_ID_TS = x.ts.SYSTEM_ID,
                CHA_TIPO_TRASM = x.ts.CHA_TIPO_TRASM,
                CHA_TIPO_RAGIONE = x.rt.CHA_TIPO_RAGIONE,
                CHA_TIPO_DEST = x.ts.CHA_TIPO_DEST
            });

            foreach (var obj in objList)
            {
                switch (obj.CHA_TIPO_RAGIONE)
                {
                    case "N": //Senza workflow
                    case "I":
                    case "S":
                        //Setto la data vista
                        var trasmUtenteNoWF = await this._dbContext.TrasmUtenteEntities.Where(x => x.DTA_VISTA == null && x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE == idUser).FirstOrDefaultAsync();
                        if (trasmUtenteNoWF == null)
                            this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista - Trasmissione a utente con id {0} per la trasmissione singola {1} non trovata o già accettata", idUser, obj.SYSTEM_ID_TS);
                        else
                        {
                            trasmUtenteNoWF.DTA_VISTA = trasmUtenteNoWF.DTA_VISTA.HasValue ? trasmUtenteNoWF.DTA_VISTA : DateTime.Now;
                            trasmUtenteNoWF.CHA_VISTA = "1";
                            //trasmUtenteNoWF.CHA_IN_TODOLIST = "0";
                            if (idDelegato != 0) //delega
                            {
                                trasmUtenteNoWF.CHA_VISTA_DELEGATO = "1";
                                trasmUtenteNoWF.ID_PEOPLE_DELEGATO = idDelegato;
                            }
                            await ((Pi3DbContext)_dbContext).SaveChangesAsync();


                            //update della DPA_TODOLIST
                            var toDoListEntity = this._dbContext.ToDoListEntities
                                .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE_DEST == idUser);

                            switch (objType)
                            {
                                case "F":
                                    toDoListEntity = toDoListEntity.Where(x => x.ID_PROJECT == objId);
                                    break;
                                case "D":
                                    toDoListEntity = toDoListEntity.Where(x => x.ID_PROFILE == objId);
                                    break;
                            }

                            var toDoListItem = await toDoListEntity.FirstOrDefaultAsync();

                            if (toDoListItem == null)
                                throw new ToDoListItemNotFountException(obj.SYSTEM_ID_TS, idUser);

                            toDoListItem.DTA_VISTA = DateTime.Now;
                            int toDoListRowUpdated = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                            if (toDoListRowUpdated == 0)
                                this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Update della DPA_TODOLIST fallito per la riga con id trasmissione singola {0}, oggetto {1}, id utente {2}", obj.SYSTEM_ID_TS, objId, idUser);

                            //read_notification = 1
                            var notificationToSetRead = await this._dbContext.NotifyEntities
                                .Where(x => x.ID_SPECIALIZED_OBJECT == obj.SYSTEM_ID_TS && x.ID_PEOPLE_RECEIVER == idUser)
                                .FirstOrDefaultAsync();

                            if (notificationToSetRead == null)
                                throw new NotifyNotFoundException(obj.SYSTEM_ID_TS, idUser);

                            notificationToSetRead.READ_NOTIFICATION = "1";
                            int notificationToSetReadUpdated = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                            if (notificationToSetReadUpdated == 0)
                                this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Delete nella DPA_NOTIFY fallito per la notifica con ID {0}", notificationToSetRead.SYSTEM_ID);

                            if (!string.IsNullOrEmpty(obj.CHA_TIPO_TRASM) && obj.CHA_TIPO_TRASM.Equals("S")
                                && !string.IsNullOrEmpty(obj.CHA_TIPO_DEST) && obj.CHA_TIPO_DEST.Equals("R"))
                            {
                                var trasmUtenteOtherUsers = await this._dbContext.TrasmUtenteEntities
                                    .Where(x => !x.DTA_VISTA.HasValue && x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE != idUser &&
                                    this._dbContext.TrasmUtenteEntities.Any(t => t.ID_TRASM_SINGOLA == x.ID_TRASM_SINGOLA && t.ID_PEOPLE == idUser))
                                    .ToListAsync();

                                trasmUtenteOtherUsers.ForEach(x =>
                                {
                                    x.CHA_VISTA = "1";
                                    //x.CHA_IN_TODOLIST = "0";
                                    if (idDelegato == 0)
                                    {
                                        x.CHA_VISTA_DELEGATO = "1";
                                        x.ID_PEOPLE_DELEGATO = idDelegato;
                                    }
                                });
                                await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                            }
                        }

                        break;
                    case "W": //con workflow
                        var trasmUtenteWF = await this._dbContext.TrasmUtenteEntities
                            .Where(x => !x.DTA_VISTA.HasValue && x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE == idUser).FirstOrDefaultAsync();
                        if (trasmUtenteWF == null)
                            this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Trasmissione a utente con id {0} per la trasmissione singola {1} non trovata o già accettata", idUser, obj.SYSTEM_ID_TS);

                        else
                        {
                            trasmUtenteWF.CHA_VISTA = "1";
                            trasmUtenteWF.DTA_VISTA = trasmUtenteWF.DTA_VISTA.HasValue ? trasmUtenteWF.DTA_VISTA : DateTime.Now; ;

                            if (idDelegato != 0) //delega
                            {
                                trasmUtenteWF.CHA_VISTA_DELEGATO = "1";
                                trasmUtenteWF.ID_PEOPLE_DELEGATO = idDelegato;
                            }
                            await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                        }
                        //rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
                        var trasmUtenteAccRif = await this._dbContext.TrasmUtenteEntities
                            .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.DTA_VISTA.HasValue
                                && (x.CHA_ACCETTATA == "1" || x.CHA_RIFIUTATA == "1"))
                            .FirstOrDefaultAsync();

                        if (trasmUtenteAccRif != null)
                        {
                            //trasmUtenteAccRif.CHA_IN_TODOLIST = "0";
                            /*
                                int toDoListRowUpdated = await((Pi3DbContext)_dbContext).SaveChangesAsync();
                                if (toDoListRowUpdated == 0)
                                    this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Update della DPA_TODOLIST per CHA_IN_TODOLIST = 0 fallito per la riga con id trasmissione singola {0}, oggetto {1}, id utente {2}", obj.SYSTEM_ID_TS, objId, idUser);
                            */
                            var toDoListEntityAccRif = this._dbContext.ToDoListEntities
                                .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS && x.ID_PEOPLE_DEST == idUser);

                            switch (objType)
                            {
                                case "F":
                                    toDoListEntityAccRif = toDoListEntityAccRif.Where(x => x.ID_PROJECT == objId);
                                    break;
                                case "D":
                                    toDoListEntityAccRif = toDoListEntityAccRif.Where(x => x.ID_PROFILE == objId);
                                    break;
                            }

                            var toDoListItemAccRif = await toDoListEntityAccRif.FirstOrDefaultAsync();

                            if (toDoListItemAccRif == null)
                                continue;
                            //throw new ToDoListItemNotFountException(obj.SYSTEM_ID_TS, idUser);

                            toDoListItemAccRif.DTA_VISTA = DateTime.Now;
                            /*
                                toDoListRowUpdated = await((Pi3DbContext)_dbContext).SaveChangesAsync();
                                if (toDoListRowUpdated == 0)
                                    this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Update della DPA_TODOLIST fallito per la riga con id trasmissione singola {0}, oggetto {1}, id utente {2}", obj.SYSTEM_ID_TS, objId, idUser);
                            */
                            //elimino la notifica
                            var idPeopleReceiverList = await this._dbContext.TrasmUtenteEntities
                                .Where(x => x.ID_TRASM_SINGOLA == obj.SYSTEM_ID_TS
                                    && x.DTA_VISTA.HasValue
                                    && (x.CHA_ACCETTATA == "1" || x.CHA_RIFIUTATA == "1"))
                                .Select(x => x.ID_PEOPLE).ToListAsync();

                            var notifyList = await this._dbContext.NotifyEntities
                                .Where(x => x.ID_SPECIALIZED_OBJECT == obj.SYSTEM_ID_TS && (x.ID_GROUP_RECEIVER == idGroup || x.ID_GROUP_RECEIVER == 0)
                                    && idPeopleReceiverList.Contains(x.ID_PEOPLE_RECEIVER)
                                    && (x.ID_GROUP_RECEIVER == idGroup || x.ID_GROUP_RECEIVER == 0))
                                .ToListAsync();

                            notifyList.ForEach(async notify =>
                            {
                                var notifyHistoryToInsert = new NotifyHistoryEntity()
                                {
                                    ID_NOTIFY = notify.SYSTEM_ID,
                                    ID_EVENT = notify.ID_EVENT,
                                    DESC_PRODUCER = notify.DESC_PRODUCER,
                                    ID_PEOPLE_RECEIVER = notify.ID_PEOPLE_RECEIVER,
                                    ID_GROUP_RECEIVER = notify.ID_GROUP_RECEIVER,
                                    TYPE_NOTIFY = notify.TYPE_NOTIFY,
                                    DTA_NOTIFY = notify.DTA_NOTIFY,
                                    FIELD_1 = notify.FIELD_1,
                                    FIELD_2 = notify.FIELD_2,
                                    FIELD_3 = notify.FIELD_3,
                                    FIELD_4 = notify.FIELD_4,
                                    MULTIPLICITY = notify.MULTIPLICITY,
                                    SPECIALIZED_FIELD = notify.SPECIALIZED_FIELD,
                                    TYPE_EVENT = notify.TYPE_EVENT,
                                    DOMAINOBJECT = notify.DOMAINOBJECT,
                                    ID_OBJECT = notify.ID_OBJECT,
                                    ID_SPECIALIZED_OBJECT = notify.ID_SPECIALIZED_OBJECT,
                                    DTA_EVENT = notify.DTA_EVENT,
                                    READ_NOTIFICATION = notify.READ_NOTIFICATION
                                };

                                await this._dbContext.NotifyHistoryEntities.AddAsync(notifyHistoryToInsert);

                                int notifyHistoryRowsIns = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                                if (notifyHistoryRowsIns > 0)
                                {
                                    //elimino la notifica
                                    this._dbContext.NotifyEntities.Remove(notify);
                                    int notifyRowsDeleted = await ((DbContext)_dbContext).SaveChangesAsync();
                                    if (notifyRowsDeleted == 0)
                                        this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Delete nella DPA_NOTIFY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                                }
                                else
                                {
                                    this._logger.LogDebug("[SetDataVistaSP.Handle] - SPsetDataVista_V2 - Insert nella DPA_NOTIFY_HISTORY fallito per la notifica con ID {0}", notify.SYSTEM_ID);
                                }
                            });
                        }

                        break;
                }
            }
            return result;
        }

        #endregion
    }
}
