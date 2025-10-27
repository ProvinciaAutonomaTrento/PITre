// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_getIdProfileByData
{
    internal class Exceptions
    {
    }

    public class ProfileEntityNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ProfileEntityNotFoundPi3Exception(long numProto, long? annoProto, long? idRegistro)
            : base(ErrorDescriptions.ProfileEntityNotFound, null, ErrorDescriptions.ResourceManager, numProto, annoProto, idRegistro)
        {
            this.NumProto = numProto;
            this.AnnoProto = annoProto;
            this.IdRegistro = idRegistro;
        }

        public long NumProto { get; init; }
        public long? AnnoProto { get; init; }
        public long? IdRegistro { get; init; }

    }
}
