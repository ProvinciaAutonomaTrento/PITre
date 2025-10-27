// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportDoc
{
    public class MultipleCorrsFoundPi3Exception : Pi3Exception
    {
        #region Public Members
        public MultipleCorrsFoundPi3Exception(string? message)
        : base(message)
        {
            _message = message;
            
        }

        #endregion
    }
    public class NoProjectFoundPi3Exception : Pi3Exception
    {
        #region Public Members
        public NoProjectFoundPi3Exception(string? message)
        : base(message)
        {
            _message = message;
            
        }

        #endregion
    }
    public class NoRfFoundPi3Exception : Pi3Exception
    {
        #region Public Members
        public NoRfFoundPi3Exception(string? message)
        : base(message)
        {
            _message = message;

        }

        #endregion
    }


    public class NoAdmFoundPi3Exception : Pi3Exception
    {
        #region Public Members
        public NoAdmFoundPi3Exception(string? message)
        : base(message)
        {
            _message = message;

        }

        #endregion
    }
    public class NoCorrsFoundFoundPi3Exception : Pi3Exception
    {
        #region Public Members
        public NoCorrsFoundFoundPi3Exception(string? message)
        : base(message)
        {
            _message = message;
        }

        #endregion
    }
}
