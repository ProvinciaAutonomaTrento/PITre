// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Deleghe;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses.Deleghe;
public class GetModelliDelegheResponse
{
    public IEnumerable<ModelloDelega>? Modelli { get; set; }
    public ListaModelliDelegaResponseCode Code { get; set; } = ListaModelliDelegaResponseCode.OK;




    public enum ListaModelliDelegaResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
