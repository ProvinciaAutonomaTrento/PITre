// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemotePdfSignStamp
{
    public class ApposizioneSigilloFileVuotoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ApposizioneSigilloFileVuotoPi3Exception(string etichetta)
            : base(ErrorDescriptions.ApposizioneSigilloFileVuoto, ErrorDescriptions.ResourceManager, etichetta)
        {
            this.Etichetta = etichetta;
        }

        public string Etichetta { get; init; }
        #endregion
    }
}
