// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions
{
    public class MezzoSpedizioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public MezzoSpedizioneNotFoundPi3Exception(string mezzoSpdizione)
            : base(ErrorDescriptions.UploadIdNotFound, null, ErrorDescriptions.ResourceManager, mezzoSpdizione)
        {
            this.MezzoSpedizione = mezzoSpdizione;
        }

        public string   MezzoSpedizione { get; init; }

        #endregion
    }
}
