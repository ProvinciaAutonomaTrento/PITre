// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ProtocolloInvioRicevutaDiRitorno
{
    public class EmailUtenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public EmailUtenteNotFoundPi3Exception(string idCorrispondente)
            : base(ErrorDescriptions.EmailCorrispondenteNonAssociata, null, ErrorDescriptions.ResourceManager, idCorrispondente)
        {
            this.IdCorrispondente = idCorrispondente;
        }

        public string IdCorrispondente { get; init; }

        #endregion
    }

    public class RegistroMittenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RegistroMittenteNotFoundPi3Exception(string idRegistro)
            : base(ErrorDescriptions.RegistroMittenteNotFound, null, ErrorDescriptions.ResourceManager, idRegistro)
        {
            this.IdRegistro = idRegistro;
        }

        public string IdRegistro { get; init; }

        #endregion
    }

    public class AooAssociataNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public AooAssociataNotFoundPi3Exception(string descRegistro)
            : base(ErrorDescriptions.AooAssociataNotFound, null, ErrorDescriptions.ResourceManager, descRegistro)
        {
            this.DescRegistro = descRegistro;
        }

        public string DescRegistro { get; init; }

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
