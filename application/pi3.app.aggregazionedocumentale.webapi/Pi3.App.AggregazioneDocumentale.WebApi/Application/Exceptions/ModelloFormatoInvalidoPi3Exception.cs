// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class ModelloFormatoInvalidoPi3Exception : Pi3Exception
    {
        private readonly string codModello;
        #region Public Members

        public ModelloFormatoInvalidoPi3Exception( string codModello)
            : base(ErrorDescriptions.FormatoModelloNonValido, null, ErrorDescriptions.ResourceManager, codModello)
        {
            this.codModello = codModello;
        }

        #endregion
    }
}
