// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.EmailCallback
{
    public class EmailCallbackResponse
    {
        public EmailCallbackResponse(bool output)
        {
            Output = output;
        }

        public bool Output { get; init; }
    }
}
