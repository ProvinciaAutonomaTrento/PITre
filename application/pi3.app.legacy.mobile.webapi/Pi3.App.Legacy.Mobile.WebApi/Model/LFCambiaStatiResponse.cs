// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class LFCambiaStatiResponse
{
    public LFCambiaStatiResponseCode Code { get; set; }

    public ElementResult[] Elementi { get; set; }

    public static LFCambiaStatiResponse ErrorResponse
    {
        get
        {
            LFCambiaStatiResponse resp = new LFCambiaStatiResponse();
            resp.Code = LFCambiaStatiResponseCode.SYSTEM_ERROR;
            return resp;
        }
    }
}

public class ElementResult
{
    public DocsPaVO.Mobile.LibroFirmaElement Elemento
    {
        get;
        set;
    }

    public string Esito
    {
        get;
        set;
    }
}

public enum LFCambiaStatiResponseCode
{
    OK, SYSTEM_ERROR
}