// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportRDEDocument
{
    public class UtenteNonAbilitatoAllaCreazioneDelProtocolloPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteNonAbilitatoAllaCreazioneDelProtocolloPi3Exception(string tipoProto)
            : base(ErrorDescriptions.UtenteNonAbilitatoAllaCreazioneDelProtocollo, null, ErrorDescriptions.ResourceManager, tipoProto)
        { }
        #endregion
    }

    public class InvalidParametersPi3Exception : Pi3Exception
    {
        #region Public Members

        public InvalidParametersPi3Exception()
            : base(ErrorDescriptions.InvalidParameters, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class SpecificaCorrispondenteNonValidaPi3Exception : Pi3Exception
    {
        #region Public Members

        public SpecificaCorrispondenteNonValidaPi3Exception(string code)
            : base(ErrorDescriptions.SpecificaCorrispondenteNonValida, null, ErrorDescriptions.ResourceManager, code)
        { }
        #endregion
    }

    public class RegistroNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RegistroNotFoundPi3Exception(string code)
            : base(ErrorDescriptions.RegistroNotFound, null, ErrorDescriptions.ResourceManager, code)
        { }
        #endregion
    }

    public class FascicoloNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public FascicoloNotFoundPi3Exception()
            : base(ErrorDescriptions.FascicoloNotFound, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }
}
