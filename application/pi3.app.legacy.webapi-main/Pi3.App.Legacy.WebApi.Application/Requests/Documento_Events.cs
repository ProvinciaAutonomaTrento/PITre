// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.filtri;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Logger.CodAzione;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record DocumentoGetListaLogResult(DocsPaVO.documento.LogDocumento[] output);

    public record DocumentoGetListaLog(string idOggetto, string varOggetto) : IRequest<DocumentoGetListaLogResult>;
    public record DocumentoGetListaLogFilterResult(DocsPaVO.documento.LogDocumento[] output);

    public record DocumentoGetListaLogFilter(string idOggetto, string idFolder, string varOggetto, FilterVisibility[] filter) : IRequest<DocumentoGetListaLogFilterResult>;
    public record toDayResult(string output);

    public record toDay() : IRequest<toDayResult>;
    public record getFirstDayOfWeekResult(string output);

    public record getFirstDayOfWeek() : IRequest<getFirstDayOfWeekResult>;
    public record getLastDayOfWeekResult(string output);

    public record getLastDayOfWeek() : IRequest<getLastDayOfWeekResult>;
    public record getFirstDayOfMonthResult(string output);

    public record getFirstDayOfMonth() : IRequest<getFirstDayOfMonthResult>;
    public record getLastDayOfMonthResult(string output);

    public record getLastDayOfMonth() : IRequest<getLastDayOfMonthResult>;
    public record GetLogAttiviByOggettoResult(infoOggetto[] output);

    public record GetLogAttiviByOggetto(string oggetto, string codAmm) : IRequest<GetLogAttiviByOggettoResult>;

}
