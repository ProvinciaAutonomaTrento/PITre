// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public interface IReportGeneratorService : IService
    {
        Task<ReportGeneratorCapabilities> GetCapabilities();

        //Task Generate(ReportModel reportModel, Stream outputStream);

        Task<GeneratedReport> Generate(ReportModel reportModel, Stream outputStream);
    }
}