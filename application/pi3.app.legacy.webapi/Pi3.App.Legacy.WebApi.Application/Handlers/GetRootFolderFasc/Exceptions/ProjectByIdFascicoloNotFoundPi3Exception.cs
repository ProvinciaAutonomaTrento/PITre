// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRootFolderFasc.Exceptions
{
    public class ProjectByIdFascicoloNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ProjectByIdFascicoloNotFoundPi3Exception(long idFascicolo)
            : base(ErrorDescriptions.ProjectNotFound, null, ErrorDescriptions.ResourceManager, idFascicolo) => this.IdFascicolo = idFascicolo;

        public long IdFascicolo { get; init; }
    }
}
