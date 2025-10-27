// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Models.SelectTemplates;
public class UserDetails
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? Cognome { get; set; }
    public string? Nome { get; set; }
    public string? UserType { get; set; } //CHA_AMMINISTRATORE : User = 0; SystemAdmin = 1; SuperAdmin = 2; UserAdmin = 3;
    public long AmministrazioneId { get; set; }
    public string? AmministrazioneCodice { get; set; }
    public string? AmministrazioneDescrizione { get; set; }
    public long CorrGlobaliId { get; set; }
    public long GroupSystemId { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
    public long GroupCorrGlobaliId { get; set; }
}
