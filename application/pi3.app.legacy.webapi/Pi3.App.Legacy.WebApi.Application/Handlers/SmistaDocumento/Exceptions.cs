// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Smistamento;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SmistaDocumento
{
    public class SmistaDocumentoPi3Exception : Pi3Exception
    {
        #region Public Members

        public SmistaDocumentoPi3Exception(EsitoSmistamentoDocumento esitoSmistamentoDocumento)
            : base(ErrorDescriptions.ErroreSmistamento, ErrorDescriptions.ResourceManager)
        {
            this.EsitoSmistamentoDocumento = esitoSmistamentoDocumento;
        }

        public EsitoSmistamentoDocumento EsitoSmistamentoDocumento { get; init; }

        #endregion
    }

}
