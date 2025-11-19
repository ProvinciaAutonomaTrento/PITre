// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails
{
    public class CorrispondenteNonTrovatoPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CorrispondenteNonTrovatoPi3Exception(string idCorrispondente)
            : base(ErrorDescriptions.CorrispondenteNonTrovato, null, ErrorDescriptions.ResourceManager, idCorrispondente)
        {
            this.IdCorrispondente = idCorrispondente;
        }

        public string IdCorrispondente { get; init; }

        #endregion
    }

}
