// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.Grid;
using DocsPaVO.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReadNotifications
{
    public class ReadNotificationsHandler : IRequestHandler<Requests.ReadNotifications, ReadNotificationsResult>
    {

        protected readonly ILogger<ReadNotificationsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected IMapper _mapper;

        public ReadNotificationsHandler(
            ILogger<ReadNotificationsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            this._mapper = InitializeMapper();
        }



        public async Task<ReadNotificationsResult> Handle(Requests.ReadNotifications request, CancellationToken cancellationToken)
        {
            DocsPaVO.Notification.Notification[]? notifications = null;
            try
            {
                var idPeople = request.idPeople.AsLong();
                var idGroup = request.idGroup.AsLong();

                var notificationOutput = await this._dbContext.NotifyEntities
                    .Where( n => n.ID_PEOPLE_RECEIVER == idPeople && (n.ID_GROUP_RECEIVER == 0 || n.ID_GROUP_RECEIVER == idGroup))
                    .Select(s => new NotificationtEntitySelect
                    {
                        ID_NOTIFY = s.SYSTEM_ID,
                        ID_EVENT = s.ID_EVENT,
                        PRODUCER = s.DESC_PRODUCER,
                        ID_PEOPLE = s.ID_PEOPLE_RECEIVER,
                        ID_GROUP = s.ID_GROUP_RECEIVER,
                        TYPE_NOTIFICATION = s.TYPE_NOTIFY,
                        DTA_NOTIFY = s.DTA_NOTIFY,
                        ITEM1 = s.FIELD_1,
                        ITEM2 = s.FIELD_2,
                        ITEM3 = s.FIELD_3,
                        ITEM4 = s.FIELD_4,
                        MULTIPLICITY = s.MULTIPLICITY,
                        ITEM_SPECIALIZED = s.SPECIALIZED_FIELD,
                        TYPE_EVENT = s.TYPE_EVENT,
                        DOMAINOBJECT = s.DOMAINOBJECT,
                        ID_OBJECT = s.ID_OBJECT,
                        ID_SPECIALIZED_OBJECT = s.ID_SPECIALIZED_OBJECT,
                        DTA_EVENT = s.DTA_EVENT,
                        READ_NOTIFICATION = s.READ_NOTIFICATION,
                        COLOR = s.COLOR,
                        NOTES = s.NOTES,
                        EXTENSION = IPi3DbContextMappedFunctions.GetChaImg(s.ID_OBJECT.Value),
                        SIGNED = IPi3DbContextMappedFunctions.GetChaFirmato(s.ID_OBJECT.Value)
                    })
                    .OrderByDescending( n => n.DTA_EVENT )
                    .ToListAsync(cancellationToken: cancellationToken);

                notifications = this._mapper.Map<Notification[]>(notificationOutput);
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, "{Message}", ex.Message);
            }

            ReadNotificationsResult result = new(notifications ?? Array.Empty<DocsPaVO.Notification.Notification>());

            return result;
        }


        private static IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NotificationtEntitySelect, Notification>()
                    .ForMember( dest => dest.ID_NOTIFY, opt => opt.MapFrom(src => src.ID_NOTIFY))
                    .ForMember(dest => dest.ID_EVENT, opt => opt.MapFrom(src => src.ID_EVENT))
                    .ForMember(dest => dest.PRODUCER, opt => opt.MapFrom(src => src.PRODUCER))
                    .ForMember(dest => dest.DTA_EVENT, opt => opt.MapFrom(src => src.DTA_EVENT))
                    .ForMember(dest => dest.DTA_NOTIFY, opt => opt.MapFrom(src => src.DTA_NOTIFY))
                    .ForMember(dest => dest.ID_PEOPLE, opt => opt.MapFrom(src => src.ID_PEOPLE))
                    .ForMember(dest => dest.ID_GROUP, opt => opt.MapFrom(src => src.ID_GROUP))
                    .ForMember(dest => dest.TYPE_NOTIFICATION, opt => opt.MapFrom(src => src.TYPE_NOTIFICATION ?? string.Empty))
                    .ForMember(dest => dest.TYPE_EVENT, opt => opt.MapFrom(src => src.TYPE_EVENT))
                    .ForMember(dest => dest.MULTIPLICITY, opt => opt.MapFrom(src => src.MULTIPLICITY))
                    .ForMember(dest => dest.DOMAINOBJECT, opt => opt.MapFrom(src => src.DOMAINOBJECT))
                    .ForMember(dest => dest.ID_OBJECT, opt => opt.MapFrom(src => src.ID_OBJECT))
                    .ForMember(dest => dest.ID_SPECIALIZED_OBJECT, opt => opt.MapFrom(src => src.ID_SPECIALIZED_OBJECT))
                    .ForMember(dest => dest.READ_NOTIFICATION, opt => opt.MapFrom(src => src.READ_NOTIFICATION ?? "0"))
                    .ForMember(dest => dest.ITEMS, opt => opt.MapFrom<Items>( src => new Items() { 
                                                                                            ITEM1 = src.ITEM1 ?? String.Empty,
                                                                                            ITEM2 = src.ITEM2 ?? String.Empty,
                                                                                            ITEM3 = src.ITEM3 ?? String.Empty,
                                                                                            ITEM4 = src.ITEM4 ?? String.Empty
                                                                                         }) )
                    .ForMember(dest => dest.ITEM_SPECIALIZED, opt => opt.MapFrom(src => src.ITEM_SPECIALIZED))
                    .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.COLOR))
                    .ForMember(dest => dest.NOTES, opt => opt.MapFrom(src => src.NOTES ?? string.Empty))
                    .ForMember(dest => dest.EXTENSION, opt => opt.MapFrom(src => src.EXTENSION == null || src.EXTENSION == "0" ? string.Empty : src.EXTENSION))
                    .ForMember(dest => dest.SIGNED, opt => opt.MapFrom(src => src.SIGNED == null || src.SIGNED == "0" ? "0" : src.SIGNED))
                    .ForMember(dest => dest.ACCESSRIGHTS, opt => opt.MapFrom(src => String.Empty))
                    .ForMember(dest => dest.TEXT_SORTING, opt => opt.MapFrom(src => String.Empty));
            });

            return configuration.CreateMapper();
        }

        private class NotificationtEntitySelect
        {
            public long ID_NOTIFY { get; internal set; }
            public long ID_EVENT { get; internal set; }
            public string? PRODUCER { get; internal set; }
            public long ID_PEOPLE { get; internal set; }
            public long? ID_GROUP { get; internal set; }
            public string? TYPE_NOTIFICATION { get; internal set; }
            public DateTime DTA_NOTIFY { get; internal set; }
            public string? ITEM1 { get; internal set; }
            public string? ITEM2 { get; internal set; }
            public string? ITEM3 { get; internal set; }
            public string? ITEM4 { get; internal set; }
            public string? MULTIPLICITY { get; internal set; }
            public string? ITEM_SPECIALIZED { get; internal set; }
            public string? TYPE_EVENT { get; internal set; }
            public string? DOMAINOBJECT { get; internal set; }
            public long? ID_OBJECT { get; internal set; }
            public long? ID_SPECIALIZED_OBJECT { get; internal set; }
            public DateTime DTA_EVENT { get; internal set; }
            public string? READ_NOTIFICATION { get; internal set; }
            public string? COLOR { get; internal set; }
            public string? NOTES { get; internal set; }
            public string? EXTENSION { get; internal set; }
            public string? SIGNED { get; internal set; }
        }


    }
}
