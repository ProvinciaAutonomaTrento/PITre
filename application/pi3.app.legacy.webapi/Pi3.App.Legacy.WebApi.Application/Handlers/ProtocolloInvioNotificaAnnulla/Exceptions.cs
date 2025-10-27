// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ProtocolloInvioNotificaAnnulla
{
    public class CaricamentoInfoMittenteSpedizionePi3Exception : Pi3Exception
    {
        #region Public Members

        public CaricamentoInfoMittenteSpedizionePi3Exception()
            : base(ErrorDescriptions.CaricamentoInfoMittenteSpedizione, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class AOOCollegataNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public AOOCollegataNotFoundPi3Exception(string descrizione)
            : base(ErrorDescriptions.AOOCollegataNotFound, null, ErrorDescriptions.ResourceManager, descrizione)
        { }
        #endregion
    }

    public class SendMailPi3Exception : Pi3Exception
    {
        public SendMailPi3Exception(string message)
            : base(message)
        {
        }
    }
    public class ProviderNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ProviderNotFoundPi3Exception(string message)
            : base(message)
        {
        }
    }
}
