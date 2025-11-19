// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class AccettaRifiutaTrasmRequest
{
    public string? IdTrasmissione
    {
        get;
        set;
    }

    public string? IdTrasmissioneUtente
    {
        get;
        set;
    }

    public string? Note
    {
        get;
        set;
    }
    
    public string? Action
    {
        get;
        set;
    }

}

public enum AccettaRifiutaAction
{
    ACCETTA, RIFIUTA
}