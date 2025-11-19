// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTrasmissioneById
{
    public class TrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneNotFoundPi3Exception(string idTrasmissione)
            : base(ErrorDescriptions.TrasmissioneNonTrovata, null, ErrorDescriptions.ResourceManager, idTrasmissione)
        {
            this.IdTrasmissione = idTrasmissione;
        }

        public string IdTrasmissione { get; init; }

        #endregion
    }
}
