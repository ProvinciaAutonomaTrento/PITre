// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Users;
public class GetUserDto // : User
{
    public long IdPeople { get; internal set; }
    public string? UserId { get; internal set; }
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
    public bool IsAdmin { get; internal set; }
    public long? IdGruppo { get; internal set; }
    public string? GroupCode { get; internal set; }
    public string? GroupDescription { get; internal set; }
    public long? IdAmministrazione { get; internal set; }
    public string? CodiceAmministrazione { get; internal set; }
    public string? DescrizioneAmministrazione { get; internal set; }


    public IEnumerable<string>? Functions { get; set; }



    public long? IdCorrGlobali { get; internal set; }
}
