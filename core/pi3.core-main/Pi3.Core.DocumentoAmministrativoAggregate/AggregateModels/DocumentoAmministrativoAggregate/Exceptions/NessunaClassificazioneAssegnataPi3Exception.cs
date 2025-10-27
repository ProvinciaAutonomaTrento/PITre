// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Exceptions
{
    public class NessunaClassificazioneAssegnataPi3Exception : Pi3Exception
    {
        #region Public Members

        public NessunaClassificazioneAssegnataPi3Exception()
            : base(ErrorDescriptions.NessunaClassificazioneAssegnata, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

}
