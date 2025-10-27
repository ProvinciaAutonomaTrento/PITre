// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFile
{
    public class DocumentoPutFilePi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoPutFilePi3Exception(string error)
            : base(ErrorDescriptions.PutFileError, ErrorDescriptions.ResourceManager, error)
        {
        }

        #endregion
    }

    public class DocumentoPutFileFormatoFileNonAmmessoPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoPutFileFormatoFileNonAmmessoPi3Exception(string ext)
            : base(ErrorDescriptions.FormatoFileNonAmmessoInAmministrazione, ErrorDescriptions.ResourceManager, ext)
        {
        }

        #endregion
    }

}
