// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;

namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface IFascicoloService
{
    Task<SERVICE_DTO.Fascicolo> GetFascicoloById( long idFascicolo, CancellationToken cancellationToken = default );
}
