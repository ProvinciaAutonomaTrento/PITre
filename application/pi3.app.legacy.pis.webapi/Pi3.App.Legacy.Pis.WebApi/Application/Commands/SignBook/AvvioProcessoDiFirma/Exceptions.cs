// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma
{
    public class ProcessoFirmaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ProcessoFirmaNotFoundPi3Exception(long idProcesso)
            : base(ErrorDescriptions.ProcessoFirmaNotFound, null, ErrorDescriptions.ResourceManager, idProcesso)
        {
            this.IdProcesso = idProcesso;
        }

        public long IdProcesso { get; init; }
        #endregion
    }

    public class PassoFacoltativoUnicoPassoPi3Exception : Pi3Exception
    {
        #region Public Members

        public PassoFacoltativoUnicoPassoPi3Exception()
            : base(ErrorDescriptions.PassoFacoltativoUnicoPasso, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class PassoFacoltativoIncludiNonSpecificatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public PassoFacoltativoIncludiNonSpecificatoPi3Exception()
            : base(ErrorDescriptions.PassoFacoltativoIncludiNonSpecificato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaFileNonAcquisitoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaFileNonAcquisitoPi3Exception(long idProfile)
            : base(ErrorDescriptions.ProcessoFirmaFileNonAcquisito, null, ErrorDescriptions.ResourceManager, idProfile)
        { }
        #endregion
    }

    public class ProcessoFirmaDocumentoConsolidatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaDocumentoConsolidatoPi3Exception(long idProfile)
            : base(ErrorDescriptions.ProcessoFirmaDocumentoConsolidato, null, ErrorDescriptions.ResourceManager, idProfile)
        { }
        #endregion
    }

    public class ProcessoFirmaDocumentoDocumentoBloccatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaDocumentoDocumentoBloccatoPi3Exception(long idProfile)
            : base(ErrorDescriptions.ProcessoFirmaDocumentoDocumentoBloccato, null, ErrorDescriptions.ResourceManager, idProfile)
        { }
        #endregion
    }

    public class ProcessoFirmaFormatoFileNonAmmessoAllaFirmaPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaFormatoFileNonAmmessoAllaFirmaPi3Exception(long idProfile)
            : base(ErrorDescriptions.ProcessoFirmaFormatoFileNonAmmessoAllaFirma, null, ErrorDescriptions.ResourceManager, idProfile)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoFirmaPadesSuFirmaCadesPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoFirmaPadesSuFirmaCadesPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoFirmaPadesSuFirmaCades, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoFirmaPadesSuFileNonPDFPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoFirmaPadesSuFileNonPDFPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoFirmaPadesSuFileNonPDF, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }
    public class ProcessoFirmaPassoProtocollazioneSuProtocollatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoProtocollazioneSuProtocollatoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoProtocollazioneSuProtocollato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoProtoDocNonPredispostoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoProtoDocNonPredispostoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoProtoDocNonPredisposto, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoProtoRegistroErratoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoProtoRegistroErratoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoProtoRegistroErrato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoProtoRegistroChiusoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoProtoRegistroChiusoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoProtoRegistroChiuso, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoSpedizioneProtoNoArrivoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoSpedizioneProtoNoArrivoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoSpedizioneProtoNoArrivo, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoRepertoriazioneDocNonTipizzatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoRepertoriazioneDocNonTipizzatoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoRepertoriazioneDocNonTipizzato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoSpedizioneRegistroErratoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoSpedizioneRegistroErratoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoSpedizioneRegistroErrato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoSpedizioneDocNonProtocollatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoSpedizioneDocNonProtocollatoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoSpedizioneDocNonProtocollato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoRepertoriazioneNoContatorePi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoRepertoriazioneNoContatorePi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoRepertoriazioneNoContatore, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoRepertoriazioneDocRepertoriatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoRepertoriazioneDocRepertoriatoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoRepertoriazioneDocRepertoriato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoRepertoriazioneRFNonSpecificatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoRepertoriazioneRFNonSpecificatoPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoRepertoriazioneRFNonSpecificato, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaPassoAutomaticoRepertoriazioneNoDirittiPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaPassoAutomaticoRepertoriazioneNoDirittiPi3Exception()
            : base(ErrorDescriptions.ProcessoFirmaPassoAutomaticoRepertoriazioneNoDiritti, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class ProcessoFirmaDocumentoInLibroFirmaPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProcessoFirmaDocumentoInLibroFirmaPi3Exception(long idProfile)
            : base(ErrorDescriptions.ProcessoFirmaDocumentoInLibroFirma, null, ErrorDescriptions.ResourceManager, idProfile)
        { }
        #endregion
    }

    public class ErroreAvvioProcessoFirmaPi3Exception : Pi3Exception
    {
        #region Public Members

        public ErroreAvvioProcessoFirmaPi3Exception()
            : base(ErrorDescriptions.ErroreAvvioProcessoFirma, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }
}
