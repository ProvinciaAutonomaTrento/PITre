// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Create
{

    public class CodiceCorrispondenteAlreadyExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public CodiceCorrispondenteAlreadyExistsPi3Exception(string codice)
            : base(ErrorDescriptions.CodiceCorrispondenteAlreadyExists, ErrorDescriptions.ResourceManager, codice)
        {
            this.Codice = codice;
        }

        public string Codice { get; init; }

        #endregion
    }

}
