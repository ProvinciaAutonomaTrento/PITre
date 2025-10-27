// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GeneratePrintRepertorio
{
    public class RegistryOpenPi3Exception : Pi3Exception
    {
        public RegistryOpenPi3Exception()
            : base(ErrorDescriptions.OpenRegistryException, Resources.ResourceManager, null, string.Empty)
        {
                
        }
    }

    public class DocumentsNotFoundPi3Exception : NotFoundPi3Exception
    {
        public DocumentsNotFoundPi3Exception()
            :base(ErrorDescriptions.DocumentsNotFoundException, null, Resources.ResourceManager, string.Empty)
        {
                
        }
    }

}
