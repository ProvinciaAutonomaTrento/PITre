// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Users;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Authentication;
public class AuthenticateUserDTO
{
    public UserDetails? UserInfo { get; set; }
    public bool OTPAllowed { get; set; }
    public bool ShareAllowed { get; set; }
    public Memento? InfoMemento { get; set; }

    public string ErrorMessageLogin {
        get
        {
            return this.Code switch
            {
                Shared.Exceptions.Internal.LoginResponseCode.OK => "",
                Shared.Exceptions.Internal.LoginResponseCode.PASSWORD_EXPIRED => "Password scaduta",
                Shared.Exceptions.Internal.LoginResponseCode.USER_NOT_FOUND => "Utente o password errati",
                _ => "Application Error",
            };
        }
    }
    public Mobile.Shared.Exceptions.Internal.LoginResponseCode Code { get; set; }
}
