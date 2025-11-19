// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FattElAttiveDaImport
{
    public class CodiceFornitoreNotFoundPi3Exception : NotFoundPi3Exception
    {
        public CodiceFornitoreNotFoundPi3Exception(string codiceFornitore)
            : base(ErrorDescriptions.CodiceFornitoreNotFound, null, ErrorDescriptions.ResourceManager, codiceFornitore)
        {
        }
    }

    public class FatturaNonFirmataPi3Exception : Pi3Exception
    {
        public FatturaNonFirmataPi3Exception()
            : base(ErrorDescriptions.FatturaNonFirmata, null, ErrorDescriptions.ResourceManager)
        {
        }
    }

    public class DirittiTipologiaPi3Exception : Pi3Exception
    {
        public DirittiTipologiaPi3Exception()
            : base(ErrorDescriptions.DirittiTipologia, null, ErrorDescriptions.ResourceManager)
        {
        }
    }

    public class TemplateFatturaNotFoundPi3Exception : NotFoundPi3Exception
    {
        public TemplateFatturaNotFoundPi3Exception()
            : base(ErrorDescriptions.TemplateFatturaNotFound, null, ErrorDescriptions.ResourceManager)
        {
        }
    }

}
