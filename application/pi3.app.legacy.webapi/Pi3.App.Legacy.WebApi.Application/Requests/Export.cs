// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Report;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record ExportReportSpedizioni(PrintReportRequest request) : IRequest<PrintReportResponse>;
    public record ExportModelliTrasmissioneUtente(PrintReportRequest request) : IRequest<PrintReportResponse>;
    public record ExportFindAndReplace(PrintReportRequest request) : IRequest<PrintReportResponse>;
    public record ExportRegistroAccessiPublish(PrintReportRequest request) : IRequest<PrintReportResponse>;
    public record ExportRegistroAccessiExport(PrintReportRequest request) : IRequest<PrintReportResponse>;
    public record ExportRicercaCasellaIstituzionale(PrintReportRequest request) : IRequest<PrintReportResponse>;
}
