// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione.Exceptions
{
    public class SenderNotInteroperablePi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public SenderNotInteroperablePi3Exception()
            : base(ErrorDescriptions.SenderNotInteroperable, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
