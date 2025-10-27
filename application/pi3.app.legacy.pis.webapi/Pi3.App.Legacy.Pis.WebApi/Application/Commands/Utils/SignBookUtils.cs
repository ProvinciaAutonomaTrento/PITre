// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Runtime.CompilerServices;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils
{
    public class SignBookUtils
    {

        #region Public Members
        public static async Task<ProcessoFirma> GetProcessoFirma(long idProcessoAsLong, SchemaProcessoFirmaEntity processoFirmaEntity, IPi3DbContext dbContext)
        {
            ProcessoFirma output = null;
            InitializeMapper();
            try
            {             

                var passiFirmaEntities = await dbContext.PassoDiFirmaEntities
                    .Join(dbContext.AnagraficaEventiEntities, passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                    .GroupJoin(dbContext.CorrGlobaliEntities, j => j.passo.ID_RUOLO_COINVOLTO, ruolo => ruolo.ID_GRUPPO, (j, ruolo) => new { j.passo, j.evento, ruoloCoinvolto = ruolo })
                    .SelectMany(j => j.ruoloCoinvolto.DefaultIfEmpty(), (j, ruolo) => new { j.passo, j.evento, ruoloCoinvolto = ruolo })
                    .GroupJoin(dbContext.CorrGlobaliEntities, j => j.passo.ID_UTENTE_COINVOLTO, utente => utente.ID_PEOPLE, (j, utente) => new { j.passo, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                    .SelectMany(j => j.utenteCoinvolto.DefaultIfEmpty(), (j, utente) => new { j.passo, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                    .Where(j => j.passo.ID_PROCESSO == idProcessoAsLong)
                    .OrderBy(j => j.passo.NUMERO_SEQUENZA)
                    .Select(j => new PassoFirmaDiProcessoEntity()
                    {
                        PassoFirma = new PassoDiFirmaEntity()
                        {
                            ID_PASSO = j.passo.ID_PASSO,
                            ID_PROCESSO = j.passo.ID_PROCESSO,
                            NUMERO_SEQUENZA = j.passo.NUMERO_SEQUENZA,
                            TIPO_FIRMA = j.passo.TIPO_FIRMA,
                            TIPO_EVENTO = j.passo.TIPO_EVENTO,
                            NOTE = j.passo.NOTE,
                            SCADENZA = j.passo.SCADENZA,
                            ELIMINATO = j.passo.ELIMINATO,
                            TICK = j.passo.TICK,
                            ID_TIPO_RUOLO_COINVOLTO = j.passo.ID_TIPO_RUOLO_COINVOLTO,
                            ID_RUOLO_COINVOLTO = j.passo.ID_RUOLO_COINVOLTO,
                            ID_UTENTE_COINVOLTO = j.passo.ID_UTENTE_COINVOLTO,
                            ID_AOO = j.passo.ID_AOO,
                            ID_RF = j.passo.ID_RF,
                            ID_MAIL_REGISTRO = j.passo.ID_MAIL_REGISTRO,
                            CHA_AUTOMATICO = j.passo.CHA_AUTOMATICO,
                            CHA_FACOLTATIVO = j.passo.CHA_FACOLTATIVO,
                            ID_TIPOLOGIA = j.passo.ID_TIPOLOGIA,
                            ID_STATO_DIAGRAMMA = j.passo.ID_STATO_DIAGRAMMA,
                            VAR_POS_SEGNATURA = j.passo.VAR_POS_SEGNATURA,
                            CHA_POS_SEGNATURA = j.passo.CHA_POS_SEGNATURA
                        },
                        Evento = new AnagraficaEventiEntity()
                        {
                            ID_EVENTO = j.evento.ID_EVENTO,
                            VAR_COD_AZIONE = j.evento.VAR_COD_AZIONE,
                            CHA_TIPO_EVENTO = j.evento.CHA_TIPO_EVENTO,
                            DESCRIZIONE = j.evento.DESCRIZIONE,
                            GRUPPO = j.evento.GRUPPO,
                            CHA_AUTOMATICO = j.evento.CHA_AUTOMATICO
                        },
                        RuoloCoinvolto = new RuoloCoinvoltoEntity()
                        {
                            ID_GROUP = j.ruoloCoinvolto.ID_GRUPPO,
                            GROUP_ID = j.ruoloCoinvolto.VAR_COD_RUBRICA,
                            GROUP_DESCRIPTION = j.ruoloCoinvolto.VAR_DESC_CORR,
                            ID_CORR_GLOBALI = j.ruoloCoinvolto.SYSTEM_ID
                        },
                        UtenteCoinvolto = new UtenteCoinvoltoEntity()
                        {
                            ID_USER = j.utenteCoinvolto.ID_PEOPLE,
                            USER_ID = j.utenteCoinvolto.VAR_COD_RUBRICA,
                            USER_DESCRIPTION = j.utenteCoinvolto.VAR_DESC_CORR,
                            ID_CORR_GLOBALI = j.utenteCoinvolto.SYSTEM_ID
                        }
                    })
                    .AsNoTracking()
                    .ToListAsync();


                output = _mapper.Map<ProcessoFirma>(processoFirmaEntity);
                if (passiFirmaEntities != null && passiFirmaEntities.Count > 0)
                {
                    output.passi = _mapper.Map<PassoFirma[]>(passiFirmaEntities).ToList();
                    output.passi.ForEach(async p =>
                    {
                        p.idEventiDaNotificare = await dbContext.PassoEventoEntities.AsNoTracking()
                        .Join(dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.ID_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo.ID_PASSO, evento.GRUPPO })
                        .Where(j => j.ID_PASSO == p.idPasso.AsLong())
                        .Select(j => j.GRUPPO)
                        .Distinct()
                        .ToListAsync();                        
                    });

                }
            }
            catch (Exception ex)
            {
                output = null;
            }

            return output;
        }

        public static async Task<IstanzaProcessoDiFirma> GetIstanzaProcessoFirma(long idIstanzaProcesso, IPi3DbContext dbContext)
        {
            IstanzaProcessoDiFirma output = new();
            InitializeMapper();
            try
            {
                var idIstanza = idIstanzaProcesso;

                var istanzaProcessoFirmaEntity = await dbContext.IstanzaProcessoFirmaEntities
                    .Join(dbContext.CorrGlobaliEntities, istanza => istanza.ID_RUOLO_PROPONENTE, ruoloProponente => ruoloProponente.ID_GRUPPO, (istanza, ruoloProponente) => new { istanza, ruoloProponente })
                    .Join(dbContext.CorrGlobaliEntities, j => j.istanza.ID_UTENTE_PROPONENTE, utenteProponente => utenteProponente.ID_PEOPLE, (j, utenteProponente) => new { j.istanza, j.ruoloProponente, utenteProponente })
                    .GroupJoin(dbContext.PeopleEntities, j => j.istanza.ID_PEOPLE_DELEGATO, delegato => delegato.SYSTEM_ID, (j, delegato) => new { j.istanza, j.ruoloProponente, j.utenteProponente, delegato })
                    .SelectMany(j => j.delegato.DefaultIfEmpty(), (j, delegato) =>
                    new IstanzaProcessiFirmaEntity()
                    {
                        IstanzaProcessoFirma = j.istanza,
                        RuoloProponente = new RuoloEntity()
                        {
                            ID_GROUP = j.ruoloProponente.ID_GRUPPO,
                            GROUP_DESCRIPTION = j.ruoloProponente.VAR_DESC_CORR,
                            GROUP_ID = j.ruoloProponente.VAR_COD_RUBRICA,
                            ID_CORR_GLOBALI_RUOLO = j.ruoloProponente.SYSTEM_ID
                        },
                        UtenteProponente = new UtenteEntity()
                        {
                            ID_CORR_GLOBALI_UTENTE = j.utenteProponente.SYSTEM_ID,
                            ID_USER = j.utenteProponente.ID_PEOPLE,
                            USER_DESCRIPTION = j.utenteProponente.VAR_DESC_CORR,
                            USER_ID = j.utenteProponente.VAR_COD_RUBRICA
                        },
                        DescUtenteDelegato = delegato.FULL_NAME
                    })
                    .Where(j => j.IstanzaProcessoFirma.ID_ISTANZA == idIstanza)
                    .FirstAsync();

                istanzaProcessoFirmaEntity.istanzePassoDiFirma = await dbContext.IstanzaPassoFirmaEntities
                    .Join(dbContext.AnagraficaEventiEntities, istanzaPasso => istanzaPasso.TIPO_EVENTO, evento => evento.ID_EVENTO, (istanzaPasso, evento) => new { istanzaPasso, evento })
                    .LeftJoin(dbContext.CorrGlobaliEntities, j => j.istanzaPasso.ID_RUOLO_COINVOLTO, ruoloCoinvolto => ruoloCoinvolto.ID_GRUPPO, (j, ruoloCoinvolto) => new { j.istanzaPasso, j.evento, ruoloCoinvolto })
                    .GroupJoin(dbContext.CorrGlobaliEntities, j => j.istanzaPasso.ID_UTENTE_COINVOLTO, utente => utente.ID_PEOPLE, (j, utente) => new { j.istanzaPasso, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                    .SelectMany(j => j.utenteCoinvolto.DefaultIfEmpty(), (j, utente) => new { j.istanzaPasso, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                    .Where(j => j.istanzaPasso.ID_ISTANZA_PROCESSO == idIstanza)
                    .Select(j => new IstanzaPassiFirmaEntity()
                    {
                        IstanzaPassoFirma = j.istanzaPasso,
                        Evento = j.evento,
                        RuoloCoinvolto = new RuoloEntity()
                        {
                            ID_GROUP = j.ruoloCoinvolto != null ? j.ruoloCoinvolto.ID_GRUPPO : null,
                            GROUP_DESCRIPTION = j.ruoloCoinvolto != null ? j.ruoloCoinvolto.VAR_DESC_CORR : string.Empty,
                            GROUP_ID = j.ruoloCoinvolto != null ? j.ruoloCoinvolto.VAR_COD_RUBRICA : string.Empty,
                            ID_CORR_GLOBALI_RUOLO = j.ruoloCoinvolto != null ? j.ruoloCoinvolto.SYSTEM_ID : 0
                        },
                        UtenteCoinvolto = new UtenteEntity()
                        {
                            ID_CORR_GLOBALI_UTENTE = j.utenteCoinvolto.SYSTEM_ID,
                            ID_USER = j.utenteCoinvolto.ID_PEOPLE,
                            USER_DESCRIPTION = j.utenteCoinvolto.VAR_DESC_CORR, 
                            USER_ID = j.utenteCoinvolto.VAR_COD_RUBRICA,
                            NAME = j.utenteCoinvolto.VAR_NOME,
                            SURNAME = j.utenteCoinvolto.VAR_COGNOME
                        }
                    })
                    .OrderBy(j => j.IstanzaPassoFirma.NUMERO_SEQUENZA).ToListAsync();

                output = _mapper.Map<IstanzaProcessoDiFirma>(istanzaProcessoFirmaEntity);

            }
            catch (Exception ex)
            {
                output = new();
            }
            return output;
        }


        public static string GetEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") ||
                    items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M"))
                    )
                {
                    retValue = items[i];
                    break;
                }
            }
            return retValue;
        }

        public static DocsPaVO.LibroFirma.ProcessoFirma GetProcessoFirmaFromDomain(SignBook.SignatureProcess process, IPi3DbContext dbContext)
        {
            DocsPaVO.LibroFirma.ProcessoFirma retVal = new DocsPaVO.LibroFirma.ProcessoFirma();
            retVal.idPeopleAutore = process.AuthorUserId;
            retVal.idProcesso = process.IdProcess;
            retVal.idRuoloAutore = process.AuthorRoleId;
            retVal.isInvalidated = process.isInvalidated;
            retVal.IsProcessModel = process.IsProcessModel;
            retVal.nome = process.Name;
            retVal.passi = new List<DocsPaVO.LibroFirma.PassoFirma>();

            foreach (SignBook.SignatureStep step in process.Steps)
            {
                retVal.passi.Add(getPassoDiFirmaFromDomain(step, dbContext));
            }

            return retVal;
        }

        public static DocsPaVO.LibroFirma.PassoFirma getPassoDiFirmaFromDomain(SignBook.SignatureStep step, IPi3DbContext dbContext)
        {
            DocsPaVO.LibroFirma.PassoFirma retVal = new DocsPaVO.LibroFirma.PassoFirma();
            retVal.DaAggiornare = step.ToUpdate;
            retVal.dataScadenza = step.ExpirationDate;
            retVal.Evento = getEventoLibroFirmaFromDomain(step.Event);
            retVal.idEventiDaNotificare = step.EventsToNotifyIds;
            retVal.idPasso = step.IdStep;
            retVal.idProcesso = step.IdProcess;
            retVal.Invalidated = step.Invalidated;
            retVal.IsModello = step.IsModel;
            retVal.note = step.Note;
            retVal.numeroSequenza = step.SequenceNumber;
            if (step.InvolvedRole != null && !string.IsNullOrEmpty(step.InvolvedRole.Id)) { retVal.ruoloCoinvolto = DBUtils.getRuoloByIdGruppo(step.InvolvedRole.Id, dbContext); }
            if (step.InvolvedUser != null && !string.IsNullOrEmpty(step.InvolvedUser.Id)) { retVal.utenteCoinvolto = DBUtils.getUtenteById(step.InvolvedUser.Id, dbContext); }
            if (!string.IsNullOrEmpty(step.InvolvedRoleTypeCode) && !string.IsNullOrEmpty(step.InvolvedRoleTypeIdAmm)) { retVal.TpoRuoloCoinvolto = DBUtils.getTipoRuoloByCodice(step.InvolvedRoleTypeCode, step.InvolvedRoleTypeIdAmm, dbContext); }

            return retVal;
        }

        public static DocsPaVO.LibroFirma.Evento getEventoLibroFirmaFromDomain(SignBook.Event ev)
        {
            DocsPaVO.LibroFirma.Evento retVal = new DocsPaVO.LibroFirma.Evento();
            retVal.CodiceAzione = ev.CodeAction;
            retVal.Descrizione = ev.Description;
            retVal.Gruppo = ev.Group;
            retVal.IdEvento = ev.IdEvent;
            retVal.TipoEvento = ev.EventType;


            return retVal;
        }

        #endregion

        #region Private Members

        protected static IMapper _mapper = null;

        protected static void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SchemaProcessoFirmaEntity, ProcessoFirma>()
                     .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.ID_PROCESSO))
                     .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.NOME))
                     .ForMember(dest => dest.IdStatoInterruzione, opt => opt.MapFrom(src => src.ID_STATO_INTERRUZIONE != null ? src.ID_STATO_INTERRUZIONE.ToString() : string.Empty))
                     .ForMember(dest => dest.isInvalidated, opt => opt.MapFrom(src => src.TICK == "1"))
                     .ForMember(dest => dest.IsProcessModel, opt => opt.MapFrom(src => src.CHA_MODELLO == "1"));

                cfg.CreateMap<AnagraficaEventiEntity, Evento>()
                    .ForMember(dest => dest.IdEvento, opt => opt.MapFrom(src => src.ID_EVENTO))
                    .ForMember(dest => dest.CodiceAzione, opt => opt.MapFrom(src => src.VAR_COD_AZIONE))
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                    .ForMember(dest => dest.TipoEvento, opt => opt.MapFrom(src => src.CHA_TIPO_EVENTO))
                    .ForMember(dest => dest.Gruppo, opt => opt.MapFrom(src => src.GRUPPO))
                    .ForMember(dest => dest.Automatico, opt => opt.MapFrom(src => src.CHA_AUTOMATICO == "1"));

                cfg.CreateMap<RuoloCoinvoltoEntity, Ruolo>()
                   .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GROUP))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.GROUP_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.GROUP_DESCRIPTION))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI));

                cfg.CreateMap<UtenteCoinvoltoEntity, Utente>()
                   .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.ID_USER))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.USER_DESCRIPTION))
                   .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI));

                cfg.CreateMap<PassoFirmaDiProcessoEntity, PassoFirma>()
                    .ForMember(dest => dest.idPasso, opt => opt.MapFrom(src => src.PassoFirma.ID_PASSO))
                    .ForMember(dest => dest.numeroSequenza, opt => opt.MapFrom(src => src.PassoFirma.NUMERO_SEQUENZA))
                    .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.PassoFirma.ID_PROCESSO))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.PassoFirma.NOTE))
                    .ForMember(dest => dest.Invalidated, opt => opt.MapFrom(src => src.PassoFirma.TICK))
                    .ForMember(dest => dest.IdAOO, opt => opt.MapFrom(src => src.PassoFirma.ID_AOO))
                    .ForMember(dest => dest.IdRF, opt => opt.MapFrom(src => src.PassoFirma.ID_RF))
                    .ForMember(dest => dest.IdMailRegistro, opt => opt.MapFrom(src => src.PassoFirma.ID_MAIL_REGISTRO))
                    .ForMember(dest => dest.IdTipologia, opt => opt.MapFrom(src => src.PassoFirma.ID_TIPOLOGIA))
                    .ForMember(dest => dest.IdStatoDiagramma, opt => opt.MapFrom(src => src.PassoFirma.ID_STATO_DIAGRAMMA))
                    .ForMember(dest => dest.IsAutomatico, opt => opt.MapFrom(src => src.PassoFirma.CHA_AUTOMATICO == "1"))
                    .ForMember(dest => dest.IsFacoltativo, opt => opt.MapFrom(src => src.PassoFirma.CHA_FACOLTATIVO == "1"))
                    .ForMember(dest => dest.ApplicaSegnaturaPermanente, opt => opt.MapFrom(src => src.PassoFirma.CHA_POS_SEGNATURA))
                    .ForMember(dest => dest.PosizioneSegnaturaPermanente, opt => opt.MapFrom(src => src.PassoFirma.VAR_POS_SEGNATURA))
                    .AfterMap((src, dest) =>
                    {
                        dest.IsModello = false;
                        if (!src.Evento.CHA_TIPO_EVENTO.Equals("W") && src.PassoFirma.ID_RUOLO_COINVOLTO == null)
                        {
                            dest.IsModello = true;
                        }
                        if (src.PassoFirma.CHA_AUTOMATICO == "1")
                        {

                            if (src.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) ||
                                src.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()) ||
                                src.Evento.VAR_COD_AZIONE.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                            {
                                if (src.PassoFirma.ID_AOO == null || src.PassoFirma.ID_RF == null)
                                {
                                    dest.IsModello = true;
                                }
                                if (src.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && src.PassoFirma.ID_MAIL_REGISTRO == null)
                                {
                                    dest.IsModello = true;
                                }
                            }
                            if (src.Evento.VAR_COD_AZIONE.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && (src.PassoFirma.ID_TIPOLOGIA == null || src.PassoFirma.ID_STATO_DIAGRAMMA == null))
                            {
                                dest.IsModello = true;
                            }
                        }
                    });

                cfg.CreateMap<IstanzaProcessiFirmaEntity, IstanzaProcessoDiFirma>()
                     .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_ISTANZA))
                     .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_PROCESSO))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.DESCRIZIONE))
                     .ForMember(dest => dest.dataAttivazione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ATTIVATO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.dataChiusura, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CONCLUSO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.NoteDiAvvio, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.NOTE ?? string.Empty))
                     .ForMember(dest => dest.MotivoRespingimento, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.MOTIVO_RESPINGIMENTO ?? string.Empty))
                     .ForMember(dest => dest.DescUtenteDelegato, opt => opt.MapFrom(src => src.DescUtenteDelegato ?? string.Empty))
                     .ForMember(dest => dest.docAll, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.DOC_ALL ?? string.Empty))
                     .ForMember(dest => dest.statoProcesso, opt => opt.MapFrom(src => (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), src.IstanzaProcessoFirma.STATO)))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_DOCUMENTO))
                     .ForMember(dest => dest.ChaInterroDa, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CHA_INTERROTTO_DA != null ? Convert.ToChar(src.IstanzaProcessoFirma.CHA_INTERROTTO_DA) : '0'))
                     .ForMember(dest => dest.AttivatoPerPassaggioStato, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CHA_CAMBIO_STATO_DIAG == "1"))
                     .ForMember(dest => dest.IdStatoInterruzione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_STATO_INTERRUZIONE != null ? src.IstanzaProcessoFirma.ID_STATO_INTERRUZIONE.ToString() : string.Empty))
                     .AfterMap((src, dest) =>
                     {
                         dest.Notifiche = new OpzioniNotifica()
                         {
                             Notifica_concluso = src.IstanzaProcessoFirma.NOTIFICA_CONCLUSO == "1",
                             Notifica_interrotto = src.IstanzaProcessoFirma.NOTIFICA_INTERROTTO == "1",
                             NotificaErrore = src.IstanzaProcessoFirma.NOTIFICA_ERRORE == "1",
                             NotificaPresenzaDestNonInterop = src.IstanzaProcessoFirma.NOTIFICA_DEST_NON_INTEROP == "1"
                         };
                     });
                cfg.CreateMap<IstanzaPassiFirmaEntity, IstanzaPassoDiFirma>()
                    .ForMember(dest => dest.CodiceTipoEvento, opt => opt.MapFrom(src => src.IstanzaPassoFirma.TIPO_FIRMA))
                    .ForMember(dest => dest.dataEsecuzione, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ESEGUITO_IL.AsDateTimeFormat()))
                    .ForMember(dest => dest.dataScadenza, opt => opt.MapFrom(src => src.IstanzaPassoFirma.SCADENZA.AsDateTimeFormat()))
                    .ForMember(dest => dest.statoPasso, opt => opt.MapFrom(src => (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), src.IstanzaPassoFirma.STATO_PASSO)))
                    .ForMember(dest => dest.idIstanzaPasso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_ISTANZA_PASSO))
                    .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_ISTANZA_PROCESSO))
                    .ForMember(dest => dest.idNotificaEffettuata, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_NOTIFICA_EFFETTUATA.HasValue ? src.IstanzaPassoFirma.ID_NOTIFICA_EFFETTUATA.ToString() : string.Empty))
                    .ForMember(dest => dest.idPasso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_PASSO))
                    .ForMember(dest => dest.motivoRespingimento, opt => opt.MapFrom(src => src.IstanzaPassoFirma.MOTIVO_RESPINGIMENTO ?? string.Empty))
                    .ForMember(dest => dest.numeroSequenza, opt => opt.MapFrom(src => src.IstanzaPassoFirma.NUMERO_SEQUENZA))
                    .ForMember(dest => dest.DescrizioneUtenteLocker, opt => opt.MapFrom(src => src.IstanzaPassoFirma.DESC_UTENTE_LOCKER))
                    .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.IstanzaPassoFirma.NOTE ?? string.Empty))
                    .ForMember(dest => dest.TipoFirma, opt => opt.MapFrom(src => src.IstanzaPassoFirma.TIPO_FIRMA))
                    .ForMember(dest => dest.IdAOO, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_AOO))
                    .ForMember(dest => dest.IdRF, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_RF))
                    .ForMember(dest => dest.IdMailRegistro, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_MAIL_REGISTRO))
                    .ForMember(dest => dest.IdTipologia, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_TIPOLOGIA))
                    .ForMember(dest => dest.IdStatoDiagramma, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_STATO_DIAGRAMMA))
                    .ForMember(dest => dest.IsAutomatico, opt => opt.MapFrom(src => src.IstanzaPassoFirma.CHA_AUTOMATICO == "1"))
                    .ForMember(dest => dest.Errore, opt => opt.MapFrom(src => src.IstanzaPassoFirma.VAR_ERRORE))
                    .ForMember(dest => dest.ApplicaSegnaturaPermanente, opt => opt.MapFrom(src => src.IstanzaPassoFirma.CHA_POS_SEGNATURA))
                    .ForMember(dest => dest.PosizioneSegnaturaPermanente, opt => opt.MapFrom(src => src.IstanzaPassoFirma.VAR_POS_SEGNATURA));

                cfg.CreateMap<RuoloEntity, Ruolo>()
                  .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GROUP != null ? src.ID_GROUP.ToString() : string.Empty))
                  .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.GROUP_ID))
                  .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.GROUP_DESCRIPTION ?? string.Empty))
                  .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI_RUOLO));

                cfg.CreateMap<UtenteEntity, Utente>()
                   .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.ID_USER))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.USER_DESCRIPTION))
                   .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI_UTENTE))
                   .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.NAME))
                   .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.SURNAME))
                   ;
                                
            });

            _mapper = configuration.CreateMapper();
        }

        protected class PassoFirmaDiProcessoEntity
        {
            public PassoDiFirmaEntity PassoFirma { get; set; }
            public AnagraficaEventiEntity Evento { get; set; }
            public RuoloCoinvoltoEntity RuoloCoinvolto { get; set; }
            public UtenteCoinvoltoEntity? UtenteCoinvolto { get; set; }
        }

        protected class RuoloCoinvoltoEntity
        {
            public long? ID_GROUP { get; set; }
            public string? GROUP_ID { get; set; }
            public string? GROUP_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
        }

        protected class UtenteCoinvoltoEntity
        {
            public long? ID_USER { get; set; }
            public string? USER_ID { get; set; }
            public string? USER_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
        }

        protected class IstanzaProcessiFirmaEntity
        {
            public IstanzaProcessoFirmaEntity IstanzaProcessoFirma { get; set; }

            public string? DescUtenteDelegato { get; set; }
            public RuoloEntity RuoloProponente { get; set; }
            public UtenteEntity UtenteProponente { get; set; }
            public List<IstanzaPassiFirmaEntity> istanzePassoDiFirma { get; set; }
        }

        protected class RuoloEntity
        {
            public long? ID_GROUP { get; set; }
            public string GROUP_ID { get; set; }
            public string GROUP_DESCRIPTION { get; set; }
            public long ID_CORR_GLOBALI_RUOLO { get; set; }
        }

        protected class UtenteEntity
        {
            public long? ID_USER { get; set; }
            public string? USER_ID { get; set; }
            public string? USER_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI_UTENTE { get; set; }
            public string? NAME { get; set; }
            public string? SURNAME { get; set; }
        }

        protected class IstanzaPassiFirmaEntity
        {
            public IstanzaPassoFirmaEntity IstanzaPassoFirma { get; set; }
            public AnagraficaEventiEntity Evento { get; set; }
            public RuoloEntity RuoloCoinvolto { get; set; }
            public UtenteEntity? UtenteCoinvolto { get; set; }
        }
        #endregion

    }


}
