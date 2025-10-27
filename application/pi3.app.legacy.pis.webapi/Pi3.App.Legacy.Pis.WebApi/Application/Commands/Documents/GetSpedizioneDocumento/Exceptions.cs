// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSpedizioneDocumento
{
    public class GetSpedizioneDocumentoPi3Exception : Pi3Exception
    {
        #region Public Members

        public GetSpedizioneDocumentoPi3Exception(string message)
            : base(message)
        {
        }

        #endregion
    }
}
