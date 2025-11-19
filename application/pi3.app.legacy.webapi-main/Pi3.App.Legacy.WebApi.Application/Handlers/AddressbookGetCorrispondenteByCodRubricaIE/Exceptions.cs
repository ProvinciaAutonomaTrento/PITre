// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteByCodRubricaIE
{
    public class CorrispondenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CorrispondenteNotFoundPi3Exception(string codiceCorrispondente)
            : base(ErrorDescriptions.CorrespondentNotFond , null, ErrorDescriptions.ResourceManager, codiceCorrispondente)
        {
            this.CodiceCorrispondente  = codiceCorrispondente;
        }

        public string CodiceCorrispondente { get; init; }

        #endregion
    }
}
