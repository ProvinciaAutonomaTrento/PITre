// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Shared.Exceptions.Internal;
public class LoginException(LoginResponseCode code) : Exception($"Errore nella procedura di login, codice: { code }")
{
    public LoginResponseCode Code { get; private set; } = code;
}

public enum LoginResponseCode
{
    OK, USER_NOT_FOUND, PASSWORD_EXPIRED, MULTIAMM, SYSTEM_ERROR, OTP_OK, OTP_USER_NOT_FOUND, OTP_EMAIL_ERROR, OTP_SEND_EMAIL_ERROR, INVALID_OTP, PASSWORD_EQUALITY, DOMAIN_AUTH_ENABLED
}
