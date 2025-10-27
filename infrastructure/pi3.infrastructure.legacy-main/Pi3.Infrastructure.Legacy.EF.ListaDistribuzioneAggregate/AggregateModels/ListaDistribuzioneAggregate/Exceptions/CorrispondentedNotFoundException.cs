// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Exceptions
{
    public class CorrispondentedNotFoundException : NotFoundPi3Exception
    {
        #region Public Members

        public CorrispondentedNotFoundException(string idUser)
            : base(ErrorDescriptions.CorrispondentedNotFound, null, ErrorDescriptions.ResourceManager, idUser)
        {
            IdUser = idUser;
        }

        public string IdUser { get; init; }

        #endregion
    }
}
