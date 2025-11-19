// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Report;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record ExportFieldCasellaIstituzionale(string contextName) : IRequest<PrintReportResponse>;
    public record ExportFieldModelliTrasmissioneUtente(string contextName) : IRequest<PrintReportResponse>;
    public record ExportFieldFindAndReplace(string contextName) : IRequest<PrintReportResponse>;
    public record ExportFieldReportSpedizioni(string contextName) : IRequest<PrintReportResponse>;
    
}
