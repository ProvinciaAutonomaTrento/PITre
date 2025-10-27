// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Login
{
    public class UtenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public UtenteNotFoundPi3Exception()
            : base(ErrorDescriptions.UtenteNotFound, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class AmministrazioneUtenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public AmministrazioneUtenteNotFoundPi3Exception()
            : base(ErrorDescriptions.AmministrazioneUtenteNotFound, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class UtenteDisabilitatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteDisabilitatoPi3Exception()
            : base(ErrorDescriptions.UtenteDisabilitato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class SessioUtenteEsistentePi3Exception : Pi3Exception
    {
        #region Public Members

        public SessioUtenteEsistentePi3Exception()
            : base(ErrorDescriptions.SessioneUtenteEsistente, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class UtenteMultiAmministrazionePi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteMultiAmministrazionePi3Exception()
            : base(ErrorDescriptions.UtenteMultiAmministrazione, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class UtenteNoRuoliPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteNoRuoliPi3Exception()
            : base(ErrorDescriptions.UtenteNoRuoli, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class PasswordScadutaPi3Exception : Pi3Exception
    {
        #region Public Members

        public PasswordScadutaPi3Exception()
            : base(ErrorDescriptions.PasswordScaduta, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }
}
