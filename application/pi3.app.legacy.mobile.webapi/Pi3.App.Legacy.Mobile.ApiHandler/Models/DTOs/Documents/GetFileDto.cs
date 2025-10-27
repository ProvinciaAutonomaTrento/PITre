// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Documents;
public class GetFileDto
{
    public Models.File? File { get; set; }
    public int Code { get; set; } // OK = 0,SYSTEM_ERROR = 1
}
