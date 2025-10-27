// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pi3.App.Legacy.Mobile.WebApi.Model;

public class ListaRuoliByUserResponse
{
    public ListaRuoliByUserResponse(){}

    public ListaRuoliByUserResponse(ListaRuoliByUserResponseCode code)
    {
        this.Code = code;
        
    }

    public DocsPaVO.Mobile.RuoloInfo[] ListaRuoli { get; set; }

    public static ListaRuoliByUserResponse ErrorResponse
    {
        get
        {
            ListaRuoliByUserResponse res = new ListaRuoliByUserResponse(ListaRuoliByUserResponseCode.SYSTEM_ERROR);
            return res;
        }
    }

    public ListaRuoliByUserResponseCode Code {get;set;}


}

public enum ListaRuoliByUserResponseCode
{
    OK, USER_NO_ROLES, SYSTEM_ERROR
}