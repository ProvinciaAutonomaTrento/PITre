// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAddParolaChiave
{
    public class KeywordAlreadyExistPi3Exception : Pi3Exception
    {
        public KeywordAlreadyExistPi3Exception()
        : base(ErrorDescription.KeywordAlreadyExist, null, ErrorDescription.ResourceManager)
        {
        }
    }
}
