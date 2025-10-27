// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SpedisciDocumento
{
    public class DocumentoNonSalvatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoNonSalvatoPi3Exception()
            : base(ErrorDescriptions.DocumentoNonSalvato, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class DocumentoNonProtocollatoInUscitaPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoNonProtocollatoInUscitaPi3Exception(string id)
            : base(ErrorDescriptions.DocumentoNonProtocollatoInUscita, ErrorDescriptions.ResourceManager, id)
        {
        }

        #endregion
    }

    public class DocumentoInLibroFirmaPassoNonAttesoPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoInLibroFirmaPassoNonAttesoPi3Exception(string id)
            : base(ErrorDescriptions.DocumentoInLibroFirmaPassoNonAtteso, ErrorDescriptions.ResourceManager, id)
        {
        }

        #endregion
    }

    public class SigilloFileSegnaturaXMLNonApplicatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public SigilloFileSegnaturaXMLNonApplicatoPi3Exception()
            : base(ErrorDescriptions.SigilloFileSegnaturaXMLNonApplicato, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
    public class DestinatarioTrasmissioneOccasionalePi3Exception : Pi3Exception
    {
        #region Public Members

        public DestinatarioTrasmissioneOccasionalePi3Exception(string id, string descrizione)
            : base(ErrorDescriptions.DestinatarioTrasmissioneOccasionale, ErrorDescriptions.ResourceManager, id, descrizione)
        {
        }

        #endregion
    }

    public class DestinatarioTrasmissioneDisabilitataPi3Exception : Pi3Exception
    {
        #region Public Members

        public DestinatarioTrasmissioneDisabilitataPi3Exception(string id, string descrizione)
            : base(ErrorDescriptions.DestinatarioTrasmissioneDisabilitata, ErrorDescriptions.ResourceManager, id, descrizione)
        {
        }

        #endregion
    }

    public class RuoloRiferimentoNotFoundPi3Exception : Pi3Exception
    {
        #region Public Members

        public RuoloRiferimentoNotFoundPi3Exception(string id, string descrizione)
            : base(ErrorDescriptions.RuoloRiferimentoNotFound, ErrorDescriptions.ResourceManager, id, descrizione)
        {
        }

        #endregion
    }

    public class UtentiRuoloNotFoundPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtentiRuoloNotFoundPi3Exception(string id, string descrizione)
            : base(ErrorDescriptions.UtentiRuoloNotFound, ErrorDescriptions.ResourceManager, id, descrizione)
        {
        }

        #endregion
    }

    public class RegistroRFMittenteNotFoundPi3Exception : Pi3Exception
    {
        #region Public Members

        public RegistroRFMittenteNotFoundPi3Exception()
            : base(ErrorDescriptions.RegistroRFMittenteNotFound, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class DestinatarioNonInteroperantePi3Exception : Pi3Exception
    {
        #region Public Members

        public DestinatarioNonInteroperantePi3Exception()
            : base(ErrorDescriptions.DestinatarioNonInteroperante, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class RegistroNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RegistroNotFoundPi3Exception()
            : base(ErrorDescriptions.RegistroNotFound, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class IdentificativoSuapNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdentificativoSuapNotFoundPi3Exception()
            : base(ErrorDescriptions.IdentificativoSuapNotFound, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class CanalePreferenzialeNonSupportatoPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CanalePreferenzialeNonSupportatoPi3Exception()
            : base(ErrorDescriptions.CanalePreferenzialeNonSupportato, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class SenderNotInteroperablePi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public SenderNotInteroperablePi3Exception()
            : base(ErrorDescriptions.SenderNotInteroperable, ErrorDescriptions.ResourceManager)
        {
        }

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
