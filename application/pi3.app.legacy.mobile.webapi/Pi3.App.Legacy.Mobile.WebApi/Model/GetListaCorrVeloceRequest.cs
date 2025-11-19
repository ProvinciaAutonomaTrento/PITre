// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class GetListaCorrVeloceRequest
{
     public string Descrizione
    {
        get;
        set;
    }

    //public int NumMaxResults
    //{
    //    get;
    //    set;
    //}


    //public int numMaxResultsForCategory
    //{
    //    get;
    //    set;
    //}

    // MEV MOBILE
    public string Ragione
    {
        get;
        set;
    }
}
