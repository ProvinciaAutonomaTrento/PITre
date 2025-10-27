// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class AuthenticateRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? OldPassword { get; set; }
    public string? idAmministrazione { get; set; }
    public string? Otp { get; set; }
    public string? Version { get; set; }
    public string? CodiceFiscale { get; set; }

}