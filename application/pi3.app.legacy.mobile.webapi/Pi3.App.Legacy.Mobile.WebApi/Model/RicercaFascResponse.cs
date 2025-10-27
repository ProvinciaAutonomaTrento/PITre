// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class RicercaFascResponse
{
    public List<RicercaFascElement> Risultati
    {
        get;
        set;
    }

    public int TotalRecordCount
    {
        get;
        set;
    }

    public RicercaFascResponseCode Code
    {
        get;
        set;
    }

    public static RicercaFascResponse ErrorResponse
    {
        get
        {
            RicercaFascResponse resp = new RicercaFascResponse();
            resp.Code = RicercaFascResponseCode.SYSTEM_ERROR;
            return resp;
        }
    }
}

public enum RicercaFascResponseCode
{
    OK, SYSTEM_ERROR
}

public class RicercaFascElement
{
    public RicercaElement InfoElement { get; set; }
    public List<DocInfo> Documenti { get; set; }
}