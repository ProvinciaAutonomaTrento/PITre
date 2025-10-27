// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults.Deleghe;
public class CreaDelegaResult
{
    public CreaDelegaResponseCode Code { get; set; }


    public enum CreaDelegaResponseCode
    {
        OK, SYSTEM_ERROR, NOT_CREATED, OVERLAPPING_PERIODS
    }
}
