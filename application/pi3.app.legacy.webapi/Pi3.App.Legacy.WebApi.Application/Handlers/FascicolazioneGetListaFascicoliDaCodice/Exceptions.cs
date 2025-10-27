// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetListaFascicoliDaCodice
{
    internal class Exceptions
    {
    }

    public class CorrGlobaliByGroupIdNotFoundPi3Exception : NotFoundPi3Exception
    {
        public CorrGlobaliByGroupIdNotFoundPi3Exception(long idGruppo)
            : base(ErrorDescriptions.CorrGlobaliByGroupIdNotFound, null, ErrorDescriptions.ResourceManager, idGruppo)
        {
            this.IdGruppo = idGruppo;
        }

        public long IdGruppo { get; init; }

    }
}
