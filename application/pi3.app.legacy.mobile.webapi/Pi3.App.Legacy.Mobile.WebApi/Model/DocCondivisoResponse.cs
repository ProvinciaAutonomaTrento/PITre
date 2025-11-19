// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class DocCondivisoResponse
{
    public DocCondivisoResponse() { }

    public DocCondivisoResponse(DocCondivisoResponseCode code)
    {
        this.Code = code;
    }

    public DocInfo DocInfo
    {
        get;
        set;
    }

    public List<DocInfo> Allegati
    {
        get;
        set;
    }

    
    public DocCondivisoResponseCode Code
    {
        get;
        set;
    }

    public static DocCondivisoResponse ErrorResponse
    {
        get
        {
            DocCondivisoResponse resp = new DocCondivisoResponse(DocCondivisoResponseCode.SYSTEM_ERROR);
            return resp;
        }
    }

}

public enum DocCondivisoResponseCode
{
    OK, WRONG_USER, EXPIRED, SYSTEM_ERROR
}