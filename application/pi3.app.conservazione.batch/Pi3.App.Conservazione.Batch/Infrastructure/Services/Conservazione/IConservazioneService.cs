// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.Conservazione
{
    internal interface IConservazioneService
    {
        Task DoWork(string operationType);

        //Task GenerateDailyErrorReport();

        //Task GeneratePolicyExecutionReport();
    }
}
