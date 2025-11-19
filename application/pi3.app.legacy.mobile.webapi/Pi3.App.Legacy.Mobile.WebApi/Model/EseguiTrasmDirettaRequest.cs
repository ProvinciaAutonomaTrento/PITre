// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class EseguiTrasmDirettaRequest
{
    public string IdDoc
    {
        get;
        set;
    }

    public string IdFasc
    {
        get;
        set;
    }

    public string IdDestinatario
    {
        get;
        set;
    }

    public string CodiceDestinatario { get; set; }

    public bool Notify { get; set; }

    public string TipoTrasmissione { get; set; }

    public string Ragione { get; set; }

    public string Note
    {
        get;
        set;
    }

    public string Path
    {
        get;
        set;
    }
}