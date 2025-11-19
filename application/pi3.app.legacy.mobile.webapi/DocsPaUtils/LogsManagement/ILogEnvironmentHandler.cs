// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
#if false // gestione log4net
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaUtils.LogsManagement
{
    public interface ILogEnvironmentHandler
    {
        string FileName
        {
            get;
        }

        string FilePath
        {
            get;
        }

        bool Enabled
        {
            get;
        }
    }
}

#endif