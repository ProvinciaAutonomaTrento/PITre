// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using LinqKit;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.Conservazione
{
    internal static class ServiceExtensions
    {
        internal static DateTime? GetDataProssimaEsecuzione(this PolicyParerEntity p)
        {
            var day = DateTime.Now;

            switch (p.CHA_PERIODICITA)
            {
                case "D":
                    return DateTime.Now.AddDays(1).Date;

                case "W":
                    day = DateTime.Now;
                    var dayOfWeek = DateTime.Now.DayOfWeek;
                    switch (p.CHA_ESECUZIONE_GIORNO)
                    {
                        case 1:
                            dayOfWeek = DayOfWeek.Monday;
                            break;
                        case 2:
                            dayOfWeek = DayOfWeek.Tuesday;
                            break;
                        case 3:
                            dayOfWeek = DayOfWeek.Wednesday;
                            break;
                        case 4:
                            dayOfWeek = DayOfWeek.Thursday;
                            break;
                        case 5:
                            dayOfWeek = DayOfWeek.Friday;
                            break;
                        case 6:
                            dayOfWeek = DayOfWeek.Saturday;
                            break;
                        case 7:
                            dayOfWeek = DayOfWeek.Sunday;
                            break;
                    }
                    do { day = day.AddDays(1); } while (day.DayOfWeek != dayOfWeek);

                    return day.Date;

                case "M":
                    var d = p.CHA_ESECUZIONE_GIORNO < 31 ? (int)p.CHA_ESECUZIONE_GIORNO : DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    if (DateTime.Now.Month == 2 && d > 28) d = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                    day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, d);

                    return DateTime.Compare(day, DateTime.Now) > 0 ? day.Date : day.AddMonths(1).Date;

                case "Y":
                    day = new DateTime(DateTime.Now.Year, (int)p.CHA_ESECUZIONE_MESE, (int)p.CHA_ESECUZIONE_GIORNO);
                    return DateTime.Compare(day, DateTime.Now) > 0 ? day.Date : day.AddYears(1).Date;

                default:
                    return null;

            }
        }

        internal static void AppendFiltriBase(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            context.Query = context.Query.Where(x => x.EXT != null
                                                    && !(x.CHA_IN_CESTINO == "1")
                                                    && !(x.IN_LIBROFIRMA == "1")
                                                    && !context.DbContext.CheckinCheckoutEntities.Any(c => c.ID_DOCUMENT == x.SYSTEM_ID)
                                                    && !context.DbContext.ProfileEntities.Any(y => y.ID_DOCUMENTO_PRINCIPALE == x.SYSTEM_ID && y.IN_LIBROFIRMA == "1")
                                                    );


            switch(p.CHA_TIPO_POLICY)
            {
                case "D":
                    context.Query = context.Query.Where(x => !(x.VAR_PROF_OGGETTO.ToUpper().Contains("REPORT ESECUZIONE POLICY") || x.VAR_PROF_OGGETTO.ToUpper().Contains("REPORT VERSAMENTI")));
                    context.Query = context.Query.Where(x => !context.DbContext.RegistriRepertorioEntities.AsNoTracking()
                                                                .Join(context.DbContext.OggettiCustomEntities.AsNoTracking(), r => r.COUNTERID, o => o.SYSTEM_ID, (r, o) => new { r, o })
                                                                .Where(y => y.r.TIPOLOGYID == x.ID_TIPO_ATTO
                                                                         && y.o.CHA_CONS_REPERTORIO == "1"
                                                                         && string.IsNullOrWhiteSpace(IPi3DbContextMappedFunctions.GetSegnaturaRepertorio(x.SYSTEM_ID, p.ID_AMM.Value))
                                                                ).Any());
                    context.Query = context.Query.Where(x => context.DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_AMM == p.ID_AMM && c.SYSTEM_ID == x.ID_UO_CREATORE).Any());
                    break;

                case "S":
                    switch(p.CHA_TIPO_REGISTRO_STAMPA)
                    {
                        case "R":
                            context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.AsNoTracking()
                                                                    .Join(context.DbContext.RegistroEntities.AsNoTracking(), s => s.ID_REGISTRO, r => r.SYSTEM_ID, (s, r) => new { s, r })
                                                                    .Where(y => y.s.DOCNUMBER == x.SYSTEM_ID
                                                                            && y.r.ID_AMM == p.ID_AMM)
                                                                    .Any());
                                                                    
                            break;
                        case "C":
                            context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.AsNoTracking()
                                                                     .Join(context.DbContext.RegistriRepertorioEntities.AsNoTracking(), s => s.ID_REPERTORIO, r => r.COUNTERID, (s, r) => new { s.DOCNUMBER, r.TIPOLOGYID, r.COUNTERID })
                                                                     .Join(context.DbContext.TipoAttoEntities.AsNoTracking(), s2 => s2.TIPOLOGYID, t => t.SYSTEM_ID, (s2, t) => new { s2.DOCNUMBER, s2.COUNTERID, t.ID_AMM })
                                                                     .Join(context.DbContext.OggettiCustomEntities.AsNoTracking(), s3 => s3.COUNTERID, o => o.SYSTEM_ID, (s3, o) => new { s3.DOCNUMBER, s3.ID_AMM, o.CHA_CONS_REPERTORIO })
                                                                     .Where(y => y.DOCNUMBER == x.SYSTEM_ID
                                                                            && y.ID_AMM == p.ID_AMM
                                                                            && y.CHA_CONS_REPERTORIO == "1")
                                                                     .Any());
                            break;
                    }
                    break;
            }
        }

        internal static void AppendFiltroStatoConservazione(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (!string.IsNullOrWhiteSpace(p.CHA_STATO_VERSAMENTO))
                context.Query = context.Query.Where(x => context.DbContext.VersamentoEntities.Where(v => v.ID_PROFILE == x.SYSTEM_ID && v.CHA_STATO == p.CHA_STATO_VERSAMENTO).Any());
            else
                context.Query = context.Query.Where(x => !context.DbContext.VersamentoEntities.Where(v => v.ID_PROFILE == x.SYSTEM_ID).Any());
        }

        internal static void AppendFiltroTipoDocumento(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            var predicate = PredicateBuilder.New<ProfileEntity>();

            var almostOneFilter = false;

            if(p.CHA_TIPO_PROTO_A == "1")
            {
                predicate = predicate.Or(x => x.CHA_TIPO_PROTO == "A" && x.CHA_DA_PROTO == "0" && x.ID_DOCUMENTO_PRINCIPALE == null);
                almostOneFilter = true;
            }

            if(p.CHA_TIPO_PROTO_P == "1")
            {
                predicate = predicate.Or(x => x.CHA_TIPO_PROTO == "P" && x.CHA_DA_PROTO == "0" && x.ID_DOCUMENTO_PRINCIPALE == null);
                almostOneFilter = true;
            }

            if (p.CHA_TIPO_PROTO_I == "1")
            {
                predicate = predicate.Or(x => x.CHA_TIPO_PROTO == "I" && x.CHA_DA_PROTO == "0" && x.ID_DOCUMENTO_PRINCIPALE == null);
                almostOneFilter = true;
            }

            if (p.CHA_TIPO_PROTO_G == "1")
            {
                predicate = predicate.Or(x => x.CHA_TIPO_PROTO == "G" && x.CHA_DA_PROTO == "0" && x.ID_DOCUMENTO_PRINCIPALE == null);
                almostOneFilter = true;
            }

            if (almostOneFilter) context.Query = context.Query.Where(predicate);


        }

        internal static void AppendFiltroTipologiaDocumento(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            switch(p.ID_TEMPLATE ?? 0)
            {
                case 0:
                    break;

                case -1:
                    context.Query = context.Query.Where(x => x.ID_TIPO_ATTO == null);
                    break;

                default:
                    context.Query = context.Query.Where(x => x.ID_TIPO_ATTO == p.ID_TEMPLATE);
                    break;
            }
        }

        internal static void AppendFiltroDiagrammiStato(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if(p.CHA_STATO.HasValue)
            {
                switch(p.CHA_STATO)
                {
                    // Diverso da
                    case 0:
                        context.Query = context.Query.Where(x => context.DbContext.DiagrammiEntities.AsNoTracking().Where(d => x.SYSTEM_ID == d.DOC_NUMBER && d.ID_STATO != null && d.ID_STATO != p.ID_STATO).Any());
                        break;

                    // Uguale a
                    case 1:
                        context.Query = context.Query.Where(x => context.DbContext.DiagrammiEntities.AsNoTracking().Where(d => x.SYSTEM_ID == d.DOC_NUMBER && d.ID_STATO != null && d.ID_STATO == p.ID_STATO).Any());
                        break;
                }
            }
        }

        internal static void AppendFiltroRegistro(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (!p.ID_REGISTRO.IsNullOrZero()) context.Query = context.Query.Where(x => x.ID_REGISTRO == p.ID_REGISTRO);
        }

        internal static void AppendFiltroRF(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (!p.ID_RF.IsNullOrZero()) context.Query = context.Query.Where(x => context.DbContext.RuoloRegistroEntities.AsNoTracking().Where(l => l.ID_RUOLO_IN_UO == x.ID_RUOLO_CREATORE && l.ID_REGISTRO == p.ID_RF).Any());
        }

        internal static void AppendFiltroUOCreatore(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if(!p.ID_UO_CREATRICE.IsNullOrZero())
            {
                if (p.CHA_UO_SOTTOPOSTE == "1")
                {
                    var uoList = new List<long?>() { p.ID_UO_CREATRICE };
                    AddChildrenUO(uoList, p.ID_UO_CREATRICE, context.DbContext);

                    context.Query = context.Query.Where(x => uoList.Any(y => y == x.ID_UO_CREATORE));
                }
                else context.Query = context.Query.Where(x => x.ID_UO_CREATORE == p.ID_UO_CREATRICE);
            }
        }

        internal static void AppendFiltroClassificazione(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            switch(p.CHA_TIPO_CLASS)
            {
                case "C":
                    break;
                case "F":
                    context.Query = context.Query.Where(x => context.DbContext.ProjectComponentEntities.AsNoTracking()
                                                                .Join(context.DbContext.ProjectEntities.AsNoTracking(), pc => pc.PROJECT_ID, p => p.SYSTEM_ID, (pc, p) => new { pc, p })
                                                                .Where(y => y.p.ID_FASCICOLO == p.ID_FASCICOLO &&
                                                                            y.pc.LINK == x.SYSTEM_ID)
                                                                .Any());
                    break;
                case "CF:":
                    break;
            }
        }

        internal static void AppendFiltroDocumentiDigitali(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (p.CHA_DOC_DIGITALI == "1") context.Query = context.Query.Where(x => IPi3DbContextMappedFunctions.IsDocCartaceo(x.SYSTEM_ID) != "1");
            
        }

        internal static void AppendFiltroEscludiFatture(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if(p.CHA_ESCLUDI_FATTURE == "1")
                context.Query = context.Query.Where(x => !context.DbContext.TipoAttoEntities.AsNoTracking().Where(t => t.SYSTEM_ID == x.ID_TIPO_ATTO && (
                                                            t.VAR_DESC_ATTO.ToUpper() == "FATTURA ELETTRONICA" ||
                                                            t.VAR_DESC_ATTO.ToUpper() == "LOTTO DI FATTURE" ||
                                                            t.VAR_DESC_ATTO.ToUpper() == "FATTURA ELETTRONICA ATTIVA" ||
                                                            t.VAR_DESC_ATTO.ToUpper() == "LOTTO DI FATTURE ATTIVE")).Any());
                
        }

        internal static void AppendFiltroFormatiDocumenti(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {

        }

        internal static void AppendFiltroDimensioniDocumenti(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {

        }

        internal static void AppendFiltroDocumentiFirmati(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (p.CHA_FIRMATO == "1") context.Query = context.Query.Where(x => IPi3DbContextMappedFunctions.AtLeastOneFirmato(x.SYSTEM_ID) == "1");
            else if (p.CHA_FIRMATO == "0") context.Query = context.Query.Where(x => IPi3DbContextMappedFunctions.AtLeastOneFirmato(x.SYSTEM_ID) == "0");
        }

        internal static void AppendFiltroDocumentiMarcati(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (p.CHA_MARCATO == "1")
            {
                context.Query = context.Query.Where(x => IPi3DbContextMappedFunctions.AtLeastOneMarcato(x.SYSTEM_ID) == "1");

                // Da gestire scadenza marca
                if(!p.CHA_SCADENZA_TIMESTAMP.IsNullOrZero())
                {
                    switch(p.CHA_SCADENZA_TIMESTAMP)
                    {
                        case "E":
                            break;

                        case "W":
                            break;

                        case "M":
                            break;

                        case "Y":
                            break;
                    }
                }
            }
            else if (p.CHA_MARCATO == "0") context.Query = context.Query.Where(x => IPi3DbContextMappedFunctions.AtLeastOneMarcato(x.SYSTEM_ID) == "0");
        }

        internal static void AppendFiltroDataCreazione(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            switch(p.CHA_DATA_CREAZIONE_TIPO)
            {
                // Valore singolo
                case "S":
                    if(p.DATA_CREAZIONE_FROM.HasValue)
                    {
                        var range = p.DATA_CREAZIONE_FROM.Value.GetRangeSingleDay();
                        context.Query = context.Query.Where(x => x.CREATION_TIME >= range.Item1 && x.CREATION_TIME <= range.Item2);
                    }
                    break;

                // Intervallo
                case "R":
                    if (p.DATA_CREAZIONE_FROM.HasValue) context.Query = context.Query.Where(x => x.CREATION_TIME >= p.DATA_CREAZIONE_FROM.Value.AsBeginOfTheDay());
                    if (p.DATA_CREAZIONE_TO.HasValue) context.Query = context.Query.Where(x => x.CREATION_TIME <= p.DATA_CREAZIONE_TO.Value.AsEndOnfTheDay());
                    break;

                // Oggi
                case "T":
                    var rangeOggi = DateTime.Now.Date.GetRangeSingleDay();
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= rangeOggi.Item1 && x.CREATION_TIME <= rangeOggi.Item2);
                    break;

                // Settimana corrente
                case "W":
                    var rangeWeek = DateTime.Now.GetRangeWeek();
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= rangeWeek.Item1 && x.CREATION_TIME <= rangeWeek.Item2);
                    break;

                // Mese corrente
                case "M":
                    var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= date);
                    break;

                // Anno corrente
                case "Y":
                    var anno = DateTime.Now.Year;
                    context.Query = context.Query.Where(x => x.CREATION_TIME!.Value.Year == anno);
                    break;

                // Ieri
                case "B":
                    var rangeIeri = DateTime.Now.AddDays(-1).Date.GetRangeSingleDay();
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= rangeIeri.Item1 && x.CREATION_TIME <= rangeIeri.Item2);
                    break;

                // Settimana precedente
                case "V":
                    var rangePreviousWeek = DateTime.Now.AddDays(-7).GetRangeWeek();
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= rangePreviousWeek.Item1 && x.CREATION_TIME <= rangePreviousWeek.Item2);
                    break;

                // Mese precedente
                case "N":
                    var rangeMese = DateTime.Now.AddMonths(-1).GetDateRangeMese();
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= rangeMese.Item1
                                                           && x.CREATION_TIME <= rangeMese.Item2);
                    break;

                // Anno precedente
                case "X":
                    var annoPrecedente = DateTime.Now.Year - 1;
                    context.Query = context.Query.Where(x => x.CREATION_TIME!.Value.Year == annoPrecedente);
                    break;

                // Numero giorni prima
                case "P":
                    var daysToSubtract = -1 * p.NUM_GIORNI_DATA_CREAZIONE ?? 0;
                    var rangeNumeroGiorniPrima = DateTime.Now.AddDays(daysToSubtract).GetRangeSingleDay();
                    context.Query = context.Query.Where(x => x.CREATION_TIME >= rangeNumeroGiorniPrima.Item1 && x.CREATION_TIME <= rangeNumeroGiorniPrima.Item2);
                    break;
            }
        }

        internal static void AppendFiltroDataProtocollazione(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            switch (p.CHA_DATA_PROTO_TIPO )
            {
                // Valore singolo
                case "S":
                    if (p.DATA_PROTO_FROM.HasValue)
                    {
                        var range = p.DATA_PROTO_FROM.Value.GetRangeSingleDay();
                        context.Query = context.Query.Where(x => x.DTA_PROTO >= range.Item1 && x.DTA_PROTO <= range.Item2);
                    }
                    break;

                // Intervallo
                case "R":
                    if (p.DATA_PROTO_FROM.HasValue) context.Query = context.Query.Where(x => x.DTA_PROTO >= p.DATA_PROTO_FROM.Value.AsBeginOfTheDay());
                    if (p.DATA_PROTO_TO.HasValue) context.Query = context.Query.Where(x => x.DTA_PROTO <= p.DATA_PROTO_TO.Value.AsEndOnfTheDay());
                    break;

                // Oggi
                case "T":
                    var rangeOggi = DateTime.Now.Date.GetRangeSingleDay();
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= rangeOggi.Item1 && x.DTA_PROTO <= rangeOggi.Item2);
                    break;

                // Settimana corrente
                case "W":
                    var rangeWeek = DateTime.Now.GetRangeWeek();
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= rangeWeek.Item1 && x.DTA_PROTO <= rangeWeek.Item2);
                    break;

                // Mese corrente
                case "M":
                    var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= date);
                    break;

                // Anno corrente
                case "Y":
                    var anno = DateTime.Now.Year;
                    context.Query = context.Query.Where(x => x.DTA_PROTO!.Value.Year == anno);
                    break;

                // Ieri
                case "B":
                    var rangeIeri = DateTime.Now.AddDays(-1).Date.GetRangeSingleDay();
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= rangeIeri.Item1 && x.DTA_PROTO <= rangeIeri.Item2);
                    break;

                // Settimana precedente
                case "V":
                    var rangePreviousWeek = DateTime.Now.AddDays(-7).GetRangeWeek();
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= rangePreviousWeek.Item1 && x.DTA_PROTO <= rangePreviousWeek.Item2);
                    break;

                // Mese precedente
                case "N":
                    var rangeMese = DateTime.Now.AddMonths(-1).GetDateRangeMese();
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= rangeMese.Item1 && x.DTA_PROTO <= rangeMese.Item2);
                    break;

                // Anno precedente
                case "X":
                    var annoPrecedente = DateTime.Now.Year - 1;
                    context.Query = context.Query.Where(x => x.DTA_PROTO!.Value.Year == annoPrecedente);
                    break;

                // Numero giorni prima
                case "P":
                    var daysToSubtract = -1 * p.NUM_GIORNI_DATA_PROTO ?? 0;
                    var rangeNumeroGiorniPrima = DateTime.Now.AddDays(daysToSubtract).GetRangeSingleDay();
                    context.Query = context.Query.Where(x => x.DTA_PROTO >= rangeNumeroGiorniPrima.Item1 && x.DTA_PROTO <= rangeNumeroGiorniPrima.Item2);
                    break;
            }
        }

        internal static void AppendFiltroDataFirma(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {

        }

        internal static void AppendFiltroTipoStampa(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            switch(p.CHA_TIPO_REGISTRO_STAMPA)
            {
                case "R":
                    context.Query = context.Query.Where(x => x.CHA_TIPO_PROTO == "R");
                    break;
                case "C":
                    context.Query = context.Query.Where(x => x.CHA_TIPO_PROTO == "C");
                    break;
            }
        }

        internal static void AppendFiltroRepertorio(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if(p.CHA_TIPO_REGISTRO_STAMPA == "C" && !p.ID_REPERTORIO_STAMPA.IsNullOrZero())
                context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.ID_REPERTORIO == p.ID_REPERTORIO_STAMPA).Any());
        }

        internal static void AppendFiltroRegistroStampa(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if (!p.ID_REGISTRO.IsNullOrZero()) context.Query = context.Query.Where(x => !x.ID_REGISTRO.HasValue || x.ID_REGISTRO == p.ID_REGISTRO);
        }

        internal static void AppendFiltroAnnoStampa(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            if(p.NUM_ANNO_STAMPA.HasValue)
            {
                switch(p.CHA_TIPO_POLICY)
                {
                    case "R":
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.NUM_ANNO == p.NUM_ANNO_STAMPA).Any());
                        break;
                    case "C":
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.NUM_ANNO == p.NUM_ANNO_STAMPA).Any());
                        break;
                }
            }
        }

        internal static void AppendFiltroDataStampa(this PolicyParerEntity p, ProfileSearchAppendContext context)
        {
            switch (p.CHA_DATA_STAMPA_TIPO )
            {
                // Valore singolo
                case "S":
                    if (p.DATA_STAMPA_FROM.HasValue)
                    {
                        var range = p.DATA_STAMPA_FROM.Value.GetRangeSingleDay();
                        if (p.CHA_TIPO_REGISTRO_STAMPA == "R") 
                            context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= range.Item1 && r.DTA_STAMPA <= range.Item2).Any());
                        else
                            context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= range.Item1 && r.DTA_STAMPA <= range.Item2).Any());
                    }
                    break;

                // Intervallo
                case "R":
                    if (p.DATA_STAMPA_FROM.HasValue)
                    {
                        if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                            context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= p.DATA_STAMPA_FROM.Value.AsBeginOfTheDay()).Any());
                        else
                            context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= p.DATA_STAMPA_FROM.Value.AsBeginOfTheDay()).Any());
                    }
                    if(p.DATA_STAMPA_TO.HasValue)
                    {
                        if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                            context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= p.DATA_STAMPA_TO.Value.AsEndOnfTheDay()).Any());
                        else
                            context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= p.DATA_STAMPA_TO.Value.AsEndOnfTheDay()).Any());
                    }
                    break;

                // Oggi
                case "T":
                    var rangeOggi = DateTime.Now.Date.GetRangeSingleDay();
                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangeOggi.Item1 && r.DTA_STAMPA <= rangeOggi.Item2).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangeOggi.Item1 && r.DTA_STAMPA <= rangeOggi.Item2).Any());
                    break;

                // Settimana corrente
                case "W":
                    var rangeWeek = DateTime.Now.GetRangeWeek();
                    if(p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangeWeek.Item1 && r.DTA_STAMPA <= rangeWeek.Item2).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangeWeek.Item1 && r.DTA_STAMPA <= rangeWeek.Item2).Any());
                    break;

                // Mese corrente
                case "M":
                    var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= date).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= date).Any());
                    break;

                // Anno corrente
                case "Y":
                    var anno = DateTime.Now.Year;
                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA!.Value.Year == anno).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA!.Value.Year == anno).Any());
                    break;

                // Ieri
                case "B":
                    var rangeIeri = DateTime.Now.AddDays(-1).Date.GetRangeSingleDay();
                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangeIeri.Item1 && r.DTA_STAMPA <= rangeIeri.Item2).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangeIeri.Item1 && r.DTA_STAMPA <= rangeIeri.Item2).Any());
                    break;

                // Settimana precedente
                case "V":
                    var rangePreviousWeek = DateTime.Now.AddDays(-7).GetRangeWeek();
                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangePreviousWeek.Item1 && r.DTA_STAMPA <= rangePreviousWeek.Item2).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA >= rangePreviousWeek.Item1 && r.DTA_STAMPA <= rangePreviousWeek.Item2).Any());
                    break;

                // Mese precedente
                case "N":
                    var rangeMese = DateTime.Now.AddMonths(-1).GetDateRangeMese();

                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID
                                                                && r.DTA_STAMPA >= rangeMese.Item1
                                                                && r.DTA_STAMPA <= rangeMese.Item2
                                                        ).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID
                                                                && r.DTA_STAMPA >= rangeMese.Item1
                                                                && r.DTA_STAMPA <= rangeMese.Item2
                                                        ).Any());
                    break;

                // Anno precedente
                case "X":
                    var annoPrecedente = DateTime.Now.Year - 1;
                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA!.Value.Year == annoPrecedente).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID && r.DTA_STAMPA!.Value.Year == annoPrecedente).Any());
                    break;

                // Numero giorni prima
                case "P":
                    var daysToSubtract = -1 * p.NUM_GIORNI_DATA_PROTO ?? 0;
                    var rangeNumeroGiorniPrima = DateTime.Now.AddDays(daysToSubtract).GetRangeSingleDay();

                    if (p.CHA_TIPO_REGISTRO_STAMPA == "R")
                        context.Query = context.Query.Where(x => context.DbContext.StampaRegistriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID
                                                                    && r.DTA_STAMPA >= rangeNumeroGiorniPrima.Item1
                                                                    && r.DTA_STAMPA <= rangeNumeroGiorniPrima.Item2).Any());
                    else
                        context.Query = context.Query.Where(x => context.DbContext.StampaRepertoriEntities.Where(r => r.DOCNUMBER == x.SYSTEM_ID
                                                                    && r.DTA_STAMPA >= rangeNumeroGiorniPrima.Item1
                                                                    && r.DTA_STAMPA <= rangeNumeroGiorniPrima.Item2).Any());
                    break;
            }
        }

        internal static TextSectionModel AsTitleSection(this string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 16,
                        FontIsBold = true
                    },
                    Value = text
                }
            };
        }

        internal static TextSectionModel AsSubTitleSection(this string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 12,
                        FontIsBold = false
                    },
                    Value = text
                }
            };
        }

        internal static TextSectionModel AsSummarySection(this string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 10,
                        FontIsBold = false
                    },
                    Value = text
                }
            };
        }

        internal static TextSectionModel AsTextSection(this string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 8,
                        FontIsBold = false
                    },
                    Value = text
                }
            };
        }


        internal static void AddHeaderRow(this GridSectionModel grid, ReportType reportType)
        {
            var header = new GridRowModel();

            var style = new TextStyleModel
            {
                FontName = "ARIAL",
                FontSize = 8,
                FontIsBold = true
            };

            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 20), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderId } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderType } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 20), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderDate } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 30), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderSubject } });

            switch (reportType)
            {
                case ReportType.ReportPolicy:
                    
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 20), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderRegister } });
                    break;
                case ReportType.ReportVersamentiRifiutati:
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderPolicyCode  } });
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderPolicyExecutionNumber  } });
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 30), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderErrorMessage  } });
                    break;
                case ReportType.ReportVersamentiFalliti:
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderPolicyCode } });
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderPolicyExecutionNumber } });
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 20), Content = new TextContentModel { Style = style, Value = Resources.ReportHeaderLastSendingDate  } });
                    break;
            }

            grid.AddRow(header);
            
        }

        internal static GridRowModel AsReportRow(this IReportItem r)
        {
            var row = new GridRowModel();

            var textStyle = new TextStyleModel
            {
                FontName = "ARIAL",
                FontSize = 7
            };

            var cellStyle = new GridCellStyleModel
            {
                Justification = Justifications.Center,
                VerticalAlignment = VerticalAlignments.Top
            };

            foreach(var p in r.GetType().GetProperties())
            {
                if (p.PropertyType == typeof(DateTime))
                {
                    var d = p.GetValue(r) as DateTime?;
                    row.AddCell(new GridCellModel { Style = cellStyle, Content = new TextContentModel { Style = textStyle, Value = d.Value.ToString("dd/MM/yyyy HH:mm:ss") } });
                }
                else row.AddCell(new GridCellModel { Style = cellStyle, Content = new TextContentModel { Style = textStyle, Value = p.GetValue(r)?.ToString() ?? string.Empty } });
            }

            return row;
        }

        private static GridCellStyleModel HeaderCellStyle(int percentage)
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                WithPercentage = percentage,
                ForegroundColor = System.Drawing.Color.Silver
            };

        private static DateTime AsBeginOfTheDay(this DateTime d)
            => new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);

        private static DateTime AsEndOnfTheDay(this DateTime d)
            => new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);

        private static (DateTime, DateTime) GetRangeSingleDay(this DateTime d)
            => (d.AsBeginOfTheDay(), d.AsEndOnfTheDay());

        private static (DateTime, DateTime) GetRangeWeek(this DateTime d)
        {
            var startDate = d.AsBeginOfTheDay();
            var endDate = d.AsEndOnfTheDay();
            
            while (startDate.DayOfWeek != DayOfWeek.Monday) startDate = startDate.AddDays(-1);
            while (endDate.DayOfWeek != DayOfWeek.Sunday) endDate = endDate.AddDays(1);

            return (startDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeMese(this DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            var endDate = initDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }


        internal static string AsTipoProto(this string? s)
        {
            switch(s)
            {
                case "A":
                    return Resources.TypeDocA;
                case "P":
                    return Resources.TypeDocP;
                case "I":
                    return Resources.TypeDocI;
                case "G":
                    return Resources.TypeDocG;
                case "R":
                    return Resources.TypeDocR;
                case "C":
                    return Resources.TypeDocC;
                default:
                    return string.Empty;
            }
        }

        private static bool IsNullOrZero(this long? l)
        {
            return (l ?? 0) == 0;
        }

        private static bool IsNullOrZero(this string? s)
        {
            return (s ?? "0") == "0";
        }

        internal async static Task<int> SaveChangesAsync(this IPi3DbContext dbContext)
        {
            return await ((DbContext)dbContext).SaveChangesAsync();
        }

        internal async static Task<int> AddVersamentiPolicyEntityAsync(this IPi3DbContext dbContext, VersamentiPolicyEntity entity)
        {
            return await ((DbContext)dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO DPA_VERSAMENTI_POLICY VALUES ({entity.ID_POLICY}, {entity.ID_PROFILE}, SYSDATE, {entity.NUM_ESECUZIONE_POLICY})");
        }

        private static void AddChildrenUO(List<long?> list, long? idParent, IPi3DbContext context)
        {
            var items = context.CorrGlobaliEntities.Where(x => x.ID_PARENT == idParent && x.CHA_TIPO_URP == "U").Select(x => x.SYSTEM_ID).ToList();
            foreach (var item in items)
            {
                list.Add(item);
                AddChildrenUO(list, item, context);
            }
        }
    }
}
