// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

namespace Pi3.App.Legacy.Mobile.Models.SelectTemplates;

public class Utente
{
    public long IdPeople { get; set; }
    public string? UserId { get; set; }
    public long? IdAmministrazione { get; set; }
    public string? FullName { get; set; }
    public string? Cognome { get; set; }
    public string? Nome { get; set; }
    public string? Disabled { get; set; }
    public string? EncryptedPassword { get; set; }
    public string? UserType { get; set; } // User = 0; SystemAdmin = 1; SuperAdmin = 2; UserAdmin = 3;
    public DateTime? PasswordCreationDate { get; set; }
    public string? PasswordNeverExpire { get; set; }
    public string? Memento { get; set; }
}
