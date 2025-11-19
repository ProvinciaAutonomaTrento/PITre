// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class SmistaRequest
{
    public string? IdTrasmissione
    {
        get;
        set;
    }

    public string ?IdTrasmissioneUtente
    {
        get;
        set;
    }

    public bool HasWorkflow { get; set; }
    public string? IdEvento { get; set; }

    //public string NoteAccettazione{get;set;}
    

    //public string Action
    //{
    //    get;
    //    set;
    //}

    public string? IdDoc
    {
        get;
        set;
    }

    public string? IdFasc
    {
        get;
        set;
    }

    public string? IdDestinatario
    {
        get;
        set;
    }

    public string? CodiceDestinatario { get; set; }

    public bool Notify { get; set; }

    public string? TipoTrasmissione { get; set; }

    public string? Ragione { get; set; }

    public string? IdModelloTrasm
    {
        get;
        set;
    }

    public string? NoteTrasm { get; set; }
    
    public string? Path
    {
        get;
        set;
    }

}