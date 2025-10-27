// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Notification;
using MediatR;
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
using Microsoft.EntityFrameworkCore;
using ModifyNotifications = Pi3.App.Legacy.WebApi.Application.Requests.ModifyNotifications;
using Pi3.Core.AggregateModels.DocumentAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using DocsPaVO.utente;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using DocsPaVO.RegistroAccessi;
using DocsPaVO.InstanceAccess.Metadata;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata.Ecma335;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using getSegnaturaRepertorioNoHTMLRequest = Pi3.App.Legacy.WebApi.Application.Requests.getSegnaturaRepertorioNoHTML;
using DocsPaVO.ProfilazioneDinamica;
using AutoMapper;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using DocsPaVO.RicercaLite;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModifyNotifications
{
    /// <summary>
    /// 
    /// </summary>
    public class ModifyNotificationsHandler : IRequestHandler<Application.Requests.ModifyNotifications, ModifyNotificationsResult>
    {

        #region Public Members

        public async Task<ModifyNotificationsResult> Handle(Application.Requests.ModifyNotifications request, CancellationToken cancellationToken)
        {
            var result = true;
            List<Notification> notifications = new List<Notification>();
            var idTenant = request.infoUtente.idAmministrazione.AsLong();

            try
            {
                var notificationList = await this._dbContext.NotifyEntities
                    .Where(ne => ne.ID_OBJECT == request.idObject.AsLong() && ne.DOMAINOBJECT == request.domainObject)
                    .ToListAsync();


                if (notificationList != null && notificationList.Any())
                {
                    if (notificationList[0].DOMAINOBJECT.Equals(ListDomainObject.DOCUMENT))
                    {
                        string tipoDoc = string.Empty;

                        var mittente = await (from dap in this._dbContext.DocArrivoParEntities
                                                        join cg in this._dbContext.CorrGlobaliEntities on dap.ID_MITT_DEST equals cg.SYSTEM_ID
                                                        where dap.ID_PROFILE == notificationList[0].ID_OBJECT && dap.CHA_TIPO_MITT_DEST == "M"
                                                        select cg.VAR_DESC_CORR)
                                                        .FirstOrDefaultAsync();

                        InfoDoc infoDoc = await
                            (from profile in _dbContext.ProfileEntities
                             join tipoAtto in _dbContext.TipoAttoEntities on profile.ID_TIPO_ATTO equals tipoAtto.SYSTEM_ID into gj
                             from subTipoatto in gj.DefaultIfEmpty()
                             where profile.SYSTEM_ID == notificationList[0].ID_OBJECT
                             select
                             new InfoDoc()
                             {
                                 Id = profile.SYSTEM_ID,
                                 Tipologia = subTipoatto.VAR_DESC_ATTO ?? String.Empty,
                                 TipoProto = profile.CHA_TIPO_PROTO,
                                 Oggetto = profile.VAR_PROF_OGGETTO,
                                 Mittente = mittente,
                                 Name = profile.DOCNAME,
                                 IdTenant = idTenant.ToString()

                             }).FirstAsync();

                        await this.ExcecuteChangeItems(request.infoUtente, request.typeOperation, notificationList[0], infoDoc, null);
                        foreach (NotifyEntity notification in notificationList)
                        {
                            //Tutte le notifiche relative ad un documento o fascicolo hanno i valori dei campi item uguali
                            notification.FIELD_1 = notificationList[0].FIELD_1;
                            notification.FIELD_2 = notificationList[0].FIELD_2;
                            notification.FIELD_3 = notificationList[0].FIELD_3;
                            notification.FIELD_4 = notificationList[0].FIELD_4;

                            //Le notifiche relative ad un documento o fascicolo posso avere ITEM_SPECIALIZED diverso(esempio note individuali o generali)
                            await this.ExcecuteChangeSpecializedItems(request.infoUtente, request.typeOperation, notification, infoDoc, null);
                        }
                    }
                    else
                    {
                        string mittente = string.Empty;

                        InfoFasc infoFasc = await (from project in _dbContext.ProjectEntities
                                                   join tipoFasc in _dbContext.TipoFascEntities on project.ID_TIPO_FASC equals tipoFasc.SYSTEM_ID into gj
                                                   from subTipoFasc in gj.DefaultIfEmpty()
                                                   where project.SYSTEM_ID == notificationList[0].ID_OBJECT
                                                   select
                                                   new InfoFasc()

                                                   {
                                                       Id = project.SYSTEM_ID,
                                                       Tipologia = subTipoFasc.VAR_DESC_FASC,
                                                       Descrizione = project.DESCRIPTION,
                                                       IdTenant = idTenant.ToString()

                                                   }).FirstAsync();

                        await this.ExcecuteChangeItems(request.infoUtente, request.typeOperation, notificationList[0], null, infoFasc);
                        foreach (NotifyEntity notification in notificationList)
                        {
                            //Tutte le notifiche relative ad un documento o fascicolo hanno i valori dei campi item uguali
                            notification.FIELD_3 = notificationList[0].FIELD_3;

                            //Le notifiche relative ad un documento o fascicolo posso avere ITEM_SPECIALIZED diverso(esempio note individuali o generali)
                            await this.ExcecuteChangeSpecializedItems(request.infoUtente, request.typeOperation, notification, null, infoFasc);
                        }
                    }


                    IDbContextTransaction? transaction = null;
                    try
                    {
                        transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
                        await ((DbContext)this._dbContext).SaveChangesAsync();

                        if (transaction != null)
                            await transaction.CommitAsync();

                        result = true;
                    }
                    catch (Exception exc)
                    {
                        if (transaction != null)
                            await transaction?.RollbackAsync();

                        this._logger.LogError(exception: exc, message: exc.Message);
                        throw exc;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                result = false;
            }


            return new ModifyNotificationsResult();
        }


        #endregion

        #region Private Members

        protected readonly ILogger<ModifyNotificationsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDocumentoAmministrativoRepository _documentoRepository;
        protected readonly IAggregazioneDocumentaleRepository _aggregatoDocumentaleRepository;
        protected readonly IMapper _mapper = null;
        protected readonly IPi3DbContext _dbContext;       

        public ModifyNotificationsHandler(ILogger<ModifyNotificationsHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDocumentoAmministrativoRepository documentoRepository, IAggregazioneDocumentaleRepository aggregatoDocumentaleRepository)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _documentoRepository = documentoRepository;
            _aggregatoDocumentaleRepository = aggregatoDocumentaleRepository;
            this._mapper = this.InitializeMapper();
        }

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VersionEntity, DocsPaVO.documento.Documento>();                

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Corrispondente>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });
            });

            return configuration.CreateMapper();
        }

        protected class SoggettoProtocolloEntity
        {
            public DocArrivoParEntity DocArrivoPar { get; set; }
            public CorrGlobaliEntity CorrGlobali { get; set; }
            public DettGlobaliEntity DettCorrGlobali { get; set; }
        }

        protected class InfoDoc
        {
            public long Id { get; set; }
            public string Tipologia { get; set; }

            public string TipoProto { get; set; }
            public string Oggetto { get; set; }
            public string Mittente { get; set; }
            public string Name { get; set; }
            public string IdTenant { get; set; }       
        }

        protected class InfoFasc
        {
            public long Id { get; set; }
            public string Tipologia { get; set; }           
            public string Descrizione { get; set; }           
            public string IdTenant { get; set; }
        }

        private async Task ExcecuteChangeItems(InfoUtente infoUtente, TypeOperation[] typeOperation, NotifyEntity notification, InfoDoc infoDoc, InfoFasc infoFasc)
        {
            DocsPaVO.Notification.Notification newNotification = new Notification();

            try
            {
                if (infoDoc != null)
                {
                    //se stampaReg o Grigio, non va bene..devo ricavarlo
                    //gestire grigio e stamparegistri
                    string tipoDoc = string.Empty;                  

                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                notification.FIELD_3 = infoDoc.Oggetto.ToString();
                                break;
                            case TypeOperation.CHANGE_TYPE_DOC:
                                var segn = await this._mediator.Send(new getSegnaturaRepertorioNoHTMLRequest(infoDoc.Id.ToString(), infoDoc.IdTenant));
                                string segnaturaRepertorio = segn.output;
                               
                                if (!string.IsNullOrEmpty(segnaturaRepertorio))
                                {
                                    notification.FIELD_4 = TagItem.LABEL + "lblRepertorio" + TagItem.CLOSE_LABEL +
                                        TagItem.COLORRED + segnaturaRepertorio + TagItem.CLOSE_COLORRED;
                                }
                                break;
                            case TypeOperation.RECORD_PREDISPOSED:
                                notification.FIELD_1 = infoDoc.Id.ToString();
                                notification.FIELD_2 = TagItem.COLORRED + infoDoc.Name +
                                    TagItem.CLOSE_COLORRED + GetLabelTypeProto(infoDoc.TipoProto);                         
                                break;
                            case TypeOperation.CHANGE_TYPE_PROTO:
                                notification.FIELD_1 = infoDoc.Id.ToString() + GetLabelTypeProto(infoDoc.TipoProto);
                                break;
                            case TypeOperation.ABORT_RECORD:
                                notification.FIELD_2 = TagItem.COLORRED_STRIKE + infoDoc.Name +
                                    TagItem.CLOSE_COLORRED_STRIKE + GetLabelTypeProto(infoDoc.TipoProto);
                                break;
                            case TypeOperation.ABORT_COUNTER_REPERTOIRE:
                                notification.FIELD_4 = notification.FIELD_4.Replace(TagItem.COLORRED, TagItem.COLORRED_STRIKE).
                                    Replace(TagItem.CLOSE_COLORRED, TagItem.CLOSE_COLORRED_STRIKE);
                                break;
                        }
                    }
                }
                else if (infoFasc != null)
                {

                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                notification.FIELD_3 = infoFasc.Descrizione.ToString();
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this._logger.LogError(exception: exc, message: exc.Message);
                throw exc;
            }
            return;
        }
        private async Task ExcecuteChangeSpecializedItems(InfoUtente infoUtente, TypeOperation[] typeOperation, NotifyEntity notification, InfoDoc infoDoc, InfoFasc infoFasc)
        {
            DocsPaVO.Notification.Notification newNotification = new Notification();

            try
            {
                if (infoDoc != null)
                {
                    string tipoDoc = string.Empty;                   
                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                notification.SPECIALIZED_FIELD = UpdateSpecializeItem(notification.SPECIALIZED_FIELD, infoDoc.Oggetto.ToString(), "lblObjectDescription");                         
                                break;
                            case TypeOperation.CHANGE_TYPE_DOC:
                                notification.SPECIALIZED_FIELD = UpdateSpecializeItem(notification.SPECIALIZED_FIELD, infoDoc.Tipologia, "lblDocType");
                                break;
                            case TypeOperation.CHANGE_SENDER:
                                string mittente = string.Empty;
                                mittente = (infoDoc.Mittente.ToString());                                
                                notification.SPECIALIZED_FIELD = UpdateSpecializeItem(notification.SPECIALIZED_FIELD, mittente, "lblSender");
                                break;                           
                        }
                    }
                }
                else if (infoFasc != null)
                {

                    foreach (TypeOperation operation in typeOperation)
                    {
                        switch (operation)
                        {
                            case TypeOperation.CHANGE_OBJECT:
                                notification.FIELD_3 = infoFasc.Descrizione.ToString();
                                break;
                            case TypeOperation.CHANGE_TYPE_PROJ:
                               if (infoFasc.Tipologia != null)
                                    newNotification.ITEM_SPECIALIZED = UpdateSpecializeItem(notification.SPECIALIZED_FIELD, infoFasc.Tipologia, "lblFascType");
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this._logger.LogError(exception: exc, message: exc.Message);
                throw exc;
            }
            return; 
        }


        protected static string GetLabelTypeProto(string typeProto)
        {
            string labelTypeProto = string.Empty;
            switch (typeProto)
            {
                case TypeProtocol.ARRIVO:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_ARRIVO + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.PARTENZA:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_PARTENZA + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.INTERNO:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_INTERNO + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.GRIGIO:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_GRIGIO + TagItem.CLOSE_LABEL + ")";
                    break;
                case TypeProtocol.STAMPAREG:
                    labelTypeProto = " (" + TagItem.LABEL + TypeProtocol.LABEL_STAMPAREG + TagItem.CLOSE_LABEL + ")";
                    break;
            }
            return labelTypeProto;
        }

        private static string UpdateSpecializeItem(string specializeItem, string newValue, string typeLabel)
        {
            if (specializeItem.Contains(typeLabel))
            {
                string[] splitL = specializeItem.Split(new string[] { typeLabel + TagItem.CLOSE_LABEL }, StringSplitOptions.None);
                string oldValue = splitL[1].Split(new string[] { TagItem.CLOSE_LINE }, StringSplitOptions.None)[0];
                specializeItem = specializeItem.Replace(oldValue, newValue);
            }
            else
            {
                specializeItem += TagItem.LINE + TagItem.LABEL +
                    typeLabel + TagItem.CLOSE_LABEL + newValue + TagItem.CLOSE_LINE;
            }

            return specializeItem;
        }


        public async Task<bool>  UpdateNotifications(List<Notification> notificationList)
        {
            bool result = false;
            IDbContextTransaction? transaction = null;
            try
            {
                transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();                             
                await ((DbContext)this._dbContext).SaveChangesAsync();

                if (transaction != null)
                    await transaction.CommitAsync();

                result = true;
            }
            catch (Exception exc)
            {
                if (transaction != null)
                    await transaction?.RollbackAsync();

                this._logger.LogError(exception: exc, message: exc.Message);
                throw exc;
            }

            return result;
        }

       
        #endregion
    }


}
