// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RegistriStampa
{
    public class DocumentsNotFoundPi3Exception : NotFoundPi3Exception
    {
        public DocumentsNotFoundPi3Exception()
            :base(ErrorDescriptions.NoItemsFound, null, ErrorDescriptions.ResourceManager)
        {
            
        }
    }
}
