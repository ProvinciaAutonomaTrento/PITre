// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.ConvertVersionToPdf
{

    public class ConvertPi3Exception : Pi3Exception
    {
        #region Public Members

        public ConvertPi3Exception(string message, System.Resources.ResourceManager resourceManager, params string[] messageParameters)
            : base(message, resourceManager, messageParameters)
        {
        }

        public ConvertPi3Exception(string message, Exception? innerException, System.Resources.ResourceManager resourceManager, params string[] messageParameters)
            : base(message, resourceManager, innerException, messageParameters)
        {
        }

        #endregion
    }
}
