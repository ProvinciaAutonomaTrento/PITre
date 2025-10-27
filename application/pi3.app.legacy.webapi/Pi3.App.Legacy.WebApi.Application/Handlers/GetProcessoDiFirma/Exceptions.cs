// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetProcessoDiFirma
{
    public class ProcessoFirmaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ProcessoFirmaNotFoundPi3Exception(long idProcesso)
            : base(ErrorDescriptions.ProcessoDiFirmaNonTrovato, null, ErrorDescriptions.ResourceManager, idProcesso)
        {
            this.IdProcesso = idProcesso;
        }

        public long IdProcesso { get; init; }

        #endregion
    }
}
