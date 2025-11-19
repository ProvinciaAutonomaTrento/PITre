// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.IsCodRubricaPresente
{
    public class CodiceRubricaEsistenteException : Pi3Exception
    {
        public CodiceRubricaEsistenteException()
            : base(ErrorDescriptions.CodiceRubricaEsistente, null, ErrorDescriptions.ResourceManager)
        {

        }
    }

    public class CodiceRubricaComuneEsistenteException : Pi3Exception
    {
        public CodiceRubricaComuneEsistenteException()
            : base(ErrorDescriptions.CodiceRubricaComuneEsistente, null, ErrorDescriptions.ResourceManager)
        {

        }
    }

    public class CodiceListaEsistenteException : Pi3Exception
    {
        public CodiceListaEsistenteException()
            : base(ErrorDescriptions.CodiceListaEsistente, null, ErrorDescriptions.ResourceManager)
        {

        }
    }
}
