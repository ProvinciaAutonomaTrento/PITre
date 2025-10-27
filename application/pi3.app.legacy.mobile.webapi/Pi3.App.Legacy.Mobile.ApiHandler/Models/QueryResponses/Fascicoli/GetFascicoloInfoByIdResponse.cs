// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Fascicoli;
using Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses.Fascicoli;
public class GetFascicoloInfoByIdResponse
{
    public Fascicolo? FascInfo { get; set; }
}
