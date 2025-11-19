// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.DatiCert;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ricercaNotificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ricercaNotifica;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.ricercaNotifica
{
    public class ricercaNotificaHandler : IRequestHandler<ricercaNotificaRequest, ricercaNotificaResult>
    {


        private readonly IPi3DbContext _dbContext;
        private readonly ILogger<ricercaNotificaHandler> _logger;
        protected IMapper _mapper = null;
        public ricercaNotificaHandler(
            IPi3DbContext dbContext,
            ILogger<ricercaNotificaHandler> logger
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this.InitializeMapper();
        }

        public async Task<ricercaNotificaResult> Handle(ricercaNotificaRequest request, CancellationToken cancellationToken)
        {
            List<Notifica> output = new List<Notifica>();
            Notifica[] notifica = new Notifica[0];

            try
            {
                var notifications = await (from not in this._dbContext.NotificaEntities.AsNoTracking()
                                           where not.DOCNUMBER == request.docNumber.AsLong()
                                           select not).ToListAsync();

                foreach (var not in notifications)
                {
                    var mappedNot = this._mapper.Map<Notifica>(not);

                    if (Uri.IsWellFormedUriString(mappedNot.mittente, UriKind.Absolute))
                    {
                        mappedNot.destinatario = await this.GetDestinatarioPerIs(mappedNot.destinatario);
                    }

                    output.Add(mappedNot);
                }

                notifica = output.ToArray();
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                notifica = new Notifica[0];

            }

            return new ricercaNotificaResult(notifica) ;
        }


        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NotificaEntity, Notifica>()
                     .ForMember(dest => dest.idNotifica, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.mittente, opt => opt.MapFrom(src => src.VAR_MITTENTE))
                     .ForMember(dest => dest.tipoDestinatario, opt => opt.MapFrom(src => src.VAR_TIPO_DESTINATARIO))
                     .ForMember(dest => dest.destinatario, opt => opt.MapFrom(src => src.VAR_DESTINATARIO))
                     .ForMember(dest => dest.risposte, opt => opt.MapFrom(src => src.VAR_RISPOSTE))
                     .ForMember(dest => dest.oggetto, opt => opt.MapFrom(src => src.VAR_OGGETTO))
                     .ForMember(dest => dest.gestioneEmittente, opt => opt.MapFrom(src => src.VAR_GESTIONE_EMITTENTE))
                     .ForMember(dest => dest.zona, opt => opt.MapFrom(src => src.VAR_ZONA))
                     .ForMember(dest => dest.data_ora, opt => opt.MapFrom(src => ((DateTime) src.VAR_GIORNO_ORA).ToString("dd/MM/yyyy HH:mm:ss")))
                     .ForMember(dest => dest.identificativo, opt => opt.MapFrom(src => src.VAR_IDENTIFICATIVO))
                     .ForMember(dest => dest.msgid, opt => opt.MapFrom(src => src.VAR_MSGID))
                     .ForMember(dest => dest.tipoRicevuta, opt => opt.MapFrom(src => src.VAR_TIPO_RICEVUTA))
                     .ForMember(dest => dest.consegna, opt => opt.MapFrom(src => src.VAR_CONSEGNA))
                     .ForMember(dest => dest.ricezione, opt => opt.MapFrom(src => src.VAR_RICEZIONE))
                     .ForMember(dest => dest.errore_esteso, opt => opt.MapFrom(src => src.VAR_ERRORE_ESTESO))
                     .ForMember(dest => dest.docnumber, opt => opt.MapFrom(src => src.DOCNUMBER))
                     .ForMember(dest => dest.idTipoNotifica, opt => opt.MapFrom(src => src.ID_TIPO_NOTIFICA))
                     .ForMember(dest => dest.erroreRicevuta, opt => opt.MapFrom(src => src.VAR_ERRORE_RICEVUTA))
                     ;
            });

            this._mapper = configuration.CreateMapper();
        }

        private async Task<string> GetDestinatarioPerIs(string destinatario)
        {
            // Pulizia dei codici (nel caso di notifica di rifiuto della spedizione, vendono 
            // inviati dei codici di corrispondenti racchiusi fra singolo apice e separati da virgola
            string[] codici = destinatario.Replace("'", string.Empty).Split(',');

            // Descrizione dei corrispondenti
            string descrizione = string.Empty;

            // Risoluzione di tutti i codici
            foreach (var codice in codici)
                descrizione += string.Format("{0} ({1}), ",
                    await this.GetCorrAttribute(codice.Trim()),
                    codice);

            return descrizione.Substring(0, descrizione.Length - 2);

        }


        private async Task<string> GetCorrAttribute(string corrCode)
        {
            string retVal = corrCode;
            retVal = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(corr => corr.VAR_CODICE == corrCode).Select(corr => corr.VAR_DESC_CORR).FirstAsync();

            return retVal;

        }
    }
}
