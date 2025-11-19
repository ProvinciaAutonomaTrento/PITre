// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertNotaInElencoDaExcel
{
    public class NoContentException : NotFoundPi3Exception
    {
        public NoContentException()
            : base(ErrorDescriptions.ContenutoNonTrovato, null, ErrorDescriptions.ResourceManager)
        {

        }
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

    public class InsertNotePi3Exception : Pi3Exception
    {
        public InsertNotePi3Exception()
            : base(ErrorDescriptions.GenericErrorInInsert, null, ErrorDescriptions.ResourceManager)
        {

        }
    }


}
