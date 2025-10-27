// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTemplateFascCampiComuniById
{
    public class TemplateFascNotFoundException : NotFoundPi3Exception
    {
        public TemplateFascNotFoundException(string idTemplate)
            : base(ErrorDescriptions.TemplateNotFound, null, ErrorDescriptions.ResourceManager, idTemplate)
        {
            this.IdTemplate = idTemplate;
        }

        public string IdTemplate { get; init; }

    }
}
