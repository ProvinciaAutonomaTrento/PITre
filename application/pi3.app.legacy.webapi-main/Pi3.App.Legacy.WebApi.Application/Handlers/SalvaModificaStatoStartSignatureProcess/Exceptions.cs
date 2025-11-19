// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SalvaModificaStatoStartSignatureProcess
{
    public class IdStatoDiagrammaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdStatoDiagrammaNotFoundPi3Exception(long idStato)
            : base(ErrorDescriptions.IdStatoDiagrammaNotFound, null, ErrorDescriptions.ResourceManager, idStato)
        {
            this.IdStato = idStato;
        }

        public long IdStato { get; init; }

        #endregion
    }

    public class ErroreAvvioProcessoFirmaPi3Exception : Pi3Exception
    {
        #region Public Members

        public ErroreAvvioProcessoFirmaPi3Exception()
            : base(ErrorDescriptions.ErroreAvvioProcessoFirma, null, ErrorDescriptions.ResourceManager)
        { }

        #endregion
    }
}
