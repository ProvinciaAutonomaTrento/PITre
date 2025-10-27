// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class VerifyUpdateRequest
{
    public string Model { get; set; }
    public string Brand { get; set; }
    public string Version { get; set; }
   
   
}