// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions
{
    public class ModelloTrasmissioneDocumentoPi3Exception : NotFoundPi3Exception
    {
        private readonly string _idAggregato;
        private readonly string _modello;
        
        #region Public Members

        public ModelloTrasmissioneDocumentoPi3Exception(string idAggregato, string modello)
            : base(ErrorDescriptions.ModelloTrasmissioneDocumento, null, ErrorDescriptions.ResourceManager, modello)
        {
            this._idAggregato = idAggregato;
            this._modello = modello;
        }

        #endregion
    }
}
