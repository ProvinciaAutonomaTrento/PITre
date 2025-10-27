// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.AggiornaStatusMask
{
    internal record AggiornaStatusMaskResult(bool retval);

    internal record AggiornaStatusMaskRequest(DocsPaVO.DatiCert.Notifica notifica) : IRequest<AggiornaStatusMaskResult>;

}
