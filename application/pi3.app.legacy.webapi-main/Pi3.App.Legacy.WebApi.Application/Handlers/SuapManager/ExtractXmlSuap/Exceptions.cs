// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SuapManager.ExtractXmlSuap
{
    public class ValidazioneXmlPi3Exception : Pi3Exception
    {
        #region Public Members

        public ValidazioneXmlPi3Exception()
            : base(ErrorDescriptions.ValidazioneXml, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class TipologiaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TipologiaNotFoundPi3Exception(string tipologia)
            : base(ErrorDescriptions.TipologiaNotFound, null, ErrorDescriptions.ResourceManager, tipologia)
        { }
        #endregion
    }
}
