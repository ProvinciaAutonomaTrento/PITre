// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModNotaInElenco
{
    public class NoteNotFoundPi3Exception : NotFoundPi3Exception
    {
        public NoteNotFoundPi3Exception(string idNota)
            : base(ErrorDescriptions.NotaNonTrovata, null, ErrorDescriptions.ResourceManager, idNota)
        {
            this.IdNota = idNota;
        }

        public string IdNota { get; init; }


    }
}
