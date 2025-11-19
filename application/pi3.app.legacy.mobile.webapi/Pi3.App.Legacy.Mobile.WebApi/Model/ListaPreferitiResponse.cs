// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class ListaPreferitiResponse
{
    public ListaPreferitiResponseCode Code { get; set; }

    public List<InfoPreferito> Preferiti
    {
        get;
        set;
    }

    public static ListaPreferitiResponse ErrorResponse
    {
        get
        {
            ListaPreferitiResponse resp = new ListaPreferitiResponse();
            resp.Code = ListaPreferitiResponseCode.SYSTEM_ERROR;
            return resp;
        }
    }
}

public enum ListaPreferitiResponseCode
{
    OK, SYSTEM_ERROR
}