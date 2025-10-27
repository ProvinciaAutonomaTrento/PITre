// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class LibroFirmaRequest
{
    public int RequestedPage
    {
        get;
        set;
    }

    public int PageSize
    {
        get;
        set;
    }

    public string? Testo
    {
        get;
        set;
    }

    public string? TipoRicerca
    { get; set; }

    //public DocsPaVO.Mobile.RicercaType TipoRicerca
    //{
    //    get;
    //    set;
    //}

    public string? Oggetto { get; set; }
    public string? DataDa { get; set; }
    public string? DataA { get; set; }
    public string? DataProtoDa { get; set; }
    public string? DataProtoA { get; set; }
    public string? Note { get; set; }
    public string? Proponente { get; set; }
    public string? IdDocumento { get; set; }
    public string? NumProto{ get; set; }
    public string? NumAnnoProto { get; set; }
}