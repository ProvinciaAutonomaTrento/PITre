// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DelegaAggregate
{
    public class ModificaDelegaAttivaPi3Exception : Pi3Exception
    {
        #region Public Members

        public ModificaDelegaAttivaPi3Exception()
            : base(ErrorDescriptions.ModificaDelegaAttiva, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class DelegaIntervalloDataInvalidPi3Exception : Pi3Exception
    {
        #region Public Members

        public DelegaIntervalloDataInvalidPi3Exception()
            : base(ErrorDescriptions.DelegaIntervalloDataInvalid, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class UtenteDeleganteNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteDeleganteNotValidPi3Exception()
            : base(ErrorDescriptions.UtenteDeleganteNotValid, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class UtenteDelegatoNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteDelegatoNotValidPi3Exception()
            : base(ErrorDescriptions.UtenteDelegatoNotValid, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class GruppoDeleganteNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public GruppoDeleganteNotValidPi3Exception()
            : base(ErrorDescriptions.GruppoDeleganteNotValid, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class GruppoDelegatoNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public GruppoDelegatoNotValidPi3Exception()
            : base(ErrorDescriptions.GruppoDelegatoNotValid, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class DelegaAttivaDataDecorrenzaUpdatePi3Exception : Pi3Exception
    {
        #region Public Members

        public DelegaAttivaDataDecorrenzaUpdatePi3Exception()
            : base(ErrorDescriptions.DelegaAttivaDataDecorrenzaUpdate, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
