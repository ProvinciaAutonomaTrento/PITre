// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.Mobile;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class RicercaUtentiWithRolesResponse
{
    public RicercaUtentiWithRolesResponseCode Code { get; set; }

    public List<UserInfo> Risultati
    {
        get;
        set;
    }

    public static RicercaUtentiWithRolesResponse ErrorResponse
    {
        get
        {
            RicercaUtentiWithRolesResponse resp = new RicercaUtentiWithRolesResponse();
            resp.Code = RicercaUtentiWithRolesResponseCode.SYSTEM_ERROR;
            return resp;
        }
    }
}

public enum RicercaUtentiWithRolesResponseCode
{
    OK,SYSTEM_ERROR
}
